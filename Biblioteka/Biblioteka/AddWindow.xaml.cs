using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Threading;

namespace Biblioteka
{
    /// <summary>
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        MainWindow Main;
        public AddWindow(MainWindow m)
        {
            InitializeComponent();
            Main = m;
            this.DataContext = m.User;
        }

        private void AuthorAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] fio = FIOText.Text.Split(' ');
                if (AuthorsList.SelectedIndex != -1)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string updateComm = String.Format("UPDATE [dbo].[Author] SET [Familiya] = '{0}',[Imya] = '{1}',[Otchestvo] = '{2}' WHERE ID={3}" 
                        , fio[0], fio[1], fio[2], (AuthorsList.SelectedItem as FIO).ID);
                    SqlCommand comm = new SqlCommand(updateComm, Main.MainConnection);
                    adapter.UpdateCommand = comm;
                    adapter.UpdateCommand.ExecuteNonQuery();
                }
                else
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string incertComm = String.Format("INSERT INTO [dbo].[Author] ([Familiya],[Imya],[Otchestvo]) VALUES ('{0}','{1}','{2}')",
                        fio[0], fio[1], fio[2]);
                    SqlCommand comm = new SqlCommand(incertComm, Main.MainConnection);
                    adapter.InsertCommand = comm;
                    adapter.InsertCommand.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось обновить автора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Refresh();
        }

        private void AuthorsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AuthorsList.SelectedIndex!=-1) FIOText.Text = (AuthorsList.SelectedItem as FIO).ToString();
        }

        private void IndexList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IndexList.SelectedIndex != -1)
            {
                IndexText.Text = (IndexList.SelectedItem as SimpleRecord).Shifr;
                IndexDescription.Text = (IndexList.SelectedItem as SimpleRecord).Description;
            }
        }

        private void IndexAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IndexList.SelectedIndex != -1)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string updateComm = String.Format("UPDATE [dbo].[Shifr] SET [Description] = '{0}' WHERE [Shifr] = '{1}'",
                        IndexDescription.Text, IndexText.Text);
                    SqlCommand comm = new SqlCommand(updateComm, Main.MainConnection);
                    adapter.UpdateCommand = comm;
                    adapter.UpdateCommand.ExecuteNonQuery();
                }
                else
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string incertComm = String.Format("INSERT INTO [dbo].[Shifr] ([Shifr],[Description]) VALUES ('{0}','{1}')",
                        IndexText.Text,IndexDescription.Text);
                    SqlCommand comm = new SqlCommand(incertComm, Main.MainConnection);
                    adapter.InsertCommand = comm;
                    adapter.InsertCommand.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось обновить индекс", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Refresh();
        }

        private void TypeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeList.SelectedIndex != -1) TypeText.Text = (TypeList.SelectedItem as SimpleRecord).Description;
        }

        private void TypeAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TypeList.SelectedIndex != -1)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string updateComm = String.Format("UPDATE [dbo].[TypeOfAccess] SET [Type] = '{0}' WHERE ID={1}",
                        TypeText.Text, int.Parse((TypeList.SelectedItem as SimpleRecord).Shifr));
                    SqlCommand comm = new SqlCommand(updateComm, Main.MainConnection);
                    adapter.UpdateCommand = comm;
                    adapter.UpdateCommand.ExecuteNonQuery();
                }
                else
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string incertComm = String.Format("INSERT INTO [dbo].[TypeOfAccess] ([Type]) VALUES ('{0}')",
                        TypeText.Text);
                    SqlCommand comm = new SqlCommand(incertComm, Main.MainConnection);
                    adapter.InsertCommand = comm;
                    adapter.InsertCommand.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось обновить тип", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Refresh();
        }

        private void DelAuthor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( AuthorsList.SelectedIndex != -1)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string delComm = String.Format("DELETE FROM [dbo].[Author] WHERE ID={0}",
                        (AuthorsList.SelectedItem as FIO).ID);
                    SqlCommand comm = new SqlCommand(delComm, Main.MainConnection);
                    adapter.DeleteCommand = comm;
                    adapter.DeleteCommand.ExecuteNonQuery();
                    Refresh();
                }
                else return;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось удалить автора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UnselectAuthor_Click(object sender, RoutedEventArgs e)
        {
            AuthorsList.SelectedIndex = -1;
        }

        private void DelIndex_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IndexList.SelectedIndex != -1)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string delComm = String.Format("DELETE FROM [dbo].[Shifr] WHERE Shifr='{0}'",
                        (IndexList.SelectedItem as SimpleRecord).Shifr);
                    SqlCommand comm = new SqlCommand(delComm, Main.MainConnection);
                    adapter.DeleteCommand = comm;
                    adapter.DeleteCommand.ExecuteNonQuery();
                    Refresh();
                }
                else return;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось удалить шифр", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddWindow1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Main.RefreshBase();
        }

        private void Refresh() {
            //Thread.Sleep(300);
            try
            {
                var Authors = new ObservableCollection<FIO>();
                SqlCommand com = Main.MainConnection.CreateCommand();
                com.CommandText = "SELECT * FROM dbo.Author";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Authors.Add(new FIO(int.Parse(Convert.ToString(table.Rows[i]["ID"])), Convert.ToString(table.Rows[i]["Familiya"]),
                        Convert.ToString(table.Rows[i]["Imya"]), Convert.ToString(table.Rows[i]["Otchestvo"])));
                }
                this.AuthorsList.ItemsSource = Authors;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список авторов, проверьте настройки конфигурации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                var Shifr = new ObservableCollection<SimpleRecord>();
                SqlCommand com = Main.MainConnection.CreateCommand();
                com.CommandText = "SELECT * FROM dbo.Shifr";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Shifr.Add(new SimpleRecord(Convert.ToString(table.Rows[i]["Shifr"]), Convert.ToString(table.Rows[i]["Description"])));
                }
                this.IndexList.ItemsSource = Shifr;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список шифров, проверьте настройки конфигурации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                var TypeOfPublic = new ObservableCollection<SimpleRecord>();
                SqlCommand com = Main.MainConnection.CreateCommand();
                com.CommandText = "SELECT * FROM dbo.TypeOfAccess";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    TypeOfPublic.Add(new SimpleRecord(int.Parse(Convert.ToString(table.Rows[i]["ID"])), Convert.ToString(table.Rows[i]["Type"])));
                }
                this.TypeList.ItemsSource = TypeOfPublic;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список типов доступа, проверьте настройки конфигурации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                var Folder = new ObservableCollection<SimpleRecord>();
                SqlCommand com = Main.MainConnection.CreateCommand();
                com.CommandText = "SELECT * FROM dbo.Folders";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Folder.Add(new SimpleRecord(int.Parse(Convert.ToString(table.Rows[i]["ID"])), Convert.ToString(table.Rows[i]["Folder"])));
                }
                this.FolderList.ItemsSource = Folder;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список папок, проверьте настройки конфигурации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                var sourse = new ObservableCollection<SimpleRecord>();
                SqlCommand com = Main.MainConnection.CreateCommand();
                com.CommandText = "SELECT * FROM dbo.Sourse";
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable table = new DataTable();
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    sourse.Add(new SimpleRecord(int.Parse(Convert.ToString(table.Rows[i]["ID"])), Convert.ToString(table.Rows[i]["Sourse"])));
                }
                this.SourseList.ItemsSource = sourse;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить список папок, проверьте настройки конфигурации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            AuthorsList.UpdateLayout();
            IndexList.UpdateLayout();
            TypeList.UpdateLayout(); 
            FolderList.UpdateLayout();
            SourseList.UpdateLayout();
        }

        private void UnselectIndex_Click(object sender, RoutedEventArgs e)
        {
            IndexList.SelectedIndex = -1;
        }

        private void DelType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TypeList.SelectedIndex != -1)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string delComm = String.Format("DELETE FROM [dbo].[TypeOfAccess] WHERE ID={0}",
                        (TypeList.SelectedItem as SimpleRecord).ID);
                    SqlCommand comm = new SqlCommand(delComm, Main.MainConnection);
                    adapter.DeleteCommand = comm;
                    adapter.DeleteCommand.ExecuteNonQuery();
                    Refresh();
                }
                else return;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось удалить тип", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UnselectType_Click(object sender, RoutedEventArgs e)
        {
            TypeList.SelectedIndex = -1;
        }

        private void FolderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FolderList.SelectedIndex != -1) FolderText.Text = (FolderList.SelectedItem as SimpleRecord).Description;
        }

        private void FolderAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FolderList.SelectedIndex != -1)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string updateComm = String.Format("UPDATE [dbo].[Folders] SET [Folder] = '{0}' WHERE ID={1}",
                        FolderText.Text, int.Parse((FolderList.SelectedItem as SimpleRecord).Shifr));
                    SqlCommand comm = new SqlCommand(updateComm, Main.MainConnection);
                    adapter.UpdateCommand = comm;
                    adapter.UpdateCommand.ExecuteNonQuery();
                }
                else
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string incertComm = String.Format("INSERT INTO [dbo].[Folders] ([Folder]) VALUES ('{0}')",
                        FolderText.Text);
                    SqlCommand comm = new SqlCommand(incertComm, Main.MainConnection);
                    adapter.InsertCommand = comm;
                    adapter.InsertCommand.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось обновить папку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Refresh();
        }

        private void DelFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FolderList.SelectedIndex != -1)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string delComm = String.Format("DELETE FROM [dbo].[Folders] WHERE ID={0}",
                        (FolderList.SelectedItem as SimpleRecord).ID);
                    SqlCommand comm = new SqlCommand(delComm, Main.MainConnection);
                    adapter.DeleteCommand = comm;
                    adapter.DeleteCommand.ExecuteNonQuery();
                    Refresh();
                }
                else return;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось удалить папку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UnselectType1_Click(object sender, RoutedEventArgs e)
        {
            FolderList.SelectedIndex = -1;
        }

        private void SourseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SourseList.SelectedIndex != -1) SourseText.Text = (SourseList.SelectedItem as SimpleRecord).Description;
        }

        private void DelSourse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SourseList.SelectedIndex != -1)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string delComm = String.Format("DELETE FROM [dbo].[Sourse] WHERE ID={0}",
                        (SourseList.SelectedItem as SimpleRecord).ID);
                    SqlCommand comm = new SqlCommand(delComm, Main.MainConnection);
                    adapter.DeleteCommand = comm;
                    adapter.DeleteCommand.ExecuteNonQuery();
                    Refresh();
                }
                else return;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось удалить источник", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SourseAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SourseList.SelectedIndex != -1)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string updateComm = String.Format("UPDATE [dbo].[Sourse] SET [Sourse] = '{0}' WHERE ID={1}",
                        SourseText.Text, (SourseList.SelectedItem as SimpleRecord).ID);
                    SqlCommand comm = new SqlCommand(updateComm, Main.MainConnection);
                    adapter.UpdateCommand = comm;
                    adapter.UpdateCommand.ExecuteNonQuery();
                }
                else
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string incertComm = String.Format("INSERT INTO [dbo].[Sourse] ([Sourse]) VALUES ('{0}')",
                        SourseText.Text);
                    SqlCommand comm = new SqlCommand(incertComm, Main.MainConnection);
                    adapter.InsertCommand = comm;
                    adapter.InsertCommand.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось обновить источник", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Refresh();
        }
    }
}
