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
    public string BinaryLocation { get; set; }= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\YouTubeDLWin\";

    private static string savePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\ytdlwin.config";

    public void CreateJsonFile(AppConfig config) {
        config.firstRun = false;


        
        
        if (!File.Exists(savePath))
        {
            File.Create(savePath);
            Directory.CreateDirectory(config.VideoDownloadLocation);
            Directory.CreateDirectory(config.AudioDownloadLocation);
            Directory.CreateDirectory(config.BinaryLocation);
            string json = System.Text.Json.JsonSerializer.Serialize(config);
            File.WriteAllText(savePath, json);
        }

           
        
    }

    public static AppConfig ReadJsonFile() {
        try
        {
            using (StreamReader r = new StreamReader(savePath))
            {
                return JsonConvert.DeserializeObject<AppConfig>(r.ReadToEnd()) ?? new AppConfig();
            }
        }
        catch (Exception e)
        {
            var app = new AppConfig();
            Debug.WriteLine($"AppConfig Save Path: {savePath}");
            return app;
        }
    }

}