using MailKit.Net.Smtp;
using MailKit.Security;
using System;
using System.Windows;

namespace mail
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new SmtpClient();
                client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                client.Authenticate(EmailTxtBox.Text, PasswordTxtBox.Password);
                User user = new User();
                user.Email = EmailTxtBox.Text;
                user.Password = PasswordTxtBox.Password;
                MainWindow window = new MainWindow(user);
                window.Show();
                this.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("uncorect login or password");
                throw;
            }

        }

        
    }
}
