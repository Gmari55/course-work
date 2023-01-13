using MailKit.Net.Imap;
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
            using(var client =new ImapClient())
            { 
                client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
                client.Authenticate(EmailTxtBox.Text, PasswordTxtBox.Password);
                
                if(client.IsAuthenticated)
                {

                User user = new User();
                user.Email = EmailTxtBox.Text;
                user.Password = PasswordTxtBox.Password;
                MainWindow window = new MainWindow(user);
                window.Show();
                this.Close();
                }
            }
            

        }

        
    }
}
