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
    /// Логика взаимодействия для AddContentTag.xaml
    /// </summary>
    public partial class AddContentTag : Window
    {
        MainWindow Main;
        public AddContentTag(MainWindow w)
        {
            InitializeComponent(); Main = w;
            this.AuthorList.ItemsSource = Main.Tags;
            ButtonAdd.Content = "Искать";
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
                if (AuthorList.SelectedIndex != -1)
                    Main.SearchTag(AuthorList.SelectedItem as FIO);
                else return;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AuthorText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                TextChanged();
            }
            else ButtonAdd_Click(this, e);
        }

        private void AuthorText_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged();
        }

        private void TextChanged()
        {
            if (Main == null) return;
            if (TagText.Text != "")
            {
                var temp = (Main.Tags);
                var ttemp = temp.Where(t => t.Familiya.ToLower().Contains(TagText.Text.ToLower()) == true);
                this.AuthorList.ItemsSource = ttemp;
            }
            else this.AuthorList.ItemsSource = Main.Tags;
        }
    }
}
