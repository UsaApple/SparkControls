using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	public sealed class SparkSaveFileDialog
    {
        #region 字段

        private SaveFileDialog _dialog = null;

        #endregion

        #region 属性

        public bool CreatePrompt
        {
            set { _dialog.CreatePrompt = value; }
            get { return _dialog.CreatePrompt; }
        }

        public bool OverwritePrompt
        {
            set { _dialog.OverwritePrompt = value; }
            get { return _dialog.OverwritePrompt; }
        }

        //
        // 摘要:
        //     获取此 System.Windows.Forms.FileDialog 实例的自定义空间的集合。
        //
        // 返回结果:
        //     此 System.Windows.Forms.FileDialog 实例的自定义空间的集合。
        public FileDialogCustomPlacesCollection CustomPlaces
        {
            get { return _dialog.CustomPlaces; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示对话框是否只接受有效的 Win32 文件名。
        //
        // 返回结果:
        //     如果对话框只接受有效 Win32 文件名，则为 true；否则为 false。默认值为 true。
        public bool ValidateNames
        {
            set { _dialog.ValidateNames = value; }
            get { return _dialog.ValidateNames; }
        }

        //
        // 摘要:
        //     获取或设置文件对话框标题。
        //
        // 返回结果:
        //     文件对话框标题。默认值为空字符串 ("")。
        public string Title
        {
            set { _dialog.Title = value; }
            get { return _dialog.Title; }
        }

        //
        // 摘要:
        //     获取或设置对话框是否支持显示和保存具有多个文件扩展名的文件。
        //
        // 返回结果:
        //     如果对话框支持多个文件扩展名，则为 true；否则为 false。默认为 false。
        public bool SupportMultiDottedExtensions
        {
            set { _dialog.SupportMultiDottedExtensions = value; }
            get { return _dialog.SupportMultiDottedExtensions; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示文件对话框中是否显示“帮助”按钮。
        //
        // 返回结果:
        //     如果对话框包含帮助按钮，则为 true；否则为 false。默认值为 false。
        public bool ShowHelp
        {
            set { _dialog.ShowHelp = value; }
            get { return _dialog.ShowHelp; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示对话框在关闭前是否还原当前目录。
        //
        // 返回结果:
        //     假设用户在搜索文件的过程中更改了目录，那么，如果对话框会将当前目录还原为初始值，则值为 true；反之，值为 false。默认值为 false。
        public bool RestoreDirectory
        {
            set { _dialog.RestoreDirectory = value; }
            get { return _dialog.RestoreDirectory; }
        }

        //
        // 摘要:
        //     获取或设置文件对话框显示的初始目录。
        //
        // 返回结果:
        //     文件对话框中显示的初始目录。默认值为空字符串 ("")。
        public string InitialDirectory
        {
            set { _dialog.InitialDirectory = value; }
            get { return _dialog.InitialDirectory; }
        }

        //
        // 摘要:
        //     获取或设置文件对话框中当前选定筛选器的索引。
        //
        // 返回结果:
        //     包含文件对话框中当前选定筛选器的索引的值。默认值为 1。
        public int FilterIndex
        {
            set { _dialog.FilterIndex = value; }
            get { return _dialog.FilterIndex; }
        }

        //
        // 摘要:
        //     获取或设置当前文件名筛选器字符串，该字符串决定对话框的“另存为文件类型”或“文件类型”框中出现的选择内容。
        //
        // 返回结果:
        //     对话框中可用的文件筛选选项。
        //
        // 异常:
        //   T:System.ArgumentException:
        //     Filter 格式无效。
        public string Filter
        {
            set { _dialog.Filter = value; }
            get { return _dialog.Filter; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示此 System.Windows.Forms.FileDialog 实例在 Windows Vista 上运行时是否应自动升级外观和行为。
        //
        // 返回结果:
        //     如果此 System.Windows.Forms.FileDialog 实例在 Windows Vista 上运行时应自动升级外观和行为，则为 true；否则为
        //     false。默认值为 true。
        public bool AutoUpgradeEnabled
        {
            set { _dialog.AutoUpgradeEnabled = value; }
            get { return _dialog.AutoUpgradeEnabled; }
        }

        //
        // 摘要:
        //     获取对话框中所有选定文件的文件名。
        //
        // 返回结果:
        //     包含对话框中所有选定文件的文件名的 System.String 类型数组。
        public string[] FileNames { get { return _dialog.FileNames; } }

        //
        // 摘要:
        //     获取或设置一个值，该值指示对话框是否返回快捷方式引用的文件的位置，或者是否返回快捷方式 (.lnk) 的位置。
        //
        // 返回结果:
        //     如果对话框返回快捷方式引用的文件位置，值为 true；反之，值为 false。默认值为 true。
        public bool DereferenceLinks
        {
            set { _dialog.DereferenceLinks = value; }
            get { return _dialog.DereferenceLinks; }
        }

        //
        // 摘要:
        //     获取或设置默认文件扩展名。
        //
        // 返回结果:
        //     默认文件扩展名。返回的字符串不包含句点。默认值为空字符串 ("")。
        public string DefaultExt
        {
            set { _dialog.DefaultExt = value; }
            get { return _dialog.DefaultExt; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示如果用户指定不存在的路径，对话框是否显示警告。
        //
        // 返回结果:
        //     当用户指定不存在的路径时，如果对话框显示警告，值为 true；反之，值为 false。默认值为 true。
        public bool CheckPathExists
        {
            set { _dialog.CheckPathExists = value; }
            get { return _dialog.CheckPathExists; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示如果用户指定不存在的文件名，对话框是否显示警告。
        //
        // 返回结果:
        //     当用户指定不存在的文件名时，如果对话框显示警告，值为 true；反之，值为 false。默认值为 false。
        public bool CheckFileExists
        {
            set { _dialog.CheckFileExists = value; }
            get { return _dialog.CheckFileExists; }
        }

        //
        // 摘要:
        //     获取或设置一个值，该值指示如果用户省略扩展名，对话框是否自动在文件名中添加扩展名。
        //
        // 返回结果:
        //     当用户省略了扩展名时，如果对话框在文件名中添加扩展名，值为 true；反之，值为 false。默认值为 true。
        public bool AddExtension
        {
            set { _dialog.AddExtension = value; }
            get { return _dialog.AddExtension; }
        }

        //
        // 摘要:
        //     获取或设置一个包含在文件对话框中选定的文件名的字符串。
        //
        // 返回结果:
        //     对话框中选择的文件名。默认值为空字符串 ("")。
        public string FileName
        {
            set { _dialog.FileName = value; }
            get { return _dialog.FileName; }
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
        //     当用户单击文件对话框中的“打开”或“保存”按钮时发生。
        public event CancelEventHandler FileOk = null;

        //
        // 摘要:
        //     当用户单击通用对话框中的“帮助”按钮时发生。
        public event EventHandler HelpRequest = null;

        #endregion

        public SparkSaveFileDialog()
        {
            _dialog = new SaveFileDialog();
            _dialog.FileOk += (s, e) => { FileOk?.Invoke(s, e); };
            _dialog.HelpRequest += (s, e) => { HelpRequest?.Invoke(s, e); };
        }

        #region 对外方法

        //
        // 摘要:
        //     打开用户选定的具有读/写权限的文件。
        //
        // 返回结果:
        //     用户选定的读/写文件。
        public Stream OpenFile()
        {
            return _dialog.OpenFile();
        }

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

        //
        // 摘要:
        //     提供此对象的字符串版本。
        //
        // 返回结果:
        //     此对象的字符串版本。
        public override string ToString()
        {
            return _dialog.ToString();
        }

        #endregion
    }
}