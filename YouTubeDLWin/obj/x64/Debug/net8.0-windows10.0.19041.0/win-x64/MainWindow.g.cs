﻿#pragma checksum "C:\Users\Duah1\source\repos\YouTubeDLWin\YouTubeDLWin\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "D72846462D30711030BD8D4850DB2D0F6E3C93D5045FB35186B0DACE44267EFD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace YouTubeDLWin
{
    partial class MainWindow : 
        global::Microsoft.UI.Xaml.Window, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2411")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // MainWindow.xaml line 29
                {
                    this.UrlVideo = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBox>(target);
                }
                break;
            case 3: // MainWindow.xaml line 33
                {
                    this.DownloadButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                }
                break;
            case 4: // MainWindow.xaml line 42
                {
                    this.VideoAudioChoice = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.CheckBox>(target);
                    ((global::Microsoft.UI.Xaml.Controls.CheckBox)this.VideoAudioChoice).Click += this.VideoChecked;
                }
                break;
            case 5: // MainWindow.xaml line 47
                {
                    this.Subtitles = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.CheckBox>(target);
                }
                break;
            case 6: // MainWindow.xaml line 52
                {
                    this.EmbedChapters = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.CheckBox>(target);
                }
                break;
            case 7: // MainWindow.xaml line 58
                {
                    global::Microsoft.UI.Xaml.Controls.Button element7 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element7).Click += this.DownloadItem;
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }


        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2411")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Microsoft.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

