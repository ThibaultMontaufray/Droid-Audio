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

namespace Droid_Audio
{
	public delegate void delegateTrack(object sender, EventArgs e);
	
	/// <summary>
	/// Description of Track.
	/// </summary>
	public class Track
	{
		#region Attribute
		private string title;
		private string year;
		private string classement;
		private string style;
		private string path_track;
		private string path_track_viewer;
		private Album album;
		private Interface_audio int_aud;
		
		private NAudio.Wave.BlockAlignReductionStream stream = null;
		private NAudio.Wave.DirectSoundOut output = null;
		private CustomWaveViewer customWaveViewer;
		
		private bool fileOpen;
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
		public CustomWaveViewer WaveViewer
		{
			// TODO : Thread this
			get
			{
				if (customWaveViewer == null) BuildCWV();
				return customWaveViewer;
			}
		}
		public NAudio.Wave.DirectSoundOut Output
		{
			get { return output; }
			set { output = value; }
		}
		public Album Album
		{
			get { return album; }
			set { album = value; }
		}
		public string Title
		{
			get { return title; }
			set { title = value; }
		}
		public string Year
		{
			get { return year; }
			set { year = value; }
		}
		public bool FileOpened
		{
			get { return fileOpen; }
		}
		public string FileName
		{
			get	{ return FName;	}
		}
		public string Path_track
		{
			get { return path_track; }
			set { path_track = value; }
		}
		public string Classement
		{
			get { return classement; }
			set { classement = value; }
		}
		public string Style
		{
			get { return style; }
			set { style = value; }
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
				if (fileOpen && Playing && !Paused)
				{
					if (Looping)
					{
						Pcommand = String.Format("play {0} notify repeat", Phandle);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1905 ] error number : " + Err.ToString());
					}
					else
					{
						Pcommand = String.Format("play {0} notify", Phandle);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1906 ] error number : " + Err.ToString());
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
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1907 ] error number : " + Err.ToString());
				}
			}
		}
		public ulong AudioLength
		{
			get
			{
				if (fileOpen)
					return Lng;
				else
					return 0;
			}
		}
		public ulong CurrentPosition
		{
			get
			{
				if (fileOpen && Playing)
				{
					StringBuilder s = new StringBuilder(128);
					Pcommand = String.Format("status {0} position", Phandle);
					if ((Err = mciSendString(Pcommand, s, 128, IntPtr.Zero)) != 0) Log.write("[ DEB : 1904 ] error number : " + Err.ToString());
					ulong position = Convert.ToUInt64(s.ToString());
					return position;
				}
				else
					return 0;
			}
		}
		#endregion
		
		#region Constructor
		public Track(string the_title, Album alb, Interface_audio ia)
		{
			int_aud = ia;
			fileOpen = true;
			album = alb;
			path_track = the_title;
			title = the_title.Split('\\')[the_title.Split('\\').Length - 1];
			album.Add(this);
			
			InitializeEnvironment();
		}
		#endregion
		
		#region Methods public
		public void Close()
		{
			Stop();
			DisposeWave();
			try
			{
				if (fileOpen)
				{
					FName = string.Empty;
					fileOpen = false;
					Playing = false;
					Paused = false;
				}
			}
			catch (Exception exp1901)
			{
				Log.write("[ ERR : 1901 ] " + exp1901.Message);
			}
		}
		
		public void Play()
		{
			if (!fileOpen)
			{
				fileOpen = BuidReader();
			}
			else
			{
				if (fileOpen)
				{
					if (output != null)
					{
						output.Play();
						IsPaused = false;
					}
					else
					{
						if (!Playing)
						{
							Playing = true;
							Pcommand = String.Format("play {0}{1} notify", Phandle, Looping ? " repeat" : string.Empty);
							if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1912 ] error number : " + Err.ToString());
						}
						else
						{
							
							if (!Paused)
							{
								Paused = true;
								Pcommand = String.Format("pause {0}", Phandle);
								if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1918 ] error number : " + Err.ToString());
							}
							else
							{
								Paused = false;
								Pcommand = String.Format("play {0}{1} notify", Phandle, Looping ? " repeat" : string.Empty);
								if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1919 ] error number : " + Err.ToString());
							}
						}
					}
				}
			}
		}
		
		public void PlayPause()
		{
			if (!fileOpen)
			{
				fileOpen = BuidReader();
			}
			else
			{
				if (output != null)
				{
					if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing)
					{
						output.Pause();
						IsPaused = true;
					}
					else if (output.PlaybackState == NAudio.Wave.PlaybackState.Paused || output.PlaybackState == NAudio.Wave.PlaybackState.Stopped)
					{
						output.Play();
						IsPaused = false;
					}
				}
				else
				{
					if (!Paused)
					{
						Paused = true;
						Pcommand = String.Format("pause {0}", Phandle);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1918 ] error number : " + Err.ToString());
					}
					else
					{
						Paused = false;
						Pcommand = String.Format("play {0}{1} notify", Phandle, Looping ? " repeat" : string.Empty);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1919 ] error number : " + Err.ToString());
					}
				}
			}
		}
		
		public void Stop()
		{
			if (fileOpen)
			{
				if (output != null)
				{
					output.Stop();
				}
				else
				{
					Playing = false;
					Paused = false;
					Pcommand = String.Format("seek {0} to start", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1916 ] error number : " + Err.ToString());
					Pcommand = String.Format("stop {0}", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1917 ] error number : " + Err.ToString());
				}
			}
		}
		#endregion
		
		#region Methods private
		private bool BuidReader()
		{
			bool built = false;
			if (Path_track.EndsWith(".mp3"))
			{
				built =openNaMp3();
			}
			else if (Path_track.EndsWith(".wav"))
			{
				built =openNaWav();
			}
			else
			{
				Log.write("[ DEB : 4602 ] Cannot open with naudio method.");
			}
			
			if (!built)
			{
				built = openMCI();
			}
			
			if (built) int_aud.Disposed += new delegateInterfaceMusic(int_aud_Disposed);
			
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
				if (!fileOpen)
				{
					Pcommand = String.Format("open \"" + Path_track + "\" type mpegvideo alias {0}", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1908 ] error number : " + Err.ToString());
					FName = int_aud.PathFile;
					Playing = false;
					Paused = false;
					Pcommand = String.Format("set {0} time format milliseconds", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1909 ] error number : " + Err.ToString());
					Pcommand = String.Format("set {0} seek exactly on", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1910 ] error number : " + Err.ToString());
					if (!CalculateLength())
					{
						int_aud.Tsm.Dispose(null);
						Log.write("[ DEB : 1904] cannot open music file");
						return false;
					}
					
					fileOpen = true;
					return true;
				}
				else
				{
					Log.write("[ DEB : 1903] cannot open music file");
					return false;
				}
			}
			catch (Exception exp1902)
			{
				int_aud.Tsm.Dispose(null);
				Log.write("[ ERR : 1902] cannot open music file.\n" + exp1902.Message);
				return false;
			}
		}
		
		private bool openNaMp3()
		{
			try
			{
				NAudio.Wave.WaveStream pcm = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(new NAudio.Wave.Mp3FileReader(Path_track));
				stream = new NAudio.Wave.BlockAlignReductionStream(pcm);
				output = new NAudio.Wave.DirectSoundOut();
				output.Init(stream);
				return true;
			}
			catch (Exception)
			{
				Log.write("[ DEB : 4600 ] Cannot open wav file with naudio.");
				return false;
			}
		}
		
		private bool openNaWav()
		{
			try
			{
				NAudio.Wave.WaveStream pcm = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(Path_track));
				stream = new NAudio.Wave.BlockAlignReductionStream(pcm);
				output = new NAudio.Wave.DirectSoundOut();
				output.Init(stream);
				return true;
			}
			catch (Exception)
			{
				Log.write("[ DEB : 4601 ] Cannot open wav file with naudio.");
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
					Log.write("[ WRN : 1907 ] Error on the length calcul in audio file");
				}
				Lng = Convert.ToUInt64(str.ToString());
				
				return true;
			}
			catch (Exception exp1906)
			{
				Log.write("[ WRN : 1906 ] Cannot calculate the length of the audio file.\n" + exp1906.Message);
				return true;
				//return false; // TODO : that bad guys !!!
			}
		}
		
		private void Seek(ulong milliseconds)
		{
			if (fileOpen && milliseconds <= Lng)
			{
				if (Playing)
				{
					if (Paused)
					{
						Pcommand = String.Format("seek {0} to {1}", Phandle, milliseconds);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1920 ] error number : " + Err.ToString());
					}
					else
					{
						Pcommand = String.Format("seek {0} to {1}", Phandle, milliseconds);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1921 ] error number : " + Err.ToString());
						Pcommand = String.Format("play {0}{1} notify", Phandle, Looping ? " repeat" : string.Empty);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1922 ] error number : " + Err.ToString());
					}
				}
			}
		}
		
		private void BuildCWV()
		{
			try
			{
				using (Mp3FileReader mp3 = new Mp3FileReader(path_track))
				{
					using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
					{
						path_track_viewer = Environment.CurrentDirectory + @"Music\waveviewer.wav";
						WaveFileWriter.CreateWaveFile(path_track_viewer, pcm);
					}
				}
				
				customWaveViewer = new CustomWaveViewer();
				customWaveViewer.Dock = System.Windows.Forms.DockStyle.Fill;
				customWaveViewer.Name = "customWaveViewer";
				customWaveViewer.SamplesPerPixel = 128;
				customWaveViewer.Size = new System.Drawing.Size(564, 187);
				customWaveViewer.StartPosition = ((long)(0));
				customWaveViewer.TabIndex = 1;
				customWaveViewer.PenColor = Color.Gray;
				
				customWaveViewer.WaveStream = new NAudio.Wave.WaveFileReader(path_track_viewer);
				customWaveViewer.FitToScreen();
				
				//OnWaveViewerBuilt(this, null);
			}
			catch (Exception exp4603)
			{
				Log.write("[ DEB : 4603 ] Error when building the wave viewer.\n" + exp4603.Message);
			}
		}
		
		private void AnalysePath()
		{
			// allow you to analyse the path and take some informations (year, artist, ...)
		}
		
		private void DisposeWave()
		{
			if (output != null)
			{
				if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing) output.Stop();
				output.Dispose();
				output = null;
			}
			if (stream != null)
			{
				stream.Dispose();
				stream = null;
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
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1923 ] error number : " + Err.ToString());
						Pcommand = string.Format("setaudio {0} right volume to {1:#}", Phandle, (1000 + value) * vPct);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1924 ] error number : " + Err.ToString());
					}
					else
					{
						Pcommand = string.Format("setaudio {0} right volume to {1:#}", Phandle, aVolume);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1925 ] error number : " + Err.ToString());
						Pcommand = string.Format("setaudio {0} left volume to {1:#}", Phandle, (1000 - value) * vPct);
						if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1926 ] error number : " + Err.ToString());
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
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1927 ] error number : " + Err.ToString());
				}
				else
				{
					Pcommand = String.Format("setaudio {0} on", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1928 ] error number : " + Err.ToString());
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
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1929 ] error number : " + Err.ToString());
				}
				else
				{
					Pcommand = String.Format("setaudio {0} left on", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1930 ] error number : " + Err.ToString());
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
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1931 ] error number : " + Err.ToString());
				}
				else
				{
					Pcommand = String.Format("setaudio {0} right on", Phandle);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1932 ] error number : " + Err.ToString());
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
				if (fileOpen && (value >= 0 && value <= 1000))
				{
					aVolume = value;
					Pcommand = String.Format("setaudio {0} volume to {1}", Phandle, aVolume);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1933 ] error number : " + Err.ToString());
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
				if (fileOpen && (value >= 0 && value <= 1000))
				{
					bVolume = value;
					Pcommand = String.Format("setaudio {0} bass to {1}", Phandle, bVolume);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1934 ] error number : " + Err.ToString());
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
				if (fileOpen && (value >= 0 && value <= 1000))
				{
					lVolume = value;
					Pcommand = String.Format("setaudio {0} left volume to {1}", Phandle, lVolume);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1935 ] error number : " + Err.ToString());
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
				if (fileOpen && (value >= 0 && value <= 1000))
				{
					rVolume = value;
					Pcommand = String.Format("setaudio {0} right volume to {1}", Phandle, rVolume);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1935 ] error number : " + Err.ToString());
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
				if (fileOpen && (value >= 0 && value <= 1000))
				{
					tVolume = value;
					Pcommand = String.Format("setaudio {0} treble to {1}", Phandle, tVolume);
					if ((Err = mciSendString(Pcommand, null, 0, IntPtr.Zero)) != 0) Log.write("[ DEB : 1936 ] error number : " + Err.ToString());
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
