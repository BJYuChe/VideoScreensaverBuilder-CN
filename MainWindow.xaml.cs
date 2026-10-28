using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using WinForms = System.Windows.Forms;

namespace VideoScreensaverBuilder;

public partial class MainWindow : Window
{
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, uint flags);

    private string? _videoPath;
    private string? _outputFolder;

    public MainWindow()
    {
        InitializeComponent();
        ApplyLanguage();
        EnableDarkTitleBar();
        CheckDotNetSdk();

        _outputFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        txtOutputPath.Text = _outputFolder;
    }

    private void ApplyLanguage()
    {
        Title = UiLanguage.Get("window.title");
        txtHeader.Text = UiLanguage.Get("header");
        txtVideoPath.Text = UiLanguage.Get("video.placeholder");
        btnBrowse.Content = UiLanguage.Get("browse");
        txtOutputNameLabel.Text = UiLanguage.Get("output.name");
        txtOutputPath.Text = UiLanguage.Get("output.folder.placeholder");
        txtStatus.Text = UiLanguage.Get("status.ready");
        btnBuild.Content = UiLanguage.Get("build");
        btnOutputBrowse.Content = UiLanguage.Get("browse");
        txtFooter.Text = UiLanguage.Get("footer");
    }

    private void EnableDarkTitleBar()
    {
        var hwnd = new WindowInteropHelper(this).EnsureHandle();

        // Dark title bar
        int value = 1;
        DwmSetWindowAttribute(hwnd, 20, ref value, sizeof(int)); // DWMWA_USE_IMMERSIVE_DARK_MODE

        // Hide icon
        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x0001;
        const uint SWP_FRAMECHANGED = 0x0020;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOZORDER = 0x0004;

        int style = GetWindowLong(hwnd, GWL_EXSTYLE);
        SetWindowLong(hwnd, GWL_EXSTYLE, style | WS_EX_DLGMODALFRAME);
        SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
    }

    private void CheckDotNetSdk()
    {
        if (!IsDotNetSdkInstalled())
        {
            var result = System.Windows.MessageBox.Show(
                UiLanguage.Get("sdk.prompt"),
                UiLanguage.Get("sdk.title"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://dotnet.microsoft.com/download/dotnet/8.0",
                    UseShellExecute = true
                });
            }

            btnBuild.IsEnabled = false;
            txtStatus.Text = UiLanguage.Get("sdk.status");
            txtStatus.Foreground = System.Windows.Media.Brushes.Orange;
        }
    }

    private bool IsDotNetSdkInstalled()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "--list-sdks",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output.Contains("8.");
        }
        catch
        {
            return false;
        }
    }

    private void btnBrowse_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = UiLanguage.Get("video.dialog.title"),
            Filter = UiLanguage.Get("video.dialog.filter")
        };

        if (dialog.ShowDialog() == true)
        {
            _videoPath = dialog.FileName;
            txtVideoPath.Text = _videoPath;

            txtOutputName.Text = Path.GetFileNameWithoutExtension(_videoPath) + "Screensaver";
        }
    }

    private void btnOutputBrowse_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new WinForms.FolderBrowserDialog
        {
            Description = UiLanguage.Get("folder.dialog.description")
        };

        if (dialog.ShowDialog() == WinForms.DialogResult.OK)
        {
            _outputFolder = dialog.SelectedPath;
            txtOutputPath.Text = _outputFolder;
        }
    }

    private async void btnBuild_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_videoPath) || !File.Exists(_videoPath))
        {
            System.Windows.MessageBox.Show(UiLanguage.Get("video.invalid"), UiLanguage.Get("error.title"),
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (string.IsNullOrEmpty(_outputFolder) || !Directory.Exists(_outputFolder))
        {
            System.Windows.MessageBox.Show(UiLanguage.Get("folder.invalid"), UiLanguage.Get("error.title"),
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        string outputName = txtOutputName.Text.Trim();
        if (string.IsNullOrEmpty(outputName))
        {
            outputName = UiLanguage.Get("output.default");
        }

        foreach (char c in Path.GetInvalidFileNameChars())
        {
            outputName = outputName.Replace(c.ToString(), "");
        }

        btnBuild.IsEnabled = false;
        btnBrowse.IsEnabled = false;
        btnOutputBrowse.IsEnabled = false;
        progressBar.Visibility = Visibility.Visible;
        progressBar.IsIndeterminate = true;

        try
        {
            await Task.Run(() => BuildScreensaver(_videoPath!, _outputFolder!, outputName));

            txtStatus.Text = UiLanguage.Get("status.success");
            txtStatus.Foreground = System.Windows.Media.Brushes.LightGreen;

            string outputFile = Path.Combine(_outputFolder!, outputName + ".scr");

            var result = System.Windows.MessageBox.Show(
                string.Format(UiLanguage.Get("success.message"), outputFile),
                UiLanguage.Get("success.title"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"/select,\"{outputFile}\""
                });
            }
        }
        catch (Exception ex)
        {
            txtStatus.Text = UiLanguage.Get("status.failed");
            txtStatus.Foreground = System.Windows.Media.Brushes.Red;
            System.Windows.MessageBox.Show(string.Format(UiLanguage.Get("build.failed"), ex.Message), UiLanguage.Get("error.title"),
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            btnBuild.IsEnabled = true;
            btnBrowse.IsEnabled = true;
            btnOutputBrowse.IsEnabled = true;
            progressBar.Visibility = Visibility.Collapsed;
        }
    }

    private void BuildScreensaver(string videoPath, string outputFolder, string outputName)
    {
        string tempFolder = Path.Combine(Path.GetTempPath(), "VideoScreensaverBuild_" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(tempFolder);

        try
        {
            UpdateStatus(UiLanguage.Get("status.extract"));

            ExtractTemplate(tempFolder);

            UpdateStatus(UiLanguage.Get("status.copy"));
            string resourcesFolder = Path.Combine(tempFolder, "Resources");
            Directory.CreateDirectory(resourcesFolder);
            File.Copy(videoPath, Path.Combine(resourcesFolder, "video.mp4"), true);

            UpdateStatus(UiLanguage.Get("status.build"));
            string publishFolder = Path.Combine(tempFolder, "publish");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o \"{publishFolder}\"",
                    WorkingDirectory = tempFolder,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Build failed:\n{stderr}\n{stdout}");
            }

            UpdateStatus(UiLanguage.Get("status.create"));
            string exePath = Path.Combine(publishFolder, "VideoScreensaver.exe");
            string scrPath = Path.Combine(outputFolder, outputName + ".scr");

            if (!File.Exists(exePath))
            {
                throw new Exception("Build output not found: " + exePath);
            }

            File.Copy(exePath, scrPath, true);

            UpdateStatus(UiLanguage.Get("status.done"));
        }
        finally
        {

            try
            {
                Directory.Delete(tempFolder, true);
            }
            catch { }
        }
    }

    private void ExtractTemplate(string targetFolder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames();
        const string prefix = "VideoScreensaverBuilder.ScreensaverSource.";

        foreach (var resourceName in resourceNames)
        {
            if (!resourceName.StartsWith(prefix))
                continue;

            string fileName = resourceName.Substring(prefix.Length);
            string targetPath = Path.Combine(targetFolder, fileName);

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) continue;

            using var fileStream = File.Create(targetPath);
            stream.CopyTo(fileStream);
        }
    }

    private void UpdateStatus(string message)
    {
        Dispatcher.Invoke(() =>
        {
            txtStatus.Text = message;
        });
    }
}
