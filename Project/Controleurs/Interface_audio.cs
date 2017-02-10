// Log code 19 - 08

// http://www.developpez.net/forums/d521994/dotnet/langages/csharp/lire-wav-mp3-plus-simplement-possible/
// http://www.codeproject.com/Articles/14709/Playing-MP3s-using-MCI

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms.VisualStyles;
using Tools4Libraries;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Xml.Serialization;
using System.Linq;
using System.Net;

/// <summary>
/// Interface for Tobi Assistant application : take care, some french word here to allow Tobi to speak with natural langage.
/// </summary>            
namespace Droid_Audio
{
	public enum MCINotify
	{
		Success = 0x01,
		Superseded = 0x02,
		Aborted = 0x04,
		Failure = 0x08
	}

	public delegate void delegateInterfaceMusic(object sender, EventArgs e);
	
	public class Interface_audio : GPInterface
	{
		#region Attributes
        private static Interface_audio _this;
		private Panel _sheet;
		private ToolStripMenuAudio _tsm;
		private List<String> listToolStrip;
		private bool panelAudioBuilt;
		private PanelAudio _panau;
		private List<Track> _listTrack;
        private List<string> _recentAudio;
        private List<string> _musicFolders;
        private string _convertSrcFile;
        private string _convertTrgFile;
        
        public event delegateInterfaceMusic TicketClose;
		public event delegateInterfaceMusic Disposed;
        #endregion

        #region Properties
        public string ConvertTargetFile
        {
            get { return _convertTrgFile; }
            set { _convertTrgFile = value; }
        }
        public string ConvertSourceFile
        {
            get { return _convertSrcFile; }
            set { _convertSrcFile = value; }
        }
        public List<string> MusicFolders
        {
            get { return _musicFolders; }
            set
            {
                _musicFolders = value;
                SaveMusicFolders();
            }
        }
        public List<string> RecentAudio
        {
            get { return _recentAudio; }
            set { _recentAudio = value; }
        }
        public PanelAudio Panau
		{
			get { return _panau; }
		}
		public List<Track> ListTrack
		{
			get { return _listTrack; }
			set { _listTrack = value; }
		}
		public override bool Openned
		{
			get { return _panau.OpenedCurrentFile; }
			set {  }
		}
		public Panel SheetMusic
		{
			get { return _sheet; }
			set { _sheet = value; }
		}
		public ToolStripMenuAudio Tsm
		{
			get { return _tsm; }
			set { _tsm = value as ToolStripMenuAudio; }
		}
		#endregion
		
		#region Constructor / Destructor
        public Interface_audio()
        {
            Init();
        }
        public Interface_audio(List<String> lts, string pathmusic)
		{
            if (!_musicFolders.Contains(pathmusic))
            {
                _musicFolders.Add(pathmusic);
                SaveMusicFolders();
            }
			listToolStrip = lts;
            Init();
        }
        #endregion

        #region Methods Public
        #region Methods Public override
        public override bool Open(object o)
		{
            string filePath = string.Empty;
            if (o == null && !(o is string))
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Audio files (.mp3 .wma)|*.mp3;*.wma|All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    filePath = ofd.FileName;
                }
            }
            else
            {
                filePath = o as string;
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                SaveAudioRecurrent(filePath);
                if (_panau == null) BuildPanel();
                return AddTitle(filePath);
            }
            else
            {
                return false;
            }
		}
		public override void Print()
		{
			
		}
		public override void Close()
		{
			OnDispose(this, null);
		}
		public override bool Save()
		{
			return false;
		}
		public override void ZoomIn()
		{
			
		}
		public override void ZoomOut()
		{
			
		}
		public override void Copy()
		{
			
		}
		public override void Cut()
		{
			
		}
		public override void Paste()
		{
			
		}
		public override void Resize()
		{
			_panau.Resize();
		}
		public override void Refresh()
		{
			if (panelAudioBuilt)
			{
				_panau.Refresh();
			}
		}
		public override void GlobalAction(object sender, EventArgs e)
		{
			ToolBarEventArgs tbea = e as ToolBarEventArgs;
			string action = tbea.EventText;
			GoAction(action);
		}
		public System.Windows.Forms.RibbonTab BuildToolBar()
		{
			_tsm = new ToolStripMenuAudio(listToolStrip);
            _tsm.ActionAppened += GlobalAction;
			return _tsm;
		}
        #endregion
        public void GoAction(string act)
		{
			switch (act.ToLower())
			{
                case "import":
                    LaunchImport();
                    break;
                case "refreshlib":
                    LaunchRefreshLib();
                    break;
                case "equalizing":
					LaunchEqualizing();
					break;
				case "pp":
					LaunchPP();
					break;
				case "stop":
					LaunchStop();
					break;
				case "back":
					LaunchBack();
					break;
				case "rewind":
					LaunchRewind();
					break;
				case "forward":
					LaunchForward();
					break;
				case "next":
					LaunchNext();
					break;
				case "loop":
					LaunchLoop();
					break;
                case "eject":
                    LaunchEject();
                    break;
                case "mp3towav":
                    LaunchConvertMp3ToWav();
                    break;
                case "wavtomp3":
                    LaunchConvertWavToMp3();
                    break;
                case "getartistpicture":
                    LaunchGetArtistPicture();
                    break;
            }
		}
		public bool AddTitle(string filePath)
		{
			return _panau.AddTicket(this, filePath);
		}
		public Track GetTrack(string path)
		{
            return _listTrack.Where(t => t.Path_track.Equals(path)).ToList()[0];
		}
		public void BuildPanel()
		{
			_panau = new PanelAudio(this);
			_panau.TicketSelectedChange += new delegateMusic(panau_TicketSelectedChange);
			_panau.TicketClose += new delegateMusic(panau_TicketClose);
			_panau.TicketPP += new delegateMusic(panau_TicketPP);
            SheetMusic.Controls.Add(_panau);
			
			_panau.RefreshPanelLibrary();
			panelAudioBuilt = true;
		}
        public void SaveUserParams(string currentAudioPath)
        {
            SaveMusicFolders();
            SaveAudioRecurrent(currentAudioPath);
            SaveAudioLib();
        }

        #region ACTION
        public static void ACTION_130_augmenter_volume()
        {
        }
        public static void ACTION_131_reduire_volume()
        {
        }
        public static void ACTION_132_couper_son()
        {
        }
        public static void ACTION_133_charger_musique()
        {
        }
        public static void ACTION_134_arreter_lecture()
        {
        }
        public static void ACTION_135_lire_morceau()
        {
        }
        public static void ACTION_136_suspendre_morceau()
        {
        }
        public static void ACTION_137_lire_precedent()
        {
        }
        public static void ACTION_138_lire_suivant()
        {
        }
        public static void ACTION_139_boucler_playlist()
        {
        }
        public static void ACTION_140_ejecter_cd()
        {
            if (_this != null) _this.LaunchEject();
        }
        public static void ACTION_141_ouvrir_egaliseur()
        {
        }
        #endregion

        #endregion

        #region Methods private

        #region Methods Private Launch
        private void LaunchImport()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult dr = fbd.ShowDialog();
            if ( dr.Equals(DialogResult.OK))
            {
                if (!_musicFolders.Contains(fbd.SelectedPath))
                {
                    _musicFolders.Add(fbd.SelectedPath);
                    SaveMusicFolders();
                }
            }
			BuildLibrary();			
		 	_panau.RefreshPanelLibrary();
        }
        private void LaunchRefreshLib()
		{
			BuildLibrary();			
		 	_panau.RefreshPanelLibrary();
		}
		private void LaunchStop()
		{
			(_panau.CurrentPlayedTrack.AssociatedObject as Track).Stop();
		}
		private void LaunchPP()
		{
            (_panau.CurrentPlayedTrack.AssociatedObject as Track).PlayPause();
		}
		private void LaunchBack()
		{
			
		}
		private void LaunchRewind()
		{
			
		}
		private void LaunchForward()
		{
			
		}
		private void LaunchNext()
		{
			
		}
		private void LaunchLoop()
		{
			
		}
		private void LaunchEject()
		{
			Eject e = new Eject();
		}
		private void LaunchEqualizing()
		{
			
		}
        private void LaunchConvertWavToMp3()
        { 
            using (var reader = new NAudio.Wave.AudioFileReader(_convertSrcFile))
            using (var writer = new NAudio.Lame.LameMP3FileWriter(_convertTrgFile, reader.WaveFormat, NAudio.Lame.LAMEPreset.STANDARD))
            {
                reader.CopyTo(writer);
            }
        }
        private void LaunchConvertMp3ToWav()
        {
            using (var reader = new Mp3FileReader(_convertSrcFile))
            using (var writer = new WaveFileWriter(_convertTrgFile, reader.WaveFormat))
            { 
                reader.CopyTo(writer);
            }
        }
        private void LaunchGetArtistPicture()
        {
        }
        #endregion

        private void Init()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Track));
            List<string> lstFolder = new List<string>();

            _this = this;
            _sheet = new Panel();
            _recentAudio = new List<string>();
            _musicFolders = new List<string>();
            _listTrack = new List<Track>();

            if (Properties.Settings.Default.Artists == null) { Properties.Settings.Default.Artists = new System.Collections.Specialized.ListDictionary(); }
            if (Properties.Settings.Default.RecentTitles != null)
            { 
                foreach (var item in Properties.Settings.Default.RecentTitles)
                {
                    _recentAudio.Add(item);
                }
            }
            if (Properties.Settings.Default.Folders != null)
            {
                foreach (var item in Properties.Settings.Default.Folders)
                {
                    lstFolder.Add(item);
                }
            }
            if (Properties.Settings.Default.AudioLib != null)
            {
                foreach (var item in Properties.Settings.Default.AudioLib)
                {
                    using (StringReader textReader = new StringReader(item))
                    {
                        object o = serializer.Deserialize(textReader) as Track;
                        if (o is Track) { _listTrack.Add(o as Track); }
                    }
                }
            }
            else { lstFolder.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)); }
            _musicFolders = lstFolder;

            BuildToolBar();
            BuildPanel();
        }
        private void SaveAudioRecurrent(string filePath)
        {
            string thisAudio = string.Format("{0}#{1}", filePath, DateTime.Now);
            string instAudio;
            int countAudioFiles = 0;
            DateTime date;
            List<string> finalList = new List<string>();
            finalList.Add(thisAudio);

            foreach (var movie in _recentAudio)
            {
                instAudio = movie.Split('#')[0];
                if (File.Exists(instAudio))
                {
                    if (!instAudio.Equals(filePath))
                    {
                        if (DateTime.TryParse(movie.Split('#')[1], out date))
                        {
                            if (date >= DateTime.Now.AddMonths(-2))
                            {
                                finalList.Add(movie);
                            }
                        }
                    }
                    countAudioFiles++;
                }
                if (countAudioFiles > 16) break;
            }
            _recentAudio = finalList;

            Properties.Settings.Default.RecentTitles = new System.Collections.Specialized.StringCollection();
            foreach (var audio in _recentAudio)
            {
                Properties.Settings.Default.RecentTitles.Add(audio);
            }
            Properties.Settings.Default.Save();
        }
        private void SaveMusicFolders()
        {
            Properties.Settings.Default.Folders = new System.Collections.Specialized.StringCollection();
            foreach (var folder in _musicFolders)
            {
                Properties.Settings.Default.Folders.Add(folder);
            }
            Properties.Settings.Default.Save();
        }
        private void SaveAudioLib()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Track));
            Properties.Settings.Default.AudioLib = new System.Collections.Specialized.StringCollection();
            foreach (var artist in _listTrack)
            {
                try
                {
                    using (StringWriter textWriter = new StringWriter())
                    {
                        serializer.Serialize(textWriter, artist);
                        Properties.Settings.Default.AudioLib.Add(textWriter.ToString());
                    }
                }
                catch (Exception exp)
                {
                    Log.Write("[ ERR : 1907 ] Error while saving artist profile.\n\n" + exp.Message);
                }
            }
            Properties.Settings.Default.Save();
        }
        private void BuildLibrary()
		{
            //bool result;
			LoadLibrary();

            foreach (string folder in _musicFolders)
            {
                try
			    {
                    if (Directory.Exists(folder))
                    {
                        List<string> dirs = new List<string>(Directory.GetDirectories(folder));
                        //List<string> dirs = new List<string>(Directory.GetFiles(path));

                        //ScanFolder(folder);
                        foreach (string dir in dirs)
                        {
                            try
                            {
                                ScanFolder(dir);

                                //Task<bool> task = new Task<bool>(() => ScanFolder(dir));
                                //task.Start();
                                //result = await task;
                            }
                            catch (Exception exp)
                            {
                                Log.Write("[ ERR : 1906 ] Cannot create this track.\n\n" + exp.Message);
                            }
                        }
                        SaveAudioLib();
                    }
                }
			    catch (Exception exp1905)
			    {
				    Log.Write("[ ERR : 1905 ] The music path doesn't exist.\nPlease set it in the preference menu.\n\n" + exp1905.Message);
                }
            }
        }
        private bool ScanFolder(string dir)
        {
            Track track;
            string[] audio_ext = { ".mp3", ".wav", ".wma", ".flv", ".cue", ".aiff" };
            
            foreach (var subdir in Directory.GetDirectories(dir))
            {
                ScanFolder(subdir);
            }
            foreach (string filePath in Directory.GetFiles(dir).Where(f => audio_ext.Contains(Path.GetExtension(f))))
            {
                track = new Track(filePath);
                if (!_listTrack.Contains(track))
                {
                    if (!Properties.Settings.Default.Artists.Contains(track.ArtistName))
                    {
                        string artist = string.IsNullOrEmpty(track.ArtistName) ? "Unknow" : track.ArtistName;
                    }
                    _listTrack.Add(track);
                }
            }
            return true;
        }
        private void LoadLibrary()
		{
			_listTrack = new List<Track>();
		}
		#endregion
		
		#region Methods protected
		protected virtual void OnTicketClose(object sender, EventArgs e)
		{
			if (TicketClose != null)
				TicketClose(sender, e);
		}
		
		protected virtual void OnDispose(object sender, EventArgs e)
		{
			if (Disposed != null)
				Disposed(sender, e);
		}
		#endregion
		
		#region Event
		private void panau_TicketPP(object sender, EventArgs e)
		{
			GoAction("pp");
		}
		private void panau_TicketSelectedChange(object sender, EventArgs e)
		{
			if(sender==null)
			{
                //SheetMusic.Controls.Text = "Music";
			}
			else
			{
				PanelFicheAudio pfa = sender as PanelFicheAudio;
				//tsm.CurrentTabPage.Text = pfa.TrackLinked.Title;
			}
		}
		private void panau_TicketClose(object sender, EventArgs e)
		{
			GoAction("stop");
			OnTicketClose(sender, null);
		}
		#endregion
		
		#region Web

		#region Event Arguments
//		public class OpenFileEventArgs : EventArgs
//		{
//			public OpenFileEventArgs(string filename)
//			{
//				this.FileName = filename;
//			}
//			public readonly string FileName;
//		}
//
//		public class PlayFileEventArgs : EventArgs
//		{
//			public PlayFileEventArgs()
//			{
//			}
//		}
//
//		public class PauseFileEventArgs : EventArgs
//		{
//			public PauseFileEventArgs()
//			{
//			}
//		}
//
//		public class StopFileEventArgs : EventArgs
//		{
//			public StopFileEventArgs()
//			{
//			}
//		}
//
//		public class CloseFileEventArgs : EventArgs
//		{
//			public CloseFileEventArgs()
//			{
//			}
//		}
//
//		public class ErrorEventArgs : EventArgs
//		{
//			[DllImport("winmm.dll")]
//			static extern bool mciGetErrorString(int errorCode, StringBuilder errorText, int errorTextSize);
//
//			public ErrorEventArgs(int ErrorCode)
//			{
//				this.ErrorCode = ErrorCode;
//
//				StringBuilder sb = new StringBuilder(256);
//				mciGetErrorString(ErrorCode, sb, 256);
//				this.ErrorString = sb.ToString();
//			}
//
//			public readonly int ErrorCode;
//			public readonly string ErrorString;
//		}
//
//		public class SongEndEventArgs : EventArgs
//		{
//			public SongEndEventArgs()
//			{
//			}
//		}
//
//		public class OtherEventArgs : EventArgs
//		{
//			public OtherEventArgs(MCINotify Notification)
//			{
//				this.Notification = Notification;
//			}
//
//			public readonly MCINotify Notification;
//		}

		#endregion

		#region Event Handlers

//		public delegate void OpenFileEventHandler(Object sender, OpenFileEventArgs oea);
//
//		public delegate void PlayFileEventHandler(Object sender, PlayFileEventArgs pea);
//
//		public delegate void PauseFileEventHandler(Object sender, PauseFileEventArgs paea);
//
//		public delegate void StopFileEventHandler(Object sender, StopFileEventArgs sea);
//
//		public delegate void CloseFileEventHandler(Object sender, CloseFileEventArgs cea);
//
//		public delegate void ErrorEventHandler(Object sender, ErrorEventArgs eea);
//
//		public delegate void SongEndEventHandler(Object sender, SongEndEventArgs seea);
//
//		public delegate void OtherEventHandler(Object sender, OtherEventArgs oea);
//
//		public event OpenFileEventHandler OpenFile;
//
//		public event PlayFileEventHandler PlayFile;
//
//		public event PauseFileEventHandler PauseFile;
//
//		public event StopFileEventHandler StopFile;
//
//		public event CloseFileEventHandler CloseFile;
//
//		public event ErrorEventHandler Error;
//
//		public event SongEndEventHandler SongEnd;
//
//		public event OtherEventHandler OtherEvent;
//
//		protected virtual void OnOpenFile(OpenFileEventArgs oea)
//		{
//			if (OpenFile != null) OpenFile(this, oea);
//		}
//
//		protected virtual void OnPlayFile(PlayFileEventArgs pea)
//		{
//			if (PlayFile != null) PlayFile(this, pea);
//		}
//
//		protected virtual void OnPauseFile(PauseFileEventArgs paea)
//		{
//			if (PauseFile != null) PauseFile(this, paea);
//		}
//
//		protected virtual void OnStopFile(StopFileEventArgs sea)
//		{
//			if (StopFile != null) StopFile(this, sea);
//		}
//
//		protected virtual void OnCloseFile(CloseFileEventArgs cea)
//		{
//			if (CloseFile != null) CloseFile(this, cea);
//		}
//
//		protected virtual void OnError(ErrorEventArgs eea)
//		{
//			if (Error != null) Error(this, eea);
//		}
//
//		protected virtual void OnSongEnd(SongEndEventArgs seea)
//		{
//			if (SongEnd != null) SongEnd(this, seea);
//		}
//
//		protected virtual void OnOtherEvent(OtherEventArgs oea)
//		{
//			if (OtherEvent != null) OtherEvent(this, oea);
//		}

		#endregion

//		protected override void WndProc(ref Message m)
//		{
//			switch (m.Msg)
//			{
//				case MM_MCINOTIFY:
//					if (m.WParam.ToInt32() == 1)
//					{
//						Stop();
//						OnSongEnd(new SongEndEventArgs());
//					}
//					break;
//				case (int)MCINotify.Aborted:
//					OnOtherEvent(new OtherEventArgs(MCINotify.Aborted));
//					break;
//				case (int)MCINotify.Failure:
//					OnOtherEvent(new OtherEventArgs(MCINotify.Failure));
//					break;
//				case (int)MCINotify.Success:
//					OnOtherEvent(new OtherEventArgs(MCINotify.Success));
//					break;
//				case (int)MCINotify.Superseded:
//					OnOtherEvent(new OtherEventArgs(MCINotify.Superseded));
//					break;
//			}
//
//			base.WndProc(ref m);
//		}
		#endregion
	}
}

