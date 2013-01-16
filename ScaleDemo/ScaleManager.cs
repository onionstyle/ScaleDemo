using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ScaleDemo
{
    class ScaleManager
    {
        public ScaleManager()
        {
            _photoChooserTask = new PhotoChooserTask();
            _photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
        }
        /// <summary>
        /// 设置可显示的区域
        /// </summary>
        public void SetShowArea()
        {
            //裁剪显示区域，隐藏超出画布的部分
            _imageControl.ClipWidth = MaxWidth;
            _imageControl.ClipHeight = MaxHeight;
            _imageControl.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, _imageControl.ClipWidth, _imageControl.ClipHeight) };
        }

        public void OpenImage()
        {
            _photoChooserTask.Show();
        }
        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                _imageData= new WriteableBitmap(bmp);
                _imageControl.Source = _imageData;
                _imageControl.Width = _imageData.PixelWidth;
                _imageControl.Height = _imageData.PixelHeight;
                //超最大宽高整张图按比例缩放
                Restrict();
                //保存默认最佳宽高
                _bestWidth = _imageControl.Width;  
                _bestHeight = _imageControl.Height;

                //设置ImageGrid边距使其居中
                _imageControl.Margin = new Thickness((_imageControl.ClipWidth - _imageControl.Width) / 2, (_imageControl.ClipHeight - _imageControl.Height) / 2, 0, 0);

            }
        }

        public void ScaleChange(double scale, Point p)
        {
            double rate = ImageDataRate;
            //原来的宽高
            double width = _imageControl.Width;
            double height = _imageControl.Height;

            // 缩放图片               
            _imageControl.Width = Convert.ToInt32(_imageControl.Width * scale);
            _imageControl.Height = Convert.ToInt32(_imageControl.Width * rate);

            Restrict(_imageData.PixelWidth * 3, _imageData.PixelHeight * 3, Convert.ToInt32(_bestWidth* 0.5), Convert.ToInt32(_bestHeight*0.5));

            double scaleX = _imageControl.Width / width;
            double scaleY = _imageControl.Height / height;
            //使其在点击的点上进行缩放        
            _imageControl.Margin = new Thickness(_imageControl.Margin.Left - p.X * (scaleX - 1), _imageControl.Margin.Top - p.Y * (scaleY - 1), 0, 0);
        }   //缩放

        public void TranslationChange(Point translate,Point origin,double ShowWidth = 0, double ShowHeight = 0, double reX = 0, double reY = 0)
        {
            if (ShowWidth == 0 || ShowHeight == 0)
            {
                ShowWidth = _imageControl.ClipWidth;
                ShowHeight = _imageControl.ClipHeight;
            }
            if (reX == 0 || reY == 0)
            {
                reX = _imageControl.ClipWidth / 2;
                reY = _imageControl.ClipHeight / 2;
            }
            //计算图片位置
            Point p = new Point(translate.X + _imageControl.Margin.Left, translate.Y + _imageControl.Margin.Top);

           
            //计算手指位置
            origin.X += _imageControl.Margin.Left;
            origin.Y += _imageControl.Margin.Top;
            //手指X坐标在显示内才判断移动图片
            if (origin.X > 0 && origin.X < ShowWidth)
            {
                if (p.X < -_imageControl.Width + reX)
                {
                    p.X = -_imageControl.Width + reX;
                }
                else
                {
                    if (p.X > ShowWidth - reX)
                    {
                        p.X = ShowWidth - reX;
                    }
                }
            }
            else
            {
                p.X = _imageControl.Margin.Left;
            }
            if (origin.Y > 0 && origin.Y < ShowHeight)
            {
                if (p.Y < -_imageControl.Height + reY)
                {
                    p.Y = -_imageControl.Height + reY;
                }
                else
                {
                    if (p.Y > ShowHeight - reY)
                    {
                        p.Y = ShowHeight - reY;
                    }
                }
            }
            else
            {
                p.Y = _imageControl.Margin.Top;
            }
            _imageControl.Margin = new Thickness(p.X, p.Y, 0, 0);
        }   //移动
       
        /// <summary>
        /// 缩放恢复动画
        /// </summary>
        public void Create_And_Run_Animation(FrameworkElement element)
        {
            double width = _bestWidth;
            double height = _bestHeight;
            if (element.Width >= width || element.Height >= height)
                return;
            //变化矩阵
            CompositeTransform moveTransform = element.RenderTransform as CompositeTransform;
            
            //创建两个DoubleAnimation于用于长宽的变化
            DoubleAnimation myDoubleAnimationScanleX = new DoubleAnimation();
            DoubleAnimation myDoubleAnimationScanleY = new DoubleAnimation();

            //变化的时间
            myDoubleAnimationScanleX.Duration = new Duration(TimeSpan.FromSeconds((float)0.3));
            myDoubleAnimationScanleY.Duration = myDoubleAnimationScanleX.Duration;

            //0表示初始未变化
            myDoubleAnimationScanleX.From = 1;
            myDoubleAnimationScanleY.From = 1;

            //变换的比例
            double tempRateX = width / element.Width;
            double tempRateY = height / element.Height;
            myDoubleAnimationScanleX.To = tempRateX;
            myDoubleAnimationScanleY.To = tempRateY;

            //设置要变化的对象
            Storyboard.SetTarget(myDoubleAnimationScanleX, moveTransform);
            Storyboard.SetTarget(myDoubleAnimationScanleY, moveTransform);

            //设置要变化的属性
            Storyboard.SetTargetProperty(myDoubleAnimationScanleX, new PropertyPath(CompositeTransform.ScaleXProperty));
            Storyboard.SetTargetProperty(myDoubleAnimationScanleY, new PropertyPath(CompositeTransform.ScaleYProperty));

            //变化主体
            Storyboard sb = new Storyboard();
            sb.Duration = myDoubleAnimationScanleX.Duration;

            sb.Children.Add(myDoubleAnimationScanleX);
            sb.Children.Add(myDoubleAnimationScanleY);

            // 开始变化
            sb.Begin();

            sb.Completed += delegate
            {
                //等待变化结束设置图片
                moveTransform.ScaleX = 1;
                moveTransform.ScaleY = 1;

                _imageControl.Width = (int)width;
                _imageControl.Height = (int)height;
                //设置ImageGrid边距使其居中
                _imageControl.Margin = new Thickness((_imageControl.ClipWidth - _imageControl.Width) / 2, (_imageControl.ClipHeight - _imageControl.Height) / 2, 0, 0);

            };
        }

        public void Restrict(int maxWidth = 0, int maxHeight = 0, int minWidth = 0, int minHeight = 0)
        {
            double rate = ImageDataRate;
            maxWidth = maxWidth != 0 ? maxWidth : MaxWidth;
            maxHeight = maxHeight != 0 ? maxHeight : MaxHeight;
            minWidth = minWidth != 0 ? minWidth : MinWidth;
            minHeight = minHeight != 0 ? minHeight : MinHeight;

            if (_imageControl.Width < minWidth)         //在宽度范围是否小于最小值
            {
                _imageControl.Width = minWidth;
                _imageControl.Height = Convert.ToInt32(_imageControl.Width * rate);
            }
            if (_imageControl.Height < minHeight)       //判断高度范围是否小于最小值
            {
                _imageControl.Height = minHeight;
                _imageControl.Width = Convert.ToInt32(_imageControl.Height / rate);
            }


            double widthRate = _imageControl.Width / (double)maxWidth;
            double heightRate = _imageControl.Height / (double)maxHeight;
            double greateRate = widthRate > heightRate ? widthRate : heightRate;

            if (greateRate > 1)     //最后以最大值为限制结束缩放
            {
                _imageControl.Width = Convert.ToInt32(_imageControl.Width / greateRate);
                _imageControl.Height = Convert.ToInt32(_imageControl.Width * rate);
            }
        }

        /// <summary>
        /// 使图片显示不留空白
        /// </summary>
        public void SuitRect()
        {
            //现在边距的长度
            double left = _imageControl.Margin.Left;
            double top = _imageControl.Margin.Top;

            //判断居中或者适应不留空白
            if (_imageControl.Width < _imageControl.ClipWidth)
            {
                left = (_imageControl.ClipWidth - _imageControl.Width) / 2;
            }
            else
            {
                //最大左边距 为0 不能留空白
                double maxLeft = 0;
                //最小为负数 右边不能留空白   
                double minLeft = _imageControl.ClipWidth - _imageControl.Width;

                left = left > maxLeft ? maxLeft : (left < minLeft)?minLeft : left;
            }

            if (_imageControl.Height < _imageControl.ClipHeight)
            {
                top = (_imageControl.ClipHeight - _imageControl.Height) / 2;
            }
            else
            {
                double maxTop = 0;
                double minTop = _imageControl.ClipHeight - _imageControl.Height;

                top = top > maxTop ? maxTop : top < minTop ? minTop : top;
            }
            _imageControl.Margin = new Thickness(left, top, 0, 0);
        }

        #region Properties
       /// <summary>
       /// 初始最佳显示宽
       /// </summary>
        double _bestWidth;
        /// <summary>
        /// 初始最佳显示高
        /// </summary>
        double _bestHeight;

        /// <summary>
        /// 图片显示最大高度
        /// </summary>
        int _maxHeight = (int)Application.Current.Host.Content.ActualHeight-104;
        public int MaxHeight
        {
            get { return _maxHeight; }
        }

        /// <summary>
        /// 图片显示最大宽度
        /// </summary>
        int _maxWidth = (int)Application.Current.Host.Content.ActualWidth;
        public int MaxWidth
        {
            get { return _maxWidth; }
        }

        private readonly int MinWidth = 100;
        private readonly int MinHeight = 100;

        private PhotoChooserTask _photoChooserTask ;
        WriteableBitmap _imageData;
        //长宽比例
        public double ImageDataRate
        {
            get { return _imageData.PixelHeight / (double)_imageData.PixelWidth; }
        }
        /// <summary>
        /// 图片数据绑定
        /// </summary>
        protected ImageControlData _imageControl;
        public ImageControlData ImageControlBindingData
        {
            get { return _imageControl ?? (_imageControl = new ImageControlData()); }
        }
        #endregion
    }
}
