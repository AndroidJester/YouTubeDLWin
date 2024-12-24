using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.AccessCache;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YouTubeDLWin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstRunDialogPage : Page
    {
        public Window window;
        public AppConfig config;
        public FirstRunDialogPage(Window window)
        {
            this.InitializeComponent();
            this.YtDLFolderButton.Content = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\YoutubeDLWin";
            this.VideoFolderButton.Content = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)}\YoutubeDLWin";
            this.AudioFolderButton.Content = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)}\YoutubeDLWin";
            this.window = window;
            this.config = new AppConfig()
            {
                BinaryLocation = this.YtDLFolderButton.Content as string ?? "",
                VideoDownloadLocation = this.VideoFolderButton.Content as string ?? "",
                AudioDownloadLocation = this.AudioFolderButton.Content as string ?? "",
            };
        }

        private async void GetFolderPath(object sender, RoutedEventArgs e)
        {
            var senderButton = sender as Button;
            senderButton.IsEnabled = false;

            FolderPicker picker = new FolderPicker();

            senderButton.Content = "Loading...";
            
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this.window);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.FileTypeFilter.Add("*");

            StorageFolder folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                senderButton.Content = folder.Path;
                senderButton.IsEnabled = true;
            }
            else 
            {
                senderButton.Content = "Failed to get folder Path";
            }


        }
    }
}
