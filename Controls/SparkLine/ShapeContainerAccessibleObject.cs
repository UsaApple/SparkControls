using System;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    internal sealed class ShapeContainerAccessibleObject : Control.ControlAccessibleObject
    {
        private ShapeContainer m_ShapeContainer;
        private string m_Value;

        public ShapeContainerAccessibleObject(ShapeContainer ownerObject) : base(ownerObject)
        {
            if (ownerObject == null)
            {
                throw new ArgumentNullException("ownerObject");
            }
            this.m_ShapeContainer = ownerObject;
        }

        public override void DoDefaultAction()
        {
        }

        public override AccessibleObject GetChild(int index)
        {
            if (((index >= 0) && (index < this.m_ShapeContainer.Shapes.Count)) && (this.m_ShapeContainer.Shapes.Count > 0))
            {
                Shape shape = (Shape)this.m_ShapeContainer.Shapes[index];
                if (shape != null)
                {
                    return shape.AccessibilityObject;
                }
            }
            return null;
        }

        public override int GetChildCount()
        {
            if (this.m_ShapeContainer.Shapes == null)
            {
                return 0;
            }
            return this.m_ShapeContainer.Shapes.Count;
        }

        public override AccessibleObject GetFocused()
        {
            int childCount = this.GetChildCount();
            if (childCount > 0)
            {
                int num3 = childCount - 1;
                for (int i = 0; i <= num3; i++)
                {
                    AccessibleObject child = this.GetChild(i);
                    if ((child != null) && ((child.State & AccessibleStates.Focused) != AccessibleStates.None))
                    {
                        return child;
                    }
                }
            }
            return null;
        }

        public override int GetHelpTopic(out string fileName)
        {
            //levy 直接设置空有没有问题?
            fileName = string.Empty;
            return 0;
        }

        public override AccessibleObject GetSelected()
        {
            int childCount = this.GetChildCount();
            if (childCount > 0)
            {
                int num3 = childCount - 1;
                for (int i = 0; i <= num3; i++)
                {
                    AccessibleObject child = this.GetChild(i);
                    if ((child != null) && ((child.State & AccessibleStates.Selected) != AccessibleStates.None))
                    {
                        return child;
                    }
                }
            }
            return null;
        }

        public override AccessibleObject HitTest(int x, int y)
        {
            Point pt = new Point(x, y);
            Shape childAtPointInternal = this.m_ShapeContainer.GetChildAtPointInternal(pt);
            if (childAtPointInternal != null)
            {
                return childAtPointInternal.AccessibilityObject;
            }
            return null;
        }

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            switch (navdir)
            {
                case AccessibleNavigation.FirstChild:
                    if (this.m_ShapeContainer.Shapes.Count <= 0)
                    {
                        return null;
                    }
                    return ((Shape)this.m_ShapeContainer.Shapes[0]).AccessibilityObject;

                case AccessibleNavigation.LastChild:
                    {
                        int count = this.m_ShapeContainer.Shapes.Count;
                        if (count <= 0)
                        {
                            return null;
                        }
                        return ((Shape)this.m_ShapeContainer.Shapes[count - 1]).AccessibilityObject;
                    }
            }
            return null;
        }

        public override void Select(AccessibleSelection flags)
        {
        }

        public override string ToString()
        {
            return (this.GetType().FullName + ": Owner = " + this.m_ShapeContainer.GetType().FullName);
        }

        public override Rectangle Bounds
        {
            get
            {
                Rectangle r = new Rectangle(0, 0, this.m_ShapeContainer.Bounds.Width, this.m_ShapeContainer.Bounds.Height);
                return this.m_ShapeContainer.RectangleToScreen(r);
            }
        }

        public override string DefaultAction
        {
            get
            {
                string accessibleDefaultActionDescription = this.m_ShapeContainer.AccessibleDefaultActionDescription;
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
                string accessibleDescription = this.m_ShapeContainer.AccessibleDescription;
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
                return null;
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
                if (this.m_ShapeContainer == null)
                {
                    return null;
                }
                string accessibleName = this.m_ShapeContainer.AccessibleName;
                if (accessibleName != null)
                {
                    return accessibleName;
                }
                if (this.m_ShapeContainer.Site != null)
                {
                    return this.m_ShapeContainer.Site.Name;
                }
                if (this.m_ShapeContainer.Name != null)
                {
                    return this.m_ShapeContainer.Name;
                }
                return base.Name;
            }
            set
            {
                if (this.m_ShapeContainer != null)
                {
                    this.m_ShapeContainer.AccessibleName = value;
                }
            }
        }

        public ShapeContainer Owner
        {
            get
            {
                return this.m_ShapeContainer;
            }
        }

        public override AccessibleObject Parent
        {
            get
            {
                if (this.m_ShapeContainer.Parent == null)
                {
                    return null;
                }
                return this.m_ShapeContainer.Parent.AccessibilityObject;
            }
        }

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.None;
            }
        }

        public override AccessibleStates State
        {
            get
            {
                return AccessibleStates.Invisible;
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