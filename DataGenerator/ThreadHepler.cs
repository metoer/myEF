using System;
using System.Threading;
using System.Windows.Threading;

namespace DataGenerator
{
    /// <summary>
    /// 线程帮助类
    /// </summary>
    public class ThreadHepler
    {
        private DispatcherObject dispatcherObject;

        Thread thread = null;

        /// <summary>
        /// 结果
        /// </summary>
        public object Result { get; private set; }

        /// <summary>
        /// 临时自定义
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 异常
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// 运行状态
        /// </summary>
        public bool IsRunning
        {
            get;
            private set;
        }

        /// <summary>
        /// 显示错误信息
        /// </summary>
        public bool IsShowMessage { get; private set; }

        public ThreadHepler(bool isShowMessage = false)
        {
            this.IsShowMessage = isShowMessage;
        }

        public ThreadHepler(DispatcherObject dispatcherObject, bool isShowMessage = false)
        {
            this.dispatcherObject = dispatcherObject;
            this.IsShowMessage = isShowMessage;
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        public void Stop()
        {
            try
            {
                if (thread != null)
                {
                    thread.Abort();
                    thread = null;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 执行前
        /// </summary>
        public delegate object RequestAction(object obj);

        public void Invoke(RequestAction action, object obj = null)
        {
            DoWork(action, obj);
        }

        /// <summary>
        /// 主线程执行
        /// </summary>
        /// <param name="a"></param>
        private void DoMain(Action a)
        {
            if (dispatcherObject == null)
            {
                a();
            }
            else
            {
                dispatcherObject.Dispatcher.Invoke(a);
            }
        }

        /// <summary>
        /// 异步线程执行
        /// </summary>
        /// <param name="action"></param>
        /// <param name="obj"></param>
        private void DoWork(RequestAction action, object obj = null)
        {
            try
            {
                this.IsRunning = true;

                OnBefore();

                this.Result = action(obj);

                OnSuccess(this);

            }
            catch (Exception ex)
            {
                this.Exception = ex;
                OnError(this);
            }
            finally
            {
                OnFinally(this);
                this.IsRunning = false;
            }
        }

        /// <summary>
        /// 异步请求
        /// </summary>
        /// <param name="httpItem"></param>
        public void BeginInvoke(RequestAction action, object obj = null)
        {
            if (IsRunning) throw new Exception("线程正在运行");

            thread = new Thread(() =>
            {
                try
                {
                    DoWork(action, obj);
                }
                catch (Exception)
                {
                }
                finally
                {
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 执行前
        /// </summary>
        public event BeforeEventHandler Before;
        public delegate void BeforeEventHandler();
        public virtual void OnBefore()
        {
            if (Before == null) return;

            DoMain(new Action(() =>
            {
                try
                {
                    Before();
                }
                catch (Exception ex)
                {
                    this.Exception = ex;
                    OnError(this);
                }
            }));
        }

        /// <summary>
        /// 执行完成
        /// </summary>
        public event FinallyEventHandler Finally;
        public delegate void FinallyEventHandler(ThreadHepler oneself);
        public virtual void OnFinally(ThreadHepler oneself)
        {
            if (Finally == null) return;

            DoMain(new Action(() =>
            {
                try
                {
                    Finally(oneself);
                }
                catch (Exception ex)
                {
                    this.Exception = ex;
                    OnError(this);
                }
            }));
        }

        /// <summary>
        /// 请求成功
        /// </summary>
        public event SuccessEventHandler Success;
        public delegate void SuccessEventHandler(ThreadHepler oneself);
        public virtual void OnSuccess(ThreadHepler oneself)
        {
            if (Success == null) return;

            DoMain(new Action(() =>
            {
                try
                {
                    Success(oneself);
                }
                catch (Exception ex)
                {
                    this.Exception = ex;
                    OnError(this);
                }
            }));
        }

        /// <summary>
        /// 失败错误
        /// </summary>
        public event ErrorEventHandler Error;
        public delegate void ErrorEventHandler(ThreadHepler oneself);
        public virtual void OnError(ThreadHepler oneself)
        {
            if (Error == null) return;

            DoMain(new Action(() =>
            {
                Error(oneself);

                //if (IsShowMessage && oneself.Exception != null)
                //{
                //    System.Windows.Forms.MessageBox.Show(oneself.Exception.ToString());
                //}
            }));
        }

        /// <summary>
        /// 进度百分比
        /// </summary>
        public event PercentEventHandler Percent;
        public delegate void PercentEventHandler(int i);
        public virtual void OnPercent(int i)
        {
            if (Percent == null) return;

            DoMain(new Action(() =>
            {
                try
                {
                    Percent(i);
                }
                catch (Exception ex)
                {
                    this.Exception = ex;
                    OnError(this);
                }
            }));
        }
    }
}
