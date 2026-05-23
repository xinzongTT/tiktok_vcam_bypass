using System.Diagnostics;
using Microsoft.Win32;

namespace TikTokVCamBypass;

public partial class MainForm : Form
{
    private const string OldFunc = "isVirtualCamera(e,t){if(!e||!t)return!1;if(e.startsWith(Yp))return!1;const i=()=>{const i=e===t,r=Boolean(e&&!(0,Rh.rG)(e));return i!==r&&o().info(\"[isVirtualCamera] validate conflict\",e,t),i||r},r=(0,at.sc)().sensitive_restricted_config;return r?!r.physical_camera_list.includes(t)&&(!!r.virtual_camera_ist.includes(t)||i()):i()}";
    private const string NewFunc = "isVirtualCamera(e,t){return!1}";
    private const string JsFileName = "7205.75336589.js";
    private const string JsRelPath = @"resources\app\static\js\";

    private string? _targetFile;
    private string? _backupFile;
    private string? _version;
    private string? _appDir;
    private bool _obsRunning;
    private bool _tiktokRunning;

    private static readonly string[] InstallPaths = {
        @"D:\TikTok LIVE Studio",
        @"C:\Program Files\TikTok LIVE Studio",
        @"C:\Program Files (x86)\TikTok LIVE Studio",
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "TikTok LIVE Studio")
    };

    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        RefreshState();
    }

    private void RefreshState()
    {
        txtLog.Clear();
        Log("═══════════════════════════");
        Log("  TikTok 虚拟摄像头绕过 v1.0");
        Log("═══════════════════════════");

        DetectEnvironment();

        if (_appDir == null || _targetFile == null)
        {
            SetStatus(false, "未找到 TikTok LIVE Studio", "请先安装或检查自定义路径");
            btnToggle.Text = "重新检测";
            btnToggle.Enabled = true;
            btnToggle.BackColor = Color.FromArgb(254, 180, 0);
            return;
        }

        bool patched = IsPatched();
        string obsStatus = _obsRunning ? "OBS 运行中 ✓" : "OBS 未检测到";
        string tiktokStatus = _tiktokRunning ? "TikTok 运行中" : "TikTok 未启动";

        if (patched)
        {
            SetStatus(true, "绕过已启用", $"v{_version}  |  {obsStatus}  |  {tiktokStatus}");
            btnToggle.Text = "恢复原始";
            btnToggle.BackColor = Color.FromArgb(50, 50, 50);
        }
        else
        {
            SetStatus(false, "绕过未启用", $"v{_version}  |  {obsStatus}  |  {tiktokStatus}");
            btnToggle.Text = "启用绕过";
            btnToggle.BackColor = Color.FromArgb(254, 44, 85);
        }
        btnToggle.Enabled = true;
    }

    private void DetectEnvironment()
    {
        _obsRunning = Process.GetProcessesByName("obs64").Length > 0;
        _tiktokRunning = Process.GetProcessesByName("TikTok LIVE Studio").Length > 0;

        if (_obsRunning) Log("OBS Studio: 运行中 ✓");
        else Log("OBS Studio: 未检测到");
        if (_tiktokRunning) Log("TikTok LIVE Studio: 运行中（切换后需重启）");
        else Log("TikTok LIVE Studio: 未启动");

        _appDir = FindTikTokInstall();
        if (_appDir == null)
        {
            Log("❌ 未找到 TikTok LIVE Studio 安装目录");
            Log("  已搜索以下路径:");
            foreach (var p in InstallPaths) Log($"    {p}");
            return;
        }

        Log($"安装目录: {_appDir}");

        var dirs = Directory.GetDirectories(_appDir)
            .Select(d => new DirectoryInfo(d))
            .Where(d => Version.TryParse(d.Name, out _))
            .OrderByDescending(d => new Version(d.Name))
            .ToList();

        if (dirs.Count == 0)
        {
            Log("❌ 未找到版本目录");
            _appDir = null;
            return;
        }

        _version = dirs[0].Name;
        _targetFile = Path.Combine(dirs[0].FullName, JsRelPath, JsFileName);
        _backupFile = _targetFile + ".backup";

        if (!File.Exists(_targetFile))
        {
            Log($"❌ 目标文件不存在: {JsFileName}");
            Log("  当前 TikTok 版本可能不受支持");
            _appDir = null;
            _targetFile = null;
            return;
        }

        Log($"版本: {_version}");
        Log($"目标: {JsFileName}");
        if (File.Exists(_backupFile)) Log("备份: 已存在 ✓");
        else Log("备份: 待创建");
    }

    private static string? FindTikTokInstall()
    {
        foreach (var path in InstallPaths)
            if (Directory.Exists(path))
                return path;

        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key != null)
            {
                foreach (var subName in key.GetSubKeyNames())
                {
                    using var sub = key.OpenSubKey(subName);
                    var name = sub?.GetValue("DisplayName") as string;
                    var loc = sub?.GetValue("InstallLocation") as string;
                    if (name != null && name.Contains("TikTok LIVE Studio") && loc != null && Directory.Exists(loc))
                        return loc;
                }
            }
        }
        catch { }

        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key != null)
            {
                foreach (var subName in key.GetSubKeyNames())
                {
                    using var sub = key.OpenSubKey(subName);
                    var name = sub?.GetValue("DisplayName") as string;
                    var loc = sub?.GetValue("InstallLocation") as string;
                    if (name != null && name.Contains("TikTok LIVE Studio") && loc != null && Directory.Exists(loc))
                        return loc;
                }
            }
        }
        catch { }

        return null;
    }

    private bool IsPatched()
    {
        if (_targetFile == null || !File.Exists(_targetFile)) return false;
        try { return File.ReadAllText(_targetFile).Contains(NewFunc); }
        catch { return false; }
    }

    private void SetStatus(bool active, string status, string info)
    {
        lblStatus.Text = status;
        lblInfo.Text = info;
        if (active)
        {
            lblStatus.ForeColor = Color.FromArgb(0, 220, 100);
            statusDot.BackColor = Color.FromArgb(0, 220, 100);
        }
        else
        {
            lblStatus.ForeColor = Color.LightGray;
            statusDot.BackColor = Color.Gray;
        }
    }

    private void BtnToggle_Click(object? sender, EventArgs e)
    {
        if (_targetFile == null)
        {
            RefreshState();
            return;
        }

        btnToggle.Enabled = false;
        btnToggle.Text = "处理中...";

        try
        {
            if (IsPatched()) Restore(); else Apply();
        }
        catch (Exception ex)
        {
            Log($"❌ 错误: {ex.Message}");
        }

        RefreshState();
    }

    private void Apply()
    {
        if (_targetFile == null || _backupFile == null) return;

        Log("");
        Log(">>> 正在应用补丁...");
        var content = File.ReadAllText(_targetFile);

        if (!content.Contains(OldFunc))
        {
            Log("⚠ 未找到预期函数签名");
            Log("  当前 TikTok 版本可能与 v1.27.0 不同");
            if (content.Contains("isVirtualCamera"))
            {
                Log("  找到 isVirtualCamera 但结构不匹配");
                Log("  无法自动处理此版本");
            }
            else
            {
                Log("  文件中未找到 isVirtualCamera");
            }
            return;
        }

        if (!File.Exists(_backupFile))
        {
            File.Copy(_targetFile, _backupFile);
            Log("已创建备份 ✓");
        }

        content = content.Replace(OldFunc, NewFunc);
        File.WriteAllText(_targetFile, content);

        if (IsPatched())
        {
            Log("补丁应用成功 ✓");
            Log("");
            Log("isVirtualCamera 已被修改为始终返回 false");
            Log("所有虚拟摄像头将被视为真实设备");
            KillServices();
        }
        else
        {
            Log("❌ 补丁应用失败，正在恢复...");
            if (File.Exists(_backupFile))
                File.Copy(_backupFile, _targetFile, true);
        }
    }

    private void Restore()
    {
        if (_targetFile == null || _backupFile == null) return;

        Log("");
        Log(">>> 正在恢复原始文件...");

        if (File.Exists(_backupFile))
        {
            File.Copy(_backupFile, _targetFile, true);
            Log("已从备份恢复 ✓");
        }
        else
        {
            var content = File.ReadAllText(_targetFile);
            if (content.Contains(NewFunc))
            {
                content = content.Replace(NewFunc, OldFunc);
                File.WriteAllText(_targetFile, content);
                Log("已内联修复 ✓");
            }
            else
            {
                Log("文件已处于原始状态");
            }
        }
        Log("虚拟摄像头检测已恢复正常");
        KillServices();
    }

    private void KillServices()
    {
        foreach (var name in new[] { "MediaSDK_Server", "dshow_server" })
        {
            try
            {
                var procs = Process.GetProcessesByName(name);
                foreach (var p in procs) p.Kill();
                if (procs.Length > 0) Log($"已结束进程: {name}.exe");
            }
            catch { }
        }
        Log("");
        Log(">>> 请重启 TikTok LIVE Studio <<<");
    }

    private void Log(string msg)
    {
        if (txtLog.InvokeRequired)
        {
            txtLog.Invoke(() => Log(msg));
            return;
        }
        var ts = DateTime.Now.ToString("HH:mm:ss");
        txtLog.AppendText($"[{ts}] {msg}\r\n");
    }

    private void StatusDot_Paint(object? sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using var brush = new SolidBrush(statusDot.BackColor);
        e.Graphics.FillEllipse(brush, 0, 0, 12, 12);
    }
}
