using Bytescout.Document;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;

namespace Biblioteka
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<SimpleRecord> Shifr;
        public ObservableCollection<SimpleRecord> TypeOfPublic;
        public ObservableCollection<SimpleRecord> Folders;
        public ObservableCollection<SimpleRecord> Sourses;
        public ObservableCollection<FIO>            Authors;
        public ObservableCollection<FIO> Tags;
        ObservableCollection<SimpleRecord>           ActualShifr;
        ObservableCollection<Card> Cards;
        public SqlConnection MainConnection;
        public UserPrivelegies User;
        
        public MainWindow()
        {
            InitializeComponent();
            MainConnection = new SqlConnection();
            OpenConnect();
            if (Directory.Exists(@".\img") == false) Directory.CreateDirectory(@".\img");
        }

        private void MenuConfigConf_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow cfg = new ConfigWindow(this);
            cfg.ShowDialog();

        }

        public bool OpenConnect() {
            if (MainConnection.State != ConnectionState.Closed) MainConnection.Close();
            MainConnection = new SqlConnection();
            MainConnection.ConnectionString = String.Format("Data Source={2}\\{0};Initial Catalog={1}; Persist Security Info=True;User Id={3};Password={4}"
                         , Properties.Settings.Default.ServerName, Properties.Settings.Default.BaseName,
                Properties.Settings.Default.Workstation, Properties.Settings.Default.Login, Properties.Settings.Default.Password);
            try {
                MainConnection.Open();
                switch (Properties.Settings.Default.Login) {
                    case "sa": {
                            User = new UserPrivelegies(true, true);
                            break;
                        }
                    case "Admin" : {
                            User = new UserPrivelegies(true, true);
                            break;
                        }
                    case "Operator": {
                            User = new UserPrivelegies(true, false);
                            break;
                        }
                    case "User": {
                            User = new UserPrivelegies(false,false);
                            break;
                        }
                    default: {
                            throw new ArgumentException("Пользователь имеет неизвестную роль.");
                        }
                }
                this.DataContext = User;
            }
            catch (Exception ex) {
                MessageBox.Show("Не удалось установить соединение, проверьте настройки подключения\r\n"+ex.Message,"Не удалось соединиться",MessageBoxButton.OK,MessageBoxImage.Error);
                return false; }
            RefreshBase();
            
            return true;
        }

        public void RefreshBase() {
            try
            {
                ActualShifr = new ObservableCollection<SimpleRecord>();
                SqlCommand com = MainConnection.CreateCommand();
                com.CommandText = "SELECT * FROM ActualShifr ORDER BY Description";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    ActualShifr.Add(new SimpleRecord(Convert.ToString(table.Rows[i]["Shifr"]), Convert.ToString(table.Rows[i]["Description"])));
                }
                ShifrPanel.ItemsSource = ActualShifr;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список актуальных шифров, проверьте настройки конфигурации","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            try
            {
                Shifr = new ObservableCollection<SimpleRecord>();
                SqlCommand com = MainConnection.CreateCommand();
                com.CommandText = "SELECT * FROM dbo.Shifr ORDER BY Description";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Shifr.Add(new SimpleRecord(Convert.ToString(table.Rows[i]["Shifr"]), Convert.ToString(table.Rows[i]["Description"])));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список шифров, проверьте настройки конфигурации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                TypeOfPublic = new ObservableCollection<SimpleRecord>();
                SqlCommand com = MainConnection.CreateCommand();
                com.CommandText = "SELECT * FROM dbo.TypeOfAccess ORDER BY Type";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    TypeOfPublic.Add(new SimpleRecord(int.Parse(Convert.ToString(table.Rows[i]["ID"])), Convert.ToString(table.Rows[i]["Type"])));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список типов доступа, проверьте настройки конфигурации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                Authors = new ObservableCollection<FIO>();
                SqlCommand com = MainConnection.CreateCommand();
                com.CommandText = "SELECT * FROM dbo.Author ORDER BY Familiya";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Authors.Add(new FIO(int.Parse(Convert.ToString(table.Rows[i]["ID"])), Convert.ToString(table.Rows[i]["Familiya"]),
                        Convert.ToString(table.Rows[i]["Imya"]), Convert.ToString(table.Rows[i]["Otchestvo"])));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список авторов, проверьте настройки конфигурации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                Folders = new ObservableCollection<SimpleRecord>();
                SqlCommand com = MainConnection.CreateCommand();
                com.CommandText = "SELECT * FROM dbo.Folders ORDER BY Folder";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Folders.Add(new SimpleRecord(int.Parse(Convert.ToString(table.Rows[i]["ID"])), Convert.ToString(table.Rows[i]["Folder"])));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список типов доступа, проверьте настройки конфигурации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                Sourses = new ObservableCollection<SimpleRecord>();
                SqlCommand com = MainConnection.CreateCommand();
                com.CommandText = "SELECT * FROM dbo.Sourse ORDER BY Sourse";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Sourses.Add(new SimpleRecord(int.Parse(Convert.ToString(table.Rows[i]["ID"])), Convert.ToString(table.Rows[i]["Sourse"])));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список источников, проверьте настройки конфигурации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                Tags = new ObservableCollection<FIO>();
                SqlCommand com = MainConnection.CreateCommand();
                com.CommandText = "SELECT * FROM dbo.Tag ORDER BY Tag";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Tags.Add(new FIO(int.Parse(Convert.ToString(table.Rows[i]["ID"])), Convert.ToString(table.Rows[i]["Tag"]),"","" ));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список авторов, проверьте настройки конфигурации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            RF_Click(this, new RoutedEventArgs());
        }

        private void ShifrPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShifrPanel.SelectedIndex == -1) return;
            if (RF.IsChecked != true)
            {
                try
                {
                    var a = ShifrPanel.SelectedItem as SimpleRecord;
                    string selShifr = a.Shifr;
                    Cards = new ObservableCollection<Card>();
                    SqlCommand com = MainConnection.CreateCommand();
                    com.CommandText = "SELECT dbo.Card.ID, dbo.Card.BibliothekNumber, dbo.Card.Name, dbo.Shifr.Description " +
                        " FROM dbo.Card INNER JOIN " +
                        " dbo.Shifr ON dbo.Card.BibliothekNumber = dbo.Shifr.Shifr " +
                        " WHERE        (dbo.Shifr.Shifr = N'" + selShifr + "') " +
                        "ORDER BY dbo.Card.Name";
                    SqlDataAdapter adapter = new SqlDataAdapter(com);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        Card c = new Card(User);
                        c.ID = Convert.ToInt32(table.Rows[i]["ID"]);
                        c.Shifr = Convert.ToString(table.Rows[i]["BibliothekNumber"]);
                        c.Name = Convert.ToString(table.Rows[i]["Name"]);
                        Cards.Add(c);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось загрузить список карточек", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else {
                try
                {
                    var a = ShifrPanel.SelectedItem as SimpleRecord;
                    string selShifr = a.Shifr;
                    Cards = new ObservableCollection<Card>();
                    SqlCommand com = MainConnection.CreateCommand();
                    com.CommandText = "SELECT dbo.Card.ID, dbo.Card.Name, dbo.Folders.Folder " +
                        " FROM            dbo.Card INNER JOIN dbo.Folders ON dbo.Card.Folder = dbo.Folders.ID " +
                        " WHERE        (dbo.Folders.ID = "+(ShifrPanel.SelectedItem as SimpleRecord).ID+") " +
                        "ORDER BY dbo.Card.Name";
                    SqlDataAdapter adapter = new SqlDataAdapter(com);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        Card c = new Card(User);
                        c.ID = Convert.ToInt32(table.Rows[i]["ID"]);
                        c.Shifr = Convert.ToString(table.Rows[i]["Folder"]);
                        c.Name = Convert.ToString(table.Rows[i]["Name"]);
                        Cards.Add(c);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось загрузить список карточек", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            CardPanel.ItemsSource = Cards;
        }

        private void CardPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenCard();
        }

        private void OpenCard()
        {
            if (CardPanel.SelectedIndex == -1) return;
            Card temp = new Card(User);
            try
            {
                temp = CardPanel.SelectedItem as Card;
                Cards = new ObservableCollection<Card>();
                SqlCommand com = MainConnection.CreateCommand();
                com.CommandText = "SELECT        dbo.Card.ID, dbo.Shifr.Shifr, dbo.Shifr.Description, dbo.Card.Name, dbo.Card.[Content], dbo.Card.DateOfReg, dbo.Card.DateOfPublic, dbo.Card.Sourse, dbo.Folders.Folder, dbo.Card.Folder AS FID, dbo.Card.TypeAccess, dbo.Card.Image, dbo.Card.ImageTitle, dbo.Card.ImageData " +
                    " FROM dbo.Card INNER JOIN dbo.Shifr ON dbo.Card.BibliothekNumber = dbo.Shifr.Shifr INNER JOIN " +
                    " dbo.Folders ON dbo.Card.Folder = dbo.Folders.ID " +
                    " WHERE (dbo.Card.ID = " + temp.ID + ")";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                temp.Shifr = Convert.ToString(table.Rows[0]["Shifr"]);
                temp.ShifrDeskription = Convert.ToString(table.Rows[0]["Description"]);
                temp.Content = Convert.ToString(table.Rows[0]["Content"]);
                if (Convert.ToString(table.Rows[0]["DateOfReg"]) != "")
                    temp.DateOfReg = DateTime.Parse(Convert.ToString(table.Rows[0]["DateOfReg"]));
                if (Convert.ToString(table.Rows[0]["DateOfPublic"]) != "")
                    temp.DateOfPublic = DateTime.Parse(Convert.ToString(table.Rows[0]["DateOfPublic"]));
                int tt;
                bool ch = int.TryParse(Convert.ToString(table.Rows[0]["Sourse"]), out tt);
                temp.Sourse = ch == true ? tt : -1;
                if (Convert.ToString(table.Rows[0]["TypeAccess"]) != "") temp.Type = int.Parse(Convert.ToString(table.Rows[0]["TypeAccess"]));
                temp.Authors = new ObservableCollection<FIO>();
                temp.FolderID = int.Parse(Convert.ToString(table.Rows[0]["FID"]));
                temp.Folder = Convert.ToString(table.Rows[0]["Folder"]);
                temp.Image = Convert.ToString(table.Rows[0]["Image"]);
                temp.ImageTitle = Convert.ToString(table.Rows[0]["ImageTitle"]);
                temp.ImageData = table.Rows[0]["ImageData"] as byte[];

                com = MainConnection.CreateCommand();
                com.CommandText = "SELECT        dbo.Author.Familiya, dbo.Author.Imya, dbo.Author.Otchestvo, dbo.Author.ID" +
                    " FROM            dbo.Author INNER JOIN " +
                    "  dbo.Authors ON dbo.Author.ID = dbo.Authors.AuthorID INNER JOIN " +
                    "  dbo.Card ON dbo.Authors.CardID = dbo.Card.ID " +
                    " WHERE (dbo.Authors.CardID = " + temp.ID + ") GROUP BY dbo.Author.Familiya, dbo.Author.Imya, dbo.Author.Otchestvo, dbo.Author.ID";
                adapter = new SqlDataAdapter(com);
                table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    temp.Authors.Add(new FIO(int.Parse(Convert.ToString(table.Rows[i]["ID"])), Convert.ToString(table.Rows[i]["Familiya"]),
                        Convert.ToString(table.Rows[i]["Imya"]), Convert.ToString(table.Rows[i]["Otchestvo"])));
                }

                temp.Tags = new ObservableCollection<SimpleRecord>();
                com = MainConnection.CreateCommand();
                com.CommandText = " SELECT dbo.Tag.ID, dbo.Tag.Tag FROM dbo.Tags INNER JOIN dbo.Tag ON dbo.Tags.TagID = dbo.Tag.ID " +
                    " WHERE (dbo.Tags.CardID = " + temp.ID + ") ORDER BY dbo.Tag.Tag";
                adapter = new SqlDataAdapter(com);
                table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    temp.Tags.Add(new SimpleRecord(int.Parse(Convert.ToString(table.Rows[i]["ID"])), Convert.ToString(table.Rows[i]["Tag"])));
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить карточку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            temp.CanInsert = User.CanInsert;
            temp.CanUpdate = User.CanUpdate;
            CardWindow CW = new CardWindow(this, temp, false);
            CW.CardShifr.ItemsSource = this.Shifr;
            CW.CardType.ItemsSource = this.TypeOfPublic;
            CW.CardFolder.ItemsSource = this.Folders;
            CW.CardSourse.ItemsSource = this.Sourses;
            CW.ShowDialog();
        }

        public bool UpdateCard(bool isNew, Card c) {
            try
            {
                if (isNew == false)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string updateComm = String.Format("UPDATE [dbo].[Card] SET [BibliothekNumber] = '{0}',[Name] = '{1}',[Content] = '{2}',[DateOfReg] = '{3}'," +
                        "[DateOfPublic] = '{4}',[TypeAccess] = {5},[Sourse] = {6}, [Folder] = {7}, [Image] = '{9}', [ImageTitle]='{10}', [ImageData]=@IData WHERE [ID]={8}", 
                        c.Shifr, c.Name, c.Content, c.DateOfReg.ToString("yyyy-MM-dd"), c.DateOfPublic.ToString("yyyy-MM-dd"), 
                        c.Type, (c.Sourse != -1 ? c.Sourse.ToString() : "NULL"), 
                        c.FolderID, c.ID, 
                        (c.Image != null && c.Image != "" && c.Image != "NULL") ==true ?  c.Image : null , 
                        c.ImageTitle);
                    SqlCommand comm = new SqlCommand(updateComm, MainConnection);
                    comm.Parameters.Add("@IData", SqlDbType.VarBinary );
                    if (c.ImageData != null) comm.Parameters["@IData"].Value = c.ImageData; else comm.Parameters["@IData"].Value = DBNull.Value;
                    adapter.UpdateCommand = comm;
                    adapter.UpdateCommand.ExecuteNonQuery();
                }
                else {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string incertComm = String.Format("INSERT INTO [dbo].[Card] ([Folder],[BibliothekNumber],[Name],[Content],[DateOfReg],[DateOfPublic]," +
                        "[TypeAccess],[Sourse],[Image],[ImageTitle],[ImageData]) VALUES ({0},'{1}','{2}','{3}','{4}','{5}',{6},{7},'{8}','{9}',@IData) SELECT @@IDENTITY as ID", 
                        c.FolderID, c.Shifr, c.Name, c.Content, c.DateOfReg.ToString("MM-dd-yyyy"),c.DateOfPublic.ToString("MM-dd-yyyy"), 
                        c.Type, (c.Sourse != -1 ? c.Sourse.ToString() : "NULL"),
                        (c.Image != null && c.Image != "" && c.Image != "NULL") == true ? c.Image : null,
                        c.ImageTitle);
                    SqlCommand comm = new SqlCommand(incertComm, MainConnection);
                    comm.Parameters.Add("@IData", SqlDbType.VarBinary);
                    if (c.ImageData != null) comm.Parameters["@IData"].Value = c.ImageData; else comm.Parameters["@IData"].Value = DBNull.Value;
                    adapter.InsertCommand = comm;
                    var res = adapter.InsertCommand.ExecuteScalar();
                    DataTable table = new DataTable();
                    if (res != null)  c.ID = Convert.ToInt32(res);
                    RefreshBase();
                }

                SqlDataAdapter ad = new SqlDataAdapter();
                string scomm = String.Format("DELETE FROM [dbo].[Authors] WHERE CardID={0}",
                    c.ID);
                SqlCommand com = new SqlCommand(scomm, MainConnection);
                ad.DeleteCommand = com;
                ad.DeleteCommand.ExecuteNonQuery();
                foreach (FIO f in c.Authors)
                {
                    if (f.ID == -1)
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        string ic = String.Format("INSERT INTO [dbo].[Author] ([Familiya],[Imya],[Otchestvo]) VALUES ('{0}','{1}','{2}') SELECT @@IDENTITY as ID",
                            f.Familiya, f.Imya, f.Otchestvo);
                        SqlCommand comm = new SqlCommand(ic, MainConnection);
                        adapter.InsertCommand = comm;
                        var res = adapter.InsertCommand.ExecuteScalar();
                        if (res!=null) f.ID = Convert.ToInt32(res); else continue;
                    }
                    scomm = String.Format("INSERT INTO [dbo].[Authors] ([CardID],[AuthorID]) VALUES ({0} ,{1})",
                                 c.ID,f.ID);
                    com = new SqlCommand(scomm, MainConnection);
                    ad.InsertCommand = com;
                    ad.InsertCommand.ExecuteNonQuery();
                }

                ad = new SqlDataAdapter();
                scomm = String.Format("DELETE FROM [dbo].[Tags] WHERE CardID={0}",
                    c.ID);
                com = new SqlCommand(scomm, MainConnection);
                ad.DeleteCommand = com;
                ad.DeleteCommand.ExecuteNonQuery();
                foreach (SimpleRecord t in c.Tags)
                {
                    scomm = String.Format("SELECT * FROM Tag WHERE (Tag = '{0}')", t.Description);
                    com = new SqlCommand(scomm,MainConnection);
                    ad = new SqlDataAdapter(com);
                    DataTable table = new DataTable();
                    ad.Fill(table);
                    if (table.Rows.Count == 0)
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        string ic = String.Format("INSERT INTO [dbo].[Tag] ([Tag]) VALUES ('{0}') SELECT @@IDENTITY as ID",
                            t.Description);
                        SqlCommand comm = new SqlCommand(ic, MainConnection);
                        adapter.InsertCommand = comm;
                        var res = adapter.InsertCommand.ExecuteScalar();
                        if (res != null) t.ID = Convert.ToInt32(res); else continue;
                    }
                    else t.ID = int.Parse(Convert.ToString(table.Rows[0]["ID"]));
                    scomm = String.Format("INSERT INTO [dbo].[Tags] ([CardID],[TagID]) VALUES ({0} ,{1})",
                                 c.ID, t.ID);
                    com = new SqlCommand(scomm, MainConnection);
                    ad.InsertCommand = com;
                    ad.InsertCommand.ExecuteNonQuery();
                }

            }
            catch (Exception) {
                return false;
            }
            return true;
        }

        private void MenuConfigAddCard_Click(object sender, RoutedEventArgs e)
        {
            Card temp = new Card(User);
            CardWindow CW = new CardWindow(this, temp, true);
            CW.CardShifr.ItemsSource = this.Shifr;
            CW.CardType.ItemsSource = this.TypeOfPublic;
            CW.CardFolder.ItemsSource = this.Folders;
            CW.CardSourse.ItemsSource = this.Sourses;
            CW.ShowDialog();
        }

        private void MenuConfigAddAuthor_Click(object sender, RoutedEventArgs e)
        {
            AddWindow Add = new AddWindow(this);
            Add.AuthorsList.ItemsSource = Authors;
            Add.IndexList.ItemsSource = Shifr;
            Add.TypeList.ItemsSource = TypeOfPublic;
            Add.FolderList.ItemsSource = Folders;
            Add.SourseList.ItemsSource = Sourses;
            Add.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainConnection.Close();
            if (Cards != null) Cards.Clear();
            if (Shifr != null) Shifr.Clear();
            if (TypeOfPublic != null) TypeOfPublic.Clear();
            if (Authors != null) Authors.Clear();
            if (ActualShifr != null) ActualShifr.Clear();
            if (Folders != null) Folders.Clear();
            if (Sourses != null) Sourses.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((SearchText.Text.Replace(" ","") == "") && ShifrPanel.SelectedIndex==-1) return;
            string fields ="(";
            bool[] b = { DescrSearch.IsChecked, NameSearch.IsChecked, SourseSearch.IsChecked };
            if (b[0] == b[1] == b[2] == true) fields = "(dbo.Card.[Content] LIKE LOWER('%@search%')) OR ( dbo.Card.Name LIKE LOWER('%@search%'))  OR (dbo.Sourse.Sourse LIKE LOWER('%@search%'))";
            else if (b[0] == b[1] == true) fields = "(dbo.Card.[Content] LIKE LOWER('%@search%')) OR ( dbo.Card.Name LIKE LOWER('%@search%'))";
            else if (b[1] == b[2] == true) fields = " ( dbo.Card.Name LIKE LOWER('%@search%'))  OR (dbo.Sourse.Sourse LIKE LOWER('%@search%'))";
            else if (b[0] == b[2] == true) fields = "(dbo.Card.[Content] LIKE LOWER('%@search%')) OR (dbo.Sourse.Sourse LIKE LOWER('%@search%'))";
            else if (b[0] == true) fields = "(dbo.Card.[Content] LIKE LOWER('%@search%'))";
            else if (b[1] == true) fields = "(dbo.Card.Name LIKE LOWER('%@search%'))";
            else if (b[2] == true) fields = "(dbo.Sourse.Sourse LIKE LOWER('%@search%'))";
            else {
                MessageBox.Show("А что вы ищете?", "Ошибка поиска", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Cards = new ObservableCollection<Card>();
            string[] Search = !SolidWord.IsChecked ? SearchText.Text.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries) : new string[] { SearchText.Text.Replace("  ", " ") };

            try
            {
                foreach (string search in Search)
                {
                    var temp = search;
                    var a = ShifrPanel.SelectedItem as SimpleRecord;
                    string selShifr = a==null ? "" : a.Shifr;
                    SqlCommand com = MainConnection.CreateCommand();
                    fields = fields.Replace("@search", temp);
                    com.CommandText = "SELECT dbo.Card.ID, dbo.Card.BibliothekNumber, dbo.Card.Name, dbo.Shifr.Description " +
                        " FROM dbo.Card INNER JOIN " +
                        " dbo.Shifr ON dbo.Card.BibliothekNumber = dbo.Shifr.Shifr INNER JOIN dbo.Sourse ON dbo.Card.Sourse = dbo.Sourse.ID WHERE (" + fields;
                    com.CommandText += ") ORDER BY dbo.Card.Name";
                    //if (SolidWord.IsChecked) com.CommandText = com.CommandText.Replace("%","");
                    SqlDataAdapter adapter = new SqlDataAdapter(com);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        Card c = new Card(User);
                        c.ID = Convert.ToInt32(table.Rows[i]["ID"]);
                        c.Shifr = Convert.ToString(table.Rows[i]["BibliothekNumber"]);
                        c.Name = Convert.ToString(table.Rows[i]["Name"]);
                        if (Cards.Where(t=>t.ID == c.ID).Count()==0) Cards.Add(c);
                    }
                }
                Cards = Cards.OrderBy(t=>t.Name) as ObservableCollection<Card>;
                CardPanel.ItemsSource = Cards;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка поиска", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SolidWord_Click(object sender, RoutedEventArgs e)
        {
            SolidWord.IsChecked = !SolidWord.IsChecked;
        }
        private void OnlySelectedIndex_Click(object sender, RoutedEventArgs e)
        {
            //OnlySelectedIndex.IsChecked = !OnlySelectedIndex.IsChecked;
        }
        private void NameSearch_Click(object sender, RoutedEventArgs e)
        {
            NameSearch.IsChecked = !NameSearch.IsChecked;
        }
        private void DescrSearch_Click(object sender, RoutedEventArgs e)
        {
            DescrSearch.IsChecked = !DescrSearch.IsChecked;
        }
        private void SourseSearch_Click(object sender, RoutedEventArgs e)
        {
            SourseSearch.IsChecked = !SourseSearch.IsChecked;
        }

        private void AuthorSearch_Click(object sender, RoutedEventArgs e)
        {
            AddContextAuthor t = new AddContextAuthor(this);
            t.ShowDialog();
        }

        public void SearchAuthor(FIO f) {
            try
            {
                Cards = new ObservableCollection<Card>();
                SqlCommand com = MainConnection.CreateCommand();
                com.CommandText = "SELECT        COUNT(dbo.Authors.CardID) AS [1], dbo.Card.ID, dbo.Card.BibliothekNumber, dbo.Card.Name, dbo.Author.Familiya, dbo.Author.Imya, dbo.Author.Otchestvo " +
                    " FROM            dbo.Card INNER JOIN  dbo.Authors ON dbo.Card.ID = dbo.Authors.CardID INNER JOIN dbo.Author ON dbo.Authors.AuthorID = dbo.Author.ID " +
                    " GROUP BY dbo.Card.BibliothekNumber, dbo.Card.Name, dbo.Card.ID, dbo.Author.Familiya, dbo.Author.Imya, dbo.Author.Otchestvo " +
                    " HAVING        (dbo.Author.Familiya = N'"+f.Familiya+"') AND (dbo.Author.Imya = N'"+f.Imya+"') AND (dbo.Author.Otchestvo = N'"+f.Otchestvo+"') " +
                    "ORDER BY dbo.Card.ID";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Card c = new Card(User);
                    c.ID = Convert.ToInt32(table.Rows[i]["ID"]);
                    c.Shifr = Convert.ToString(table.Rows[i]["BibliothekNumber"]);
                    c.Name = Convert.ToString(table.Rows[i]["Name"]);
                    Cards.Add(c);
                }
                this.CardPanel.ItemsSource = Cards;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список карточек выбранного автора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SearchTag(FIO f)
        {
            try
            {
                Cards = new ObservableCollection<Card>();
                SqlCommand com = MainConnection.CreateCommand();
                com.CommandText = "SELECT dbo.Card.ID, dbo.Card.BibliothekNumber, dbo.Card.Name FROM dbo.Card INNER JOIN " +
                    "dbo.Tags ON dbo.Card.ID = dbo.Tags.CardID INNER JOIN dbo.Tag ON dbo.Tags.TagID = dbo.Tag.ID " +
                    "GROUP BY dbo.Card.BibliothekNumber, dbo.Card.Name, dbo.Card.ID, dbo.Tag.Tag "+
                    "HAVING (dbo.Tag.Tag = N'"+f.Familiya+ "') ORDER BY dbo.Card.Name";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Card c = new Card(User);
                    c.ID = Convert.ToInt32(table.Rows[i]["ID"]);
                    c.Shifr = Convert.ToString(table.Rows[i]["BibliothekNumber"]);
                    c.Name = Convert.ToString(table.Rows[i]["Name"]);
                    Cards.Add(c);
                }
                this.CardPanel.ItemsSource = Cards;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список карточек выбранного тега", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RF_Click(object sender, RoutedEventArgs e)
        {
            ShifrPanel.ItemsSource = RF.IsChecked == true ? Folders : ActualShifr;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CardPanel.SelectedIndex != -1)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string delComm = String.Format("DELETE FROM [dbo].[Card] WHERE ID={0}",
                        (CardPanel.SelectedItem as Card).ID);
                    SqlCommand comm = new SqlCommand(delComm, MainConnection);
                    adapter.DeleteCommand = comm;
                    Cards.Remove(CardPanel.SelectedItem as Card);
                    adapter.DeleteCommand.ExecuteNonQuery();
                }
                else return;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось удалить карточку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuConfigImport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Файлы карточек|*.doc;*.docx";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() != false) {
                foreach (string file in dlg.FileNames) {
                    try
                    {
                        Document doc = new Document();
                        Card temp = new Card(User);
                        CardWindow CW = new CardWindow(this, temp, true);
                        CW.CardShifr.ItemsSource = this.Shifr;
                        CW.CardType.ItemsSource = this.TypeOfPublic;
                        CW.CardFolder.ItemsSource = this.Folders;
                        doc.Open(file);
                        for (int i = 0; i < doc.ParagraphCount; i++)
                        {
                            for (int j = 0; j < doc.GetParagraph(i).TextRunCount; j++)
                            {
                                temp.Content += doc.GetParagraph(i).GetTextRun(j).Text;
                            }
                        }
                        CW.ShowDialog();
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }

        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenCard();
        }

        private void TagSearch_Click(object sender, RoutedEventArgs e)
        {

            AddContentTag t = new AddContentTag(this);
            t.ShowDialog();
        }

        private void RateUs_Click(object sender, RoutedEventArgs e)
        {
            new RateWindow().ShowDialog();
        }
    }

    public class Card {
        public int ID { get; set; }
        public int FolderID { get; set; }
        public string Folder { get; set; }
        public string Shifr { get; set; }
        public string ShifrDeskription { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime DateOfReg { get; set; }
        public DateTime DateOfPublic { get; set; }
        public int Type { get; set; }
        public int Sourse { get; set; }
        public ObservableCollection<FIO> Authors { get; set; }
        public ObservableCollection<SimpleRecord> Tags { get; set; }
        public string Image { get; set; }
        public string ImageTitle { get; set; }

        public byte[] ImageData { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanInsert { get; set; }

        public Card(UserPrivelegies u) {
            //DateOfPublic = DateTime.Today;
            //DateOfReg = DateTime.Today;
            Authors = new ObservableCollection<FIO>();
            Tags = new ObservableCollection<SimpleRecord>();
            this.CanInsert = u.CanInsert;
            this.CanUpdate = u.CanUpdate;
        }
    }

    public class SimpleRecord {
        public string Shifr { get; set; }
        public string Description { get; set; }
        public int ID { get; set; }
        public SimpleRecord(string a, string b)
        {
            Shifr = a;
            Description = b;
            ID = -1;
        }
        public SimpleRecord(int a, string b)
        {
            ID = a;
            Description = b;
            Shifr = a.ToString();
        }
    }

    public class FIO
    {
        public int ID { get; set; }
        public string Familiya { get; set; }
        public string Imya { get; set; }
        public string Otchestvo { get; set; }
        public FIO(int i, string a, string b, string c)
        {
            ID = i;
            Familiya = a;
            Imya = b;
            Otchestvo = c;
        }
        public new string ToString() {
            return Familiya + " " + Imya + " " + Otchestvo;
        }
    }

    public class UserPrivelegies {
        public bool CanUpdate { get; set; }
        public bool CanInsert { get; set; }
        public UserPrivelegies(bool _canUpt, bool _canIns) {
            CanUpdate = _canUpt;
            CanInsert = _canIns;
        }
    }

}
