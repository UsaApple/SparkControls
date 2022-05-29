using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SparkControls.Controls
{
    internal sealed class PasteHandler : IDisposable
    {
        private bool m_DisposedValue;
        private IDesignerEventService m_EventService;
        private Dictionary<IDesignerHost, MenuCommand> m_HostStdCommandMap;
        private Dictionary<MenuCommand, IDesignerHost> m_NewCommandHostMap;

        public PasteHandler(IDesignerEventService eventSvc)
        {
            this.m_EventService = eventSvc;
            if (this.m_EventService != null)
            {
                this.m_EventService.SelectionChanged += new EventHandler(this.OnSelectionChanged);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.m_DisposedValue && disposing)
            {
                if (this.m_NewCommandHostMap != null)
                {
                    this.m_NewCommandHostMap.Clear();
                    this.m_NewCommandHostMap = null;
                }
                if (this.m_HostStdCommandMap != null)
                {
                    this.m_HostStdCommandMap.Clear();
                    this.m_HostStdCommandMap = null;
                }
                if (this.m_EventService != null)
                {
                    this.m_EventService.SelectionChanged -= new EventHandler(this.OnSelectionChanged);
                    this.m_EventService = null;
                }
            }
            this.m_DisposedValue = true;
        }

        private void OnActiveDesignerChanged(object sender, ActiveDesignerEventArgs e)
        {
            if (((e.NewDesigner != null) && !this.HostStdCommandMap.ContainsKey(e.NewDesigner)) && !SkipCacheDesignerHost(e.NewDesigner))
            {
                IMenuCommandService service = (IMenuCommandService)e.NewDesigner.GetService(typeof(IMenuCommandService));
                if (service != null)
                {
                    MenuCommand command2 = service.FindCommand(StandardCommands.Paste);
                    MenuCommand command = new ShapeContainerMenuCommand(new EventHandler(this.OnStatusPaste), new EventHandler(this.OnMenuPaste), StandardCommands.Paste, true);
                    service.RemoveCommand(command2);
                    service.AddCommand(command);
                    this.NewCommandHostMap.Add(command, e.NewDesigner);
                    this.HostStdCommandMap.Add(e.NewDesigner, command2);
                }
            }
        }

        public void OnDesignerDisposed(object sender, DesignerEventArgs e)
        {
            if ((e.Designer != null) && this.HostStdCommandMap.ContainsKey(e.Designer))
            {
                IMenuCommandService service = (IMenuCommandService)e.Designer.GetService(typeof(IMenuCommandService));
                if (service != null)
                {
                    MenuCommand command = service.FindCommand(StandardCommands.Paste);
                    service.RemoveCommand(command);
                    MenuCommand command2 = this.HostStdCommandMap[e.Designer];
                    service.AddCommand(command2);
                    if (this.NewCommandHostMap.ContainsKey(command))
                    {
                        this.NewCommandHostMap.Remove(command);
                    }
                }
                this.HostStdCommandMap.Remove(e.Designer);
            }
        }

        private void OnMenuPaste(object sender, EventArgs e)
        {
            if ((sender != null) && typeof(MenuCommand).IsAssignableFrom(sender.GetType()))
            {
                MenuCommand command = (MenuCommand)sender;
                IDesignerHost host = this.NewCommandHostMap[command];
                if (host != null)
                {
                    IDataObject dataObject = Clipboard.GetDataObject();
                    if (dataObject != null)
                    {
                        if (!dataObject.GetDataPresent("SHAPECONTAINER_DESIGNERCOMPONENTS"))
                        {
                            MenuCommand command2 = this.HostStdCommandMap[host];
                            if (command2 != null)
                            {
                                command2.Invoke();
                            }
                        }
                        else
                        {
                            IComponentChangeService cs = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
                            if (cs != null)
                            {
                                ISelectionService service = (ISelectionService)host.GetService(typeof(ISelectionService));
                                if (service != null)
                                {
                                    IDesignerSerializationService service2 = (IDesignerSerializationService)host.GetService(typeof(IDesignerSerializationService));
                                    if (service2 != null)
                                    {
                                        Cursor current = Cursor.Current;
                                        DesignerTransaction transaction = null;
                                        try
                                        {
                                            Cursor.Current = Cursors.WaitCursor;
                                            transaction = host.CreateTransaction("Paste components");
                                            object objectValue = RuntimeHelpers.GetObjectValue(dataObject.GetData("SHAPECONTAINER_DESIGNERCOMPONENTS"));
                                            ICollection is2 = null;
                                            byte[] buffer = (byte[])objectValue;
                                            if (buffer != null)
                                            {
                                                MemoryStream serializationStream = new MemoryStream(buffer);
                                                if (serializationStream != null)
                                                {
                                                    BinaryFormatter formatter = new BinaryFormatter();
                                                    serializationStream.Seek(0L, SeekOrigin.Begin);
                                                    object obj4 = RuntimeHelpers.GetObjectValue(formatter.Deserialize(serializationStream));
                                                    is2 = service2.Deserialize(RuntimeHelpers.GetObjectValue(obj4));
                                                    if ((is2 != null) && (is2.Count >= 2))
                                                    {
                                                        ShapeContainer shapecontainer = DesignerUtility.GetShapeContainer(host, service, cs);
                                                        if (shapecontainer != null)
                                                        {
                                                            IEnumerator enumerator = null;
                                                            List<Shape> shapeList = new List<Shape>();
                                                            try
                                                            {
                                                                enumerator = is2.GetEnumerator();
                                                                while (enumerator.MoveNext())
                                                                {
                                                                    object obj5 = RuntimeHelpers.GetObjectValue(enumerator.Current);
                                                                    if (typeof(Shape).IsAssignableFrom(obj5.GetType()))
                                                                    {
                                                                        shapeList.Add((Shape)obj5);
                                                                    }
                                                                }
                                                            }
                                                            finally
                                                            {
                                                                if (enumerator is IDisposable)
                                                                {
                                                                    (enumerator as IDisposable).Dispose();
                                                                }
                                                            }
                                                            DesignerUtility.PasteShapes(host, shapeList, shapecontainer, cs);
                                                            if (shapeList.Count > 0)
                                                            {
                                                                service.SetSelectedComponents(shapeList, SelectionTypes.Replace);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception exception1)
                                        {
                                            //ProjectData.SetProjectError(exception1);
                                            Exception exception = exception1;
                                            if (transaction != null)
                                            {
                                                transaction.Cancel();
                                                ((IDisposable)transaction).Dispose();
                                            }
                                            //ProjectData.ClearProjectError();
                                        }
                                        finally
                                        {
                                            if (transaction != null)
                                            {
                                                transaction.Commit();
                                                ((IDisposable)transaction).Dispose();
                                            }
                                            Cursor.Current = current;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if ((sender != null) && typeof(IDesignerEventService).IsAssignableFrom(sender.GetType()))
            {
                IDesignerHost activeDesigner = ((IDesignerEventService)sender).ActiveDesigner;
                if (activeDesigner != null)
                {
                    ISelectionService service = (ISelectionService)activeDesigner.GetService(typeof(ISelectionService));
                    if (service != null)
                    {
                        if (service.SelectionCount > 0)
                        {
                            this.OnActiveDesignerChanged(null, new ActiveDesignerEventArgs(null, activeDesigner));
                        }
                        else
                        {
                            this.OnDesignerDisposed(null, new DesignerEventArgs(activeDesigner));
                        }
                    }
                }
            }
        }

        private void OnStatusPaste(object sender, EventArgs e)
        {
            if ((sender != null) && typeof(MenuCommand).IsAssignableFrom(sender.GetType()))
            {
                MenuCommand key = (MenuCommand)sender;
                if (this.NewCommandHostMap.ContainsKey(key))
                {
                    IDesignerHost host = this.NewCommandHostMap[key];
                    if (host != null)
                    {
                        IComponent rootComponent = host.RootComponent;
                        if (rootComponent != null)
                        {
                            IDesigner designer = host.GetDesigner(rootComponent);
                            if ((designer != null) && typeof(ParentControlDesigner).IsAssignableFrom(designer.GetType()))
                            {
                                InheritanceAttribute attribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(rootComponent)[typeof(InheritanceAttribute)];
                                if (attribute.InheritanceLevel == InheritanceLevel.InheritedReadOnly)
                                {
                                    key.Enabled = false;
                                    return;
                                }
                            }
                        }
                    }
                    IDataObject dataObject = Clipboard.GetDataObject();
                    bool flag = false;
                    if (dataObject != null)
                    {
                        if (dataObject.GetDataPresent("CF_DESIGNERCOMPONENTS_V2"))
                        {
                            flag = true;
                            ISelectionService service = (ISelectionService)host.GetService(typeof(ISelectionService));
                            if (((service != null) && (service.PrimarySelection != null)) && typeof(ShapeContainer).IsAssignableFrom(service.PrimarySelection.GetType()))
                            {
                                flag = false;
                            }
                        }
                        else if (dataObject.GetDataPresent("SHAPECONTAINER_DESIGNERCOMPONENTS"))
                        {
                            flag = true;
                        }
                        else
                        {
                            IToolboxService service2 = (IToolboxService)host.GetService(typeof(IToolboxService));
                            if (service2 != null)
                            {
                                if (host == null)
                                {
                                    flag = service2.IsToolboxItem(dataObject);
                                }
                                else
                                {
                                    flag = service2.IsSupported(dataObject, host);
                                }
                            }
                        }
                    }
                    key.Enabled = flag;
                }
            }
        }

        private static bool SkipCacheDesignerHost(IDesignerHost host)
        {
            if (host.RootComponent != null)
            {
                object designer = host.GetDesigner(host.RootComponent);
                if ((designer != null) && typeof(ParentControlDesigner).IsAssignableFrom(designer.GetType()))
                {
                    return false;
                }
            }
            return true;
        }

        private Dictionary<IDesignerHost, MenuCommand> HostStdCommandMap
        {
            get
            {
                if (this.m_HostStdCommandMap == null)
                {
                    this.m_HostStdCommandMap = new Dictionary<IDesignerHost, MenuCommand>();
                }
                return this.m_HostStdCommandMap;
            }
        }

        private Dictionary<MenuCommand, IDesignerHost> NewCommandHostMap
        {
            get
            {
                if (this.m_NewCommandHostMap == null)
                {
                    this.m_NewCommandHostMap = new Dictionary<MenuCommand, IDesignerHost>();
                }
                return this.m_NewCommandHostMap;
            }
        }
    }
}