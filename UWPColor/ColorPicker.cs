using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace UWPColor
{
    public class ColorPicker : Grid
    {
        private readonly Rectangle _actColorElement;
        private readonly Polygon _fleche;
        private Rectangle _spectreChoice;
        private Color _actSpectre;
        private SolidColorBrush _actColor;
        private readonly Grid _choiceGrid;
        private Canvas _pickerCanvas;
        private Grid _gridEllipse;
        private double _x;
        private double _y;
        private bool _spectrChanged;

        public static readonly DependencyProperty ActualColorProperty = DependencyProperty.Register(
            "ActualColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(default(Color), ActualColorChangedCallback));

        private static void ActualColorChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var test = dependencyObject as ColorPicker;
            test?.UpdateActualColor();
        }

        private void UpdateActualColor()
        {
            double[] hsl = RgbToHsl(ActualColor);
            Color v = HslToRgb(hsl[0], 1, 0.5);
            _choiceGrid.Background = new SolidColorBrush(v);
            if (!_spectrChanged)
            {
                _actSpectre = v;
            }
            _spectrChanged = false;
            _actColor = new SolidColorBrush(ActualColor);
            _actColorElement.Fill = _actColor;
        }

        public Color ActualColor
        {
            get { return (Color)GetValue(ActualColorProperty); }
            set { SetValue(ActualColorProperty, value); }
        }

        public delegate void ActualColorEvent(Color newColor);
        public event ActualColorEvent ActualColorChanged;

        private void DefGrid()
        {
            var a = new ColumnDefinition { Width = new GridLength(150, GridUnitType.Star) };
            ColumnDefinitions.Add(a);
            a = new ColumnDefinition { Width = new GridLength(16) };
            ColumnDefinitions.Add(a);
            a = new ColumnDefinition { Width = new GridLength(10) };
            ColumnDefinitions.Add(a);
            var b = new RowDefinition { Height = new GridLength(30, GridUnitType.Pixel) };
            RowDefinitions.Add(b);
            b = new RowDefinition { Height = new GridLength(8, GridUnitType.Pixel) };
            RowDefinitions.Add(b);
            b = new RowDefinition { Height = new GridLength(150, GridUnitType.Star) };
            RowDefinitions.Add(b);
        }

        private void DefSpectre()
        {
            _spectreChoice = new Rectangle
            {
                Margin = new Thickness(1, 0, 1, 0),
                Fill = new LinearGradientBrush
                {
                    GradientStops = {
                        new GradientStop
                        {
                            Offset = 0,
                            Color = Color.FromArgb(255, 255, 0, 0)
                        },
                        new GradientStop
                        {
                            Offset = 0.2,
                            Color = Color.FromArgb(255, 255, 255, 0)
                        },
                        new GradientStop
                        {
                            Offset = 0.4,
                            Color = Color.FromArgb(255, 0, 255, 0)
                        },
                        new GradientStop
                        {
                            Offset = 0.6,
                            Color = Color.FromArgb(255, 0, 0, 255)
                        },
                        new GradientStop
                        {
                            Offset = 0.8,
                            Color = Color.FromArgb(255, 255, 0, 255)
                        },
                        new GradientStop
                        {
                            Offset = 1,
                            Color = Color.FromArgb(255, 255, 0, 0)
                        }
                    },
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1)
                }
            };
            _spectreChoice.PointerPressed += (sender, args) =>
            {
                SpectreChoiceOnPointerPressed(sender, args);
                PointerMoved += SpectreChoiceOnPointerPressed;
            };
            PointerReleased += (sender, args) => PointerMoved -= SpectreChoiceOnPointerPressed;
            SetColumn(_spectreChoice, 1);
            SetRow(_spectreChoice, 2);

        }

        private void SpectreChoiceOnPointerPressed(object sender, PointerRoutedEventArgs args)
        {

            var t = _spectreChoice.ActualHeight;
            var x = args.GetCurrentPoint(_spectreChoice).Position.Y;
            //_fleche
            x = x <= 0 ? 0 : x;
            Canvas.SetTop(_fleche, x);
            _actSpectre = HslToRgb(x / t * 360f, 1, 0.5);
            _spectrChanged = true;
            ActualColor = RecalculerCouleur();
            ActualColorChanged?.Invoke(ActualColor);
            _actColorElement.Fill = new SolidColorBrush(ActualColor);
            _choiceGrid.Background = new SolidColorBrush(_actSpectre);

        }

        private Color RecalculerCouleur()
        {
            var eheight = _choiceGrid.ActualHeight;
            var ewidth = _choiceGrid.ActualWidth;
            var width = Math.Max(_x, 0);
            var height = Math.Max(_y, 0);
            height = height < eheight ? height : eheight;
            width = width < ewidth ? width : ewidth;
            var ratiox = 1 - width / ewidth;
            var ratioy = 1 - height / eheight;
            var newr = (byte)((_actSpectre.R + (255 - _actSpectre.R) * ratiox) * ratioy);
            var newb = (byte)((_actSpectre.B + (255 - _actSpectre.B) * ratiox) * ratioy);
            var newg = (byte)((_actSpectre.G + (255 - _actSpectre.G) * ratiox) * ratioy);
            return Color.FromArgb(255, newr, newg, newb);
        }

        private void DefPickerCanvas()
        {
            _pickerCanvas = new Canvas
            {
                Background = new SolidColorBrush(Colors.Transparent)
            };
            _gridEllipse = new Grid
            {
                Margin = new Thickness(-7, -7, 0, 0)
            };
            Canvas.SetTop(_gridEllipse, _y);
            Canvas.SetLeft(_gridEllipse, _y);
            _gridEllipse.Children.Add(new Ellipse
            {
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 3,
                Width = 14,
                Height = 14,
                UseLayoutRounding = false
            });
            _gridEllipse.Children.Add(new Ellipse
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                Width = 12,
                Height = 12,
                UseLayoutRounding = false
            });
            _pickerCanvas.Children.Add(_gridEllipse);
        }

        public ColorPicker()
        {
            _actColor = new SolidColorBrush(_actSpectre);
            DefPickerCanvas();
            DefSpectre();
            _actColorElement = new Rectangle
            {
                Fill = _actColor,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1

            };
            _choiceGrid = new Grid
            {
                Background = _actColor
            };
            _choiceGrid.PointerPressed += (sender, args) =>
            {
                UpdatingColor(sender, args);
                PointerMoved += UpdatingColor;
            };
            _choiceGrid.SizeChanged += ColorPicker_SizeChanged;
            _choiceGrid.Children.Add(_pickerCanvas);
            PointerReleased += (sender, args) => PointerMoved -= UpdatingColor;
            _choiceGrid.Children.Add(new Rectangle
            {
                Fill = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 0),
                    GradientStops =
                    {
                        new GradientStop
                        {
                            Offset = 0,
                            Color = Colors.White
                        },
                        new GradientStop
                        {
                            Offset = 1,
                            Color = Color.FromArgb(0,255,255,255)
                        }
                    }
                }
            });
            _choiceGrid.Children.Add(new Rectangle
            {
                Fill = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1),
                    GradientStops =
                    {
                        new GradientStop
                        {
                            Offset = 0,
                            Color = Color.FromArgb(0, 0, 0, 0)
                        },
                        new GradientStop
                        {
                            Offset = 1,
                            Color = Colors.Black
                        }
                    }
                }
            });

            SetColumn(_choiceGrid, 0);
            SetRow(_choiceGrid, 2);

            Canvas flecheCanvas = new Canvas();
            flecheCanvas.SizeChanged += FlecheCanvasSizeChanged;
            _fleche = new Polygon
            {
                Points = { new Point(8, -3), new Point(0, 0), new Point(8, 3) },
                Fill = new SolidColorBrush(Colors.White),
            };
            flecheCanvas.Children.Add(_fleche);
            SetColumn(_actColorElement, 0);
            SetRow(_actColorElement, 0);
            SetColumn(flecheCanvas, 2);
            SetRow(flecheCanvas, 2);

            _actColorElement.VerticalAlignment = VerticalAlignment.Stretch;
            _actColorElement.HorizontalAlignment = HorizontalAlignment.Stretch;
            Children.Add(_actColorElement);
            Children.Add(flecheCanvas);
            Children.Add(_spectreChoice);
            Children.Add(_choiceGrid);
            DefGrid();
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            _choiceGrid.Loaded += _choiceGrid_Loaded;
            _spectreChoice.Loaded += _spectreChoice_Loaded;
            ActualColor = Colors.Red;
        }

        private void _spectreChoice_Loaded(object sender, RoutedEventArgs e)
        {
            double h = RgbToHsl(ActualColor)[0];
            Canvas.SetTop(_fleche, (h / 360f) * _spectreChoice.ActualHeight);
        }

        private void _choiceGrid_Loaded(object sender, RoutedEventArgs e)
        {
            double[] hsl = RgbToHsl(ActualColor);
            UpdatePosition(_choiceGrid.ActualHeight * (1 - 2 * hsl[2]), _choiceGrid.ActualWidth * hsl[1]);
        }

        private void FlecheCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var i = Canvas.GetTop(_fleche);
            var j = i / e.PreviousSize.Height;
            if (double.IsNaN(j))
                j = 0;
            Canvas.SetTop(_fleche, j * e.NewSize.Height);
        }

        private void ColorPicker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var i = Canvas.GetTop(_gridEllipse);
            var j = Canvas.GetLeft(_gridEllipse);
            var k = i / e.PreviousSize.Height;
            var l = j / e.PreviousSize.Width;
            if (double.IsNaN(k))
                k = 0;
            if (double.IsNaN(l))
                l = 0;
            Canvas.SetTop(_gridEllipse, k * e.NewSize.Height);
            Canvas.SetLeft(_gridEllipse, l * e.NewSize.Width);
        }

        private void UpdatingColor(object sender, PointerRoutedEventArgs args)
        {
            var eheight = _choiceGrid.ActualHeight;
            var ewidth = _choiceGrid.ActualWidth;
            var i = args.GetCurrentPoint(_choiceGrid).Position;
            _x = i.X;
            _y = i.Y;
            var width = Math.Max(_x, 0);
            var height = Math.Max(_y, 0);
            height = height < eheight ? height : eheight;
            width = width < ewidth ? width : ewidth;
            UpdatePosition(height, width);
            var ratiox = 1 - width / ewidth;
            var ratioy = 1 - height / eheight;
            var newr = (byte)((_actSpectre.R + (255 - _actSpectre.R) * ratiox) * ratioy);
            var newb = (byte)((_actSpectre.B + (255 - _actSpectre.B) * ratiox) * ratioy);
            var newg = (byte)((_actSpectre.G + (255 - _actSpectre.G) * ratiox) * ratioy);
            var actColor = Color.FromArgb(255, newr, newg, newb);
            _actColor = new SolidColorBrush(actColor);
            _actColorElement.Fill = _actColor;
            ActualColor = actColor;
            ActualColorChanged?.Invoke(actColor);
        }

        public static Color HslToRgb(double h, double s, double l)
        {
            var c = (1 - Math.Abs(2 * l - 1)) * s;
            var x = c * (1 - Math.Abs(h / 60 % 2 - 1));
            var m = l - c / 2;
            double r, g, b;
            if (h < 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (h < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (h < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (h < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (h < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else
            {
                r = c;
                g = 0;
                b = x;
            }
            return Color.FromArgb(255, (byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b + m) * 255));
        }

        public static double[] RgbToHsl(double r, double g, double b)
        {
            double m1 = Math.Max(Math.Max(r, g), b);
            double m2 = Math.Min(Math.Min(r, g), b);
            double c = m1 - m2;
            double h2;
            if (c == 0)
            {
                h2 = 0;
            }
            else if (m1 == r)
            {
                h2 = ((g - b) / c + 6) % 6;
            }
            else if (m1 == g)
            {
                h2 = (b - r) / c + 2;
            }
            else
            {
                h2 = (r - g) / c + 4;
            }
            double h = 60f * h2;
            double l = 0.5f * (m1 + m2);
            double s = l == 1 ? 0 : c / m1;
            return new[] { h, s, l / 255f };

        }

        public static double[] RgbToHsl(Color value)
        {
            return RgbToHsl(value.R, value.G, value.B);
        }

        private void UpdatePosition(double h, double w)
        {
            _x = w;
            _y = h;
            Canvas.SetTop(_gridEllipse, h);
            Canvas.SetLeft(_gridEllipse, w);
        }
    }
}