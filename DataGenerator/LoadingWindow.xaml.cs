using System;
using System.Windows;

namespace DataGenerator
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingWindow : Window
    {
        #region 属性、变量、构造
        /// <summary>
        /// 是否需要通过此窗口发送消息
        /// </summary>
        bool isSendMsg = false;

        Action action;

        public LoadingWindow(string msg, Action action = null)
        {
            InitializeComponent();
            tbMsg.Text = msg;
            this.action = action;
        }

        public string SetMsg
        {
            get
            {
                return tbMsg.Text;
            }
            set
            {
                tbMsg.Text = value;
            }
        }

        public string SetProgress
        {
            get
            {
                return tbPr.Text;
            }
            set
            {
                tbPr.Text = value;
            }
        }

        /// <summary>
        /// 操作结果,成功YES，失败NO,超时Canel
        /// </summary>
        public MessageBoxResult MessageBoxResult
        {
            get;
            private set;
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loading.ChangeState();
            if (action != null)
            {
                action();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
