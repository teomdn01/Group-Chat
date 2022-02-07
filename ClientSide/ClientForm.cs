using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientSide
{
    /// <summary>
    /// Graphical user interface for the Client Side of the application
    /// </summary>
    public partial class ClientForm : Form
    {
        public TcpClient TcpClient { get; set; }
        public NetworkStream NetworkStream { get; set; }
        public ClientPacketReader Reader { get; set; }
        public string Username { get; set; }

        private static string groupTitle;
        private static string profilePicturePath;
        private Label groupPhotoLabel;
        private Label groupNameLabel;

        private Panel panelTop;
        private TextBox textBoxMessage;
        private Button buttonSendMessage;
        private TextBox textBoxConversation;

        /// <summary>
        /// ClientForm class constructor. Opens the reading/writing channels, sets the username and shows the GUI window
        /// </summary>
        /// <param name="tcpClient">The socket on which the client is connected</param>
        /// <param name="username">The username received from the previous set-up form</param>
        public ClientForm(TcpClient tcpClient, string username)
        {
            try
            {
                this.TcpClient = tcpClient;
                NetworkStream = tcpClient.GetStream();
                Reader = new ClientPacketReader(NetworkStream);
                this.Username = username;
            }
            catch (ArgumentException e)
            {
                CloseEverything(TcpClient, Reader);
            }
            catch (IOException e)
            {
                CloseEverything(TcpClient, Reader);
            }

            InitializeComponent();
            SetupWindow();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        /// <summary>
        /// Sets up the graphical user interface method
        /// </summary>
        private void SetupWindow()
        {
            panelTop = new Panel();
            panelTop.BackColor = Color.FromArgb(7, 94, 84);
            panelTop.Bounds = new Rectangle(0, 0, 450, 70);
            this.Controls.Add(panelTop);

            PictureBox pictureBoxBack = new PictureBox();
            pictureBoxBack.Location = new Point(5, 17);
            pictureBoxBack.Size = new Size(30, 30);
            pictureBoxBack.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxBack.Image = Image.FromFile(@"icons/3.png");
            panelTop.Controls.Add(pictureBoxBack);

            profilePicturePath = @"icons/charles.png";
            PictureBox pictureBoxGroupPhoto = new PictureBox();
            pictureBoxGroupPhoto.Location = new Point(40, 5);
            pictureBoxGroupPhoto.Size = new Size(60, 60);
            pictureBoxGroupPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxGroupPhoto.Image = Image.FromFile(@"icons/charles.png");
            panelTop.Controls.Add(pictureBoxGroupPhoto);

            var titlesPanel = new FlowLayoutPanel();
            titlesPanel.FlowDirection = FlowDirection.TopDown;
            groupTitle = "Charles University";
            this.groupNameLabel = new Label();
            this.groupNameLabel.Text = groupTitle;
            this.groupNameLabel.Font = new Font("SAN_SERIF", 14, FontStyle.Bold);
            this.groupNameLabel.ForeColor = Color.White;
            this.groupNameLabel.Location = new Point(110, 15);
            this.groupNameLabel.Size = new Size(100, 18);
            panelTop.Controls.Add(groupNameLabel);

            var titlesLabel = new Label();
            titlesLabel.Text = "Group chat";
            titlesLabel.Font = new Font("SAN_SERIF", 12, FontStyle.Regular);
            titlesLabel.ForeColor = Color.White;
            titlesLabel.Location = new Point(110, 35);
            titlesLabel.Size = new Size(160, 20);
            panelTop.Controls.Add(titlesLabel);


            PictureBox pictureBoxVideo = new PictureBox();
            pictureBoxVideo.Location = new Point(290, 20);
            pictureBoxVideo.Size = new Size(30, 30);
            pictureBoxVideo.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxVideo.Image = Image.FromFile(@"icons/video.png");
            panelTop.Controls.Add(pictureBoxVideo);

            PictureBox pictureBoxPhone = new PictureBox();
            pictureBoxPhone.Location = new Point(350, 20);
            pictureBoxPhone.Size = new Size(35, 30);
            pictureBoxPhone.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxPhone.Image = Image.FromFile(@"icons/phone.png");
            panelTop.Controls.Add(pictureBoxPhone);

            PictureBox pictureBoxDots = new PictureBox();
            pictureBoxDots.Location = new Point(410, 20);
            pictureBoxDots.Size = new Size(13, 25);
            pictureBoxDots.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxDots.Image = Image.FromFile(@"icons/3icon.png");
            panelTop.Controls.Add(pictureBoxDots);

            textBoxConversation = new TextBox();
            textBoxConversation.Location = new Point(5, 75);
            textBoxConversation.Size = new Size(440, 570);
            textBoxConversation.Multiline = true;
            textBoxConversation.ReadOnly = true;
            textBoxConversation.WordWrap = true;
            textBoxConversation.ScrollBars = ScrollBars.Vertical;
            
            textBoxConversation.Font = new Font("SAN_SERIF", 14, FontStyle.Regular);
            this.Controls.Add(textBoxConversation);

            textBoxMessage = new TextBox();
            textBoxMessage.Location = new Point(5, 655);
            textBoxMessage.Size = new Size(310, 40);
            textBoxMessage.Font = new Font("SAN_SERIF", 14, FontStyle.Regular);
            this.Controls.Add(textBoxMessage);
            
            buttonSendMessage = new Button();
            buttonSendMessage.Text = "Send";
            buttonSendMessage.Location = new Point(320, 655);
            buttonSendMessage.Size = new Size(123, 40);
            buttonSendMessage.BackColor = Color.FromArgb(7, 94, 84);
            buttonSendMessage.ForeColor = Color.White;
            buttonSendMessage.Font = new Font("SAN_SERIF", 14, FontStyle.Regular);
            buttonSendMessage.Click += SendMessageAction;
            this.Controls.Add(buttonSendMessage);

            this.Size = new Size(460, 735);
            this.Location = new Point(20, 20);
            this.BackColor = Color.White;
            this.Text = Username;
        }

        /// <summary>
        /// ends the username of the client to the clienthandler list
        /// </summary>
        public void SendUsername()
        {
            try
            {
                if (TcpClient != null && TcpClient.Client != null && TcpClient.Client.Connected)
                {
                    var Writer = new ClientPacketWriter();
                    Writer.WriteMessage(Username);
                    TcpClient.Client.Send(Writer.GetPacketBytes());
                }
            }
            catch (IOException e)
            {
                CloseEverything(TcpClient, Reader);
            }
        }

        /// <summary>
        /// Appends the message of the current client to their respective interface, then sends the message to the other clients
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button click event</param>
        private void SendMessageAction(object sender, EventArgs e)
        {
            try
            {
                if (TcpClient != null && TcpClient.Client != null && TcpClient.Client.Connected)
                {
                    string message = Username + ": " + textBoxMessage.Text;
                    var Writer = new ClientPacketWriter();
                    Writer.WriteMessage(message);
                    TcpClient.Client.Send(Writer.GetPacketBytes());
                    string addedText = DateTime.Now.Hour + ":" + DateTime.Now.Minute + " - You: " + textBoxMessage.Text;
                    this.textBoxConversation.AppendText(addedText + Environment.NewLine);
                    this.textBoxMessage.Text = "";
                }
            }
            catch (IOException ex)
            {
                CloseEverything(TcpClient, Reader);
            }
        }

        /// <summary>
        /// Clients wait to receive messages from the other clients.
        /// When it is able to read from the stream, it displays the message on the text area
        /// Because listening to message is a blocking method (waits for a new message to be received), it runs on a separate thread.
        /// </summary>
        public void ListenToMessage()
        {
            Task.Run(() =>
            {
                string messageFromGroupChat;
                while (TcpClient != null && TcpClient.Client != null && TcpClient.Client.Connected)
                {
                    try
                    {
                        messageFromGroupChat = Reader.ReadMessage();
                        string addedText = DateTime.Now.Hour + ":" + DateTime.Now.Minute + " - " + messageFromGroupChat + "\n";
                        this.textBoxConversation.AppendText(addedText + Environment.NewLine);
                    }
                    catch (IOException e)
                    {
                         CloseEverything(TcpClient, Reader);
                    }

                }
            });
        }
        /// <summary>
        ///  Closing the connection and the reading stream
        /// </summary>
        /// <param name="tcpClient">the socket on which the client is connected</param>
        /// <param name="reader">reading channel</param>
        public void CloseEverything(TcpClient tcpClient, ClientPacketReader reader)
        {
            if (reader != null)
            {
                reader.Close();
            }
            if (tcpClient.Client != null)
            {
                tcpClient.Client.Close();
                
            }
            if (tcpClient != null)
            {
                tcpClient.Close();
            }
        }
    }
}
