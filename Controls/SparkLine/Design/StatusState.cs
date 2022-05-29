namespace SparkControls.Controls
{
    internal sealed class StatusState
    {
        private const int Checked = 4;
        private const int Enabled = 1;
        private int m_StatusFlags = 0x10;
        private const int NeedsUpdate = 0x10;
        private const int Supported = 8;
        private const int Visible = 2;

        internal void ApplyState(ShapeContainerMenuCommand cmd)
        {
            cmd.Enabled = (this.m_StatusFlags & 1) == 1;
            cmd.Visible = (this.m_StatusFlags & 2) == 2;
            cmd.Checked = (this.m_StatusFlags & 4) == 4;
            cmd.Supported = (this.m_StatusFlags & 8) == 8;
        }

        internal void Invalidate()
        {
            this.m_StatusFlags |= 0x10;
        }

        internal void SaveState(ShapeContainerMenuCommand cmd)
        {
            this.m_StatusFlags = 0;
            if (cmd.Enabled)
            {
                this.m_StatusFlags |= 1;
            }
            if (cmd.Visible)
            {
                this.m_StatusFlags |= 2;
            }
            if (cmd.Checked)
            {
                this.m_StatusFlags |= 4;
            }
            if (cmd.Supported)
            {
                this.m_StatusFlags |= 8;
            }
        }

        internal bool Valid
        {
            get
            {
                return ((this.m_StatusFlags & 0x10) != 0x10);
            }
        }
    }
}