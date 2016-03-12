﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Win2ch.Common;

namespace Win2ch.Controls {
    public sealed class SwipeableSplitView : SplitView {
        #region private variables

        private Grid paneRoot;
        private Grid overlayRoot;
        private Rectangle panArea;
        private Rectangle dismissLayer;
        private CompositeTransform paneRootTransform;
        private CompositeTransform panAreaTransform;
        private Storyboard openSwipeablePane;
        private Storyboard closeSwipeablePane;

        private Selector menuHost;
        private IList<SelectorItem> menuItems = new List<SelectorItem>();
        private int toBeSelectedIndex;
        private static double _totalPanningDistance = 160d;
        private double distancePerItem;
        private double startingDistance;

        #endregion

        public SwipeableSplitView() {
            DefaultStyleKey = typeof(SwipeableSplitView);
        }

        #region properties

        // safely subscribe/unsubscribe manipulation events here
        internal Grid PaneRoot {
            get { return paneRoot; }
            set {
                if (paneRoot != null) {
                    paneRoot.Loaded -= OnPaneRootLoaded;
                    paneRoot.ManipulationStarted -= OnManipulationStarted;
                    paneRoot.ManipulationDelta -= OnManipulationDelta;
                    paneRoot.ManipulationCompleted -= OnManipulationCompleted;
                }

                paneRoot = value;

                if (paneRoot != null) {
                    paneRoot.Loaded += OnPaneRootLoaded;
                    paneRoot.ManipulationStarted += OnManipulationStarted;
                    paneRoot.ManipulationDelta += OnManipulationDelta;
                    paneRoot.ManipulationCompleted += OnManipulationCompleted;
                }
            }
        }

        // safely subscribe/unsubscribe manipulation events here
        internal Rectangle PanArea {
            get { return panArea; }
            set {
                if (panArea != null) {
                    panArea.ManipulationStarted -= OnManipulationStarted;
                    panArea.ManipulationDelta -= OnManipulationDelta;
                    panArea.ManipulationCompleted -= OnManipulationCompleted;
                    panArea.Tapped -= OnDismissLayerTapped;
                }

                panArea = value;

                if (panArea != null) {
                    panArea.ManipulationStarted += OnManipulationStarted;
                    panArea.ManipulationDelta += OnManipulationDelta;
                    panArea.ManipulationCompleted += OnManipulationCompleted;
                    panArea.Tapped += OnDismissLayerTapped;
                }
            }
        }

        // safely subscribe/unsubscribe manipulation events here
        internal Rectangle DismissLayer {
            get { return dismissLayer; }
            set {
                if (dismissLayer != null) {
                    dismissLayer.Tapped -= OnDismissLayerTapped;
                }

                dismissLayer = value;

                if (dismissLayer != null) {
                    dismissLayer.Tapped += OnDismissLayerTapped; ;
                }
            }
        }

        // safely subscribe/unsubscribe animation completed events here
        internal Storyboard OpenSwipeablePaneAnimation {
            get { return openSwipeablePane; }
            set {
                if (openSwipeablePane != null) {
                    openSwipeablePane.Completed -= OnOpenSwipeablePaneCompleted;
                }

                openSwipeablePane = value;

                if (openSwipeablePane != null) {
                    openSwipeablePane.Completed += OnOpenSwipeablePaneCompleted;
                }
            }
        }

        // safely subscribe/unsubscribe animation completed events here
        internal Storyboard CloseSwipeablePaneAnimation {
            get { return closeSwipeablePane; }
            set {
                if (closeSwipeablePane != null) {
                    closeSwipeablePane.Completed -= OnCloseSwipeablePaneCompleted;
                }

                closeSwipeablePane = value;

                if (closeSwipeablePane != null) {
                    closeSwipeablePane.Completed += OnCloseSwipeablePaneCompleted;
                }
            }
        }

        public bool IsSwipeablePaneOpen {
            get { return (bool)GetValue(IsSwipeablePaneOpenProperty); }
            set { SetValue(IsSwipeablePaneOpenProperty, value); }
        }

        public static readonly DependencyProperty IsSwipeablePaneOpenProperty =
            DependencyProperty.Register(nameof(IsSwipeablePaneOpen), typeof(bool), typeof(SwipeableSplitView), new PropertyMetadata(false, OnIsSwipeablePaneOpenChanged));

        private static void OnIsSwipeablePaneOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var splitView = (SwipeableSplitView)d;

            switch (splitView.DisplayMode) {
                case SplitViewDisplayMode.Inline:
                case SplitViewDisplayMode.CompactOverlay:
                case SplitViewDisplayMode.CompactInline:
                    splitView.IsPaneOpen = (bool)e.NewValue;
                    break;

                case SplitViewDisplayMode.Overlay:
                    if (splitView.OpenSwipeablePaneAnimation == null || splitView.CloseSwipeablePaneAnimation == null) return;
                    if ((bool)e.NewValue) {
                        splitView.OpenSwipeablePane();
                    } else {
                        splitView.CloseSwipeablePane();
                    }
                    break;
            }
        }

        public double PanAreaInitialTranslateX {
            get { return (double)GetValue(PanAreaInitialTranslateXProperty); }
            set { SetValue(PanAreaInitialTranslateXProperty, value); }
        }

        public static readonly DependencyProperty PanAreaInitialTranslateXProperty =
            DependencyProperty.Register(nameof(PanAreaInitialTranslateX), typeof(double), typeof(SwipeableSplitView), new PropertyMetadata(0d));

        public double PanAreaThreshold {
            get { return (double)GetValue(PanAreaThresholdProperty); }
            set { SetValue(PanAreaThresholdProperty, value); }
        }

        public static readonly DependencyProperty PanAreaThresholdProperty =
            DependencyProperty.Register(nameof(PanAreaThreshold), typeof(double), typeof(SwipeableSplitView), new PropertyMetadata(36d));


        /// <summary>
        /// enabling this will allow users to select a menu item by panning up/down on the bottom area of the left pane,
        /// this could be particularly helpful when holding large phones since users don't need to stretch their fingers to
        /// reach the top part of the screen to select a different menu item.
        /// </summary>
        public bool IsPanSelectorEnabled {
            get { return (bool)GetValue(IsPanSelectorEnabledProperty); }
            set { SetValue(IsPanSelectorEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsPanSelectorEnabledProperty =
            DependencyProperty.Register(nameof(IsPanSelectorEnabled), typeof(bool), typeof(SwipeableSplitView), new PropertyMetadata(true));

        #endregion

        protected override void OnApplyTemplate() {
            base.OnApplyTemplate();

            PaneRoot = GetTemplateChild<Grid>("PaneRoot");
            overlayRoot = GetTemplateChild<Grid>("OverlayRoot");
            PanArea = GetTemplateChild<Rectangle>("PanArea");
            DismissLayer = GetTemplateChild<Rectangle>("DismissLayer");

            var rootGrid = paneRoot.GetParent<Grid>();

            OpenSwipeablePaneAnimation = rootGrid.GetStoryboard("OpenSwipeablePane");
            CloseSwipeablePaneAnimation = rootGrid.GetStoryboard("CloseSwipeablePane");

            // initialization
            OnDisplayModeChanged(null, null);

            RegisterPropertyChangedCallback(DisplayModeProperty, OnDisplayModeChanged);

            // disable ScrollViewer as it will prevent finger from panning
            if (Pane is ListView || Pane is ListBox) {
                ScrollViewer.SetHorizontalScrollMode(Pane, ScrollMode.Disabled);
            }
        }

        #region native property change handlers

        private void OnDisplayModeChanged(DependencyObject sender, DependencyProperty dp) {
            switch (DisplayMode) {
                case SplitViewDisplayMode.Inline:
                case SplitViewDisplayMode.CompactOverlay:
                case SplitViewDisplayMode.CompactInline:
                    PanAreaInitialTranslateX = 0d;
                    overlayRoot.Visibility = Visibility.Collapsed;
                    break;

                case SplitViewDisplayMode.Overlay:
                    PanAreaInitialTranslateX = OpenPaneLength * -1;
                    overlayRoot.Visibility = Visibility.Visible;
                    break;
            }

            ((CompositeTransform)paneRoot.RenderTransform).TranslateX = PanAreaInitialTranslateX;
        }

        #endregion

        #region manipulation event handlers

        private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e) {
            panAreaTransform = PanArea.GetCompositeTransform();
            paneRootTransform = PaneRoot.GetCompositeTransform();
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
            var x = panAreaTransform.TranslateX + e.Delta.Translation.X;

            // keep the pan within the bountry
            if (x < PanAreaInitialTranslateX || x > 0) return;

            // while we are panning the PanArea on X axis, let's sync the PaneRoot's position X too
            paneRootTransform.TranslateX = panAreaTransform.TranslateX = x;

            if (sender == paneRoot && IsPanSelectorEnabled) {
                // un-highlight everything first
                foreach (var item in menuItems) {
                    VisualStateManager.GoToState(item, "Normal", true);
                }

                toBeSelectedIndex = (int)Math.Round((e.Cumulative.Translation.Y + startingDistance) / distancePerItem, MidpointRounding.AwayFromZero);
                if (toBeSelectedIndex < 0) {
                    toBeSelectedIndex = 0;
                } else if (toBeSelectedIndex >= menuItems.Count) {
                    toBeSelectedIndex = menuItems.Count - 1;
                }

                // highlight the item that's going to be selected
                var itemContainer = menuItems[toBeSelectedIndex];
                VisualStateManager.GoToState(itemContainer, "PointerOver", true);
            }
        }

        private async void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            var x = e.Velocities.Linear.X;

            // ignore a little bit velocity (+/-0.1)
            if (x <= -0.1) {
                CloseSwipeablePane();
            } else if (x > -0.1 && x < 0.1) {
                if (Math.Abs(panAreaTransform.TranslateX) > Math.Abs(PanAreaInitialTranslateX) / 2) {
                    CloseSwipeablePane();
                } else {
                    OpenSwipeablePane();
                }
            } else {
                OpenSwipeablePane();
            }

            if (IsPanSelectorEnabled) {
                if (sender == paneRoot) {
                    // if it's a flick, meaning the user wants to cancel the action, so we remove all the highlights;
                    // or it's intended to be a horizontal gesture, we also remove all the highlights
                    if (Math.Abs(e.Velocities.Linear.Y) >= 2 ||
                        Math.Abs(e.Cumulative.Translation.X) > Math.Abs(e.Cumulative.Translation.Y)) {
                        foreach (var item in menuItems) {
                            VisualStateManager.GoToState(item, "Normal", true);
                        }

                        return;
                    }

                    // un-highlight everything first
                    foreach (var item in menuItems) {
                        VisualStateManager.GoToState(item, "Unselected", true);
                    }

                    // highlight the item that's going to be selected
                    var itemContainer = menuItems[toBeSelectedIndex];
                    VisualStateManager.GoToState(itemContainer, "Selected", true);

                    // do a selection after a short delay to allow visual effect takes place first
                    await Task.Delay(250);
                    menuHost.SelectedIndex = toBeSelectedIndex;
                } else {
                    // recalculate the starting distance
                    startingDistance = distancePerItem * menuHost.SelectedIndex;
                }
            }
        }

        #endregion

        #region DismissLayer tap event handlers

        private void OnDismissLayerTapped(object sender, TappedRoutedEventArgs e) {
            CloseSwipeablePane();
        }

        #endregion

        #region animation completed event handlers

        private void OnOpenSwipeablePaneCompleted(object sender, object e) {
            DismissLayer.IsHitTestVisible = true;
        }

        private void OnCloseSwipeablePaneCompleted(object sender, object e) {
            DismissLayer.IsHitTestVisible = false;
        }

        #endregion

        #region loaded event handlers

        private void OnPaneRootLoaded(object sender, RoutedEventArgs e) {
            // fill the local menu items collection for later use
            if (IsPanSelectorEnabled) {
                var border = (Border)PaneRoot.Children[0];
                menuHost = border.GetChild<Selector>("For the bottom panning to work, the Pane's Child needs to be of type Selector.");

                foreach (var item in menuHost.Items) {
                    var container = (SelectorItem)menuHost.ContainerFromItem(item);
                    menuItems.Add(container);
                }

                distancePerItem = _totalPanningDistance / menuItems.Count;

                // calculate the initial starting distance
                startingDistance = distancePerItem * menuHost.SelectedIndex;
            }
        }

        #endregion

        #region private methods

        private void OpenSwipeablePane() {
            if (IsSwipeablePaneOpen) {
                OpenSwipeablePaneAnimation.Begin();
            } else {
                IsSwipeablePaneOpen = true;
            }
        }

        private void CloseSwipeablePane() {
            if (!IsSwipeablePaneOpen) {
                CloseSwipeablePaneAnimation.Begin();
            } else {
                IsSwipeablePaneOpen = false;
            }
        }

        private T GetTemplateChild<T>(string name, string message = null) where T : DependencyObject {
            var child = GetTemplateChild(name) as T;

            if (child == null) {
                if (message == null) {
                    message = $"{name} should not be null! Check the default Generic.xaml.";
                }

                throw new NullReferenceException(message);
            }

            return child;
        }

        #endregion
    }
}
