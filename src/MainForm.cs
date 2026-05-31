using System.Diagnostics;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace TikTokVCamBypass;

public partial class MainForm : Form
{
    private const string NewFunc = "isVirtualCamera(e,t){return!1}";
    private const string JsRelPath = @"resources\app\static\js\";
    private const string FuncMarker = "isVirtualCamera(e,t){";
    private const string PatchedMarker = "isVirtualCamera(e,t){return!1}";

    private string? _targetFile;
    private string? _backupFile;
    private string? _version;
    private string? _appDir;
    private string? _jsFileName;
    private bool _obsRunning;
    private bool _tiktokRunning;
    private string _originalFunc = "";
    private bool _needAdmin;

    private static readonly string[] InstallPaths = {
        @"D:\TikTok LIVE Studio",
        @"C:\Program Files\TikTok LIVE Studio",
        @"C:\Program Files (x86)\TikTok LIVE Studio",
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "TikTok LIVE Studio"),
        @"C:\Program Files\TikTok LIVEStudio"
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
        Log("  TikTok 虚拟摄像头绕过 v1.0.2");
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
        Log($"版本: {_version}");
        Log($"目标: {_jsFileName}");
        if (File.Exists(_backupFile)) Log("备份: 已存在 ✓");
        else Log("备份: 待创建");

        // Check write permission
        if (!IsPatched())
        {
            try
            {
                using var fs = File.Open(_targetFile, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (UnauthorizedAccessException)
            {
                _needAdmin = true;
            }
        }
        if (_needAdmin) Log("⚠ 需要管理员权限（位于 Program Files）");
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

    private static string? FindJsFile(string versionDir)
    {
        var jsDir = Path.Combine(versionDir, JsRelPath);
        if (!Directory.Exists(jsDir)) return null;

        foreach (var filePath in Directory.GetFiles(jsDir, "*.js"))
        {
            try
            {
                var content = File.ReadAllText(filePath);
                if (content.Contains(FuncMarker))
                    return filePath;
            }
            catch { }
        }
        return null;
    }

    private bool IsPatched()
    {
        if (_targetFile == null || !File.Exists(_targetFile)) return false;
        try { return File.ReadAllText(_targetFile).Contains(PatchedMarker); }
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

        if (_needAdmin && !IsPatched())
        {
            Log("");
            Log(">>> 需要管理员权限 <<<");
            Log("正在以管理员身份重新启动...");
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(psi);
                Application.Exit();
                return;
            }
            catch
            {
                Log("❌ 提权失败，请右键以管理员身份运行此程序");
                return;
            }
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

        // Find isVirtualCamera(e,t){ and extract the full function
        int start = content.IndexOf(FuncMarker, StringComparison.Ordinal);
        if (start == -1)
        {
            Log("❌ 未找到 isVirtualCamera 函数定义");
            return;
        }

        // Use bracket counting to find the matching closing brace
        int end = FindMatchingBrace(content, start + FuncMarker.Length - 1);
        if (end == -1)
        {
            Log("❌ 无法解析函数结构（括号不匹配）");
            return;
        }

        // Extract original function
        _originalFunc = content.Substring(start, end - start + 1);
        Log($"匹配到函数: {_originalFunc.Length} 字符");

        // Create backup
        if (!File.Exists(_backupFile))
        {
            File.Copy(_targetFile, _backupFile);
            Log("已创建备份 ✓");
        }

        // Save original function for restore
        var backupMeta = _backupFile + ".meta";
        File.WriteAllText(backupMeta, _originalFunc);

        // Replace
        var newContent = content.Remove(start, end - start + 1)
                                .Insert(start, NewFunc);
        try
        {
            File.WriteAllText(_targetFile, newContent);
        }
        catch (UnauthorizedAccessException)
        {
            Log("❌ 权限不足，请以管理员身份运行");
            return;
        }

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
            var backupMeta = _backupFile + ".meta";
            if (File.Exists(backupMeta))
            {
                var originalFunc = File.ReadAllText(backupMeta);
                var content = File.ReadAllText(_targetFile);

                int start = content.IndexOf(PatchedMarker, StringComparison.Ordinal);
                if (start >= 0)
                {
                    content = content.Remove(start, PatchedMarker.Length)
                                     .Insert(start, originalFunc);
                    File.WriteAllText(_targetFile, content);
                    Log("已从元数据恢复 ✓");
                }
                else
                {
                    Log("文件已处于原始状态或无法识别");
                }
            }
            else
            {
                Log("备份文件不存在，无法恢复");
            }
        }
        Log("虚拟摄像头检测已恢复正常");
        KillServices();
    }

    private static int FindMatchingBrace(string content, int openBracePos)
    {
        int depth = 0;
        for (int i = openBracePos; i < content.Length; i++)
        {
            if (content[i] == '{') depth++;
            else if (content[i] == '}')
            {
                depth--;
                if (depth == 0) return i;
            }
        }
        return -1;
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
