// Log code 19

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
		private Panel sheet;
		private ToolStripMenuAudio tsm;
		private List<String> listToolStrip;
		private bool panelAudioBuilt;
		private PanelAudio panau;
		private List<Artist> listArtist;
		private Artist current_artist;
		private string path;
		
		public event delegateInterfaceMusic TicketClose;
		public event delegateInterfaceMusic Disposed;
		#endregion
		
		#region Properties
		public PanelAudio Panau
		{
			get { return panau; }
		}
		
		public List<Artist> ListArtist
		{
			get { return listArtist; }
			set { listArtist = value; }
		}
		
		public override bool Openned
		{
			get { return panau.OpenedCurrentFile; }
			set {  }
		}
		
		public Panel SheetMusic
		{
			get { return sheet; }
			set { sheet = value; }
		}
		
		public new ToolStripMenuAudio Tsm
		{
			get { return tsm; }
			set { tsm = value as ToolStripMenuAudio; }
		}
		#endregion
		
		#region Constructor / Destructor
        public Interface_audio()
        {
            BuildToolBar();
            _this = this;
        }
        public Interface_audio(List<String> lts, string pathmusic)
		{
			path = pathmusic;
			listToolStrip = lts;
            BuildToolBar();
            _this = this;
		}
		#endregion
		
		#region Methods Public
		#region Methods Public override
		public override bool Open(object o)
		{
			if(panau == null) BuildPanel();
			return AddTitle();
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
			panau.Resize();
		}
		public override void Refresh()
		{
			if (panelAudioBuilt)
			{
				panau.Refresh();
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
			tsm = new Assistant.ToolStripMenuAudio(listToolStrip);
			return tsm;
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
			}
		}
		public bool AddTitle()
		{
			return panau.AddTicket(this);
		}
		public Track GetTrack(string artist, string album, string path)
		{
			if (!string.IsNullOrEmpty(artist) && !string.IsNullOrEmpty(album)) 
			{
				foreach (Artist art in listArtist) 
				{
					foreach (Album alb in art.ListAlbum) 
					{
						foreach (Track t in alb.ListTrack) 
						{
							if (t.Path_track.Equals(path)) 
								return t;
						}
					}
				}
				//return new Track(sheet.Text, new Album(new Artist("Unknow")));
				return new Track(path, new Album(new Artist("Unknow")), this);
			}
			else
			{
				return new Track(path, new Album(new Artist("Unknow")), this);
			}
		}
		public void BuildPanel()
		{
			panau = new PanelAudio(this);
			panau.BackgroundImage = tsm.Gui.BackgroundImage;
			panau.BackgroundImageLayout = ImageLayout.Stretch;
			panau.TicketSelectedChange += new delegateMusic(panau_TicketSelectedChange);
			panau.TicketClose += new delegateMusic(panau_TicketClose);
			panau.TicketPP += new delegateMusic(panau_TicketPP);
            SheetMusic.Controls.Add(panau);
			
			BuildLibrary();
			panau.RefreshPanelLibrary();
			panelAudioBuilt = true;
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
                path = fbd.SelectedPath;
            }
			BuildLibrary();			
		 	panau.RefreshPanelLibrary();
        }
        private void LaunchRefreshLib()
		{
			BuildLibrary();			
		 	panau.RefreshPanelLibrary();
		}
		private void LaunchStop()
		{
			//panau.CurrentPFA.Stop();
			panau.CurrentPFA.TrackLinked.Stop();
		}
		private void LaunchPP()
		{
			//panau.CurrentPFA.Play();
			panau.CurrentPFA.TrackLinked.PlayPause();
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
		#endregion
		
		private void BuildLibrary()
		{
			LoadLibrary();
			
			try
			{
                if (Directory.Exists(path))
                {
                    List<string> dirs = new List<string>(Directory.GetDirectories(path));
                    //List<string> dirs = new List<string>(Directory.GetFiles(path));

                    ScanFolder(path);
                    foreach (string dir in dirs)
                    {
                        ScanFolder(dir);
                    }
                }
			}
			catch (Exception exp1905)
			{
				Log.write("[ ERR : 1905 ] The music path doesn't exist.\nPlease set it in the preference menu.\n\n" + exp1905.Message);
				return;
			}
		}
        private void ScanFolder(string dir)
        {
            string album_tamp = "";
            string author_tamp = "";
            bool flagVoidAlbum = false;

            string[] audio_ext = { "mp3", "wav", "wma", "flv" };
            List<string> list_ext = new List<string>(audio_ext);
			
            current_artist = new Artist("");
            listArtist.Add(current_artist);

            album_tamp = dir;
            author_tamp = dir;
            // here we have list of tracks or album lists
            //List<string> dir_artist_list = new List<string>(Directory.EnumerateDirectories(dir));
            List<string> dir_artist_list = new List<string>(Directory.GetDirectories(dir));
            foreach (string dir_album in dir_artist_list)
            {
                // list of tracks
                current_artist.Name = dir.Split('\\')[dir.Split('\\').Length - 1];
                album_tamp = dir_album;
                AddTrack(album_tamp);
            }
            //List<string> list_track = new List<string>(Directory.EnumerateFiles(dir));
            List<string> list_track = new List<string>(Directory.GetFiles(dir));

            if (string.IsNullOrEmpty(current_artist.Name))
            {
                current_artist.Name = "Unknow";
            }

            foreach (string trackTitle in list_track)
            {
                if (list_ext.Contains(trackTitle.Split('.')[trackTitle.Split('.').Length - 1].ToLower()))
                {
                    if (!flagVoidAlbum && dir_artist_list.Count > 0)
                    {
                        current_artist.GetLastAlbum().Name = "No album";
                        flagVoidAlbum = true;
                    }
                    if (dir_artist_list.Count == 0) current_artist.GetLastAlbum().Name = author_tamp.Split('\\')[author_tamp.Split('\\').Length - 1];

                    Track t = new Track(trackTitle, current_artist.GetLastAlbum(), this);
                }
            }
        }
        private void LoadLibrary()
		{
			listArtist = new List<Artist>();
			// TODO : add the save and import library.
		}
		private void AddTrack(string album)
		{
			string[] audio_ext = {"mp3", "wav", "wma", "flv"};
			List<string> list_ext = new List<string>(audio_ext);
			
			string[] audio_cover = {"jpg", "png"};
			List<string> list_cover = new List<string>(audio_cover);
			
			current_artist.AddAlbum(album);
			
			//List<string> list_title = new List<string>(Directory.EnumerateFiles(album));
			List<string> list_title = new List<string>(Directory.GetFiles(album));
			foreach (string s in list_title)
			{
				if(list_ext.Contains(s.Split('.')[s.Split('.').Length-1].ToLower()))
				{
					Track track = new Track(s, current_artist.GetLastAlbum(), this);
				}
				else if(list_cover.Contains(s.Split('.')[s.Split('.').Length-1].ToLower()))
				{
					if (s.ToLower().Contains("large")) current_artist.GetLastAlbum().Path_cover_large = s;
					else current_artist.GetLastAlbum().Path_cover_smart = s;
				}
				else
				{
					
				}
			}
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

