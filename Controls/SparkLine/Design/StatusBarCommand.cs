using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design;

namespace SparkControls.Controls
{
    internal sealed class StatusBarCommand
    {
        private IMenuCommandService m_MenuCommandService;
        private IServiceProvider m_ServiceProvider;
        private MenuCommand m_StatusRectCommand;

        private IMenuCommandService MenuCommandService
        {
            get
            {
                if (this.m_MenuCommandService == null)
                {
                    this.m_MenuCommandService = (IMenuCommandService)this.m_ServiceProvider.GetService(typeof(IMenuCommandService));
                }
                return this.m_MenuCommandService;
            }
        }

        private MenuCommand StatusRectCommand
        {
            get
            {
                if (this.m_StatusRectCommand == null && this.MenuCommandService != null)
                {
                    this.m_StatusRectCommand = this.MenuCommandService.FindCommand(MenuCommands.SetStatusRectangle);
                }
                return this.m_StatusRectCommand;
            }
        }

        public StatusBarCommand(IServiceProvider provider)
        {
            this.m_ServiceProvider = provider;
        }

        public void SetLineStatusBarInfo(Point p1, Point p2)
        {
            Rectangle statusBarInfo = checked(new Rectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y)));
            this.SetStatusBarInfo(statusBarInfo);
        }

        public void SetStatusBarInfo(Rectangle bounds)
        {
            if (this.StatusRectCommand != null)
            {
                this.StatusRectCommand.Invoke(bounds);
            }
        }

        public void SetStatusBarInfo(Shape sh)
        {
            if (sh != null)
            {
                if (typeof(SparkLine).IsAssignableFrom(sh.GetType()))
                {
                    SparkLine lineShape = (SparkLine)sh;
                    this.SetLineStatusBarInfo(lineShape.StartPoint, lineShape.EndPoint);
                }
                //levy 去掉SimpleShape的分支
                //else if (typeof(SimpleShape).IsAssignableFrom(sh.GetType()))
                //{
                //	this.SetStatusBarInfo(((SimpleShape)sh).Bounds);
                //}
            }
        }
    }
}