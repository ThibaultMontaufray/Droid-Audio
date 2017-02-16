// Log code 29 01

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools4Libraries;
using YoutubeExtractor;

namespace Droid_Audio
{
    public delegate void YoutubeExtractorEventHandler(object o);
    public class YoutubeExtractor
    {
        #region Attribute
        public event YoutubeExtractorEventHandler ProgressionChanged;
        public event YoutubeExtractorEventHandler CannotExtractAudio;
        public event YoutubeExtractorEventHandler TitleChanged;

        private VideoDownloader _videoDownloader;
        private int _extractProgression;
        private string _title;
        private string _mp3File;
        private string _mp4File;
        private System.Windows.Forms.Timer _timer;
        private CancellationTokenSource _cancelationToken;
        private CancellationToken _token;
        private Task<string> _extractionTask;
        private Interface_audio _intAud;
        #endregion

        #region Properties
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public int ExtractProgression
        {
            get { return _extractProgression; }
            set { _extractProgression = value; }
        }
        #endregion

        #region Constructor
        public YoutubeExtractor(Interface_audio intAud)
        {
            _intAud = intAud;
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 500;
            _timer.Tick += _timer_Tick;

            _cancelationToken = new CancellationTokenSource();
            _token = _cancelationToken.Token;
        }
        #endregion

        #region Methods public
        public void Abort()
        {
            _timer.Stop();
            _cancelationToken.Cancel();
            if (_videoDownloader != null) _videoDownloader.Dispose();
        }
        public async void YoutubeToAudioFile(string url, string audioFilePath)
        {
            _timer.Start();
            try
            {
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Servodroid\Droid-Audio"))
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Servodroid\Droid-Audio");
                }

                IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url, false);

                object[] parameters = new object[2];
                parameters[0] = videoInfos;
                parameters[1] = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Servodroid\Droid-Audio";

                _extractionTask = Task.Factory.StartNew(() => DownloadVideo(parameters), _token);
                _mp4File = await _extractionTask;

                //string mp4File = DownloadVideo(videoInfos, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Servodroid\Droid-Audio");
                if (!string.IsNullOrEmpty(_mp4File) && File.Exists(_mp4File))
                {
                    _mp3File = _mp4File.Replace(".mp4", ".mp3");
                    _intAud.ConvertSourceFile = _mp4File;
                    _intAud.ConvertTargetFile = _mp3File;

                    ToolBarEventArgs action = new ToolBarEventArgs("mp4tomp3");
                    _intAud.GlobalAction(null, action);

                    File.Copy(_mp3File, Path.Combine(audioFilePath, Path.GetFileName(_mp3File)));
                    File.Delete(_mp4File);
                    File.Delete(_mp3File);
                }
            }
            finally
            {
                _timer.Stop();
            }
        }
        #endregion

        #region Methods private
        private string DownloadAudio(IEnumerable<VideoInfo> videoInfos, string path)
        {
            try
            {
                VideoInfo video = videoInfos.Where(info => info.CanExtractAudio).OrderByDescending(info => info.AudioBitrate).First();
                if (TitleChanged != null) TitleChanged(video.Title);

                if (video.RequiresDecryption)
                {
                    DownloadUrlResolver.DecryptDownloadUrl(video);
                }

                var audioDownloader = new AudioDownloader(video, Path.Combine(path, RemoveIllegalPathCharacters(video.Title) + video.AudioExtension));

                audioDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage * 0.85);
                audioDownloader.AudioExtractionProgressChanged += (sender, args) => Console.WriteLine(85 + args.ProgressPercentage * 0.15);
                //audioDownloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;

                audioDownloader.Execute();
                return Path.Combine(path, RemoveIllegalPathCharacters(video.Title) + video.VideoExtension);
            }
            catch (Exception exp)
            {
                if (CannotExtractAudio != null) CannotExtractAudio(0);
                Log.Write("[ ERR : 2900 ] Error while saving artist profile.\n\n" + exp.Message);
                return null;
            }
        }
        private string DownloadVideo(object[] parameters)
        {
            try
            {
                IEnumerable<VideoInfo> videoInfos = parameters[0] as IEnumerable<VideoInfo>;
                string path = parameters[1] as string;

                VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);
                _title = video.Title;

                if (video.RequiresDecryption)
                {
                    DownloadUrlResolver.DecryptDownloadUrl(video);
                }

                using (_videoDownloader = new VideoDownloader(video, Path.Combine(path, RemoveIllegalPathCharacters(video.Title) + video.VideoExtension)))
                { 
                    _videoDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage);
                    _videoDownloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;

                    _videoDownloader.Execute();
                }
                return Path.Combine(path, RemoveIllegalPathCharacters(video.Title) + video.VideoExtension);
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 2900 ] Cannot extract video from youtube.\n\n" + exp.Message);
                return string.Empty;
            }
        }
        private string RemoveIllegalPathCharacters(string path)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }
        private void Downloader_DownloadProgressChanged(object sender, ProgressEventArgs e)
        {
            _extractProgression = (int)e.ProgressPercentage;
        }
        #endregion

        #region Event
        private void _timer_Tick(object sender, EventArgs e)
        {
            if (ProgressionChanged != null) ProgressionChanged(_extractProgression);
            if (TitleChanged != null) TitleChanged(_title);
        }
        #endregion
    }
}
