using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Security.Permissions;

namespace SparkControls.Controls
{
    internal class ShapeContainerMenuCommand : MenuCommand
    {
        private bool m_ImmediateStatus;
        private bool m_NeedUpdate;
        private static Dictionary<EventHandler, StatusState> m_StatusCache;
        private EventHandler m_StatusHandler;
        private bool m_UpdatingCommand;

        public ShapeContainerMenuCommand(EventHandler statusHandler, EventHandler invokeHandler, CommandID id, bool immediateStatus) : base(invokeHandler, id)
        {
            this.m_NeedUpdate = true;
            this.m_StatusHandler = statusHandler;
            this.m_ImmediateStatus = immediateStatus;
            if (!this.m_ImmediateStatus)
            {
                if (m_StatusCache == null)
                {
                    m_StatusCache = new Dictionary<EventHandler, StatusState>();
                }
                if (!m_StatusCache.ContainsKey(this.m_StatusHandler))
                {
                    m_StatusCache.Add(this.m_StatusHandler, new StatusState());
                }
            }
        }

        private void ApplyCachedStatus()
        {
            if (m_StatusCache.ContainsKey(this.m_StatusHandler))
            {
                try
                {
                    this.m_UpdatingCommand = true;
                    m_StatusCache[this.m_StatusHandler].ApplyState(this);
                }
                finally
                {
                    this.m_UpdatingCommand = false;
                }
            }
        }

        protected override void OnCommandChanged(EventArgs e)
        {
            if (!this.m_UpdatingCommand)
            {
                base.OnCommandChanged(e);
            }
        }

        internal void OnSelectionChanged()
        {
            StatusState statusState = this.StatusState;
            if (statusState != null)
            {
                statusState.Invalidate();
            }
            this.m_NeedUpdate = true;
        }

        private void SaveCommandStatus()
        {
            StatusState statusState = this.StatusState;
            if (statusState != null)
            {
                statusState.SaveState(this);
            }
        }

        public void UpdateStatus()
        {
            if (this.m_StatusHandler != null)
            {
                if (this.m_ImmediateStatus)
                {
                    try
                    {
                        this.m_StatusHandler(this, EventArgs.Empty);
                    }
                    catch (Exception exception1)
                    {
                        //ProjectData.SetProjectError(exception1);
                        Exception exception = exception1;
                        //ProjectData.ClearProjectError();
                    }
                }
                else
                {
                    this.m_NeedUpdate = false;
                    if (!this.CommandStatusValid)
                    {
                        try
                        {
                            this.m_StatusHandler(this, EventArgs.Empty);
                            this.SaveCommandStatus();
                        }
                        catch (Exception exception3)
                        {
                            //ProjectData.SetProjectError(exception3);
                            Exception exception2 = exception3;
                            //ProjectData.ClearProjectError();
                        }
                    }
                    else
                    {
                        this.ApplyCachedStatus();
                    }
                }
            }
        }

        private bool CommandStatusValid
        {
            get
            {
                StatusState statusState = this.StatusState;
                return ((statusState != null) && statusState.Valid);
            }
        }

        public override int OleStatus
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.AllFlags)]
            get
            {
                if (this.m_ImmediateStatus || this.m_NeedUpdate)
                {
                    this.UpdateStatus();
                }
                return base.OleStatus;
            }
        }

        private StatusState StatusState
        {
            get
            {
                if ((this.m_StatusHandler != null) && m_StatusCache.ContainsKey(this.m_StatusHandler))
                {
                    return m_StatusCache[this.m_StatusHandler];
                }
                return null;
            }
        }
    }
}