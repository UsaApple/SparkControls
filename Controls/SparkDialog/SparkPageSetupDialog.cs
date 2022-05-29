using System;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
#pragma warning disable
	public sealed class SparkPageSetupDialog
    {
        #region 字段

        private PageSetupDialog _dialog = null;

        #endregion

        #region 属性

        //
        // 摘要:
        //     获取或设置一个值，该值指示是否启用对话框的边距部分。
        //
        // 返回结果:
        //     如果启用了对话框的边距部分，则为 true；否则为 false。默认为 true。
        public bool AllowMargins
        {
            set { _dialog.AllowMargins = value; }
            get { return _dialog.AllowMargins; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示是否启用对话框的方向部分（横向对纵向）。
        //
        // 返回结果:
        //     如果启用了对话框的方向部分，则为 true；否则为 false。默认为 true。
        public bool AllowOrientation
        {
            set { _dialog.AllowOrientation = value; }
            get { return _dialog.AllowOrientation; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示是否启用对话框的纸张部分（纸张大小和纸张来源）。
        //
        // 返回结果:
        //     如果启用了对话框的纸张部分，则为 true；否则为 false。默认为 true。
        public bool AllowPaper
        {
            set { _dialog.AllowPaper = value; }
            get { return _dialog.AllowPaper; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示是否启用“打印机”按钮。
        //
        // 返回结果:
        //     如果启用“打印机”按钮，则为 true；否则为 false。默认值为 true。
        public bool AllowPrinter
        {
            set { _dialog.AllowPrinter = value; }
            get { return _dialog.AllowPrinter; }
        }

        //
        // 摘要:
        //     获取或设置一个值，指示从中获取页面设置的 System.Drawing.Printing.PrintDocument。
        //
        // 返回结果:
        //     从中获得页面设置的 System.Drawing.Printing.PrintDocument。默认为 null。
        public PrintDocument Document
        {
            set { _dialog.Document = value; }
            get { return _dialog.Document; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示以毫米为单位显示边距设置时，是否自动将边距设置转换为以百分之一英寸为单位（或相反）。
        //
        // 返回结果:
        //     如果将自动转换边距，则为 true；否则为 false。默认为 false。
        public bool EnableMetric
        {
            set { _dialog.EnableMetric = value; }
            get { return _dialog.EnableMetric; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示允许用户选择的最小边距（以百分之一英寸为单位）。
        //
        // 返回结果:
        //     允许用户选择的最小边距（以百分之一英寸为单位）。默认为 null。
        public Margins MinMargins
        {
            set { _dialog.MinMargins = value; }
            get { return _dialog.MinMargins; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示要修改的页设置。
        //
        // 返回结果:
        //     要修改的 System.Drawing.Printing.PageSettings。默认为 null。
        public PageSettings PageSettings
        {
            set { _dialog.PageSettings = value; }
            get { return _dialog.PageSettings; }
        }

        //
        // 摘要:
        //     获取或设置用户单击对话框中“打印机”按钮时修改的打印机设置。
        //
        // 返回结果:
        //     用户单击“打印机”按钮时要修改的 System.Drawing.Printing.PrinterSettings。默认为 null。
        public PrinterSettings PrinterSettings
        {
            set { _dialog.PrinterSettings = value; }
            get { return _dialog.PrinterSettings; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示“帮助”按钮是否可见。
        //
        // 返回结果:
        //     如果“帮助”按钮是可见的，则为 true；否则为 false。默认为 false。
        public bool ShowHelp
        {
            set { _dialog.ShowHelp = value; }
            get { return _dialog.ShowHelp; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示“网络”按钮是否可见。
        //
        // 返回结果:
        //     如果“网络”按钮是可见的，则为 true；否则为 false。默认为 true。
        public bool ShowNetwork
        {
            set { _dialog.ShowNetwork = value; }
            get { return _dialog.ShowNetwork; }
        }

        //
        // 摘要:
        //     获取或设置一个对象，该对象包含控件的数据。
        //
        // 返回结果:
        //     包含有关 System.Windows.Forms.CommonDialog 的数据的对象。
        public object Tag
        {
            set { _dialog.Tag = value; }
            get { return _dialog.Tag; }
        }

        #endregion

        #region 自定义事件

        //
        // 摘要:
        //     当用户单击通用对话框中的“帮助”按钮时发生。
        public event EventHandler HelpRequest = null;

        #endregion

        public SparkPageSetupDialog()
        {
            _dialog = new PageSetupDialog();
            _dialog.HelpRequest += (s, e) => { HelpRequest?.Invoke(s, e); };
        }

        #region 对外方法

        //
        // 摘要:
        //     在派生类中被重写时，将通用对话框的属性重置为默认值。
        public void Reset()
        {
            _dialog.Reset();
        }

        //
        // 摘要:
        //     用默认的所有者运行通用对话框。
        //
        // 返回结果:
        //     如果用户在对话框中单击“确定”，则为 System.Windows.Forms.DialogResult.OK；否则为 System.Windows.Forms.DialogResult.Cancel。
        public DialogResult ShowDialog()
        {
            return _dialog.ShowDialog();
        }

        //
        // 摘要:
        //     运行具有指定所有者的通用对话框。
        //
        // 参数:
        //   owner:
        //     任何实现 System.Windows.Forms.IWin32Window（表示将拥有模式对话框的顶级窗口）的对象。
        //
        // 返回结果:
        //     如果用户在对话框中单击“确定”，则为 System.Windows.Forms.DialogResult.OK；否则为 System.Windows.Forms.DialogResult.Cancel。
        public DialogResult ShowDialog(IWin32Window owner)
        {
            return _dialog.ShowDialog(owner);
        }

        #endregion
    }
}