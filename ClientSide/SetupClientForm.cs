using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientSide
{
    /// <summary>
    /// GUI class which is used to set up the settings of the Client which follows to be loaded.
    /// </summary>
    public partial class SetupClientForm : Form
    {
        private Button chooseUsernameButton;
        private TextBox usernameField;
        private Panel panel;
        /// <summary>
        /// Chooses the username for the new client that wants to join the chat.
        /// </summary>
        public SetupClientForm()
        {
            InitializeComponent();
            panel = new Panel();
            panel.Size = new Size(450, 70);


            usernameField = new TextBox();
            usernameField.Size = new Size(300, 40);
            usernameField.Location = new Point(10, 10);
            panel.Controls.Add(usernameField);

            chooseUsernameButton = new Button();
            chooseUsernameButton.Text = "Enter chat";
            chooseUsernameButton.Bounds = new Rectangle(320, 10, 123, 40);
            chooseUsernameButton.Click += EnterChat;
            panel.Controls.Add(chooseUsernameButton);

            this.Controls.Add(panel);

            this.Size = new Size(460, 100);
        }

        private void EnterChat(object sender, EventArgs e)
        {
            string username = usernameField.Text;
            if (!String.IsNullOrEmpty(username))
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 1234);

                this.Hide();
                ClientForm clientForm = new ClientForm(tcpClient, username);


                clientForm.ListenToMessage(); //runs on a different thread
                clientForm.SendUsername();

                clientForm.Closed += (s, args) => this.Close();
                clientForm.Show();
            }
            else
            {
                MessageBox.Show("Username field cannot be empty!");
            }
        }
    }
}
