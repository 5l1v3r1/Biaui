﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;
using static System.Windows.PresentationSource;

namespace Biaui.Controls
{
    public class BiaColorBar : FrameworkElement
    {
        #region Value

        public double Value
        {
            get => _value;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _value)
                    SetValue(ValueProperty, value);
            }
        }

        private double _value = default(double);

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(BiaColorBar),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._value = (double) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Color0

        public Color Color0
        {
            get => _Color0;
            set
            {
                if (value != _Color0)
                    SetValue(Color0Property, value);
            }
        }

        private Color _Color0 = Colors.Black;

        public static readonly DependencyProperty Color0Property =
            DependencyProperty.Register(nameof(Color0), typeof(Color), typeof(BiaColorBar),
                new PropertyMetadata(
                    Boxes.ColorBlack,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._Color0 = (Color) e.NewValue;
                        self._isRequestUpdateBackgroundBrush = true;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Color1

        public Color Color1
        {
            get => _Color1;
            set
            {
                if (value != _Color1)
                    SetValue(Color1Property, value);
            }
        }

        private Color _Color1 = Colors.White;

        public static readonly DependencyProperty Color1Property =
            DependencyProperty.Register(nameof(Color1), typeof(Color), typeof(BiaColorBar),
                new PropertyMetadata(
                    Boxes.ColorWhite,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._Color1 = (Color) e.NewValue;
                        self._isRequestUpdateBackgroundBrush = true;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region BorderColor

        public Color BorderColor
        {
            get => _BorderColor;
            set
            {
                if (value != _BorderColor)
                    SetValue(BorderColorProperty, value);
            }
        }

        private Color _BorderColor = Colors.Red;

        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaColorBar),
                new PropertyMetadata(
                    Boxes.ColorRed,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._BorderColor = (Color) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region IsInverseValue

        public bool IsInverseValue
        {
            get => _IsInverseValue;
            set
            {
                if (value != _IsInverseValue)
                    SetValue(IsInverseValueProperty, value);
            }
        }

        private bool _IsInverseValue = default(bool);

        public static readonly DependencyProperty IsInverseValueProperty =
            DependencyProperty.Register(nameof(IsInverseValue), typeof(bool), typeof(BiaColorBar),
                new PropertyMetadata(
                    Boxes.BoolFalse,
                    (s, e) =>
                    {
                        var self = (BiaColorBar) s;
                        self._IsInverseValue = (bool) e.NewValue;
                    }));

        #endregion

        private Brush _backgroundBrush;
        private bool _isRequestUpdateBackgroundBrush = true;

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (_isRequestUpdateBackgroundBrush)
            {
                _isRequestUpdateBackgroundBrush = false;
                UpdateBackgroundBrush();
            }

            dc.DrawRectangle(_backgroundBrush, Caches.GetBorderPen(BorderColor, 1), ActualRectangle);

            var y = Value * ActualHeight;
            if (IsInverseValue)
                y = ActualHeight - y;

            var r = new Rect(1, y - 2, ActualWidth - 2, 5);

            dc.DrawRectangle(null, Caches.PointOut, r);
            dc.DrawRectangle(null, Caches.PointIn, r);
        }

        private void UpdateBackgroundBrush()
        {
            _backgroundBrush = new LinearGradientBrush(Color1, Color0, 90);
            _backgroundBrush.Freeze();
        }

        private const double borderSize = 1.0;

        private void UpdateParams(MouseEventArgs e)
        {
            var pos = e.GetPosition(this);

            var y = pos.Y / (ActualHeight - borderSize);

            Value = IsInverseValue ? 1 - y : y;
        }

        private bool _isMouseDown;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _isMouseDown = true;
            GuiHelper.HideCursor();

            UpdateParams(e);

            // マウス可動域を設定
            {
                var p0 = new Point(0, 0);
                var p1 = new Point(ActualWidth, ActualHeight);
                var dp0 = PointToScreen(p0);
                var dp1 = PointToScreen(p1);
                var cr = new Win32Helper.RECT((int) dp0.X, (int) dp0.Y, (int) dp1.X, (int) dp1.Y);
                Win32Helper.ClipCursor(ref cr);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isMouseDown == false)
                return;

            UpdateParams(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            _isMouseDown = false;
            Win32Helper.ClipCursor(IntPtr.Zero);
            GuiHelper.ShowCursor();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_isMouseDown)
            {
                _isMouseDown = false;
                ReleaseMouseCapture();
                Win32Helper.ClipCursor(IntPtr.Zero);
            }

            InvalidateVisual();
        }

        private Rect ActualRectangle => new Rect(new Size(ActualWidth, ActualHeight));
    }
}