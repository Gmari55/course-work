using MailKit.Net.Imap;
using MimeKit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

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
        List<string> list = new List<string>();
        public MessageWindow(MimeMessage mimeMessage, User user)
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
            date.Content = MimeMessage.Date;
            if (MimeMessage.Body != null)
            {

                Text.Text = MimeMessage.TextBody.ToString();
            }

            foreach (var attachment in MimeMessage.Attachments)
            {
                var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                list.Add(fileName);
            }
            Attlistbox.ItemsSource = list;
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

        private void DownloadAttClick(object sender, RoutedEventArgs e)
        {
            string name= (string)(sender as System.Windows.Controls.Button).DataContext;

            foreach (var attachment in MimeMessage.Attachments)
            {
                var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                if (fileName == name)
                {
                    var fb = new FolderBrowserDialog();
                   string fullname;
                    fb.
                    using (var stream = File.Create(fileName))
                    {
                        if (attachment is MessagePart)
                        {
                            var rfc822 = (MessagePart)attachment;

                            rfc822.Message.WriteTo(stream);
                        }
                        else
                        {
                            var part = (MimePart)attachment;

                            part.Content.DecodeTo(stream);
                        }
                    }
                }
            }

        }
    }
}
