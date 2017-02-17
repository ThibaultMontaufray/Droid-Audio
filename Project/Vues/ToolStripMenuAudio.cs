// Log code 41

using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Tools4Libraries;

namespace Droid_Audio
{
	public class ToolStripMenuAudio : RibbonTab
    {
        #region Attributes
        public event EventHandlerAction ActionAppened;

        private GUI _gui;
		private RibbonPanel _panelTools;
        private RibbonButton _rb_refreshLibrary;
        private RibbonButton _rb_equalizer;
        private RibbonButton _rb_import;

        private RibbonPanel _panelModification;
        private RibbonButton _rb_convert;
        private RibbonButton _rb_convert_wav_mp3;
        private RibbonButton _rb_convert_mp3_wav;
        private RibbonButton _rb_convert_mp4_mp3;
        private RibbonButton _rb_convert_mp4_flac;

        private RibbonPanel _panelDefault;
        private RibbonLabel _lbl_title;
        private RibbonLabel _lbl_artist;
        private RibbonLabel _lbl_album;
        private RibbonLabel _lbl_folder;
        private RibbonLabel _lbl_year;
        private RibbonLabel _lbl_type;

        private RibbonPanel _panelDownload;
        private RibbonButton _rb_youtube;
        #endregion

        #region Properties
        public GUI Gui
		{
			get { return _gui; }
		}
		#endregion
		
		#region Constructor
		public ToolStripMenuAudio(List<string> theList)
		{
			try
			{
				_gui = new GUI();

                BuildPanelTools();
                BuildPanelConvert();
                BuildPanelDownload();

                this.Text = "Music";
			}
			catch (Exception exp4100)
			{
				Log.Write("[ CRT : 4100 ] Cannot open audio menu.\n" + exp4100.Message);
				this.Dispose(null);
			}
		}
		#endregion
		
		#region Methods public
		public void RefreshComponent(List<string> ListComponents)
		{
			// nothing to do for this kind of file
			// everything is allow always
		}
		public void Dispose(List<string> theList)
		{
            this.Dispose();
			//theList.Remove("manager_audio_" + CurrentTabPage.Text);
		}
        public void UpdateTrack(Track currentTrack)
        {
            _lbl_title.Text = "Title : " + currentTrack.Title;
            _lbl_album.Text = "Album : " + currentTrack.Albums;
            _lbl_artist.Text = "Artist : ";
            _lbl_folder.Text = "Folder : " + currentTrack.Path_track.Split('\\')[currentTrack.Path_track.Split('\\').Length - 1];
            _lbl_year.Text = "Year : ";
            _lbl_type.Text = "Type : ";

        }
        #endregion

        #region Methods private
        private void BuildPanelTools()
        {
            _rb_refreshLibrary = new RibbonButton("Refresh library");
            _rb_refreshLibrary.Click += new EventHandler(tsb_refreshLib_Click);
            _rb_refreshLibrary.Image = Tools4Libraries.Resources.ResourceIconSet32Default.zoom_refresh;

            _rb_equalizer = new RibbonButton("Equalizer");
            _rb_equalizer.Click += new EventHandler(tsb_equlizer_Click);
            _rb_equalizer.Image = Tools4Libraries.Resources.ResourceIconSet32Default.control_equalizer;
            _rb_equalizer.Enabled = false;

            _rb_import = new RibbonButton("Import");
            _rb_import.Click += new EventHandler(tsb_import_Click);
            _rb_import.Image = Tools4Libraries.Resources.ResourceIconSet32Default.bookshelf;

            _panelTools = new RibbonPanel();
            _panelTools.Text = "Tools";
            _panelTools.Items.Add(_rb_refreshLibrary);
            _panelTools.Items.Add(_rb_equalizer);
            _panelTools.Items.Add(_rb_import);
            this.Panels.Add(_panelTools);
        }
        private void BuildPanelConvert()
        {
            _rb_convert_mp3_wav = new RibbonButton("MP3 -> WAV");
            _rb_convert_mp3_wav.Click += _rb_convert_mp3_wav_Click;
            _rb_convert_mp3_wav.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.file_extension_wav;

            _rb_convert_wav_mp3 = new RibbonButton("WAV -> MP3");
            _rb_convert_wav_mp3.Click += _rb_convert_wav_mp3_Click;
            _rb_convert_wav_mp3.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.file_extension_wav;

            _rb_convert_mp4_mp3 = new RibbonButton("MP4 -> MP3");
            _rb_convert_mp4_mp3.Click += _rb_convert_mp4_mp3_Click;
            _rb_convert_mp4_mp3.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.file_extension_mp4;

            _rb_convert_mp4_flac = new RibbonButton("MP4 -> FLAC");
            _rb_convert_mp4_flac.Click += _rb_convert_mp4_flac_Click;
            _rb_convert_mp4_flac.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.file_extension_mp4;

            _rb_convert = new RibbonButton("Convert");
            _rb_convert.Image = Tools4Libraries.Resources.ResourceIconSet32Default.arrow_switch;
            _rb_convert.Style = RibbonButtonStyle.DropDown;
            _rb_convert.DropDownItems.Add(_rb_convert_mp3_wav);
            _rb_convert.DropDownItems.Add(_rb_convert_wav_mp3);
            _rb_convert.DropDownItems.Add(_rb_convert_mp4_mp3);
            _rb_convert.DropDownItems.Add(_rb_convert_mp4_flac);

            _panelModification = new RibbonPanel();
            _panelModification.Text = "Modification";
            _panelModification.Items.Add(_rb_convert);
            this.Panels.Add(_panelModification);
        }
        private void BuildPanelDetails()
        {
            _lbl_title = new RibbonLabel();
            _lbl_title.Text = "Title : ";

            _lbl_album = new RibbonLabel();
            _lbl_album.Text = "Album : ";

            _lbl_artist = new RibbonLabel();
            _lbl_artist.Text = "Artist : ";

            _lbl_folder = new RibbonLabel();
            _lbl_folder.Text = "Folder : ";

            _lbl_year = new RibbonLabel();
            _lbl_year.Text = "Year : ";

            _lbl_type = new RibbonLabel();
            _lbl_type.Text = "Type : ";

            _panelDefault = new RibbonPanel();
            _panelDefault.Text = "Details";
            _panelDefault.Items.Add(_lbl_title);
            _panelDefault.Items.Add(_lbl_album);
            _panelDefault.Items.Add(_lbl_artist);
            _panelDefault.Items.Add(_lbl_folder);
            _panelDefault.Items.Add(_lbl_year);
            _panelDefault.Items.Add(_lbl_type);
            this.Panels.Add(_panelDefault);
        }
        private void BuildPanelDownload()
        {
            _rb_youtube = new RibbonButton("YouTube");
            _rb_youtube.Click += new EventHandler(rb_youtube_Click);
            _rb_youtube.Image = Tools4Libraries.Resources.ResourceIconSet32Default.youtube;
            _rb_youtube.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.youtube;
            
            _panelDownload = new RibbonPanel();
            _panelDownload.Text = "Download";
            _panelDownload.Items.Add(_rb_youtube);
            this.Panels.Add(_panelDownload);
        }
        #endregion

        #region Events
        public void OnAction(EventArgs e)
        {
            if (ActionAppened != null) ActionAppened(this, e);
        }
        public void rb_youtube_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("downloadyoutube");
            OnAction(action);
        }
        public void tsb_refreshLib_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("refreshLib");
            OnAction(action);
        }
        public void tsb_equlizer_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("equalizing");
            OnAction(action);
        }
        public void tsb_import_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("import");
            OnAction(action);
        }
        private void _rb_convert_mp3_wav_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("mp3towav");
            OnAction(action);
        }
        private void _rb_convert_wav_mp3_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("wavtomp3");
            OnAction(action);
        }
        private void _rb_convert_mp4_mp3_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("mp4tomp3");
            OnAction(action);
        }
        private void _rb_convert_mp4_flac_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("mp4toflac");
            OnAction(action);
        }
        #endregion
    }
}
