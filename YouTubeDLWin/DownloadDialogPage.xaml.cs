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
using Windows.Storage.Pickers;
using YoutubeDLSharp;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YouTubeDLWin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DownloadDialogPage : Page
    {
        public DownloadDialogPage()
        {
            this.InitializeComponent();
        }

        public void GetDownloadInfo(DownloadProgress p)
        {
            this.DownloadSpeed.Text = p.DownloadSpeed;
            this.ProgressBar.Value = p.Progress;
        }

        public void GetOutputInfo(string s)
        {
            this.Output.Text = s;
        }

    }
}
