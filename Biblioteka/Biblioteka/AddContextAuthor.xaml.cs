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
using System.Collections.ObjectModel;
using System.Data;

namespace Biblioteka
{
    /// <summary>
    /// Логика взаимодействия для AddContextAuthor.xaml
    /// </summary>
    public partial class AddContextAuthor : Window
    {
        CardWindow CardWindow;
        MainWindow Main;
        public AddContextAuthor(CardWindow w)
        {
            InitializeComponent();
            CardWindow = w;
            this.AuthorList.ItemsSource = CardWindow.Main.Authors;
        }

        public AddContextAuthor(MainWindow w)
        {
            InitializeComponent();
            Main = w;
            this.AuthorList.ItemsSource = Main.Authors;
            ButtonAdd.Content = "Искать";
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (CardWindow != null)
            {
                if (AuthorList.SelectedIndex == -1)
                {
                    var a = AuthorText.Text.Split(' ');
                    switch (a.Length)
                    {
                        case 0:
                            { break; }
                        case 1:
                            { CardWindow.AddAuthorToCard(new FIO(-1, a[0], "", "")); break; }
                        case 2:
                            { CardWindow.AddAuthorToCard(new FIO(-1, a[0], a[1], "")); break; }
                        case 3:
                            { CardWindow.AddAuthorToCard(new FIO(-1, a[0], a[1], a[2])); break; }
                    }
                }
                else CardWindow.AddAuthorToCard(AuthorList.SelectedItem as FIO);
            }
            else {
                if (AuthorList.SelectedIndex != -1)
                    Main.SearchAuthor(AuthorList.SelectedItem as FIO);
                else return;
            }
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

        private void TextChanged() {
            if (Main != null || CardWindow != null)
            {
                if (AuthorText.Text != "")
                {
                    var temp = Main == null ? (CardWindow.Main.Authors) : (Main.Authors);
                    var ttemp = temp.Where(t => t.Familiya.ToLower().Contains(AuthorText.Text.ToLower()) == true ||
                    t.Imya.ToLower().Contains(AuthorText.Text.ToLower()) == true ||
                    t.Otchestvo.ToLower().Contains(AuthorText.Text.ToLower()) == true);
                    this.AuthorList.ItemsSource = ttemp;
                }
                else this.AuthorList.ItemsSource = Main == null ? (CardWindow.Main.Authors) : (Main.Authors);
            }
        }
    }
}
