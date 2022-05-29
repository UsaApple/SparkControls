using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Foundation;

namespace SparkControls.Controls
{
	/// <summary>
	/// 为控件提供【权限标识】扩展属性
	/// </summary>
	[Description("为控件提供权限标识的扩展属性容器")]
    [ProvideProperty("RightId", typeof(object))]
    [ToolboxItem(true)]
    public class SparkControlRightHolder : Component, IExtenderProvider
    {
        /// <summary>
        /// 存储所服务的对象及其权限标识
        /// </summary>
        readonly Dictionary<object, string> _dictionary;

        /// <summary>
        /// 获取或设置用于显示菜单项描述的控件
        /// </summary>
        [DefaultValue(null), Description("获取或设置用于显示菜单项描述的控件")]
        public Component Viewer { get; set; }

        /// <summary>
        /// 初始 <see cref="SparkControlRightHolder"/> 类型的新实例。
        /// </summary>
        public SparkControlRightHolder()
        {
            _dictionary = new Dictionary<object, string>();
        }

        /// <summary>
        /// 获取菜单项描述
        /// </summary>
        [Description("设置菜单项描述")] //虽然方法为Get，但在VS中显示为“设置”才符合理解
        [DefaultValue(null)]
        public string GetRightId(object item)
        {
            //从集合中取出该项的描述
            _dictionary.TryGetValue(item, out string value);
            return value;
        }

        /// <summary>
        /// 设置菜单项描述
        /// </summary>
        public void SetRightId(object item, string value)
        {
            if (item == null) { return; }

            //如果赋值为null或string.Empty，视为不再扩展该ToolStripItem
            if (value == null)
            {
                //从集合中移除该item，并取消其相关事件绑定
                _dictionary.Remove(item);
            }
            else
            {
                //添加或更改该item的描述
                _dictionary[item] = value;//这种写法对于dic中不存在的Key，会自动添加
            }
        }

        /// <summary>
        /// 是否可为某对象扩展属性
        /// </summary>
        public bool CanExtend(object extendee)
        {
            //在设计哪些控件可以设置此属性。代码中可以任意设置
            //后面可以在扩展

            return !(extendee is SparkControlRightHolder) && (
              extendee is Control ||
              extendee is ToolStripItem ||
              extendee is ToolStripLabel
            );
        }


        #region 设置权限
        /// <summary>
        /// 设置权限
        /// </summary>
        /// <param name="list"></param>
        public void SetRight(IEnumerable<SparkControlRight> list)
        {
            foreach (var item in _dictionary)
            {
                var right = list?.FirstOrDefault(a => a.RightId == item.Value);
                if (right != null)
                {
                    SetObject(item.Key, right.RightType);
                }
                else
                {//没有权限，默认设置为禁用
                    SetObject(item.Key, NoRightMode.Invisible);
                }
            }
        }

        /// <summary>
        /// 设置权限
        /// </summary>
        /// <param name="list"></param>
        /// <param name="notRightShowType">如果没有权限，默认控件的显示方式,现在默认为禁用</param>
        public void SetRightByMenu(IEnumerable<BaseObject> list, NoRightMode notRightShowType = NoRightMode.Disable)
        {
            foreach (var item in _dictionary)
            {
                if (string.IsNullOrEmpty(item.Value)) continue;
                var right = list?.FirstOrDefault(a => a.Id == item.Value);
                if (right != null)
                {
                    SetObject(item.Key, NoRightMode.Show);
                }
                else
                {
                    //没有权限，默认设置为notRightShowType
                    SetObject(item.Key, notRightShowType);
                }
            }
        }


        private void SetObject(object obj, NoRightMode type)
        {
            switch (type)
            {
                case NoRightMode.Show:
                    SetObjectVisible(obj, true);
                    break;
                case NoRightMode.Disable:
                    SetObjectEnabled(obj, false);
                    break;
                case NoRightMode.Invisible:
                    SetObjectVisible(obj, false);
                    break;
            }
        }

        /// <summary>
        /// 设置控件“Enabled”属性的值
        /// </summary>
        /// <param name="obj">要设置属性值的控件</param>
        /// <param name="value">要设置的属性值</param>
        private void SetObjectEnabled(object obj, bool value)
        {
            if (obj == null) return;
            var proInfo = obj.GetType().GetProperties().FirstOrDefault(a => a.Name == "Enabled");
            if (proInfo != null)
            {
                proInfo.SetValue(obj, value, null);
            }
        }


        /// <summary>
        /// 设置控件“Visible”属性的值
        /// </summary>
        /// <param name="obj">要设置属性值的控件</param>
        /// <param name="value">要设置的属性值</param>
        private void SetObjectVisible(object obj, bool value)
        {
            if (obj == null) return;
            var proInfo = obj.GetType().GetProperties().FirstOrDefault(a => a.Name == "Visible");
            if (proInfo != null)
            {
                proInfo.SetValue(obj, value, null);
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            _dictionary?.Clear();
            base.Dispose(disposing);
        }
    }
}