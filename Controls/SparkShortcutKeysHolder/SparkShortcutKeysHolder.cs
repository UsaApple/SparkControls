using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 为控件提供快捷键【ShortcutKeys】扩展属性
	/// </summary>
	[Description("为控件提供快捷键【ShortcutKeys】扩展属性的容器")]
    [ProvideProperty("ShortcutKeys", typeof(object))]
    [ToolboxItem(true)]
    public class SparkShortcutKeysHolder : Component, IExtenderProvider
    {
        /// <summary>
        /// 存储所服务的对象及ShortcutKeys
        /// </summary>
        private readonly Dictionary<Keys, object> _dictionary = new Dictionary<Keys, object>();
        private Component viewer = null;

        /// <summary>
        /// 获取或设置用于显示菜单项描述的控件
        /// </summary>
        [DefaultValue(null), Description("获取或设置用于显示菜单项描述的控件")]
        public Component Viewer
        {
            get => viewer;
            set
            {
                if (viewer != value)
                {
                    RemoveEvent(viewer);
                    viewer = value;
                    AddEvent(viewer);
                }
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkShortcutKeysHolder()
        {

        }

        /// <summary>
        /// 获取菜单项描述
        /// </summary>
        [Description("设置菜单项描述")] //虽然方法为Get，但在VS中显示为“设置”才符合理解
        [DefaultValue(Keys.None)]
        public Keys GetShortcutKeys(object item)
        {
            //从集合中取出该item的描述
            if (_dictionary != null)
            {
                var child = _dictionary.FirstOrDefault(a => a.Value == item);
                if (!child.Equals(default(KeyValuePair<object, Keys>)))
                {
                    return child.Key;
                }
            }
            return Keys.None;
        }

        /// <summary>
        /// 设置菜单项描述
        /// </summary>
        public void SetShortcutKeys(object item, Keys key)
        {
            if (item == null) { return; }
            var objKey = _dictionary.FirstOrDefault(a => a.Value == item);
            if (!objKey.Equals(default(KeyValuePair<object, Keys>)))
            {
                _dictionary.Remove(objKey.Key);
                RemoveParentShortcutKeys(objKey.Key);
            }
            if (key != Keys.None)
            {
                //添加或更改该item的描述
                _dictionary[key] = item;//这种写法对于dic中不存在的Key，会自动添加
                AddParentShortcutKeys(item, key);
            }
        }

        /// <summary>
        /// 是否可为某对象扩展属性
        /// </summary>
        public bool CanExtend(object extendee)
        {
            //在设计哪些控件可以设置此属性。代码中可以任意设置
            //后面可以在扩展
            return !(extendee is SparkShortcutKeysHolder) && (
              extendee is ToolStripButton ||
              extendee is ToolStripLabel ||
              extendee is Button ||
              extendee is TextBox
              );
        }

        private void AddEvent(Component component)
        {
            if (this.DesignMode)
            {
                return;
            }
            void load(object sender1, EventArgs e1)
            {
                if (sender1 is Control interCtrl)
                {
                    var cmdKeyParent = GetParentIProcessCmdKey(interCtrl);
                    if (cmdKeyParent != null)
                    {
                        if (!cmdKeyParent.IsRegisteredProcessCmdKey)
                        {
                            cmdKeyParent.ProcessCmdKey += Ctrl_ProcessCmdKey;
                        }
                        AddParentAllShortcutKeys();
                    }
                }
            };
            if (component is UserControl uc)
            {
                uc.Load += load;
            }
            else if (component is Form frm)
            {
                frm.Load += load;
            }
            return;
            //自己是IProcessCmdKey，接口注册自己
            //自己不是IProcessCmdKey接口，到父窗口还是父控件
            if (component is UserControl ctrl)
            {
                if (component is IProcessCmdKey cmdKey)
                {
                    RemoveEvent(component);
                    cmdKey.ProcessCmdKey += Ctrl_ProcessCmdKey;
                }
                else
                {
                    void parentChanged(object sender, EventArgs e)
                    {
                        if (sender is Control interCtrl)
                        {
                            var cmdKeyParent = GetParentIProcessCmdKey(interCtrl);
                            if (cmdKeyParent != null)
                            {
                                if (!cmdKeyParent.IsRegisteredProcessCmdKey)
                                {
                                    cmdKeyParent.ProcessCmdKey += Ctrl_ProcessCmdKey;
                                }
                                AddParentAllShortcutKeys();
                            }
                        }
                    };
                    ctrl.ParentChanged += parentChanged;
                }
            }
        }

        private void RemoveEvent(Component component)
        {
            if (this.DesignMode)
            {
                return;
            }
            if (component is IProcessCmdKey cmdKey)
            {
                cmdKey.ProcessCmdKey -= Ctrl_ProcessCmdKey;
            }
        }

        private bool Ctrl_ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_dictionary == null) return true;
			_dictionary.TryGetValue(keyData, out object ctrl);
			if (ctrl == null)
            {
                if (Viewer is IProcessCmdKey cmdKey)
                {
                    ctrl = cmdKey.Shortcuts[keyData];
                }
                if (ctrl == null)
                {
                    var keyCmd = GetParentIProcessCmdKey();
                    if (keyCmd != null)
                    {
                        ctrl = keyCmd.Shortcuts[keyData];
                    }
                }
            }
            if (ctrl != null)
            {
                if (ctrl is ToolStripItem toolItem)
                {
                    toolItem.PerformClick();
                    return false;
                }
                else if (ctrl is Button btn)
                {
                    btn.PerformClick();
                    return false;
                }
                else if (ctrl is TextBox text)
                {
                    text.Focus();
                    return false;
                }
            }
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            RemoveAllParentShortcutKeys();
            _dictionary?.Clear();

            base.Dispose(disposing);
        }

        private void AddParentShortcutKeys(object item, Keys key)
        {
            if (this.DesignMode) return;
            var cmdKey = GetParentIProcessCmdKey();
            if (cmdKey != null)
            {
                cmdKey.Shortcuts[key] = item;
            }
        }

        private void AddParentAllShortcutKeys()
        {
            if (_dictionary != null && _dictionary.Any())
            {
                foreach (var item in _dictionary)
                {
                    AddParentShortcutKeys(item.Value, item.Key);
                }
            }
        }

        private IProcessCmdKey GetParentIProcessCmdKey(Control ctrl, bool isForm = false)
        {
            if (ctrl == null) return null;
            if (isForm)
            {
                var frm = ctrl.FindForm();
                if (frm is IProcessCmdKey cmdKey)
                {
                    return cmdKey;
                }
            }
            else
            {
                Control parent = ctrl.Parent;
                while (parent != null)
                {
                    if (parent is IProcessCmdKey cmdKey)
                    {
                        return cmdKey;
                    }
                    parent = parent.Parent;
                }
            }
            return null;
        }

        private IProcessCmdKey GetParentIProcessCmdKey()
        {
            if (Viewer == null) return null;
            if (Viewer is UserControl ctrl)
            {
                return GetParentIProcessCmdKey(ctrl);
            }
            return null;
        }

        private void RemoveParentShortcutKeys(Keys item)
        {
            if (this.DesignMode) return;
            var cmdKey = GetParentIProcessCmdKey();
            if (cmdKey != null)
            {
                if (cmdKey.Shortcuts.ContainsKey(item))
                {
                    cmdKey.Shortcuts.Remove(item);
                }
            }
        }

        private void RemoveAllParentShortcutKeys()
        {
            try
            {
                if (_dictionary != null && _dictionary.Any())
                {
                    foreach (var item in _dictionary)
                    {
                        RemoveParentShortcutKeys(item.Key);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}