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

namespace NLog.Targets.InfluxDB.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Some Info");
        }

        private void Debug_Click(object sender, RoutedEventArgs e)
        {
            Logger.Debug("Some Debug");
        }

        private void Warn_Click(object sender, RoutedEventArgs e)
        {
            Logger.Warn("Warning!!");
        }

        private void Error_Click(object sender, RoutedEventArgs e)
        {
            Logger.Error("Error!!");
        }
    }
}
