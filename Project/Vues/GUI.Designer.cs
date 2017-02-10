/*
 * Created by SharpDevelop.
 * User: C357555
 * Date: 11/10/2011
 * Time: 18:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Droid_Audio
{
	partial class GUI
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
            this.imageListFicheAudio = new System.Windows.Forms.ImageList(this.components);
            this.imageListFicheAudio32 = new System.Windows.Forms.ImageList(this.components);
            this.imageListFicheAudio16 = new System.Windows.Forms.ImageList(this.components);
            this.imageListTreeview = new System.Windows.Forms.ImageList(this.components);
            this.pictureBoxNavigation = new System.Windows.Forms.PictureBox();
            this.imageListMenu = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNavigation)).BeginInit();
            this.SuspendLayout();
            // 
            // imageListFicheAudio
            // 
            this.imageListFicheAudio.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListFicheAudio.ImageStream")));
            this.imageListFicheAudio.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListFicheAudio.Images.SetKeyName(0, "void");
            this.imageListFicheAudio.Images.SetKeyName(1, "none");
            this.imageListFicheAudio.Images.SetKeyName(2, "artist");
            // 
            // imageListFicheAudio32
            // 
            this.imageListFicheAudio32.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListFicheAudio32.ImageStream")));
            this.imageListFicheAudio32.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListFicheAudio32.Images.SetKeyName(0, "forward");
            this.imageListFicheAudio32.Images.SetKeyName(1, "eject");
            this.imageListFicheAudio32.Images.SetKeyName(2, "next");
            this.imageListFicheAudio32.Images.SetKeyName(3, "back");
            this.imageListFicheAudio32.Images.SetKeyName(4, "rewind");
            this.imageListFicheAudio32.Images.SetKeyName(5, "loop");
            this.imageListFicheAudio32.Images.SetKeyName(6, "pause");
            this.imageListFicheAudio32.Images.SetKeyName(7, "play");
            this.imageListFicheAudio32.Images.SetKeyName(8, "stop");
            this.imageListFicheAudio32.Images.SetKeyName(9, "vol_red");
            this.imageListFicheAudio32.Images.SetKeyName(10, "vol_0");
            this.imageListFicheAudio32.Images.SetKeyName(11, "vol_1");
            this.imageListFicheAudio32.Images.SetKeyName(12, "vol_2");
            this.imageListFicheAudio32.Images.SetKeyName(13, "vol_3");
            this.imageListFicheAudio32.Images.SetKeyName(14, "vol_4");
            this.imageListFicheAudio32.Images.SetKeyName(15, "refresh_green");
            this.imageListFicheAudio32.Images.SetKeyName(16, "refreshLib");
            this.imageListFicheAudio32.Images.SetKeyName(17, "equalizer");
            this.imageListFicheAudio32.Images.SetKeyName(18, "import");
            // 
            // imageListFicheAudio16
            // 
            this.imageListFicheAudio16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListFicheAudio16.ImageStream")));
            this.imageListFicheAudio16.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListFicheAudio16.Images.SetKeyName(0, "close");
            this.imageListFicheAudio16.Images.SetKeyName(1, "back");
            this.imageListFicheAudio16.Images.SetKeyName(2, "eject");
            this.imageListFicheAudio16.Images.SetKeyName(3, "forward");
            this.imageListFicheAudio16.Images.SetKeyName(4, "loop");
            this.imageListFicheAudio16.Images.SetKeyName(5, "loop_green");
            this.imageListFicheAudio16.Images.SetKeyName(6, "next");
            this.imageListFicheAudio16.Images.SetKeyName(7, "pause");
            this.imageListFicheAudio16.Images.SetKeyName(8, "play");
            this.imageListFicheAudio16.Images.SetKeyName(9, "rewind");
            this.imageListFicheAudio16.Images.SetKeyName(10, "stop");
            this.imageListFicheAudio16.Images.SetKeyName(11, "vol_0");
            this.imageListFicheAudio16.Images.SetKeyName(12, "vol_1");
            this.imageListFicheAudio16.Images.SetKeyName(13, "vol_2");
            this.imageListFicheAudio16.Images.SetKeyName(14, "vol_3");
            this.imageListFicheAudio16.Images.SetKeyName(15, "vol_4");
            this.imageListFicheAudio16.Images.SetKeyName(16, "vol_x0");
            this.imageListFicheAudio16.Images.SetKeyName(17, "vol_x1");
            // 
            // imageListTreeview
            // 
            this.imageListTreeview.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTreeview.ImageStream")));
            this.imageListTreeview.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTreeview.Images.SetKeyName(0, "default");
            this.imageListTreeview.Images.SetKeyName(1, "folder");
            this.imageListTreeview.Images.SetKeyName(2, "folder_user");
            this.imageListTreeview.Images.SetKeyName(3, "folder_desktop");
            this.imageListTreeview.Images.SetKeyName(4, "Bibliotheque");
            this.imageListTreeview.Images.SetKeyName(5, "Album");
            this.imageListTreeview.Images.SetKeyName(6, "List");
            this.imageListTreeview.Images.SetKeyName(7, "Artist");
            this.imageListTreeview.Images.SetKeyName(8, "Title");
            this.imageListTreeview.Images.SetKeyName(9, "Selection");
            this.imageListTreeview.Images.SetKeyName(10, "Style");
            this.imageListTreeview.Images.SetKeyName(11, "Year");
            // 
            // pictureBoxNavigation
            // 
            this.pictureBoxNavigation.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.pictureBoxNavigation.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxNavigation.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxNavigation.BackgroundImage")));
            this.pictureBoxNavigation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBoxNavigation.Location = new System.Drawing.Point(0, 216);
            this.pictureBoxNavigation.Name = "pictureBoxNavigation";
            this.pictureBoxNavigation.Size = new System.Drawing.Size(427, 50);
            this.pictureBoxNavigation.TabIndex = 0;
            this.pictureBoxNavigation.TabStop = false;
            // 
            // imageListMenu
            // 
            this.imageListMenu.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMenu.ImageStream")));
            this.imageListMenu.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMenu.Images.SetKeyName(0, "MenuBackGround");
            this.imageListMenu.Images.SetKeyName(1, "MenuAlbumSelected");
            this.imageListMenu.Images.SetKeyName(2, "MenuArtistSelected");
            this.imageListMenu.Images.SetKeyName(3, "MenuFolderSelected");
            this.imageListMenu.Images.SetKeyName(4, "MenuTypeSelected");
            this.imageListMenu.Images.SetKeyName(5, "MenuAlbum");
            this.imageListMenu.Images.SetKeyName(6, "MenuAlbumHover");
            this.imageListMenu.Images.SetKeyName(7, "MenuArtist");
            this.imageListMenu.Images.SetKeyName(8, "MenuArtistHover");
            this.imageListMenu.Images.SetKeyName(9, "MenuFolder");
            this.imageListMenu.Images.SetKeyName(10, "MenuFolderHover");
            this.imageListMenu.Images.SetKeyName(11, "MenuType");
            this.imageListMenu.Images.SetKeyName(12, "MenuTypeHover");
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(427, 266);
            this.Controls.Add(this.pictureBoxNavigation);
            this.Name = "GUI";
            this.Text = "GUI";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNavigation)).EndInit();
            this.ResumeLayout(false);

		}
		public System.Windows.Forms.PictureBox pictureBoxNavigation;
		public System.Windows.Forms.ImageList imageListTreeview;
		public System.Windows.Forms.ImageList imageListFicheAudio16;
		public System.Windows.Forms.ImageList imageListFicheAudio32;
		public System.Windows.Forms.ImageList imageListFicheAudio;
		public System.Windows.Forms.ImageList imageListManager;
        public System.Windows.Forms.ImageList imageListMenu;
    }
}
