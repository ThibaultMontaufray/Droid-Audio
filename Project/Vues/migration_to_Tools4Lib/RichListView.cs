using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Tools4Libraries;
using System.Linq;

namespace Droid_Audio
{
    public delegate void TileEvent(string eventName);
    public class RichListView : Panel
    {
        #region Attribute
        public event TileEvent TileEvent;

        private List<RichListViewItem> _itemList;
        private System.Windows.Forms.View _display;
        private ImageList _imageList;
        private List<string> _groups;
        private int _panelComponentX;
        private int _panelComponentY;
        private int _panelX = 5;
        private int _panelY = 5;
        private int _tilesColumCapacity;
        private RichListViewItem.Format _size;
        private bool _selected;
        #endregion

        #region Properties
        public List<RichListViewItem> Items
        {
            get { return _itemList; }
            set { _itemList = value; }
        }
        public View Display
        {
            get { return _display; }
            set { _display = value; }
        }
        public ImageList IconList
        {
            get { return _imageList; }
            set { _imageList = value; }
        }
        public List<string> Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }
        public RichListViewItem.Format ComponentSize
        {
            get { return _size; }
            set { _size = value; }
        }
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                ItemSelectionChanged();
            }
        }
        #endregion

        #region Constructor
        public RichListView()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods public
        public void RefreshComponent()
        {
            if (this.Width / ((int)_size + 5) != _tilesColumCapacity)
            {
                _panelX = 5;
                _panelY = 5;
                this.Controls.Clear();
                if (_display == View.Tile) DrawTiles();
                else if (_display == View.Details) DrawTiles();
            }
        }
        #endregion

        #region Methods private
        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.Resize += new EventHandler(RichListView_Resize);

            _groups = new List<string>();
            _itemList = new List<RichListViewItem>();
            _display = View.Tile;

            RefreshComponent();
        }

        private void DrawTiles()
        {
            this.Controls.Clear();
            _panelX = 5;
            _panelY = 5;
            foreach (RichListViewItem ril in _itemList)
            {
                DrawTile(ril);
            }
        }
        private void DrawTile(RichListViewItem ril)
        {
            Panel p = new Panel();

            switch (_size)
            {
                case RichListViewItem.Format.MINUS:
                    p.Width = (int)RichListViewItem.Format.MINUS;
                    p.Height = 32;
                    break;
                case RichListViewItem.Format.SMALL:
                    p.Width = (int)RichListViewItem.Format.SMALL;
                    p.Height = 48;
                    break;
                case RichListViewItem.Format.MEDIUM:
                    p.Width = (int)RichListViewItem.Format.MEDIUM;
                    p.Height = 64;
                    break;
                case RichListViewItem.Format.LARGE:
                    p.Width = (int)RichListViewItem.Format.LARGE;
                    p.Height = 70;
                    break;
                case RichListViewItem.Format.AUDIO:
                    p.Width = 80;
                    p.Height = 120;
                    break;
            }

            _tilesColumCapacity = this.Width / (p.Width + 5);

            p.Name = ril.Text;
            p.Top = _panelY;
            p.Left = _panelX;
            p.BackColor = Color.White;

            CalculatePanelCoord(p);

            _panelComponentX = 50;
            _panelComponentY = 26;

            for (int i = 0; i < ril.Details.Count; i++)
            {
                switch (ril.Details[i].DetFamily)
                {
                    case RichListViewItem.Family.LABEL:
                        DrawPicture(p, ril);
                        DrawLabelText(p, ril);
                        DrawComponentLabel(p, ril.Details[i].DetValue);
                        break;
                    case RichListViewItem.Family.AUDIO:
                        DrawAudioTile(p, ril);
                        break;
                    case RichListViewItem.Family.PROGRESSBAR:
                        DrawPicture(p, ril);
                        DrawLabelText(p, ril);
                        DrawComponentProgressBar(p, ril.Details[i].DetValue);
                        break;
                }
            }

            this.Controls.Add(p);
        }
        private void DrawTransparentFilter()
        {
            //_transparentFilter = new TransparentPanel();
            //_transparentFilter.Top = 0;
            //_transparentFilter.Left = 0;
            //_transparentFilter.Width = this.Width;
            //_transparentFilter.Height = this.Height;
            //_transparentFilter.MouseHover += _transparentFilter_MouseHover;
            //_transparentFilter.MouseLeave += _transparentFilter_MouseLeave;
            //this.Controls.Add(_transparentFilter);
        }
        private void DrawPicture(Panel p, RichListViewItem ril)
        {
            PictureBox pb = new PictureBox();
            pb.Width = 50;
            pb.Height = 50;
            if (ril.Picture != null) pb.Image = ril.Picture;
            else pb.Image = this.IconList.Images[ril.ImageIndex > 0 ? ril.ImageIndex : 0];
            pb.Top = 0;
            pb.Left = 0;
            p.Controls.Add(pb);
        }
        private void DrawLabelText(Panel p, RichListViewItem ril)
        {
            Label lb_text = new Label();
            lb_text.Name = "text";
            lb_text.Text = ril.Text;
            lb_text.ForeColor = Color.Black;
            lb_text.Height = 20;
            lb_text.TextAlign = ContentAlignment.BottomLeft;
            if (ril.Details.Count > 0)
            {
                lb_text.Top = 0;
                lb_text.Left = 50;
            }
            else
            {
                lb_text.Top = 14;
                lb_text.Left = 50;
            }
            p.Controls.Add(lb_text);

        }
        private void DrawComponentLabel(Panel p, string val)
        {
            Label lb_desc = new Label();
            lb_desc.Name = "desc_" + val;
            lb_desc.Text = val;
            lb_desc.Top = _panelComponentY;
            lb_desc.Left = _panelComponentX;
            lb_desc.BackColor = Color.Transparent;
            lb_desc.Height = 20;
            lb_desc.AutoSize = true;
            lb_desc.ForeColor = Color.Gray;
            p.Controls.Add(lb_desc);

            _panelComponentX += lb_desc.Width + 4;
        }
        private void DrawComponentProgressBar(Panel p, string val)
        {
            int percent = 0;
            int.TryParse(val, out percent);

            RichProgressBar pba = new RichProgressBar();
            pba.Name = "desc_" + val;
            pba.Value = percent;
            pba.Maximum = 100;
            pba.Top = _panelComponentY;
            pba.Left = _panelComponentX;
            pba.Width = p.Width - 55;
            pba.Height = 12;
            pba.Style = ProgressBarStyle.Blocks;
            pba.ForeColor = percent > 90 ? Color.Red : Color.DeepSkyBlue;
            p.Controls.Add(pba);

            _panelComponentY += pba.Height + 4;
        }
        private void DrawAudioTile(Panel p, RichListViewItem ril)
        {
            PictureBox pb = new PictureBox();
            pb.Width = 75;
            pb.Height = 75;
            if (ril.Picture != null) pb.BackgroundImage = ril.Picture;
            pb.Top = 1;
            pb.Left = 1;
            pb.BackgroundImageLayout = ImageLayout.Zoom;
            pb.BackColor = Color.Black;
            p.Controls.Add(pb);
            p.Name = (ril.AssociatedObject as Track).Path_track;

            Button button_close = new Button();
            button_close.Width = 16;
            button_close.Height = 16;
            button_close.Top = 1;
            button_close.Left = 61;
            button_close.FlatStyle = FlatStyle.Flat;
            button_close.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button_close.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button_close.BackColor = Color.Transparent;
            button_close.BackgroundImage = Tools4Libraries.Resources.ResourceIconSet16Default.cross;// tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("close")];
            button_close.BackgroundImageLayout = ImageLayout.Stretch;
            button_close.BackColor = Color.Transparent;
            button_close.FlatAppearance.BorderSize = 0;
            button_close.Click += new EventHandler(button_close_Click);
            button_close.Name = "btnClose";
            button_close.Visible = true;
            pb.Controls.Add(button_close);
            
            Label labelTitle = new Label();
            labelTitle.Top = 80;
            labelTitle.Left = 0;
            labelTitle.Width = 80;
            labelTitle.Height = 14;
            labelTitle.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelTitle.TextAlign = ContentAlignment.TopCenter;
            labelTitle.ForeColor = Color.WhiteSmoke;
            labelTitle.BackColor = Color.Transparent;
            labelTitle.Text = (ril.AssociatedObject as Track).Title;
            labelTitle.ForeColor = Color.WhiteSmoke;
            p.Controls.Add(labelTitle);

            Label labelAlbum = new Label();
            labelAlbum.Top = 100;
            labelAlbum.Left = 0;
            labelAlbum.Width = 80;
            labelAlbum.Height = 14;
            labelAlbum.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelAlbum.TextAlign = ContentAlignment.TopCenter;
            labelAlbum.ForeColor = Color.WhiteSmoke;
            labelAlbum.BackColor = Color.Transparent;
            labelAlbum.ForeColor = Color.WhiteSmoke;
            labelAlbum.Text = (ril.AssociatedObject as Track).AlbumName;
            p.Controls.Add(labelAlbum);

            //Button btn_arrowLeft = new Button();
            //btn_arrowLeft.Width = 16;
            //btn_arrowLeft.Height = 16;
            //btn_arrowLeft.Top = 100;
            //btn_arrowLeft.Left = 0;
            //btn_arrowLeft.FlatStyle = FlatStyle.Flat;
            //btn_arrowLeft.FlatAppearance.MouseDownBackColor = Color.Transparent;
            //btn_arrowLeft.FlatAppearance.MouseOverBackColor = Color.Transparent;
            //btn_arrowLeft.BackColor = Color.Transparent;
            //btn_arrowLeft.BackgroundImage = Tools4Libraries.Resources.ResourceIconSet32Default.bullet_arrow_left;
            //btn_arrowLeft.BackgroundImageLayout = ImageLayout.Stretch;
            //btn_arrowLeft.BackColor = Color.Transparent;
            //btn_arrowLeft.FlatAppearance.BorderSize = 0;
            //btn_arrowLeft.Click += Btn_arrowLeft_Click;
            //btn_arrowLeft.Name = "btnArrowLeft";
            //p.Controls.Add(btn_arrowLeft);

            //Button btn_arrowRight = new Button();
            //btn_arrowRight.Width = 16;
            //btn_arrowRight.Height = 16;
            //btn_arrowRight.Top = 100;
            //btn_arrowRight.Left = 60;
            //btn_arrowRight.FlatStyle = FlatStyle.Flat;
            //btn_arrowRight.FlatAppearance.MouseDownBackColor = Color.Transparent;
            //btn_arrowRight.FlatAppearance.MouseOverBackColor = Color.Transparent;
            //btn_arrowRight.BackColor = Color.Transparent;
            //btn_arrowRight.BackgroundImage = Tools4Libraries.Resources.ResourceIconSet32Default.bullet_arrow_right;
            //btn_arrowRight.BackgroundImageLayout = ImageLayout.Stretch;
            //btn_arrowRight.BackColor = Color.Transparent;
            //btn_arrowRight.FlatAppearance.BorderSize = 0;
            //btn_arrowRight.Click += Btn_arrowRight_Click;
            //btn_arrowRight.Name = "btnArrowRight";
            //p.Controls.Add(btn_arrowRight);

            p.BackColor = Color.Black;
            p.BorderStyle = BorderStyle.FixedSingle;
            p.ForeColor = Color.Pink;
        }
        
        private void CalculatePanelCoord(Panel p)
        {
            if (_panelX + 5 + (2 * p.Width) > this.Width)
            {
                _panelX = 5;
                _panelY += 5 + p.Height;
            }
            else
            {
                _panelX += 5 + p.Width;
            }
        }
        private void ItemButtonShow()
        {
            Panel p;
            Button btn;
            foreach (var element in this.Controls)
            {
                if (element is Panel)
                {
                    p = element as Panel;
                    p.ForeColor = Color.DarkOrange;
                    foreach (var ctrl in p.Controls)
                    {
                        if (ctrl is PictureBox)
                        {
                            foreach (var item in (ctrl as PictureBox).Controls)
                            {
                                if (item is Button)
                                {
                                    btn = item as Button;
                                    if (btn.Name.Equals("btnPlay") || btn.Name.Equals("btnClose"))
                                    {
                                        btn.Visible = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void ItemButtonHide()
        {
            Panel p;
            Button btn;
            foreach (var element in this.Controls)
            {
                if (element is Panel)
                {
                    p = element as Panel;
                    p.ForeColor = Color.Gray;
                    foreach (var ctrl in p.Controls)
                    {
                        if (ctrl is PictureBox)
                        {
                            foreach (var item in (ctrl as PictureBox).Controls)
                            {
                                if (item is Button)
                                {
                                    btn = item as Button;
                                    if (btn.Name.Equals("btnPlay") || btn.Name.Equals("btnClose"))
                                    {
                                        btn.Visible = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void ItemSelectionChanged()
        {
            if (_selected)
            {
                this.BackColor = Color.DimGray;
            }
            else
            {
                this.BackColor = Color.Black;
            }
        }
        #endregion

        #region Event
        private void RichListView_Resize(object sender, EventArgs e)
        {
            RefreshComponent();
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            if (TileEvent != null) TileEvent("Close");

            Button btn = sender as Button;
            PictureBox pb = btn.Parent as PictureBox;
            Panel p = pb.Parent as Panel;

            foreach (RichListViewItem ril in _itemList.Where(i => (i.AssociatedObject as Track).Path_track.Equals(p.Name)).ToList())
            {
                _itemList.Remove(ril);
            }
            DrawTiles();

            //int_aud.Panau.ButtonsDisable();
            //this.trackLinked.Close();
            //this.Dispose();
            //OnClose(this, null);
        }
        private void button_pp_Click(object sender, EventArgs e)
        {
            if (TileEvent != null) TileEvent("PlayPause");
            //OnPP(sender, e);
        }
        private void Btn_arrowLeft_Click(object sender, EventArgs e)
        {

        }
        private void Btn_arrowRight_Click(object sender, EventArgs e)
        {

        }
        #endregion
    }
}