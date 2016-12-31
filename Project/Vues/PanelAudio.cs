// Log code : 23

using System;
using System.IO;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.Collections.Generic;
using Tools4Libraries;

namespace Droid_Audio
{
	public delegate void delegateMusic(object sender, EventArgs e);
	
	public partial class PanelAudio : Panel
	{
		#region Attribute
		private Interface_audio int_aud;
		private int index_fiche_selected;
		
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
		private Panel panel_right;
		
		private Panel panelControlLeft;
		private Panel panelControlMiddle;
		private Panel panelControlRight;
		
		private Label label_current_selection;
		private Panel panel_current_selection;
		private Panel panel_list_to_play;
		private Panel panel_navigation;
		
		private List<PanelFicheAudio> listPfaToPlay;
		private TreeView treeview_directory;
		private TreeView treeview_bibliotheque;
		
		private string displayMode;
		private int index_X;
		private int index_Y;
		private bool flagRefresh;
		private bool succeed;
		private string[] alpha = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
		//private string[] alpha = {"a", "b", "c"};
		private List<string> listAlpha;
		
		public event delegateMusic TicketSelectedChange;
		public event delegateMusic TicketPP;
		public event delegateMusic TicketClose;
		#endregion
		
		#region Properties
		public PanelFicheAudio CurrentPFA
		{
			get { return listPfaToPlay[index_fiche_selected]; }
		}
		
		public bool OpenedCurrentFile
		{
			get { return listPfaToPlay[index_fiche_selected].TrackLinked.FileOpened; }
		}
		
		private ToolStripMenuAudio tsm
		{
			get { return int_aud.Tsm as ToolStripMenuAudio; }
		}
		
		private int Index_X
		{
			get { return index_X; }
			set { index_X = value; }
		}
		
		private int Index_Y
		{
			get { return index_Y; }
			set { index_Y = value; }
		}
		#endregion
		
		#region Constructor
		public PanelAudio(Interface_audio inter_aud)
		{
			displayMode = "Artist";
			int_aud = inter_aud;
			listPfaToPlay = new List<PanelFicheAudio>();
			InitializeComponent();
			ButtonsDisable();
		}
		#endregion
		
		#region Methods public
		public new void Resize()
		{
			//this.Width = tsm.CurrentTabPage.Width;
			//this.Height = tsm.CurrentTabPage.Height;
			
			panelControlMiddle.Width = panel_middle.Width - 15 - panelControlRight.Width;
			panelControlMiddle.Height = panel_middle.Height - 10;
			
			panelControlLeft.Width = panel_left.Width - 5;
			panelControlLeft.Height = panel_left.Height - 10;
			
			panelControlRight.Width = panel_right.Width - 10;
			panelControlRight.Height = panel_right.Height - 10;
			
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
		}
		public void RefreshPanelLibrary()
		{
			try
			{
                bool headerDrawn = false;
                listAlpha = new List<string>(alpha);
				panelControlMiddle.Controls.Clear();
				Index_Y = 5;
				
				foreach (string s in listAlpha)
				{
                    headerDrawn = false;
					succeed = false;
					Index_X = 5;
					
					//if(displayMode.Equals("Album") || displayMode.Equals("Artist")) BuildLabelAlpha(s.ToUpper());
					
					foreach (Artist art in int_aud.ListArtist)
					{
						if (displayMode.Equals("Album"))
						{
							foreach (Album alb in art.ListAlbum)
							{
								if(alb.Name.Substring(0, 1).ToLower().Equals(s))
								{
                                    if (!headerDrawn)
                                    {
                                        BuildLabelAlpha(s.ToUpper());
                                        headerDrawn = true;
                                    }
									BuildAlbumView(alb);
									succeed = true;
								}
							}
						}
						else if(displayMode.Equals("Artist"))
						{
							if(art.Name.Substring(0, 1).ToLower().Equals(s))
                            {
                                if (!headerDrawn)
                                {
                                    BuildLabelAlpha(s.ToUpper());
                                    headerDrawn = true;
                                }
								BuildArtistView(art);
								succeed = true;
							}
						}
						else if(displayMode.Equals("Title"))
						{
							foreach (Album alb in art.ListAlbum)
							{
								if(alb.Name.Substring(0, 1).ToLower().Equals(s))
                                {
                                    if (!headerDrawn)
                                    {
                                        BuildLabelAlpha(s.ToUpper());
                                        headerDrawn = true;
                                    }
									Index_X = 5;
									BuildLabelAlpha(alb.Name);
									BuildTitleView(alb);
									succeed = true;
								}
							}
						}
						else
						{
							Log.write("[ ERR : 2320 ] Error, cannot make the filter with you're option : " + displayMode);
						}
					}
					if(succeed)
					{
						Index_Y += 139;
					}
				}
				panelControlMiddle.Invalidate();
			}
			catch (Exception exp2340)
			{
				Log.write("[ ERR : 2340 ] Cannot load music.\n" + exp2340.Message);
			}
		}
		public void TitleChanged()
		{
			if(index_fiche_selected<0 || string.IsNullOrEmpty(listPfaToPlay[index_fiche_selected].TrackLinked.Title))
			{
				label_current_selection.Text = "No title selected.";
				panel_navigation.Controls.Clear();
			}
			else
			{
				Cursor = System.Windows.Forms.Cursors.WaitCursor;
				label_current_selection.Text = listPfaToPlay[index_fiche_selected].TrackLinked.Title + " - " + listPfaToPlay[index_fiche_selected].TrackLinked.Album.Name;
				
				panel_navigation.Controls.Clear();
				try
				{
					panel_navigation.Controls.Add(listPfaToPlay[index_fiche_selected].TrackLinked.WaveViewer);
				}
				catch (Exception exp2345)
				{
					Log.write("[ DEB : 2345 ] Cannot load wave viewer.\n" + exp2345.Message);
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
		public bool AddTicket(Interface_audio inter_aud)
		{
			return LoadTicket(inter_aud, inter_aud.GetTrack("", "", inter_aud.PathFile));
		}
		#endregion
		
		#region Methods private
		private void RefreshTitle(List<Album> alb_list)
		{
			int refvalue;
			Index_Y = 5;
			
			panelControlMiddle.Controls.Clear();
			foreach (string s in listAlpha)
			{
				refvalue = Index_Y;
				succeed = false;
				foreach (Album alb in alb_list)
				{
					if(alb.Name.Substring(0, 1).ToLower().Equals(s))
					{
						Index_X = 5;
						BuildLabelAlpha(alb.Name);
						BuildTitleView(alb);
						succeed = true;
					}
				}
				if(succeed && (Index_Y < refvalue + 139))
				{
					Index_Y = refvalue + 139;
				}
			}
			panelControlMiddle.Invalidate();
		}
		
		private void InitializeComponent()
		{
			index_fiche_selected = 0;
			
			this.BackColor = Color.FromArgb(20, 20, 20);
			this.Width = int_aud.SheetMusic.Width;
			this.Height = int_aud.SheetMusic.Height;
			this.Dock = DockStyle.Fill;
			
			BuildPanel_Right();
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
			panel_list_to_play = new Panel();
			panel_list_to_play.Height = 130;
			panel_list_to_play.Dock = DockStyle.Bottom;
			panel_list_to_play.BackColor = Color.FromArgb(20, 20, 20);
			this.Controls.Add(panel_list_to_play);
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
			panel_left.Width = 150;
			panel_left.BackColor = Color.Transparent;
			this.Controls.Add(panel_left);
			
			panelControlLeft = new Panel();
			panelControlLeft.BackColor = Color.LightGray;
			panelControlLeft.Top = 5;
			panelControlLeft.Left = 5;
			panelControlLeft.Width = panel_left.Width - 5;
			panelControlLeft.Height = panel_left.Height - 10;
			panel_left.Controls.Add(panelControlLeft);
			
			BuildTreeView();
			panelControlLeft.Controls.Add(treeview_directory);
		}
		
		private void BuildPanel_Middle()
		{
			panel_middle = new Panel();
			panel_middle.Dock = DockStyle.Fill;
			panel_middle.BackColor = Color.Transparent;
			this.Controls.Add(panel_middle);
			
			panelControlMiddle = new Panel();
			panelControlMiddle.BackColor = Color.DarkGray;
			panelControlMiddle.Top = 5;
			panelControlMiddle.Left = 5;
			panelControlMiddle.Width = panel_middle.Width - 10 - 150;
			panelControlMiddle.Height = panel_middle.Height - 10 - 150;
			panelControlMiddle.AutoScroll = true;
			panel_middle.Controls.Add(panelControlMiddle);
		}
		
		private void BuildPanel_Right()
		{
			panel_right = new Panel();
			panel_right.Dock = DockStyle.Right;
			panel_right.Width = 150;
			panel_right.BackColor = Color.Transparent;
			this.Controls.Add(panel_right);
			
			panelControlRight = new Panel();
			panelControlRight.BackColor = Color.LightGray;
			panelControlRight.Top = 5;
			panelControlRight.Left = 5;
			panelControlRight.Width = panel_right.Width - 10;
			panelControlRight.Height = panel_right.Height - 10;
			panel_right.Controls.Add(panelControlRight);
			
			BuildBibliotheque();
		}
		
		private void BuildTreeView()
		{
			treeview_directory = new TreeView();
			treeview_directory.Dock = DockStyle.Fill;
			treeview_directory.BackColor = Color.LightGray;
			treeview_directory.ImageList = tsm.Gui.imageListTreeview;
			treeview_directory.AfterSelect += new TreeViewEventHandler(treeview_directory_AfterSelect);
			
//			TreeNode tn_my_doc = new TreeNode("Mes documents");
//			tn_my_doc.ImageKey = "folder_user";
//			//string dirPath = @"\\Atlas\C357555\User\";
//			string dirPath = @"Amos";
//			List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
//			foreach (var dir in dirs)
//			{
//				TreeNode tn = new TreeNode(dir.Substring(dir.LastIndexOf("\\") + 1));
//				tn.ImageKey = "folder";
//				tn_my_doc.Nodes.Add(tn);
//			}
//			treeview_directory.Nodes.Add(tn_my_doc);
			
//			// Répertoire spéciaux
			//            Console.WriteLine("Répertoire spéciaux");
			//            Environment.SpecialFolder[] sfe = (Environment.SpecialFolder[])
			//            Enum.GetValues(typeof(Environment.SpecialFolder));
			//            for (int i = 0; i < sfe.Length; i++)
			//            {
			//                //Console.WriteLine(Environment.GetFolderPath(sfe[i]));
			//            	TreeNode tn = new TreeNode(Environment.GetFolderPath(sfe[i]));
//				tn.ImageKey = "folder";
//				treeview_directory.Nodes.Add(tn);
			//            }
			
//			TreeNode tn_my_desktop = new TreeNode("Bureau");
//			tn_my_desktop.ImageKey = "folder_desktop";
//			dirPath = @"C:\Documents and Settings\All Users\Bureau";
//			dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
//			foreach (var dir in dirs)
//			{
//				TreeNode tn = new TreeNode(dir.Substring(dir.LastIndexOf("\\") + 1));
//				tn.ImageKey = "folder";
//				tn_my_desktop.Nodes.Add(tn);
//			}
//			treeview_directory.Nodes.Add(tn_my_desktop);
			
			TreeNode tn_my_c = new TreeNode("C:");
			tn_my_c.ImageKey = "folder";
			treeview_directory.Nodes.Add(tn_my_c);
			
		}
		
		private void BuildBibliotheque()
		{
			treeview_bibliotheque = new TreeView();
			treeview_bibliotheque.BackColor = Color.LightGray;
			treeview_bibliotheque.Dock = DockStyle.Fill;
			treeview_bibliotheque.ImageList = tsm.Gui.imageListTreeview;
			treeview_bibliotheque.AfterSelect += new TreeViewEventHandler(treeview_bibliotheque_AfterSelect);
			panelControlRight.Controls.Add(treeview_bibliotheque);
//
			// TODO : add in the further version
//			TreeNode nodeSelection = new TreeNode("Selection");
//			nodeSelection.ImageKey = "Selection";
//			treeview_bibliotheque.Nodes.Add(nodeSelection);
//
			TreeNode nodeBibliotheque = new TreeNode("Bibliotheque");
			nodeBibliotheque.ImageKey = "Bibliotheque";
			nodeBibliotheque.ExpandAll();
			treeview_bibliotheque.Nodes.Add(nodeBibliotheque);
			
			TreeNode nodeArtist = new TreeNode("Artist");
			nodeArtist.ImageKey = "Artist";
			nodeBibliotheque.Nodes.Add(nodeArtist);
			
			TreeNode nodeAlbum = new TreeNode("Album");
			nodeAlbum.ImageKey = "Album";
			nodeBibliotheque.Nodes.Add(nodeAlbum);
			
			TreeNode nodeTitle = new TreeNode("Title");
			nodeTitle.ImageKey = "Title";
			nodeBibliotheque.Nodes.Add(nodeTitle);
//
			// TODO : add this in the further version
//			TreeNode nodeYear = new TreeNode("Year");
//			nodeYear.ImageKey = "Year";
//			nodeBibliotheque.Nodes.Add(nodeYear);
//
//			TreeNode nodeStyle = new TreeNode("Style");
//			nodeStyle.ImageKey = "Style";
//			nodeBibliotheque.Nodes.Add(nodeStyle);
			
		}
		
		private void BuildLabelAlpha(string lettre)
		{
			Label labelAlpha = new Label();
			labelAlpha.Top = Index_Y;
			labelAlpha.Left = Index_X;
			labelAlpha.Text = lettre;
			labelAlpha.Width = panelControlMiddle.Width;
			labelAlpha.Height = 14;
			panelControlMiddle.Controls.Add(labelAlpha);
			Index_Y += 6;
			
			Label labelSeparation = new Label();
			labelSeparation.Width = panelControlMiddle.Width;
			labelSeparation.Height = 14;
			for (int i=0 ; i<panelControlMiddle.Width/6 ; i++) labelSeparation.Text += "_";
			labelSeparation.Top = Index_Y;
			labelSeparation.Left = Index_X;
			panelControlMiddle.Controls.Add(labelSeparation);
			Index_Y += 18;
		}
		
		private void BuildArtistView(Artist art)
		{
			if (art.ListAlbum.Count > 1)
			{
				PanelAlbum pa = new PanelAlbum(int_aud, art.ListAlbum, "Artist");
				pa.TicketClick += new delegatePanelAlbum(pa_TicketClick);
				pa.Top = Index_Y;
				pa.Left = Index_X;
				panelControlMiddle.Controls.Add(pa);
				Index_X += pa.Width + 5;
			}
			else
			{
				PanelAlbum pa = new PanelAlbum(int_aud, art.ListAlbum[0], "Artist");
				pa.TicketClick += new delegatePanelAlbum(pa_TicketClick);
				pa.Top = Index_Y;
				pa.Left = Index_X;
				panelControlMiddle.Controls.Add(pa);
				Index_X += pa.Width + 5;
			}
		}
		
		private void BuildAlbumView(Album alb)
		{
			PanelAlbum pa = new PanelAlbum(int_aud, alb, "Album");
			pa.Top = Index_Y;
			pa.Left = Index_X;
			pa.TicketClick += new delegatePanelAlbum(pa_TicketClick);
			panelControlMiddle.Controls.Add(pa);
			Index_X += pa.Width + 5;
		}
		
		private void BuildTitleView(Album alb)
		{
			int refindex = Index_Y;
			
			PanelAlbum pa = new PanelAlbum(int_aud, alb, "Album");
			pa.TicketClick += new delegatePanelAlbum(pa_TicketClick);
			pa.Top = Index_Y;
			pa.Left = Index_X;
			panelControlMiddle.Controls.Add(pa);
			
			Label labelTrack;
			foreach (Track t in alb.ListTrack)
			{
				labelTrack = new Label();
				labelTrack.Width = panelControlMiddle.Width - 150;
				labelTrack.BackColor = Color.LightGray;
				labelTrack.Height = 16;
				labelTrack.Top = Index_Y;
				labelTrack.Left = Index_X + 110;
				labelTrack.Text = t.Title;
				labelTrack.Name = alb.My_Artist.Name + "|" + alb.Name + "|" + t.Path_track;
				labelTrack.MouseHover += new EventHandler(labelTrack_MouseHover);
				labelTrack.MouseLeave += new EventHandler(labelTrack_MouseLeave);
				labelTrack.MouseDoubleClick += new MouseEventHandler(labelTrack_MouseDoubleClick);
				panelControlMiddle.Controls.Add(labelTrack);
				Index_Y += 16;
			}
			
			if (Index_Y < refindex + pa.Height) Index_Y = refindex + pa.Height;
		}
		
		private void BuildButtons()
		{
			button_eject = new Button();
			button_eject.Top = 5;
			button_eject.Left = 0;
			button_eject.Width = 16;
			button_eject.Height = 16;
			button_eject.FlatStyle = FlatStyle.Flat;
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
				PanelFicheAudio pfa;
				//Track t = inter_aud.GetTrack("", "", inter_aud.Path);
				
				pfa = new PanelFicheAudio(inter_aud, t);
				pfa.Top = 5;
				pfa.Left = 5 + (listPfaToPlay.Count*90);
				pfa.Enabled = true;
				pfa.CloseEvent += new delegateMusicTicket(pfa_CloseEvent);
				pfa.PPEvent += new delegateMusicTicket(pfa_PPEvent);
				panel_list_to_play.Controls.Add(pfa);
				listPfaToPlay.Add(pfa);
				
				if (listPfaToPlay.Count>1) listPfaToPlay[index_fiche_selected].IsSeleccted = false;
				index_fiche_selected = listPfaToPlay.Count - 1;
				OnTicketSelectedChange(listPfaToPlay[index_fiche_selected], null);
				return true;
			}
			catch (Exception exp2300)
			{
				Log.write("[ ERR : 2300 ] cannot load music data.\n" + exp2300.Message);
				return false;
			}
		}
		
		private void UpdateTicketList()
		{
			for(int i=0 ; i<listPfaToPlay.Count ; i++)
			{
				panel_list_to_play.Controls.Remove(listPfaToPlay[i]);
				listPfaToPlay[i].Top = 5;
				listPfaToPlay[i].Left = 5 + (i*90);
				panel_list_to_play.Controls.Add(listPfaToPlay[i]);
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
		private void treeview_bibliotheque_AfterSelect(object sender, TreeViewEventArgs e)
		{
			flagRefresh = false;
			if (treeview_bibliotheque.SelectedNode != null)
			{
				treeview_bibliotheque.SelectedImageKey = treeview_bibliotheque.SelectedNode.Text;
				if(treeview_bibliotheque.SelectedNode.Text.Equals("Album") || treeview_bibliotheque.SelectedNode.Text.Equals("Artist") || treeview_bibliotheque.SelectedNode.Text.Equals("Title"))
				{
					displayMode = treeview_bibliotheque.SelectedNode.Text;
					if (!flagRefresh)
					{
						flagRefresh = true;
						
						this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
						RefreshPanelLibrary();
						this.Cursor = System.Windows.Forms.Cursors.Default;
					}
				}
			}
		}
		
		private void treeview_directory_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (treeview_directory.SelectedNode != null)
			{
				treeview_directory.SelectedImageKey = "folder";
			}
		}
		
		private void pfa_CloseEvent(object sender, EventArgs e)
		{
			OnTicketClose(listPfaToPlay[index_fiche_selected], null);
			
			listPfaToPlay.Remove(sender as PanelFicheAudio);
			if(listPfaToPlay.Count-1 < index_fiche_selected) index_fiche_selected --;
			if(index_fiche_selected>=0)
			{
				listPfaToPlay[index_fiche_selected].IsSeleccted=true;
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
			panel_navigation.Controls.Add(listPfaToPlay[index_fiche_selected].TrackLinked.WaveViewer);
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
			
			LoadTicket(int_aud, int_aud.GetTrack(artist, album, path));
		}

		private void labelTrack_MouseLeave(object sender, EventArgs e)
		{
			Label l = sender as Label;
			l.ForeColor = Color.Black;
			l.BackColor = Color.LightGray;
		}
		
		private void labelTrack_MouseHover(object sender, EventArgs e)
		{
			Label l = sender as Label;
			l.ForeColor = Color.Black;
			l.BackColor = Color.LightBlue;
		}
		
		private void pa_TicketClick(object sender, EventArgs e)
		{
			PanelAlbum pa = sender as PanelAlbum;
			if (pa != null)
			{
				RefreshTitle(pa.TicketArtist.ListAlbum);
			}
		}

		private void pfa_PPEvent(object sender, EventArgs e)
		{
			OnTicketPP(sender, e);
		}
		
		private void button_pp_Click(object sender, EventArgs e)
		{
			int_aud.GoAction("pp");
			if(CurrentPFA.TrackLinked.IsPaused) button_pp.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("play")];
			else button_pp.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("pause")];
		}

		private void button_stop_Click(object sender, EventArgs e)
		{
			int_aud.GoAction("stop");
			button_pp.Image = tsm.Gui.imageListFicheAudio16.Images[tsm.Gui.imageListFicheAudio16.Images.IndexOfKey("play")];
		}

		private void button_rewind_Click(object sender, EventArgs e)
		{
			int_aud.GoAction("rewind");
		}

		private void button_forward_Click(object sender, EventArgs e)
		{
			int_aud.GoAction("forward");
		}

		private void button_next_Click(object sender, EventArgs e)
		{
			int_aud.GoAction("next");
		}
		
		private void button_back_Click(object sender, EventArgs e)
		{
			int_aud.GoAction("back");
		}

		private void button_vol_Click(object sender, EventArgs e)
		{
			int_aud.GoAction("volume");
		}

		private void button_eject_Click(object sender, EventArgs e)
		{
			int_aud.GoAction("eject");
		}

		private void button_loop_Click(object sender, EventArgs e)
		{
			int_aud.GoAction("loop");
		}
		#endregion
	}
}
