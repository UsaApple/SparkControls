using System;
using System.Windows.Forms;
using SparkControls.Foundation;
using SparkControls.Public;

namespace SparkControls.Controls
{
    /// <summary>
    /// 消息提示类
    /// </summary>
    public static class SparkMessageBox
    {
        #region 消息框
        /// <summary>
        /// 显示错误消息
        /// </summary>
        /// <param name="message">消息内容</param>
        public static void ShowErrorMessage(string message)
        {
            ShowErrorMessage(message, "");
        }

        /// <summary>
        /// Format格式显示错误消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="args">Format的参数</param>
        public static void ShowErrorMessage(string message, params object[] args)
        {
            ShowErrorMessage(string.Format(message, args));
        }

        /// <summary>
        /// 显示提示消息
        /// </summary>
        /// <param name="message">消息内容</param>
        public static void ShowInfoMessage(string message)
        {
            ShowInfoMessage(message, "");
        }

        /// <summary>
        /// Format格式显示提示消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="args">Format的参数</param>
        public static void ShowInfoMessage(string message, params object[] args)
        {
            ShowInfoMessage(string.Format(message, args));
        }

        /// <summary>
        /// 显示警告消息
        /// </summary>
        /// <param name="message">消息内容</param>
        public static void ShowWarningMessage(string message)
        {
            ShowWarningMessage(message, "");
        }

        /// <summary>
        /// Format格式显示提示消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="args">Format的参数</param>
        public static void ShowWarningMessage(string message, params object[] args)
        {
            ShowWarningMessage(string.Format(message, args));
        }

        /// <summary>
        /// buttonNum = 3 三个按钮 返回 Yes,No,Cancel
        /// buttonNum = 2 两个按钮 返回 Yes,No
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="buttonNum">按钮数量</param>
        /// <returns>返回一个对话框</returns>
        public static DialogResult ShowQuestionMessage(string message, int buttonNum = 3)
        {
            MessageBoxButtons btn = MessageBoxButtons.YesNoCancel;
            if (buttonNum == 2) btn = MessageBoxButtons.YesNo;
            return ShowQuestionMessage(message, btn, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// 询问提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="btn">按钮枚举</param>
        /// <param name="defaultButton">默认按钮</param>
        /// <returns></returns>
        public static DialogResult ShowQuestionMessage(string message, MessageBoxButtons btn, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            return MessageBoxEx.Show(message, "询问", "", btn, MessageBoxIcon.Question, defaultButton);
        }

        #endregion

        #region 有父窗口
        /// <summary>
        /// 显示错误消息
        /// </summary>
        /// <param name="message">消息内容</param>
        public static void ShowErrorMessage(IWin32Window owner, string message)
        {
            if (owner != null)
            {
                if ((owner as Control).InvokeRequired)
                {
                    (owner as Control).Invoke((MethodInvoker)(() => { ShowErrorMessage(owner, message); }));
                }
                else
                {
                    if (SetInitializationMessage(message))
                    {
                        return;
                    }
                    MessageBoxEx.Show(owner, message, "错误", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                ShowErrorMessage(message);
            }
        }

        /// <summary>
        /// Format格式显示错误消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="args">Format的参数</param>
        public static void ShowErrorMessage(IWin32Window owner, string message, params object[] args)
        {
            ShowErrorMessage(owner, string.Format(message, args));
        }

        /// <summary>
        /// 显示提示消息
        /// </summary>
        /// <param name="message">消息内容</param>
        public static void ShowInfoMessage(IWin32Window owner, string message)
        {
            if (owner != null)
            {
                if ((owner as Control).InvokeRequired)
                {
                    (owner as Control).Invoke((MethodInvoker)(() => { ShowInfoMessage(owner, message); }));
                }
                else
                {
                    if (SetInitializationMessage(message))
                    {
                        return;
                    }
                    MessageBoxEx.Show(owner, message, "提示", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                ShowInfoMessage(message);
            }
        }

        /// <summary>
        /// Format格式显示提示消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="args">Format的参数</param>
        public static void ShowInfoMessage(IWin32Window owner, string message, params object[] args)
        {
            ShowInfoMessage(owner, string.Format(message, args));
        }

        /// <summary>
        /// 显示警告消息
        /// </summary>
        /// <param name="message">消息内容</param>
        public static void ShowWarningMessage(IWin32Window owner, string message)
        {
            if (owner != null)
            {
                if ((owner as Control).InvokeRequired)
                {
                    (owner as Control).Invoke((MethodInvoker)(() => { ShowWarningMessage(owner, message); }));
                }
                else
                {
                    if (SetInitializationMessage(message))
                    {
                        return;
                    }
                    MessageBoxEx.Show(owner, message, "警告", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                ShowWarningMessage(message);
            }
        }

        /// <summary>
        /// Format格式显示提示消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="args">Format的参数</param>
        public static void ShowWarningMessage(IWin32Window owner, string message, params object[] args)
        {
            ShowWarningMessage(owner, string.Format(message, args));
        }

        /// <summary>
        /// buttonNum = 3 三个按钮 返回 Yes,No,Cancel
        /// buttonNum = 2 两个按钮 返回 Yes,No
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="buttonNum">按钮数量</param>
        /// <returns>返回DialogResult</returns>
        public static DialogResult ShowQuestionMessage(IWin32Window owner, string message, int buttonNum = 3)
        {
            MessageBoxButtons btn = MessageBoxButtons.YesNoCancel;
            if (buttonNum == 2) btn = MessageBoxButtons.YesNo;
            return ShowQuestionMessage(owner, message, btn, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// 询问提示框
        /// </summary>
        /// <param name="owner">父对象</param>
        /// <param name="message">消息文本</param>
        /// <param name="btn">按钮枚举</param>
        /// <param name="defaultButton">默认按钮</param>
        /// <returns></returns>
        public static DialogResult ShowQuestionMessage(IWin32Window owner, string message, MessageBoxButtons btn, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            if (owner != null)
            {
                if ((owner as Control).InvokeRequired)
                {
                    return (DialogResult)(owner as Control).Invoke(new Func<IWin32Window, string, MessageBoxButtons, MessageBoxDefaultButton, DialogResult>(ShowQuestionMessage), owner, message, btn, defaultButton);
                }
                else
                {
                    return MessageBoxEx.Show(owner, message, "询问", "", btn, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                return ShowQuestionMessage(message, btn, defaultButton);
            }
        }
        #endregion

        #region 有附加信息的消息框
        /// <summary>
        /// 显示有附件消息的错误提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">附加消息</param>
        public static void ShowErrorMessage(string message, string attachMessage)
        {
            if (SetInitializationMessage(message))
            {
                return;
            }
            MessageBoxEx.Show(message, "错误", attachMessage, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 显示有附件消息的错误提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">异常信息</param>
        public static void ShowErrorMessage(string message, Exception exception)
        {
            if (SetInitializationMessage(message))
            {
                return;
            }
            MessageBoxEx.Show(message, "错误", exception, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 显示有附件消息的提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">附加信息</param>
        public static void ShowInfoMessage(string message, string attachMessage)
        {
            if (SetInitializationMessage(message))
            {
                return;
            }
            MessageBoxEx.Show(message, "提示", attachMessage, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示有附件消息的提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">异常信息</param>
        public static void ShowInfoMessage(string message, Exception exception)
        {
            if (SetInitializationMessage(message))
            {
                return;
            }
            MessageBoxEx.Show(message, "提示", exception, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示有附件消息的警告提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">附加信息</param>
        public static void ShowWarningMessage(string message, string attachMessage)
        {
            if (SetInitializationMessage(message))
            {
                return;
            }
            MessageBoxEx.Show(message, "警告", attachMessage, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 显示有附件消息的警告提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">异常信息</param>
        public static void ShowWarningMessage(string message, Exception exception)
        {
            if (SetInitializationMessage(message))
            {
                return;
            }
            MessageBoxEx.Show(message, "警告", exception, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 询问提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">附加信息</param>
        /// <param name="btn">按钮枚举</param>
        /// <param name="defaultButton">默认按钮</param>
        /// <returns></returns>
        public static DialogResult ShowQuestionMessage(string message, string attachMessage, MessageBoxButtons btn, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            return MessageBoxEx.Show(message, "询问", attachMessage, btn, MessageBoxIcon.Question, defaultButton);
        }

        #endregion

        #region 有附加信息且有父的提示框
        /// <summary>
        /// 显示有附件消息的错误提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">附加消息</param>
        public static void ShowErrorMessage(IWin32Window owner, string message, string attachMessage)
        {
            if (owner != null)
            {
                if ((owner as Control).InvokeRequired)
                {
                    (owner as Control).Invoke((MethodInvoker)(() => { ShowErrorMessage(owner, message, attachMessage); }));
                }
                else
                {
                    if (SetInitializationMessage(message))
                    {
                        return;
                    }
                    MessageBoxEx.Show(owner, message, "错误", attachMessage, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                ShowErrorMessage(message, attachMessage);
            }
        }

        /// <summary>
        /// 显示有附件消息的错误提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">异常信息</param>
        public static void ShowErrorMessage(IWin32Window owner, string message, Exception exception)
        {
            if (owner != null)
            {
                if ((owner as Control).InvokeRequired)
                {
                    (owner as Control).Invoke((MethodInvoker)(() => { ShowErrorMessage(owner, message, exception); }));
                }
                else
                {
                    if (SetInitializationMessage(message))
                    {
                        return;
                    }
                    MessageBoxEx.Show(owner, message, "错误", exception, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                ShowErrorMessage(message, exception);
            }
        }

        /// <summary>
        /// 显示有附件消息的提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">附加信息</param>
        public static void ShowInfoMessage(IWin32Window owner, string message, string attachMessage)
        {
            if (owner != null)
            {
                if ((owner as Control).InvokeRequired)
                {
                    (owner as Control).Invoke((MethodInvoker)(() => { ShowInfoMessage(owner, message, attachMessage); }));
                }
                else
                {
                    if (SetInitializationMessage(message))
                    {
                        return;
                    }
                    MessageBoxEx.Show(owner, message, "提示", attachMessage, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                ShowInfoMessage(message, attachMessage);
            }
        }

        /// <summary>
        /// 显示有附件消息的提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">异常信息</param>
        public static void ShowInfoMessage(IWin32Window owner, string message, Exception exception)
        {
            if (owner != null)
            {
                if ((owner as Control).InvokeRequired)
                {
                    (owner as Control).Invoke((MethodInvoker)(() => { ShowInfoMessage(owner, message, exception); }));
                }
                else
                {
                    if (SetInitializationMessage(message))
                    {
                        return;
                    }
                    MessageBoxEx.Show(owner, message, "提示", exception, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                ShowInfoMessage(message, exception);
            }
        }

        /// <summary>
        /// 显示有附件消息的警告提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">附加信息</param>
        public static void ShowWarningMessage(IWin32Window owner, string message, string attachMessage)
        {
            if (owner != null)
            {
                if ((owner as Control).InvokeRequired)
                {
                    (owner as Control).Invoke((MethodInvoker)(() => { ShowWarningMessage(owner, message, attachMessage); }));
                }
                else
                {
                    if (SetInitializationMessage(message))
                    {
                        return;
                    }
                    MessageBoxEx.Show(owner, message, "警告", attachMessage, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                ShowWarningMessage(message, attachMessage);
            }
        }

        /// <summary>
        /// 显示有附件消息的警告提示框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">异常信息</param>
        public static void ShowWarningMessage(IWin32Window owner, string message, Exception exception)
        {
            if (owner != null)
            {
                if ((owner as Control).InvokeRequired)
                {
                    (owner as Control).Invoke((MethodInvoker)(() => { ShowWarningMessage(owner, message, exception); }));
                }
                else
                {
                    if (SetInitializationMessage(message))
                    {
                        return;
                    }
                    MessageBoxEx.Show(owner, message, "警告", exception, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                ShowWarningMessage(message, exception);
            }
        }

        /// <summary>
        /// 询问提示框
        /// </summary>
        /// <param name="owner">父对象</param>
        /// <param name="message">消息文本</param>
        /// <param name="attachMessage">附加信息</param>
        /// <param name="btn">按钮枚举</param>
        /// <param name="defaultButton">默认按钮</param>
        /// <returns></returns>
        public static DialogResult ShowQuestionMessage(IWin32Window owner, string message, string attachMessage, MessageBoxButtons btn, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            return MessageBoxEx.Show(owner, message, "询问", attachMessage, btn, MessageBoxIcon.Question, defaultButton);
        }
        #endregion


        private static bool SetInitializationMessage(string msg)
        {
            if (Comm.IsSystemInitialization)
            {
                Comm.InitializationMessage += $"{msg}{Environment.NewLine}";
                return true;
            }
            return false;
        }
    }
}