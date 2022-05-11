using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace RadarChart
{
    public abstract class Notify : Animatable, System.ComponentModel.INotifyPropertyChanged
    {

        #region INotifyPropertyChanged Members
        public void OnChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
    public class RadarLineSize : Notify
    {
        private Point _start;

        public Point Start
        {
            get { return _start; }
            set
            {
                _start = value;
                OnChanged();
            }
        }

        private Point _end;

        public Point End
        {
            get { return _end; }
            set
            {
                _end = value;
                OnChanged();
            }
        }
        private Brush _color;

        public Brush Color
        {
            get { return _color; }
            set
            {
                _color = value;
                OnChanged();
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return (Freezable)Activator.CreateInstance(this.GetType());
        }

    }
    public class RadarSize : Notify
    {
        public RadarSize()
        {

        }
        private double _width;

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnChanged();
            }
        }
        private double _height;

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnChanged();
            }
        }
        private Brush _color;

        public Brush Color
        {
            get { return _color; }
            set
            {
                _color = value;
                OnChanged();
            }
        }
        private double _strokeThickness = 1d;

        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                _strokeThickness = value;
                OnChanged();
            }
        }

        private DoubleCollection _strokeDashArray = new DoubleCollection(0);

        public DoubleCollection StrokeDashArray
        {
            get { return _strokeDashArray; }
            set
            {
                _strokeDashArray = value;
                OnChanged();
            }
        }
        protected override Freezable CreateInstanceCore()
        {
            return (Freezable)Activator.CreateInstance(this.GetType());
        }
    }

    /// <summary>
    /// RadarChart子项 
    /// </summary>
    public class RadarItem : Control
    {

        static RadarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadarItem), new FrameworkPropertyMetadata(typeof(RadarItem)));
        }
        public RadarItem()
        {

        }

        /// <summary>
        /// 转弧度
        /// </summary>
        /// <param name="val">角度</param>
        /// <returns>弧度制</returns>
        double Rad(double val)
        {
            return val * Math.PI / 180;
        }
        /// <summary>
        /// 线性缩放
        /// </summary>
        /// <param name="size">半径</param>
        internal void LineScar(double size)
        {
            var midpoint = new Vector(size, size);
            var vp = new Vector(Left, Top);
            var sub = vp - midpoint;
            var angle = Vector.AngleBetween(sub, new Vector(size, 1));
            angle = angle > 0 ? angle : angle + 360;
            //距离大于半径,根据半径重新绘制
            if (sub.Length >= size)
            {
                Top = size - size * Math.Sin(Rad(angle)) - Width / 2;
                Left = size + size * Math.Cos(Rad(angle)) - Width / 2;
            }
        }

        /// <summary>
        /// 顶部距离,用canvas.top绘制
        /// </summary>
        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        /// <summary>
        /// 顶部距离,用canvas.top绘制
        /// </summary>
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(RadarItem), new PropertyMetadata(0.0));


        /// <summary>
        /// 左侧距离,用于canvas.left绘制
        /// </summary>
        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        /// <summary>
        /// 左侧距离,用于canvas.left绘制
        /// </summary>
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(double), typeof(RadarItem), new PropertyMetadata(0.0));


        /// <summary>
        /// 填充颜色
        /// </summary>
        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        /// <summary>
        /// 填充颜色
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(RadarItem), 
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4166D7"))));
    }
    /// <summary>
    /// RadarChart
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(RadarItem))]
    public class Radar : ItemsControl
    {
        static Radar()
        {
            //添加此项，获取xaml资源
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Radar),
                new System.Windows.FrameworkPropertyMetadata(typeof(Radar)));
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public Radar()
        {
            //添加数据
            SetValue(RadarLineProperty, new FreezableCollection<RadarLineSize>());
            SetValue(RadarCircleProperty, new FreezableCollection<RadarSize>());
            SetValue(CircelColorProperty, new FreezableCollection<Brush>());
        }

        /// <summary>
        /// RadarChart扫描的颜色
        /// </summary>
        public Brush RadarColor
        {
            get { return (Brush)GetValue(RadarColorProperty); }
            set { SetValue(RadarColorProperty, value); }
        }

        /// <summary>
        /// RadarChart扫描的颜色
        /// </summary>
        public static readonly DependencyProperty RadarColorProperty =
            DependencyProperty.Register("RadarColor", typeof(Brush), typeof(Radar));

        /// <summary>
        /// 是否使用count作为圈数标记,和RadarChart子项冲突
        /// </summary>
        public bool UseCount
        {
            get { return (bool)GetValue(UseCountProperty); }
            set { SetValue(UseCountProperty, value); }
        }

        /// <summary>
        /// 是否使用count作为圈数标记,和RadarChart子项冲突
        /// </summary>
        public static readonly DependencyProperty UseCountProperty =
            DependencyProperty.Register("UseCount", typeof(bool), typeof(Radar), new PropertyMetadata(true, new PropertyChangedCallback(OnUserCountValueChanged)));

        private static void OnUserCountValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //重绘,可能无用
            ReSize(d);
        }


        /// <summary>
        /// 是否统一颜色,当存在冲突时,使用默认颜色
        /// </summary>
        public bool UnionColor
        {
            get { return (bool)GetValue(UnionColorProperty); }
            set { SetValue(UnionColorProperty, value); }
        }
        /// <summary>
        /// 是否统一颜色,当存在冲突时,使用默认颜色
        /// </summary>
        public static readonly DependencyProperty UnionColorProperty =
            DependencyProperty.Register("UnionColor", typeof(bool), typeof(Radar), new PropertyMetadata(true, new PropertyChangedCallback(OnUnionColorValueChanged)));

        private static void OnUnionColorValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ReSize(d);
        }
        /// <summary>
        /// 每圈的颜色设置,可以设置一个,但是需要使用unioncolor.可以多个颜色配合每圈.
        /// </summary>
        public FreezableCollection<Brush> CircelColor
        {
            get { return (FreezableCollection<Brush>)GetValue(CircelColorProperty); }
            set { SetValue(CircelColorProperty, value); }
        }

        /// <summary>
        /// 每圈的颜色设置,可以设置一个,但是需要使用unioncolor.可以多个颜色配合每圈.
        /// </summary>
        public static readonly DependencyProperty CircelColorProperty =
            DependencyProperty.Register("CircelColor", typeof(FreezableCollection<Brush>), typeof(Radar), new PropertyMetadata(new PropertyChangedCallback(OnColorValueChanged)));

        private static void OnColorValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ReSize(d);
        }
        /// <summary>
        /// 每圈的大小
        /// </summary>
        public FreezableCollection<RadarSize> RadarCircle
        {
            get { return (FreezableCollection<RadarSize>)GetValue(RadarCircleProperty); }
            set { SetValue(RadarCircleProperty, value); }
        }

        /// <summary>
        /// 每圈的大小
        /// </summary>
        public static readonly DependencyProperty RadarCircleProperty =
            DependencyProperty.Register("RadarCircle", typeof(FreezableCollection<RadarSize>), typeof(Radar), new PropertyMetadata(new PropertyChangedCallback(OnRadarCircelValueChanged)));

        private static void OnRadarCircelValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ReSize(d);
        }


        /// <summary>
        /// RadarChart的分割线,目前固定为6,可以自行修改
        /// </summary>
        public FreezableCollection<RadarLineSize> RadarLine
        {
            get { return (FreezableCollection<RadarLineSize>)GetValue(RadarLineProperty); }
            set { SetValue(RadarLineProperty, value); }
        }

        /// <summary>
        /// RadarChart的分割线,目前固定为6,可以自行修改
        /// </summary>
        public static readonly DependencyProperty RadarLineProperty =
            DependencyProperty.Register("RadarLine", typeof(FreezableCollection<RadarLineSize>), typeof(Radar));



        /// <summary>
        /// RadarChart的圈数,默认为5
        /// </summary>
        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }
        /// <summary>
        /// RadarChart圈数
        /// </summary>
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(Radar), new PropertyMetadata(5, new PropertyChangedCallback(OnCountValueChanged)));

        private static void OnCountValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ReSize(d);
        }

        /// <summary>
        /// RadarChart分割线的颜色
        /// </summary>
        public Brush RadarLineColor
        {
            get { return (Brush)GetValue(RadarLineColorProperty); }
            set { SetValue(RadarLineColorProperty, value); }
        }

        /// <summary>
        /// RadarChart分割线的颜色
        /// </summary>
        public static readonly DependencyProperty RadarLineColorProperty =
            DependencyProperty.Register("RadarLineColor", typeof(Brush), typeof(Radar), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnRadarLineColorChanged)));

        private static void OnRadarLineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ReSize(d);
        }


        /// <summary>
        /// RadarChart半径
        /// </summary>
        public Double RadarRadius
        {
            get { return (Double)GetValue(RadarRadiusProperty); }
            set { SetValue(RadarRadiusProperty, value); }
        }

        /// <summary>
        /// RadarChart半径
        /// </summary>
        public static readonly DependencyProperty RadarRadiusProperty =
            DependencyProperty.Register("RadarRadius", typeof(Double), typeof(Radar), new PropertyMetadata(0.0));

        /// <summary>
        /// RadarChart全尺寸
        /// </summary>
        public double RadarFillWidth
        {
            get { return (double)GetValue(RadarFillWidthProperty); }
            set { SetValue(RadarFillWidthProperty, value); }
        }

        /// <summary>
        /// RadarChart全尺寸
        /// </summary>
        public static readonly DependencyProperty RadarFillWidthProperty =
            DependencyProperty.Register("RadarFillWidth", typeof(double), typeof(Radar), new PropertyMetadata(0.0));


        /// <summary>
        /// 是否播放动画
        /// </summary>
        public bool Play
        {
            get { return (bool)GetValue(PlayProperty); }
            set { SetValue(PlayProperty, value); }
        }

        /// <summary>
        /// 是否播放动画
        /// </summary>
        public static readonly DependencyProperty PlayProperty =
            DependencyProperty.Register("Play", typeof(bool), typeof(Radar), new PropertyMetadata(false));

        /// <summary>
        /// 重新绘制
        /// </summary>
        private static void ReSize(DependencyObject d)
        {
            var ele = (Radar)d;
            //布局无效
            ele.InvalidateVisual();
        }
        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="val">角度</param>
        /// <returns>弧度制</returns>
        private double Rad(double val)
        {
            return val * Math.PI / 180;
        }
        /// <summary>
        /// 重新包装,此时用作重新更改子项存在与圆内
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var ele = element as RadarItem;
            if (ActualWidth == 0)
            {
                //0时抛弃
                return;
            }
            //缩小范围
            ele.LineScar(RadarRadius);
            base.PrepareContainerForItemOverride(element, item);
        }
        /// <summary>
        /// 绘制布局
        /// </summary>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);
            var h = size.Height / Count;
            var w = size.Width / Count;
            var circlesize = h > w ? w : h;
            //是否强制圈数
            if (UseCount)
            {
                RadarCircle.Clear();
                RadarLine.Clear();
                for (int i = 1; i <= Count; i++)
                {
                    RadarSize radar = new RadarSize();
                    if (i == 1)
                        radar.Width = 25;
                    else
                        radar.Width = circlesize * i;
                    if(i == 1)
                        radar.Height = 25;
                    else
                        radar.Height = circlesize * i;
                   
                    if ( i == Count - 2)
                        radar.StrokeThickness = 2;
                    if (i == Count - 1)
                        radar.StrokeDashArray = new DoubleCollection() { 7};
                    Brush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4166D7"));
                    //是否强制统一颜色
                    if (UnionColor)
                    {
                        if (CircelColor.Count > 0)
                        {
                            brush = CircelColor[0];
                        }
                    }
                    else
                    {
                        if (i <= CircelColor.Count)
                        {
                            brush = CircelColor[i - 1];
                        }
                    }
                    radar.Color = brush;
                    brush.Freeze();
                    RadarCircle.Add(radar);
                }
            }
            //绘制分割线
            var angle = 180.0 / 4;
            circlesize = size.Height > size.Width ? size.Width : size.Height;
            circlesize = circlesize / 1.5;
            RadarFillWidth = circlesize;
            var midx = circlesize / 2.0;
            var midy = circlesize / 2.0;
            circlesize = circlesize / 2;
            RadarRadius = circlesize;

            for (int i = 0; i < 4; i++)
            {
                var baseangel = angle * i;
                var l1 = new Point(midx + circlesize * Math.Cos(Rad(baseangel)), midy - circlesize * Math.Sin(Rad(baseangel)));
                var half = baseangel + 180;
                var l2 = new Point(midx + circlesize * Math.Cos(Rad(half)), midy - circlesize * Math.Sin(Rad(half)));
                RadarLineSize radarLine = new RadarLineSize();
                radarLine.Start = l1;
                radarLine.End = l2;
                radarLine.Color = RadarLineColor;
                RadarLine.Add(radarLine);
            }
            return size;
        }

        /// <summary>
        /// 是否自动扫描
        /// </summary>
        public bool AutoScan
        {
            get { return (bool)GetValue(AutoScanProperty); }
            set { SetValue(AutoScanProperty, value); }
        }


        public static readonly DependencyProperty AutoScanProperty =
            DependencyProperty.Register("AutoScan", typeof(bool), typeof(Radar), new PropertyMetadata(false, new PropertyChangedCallback(OnAutoScanValueChanged)));

        private static ObservableCollection<RadarItem> radarItems = new ObservableCollection<RadarItem>();

        private static Random rnd = new Random();

        private static DispatcherTimer dt = new DispatcherTimer();

        private static void AutoPlay(Radar radar, bool val)
        {
            if (val)
            {
                radar.ItemsSource = radarItems;
                dt.Interval = TimeSpan.FromMilliseconds(1000);
                dt.Tick += Dt_Tick;
                dt.Start();
            }
            else
            {
                dt.Stop();
                radar.ItemsSource = null;
                radarItems.Clear();
            }
        }
        private static Radar autoplayradar;
        private static void Dt_Tick(object sender, EventArgs e)
        {
            radarItems.Add(new RadarItem()
            {
                Width = 10,
                Height = 10,
                Left = rnd.Next(0, (int)autoplayradar.RadarFillWidth),
                Top = rnd.Next(0, (int)autoplayradar.RadarFillWidth),
            });
            if (radarItems.Count > 5)
            {
                radarItems.RemoveAt(0);
            }
        }

        private static void OnAutoScanValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var val = (bool)e.NewValue;
            var ele = d as Radar;
            autoplayradar = ele;
            ele.Play = val;
            AutoPlay(ele, val);
        }
    }
}
