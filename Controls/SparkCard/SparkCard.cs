using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using SparkControls.Controls;
using static SparkControls.Controls.SparkCardControl;

namespace SparkControls.Controls
{
    public class SparkCard
    {
        public Size Size { get => SparkCardStyle.Size; set => SparkCardStyle.Size = value; }

        /// <summary>
        /// 获取或设置控件位置。
        /// </summary>
        public Point Location { get => SparkCardStyle.Location; set => SparkCardStyle.Location = value; }

        public Rectangle Rectangle => SparkCardStyle.Rectangle;

        /// <summary>
        /// 获取或设置面板是否处于激活状态。
        /// </summary>
        public bool IsActived { get => ((SparkCardTemplateStyle)SparkCardStyle).IsActived; set => ((SparkCardTemplateStyle)SparkCardStyle).IsActived = value; }

        public Color ActivedBackColor => ((SparkCardTemplateStyle)SparkCardStyle).ActivedBackColor;

        public IDrawStyle SparkCardStyle { get; private set; }

        /// <summary>
        /// 获取或设置行数据源。
        /// </summary>
        public DataRowView DataSource { get; }

        public SparkCardTemplate SparkCardTemplate { get; private set; }

        public void Paint(Graphics g)
        {
            SparkCardStyle.Paint(g);
        }

        public SparkCard(SparkCardTemplate trtCardControl, DataRowView dataRowView)
        {
            DataSource = dataRowView;
            this.SparkCardTemplate = trtCardControl;
            this.SparkCardStyle = trtCardControl.CreateStyle(dataRowView, null);
        }

        public void Reset(SparkCardTemplate trtCardControl)
        {
            var loc = Location;
            this.SparkCardTemplate = trtCardControl;
            this.SparkCardStyle = trtCardControl.CreateStyle(DataSource, null);
            this.Location = loc;
        }

        public void ResetSize()
        {
            this.Size = this.SparkCardTemplate.Size;
        }
    }



}