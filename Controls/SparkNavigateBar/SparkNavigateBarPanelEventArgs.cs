using System;

namespace SparkControls.Controls
{
    /// <summary>
    /// SparkNavigateBarPanel容器的事件
    /// </summary>
   
    public class SparkNavigateBarPanelEventArgs : EventArgs
    {
        private readonly SparkNavigateBarPanel _page;

        public SparkNavigateBarPanelEventArgs(SparkNavigateBarPanel page)
        {
            this._page = page;
        }

        public SparkNavigateBarPanel Page => this._page;
    }
}