// Log code : 23 - 28

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Tools4Libraries;
using System.Threading.Tasks;
using System.Linq;

namespace Droid_Audio
{
	public delegate void delegateMusic(object sender, EventArgs e);
	
	public class PanelAudio : Panel
	{
        #region Enum
        public enum Presentation
        {
            ALBUM,
            ARTIST,
            FOLDER,
            GENRE,
            TRACKS
        }
        #endregion

        #region Attribute
        private Interface_audio _intAud;
		private int index_fiche_selected;
        private Presentation _currentPresentation = Presentation.GENRE;

        private Button button_pp;
		private Button button_stop;
		private Button button_rewind;
		private Button button_forward;
		private Button button_back;
		private Button button_next;
		private Button button_vol;
		private Button button_eject;
		private Button button_loop;
		
		private Panel panel_left;
		private Panel panel_middle;
		
		private Panel panelControlLeft;
		private Panel panelControlMiddle;
		
		private Label label_current_selection;
        private Label _labelSeparation;
        private Panel panel_current_selection;
		private Panel panel_navigation;
        private RichListView panel_list_to_play;

        private List<RichListViewItem> listPfaToPlay;
        private Button _menuFolder;
        private Button _menuArtist;
        private Button _menuAlbum;
        private Button _menuType;

        private Presentation displayMode;
		private int _index_X;
		private int _index_Y;
		private bool succeed;
		private string[] alpha = { "_", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
		private List<string> listAlpha;
		
		public event delegateMusic TicketSelectedChange;
		public event delegateMusic TicketPP;
        public event delegateMusic TicketClose;
        #endregion

        #region Properties
        public RichListViewItem CurrentPlayedTrack
		{
			get { return listPfaToPlay[index_fiche_selected]; }
		}
		public bool OpenedCurrentFile
		{
			get { return (listPfaToPlay[index_fiche_selected].AssociatedObject as Track).FileOpened; }
		}
		private ToolStripMenuAudio tsm
		{
			get { return _intAud.Tsm as ToolStripMenuAudio; }
		}
		public Presentation CurrentPresentation
        {
            get { return _currentPresentation; }
            set { _currentPresentation = value; }
        }
        #endregion

        #region Constructor
        public PanelAudio(Interface_audio inter_aud)
		{
			displayMode = Presentation.GENRE;
			_intAud = inter_aud;
			listPfaToPlay = new List<RichListViewItem>();
			InitializeComponent();
			ButtonsDisable();

            _menuType_Click(null, null);
        }
		#endregion
		
		#region Methods public
		public new void Resize()
		{
            panelControlMiddle.Width = panel_middle.Width;
            panelControlMiddle.Height = panel_middle.Height - 150;

            _index_X = 5;
            _index_Y = 5;
            
            foreach (Control ctrl in panelControlMiddle.Controls)
            {
                if (_index_X > panelControlMiddle.Width - 120)
                {
                    _index_X = 5;
                    _index_Y += 145;
                }
                if (ctrl is Panel)
                {
                    (ctrl as Panel).Top = _index_Y;
                    (ctrl as Panel).Left = _index_X;
                    _index_X += (ctrl as Panel).Width + 5;
                }
                else if (ctrl is Label)
                {
                    if ("rowletter".Equals(ctrl.Name))
                    {
                        if (_index_Y > 7) { _index_Y += 150; }
                        _index_X = 5;
                        (ctrl as Label).Top = _index_Y + 10;
                        (ctrl as Label).Left = _index_X;
                        _index_Y += 20;
                    }
                    else if ("rowseparation".Equals(ctrl.Name))
                    {
                        _index_X = 5;
                        (ctrl as Label).Top = _index_Y;
                        (ctrl as Label).Left = _index_X;
                        _index_X = 5;
                        _index_Y += 18;
                    }
                }
            }

            panelControlLeft.Width = panel_left.Width;
            panelControlLeft.Height = panel_left.Height - 150;
			
			int dist = (panel_current_selection.Width / 2) + 100;
			button_eject.Left = panel_current_selection.Width - dist;
			button_back.Left = panel_current_selection.Width - dist + 40;
			button_rewind.Left = panel_current_selection.Width - dist + 60;
			button_stop.Left = panel_current_selection.Width - dist + 80;
			button_pp.Left = panel_current_selection.Width - dist + 100;
			button_forward.Left = panel_current_selection.Width - dist + 120;
			button_next.Left = panel_current_selection.Width - dist + 140;
			button_vol.Left = panel_current_selection.Width - dist + 180;
			button_loop.Left = panel_current_selection.Width - dist + 200;

            if (_labelSeparation != null)
            {
                _labelSeparation.Text = string.Empty;
                for (int i = 0; i < panelControlMiddle.Width / 6; i++) _labelSeparation.Text += "_";
            }
        }
		public void RefreshPanelLibrary()
        {
            try
            {
                listAlpha = new List<string>(alpha);
                panelControlMiddle.Controls.Clear();
                _index_Y = 5;

                foreach (string s in listAlpha)
                {
                    succeed = false;
                    _index_X = 5;
                    
                    switch (_currentPresentation)
                    {
                        case Presentation.ALBUM:
                            CreateLetterAlbum(s);
                            break;
                        case Presentation.ARTIST:
                            CreateLetterArtist(s);
                            break;
                        case Presentation.FOLDER:
                            CreateLetterFolder(s);
                            break;
                        case Presentation.GENRE:
                            CreateLetterType();
                            panelControlMiddle.Invalidate();
                            return;
                        default:
                            Log.Write("[ ERR : 2320 ] Error, cannot make the filter with you're option : " + displayMode);
                            break;
                    }
                    if (succeed)
                    {
                        _index_Y += 139;
                    }
                }
                panelControlMiddle.Invalidate();
            }
            catch (Exception exp2340)
            {
                Log.Write("[ ERR : 2340 ] Cannot load music.\n" + exp2340.Message);
            }
        }
        public void TitleChanged()
		{
			if(index_fiche_selected<0 || string.IsNullOrEmpty((listPfaToPlay[index_fiche_selected].AssociatedObject as Track).Title))
			{
				label_current_selection.Text = "No title selected.";
				panel_navigation.Controls.Clear();
			}
			else
			{
				Cursor = System.Windows.Forms.Cursors.WaitCursor;
				label_current_selection.Text = (listPfaToPlay[index_fiche_selected].AssociatedObject as Track).Title + " - " + (listPfaToPlay[index_fiche_selected].AssociatedObject as Track).Albums;
				
				panel_navigation.Controls.Clear();
				try
				{
					panel_navigation.Controls.Add((listPfaToPlay[index_fiche_selected].AssociatedObject as Track).WaveViewer);
				}
				catch (Exception exp2345)
				{
					Log.Write("[ DEB : 2345 ] Cannot load wave viewer.\n" + exp2345.Message);
				}
//				if (listPfaToPlay[index_fiche_selected].TrackLinked.WaveViewer != null) panel_navigation.Controls.Add(listPfaToPlay[index_fiche_selected].TrackLinked.WaveViewer);
//				else
//				{
//					listPfaToPlay[index_fiche_selected].TrackLinked.AddWaveViewer(panel_navigation);
//					listPfaToPlay[index_fiche_selected].TrackLinked.WaveViewerBuilt += new delegateMusicTicket(PanelAudio_WaveViewerBuilt);
//				}
				panel_navigation.Invalidate();
				Cursor = System.Windows.Forms.Cursors.Default;
			}
		}
		public void ButtonsEnable()
		{
			button_pp.Enabled = true;
			button_stop.Enabled = true;
			button_rewind.Enabled = false;
			button_forward.Enabled = false;
			button_back.Enabled = false;
			button_next.Enabled = false;
			button_vol.Enabled = false;
			button_eject.Enabled = true;
			button_loop.Enabled = false;
		}
		public void ButtonsDisable()
		{
			button_pp.Enabled = false;
			button_stop.Enabled = false;
			button_rewind.Enabled = false;
			button_forward.Enabled = false;
			button_back.Enabled = false;
			button_next.Enabled = false;
			button_vol.Enabled = false;
			button_eject.Enabled = false;
			button_loop.Enabled = false;
		}
		public bool AddTicket(Interface_audio inter_aud, string filePath)
		{
			return LoadTicket(inter_aud, inter_aud.GetTrack(filePath));
		}
        #endregion

        #region Methods private
        //private async void RefreshTitle(List<Track> tracks)
        //{
        //    _index_Y = 5;

        //    List<string> albums = new List<string>();
        //    foreach (Track track in tracks)
        //    {
        //        if (!albums.Contains(track.AlbumName)) albums.Add(track.AlbumName);
        //    }

        //    panelControlMiddle.Controls.Clear();

        //    Task<bool> taskTile = Task.Run(() => BuildTiles());
        //    bool completed = await taskTile;

        //    panelControlMiddle.Invalidate();
        //}
        private void RefreshTitle(PanelMusicTile musicTile)
        {
            _index_Y = 5;
            panelControlMiddle.Controls.Clear();

            switch (musicTile.KindOfTicket)
            {
                case PanelMusicTile.Ticket.ALBUM:
                    _currentPresentation = Presentation.TRACKS;
                    _intAud.ListTrackFiltered = _intAud.ListTrack.Where(t => !string.IsNullOrEmpty(t.AlbumName) && musicTile.TicketAlbums[0].ToLower().Equals(t.AlbumName.ToLower())).ToList();
                    break;
                case PanelMusicTile.Ticket.ARTIST:
                    _currentPresentation = Presentation.ALBUM;
                    _intAud.ListTrackFiltered = _intAud.ListTrack.Where(t => !string.IsNullOrEmpty(t.ArtistName) && musicTile.TicketArtist[0].ToLower().Equals(t.ArtistName.ToLower())).ToList();
                    break;
                case PanelMusicTile.Ticket.FOLDER:
                    _currentPresentation = Presentation.ALBUM;
                    _intAud.ListTrackFiltered = _intAud.ListTrack.Where(t => !string.IsNullOrEmpty(t.Path_track) && musicTile.TicketFolders.Contains(t.Path_track)).ToList();
                    break;
                case PanelMusicTile.Ticket.GENRE:
                    _currentPresentation = Presentation.ALBUM;
                    _intAud.ListTrackFiltered = _intAud.ListTrack.Where(t => t.Genre != null && t.Genre.Count > 0 && musicTile.Genre != null && !string.IsNullOrEmpty(t.Genre[0]) && musicTile.Genre.Equals(t.Genre[0].ToLower())).ToList();
                    break;
                default:
                    _currentPresentation = Presentation.ALBUM;
                    break;
            }

            BuildTiles();
            //Task<bool> taskTile = Task.Run(() => BuildTiles());
            //bool completed = await taskTile;

            panelControlMiddle.Invalidate();
        }
        private bool BuildTiles()
        {
            bool succeed = false;
            
            switch (_currentPresentation)
            {
                case Presentation.ALBUM:
                    BuildTilesAlbum();
                    break;
                case Presentation.ARTIST:
                    BuildTilesArtist();
                    break;
                case Presentation.FOLDER:
                    BuildTilesFolder();
                    break;
                case Presentation.GENRE:
                    BuildTilesGenre();
                    break;
                default:
                    break;
            }
            return succeed;
        }
        private void BuildTilesAlbum()
        {
            int refvalue;
            List<string> albums = new List<string>();

            foreach (Track track in _intAud.ListTrackFiltered)
            {
                if (!albums.Contains(track.AlbumName) && track.AlbumName != null)
                {
                    albums.Add(track.AlbumName);
                }
            }
            albums.Sort();
            foreach (string s in listAlpha)
            {
                refvalue = _index_Y;
                succeed = false;
                foreach (string alb in albums)
                {
                    try
                    {
                        if (alb != null && alb.ToLower().StartsWith(s))
                        {
                            _index_X = 5;

                            BuildLabelTitleSortAlpha(alb);
                            BuildAblumDetailsView(_intAud.ListTrackFiltered.Where(t => !string.IsNullOrEmpty(t.AlbumName) && t.AlbumName.Equals(alb)).ToList());
                            succeed = true;
                        }
                    }
                    catch (Exception exp)
                    {
                        Log.Write("[ ERR : 2325 ] Cannot create album tile.\n\n" + exp.Message);
                    }
                }
                if (succeed && (_index_Y < refvalue + 139))
                {
                    _index_Y = refvalue + 139;
                }
            }
        }
        private void BuildTilesArtist()
        {
            int refvalue;
            List<string> artists = new List<string>();

            foreach (Track track in _intAud.ListTrackFiltered)
            {
                if (!artists.Contains(track.ArtistName) && track.ArtistName != null)
                {
                    artists.Add(track.ArtistName);
                }
            }
            artists.Sort();
            foreach (string s in listAlpha)
            {
                refvalue = _index_Y;
                succeed = false;
                foreach (string artist in artists)
                {
                    try
                    {
                        if (artist != null && artist.ToLower().StartsWith(s))
                        {
                            _index_X = 5;

                            BuildLabelTitleSortAlpha(artist);
                            BuildAblumDetailsView(_intAud.ListTrackFiltered.Where(t => !string.IsNullOrEmpty(t.ArtistName) && t.AlbumName.Equals(artist)).ToList());
                            succeed = true;
                        }
                    }
                    catch (Exception exp)
                    {
                        Log.Write("[ ERR : 2326 ] Cannot create album artist.\n\n" + exp.Message);
                    }
                }
                if (succeed && (_index_Y < refvalue + 139))
                {
                    _index_Y = refvalue + 139;
                }
            }
        }
        private void BuildTilesFolder()
        {
            int refvalue;
            List<string> folders = new List<string>();

            foreach (Track track in _intAud.ListTrackFiltered)
            {
                if (!folders.Contains(track.Path_track) && track.Path_track != null)
                {
                    folders.Add(track.Path_track);
                }
            }
            folders.Sort();
            foreach (string s in listAlpha)
            {
                refvalue = _index_Y;
                succeed = false;
                foreach (string folder in folders)
                {
                    if (folder != null && folder.ToLower().StartsWith(s))
                    {
                        try
                        {
                            _index_X = 5;

                            BuildLabelTitleSortAlpha(folder);
                            BuildAblumDetailsView(_intAud.ListTrackFiltered.Where(t => !string.IsNullOrEmpty(t.Path_track) && t.Path_track.Equals(folder)).ToList());
                            succeed = true;
                        }
                        catch (Exception exp)
                        {
                            Log.Write("[ ERR : 2327 ] Cannot create folder tile.\n\n" + exp.Message);
                        }
                    }
                }
                if (succeed && (_index_Y < refvalue + 139))
                {
                    _index_Y = refvalue + 139;
                }
            }
        }
        private void BuildTilesGenre()
        {
            int refvalue;
            List<string> genres = new List<string>();

            foreach (Track track in _intAud.ListTrackFiltered)
            {
                if (track.Genre != null && track.Genre.Count > 0 && !genres.Contains(track.Genre[0]) && track.Genre[0] != null)
                {
                    genres.Add(track.Genre[0]);
                }
            }
            genres.Sort();
            foreach (string s in listAlpha)
            {
                refvalue = _index_Y;
                succeed = false;
                foreach (string genre in genres)
                {
                    if (genre != null && genre.ToLower().StartsWith(s))
                    {
                        try
                        {
                            _index_X = 5;

                            BuildLabelTitleSortAlpha(genre);
                            BuildAblumDetailsView(_intAud.ListTrackFiltered.Where(t => t.Genre != null && t.Genre.Count > 0 && t.Genre[0].Equals(genre)).ToList());
                            succeed = true;
                        }
                        catch (Exception exp)
                        {
                            Log.Write("[ ERR : 2328 ] Cannot create genre tile.\n\n" + exp.Message);
                        }
                    }
                }
                if (succeed && (_index_Y < refvalue + 139))
                {
                    _index_Y = refvalue + 139;
                }
            }
        }

        private void InitializeComponent()
		{
			index_fiche_selected = 0;
			
			this.BackColor = Color.FromArgb(20, 20, 20);
			if (_intAud != null && _intAud.SheetMusic != null) this.Width = _intAud.SheetMusic.Width;
			this.Height = _intAud.SheetMusic.Height;
			this.Dock = DockStyle.Fill;
            this.SizeChanged += PanelAudio_SizeChanged;
			
			BuildPanel_Middle();
			BuildPanel_Left();
			BuildPanel_navigation();
			BuildPanel_current_selection();
			BuildPanel_list_to_play();
		}
        private void BuildPanel_navigation()
		{
			panel_navigation = new Panel();
			panel_navigation.Height = 30;
			panel_navigation.Dock = DockStyle.Bottom;
			panel_navigation.BackColor = Color.FromArgb(20, 20, 20);
			this.Controls.Add(panel_navigation);
        }
        private void BuildPanel_list_to_play()
        {
            panel_list_to_play = new RichListView();
            panel_list_to_play.Height = 130;
            panel_list_to_play.Dock = DockStyle.Bottom;
            panel_list_to_play.BackColor = Color.FromArgb(20, 20, 20);
            panel_list_to_play.ComponentSize = RichListViewItem.Format.AUDIO;
            panel_list_to_play.Scroll += Panel_list_to_play_Scroll;
            this.Controls.Add(panel_list_to_play);
        }
        private void Panel_list_to_play_Scroll(object sender, ScrollEventArgs e)
        {
            panel_list_to_play.AutoScrollOffset = new Point(50, 0);
        }
        private void BuildPanel_current_selection()
		{
			panel_current_selection = new Panel();
			panel_current_selection.Height = 34;
			panel_current_selection.Dock = DockStyle.Bottom;
			panel_current_selection.BackColor = Color.Black;
			this.Controls.Add(panel_current_selection);
			
			label_current_selection = new Label();
			label_current_selection.Left = 10;
			label_current_selection.Top = 10;
			label_current_selection.Width = 150;
			label_current_selection.Text = "No music selected";
			label_current_selection.ForeColor = Color.WhiteSmoke;
			panel_current_selection.Controls.Add(label_current_selection);
			
			BuildButtons();
		}
		private void BuildPanel_Left()
		{
			panel_left = new Panel();
			panel_left.Dock = DockStyle.Left;
			panel_left.Width = 145;
			panel_left.BackColor = Color.Transparent;
            this.Controls.Add(panel_left);
			
			panelControlLeft = new Panel();
			panelControlLeft.BackColor = Color.FromArgb(255, 103, 103, 103);
			panelControlLeft.Top = 150;
			panelControlLeft.Left = 0;
			panelControlLeft.Width = panel_left.Width;
			panelControlLeft.Height = panel_left.Height + 20;
            panelControlLeft.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuBackGround")];
            panelControlLeft.BackgroundImageLayout = ImageLayout.Stretch;

            panel_left.Controls.Add(panelControlLeft);
			
            _menuAlbum = new Button();
            _menuAlbum.FlatStyle = FlatStyle.Flat;
            _menuAlbum.FlatAppearance.BorderSize = 0;
            _menuAlbum.Top = 5;
            _menuAlbum.Left = 5;
            _menuAlbum.Width = 140;
            _menuAlbum.Height = 32;
            _menuAlbum.FlatAppearance.MouseDownBackColor = Color.Transparent;
            _menuAlbum.FlatAppearance.MouseOverBackColor = Color.Transparent;
            _menuAlbum.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuAlbum")];
            _menuAlbum.BackColor = Color.Transparent;
            _menuAlbum.Click += _menuAlbum_Click;
            _menuAlbum.MouseHover += _menuAlbum_MouseHover;
            _menuAlbum.MouseLeave += _menuAlbum_MouseLeave;
            _menuAlbum.Text = "          Album";
            _menuAlbum.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _menuAlbum.ForeColor = Color.White;
            _menuAlbum.TextAlign = ContentAlignment.MiddleLeft;
            panelControlLeft.Controls.Add(_menuAlbum);

            _menuArtist = new Button();
            _menuArtist.FlatStyle = FlatStyle.Flat;
            _menuArtist.FlatAppearance.BorderSize = 0;
            _menuArtist.Top = 42;
            _menuArtist.Left = 5;
            _menuArtist.Width = 140;
            _menuArtist.Height = 32;
            _menuArtist.FlatAppearance.MouseDownBackColor = Color.Transparent;
            _menuArtist.FlatAppearance.MouseOverBackColor = Color.Transparent;
            _menuArtist.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuArtist")];
            _menuArtist.BackColor = Color.Transparent;
            _menuArtist.Click += _menuArtist_Click;
            _menuArtist.MouseHover += _menuArtist_MouseHover;
            _menuArtist.MouseLeave += _menuArtist_MouseLeave;
            _menuArtist.Text = "          Artist";
            _menuArtist.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _menuArtist.ForeColor = Color.White;
            _menuArtist.TextAlign = ContentAlignment.MiddleLeft;
            panelControlLeft.Controls.Add(_menuArtist);

            _menuFolder = new Button();
            _menuFolder.FlatStyle = FlatStyle.Flat;
            _menuFolder.FlatAppearance.BorderSize = 0;
            _menuFolder.Top = 79;
            _menuFolder.Left = 5;
            _menuFolder.Width = 140;
            _menuFolder.Height = 32;
            _menuFolder.FlatAppearance.MouseDownBackColor = Color.Transparent;
            _menuFolder.FlatAppearance.MouseOverBackColor = Color.Transparent;
            _menuFolder.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuFolder")];
            _menuFolder.BackColor = Color.Transparent;
            _menuFolder.Click += _menuFolder_Click;
            _menuFolder.MouseHover += _menuFolder_MouseHover;
            _menuFolder.MouseLeave += _menuFolder_MouseLeave;
            _menuFolder.Text = "          Folder";
            _menuFolder.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _menuFolder.ForeColor = Color.White;
            _menuFolder.TextAlign = ContentAlignment.MiddleLeft;
            panelControlLeft.Controls.Add(_menuFolder);

            _menuType = new Button();
            _menuType.FlatStyle = FlatStyle.Flat;
            _menuType.FlatAppearance.BorderSize = 0;
            _menuType.Top = 116;
            _menuType.Left = 5;
            _menuType.Width = 140;
            _menuType.Height = 32;
            _menuType.FlatAppearance.MouseDownBackColor = Color.Transparent;
            _menuType.FlatAppearance.MouseOverBackColor = Color.Transparent;
            _menuType.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuType")];
            _menuType.BackColor = Color.Transparent;
            _menuType.Click += _menuType_Click;
            _menuType.MouseHover += _menuType_MouseHover;
            _menuType.MouseLeave += _menuType_MouseLeave;
            _menuType.Text = "          Category";
            _menuType.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _menuType.ForeColor = Color.White;
            _menuType.TextAlign = ContentAlignment.MiddleLeft;
            panelControlLeft.Controls.Add(_menuType);
        }
        private void BuildPanel_Middle()
		{
			panel_middle = new Panel();
			panel_middle.Dock = DockStyle.Fill;
            panel_middle.BackColor = Color.Transparent;
            this.Controls.Add(panel_middle);
			
			panelControlMiddle = new Panel();
			panelControlMiddle.BackColor = Color.FromArgb(255, 81, 81, 81);
			panelControlMiddle.Top = 150;
			panelControlMiddle.Left = 0;
			panelControlMiddle.Width = panel_middle.Width - 100;
			panelControlMiddle.Height = panel_middle.Height - 50;
			panelControlMiddle.AutoScroll = true;
			panel_middle.Controls.Add(panelControlMiddle);
		}

        private bool BuildLabelTitleSortAlpha(string lettre)
		{
            Label labelAlpha = new Label();
			labelAlpha.Top = _index_Y + 10;
			labelAlpha.Left = _index_X;
            labelAlpha.Text = lettre;
            labelAlpha.Name = "rowletter";
            labelAlpha.ForeColor = Color.White;
            labelAlpha.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelAlpha.Width = panelControlMiddle.Width;
            labelAlpha.Height = 20;
            labelAlpha.AutoSize = false;
            labelAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            labelAlpha.TextAlign = ContentAlignment.MiddleLeft;
            panelControlMiddle.Controls.Add(labelAlpha);
            _index_Y += 20;

            _labelSeparation = new Label();
			_labelSeparation.Width = panelControlMiddle.Width;
			_labelSeparation.Height = 14;
            _labelSeparation.Name = "rowseparation";
            for (int i=0 ; i<panelControlMiddle.Width/6 ; i++) _labelSeparation.Text += "_";
			_labelSeparation.Top = _index_Y;
			_labelSeparation.Left = _index_X;
            _labelSeparation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            panelControlMiddle.Controls.Add(_labelSeparation);
            _index_Y += 18;
            return true;
        }
        private bool BuildArtistView(List<string> artists)
        {
            foreach (string artistName in artists)
            {
                if (_index_X > panelControlMiddle.Width - 120)
                {
                    _index_X = 5;
                    _index_Y += 145;
                }
                PanelMusicTile pa = new PanelMusicTile(_intAud, _intAud.ListTrack.Where(t => !string.IsNullOrEmpty(t.ArtistName) && t.ArtistName.Equals(artistName)).ToList(), PanelMusicTile.Ticket.ARTIST);
                pa.TicketClick += new delegatePanelMusicTile(pa_TicketClick);
                pa.Top = _index_Y;
                pa.Left = _index_X;
                panelControlMiddle.Controls.Add(pa);
                _index_X += pa.Width + 5;
            }
            return true;
        }
        private bool BuildFoldersView(List<string> folders)
        {
            foreach (string folderName in folders)
            {
                if (_index_X > panelControlMiddle.Width - 120)
                {
                    _index_X = 5;
                    _index_Y += 145;
                }
                PanelMusicTile pa = new PanelMusicTile(_intAud, _intAud.ListTrack.Where(t => !string.IsNullOrEmpty(t.Path_track) && t.Path_track.Contains(folderName)).ToList(), PanelMusicTile.Ticket.FOLDER);
                pa.TicketClick += new delegatePanelMusicTile(pa_TicketClick);
                pa.Top = _index_Y;
                pa.Left = _index_X;
                panelControlMiddle.Controls.Add(pa);
                _index_X += pa.Width + 5;
            }
            return true;
        }
        private bool BuildAlbumView(List<string> albums)
        {
            foreach (string albumName in albums)
            {
                if (_index_X > panelControlMiddle.Width - 120)
                {
                    _index_X = 5;
                    _index_Y += 145;
                }
                PanelMusicTile pa = new PanelMusicTile(_intAud, _intAud.ListTrack.Where(t => !string.IsNullOrEmpty(t.AlbumName) && t.AlbumName.Equals(albumName)).ToList(), PanelMusicTile.Ticket.ALBUM);
                pa.TicketClick += new delegatePanelMusicTile(pa_TicketClick);
                pa.Top = _index_Y;
                pa.Left = _index_X;
                panelControlMiddle.Controls.Add(pa);
                _index_X += pa.Width + 5;
            }
            return true;
        }
        private bool BuildGenreView(List<string> genres)
        {
            foreach (string genre in genres)
            {
                if (_index_X > panelControlMiddle.Width - 120)
                {
                    _index_X = 5;
                    _index_Y += 145;
                }
                PanelMusicTile pa = new PanelMusicTile(_intAud, genre, PanelMusicTile.Ticket.GENRE);
                pa.TicketClick += new delegatePanelMusicTile(pa_TicketClick);
                pa.Top = _index_Y;
                pa.Left = _index_X;
                panelControlMiddle.Controls.Add(pa);
                _index_X += pa.Width + 5;
            }
            return true;
        }
        private void BuildAblumDetailsView(List<Track> tracks)
		{
			int refindex = _index_Y;
			
			PanelMusicTile panelAlbumIcon = new PanelMusicTile(_intAud, tracks, PanelMusicTile.Ticket.ALBUM);
			panelAlbumIcon.TicketClick += new delegatePanelMusicTile(pa_TicketClick);
			panelAlbumIcon.Top = _index_Y;
			panelAlbumIcon.Left = _index_X;
            panelControlMiddle.Controls.Add(panelAlbumIcon);
			
			Label labelRowTrack;
			foreach (Track t in tracks)
			{
				labelRowTrack = new Label();
				labelRowTrack.Width = panelControlMiddle.Width - 150;
				labelRowTrack.BackColor = Color.Transparent;
                labelRowTrack.ForeColor = Color.White;
                labelRowTrack.Height = 16;
				labelRowTrack.Top = _index_Y;
				labelRowTrack.Left = _index_X + 110;
				labelRowTrack.Text = t.Title;
				labelRowTrack.Name = t.ArtistName + "|" + t.AlbumName + "|" + t.Path_track;
                labelRowTrack.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                labelRowTrack.MouseHover += new EventHandler(labelTrack_MouseHover);
				labelRowTrack.MouseLeave += new EventHandler(labelTrack_MouseLeave);
				labelRowTrack.MouseDoubleClick += new MouseEventHandler(labelTrack_MouseDoubleClick);
				panelControlMiddle.Controls.Add(labelRowTrack);
                _index_Y += 16;
			}
			
			if (_index_Y < refindex + panelAlbumIcon.Height) _index_Y = refindex + panelAlbumIcon.Height;
		}

		private void BuildButtons()
		{
			button_eject = new Button();
			button_eject.Top = 5;
			button_eject.Left = 0;
			button_eject.Width = 16;
			button_eject.Height = 16;
            button_eject.FlatStyle = FlatStyle.Flat;
            button_eject.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button_eject.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button_eject.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("eject")];
			button_eject.BackgroundImageLayout = ImageLayout.Stretch;
			button_eject.BackColor = Color.Transparent;
			button_eject.FlatAppearance.BorderSize = 0;
			button_eject.Click += new EventHandler(button_eject_Click);
			panel_current_selection.Controls.Add(button_eject);
			
			button_back = new Button();
			button_back.Top = 5;
			button_back.Left = 0;
			button_back.Width = 16;
			button_back.Height = 16;
			button_back.FlatStyle = FlatStyle.Flat;
            button_back.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button_back.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button_back.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("back")];
			button_back.BackgroundImageLayout = ImageLayout.Stretch;
			button_back.BackColor = Color.Transparent;
			button_back.FlatAppearance.BorderSize = 0;
			button_back.Click += new EventHandler(button_back_Click);
			panel_current_selection.Controls.Add(button_back);
			
			button_rewind = new Button();
			button_rewind.Top = 5;
			button_rewind.Left = 0;
			button_rewind.Width = 16;
			button_rewind.Height = 16;
			button_rewind.FlatStyle = FlatStyle.Flat;
            button_rewind.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button_rewind.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button_rewind.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("rewind")];
			button_rewind.BackgroundImageLayout = ImageLayout.Stretch;
			button_rewind.BackColor = Color.Transparent;
			button_rewind.FlatAppearance.BorderSize = 0;
			button_rewind.Click += new EventHandler(button_rewind_Click);
			panel_current_selection.Controls.Add(button_rewind);
			
			button_stop = new Button();
			button_stop.Top = 5;
			button_stop.Left = 0;
			button_stop.Width = 16;
			button_stop.Height = 16;
			button_stop.FlatStyle = FlatStyle.Flat;
            button_stop.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button_stop.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button_stop.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("stop")];
			button_stop.BackgroundImageLayout = ImageLayout.Stretch;
			button_stop.BackColor = Color.Transparent;
			button_stop.FlatAppearance.BorderSize = 0;
			button_stop.Click += new EventHandler(button_stop_Click);
			panel_current_selection.Controls.Add(button_stop);
			
			button_pp = new Button();
			button_pp.Top = 5;
			button_pp.Left = 0;
			button_pp.Width = 16;
			button_pp.Height = 16;
			button_pp.FlatStyle = FlatStyle.Flat;
            button_pp.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button_pp.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button_pp.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("play")];
			button_pp.BackgroundImageLayout = ImageLayout.Stretch;
			button_pp.BackColor = Color.Transparent;
			button_pp.FlatAppearance.BorderSize = 0;
			button_pp.Click += new EventHandler(button_pp_Click);
			panel_current_selection.Controls.Add(button_pp);
			
			button_forward = new Button();
			button_forward.Top = 5;
			button_forward.Left = 0;
			button_forward.Width = 16;
			button_forward.Height = 16;
			button_forward.FlatStyle = FlatStyle.Flat;
            button_forward.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button_forward.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button_forward.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("forward")];
			button_forward.BackgroundImageLayout = ImageLayout.Stretch;
			button_forward.BackColor = Color.Transparent;
			button_forward.FlatAppearance.BorderSize = 0;
			button_forward.Click += new EventHandler(button_forward_Click);
			panel_current_selection.Controls.Add(button_forward);
			
			button_next = new Button();
			button_next.Top = 5;
			button_next.Left = 0;
			button_next.Width = 16;
			button_next.Height = 16;
			button_next.FlatStyle = FlatStyle.Flat;
            button_next.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button_next.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button_next.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("next")];
			button_next.BackgroundImageLayout = ImageLayout.Stretch;
			button_next.BackColor = Color.Transparent;
			button_next.FlatAppearance.BorderSize = 0;
			button_next.Click += new EventHandler(button_next_Click);
			panel_current_selection.Controls.Add(button_next);
			
			button_vol = new Button();
			button_vol.Top = 5;
			button_vol.Left = 0;
			button_vol.Width = 16;
			button_vol.Height = 16;
			button_vol.FlatStyle = FlatStyle.Flat;
            button_vol.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button_vol.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button_vol.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("vol_0")];
			button_vol.BackgroundImageLayout = ImageLayout.Stretch;
			button_vol.BackColor = Color.Transparent;
			button_vol.FlatAppearance.BorderSize = 0;
			button_vol.Click += new EventHandler(button_vol_Click);
			panel_current_selection.Controls.Add(button_vol);
			
			button_loop = new Button();
			button_loop.Top = 5;
			button_loop.Left = 0;
			button_loop.Width = 16;
			button_loop.Height = 16;
			button_loop.FlatStyle = FlatStyle.Flat;
            button_loop.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button_loop.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button_loop.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("loop")];
			button_loop.BackgroundImageLayout = ImageLayout.Stretch;
			button_loop.BackColor = Color.Transparent;
			button_loop.FlatAppearance.BorderSize = 0;
			button_loop.Click += new EventHandler(button_loop_Click);
			panel_current_selection.Controls.Add(button_loop);
			
		}
        private bool LoadTicket(Interface_audio inter_aud, Track t)
        {
            try
            {
                RichListViewItem listViewItem;
                //Track t = inter_aud.GetTrack("", "", inter_aud.Path);

                Detail detail = new Detail();
                detail.DetFamily = RichListViewItem.Family.AUDIO;
                detail.DetValue = t.AlbumName;

                listViewItem = new RichListViewItem(Path.GetFileName(t.Path_track));
                if (t.Path_cover_smart == null) listViewItem.Picture = _intAud.Tsm.Gui.imageListFicheAudio.Images[_intAud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
                else listViewItem.Picture = Image.FromFile(t.Path_cover_smart);
                listViewItem.Details = new List<Detail>() { detail };
                listViewItem.AssociatedObject = t;
                listViewItem.Size = RichListViewItem.Format.AUDIO;
                panel_list_to_play.Items.Add(listViewItem);
                listPfaToPlay.Add(listViewItem);

                if (listPfaToPlay.Count > 1) listPfaToPlay[index_fiche_selected].IsSelected = false;
                index_fiche_selected = listPfaToPlay.Count - 1;
                OnTicketSelectedChange(listPfaToPlay[index_fiche_selected], null);

                panel_list_to_play.RefreshComponent();
                if (panel_list_to_play.Items.Count > 0) { ButtonsEnable(); }
                else { ButtonsDisable(); }
                return true;
            }
            catch (Exception exp2300)
            {
                Log.Write("[ ERR : 2300 ] cannot load music data.\n" + exp2300.Message);
                return false;
            }
        }
        private void UpdateTicketList()
		{
			for(int i=0 ; i<listPfaToPlay.Count ; i++)
			{
				panel_list_to_play.Items.Remove(listPfaToPlay[i]);
				panel_list_to_play.Items.Add(listPfaToPlay[i]);
			}
		}
        private void SuspendEventHandler()
        {
            _menuAlbum.MouseHover -= _menuAlbum_MouseHover;
            _menuAlbum.MouseLeave -= _menuAlbum_MouseLeave;
            _menuArtist.MouseHover -= _menuArtist_MouseHover;
            _menuArtist.MouseLeave -= _menuArtist_MouseLeave;
            _menuFolder.MouseHover -= _menuFolder_MouseHover;
            _menuFolder.MouseLeave -= _menuFolder_MouseLeave;
            _menuType.MouseHover -= _menuType_MouseHover;
            _menuType.MouseLeave -= _menuType_MouseLeave;
        }
        private void CreateLetterAlbum(string letter)
        {
            try
            {
                List<string> albums = new List<string>();
                foreach (Track t in _intAud.ListTrack.Where(t => !string.IsNullOrEmpty(t.AlbumName) && t.AlbumName.ToLower().StartsWith(letter)))
                {
                    if (!albums.Contains(t.AlbumName))
                    { 
                        albums.Add(t.AlbumName);
                        succeed = true;
                    }
                }
                if (albums.Count > 0)
                {
                    BuildLabelTitleSortAlpha(letter.ToUpper());
                    BuildAlbumView(albums);
                }
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 2321 ] Error while presenting \"" + letter + "\" part for album: " + exp.Message);
            }
        }
        private void CreateLetterArtist(string letter)
        {
            try
            {
                List<string> artists = new List<string>();
                foreach (Track t in _intAud.ListTrack.Where(t => !string.IsNullOrEmpty(t.ArtistName) && t.ArtistName.ToLower().StartsWith(letter)))
                {
                    if (!artists.Contains(t.ArtistName))
                    {
                        artists.Add(t.ArtistName);
                        succeed = true;
                    }
                }
                if (artists.Count > 0)
                {
                    BuildLabelTitleSortAlpha(letter.ToUpper());
                    BuildArtistView(artists);
                }
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 2322 ] Error while presenting \"" + letter + "\" part for artist: " + exp.Message);
            }
        }
        private void CreateLetterFolder(string letter)
        {
            try
            {
                string folder;
                List<string> folders = new List<string>();
                foreach (Track t in _intAud.ListTrack.Where(t => !string.IsNullOrEmpty(t.Path_track)))
                {
                    folder = Path.GetDirectoryName(t.Path_track);
                    folder = folder.Split('\\')[folder.Split('\\').Length - 1];
                    if (folder.ToLower().StartsWith(letter) && !folders.Contains(folder))
                    {
                        folders.Add(folder);
                        succeed = true;
                    }
                }
                if (folders.Count > 0)
                {
                    BuildLabelTitleSortAlpha(letter.ToUpper());
                    BuildFoldersView(folders);
                }
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 2323 ] Error while presenting \"" + letter + "\" part for folder: " + exp.Message);
            }
        }
        private void CreateLetterType()
        {
            string genre;
            try
            {
                List<string> groupGenre = new List<string>();
                foreach (Track t in _intAud.ListTrack)
                {
                    if (t.Genre != null && t.Genre.Count > 0 && !string.IsNullOrEmpty(t.Genre[0]))
                    {
                        genre = t.Genre[0].ToLower();
                        CleanGenre(ref genre);
                        if (!groupGenre.Contains(genre)) {  groupGenre.Add(genre); }
                    }
                    succeed = true;
                }
                groupGenre.Sort();
                BuildGenreView(groupGenre);
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 2324 ] Error while presenting genre : " + exp.Message);
            }
        }
        private void CleanGenre(ref string genre)
        {
            if (!string.IsNullOrEmpty(genre))
            {
                genre = genre.ToLower();

                if (genre.Contains("rock")) { genre = "rock"; }
                if (genre.Contains("electr")) { genre = "electro"; }
                if (genre.Contains("folk")) { genre = "folk"; }
                if (genre.Contains("metal")) { genre = "metal"; }
                if (genre.Contains("irlan")) { genre = "irlandais"; }
                if (genre.Contains("unknow")) { genre = "other"; }
                if (genre.Contains("classi")) { genre = "classical"; }
            }
        }
        #endregion

        #region Methods protected
        protected virtual void OnTicketSelectedChange(object sender, EventArgs e)
		{
			TitleChanged();
			if (TicketSelectedChange != null)
				TicketSelectedChange(sender, e);
		}
		protected virtual void OnTicketClose(object sender, EventArgs e)
		{
			if (TicketClose != null)
				TicketClose(sender, e);
		}
		protected virtual void OnTicketPP(object sender, EventArgs e)
		{
			if (TicketPP != null)
				TicketPP(sender, e);
		}
        #endregion

        #region Event
        private void PanelAudio_SizeChanged(object sender, EventArgs e)
        {
            Resize();
        }
		private void pfa_CloseEvent(object sender, EventArgs e)
		{
			OnTicketClose(listPfaToPlay[index_fiche_selected], null);
			
			listPfaToPlay.Remove(sender as RichListViewItem);
			if(listPfaToPlay.Count-1 < index_fiche_selected) index_fiche_selected --;
			if(index_fiche_selected>=0)
			{
				listPfaToPlay[index_fiche_selected].IsSelected=true;
				TitleChanged();
				UpdateTicketList();
				OnTicketSelectedChange(listPfaToPlay[index_fiche_selected], null);
			}
			else
			{
				OnTicketSelectedChange(null, null);
			}
		}
		private void PanelAudio_WaveViewerBuilt(object sender, EventArgs e)
		{
			panel_navigation.Controls.Add((listPfaToPlay[index_fiche_selected].AssociatedObject as Track).WaveViewer);
			panel_navigation.Invalidate();
		}
		private void labelTrack_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			Label l = sender as Label;
			l.BackColor = Color.DarkBlue;
			l.ForeColor = Color.WhiteSmoke;
			string[] tmp = l.Name.Split('|');
			string artist = tmp[0];
			string album = tmp[1];
			string path = tmp[2];
			
			LoadTicket(_intAud, _intAud.GetTrack(path));
            _intAud.SaveUserParams(path);
		}
		private void labelTrack_MouseLeave(object sender, EventArgs e)
		{
			Label l = sender as Label;
			l.ForeColor = Color.White;
			l.BackColor = Color.Transparent;
		}
		private void labelTrack_MouseHover(object sender, EventArgs e)
		{
			Label l = sender as Label;
			l.ForeColor = Color.Orange;
			l.BackColor = Color.Black;
		}
		private void pa_TicketClick(object sender, EventArgs e)
		{
			PanelMusicTile pa = sender as PanelMusicTile;
			if (pa != null)
			{
				RefreshTitle(pa);
			}
		}
		private void pfa_PPEvent(object sender, EventArgs e)
		{
			OnTicketPP(sender, e);
		}
		private void button_pp_Click(object sender, EventArgs e)
		{
			_intAud.GoAction("pp");
			if((CurrentPlayedTrack.AssociatedObject as Track).IsPaused) button_pp.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("play")];
			else button_pp.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("pause")];
		}
		private void button_stop_Click(object sender, EventArgs e)
		{
			_intAud.GoAction("stop");
			button_pp.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("play")];
		}
		private void button_rewind_Click(object sender, EventArgs e)
		{
			_intAud.GoAction("rewind");
		}
		private void button_forward_Click(object sender, EventArgs e)
		{
			_intAud.GoAction("forward");
		}
		private void button_next_Click(object sender, EventArgs e)
		{
			_intAud.GoAction("next");
		}
		private void button_back_Click(object sender, EventArgs e)
		{
			_intAud.GoAction("back");
		}
		private void button_vol_Click(object sender, EventArgs e)
		{
			_intAud.GoAction("volume");
		}
		private void button_eject_Click(object sender, EventArgs e)
		{
			_intAud.GoAction("eject");
		}
		private void button_loop_Click(object sender, EventArgs e)
		{
			_intAud.GoAction("loop");
        }
        private void _menuAlbum_MouseLeave(object sender, EventArgs e)
        {
            _menuAlbum.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuAlbum")];
        }
        private void _menuArtist_MouseLeave(object sender, EventArgs e)
        {
            _menuArtist.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuArtist")];
        }
        private void _menuFolder_MouseLeave(object sender, EventArgs e)
        {
            _menuFolder.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuFolder")];
        }
        private void _menuType_MouseLeave(object sender, EventArgs e)
        {
            _menuType.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuType")];
        }
        private void _menuAlbum_MouseHover(object sender, EventArgs e)
        {
            _menuAlbum.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuAlbumHover")];
        }
        private void _menuArtist_MouseHover(object sender, EventArgs e)
        {
            _menuArtist.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuArtistHover")];
        }
        private void _menuFolder_MouseHover(object sender, EventArgs e)
        {
            _menuFolder.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuFolderHover")];
        }
        private void _menuType_MouseHover(object sender, EventArgs e)
        {
            _menuType.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuTypeHover")];
        }
        private void _menuAlbum_Click(object sender, EventArgs e)
        {
            SuspendEventHandler();
            _menuArtist.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuArtist")];
            _menuFolder.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuFolder")];
            _menuType.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuType")];

            _menuAlbum.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuAlbumSelected")];
            
            _menuArtist.MouseHover += _menuArtist_MouseHover;
            _menuArtist.MouseLeave += _menuArtist_MouseLeave;
            _menuFolder.MouseHover += _menuFolder_MouseHover;
            _menuFolder.MouseLeave += _menuFolder_MouseLeave;
            _menuType.MouseHover += _menuType_MouseHover;
            _menuType.MouseLeave += _menuType_MouseLeave;

            _currentPresentation = Presentation.ALBUM;
            RefreshPanelLibrary();
        }
        private void _menuArtist_Click(object sender, EventArgs e)
        {
            SuspendEventHandler();
            _menuAlbum.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuAlbum")];
            _menuFolder.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuFolder")];
            _menuType.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuType")];

            _menuArtist.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuArtistSelected")];

            _menuAlbum.MouseHover += _menuAlbum_MouseHover;
            _menuAlbum.MouseLeave += _menuAlbum_MouseLeave;
            _menuFolder.MouseHover += _menuFolder_MouseHover;
            _menuFolder.MouseLeave += _menuFolder_MouseLeave;
            _menuType.MouseHover += _menuType_MouseHover;
            _menuType.MouseLeave += _menuType_MouseLeave;

            _currentPresentation = Presentation.ARTIST;
            RefreshPanelLibrary();
        }
        private void _menuFolder_Click(object sender, EventArgs e)
        {
            SuspendEventHandler();
            _menuAlbum.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuAlbum")];
            _menuArtist.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuArtist")];
            _menuType.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuType")];

            _menuFolder.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuFolderSelected")];

            _menuAlbum.MouseHover += _menuAlbum_MouseHover;
            _menuAlbum.MouseLeave += _menuAlbum_MouseLeave;
            _menuArtist.MouseHover += _menuArtist_MouseHover;
            _menuArtist.MouseLeave += _menuArtist_MouseLeave;
            _menuType.MouseHover += _menuType_MouseHover;
            _menuType.MouseLeave += _menuType_MouseLeave;

            _currentPresentation = Presentation.FOLDER;
            RefreshPanelLibrary();
        }
        private void _menuType_Click(object sender, EventArgs e)
        {
            SuspendEventHandler();
            _menuAlbum.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuAlbum")];
            _menuArtist.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuArtist")];
            _menuFolder.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuFolder")];

            _menuType.BackgroundImage = _intAud.Tsm.Gui.imageListMenu.Images[_intAud.Tsm.Gui.imageListMenu.Images.IndexOfKey("MenuTypeSelected")];

            _menuAlbum.MouseHover += _menuAlbum_MouseHover;
            _menuAlbum.MouseLeave += _menuAlbum_MouseLeave;
            _menuArtist.MouseHover += _menuArtist_MouseHover;
            _menuArtist.MouseLeave += _menuArtist_MouseLeave;
            _menuFolder.MouseHover += _menuFolder_MouseHover;
            _menuFolder.MouseLeave += _menuFolder_MouseLeave;

            _currentPresentation = Presentation.GENRE;
            RefreshPanelLibrary();
        }
        #endregion
    }
}
