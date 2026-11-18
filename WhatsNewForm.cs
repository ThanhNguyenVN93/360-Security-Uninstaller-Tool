using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace frm360security
{
    internal class WhatsNewForm : Form
    {
        private readonly ReleaseInfo _release;
        private readonly bool _isEnglish;

        public WhatsNewForm(ReleaseInfo release, bool isEnglish)
        {
            _release = release;
            _isEnglish = isEnglish;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            bool en = _isEnglish;

            this.Text = en
                ? "New version available — " + _release.TagName
                : "Co phien ban moi — " + _release.TagName;
            this.Size = new Size(540, 440);
            this.MinimumSize = new Size(400, 320);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header label
            Label lblTitle = new Label();
            lblTitle.Text = en
                ? "A new version is available: " + _release.TagName
                : "Co phien ban moi: " + _release.TagName;
            lblTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblTitle.AutoSize = false;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 36;
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTitle.Padding = new Padding(8, 0, 0, 0);
            lblTitle.ForeColor = Color.FromArgb(0, 120, 212);

            // What's new heading
            Label lblWhatsNew = new Label();
            lblWhatsNew.Text = en ? "What's New:" : "Noi dung cap nhat:";
            lblWhatsNew.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblWhatsNew.AutoSize = false;
            lblWhatsNew.Dock = DockStyle.Top;
            lblWhatsNew.Height = 22;
            lblWhatsNew.Padding = new Padding(8, 2, 0, 0);

            // Release notes
            RichTextBox rtb = new RichTextBox();
            rtb.ReadOnly = true;
            rtb.BackColor = SystemColors.Window;
            rtb.BorderStyle = BorderStyle.None;
            rtb.Font = new Font("Segoe UI", 9f);
            rtb.Dock = DockStyle.Fill;
            rtb.Padding = new Padding(6);
            string notes = string.IsNullOrEmpty(_release.Body) || _release.Body.Trim().Length == 0
                ? (en ? "(No release notes provided.)" : "(Khong co noi dung.)")
                : _release.Body;
            rtb.Text = notes;

            // Button panel
            Panel pnlButtons = new Panel();
            pnlButtons.Height = 44;
            pnlButtons.Dock = DockStyle.Bottom;

            Button btnDownload = new Button();
            btnDownload.Text = en ? "Download" : "Tai xuong";
            btnDownload.Width = 110;
            btnDownload.Height = 30;
            btnDownload.Location = new Point(this.ClientSize.Width - 240, 7);
            btnDownload.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnDownload.BackColor = Color.FromArgb(0, 120, 212);
            btnDownload.ForeColor = Color.White;
            btnDownload.FlatStyle = FlatStyle.Flat;
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(_release.HtmlUrl))
                    Process.Start(_release.HtmlUrl);
                this.Close();
            };

            Button btnLater = new Button();
            btnLater.Text = en ? "Later" : "De sau";
            btnLater.Width = 90;
            btnLater.Height = 30;
            btnLater.Location = new Point(this.ClientSize.Width - 120, 7);
            btnLater.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnLater.FlatStyle = FlatStyle.Flat;
            btnLater.Click += (s, e) => this.Close();

            pnlButtons.Controls.Add(btnDownload);
            pnlButtons.Controls.Add(btnLater);

            // Panel with separator line above buttons
            Panel pnlSep = new Panel();
            pnlSep.Height = 1;
            pnlSep.Dock = DockStyle.Bottom;
            pnlSep.BackColor = Color.FromArgb(220, 220, 220);

            // Layout: add in reverse DockStyle.Top order
            this.Controls.Add(rtb);
            this.Controls.Add(lblWhatsNew);
            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlSep);
            this.Controls.Add(pnlButtons);
        }
    }
}
