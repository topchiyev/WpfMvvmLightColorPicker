using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Gymdata.Accreditation.Util;

namespace Gymdata.Accreditation.ViewModel
{
    public class ColorPickerViewModel: ViewModelBase
    {
        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;
                _isPickingHue = false;
                _isPickingColor = false;
                RaisePropertyChanged(() => IsOpen);
            }
        }

        public Action<string> OnColorPicked { get; set; }
        public Action OnCancelled { get; set; }

        private string _originalColor = null;

        private string _color = "#000000";
        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                RaisePropertyChanged(() => Color);
            }
        }
        
        private bool _isPickingHue;
        private bool _isPickingColor;

        private Point _huePosition;
        public Point HuePosition
        {
            get { return _huePosition; }
            set
            {
                _huePosition = value;
                RaisePropertyChanged(() => HuePosition);
                RaisePropertyChanged(() => HuePointerMargin);
            }
        }

        public Thickness HuePointerMargin => new Thickness(0, HuePosition.Y, 0, 0);

        private Point _dialPosition;
        public Point DialPosition
        {
            get { return _dialPosition; }
            set
            {
                _dialPosition = value;
                RaisePropertyChanged(() => DialPosition);
                RaisePropertyChanged(() => DialPointerMargin);
            }
        }
        
        public Thickness DialPointerMargin => new Thickness(DialPosition.X, DialPosition.Y, 0, 0);

        private RenderTargetBitmap _hueBitmap;
        private DrawingVisual _hueImage;
        public DrawingVisual HueImage
        {
            get
            {
                if (_hueImage == null && HueView != null && HueView.ActualHeight > 0)
                {
                    var w = 20;
                    var h = Math.Max(1, (int)HueView.ActualHeight);

                    var visual = new DrawingVisual();
                    var context = visual.RenderOpen();
                    var brush = new LinearGradientBrush(new GradientStopCollection(new []
                    {
                        new GradientStop(ColorUtils.HexStringToColor("#FF0000"), 0),
                        new GradientStop(ColorUtils.HexStringToColor("#FFFF00"), 0.17),
                        new GradientStop(ColorUtils.HexStringToColor("#00FF00"), 0.34),
                        new GradientStop(ColorUtils.HexStringToColor("#00FFFF"), 0.51),
                        new GradientStop(ColorUtils.HexStringToColor("#0000FF"), 0.68),
                        new GradientStop(ColorUtils.HexStringToColor("#FF00FF"), 0.85),
                        new GradientStop(ColorUtils.HexStringToColor("#FF0000"), 1),
                    }), 90);
                    context.DrawRectangle(brush, null, new Rect(new Point(0, 0), new Size(w, h)));
                    context.Close();

                    var target = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
                    target.Render(visual);

                    _hueBitmap = target;

                    _hueImage = visual;
                }

                return _hueImage;
            }
        }

        private RenderTargetBitmap _dialBitmap;
        private DrawingVisual _dialImage;
        public DrawingVisual DialImage
        {
            get
            {
                if (_dialImage == null && DialView != null && DialView.ActualWidth > 0 && DialView.ActualHeight > 0)
                {
                    var w = Math.Max(1, (int)DialView.ActualWidth);
                    var h = Math.Max(1, (int)DialView.ActualHeight);

                    var hueColor = GetHueColor(HuePosition);

                    var visual = new DrawingVisual();
                    var context = visual.RenderOpen();

                    var rect = new Rect(new Point(0, 0), new Size(w, h));

                    context.DrawRectangle(new SolidColorBrush(hueColor), null, rect);

                    var whiteBrush = new LinearGradientBrush(new GradientStopCollection(new []
                    {
                        new GradientStop(ColorUtils.HexRgbaStringToColor("#FFFFFFFF"), 0),
                        new GradientStop(ColorUtils.HexRgbaStringToColor("#FFFFFF00"), 1),
                    }), 0);
                    
                    context.DrawRectangle(whiteBrush, null, rect);

                    var blackBrush = new LinearGradientBrush(new GradientStopCollection(new []
                    {
                        new GradientStop(ColorUtils.HexRgbaStringToColor("#00000000"), 0),
                        new GradientStop(ColorUtils.HexRgbaStringToColor("#000000FF"), 1),
                    }), 90);

                    context.DrawRectangle(blackBrush, null, rect);

                    context.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(new Point(0, 0), new Size(1, 1)));
                    context.DrawRectangle(new SolidColorBrush(Colors.Black), null, new Rect(new Point(0, h-1), new Size(w, 1)));

                    context.Close();

                    var target = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
                    target.Render(visual);

                    _dialBitmap = target;

                    _dialImage = visual;
                }

                return _dialImage;
            }
        }

        private FrameworkElement _dialView;
        public FrameworkElement DialView
        {
            get { return _dialView; }
            set
            {
                _dialView = value;
                RaisePropertyChanged(() => DialView);
            }
        }

        private FrameworkElement _hueView;
        public FrameworkElement HueView
        {
            get { return _hueView; }
            set
            {
                _hueView = value;
                RaisePropertyChanged(() => HueView);
            }
        }
        
        private void PickHue(Point pos)
        {
            var bmp = _hueBitmap;
            if (bmp == null) return;

            var pixels = BitmapUtils.GetPixels(bmp);
            if (pixels == null) return;

            if (pixels.GetLength(0) == 0 || pixels.GetLength(1) == 0) return;

            if (pos.X < 0) pos.X = 0;
            if (pos.Y < 0) pos.Y = 0;
            if (pos.X >= pixels.GetLength(0)) pos.X = pixels.GetLength(0) - 1;
            if (pos.Y >= pixels.GetLength(1)) pos.Y = pixels.GetLength(1) - 1;

            HuePosition = pos;
        }

        private void PickColor(Point pos)
        {
            var bmp = _dialBitmap;
            if (bmp == null) return;

            var pixels = BitmapUtils.GetPixels(bmp);
            if (pixels == null) return;

            if (pixels.GetLength(0) == 0 || pixels.GetLength(1) == 0) return;

            if (pos.X < 0) pos.X = 0;
            if (pos.Y < 0) pos.Y = 0;
            if (pos.X >= pixels.GetLength(0)) pos.X = pixels.GetLength(0) - 1;
            if (pos.Y >= pixels.GetLength(1)) pos.Y = pixels.GetLength(1) - 1;

            DialPosition = pos;

            var pixel = pixels[(int)Math.Floor(pos.X), (int)Math.Floor(pos.Y)];
            var color = System.Windows.Media.Color.FromRgb(pixel.Red, pixel.Green, pixel.Blue);
            Color = ColorUtils.ColorToHexString(color);
        }

        private void LoadFromColor()
        {
            var hex = Color;
            var color = ColorUtils.HexStringToNullableColor(hex) ?? Colors.Black;

            ResetHue();
            HuePosition = FindHuePosition(color);
            PickHue(HuePosition);
            ResetDial();
            DialPosition = FindDialPosition(color);
        }
        
        private void ResetHue()
        {
            _hueImage = null;
            RaisePropertyChanged(() => HueImage);
        }

        private void ResetDial()
        {
            _dialImage = null;
            RaisePropertyChanged(() => DialImage);
        }

        private bool PointIsInHueView(Point pt)
        {
            return HueView != null && HueView.ActualHeight > 0 && HueView.ActualWidth > 0 &&
                   pt.Y >= 0 && pt.Y < HueView.ActualHeight && pt.X >= 0 && pt.X < HueView.ActualWidth;
        }

        private bool PointIsInDialView(Point pt)
        {
            return DialView != null && DialView.ActualHeight > 0 && DialView.ActualWidth > 0 &&
                   pt.Y >= 0 && pt.Y < DialView.ActualHeight && pt.X >= 0 && pt.X < DialView.ActualWidth;
        }

        private Point FindHuePosition(Color color)
        {
            var res = new Point(0, 0);

            var bmp = _hueBitmap;
            if (bmp == null) return res;

            var pixels = BitmapUtils.GetPixels(bmp);
            if (pixels == null) return res;

            if (pixels.GetLength(0) == 0 || pixels.GetLength(1) == 0) return res;
            
            var hsv0 = ColorUtils.RgbToHsv(color.R, color.G, color.B);

            var y = pixels.GetLength(1);

            const int x = 0;
            var delta = 0.0001d;
            var found = false;
            while (delta < 1000)
            {
                for (var y0 = 0; y0 < pixels.GetLength(1); y0++)
                {
                    var c = pixels[x, y0];
                    var hsv1 = ColorUtils.RgbToHsv(c.Red, c.Green, c.Blue);

                    if (Math.Abs(hsv1.h - hsv0.h) < delta)
                    {
                        y = y0;
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    break;
                }

                delta = delta * 10;
            }

            return new Point(x, y);
        }

        private Point FindDialPosition(Color color)
        {
            var res = new Point(0, 0);

            var bmp = _dialBitmap;
            if (bmp == null) return res;

            var pixels = BitmapUtils.GetPixels(bmp);
            if (pixels == null) return res;

            if (pixels.GetLength(0) == 0 || pixels.GetLength(1) == 0) return res;

            var hsv0 = ColorUtils.RgbToHsv(color.R, color.G, color.B);

            var x = pixels.GetLength(0)-1;
            var y = pixels.GetLength(1)-1;
            
            var delta = 0.0000001d;
            var found = false;
            while (delta < 1000)
            {
                for (var x0 = 0; x0 < pixels.GetLength(0); x0++)
                {
                    for (var y0 = 0; y0 < pixels.GetLength(1); y0++)
                    {
                        var c = pixels[x0, y0];
                        var hsv1 = ColorUtils.RgbToHsv(c.Red, c.Green, c.Blue);

                        if (Math.Abs(hsv1.s - hsv0.s) < delta && Math.Abs(hsv1.v - hsv0.v) < delta)
                        {
                            x = x0;
                            y = y0;
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                {
                    break;
                }

                delta = delta * 10;
            }

            return new Point(x, y);
        }

        private Color GetHueColor(Point position)
        {
            var bmp = _hueBitmap;
            if (bmp == null) return Colors.Red;

            var pixels = BitmapUtils.GetPixels(bmp);
            if (pixels == null) return Colors.Red;

            var pixel = pixels[(int) Math.Floor(position.X), (int) Math.Floor(position.Y)];
            var color = System.Windows.Media.Color.FromRgb(pixel.Red, pixel.Green, pixel.Blue);

            return color;
        }
        
        private RelayCommand _viewLoadedCmd;
        public RelayCommand ViewLoadedCmd => _viewLoadedCmd ?? (_viewLoadedCmd = new RelayCommand(() =>
        {
            _originalColor = _color;
            Color = _color;
            LoadFromColor();
        }));

        private RelayCommand<RoutedEventArgs> _dialViewLoadedCmd;
        public RelayCommand<RoutedEventArgs> DialViewLoadedCmd => _dialViewLoadedCmd ?? (_dialViewLoadedCmd = new RelayCommand<RoutedEventArgs>(args =>
        {
            DialView = args.OriginalSource as FrameworkElement;
            ResetDial();
        }));

        private RelayCommand<RoutedEventArgs> _hueViewLoadedCmd;
        public RelayCommand<RoutedEventArgs> HueViewLoadedCmd => _hueViewLoadedCmd ?? (_hueViewLoadedCmd = new RelayCommand<RoutedEventArgs>(args =>
        {
            HueView = args.OriginalSource as FrameworkElement;
            ResetHue();
        }));
        
        private RelayCommand<MouseButtonEventArgs> _mouseDownCmd;
        public RelayCommand<MouseButtonEventArgs> MouseDownCmd => _mouseDownCmd ?? (_mouseDownCmd = new RelayCommand<MouseButtonEventArgs>(args =>
        {
            args.Handled = true;

            var huePos = args.GetPosition(HueView);
            var dialPos = args.GetPosition(DialView);

            if (PointIsInHueView(huePos))
            {
                _isPickingHue = true;
                PickHue(huePos);
                ResetDial();
                PickColor(DialPosition);
            }
            else if (PointIsInDialView(dialPos))
            {
                _isPickingColor = true;
                DialPosition = dialPos;
                PickColor(dialPos);
            }
        }));

        private RelayCommand<MouseEventArgs> _mouseMoveCmd;
        public RelayCommand<MouseEventArgs> MouseMoveCmd => _mouseMoveCmd ?? (_mouseMoveCmd = new RelayCommand<MouseEventArgs>(args =>
        {
            args.Handled = true;

            if (_isPickingHue && HueView != null && HueView.ActualWidth > 0 && HueView.ActualHeight > 0)
            {
                var pos = args.GetPosition(HueView);
                PickHue(pos);
                ResetDial();
                PickColor(DialPosition);
            }
            else if (_isPickingColor && DialView != null && DialView.ActualWidth > 0 && DialView.ActualHeight > 0)
            {
                var pos = args.GetPosition(DialView);
                PickColor(pos);
            }
        }));

        private RelayCommand<MouseButtonEventArgs> _mouseUpCmd;
        public RelayCommand<MouseButtonEventArgs> MouseUpCmd => _mouseUpCmd ?? (_mouseUpCmd = new RelayCommand<MouseButtonEventArgs>(args =>
        {
            args.Handled = true;

            _isPickingHue = false;
            _isPickingColor = false;
        }));

        private RelayCommand<RoutedEventArgs> _inputChangedCmd;
        public RelayCommand<RoutedEventArgs> InputChangedCmd => _inputChangedCmd ?? (_inputChangedCmd = new RelayCommand<RoutedEventArgs>(args =>
        {
            var hex = Color;
            var color = ColorUtils.HexStringToNullableColor(hex);
            if (color != null)
            {
                LoadFromColor();
            }
        }));

        private RelayCommand _submitCmd;
        public RelayCommand SubmitCmd => _submitCmd ?? (_submitCmd = new RelayCommand(() =>
        {
            OnColorPicked?.Invoke(Color);
            IsOpen = false;
        }));

        private RelayCommand _resetCmd;
        public RelayCommand ResetCmd => _resetCmd ?? (_resetCmd = new RelayCommand(() =>
        {
            Color = _originalColor;
            LoadFromColor();
        }));

        private RelayCommand _cancelCmd;
        public RelayCommand CancelCmd => _cancelCmd ?? (_cancelCmd = new RelayCommand(() =>
        {
            Color = _originalColor;
            OnCancelled?.Invoke();
            IsOpen = false;
        }));

    }
}
