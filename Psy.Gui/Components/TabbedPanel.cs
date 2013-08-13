using System;
using System.Collections.Generic;
using System.Linq;
using Psy.Gui.Events;
using SlimMath;

namespace Psy.Gui.Components
{

    public sealed class TabbedPanel : Widget
    {
        private const int ButtonHeight = 20;
        private const int ButtonWidth = 100;

        private class TabbedPanelTab
        {
            public readonly string TabName;
            public readonly Tab Tab;
            public readonly ToggleButton ToggleButton;

            public TabbedPanelTab(string tabName, ToggleButton button, Tab tab)
            {
                TabName = tabName;
                Tab = tab;
                ToggleButton = button;
            }
        }

        private readonly List<TabbedPanelTab> _tabs;

        internal TabbedPanel(GuiManager guiManager, Widget parent)
            : base(guiManager, parent)
        {
            _tabs = new List<TabbedPanelTab>(4);
        }

        public override Vector2 ClientSize
        {
            get
            {
                return base.ClientSize - new Vector2(0, ButtonHeight);
            }
        }

        public Tab GetTab(string tabName)
        {
            return _tabs.Single(p => p.TabName == tabName).Tab;
        }

        public Tab AddTab(string tabName)
        {
            if (_tabs.Any(p => p.TabName == tabName))
            {
                throw new ArgumentException(string.Format("Tab already exists with name `{0}`", tabName));
            }

            var tabbedPanelPanel = new TabbedPanelTab(
                tabName, 
                GuiManager.CreateToggleButton(tabName, new Vector2(), new Vector2(), true, this),
                GuiManager.CreatePanel(this));

            tabbedPanelPanel.ToggleButton.Click += ChangeTab;

            tabbedPanelPanel.ToggleButton.Position = new Vector2(_tabs.Count * ButtonWidth, 0);
            tabbedPanelPanel.ToggleButton.Size = new Vector2(ButtonWidth, ButtonHeight);
            tabbedPanelPanel.Tab.Visible = false;

            _tabs.Add(tabbedPanelPanel);

            tabbedPanelPanel.Tab.Position = new Vector2(0, ButtonHeight);

            tabbedPanelPanel.Tab.AutoSize = AutoSize.Width | AutoSize.Height;

            SwitchToTab(tabbedPanelPanel);

            return tabbedPanelPanel.Tab;
        }

        private void SwitchToTab(TabbedPanelTab panel)
        {
            foreach (var tabbedPanelPanel in _tabs)
            {
                tabbedPanelPanel.Tab.Visible = false;
            }
            panel.Tab.Visible = true;
        }

        private void ChangeTab(object sender, ClickEventArgs args)
        {
            var button = (ToggleButton)sender;
            var tabbedPanelTab = _tabs.Single(p => p.ToggleButton == button);
            SwitchToTab(tabbedPanelTab);
        }
    }
}