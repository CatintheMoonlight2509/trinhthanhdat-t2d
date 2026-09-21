using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace CMkeyCombat
{
    sealed class AboutForm : Form
    {
        public AboutForm()
        {
            Text = "CMkey — Giới thiệu";
            StartPosition = FormStartPosition.CenterScreen;
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 10F);
            int extraWidth = TextRenderer.MeasureText("aa", Font, Size.Empty,
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width;
            ClientSize = new Size(880 + extraWidth, 510);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            var windowIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? (Icon)SystemIcons.Application.Clone();
            Icon = windowIcon;
            Disposed += (s, e) => windowIcon.Dispose();
            ShowInTaskbar = false;
            BackColor = SystemColors.Window;

            var footer = new Panel { Dock = DockStyle.Bottom, Height = 54, Padding = new Padding(16, 8, 16, 8) };
            var close = new Button { Text = "Đóng", DialogResult = DialogResult.OK, Dock = DockStyle.Right, Width = 88 };
            footer.Controls.Add(close);
            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            Controls.Add(scroll);
            Controls.Add(footer);

            int logoSize = Font.Height * 9;
            int logoColumnWidth = logoSize + 16;
            var layout = new TableLayoutPanel
            {
                AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2, RowCount = 8, Padding = new Padding(20, 16, 20, 4),
                Margin = new Padding(0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, logoColumnWidth));
            for (int row = 0; row < 8; row++) layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            scroll.Controls.Add(layout);

            var logo = new PictureBox
            {
                Size = new Size(logoSize, logoSize),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new Padding(16, 0, 0, 0), TabStop = false
            };
            using (var stream = typeof(AboutForm).Assembly.GetManifestResourceStream("CMkey.Logo.png"))
            {
                if (stream != null)
                {
                    using (var image = Image.FromStream(stream))
                        logo.Image = new Bitmap(image);
                }
                else logo.Image = windowIcon.ToBitmap();
            }
            logo.Disposed += (s, e) => { if (logo.Image != null) logo.Image.Dispose(); };
            layout.Controls.Add(logo, 1, 0);
            layout.SetRowSpan(logo, 4);

            layout.Controls.Add(new Label
            {
                Text = "CMkey", Font = new Font(Font.FontFamily, 18F, FontStyle.Bold),
                AutoSize = true, Margin = new Padding(0, 0, 0, 6)
            }, 0, 0);
            var intro = new Label
            {
                Text = "CMkey cho Windows là công cụ biến đổi ký tự từ bàn phím,\r\nhoạt động qua biểu tượng khay hệ thống.",
                AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 10)
            };
            layout.Controls.Add(intro, 0, 1);
            var links = new FlowLayoutPanel
            {
                AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill, WrapContents = true, Margin = new Padding(0, 0, 0, 8)
            };
            links.Controls.Add(CreateLink("Điều khoản dịch vụ", "https://catinthemoonlight2509.github.io/cmkey-policy/terms.html"));
            links.Controls.Add(CreateLink("Chính sách bảo mật", "https://catinthemoonlight2509.github.io/cmkey-policy/privacy.html"));
            layout.Controls.Add(links, 0, 2);
            var contact = new Label
            {
                Text = "Liên hệ: cmkey.t2d@gmail.com", AutoSize = true,
                Margin = new Padding(0, 0, 0, 4)
            };
            var androidIntro = new Label
            {
                Text = "CMkey cũng có 1 bản có sẵn cho Android đó nhé bạn hiền!",
                AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 16),
                ForeColor = Color.FromArgb(112, 82, 190)
            };
            layout.Controls.Add(androidIntro, 0, 3);
            var disclaimer = new Label
            {
                AutoSize = true, Dock = DockStyle.Fill, Padding = new Padding(14),
                Margin = new Padding(0, 0, 0, 12),
                Text = "TUYÊN BỐ TRÁCH NHIỆM\r\n\r\nỨng dụng được tạo ra vì mục đích giải trí. T2D Team không chịu trách nhiệm cho bất kỳ hành vi nào lợi dụng sản phẩm với mục đích lừa đảo hoặc vi phạm pháp luật.",
                BackColor = Color.MistyRose, ForeColor = Color.Maroon, Font = new Font(Font, FontStyle.Bold)
            };
            layout.Controls.Add(disclaimer, 0, 4);
            layout.SetColumnSpan(disclaimer, 2);
            var donation = new Label
            {
                Text = "ỦNG HỘ PHÁT TRIỂN CMKEY\r\n\r\nBạn thấy CMkey thế nào? Mời T2D Team một ly cà phê để tụi mình tiếp tục chăm chút CMkey nhé! ☕\r\nDonate MoMo: 0368161768",
                AutoSize = true, Dock = DockStyle.Fill, Padding = new Padding(14),
                Margin = new Padding(0, 0, 0, 12), BackColor = Color.Lavender,
                ForeColor = Color.FromArgb(65, 43, 112)
            };
            layout.Controls.Add(donation, 0, 5);
            layout.SetColumnSpan(donation, 2);
            var copyright = new Label
            {
                Text = "© 2026 T2D Team. Bảo lưu mọi quyền.", AutoSize = true,
                Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 4)
            };
            layout.Controls.Add(copyright, 0, 6);
            layout.SetColumnSpan(copyright, 2);
            layout.Controls.Add(contact, 0, 7);
            layout.SetColumnSpan(contact, 2);

            // Bound long labels before measuring their height, including at larger DPI.
            Action resizeContent = () =>
            {
                int width = Math.Max(1, scroll.ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
                layout.SuspendLayout();
                layout.MaximumSize = new Size(width, 0);
                layout.MinimumSize = new Size(width, 0);
                layout.Width = width;
                int textWidth = Math.Max(1, width - layout.Padding.Horizontal
                    - (int)layout.ColumnStyles[1].Width);
                links.MaximumSize = new Size(textWidth, 0);
                foreach (Label label in new[] { intro, androidIntro })
                    label.MaximumSize = new Size(Math.Max(1, textWidth - label.Margin.Horizontal), 0);
                contact.MaximumSize = new Size(Math.Max(1, width - layout.Padding.Horizontal), 0);
                foreach (Label label in new[] { disclaimer, donation, copyright })
                    label.MaximumSize = new Size(Math.Max(1, width - layout.Padding.Horizontal), 0);
                layout.ResumeLayout(true);
            };
            scroll.SizeChanged += (s, e) => resizeContent();
            Shown += (s, e) => resizeContent();
            resizeContent();
            AcceptButton = close;
            CancelButton = close;
        }

        LinkLabel CreateLink(string text, string url)
        {
            var link = new LinkLabel
            {
                Text = text, AutoSize = true, Margin = new Padding(0, 0, 20, 6),
                LinkColor = Color.FromArgb(112, 82, 190)
            };
            link.LinkClicked += (s, e) =>
            {
                try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); }
                catch (Exception)
                {
                    MessageBox.Show(this, "Không thể mở trình duyệt. Bạn có thể mở liên kết này thủ công:\r\n" + url,
                        "CMkey", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            return link;
        }
    }
}
