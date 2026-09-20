using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CMkeyCombat
{
    sealed class CustomOverridesForm : Form
    {
        sealed class ReplacementChoice
        {
            public readonly string Value;
            public readonly string Label;
            public readonly bool IsDefault;
            public ReplacementChoice(string value, string label, bool isDefault)
            { Value = value; Label = label; IsDefault = isDefault; }
            public override string ToString() { return Label; }
        }

        sealed class ChoiceState
        {
            public int CheckedIndex = -1;
        }

        readonly ComboBox _sourceBox;
        readonly ComboBox _replacementBox;
        readonly Dictionary<char, string> _workingOverrides;
        readonly List<char> _sources;
        bool _loading;
        bool _replacementEdited;
        char _currentSource;

        public CustomOverridesForm()
        {
            Text = "CMkey — Cá nhân hóa ký tự";
            var windowIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? (Icon)SystemIcons.Application.Clone();
            Icon = windowIcon;
            Disposed += (s, e) => windowIcon.Dispose();
            StartPosition = FormStartPosition.CenterScreen;
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(560, 190);
            MinimumSize = new Size(500, 220);
            ShowInTaskbar = false;
            _workingOverrides = new Dictionary<char, string>(CombatMap.CustomOverrides);
            _sources = CharacterCatalog.Entries.Select(x => x.Source).ToList();
            foreach (char source in _workingOverrides.Keys.OrderBy(x => x))
                if (!_sources.Contains(source)) _sources.Add(source);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3,
                Padding = new Padding(20, 20, 20, 16), AutoSize = false
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(layout);

            layout.Controls.Add(new Label { Text = "Chọn ký tự gốc:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 4, 14, 12) }, 0, 0);
            _sourceBox = CreateChoiceBox();
            _sourceBox.DropDownStyle = ComboBoxStyle.DropDown;
            _sourceBox.DrawMode = DrawMode.OwnerDrawVariable;
            _sourceBox.MeasureItem += MeasureSourceChoice;
            _sourceBox.MaxLength = 1;
            _sourceBox.TextUpdate += (s, e) =>
            {
                char source;
                if (!Char.TryParse(_sourceBox.Text, out source) || Char.IsSurrogate(source)) return;
                int index = _sources.IndexOf(source);
                if (index < 0)
                {
                    _sources.Add(source);
                    index = _sourceBox.Items.Add(source.ToString());
                }
                _sourceBox.SelectedIndex = index;
            };
            _sourceBox.Width = 350;
            foreach (char source in _sources) _sourceBox.Items.Add(source.ToString());
            _sourceBox.SelectedIndexChanged += (s, e) => { if (!_sourceBox.DroppedDown) LoadSource(); };
            _sourceBox.SelectionChangeCommitted += (s, e) => LoadSource();
            layout.Controls.Add(_sourceBox, 1, 0);

            layout.Controls.Add(new Label { Text = "Chọn ký tự biến đổi:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 4, 14, 12) }, 0, 1);
            _replacementBox = CreateChoiceBox();
            _replacementBox.DropDownStyle = ComboBoxStyle.DropDown;
            _replacementBox.Width = 350;
            _replacementBox.SelectedIndexChanged += (s, e) => { if (!_replacementBox.DroppedDown) ApplySelectedReplacement(); };
            _replacementBox.SelectionChangeCommitted += (s, e) => ApplySelectedReplacement();
            _replacementBox.TextUpdate += (s, e) => { if (!_loading) _replacementEdited = true; };
            _replacementBox.Leave += (s, e) => ApplyTypedReplacement();
            layout.Controls.Add(_replacementBox, 1, 1);

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, AutoSize = true, Margin = new Padding(0) };
            var cancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel, AutoSize = true, Margin = new Padding(8, 0, 0, 0) };
            var save = new Button { Text = "Lưu", AutoSize = true };
            save.Click += (s, e) => SaveAndClose();
            buttons.Controls.Add(cancel); buttons.Controls.Add(save);
            layout.SetColumnSpan(buttons, 2);
            layout.Controls.Add(buttons, 0, 2);
            AcceptButton = save; CancelButton = cancel;

            _sourceBox.SelectedIndex = 0;
        }

        ComboBox CreateChoiceBox()
        {
            var state = new ChoiceState();
            var box = new ComboBox
            {
                DrawMode = DrawMode.OwnerDrawFixed, IntegralHeight = true,
                DropDownHeight = 260, FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                ItemHeight = Math.Max(26, Font.Height + 10),
                BackColor = SystemColors.Window, ForeColor = SystemColors.WindowText,
                Tag = state
            };
            box.DropDown += (s, e) => state.CheckedIndex = box.SelectedIndex;
            box.SelectedIndexChanged += (s, e) =>
            {
                if (!box.DroppedDown) state.CheckedIndex = box.SelectedIndex;
            };
            box.SelectionChangeCommitted += (s, e) => state.CheckedIndex = box.SelectedIndex;
            box.TextUpdate += (s, e) => state.CheckedIndex = -1;
            box.DrawItem += DrawChoice;
            return box;
        }

        int SourceGroupGap(int index)
        {
            if (index <= 0 || index >= _sources.Count) return 0;
            return Char.ToUpperInvariant(_sources[index - 1]) == Char.ToUpperInvariant(_sources[index])
                ? 0 : Math.Max(12, Font.Height * 2 / 3);
        }

        void MeasureSourceChoice(object sender, MeasureItemEventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            e.ItemHeight = Math.Max(26, box.Font.Height + 10) + SourceGroupGap(e.Index);
            e.ItemWidth = box.Width;
        }

        void DrawChoice(object sender, DrawItemEventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            // Native hover state can differ from SelectedIndex while the list repaints.
            // Clear the entire old row, then highlight only the row Windows marks active.
            using (var brush = new SolidBrush(SystemColors.Window))
                e.Graphics.FillRectangle(brush, e.Bounds);
            if (e.Index < 0 || e.Index >= box.Items.Count) return;

            Rectangle bounds = e.Bounds;
            if (box == _sourceBox && (e.State & DrawItemState.ComboBoxEdit) == 0)
            {
                int gap = SourceGroupGap(e.Index);
                if (gap > 0)
                {
                    Rectangle separator = new Rectangle(bounds.X, bounds.Y, bounds.Width, gap);
                    using (var brush = new SolidBrush(Color.FromArgb(240, 235, 250)))
                        e.Graphics.FillRectangle(brush, separator);
                    using (var pen = new Pen(Color.FromArgb(185, 166, 221)))
                        e.Graphics.DrawLine(pen, bounds.Left + 6, bounds.Top + gap / 2,
                            bounds.Right - 6, bounds.Top + gap / 2);
                }
                bounds.Y += gap;
                bounds.Height -= gap;
            }
            bool highlighted = (e.State & DrawItemState.Selected) != 0;
            bool isChecked = e.Index == ((ChoiceState)box.Tag).CheckedIndex;
            Color back = highlighted ? Color.FromArgb(112, 82, 190) : SystemColors.Window;
            Color fore = highlighted ? Color.White : SystemColors.WindowText;
            using (var brush = new SolidBrush(back)) e.Graphics.FillRectangle(brush, bounds);

            int inset = Math.Max(4, box.Font.Height / 4);
            int checkWidth = box.Font.Height + inset;
            Rectangle checkBounds = new Rectangle(bounds.X + inset, bounds.Y, checkWidth, bounds.Height);
            Rectangle textBounds = new Rectangle(checkBounds.Right + inset, bounds.Y,
                Math.Max(1, bounds.Right - checkBounds.Right - inset * 2), bounds.Height);
            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding;
            if (isChecked) TextRenderer.DrawText(e.Graphics, "✓", box.Font, checkBounds, fore, flags);
            TextRenderer.DrawText(e.Graphics, Convert.ToString(box.Items[e.Index]), box.Font, textBounds, fore,
                flags | TextFormatFlags.EndEllipsis);
            if ((e.State & DrawItemState.Focus) != 0)
                ControlPaint.DrawFocusRectangle(e.Graphics, bounds, fore, back);
        }

        void LoadSource()
        {
            if (_loading || _sourceBox.SelectedIndex < 0) return;
            ApplyTypedReplacement();
            _currentSource = _sources[_sourceBox.SelectedIndex];
            _loading = true;
            _replacementBox.Items.Clear();
            CharacterCatalogEntry entry = CharacterCatalog.Entries.FirstOrDefault(x => x.Source == _currentSource);
            string defaultValue = entry == null ? CombatMap.TransformDefault(_currentSource) ?? _currentSource.ToString() : entry.DefaultReplacement;
            _replacementBox.Items.Add(new ReplacementChoice(defaultValue, "Mặc định CMkey: " + defaultValue, true));
            if (entry != null)
            {
                foreach (string value in ParseVariants(entry.Variants))
                    _replacementBox.Items.Add(new ReplacementChoice(value, value, false));
            }
            string custom;
            if (_workingOverrides.TryGetValue(_currentSource, out custom) && !_replacementBox.Items.Cast<ReplacementChoice>().Any(x => !x.IsDefault && x.Value == custom))
                _replacementBox.Items.Add(new ReplacementChoice(custom, "Tùy chỉnh: " + custom, false));
            int selected = 0;
            if (_workingOverrides.TryGetValue(_currentSource, out custom))
                selected = FindChoice(custom);
            _replacementBox.SelectedIndex = selected;
            _replacementEdited = false;
            _loading = false;
            _sourceBox.BackColor = _replacementBox.BackColor = Color.FromArgb(112, 82, 190);
            _sourceBox.ForeColor = _replacementBox.ForeColor = Color.White;
        }

        int FindChoice(string value)
        {
            for (int i = 1; i < _replacementBox.Items.Count; i++)
                if (((ReplacementChoice)_replacementBox.Items[i]).Value == value) return i;
            return 0;
        }

        void ApplySelectedReplacement()
        {
            if (_loading || _replacementBox.SelectedIndex < 0) return;
            var choice = (ReplacementChoice)_replacementBox.SelectedItem;
            _replacementEdited = false;
            if (choice.IsDefault) _workingOverrides.Remove(_currentSource);
            else _workingOverrides[_currentSource] = choice.Value;
        }

        void ApplyTypedReplacement()
        {
            if (_loading || !_replacementEdited) return;
            string value = _replacementBox.Text;
            if (String.IsNullOrEmpty(value)) return;
            _workingOverrides[_currentSource] = value;
            _replacementEdited = false;
        }

        static IEnumerable<string> ParseVariants(string variants)
        {
            foreach (string raw in variants.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string token = raw.Trim();
                int separator = token.IndexOf(" — ", StringComparison.Ordinal);
                if (separator >= 0) token = token.Substring(0, separator).Trim();
                if (!String.IsNullOrEmpty(token)) yield return token;
            }
        }

        void SaveAndClose()
        {
            char source;
            if (!Char.TryParse(_sourceBox.Text, out source) || Char.IsSurrogate(source))
            {
                MessageBox.Show(this, "Hãy chọn một ký tự gốc hợp lệ.", "CMkey", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _sourceBox.Focus();
                return;
            }
            if (String.IsNullOrEmpty(_replacementBox.Text))
            {
                MessageBox.Show(this, "Hãy chọn hoặc nhập ký tự biến đổi.", "CMkey", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _replacementBox.Focus();
                return;
            }
            ApplyTypedReplacement();
            try
            {
                CustomOverridesStore.Save(_workingOverrides);
                CombatMap.ReplaceCustomOverrides(_workingOverrides);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Không thể lưu thiết lập: " + ex.Message, "CMkey", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
