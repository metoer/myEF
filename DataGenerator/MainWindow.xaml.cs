using DataBase.Entity.Infrastructure;
using DataBase.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
namespace DataGenerator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string error = string.Empty;

        LoadingWindow loadingWindow;

        int stationCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            txtClientCode.Text = AppDomain.CurrentDomain.BaseDirectory + "Codes\\ClientCode.txt";
            //txtAlarmCode.Text = AppDomain.CurrentDomain.BaseDirectory + "Codes\\AlarmCode.xml";
            txtManageCode.Text = AppDomain.CurrentDomain.BaseDirectory + "Codes\\ManageCenterLog.xml";
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            UpdateConfigInfo();
            loadingWindow = new LoadingWindow("连接测试中", TestDataBaseConnect);
            loadingWindow.Owner = this;
            loadingWindow.ShowDialog();
        }

        private void TestDataBaseConnect()
        {
            string ip = txtIp.Text;
            string port = txtPort.Text;
            string useName = txtUserName.Text;
            string password = txtPassowrd.Text;
            string dataBaseName = txtDataBaseName.Text;
            new Thread(() =>
            {
                bool result = Common.CheckDataBaseExist();
                this.Dispatcher.Invoke(new Action(() =>
                {
                    loadingWindow.Close();
                    if (result)
                    {
                        MessageBox.Show("连接成功");
                    }
                    else
                    {
                        MessageBox.Show("连接失败");
                    }
                }));

            }) { IsBackground = true }.Start();
        }

        private void btnSelectLog_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            OpenFileDialog fd = new OpenFileDialog();
            fd.Title = "请选择文件";
            fd.Filter = button.Tag.ToString();
            if (fd.ShowDialog() == true)
            {
                TextBox textBox = ((button.Parent) as Panel).Children[0] as TextBox;
                textBox.Text = fd.FileName;
            }
        }

        private void btnSelectDir_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.Description = "请选择文件采集路径";

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextBox textBox = ((button.Parent) as Panel).Children[0] as TextBox;
                textBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!CheceUI())
            {
                return;
            }

            UpdateConfigInfo();

            loadingWindow = new LoadingWindow("开始创建数据", StartCreateThread);
            loadingWindow.Owner = this;
            loadingWindow.ShowDialog();
        }

        private bool CheceUI()
        {
            foreach (var item in gridDataBase.Children)
            {
                TextBox textbox = item as TextBox;
                if (textbox != null && string.IsNullOrEmpty(textbox.Text))
                {
                    MessageBox.Show("请将数据库配置信息填满");
                    return false;
                }
            }

            foreach (var item in gridOp.Children)
            {
                TextBox textbox = item as TextBox;
                if (textbox != null && string.IsNullOrEmpty(textbox.Text))
                {
                    MessageBox.Show("请将控制信息填满");
                    return false;
                }

                DatePicker datePicker = item as DatePicker;

                if (datePicker != null && string.IsNullOrEmpty(datePicker.Text))
                {
                    MessageBox.Show("请将控制信息填满");
                    return false;
                }
            }

            DateTime startTime = Convert.ToDateTime(dpStartTime.Text);
            DateTime endTime = Convert.ToDateTime(dpEndTime.Text);

            if (startTime > endTime)
            {
                MessageBox.Show("结束日期不能小于开始日期");
                return false;
            }

            int stationStart = 0;
            int stationEnd = 0;

            if (!(int.TryParse(txtStationStart.Text, out stationStart) && int.TryParse(txtStationEnd.Text, out stationEnd)) || stationStart > stationEnd)
            {
                MessageBox.Show("采集站编号范围填写不正确");
                return false;
            }

            stationCount = stationEnd - stationStart + 1;

            if (!((bool)cbDeviceLog.IsChecked || (bool)cbMediaLog.IsChecked || (bool)cbStationLog.IsChecked))
            {
                MessageBox.Show("表策略中最少需要选择一个表创建数据");
                return false;
            }

            if ((bool)cbDeviceLog.IsChecked && string.IsNullOrEmpty(txtDeviceCode.Text))
            {
                MessageBox.Show("请添加执法记录仪操作编码或者编码文件");
                return false;
            }

            if ((bool)cbStationLog.IsChecked && string.IsNullOrEmpty(txtClientCode.Text))
            {
                MessageBox.Show("请添加执法记录仪操作编码或者编码文件");
                return false;
            }

            if ((bool)cbMediaLog.IsChecked && string.IsNullOrEmpty(txtVideo.Text) && string.IsNullOrEmpty(txtAudio.Text) && string.IsNullOrEmpty(txtImage.Text))
            {
                MessageBox.Show("媒体记录表中最少需要一种文件");
                return false;
            }

            if ((bool)cbMediaLog.IsChecked && (string.IsNullOrEmpty(txtCollectDir.Text)))
            {
                MessageBox.Show("请选择采集文件路径");
                return false;
            }

            if ((bool)rb2.IsChecked && (bool)cbManageLog.IsChecked && (string.IsNullOrEmpty(txtManageCode.Text)))
            {
                MessageBox.Show("请添加操作编码文件路径");
                return false;
            }


            return true;
        }

        private void StartCreateThread()
        {
            DateTime startTime = Convert.ToDateTime(dpStartTime.Text);
            DateTime endTime = Convert.ToDateTime(dpEndTime.Text);
            int stationCodeStart = Convert.ToInt32(txtStationStart.Text);
            int stationCodeEnd = Convert.ToInt32(txtStationEnd.Text);
            int oneNum = Convert.ToInt32(txtNum.Text);
            int oneSleep = Convert.ToInt32(txtSleep.Text);
            bool isStation = rb1.IsChecked == true;
            FileCopy.IsCopy = (bool)cbFileCopy.IsChecked;
            ThreadHepler threadHelper = new ThreadHepler(this);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            threadHelper.Success += new ThreadHepler.SuccessEventHandler((oneself) =>
            {

            });

            threadHelper.Finally += new ThreadHepler.FinallyEventHandler((oneself) =>
            {
                stopwatch.Stop();
                loadingWindow.Close();
                int result = Convert.ToInt32(oneself.Result);
                PrintResultMsg(result, stopwatch.Elapsed);
            });

            threadHelper.BeginInvoke(new ThreadHepler.RequestAction((myObj) =>
            {
                int result = isStation ? CreateDataByDs(startTime, endTime, stationCodeStart, stationCodeEnd, oneNum, oneSleep) :
                    CreateDataByMc(startTime, endTime, stationCodeStart, stationCodeEnd, oneNum, oneSleep);
                return result;
            })); ;
        }

        private void PrintResultMsg(int result, TimeSpan time)
        {
            string message = string.Empty;
            switch (result)
            {
                case -1:
                    message = error;
                    break;

                case 1:
                    message = "数据库连接失败";
                    break;
                case 2:
                    message = "没有获取到指定的用户信息";
                    break;
                case 3:
                    message = "没有获取组织架构信息";
                    break;
                case 4:
                    message = "没有获取到执法记录仪信息";
                    break;
                case 5:
                    message = "没有获取到指定用户的组织架构信息";
                    break;
                default:
                    message = string.Format("共成功模拟信息数量:{0},用时:{1}小时,{2}分钟,{3}秒", result, time.Hours, time.Minutes, time.Seconds);
                    break;

            }

            MessageBox.Show(message);
        }



        /// <summary>
        /// 1、数据连接失败；2、用户信息没有找到；3、组织架构为空
        /// </summary>
        /// <returns></returns>
        private int CreateDataByDs(DateTime startTime, DateTime endTime, int startCode, int endCode, int OneNum, int OneSleep)
        {
            try
            {
                SetProgess("0%");
                SetMsg("正在检查数据库连接");
                bool result = Common.CheckDataBaseExist();

                if (!result)
                {
                    return 1;
                }

                SetProgess("2%");
                SetMsg("正在获取指定用户信息");
                ds_user userInfo = LibHelper.GetUserInfoByPoliceCode(GetTextBoxValue(txtPoliceCode));

                if (userInfo == null)
                {
                    return 2;
                }

                SetProgess("5%");
                SetMsg("正在获取组织架构信息");
                List<ds_organization> orgList = LibHelper.GetOrgInfos();
                if (orgList == null || orgList.Count == 0)
                {
                    return 3;
                }

                SetProgess("8%");
                SetMsg("正在获取设备信息");
                List<ds_device> deviceInfos = LibHelper.GetDeviceInfos(GetTextBoxValue(txtDeviceID));
                if (deviceInfos == null || deviceInfos.Count == 0)
                {
                    return 4;
                }

                ds_organization userOrgInfo = orgList.Find(p => p.org_id.Equals(userInfo.org_id));
                if (userOrgInfo == null)
                {
                    return 5;
                }

                int count = 0;
                int index = 0;

                for (int i = startCode; i <= endCode; i++)
                {
                    if (GetCheckValue(cbDeviceLog))
                    {
                        index++;
                        SetMsg(string.Format("正在为{0}创建执法记录仪操作日志信息", i));
                        count += LibHelper.AddCameraLog(
                                                           startTime,
                                                           endTime,
                                                           Convert.ToInt32(GetTextBoxValue(txtDayNum)),
                                                            i.ToString(),
                                                           GetTextBoxValue(txtDeviceCode),
                                                           userInfo,
                                                           orgList,
                                                           userOrgInfo,
                                                           deviceInfos,
                                                           index,
                                                           OneNum,
                                                           OneSleep,
                                                           CountProgess
                                                           );
                    }

                    if (GetCheckValue(cbStationLog))
                    {
                        index++;
                        SetMsg(string.Format("正在为{0}创建采集工作站操作日志信息", i));
                        count += LibHelper.AddStationLog(
                                                           startTime,
                                                           endTime,
                                                           Convert.ToInt32(GetTextBoxValue(txtDayNum)),
                                                           i.ToString(),
                                                           GetTextBoxValue(txtClientCode),
                                                           userInfo,
                                                           userOrgInfo,
                                                           index, OneNum,
                                                           OneSleep,
                                                           CountProgess
                                                           );
                    }

                    if (GetCheckValue(cbMediaLog))
                    {
                        index++;
                        SetMsg(string.Format("正在为{0}创建采集工作站媒体数据信息", i));
                        count += LibHelper.AddMediaLog(
                                                           startTime,
                                                           endTime,
                                                           Convert.ToInt32(GetTextBoxValue(txtDayNum)),
                                                           i.ToString(),
                                                           userInfo,
                                                           userOrgInfo,
                                                           deviceInfos,
                                                           orgList,
                                                           GetTextBoxValue(txtVideo),
                                                           GetTextBoxValue(txtAudio),
                                                           GetTextBoxValue(txtImage),
                                                           GetTextBoxValue(txtCollectDir),
                                                           index, OneNum,
                                                           OneSleep,
                                                           CountProgess
                                                           );
                    }

                }

                SetProgess("100%");
                SetMsg("创建采集工作站模拟信息完成");
                return count;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        /// <summary>
        /// 后台数据库插入
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private int CreateDataByMc(DateTime startTime, DateTime endTime, int startCode, int endCode, int OneNum, int OneSleep)
        {
            try
            {
                SetProgess("0%");
                SetMsg("正在检查数据库连接");
                bool result = Common.CheckDataBaseExist();

                if (!result)
                {
                    return 1;
                }

                SetProgess("2%");
                SetMsg("正在获取指定用户信息");
                mc_user userInfo = LibHelper.GetUserInfoByPoliceCodeMC(GetTextBoxValue(txtPoliceCode));

                if (userInfo == null)
                {
                    return 2;
                }

                SetProgess("5%");
                SetMsg("正在获取组织架构信息");
                List<mc_organization> orgList = LibHelper.GetOrgInfosMC();
                if (orgList == null || orgList.Count == 0)
                {
                    return 3;
                }

                SetProgess("8%");
                SetMsg("正在获取设备信息");
                List<mc_device> deviceInfos = LibHelper.GetDeviceInfosMC(GetTextBoxValue(txtDeviceID));
                if (deviceInfos == null || deviceInfos.Count == 0)
                {
                    return 4;
                }

                mc_organization userOrgInfo = orgList.Find(p => p.org_id.Equals(userInfo.org_id));
                if (userOrgInfo == null)
                {
                    return 5;
                }

                int count = 0;
                int index = 0;
                for (int i = startCode; i <= endCode; i++)
                {
                    if (GetCheckValue(cbDeviceLog))
                    {
                        index++;
                        SetMsg(string.Format("正在为{0}创建WEB执法记录仪操作日志信息", i));
                        count += LibHelper.AddCameraLogByMc(
                                                           startTime,
                                                           endTime,
                                                           Convert.ToInt32(GetTextBoxValue(txtDayNum)),
                                                          i.ToString(),
                                                           GetTextBoxValue(txtDeviceCode),
                                                           userInfo,
                                                           orgList,
                                                           userOrgInfo,
                                                           deviceInfos,
                                                           index, OneNum,
                                                           OneSleep,
                                                           CountProgess
                                                           );
                    }

                    if (GetCheckValue(cbStationLog))
                    {
                        index++;
                        SetMsg(string.Format("正在为{0}创建WEB采集站操作日志信息", i));
                        count += LibHelper.AddStationLogByMC(
                                                           startTime,
                                                           endTime,
                                                           Convert.ToInt32(GetTextBoxValue(txtDayNum)),
                                                          startCode.ToString(),
                                                           GetTextBoxValue(txtClientCode),
                                                           userInfo,
                                                           userOrgInfo,
                                                           index, OneNum,
                                                           OneSleep,
                                                           CountProgess
                                                           );
                    }

                    if (GetCheckValue(cbMediaLog))
                    {
                        index++;
                        SetMsg(string.Format("正在为{0}创建WEB媒体数据信息", i));
                        count += LibHelper.AddMediaLogMC(
                                                           startTime,
                                                           endTime,
                                                           Convert.ToInt32(GetTextBoxValue(txtDayNum)),
                                                           i.ToString(),
                                                           userInfo,
                                                           userOrgInfo,
                                                           deviceInfos,
                                                           orgList,
                                                           GetTextBoxValue(txtVideo),
                                                           GetTextBoxValue(txtAudio),
                                                           GetTextBoxValue(txtImage),
                                                           GetTextBoxValue(txtCollectDir),
                                                           index, OneNum,
                                                           OneSleep,
                                                           CountProgess
                                                           );
                    }
                    //if (GetCheckValue(cbalarmLog))
                    //{
                    //    index++;
                    //    SetMsg("正在创建WEB告警信息");
                    //    count += LibHelper.AddAlarmlog(
                    //                                       startTime,
                    //                                       endTime,
                    //                                       Convert.ToInt32(GetTextBoxValue(txtDayNum)),
                    //                                       GetTextBoxValue(txtAlarmCode),
                    //                                       index,
                    //                                       CountProgess
                    //                                       );
                    //}

                    if (GetCheckValue(cbManageLog))
                    {
                        index++;
                        SetMsg(string.Format("正在为{0}创建媒体数据信息", i));
                        count += LibHelper.AddManageCenterlog(
                                                           startTime,
                                                           endTime,
                                                           Convert.ToInt32(GetTextBoxValue(txtDayNum)),
                                                          i.ToString(),
                                                           userInfo,
                                                           userOrgInfo,
                                                           GetTextBoxValue(txtManageCode),
                                                           index, OneNum,
                                                           OneSleep,
                                                           CountProgess
                                                           );
                    }

                    //if (GetCheckValue(cbStatusLog))
                    //{
                    //    index++;
                    //    SetMsg("正在创建WEB状态信息");
                    //    count += LibHelper.AddStatuslog(
                    //                                       startTime,
                    //                                       endTime,
                    //                                       Convert.ToInt32(GetTextBoxValue(txtDayNum)),
                    //                                       index,
                    //                                       CountProgess
                    //                                       );
                    //}

                }

                SetProgess("100%");
                SetMsg("创建WEB模拟信息完成");
                return count;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        private void CountProgess(decimal value, int index)
        {
            int selectCount = (GetCheckValue(cbDeviceLog) ? 1 : 0) + (GetCheckValue(cbMediaLog) ? 1 : 0) + (GetCheckValue(cbStationLog) ? 1 : 0);

            if (GetRadioButtonValue(rb2))
            {
                selectCount += GetCheckValue(cbManageLog) ? 1 : 0;
            }

            selectCount = selectCount * stationCount;

            double result = Convert.ToDouble(Math.Round(value / selectCount * (decimal)0.9, 4) * 100 + 10 + (index - 1) * (90 / selectCount));

            SetProgess(result.ToString("00.00") + "%");
        }

        private string GetTextBoxValue(TextBox textBox)
        {
            string text = string.Empty;
            this.Dispatcher.Invoke(new Action(() =>
            {
                text = textBox.Text;
            }));

            return text;
        }

        private bool GetCheckValue(CheckBox checkBox)
        {
            bool result = false;
            this.Dispatcher.Invoke(new Action(() =>
            {
                result = (bool)checkBox.IsChecked;
            }));

            return result;
        }

        private bool GetRadioButtonValue(RadioButton rb)
        {
            bool result = false;
            this.Dispatcher.Invoke(new Action(() =>
            {
                result = (bool)rb2.IsChecked;
            }));

            return result;
        }

        private void UpdateConfigInfo()
        {
            string ip = txtIp.Text;
            string port = txtPort.Text;
            string useName = txtUserName.Text;
            string password = txtPassowrd.Text;
            string dataBaseName = txtDataBaseName.Text;
            LibHelper.UpdateConfig(ip, port, useName, password, dataBaseName);
        }

        private void SetMsg(string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                loadingWindow.SetMsg = message;
            }));
        }

        private void SetProgess(string progess)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {

                loadingWindow.SetProgress = progess;
            }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (FileCopy.Sources.Count > 0)
            {
                if (MessageBox.Show("还有文件复制没有完成，是否结束？", "询问", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
