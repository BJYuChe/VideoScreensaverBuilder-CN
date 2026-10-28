using System.Globalization;

namespace VideoScreensaverBuilder;

internal static class UiLanguage
{
    private static readonly IReadOnlyDictionary<string, string> Chinese = new Dictionary<string, string>
    {
        ["window.title"] = "视频屏保制作器",
        ["header"] = "视频屏保制作器",
        ["video.placeholder"] = "请选择视频文件...",
        ["browse"] = "浏览...",
        ["output.name"] = "输出名称：",
        ["output.folder.placeholder"] = "请选择输出文件夹...",
        ["status.ready"] = "就绪",
        ["build"] = "生成 .scr",
        ["footer"] = "由 ControlOverflow 制作 宇澈音频©汉化中文",
        ["sdk.prompt"] = "生成屏保需要 .NET 8 SDK。\n\n是否现在下载？",
        ["sdk.title"] = "需要 SDK",
        ["sdk.status"] = "请安装 .NET 8 SDK 后重新启动",
        ["video.dialog.title"] = "选择视频文件",
        ["video.dialog.filter"] = "视频文件|*.mp4;*.avi;*.wmv;*.mkv;*.mov|MP4 文件|*.mp4|所有文件|*.*",
        ["folder.dialog.description"] = "选择输出文件夹",
        ["video.invalid"] = "请选择有效的视频文件。",
        ["folder.invalid"] = "请选择有效的输出文件夹。",
        ["error.title"] = "错误",
        ["output.default"] = "VideoScreensaver",
        ["status.success"] = "生成成功！",
        ["status.failed"] = "生成失败",
        ["success.message"] = "屏保生成成功！\n\n{0}\n\n要打开输出文件夹吗？",
        ["success.title"] = "成功",
        ["build.failed"] = "生成失败：\n\n{0}",
        ["status.extract"] = "正在提取模板...",
        ["status.copy"] = "正在复制视频...",
        ["status.build"] = "正在生成屏保（可能需要一分钟）...",
        ["status.create"] = "正在创建 .scr 文件...",
        ["status.done"] = "完成！",
    };

    private static bool IsSimplifiedChinese
    {
        get
        {
            string language = CultureInfo.CurrentUICulture.Name;
            return language.Equals("zh-CN", StringComparison.OrdinalIgnoreCase)
                || language.Equals("zh-SG", StringComparison.OrdinalIgnoreCase)
                || language.StartsWith("zh-Hans", StringComparison.OrdinalIgnoreCase);
        }
    }

    public static string Get(string key)
    {
        if (IsSimplifiedChinese)
        {
            return Chinese[key];
        }

        return key switch
        {
            "window.title" or "header" => "Video Screensaver Builder",
            "video.placeholder" => "Select a video file...",
            "browse" => "Browse...",
            "output.name" => "Output name:",
            "output.folder.placeholder" => "Select output folder...",
            "status.ready" => "Ready",
            "build" => "Build .scr",
            "footer" => "Made by ControlOverflow",
            "sdk.prompt" => ".NET 8 SDK is required to build screensavers.\n\nWould you like to download it now?",
            "sdk.title" => "SDK Required",
            "sdk.status" => "Install .NET 8 SDK and restart",
            "video.dialog.title" => "Select Video File",
            "video.dialog.filter" => "Video Files|*.mp4;*.avi;*.wmv;*.mkv;*.mov|MP4 Files|*.mp4|All Files|*.*",
            "folder.dialog.description" => "Select Output Folder",
            "video.invalid" => "Please select a valid video file.",
            "folder.invalid" => "Please select a valid output folder.",
            "error.title" => "Error",
            "output.default" => "VideoScreensaver",
            "status.success" => "Build successful!",
            "status.failed" => "Build failed",
            "success.message" => "Screensaver built successfully!\n\n{0}\n\nOpen output folder?",
            "success.title" => "Success",
            "build.failed" => "Build failed:\n\n{0}",
            "status.extract" => "Extracting templates...",
            "status.copy" => "Copying video...",
            "status.build" => "Building screensaver (this may take a minute)...",
            "status.create" => "Creating .scr file...",
            "status.done" => "Done!",
            _ => throw new KeyNotFoundException($"Unknown UI text key: {key}")
        };
    }
}
