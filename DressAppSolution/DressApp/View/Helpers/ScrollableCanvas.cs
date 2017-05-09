﻿//using System;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Threading;
//using KinectFittingRoom.View.Buttons.Events;

//namespace DressApp.View.Helpers
//{
//    /// <summary>
//    /// ItemsControl class that responds to Kinect events
//    /// </summary>
//    public class ScrollableCanvas : ItemsControl
//    {
//        #region Constants
//        /// <summary>
//        /// Number of seconds to check position of Hand
//        /// </summary>
//        private const int EnterTimeout = 4;
//        /// <summary>
//        /// Translation of controls in panels
//        /// </summary>
//        private const int Distance = 10;
//        /// <summary>
//        /// Number of milliseconds of animation
//        /// </summary>
//        private const int TimeOfAnimation = 10;
//        /// <summary>
//        /// The minimum height factor
//        /// </summary>
//        private const double MinHeightFactor = 0.2;
//        /// <summary>
//        /// The maximum height factor
//        /// </summary>
//        private const double MaxHeightFactor = 0.4;
//        #endregion
//        #region Fields
//        /// <summary>
//        /// Position of LeftPanel
//        /// </summary>
//        private Point _leftPanelPosition;
//        /// <summary>
//        /// Determines how much time elapsed since hand position over canvas checked
//        /// </summary>
//        private readonly DispatcherTimer _enterTimer;
//        /// <summary>
//        /// Number of elapsed ticks for _enterTimer
//        /// </summary>
//        private int _enterTimerTicks;
//        /// <summary>
//        /// Determines if hand is over canvas
//        /// </summary>
//        private bool _isHandOverCanvas;
//        /// <summary>
//        /// Actual hand position
//        /// </summary>
//        private Point _handPosition;
//        /// <summary>
//        /// Position of last button in panel
//        /// </summary>
//        double _lastButtonPositionY;
//        /// <summary>
//        /// Position of first button in panel
//        /// </summary>
//        double _firstButtonPositionY;
//        /// <summary>
//        /// Start point of animation
//        /// </summary>
//        double _startAnimationPoint;
//        /// <summary>
//        /// Defines if buttons are moving
//        /// </summary>
//        private bool _isMoved;
//        /// <summary>
//        /// Top boundary to start scroll up
//        /// </summary>
//        private double _canvasMinHeight;
//        /// <summary>
//        /// Bottom boundary to start scroll down
//        /// </summary>
//        private double _canvasMaxHeight;
//        #endregion
//        #region Events
//        /// <summary>
//        /// Hand cursor enter event
//        /// </summary>
//        public static readonly RoutedEvent HandCursorEnterEvent
//            = KinectEvents.HandCursorEnterEvent.AddOwner(typeof(ScrollableCanvas));
//        /// <summary>
//        /// Hand cursor leave event
//        /// </summary>
//        public static readonly RoutedEvent HandCursorLeaveEvent
//            = KinectEvents.HandCursorLeaveEvent.AddOwner(typeof(ScrollableCanvas));
//        /// <summary>
//        /// Hand cursor move event
//        /// </summary>
//        public static readonly RoutedEvent HandCursorMoveEvent
//            = KinectEvents.HandCursorMoveEvent.AddOwner(typeof(ScrollableCanvas));
//        #endregion
//        #region Event handlers
//        /// <summary>
//        /// Hand cursor enter event handler
//        /// </summary>
//        public event HandCursorEventHandler HandCursorEnter
//        {
//            add { AddHandler(HandCursorEnterEvent, value); }
//            remove { RemoveHandler(HandCursorEnterEvent, value); }
//        }
//        /// <summary>
//        /// Hand cursor leave event handler
//        /// </summary>
//        public event HandCursorEventHandler HandCursorLeave
//        {
//            add { AddHandler(HandCursorLeaveEvent, value); }
//            remove { RemoveHandler(HandCursorLeaveEvent, value); }
//        }
//        /// <summary>
//        /// Hand cursor move event handler
//        /// </summary>
//        public event HandCursorEventHandler HandCursorMove
//        {
//            add { AddHandler(HandCursorLeaveEvent, value); }
//            remove { RemoveHandler(HandCursorLeaveEvent, value); }
//        }
//        #endregion Event handlers
//        #region .ctor
//        /// <summary>
//        /// Initializes a new instance of the <see cref="ScrollableCanvas"/> class.
//        /// </summary>
//        public ScrollableCanvas()
//        {
//            HandCursorEnter += ScrollableCanvas_HandCursorEnter;
//            HandCursorLeave += ScrollableCanvas_HandCursorLeave;
//            HandCursorMove += ScrollableCanvas_HandCursorMove;

//            _enterTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 1) };
//            _enterTimerTicks = 0;
//            _enterTimer.Tick += EnterTimer_Tick;

//            Items.CurrentChanged += (sender, args) => { _startAnimationPoint = 0; };
//        }
//        #endregion
//        #region Methods
//        /// <summary>
//        /// Counts the number of timer ticks of_enterTimer
//        /// </summary>
//        private void EnterTimer_Tick(object sender, EventArgs e)
//        {
//            _enterTimerTicks++;

//            if (_enterTimerTicks < EnterTimeout)
//                return;

//            _enterTimer.Stop();
//            _enterTimerTicks = 0;
//            if (_isHandOverCanvas)
//                RaiseEvent(new HandCursorEventArgs(HandCursorEnterEvent, _handPosition));
//        }
//        /// <summary>
//        /// Handles HandCursorMove event
//        /// </summary>
//        private void ScrollableCanvas_HandCursorMove(object sender, HandCursorEventArgs args)
//        {
//            if (_isHandOverCanvas)
//                _handPosition = new Point(args.X, args.Y);
//        }
//        /// <summary>
//        /// Handles HandCursorLeave event
//        /// </summary>
//        private void ScrollableCanvas_HandCursorLeave(object sender, HandCursorEventArgs args)
//        {
//            _isHandOverCanvas = false;
//        }
//        /// <summary>
//        /// Handles HandCursorEnter event
//        /// </summary>
//        private void ScrollableCanvas_HandCursorEnter(object sender, HandCursorEventArgs args)
//        {
//            if (!_isHandOverCanvas)
//                _handPosition = new Point(args.X, args.Y);
//            _isHandOverCanvas = true;
//            _isMoved = true;

//            StackPanel stackPanel = (Name == "LeftScrollableCanvas") ? FindChild<StackPanel>(Application.Current.MainWindow, "LeftStackPanel") : FindChild<StackPanel>(Application.Current.MainWindow, "RightStackPanel");
//            if (stackPanel.Children.Count == 0)
//                return;

//            SetPositions(stackPanel);
//            if (!CheckHandPosition(stackPanel))
//                return;

//            if (_isHandOverCanvas)
//                _enterTimer.Start();
//        }
//        /// <summary>
//        /// Checks hand position and runs MoveButtons method
//        /// </summary>
//        /// <param name="stackPanel">Collection of buttons in panel</param>
//        /// <returns>Value if hand is over panel sensitive area</returns>
//        private bool CheckHandPosition(StackPanel stackPanel)
//        {
//            if (_handPosition.Y > _canvasMinHeight && _handPosition.Y < _canvasMaxHeight)
//                return false;
//            if (_handPosition.Y > _canvasMaxHeight)
//                while (_isMoved && _lastButtonPositionY + _startAnimationPoint > _canvasMaxHeight)
//                    MoveButtons(stackPanel, true);
//            else if (_handPosition.Y < _canvasMinHeight)
//                while (_isMoved && _firstButtonPositionY + _startAnimationPoint < _firstButtonPositionY)
//                    MoveButtons(stackPanel, false);
//            return true;
//        }
//        /// <summary>
//        /// Sets positions of first and last buttons in panel
//        /// Sets minimum and maximum height of panel sensitive area 
//        /// </summary>
//        /// <param name="stackPanel">Collection of buttons in panel</param>
//        private void SetPositions(StackPanel stackPanel)
//        {
//            if (_firstButtonPositionY == 0)
//                _firstButtonPositionY = stackPanel.Children[0].TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0)).Y;
//            _lastButtonPositionY = stackPanel.Children[stackPanel.Children.Count - 1].TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0)).Y;

//            if (_leftPanelPosition.X == 0 && _leftPanelPosition.Y == 0)
//            {
//                _leftPanelPosition = TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
//                _canvasMinHeight = ActualHeight * MinHeightFactor + _leftPanelPosition.Y;
//                _canvasMaxHeight = ActualHeight * MaxHeightFactor + _leftPanelPosition.Y;
//            }
//        }
//        /// <summary>
//        /// Moves buttons in panels
//        /// </summary>
//        /// <param name="stackpanel">Collection of buttons in panel</param>
//        /// <param name="moveUp">Defines if buttons are moving up</param>
//        private void MoveButtons(StackPanel stackpanel, bool moveUp)
//        {
//            _startAnimationPoint = moveUp ? _startAnimationPoint - Distance : _startAnimationPoint + Distance;

//            Button button;
//            TranslateTransform translation = new TranslateTransform();
//            DoubleAnimation animation = new DoubleAnimation()
//            {
//                Duration = TimeSpan.FromMilliseconds(TimeOfAnimation),
//                From = moveUp ? _startAnimationPoint + Distance : _startAnimationPoint,
//                To = moveUp ? _startAnimationPoint : _startAnimationPoint + Distance
//            };

//            foreach (var control in stackpanel.Children)
//            {
//                button = FindChild<Button>(control as ContentPresenter, "");
//                if (button != null)
//                    button.RenderTransform = translation;
//            }

//            translation.BeginAnimation(TranslateTransform.YProperty, animation);
//            _isMoved = !_isMoved;
//        }
//        /// <summary>
//        /// Find child control in Visual Tree Helper of parent control
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="parent">Parent control</param>
//        /// <param name="childName">Name of child control</param>
//        /// <returns>Found child control</returns>
//        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
//        {
//            if (parent == null)
//                return null;

//            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
//            for (int i = 0; i < childrenCount; i++)
//            {
//                var child = VisualTreeHelper.GetChild(parent, i);
//                T childType = child as T;
//                if (childType == null)
//                {
//                    T foundChild = FindChild<T>(child, childName);
//                    if (foundChild != null)
//                        return foundChild;
//                }
//                else if (!string.IsNullOrEmpty(childName))
//                {
//                    var frameworkElement = child as FrameworkElement;
//                    if (frameworkElement != null && frameworkElement.Name == childName)
//                        return (T)child;
//                }
//                else
//                    return (T)child;
//            }
//            return null;
//        }
//        #endregion
//    }
//}
