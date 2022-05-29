using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

namespace SparkControls.Controls
{
    internal sealed class MenuCommandHandler : IMenuCommandService
    {
        private IMenuCommandService m_MenuCommandService;
        private ISelectionService m_SelectionService;

        public MenuCommandHandler(IMenuCommandService menuService, ISelectionService selectionService)
        {
            this.m_MenuCommandService = menuService;
            this.m_SelectionService = selectionService;
        }

        public void AddCommand(MenuCommand command)
        {
            this.m_MenuCommandService.AddCommand(command);
        }

        public void AddVerb(DesignerVerb verb)
        {
            this.m_MenuCommandService.AddVerb(verb);
        }

        public MenuCommand FindCommand(CommandID commandID)
        {
            return this.m_MenuCommandService.FindCommand(commandID);
        }

        public bool GlobalInvoke(CommandID commandID)
        {
            return this.m_MenuCommandService.GlobalInvoke(commandID);
        }

        public void RemoveCommand(MenuCommand command)
        {
            this.m_MenuCommandService.RemoveCommand(command);
        }

        public void RemoveVerb(DesignerVerb verb)
        {
            this.m_MenuCommandService.RemoveVerb(verb);
        }

        public void ShowContextMenu(CommandID menuID, int x, int y)
        {
            if ((this.m_SelectionService != null) & (this.m_SelectionService.SelectionCount > 0))
            {
                if (this.m_SelectionService.PrimarySelection != null)
                {
                    if (typeof(Shape).IsAssignableFrom(this.m_SelectionService.PrimarySelection.GetType()))
                    {
                        return;
                    }
                    if (typeof(ShapeContainer).IsAssignableFrom(this.m_SelectionService.PrimarySelection.GetType()))
                    {
                        return;
                    }
                }
                else
                {
                    IEnumerator enumerator = null;
                    try
                    {
                        enumerator = this.m_SelectionService.GetSelectedComponents().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                            if (typeof(Shape).IsAssignableFrom(objectValue.GetType()) || typeof(ShapeContainer).IsAssignableFrom(objectValue.GetType()))
                            {
                                return;
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
                }
            }
            this.m_MenuCommandService.ShowContextMenu(menuID, x, y);
        }

        public IMenuCommandService MenuService
        {
            get
            {
                return this.m_MenuCommandService;
            }
        }

        public DesignerVerbCollection Verbs
        {
            get
            {
                return this.m_MenuCommandService.Verbs;
            }
        }
    }
}