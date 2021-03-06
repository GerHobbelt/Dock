﻿using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Dock.Avalonia.Internal;
using Dock.Model;
using Dock.Model.Core;

namespace Dock.Avalonia.Controls
{
    /// <summary>
    /// Interaction logic for <see cref="DockControl"/> xaml.
    /// </summary>
    public class DockControl : TemplatedControl, IDockControl
    {
        private readonly DockManager _dockManager;
        private readonly DockControlState _dockControlState;

        /// <summary>
        /// Defines the <see cref="Layout"/> property.
        /// </summary>
        public static readonly StyledProperty<IDock?> LayoutProperty =
            AvaloniaProperty.Register<DockControl, IDock?>(nameof(Layout));

        /// <summary>
        /// Defines the <see cref="InitializeLayout"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> InitializeLayoutProperty =
            AvaloniaProperty.Register<DockControl, bool>(nameof(InitializeLayout));

        /// <summary>
        /// Defines the <see cref="InitializeFactory"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> InitializeFactoryProperty =
            AvaloniaProperty.Register<DockControl, bool>(nameof(InitializeFactory));

        /// <inheritdoc/>
        public IDockManager DockManager => _dockManager;

        /// <inheritdoc/>
        public IDockControlState DockControlState => _dockControlState;

        /// <inheritdoc/>
        [Content]
        public IDock? Layout
        {
            get => GetValue(LayoutProperty);
            set => SetValue(LayoutProperty, value);
        }

        /// <inheritdoc/>
        public bool InitializeLayout
        {
            get => GetValue(InitializeLayoutProperty);
            set => SetValue(InitializeLayoutProperty, value);
        }

        /// <inheritdoc/>
        public bool InitializeFactory
        {
            get => GetValue(InitializeFactoryProperty);
            set => SetValue(InitializeFactoryProperty, value);
        }

        /// <summary>
        /// Initialize the new instance of the <see cref="DockControl"/>.
        /// </summary>
        public DockControl()
        {
            _dockManager = new DockManager();
            _dockControlState = new DockControlState(_dockManager);
            AddHandler(PointerPressedEvent, Pressed, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            AddHandler(PointerReleasedEvent, Released, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            AddHandler(PointerMovedEvent, Moved, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            AddHandler(PointerEnterEvent, Enter, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            AddHandler(PointerLeaveEvent, Leave, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            AddHandler(PointerCaptureLostEvent, CaptureLost, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            AddHandler(PointerWheelChangedEvent, WheelChanged, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        }

        /// <inheritdoc/>
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (Layout?.Factory is null)
            {
                return;
            }
 
            Layout.Factory.DockControls.Add(this);

            if (InitializeFactory)
            {
                Layout.Factory.ContextLocator = new Dictionary<string, Func<object>>();

                Layout.Factory.DockableLocator = new Dictionary<string, Func<IDockable?>>();

                Layout.Factory.HostWindowLocator = new Dictionary<string, Func<IHostWindow>>
                {
                    [nameof(IDockWindow)] = () => new HostWindow()
                };
            }

            if (InitializeLayout)
            {
                Layout.Factory.InitLayout(Layout);
            }
        }

        /// <inheritdoc/>
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (Layout?.Factory is null)
            {
                return;
            }

            Layout.Factory.DockControls.Remove(this);

            if (InitializeLayout && Layout is { })
            {
                if (Layout.Close.CanExecute(null))
                {
                    Layout.Close.Execute(null);
                }
            }
        }

        private static DragAction ToDragAction(PointerEventArgs e)
        {
            if (e.KeyModifiers.HasFlag(KeyModifiers.Alt))
            {
                return DragAction.Link;
            }

            if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                return DragAction.Move;
            }

            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                return DragAction.Copy;
            }

            return DragAction.Move;
        }

        private void Pressed(object? sender, PointerPressedEventArgs e)
        {
            if (Layout?.Factory?.DockControls is { })
            {
                var position = e.GetPosition(this);
                var delta = new Vector();
                var action = ToDragAction(e);
                _dockControlState.Process(position, delta, EventType.Pressed, action, this, Layout.Factory.DockControls);
            }
        }

        private void Released(object? sender, PointerReleasedEventArgs e)
        {
            if (Layout?.Factory?.DockControls is { })
            {
                var position = e.GetPosition(this);
                var delta = new Vector();
                var action = ToDragAction(e);
                _dockControlState.Process(position, delta, EventType.Released, action, this, Layout.Factory.DockControls);
            }
        }

        private void Moved(object? sender, PointerEventArgs e)
        {
            if (Layout?.Factory?.DockControls is { })
            {
                var position = e.GetPosition(this);
                var delta = new Vector();
                var action = ToDragAction(e);
                _dockControlState.Process(position, delta, EventType.Moved, action, this, Layout.Factory.DockControls);
            }
        }

        private void Enter(object? sender, PointerEventArgs e)
        {
            if (Layout?.Factory?.DockControls is { })
            {
                var position = e.GetPosition(this);
                var delta = new Vector();
                var action = ToDragAction(e);
                _dockControlState.Process(position, delta, EventType.Enter, action, this, Layout.Factory.DockControls);
            }
        }

        private void Leave(object? sender, PointerEventArgs e)
        {
            if (Layout?.Factory?.DockControls is { })
            {
                var position = e.GetPosition(this);
                var delta = new Vector();
                var action = ToDragAction(e);
                _dockControlState.Process(position, delta, EventType.Leave, action, this, Layout.Factory.DockControls);
            }
        }

        private void CaptureLost(object? sender, PointerCaptureLostEventArgs e)
        {
            if (Layout?.Factory?.DockControls is { })
            {
                var position = new Point();
                var delta = new Vector();
                var action = DragAction.None;
                _dockControlState.Process(position, delta, EventType.CaptureLost, action, this, Layout.Factory.DockControls);
            }
        }

        private void WheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (Layout?.Factory?.DockControls is { })
            {
                var position = e.GetPosition(this);
                var delta = e.Delta;
                var action = ToDragAction(e);
                _dockControlState.Process(position, delta, EventType.WheelChanged, action, this, Layout.Factory.DockControls);
            }
        }
    }
}
