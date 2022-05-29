using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Win32;

namespace SparkControls.Controls
{
    /// <summary>
    /// 弹出框
    /// </summary>
    [ToolboxBitmap(typeof(Popup)), Description("弹出框")]
    public sealed class SparkPopup : Form, IMessageFilter
    {
        #region 变量
        /// <summary>
        /// 父容器
        /// </summary>
        private readonly Control _parentControl = null;

        /// <summary>
        /// 行高，如果是表格控件，要知道当前行的高度，以便弹出框显示不下时，重新计算位置
        /// </summary>
        public int RowHeight { get; set; }

        /// <summary>
        /// 定时器
        /// </summary>
        private readonly Timer _timer = null;

        /// <summary>
        /// 是否失去焦点
        /// </summary>
        private readonly bool _isNotFocus = true;

        /// <summary>
        /// 子容器大小
        /// </summary>
        private Size _size;

        /// <summary>
        /// 平移坐标
        /// </summary>
        private Point? _deviation;
        #endregion

        /// <summary>
        ///  获取一个值，该值指示显示窗口时是否激活它。如果显示窗口时不将其激活，则为 true；否则为 false。 默认值为 false。
        /// </summary>
        protected override bool ShowWithoutActivation => _isNotFocus;

        #region 构造器
        /// <summary>
        /// 构造函数
        /// </summary>
        private SparkPopup()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.None;
            this.ClientSize = new Size(45, 48);
            this.FormBorderStyle = FormBorderStyle.None;
            this.ControlBox = false;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = false;//千万不能设置为true,否在窗口会激活并获取焦点
            _timer = new Timer() { Interval = 100 };
            _timer.Tick += Timer_Tick;
            this.VisibleChanged += Form_VisibleChanged;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="parentControl">父控件</param>
        /// <param name="childControl">子控件</param>
        /// <param name="deviation">偏移,以parentControl父控件的屏幕坐标为标准</param>
        /// <param name="isNotFocus">是否无焦点窗体</param>
        public SparkPopup(Control parentControl, Control childControl, Point? deviation = null, bool isNotFocus = true) : this()
        {
            _isNotFocus = isNotFocus;
            _parentControl = parentControl;
            this.Size = childControl.Size;
            this.HandleCreated += Form_HandleCreated;
            this.HandleDestroyed += Form_HandleDestroyed;
            this.Controls.Add(childControl);
            childControl.Dock = DockStyle.Fill;

            _size = childControl.Size;
            _deviation = deviation;

            if (parentControl.FindForm() != null)
            {
                Form frmP = parentControl.FindForm();
                if (!frmP.IsDisposed)
                {
                    frmP.LocationChanged += Form_LocationChanged;
                    frmP.MouseCaptureChanged += FrmP_MouseCaptureChanged;
                }
            }
            parentControl.LocationChanged += Form_LocationChanged;
        }

        private void FrmP_MouseCaptureChanged(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="parentControl">父容器</param>
        /// <param name="size">窗口大小</param>
        /// <param name="deviation">平移量</param>
        /// <param name="isNotFocus">是否失去焦点.</param>
        public SparkPopup(Control parentControl, Size size, Point? deviation = null, bool isNotFocus = true) : this()
        {
            _isNotFocus = isNotFocus;
            _parentControl = parentControl;
            this.Size = size;
            this.HandleCreated += Form_HandleCreated;
            this.HandleDestroyed += Form_HandleDestroyed;

            _size = size;
            _deviation = deviation;
        }

        #endregion

        #region 事件处理程序
        /// <summary>
        /// 位置改变处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_LocationChanged(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 应用程序的消息泵移除一个消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_HandleDestroyed(object sender, EventArgs e)
        {
            Application.RemoveMessageFilter(this);
        }

        /// <summary>
        /// 应用程序的消息泵添加一个消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_HandleCreated(object sender, EventArgs e)
        {
            Application.AddMessageFilter(this);
        }

        /// <summary>
        /// 可见状态改变处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_VisibleChanged(object sender, EventArgs e)
        {
            //计算需要考虑多屏的情况
            if (_timer != null) _timer.Enabled = this.Visible;
            if (!this.Visible) return;

            Screen currentScreen = Screen.FromControl(_parentControl);//获取当前屏幕
            var workingArea = currentScreen.WorkingArea;

            Point p = _parentControl.PointToScreen(new Point(0, 0));//父窗口的屏幕坐标
            if (_deviation.HasValue)
            {//先计算偏移量
                p.X += _deviation.Value.X;
                p.Y += _deviation.Value.Y + _parentControl.Height;
            }
            else
            {
                p.Y += _parentControl.Height;
            }

            int intY;
            if (p.Y + _size.Height - workingArea.Y > workingArea.Height)
            {
                if (RowHeight > 0)
                {
                    intY = p.Y - _size.Height - 1 - RowHeight;
                }
                else
                {
                    intY = p.Y - _size.Height - 1 - _parentControl.Height;
                }
            }
            else
            {
                intY = p.Y + 1;
            }

            int intX;
            if (p.X + _size.Width - workingArea.X > workingArea.Width)
            {
                intX = p.X - _size.Width;
            }
            else
            {
                intX = p.X;
            }

            this.Location = new Point(intX, intY);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// 在调度消息之前将其筛选出来。
        /// </summary>
        /// <param name="m">要调度的消息。无法修改此消息。</param>
        /// <returns>如果筛选消息并禁止消息被调度，则为 true；
        /// 如果允许消息继续到达下一个筛选器或控件，则为 false。</returns>
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg != (int)Msgs.WM_LBUTTONDOWN || this.Visible == false) return false; //513

            var pt = this.PointToClient(MousePosition);
            this.Visible = this.ClientRectangle.Contains(pt);
            if (!Visible) Close();
            return false;
        }

        /// <summary>
        /// 时钟轮询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (this.Owner == null && _parentControl != null)
            {
                this.Owner = _parentControl.FindForm();
                return;
            }
            if (this.Owner == null)
            {
                return;
            }
            IntPtr _ptr = NativeMethods.GetForegroundWindow();
            try
            {
                if (_ptr != this.Owner.Handle && _ptr != this.Handle) this.Close();
            }
            catch { }
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="m"></param>
        //protected override void WndProc(ref Message m)
        //{
        //    if (_isNotFocus)
        //    {
        //        Spark.Foundation.Comm.Logger.WriteDebug($"WndProc,Msg={m.Msg}  WParam={m.WParam}  LParam={m.LParam}  Result={m.Result}");
        //        if (this.Visible && m.Msg == Win32Consts.WM_ACTIVATEAPP && m.WParam == IntPtr.Zero)
        //        {//表示失效激活
        //            this.Close();
        //            return;
        //        };
        //        if (m.Msg == Win32Consts.WM_MOUSEACTIVATE)//33
        //        {
        //            m.Result = new IntPtr(Win32Consts.MA_NOACTIVATE);//3
        //            return;
        //        }
        //        else if (m.Msg == Win32Consts.WM_NCACTIVATE)//134
        //        {
        //            if (((int)m.WParam & 0xFFFF) != Win32Consts.WA_INACTIVE)
        //            {
        //                if (m.LParam != IntPtr.Zero) NativeMethods.SetActiveWindow(m.LParam);
        //            }
        //            else NativeMethods.SetActiveWindow(IntPtr.Zero);
        //        }
        //    }
        //    try
        //    {
        //        base.WndProc(ref m);
        //    }
        //    catch { }
        //}

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SparkPopup
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "SparkPopup";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.ResumeLayout(false);
        }
    }
}