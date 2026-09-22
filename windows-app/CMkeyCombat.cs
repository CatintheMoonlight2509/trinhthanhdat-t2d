// CMkeyCombat.cs — CMkey portable keyboard helper for Windows.
// Global keyboard hook with three explicit modes:
//   Tắt       — pass all input through unchanged.
//   Tiếng Anh — apply the current Android GhostEngine character map only.
//   Tiếng Việt — compose the existing Telex buffer, then apply GhostEngine.
//
// Biên dịch (từ thư mục này):
// csc /nologo /target:winexe /codepage:65001 /optimize+ /out:CMkey.exe /win32icon:CMkey.ico /win32manifest:CMkeyCombat.manifest /resource:CMkey-logo.png,CMkey.Logo.png /reference:System.dll /reference:System.Core.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll /reference:System.Xml.dll CMkeyCombat.cs TelexEngine.cs CharacterCatalog.cs CustomOverrides.cs CustomOverridesForm.cs AboutForm.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CMkeyCombat
{
    static class CombatMap
    {
        // Exact data and lookup order from the current Android GhostEngine.kt.
        // combatMap is checked before digitMap, so 2/3/4 use these values.
        public static readonly Dictionary<char, string> Combat = new Dictionary<char, string>
        {
            {'2', "ᒿ"}, {'3', "ვ"}, {'4', "Ꮞ"},
            {'a', "ɑ"}, {'e', "ϵ"}, {'i', "ɩ"}, {'o', "σ"}, {'u', "υ"}, {'y', "γ"},
            {'b', "ხ"}, {'c', "с"}, {'d', "ᑯ"}, {'đ', "ᴆ"}, {'f', "ғ"}, {'g', "ԍ"},
            {'h', "ҥ"}, {'j', "ј"}, {'k', "κ"}, {'l', "ℓ"}, {'m', "ʍ"}, {'n', "ͷ"},
            {'p', "ρ"}, {'q', "ԛ"}, {'r', "ʀ"}, {'s', "ຣ"}, {'t', "τ"}, {'v', "ѵ"},
            {'w', "ω"}, {'x', "х"}, {'z', "ⴭ"}
        };

        // Explicit uppercase entries take precedence over the lowercase map.
        public static readonly Dictionary<char, string> Upper = new Dictionary<char, string>
        {
            {'A', "ᗅ"}, {'B', "β"}, {'C', "С"}, {'D', "ᗪ"}, {'Đ', "Ð"}, {'E', "€"},
            {'F', "Ғ"}, {'G', "Ԍ"}, {'H', "Ң"}, {'I', "l"}, {'J', "Ꭻ"}, {'K', "₭"},
            {'L', "Լ"}, {'M', "Ϻ"}, {'N', "Ν"}, {'O', "ϴ"}, {'P', "ᑭ"}, {'Q', "Ԛ"},
            {'R', "Ʀ"}, {'S', "ჽ"}, {'T', "Ͳ"}, {'U', "Ս"}, {'Ư', "Մ"}, {'V', "Ѵ"},
            {'W', "Ԝ"}, {'X', "Х"}, {'Y', "ϒ"}, {'Z', "Ζ"}
        };

        public static readonly Dictionary<char, string> Digit = new Dictionary<char, string>
        {
            {'0', "θ"}, {'1', "1"}, {'2', "２"}, {'3', "ვ"}, {'4', "Ꮞ"},
            {'5', "Ƽ"}, {'6', "б"}, {'7', "⁊"}, {'8', "Ȣ"}, {'9', "୨"}
        };

        public static readonly Dictionary<char, string> BuiltInCustomOverrides = new Dictionary<char, string>();
        public static readonly Dictionary<char, string> CustomOverrides = new Dictionary<char, string>();

        // Default 2 keeps the built-in uppercase map and uses Latin small caps for lowercase ASCII.
        // U+1A3E TAI THAM LETTER MA is a temporary stand-in for a small-capital Q.
        public static Dictionary<char, string> CreateDefault2Overrides()
        {
            return new Dictionary<char, string>
            {
                {'a', "ᴀ"}, {'b', "ʙ"}, {'c', "ᴄ"}, {'d', "ᴅ"}, {'e', "ᴇ"},
                {'f', "ꜰ"}, {'g', "ɢ"}, {'h', "ʜ"}, {'i', "ɪ"}, {'j', "ᴊ"},
                {'k', "ᴋ"}, {'l', "ʟ"}, {'m', "ᴍ"}, {'n', "ɴ"}, {'o', "ᴏ"},
                {'p', "ᴘ"}, {'q', "ᨾ"}, {'r', "ʀ"}, {'s', "ꜱ"}, {'t', "ᴛ"},
                {'u', "ᴜ"}, {'v', "ᴠ"}, {'w', "ᴡ"}, {'x', "x"}, {'y', "ʏ"},
                {'z', "ᴢ"}
            };
        }

        public static void LoadCustomOverrides()
        {
            CustomOverrides.Clear();
            foreach (var entry in CustomOverridesStore.Load())
                CustomOverrides[entry.Key] = entry.Value;
        }

        public static void SaveCustomOverrides()
        {
            CustomOverridesStore.Save(CustomOverrides);
        }

        public static void ReplaceCustomOverrides(IDictionary<char, string> replacements)
        {
            CustomOverrides.Clear();
            foreach (var entry in replacements)
                CustomOverrides[entry.Key] = entry.Value;
        }

        public static void SetCustomOverride(char source, string replacement)
        {
            if (String.IsNullOrEmpty(replacement)) CustomOverrides.Remove(source);
            else CustomOverrides[source] = replacement;
        }

        public static string Transform(char ch)
        {
            string replacement;

            if (CustomOverrides.TryGetValue(ch, out replacement))
                return replacement;

            return TransformDefault(ch);
        }

        public static string TransformDefault(char ch)
        {
            string replacement;
            if (char.IsUpper(ch) && Upper.TryGetValue(ch, out replacement))
                return replacement;

            char lower = char.ToLowerInvariant(ch);
            if (Combat.TryGetValue(lower, out replacement)
                    || (char.IsDigit(lower) && Digit.TryGetValue(lower, out replacement)))
            {
                string result = char.IsUpper(ch) ? replacement.ToUpperInvariant() : replacement;
                if (result == ch.ToString()) return null;
                return result;
            }

            return null;
        }

        public static string ApplyGhostFilter(string input)
        {
            if (String.IsNullOrEmpty(input)) return input ?? "";

            var result = new StringBuilder(input.Length);
            foreach (char ch in input)
            {
                string replacement = Transform(ch);
                result.Append(replacement ?? ch.ToString());
            }
            return result.ToString();
        }
    }

    static class Program
    {
        // ── Win32 hook ───────────────────────────────────────────────
        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x0100;
        const int WM_SYSKEYDOWN = 0x0104;

        const uint VK_BACK = 0x08;
        const uint VK_TAB = 0x09;
        const uint VK_RETURN = 0x0D;
        const uint VK_SHIFT = 0x10, VK_CONTROL = 0x11, VK_MENU = 0x12, VK_CAPITAL = 0x14;
        const uint VK_ESCAPE = 0x1B;
        const uint VK_SPACE = 0x20;
        const uint VK_PRIOR = 0x21, VK_NEXT = 0x22, VK_END = 0x23, VK_HOME = 0x24;
        const uint VK_LEFT = 0x25, VK_UP = 0x26, VK_RIGHT = 0x27, VK_DOWN = 0x28;
        const uint VK_INSERT = 0x2D, VK_DELETE = 0x2E;
        const uint VK_LSHIFT = 0xA0, VK_RSHIFT = 0xA1;
        const uint VK_LCONTROL = 0xA2, VK_RCONTROL = 0xA3;
        const uint VK_LMENU = 0xA4, VK_RMENU = 0xA5;
        const uint VK_LWIN = 0x5B, VK_RWIN = 0x5C;
        const uint VK_F1 = 0x70, VK_F24 = 0x87;

        [DllImport("user32.dll")]
        static extern short GetKeyState(int nVirtKey);

        static bool CtrlAltWinDown()
        {
            return (GetKeyState((int)VK_CONTROL) & 0x8000) != 0
                || (GetKeyState((int)VK_MENU) & 0x8000) != 0
                || (GetKeyState((int)VK_LWIN) & 0x8000) != 0
                || (GetKeyState((int)VK_RWIN) & 0x8000) != 0;
        }

        static bool IsModifierVk(uint vk)
        {
            return vk == VK_SHIFT || vk == VK_LSHIFT || vk == VK_RSHIFT
                || vk == VK_CONTROL || vk == VK_LCONTROL || vk == VK_RCONTROL
                || vk == VK_MENU || vk == VK_LMENU || vk == VK_RMENU
                || vk == VK_CAPITAL || vk == VK_LWIN || vk == VK_RWIN;
        }

        static bool IsFunctionalOrNavigationVk(uint vk)
        {
            return vk == VK_TAB || vk == VK_RETURN || vk == VK_ESCAPE || vk == VK_SPACE
                || (vk >= VK_PRIOR && vk <= VK_DOWN)
                || vk == VK_INSERT || vk == VK_DELETE
                || (vk >= VK_F1 && vk <= VK_F24);
        }

        delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
            StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);
        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);
        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        // ── SendInput để gõ ký tự Unicode ────────────────────────────
        const uint INPUT_KEYBOARD = 1;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_KEYUP = 0x0002;

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT { public uint type; public KEYBDINPUT ki; }
        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk; public ushort wScan; public uint dwFlags;
            public uint time; public IntPtr dwExtraInfo;
            public int pad1; public int pad2;
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        // Events generated by this process are ignored by the low-level hook.
        static readonly IntPtr SELF_TAG = new IntPtr(0x434D4B59); // "CMKY"

        enum Mode { Off, Vietnamese, English }

        const string SINGLE_INSTANCE_NAME = "Local\\CMkeyPortableKeyboard";
        static Mutex _singleInstanceMutex;
        static bool _ownsSingleInstanceMutex;
        static bool _cleanedUp;

        static IntPtr _hook = IntPtr.Zero;
        static HookProc _proc;
        static Mode _mode = Mode.Off;
        static NotifyIcon _tray;
        static Icon _trayIcon;
        static Icon _vietnameseTrayIcon;
        static ToolStripMenuItem _vietnameseItem, _englishItem, _customizeItem, _offItem, _closeItem;

        // rawBuffer contains the keys in the current word. lastEmitted is always
        // the exact Ghost-filtered text currently visible in the target application.
        static readonly StringBuilder _rawBuffer = new StringBuilder();
        static string _lastEmitted = "";

        [STAThread]
        static void Main(string[] args)
        {
            Mode startupMode = ParseStartupMode(args);

            if (!AcquireSingleInstance()) return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _trayIcon = LoadTrayIcon();
            _vietnameseTrayIcon = CreateCheckedTrayIcon(_trayIcon);
            _tray = new NotifyIcon
            {
                Icon = _trayIcon,
                Text = "CMkey — Bình thường",
                Visible = true
            };

            var menu = new ContextMenuStrip();
            _vietnameseItem = new ToolStripMenuItem("Tiếng Việt (Telex)");
            _vietnameseItem.Click += (s, e) => SelectMode(Mode.Vietnamese);
            _englishItem = new ToolStripMenuItem("English");
            _englishItem.Click += (s, e) => SelectMode(Mode.English);
            _customizeItem = new ToolStripMenuItem("Cá nhân hóa");
            _customizeItem.Click += (s, e) => ShowCustomization();
            var aboutItem = new ToolStripMenuItem("About");
            aboutItem.Click += (s, e) =>
            {
                using (var form = new AboutForm()) form.ShowDialog();
            };
            _offItem = new ToolStripMenuItem("Bình thường");
            _offItem.Click += (s, e) => SetMode(Mode.Off);
            _closeItem = new ToolStripMenuItem("Đóng");
            _closeItem.Click += (s, e) =>
            {
                SetMode(Mode.Off);
                Cleanup();
                Application.Exit();
            };
            // Both columns share the same row heights, text inset and check gutter.
            Font menuFont = SystemFonts.MessageBoxFont;
            int inset = Math.Max(8, menuFont.Height / 2);
            int rowGap = Math.Max(4, menuFont.Height / 4);
            int rowHeight = menuFont.Height + inset * 2;
            int columnWidth = 0;
            var modes = new[] { _vietnameseItem, _englishItem, _offItem };
            var tools = new[] { _customizeItem, aboutItem, _closeItem };
            foreach (ToolStripMenuItem item in new[] {
                _vietnameseItem, _englishItem, _offItem, _customizeItem, aboutItem, _closeItem })
                columnWidth = Math.Max(columnWidth, TextRenderer.MeasureText(item.Text, menuFont).Width);
            columnWidth += menuFont.Height + inset * 3;
            int dividerWidth = inset * 2 + 1;
            int rightTrim = Math.Max(12, inset * 2 + 2);
            var popup = new TableLayoutPanel
            {
                AutoSize = false,
                ColumnCount = 3,
                RowCount = 3,
                Padding = new Padding(inset),
                Margin = new Padding(0),
                Size = new Size(columnWidth * 2 + dividerWidth + inset * 2,
                    rowHeight * 3 + rowGap * 2 + inset * 2),
                BackColor = SystemColors.Window,
                Font = menuFont
            };
            popup.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, columnWidth));
            popup.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, dividerWidth));
            popup.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, columnWidth));
            var modeButtons = new Button[3];
            for (int row = 0; row < 3; row++)
            {
                int gap = row < 2 ? rowGap : 0;
                popup.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight + gap));
                modeButtons[row] = CreateMenuButton(modes[row], menu, true);
                Button toolButton = CreateMenuButton(tools[row], menu, false);
                modeButtons[row].Margin = toolButton.Margin = new Padding(0, 0, 0, gap);
                popup.Controls.Add(modeButtons[row], 0, row);
                popup.Controls.Add(toolButton, 2, row);
            }
            var divider = new Panel
            {
                Dock = DockStyle.Fill, BackColor = SystemColors.ControlLight,
                Margin = new Padding(inset, 0, inset, 0)
            };
            popup.Controls.Add(divider, 1, 0);
            popup.SetRowSpan(divider, 3);
            menu.ShowImageMargin = false;
            menu.ShowCheckMargin = false;
            menu.Padding = new Padding(1);
            menu.BackColor = SystemColors.Window;
            var host = new ToolStripControlHost(popup)
            {
                AutoSize = false, Margin = new Padding(0), Padding = new Padding(0), Size = popup.Size
            };
            menu.Items.Add(host);
            menu.PerformLayout();
            // Trim the unused ToolStrip gutter, not either of the equal-width columns.
            Size menuSize = menu.PreferredSize;
            menu.AutoSize = false;
            menu.Size = new Size(Math.Max(host.Bounds.Right + inset, menuSize.Width - rightTrim),
                menuSize.Height);
            menu.Opening += (s, e) =>
            {
                foreach (Button button in modeButtons)
                {
                    var item = (ToolStripMenuItem)button.Tag;
                    button.BackColor = item.Checked ? Color.FromArgb(112, 82, 190) : SystemColors.Window;
                    button.ForeColor = item.Checked ? Color.White : SystemColors.WindowText;
                    button.FlatAppearance.MouseOverBackColor = item.Checked ? Color.FromArgb(98, 67, 176) : Color.FromArgb(240, 235, 250);
                    button.FlatAppearance.MouseDownBackColor = item.Checked ? Color.FromArgb(85, 56, 158) : Color.FromArgb(226, 216, 245);
                    button.AccessibleDescription = item.Checked ? "Đang chọn" : "";
                    button.Invalidate();
                }
            };
            _tray.ContextMenuStrip = menu;
            _tray.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left && menu.Enabled)
                    SelectMode(_mode == Mode.Vietnamese ? Mode.Off : Mode.Vietnamese);
            };

            SetMode(Mode.Off);
            CombatMap.LoadCustomOverrides();

            _proc = HookCallback;
            _hook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(null), 0);
            if (_hook == IntPtr.Zero)
            {
                MessageBox.Show("CMkey không cài được global keyboard hook. Bàn phím sẽ không bị thay đổi.",
                    "CMkey", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Application.ApplicationExit += (s, e) => Cleanup();

            if (startupMode == Mode.Vietnamese)
                SetMode(Mode.Vietnamese);
            else if (startupMode == Mode.English)
                SetMode(Mode.English);

            Application.Run();
        }

        static Button CreateMenuButton(ToolStripMenuItem item, ContextMenuStrip menu, bool modeButton)
        {
            Font font = SystemFonts.MessageBoxFont;
            int inset = Math.Max(8, font.Height / 2);
            var button = new Button
            {
                Tag = item,
                AccessibleName = item.Text,
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = font,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0),
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText,
                UseVisualStyleBackColor = false,
                UseMnemonic = false,
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 235, 250);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(226, 216, 245);
            button.Paint += (s, e) =>
            {
                Rectangle bounds = button.ClientRectangle;
                Rectangle checkBounds = new Rectangle(inset, 0, font.Height, bounds.Height);
                Rectangle textBounds = new Rectangle(checkBounds.Right + inset, 0,
                    Math.Max(1, bounds.Width - checkBounds.Right - inset * 2), bounds.Height);
                TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding;
                if (modeButton && item.Checked)
                    TextRenderer.DrawText(e.Graphics, "✓", font, checkBounds, button.ForeColor, flags);
                TextRenderer.DrawText(e.Graphics, item.Text, font, textBounds, button.ForeColor,
                    flags | TextFormatFlags.EndEllipsis);
            };
            button.Click += (s, e) =>
            {
                menu.Close();
                item.PerformClick();
            };
            return button;
        }

        static void ShowCustomization()
        {
            // The global hook must not rewrite text typed into this configuration form.
            // Restore the selected mode after the form closes; saving the form only
            // changes the mapping, not the user's current mode.
            Mode previousMode = _mode;
            SetMode(Mode.Off);
            _tray.ContextMenuStrip.Enabled = false;
            try
            {
                using (var form = new CustomOverridesForm())
                    form.ShowDialog();
            }
            finally
            {
                _tray.ContextMenuStrip.Enabled = true;
                SetMode(previousMode);
            }
        }

        static void SelectMode(Mode requested)
        {
            if (requested == _mode) return;
            SetMode(requested);
        }

        static void SetMode(Mode mode)
        {
            _mode = mode;
            ResetTelexBuffer();

            if (_tray != null)
            {
                _tray.Icon = mode == Mode.Vietnamese ? _vietnameseTrayIcon : _trayIcon;
                _tray.Text = mode == Mode.Vietnamese ? "CMkey — Tiếng Việt (Telex) ✓"
                    : mode == Mode.English ? "CMkey — Tiếng Anh" : "CMkey — Bình thường";
            }

            if (_vietnameseItem != null) _vietnameseItem.Checked = mode == Mode.Vietnamese;
            if (_englishItem != null) _englishItem.Checked = mode == Mode.English;
            if (_offItem != null) _offItem.Checked = mode == Mode.Off;
        }

        static void ResetTelexBuffer()
        {
            _rawBuffer.Length = 0;
            _lastEmitted = "";
        }

        static void Cleanup()
        {
            if (_cleanedUp) return;
            _cleanedUp = true;

            if (_hook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hook);
                _hook = IntPtr.Zero;
            }
            if (_tray != null)
            {
                _tray.Visible = false;
                _tray.Dispose();
                _tray = null;
            }
            if (_vietnameseTrayIcon != null)
            {
                _vietnameseTrayIcon.Dispose();
                _vietnameseTrayIcon = null;
            }
            if (_trayIcon != null)
            {
                _trayIcon.Dispose();
                _trayIcon = null;
            }
            if (_ownsSingleInstanceMutex && _singleInstanceMutex != null)
            {
                try { _singleInstanceMutex.ReleaseMutex(); } catch (ApplicationException) { }
                _singleInstanceMutex.Dispose();
                _singleInstanceMutex = null;
                _ownsSingleInstanceMutex = false;
            }
        }

        static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && _mode != Mode.Off
                    && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                try
                {
                    KBDLLHOOKSTRUCT data = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(
                        lParam, typeof(KBDLLHOOKSTRUCT));

                    // Do not process events injected by CMkey itself.
                    if (data.dwExtraInfo == SELF_TAG)
                        return CallNextHookEx(_hook, nCode, wParam, lParam);

                    if (_mode == Mode.English)
                    {
                        if (IsModifierVk(data.vkCode) || CtrlAltWinDown())
                            return CallNextHookEx(_hook, nCode, wParam, lParam);

                        char typed = KeyToChar(data.vkCode, data.scanCode);
                        if (typed != '\0')
                        {
                            string replacement = CombatMap.Transform(typed);
                            if (!String.IsNullOrEmpty(replacement)
                                    && replacement != typed.ToString()
                                    && SendUnicode(replacement))
                                return (IntPtr)1;
                        }
                    }
                    else if (_mode == Mode.Vietnamese)
                    {
                        if (HandleVietnameseKey(data.vkCode, data.scanCode))
                            return (IntPtr)1;
                    }
                }
                catch
                {
                    // A hook callback must never break the target application's
                    // input chain. Drop only our composition state on an error.
                    ResetTelexBuffer();
                }
            }

            return CallNextHookEx(_hook, nCode, wParam, lParam);
        }

        // Returns true only when CMkey successfully replaced the original key.
        static bool HandleVietnameseKey(uint vk, uint scan)
        {
            if (CtrlAltWinDown())
            {
                ResetTelexBuffer();
                return false;
            }

            // Shift/CapsLock are allowed to affect the next KeyToChar result.
            if (IsModifierVk(vk)) return false;

            if (vk == VK_BACK)
            {
                if (_rawBuffer.Length == 0)
                {
                    ResetTelexBuffer();
                    return false;
                }

                _rawBuffer.Length -= 1;
                string composed = TelexEngine.Translate(_rawBuffer.ToString());
                return Reconcile(CombatMap.ApplyGhostFilter(composed));
            }

            // Navigation, function keys, whitespace and punctuation commit what is
            // already visible and must not leave a stale raw composition behind.
            // already visible and must not leave a stale raw composition behind.
            if (IsFunctionalOrNavigationVk(vk))
            {
                ResetTelexBuffer();
                return false;
            }

            char typed = KeyToChar(vk, scan);
            if (typed == '\0')
            {
                ResetTelexBuffer();
                return false;
            }

            if (char.IsLetter(typed))
            {
                _rawBuffer.Append(typed);
                string composed = TelexEngine.Translate(_rawBuffer.ToString());
                return Reconcile(CombatMap.ApplyGhostFilter(composed));
            }

            string customReplacement = CombatMap.Transform(typed);
            if (!String.IsNullOrEmpty(customReplacement) && customReplacement != typed.ToString())
            {
                ResetTelexBuffer();
                return SendUnicode(customReplacement);
            }

            // Digits and punctuation are boundaries. Let the boundary itself pass
            // through unchanged after forgetting the internal composition state.
            ResetTelexBuffer();
            return false;
        }

        // Reconcile the exact transformed string visible in the target app.
        static bool Reconcile(string newEmitted)
        {
            int common = 0;
            int max = Math.Min(_lastEmitted.Length, newEmitted.Length);
            while (common < max && _lastEmitted[common] == newEmitted[common]) common++;

            int backspaces = _lastEmitted.Length - common;
            if (backspaces > 0 && !SendBackspaces(backspaces))
            {
                ResetTelexBuffer();
                return false;
            }

            if (newEmitted.Length > common
                    && !SendUnicode(newEmitted.Substring(common)))
            {
                ResetTelexBuffer();
                return false;
            }

            _lastEmitted = newEmitted;
            return true;
        }

        static bool SendBackspaces(int count)
        {
            if (count <= 0) return true;

            var inputs = new INPUT[count * 2];
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                inputs[j++] = new INPUT
                {
                    type = INPUT_KEYBOARD,
                    ki = new KEYBDINPUT { wVk = (ushort)VK_BACK, dwFlags = 0, dwExtraInfo = SELF_TAG }
                };
                inputs[j++] = new INPUT
                {
                    type = INPUT_KEYBOARD,
                    ki = new KEYBDINPUT { wVk = (ushort)VK_BACK, dwFlags = KEYEVENTF_KEYUP, dwExtraInfo = SELF_TAG }
                };
            }
            return SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT))) == inputs.Length;
        }

        // Dịch vkCode + trạng thái Shift/CapsLock/layout hiện tại → ký tự thật.
        static char KeyToChar(uint vk, uint scan)
        {
            byte[] keyboardState = new byte[256];
            if (!GetKeyboardState(keyboardState)) return '\0';

            IntPtr hkl = GetKeyboardLayout(0);
            var sb = new StringBuilder(8);
            int rc = ToUnicodeEx(vk, scan, keyboardState, sb, sb.Capacity, 0, hkl);
            // rc < 0 is a dead key; rc > 1 can contain more than one UTF-16 unit.
            if (rc == 1 && sb.Length > 0) return sb[0];
            return '\0';
        }

        static bool SendUnicode(string text)
        {
            if (String.IsNullOrEmpty(text)) return true;

            var inputs = new List<INPUT>();
            foreach (char c in text)
            {
                inputs.Add(new INPUT
                {
                    type = INPUT_KEYBOARD,
                    ki = new KEYBDINPUT { wScan = c, dwFlags = KEYEVENTF_UNICODE, dwExtraInfo = SELF_TAG }
                });
                inputs.Add(new INPUT
                {
                    type = INPUT_KEYBOARD,
                    ki = new KEYBDINPUT { wScan = c, dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP, dwExtraInfo = SELF_TAG }
                });
            }
            INPUT[] inputArray = inputs.ToArray();
            return SendInput((uint)inputArray.Length, inputArray,
                Marshal.SizeOf(typeof(INPUT))) == inputArray.Length;
        }

        [DllImport("user32.dll")]
        static extern bool DestroyIcon(IntPtr icon);

        static Icon CreateCheckedTrayIcon(Icon original)
        {
            using (var bitmap = new Bitmap(32, 32))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.Transparent);
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    graphics.DrawIcon(original, new Rectangle(0, 0, 28, 28));
                    // Leave the lower-right corner for the badge, away from the main artwork.
                    using (var background = new SolidBrush(Color.FromArgb(112, 82, 190)))
                        graphics.FillEllipse(background, 18, 18, 13, 13);
                    using (var border = new Pen(Color.White, 1.5F))
                        graphics.DrawEllipse(border, 18, 18, 13, 13);
                    using (var check = new Pen(Color.White, 2.5F))
                    {
                        check.StartCap = check.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                        check.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                        graphics.DrawLines(check, new[] {
                            new PointF(21, 25), new PointF(24, 28), new PointF(28.5F, 22)
                        });
                    }
                }
                IntPtr handle = bitmap.GetHicon();
                try
                {
                    using (Icon borrowed = Icon.FromHandle(handle))
                        return (Icon)borrowed.Clone();
                }
                finally { DestroyIcon(handle); }
            }
        }

        static Icon LoadTrayIcon()
        {
            try
            {
                Icon associated = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                if (associated != null) return associated;
            }
            catch { }

            // This is only a defensive fallback for an unbuilt/debug binary. The
            // release build embeds CMkey.ico from the Android launcher artwork.
            return (Icon)SystemIcons.Application.Clone();
        }

        static bool AcquireSingleInstance()
        {
            Mutex candidate = null;
            try
            {
                bool createdNew;
                candidate = new Mutex(true, SINGLE_INSTANCE_NAME, out createdNew);
                if (createdNew)
                {
                    _singleInstanceMutex = candidate;
                    _ownsSingleInstanceMutex = true;
                    return true;
                }
                candidate.Dispose();
            }
            catch (Exception ex)
            {
                if (candidate != null) candidate.Dispose();
                MessageBox.Show("CMkey không tạo được khóa chạy một phiên: " + ex.Message,
                    "CMkey", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // A duplicate launch is intentionally silent; the existing tray
            // process already owns the only global keyboard hook.
            return false;
        }

        static Mode ParseStartupMode(string[] args)
        {
            if (args == null) return Mode.Off;
            foreach (string arg in args)
            {
                if (String.Equals(arg, "--mode=vietnamese", StringComparison.OrdinalIgnoreCase))
                    return Mode.Vietnamese;
                if (String.Equals(arg, "--mode=english", StringComparison.OrdinalIgnoreCase))
                    return Mode.English;
            }
            return Mode.Off;
        }
    }
}
