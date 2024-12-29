using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
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
using System.Reflection.Metadata;
using System.Collections.ObjectModel;
using System.Management;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YouTubeDLWin;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private IProgress<DownloadProgress> progress;
    private IProgress<string> output;
    private YoutubeDL ytdl;
    private AppConfig config;
    // private ObservableCollection<DownloadInfo> downloadItems;
    // private DownloadLists downloadListPage;
    public MainWindow()
    {
        this.InitializeComponent();

        // Event to detect external drive and change to its directory
        WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");

        ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
        insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
        insertWatcher.Start();

        WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
        ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
        removeWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
        removeWatcher.Start();




        var root = Content as FrameworkElement;
        config = AppConfig.ReadJsonFile();

        if (root != null) root.Loaded += (s, e) => firstLaunched();
        ytdl = new YoutubeDL()
        {
            FFmpegPath = $@"{config.BinaryLocation}\ffmpeg.exe",
            YoutubeDLPath = $@"{config.BinaryLocation}\yt-dlp.exe",
            IgnoreDownloadErrors = false
        };

        Debug.WriteLine($"yt-dlp path: {ytdl.YoutubeDLPath}");


        if (VideoAudioChoice.IsChecked == true)
        {
            Subtitles.Visibility = Visibility.Visible;
            EmbedChapters.Visibility = Visibility.Visible;
        }

        // DownloadInfoList.Visibility = Visibility.Collapsed;
        // downloadItems = new ObservableCollection<DownloadInfo>();
        // downloadListPage = new DownloadLists();
        // downloadListPage.GetItems(downloadItems);
    }


    private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
    {
        //ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
        config.AddExternalStorage();
    }

    private void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
    {
        config.RemoveExternalStorage();
    }

    public async void firstLaunched()
    {
        Debug.WriteLine($"[+] First Run: {config.firstRun}");

        if (config.firstRun)
        {
            var page = new FirstRunDialogPage(this);
            config = page.config;
            var dialog = new ContentDialog()
            {
                XamlRoot = Content.XamlRoot,
                Content = page,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                PrimaryButtonText = "Ok"
            };

            dialog.ShowAsync();

            if (!File.Exists(ytdl.YoutubeDLPath))
            {
                Directory.CreateDirectory(config.BinaryLocation);
                Debug.WriteLine("[+] Creating Directory");
                var task = Utils.DownloadBinaries(directoryPath: config.BinaryLocation);
                await task;
                if (task.IsCompleted) page.Switch();
                Debug.WriteLine("[+] Downloaded Binaries");
            }

            dialog.Closed += (sender, args) =>
            {
                Debug.WriteLine("[++] Closing Dialog and Creating config file");
                config.CreateJsonFile(config);
            };
        }
    }


    public async void DownloadItem(object sender, RoutedEventArgs e)
    {
        DownloadDialogPage downloadPage = new DownloadDialogPage();
        ContentDialog downloadDialog = new ContentDialog()
        {
            XamlRoot = Content.XamlRoot,
            Content = downloadPage,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            PrimaryButtonText = "Cancel",
            
            Title = "Download Progress"
        };
        
        progress = new Progress<DownloadProgress>((p) =>
        {
            Debug.WriteLine("Downloading...");
            downloadPage.GetProgressData((p.Progress * 100), p.DownloadSpeed);


            Debug.WriteLine($"Download Progress: {p.Progress * 100}");
            Debug.WriteLine($"Download Speed: {p.DownloadSpeed}");

            // if (downloadItem.IsComplete)
            // {
            //     downloadItems.Remove(downloadItem);
            //     if (downloadItems.Count <= 0) DownloadInfoList.Visibility = Visibility.Collapsed;
            //     Debug.WriteLine("Download Cancelled or Completed");
            // }
        });
        output = new Progress<string>((s) =>
        {
            //downloadItem.Output = s;
            downloadPage.GetOutput(s);
            Debug.WriteLine($"Output: {s}");
        });


        var options = new OptionSet()
        {
            RecodeVideo = VideoRecodeFormat.Mkv,
            ExtractAudio = !VideoAudioChoice.IsChecked ?? false,
            AudioFormat = AudioConversionFormat.Mp3,
            RemuxVideo = "mkv",
            EmbedThumbnail = true,
            EmbedMetadata = true,
            EmbedInfoJson = true,
            EmbedSubs = Subtitles.IsChecked ?? false,
            EmbedChapters = EmbedChapters.IsChecked ?? true,
            Output = getDownloadPath(VideoAudioChoice.IsChecked ?? false, UrlVideo.Text.Contains("playlist"))
        };

        var ct = new CancellationTokenSource();

        Debug.WriteLine($"[File Exists]: {File.Exists(ytdl.YoutubeDLPath)}");
        if (File.Exists(ytdl.YoutubeDLPath))
        {
            Debug.WriteLine("[++] Download Starting...");
            RunResult<string> result;
            downloadDialog.ShowAsync();

            downloadDialog.PrimaryButtonClick += (sender, args) => {
                ct.Cancel();
            };

            if (VideoAudioChoice.IsChecked ?? false)
            {
                result = await ytdl.RunVideoDownload(UrlVideo.Text, progress: progress, output: output,
                    overrideOptions: options, ct: ct.Token);
            }
            else
            {
                result = await ytdl.RunAudioDownload(UrlVideo.Text, progress: progress, output: output,
                    overrideOptions: options, ct: ct.Token);
            }
            if (result.Success)
            {
                downloadDialog.Hide();
                Debug.WriteLine("[++] Download Complete...");
            } else
            {
                Debug.WriteLine("[--] Download Failed");

            }


        }
       

    }

    private string getDownloadPath(bool videoAudio = true, bool playlist = false)
    {
        if (playlist)
            return videoAudio
                ? $@"{config.VideoDownloadLocation}\%(playlist)s\%(playlist-index)s-%(title)s.%(ext)s"
                : $@"{config.AudioDownloadLocation}\%(playlist)s\%(playlist-index)s-%(title)s.%(ext)s";
        else
            return videoAudio
                ? $@"{config.VideoDownloadLocation}\%(title)s.%(ext)s"
                : $@"{config.AudioDownloadLocation}\%(title)s.%(ext)s";
    }

    private void VideoChecked(object sender, RoutedEventArgs e)
    {
        var checkbox = sender as CheckBox;
        if (checkbox.IsChecked == true)
        {
            Subtitles.Visibility = Visibility.Visible;
            EmbedChapters.Visibility = Visibility.Visible;
        }
        else {
            Subtitles.Visibility = Visibility.Collapsed;
            EmbedChapters.Visibility = Visibility.Visible;
        }
    }

      
}