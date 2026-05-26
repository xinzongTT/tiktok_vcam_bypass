namespace TikTokVCamBypass;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Panel statusDot;
    private System.Windows.Forms.Button btnToggle;
    private System.Windows.Forms.TextBox txtLog;
    private System.Windows.Forms.Label lblVersion;
    private System.Windows.Forms.Label lblInfo;
    private System.Windows.Forms.Label lblFooter;
    private System.Windows.Forms.Panel panelTop;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.lblTitle = new System.Windows.Forms.Label();
        this.lblStatus = new System.Windows.Forms.Label();
        this.statusDot = new System.Windows.Forms.Panel();
        this.btnToggle = new System.Windows.Forms.Button();
        this.txtLog = new System.Windows.Forms.TextBox();
        this.lblVersion = new System.Windows.Forms.Label();
        this.lblInfo = new System.Windows.Forms.Label();
        this.lblFooter = new System.Windows.Forms.Label();
        this.panelTop = new System.Windows.Forms.Panel();
        this.SuspendLayout();

        this.lblTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold);
        this.lblTitle.ForeColor = System.Drawing.Color.White;
        this.lblTitle.Location = new System.Drawing.Point(24, 20);
        this.lblTitle.Size = new System.Drawing.Size(370, 30);
        this.lblTitle.Text = "TikTok 虚拟摄像头绕过";

        this.lblVersion.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
        this.lblVersion.ForeColor = System.Drawing.Color.Gray;
        this.lblVersion.Location = new System.Drawing.Point(25, 50);
        this.lblVersion.Size = new System.Drawing.Size(370, 20);
        this.lblVersion.Text = "OBS Virtual Camera → TikTok LIVE Studio";

        this.statusDot.BackColor = System.Drawing.Color.Gray;
        this.statusDot.Location = new System.Drawing.Point(24, 85);
        this.statusDot.Size = new System.Drawing.Size(12, 12);
        this.statusDot.Paint += new System.Windows.Forms.PaintEventHandler(this.StatusDot_Paint);

        this.lblStatus.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
        this.lblStatus.ForeColor = System.Drawing.Color.LightGray;
        this.lblStatus.Location = new System.Drawing.Point(42, 82);
        this.lblStatus.Size = new System.Drawing.Size(355, 22);
        this.lblStatus.Text = "检测中...";

        this.lblInfo.Font = new System.Drawing.Font("Microsoft YaHei UI", 8F);
        this.lblInfo.ForeColor = System.Drawing.Color.DimGray;
        this.lblInfo.Location = new System.Drawing.Point(42, 104);
        this.lblInfo.Size = new System.Drawing.Size(355, 20);
        this.lblInfo.Text = "";

        this.btnToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnToggle.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
        this.btnToggle.ForeColor = System.Drawing.Color.White;
        this.btnToggle.BackColor = System.Drawing.Color.FromArgb(254, 44, 85);
        this.btnToggle.FlatAppearance.BorderSize = 0;
        this.btnToggle.Location = new System.Drawing.Point(24, 135);
        this.btnToggle.Size = new System.Drawing.Size(370, 45);
        this.btnToggle.Text = "启用绕过";
        this.btnToggle.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnToggle.Click += new System.EventHandler(this.BtnToggle_Click);

        this.txtLog.BackColor = System.Drawing.Color.FromArgb(18, 18, 18);
        this.txtLog.ForeColor = System.Drawing.Color.FromArgb(0, 200, 0);
        this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
        this.txtLog.Location = new System.Drawing.Point(24, 200);
        this.txtLog.Multiline = true;
        this.txtLog.ReadOnly = true;
        this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtLog.Size = new System.Drawing.Size(370, 215);
        this.txtLog.Text = "";

        this.lblFooter.Font = new System.Drawing.Font("Microsoft YaHei UI", 7F);
        this.lblFooter.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.lblFooter.Location = new System.Drawing.Point(24, 425);
        this.lblFooter.Size = new System.Drawing.Size(370, 18);
        this.lblFooter.Text = "切换后需重启 TikTok LIVE Studio 生效";

        this.panelTop.BackColor = System.Drawing.Color.FromArgb(254, 44, 85);
        this.panelTop.Height = 3;
        this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;

        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.FromArgb(12, 12, 12);
        this.ClientSize = new System.Drawing.Size(418, 455);
        this.Controls.Add(this.panelTop);
        this.Controls.Add(this.lblTitle);
        this.Controls.Add(this.lblVersion);
        this.Controls.Add(this.statusDot);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.lblInfo);
        this.Controls.Add(this.btnToggle);
        this.Controls.Add(this.txtLog);
        this.Controls.Add(this.lblFooter);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "TikTok 虚拟摄像头绕过 v1.0.2";
        this.Load += new System.EventHandler(this.MainForm_Load);
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
