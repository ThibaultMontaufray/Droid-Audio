// Log code 41

using System;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using Assistant;
using Tools4Libraries;

namespace Droid_Audio
{
	public class ToolStripMenuAudio : RibbonTab
    {
		#region Attributes
		private GUI gui;
		private RibbonPanel panelTools;
        private RibbonButton rb_refreshLibrary;
        private RibbonButton rb_equalizer;
        private RibbonButton rb_import;
        #endregion
		
		#region Properties
		public GUI Gui
		{
			get { return gui; }
		}
		#endregion
		
		#region Constructor
		public ToolStripMenuAudio(List<string> theList)
		{
			try
			{
				gui = new GUI();

                rb_refreshLibrary = new RibbonButton("Refresh library");
                rb_refreshLibrary.Click += new EventHandler(tsb_refreshLib_Click);
                rb_refreshLibrary.Image = gui.imageListFicheAudio32.Images[gui.imageListFicheAudio32.Images.IndexOfKey("refreshLib")];

                rb_equalizer = new RibbonButton("Equalizer");
                rb_equalizer.Click += new EventHandler(tsb_equlizer_Click);
                rb_equalizer.Image = gui.imageListFicheAudio32.Images[gui.imageListFicheAudio32.Images.IndexOfKey("equalizer")];
                rb_equalizer.Enabled = false;

                rb_import = new RibbonButton("Import");
                rb_import.Click += new EventHandler(tsb_import_Click);
                rb_import.Image = gui.imageListFicheAudio32.Images[gui.imageListFicheAudio32.Images.IndexOfKey("import")];

                panelTools = new RibbonPanel();
                panelTools.Text = "Tools";
                panelTools.Items.Add(rb_refreshLibrary);
                panelTools.Items.Add(rb_equalizer);
                panelTools.Items.Add(rb_import);

                this.Panels.Add(panelTools);
				this.Text = "Music";
			}
			catch (Exception exp4100)
			{
				Log.write("[ CRT : 4100 ] Cannot open audio menu.\n" + exp4100.Message);
				this.Dispose(null);
			}
		}
		#endregion
		
		#region Methods
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
		#endregion
		
		#region Events
		public event EventHandlerAction ActionAppened;

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
		public void OnAction(EventArgs e)
		{
			if (ActionAppened != null) ActionAppened(this, e);
		}
		#endregion
	}
}
