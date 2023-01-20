using Microsoft.Win32;
using MimeKit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace mail
{
    public partial class SendWindow : Window
    {

        User user = new User();
        MailMessage Mail = new MailMessage();
        List<AttachmentList> attachments = new List<AttachmentList>();
        List<AttachmentList> attachmentsdel = new List<AttachmentList>();
        MimeMessage mimemessage;

        public SendWindow(User user)
        {
            InitializeComponent();
            this.user = user;

        }
        public SendWindow(User user, string address)
        {
            InitializeComponent();
            this.user = user;
            toTxtBox.Text = address.ToString();
        }
        public SendWindow(User user, MimeMessage mail)
        {
            InitializeComponent();
            this.user = user;
            Forvardmessage(mail);
        }
        private void Forvardmessage(MimeMessage mimeMessage)
        {

            bodyTxtBox.Text = "------Forwarded message------\n";
            bodyTxtBox.Text += "From: " + mimeMessage.From.ToString() + "\n";
            bodyTxtBox.Text += "Date:" + mimeMessage.Date.ToString() + "\n";
            if (mimeMessage.Subject != null)
                bodyTxtBox.Text += "Subject: " + mimeMessage.Subject.ToString() + "\n";
            else
                bodyTxtBox.Text += "Subject:\n";
            bodyTxtBox.Text += "To: " + mimeMessage.To.ToString() + "\n\n\n";
            if (mimeMessage.Body != null)
                bodyTxtBox.Text += mimeMessage.TextBody.ToString();
            mimemessage = mimeMessage;



            foreach (var attachment in mimeMessage.Attachments)
            {
                var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;


                using (var stream = File.Create(fileName))
                {
                    if (attachment is MessagePart)
                    {
                        var rfc822 = (MessagePart)attachment;

                        rfc822.Message.WriteTo(stream);

                        stream.Close();

                    }
                    else
                    {
                        var part = (MimePart)attachment;

                        part.Content.DecodeTo(stream);

                        stream.Close();
                    }

                    addatt(fileName);
                    attachmentsdel.Add(new AttachmentList(fileName));
                   

                }

            }


        }

        private void deleteattachment()
        {

            foreach (var attachment in attachmentsdel)
            {
               
                    File.Delete(attachment.FullName);
               
            }

        }
        private void SendBtnClick(object sender, RoutedEventArgs e)
        {
            if (toTxtBox.Text.Length > 0)
            {
                MailAddress mailAddress = new MailAddress(user.Email);
                Mail.From = mailAddress;
                Mail.To.Add(toTxtBox.Text);
                Mail.Subject = subjectTxtBox.Text;
                Mail.Body = bodyTxtBox.Text;




                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(user.Email, user.Password),
                    EnableSsl = true
                })
                {
                    for (int i = 0; i < attachments.Count; i++)
                    {
                        Mail.Attachments.Add(new Attachment(attachments[i].FullName));
                    }

                    client.Send(Mail);
                    
                    Mail.Dispose();

                }
                deleteattachment();
                this.Close();
            }
        }
        private void addatt(string fullname)
        {
            string name = Path.GetFileNameWithoutExtension(fullname);
            attachments.Add(new AttachmentList(fullname, name));
            Attlistbox.ItemsSource = null;
            Attlistbox.ItemsSource = attachments;
        }


        private void AttachBtnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                string fullname = dialog.FileName;
                addatt(fullname);

            }
        }



        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.RadioButton li = (sender as System.Windows.Controls.RadioButton);
            switch (li.Content.ToString())
            {
                case "Low":
                    Mail.Priority = MailPriority.Low;
                    break;
                case "Normal":
                    Mail.Priority = MailPriority.Normal;
                    break;
                case "High":
                    Mail.Priority = MailPriority.High;
                    break;
            }


        }

        private void DeleteAttachmentClick(object sender, RoutedEventArgs e)
        {
            AttachmentList item = (AttachmentList)(sender as System.Windows.Controls.Button).DataContext;
            attachments.Remove(item);
            Attlistbox.ItemsSource = null;
            Attlistbox.ItemsSource = attachments;

        }




    }
}
