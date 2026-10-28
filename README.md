# 🎬 Video Screensaver Builder‑CN
一款 Windows 自定义视频屏保制作工具，可以生成内嵌视频的独立 `.scr` 屏保单文件，**无需任何额外依赖**。

## ✨ 功能特性
- 📦 **单文件输出**：仅生成一个 `.scr` 文件，不需要附带任何 DLL 动态库
- 🌐 **自动界面语言**：简体中文 Windows 自动加载中文界面，其他系统默认英文
- 🎞️ **视频内嵌打包**：视频资源直接封装进屏保内部，不用单独存放视频
- 🖥️ **多显示器支持**：自动识别所有显示器，全部屏幕同步播放视频
- 🔁 **自动循环播放**：视频播放完毕自动从头重播
- 🚪 **便捷退出**：移动鼠标、点击鼠标、按下任意键盘按键即可退出屏保

## 🚀 快速上手
1. 前往 [Releases](../../releases) 下载 `VideoScreensaverBuilder.exe`
2. 运行工具，选择你的视频文件（**推荐 MP4 格式，兼容性最好**）
3. 点击【生成 .scr】
4. 对生成好的 `.scr` 文件右键 → **安装**，即可设置为系统屏保

## 💻 系统要求
- 操作系统：Windows 10 / Windows 11
- 运行打包后的 `.scr` 屏保：不需要额外安装任何运行库
- 运行打包工具本体：需要预先安装 **.NET 8 SDK**

## 🛠️ 源码编译
```bash
git clone https://github.com/BJYuChe/VideoScreensaverBuilder-CN.git
cd VideoScreensaverBuilder-CN
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o publish
编译完成后，程序输出在 `publish` 文件夹内。

VideoScreensaverBuilder-CN/
├── VideoScreensaverBuilder.sln      # 解决方案文件
├── VideoScreensaverBuilder.csproj   # 打包工具主项目
├── App.xaml                         # WPF 应用入口
├── MainWindow.xaml                  # 打包工具 UI 界面
├── MainWindow.xaml.cs               # 打包工具业务逻辑
├── ScreensaverSource/               # 屏保模板（程序内嵌调用）
│   ├── VideoScreensaver.csproj
│   ├── Program.cs
│   ├── ScreensaverWindow.xaml
│   └── ScreensaverWindow.xaml.cs
├── README.md
└── LICENSE


## 🧩 工作原理

1. 打包工具将内置屏保模板解压到系统临时文件夹
2. 将选中视频复制到模板资源目录，命名为 `Resources/video.mp4`
3. 调用 `dotnet publish` 编译模板，生成独立单文件程序
4. 将编译输出重命名为 `.scr`，也就是 Windows 识别的屏保格式

## ⚙️ 技术栈

- .NET 8 + WPF（Windows Presentation Foundation）
- Windows Media Foundation：调用系统原生解码器播放视频
- .NET 单文件独立发布模式（Self-contained）

## 📜 开源协议

MIT 开源协议，详情查看 [LICENSE](LICENSE) 文件。

## 📮 原作者信息

宇澈音频 © 独家编译
