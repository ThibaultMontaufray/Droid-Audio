using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Tools4Libraries;

namespace Droid_Audio
{
    public partial class Demo : Form
    {
        #region Attribute
        private Interface_audio _intAudio;
        private Ribbon _ribbon;
        private RibbonButton _btn_open;
        private RibbonButton _btn_exit;
        #endregion

        #region Properties
        public Interface_audio InterfaceAudio
        {
            get { return _intAudio; }
            set { _intAudio = value; }
        }
        #endregion

        #region Constructor
        public Demo()
        {
            InitializeComponent();
            Init();
        }
        #endregion

        #region Methods public
        #endregion

        #region Methods private
        private void Init()
        {
            Tools4Libraries.Log.ApplicationAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Servodroid\Droid-Audio";
            Tools4Libraries.Log.LogLevel = 0;

            _intAudio = new Interface_audio();
            BuildRibbon();
            _intAudio.SheetMusic.Dock = DockStyle.Fill;
            this.Controls.Add(_intAudio.SheetMusic);
        }
        private void BuildRibbon()
        {
            _ribbon = new Ribbon();
            _ribbon.Height = 150;
            _ribbon.ThemeColor = RibbonTheme.Black;
            _ribbon.OrbDropDown.Width = 150;
            _ribbon.OrbStyle = RibbonOrbStyle.Office_2013;
            _ribbon.OrbText = GetText.Text("File");
            _ribbon.QuickAccessToolbar.MenuButtonVisible = false;
            _ribbon.QuickAccessToolbar.Visible = false;
            _ribbon.QuickAccessToolbar.MinSizeMode = RibbonElementSizeMode.Compact;
            _ribbon.Tabs.Add(_intAudio.Tsm);

            _btn_open = new RibbonButton(GetText.Text("Open"));
            _btn_open.Image = Tools4Libraries.Resources.ResourceIconSet32Default.open_folder;
            _btn_open.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.open_folder;
            _btn_open.Click += B_open_Click;
            _ribbon.OrbDropDown.MenuItems.Add(_btn_open);

            LoadRecentFiles();

            _btn_exit = new RibbonButton(GetText.Text("Exit"));
            _btn_exit.Image = Tools4Libraries.Resources.ResourceIconSet32Default.door_out;
            _btn_exit.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.door_out;
            _btn_exit.Click += B_exit_Click;
            _ribbon.OrbDropDown.MenuItems.Add(_btn_exit);

            _ribbon.OrbDropDown.Width = 700;
            this.Controls.Add(_ribbon);
        }
        private void LoadRecentFiles()
        {
            try
            {
                DateTime date;
                KeyValuePair<DateTime, string> movieItem;
                List<KeyValuePair<DateTime, string>> list = new List<KeyValuePair<DateTime, string>>();
                if (_intAudio != null && _intAudio.RecentAudio != null)
                {
                    foreach (var item in _intAudio.RecentAudio)
                    {
                        if (DateTime.TryParse(item.Split('#')[1], out date))
                        {
                            movieItem = new KeyValuePair<DateTime, string>(date, item.Split('#')[0].ToString());
                            list.Add(movieItem);
                        }
                    }
                }
                list.Sort(CompareMoviesRecentList);

                _ribbon.OrbDropDown.RecentItems.Clear();
                int maxFiles = list.Count > 16 ? 16 : list.Count;
                for (int i = list.Count; (i > list.Count - maxFiles) || (i < 0); i--)
                {
                    RibbonItem recentItem = new RibbonOrbRecentItem();
                    recentItem.Text = Path.GetFileName(list[i-1].Value);
                    recentItem.Value = list[i-1].Value;
                    recentItem.Click += RecentItem_Click;
                    _ribbon.OrbDropDown.RecentItems.Add(recentItem);
                }
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 0201 ] Cannot load recent files. \n" + exp.Message);
            }
        }
        static int CompareMoviesRecentList(KeyValuePair<DateTime, string> a, KeyValuePair<DateTime, string> b)
        {
            return a.Key.CompareTo(b.Key);
        }
        #endregion

        #region Event
        private void B_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void B_open_Click(object sender, EventArgs e)
        {
            _intAudio.Open(null);
        }
        private void RecentItem_Click(object sender, EventArgs e)
        {
            RibbonOrbRecentItem rori = sender as RibbonOrbRecentItem;
            _intAudio.Open(rori.Value);
        }
        #endregion
    }
}
