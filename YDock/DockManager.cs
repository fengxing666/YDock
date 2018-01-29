﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;
using YDock.View;

namespace YDock
{
    [ContentProperty("Root")]
    public class DockManager : Control, IDockView
    {
        static DockManager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockManager), new FrameworkPropertyMetadata(typeof(DockManager)));
            FocusableProperty.OverrideMetadata(typeof(DockManager), new FrameworkPropertyMetadata(false));
        }

        public DockManager()
        {
            Root = new DockRoot();
            _dragManager = new DragManager(this);
            _dockControls = new List<IDockControl>();
            _floatControls = new List<IDockControl>();
        }

        #region Root
        private DockRoot _root;
        internal DockRoot Root
        {
            get { return _root; }
            set
            {
                if (_root != value)
                {
                    if (_root != null)
                        _root.Dispose();
                    _root = value;
                    if (_root != null)
                        _root.DockManager = this;
                }
            }
        }
        #endregion

        #region DragManager
        private DragManager _dragManager;
        internal DragManager DragManager
        {
            get { return _dragManager; }
        }
        #endregion

        #region DependencyProperty
        public static readonly DependencyProperty LeftSideProperty =
            DependencyProperty.Register("LeftSide", typeof(DockBarGroupControl), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnLeftSideChanged)));

        public DockBarGroupControl LeftSide
        {
            get { return (DockBarGroupControl)GetValue(LeftSideProperty); }
            set { SetValue(LeftSideProperty, value); }
        }

        private static void OnLeftSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockManager)d).OnLeftSideChanged(e);
        }

        protected virtual void OnLeftSideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty RightSideProperty =
            DependencyProperty.Register("RightSide", typeof(DockBarGroupControl), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnRightSideChanged)));

        public DockBarGroupControl RightSide
        {
            get { return (DockBarGroupControl)GetValue(RightSideProperty); }
            set { SetValue(RightSideProperty, value); }
        }

        private static void OnRightSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockManager)d).OnRightSideChanged(e);
        }

        protected virtual void OnRightSideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty TopSideProperty =
            DependencyProperty.Register("TopSide", typeof(DockBarGroupControl), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnTopSideChanged)));

        public DockBarGroupControl TopSide
        {
            get { return (DockBarGroupControl)GetValue(TopSideProperty); }
            set { SetValue(TopSideProperty, value); }
        }

        private static void OnTopSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockManager)d).OnTopSideChanged(e);
        }

        protected virtual void OnTopSideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty BottomSideProperty =
            DependencyProperty.Register("BottomSide", typeof(DockBarGroupControl), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnBottomSideChanged)));

        public DockBarGroupControl BottomSide
        {
            get { return (DockBarGroupControl)GetValue(BottomSideProperty); }
            set { SetValue(BottomSideProperty, value); }
        }

        private static void OnBottomSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockManager)d).OnBottomSideChanged(e);
        }

        protected virtual void OnBottomSideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty LayoutRootPanelProperty =
            DependencyProperty.Register("LayoutRootPanel", typeof(LayoutRootPanel), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnLayoutRootPanelChanged)));

        public LayoutRootPanel LayoutRootPanel
        {
            get { return (LayoutRootPanel)GetValue(LayoutRootPanelProperty); }
            set { SetValue(LayoutRootPanelProperty, value); }
        }

        private static void OnLayoutRootPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockManager)d).OnLayoutRootPanelChanged(e);
        }

        protected virtual void OnLayoutRootPanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }
        #endregion

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (_root.DockManager == this)
            {
                LayoutRootPanel = new LayoutRootPanel(_root);
                LeftSide = new DockBarGroupControl(_root.LeftSide);
                RightSide = new DockBarGroupControl(_root.RightSide);
                BottomSide = new DockBarGroupControl(_root.BottomSide);
                TopSide = new DockBarGroupControl(_root.TopSide);
            }
        }

        /// <summary>
        /// 自动隐藏窗口的Model
        /// </summary>
        public IDockElement AutoHideElement
        {
            get { return LayoutRootPanel.AHWindow.Model; }
            internal set
            {
                if (LayoutRootPanel.AHWindow.Model != value)
                {
                    if(LayoutRootPanel.AHWindow.Model != null)
                        LayoutRootPanel.AHWindow.Model.IsVisible = false;
                    LayoutRootPanel.AHWindow.Model = value as DockElement;
                    if (LayoutRootPanel.AHWindow.Model != null)
                        LayoutRootPanel.AHWindow.Model.IsVisible = true;
                }
            }
        }

        /// <summary>
        /// current ActiveElement
        /// </summary>
        public DockElement ActiveElement
        {
            get { return _activeElement; }
            internal set
            {
                if (_activeElement != value)
                {
                    if (_activeElement != null)
                        _activeElement.IsActive = false;
                    _activeElement = value;
                    if (_activeElement != null)
                    {
                        _activeElement.IsActive = true;
                        //这里必须将AutoHideElement设为NULL，保证当前活动窗口只有一个
                        AutoHideElement = null;
                    }
                }
            }
        }
        private DockElement _activeElement;

        public IDockModel Model
        {
            get
            {
                return null;
            }
        }

        public IDockView DockViewParent
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// all registed DockControl
        /// </summary>
        public IEnumerable<IDockControl> DockControls
        {
            get
            {
                foreach (var ctrl in _dockControls)
                    yield return ctrl;
            }
        }
        private IList<IDockControl> _dockControls;

        /// <summary>
        /// all registed FloatControls
        /// </summary>
        public IEnumerable<IDockControl> FloatControls
        {
            get
            {
                foreach (var ctrl in _floatControls)
                    yield return ctrl;
            }
        }
        private IList<IDockControl> _floatControls;

        public int AllControlsCount
        {
            get
            {
                return _dockControls.Count + _floatControls.Count;
            }
        }

        internal void ChangeControlMode(IDockControl ctrl)
        {
            if (ctrl.Mode == DockMode.Float)
            {
                _floatControls.Remove(ctrl);
                _dockControls.Add(ctrl);
            }
            else
            {
                _dockControls.Remove(ctrl);
                _floatControls.Add(ctrl);
            }
        }

        /// <summary>
        /// all Docked child
        /// </summary>
        internal IEnumerable<IDockElement> DockedChildren
        {
            get
            {
                foreach (DockControl ctrl in _dockControls)
                    yield return ctrl.Prototype;
            }
        }

        /// <summary>
        /// all Docked child
        /// </summary>
        internal IEnumerable<IDockElement> FloatedChildren
        {
            get
            {
                foreach (DockControl ctrl in _floatControls)
                    yield return ctrl.Prototype;
            }
        }

        #region Register
        /// <summary>
        /// 以选项卡模式向DockManager注册一个DockElement并返回对应的DockControl
        /// </summary>
        /// <param name="title">标题栏文字</param>
        /// <param name="content">内容</param>
        /// <param name="imageSource">标题栏图标</param>
        /// <param name="canSelect">是否直接停靠在选项栏中供用户选择(默认为False)</param>
        /// <param name="desiredWidth">期望的宽度</param>
        /// <param name="desiredHeight">期望的高度</param>
        /// <returns></returns>
        public DockControl RegisterDocument(string title, UIElement content, ImageSource imageSource = null, bool canSelect = false, double desiredWidth = Constants.DockDefaultWidthLength, double desiredHeight = Constants.DockDefaultHeightLength)
        {
            DockElement ele = new DockElement()
            {
                ID = AllControlsCount,
                Title = title,
                Content = content,
                ImageSource = imageSource,
                Side = DockSide.None,
                Mode = DockMode.Normal,
                CanSelect = canSelect,
                DesiredWidth = desiredWidth,
                DesiredHeight = desiredHeight
            };
            var ctrl = new DockControl(ele);
            _dockControls.Add(ctrl);
            _root.DocumentModel.Attach(ele);
            return ctrl;
        }
        /// <summary>
        /// 以DockBar模式（必须指定停靠方向，否则默认停靠在左侧）向DockManager注册一个DockElement并返回对应的DockControl
        /// </summary>
        /// <param name="title">标题栏文字</param>
        /// <param name="content">内容</param>
        /// <param name="imageSource">标题栏图标</param>
        /// <param name="side">停靠方向（默认左侧）</param>
        /// <param name="canSelect">是否直接停靠在选项栏中供用户选择(默认为False)</param>
        /// <param name="desiredWidth">期望的宽度</param>
        /// <param name="desiredHeight">期望的高度</param>
        /// <returns></returns>
        public DockControl RegisterDock(string title, UIElement content, ImageSource imageSource = null, DockSide side = DockSide.Left, bool canSelect = false, double desiredWidth = Constants.DockDefaultWidthLength, double desiredHeight = Constants.DockDefaultHeightLength)
        {
            DockElement ele = new DockElement()
            {
                ID = AllControlsCount,
                Title = title,
                Content = content,
                ImageSource = imageSource,
                Side = side,
                Mode = DockMode.DockBar,
                CanSelect = canSelect,
                DesiredWidth = desiredWidth,
                DesiredHeight = desiredHeight
            };
            switch (side)
            {
                case DockSide.Left:
                case DockSide.Right:
                case DockSide.Top:
                case DockSide.Bottom:
                    _root.AddSideChild(ele, side);
                    break;
                default://其他非法方向返回NULL
                    ele.Dispose();
                    return null;
            }
            var ctrl = new DockControl(ele);
            _dockControls.Add(ctrl);
            return ctrl;
        }
        /// <summary>
        /// 以Float模式向DockManager注册一个DockElement并返回对应的DockControl
        /// </summary>
        /// <param name="title">标题栏文字</param>
        /// <param name="content">内容</param>
        /// <param name="imageSource">标题栏图标</param>
        /// <param name="side">停靠方向（默认左侧）</param>
        /// <param name="desiredWidth">期望的宽度</param>
        /// <param name="desiredHeight">期望的高度</param>
        /// <returns></returns>
        public DockControl RegisterFloat(string title, UIElement content, ImageSource imageSource = null, DockSide side = DockSide.Left, double desiredWidth = Constants.DockDefaultWidthLength, double desiredHeight = Constants.DockDefaultHeightLength)
        {
            DockElement ele = new DockElement()
            {
                ID = AllControlsCount,
                Title = title,
                Content = content,
                ImageSource = imageSource,
                Side = side,
                Mode = DockMode.Float,
                DesiredWidth = desiredWidth,
                DesiredHeight = desiredHeight
            };
            var ctrl = new DockControl(ele);
            _floatControls.Add(ctrl);
            return ctrl;
        }
        #endregion

        internal void ChangeSide(IDockView view, DockSide side)
        {
            if (view.Model.Side == side) return;
            if (view is BaseGroupControl)
                (view.Model as BaseLayoutGroup).Side = side;
            if (view is LayoutGroupPanel)
                (view as LayoutGroupPanel).Side = side;
        }


        public void Dispose()
        {
            foreach (var ctrl in _dockControls)
                ctrl.Dispose();
            _dockControls.Clear();
            _dockControls = null;
            foreach (var ctrl in _floatControls)
                ctrl.Dispose();
            _floatControls.Clear();
            _floatControls = null;
            Root = null;
        }
    }
}