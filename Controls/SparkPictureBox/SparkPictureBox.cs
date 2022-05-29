using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	[ToolboxBitmap(typeof(PictureBox)), Description("图片")]
	public class SparkPictureBox : PictureBox, IDataBinding
	{
		#region IDataBinding 接口成员

		/// <summary>
		/// 获取或设置控件绑定的字段名。
		/// </summary>
		[Category("Spark"), Description("控件绑定的字段名。")]
		[DefaultValue(null)]
		public virtual string FieldName { get; set; } = null;

		/// <summary>
		/// 获取或设置控件的值。
		/// </summary>
		[Browsable(false)]
		[DefaultValue(null)]
		public virtual object Value
		{
			get => this.Image;
			set
			{
				if (value == null)
				{
					this.Image = null;
				}
				else if (value is Image image)
				{
					this.Image = image;
				}
				else if (value is byte[] bytes)
				{
					try
					{
						MemoryStream stream = new MemoryStream(bytes);
						this.Image = Image.FromStream(stream);
					}
					catch (Exception e)
					{
						throw new Exception("无效的值。", e);
					}
				}
				else if (value is string base64)
				{
					try
					{
						base64 = base64.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");
						byte[] imageBytes = Convert.FromBase64String(base64);
						using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
						{
							ms.Write(imageBytes, 0, imageBytes.Length);
							this.Image = Image.FromStream(ms, true);
						}
					}
					catch (Exception e)
					{
						throw new Exception("无效的值。", e);
					}
				}
			}
		}

		#endregion
	}
}