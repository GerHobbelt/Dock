﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Avalonia.Controls
{
    /// <summary>
    /// Represents a control that lets the user change the size of elements in a <see cref="DockPanel" />.
    /// </summary>
    public class DockPanelSplitter : Thumb
    {
        private Control _element;

        /// <summary>
        /// Defines the <see cref="Thickness"/> property.
        /// </summary>
        public static readonly StyledProperty<double> ThicknessProperty =
            AvaloniaProperty.Register<DockPanel, double>(
                nameof(Thickness),
                defaultValue: 4.0);

        /// <summary>
        /// Gets or sets the thickness (height or width, depending on orientation).
        /// </summary>
        /// <value>The thickness.</value>
        public double Thickness
        {
            get { return GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DockPanelSplitter" /> class.
        /// </summary>
        public DockPanelSplitter()
        {
        }

        /// <summary>
        /// Gets a value indicating whether this splitter is horizontal.
        /// </summary>
        public bool IsHorizontal
        {
            get
            {
                var dock = DockPanel.GetDock(this);
                return dock == Dock.Top || dock == Dock.Bottom;
            }
        }

        protected override void OnDragDelta(VectorEventArgs e)
        {
            var dock = DockPanel.GetDock(this);
            if (IsHorizontal)
            {
                AdjustHeight(e.Vector.Y, dock);
            }
            else
            {
                AdjustWidth(e.Vector.X, dock);
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            UpdateHeightOrWidth();
            UpdateTargetElement();
        }

        private void AdjustHeight(double dy, Dock dock)
        {
            if (dock == Dock.Bottom)
            {
                dy = -dy;
            }
            SetTargetHeight(dy);
        }

        private void AdjustWidth(double dx, Dock dock)
        {
            if (dock == Dock.Right)
            {
                dx = -dx;
            }
            SetTargetWidth(dx);
        }

        private void SetTargetHeight(double dy)
        {
            double height = _element.DesiredSize.Height + dy;

            if (height < _element.MinHeight)
            {
                height = _element.MinHeight;
            }

            if (height > _element.MaxHeight)
            {
                height = _element.MaxHeight;
            }

            var panel = Parent as Panel;
            var dock = DockPanel.GetDock(this);
            if (dock == Dock.Top && height > panel.DesiredSize.Height - Thickness)
            {
                height = panel.DesiredSize.Height - Thickness;
            }

            System.Console.WriteLine($"{_element.Name} height: {height} dy: {dy}");

            _element.Height = height;
        }

        private void SetTargetWidth(double dx)
        {
            double width = _element.DesiredSize.Width + dx;

            if (width < _element.MinWidth)
            {
                width = _element.MinWidth;
            }

            if (width > _element.MaxWidth)
            {
                width = _element.MaxWidth;
            }

            var panel = Parent as Panel;
            var dock = DockPanel.GetDock(this);
            if (dock == Dock.Left && width > panel.DesiredSize.Width - Thickness)
            {
                width = panel.DesiredSize.Width - Thickness;
            }

            System.Console.WriteLine($"{_element.Name} width: {width} dx: {dx}");

            _element.Width = width;
        }

        private void UpdateHeightOrWidth()
        {
            if (IsHorizontal)
            {
                Height = Thickness;
                Width = double.NaN;
                Cursor = new Cursor(StandardCursorType.SizeNorthSouth);
                PseudoClasses.Add(":horizontal");
            }
            else
            {
                Width = Thickness;
                Height = double.NaN;
                Cursor = new Cursor(StandardCursorType.SizeWestEast);
                PseudoClasses.Add(":vertical");
            }
        }

        private void UpdateTargetElement()
        {
            if (!(this.Parent is Panel panel))
            {
                return;
            }

            int index = panel.Children.IndexOf(this);
            if (index > 0 && panel.Children.Count > 0)
            {
                _element = panel.Children[index - 1] as Control;
            }
        }
    }
}
