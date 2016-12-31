// Log code 22

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

// TODO : add predicat gestion.

namespace Droid_Audio
{
	public delegate void delegateMusicTicket(object sender, EventArgs e);
	
	public partial class PanelFicheAudio : Panel
	{
		#region Attribute
		private Graphics graphic;
		private GroupBox gb;
		private PictureBox pictureBox;
		private Label labelTitle;
		private Label labelAlbum;
		private Button button_pp;
		private Button button_close;
		private Track trackLinked;
		
		private bool isSelected;
		private bool isHover;
		private Interface_audio int_aud;
		
		public event delegateMusicTicket CloseEvent;
		public event delegateMusicTicket PPEvent;
		#endregion
		
		#region Properties
		public Track TrackLinked
		{
			get { return trackLinked; }
			set { trackLinked = value; }
		}
		
		private ToolStripMenuAudio tsm
		{
			get { return int_aud.Tsm as ToolStripMenuAudio; }
		}
		
		public bool IsSeleccted
		{
			get { return isSelected; }
			set
			{
				isSelected = value;
				this.Refresh();
			}
		}
		#endregion
		
		#region Constructor
		public PanelFicheAudio(Interface_audio inter, Track track)
		{
			trackLinked = track;
			int_aud = inter;
			isSelected = true;
			isHover = false;
			
			this.MouseHover += new EventHandler(PanelFicheAudio_MouseHover);
			
			InitializeComponent();
			InitializeData();
		}

		void PanelFicheAudio_MouseHover(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
		#endregion
		
		#region Methods public
		#endregion
		
		#region Methods private
		private void InitializeComponent()
		{
			this.Width = 80;
			this.Height = 120;
			this.BackColor = Color.Black;
			this.MouseHover += new EventHandler(EventMouseHover);
			this.MouseLeave += new EventHandler(EventMouseLeave);
			
			gb = new GroupBox();
			gb.Dock = DockStyle.Fill;
			gb.BackColor = Color.Black;
			gb.Paint += PaintBorderlessGroupBox;
			gb.MouseHover += new EventHandler(EventMouseHover);
			gb.MouseLeave += new EventHandler(EventMouseLeave);
			gb.Refresh();
			this.Controls.Add(gb);
			
			button_close = new Button();
			button_close.Width = 16;
			button_close.Height = 16;
			button_close.Top = 0;
			button_close.Left = 63;
			button_close.FlatStyle = FlatStyle.Flat;
			button_close.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("close")];
			button_close.BackgroundImageLayout = ImageLayout.Stretch;
			button_close.BackColor = Color.Transparent;
			button_close.FlatAppearance.BorderSize = 0;
			button_close.Click += new EventHandler(button_close_Click);
			button_close.MouseHover += new EventHandler(EventMouseHover);
			button_close.MouseLeave += new EventHandler(EventMouseLeave);
			gb.Controls.Add(button_close);
			
			button_pp = new Button();
			button_pp.Width = 32;
			button_pp.Height = 32;
			button_pp.Top = 22;
			button_pp.Left = 22;
			button_pp.FlatStyle = FlatStyle.Popup;
			button_pp.Image = tsm.Gui.imageListFicheAudio32.Images[tsm.Gui.imageListFicheAudio32.Images.IndexOfKey("play")];
			button_pp.BackgroundImageLayout = ImageLayout.Stretch;
			button_pp.BackColor = Color.Transparent;
			button_pp.FlatAppearance.BorderSize = 0;
			button_pp.Click += new EventHandler(button_pp_Click);
			button_pp.MouseHover += new EventHandler(EventMouseHover);
			//gb.Controls.Add(button_pp);
			
			pictureBox = new PictureBox();
			pictureBox.Width = 74;
			pictureBox.Height = 74;
			pictureBox.Top = 1;
			pictureBox.Left = 1;
			pictureBox.BackgroundImageLayout = ImageLayout.Stretch;
			pictureBox.BackgroundImage = tsm.Gui.imageListFicheAudio.Images[tsm.Gui.imageListFicheAudio.Images.IndexOfKey("none")];
			pictureBox.MouseHover += new EventHandler(EventMouseHover);
			pictureBox.MouseLeave += new EventHandler(EventMouseLeave);
			gb.Controls.Add(pictureBox);
			
			labelTitle = new Label();
			labelTitle.Top = 80;
			labelTitle.Left = 0;
			labelTitle.Width = 80;
			labelTitle.Height = 14;
			labelTitle.TextAlign = ContentAlignment.TopCenter;
			labelTitle.ForeColor = Color.WhiteSmoke;
			labelTitle.BackColor = Color.Transparent;
			labelTitle.MouseHover += new EventHandler(EventMouseHover);
			labelTitle.MouseLeave += new EventHandler(EventMouseLeave);
			labelTitle.Text = "";
			gb.Controls.Add(labelTitle);
			
			labelAlbum = new Label();
			labelAlbum.Top = 100;
			labelAlbum.Left = 0;
			labelAlbum.Width = 80;
			labelAlbum.Height = 14;
			labelAlbum.TextAlign = ContentAlignment.TopCenter;
			labelAlbum.ForeColor = Color.WhiteSmoke;
			labelAlbum.BackColor = Color.Transparent;
			labelAlbum.MouseHover += new EventHandler(EventMouseHover);
			labelAlbum.MouseLeave += new EventHandler(EventMouseLeave);
			labelAlbum.Text = "";
			gb.Controls.Add(labelAlbum);
		}
		
		private void InitializeData()
		{
			if (!string.IsNullOrEmpty(trackLinked.Title))
			{
				if (trackLinked.Album != null && trackLinked.Album.Path_cover_smart != null) pictureBox.BackgroundImage = new Bitmap(trackLinked.Album.Path_cover_smart);
				else pictureBox.BackgroundImage = tsm.Gui.imageListFicheAudio.Images[tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
				
				if(trackLinked.Title.Length<10) labelTitle.Text = trackLinked.Title;
				else labelTitle.Text = trackLinked.Title.Substring(0, 10) + "...";
				
				if(trackLinked.Album.Name.Length<10) labelAlbum.Text = trackLinked.Album.Name;
				else labelAlbum.Text = trackLinked.Album.Name.Substring(0, 10) + "...";
			}
		}
		#endregion
		
		#region Methods protected
		protected virtual void OnClose(object sender, EventArgs e)
		{
			if (CloseEvent != null)
				CloseEvent(sender, e);
		}
		protected virtual void OnPP(object sender, EventArgs e)
		{
			if (PPEvent != null)
				PPEvent(sender, e);
		}
		#endregion
		
		#region Event
		private void EventMouseHover(object sender, EventArgs e)
		{
			isHover = true;
			this.Invalidate();
		}

		private void EventMouseLeave(object sender, EventArgs e)
		{
			isHover = false;
			this.Invalidate();
		}
		
		private void button_close_Click(object sender, EventArgs e)
		{
			int_aud.Panau.ButtonsDisable();
			this.trackLinked.Close();
			this.Dispose();
			OnClose(this, null);
		}
		
		private void button_pp_Click(object sender, EventArgs e)
		{
			OnPP(sender, e);
		}
		
		private void PaintBorderlessGroupBox(object sender, PaintEventArgs e)
		{
			graphic = e.Graphics;
			Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
			if (isSelected)
			{
				using(Brush brush = new SolidBrush(Color.FromArgb(40, 40, 40))){
					using(Pen borderPen = new Pen(Color.Yellow)){
						graphic.FillRectangle(brush, r);
						graphic.DrawRectangle(borderPen, r);
					}
				}
				int_aud.Panau.ButtonsEnable();
			}
			else if(isHover)
			{
				using(Brush brush = new SolidBrush(Color.FromArgb(40, 40, 40))){
					using(Pen borderPen = new Pen(Color.DarkOrange)){
						graphic.FillRectangle(brush, r);
						graphic.DrawRectangle(borderPen, r);
					}
				}
			}
			else
			{
				using(Brush brush = new SolidBrush(Color.FromArgb(40, 40, 40))){
					using(Pen borderPen = new Pen(Color.FromArgb(30, 30, 30))){
						graphic.FillRectangle(brush, r);
						graphic.DrawRectangle(borderPen, r);
					}
				}
				if(trackLinked.WaveViewer != null && trackLinked.WaveViewer.WaveStream != null) trackLinked.WaveViewer.WaveStream.Close();
				int_aud.Panau.ButtonsDisable();
			}
		}
		#endregion
	}
}