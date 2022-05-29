using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SparkControls.Forms
{
    public partial class SparkFrmLoadWait<T> : SparkControls.Forms.SparkFormBase
    {
        private string waitInfo;
        private IWin32Window _parent = null;
        T obj;
        private SparkFrmLoadWait()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
        }

        protected Func<Action<string>, T> TaskLoad { get; private set; }

        protected Action<T> TaskComplete { get; private set; }

        protected string WaitInfo
        {
            get => waitInfo;
            private set
            {
                waitInfo = value;
                SetWaitInfo(value);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Text = "请稍等...";
            base.OnLoad(e);

        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(5);
                ExecLoad();
                CloseMyself();
                ExecComplete();
            });
        }

        private void ExecLoad()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => { ExecLoad(); }));
            }
            else
            {
                if (TaskLoad != null)
                {
                    try
                    {
                        obj = TaskLoad.Invoke(UpdateInfo);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        private void UpdateInfo(string info)
        {
            WaitInfo = info;
        }

        private void SetWaitInfo(string info)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => { SetWaitInfo(info); }));
            }
            else
            {
                trtLabel1.Text = info;
            }
        }

        private void ExecComplete()
        {
            if (_parent != null && _parent is Control ctrl && ctrl.InvokeRequired)
            {
                ctrl.Invoke((MethodInvoker)(() => { ExecComplete(); }));
            }
            else
            {
                if (TaskComplete != null)
                {
                    try
                    {
                        TaskComplete.Invoke(obj);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void CloseMyself()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => { CloseMyself(); }));
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// 显示等待窗口
        /// </summary>
        /// <param name="parent">父窗口</param>
        /// <param name="info">提示信息</param>
        /// <param name="taskLoad">异步执行数据加载方法</param>
        /// <param name="taskComplete">异步加载完成后执行的方法</param>
        public static void ShowWait(IWin32Window parent, string info, Func<T> taskLoad, Action<T> taskComplete)
        {
            ShowWait(parent, info, task =>
            {
                try
                {
                    if (taskLoad != null)
                        return taskLoad.Invoke();
                    else return default(T);
                }
                catch (Exception ex)
                {
                    return default(T);
                }
            }, taskComplete);
        }

        /// <summary>
        /// 显示等待窗口，并更新提示信息
        /// </summary>
        /// <param name="parent">父窗口</param>
        /// <param name="info">默认提示信息</param>
        /// <param name="taskLoad">异步执行数据加载方法，并提供更新提示信息的方法</param>
        /// <param name="taskComplete">异步加载完成后执行的方法</param>
        public static void ShowWait(IWin32Window parent, string info, Func<Action<string>, T> taskLoad, Action<T> taskComplete)
        {
            if (string.IsNullOrEmpty(info))
            {
                info = "请稍等......";
            }
            SparkFrmLoadWait<T> loadWaitForm = new SparkFrmLoadWait<T>()
            {
                WaitInfo = info,
                TaskComplete = taskComplete,
                TaskLoad = taskLoad,
                TopMost = true,
                _parent = parent,
            };
            if (parent != null)
            {
                if (parent is Control ctrl && ctrl.IsDisposed)
                {
                    return;
                }
                loadWaitForm.ShowDialog(parent);
            }
            else
            {
                loadWaitForm.ShowDialog();
            }
        }

    }
}
