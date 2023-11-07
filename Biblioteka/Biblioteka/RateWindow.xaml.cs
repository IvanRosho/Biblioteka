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
using System.Net.Mail;
using System.Net;

namespace Biblioteka
{
    /// <summary>
    /// Логика взаимодействия для RateWindow.xaml
    /// </summary>
    public partial class RateWindow : Window
    {
        int Ozenka;
        public RateWindow()
        {
            InitializeComponent();
            Ozenka = Properties.Settings.Default.Rate;
            if (Ozenka == 1) RB1.IsChecked = true;
            if (Ozenka == 2) RB2.IsChecked = true;
            if (Ozenka == 3) RB3.IsChecked = true;
            if (Ozenka == 4) RB4.IsChecked = true;
            if (Ozenka == 5) RB5.IsChecked = true;
        }

        private void ButtonRate_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxEmail.Text == "") return;

            MailAddress ToUser;
            try
            {
                ToUser = new MailAddress(TextBoxEmail.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(String.Format("Проверьте указанную почту"), "Не указана почта", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MailAddress from = new MailAddress("programma.biblioteka@bk.ru", "Rate Report");
            // кому отправляем
            MailAddress to = new MailAddress("linder881012@gmail.com");
            MailAddress tocopy = new MailAddress("angleterre2014@gmail.com");
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);

            MailMessage mToUser = new MailMessage(from, ToUser);
            mToUser.Subject = "Отзыв о программе Biblioteka";
            mToUser.Body += "Здравствуйте! Вы оставили отзыв о программе Biblioteka!\r\n";
            mToUser.Body += String.Format("Ваша оценка {1} баллов! \r\n", TextBoxEmail.Text, Ozenka);
            mToUser.Body += String.Format("Получено {0} в {1} \r\n",DateTime.Now.ToLongDateString(),DateTime.Now.ToShortTimeString());
            mToUser.Body += String.Format("\r\nТекст отзыва: \r\n");
            mToUser.Body += ReportBox.Text;
            mToUser.Body += String.Format("\r\n\r\n--\r\nДанное сообщение сформировано автоматически. Пожалуйста, не отвечайте на данное сообщение\r\n");
            m.CC.Add(tocopy);
            // тема письма
            m.Subject = "Отзыв о программе";
            // текст письма
            m.Body += String.Format("Новая оценка от {0} на {1} баллов! \r\n",TextBoxEmail.Text,Ozenka);
            m.Body += String.Format("Получено {0} в {1} \r\n",DateTime.Now.ToLongDateString(),DateTime.Now.ToShortTimeString());
            m.Body += String.Format("\r\nТекст отзыва: \r\n");
            m.Body += ReportBox.Text;

            m.IsBodyHtml = false;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 2525);
            // логин и пароль
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("programma.biblioteka@bk.ru", "UKqYSc6f");
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Timeout = 20000;
            try
            {
                smtp.Send(m);
                System.Threading.Thread.Sleep(2000);
                smtp.Send(mToUser);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Не удалось отправить отзыв, попробуйте позднее. {0}",ex.Message), "Не удалось отправить отзыв", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
        }

        private void RateWindow1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Rate = Ozenka;
            Properties.Settings.Default.Save();
        }

        private void RB1_Checked(object sender, RoutedEventArgs e)
        {
            if (RB1.IsChecked == true) Ozenka = 1;
            if (RB2.IsChecked == true) Ozenka = 2;
            if (RB3.IsChecked == true) Ozenka = 3;
            if (RB4.IsChecked == true) Ozenka = 4;
            if (RB5.IsChecked == true) Ozenka = 5;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
