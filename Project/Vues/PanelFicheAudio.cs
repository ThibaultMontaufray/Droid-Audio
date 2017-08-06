//// Log code 22

//using System;
//using System.Drawing;
//using System.Text;
//using System.Windows.Forms;

//// TODO : add predicat gestion.

//namespace Droid_Audio
//{
//	public delegate void delegateMusicTicket(object sender, EventArgs e);
	
//	public class PanelFicheAudio : Panel
//	{
//		#region Attribute
//		private Graphics _graphic;
//		private GroupBox _gb;
//		private PictureBox _pictureBox;
//		private Label _labelTitle;
//		private Label _labelAlbum;
//		private Button _button_pp;
//		private Button _button_close;
//		private Track _trackLinked;
		
//		private bool _isSelected;
//		private bool _isHover;
//		private Interface_audio _intAud;
		
//		public event delegateMusicTicket CloseEvent;
//		public event delegateMusicTicket PPEvent;
//		#endregion
		
//		#region Properties
//		public Track TrackLinked
//		{
//			get { return _trackLinked; }
//			set { _trackLinked = value; }
//		}
//		public bool IsSeleccted
//		{
//			get { return _isSelected; }
//			set
//			{
//				_isSelected = value;
//				this.Refresh();
//			}
//		}
//		#endregion
		
//		#region Constructor
//		public PanelFicheAudio(Interface_audio inter, Track track)
//		{
//			_trackLinked = track;
//            _trackLinked.StatusChanged += TrackLinked_Status;
//            _intAud = inter;
//			_isSelected = true;
//			_isHover = false;
			
//			InitializeComponent();
//			InitializeData();
//		}
//        #endregion

//        #region Methods public
//        #endregion

//        #region Methods private
//        private void InitializeComponent()
//		{
//			this.Width = 80;
//			this.Height = 120;
//			this.BackColor = Color.Black;
//			this.MouseHover += new EventHandler(EventMouseHover);
//			this.MouseLeave += new EventHandler(EventMouseLeave);
			
//			_gb = new GroupBox();
//			_gb.Dock = DockStyle.Fill;
//			_gb.BackColor = Color.Black;
//			_gb.Paint += PaintBorderlessGroupBox;
//			_gb.MouseHover += new EventHandler(EventMouseHover);
//			_gb.MouseLeave += new EventHandler(EventMouseLeave);
//			_gb.Refresh();
//			this.Controls.Add(_gb);
			
//			_button_close = new Button();
//			_button_close.Width = 16;
//			_button_close.Height = 16;
//			_button_close.Top = 0;
//			_button_close.Left = 63;
//			_button_close.FlatStyle = FlatStyle.Flat;
//			_button_close.Image = _intAud.Tsm.Gui.imageListFicheAudio16.Images[_intAud.Tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("close")];
//			_button_close.BackgroundImageLayout = ImageLayout.Stretch;
//			_button_close.BackColor = Color.Transparent;
//			_button_close.FlatAppearance.BorderSize = 0;
//			_button_close.Click += new EventHandler(button_close_Click);
//			_button_close.MouseHover += new EventHandler(EventMouseHover);
//			_button_close.MouseLeave += new EventHandler(EventMouseLeave);
//			_gb.Controls.Add(_button_close);
			
//			_button_pp = new Button();
//			_button_pp.Width = 32;
//			_button_pp.Height = 32;
//			_button_pp.Top = 22;
//			_button_pp.Left = 22;
//			_button_pp.FlatStyle = FlatStyle.Popup;
//			_button_pp.Image = _intAud.Tsm.Gui.imageListFicheAudio32.Images[_intAud.Tsm.Gui.imageListFicheAudio32.Images.IndexOfKey("play")];
//			_button_pp.BackgroundImageLayout = ImageLayout.Stretch;
//			_button_pp.BackColor = Color.Transparent;
//			_button_pp.FlatAppearance.BorderSize = 0;
//			_button_pp.Click += new EventHandler(button_pp_Click);
//			_button_pp.MouseHover += new EventHandler(EventMouseHover);
			
//			_pictureBox = new PictureBox();
//			_pictureBox.Width = 74;
//			_pictureBox.Height = 74;
//			_pictureBox.Top = 1;
//			_pictureBox.Left = 1;
//			_pictureBox.BackgroundImageLayout = ImageLayout.Stretch;
//			_pictureBox.BackgroundImage = _intAud.Tsm.Gui.imageListFicheAudio.Images[_intAud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("none")];
//			_pictureBox.MouseHover += new EventHandler(EventMouseHover);
//			_pictureBox.MouseLeave += new EventHandler(EventMouseLeave);
//			_gb.Controls.Add(_pictureBox);
			
//			_labelTitle = new Label();
//			_labelTitle.Top = 80;
//			_labelTitle.Left = 0;
//			_labelTitle.Width = 80;
//			_labelTitle.Height = 14;
//			_labelTitle.TextAlign = ContentAlignment.TopCenter;
//			_labelTitle.ForeColor = Color.WhiteSmoke;
//			_labelTitle.BackColor = Color.Transparent;
//			_labelTitle.MouseHover += new EventHandler(EventMouseHover);
//			_labelTitle.MouseLeave += new EventHandler(EventMouseLeave);
//			_labelTitle.Text = "";
//			_gb.Controls.Add(_labelTitle);
			
//			_labelAlbum = new Label();
//			_labelAlbum.Top = 100;
//			_labelAlbum.Left = 0;
//			_labelAlbum.Width = 80;
//			_labelAlbum.Height = 14;
//			_labelAlbum.TextAlign = ContentAlignment.TopCenter;
//			_labelAlbum.ForeColor = Color.WhiteSmoke;
//			_labelAlbum.BackColor = Color.Transparent;
//			_labelAlbum.MouseHover += new EventHandler(EventMouseHover);
//			_labelAlbum.MouseLeave += new EventHandler(EventMouseLeave);
//			_labelAlbum.Text = "";
//			_gb.Controls.Add(_labelAlbum);
//		}
//		private void InitializeData()
//		{
//			if (!string.IsNullOrEmpty(_trackLinked.Title) && _trackLinked.AlbumName != null)
//			{
//				if (_trackLinked.Albums != null && _trackLinked.Path_cover_smart != null) _pictureBox.BackgroundImage = new Bitmap(_trackLinked.Path_cover_smart);
//				else _pictureBox.BackgroundImage = _intAud.Tsm.Gui.imageListFicheAudio.Images[_intAud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
				
//				if(_trackLinked.Title.Length<10) _labelTitle.Text = _trackLinked.Title;
//				else _labelTitle.Text = _trackLinked.Title.Substring(0, 10) + "...";
				
//				if(_trackLinked.AlbumName.Length<10) _labelAlbum.Text = _trackLinked.AlbumName;
//				else _labelAlbum.Text = _trackLinked.AlbumName.Substring(0, 10) + "...";
//			}
//		}
//        private void UpdateStatusStyle()
//        {
//            if (_trackLinked.IsPlaying)
//            {
//                this.BackColor = Color.DimGray;
//            }
//            else
//            {
//                this.BackColor = Color.Black;
//            }
//        }
//		#endregion
		
//		#region Methods protected
//		protected virtual void OnClose(object sender, EventArgs e)
//		{
//			if (CloseEvent != null)
//				CloseEvent(sender, e);
//		}
//		protected virtual void OnPP(object sender, EventArgs e)
//		{
//			if (PPEvent != null)
//				PPEvent(sender, e);
//		}
//        #endregion

//        #region Event
//        private void EventMouseHover(object sender, EventArgs e)
//		{
//			_isHover = true;
//			this.Invalidate();
//		}
//		private void EventMouseLeave(object sender, EventArgs e)
//		{
//			_isHover = false;
//			this.Invalidate();
//		}
//		private void button_close_Click(object sender, EventArgs e)
//		{
//			_intAud.Panau.ButtonsDisable();
//			this._trackLinked.Close();
//			this.Dispose();
//			OnClose(this, null);
//		}
//		private void button_pp_Click(object sender, EventArgs e)
//		{
//			OnPP(sender, e);
//		}
//		private void PaintBorderlessGroupBox(object sender, PaintEventArgs e)
//		{
//			_graphic = e.Graphics;
//			Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
//			if (_isSelected)
//			{
//				using(Brush brush = new SolidBrush(Color.FromArgb(40, 40, 40))){
//					using(Pen borderPen = new Pen(Color.Yellow)){
//						_graphic.FillRectangle(brush, r);
//						_graphic.DrawRectangle(borderPen, r);
//					}
//				}
//				_intAud.Panau.ButtonsEnable();
//			}
//			else if(_isHover)
//			{
//				using(Brush brush = new SolidBrush(Color.FromArgb(40, 40, 40))){
//					using(Pen borderPen = new Pen(Color.DarkOrange)){
//						_graphic.FillRectangle(brush, r);
//						_graphic.DrawRectangle(borderPen, r);
//					}
//				}
//			}
//			else
//			{
//				using(Brush brush = new SolidBrush(Color.FromArgb(40, 40, 40))){
//					using(Pen borderPen = new Pen(Color.FromArgb(30, 30, 30))){
//						_graphic.FillRectangle(brush, r);
//						_graphic.DrawRectangle(borderPen, r);
//					}
//				}
//				if(_trackLinked.WaveViewer != null && _trackLinked.WaveViewer.WaveStream != null) _trackLinked.WaveViewer.WaveStream.Close();
//				_intAud.Panau.ButtonsDisable();
//			}
//        }
//        private void TrackLinked_Status(object sender, EventArgs e)
//        {
//            UpdateStatusStyle();
//        }
//        #endregion
//    }
//}