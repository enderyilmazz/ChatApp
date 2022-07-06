using SocketIOClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatAppDesktopClient
{
    public partial class ChatForm : Form
    {
        public SocketIO socket = new SocketIO("http://localhost:3000");
        public class ChatData
        {
            public string message { get; set; }
            public string sender { get; set; }
        }
        public ChatForm()
        {
            InitializeComponent();
        }
        private void ChatForm_Load(object sender, EventArgs e)
        {
            typingLabel.Text = "";
            statusText.Text = "Sunucu ile bağlantı bekleniyor...";
            senderTextBox.Enabled = false;
            messageTextBox.Enabled = false;
            sendButton.Enabled = false;
            chatList.Enabled = false;
            Control.CheckForIllegalCrossThreadCalls = false;
            timer1.Interval = 1000;
            timer1.Start();
        }
        private async void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 10000;
            if (socket.Connected != true)
            {
                await socket.ConnectAsync();
                senderTextBox.Enabled = true;
                messageTextBox.Enabled = true;
                sendButton.Enabled = true;
                chatList.Enabled = true;
                statusText.Text = "Sunucu ile bağlantı kuruldu.";
            }
            await socket.EmitAsync("ack", 1);
            socket.On("chat", response =>
            {
                var chatData = response.GetValue<ChatData>();
                chatList.Items.Add(chatData.sender + ">>> " + chatData.message);
                typingLabel.Text = "";
            });
            socket.On("typing", response =>
            {
                typingLabel.Text = response.GetValue<string>() + " yazıyor...";
            });
        }
        private async void sendButton_Click(object sender, EventArgs e)
        {
            await socket.EmitAsync("chat", new ChatData
            {
                message = messageTextBox.Text,
                sender = senderTextBox.Text
            });
        }
        private async void messageTextBox_TextChanged(object sender, EventArgs e)
        {
            await socket.EmitAsync("typing", senderTextBox.Text);
        }
    }
}
