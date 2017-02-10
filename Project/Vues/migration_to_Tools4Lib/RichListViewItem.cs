using System.Collections.Generic;
using System.Drawing;

namespace Droid_Audio
{
    public struct Detail
    {
        public RichListViewItem.Family DetFamily;
        public string DetValue;
    }

    public class RichListViewItem
    {
        #region enum
        public enum Format
        {
            MINUS = 16,
            SMALL = 32,
            MEDIUM = 48,
            LARGE = 220,
            AUDIO = 120
        }
        public enum Family
        {
            LABEL,
            AUDIO,
            PROGRESSBAR
        }
        #endregion

        #region Attribute
        private string _text;
        private List<Detail> _details;
        private Format _format = Format.MEDIUM;
        private int _imageIndex;
        private string _group;
        private Image _picture;
        private bool _isSelected;
        private object _associatedObject;
        #endregion

        #region Properties
        public object AssociatedObject
        {
            get { return _associatedObject; }
            set { _associatedObject = value; }
        }
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }
        public int ImageIndex
        {
            get { return _imageIndex; }
            set { _imageIndex = value; }
        }
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        public List<Detail> Details
        {
            get { return _details; }
            set { _details = value; }
        }
        public Format Size
        {
            get { return _format; }
            set { _format = value; }
        }
        public Image Picture
        {
            get { return _picture; }
            set { _picture = value; }
        }
        #endregion

        #region Constructor
        public RichListViewItem()
        {
            _details = new List<Detail>();
        }
        public RichListViewItem(string text)
        {
            _details = new List<Detail>();
            this.Text = text;
        }
        #endregion
    }
}