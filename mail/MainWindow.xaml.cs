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
using System.Reflection;
using System.Diagnostics.Metrics;
using System.Security.Cryptography;

namespace mail
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User user = new User();

        public ImapClient client { get; set; }
        int pagesize = 5;
        int page = 0;
        int pagecount = 0;
        public MainWindow(User user)
        {
            InitializeComponent();
            this.user = user;

            connect();

        }


        public void connect()
        {
            client = new ImapClient();

            client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);

            client.Authenticate(user.Email, user.Password);

            fillfolderlist();



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
        private void Filldatagrid()
        {


            List<Letter> leters = new List<Letter>();

            if (this.ForldersListBox.SelectedItem != null)
            {

                var folder = client.GetFolder(this.ForldersListBox.SelectedItem.ToString());

                folder.Open(FolderAccess.ReadWrite);






                for (int j = 0, i = folder.Count - 1 - page * pagesize; i >= 0 && j < pagesize; j++, i--)
                {
                    var message = folder.GetMessage(i);
                    Letter letter = new Letter();

                    letter.Id = message.MessageId;
                    letter.Subject = message.Subject;
                    letter.dateTime = message.Date.ToString("hh.mm");




                    if (message.To.ToString() == user.Email)

                        letter.email = message.From.ToString();

                    else

                        letter.email = message.To.ToString();


                    leters.Add(letter);
                }

            }


            dg.ItemsSource = leters;
        }
       
        private void Filldatagrid(IList<UniqueId> uids, IMailFolder folder)
        {
            nextbtn.IsEnabled = false;
            previousbtn.IsEnabled = false;
            List<Letter> leters = new List<Letter>();

            foreach (var i in uids)
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            Letter letter = dg.SelectedItem as Letter;


            SendWindow sendWindow = new SendWindow(user, letter.email);
            sendWindow.Show();
        }



        private void folderlistselected(object sender, SelectionChangedEventArgs e)
        {
            page = 0;
            pagecount = 0;

            nextbtn.IsEnabled = false;
            previousbtn.IsEnabled = false;

            if (this.ForldersListBox.SelectedItem != null)
            {

                var folder = client.GetFolder(this.ForldersListBox.SelectedItem.ToString());
                folder.Open(FolderAccess.ReadWrite);

                pagecount = (int)Math.Ceiling((float)(folder.Count) / (float)(pagesize));

                Filldatagrid();

            }


            if (page == 0)
                previousbtn.IsEnabled = false;
            else
                previousbtn.IsEnabled = true;
            if (page < pagecount)
                nextbtn.IsEnabled = true;
            if(pagecount==1)
                nextbtn.IsEnabled = false;
     
            if (page == pagecount)
                nextbtn.IsEnabled = false;


        }

        private void readmessage(object sender, MouseButtonEventArgs e)
        {
            Letter letter = (Letter)dg.SelectedItem;

            var folder = client.GetFolder(this.ForldersListBox.SelectedItem.ToString());
            folder.Open(FolderAccess.ReadWrite);

            var message = folder.First(m => m.MessageId == letter.Id);


            MessageWindow messageWindow = new MessageWindow(message, user);

            messageWindow.Owner = this;
            messageWindow.ShowDialog();

        }

        private void refreshbtbclick(object sender, RoutedEventArgs e)
        {

            refresh();


        }
        private void refresh()
        {
            this.ForldersListBox.SelectedItem = null;
            dg.SelectedItem = null;
            dg.ItemsSource = null;
            client.Disconnect(true);
            connect();
            page = 0;
            pagecount = 0;

        }

        private void nextbtn_Click(object sender, RoutedEventArgs e)
        {
            if (page < pagecount)
            {
                page++;
                previousbtn.IsEnabled = true;
            }

            if (page == pagecount - 1)
                nextbtn.IsEnabled = false;
            Filldatagrid();
        }

        private void previousbtn_Click(object sender, RoutedEventArgs e)
        {
            if (page > 0)
            {
                page--;
                nextbtn.IsEnabled = true;
            }

            if (page == 0)
                previousbtn.IsEnabled = false;
            Filldatagrid();

        }

        private void searchbtn_Click(object sender, RoutedEventArgs e)
        {
            if (SearchTxtBox.Text != null && ForldersListBox.SelectedItem != null)
            {
                var folder = client.GetFolder(this.ForldersListBox.SelectedItem.ToString());
                IList<UniqueId> uids = folder.Search(SearchQuery.MessageContains(SearchTxtBox.Text));
                Filldatagrid(uids, folder);
            }
            else
            {
                Filldatagrid();
            }


        }

        private void deletefoldercclick(object sender, RoutedEventArgs e)
        {
            var personal = client.GetFolder(client.PersonalNamespaces[0]);
            var folder = personal.GetSubfolder(this.ForldersListBox.SelectedItem.ToString());
            folder.Delete();
            refresh();
        }

        private void Addfolder(object sender, RoutedEventArgs e)
        {
            if (addfoldertxt.Text != "")
            {

                var folder = client.GetFolder(client.PersonalNamespaces[0]);
                var mailkit = folder.Create(addfoldertxt.Text, false);
                refresh();
            }


        }

        private void deletemessageclick(object sender, RoutedEventArgs e)
        {
            var folder = client.GetFolder(this.ForldersListBox.SelectedItem.ToString());
            Letter letter = (Letter)dg.SelectedItem;


            var uids = folder.Search(SearchQuery.HeaderContains("Message-Id", letter.Id));
            foreach (var uid in uids)
            {

                folder.AddFlags(new UniqueId[] { uid }, MessageFlags.Deleted, true);
                folder.Expunge();
            }
            refresh();
            

        }
    }
}


