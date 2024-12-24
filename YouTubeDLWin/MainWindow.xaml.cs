using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.Graphics;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using ABI.Microsoft.UI.Windowing;
using Microsoft.UI;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YouTubeDLWin
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private IProgress<DownloadProgress> progress;
        private IProgress<string> output;
        private YoutubeDL ytdl;
        private AppConfig config;
        private DownloadDialogPage dialogPage;

        public MainWindow()
        {
            
            this.InitializeComponent();
            var root = this.Content as FrameworkElement;
            if (root != null)
            {
                root.Loaded += (s, e) => firstLaunched();
            }

            this.dialogPage = new DownloadDialogPage();
            this.progress = new Progress<DownloadProgress>(showProgress);
            this.output = new Progress<string>((s) => this.dialogPage.GetOutputInfo(s));

            if(this.videoAudioChoice.IsChecked == true)
            {
                this.subtitles.Visibility = Visibility.Visible;
                this.embedChapters.Visibility = Visibility.Visible;
            }


        }

        private void showProgress(DownloadProgress p)
        {
            dialogPage.GetDownloadInfo(p);
            Debug.WriteLine($"Download Progress: {p.Progress}");
            Debug.WriteLine($"Download Speed: {p.DownloadSpeed}");
        }

        public async void firstLaunched()
        {

            this.config = AppConfig.ReadJsonFile();
            if (Directory.Exists(config.BinaryLocation))
            {
                await Utils.DownloadBinaries(directoryPath: config.BinaryLocation);
            }
            else
            {
                Directory.CreateDirectory(config.BinaryLocation);
                await Utils.DownloadBinaries(directoryPath: config.BinaryLocation);
            }

            if(this.config.firstRun)
            {
                FirstRunDialogPage page = new FirstRunDialogPage(window: this);
                ContentDialog dialog = new ContentDialog()
                {
                    XamlRoot = this.Content.XamlRoot,

                    Content = page,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    PrimaryButtonText = "OK",
                    SecondaryButtonText = "Cancel"
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    page.config.CreateJsonFile(this.config);

                }
               

            } 
            this.ytdl = new YoutubeDL()
            {
                FFmpegPath = $@"{config.BinaryLocation}ffmpeg.exe",
                YoutubeDLPath = $@"{config.BinaryLocation}yt-dlp.exe",
                IgnoreDownloadErrors = false
            };
            
        }


        public async void DownloadItem(object sender, RoutedEventArgs e)
        {
            var options = new OptionSet()
            {
                RecodeVideo = VideoRecodeFormat.Mkv,
                ExtractAudio = !this.videoAudioChoice.IsChecked ?? false,
                AudioFormat = AudioConversionFormat.Mp3,
                RemuxVideo = "mkv",
                EmbedThumbnail = true,
                EmbedMetadata = true,
                EmbedInfoJson = true,
                EmbedSubs = this.subtitles.IsChecked ?? false,
                EmbedChapters = this.embedChapters.IsChecked ?? true,
                Output = getDownloadPath(this.videoAudioChoice.IsChecked ?? false, UrlVideo.Text.Contains("playlist"))
            };
            var ct = new CancellationTokenSource();
            
            ContentDialog dialogue = new ContentDialog()
            {
                XamlRoot = this.Content.XamlRoot,
                Title = "Done",
                PrimaryButtonText = "Close",
                Content = dialogPage,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                
                
            };
            dialogue.CloseButtonClick += async (s, e) => ct.Cancel();
            var result = dialogue.ShowAsync();
            
            
            
            
            Debug.WriteLine($"YouTubeDL Instance: {ytdl.YoutubeDLPath}");
            if(File.Exists($@"{ytdl.YoutubeDLPath}"))
            {
                if (this.videoAudioChoice.IsChecked ?? true)
                {
                   var result = await this.ytdl.RunVideoDownload(UrlVideo.Text, progress: progress, output: output,  overrideOptions: options, ct: ct.Token);
                   
                   if (result.Success)
                   {
                   }
                   
                }
                else
                {
                    var result = await this.ytdl.RunAudioDownload(UrlVideo.Text, progress: progress, output: output,  overrideOptions: options, ct: ct.Token);
                    if (result.Success)
                    {
                    }
                }
            }
            else
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "No Yt-dlp found",

                };
                _ = dialog.ShowAsync();
            }


        }

        private string getDownloadPath(bool videoAudio = true, bool playlist = false)
        {
            if (playlist) {
                return (videoAudio) ?
                    $@"{config.VideoDownloadLocation}\%(playlist)s\%(playlist-index)s-%(title)s.%(ext)s"
                  : $@"{config.AudioDownloadLocation}\%(playlist)s\%(playlist-index)s-%(title)s.%(ext)s";
            } else
            {

                return (videoAudio) ?
                     $@"{config.VideoDownloadLocation}\%(title)s.%(ext)s"
                   : $@"{config.AudioDownloadLocation}\%(title)s.%(ext)s";
            }
        }

        private void VideoChecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox.IsChecked == true)
            {
                subtitles.Visibility = Visibility.Visible;
            }
            else
            {
                subtitles.Visibility = Visibility.Collapsed;
                
            }
        }
    }
}
