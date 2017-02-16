// Log code : 74 01

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools4Libraries;

namespace Droid_Audio
{
    public partial class Downloader : Form
    {
        #region Attribute
        private YoutubeExtractor _extractor;
        private Interface_audio _intAud;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public Downloader(Interface_audio intAud)
        {
            _intAud = intAud;
            InitializeComponent();
            buttonExtract.Enabled = false;
            textBoxUrl.TextChanged += TextBoxUrl_TextChanged;
            textBoxFilePath.TextChanged += TextBoxFilePath_TextChanged;
            this.Disposed += Downloader_Disposed;

            _extractor = new Droid_Audio.YoutubeExtractor(_intAud);
            _extractor.ProgressionChanged += YoutubeExtractor_ProgressionChanged;
            _extractor.CannotExtractAudio += YoutubeExtractor_CannotExtractAudio;
            _extractor.TitleChanged += YoutubeExtractor_TitleChanged;
        }

        #endregion

        #region Methods public
        #endregion

        #region Methods private
        private void ProcessExtraction()
        {
            if (Directory.Exists(textBoxFilePath.Text))
            {
                textBoxFilePath.BackColor = Color.White;
                DisableControls();
                Extraction();
            }
            else
            {
                textBoxFilePath.BackColor = Color.LightYellow;
            }
        }
        private void DisableControls()
        {
            textBoxFilePath.Enabled = false;
            textBoxUrl.Enabled = false;
            buttonBrowse.Enabled = false;
            buttonExtract.Enabled = false;
        }
        private void EnableControls()
        {
            textBoxFilePath.Enabled = true;
            textBoxUrl.Enabled = true;
            buttonBrowse.Enabled = true;
            buttonExtract.Enabled = true;
        }
        private bool Extraction()
        {
            try
            {
                _extractor.YoutubeToAudioFile(textBoxUrl.Text, textBoxFilePath.Text);
                return true;
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 7400 ] Error while extracting video from youtube.\n\n" + exp.Message);
                return false;
            }
        }
        #endregion

        #region Event
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = fbd.SelectedPath;
            }
        }
        private void buttonExtract_Click(object sender, EventArgs e)
        {
            ProcessExtraction();
        }
        private void YoutubeExtractor_ProgressionChanged(object o)
        {
            progressBar1.Value = (int) o;
            labelPercent.Text = string.Format("Extraction : {0} %", progressBar1.Value.ToString("N0"));
            if (progressBar1.Value >= 100) { this.Close(); }
        }
        private void YoutubeExtractor_CannotExtractAudio(object o)
        {
            this.Text = "Download : cannot convert that file";
        }
        private void YoutubeExtractor_TitleChanged(object o)
        {
            if (o != null && o is string)
            { 
                this.Text = "Download : " + o.ToString();
            }
            else
            {
                this.Text = "Download in progress";
            }
        }
        private void TextBoxUrl_TextChanged(object sender, EventArgs e)
        {
            this.Text = "Download";
            buttonExtract.Enabled = (!string.IsNullOrEmpty(textBoxFilePath.Text) && !string.IsNullOrEmpty(textBoxUrl.Text));
        }
        private void TextBoxFilePath_TextChanged(object sender, EventArgs e)
        {
            buttonExtract.Enabled = (!string.IsNullOrEmpty(textBoxFilePath.Text) && !string.IsNullOrEmpty(textBoxUrl.Text));
        }
        private void Downloader_Disposed(object sender, EventArgs e)
        {
            try
            {
                string downloadedFile = string.Empty;
                if (!string.IsNullOrEmpty(_extractor.Title))
                {
                    downloadedFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Servodroid\Droid-Audio", _extractor.Title);
                }
                _extractor.Abort();
                if (File.Exists(downloadedFile + ".mp4"))
                {
                    File.Delete(downloadedFile + ".mp4");
                }
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 7401 ] Error in aborting the download.\n\n" + exp.Message);
            }
        }
        #endregion
    }
}
