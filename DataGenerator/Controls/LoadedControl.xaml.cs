using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DataGenerator.Controls
{
    /// <summary>
    /// LoadedControl.xaml 的交互逻辑
    /// </summary>
    public partial class LoadedControl : UserControl
    {
        /// <summary>
        /// 让此界面1秒钟后才显示
        /// </summary>
        private DispatcherTimer testTimer = new DispatcherTimer();

        bool isStart = false;
        int count = 0;
        public LoadedControl()
        {
            InitializeComponent();

            testTimer.Interval = TimeSpan.FromMilliseconds(500);
            testTimer.Tick += new EventHandler(Timer_Tick);
        }

        /// <summary>
        /// 注册消息按钮样式属性
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(LoadedControl), new PropertyMetadata("全速为您加载中..."));
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// 开始显示或隐藏加载窗口
        /// </summary>
        public void StartStopWait()
        {
            loading.ChangeState();
            if (isStart)
            {
                testTimer.Stop();
                count = 0;
                this.Visibility = Visibility.Hidden;
                isStart = false;
            }
            else
            {
                testTimer.Start();
                isStart = true;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            count++;
            if (count > 1)
            {
                this.Visibility = Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                count = 0;
                testTimer.Stop();
            }
        }
    }
}
