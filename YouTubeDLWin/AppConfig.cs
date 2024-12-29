using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage.Search;

namespace YouTubeDLWin;

public class AppConfig
{
    public bool firstRun { get; set; } = true;
    public string VideoDownloadLocation { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + @"\YouTubeDLWin\";
    public string AudioDownloadLocation { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\YouTubeDLWin\";
    public string BinaryLocation { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\YouTubeDLWin\";

    public string ExternalDriveRoot { get; } = "";

    private static string savePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\ytdlwin.config";

    public void CreateJsonFile(AppConfig config) {
        config.firstRun = false;


        
        
        if (!File.Exists(savePath))
        {
            //File.Create(savePath);
            Debug.WriteLine($"[+] Creating Config File: {savePath}");
            Directory.CreateDirectory(config.VideoDownloadLocation);
            Directory.CreateDirectory(config.AudioDownloadLocation);
            string json = System.Text.Json.JsonSerializer.Serialize(config);
            File.WriteAllText(savePath, json);
            Debug.WriteLine($"[+] Writing to file complete: {savePath}");

        }



    }

    public static AppConfig ReadJsonFile() {
        try
        {
            using (StreamReader r = new StreamReader(savePath))
            {
                Debug.WriteLine($"AppConfig Save Path: {savePath}");
                return JsonConvert.DeserializeObject<AppConfig>(r.ReadToEnd()) ?? new AppConfig();
            }
        }
        catch (Exception e)
        {
            var app = new AppConfig();
            return app;
        }
    }

    public void AddExternalStorage()
    {
        DriveInfo[] drives = DriveInfo.GetDrives();


        foreach (var drive in drives) 
        {
            if (drive.DriveType == DriveType.Removable) 
            {
                Debug.WriteLine($"[+++] Root Path: {drive.RootDirectory.FullName}");
                this.AudioDownloadLocation = $"{drive.RootDirectory.FullName}";
                this.VideoDownloadLocation = this.AudioDownloadLocation;
            }    
        }
    }

    public void RemoveExternalStorage()
    {
        this.AudioDownloadLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\YouTubeDLWin\";
        this.VideoDownloadLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + @"\YouTubeDLWin\";
    }



}