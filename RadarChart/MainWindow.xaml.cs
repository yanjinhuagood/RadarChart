using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RadarChart
{
    public class Model : Notify
    {
        protected override Freezable CreateInstanceCore()
        {
            return new Model() { };
        }
        private System.Collections.ObjectModel.ObservableCollection<RadarItem> radarItems =
            new System.Collections.ObjectModel.ObservableCollection<RadarItem>() { };

        public System.Collections.ObjectModel.ObservableCollection<RadarItem> Items
        {
            get { return radarItems; }
            set
            {
                radarItems = value;
                OnChanged();
            }
        }
        FreezableCollection<RadarSize> size = new FreezableCollection<RadarSize>();

        public FreezableCollection<RadarSize> Size
        {
            get { return size; }
            set
            {
                size = value;
                OnChanged();
            }
        }

    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        Model model = new Model();
        public MainWindow()
        {
            this.DataContext = model;
            InitializeComponent();

        }
        Random random = new Random();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            model.Items.Add(new RadarItem()
            {
                Width = 15,
                Height = 15,
                Color = new SolidColorBrush(Colors.Red),
                Left = random.Next(0, (int)(r1.Width)),
                Top = random.Next(0, (int)(r1.Height))
            });
        }
    }
}
