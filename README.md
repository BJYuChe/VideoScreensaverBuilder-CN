# Video Screensaver Builder

A Windows tool to create custom video screensavers. Generate a single `.scr` file with your video embedded - no dependencies required.

## Features

- **Single file output** - Creates one `.scr` file, no DLLs needed
- **Automatic interface language** - Uses Simplified Chinese on Simplified Chinese Windows; otherwise English
- **Embedded video** - Video is packed inside the screensaver
- **Multi-monitor** - Plays on all screens simultaneously
- **Loop playback** - Video restarts automatically
- **Easy exit** - Move mouse, press any key, or click to exit

## Quick Start

1. Download `VideoScreensaverBuilder.exe` from [Releases](../../releases)
2. Run the Builder and select your video file (MP4 recommended)
3. Click **Build .scr** (or **生成 .scr** on Simplified Chinese Windows)
4. Right-click the generated `.scr` file → **Install**

## System Requirements

- Windows 10 or Windows 11
- To run generated `.scr` files: Nothing additional
- To use Builder: .NET 8 SDK

## Building from Source

```bash
git clone https://github.com/ControlOverflow/VideoScreensaverBuilder.git
cd VideoScreensaverBuilder
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o publish
```

## Project Structure

```
VideoScreensaverBuilder/
├── VideoScreensaverBuilder.sln      # Solution file
├── VideoScreensaverBuilder.csproj   # Builder project
├── App.xaml                         # WPF application
├── MainWindow.xaml                  # Builder UI
├── MainWindow.xaml.cs               # Builder logic
├── ScreensaverSource/               # Screensaver template (embedded)
│   ├── VideoScreensaver.csproj
│   ├── Program.cs
│   ├── ScreensaverWindow.xaml
│   └── ScreensaverWindow.xaml.cs
├── README.md
└── LICENSE
```

## How It Works

1. Builder extracts template files to a temp folder
2. Copies your video as `Resources/video.mp4`
3. Runs `dotnet publish` to create a single-file executable
4. Renames output to `.scr` (Windows screensaver format)

## Technologies

- **.NET 8** - WPF (Windows Presentation Foundation)
- **Windows Media Foundation** - Native video playback
- **Single-file publish** - Self-contained executable

## License

MIT License - See [LICENSE](LICENSE) for details.

## Contact

ControlOverflow - contact@controloverflow.dev
