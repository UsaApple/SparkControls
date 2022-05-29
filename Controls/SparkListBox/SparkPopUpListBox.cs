using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

using SparkControls.Foundation;

namespace SparkControls.Controls
{
	/// <summary>
	/// 弹出列表框。
	/// </summary>
	public class SparkPopUpListBox : SparkListBox
	{
		/// <summary> 
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// 初始 <see cref="SparkPopUpListBox"/> 类型的新实例。
		/// </summary>
		public SparkPopUpListBox()
		{
			// 该调用是 Windows.Forms 窗体设计器所必需的。
			this.InitializeComponent();

			// TODO: 在 InitializeComponent 调用后添加任何初始化
			this.Init();
			base.KeyDown += new KeyEventHandler(this.ListBox_KeyDown);
			base.Click += new EventHandler(this.ListBox_Click);
		}

		/// <summary> 
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.components != null)
				{
					this.components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region 组件设计器生成的代码
		/// <summary> 
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
		}
		#endregion

		private readonly DataSet dsItems = new DataSet();
		private int Spell = 0;

		/// <summary>
		/// 设置输入法
		/// </summary>
		public int InputCode
		{
			get => this.Spell;
			set
			{
				this.Spell = value;
				if (this.Spell > 3 || this.Spell < 0) this.Spell = 0;
			}
		}
		/// <summary>
		/// 是否模糊查询
		/// </summary>
		public bool OmitFilter { get; set; } = true;
		/// <summary>
		/// 所有的数据，原来的alItems
		/// </summary>
		public new ArrayList Items { get; set; } = new ArrayList();
		/// <summary>
		/// 当输入空字符串时 ,是否不选中任何项
		/// </summary>
		public bool SelectNone { get; set; } = false;
		/// <summary>
		/// 是否显示代码
		/// </summary>
		public bool IsShowID { get; set; } = true;

		public delegate int MyDelegate(Keys key);
		public event MyDelegate SelectItem;
		public new event EventHandler SelectedItem;

		/// <summary>
		/// 初始化
		/// </summary>
		/// <returns></returns>
		private int Init()
		{
			this.dsItems.Tables.Add("items");
			this.dsItems.Tables["items"].Columns.AddRange(new DataColumn[]
				{
					new DataColumn("ID",Type.GetType("System.String")),//ID
					new DataColumn("Name",Type.GetType("System.String")),//名称
					new DataColumn("spell_code",Type.GetType("System.String")),//拼音码
					new DataColumn("input_code",Type.GetType("System.String")),//输入码
					new DataColumn("wb_code",Type.GetType("System.String"))//五笔码
				});
			this.dsItems.CaseSensitive = false;
			return 1;
		}
		/// <summary>
		/// 添加项目列表
		/// </summary>
		/// <param name="Items"></param>
		/// <returns></returns>
		public int AddItems(ArrayList Items)
		{
			base.Items.Clear();

			this.Items = Items;
			this.dsItems.Tables["items"].Rows.Clear();
			BaseObject objItem;
			try
			{
				for (int i = 0; i < this.Items.Count; i++)
				{
					objItem = (BaseObject)this.Items[i];
					this.dsItems.Tables["items"].Rows.Add(new object[]{
						objItem.Id, objItem.Name, objItem.PYCode, objItem.InputCode, objItem.WBCode
					});
				}
			}
			catch (Exception error)
			{
				MessageBox.Show("添加项目列表出错!" + error.Message, "ListBox");
				return -1;
			}
			return 1;
		}
		/// <summary>
		/// 过滤项目
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		public int Filter(string where)
		{
			DataView _dv = new DataView(this.dsItems.Tables["items"]);
			if (this.Spell == 0)
			{
				if (this.OmitFilter)
				{
					_dv.RowFilter = "(ID like '%" + where + "%') or (Name like '%" + where + "%') or (spell_code like '%" + where + "%')";
				}
				else
				{
					_dv.RowFilter = "(ID like '" + where + "%') or (Name like '" + where + "%') or (spell_code like '" + where + "%')";
				}
			}
			else if (this.Spell == 1)
			{
				if (this.OmitFilter)
				{
					_dv.RowFilter = "(ID like '%" + where + "%') or (Name like '%" + where + "%') or (input_code like '%" + where + "%')";
				}
				else
				{
					_dv.RowFilter = "(ID like '" + where + "%') or (Name like '" + where + "%') or (input_code like '" + where + "%')";
				}
			}
			else if (this.Spell == 2)
			{
				if (this.OmitFilter)
				{
					_dv.RowFilter = "(ID like '%" + where + "%') or (Name like '%" + where + "%') or (wb_code like '%" + where + "%')";
				}
				else
				{
					_dv.RowFilter = "(ID like '" + where + "%') or (Name like '" + where + "%') or (wb_code like '" + where + "%')";
				}
			}
			else
			{
				if (this.OmitFilter)
				{
					_dv.RowFilter = "(ID like '%" + where + "%') or (Name like '%" + where + "%') or (wb_code like '%" + where + "%') or (input_code like '%" + where + "%') or (spell_code like '%" + where + "%')";
				}
				else
				{
					_dv.RowFilter = "(ID like '" + where + "%') or (Name like '" + where + "%') or (wb_code like '" + where + "%') or (input_code like '" + where + "%') or (spell_code like '" + where + "%')";
				}
			}

			base.Items.Clear();
			for (int i = 0; i < _dv.Count; i++)
			{
				DataRowView _row = _dv[i];
				base.Items.Add(_row["ID"].ToString() + ". " + _row["Name"].ToString());
			}
			if (base.Items.Count > 0)
				base.SelectedIndex = 0;

			return 1;
		}
		/// <summary>
		/// 移动下一行
		/// </summary>
		/// <returns></returns>
		public int NextRow()
		{
			int index = base.SelectedIndex;
			if (index >= base.Items.Count - 1) return 1;

			base.SelectedIndex = index + 1;
			return 1;
		}
		/// <summary>
		/// 移动上一行
		/// </summary>
		/// <returns></returns>
		public int PriorRow()
		{
			int index = base.SelectedIndex;
			if (index <= 0) return 1;

			base.SelectedIndex = index - 1;
			return 1;
		}
		/// <summary>
		/// 切换输入法
		/// </summary>
		/// <returns></returns>
		public int SetInputMode()
		{
			this.Spell++;
			if (this.Spell > 2) this.Spell = 0;
			return 1;
		}
		/// <summary>
		/// 获得选中项
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int GetSelectedItem(out BaseObject item)
		{
			int index = base.SelectedIndex;
			if (index < 0 || index > base.Items.Count - 1)
			{
				item = new BaseObject();
				return -1;
			}

			//获得ID
			string itemname = base.SelectedItem.ToString();
			string ID = itemname.Substring(0, itemname.IndexOf(". ", 0));
			for (int i = 0; i < this.Items.Count; i++)
			{
				BaseObject obj = (BaseObject)this.Items[i];
				if (obj.Id == ID)
				{
					item = obj.Clone();
					return 1;
				}
			}
			item = new BaseObject();
			return -1;
		}

		private void ListBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				if (SelectItem != null)
					SelectItem(Keys.Enter);
				if (SelectedItem != null)
					SelectedItem(this.GetSelectedItem(), e);
			}
		}

		private void ListBox_Click(object sender, EventArgs e)
		{
			if (SelectItem != null)
				SelectItem(Keys.Enter);
		}

		#region IFpInputable 成员
		/// <summary>
		/// 下移
		/// </summary>
		public void MoveNext()
		{
			// TODO:  添加 PopUpListBox.MoveNext 实现
			this.NextRow();
		}

		/// <summary>
		/// 上移
		/// </summary>
		public void MovePrevious()
		{
			// TODO:  添加 PopUpListBox.MovePrevious 实现
			this.PriorRow();
		}

		/// <summary>
		/// 下页
		/// </summary>
		public void NextPage()
		{
			// TODO:  添加 PopUpListBox.NextPage 实现

		}

		/// <summary>
		/// 上页
		/// </summary>
		public void PreviousPage()
		{
			// TODO:  添加 PopUpListBox.PreviousPage 实现
		}

		/// <summary>
		/// 获得当前行
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public object GetRow(int row)
		{
			// TODO:  添加 PopUpListBox.GetRow 实现
			if (row >= this.Items.Count)
				return null;
			else
				return this.Items[row];
		}

		/// <summary>
		/// 变化输入法
		/// </summary>
		public void ChangeInput()
		{
			// TODO:  添加 PopUpListBox.ChangeInput 实现
			this.SetInputMode();
		}

		/// <summary>
		/// 获得项目
		/// </summary>
		public object GetSelectedItem()
		{
			// TODO:  添加 PopUpListBox.GetSelectedItem 实现
			this.GetSelectedItem(out BaseObject obj);
			return obj;
		}
		#endregion
	}
}