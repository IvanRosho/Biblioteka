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
using System.Windows.Shapes;

namespace Biblioteka
{
    /// <summary>
    /// Логика взаимодействия для ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        MainWindow Main;
        public ConfigWindow(MainWindow w)
        {
            InitializeComponent();
            Main = w;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            if (Main.OpenConnect() == false) return;
            this.Close();
        }

        private void ConfigWindow1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Properties.Settings.Default.Save();
            //base.OnClosing(e);
        }
    }
}
