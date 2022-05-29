using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using SparkControls.Foundation;

namespace SparkControls.Public
{
    #region 控件边框
    public enum EnumControlBorder
    {
        /// <summary>
        /// 无边框
        /// </summary>
        None = 0,
        /// <summary>
        /// 有边框
        /// </summary>
        Border = 2,
        /// <summary>
        /// 自动边框
        /// </summary>
        Line = 3
    }
    #endregion

    /// <summary>
    ///    '****************************************************<br/>
    ///    '
    ///    '打印函数库<br/>
    ///    '---------------------------------------------------<br/>
    ///    '   修改记录:<br/>
    ///    '****************************************************<br/>
    /// </summary>
    public class Printer
    {
        public Printer()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            try
            {
                InitializeComponent();//初始化控件
            }
            catch (Exception e) { throw e; }
        }

        ~Printer()
        {
            try
            {
                printDocument1.Dispose();
                this.components.Dispose();
            }
            catch { throw; }
        }


        #region 私有变量
        protected Image gCheked = null;
        protected Image gUnCheked = null;
        protected Control CurrentForm = null;
        private Container components;

        protected int B5PageHeight = 856;//856
        protected int A4PageHeight = 1145;//A4
        protected int PrePayHeight = 372;//预交金纸张长度

        /// <summary>
        /// 空白边缘
        /// </summary>
        protected Point pBlankMargin = new Point(0, 0);

        /// <summary>
        /// 控件打印边框
        /// </summary>
        protected EnumControlBorder myControlBorder = EnumControlBorder.Border;

        /// <summary>
        /// 打印任务
        /// </summary>
        public System.Drawing.Printing.PrintDocument printDocument1 = null;

        /// <summary>
        /// 是否多行文本框自动扩展
        /// </summary>
        public bool bIsDataAutoExtend = true;

        /// <summary>
        /// 是否打印背景图象
        /// </summary>
        protected bool bIsPrintBackImage = true;

        /// <summary>
        /// 打印容器组
        /// </summary>
        protected Control[] cContainer;

        /// <summary>
        /// 当前打印的页码
        /// </summary>
        protected int iPage = 0;//当前页

        /// <summary>
        /// 当前打印纸张高度
        /// </summary>
        public int iPageHeight = 1145;//默认A4纸

        /// <summary>
        /// 页大小
        /// </summary>
        protected int PageHeight
        {
            set => this.iPageHeight = value;
            get => this.iPageHeight;
        }

        /// <summary>
        /// 是否套打
        /// </summary>
        protected bool isPrintInputBox = false;

        /// <summary>
        /// 是否自动设置纸张
        /// </summary>
        private bool isResetPage = false;

        /// <summary>
        /// 页码控件
        /// </summary>
        protected Control myPageLabel = null;

        /// <summary>
        /// 全部高度
        /// </summary>
        protected int allTop = 0;

        /// <summary>
        /// 自动变化字体适应多行文本框
        /// </summary>
        private bool bIsAutoFont = true;

        /// <summary>
        /// 是否含有表格，默认是false.
        /// 是否包含Farpoint
        /// </summary>
        private bool bHaveGrid = false;

        /// <summary>
        /// 是否显示取消窗口
        /// 默认false
        /// </summary>
        private bool bIsCanCancel = false;

        /// <summary>
        /// 添加的页码，需要重新
        /// </summary>
        public int addpage = 0;

        protected System.Drawing.Printing.PaperSize pageSize = new System.Drawing.Printing.PaperSize("A4", 800, 1145);
        private int iLoop = 0;

        protected int CurrentPageHeight = 0;

        /// <summary>
        /// 最大页
        /// </summary>
        public int maxpage = 0;

        protected int offsetX = 0;
        protected int offsetY = 0;
        private ImageList imageList1;
        protected Panel p = new Panel();

        #endregion

        #region 属性


        /// <summary>
        /// 空白边缘
        /// </summary>
        public Point BlankMargin
        {
            get => pBlankMargin;
            set => this.pBlankMargin = value;
        }


        /// <summary>
        /// 打印控件边框
        /// </summary>
        public EnumControlBorder ControlBorder
        {
            set => this.myControlBorder = value;
            get => this.myControlBorder;
        }


        /// <summary>
        /// 打印文档
        /// </summary>
        public System.Drawing.Printing.PrintDocument PrintDocument => this.printDocument1;


        /// <summary>
        /// 数据窗口行自动扩展到页底
        /// </summary>
        public bool IsDataAutoExtend
        {
            set => this.bIsDataAutoExtend = value;
        }


        /// <summary>
        /// 是否打印背景图片
        /// </summary>
        public bool IsPrintBackImage
        {
            set => this.bIsPrintBackImage = value;
        }


        /// <summary>
        /// 是否只打印输入部分
        /// </summary>
        public bool IsPrintInputBox
        {
            get => this.isPrintInputBox;
            set => this.isPrintInputBox = value;
        }


        /// <summary>
        /// 是否重新设置纸张
        /// false 如果纸张存在，就不重新设置
        /// true 纸张存在，重新设置大小
        /// </summary>
        public bool IsResetPage
        {
            get => this.isResetPage;
            set => this.isResetPage = value;
        }


        /// <summary>
        /// 显示页码的控件
        /// </summary>
        public Control PageLabel
        {
            get => this.myPageLabel;
            set => this.myPageLabel = value;
        }


        /// <summary>
        /// 自动字体变动
        /// </summary>
        public bool IsAutoFont
        {
            get => this.bIsAutoFont;
            set => this.bIsAutoFont = value;
        }


        /// <summary>
        /// 是否含有表格
        /// </summary>
        public bool IsHaveGrid
        {
            set => bHaveGrid = value;
        }


        /// <summary>
        /// 是否显示取消窗口
        /// 默认false
        /// </summary>
        public bool IsCanCancel
        {
            get => this.bIsCanCancel;
            set => this.bIsCanCancel = value;
        }


        #endregion

        #region 私有函数

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Printer));
            this.components = new System.ComponentModel.Container();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            // 
            // imageList1
            // 
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;

            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printDocument1.DefaultPageSettings.Margins.Left = 0;
            this.printDocument1.DefaultPageSettings.Margins.Right = 0;
            this.printDocument1.DefaultPageSettings.Margins.Top = 0;
            this.printDocument1.DefaultPageSettings.Margins.Bottom = 0;
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocument1_PrintPage);
            gCheked = this.imageList1.Images[0];
            gUnCheked = this.imageList1.Images[1];
            this.iPageHeight = 1145; //默认A4纸张
        }


        /// <summary>
        /// 获得控件位移
        /// </summary>
        /// <param name="c"></param>
        private void GetOffSet(Control c)
        {
            if (c.Parent != null && c.Parent != this.CurrentForm)
            {
                offsetX += c.Parent.Left; offsetY += c.Parent.Top;
                if (c.Parent != null) GetOffSet(c.Parent);
            }
        }

        /// <summary>
        /// 画背景
        /// </summary>
        /// <param name="g"></param>
        /// <param name="form"></param>
        /// <param name="page"></param>
        protected virtual void Draw(Graphics g, Control form, int page)
        {
            allTop = 0;
            g.FillRectangle(new SolidBrush(form.BackColor), 0, 0, this.printDocument1.DefaultPageSettings.PaperSize.Width, this.iPageHeight);
            if (this.bIsPrintBackImage)//打印背景图片
            {
                if (form.BackgroundImage is Image c)
                {
                    int iX = this.printDocument1.DefaultPageSettings.PaperSize.Width / c.Width, iY = this.iPageHeight / c.Height;
                    for (int j = 0; j < iY + 1; j++)
                    {
                        for (int i = 0; i < iX + 1; i++)
                            g.DrawImage(c, c.Width * i, c.Height * j);
                    }
                }
            }
            //电子病历容器
            //if(form.GetType()== typeof(WNT.EPRControl.emrPanel))
            //{
            //    if(((WNT.EPRControl.emrPanel)form).自动分页)
            //        this.bIsDataAutoExtend = false;//非自动扩展
            //}
            this.CurrentForm = form;
            this.DrawForm(g, form, page);
            DrawPageNum(g, form, page);//画页码
        }


        /// <summary>
        /// 画容器
        /// </summary>
        /// <param name="g"></param>
        /// <param name="form"></param>
        /// <param name="page"></param>
        protected virtual void DrawForm(Graphics g, Control form, int page)
        {
            //判断是否用户组件
            if (form.Container != null)
            {
                foreach (Component m in form.Container.Components)
                {
                    if (m is Control c && c.Visible)
                    {
                        offsetX = 0; offsetY = 0;
                        GetOffSet(c);//获得位移							
                        if ((c.Top + offsetY >= page * iPageHeight && c.Top < (page + 1) * iPageHeight) || (c.Top + offsetY + c.Height > page * iPageHeight && c.Top + offsetY <= page * iPageHeight))
                        {
                            allTop = c.Top - (page * iPageHeight);
                            if (c != this.PageLabel)
                            {
                                //Control iu = c as WNT.EPRControl.IUserControlable;
                                //if(iu!=null) iu.IsPrint = true;//打印开始
                                this.DrawControl(c, g, allTop);
                                //if(iu!=null) iu.IsPrint = false;//打印完毕
                            }
                        }

                    }
                }


            }
            //else if (form.GetType().ToString().IndexOf("Spread") > 0)
            //{
            //李大夫说屏蔽 放开后会跳页打印 
            //this.DrawControl(form, g, allTop);
            //}
            else
            {
                foreach (Control c in form.Controls)
                {
                    if (c != null && c.Visible)
                    {
                        try
                        {
                            offsetX = 0; offsetY = 0;
                            GetOffSet(c);//获得位移

                            //判断是否用户组件
                            //C iu = c as WNT.EPRControl.IUserControlable;
                            //if(iu!=null) iu.IsPrint = true;//打印开始
                            if ((c.Top + offsetY >= page * iPageHeight && c.Top < (page + 1) * iPageHeight) || (c.Top + offsetY + c.Height > page * iPageHeight && c.Top + offsetY <= page * iPageHeight))
                            {
                                allTop = c.Top - (page * iPageHeight);
                                if (c != this.PageLabel)
                                {
                                    this.DrawControl(c, g, allTop);
                                }
                            }

                            if (ContinuePrint(c.GetType().Name))
                            {
                                continue;
                            }
                            if (c.Controls.Count > 0)
                            {
                                this.DrawForm(g, c, page);
                            }
                            //if(iu!=null) iu.IsPrint = false;//打印完毕
                        }
                        catch { }
                    }
                }
            }
        }


        /// <summary>
        /// 画控件
        /// </summary>
        /// <param name="c"></param>
        /// <param name="g"></param>
        /// <param name="allTop"></param>
        protected virtual void DrawControl(Control c, Graphics g, int allTop)
        {
            //控件不显示不画
            if (c.Visible == false) return;

            #region  判断位置
            string strType = c.GetType().ToString().Substring(c.GetType().ToString().LastIndexOf(".") + 1);

            //是表格不画子控件
            //判断父级控件类型是不是表格，是表格不画里面的线
            if (c.Parent != null)
            {
                string parentType = c.Parent.GetType().ToString().Substring(c.Parent.GetType().ToString().LastIndexOf(".") + 1);



                if (parentType == "Grid") return;
            }



            int ControlLeft = c.Left + pBlankMargin.X + offsetX;
            int ControlTop = allTop + (int)pBlankMargin.Y + offsetY;
            int ControlWidth = c.Width;
            int ControlHeight = c.Height;
            int iFill = 0;
            int ControlBackWidth = 0;
            int ControlBackHeight = 0;
            int ControlBackLeft;
            int ControlBackTop;
            int ControlForeLeft;
            int ControlForeTop;
            GetOffSet(c, allTop, ref iFill, ref ControlBackWidth, ref ControlBackHeight, out ControlBackLeft, out ControlBackTop, out ControlForeLeft, out ControlForeTop);

            #endregion

            #region 画页码
            if (c != null && c == this.myPageLabel)
            {
                int page = 0;
                if (addpage == 0)
                {
                    if (maxpage == 0) maxpage = 1;
                    page = maxpage;
                }
                else
                    page = addpage - 1;


                if (c.Tag != null && c.Tag.ToString().IndexOf("{0}") >= 0 && c.Tag.ToString().IndexOf("{1}") >= 0)
                {
                    c.Text = string.Format(c.Tag.ToString(), page, maxpage);
                }
                else if (c.Tag != null && c.Tag.ToString().IndexOf("{0}") >= 0)
                {
                    c.Text = string.Format(c.Tag.ToString(), page);
                }
                else
                {
                    c.Text = string.Format("第{0}/{1}页", page, maxpage);
                }

            }
            #endregion

            //非打印控件
            if (c.Tag != null && c.Tag.ToString() == "EMRGRIDLINE") return;

            if (strType == "Label" || strType == "SparkLabel" || strType == "TcLabel" || strType == "LcLabel")
            {
                if (this.isPrintInputBox) return;
                Label t = c as Label;

                if (t.BorderStyle == BorderStyle.FixedSingle)
                {
                    ControlPaint.DrawBorder(g, new Rectangle(ControlLeft, ControlTop, ControlWidth, ControlHeight), Color.Black, ButtonBorderStyle.Solid);

                    if (t.AutoSize)
                        g.DrawString(c.Text, c.Font, new SolidBrush(c.ForeColor), ControlForeLeft, ControlForeTop - 2, new StringFormat());
                    else
                    {
                        #region {629A6AFE-71BC-4ab7-9B9A-AFF8B12F21C1}
                        //修改label打印没有按照文本的对齐属性打印
                        //目前只对left，right，center做了处理，对top，bottom没做处理
                        SizeF lblsize = g.MeasureString(c.Text, c.Font);
                        Rectangle r = new Rectangle();
                        if (t.TextAlign == ContentAlignment.TopLeft)
                        {
                            r = new Rectangle(ControlLeft, ControlTop, ControlWidth, ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.TopCenter)
                        {
                            r = new Rectangle(ControlLeft + Convert.ToInt32((ControlWidth - lblsize.Width) / 2), ControlTop, ControlWidth - Convert.ToInt32((ControlWidth - lblsize.Width) / 2), ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.TopRight)
                        {
                            r = new Rectangle(Convert.ToInt32(ControlLeft + ControlWidth - lblsize.Width), ControlTop, Convert.ToInt32(lblsize.Width), ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.MiddleLeft)
                        {
                            r = new Rectangle(ControlLeft, ControlTop + Convert.ToInt32((ControlHeight - lblsize.Height) / 2), ControlWidth, ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.MiddleCenter)
                        {
                            r = new Rectangle(ControlLeft + Convert.ToInt32((ControlWidth - lblsize.Width) / 2), ControlTop + Convert.ToInt32((ControlHeight - lblsize.Height) / 2), ControlWidth - Convert.ToInt32((ControlWidth - lblsize.Width) / 2), ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.MiddleRight)
                        {
                            r = new Rectangle(Convert.ToInt32(ControlLeft + ControlWidth - lblsize.Width), ControlTop + Convert.ToInt32((ControlHeight - lblsize.Height) / 2), Convert.ToInt32(lblsize.Width), ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.BottomLeft)
                        {
                            r = new Rectangle(ControlLeft, ControlTop, ControlWidth, ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.BottomCenter)
                        {
                            r = new Rectangle(ControlLeft + Convert.ToInt32((ControlWidth - lblsize.Width) / 2), ControlTop, ControlWidth - Convert.ToInt32((ControlWidth - lblsize.Width) / 2), ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.BottomRight)
                        {
                            r = new Rectangle(Convert.ToInt32(ControlLeft + ControlWidth - lblsize.Width), ControlTop, Convert.ToInt32(lblsize.Width), ControlHeight);
                        }
                        else
                        {
                            r = new Rectangle(ControlLeft, ControlTop, ControlWidth, ControlHeight);
                        }

                        g.DrawString(c.Text, c.Font, new SolidBrush(c.ForeColor), r, new StringFormat());
                        //g.DrawString(c.Text, c.Font, new SolidBrush(c.ForeColor),new Rectangle(ControlLeft, ControlTop, ControlWidth, ControlHeight), new StringFormat());                        
                        #endregion

                    }
                }
                else
                {
                    if (t.AutoSize)
                    {
                        var f = StringFormat.GenericDefault;
                        g.DrawString(c.Text, c.Font, new SolidBrush(c.ForeColor), ControlForeLeft, ControlForeTop, f);
                    }
                    else
                    {
                        #region {629A6AFE-71BC-4ab7-9B9A-AFF8B12F21C1}
                        //修改label打印没有按照文本的对齐属性打印
                        //目前只对left，right，center做了处理，对top，bottom没做处理
                        SizeF lblsize = g.MeasureString(c.Text, c.Font);
                        Rectangle r = new Rectangle();
                        if (t.TextAlign == ContentAlignment.TopLeft)
                        {
                            r = new Rectangle(ControlLeft, ControlTop, ControlWidth, ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.TopCenter)
                        {
                            r = new Rectangle(ControlLeft + Convert.ToInt32((ControlWidth - lblsize.Width) / 2), ControlTop, ControlWidth - Convert.ToInt32((ControlWidth - lblsize.Width) / 2), ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.TopRight)
                        {
                            r = new Rectangle(Convert.ToInt32(ControlLeft + ControlWidth - lblsize.Width), ControlTop, Convert.ToInt32(lblsize.Width), ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.MiddleLeft)
                        {
                            r = new Rectangle(ControlLeft, ControlTop, ControlWidth, ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.MiddleCenter)
                        {
                            r = new Rectangle(ControlLeft + Convert.ToInt32((ControlWidth - lblsize.Width) / 2), ControlTop, ControlWidth - Convert.ToInt32((ControlWidth - lblsize.Width) / 2), ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.MiddleRight)
                        {
                            r = new Rectangle(Convert.ToInt32(ControlLeft + ControlWidth - lblsize.Width), ControlTop, Convert.ToInt32(lblsize.Width), ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.BottomLeft)
                        {
                            r = new Rectangle(ControlLeft, ControlTop, ControlWidth, ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.BottomCenter)
                        {
                            r = new Rectangle(ControlLeft + Convert.ToInt32((ControlWidth - lblsize.Width) / 2), ControlTop, ControlWidth - Convert.ToInt32((ControlWidth - lblsize.Width) / 2), ControlHeight);
                        }
                        else if (t.TextAlign == ContentAlignment.BottomRight)
                        {
                            r = new Rectangle(Convert.ToInt32(ControlLeft + ControlWidth - lblsize.Width), ControlTop, Convert.ToInt32(lblsize.Width), ControlHeight);
                        }
                        else
                        {
                            r = new Rectangle(ControlLeft, ControlTop, ControlWidth, ControlHeight);
                        }

                        g.DrawString(c.Text, c.Font, new SolidBrush(c.ForeColor), r, new StringFormat());

                        //g.DrawString(c.Text, c.Font, new SolidBrush(c.ForeColor), new Rectangle(ControlLeft, ControlTop, ControlWidth, ControlHeight), new StringFormat());                        
                        #endregion
                    }
                }


            }
            else if (strType == "CheckBox" || strType == "SparkCheckBox" || strType == "TcCheckBox")
            {
                if (this.isPrintInputBox) return;
                CheckBox t = c as CheckBox;
                ControlTop += 2;
                if (c.Height - gCheked.Height > 0)
                {
                    ControlTop += (c.Height - gCheked.Height) / 2;
                }
                if (t.Checked)
                {

                    g.DrawImage(gCheked, ControlLeft, ControlTop);
                }
                else
                {
                    g.DrawImage(gUnCheked, ControlLeft, ControlTop);
                }
                g.DrawString(c.Text, c.Font, new SolidBrush(c.ForeColor), ControlForeLeft + gCheked.Width, ControlForeTop, new StringFormat());
            }
            else if (strType == "RadioButton" || strType == "SparkRadioButton" || strType == "TcRadioButton")
            {
                if (this.isPrintInputBox) return;
                RadioButton t = c as RadioButton;
                ControlTop += 2;

                if (c.Height - gCheked.Height > 0)
                {
                    ControlTop += (c.Height - gCheked.Height) / 2;
                }
                if (t.Checked)
                {
                    g.DrawImage(gCheked, ControlLeft, ControlTop);
                }
                else
                {
                    g.DrawImage(gUnCheked, ControlLeft, ControlTop);
                }
                g.DrawString(c.Text, c.Font, new SolidBrush(c.ForeColor), ControlForeLeft + gCheked.Width, ControlForeTop, new StringFormat());
            }
            else if (strType == "GroupBox" || strType == "SparkGroupBox")
            {
                if (this.isPrintInputBox) return;
                ControlPaint.DrawBorder(g, new Rectangle(ControlLeft, ControlTop, ControlWidth, ControlHeight), Color.Black, ButtonBorderStyle.Solid);
                g.FillRectangle(new SolidBrush(c.BackColor), ControlLeft + 10, ControlTop - 8, g.MeasureString(c.Text, c.Font).Width, g.MeasureString(c.Text, c.Font).Height);
                g.DrawString(c.Text, c.Font, new SolidBrush(c.ForeColor), ControlLeft + 10, ControlTop - 8, new StringFormat());
            }
            else if (strType == "PictureBox" || strType == "SparkPictureBox")
            {
                if (this.isPrintInputBox) return;
                PictureBox t = c as PictureBox;
                if (t.Image != null)
                {
                    try
                    {
                        Image m = t.Image.Clone() as Image;
                        g.DrawImage(m, ControlLeft, ControlTop, ControlWidth, ControlHeight);
                    }
                    catch { }
                }
            }
            else if (strType == "Panel" || strType == "SparkPanel")
            {
                if (this.isPrintInputBox) return;
                g.FillRectangle(new SolidBrush(c.BackColor), ControlBackLeft, ControlBackTop, ControlBackWidth, ControlBackHeight);
            }
            else if (strType == "TabPage" || strType == "TabControl" || strType == "SparkTabPage" || strType == "SparkTabControl")
            {

            }
            else if (strType == "ShapeContainer")
            {
                DrawLine(g, c, ControlLeft, ControlTop);
            }
            else if (strType == "SparkLine" || strType == "SparkLineH" || strType == "SparkLineV" ||
                     strType == "TcLine" || strType == "TcLineH" || strType == "TcLineV")
            {
                g.FillRectangle(new SolidBrush(c.BackColor), ControlBackLeft, ControlBackTop, ControlWidth, ControlHeight);
            }
            else if (strType == "RichTextBox" || strType == "SparkRichTextBox")
            {
                DrawRichText(g, c, ControlLeft, ControlTop, ControlWidth, ControlHeight, ControlForeLeft, ControlForeTop);
            }
            else if (strType == "FpSpread" || strType == "SparkFpEnter")
            {
                if (this.isPrintInputBox) return;//不打印没有输入的             
                DrawFarpoint(g, c, ControlLeft, ControlTop, ControlWidth, ControlHeight);
            }

            else if (strType.IndexOf("SpreadView") >= 0 || strType == "HScrollBar" || strType == "VScrollBar" || strType.IndexOf("ScrollBar") >= 0)
            {

            }
            else if (strType == "DataGrid")//strType.IndexOf("DataGrid")>=0)//DataGrid
            {
                if (this.isPrintInputBox) return;//不打印没有输入的
                #region 画表格 --必须有dtTable
                DataGrid t = c as DataGrid;
                int CaptionHeight = 20;
                //画表格
                g.FillRectangle(new SolidBrush(t.BackColor), ControlLeft, ControlTop, ControlWidth, ControlHeight);
                if (t.CaptionVisible)
                {
                    g.FillRectangle(new SolidBrush(t.CaptionBackColor), ControlLeft, ControlTop, ControlWidth, CaptionHeight);
                    g.DrawString(t.CaptionText, t.Font, new SolidBrush(t.CaptionForeColor), ControlForeLeft, ControlForeTop);
                }

                //画列
                System.Data.DataTable dt = t.DataSource as System.Data.DataTable;
                Rectangle r;
                string sTemp = "";
                if (dt != null)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        r = t.GetCellBounds(0, i);
                        g.FillRectangle(new SolidBrush(t.HeaderBackColor), ControlLeft + r.Left, ControlTop + CaptionHeight, r.Width, r.Height);
                        g.DrawString(dt.Columns[i].ColumnName, t.Font, new SolidBrush(t.HeaderForeColor), ControlLeft + r.Left, ControlTop + CaptionHeight);

                    }
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            try
                            {
                                sTemp = dt.Rows[j][i].ToString();
                            }
                            catch
                            {
                                sTemp = "";
                            }
                            r = t.GetCellBounds(j, i);
                            g.FillRectangle(new SolidBrush(t.BackColor), ControlLeft + r.Left, ControlTop + r.Top, r.Width, r.Height);
                            g.DrawString(sTemp, t.Font, new SolidBrush(t.ForeColor), ControlLeft + r.Left, ControlTop + r.Top);
                            //画grid 竖线
                            Pen pen = new Pen(t.GridLineColor);
                            g.DrawRectangle(pen, ControlLeft + r.Left, ControlTop + r.Top, r.Width, r.Height);

                        }
                    }
                }
                #endregion

            }
            else if (strType == "DateTimePicker" || strType == "SparkDateTimePicker")
            {
                //日期，属于输入控件
                try
                {
                    if (ControlBorder != EnumControlBorder.None && this.isPrintInputBox == false)//打印边框 
                    {
                        //ControlPaint.DrawButton(g,ControlLeft,ControlTop,ControlWidth,ControlHeight,bState);
                        if (ControlBorder == EnumControlBorder.Line)
                        {
                            //ControlPaint.DrawButton(g,new Rectangle(ControlLeft,ControlTop,ControlWidth,ControlHeight),Color.Gray,ButtonBorderStyle.Solid);
                            g.DrawLine(new Pen(Color.FromArgb(192, 192, 192), 1), ControlLeft, ControlTop + ControlHeight + 2, ControlLeft + ControlWidth, ControlTop + ControlHeight + 2);
                        }
                        else
                        {
                            ControlPaint.DrawBorder(g, new Rectangle(ControlLeft, ControlTop + 2, ControlWidth, ControlHeight), Color.Black, ButtonBorderStyle.Solid);
                        }
                    }
                    g.FillRectangle(new SolidBrush(c.BackColor), ControlBackLeft, ControlBackTop + 2, ControlBackWidth, ControlBackHeight);
                    if (((DateTimePicker)c).Value == DateTime.MinValue)
                    {
                        if (this.isPrintInputBox) return;//不打印数值
                        g.DrawString("00-00-00", c.Font, new SolidBrush(c.ForeColor), ControlForeLeft, ControlForeTop, new StringFormat());
                    }
                    else
                    {
                        g.DrawString(c.Text, c.Font, new SolidBrush(c.ForeColor), ControlForeLeft, ControlForeTop + 2, new StringFormat());
                    }
                }
                catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message); }
            }
            else if (strType == "Button" || strType == "SparkButton" || strType == "TcButton")//不打印按钮
            {
            }
            else if (strType == "Form" || strType == "SparkForm")//不打印窗口
            {

            }
            else if (strType == "TextBox" || strType == "SparkTextBox")
            {
                int topOffSet = 0;
                if (c.Parent != null && c.Parent.GetType().Name.StartsWith("Tc") || c.Parent.GetType().Name.StartsWith("Lc"))
                {
                    topOffSet = 2;
                }
                g.DrawString(c.Text, c.Font, new SolidBrush(c.ForeColor), ControlForeLeft, ControlForeTop + 2 + topOffSet, new StringFormat());
            }
            else
            {
                if (this.isPrintInputBox && c.Text == "") return;//不打印没有输入的
                if (strType.StartsWith("Tc") || strType.StartsWith("Lc")) return;
                var borderStyle = GetControlBorderStyle(c);
                if (borderStyle == null)
                {
                    if (ControlBorder != EnumControlBorder.None && this.isPrintInputBox == false)//打印边框 
                    {
                        if (ControlBorder == EnumControlBorder.Line)
                        {
                            g.DrawLine(new Pen(Color.FromArgb(192, 192, 192), 1), ControlLeft, ControlTop + ControlHeight + 2, ControlLeft + ControlWidth, ControlTop + ControlHeight + 2);
                        }
                        else
                        {
                            ControlPaint.DrawBorder(g, new Rectangle(ControlLeft, ControlTop + 2, ControlWidth, ControlHeight), Color.Black, ButtonBorderStyle.Solid);
                        }
                    }
                }
                else if (borderStyle.Value != BorderStyle.None)
                {
                    ControlPaint.DrawBorder(g, new Rectangle(ControlLeft, ControlTop + 2, ControlWidth, ControlHeight), Color.Black, ButtonBorderStyle.Solid);
                }
                g.FillRectangle(new SolidBrush(c.BackColor), ControlBackLeft, ControlBackTop + 2, ControlBackWidth, ControlBackHeight);
                if (c.Text != "")
                {
                    g.DrawString(c.Text, AutoFont(c, g), new SolidBrush(c.ForeColor), ControlForeLeft, ControlForeTop + 2, new StringFormat());//2+c.Left+(int)pBlankMargin.X,3+allTop+c.Height /2 -g.MeasureString("李",c.Font).Height/2+(int)pBlankMargin.Y		break;
                }

            }
        }

        private BorderStyle? GetControlBorderStyle(Control ctrl)
        {
            //if (ctrl.Parent != null)
            //{
            //    var type = ctrl.Parent.GetType().Name;
            //    if (type.StartsWith("Tc") || type.StartsWith("Lc"))
            //    {
            //        ctrl = ctrl.Parent;
            //    }
            //}
            var pro = ctrl.GetType().GetProperty("BorderStyle", BindingFlags.Instance | BindingFlags.Public | BindingFlags.ExactBinding);
            if (pro != null)
            {
                var obj = pro.GetValue(ctrl, null);
                if (obj is BorderStyle style)
                {
                    return style;
                }
            }
            return null;
        }


        private void GetOffSet(Control c, int allTop, ref int iFill, ref int ControlBackWidth, ref int ControlBackHeight, out int ControlBackLeft, out int ControlBackTop, out int ControlForeLeft, out int ControlForeTop)
        {
            if (this.ControlBorder == EnumControlBorder.Line)
            {
                //bState  =ButtonState.All;
                iFill = -2;
            }
            else if (this.ControlBorder == EnumControlBorder.Border)
            {
                //bState  =ButtonState.Flat;
                iFill = 2;
            }
            else if (this.ControlBorder == EnumControlBorder.None)
            {
                //bState  =ButtonState.Checked;
                iFill = 0;
            }

            ControlBackWidth = c.Width - (iFill * 2);
            ControlBackHeight = c.Height - (iFill * 2);

            ControlBackLeft = c.Left + pBlankMargin.X + iFill + offsetX;
            ControlBackTop = allTop + (int)pBlankMargin.Y + iFill + offsetY;


            if (iFill < 0)
            {
                //ControlBackWidth = c.Width;//-(iFill*2);
                ControlBackHeight = c.Height;
            }

            //如果控件高度小于零
            if (ControlBackHeight <= 0)
            {
                ControlBackHeight = c.Height;
            }
            //如果控件宽度
            if (ControlBackWidth <= 0)
            {
                ControlBackWidth = c.Width;
            }

            ControlForeLeft = c.Left + pBlankMargin.X + iFill + 2 + offsetX;
            ControlForeTop = allTop + (int)pBlankMargin.Y + iFill + 3 + offsetY;
        }


        /// <summary>
        /// 画页码
        /// </summary>
        /// <param name="g"></param>
        /// <param name="form"></param>
        /// <param name="page"></param>
        private void DrawPageNum(Graphics g, Control form, int page)
        {
            //加载页码
            if (this.PageLabel != null)
            {
                if (form.Container != null)
                {
                    foreach (Component m in form.Container.Components)
                    {
                        Control c = m as Control;
                        if (c != null && c.Visible)
                        {
                            offsetX = 0; offsetY = 0;
                            GetOffSet(c);//获得位移							
                            if ((c.Top + offsetY >= page * iPageHeight && c.Top < (page + 1) * iPageHeight) || (c.Top + offsetY + c.Height > page * iPageHeight && c.Top + offsetY <= page * iPageHeight))
                            {
                                allTop = c.Top - (page * iPageHeight);
                                if (c == this.PageLabel)
                                {
                                    this.DrawControl(c, g, allTop);
                                }
                            }

                        }
                    }
                }
                else
                {
                    foreach (Control c in form.Controls)
                    {
                        if (c != null && c.Visible)
                        {
                            try
                            {
                                offsetX = 0; offsetY = 0;
                                GetOffSet(c);//获得位移
                                if ((c.Top + offsetY >= page * iPageHeight && c.Top < (page + 1) * iPageHeight) || (c.Top + offsetY + c.Height > page * iPageHeight && c.Top + offsetY <= page * iPageHeight))
                                {
                                    allTop = c.Top - (page * iPageHeight);
                                    if (c == this.PageLabel)
                                    {
                                        this.DrawControl(c, g, allTop);
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
            }

        }


        /// <summary>
        /// 获得控件高度
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int GetControlHeight(Control p)
        {
            int max = 0;
            foreach (Control c in p.Controls)
            {
                if ((c.Top + c.Height) > max)
                {
                    max = c.Top + c.Height;
                }
            }
            return max;
        }


        /// <summary>
        /// 设置纸张大小
        /// </summary>
        /// <param name="document"></param>
        protected void setpagesize(ref System.Drawing.Printing.PrintDocument document)
        {
            document.DefaultPageSettings.PaperSize = pageSize;
            //
            //查找最符合的纸张
            //
            foreach (System.Drawing.Printing.PaperSize p in document.PrinterSettings.PaperSizes)
            {
                if (p.PaperName == pageSize.PaperName)
                {
                    document.DefaultPageSettings.PaperSize = p;
                    document.PrinterSettings.DefaultPageSettings.PaperSize = p;
                }
            }

        }


        /// <summary>
        /// 打印字体自动变动
        /// </summary>
        /// <param name="t"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        private Font AutoFont(Control t, Graphics g)
        {
            Font newFont = null;
            if (this.bIsAutoFont)//自动
            {

                {
                    newFont = t.Font;
                }
            }
            else
            {
                newFont = t.Font;
            }
            return newFont;
        }


        /// <summary>
        /// 画多行文本框
        /// </summary>
        /// <param name="g"></param>
        /// <param name="c"></param>
        /// <param name="ControlLeft"></param>
        /// <param name="ControlTop"></param>
        /// <param name="ControlWidth"></param>
        /// <param name="ControlHeight"></param>
        /// <param name="ControlForeLeft"></param>
        /// <param name="ControlForeTop"></param>
        private void DrawRichText(System.Drawing.Graphics g, Control c, int ControlLeft, int ControlTop, int ControlWidth, int ControlHeight, int ControlForeLeft, int ControlForeTop)
        {
            RichTextBox t = c as RichTextBox;
            t.Select(0, 0);
            t.ScrollToCaret();
            string oldText = t.Text;
            RectangleF r;
            int x = 0, y = 0;
            int ioffsetY = t.GetPositionFromCharIndex(0).Y;
            y = t.GetPositionFromCharIndex(t.TextLength - 1).Y - ioffsetY;

            int iCount = 0;

            #region 多行文本框
            if (bIsDataAutoExtend)//自动扩展
            {
                if (y + (int)g.MeasureString("李", t.Font).Height + 2 > ControlHeight)
                {
                    ControlHeight = y + (int)g.MeasureString("李", t.Font).Height + 2;
                    //this.allOffsetY = ControlHeight - c.Height;

                }
                //
                for (int iTextLength = 0; iTextLength < t.TextLength; iTextLength++)//for(int iTextLength =t.TextLength-1;iTextLength>=0;iTextLength--)
                {
                    t.Select(0, 0);
                    Point point = t.GetPositionFromCharIndex(iTextLength);
                    x = point.X;
                    y = point.Y;
                    y = y - ioffsetY;
                    t.Select(iTextLength, 1);
                    r = new RectangleF(ControlForeLeft + x, ControlForeTop + y, g.MeasureString(t.SelectedText, t.Font).Width, g.MeasureString(t.SelectedText, t.Font).Height);
                    g.DrawString(t.SelectedText, t.SelectionFont, new SolidBrush(t.SelectionColor), r);
                }
            }
            else //自动分页
            {
                iCount = y / t.Height + 1;
                if (maxpage < iCount) maxpage = iCount; //为多个分屏控件打印用

                if (addpage == 0 && maxpage == iCount)
                {
                    addpage = 1;
                }
                if (addpage <= iCount)
                {
                    int iTextLength = 0;
                    bool bCanSave = false;
                    if (c.Tag != null)
                    {
                        iTextLength = Converter.ToInt32(c.Tag);
                        bCanSave = true;
                    }
                    int iLength = iTextLength;
                    for (iTextLength = iLength; iTextLength < t.TextLength; iTextLength++)
                    {
                        t.Select(0, 0);
                        //t.ScrollToCaret();
                        y = t.GetPositionFromCharIndex(iTextLength).Y;
                        y = y - ioffsetY;

                        if (y >= (addpage - 1) * t.Height - g.MeasureString("李", t.Font).Height && y < (addpage) * t.Height - (g.MeasureString("李", t.Font).Height))
                        {
                            x = t.GetPositionFromCharIndex(iTextLength).X;
                            t.Select(iTextLength, 1);
                            y = y - (addpage - 1) * t.Height;
                            r = new RectangleF(ControlForeLeft + x, ControlForeTop + y + 2, g.MeasureString(t.SelectedText, t.Font).Width, g.MeasureString(t.SelectedText, t.Font).Height);
                            g.DrawString(t.SelectedText, t.SelectionFont, new SolidBrush(t.SelectionColor), r);
                        }
                        else if (y >= (addpage) * t.Height - g.MeasureString("李", t.Font).Height)
                        {
                            break;
                        }
                    }
                    if (bCanSave) c.Tag = iTextLength;
                    addpage++;
                }
                if (addpage > maxpage)
                {
                    addpage = 0;
                }
            }
            t.Select(0, 0);

            #endregion

            if (this.isPrintInputBox == false)//套打，不打印边框
            {
                if (t.BorderStyle != BorderStyle.None)
                {
                    //wolf edit 多行文本框也不打印边框，变态
                    ////打边框
                    //if(ControlBorder == enuControlBorder.Line && t.Multiline == false)
                    //{
                    //    g.DrawLine(new Pen(Color.FromArgb(192,192,192),1),ControlLeft,ControlTop+ControlHeight,ControlLeft+ControlWidth,ControlTop+ControlHeight);							
                    //}
                    //else
                    //{
                    //    if(iCount>1)ControlHeight=ControlHeight+5;//调整一下位置
                    //    ControlPaint.DrawBorder(g,new Rectangle(ControlLeft,ControlTop,ControlWidth,ControlHeight),Color.Black,ButtonBorderStyle.Solid);
                    //}
                }
            }
        }

        /// <summary>
        /// 画farpoint
        /// </summary>
        /// <param name="g"></param>
        /// <param name="c"></param>
        /// <param name="ControlLeft"></param>
        /// <param name="ControlTop"></param>
        /// <param name="ControlWidth"></param>
        /// <param name="ControlHeight"></param>
        private void DrawFarpoint(System.Drawing.Graphics g, Control c, int ControlLeft, int ControlTop, int ControlWidth, int ControlHeight)
        {
            //反射调用
            try
            {
                string path = Path.Combine(Consts.CURRENT_FILE_PATH, "SparkControls.FpSpread.dll");
                if (File.Exists(path))
                {
                    var ass = Assembly.UnsafeLoadFrom(path);
                    if (ass != null)
                    {
                        var type = ass.GetType("SparkControls.FpSpread.PrinterFpSpread");//类名
                        if (type != null)
                        {
                            var obj = Activator.CreateInstance(type, new object[] { this });//实例化
                            if (obj != null)
                            {
                                MethodInfo meth = type.GetMethod("DrawFarpoint");//加载方法
                                if (meth != null)
                                {
                                    meth.Invoke(obj, new Object[] { g, c, ControlLeft, ControlTop, ControlWidth, ControlHeight });//执行
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 画打印
        protected virtual void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (e.Cancel == true && bIsCanCancel)
            {
                MessageBox.Show("取消了");
                return;
            }
            Graphics g = null;
            if (iLoop > cContainer.GetUpperBound(0))
            {
                iLoop = 0;
                CurrentPageHeight = 0;
                return;
            }
            //大于pageHeight分页打印
            int intPrintAreaHeight, intPrintAreaWidth, marginLeft, marginTop;

            intPrintAreaHeight = printDocument1.DefaultPageSettings.PaperSize.Height - printDocument1.DefaultPageSettings.Margins.Top - printDocument1.DefaultPageSettings.Margins.Bottom;
            intPrintAreaWidth = printDocument1.DefaultPageSettings.PaperSize.Width - printDocument1.DefaultPageSettings.Margins.Left - printDocument1.DefaultPageSettings.Margins.Right;
            marginLeft = printDocument1.DefaultPageSettings.Margins.Left; // X coordinate
            marginTop = printDocument1.DefaultPageSettings.Margins.Top; // Y coordinate

            if (printDocument1.DefaultPageSettings.Landscape)//横打
            {
                int intTemp = 0;
                intTemp = intPrintAreaHeight;
                intPrintAreaHeight = intPrintAreaWidth;
                intPrintAreaWidth = intTemp;
            }

            //将容器调到最上面
            try
            {
                ((Panel)cContainer[iLoop]).AutoScrollPosition = new Point(0, 0);
            }
            catch { }

            if (CurrentPageHeight == 0) CurrentPageHeight = GetControlHeight(cContainer[iLoop]);

            //bool bDraw = false;
            int page = 0;
            if (addpage == 0)
                page = iPage + 1;//当前页加一，从一开始
            else
                page = addpage;

            //默认全部
            if ((this.printDocument1.PrinterSettings.FromPage == 0 && this.printDocument1.PrinterSettings.ToPage == 0) || this.printDocument1.PrinterSettings.FromPage < 0 || this.printDocument1.PrinterSettings.ToPage < 0)
            {
                g = e.Graphics;
                this.Draw(g, cContainer[iLoop], iPage);
            }
            else//当前指定页打印
            {

                if (page >= this.printDocument1.PrinterSettings.FromPage &&
                    page <= this.printDocument1.PrinterSettings.ToPage)
                {

                    //画窗体
                    g = e.Graphics;
                    this.Draw(g, cContainer[iLoop], iPage);

                    if (page + 1 > this.printDocument1.PrinterSettings.ToPage)
                    {
                        e.HasMorePages = false;
                        return;
                    }
                }
                else
                {
                    if (bHaveGrid == false)
                    {
                        iPage = iPage + 1;//如果不是自动特殊扩展控件
                    }

                    if (iLastPage == page) //上次打印等于这次打印
                    {
                        e.HasMorePages = false;
                        return;
                    }
                    else
                    {
                        iLastPage = page;
                    }

                    this.Draw(p.CreateGraphics(), cContainer[iLoop], iPage);
                    try
                    {
                        printDocument1_PrintPage(sender, e);
                        return;
                    }
                    catch { }
                }
            }

            if (CurrentPageHeight > this.PageHeight * (iPage + 1))//当前页太大 或 添加页大于零
            {
                iPage++;
                e.HasMorePages = true;
                return;
            }
            else if (addpage > 0)
            {
                e.HasMorePages = true;
                return;
            }
            else
            {
                iPage = 0;
            }

            iLoop++;

            if (iLoop < cContainer.GetUpperBound(0) + 1)
            {
                maxpage = 0;
                e.HasMorePages = true;
            }
            else //循环完成加载所有的页
            {

                e.HasMorePages = false;
                iLoop = 0;
            }

        }
        private int iLastPage = -1;
        #endregion

        #endregion

        #region 打印函数

        /// <summary>
        /// 重新设置页码
        /// </summary>
        public void ResetPage()
        {
            addpage = 0;
            maxpage = 0;

            offsetX = 0;
            offsetY = 0;

            this.CurrentPageHeight = 0;
            this.iLoop = 0;
            this.iPage = 0;//当前页
        }

        //    '------------------------------------------------------------------------
        //    '  不打印预览,直接打印
        //    '  Robin 2003-08-11
        //    '------------------------------------------------------------------------
        public int PrintPage(int iLeft, int iTop, params Control[] c)
        {
            cContainer = c;
            this.ResetPage();
            this.pBlankMargin.X = iLeft;
            this.pBlankMargin.Y = iTop;
            this.setpagesize(ref this.printDocument1);
            if (this.IsCanCancel == false)
                this.printDocument1.PrintController = new System.Drawing.Printing.StandardPrintController();

            try
            {
                printDocument1.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("打印机报错！" + ex.Message);
                return -1;
            }
            return 0;
        }


        /// <summary>
        /// 打印预览
        /// </summary>
        /// 		/// <param name="iLeft"></param>
        /// <param name="iTop"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public int PrintPreview(int iLeft, int iTop, params Control[] c)
        {
            cContainer = c;
            this.ResetPage();
            this.pBlankMargin.X = iLeft;
            this.pBlankMargin.Y = iTop;
            this.setpagesize(ref this.printDocument1);
            //this.printDocument1.BeginPrint+=new System.Drawing.Printing.PrintEventHandler(printDocument1_BeginPrint);
            //this.printDocument1.EndPrint+=new System.Drawing.Printing.PrintEventHandler(printDocument1_EndPrint);
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();

            printPreviewDialog.Document = this.printDocument1;

            //PrintPreviewControl previewControl = new PrintPreviewControl();
            //previewControl.Document = this.printDocument1;

            ////{71D960EF-3DFA-452e-AC54-3D497E666070} 防止预览后打印addpage未清空，指定打印页数无效 20100517 yangw
            printPreviewDialog.Layout += new LayoutEventHandler(printPreviewDialog_Layout);

            try
            {
                ((Form)printPreviewDialog).WindowState = FormWindowState.Maximized;
            }
            catch { }
            try
            {
                printPreviewDialog.ShowDialog();
                printPreviewDialog.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("打印机报错！" + ex.Message);
                return -1;
            }
            return 0;
        }


        /// <summary>
        /// 打印预览
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public int PrintPreview(Control c)
        {
            int iLeft = this.printDocument1.DefaultPageSettings.Margins.Left;
            int iTop = this.printDocument1.DefaultPageSettings.Margins.Top;
            int iRight = this.printDocument1.DefaultPageSettings.Margins.Right;
            int iBottom = this.printDocument1.DefaultPageSettings.Margins.Bottom;
            return this.PrintPreview(iLeft, iTop, c);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iLeft"></param>
        /// <param name="iTop"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public int PrintPreview(int iLeft, int iTop, Control c)
        {
            Control[] myControl = new Control[1];
            myControl[0] = c;
            return this.PrintPreview(iLeft, iTop, myControl);
        }

        protected void printPreviewDialog_Layout(object sender, LayoutEventArgs e)
        {//{71D960EF-3DFA-452e-AC54-3D497E666070} 防止预览后打印addpage未清空，指定打印页数无效 20100517 yangw
            if (this.printDocument1 == null)
            {
                return;
            }
            if (this.printDocument1.PrinterSettings.FromPage != 0 && this.printDocument1.PrinterSettings.ToPage != 0)
            {
                this.ResetPage();
            }
        }


        /// <summary>
        /// 设置打印纸张
        /// </summary>
        public void ShowPageSetup()
        {
            try
            {
                PageSetupDialog psd = new PageSetupDialog();
                psd.Document = this.printDocument1;

                if (psd.ShowDialog() == DialogResult.OK)
                {
                    this.printDocument1 = psd.Document;
                    this.iPageHeight = this.printDocument1.DefaultPageSettings.PaperSize.Height;
                }
                else
                {
                    //this.IsSetPageSetup = false;
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        /// <summary>
        /// 设置打印页
        /// </summary>
        public void ShowPrintPageDialog()
        {
            PrintDialog psd = new PrintDialog();
            psd.Document = this.printDocument1;
            psd.AllowSelection = true;
            psd.AllowSomePages = true;
            if (psd.ShowDialog() == DialogResult.OK)
            {
                this.printDocument1.PrinterSettings = psd.Document.PrinterSettings;

            }
        }


        /// <summary>
        /// 设置纸张大小
        /// </summary>
        /// <param name="pagesize"></param>
        public void SetPageSize(System.Drawing.Printing.PaperSize pagesize)
        {
            this.pageSize = pagesize;
            this.iPageHeight = pagesize.Height;
        }


        /// <summary>
        /// 设置纸张
        /// </summary>
        /// <param name="pagesize"></param>
        public void SetPageSize(System.Drawing.Printing.PaperKind pagesize)
        {
            System.Drawing.Printing.PaperSize c = new System.Drawing.Printing.PaperSize(pagesize.ToString(), 0, 0);
            this.pageSize = c;
            this.iPageHeight = c.Height;
        }


        /// <summary>
        /// 设置纸张
        /// </summary>
        /// <param name="pagesize"></param>
        public void SetPageSize(string printer, string pageName, int width, int height, int left, int top)
        {

            System.Drawing.Printing.PaperSize c = new System.Drawing.Printing.PaperSize(pageName, width, height);
            this.pageSize = c;
            this.iPageHeight = c.Height;
            this.printDocument1.DefaultPageSettings.Margins.Left = left;
            this.printDocument1.DefaultPageSettings.Margins.Top = top;

            if (printer.Trim() != "")
            {
                this.printDocument1.PrinterSettings.PrinterName = printer;
            }
        }




        #endregion

        #region GDI+
        /// <summary>
        /// 画图像
        /// </summary>
        /// <param name="g"></param>
        /// <param name="container"></param>
        public virtual void DrawGraphic(Graphics g, Control container)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            //将容器调到最上面
            try
            {
                ((Panel)container).AutoScrollPosition = new Point(0, 0);
            }
            catch { }

            CurrentPageHeight = GetControlHeight(container);

            //bool bDraw = false;
            int page = 0;
            if (addpage == 0)
                page = iPage + 1;//当前页加一，从一开始
            else
                page = addpage;

            this.Draw(g, container, iPage);

        }

        public virtual void DrawGraphic(Graphics g, Control container, int curPage)
        {

            //将容器调到最上面
            try
            {
                ((Panel)container).AutoScrollPosition = new Point(0, 0);
            }
            catch { }

            CurrentPageHeight = GetControlHeight(container);

            if (curPage > 0)
                this.addpage = curPage + 1;

            this.Draw(g, container, 0);

        }

        /// <summary>
        /// 保存为bmp文件
        /// </summary>
        /// <param name="container"></param>
        /// <param name="fileName"></param>
        public virtual void SaveAsFile(Control container, string fileName, int width, int height)
        {

            if (width <= 0)
            {
                width = this.pageSize.Width;
            }
            if (height <= 0)
            {
                height = this.GetControlHeight(container);
            }
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            this.DrawGraphic(g, container);
            bmp.Save(fileName);


        }
        /// <summary>
        /// 保存bmp文件
        /// </summary>
        /// <param name="container"></param>
        /// <param name="fileName"></param>
        public virtual void SaveAsFile(Control container, string fileName)
        {
            this.SaveAsFile(container, fileName, 0, 0);
        }

        public virtual void SaveAsFile(Control container, string fileName, int curPage)
        {
            int width = this.pageSize.Width;
            int height = this.GetControlHeight(container);

            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            this.DrawGraphic(g, container, curPage);
            bmp.Save(fileName);
        }
        #endregion

        #region 静态处理打印任务
        /// <summary>
        /// 清除打印作业
        /// </summary>
        /// <param name="JobNum"></param>
        /// <returns></returns>
        [DllImport("Select.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ClearPrintJob(int JobNum);

        /// <summary>
        /// 取消打印机暂停
        /// </summary>
        /// <param name="JobNum"></param>
        /// <returns></returns>
        [DllImport("Select.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ResumePrintJob(int JobNum);

        /// <summary>
        /// 暂停打印
        /// </summary>
        /// <param name="JobNum"></param>
        /// <returns></returns>
        [DllImport("Select.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool PausePrintJob(int JobNum);
        #endregion

        #region 续打否
        private bool bIsContinuePrint = false;
        /// <summary>
        /// 是否续打
        /// </summary>
        public bool IsContinuePrint
        {
            get
            {
                return this.bIsContinuePrint;
            }
            set
            {
                this.bIsContinuePrint = value;
            }
        }

        #endregion

        #region 调整RichTextBox行距

        public const int WM_USER = 0x0400;
        public const int EM_GETPARAFORMAT = WM_USER + 61;
        public const int EM_SETPARAFORMAT = WM_USER + 71;
        public const long MAX_TAB_STOPS = 32;
        public const uint PFM_LINESPACING = 0x00000100;
        [StructLayout(LayoutKind.Sequential)]
        private struct PARAFORMAT2
        {
            public int cbSize;
            public uint dwMask;
            public short wNumbering;
            public short wReserved;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public short wAlignment;
            public short cTabCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[] rgxTabs;
            public int dySpaceBefore;
            public int dySpaceAfter;
            public int dyLineSpacing;
            public short sStyle;
            public byte bLineSpacingRule;
            public byte bOutlineLevel;
            public short wShadingWeight;
            public short wShadingStyle;
            public short wNumberingStart;
            public short wNumberingStyle;
            public short wNumberingTab;
            public short wBorderSpace;
            public short wBorderWidth;
            public short wBorders;
        }
        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref PARAFORMAT2 lParam);
        //调整高度
        public void AdjustLineSpace(RichTextBox rc, double times)
        {
            rc.SelectAll();
            //double RowDist = double.Parse(this.comboBox1.Text);
            //RichTextBox行距为RowDist

            PARAFORMAT2 fmt = new PARAFORMAT2();
            fmt.cbSize = Marshal.SizeOf(fmt);
            fmt.bLineSpacingRule = 4; //4：固定高度
            fmt.dyLineSpacing = (int)(((int)rc.Font.Size) * 20 * times);
            fmt.dwMask = PFM_LINESPACING;
            SendMessage(new HandleRef(rc, rc.Handle), EM_SETPARAFORMAT, 0, ref fmt);

        }

        #endregion

        #region 打印 SparkLine 线条的处理
        private void DrawLine(Graphics g, Control c, int offsetX, int offsetY)
        {
            //设置高质量绘制，抗齿轮
            SparkControls.Controls.GDIHelper.InitializeGraphics(g);
            if (c.GetType().Name == "ShapeContainer")
            {
                //线条做特殊处理
                var pro = c.GetType().GetProperty("Shapes");
                var listLine = pro.GetValue(c, null) as System.Collections.IList;
                foreach (var line in listLine)
                {//遍历线条
                    //获取线的X1,Y1,X2,Y2
                    var type = line.GetType();
                    bool visible = (bool)type.GetProperty("Visible").GetValue(line, null);
                    if (!visible) continue;
                    int x1 = (int)type.GetProperty("X1").GetValue(line, null) + offsetX;
                    int y1 = (int)type.GetProperty("Y1").GetValue(line, null) + offsetY;
                    int x2 = (int)type.GetProperty("X2").GetValue(line, null) + offsetX;
                    int y2 = (int)type.GetProperty("Y2").GetValue(line, null) + offsetY;
                    int borderWidth = (int)type.GetProperty("BorderWidth").GetValue(line, null);
                    g.DrawLine(new Pen(Color.Black, borderWidth), x1, y1, x2, y2);
                }
            }
        }
        #endregion

        /// <summary>
        /// 以下控件跳过打印判断
        /// </summary>
        /// <param name="controlName"></param>
        /// <returns></returns>
        private bool ContinuePrint(string controlName)
        {
            if (controlName == "ShapeContainer" ||
                controlName == "SparkComboBox" ||
                controlName == "SparkDataGridComboBox" ||
                controlName == "SparkFontComboBox" ||
                controlName == "SparkMultiselectComboBox" ||
                controlName == "SparkMultiselectDataGridComboBox" ||
                controlName == "SparkDateTimePicker"
            )
            {
                return true;
            }
            return false;
        }
    }
}