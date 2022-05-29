using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    internal sealed class LineShapeDesigner : ShapeDesigner
    {
        public override bool CanDrag(HitFlag hitObj)
        {
            if (((hitObj != HitFlag.HitMove) && (hitObj != HitFlag.HitStart)) && (hitObj != HitFlag.HitEnd))
            {
                return false;
            }
            return true;
        }

        public override void Drag(HitFlag hitFlag, int x, int y)
        {
            DesignerTransaction transaction = null;
            IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
            try
            {
                if (hitFlag == HitFlag.HitMove)
                {
                    transaction = host.CreateTransaction("Move " + this.Component.Site.Name);
                }
                else if ((hitFlag == HitFlag.HitStart) | (hitFlag == HitFlag.HitEnd))
                {
                    transaction = host.CreateTransaction("Resize " + this.Component.Site.Name);
                }
                else
                {
                    return;
                }
                IComponentChangeService service = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
                if (service != null)
                {
                    service.OnComponentChanging(this.LineShape, null);
                }
                if (hitFlag == HitFlag.HitMove)
                {
                    this.LineShape.X1 += x;
                    this.LineShape.Y1 += y;
                    this.LineShape.X2 += x;
                    this.LineShape.Y2 += y;
                }
                else if (hitFlag == HitFlag.HitStart)
                {
                    this.LineShape.X1 += x;
                    this.LineShape.Y1 += y;
                }
                else if (hitFlag == HitFlag.HitEnd)
                {
                    this.LineShape.X2 += x;
                    this.LineShape.Y2 += y;
                }
                if (service != null)
                {
                    service.OnComponentChanged(this.LineShape, null, null, null);
                }
                this.OnDragging = false;
            }
            catch (Exception exception1)
            {
                //ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                //ProjectData.ClearProjectError();
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Commit();
                    ((IDisposable)transaction).Dispose();
                }
            }
        }

        public override SelectObject DrawAdornments(Graphics g, bool primary, Point screenOffset, Color backcolor)
        {
            Point startPoint = this.LineShape.Parent.PointToScreen(this.LineShape.StartPoint);
            startPoint.Offset(0 - screenOffset.X, 0 - screenOffset.Y);
            Point endPoint = this.LineShape.Parent.PointToScreen(this.LineShape.EndPoint);
            endPoint.Offset(0 - screenOffset.X, 0 - screenOffset.Y);
            int dx = -3;
            startPoint.Offset(dx, dx);
            endPoint.Offset(dx, dx);
            SelectLineObject obj3 = new SelectLineObject(startPoint, endPoint, primary);
            Rectangle bounds = new Rectangle(startPoint.X, startPoint.Y, 6, 6);
            DesignerUtility.DrawGrabHandler(g, bounds, primary);
            bounds.Width++;
            bounds.Height++;
            obj3.DrawBounds.Add(bounds);
            bounds = new Rectangle(endPoint.X, endPoint.Y, 6, 6);
            DesignerUtility.DrawGrabHandler(g, bounds, primary);
            bounds.Width++;
            bounds.Height++;
            obj3.DrawBounds.Add(bounds);
            return obj3;
        }

        public override void DrawFeedback(HitFlag hitFlag, Point offset, ref Graphics g, StatusBarCommand cmd, Control newContainer)
        {
            if (((hitFlag == HitFlag.HitEnd) || (hitFlag == HitFlag.HitStart)) || (hitFlag == HitFlag.HitMove))
            {
                Point endPoint = this.LineShape.EndPoint;
                endPoint = this.LineShape.Parent.PointToScreen(endPoint);
                Point startPoint = this.LineShape.StartPoint;
                startPoint = this.LineShape.Parent.PointToScreen(startPoint);
                if (hitFlag == HitFlag.HitEnd)
                {
                    endPoint.Offset(offset);
                }
                else if (hitFlag == HitFlag.HitStart)
                {
                    startPoint.Offset(offset);
                }
                else if (hitFlag == HitFlag.HitMove)
                {
                    endPoint.Offset(offset);
                    startPoint.Offset(offset);
                }
                this.LineShape.DrawInternal(g, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                if (cmd != null)
                {
                    if (newContainer != null)
                    {
                        endPoint = newContainer.PointToClient(endPoint);
                        startPoint = newContainer.PointToClient(startPoint);
                    }
                    else
                    {
                        endPoint = this.LineShape.Parent.PointToClient(endPoint);
                        startPoint = this.LineShape.Parent.PointToClient(startPoint);
                    }
                    cmd.SetLineStatusBarInfo(startPoint, endPoint);
                }
            }
        }

        public override HitFlag GetAdornmentsHitTest(Point p, bool testMove)
        {
            if (this.LineShape.Parent != null)
            {
                Point pt = this.LineShape.Parent.PointToClient(p);
                Size size = new Size(6, 6);
                Rectangle rectangle2 = new Rectangle(this.LineShape.StartPoint, size);
                size = new Size(6, 6);
                Rectangle rectangle = new Rectangle(this.LineShape.EndPoint, size);
                int x = -3;
                rectangle2.Offset(x, x);
                rectangle.Offset(x, x);
                if (rectangle2.Contains(pt))
                {
                    return HitFlag.HitStart;
                }
                if (rectangle.Contains(pt))
                {
                    return HitFlag.HitEnd;
                }
                if (testMove && this.LineShape.HitTest(p.X, p.Y))
                {
                    return HitFlag.HitMove;
                }
            }
            return HitFlag.HitNone;
        }

        public override Cursor GetCursor(HitFlag hitFlag)
        {
            if (hitFlag == HitFlag.HitMove)
            {
                return Cursors.SizeAll;
            }
            if (this.LineShape.X1 == this.LineShape.X2)
            {
                return Cursors.SizeNS;
            }
            if (this.LineShape.Y1 == this.LineShape.Y2)
            {
                return Cursors.SizeWE;
            }
            double num = ((double)(this.LineShape.Y2 - this.LineShape.Y1)) / ((double)(this.LineShape.X2 - this.LineShape.X1));
            if (num > 0.0)
            {
                return Cursors.SizeNWSE;
            }
            return Cursors.SizeNESW;
        }

        protected override void PostFilterProperties(IDictionary properties)
        {
            base.PostFilterProperties(properties);
            properties["Bounds"] = TypeDescriptor.CreateProperty(typeof(LineShapeDesigner), "Bounds", typeof(Rectangle), new System.Attribute[] { CategoryAttribute.Design, DesignOnlyAttribute.Yes, BrowsableAttribute.No });
            properties["Location"] = TypeDescriptor.CreateProperty(typeof(LineShapeDesigner), "Location", typeof(Point), new System.Attribute[] { CategoryAttribute.Design, DesignOnlyAttribute.Yes, BrowsableAttribute.No });
            properties["Size"] = TypeDescriptor.CreateProperty(typeof(LineShapeDesigner), "Size", typeof(System.Drawing.Size), new System.Attribute[] { CategoryAttribute.Design, DesignOnlyAttribute.Yes, BrowsableAttribute.No });
        }

        public override void SetLocation(Point p)
        {
            this.LineShape.SetLocation(p);
        }

        private Rectangle Bounds
        {
            get
            {
                return this.LineShape.BoundRect;
            }
            set
            {
                //levy
            }
        }

        private SparkLine LineShape
        {
            get
            {
                return (SparkLine)this.Component;
            }
        }

        public Point Location
        {
            get
            {
                return this.LineShape.Location;
            }
            set
            {
                this.LineShape.Location = value;
            }
        }

        public Size Size
        {
            get
            {
                return this.LineShape.Size;
            }
            set
            {
                this.LineShape.Size = value;
            }
        }
    }
}