// Log code 46

/*
 * User: Thibault MONTAUFRAY
 */
using NAudio;
using NAudio.Wave;
using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using Tools4Libraries;
using System.Collections.Generic;

namespace Droid_Audio
{
	public delegate void delegateTrack(object sender, EventArgs e);
	
	/// <summary>
	/// Description of Track.
	/// </summary>
	public class Track
	{
        #region Attribute
        private uint _year;
        private uint _number;
        private string _title;
		private string _classement;
		private string _style;
		private string _path_track;
		private string _path_track_viewer;
        private string _format;
        private string _path_cover_smart;
        private string _path_cover_large;
        private string name;
        private List<string> _genre;
        private List<string> _albums;
        private List<string> _artists;

        private Interface_audio _int_aud;
        private WaveStream _mainOutputStream;
        private WaveChannel32 _volumeStream;
        private WaveOutEvent _player;

        private NAudio.Wave.BlockAlignReductionStream _stream = null;
		private NAudio.Wave.DirectSoundOut _output = null;
		private CustomWaveViewer _customWaveViewer;
		
		private bool _fileOpen;
		private string Pcommand, FName, Phandle;
		private bool Playing, Paused, Looping, MutedAll, MutedLeft, MutedRight;
		private const int MM_MCINOTIFY = 0x03b9;
		private int Err, aVolume, bVolume, lVolume, pSpeed, rVolume, tVolume, VolBalance;
		private ulong Lng;
		[DllImport("winmm.dll")]
		private static extern int mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);
		
		public event delegateMusicTicket WaveViewerBuilt;
        #endregion

        #region Properties
        public List<string> Genre
        {
            get { return _genre; }
            set { _genre = value; }
        }
        public uint Number
        {
            get { return _number; }
            set { _number = value; }
        }
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }
        public CustomWaveViewer WaveViewer
		{
			// TODO : Thread this
			get
			{
				if (_customWaveViewer == null) BuildCWV();
				return _customWaveViewer;
			}
		}
		public NAudio.Wave.DirectSoundOut Output
		{
			get { return _output; }
			set { _output = value; }
		}
		public List<string> Albums
		{
			get { return _albums; }
			set { _albums = value; }
        }
        public List<string> Artists
        {
            get { return _artists; }
            set { _artists = value; }
        }
        public string AlbumName
        {
            get
            {
                if (_albums.Count == 0) return string.Empty;
                else return _albums[0];
            }
        }
        public string ArtistName
        {
            get
            {
                if (_artists.Count == 0) return string.Empty;
                else return _artists[0];
            }
        }
        public string Title
		{
			get { return _title; }
			set { _title = value; }
		}
		public uint Year
		{
			get { return _year; }
			set { _year = value; }
		}
		public bool FileOpened
		{
			get { return _fileOpen; }
		}
		public string FileName
		{
			get	{ return FName;	}
		}
		public string Path_track
		{
			get { return _path_track; }
			set
            {
                _path_track = value;

                TagLib.File tagFile = TagLib.File.Create(_path_track);
                _title = tagFile.Tag.Title;
                _artists.Add(tagFile.Tag.FirstAlbumArtist);
                _albums.Add(tagFile.Tag.Album);
                _year = tagFile.Tag.Year;
                _number = tagFile.Tag.Track;
                _genre = new List<string>(tagFile.Tag.Genres);
            }
        }
        public string Path_cover_smart
        {
            get { return _path_cover_smart; }
            set { _path_cover_smart = value; }
        }
        public string Path_cover_large
        {
            get { return _path_cover_large; }
            set { _path_cover_large = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Classement
		{
			get { return _classement; }
			set { _classement = value; }
		}
		public string Style
		{
			get { return _style; }
			set { _style = value; }
		}
		public string PHandle
		{
			get { return Phandle; }
		}
		public bool IsPlaying
		{
			get	{ return Playing; }
		}
		public bool IsPaused
		{
			get { return Paused; }
			set { Paused = value; }
		}
		public bool IsLooping
		{
			get { return Looping; }
			set
			{
				Looping = value;
				if (_fileOpen && Playing && !Paused)
				{
					if (Looping)
					{
						Pcommand = String.Format("play {0} notify repeat", Phandle);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1905 ] error number : " + Err.ToString());
					}
					else
					{
						Pcommand = String.Format("play {0} notify", Phandle);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1906 ] error number : " + Err.ToString());
					}
				}
			}
		}
		public int Speed
		{
			get
			{
				return pSpeed;
			}
			set
			{
				if (value >= 3 && value <= 4353)
				{
					pSpeed = value;

					Pcommand = String.Format("set {0} speed {1}", Phandle, pSpeed);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1907 ] error number : " + Err.ToString());
				}
			}
		}
		public ulong AudioLength
		{
			get
			{
				if (_fileOpen)
					return Lng;
				else
					return 0;
			}
		}
		public ulong CurrentPosition
		{
			get
			{
				if (_fileOpen && Playing)
				{
					StringBuilder s = new StringBuilder(128);
					Pcommand = String.Format("status {0} position", Phandle);
					if ((Err = mciSendString(Pcommand, s, 128, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1904 ] error number : " + Err.ToString());
					ulong position = Convert.ToUInt64(s.ToString());
					return position;
				}
				else
					return 0;
			}
		}
        #endregion

        #region Constructor
        public Track()
        {
            _genre = new List<string>();
            _albums = new List<string>();
            _artists = new List<string>();

            InitializeEnvironment();
        }
        public Track(string filePath)
        {
            _genre = new List<string>();
            _albums = new List<string>();
            _artists = new List<string>();
            Path_track = filePath;
            
            InitializeEnvironment();
        }
        public Track(string filePath, Interface_audio ia)
        {
            _genre = new List<string>();
            _albums = new List<string>();
            _artists = new List<string>();
            Path_track = filePath;
            _int_aud = ia;

            InitializeEnvironment();
        }
        #endregion

        #region Methods public
        public void Close()
		{
            if (!_fileOpen) Init();
			Stop();
			DisposeWave();
			try
			{
				if (_fileOpen)
				{
					FName = string.Empty;
					_fileOpen = false;
					Playing = false;
					Paused = false;
				}
			}
			catch (Exception exp1901)
			{
				Log.Write("[ ERR : 1901 ] " + exp1901.Message);
			}
		}
		public void Play()
        {
            if (!_fileOpen) Init();
            _player.Play();

			//if (!_fileOpen)
			//{
			//	_fileOpen = BuidReader();
			//}
			//else
			//{
			//	if (_fileOpen)
			//	{
			//		if (_output != null)
			//		{
			//			_output.Play();
			//			IsPaused = false;
			//		}
			//		else
			//		{
			//			if (!Playing)
			//			{
			//				Playing = true;
			//				Pcommand = String.Format("play {0}{1} notify", Phandle, Looping ? " repeat" : string.Empty);
			//				if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1912 ] error number : " + Err.ToString());
			//			}
			//			else
			//			{
							
			//				if (!Paused)
			//				{
			//					Paused = true;
			//					Pcommand = String.Format("pause {0}", Phandle);
			//					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1918 ] error number : " + Err.ToString());
			//				}
			//				else
			//				{
			//					Paused = false;
			//					Pcommand = String.Format("play {0}{1} notify", Phandle, Looping ? " repeat" : string.Empty);
			//					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1919 ] error number : " + Err.ToString());
			//				}
			//			}
			//		}
			//	}
			//}
		}
		public void PlayPause()
        {
            if (!_fileOpen) Init();
            if (_fileOpen)
            {
                _player.Play();
            }

		//	if (!_fileOpen)
		//	{
		//		_fileOpen = BuidReader();
		//	}
		//	else
		//	{
		//		if (_output != null)
		//		{
		//			if (_output.PlaybackState == NAudio.Wave.PlaybackState.Playing)
		//			{
		//				_output.Pause();
		//				IsPaused = true;
		//			}
		//			else if (_output.PlaybackState == NAudio.Wave.PlaybackState.Paused || _output.PlaybackState == NAudio.Wave.PlaybackState.Stopped)
		//			{
		//				_output.Play();
		//				IsPaused = false;
		//			}
		//		}
		//		else
		//		{
		//			if (!Paused)
		//			{
		//				Paused = true;
		//				Pcommand = String.Format("pause {0}", Phandle);
		//				if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1918 ] error number : " + Err.ToString());
		//			}
		//			else
		//			{
		//				Paused = false;
		//				Pcommand = String.Format("play {0}{1} notify", Phandle, Looping ? " repeat" : string.Empty);
		//				if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1919 ] error number : " + Err.ToString());
		//			}
		//		}
		//	}
		}
		public void Stop()
        {
            if (!_fileOpen) Init();
            _player.Stop();
			//if (_fileOpen)
			//{
			//	if (_output != null)
			//	{
			//		_output.Stop();
			//	}
			//	else
			//	{
			//		Playing = false;
			//		Paused = false;
			//		Pcommand = String.Format("seek {0} to start", Phandle);
			//		if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1916 ] error number : " + Err.ToString());
			//		Pcommand = String.Format("stop {0}", Phandle);
			//		if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1917 ] error number : " + Err.ToString());
			//	}
			//}
		}
		#endregion
		
		#region Methods private
        private void Init()
        {
            _fileOpen = false;

            if (_path_track != null)
            { 
                if (_format != null)
                { 
                    switch (_format.ToUpper())
                    {
                        case "WAV":
                            _mainOutputStream = new WaveFileReader(_path_track);
                            break;
                        case "MP3":
                            _mainOutputStream = new Mp3FileReader(_path_track);
                            break;
                        case "CUE":
                            _mainOutputStream = new CueWaveFileReader(_path_track);
                            break;
                        case "AIFF":
                            _mainOutputStream = new AiffFileReader(_path_track);
                            break;
                        default:
                            _mainOutputStream = new AudioFileReader(_path_track);
                            break;
                    }
                }
                else
                {
                    _mainOutputStream = new AudioFileReader(_path_track);
                }
                _volumeStream = new WaveChannel32(_mainOutputStream);
                _player = new WaveOutEvent();
                _player.Init(_volumeStream);
                _fileOpen = true;
            }
        }
        private bool BuidReader()
		{
			bool built = false;
            if (string.IsNullOrEmpty(Path_track)) { return false; }
			if (Path_track.EndsWith(".mp3")) { built =openNaMp3(); }
			else if (Path_track.EndsWith(".wav")) { built =openNaWav(); }
			else { Log.Write("[ DEB : 4602 ] Cannot open with naudio method."); }
			
			if (!built) { built = openMCI(); }
			if (built) _int_aud.Disposed += new delegateInterfaceMusic(int_aud_Disposed);
			
			return built;
		}
		private bool InitializeEnvironment()
		{
			Pcommand = string.Empty;
			FName = string.Empty;
			Playing = false;
			Paused = false;
			Looping = false;
			MutedAll = MutedLeft = MutedRight = false;
			rVolume = lVolume = aVolume = tVolume = bVolume = 1000;
			pSpeed = 1000;
			Lng = 0;
			VolBalance = 0;
			Err = 0;
			Phandle = "MP3Player";
			return true;
		}
		private bool openMCI()
		{
			try
			{
				if (!_fileOpen)
				{
					Pcommand = String.Format("open \"" + Path_track + "\" type mpegvideo alias {0}", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1908 ] error number : " + Err.ToString());
					FName = _int_aud.PathFile;
					Playing = false;
					Paused = false;
					Pcommand = String.Format("set {0} time format milliseconds", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1909 ] error number : " + Err.ToString());
					Pcommand = String.Format("set {0} seek exactly on", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1910 ] error number : " + Err.ToString());
					if (!CalculateLength())
					{
						_int_aud.Tsm.Dispose(null);
						Log.Write("[ DEB : 1904] cannot open music file");
						return false;
					}
					
					_fileOpen = true;
					return true;
				}
				else
				{
					Log.Write("[ DEB : 1903] cannot open music file");
					return false;
				}
			}
			catch (Exception exp1902)
			{
				_int_aud.Tsm.Dispose(null);
				Log.Write("[ ERR : 1902] cannot open music file.\n" + exp1902.Message);
				return false;
			}
		}
		private bool openNaMp3()
		{
			try
			{
				NAudio.Wave.WaveStream pcm = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(new NAudio.Wave.Mp3FileReader(Path_track));
				_stream = new NAudio.Wave.BlockAlignReductionStream(pcm);
				_output = new NAudio.Wave.DirectSoundOut();
				_output.Init(_stream);
				return true;
			}
			catch (Exception)
			{
				Log.Write("[ DEB : 4600 ] Cannot open wav file with naudio.");
				return false;
			}
		}
		private bool openNaWav()
		{
			try
			{
				NAudio.Wave.WaveStream pcm = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(Path_track));
				_stream = new NAudio.Wave.BlockAlignReductionStream(pcm);
				_output = new NAudio.Wave.DirectSoundOut();
				_output.Init(_stream);
				return true;
			}
			catch (Exception)
			{
				Log.Write("[ DEB : 4601 ] Cannot open wav file with naudio.");
				return false;
			}
		}
		private bool CalculateLength()
		{
			try
			{
				StringBuilder str = new StringBuilder(256);
				Pcommand = "status " + Phandle + " length";
				if ((Err = mciSendString(Pcommand, str, 256, IntPtr.Zero)) != 0)
				{
					Log.Write("[ WRN : 1907 ] Error on the length calcul in audio file");
				}
				Lng = Convert.ToUInt64(str.ToString());
				
				return true;
			}
			catch (Exception exp1906)
			{
				Log.Write("[ WRN : 1906 ] Cannot calculate the length of the audio file.\n" + exp1906.Message);
				return true;
				//return false; // TODO : that bad guys !!!
			}
		}
		private void Seek(ulong milliseconds)
		{
			if (_fileOpen && milliseconds <= Lng)
			{
				if (Playing)
				{
					if (Paused)
					{
						Pcommand = String.Format("seek {0} to {1}", Phandle, milliseconds);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1920 ] error number : " + Err.ToString());
					}
					else
					{
						Pcommand = String.Format("seek {0} to {1}", Phandle, milliseconds);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1921 ] error number : " + Err.ToString());
						Pcommand = String.Format("play {0}{1} notify", Phandle, Looping ? " repeat" : string.Empty);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1922 ] error number : " + Err.ToString());
					}
				}
			}
		}
		private void BuildCWV()
		{
			try
			{
				using (Mp3FileReader mp3 = new Mp3FileReader(_path_track))
				{
					using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
					{
						_path_track_viewer = Environment.CurrentDirectory + @"Music\waveviewer.wav";
						WaveFileWriter.CreateWaveFile(_path_track_viewer, pcm);
					}
				}
				
				_customWaveViewer = new CustomWaveViewer();
				_customWaveViewer.Dock = System.Windows.Forms.DockStyle.Fill;
				_customWaveViewer.Name = "customWaveViewer";
				_customWaveViewer.SamplesPerPixel = 128;
				_customWaveViewer.Size = new System.Drawing.Size(564, 187);
				_customWaveViewer.StartPosition = ((long)(0));
				_customWaveViewer.TabIndex = 1;
				_customWaveViewer.PenColor = Color.Gray;
				
				_customWaveViewer.WaveStream = new NAudio.Wave.WaveFileReader(_path_track_viewer);
				_customWaveViewer.FitToScreen();
				
				//OnWaveViewerBuilt(this, null);
			}
			catch (Exception exp4603)
			{
				Log.Write("[ DEB : 4603 ] Error when building the wave viewer.\n" + exp4603.Message);
			}
		}
		private void AnalysePath()
		{
			// allow you to analyse the path and take some informations (year, artist, ...)
		}
		private void DisposeWave()
		{
			if (_output != null)
			{
				if (_output.PlaybackState == NAudio.Wave.PlaybackState.Playing) _output.Stop();
				_output.Dispose();
				_output = null;
			}
			if (_stream != null)
			{
				_stream.Dispose();
				_stream = null;
			}
		}
		private void int_aud_Disposed(object sender, EventArgs e)
		{
			Close();
		}
		#endregion
		
		#region Methods private : Volume
		private int Balance
		{
			get
			{
				return VolBalance;
			}
			set
			{
				{
					VolBalance = value;
					double vPct = Convert.ToDouble(aVolume) / 1000.0;

					if (value < 0)
					{
						Pcommand = string.Format("setaudio {0} left volume to {1:#}", Phandle, aVolume);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1923 ] error number : " + Err.ToString());
						Pcommand = string.Format("setaudio {0} right volume to {1:#}", Phandle, (1000 + value) * vPct);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1924 ] error number : " + Err.ToString());
					}
					else
					{
						Pcommand = string.Format("setaudio {0} right volume to {1:#}", Phandle, aVolume);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1925 ] error number : " + Err.ToString());
						Pcommand = string.Format("setaudio {0} left volume to {1:#}", Phandle, (1000 - value) * vPct);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1926 ] error number : " + Err.ToString());
					}
				}
			}
		}
		private bool MuteAll
		{
			get
			{
				return MutedAll;
			}
			set
			{
				MutedAll = value;
				if (MutedAll)
				{
					Pcommand = String.Format("setaudio {0} off", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1927 ] error number : " + Err.ToString());
				}
				else
				{
					Pcommand = String.Format("setaudio {0} on", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1928 ] error number : " + Err.ToString());
				}
			}

		}
		private bool MuteLeft
		{
			get
			{
				return MutedLeft;
			}
			set
			{
				MutedLeft = value;
				if (MutedLeft)
				{
					Pcommand = String.Format("setaudio {0} left off", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1929 ] error number : " + Err.ToString());
				}
				else
				{
					Pcommand = String.Format("setaudio {0} left on", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1930 ] error number : " + Err.ToString());
				}
			}

		}
		private bool MuteRight
		{
			get
			{
				return MutedRight;
			}
			set
			{
				MutedRight = value;
				if (MutedRight)
				{
					Pcommand = String.Format("setaudio {0} right off", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1931 ] error number : " + Err.ToString());
				}
				else
				{
					Pcommand = String.Format("setaudio {0} right on", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1932 ] error number : " + Err.ToString());
				}
			}

		}
		private int VolumeAll
		{
			get
			{
				return aVolume;
			}
			set
			{
				if (_fileOpen && (value >= 0 && value <= 1000))
				{
					aVolume = value;
					Pcommand = String.Format("setaudio {0} volume to {1}", Phandle, aVolume);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1933 ] error number : " + Err.ToString());
				}
			}
		}
		private int VolumeBass
		{
			get
			{
				return bVolume;
			}
			set
			{
				if (_fileOpen && (value >= 0 && value <= 1000))
				{
					bVolume = value;
					Pcommand = String.Format("setaudio {0} bass to {1}", Phandle, bVolume);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1934 ] error number : " + Err.ToString());
				}
			}
		}
		private int VolumeLeft
		{
			get
			{
				return lVolume;
			}
			set
			{
				if (_fileOpen && (value >= 0 && value <= 1000))
				{
					lVolume = value;
					Pcommand = String.Format("setaudio {0} left volume to {1}", Phandle, lVolume);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1935 ] error number : " + Err.ToString());
				}
			}
		}
		private int VolumeRight
		{
			get
			{
				return rVolume;
			}
			set
			{
				if (_fileOpen && (value >= 0 && value <= 1000))
				{
					rVolume = value;
					Pcommand = String.Format("setaudio {0} right volume to {1}", Phandle, rVolume);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1935 ] error number : " + Err.ToString());
				}
			}
		}
		private int VolumeTreble
		{
			get
			{
				return tVolume;
			}
			set
			{
				if (_fileOpen && (value >= 0 && value <= 1000))
				{
					tVolume = value;
					Pcommand = String.Format("setaudio {0} treble to {1}", Phandle, tVolume);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.Write("[ DEB : 1936 ] error number : " + Err.ToString());
				}
			}
		}
		#endregion
		
		#region Methods protected
		protected virtual void OnWaveViewerBuilt(object sender, EventArgs e)
		{
			if (WaveViewerBuilt != null)
				WaveViewerBuilt(sender, e);
		}
		#endregion
	}
}
