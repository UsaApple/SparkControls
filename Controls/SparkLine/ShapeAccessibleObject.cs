using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    internal abstract class ShapeAccessibleObject : AccessibleObject
    {
        private Shape m_Shape;
        private string m_Value;

        protected ShapeAccessibleObject(Shape ownerObject)
        {
            if (ownerObject == null)
            {
                throw new ArgumentNullException("ownerObject");
            }
            this.m_Shape = ownerObject;
        }

        public override void DoDefaultAction()
        {
            int x = (int)Math.Round((double)(((double)this.m_Shape.BoundRect.Width) / 2.0));
            int y = (int)Math.Round((double)(((double)this.m_Shape.BoundRect.Height) / 2.0));
            this.m_Shape.OnClick(new MouseEventArgs(MouseButtons.Left, 1, x, y, 0));
        }

        public override AccessibleObject GetChild(int index)
        {
            return null;
        }

        public override int GetChildCount()
        {
            return 0;
        }

        public override AccessibleObject GetFocused()
        {
            return null;
        }

        public override int GetHelpTopic(out string fileName)
        {
            int num2 = 0;
            QueryAccessibilityHelpEventArgs e = new QueryAccessibilityHelpEventArgs();
            this.m_Shape.DoQueryAccessibilityHelp(e);
            fileName = e.HelpNamespace;
            if ((fileName != null) && (fileName.Length > 0))
            {
                string path = fileName;
                FileIOPermission permission = new FileIOPermission(PermissionState.None)
                {
                    AllFiles = FileIOPermissionAccess.PathDiscovery
                };
                permission.Assert();
                try
                {
                    path = Path.GetFullPath(fileName);
                }
                catch (ArgumentException exception1)
                {
                    //ProjectData.SetProjectError(exception1);
                    ArgumentException exception = exception1;
                    //ProjectData.ClearProjectError();
                }
                catch (SecurityException exception8)
                {
                    //ProjectData.SetProjectError(exception8);
                    SecurityException exception2 = exception8;
                    //ProjectData.ClearProjectError();
                }
                catch (NotSupportedException exception9)
                {
                    //ProjectData.SetProjectError(exception9);
                    NotSupportedException exception3 = exception9;
                    //ProjectData.ClearProjectError();
                }
                catch (PathTooLongException exception10)
                {
                    //ProjectData.SetProjectError(exception10);
                    PathTooLongException exception4 = exception10;
                    //ProjectData.ClearProjectError();
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
                new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path).Demand();
            }
            if (e.HelpKeyword == null)
            {
                return base.GetHelpTopic(out fileName);
            }
            try
            {
                num2 = int.Parse(e.HelpKeyword, CultureInfo.InvariantCulture);
            }
            catch (ArgumentNullException exception11)
            {
                //ProjectData.SetProjectError(exception11);
                ArgumentNullException exception5 = exception11;
                //ProjectData.ClearProjectError();
            }
            catch (FormatException exception12)
            {
                //ProjectData.SetProjectError(exception12);
                FormatException exception6 = exception12;
                //ProjectData.ClearProjectError();
            }
            catch (OverflowException exception13)
            {
                //ProjectData.SetProjectError(exception13);
                OverflowException exception7 = exception13;
                //ProjectData.ClearProjectError();
            }
            return num2;
        }

        public override AccessibleObject GetSelected()
        {
            return null;
        }

        public override AccessibleObject HitTest(int x, int y)
        {
            return null;
        }

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            return null;
        }

        public override void Select(AccessibleSelection flags)
        {
            if (flags == AccessibleSelection.TakeFocus)
            {
                this.Owner.Focus();
            }
        }

        public override string ToString()
        {
            return (this.GetType().FullName + ": Owner = " + this.m_Shape.GetType().FullName);
        }

        public override Rectangle Bounds
        {
            get
            {
                Point location = new Point(0, 0);
                Rectangle rect = new Rectangle(location, this.m_Shape.BoundRect.Size);
                return this.m_Shape.RectangleToScreen(rect);
            }
        }

        public override string DefaultAction
        {
            get
            {
                string accessibleDefaultActionDescription = this.m_Shape.AccessibleDefaultActionDescription;
                if (accessibleDefaultActionDescription != null)
                {
                    return accessibleDefaultActionDescription;
                }
                return base.DefaultAction;
            }
        }

        public override string Description
        {
            get
            {
                string accessibleDescription = this.m_Shape.AccessibleDescription;
                if (accessibleDescription != null)
                {
                    return accessibleDescription;
                }
                return base.Description;
            }
        }

        public override string Help
        {
            get
            {
                QueryAccessibilityHelpEventArgs e = new QueryAccessibilityHelpEventArgs();
                this.Owner.DoQueryAccessibilityHelp(e);
                if (e.HelpString != null)
                {
                    return e.HelpString;
                }
                return base.Help;
            }
        }

        public override string KeyboardShortcut
        {
            get
            {
                return null;
            }
        }

        public override string Name
        {
            get
            {
                if (this.m_Shape == null)
                {
                    return null;
                }
                string accessibleName = this.m_Shape.AccessibleName;
                if (accessibleName != null)
                {
                    return accessibleName;
                }
                if (this.m_Shape.Site != null)
                {
                    return this.m_Shape.Site.Name;
                }
                if (this.m_Shape.Name != null)
                {
                    return this.m_Shape.Name;
                }
                return base.Name;
            }
            set
            {
                if (this.m_Shape != null)
                {
                    this.m_Shape.AccessibleName = value;
                }
            }
        }

        public Shape Owner
        {
            get
            {
                return this.m_Shape;
            }
        }

        public override AccessibleObject Parent
        {
            get
            {
                if (this.m_Shape.Parent == null)
                {
                    return null;
                }
                return this.m_Shape.Parent.AccessibilityObject;
            }
        }

        public override AccessibleRole Role
        {
            get
            {
                AccessibleRole accessibleRole = this.m_Shape.AccessibleRole;
                if (accessibleRole == AccessibleRole.Default)
                {
                    return AccessibleRole.Client;
                }
                return accessibleRole;
            }
        }

        public override AccessibleStates State
        {
            get
            {
                if (!this.m_Shape.InDesignMode)
                {
                    return AccessibleStates.None;
                }
                if ((this.Owner != null) && (this.Owner.Site != null))
                {
                    ISelectionService service = (ISelectionService)this.Owner.Site.GetService(typeof(ISelectionService));
                    if (service != null)
                    {
                        if (service.PrimarySelection.Equals(this.Owner))
                        {
                            return (AccessibleStates.Focused | AccessibleStates.Selected);
                        }
                        if (service.GetComponentSelected(this.Owner))
                        {
                            return AccessibleStates.Selected;
                        }
                    }
                }
                if (!this.m_Shape.Enabled)
                {
                    return AccessibleStates.Unavailable;
                }
                if (!this.m_Shape.Visible)
                {
                    return AccessibleStates.Invisible;
                }
                AccessibleStates none = AccessibleStates.None;
                if (this.m_Shape.CanSelect)
                {
                    none |= AccessibleStates.Selectable;
                }
                else
                {
                    return none;
                }
                if (this.m_Shape.CanFocus)
                {
                    none |= AccessibleStates.Focusable;
                }
                else
                {
                    return none;
                }
                if (this.m_Shape.Selected)
                {
                    none |= AccessibleStates.Selected;
                }
                else
                {
                    return none;
                }
                if (this.m_Shape.Focused)
                {
                    none |= AccessibleStates.Focused;
                }
                return none;
            }
        }

        public override string Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
            }
        }
    }
}