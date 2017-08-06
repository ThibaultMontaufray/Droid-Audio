// Log code 27 03

/*
 * User: Thibault MONTAUFRAY
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Tools4Libraries;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Droid_Audio
{
	public delegate void delegatePanelMusicTile(object sender, EventArgs e);
	
	public class PanelMusicTile : Panel
	{
        #region Enum
        public enum Ticket
        {
            ALBUM,
            ARTIST,
            FOLDER,
            GENRE
        }
        #endregion

        #region Attribute
        private Interface_audio _intAud;
		private List<string> _albums;
		private List<string> _artists;
        private List<string> _folders;
        private List<Track> _tracks;
        private string _genre;
        private Ticket _kindOfTicket;

        private Label _labelSecond;
		private Label _labelMain;
		private PictureBox _pictureMain;
		private PictureBox _picture1;
		private PictureBox _picture2;
		private PictureBox _picture3;

        public event delegatePanelMusicTile TicketClick;
        #endregion

        #region Properties
        public List<string> TicketFolders
        {
            get { return _folders; }
            set { _folders = value; }
        }
        public List<string> TicketArtist
		{
			get { return _artists; }
		}
		public List<string> TicketAlbums
		{
			get { return _albums; }
		}
		public Ticket KindOfTicket
		{
			get { return _kindOfTicket; }
		}
        public string Genre
        {
            get { return _genre; }
            set { _genre = value; }
        }
        #endregion

        #region Constructor
        public PanelMusicTile(Interface_audio inter_aud, List<Track> trackLst, Ticket kind_of_ticket)
        {
            _albums = new List<string>();
            _artists = new List<string>();
            _folders = new List<string>();

            _kindOfTicket = kind_of_ticket;
            _intAud = inter_aud;
            _tracks = trackLst;

            foreach (Track track in trackLst)
            {
                if (track.AlbumName != null && !_albums.Contains(track.AlbumName)) _albums.Add(track.AlbumName);
                if (track.ArtistName != null && !_artists.Contains(track.ArtistName)) _artists.Add(track.ArtistName);
                if (track.Path_track != null && !_folders.Contains(track.Path_track)) _folders.Add(track.Path_track);
            }
            BuildComponent();
        }
        public PanelMusicTile(Interface_audio inter_aud, string genre, Ticket kind_of_ticket)
        {
            _albums = new List<string>();
            _artists = new List<string>();
            _folders = new List<string>();

            _kindOfTicket = kind_of_ticket;
            _intAud = inter_aud;
            _tracks = null;
            _genre = genre;
            BuildComponent();
        }
        #endregion

        #region Methods protected
        protected virtual void OnTicketClick(object sender, EventArgs e)
        {
            if (TicketClick != null) TicketClick(sender, e);
        }
        #endregion
        
        #region Methods private
        private void BuildComponent()
        {
            BuildComponenCommonBase();
            switch (_kindOfTicket)
            {
                case Ticket.ALBUM:
                    BuildComponentAlbum();
                    break;
                case Ticket.ARTIST:
                    BuildComponentArtist();
                    break;
                case Ticket.FOLDER:
                    BuildComponentFolder();
                    break;
                case Ticket.GENRE:
                    BuildComponentGenre();
                    break;
            }
        }
        private void BuildComponenCommonBase()
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.Width = 100;
            this.Height = 134;
            this.MouseHover += new EventHandler(PanelAlbum_MouseHover);
            this.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
            this.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
        }
        private void BuildComponentAlbum()
        {
			_labelMain = new Label();
			_labelMain.Top = 4;
			_labelMain.Left = 4;
			_labelMain.Width = this.Width;
			_labelMain.Height = 14;
			_labelMain.ForeColor = Color.White;
			_labelMain.MouseHover += new EventHandler(PanelAlbum_MouseHover);
			_labelMain.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
			_labelMain.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
            _labelMain.Text = _albums.Count > 0 ? _albums[0] : "Unknow"; 
			this.Controls.Add(_labelMain);
			
			if(_albums!=null)
			{
				_pictureMain = new PictureBox();
				if(!string.IsNullOrEmpty(_tracks[0].Path_cover_smart)) 
				{
					try 
					{
						_pictureMain.BackgroundImage = new Bitmap(_tracks[0].Path_cover_smart);
					} 
					catch (Exception exp2700) 
					{
						_pictureMain.BackgroundImage = _intAud.Tsm.Gui.imageListFicheAudio.Images[_intAud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
						Log.Write("[ WRN : 2700 ] Cannot open bitmap file.\n" + exp2700.Message);
					}
				}
				else _pictureMain.BackgroundImage = _intAud.Tsm.Gui.imageListFicheAudio.Images[_intAud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
				_pictureMain.BackgroundImageLayout = ImageLayout.Stretch;
				_pictureMain.Width = 90;
				_pictureMain.Height = 90;
				_pictureMain.Top = 40;
				_pictureMain.Left = 5;
				_pictureMain.MouseHover += new EventHandler(PanelAlbum_MouseHover);
				_pictureMain.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
				_pictureMain.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
				this.Controls.Add(_pictureMain);
			}
			else
			{
				BuildMultipleAlbum();
			}
        }
        private async void BuildComponentArtist()
        {
            _labelMain = new Label();
            _labelMain.Top = 4;
            _labelMain.Left = 4;
            _labelMain.Width = this.Width;
            _labelMain.Height = 14;
            _labelMain.ForeColor = Color.White;
            _labelMain.MouseHover += new EventHandler(PanelAlbum_MouseHover);
            _labelMain.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
            _labelMain.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
            _labelMain.Text = _artists.Count > 0 ? _artists[0] : "Unknow";
            this.Controls.Add(_labelMain);
            
            _pictureMain = new PictureBox();
            _pictureMain.BackgroundImage = _intAud.Tsm.Gui.imageListFicheAudio.Images[_intAud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("artist")];
            _pictureMain.BackgroundImageLayout = ImageLayout.Zoom;
            _pictureMain.Width = 90;
            _pictureMain.Height = 90;
            _pictureMain.Top = 40;
            _pictureMain.Left = 5;
            _pictureMain.MouseHover += new EventHandler(PanelAlbum_MouseHover);
            _pictureMain.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
            _pictureMain.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
            this.Controls.Add(_pictureMain);

            try
            {
                Task<Image> taskArtistPicture = Task.Run(() => GetArtistPicture());
                _pictureMain.BackgroundImage = await taskArtistPicture;
            }
            catch (Exception exp)
            {
                Log.Write("[ WRN : 2701 ] Cannot load artist picture async.\n" + exp.Message);
                _pictureMain.BackgroundImage = _intAud.Tsm.Gui.imageListFicheAudio.Images[_intAud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("artist")];
            }
        }
        private void BuildComponentFolder()
        {

        }
        private void BuildComponentGenre()
        {
            _labelMain = new Label();
            _labelMain.Top = 4;
            _labelMain.Left = 4;
            _labelMain.Width = this.Width;
            _labelMain.Height = 14;
            _labelMain.ForeColor = Color.White;
            _labelMain.MouseHover += new EventHandler(PanelAlbum_MouseHover);
            _labelMain.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
            _labelMain.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
            this.Controls.Add(_labelMain);

            _pictureMain = new PictureBox();
            _pictureMain.BackgroundImageLayout = ImageLayout.Zoom;
            _pictureMain.Width = 90;
            _pictureMain.Height = 90;
            _pictureMain.Top = 40;
            _pictureMain.Left = 5;
            _pictureMain.MouseHover += new EventHandler(PanelAlbum_MouseHover);
            _pictureMain.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
            _pictureMain.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
            this.Controls.Add(_pictureMain);
            
            if (!string.IsNullOrEmpty(_genre))
            {
                _labelMain.Text = _genre;
                if (_intAud.Tsm.Gui.imageListType.Images.IndexOfKey(_genre) != -1)
                {
                    _pictureMain.BackgroundImage = _intAud.Tsm.Gui.imageListType.Images[_intAud.Tsm.Gui.imageListType.Images.IndexOfKey(_genre)];
                }
                else
                {
                    _pictureMain.BackgroundImage = _intAud.Tsm.Gui.imageListType.Images[_intAud.Tsm.Gui.imageListType.Images.IndexOfKey("other")];
                }
            }
            else
            {
                _pictureMain.BackgroundImage = _intAud.Tsm.Gui.imageListType.Images[_intAud.Tsm.Gui.imageListType.Images.IndexOfKey("other")];
                _labelMain.Text = "Other";
            }
        }
        private void BuildMultipleAlbum()
		{
			if(_albums.Count > 0)
			{
				_pictureMain = new PictureBox();
				if(!string.IsNullOrEmpty(_tracks[0].Path_cover_smart)) _pictureMain.BackgroundImage = new Bitmap(_tracks[0].Path_cover_smart);
				else _pictureMain.BackgroundImage = _intAud.Tsm.Gui.imageListFicheAudio.Images[_intAud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
				_pictureMain.BackgroundImageLayout = ImageLayout.Stretch;
				_pictureMain.Width = 40;
				_pictureMain.Height = 40;
				_pictureMain.Top = 40;
				_pictureMain.Left = 5;
				_pictureMain.MouseHover += new EventHandler(PanelAlbum_MouseHover);
				_pictureMain.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
				_pictureMain.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
				this.Controls.Add(_pictureMain);
			}
			
			//if(_albums.Count > 1)
			//{
			//	_picture1 = new PictureBox();
			//	if(!string.IsNullOrEmpty(_albums[1].Path_cover_smart)) _picture1.BackgroundImage = new Bitmap(_albums[1].Path_cover_smart);
			//	else _picture1.BackgroundImage = _intAud.Tsm.Gui.imageListFicheAudio.Images[_intAud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
			//	_picture1.BackgroundImageLayout = ImageLayout.Stretch;
			//	_picture1.Width = 40;
			//	_picture1.Height = 40;
			//	_picture1.Top = 40;
			//	_picture1.Left = 48;
			//	_picture1.MouseHover += new EventHandler(PanelAlbum_MouseHover);
			//	_picture1.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
			//	_picture1.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
			//	this.Controls.Add(_picture1);
			//}
			
			//if(_albums.Count > 2)
			//{
			//	_picture2 = new PictureBox();
			//	if(!string.IsNullOrEmpty(_albums[2].Path_cover_smart)) _picture2.BackgroundImage = new Bitmap(_albums[2].Path_cover_smart);
			//	else _picture2.BackgroundImage = _intAud.Tsm.Gui.imageListFicheAudio.Images[_intAud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
			//	_picture2.BackgroundImageLayout = ImageLayout.Stretch;
			//	_picture2.Width = 40;
			//	_picture2.Height = 40;
			//	_picture2.Top = 87;
			//	_picture2.Left = 5;
			//	_picture2.MouseHover += new EventHandler(PanelAlbum_MouseHover);
			//	_picture2.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
			//	_picture2.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
			//	this.Controls.Add(_picture2);
			//}
			
			//if(_albums.Count > 3)
			//{
			//	_picture3 = new PictureBox();
			//	if(!string.IsNullOrEmpty(_albums[3].Path_cover_smart)) _picture3.BackgroundImage = new Bitmap(listAlbum[3].Path_cover_smart);
			//	else _picture3.BackgroundImage = _intAud.Tsm.Gui.imageListFicheAudio.Images[_intAud.Tsm.Gui.imageListFicheAudio.Images.IndexOfKey("void")];
			//	_picture3.BackgroundImageLayout = ImageLayout.Stretch;
			//	_picture3.Width = 40;
			//	_picture3.Height = 40;
			//	_picture3.Top = 87;
			//	_picture3.Left = 48;
			//	_picture3.MouseHover += new EventHandler(PanelAlbum_MouseHover);
			//	_picture3.MouseLeave += new EventHandler(PanelAlbum_MouseLeave);
			//	_picture3.MouseClick += new MouseEventHandler(PanelAlbum_MouseClick);
			//	this.Controls.Add(_picture3);
			//}
		}

        private Image GetArtistPicture()
        {
            string pictureFileName = Path.Combine(_intAud.WORKING_PATH, "Artists", _artists[0]) + ".jpg";
            if (!Directory.Exists(Path.Combine(_intAud.WORKING_PATH, "Artists")))
            {
                Directory.CreateDirectory(Path.Combine(_intAud.WORKING_PATH, "Artists"));
            }

            if (_artists.Count > 0)
            {
                try
                {
                    if (!File.Exists(pictureFileName))
                    {
                        try
                        {
                            List<string> imgUrl = Droid_web.Web.GetImages(string.Format("\"{0}\" music artist profil", _artists[0]));
                            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(imgUrl[0]);
                            webRequest.AllowWriteStreamBuffering = true;
                            webRequest.Timeout = 30000;
                            WebResponse webResponse = webRequest.GetResponse();
                            Stream stream = webResponse.GetResponseStream();

                            Image artistPic = Image.FromStream(stream);
                            ResizePicture(ref artistPic, 200, 200);
                            artistPic.Save(pictureFileName);
                            
                            webResponse.Close();
                        }
                        catch (Exception exp)
                        {
                            Log.Write("[ ERR : 2703 ] Error while loading artist picture.\n\n" + exp.Message);
                        }
                    }
                    if (File.Exists(pictureFileName))
                    {
                        return Image.FromFile(pictureFileName);
                    }
                }
                catch (Exception exp)
                {
                    Log.Write("[ ERR : 2700 ] Error while loading artist picture.\n\n" + exp.Message);
                }
            }
            return null;
        }
        #endregion
        
        // TODO : migrate this in Droid_Image
        private string ImageToString(Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, img.RawFormat);
            byte[] array = ms.ToArray();
            string imgString = Convert.ToBase64String(array);
            return Convert.ToBase64String(array);
        }
        // TODO : migrate this in Droid_Image
        private Image StringToImage(string imageString)
        {
            if (imageString == null)
            {
                Log.Write("[ INF : 2702 ] Image string doesn't exist.");
                return null;
            }
            byte[] array = Convert.FromBase64String(imageString);
            Image image = Image.FromStream(new MemoryStream(array));
            return image;
        }
        // TODO : migrate this in Droid_Image
        private void ResizePicture(ref Image image, int maxWidth, int maxHeight)
        {
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            image = newImage;
        }

        #region Event
        private void PanelAlbum_MouseClick(object sender, MouseEventArgs e)
		{
			OnTicketClick(sender, e);
		}
		private void PanelAlbum_MouseHover(object sender, EventArgs e)
		{
			this.BorderStyle = BorderStyle.FixedSingle;
			this.BackColor = Color.FromArgb(150 ,150 ,150);
		}
		private void PanelAlbum_MouseLeave(object sender, EventArgs e)
		{
			this.BorderStyle = BorderStyle.None;
			this.BackColor = Color.FromArgb(30 ,30 ,30);
		}
		#endregion
	}
}
