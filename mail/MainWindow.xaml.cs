using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MailKit;
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
using System.Windows.Controls.Primitives;
using System.Threading;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Asn1.X509;

namespace mail
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User user = new User();
       
        public ImapClient client { get; set; }
        public MainWindow(User user)
        {
            InitializeComponent();
            this.user = user;

            connect();

        }
        int pagesize = 15;
        int page = 0;
        private async void Filldatagrid(IMailFolder folder)
        {
            List<Letter> leters = new List<Letter>();




            for (int j = 0, i = folder.Count - 1 - page * pagesize; i >= 0 && j < pagesize; j++, i--)
            {
                var message = folder.GetMessage(i);
                Letter letter = new Letter();
                
               
                letter.Subject = message.Subject;
                letter.dateTime = message.Date.ToString("hh.mm");


               

                if (message.To.ToString() == user.Email)

                    letter.email = message.From.ToString();

                else

                    letter.email = message.To.ToString();


                leters.Add(letter);
            }



            dg.ItemsSource = leters;
        }

        private void SendBtnClick(object sender, RoutedEventArgs e)
        {
            SendWindow window = new SendWindow(user);
            window.Show();
        }
        void fillfolderlist()
        {
            this.ForldersListBox.Items.Clear(); 
            var folders = client.GetFolders(client.PersonalNamespaces[0]);

            foreach (var item in folders)
            {
                this.ForldersListBox.Items.Add(item.FullName);

            }
        }
        public void connect()
        {
            client = new ImapClient();
            
                client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);

            client.Authenticate(user.Email, user.Password);
         
                fillfolderlist();


            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string mail;
            Letter letter = dg.SelectedItem as Letter;

            SendWindow sendWindow = new SendWindow(user, letter.email);
            sendWindow.Show();
        }



        private void folderlistselected(object sender, SelectionChangedEventArgs e)
        {

            if(((ListBox)sender).SelectedItem!=null)
            {

            var folder = client.GetFolder(((ListBox)sender).SelectedItem.ToString());

             folder.Open(FolderAccess.ReadOnly);
            Filldatagrid(folder);
             folder.Close();
            }


        }

        private void readmessage(object sender, MouseButtonEventArgs e)
        {
            Letter letter = (Letter)dg.SelectedItem;

            var folder = client.GetFolder(this.ForldersListBox.SelectedItem.ToString());
            folder.Open(FolderAccess.ReadOnly);

            var message =folder.First(m=>m.Subject== letter.Subject);
           
            MessageWindow messageWindow = new MessageWindow(message,user);
            
            messageWindow.Owner= this;
            messageWindow.ShowDialog();
            
        }

        private void refrechbtbclick(object sender, RoutedEventArgs e)
        {
            this.ForldersListBox.SelectedItem= null;
            dg.SelectedItem= null;
            dg.ItemsSource= null;
           client.Disconnect(true);
            connect();

            
        }
    }
}


