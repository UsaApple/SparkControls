using System;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    partial class SparkTabStripLayoutEngine
    {
       
        private sealed class TabStripLayoutPass : IDisposable
        {
            private readonly SparkTabStripLayoutEngine _engine;
            private readonly TabStripScrollDirection _direction;

            private Rectangle _displayRectangle;
            private Point _currentPosition;

            private int _tabWidth;
            private int _tabsToShow;

            public TabStripLayoutPass(SparkTabStripLayoutEngine engine)
            {
                _engine = engine;
                _direction = engine.ScrollDirection;

                engine.ScrollDirection = TabStripScrollDirection.None;

                if (_engine._farTab >= TabCount)
                {
                    _engine._farTab = TabCount - 1;
                }

                _tabsToShow = engine.AvailableWidth / GetTabWidth(TabCount);
                _tabWidth = GetTabWidth(_tabsToShow);

                _displayRectangle = engine.DisplayRectangle;
                _currentPosition = _displayRectangle.Location;

                if (engine.RTL)
                {
                    _currentPosition = new Point(_displayRectangle.Right, _displayRectangle.Top);
                }
            }

            public void DoLayout()
            {
                if (_direction == TabStripScrollDirection.Left)
                {
                    ScrollNear();
                }

                if (_direction == TabStripScrollDirection.Right)
                {
                    ScrollFar();
                }

                if (_direction == TabStripScrollDirection.None)
                {
                    UpdateLayout();
                }
            }

            public void Dispose()
            {
                Commit();
            }

            private int NearTabIndex
            {
                get { return _engine._nearTab; }
                set
                {
                    if (value < 0)
                        throw new InvalidOperationException();

                    _engine._nearTab = value;
                }
            }

            private int FarTabIndex
            {
                get { return _engine._farTab; }
                set
                {
                    if (value >= TabCount)
                        throw new InvalidOperationException();

                    _engine._farTab = value;
                }
            }

            private int SelectedTabIndex
            {
                get { return _engine.SelectedTabIndex; }
                set { _engine.SelectedTabIndex = value; }
            }

            private int VisibleTabCount
            {
                get { return (FarTabIndex - NearTabIndex) + 1; }
            }

            private Size TabSize
            {
                get { return new Size(_tabWidth, _displayRectangle.Height); }
            }

            private int TabCount
            {
                get { return _engine.TabCount; }
            }

            private int GetTabWidth(int count)
            {
                if (count > 0)
                {
                    int width = _engine.AvailableWidth / count;

                    if (width < _engine.MinimumTabWidth)
                    {
                        return _engine.MinimumTabWidth;
                    }

                    return Math.Min(_engine.MaximumTabWidth, width);
                }

                return _engine.MaximumTabWidth;
            }

            private bool TryFitAllTabs()
            {
                if (_tabsToShow >= TabCount)
                {
                    NearTabIndex = 0;
                    FarTabIndex = TabCount - 1;

                    return true;
                }

                return false;
            }

            private void ScrollNear()
            {
                if (!TryFitAllTabs())
                {
                    NearTabIndex = Math.Max(0, SelectedTabIndex - _tabsToShow);
                    FarTabIndex = NearTabIndex + (_tabsToShow - 1);
                    SelectedTabIndex = NearTabIndex;
                }
            }

            private void ScrollFar()
            {
                if (!TryFitAllTabs())
                {
                    FarTabIndex = Math.Min(TabCount - 1, SelectedTabIndex + _tabsToShow);

                    if (NearTabIndex == 0)
                    {
                        NearTabIndex = SelectedTabIndex + 1;
                    }

                    NearTabIndex = FarTabIndex - (_tabsToShow - 1);
                    SelectedTabIndex = FarTabIndex;
                }
            }

            private void UpdateLayout()
            {
                //_engine.ContainsTabListButton = (TabCount > 1);

                if (TryFitAllTabs())
                {
                    return;
                }

                if (VisibleTabCount <= _tabsToShow)
                {
                    int freeTabs = _tabsToShow - VisibleTabCount;
                    int freeTabsLeft = Math.Min(NearTabIndex, freeTabs);

                    NearTabIndex -= freeTabsLeft;
                    freeTabs -= freeTabsLeft;

                    if (freeTabs > 0)
                    {
                        FarTabIndex = Math.Min(TabCount - 1, FarTabIndex + freeTabs);
                    }

                    if (SelectedTabIndex < NearTabIndex)
                    {
                        NearTabIndex = SelectedTabIndex;
                        FarTabIndex = SelectedTabIndex + _tabsToShow - 1;
                    }
                    else if (SelectedTabIndex >= FarTabIndex)
                    {
                        while ((FarTabIndex < SelectedTabIndex))
                        {
                            ++FarTabIndex;
                        }

                        NearTabIndex = FarTabIndex - (_tabsToShow - 1);
                    }
                }
                else
                {
                    while ((VisibleTabCount >= _tabsToShow) &&
                             (FarTabIndex != SelectedTabIndex))
                    {
                        --FarTabIndex;
                    }

                    while ((VisibleTabCount >= _tabsToShow) &&
                              (NearTabIndex != SelectedTabIndex))
                    {
                        ++NearTabIndex;
                    }
                }
            }

            private void UpdateScrollButtons()
            {
                bool near = (NearTabIndex > 0);
                bool far = (TabCount - FarTabIndex > 1);

                _engine.ContainsScrollNearButton = near;
                _engine.ContainsScrollFarButton = near || far;
                _engine.ScrollFarButton.Enabled = far;

                _tabWidth = GetTabWidth(_tabsToShow);
                //_tabsToShow = _context.AvailableWidth / _tabWidth;
            }

            private void Commit()
            {
                UpdateScrollButtons();
                LayoutUnknownItems(ToolStripItemAlignment.Left);

                if (_engine.ContainsScrollNearButton)
                {
                    LayoutButton(_engine.ScrollNearButton);
                }

                int tabIndex = 0;

                foreach (var item in _engine.GetTabItems())
                {
                    item.Available = (tabIndex >= NearTabIndex) &&
                                     (tabIndex <= FarTabIndex);

                    if (item.Available)
                    {
                        LayoutItem(item, TabSize, true);
                    }

                    ++tabIndex;
                }

                if (_engine.ContainsScrollFarButton)
                {
                    LayoutButton(_engine.ScrollFarButton);
                }

                LayoutUnknownItems(ToolStripItemAlignment.Right);

                _engine.ClearVolatileState();
            }

            private void LayoutButton(ToolStripButton button)
            {
                Size size = button.GetPreferredSize(_displayRectangle.Size);
                size.Height = TabSize.Height;

                LayoutItem(button, size, true);
            }

            private void LayoutItem(ToolStripItem item, Size size, bool overlap)
            {
                var w = size.Width;

                if (overlap)
                {
                    w -= SparkTabStripLayoutEngine.TabOverlap;
                }

                if (_engine.RTL)
                {
                    _currentPosition.X -= w;
                    _engine.LayoutItem(item, _currentPosition, size);
                }
                else
                {
                    _engine.LayoutItem(item, _currentPosition, size);
                    _currentPosition.X += w;
                }
            }

            private void LayoutUnknownItems(ToolStripItemAlignment alignment)
            {
                foreach (var item in _engine.GetAvailableUnknownItems())
                {
                    if (item.Alignment == alignment)
                    {
                        LayoutItem(item, item.GetPreferredSize(_displayRectangle.Size), item is SparkNavigationBarStripButton);
                    }
                }
            }
        }
    }
}