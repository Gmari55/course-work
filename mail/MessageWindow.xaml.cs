using MailKit.Net.Imap;
using MimeKit;
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

namespace mail
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        MimeMessage MimeMessage;
        User user;
        ImapClient client;
        public MessageWindow(MimeMessage mimeMessage,User user)
        {
            InitializeComponent();
            this.MimeMessage = mimeMessage;
            this.user = user;
            fill();
           
        }
        void fill()
        {
            Subject.Text = MimeMessage.Subject;
            From.Content += MimeMessage.From.OfType<MailboxAddress>().Single().Address;
            To.Content += MimeMessage.To.ToString();
            date.Content=MimeMessage.Date;
            if(MimeMessage.Body!= null)
            {

            Text.Text=MimeMessage.TextBody.ToString();
            }
        }

        private void forwardbtnclick(object sender, RoutedEventArgs e)
        {
            SendWindow sendWindow = new SendWindow(user, MimeMessage);
            sendWindow.Show();  
        }

        private void replybtnclicl(object sender, RoutedEventArgs e)
        {
            SendWindow sendWindow = new SendWindow(user, MimeMessage.From.OfType<MailboxAddress>().Single().Address);
            sendWindow.Show();
        }
    }
}
