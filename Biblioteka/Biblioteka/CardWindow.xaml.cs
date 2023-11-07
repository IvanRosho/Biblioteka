using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System;
using System.Windows.Input;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Controls;

namespace Biblioteka
{
    /// <summary>
    /// Логика взаимодействия для CardWindow.xaml
    /// </summary>
    public partial class CardWindow : Window
    {
        public MainWindow Main;
        private Card EditableCard;
        public bool IsNew;
        private ObservableCollection<string> Links;
        public CardWindow(MainWindow w, Card c, bool isNew)
        {
            InitializeComponent();
            Main = w;
            EditableCard = c;
            IsNew = isNew;
            Links = new ObservableCollection<string>();
            this.DataContext = EditableCard;
            this.Authors.ItemsSource = EditableCard.Authors;
            string ss = EditableCard.Content;
            if (isNew == true) return;
            while (ss.ToLower().Contains("http") == true) {
                int st = ss.ToLower().IndexOf("http");
                int fin = ss.ToLower().IndexOf(" ", st + 1);
                if (fin == -1) fin = ss.Length - 1;
                Links.Add(ss.Substring(st, fin - st));
                ss = ss.Remove(st, fin - st);
            }
            //Links.Add("test");
            if (Links.Count != 0) LinksPanel.ItemsSource = Links;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsNew == true) return;
            if (EditableCard.Type != 0)
            {
                var sh = ((this.CardType.ItemsSource) as ObservableCollection<SimpleRecord>);
                int i = sh.IndexOf(sh.Where(t => t.ID == EditableCard.Type).First());
                CardType.SelectedIndex = i;
            }
            if (EditableCard.Shifr != null && EditableCard.Shifr != "")
            {
                var sh = ((this.CardShifr.ItemsSource) as ObservableCollection<SimpleRecord>);
                int i = sh.IndexOf(sh.Where(t => t.Shifr == EditableCard.Shifr).First());
                CardShifr.SelectedIndex = i;
            }
            if (EditableCard.Folder != null && EditableCard.Folder != "")
            {
                var sh = ((this.CardFolder.ItemsSource) as ObservableCollection<SimpleRecord>);
                int i = sh.IndexOf(sh.Where(t => t.ID == EditableCard.FolderID).First());
                CardFolder.SelectedIndex = i;
            }
            if (EditableCard.Sourse != -1)
            {
                var sh = ((this.CardSourse.ItemsSource) as ObservableCollection<SimpleRecord>);
                int i = sh.IndexOf(sh.Where(t => t.ID == EditableCard.Sourse).First());
                CardSourse.SelectedIndex = i;
            }
            string ts = "";
            foreach (SimpleRecord r in EditableCard.Tags) ts += r.Description + ", ";
            CardTags.Text = ts; LoadImage();
        }

        private void ShifrSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (ShifrSearch.Text == "") CardShifr.ItemsSource = Main.Shifr;
            else {
                var temp = Main.Shifr;
                var ttemp = temp.Where(t => t.Description.ToLower().Contains(ShifrSearch.Text.ToLower()) == true ||
                t.Shifr.ToLower().Contains(ShifrSearch.Text.ToLower()) == true);
                this.CardShifr.ItemsSource = ttemp;
                CardShifr.IsDropDownOpen = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CardType.SelectedIndex == -1 || CardFolder.SelectedIndex == -1) {
                MessageBox.Show("Вы не выбрали поля Тип доступа и папку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            EditableCard.Type = (CardType.SelectedItem as SimpleRecord).ID;
            EditableCard.Shifr = (CardShifr.SelectedItem as SimpleRecord).Shifr;
            EditableCard.FolderID = (CardFolder.SelectedItem as SimpleRecord).ID;
            if (CardSourse.SelectedIndex != -1) EditableCard.Sourse = (CardSourse.SelectedItem as SimpleRecord).ID; else EditableCard.Sourse = -1;
            EditableCard.Tags = new ObservableCollection<SimpleRecord>();
            foreach (string s in CardTags.Text.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)) {
                string tem = s.Replace("  ", " ").Trim(' ');
                if (tem == "") continue;
                EditableCard.Tags.Add(new SimpleRecord(-1, tem));
            }

            if (Main.UpdateCard(IsNew, EditableCard) == false) {
                MessageBox.Show("Не удалось записать карточку!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            this.Close();
        }

        private void DelAuthor_Click(object sender, RoutedEventArgs e)
        {
            EditableCard.Authors.Remove(Authors.SelectedItem as FIO);
        }

        public void AddAuthorToCard(FIO f) {
            EditableCard.Authors.Add(f);
        }

        private void AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            AddContextAuthor t = new AddContextAuthor(this);
            t.ShowDialog();
        }

        private void Sourse_Add_Click(object sender, RoutedEventArgs e)
        {
            CardSourse.Text = "";
            CardSourse.IsEditable = true;
        }

        private void CardSourse_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CardSourse.IsEditable = false;
                CardSourse.Text = "";
                if (EditableCard.Sourse != -1)
                {
                    var sh = ((this.CardSourse.ItemsSource) as ObservableCollection<SimpleRecord>);
                    int i = sh.IndexOf(sh.Where(t => t.ID == EditableCard.Sourse).First());
                    CardSourse.SelectedIndex = i;
                }
                return;
            }
            if (CardSourse.IsEditable == false || e.Key != Key.Enter) return;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string ic = String.Format("INSERT INTO [dbo].[Sourse] ([Sourse]) VALUES ('{0}') SELECT @@IDENTITY as ID", CardSourse.Text);
            SqlCommand comm = new SqlCommand(ic, Main.MainConnection);
            adapter.InsertCommand = comm;
            var res = adapter.InsertCommand.ExecuteScalar();
            if (res != null)
            {
                EditableCard.Sourse = Convert.ToInt32(res);
                (CardSourse.ItemsSource as ObservableCollection<SimpleRecord>).Add(new SimpleRecord(EditableCard.Sourse, CardSourse.Text));
                CardSourse.SelectedIndex = CardSourse.Items.Count - 1;
            }
            else return;
            CardSourse.IsEditable = false;
        }

        private void Folder_Add_Click(object sender, RoutedEventArgs e)
        {
            CardFolder.Text = "";
            CardFolder.IsEditable = true;
        }

        private void Folder_Add_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CardFolder.IsEditable = false;
                CardFolder.Text = "";
                if (EditableCard.Folder != null && EditableCard.Folder != "")
                {
                    var sh = ((this.CardFolder.ItemsSource) as ObservableCollection<SimpleRecord>);
                    int i = sh.IndexOf(sh.Where(t => t.ID == EditableCard.FolderID).First());
                    CardFolder.SelectedIndex = i;
                }
                return;
            }
            if (CardFolder.IsEditable == false || e.Key != Key.Enter) return;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string ic = String.Format("INSERT INTO [dbo].[Folders] ([Folder] ,[ParentFolder]) VALUES ('{0}', NULL) SELECT @@IDENTITY as ID", CardFolder.Text);
            SqlCommand comm = new SqlCommand(ic, Main.MainConnection);
            adapter.InsertCommand = comm;
            var res = adapter.InsertCommand.ExecuteScalar();
            if (res != null)
            {
                EditableCard.FolderID = Convert.ToInt32(res);
                EditableCard.Folder = CardFolder.Text;
                (CardFolder.ItemsSource as ObservableCollection<SimpleRecord>).Add(new SimpleRecord(EditableCard.Sourse, CardFolder.Text));
                CardFolder.SelectedIndex = CardFolder.Items.Count - 1;
            }
            else return;
            CardFolder.IsEditable = false;
        }

        private void Type_Add_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CardType.IsEditable = false;
                CardType.Text = "";
                if (EditableCard.Type != 0)
                {
                    var sh = ((this.CardType.ItemsSource) as ObservableCollection<SimpleRecord>);
                    int i = sh.IndexOf(sh.Where(t => t.ID == EditableCard.Type).First());
                    CardType.SelectedIndex = i;
                }
                return;
            }
            if (CardType.IsEditable == false || e.Key != Key.Enter) return;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string ic = String.Format("INSERT INTO [dbo].[TypeOfAccess] ([Type]) VALUES ('{0}') SELECT @@IDENTITY as ID", CardType.Text);
            SqlCommand comm = new SqlCommand(ic, Main.MainConnection);
            adapter.InsertCommand = comm;
            var res = adapter.InsertCommand.ExecuteScalar();
            if (res != null)
            {
                EditableCard.Type = Convert.ToInt32(res);
                (CardType.ItemsSource as ObservableCollection<SimpleRecord>).Add(new SimpleRecord(EditableCard.Type, CardType.Text));
                CardType.SelectedIndex = CardType.Items.Count - 1;
            }
            else return;
            CardType.IsEditable = false;
        }

        private void Type_Add_Click(object sender, RoutedEventArgs e)
        {
            CardType.Text = "";
            CardType.IsEditable = true;
        }

        private void LinkClick(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start((sender as System.Windows.Controls.Button).Content as string);
        }

        private void CardImageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Файлы изображений|*.bmp;*.jpg;*.png;*.gif";
                dlg.Multiselect = false;
                if (dlg.ShowDialog() != false)
                {
                    string s = dlg.FileName;
                    EditableCard.Image = Path.GetFileName(s);
                    File.Copy(s, @".\img\" + EditableCard.Image, true);
                    using (System.IO.FileStream fs = new System.IO.FileStream(s, FileMode.Open))
                    {
                        EditableCard.ImageData = new byte[fs.Length];
                        fs.Read(EditableCard.ImageData, 0, EditableCard.ImageData.Length);
                    }
                    LoadImage();
                }
            }
            catch (Exception)
            {
                EditableCard.Image = "";
                MessageBox.Show("Не удалось скопировать файл, файл поврежден или недоступен","Не удалось скопировать файл",MessageBoxButton.OK,MessageBoxImage.Information);
            }
        }

        private void LoadImage() {
            if (EditableCard.ImageData != null) {
                System.Windows.Media.Imaging.BitmapImage btm = new System.Windows.Media.Imaging.BitmapImage();
                using (var mem = new MemoryStream(EditableCard.ImageData))
                {
                    mem.Position = 0;
                    btm.BeginInit();
                    btm.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat;
                    btm.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    btm.UriSource = null;
                    btm.StreamSource = mem;
                    btm.EndInit();
                }
                btm.Freeze();
                CardImage.Source = btm;
                return;
            }
            else if (EditableCard.Image != null && EditableCard.Image != "" && EditableCard.Image != "NULL")
            {
                try
                {
                    System.Windows.Media.Imaging.BitmapImage btm = new System.Windows.Media.Imaging.BitmapImage();
                    btm.BeginInit();
                    btm.UriSource = new Uri(Directory.GetCurrentDirectory() + "\\img\\" + EditableCard.Image);
                    //btm.DecodePixelWidth = (int)CardImage.Width;
                    btm.EndInit();
                    CardImage.Source = btm;
                    using (System.IO.FileStream fs = new System.IO.FileStream(Directory.GetCurrentDirectory() + "\\img\\" + EditableCard.Image, FileMode.Open))
                    {
                        EditableCard.ImageData = new byte[fs.Length];
                        fs.Read(EditableCard.ImageData, 0, EditableCard.ImageData.Length);
                    }
                }
                catch (Exception)
                {
                    EditableCard.Image = "";
                    MessageBox.Show("Не удалось загрузить картинку, файл поврежден или недоступен", "Не удалось загрузить файл", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else EditableCard.Image = "";
        }
    }
}
