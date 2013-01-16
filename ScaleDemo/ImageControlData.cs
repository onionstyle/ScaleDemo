using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ScaleDemo
{

    /// <summary>
    /// 中间图片显示绑定源
    /// </summary>
    public class ImageControlData : INotifyPropertyChanged
    {
        #region Properties
        private RectangleGeometry _clip;

        /// <summary>
        /// clip宽度
        /// </summary>
        private int _clipWidth;
        /// <summary>
        /// clip高度
        /// </summary>
        private int _clipheight;
        /// <summary>
        /// clip边距
        /// </summary>
        private Thickness _clipMargin;

        /// <summary>
        /// image宽度
        /// </summary>
        private int _width;

        /// <summary>
        /// image高度
        /// </summary>
        private int _height;

        /// <summary>
        /// image边距
        /// </summary>
        private Thickness _margin;

        private ImageSource _imageSource;

        #endregion

        public RectangleGeometry Clip
        {
            get { return _clip; }
            set
            {
                _clip = value;
                RaisePropertyChanged("Clip");
            }
        }

        public int ClipWidth
        {
            get { return _clipWidth; }
            set
            {
                _clipWidth = value;
                RaisePropertyChanged("ClipWidth");
            }
        }

        public int ClipHeight
        {
            get { return _clipheight; }
            set
            {
                _clipheight = value;
                RaisePropertyChanged("ClipHeight");
            }
        }

        public Thickness ClipMargin
        {
            get { return _clipMargin; }
            set
            {
                _clipMargin = value;
                RaisePropertyChanged("ClipMargin");
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged("Width");
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged("Height");
            }
        }

        public Thickness Margin
        {
            get { return _margin; }
            set
            {
                _margin = value;
                RaisePropertyChanged("Margin");
            }
        }

        public ImageSource Source
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                RaisePropertyChanged("Source");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

}
