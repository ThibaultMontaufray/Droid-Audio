// Log code 27

/*
 * User: Thibault MONTAUFRAY
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Tools4Libraries;

namespace Droid_Audio
{
	public delegate void delegatePanelAlbum(object sender, EventArgs e);
	
	/// <summary>
	/// Description of PanelAlbum.
	/// </summary>
	public class PanelAlbum : Panel
	{
		#region Attribute
		private Interface_audio int_aud;
		private List<Album> listAlbum;
		private Album album;
		private string kindOfTicket;
		private Artist artist;
		
		private Label labelAlbum;
		private Label labelArtist;
		private PictureBox picture0;
		private PictureBox picture1;
		private PictureBox picture2;
		private PictureBox picture3;
		
		public event delegatePanelAlbum TicketClick;
		#endregion
		
		#region Properties
		public Artist TicketArtist
		{
			get { return artist; }
		}
		
		public Album TicketAlbum
		{
			get { return album; }
		}
		
		public string KindOfTicket
		{
			get { return kindOfTicket; }
		}
		#endregion
		
		#region Constructor
		public PanelAlbum(Interface_audio inter_aud, Album alb, string kind_of_ticket)
		{
			kindOfTicket = kind_of_ticket;
			album = alb;
			artist = album.My_Artist;
			int_aud = inter_aud;
			InitializeComponent();
		}
		
		public PanelAlbum(Interface_audio inter_aud, List<Album> list_alb, string kind_of_ticket)
		{
			kindOfTicket = kind_of_ticket;
			listAlbum = list_alb;
			if(listAlbum.Count>0) artist = listAlbum[0].My_Artist;
			int_aud = inter_aud;
			InitializeComponent();
		}
		#endregion
		
		#region Methods
		private void InitializeComponent()
		{
			this.BackColor = Color.FromArgb(30, 30, 30);
			this.Width = 100;
			this.Height = 134;
			this.MouseHover += new EventHandler(PanelAlbum_MouseHover);
			this.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
			this.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
			
			labelArtist = new Label();
			labelArtist.Top = 4;
			labelArtist.Left = 4;
			labelArtist.Width = this.Width;
			labelArtist.Height = 14;
			labelArtist.ForeColor = Color.White;
			labelArtist.MouseHover += new EventHandler(PanelAlbum_MouseHover);
			labelArtist.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
			labelArtist.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
			if(listAlbum==null) labelArtist.Text = album.My_Artist.Name;
			else labelArtist.Text = artist.Name;
			this.Controls.Add(labelArtist);
			
			labelAlbum = new Label();
			labelAlbum.Top = 22;
			labelAlbum.Left = 4;
			labelAlbum.Width = this.Width;
			labelAlbum.Height = 14;
			labelAlbum.ForeColor = Color.White;
			labelAlbum.MouseHover += new EventHandler(PanelAlbum_MouseHover);
			labelAlbum.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
			labelAlbum.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
			if(listAlbum==null) labelAlbum.Text = album.Name;
			else labelAlbum.Text = "...";
			this.Controls.Add(labelAlbum);
			
			if(album!=null)
			{
				picture0 = new PictureBox();
				if(!string.IsNullOrEmpty(album.Path_cover_smart)) 
				{
					try 
					{
						picture0.BackgroundImage = new Bitmap(album.Path_cover_smart);
					} 
					catch (Exception exp2700) 
					{
						picture0.BackgroundImage = int_aud.Tsm.Gui.imageListFicheAudio.Images[int_aud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
						Log.write("[ WRN : 2700 ] Cannot open bitmap file.\n" + exp2700.Message);
					}
				}
				else picture0.BackgroundImage = int_aud.Tsm.Gui.imageListFicheAudio.Images[int_aud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
				picture0.BackgroundImageLayout = ImageLayout.Stretch;
				picture0.Width = 90;
				picture0.Height = 90;
				picture0.Top = 40;
				picture0.Left = 5;
				picture0.MouseHover += new EventHandler(PanelAlbum_MouseHover);
				picture0.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
				picture0.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
				this.Controls.Add(picture0);
			}
			else
			{
				BuildMultipleAlbum();
			}
		}
		private void BuildMultipleAlbum()
		{
			if(listAlbum.Count > 0)
			{
				picture0 = new PictureBox();
				if(!string.IsNullOrEmpty(listAlbum[0].Path_cover_smart)) picture0.BackgroundImage = new Bitmap(listAlbum[0].Path_cover_smart);
				else picture0.BackgroundImage = int_aud.Tsm.Gui.imageListFicheAudio.Images[int_aud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
				picture0.BackgroundImageLayout = ImageLayout.Stretch;
				picture0.Width = 40;
				picture0.Height = 40;
				picture0.Top = 40;
				picture0.Left = 5;
				picture0.MouseHover += new EventHandler(PanelAlbum_MouseHover);
				picture0.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
				picture0.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
				this.Controls.Add(picture0);
			}
			
			if(listAlbum.Count > 1)
			{
				picture1 = new PictureBox();
				if(!string.IsNullOrEmpty(listAlbum[1].Path_cover_smart)) picture1.BackgroundImage = new Bitmap(listAlbum[1].Path_cover_smart);
				else picture1.BackgroundImage = int_aud.Tsm.Gui.imageListFicheAudio.Images[int_aud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
				picture1.BackgroundImageLayout = ImageLayout.Stretch;
				picture1.Width = 40;
				picture1.Height = 40;
				picture1.Top = 40;
				picture1.Left = 48;
				picture1.MouseHover += new EventHandler(PanelAlbum_MouseHover);
				picture1.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
				picture1.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
				this.Controls.Add(picture1);
			}
			
			if(listAlbum.Count > 2)
			{
				picture2 = new PictureBox();
				if(!string.IsNullOrEmpty(listAlbum[2].Path_cover_smart)) picture2.BackgroundImage = new Bitmap(listAlbum[2].Path_cover_smart);
				else picture2.BackgroundImage = int_aud.Tsm.Gui.imageListFicheAudio.Images[int_aud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
				picture2.BackgroundImageLayout = ImageLayout.Stretch;
				picture2.Width = 40;
				picture2.Height = 40;
				picture2.Top = 87;
				picture2.Left = 5;
				picture2.MouseHover += new EventHandler(PanelAlbum_MouseHover);
				picture2.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
				picture2.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
				this.Controls.Add(picture2);
			}
			
			if(listAlbum.Count > 3)
			{
				picture3 = new PictureBox();
				if(!string.IsNullOrEmpty(listAlbum[3].Path_cover_smart)) picture3.BackgroundImage = new Bitmap(listAlbum[3].Path_cover_smart);
				else picture3.BackgroundImage = int_aud.Tsm.Gui.imageListFicheAudio.Images[int_aud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
				picture3.BackgroundImageLayout = ImageLayout.Stretch;
				picture3.Width = 40;
				picture3.Height = 40;
				picture3.Top = 87;
				picture3.Left = 48;
				picture3.MouseHover += new EventHandler(PanelAlbum_MouseHover);
				picture3.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
				picture3.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
				this.Controls.Add(picture3);
			}
		}
		protected virtual void OnTicketClick(object sender, EventArgs e)
		{
			if (TicketClick != null)
				TicketClick(sender, e);
		}
		#endregion
		
		#region Event
		private void PanelAlbum_MouseClick(object sender, MouseEventArgs e)
		{
			OnTicketClick(sender, e);
		}
		
		private void PanelAlbum_MouseHover(object sender, EventArgs e)
		{
			this.BorderStyle = BorderStyle.FixedSingle;
			this.BackColor = Color.FromArgb(100 ,100 ,100);
		}
		
		private void PanelAlbum_MouseLeave(object sender, EventArgs e)
		{
			this.BorderStyle = BorderStyle.None;
			this.BackColor = Color.FromArgb(30 ,30 ,30);
		}
		#endregion
	}
}
