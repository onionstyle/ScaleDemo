using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ScaleDemo.Resources;
using System.Windows.Media;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Input.Touch;

namespace ScaleDemo
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _scaleManager=  new ScaleManager();
            ShowClipGrid.DataContext = _scaleManager.ImageControlBindingData;   //绑定图片数据
           
            _scaleManager.SetShowArea();    //设置显示区域

            TouchPanel.EnabledGestures = GestureType.FreeDrag;//可进行的TouchPanel操作
        }

    
        private void ShowImage_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {  
            bool oldIsPinch = _isPinch;
            _isPinch = e.PinchManipulation != null;

            if (oldIsPinch == true && _isPinch == true) 
            {
                _scaleManager.ScaleChange(e.PinchManipulation.DeltaScale, e.PinchManipulation.Current.Center);

                while (TouchPanel.IsGestureAvailable)   //获取Delta数据实现多点的移动
                {
                    GestureSample sample = TouchPanel.ReadGesture();
                    Point sampleDelta = new Point(sample.Delta.X, sample.Delta.Y);
                    _scaleManager.TranslationChange(sampleDelta, e.PinchManipulation.Current.Center);
                }
            }
            else
            {
                while (TouchPanel.IsGestureAvailable)   //将无用Gesture操作读取出来，不用
                {
                    GestureSample sample = TouchPanel.ReadGesture();    
                }
            }
        }

        private void ShowImage_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            _scaleManager.SuitRect();   //缩放完后居中不留空白
            _scaleManager.Create_And_Run_Animation(ShowImage);  //小于最佳比例时动画恢复
        }

        private void Open_Click(object sender, EventArgs e)
        {
            _scaleManager.OpenImage();

        }
        
        bool  _isPinch;
        ScaleManager _scaleManager;
    }
}