using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparkControls.Controls
{
    [Serializable]
    public class DataGridViewColumnStyle : ICloneable
    {
        #region 字段
        private string _headerText = "";
        int _width = 50;
        string _dataPropertyName = "";
        private System.Windows.Forms.DataGridViewContentAlignment _cellAlignment = System.Windows.Forms.DataGridViewContentAlignment.NotSet;
        private bool _hide = false;
        private bool _colFrozen = false;
        private int _sort = 0;
        #endregion

        #region 属性
        /// <summary>
        /// 数据是否改变
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public bool IsModify { get; set; }

        /// <summary>
        /// 列标题
        /// </summary>
        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                if (_headerText != value)
                {
                    _headerText = value;
                    IsModify = true;
                }

            }
        }

        /// <summary>
        /// 列宽
        /// </summary>
        public int Width
        {
            get { return _width; }
            set
            {
                if (this._width != value)
                {
                    if (Width < 0)
                        _width = 50;
                    else
                        _width = value;
                    this.IsModify = true;
                }
            }
        }
        /// <summary>
        /// 绑定的数据列名
        /// </summary>
        public string DataPropertyName
        {
            get { return _dataPropertyName; }
            set
            {
                if (_dataPropertyName != value)
                {
                    _dataPropertyName = value;
                    this.IsModify = true;
                }
            }
        }

        /// <summary>
        /// 列单元格对齐方式
        /// </summary>
        public System.Windows.Forms.DataGridViewContentAlignment CellAlignment
        {
            get { return _cellAlignment; }
            set
            {
                if (_cellAlignment != value)
                {
                    _cellAlignment = value;
                    this.IsModify = true;
                }
            }
        }

        /// <summary>
        /// 列隐藏
        /// </summary>
        public bool Hide
        {
            get { return _hide; }
            set
            {
                if (_hide != value)
                {
                    _hide = value;
                    this.IsModify = true;
                }
            }
        }

        /// <summary>
        /// 列冻结
        /// </summary>
        public bool ColFrozen
        {
            get { return _colFrozen; }
            set
            {
                if (this._colFrozen != value)
                {
                    _colFrozen = value;
                    this.IsModify = true;
                }
            }
        }

        /// <summary>
        /// 列的顺序
        /// </summary>
        public int Sort
        {
            get { return _sort; }
            set
            {
                if (this._sort != value)
                {
                    _sort = value;
                    this.IsModify = true;
                }
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public string CellAlignmentString
        {
            get
            {
                return CellAlignment.ToString();
            }
            set
            {
                CellAlignment = value.ToEnum<System.Windows.Forms.DataGridViewContentAlignment>(System.Windows.Forms.DataGridViewContentAlignment.NotSet);
            }
        }
        #endregion

        public object Clone()
        {
            DataGridViewColumnStyle instance = new DataGridViewColumnStyle()
            {
                CellAlignment = this.CellAlignment,
                ColFrozen = this.ColFrozen,
                DataPropertyName = this.DataPropertyName,
                HeaderText = this.HeaderText,
                Hide = this.Hide,
                Width = this.Width,
                Sort = this.Sort,
            };
            return instance;
        }

        internal void SetStyle(DataGridViewColumnStyle style)
        {
            this.CellAlignment = style.CellAlignment;
            this.ColFrozen = style.ColFrozen;
            this.DataPropertyName = style.DataPropertyName;
            this.HeaderText = style.HeaderText;
            this.Hide = style.Hide;
            this.Width = style.Width;
            this.Sort = style.Sort;
        }

    }

}
