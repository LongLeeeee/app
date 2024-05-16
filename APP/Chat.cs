using APP.Resources;
using Bunifu.UI.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace APP
{
    public partial class Chat : Form
    {
        private Thread receiveThread;
        private TcpClient client;
        private string username;
        private ChatlistUser[] chatlistUsers;
        private UserFriend[] listFriends;
        private StreamReader reader;
        private bool isRunning = false;
        private StreamWriter writer;
        Image receivedImage;
        //list bạn bè tạm
        private string[] friendList;
        private string[] userList;
        string roomList;
        // tạo ra 1 list các pair chatlistuser và flowlayoutpenl
        private Dictionary<ChatlistUser, FlowLayoutPanel> chatListUserToFlowLayoutPanelMap = new Dictionary<ChatlistUser, FlowLayoutPanel>();
        private Dictionary<ChatlistUser, string> keyValuePairs = new Dictionary<ChatlistUser, string>();
        private Dictionary<string, UserFriend> keyValuePairs2 = new Dictionary<string, UserFriend>();
        public Chat(TcpClient tcpClient, string username)
        {
            this.client = tcpClient;
            this.username = username;
            this.isRunning = true;
            InitializeComponent();
            this.Load += LoadDatatoApp;
        }

        private void LoadData()
        {
            bunifuLabel1.Text = username;
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;
            //yêu cầu danh sách ds user
            writer.WriteLine("ListUser");

            //Nhận ds user
            string temp1 = reader.ReadLine();
            userList = temp1.Split('|');

            //yêau cầu nhận ds bạn bè
            writer.WriteLine("Listfriend");


            //nhận ds bạn bè
            string temp = reader.ReadLine();
            if (temp == "Null")
            {
            }
            else
            {
                friendList = temp.Split('|');
            }

            detailAn();
            Colorcolumn1();
            listfriendshow();
            listconversation();

            convertRoomList();
            writer.WriteLine("LoadMessage");
            writer.WriteLine(roomList);

            string receive = "";
            string receive1 = "";
            //bool check = true;
            string[] temp2;
            while (receive != "Null")
            {
                receive = reader.ReadLine();
                foreach (var item in keyValuePairs)
                {
                    if (item.Value == receive)
                    {
                        receive1 = "";
                        receive1 = reader.ReadLine();
                        temp2 = receive1.Split('|');
                        foreach (var item2 in temp2)
                        {
                            var remessage = new ReMessage();
                            var semessage = new SeMessage();
                            if (!string.IsNullOrEmpty(item2))
                            {
                                string checkSender = item2.Substring(0, item2.IndexOf(":"));
                                if (checkSender == username)
                                {
                                    semessage.message = item2;
                                    chatListUserToFlowLayoutPanelMap[item.Key].Controls.Add(semessage);
                                }
                                else
                                {
                                    remessage.messgae = item2;
                                    chatListUserToFlowLayoutPanelMap[item.Key].Controls.Add(remessage);
                                }
                            }
                        }
                        break;
                    }
                }
            }

            Thread thread = new Thread(Receive);
            thread.Start();
            thread.IsBackground = true;
        }
        //cấu trúc 1 tin nhắn được gửi đi 
        private void LoadDatatoApp(object sender, EventArgs e)
        {
            Thread loadAppThread = new Thread(() => ShowLoadingMessageBoxAsync());
            loadAppThread.Start();

            LoadData();

            loadAppThread.Join();
        }
        private void ShowLoadingMessageBoxAsync()
        {
            // Hiển thị MessageBox thông báo chờ
            MessageBox.Show("Vui lòng đợi, đang tải dữ liệu...", "Đang tải", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        class tinNhan
        {
            public string sender { get; set; }
            public string contentMess { get; set; }
            public string receiver { get; set; }
            public string roomkey { get; set; }
        }
        private void detailAn()
        {
            Detail.Visible = false;
            listFriend.Visible = false;
            MoreConversation.Visible = false;
            searchchat.Visible = false;
        }
        private void Colorcolumn1()
        {
           
                conversation.BackColor = Color.LightGray;
            
        }
        private void Chat_Load(object sender, EventArgs e)
        {
            
        }

        private void bunifuPanel1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuPanel4_Click(object sender, EventArgs e)
        {
           
          
            
        }
        
        private void bunifuPictureBox1_Click(object sender, EventArgs e)
        {
           
            if (Detail.Visible == false)
            {
                Detail.Visible = true;
                listFriend.Visible = false;
                Chatlist.Visible = false;
                conversation.BackColor = Color.WhiteSmoke;
                add.BackColor = Color.WhiteSmoke;

            }
            else if (Detail.Visible == true)
            {
                Detail.Visible = false;
            }

        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            ChangeNameAndImae form = new ChangeNameAndImae();
            form.ShowDialog();
        }

        private void listFriend_Click(object sender, EventArgs e)
        {
            
        }
        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            if (Chatlist.Visible == false)
            {
                Chatlist.Visible = true;
                listFriend.Visible = false;
                Detail.Visible = false;
                add.BackColor = Color.WhiteSmoke;
                conversation.BackColor= Color.LightGray;
            }
            else
            {
                Chatlist.Visible = false;
                conversation.BackColor = Color.WhiteSmoke;
            }
            

          
        }
        private void listfriendshow()
        {
            if (friendList != null)
            {
                if (friendList.Length == 0)
                {
                    return;
                }
                listFriends = new UserFriend[userList.Length];
                for (int i = 0; i < listFriends.Length; i++)
                {
                    bool isExist = false;
                    foreach (var item in friendList)
                    {
                        if (userList[i] == item)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist)
                    {
                        if (userList[i] != "")
                        {
                            if (userList[i] != username)
                            {
                                listFriends[i] = new UserFriend(client, username);
                                Image image = Image.FromFile("Resources\\4.jpg");
                                listFriends[i].userimage = image;
                                listFriends[i].username = userList[i];
                                keyValuePairs2.Add(userList[i], listFriends[i]);
                                if (flowLayoutPanelListfriend.Controls.Count < 0)
                                {
                                    flowLayoutPanelListfriend.Controls.Clear();
                                }
                                else
                                {
                                    flowLayoutPanelListfriend.Controls.Add(listFriends[i]);
                                }
                            }
                        }
                    }
                }
            }
            else if (friendList == null)
            {
                listFriends = new UserFriend[userList.Length];
                for (int i = 0; i < listFriends.Length; i++)
                {
                    if (userList[i] != "")
                    {
                        if (userList[i] != username)
                        {
                            listFriends[i] = new UserFriend(client, username);
                            Image image = Image.FromFile("Resources\\4.jpg");
                            listFriends[i].userimage = image;
                            listFriends[i].username = userList[i];
                            keyValuePairs2.Add(userList[i], listFriends[i]);
                            if (flowLayoutPanelListfriend.Controls.Count < 0)
                            {
                                flowLayoutPanelListfriend.Controls.Clear();
                            }
                            else
                            {
                                flowLayoutPanelListfriend.Controls.Add(listFriends[i]);
                            }
                        }
                    }
                }
            }
        }
        private void listconversation()
        {
            if (friendList != null)
            {
                chatlistUsers = new ChatlistUser[friendList.Length];
                for (int i = 0; i < chatlistUsers.Length; i++)
                {
                    if (friendList[i] != "")
                    {
                        chatlistUsers[i] = new ChatlistUser();
                        Image image = Image.FromFile("Resources\\4.jpg");
                        // tạo ra 1 chatlistuser
                        chatlistUsers[i].username = friendList[i];
                        chatlistUsers[i].userimage = image;
                        // tạo ra 1 flowlayoutpanel tương ứng với chatlistuser ở trên
                        FlowLayoutPanel tempFlowLayoutPanel = createFlowlayoutPanel();
                        getRoomKey(username, friendList[i]);
                        keyValuePairs.Add(chatlistUsers[i], getRoomKey(username, friendList[i]));
                        chatListUserToFlowLayoutPanelMap.Add(chatlistUsers[i], tempFlowLayoutPanel);
                        // gán sự kiện hiển thị flowlayoutpanel khi ấn vào chatlistuser tương ứng
                        chatlistUsers[i].MouseDown += click_show_panel;
                        ContactNameConversation.Text = "Unknow";
                        ContactNameMore.Text = "Unknow";
                        // thêm chatlistuser vào panel bên trái 
                        ChatlistFlowPanel.Controls.Add(chatlistUsers[i]);
                    }
                }
            }
        }
        private void click_show_panel(object sender, EventArgs e)
        {
            //bắt sự kiện xem chatlistuser nào được ấn
            ChatlistUser clickedChatListUser = (ChatlistUser)sender;    
            if (chatListUserToFlowLayoutPanelMap.ContainsKey(clickedChatListUser))
            {
                // tìm flowlayoutpanel tương ứng để hiển thị lên
                foreach (var item in chatListUserToFlowLayoutPanelMap)
                {
                    if (item.Key == clickedChatListUser)
                    {
                        item.Value.Visible = true;
                        bunifuPanel12.Visible = true;
                        bunifuTextBox3.Clear();
                        ContactNameConversation.Text = item.Key.username;
                        ContactNameMore.Text = item.Key.username;
                    }
                    else
                    {
                        item.Value.Visible = false;
                    }
                }
            }
        }
        // tạo ra một flowlayoutpanel 
        private FlowLayoutPanel createFlowlayoutPanel()
        {
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.Location = new Point(0, 183);
            flowLayoutPanel.AutoScroll = true;
            flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel.AllowDrop = true;
            flowLayoutPanel.BackColor = Color.White;
            flowLayoutPanel.Dock = DockStyle.Fill;
            panel2.Controls.Add(flowLayoutPanel);
            //flowLayoutPanel.AutoSize = true;
            //flowLayoutPanel.Size = new Size(225, 718);
            flowLayoutPanel.Visible = false;
            return flowLayoutPanel;
        }
        private void add_Click(object sender, EventArgs e)
        {
            if (listFriend.Visible == false || Detail.Visible == true)
            {
                Detail.Visible = false;
                Chatlist.Visible = false;
                listFriend.Visible = true;
                add.BackColor = Color.LightGray;
                conversation.BackColor = Color.WhiteSmoke;

            }
            else if (listFriend.Visible == true)
            {
                listFriend.Visible = false;
               add.BackColor = Color.WhiteSmoke;
            }

        }

        private void More_Click(object sender, EventArgs e)
        {
            if(MoreConversation.Visible == false)
            {
                MoreConversation.Visible = true;
            }
            else
            {
                MoreConversation.Visible = false;
            }
            
        }
        private void convertRoomList()
        {
            if (friendList != null)
            {
                foreach (var item in friendList)
                {
                    string temp = getRoomKey(username, item);
                    roomList += temp + "|";
                }
            }
        }
        private string getRoomKey(string username1,string username2 )
        {
            int total = 0;

            foreach (char item in username1)
            {
                total += (int)item;
            }
            foreach (char item in username2)
            {
                total += (int)item;
            }
            return total.ToString();
        }
        // gửi tin nhắn đển server
        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            tinNhan newMsg = new tinNhan { sender = username, contentMess = bunifuTextBox3.Text, receiver = ContactNameConversation.Text, roomkey = getRoomKey(username,ContactNameConversation.Text)};
            string stringMess = JsonConvert.SerializeObject(newMsg);
            writer.WriteLine("Message");
            writer.WriteLine(stringMess);
            string messDisplay = $"{username}: " + bunifuTextBox3.Text + "\r\n";
            foreach (var item in chatListUserToFlowLayoutPanelMap)
            {
                if (item.Key.username == $"{newMsg.receiver}")
                {

                    SeMessage se = new SeMessage();
                    //System.Windows.Forms.Label nl1 = new System.Windows.Forms.Label();
                    // nl1.Text = messDisplay;
                    se.message = messDisplay;
                    item.Value.Controls.Add(se);
                    bunifuTextBox3.Clear();
                }
            }
        }

        private void bunifuPanel11_Click(object sender, EventArgs e)
        {

        }

        

        private void bunifuPanel5_Click(object sender, EventArgs e)
        {

        }

        private void bunifuPanel10_Click(object sender, EventArgs e)
        {

        }

        private void bunifuImageButton2_Click_1(object sender, EventArgs e)
        {
            if (searchchat.Visible == false)
            {
                searchchat.Visible = true;

            }
            else
            {
                searchchat.Visible = false;
            }
        }

        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
            // gửi yêu cầu ngắt kết nối đến server
            string disconnect = "quit";
            writer.WriteLine(disconnect);
            // đóng kết nối tại client
            client.Close();

            // dừng luồng nhận tin nhắn từ server
            Application.Exit();
        }

        // nhận tin nhắn từ server 
        private void Receive()
        {
            try
            {
                while (isRunning)
                {
                    string messageFromServer = reader.ReadLine();

                    if (messageFromServer == "Message")
                    {
                        string newMsg = reader.ReadLine();
                        tinNhan tinNhan = JsonConvert.DeserializeObject<tinNhan>(newMsg);
                        foreach (var item in chatListUserToFlowLayoutPanelMap)
                        {
                            if (item.Key.username == tinNhan.sender)
                            {
                                Invoke(new Action(() =>
                                {
                                    ReMessage re = new ReMessage();
                                    re.messgae = tinNhan.sender + ": " + tinNhan.contentMess;
                                    //System.Windows.Forms.Label lb = new System.Windows.Forms.Label();
                                    //lb.Text = tinNhan.sender + ": " + tinNhan.contentMess;
                                    item.Value.Controls.Add(re);
                                }));
                            }
                        }
                    }
                    else if (messageFromServer == "Image")
                    {
                        // StreamReader reader1 = new StreamReader(client.GetStream());
                        string senderName = reader.ReadLine();
                        string imageData = reader.ReadLine();

                       
                        receivedImage = StringToImage(imageData);
                        string savePath = "Resources\\";
                        string fileName = $"{senderName}_{DateTime.Now:yyyyMMddHHmmss}.png";
                        string fullPath = System.IO.Path.Combine(savePath, fileName);

                        try
                        {
                            
                            if (!System.IO.Directory.Exists(savePath))
                            {
                                System.IO.Directory.CreateDirectory(savePath);
                            }

                          
                            using (var bmp = new Bitmap(receivedImage))
                            {
                                bmp.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                            }

                            //MessageBox.Show($"Image saved successfully at {fullPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show($"An error occurred while saving the image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }



                        foreach (var item in chatListUserToFlowLayoutPanelMap)
                        {
                            if (item.Key.username == senderName)
                            {
                                Invoke(new Action(() =>
                                {
                                    ReImage re = new ReImage();
                                    re.image = receivedImage;
                                    /*PictureBox newPictureBox = new PictureBox();
                                    newPictureBox.Size = new Size(250, 250);
                                    newPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                                    newPictureBox.Image = receivedImage;*/
                                    item.Value.Controls.Add(re);
                                }));
                            }
                        }
                    }
                    else if (messageFromServer == "File")
                    {
                        NetworkStream networkStream = client.GetStream();
                        string sendername = reader.ReadLine();
                        string filename = reader.ReadLine();
                        long filesize = Convert.ToInt64(reader.ReadLine());
                        string filePath = Path.Combine("Resources\\", filename);
                        byte[] buffer = new byte[52428800];
                        int bytesRead;
                        long bytesReceived = 0;
                        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {

                            while (bytesReceived < filesize &&
                               (bytesRead = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                            {

                                fs.Write(buffer, 0, bytesRead);
                                bytesReceived += bytesRead;

                            }

                        }
                        foreach (var item in chatListUserToFlowLayoutPanelMap)
                        {
                            if (item.Key.username == sendername)
                            {
                                Invoke(new Action(() =>
                                {
                                    ReFile re = new ReFile();
                                    re.filename = filename;
                                    /*PictureBox newPictureBox = new PictureBox();
                                    newPictureBox.Size = new Size(250, 250);
                                    newPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                                    newPictureBox.Image = receivedImage;*/
                                    item.Value.Controls.Add(re);
                                }));
                            }
                        }

                    }
                    else if (messageFromServer == "AddFriend")
                    {
                        string sender = reader.ReadLine();
                        if (keyValuePairs2.ContainsKey(sender))
                        {
                            Invoke(new Action(() =>
                            {
                                keyValuePairs2[sender].setButton("Chấp nhận");
                            }));
                        }
                    }
                    else if (messageFromServer == "AcceptedSuccessfullyForReceiver")
                    {
                        string userName = reader.ReadLine();
                        Invoke(new Action(() =>
                        {
                            ChatlistUser temp = new ChatlistUser();
                            Image image = Image.FromFile("Resources\\4.jpg");
                            // tạo ra 1 chatlistuser
                            temp.username = userName;
                            temp.userimage = image;
                            // tạo ra 1 flowlayoutpanel tương ứng với chatlistuser ở trên
                            FlowLayoutPanel tempFlowLayoutPanel = createFlowlayoutPanel();
                            getRoomKey(username, userName);
                            keyValuePairs.Add(temp, getRoomKey(username, userName));
                            chatListUserToFlowLayoutPanelMap.Add(temp, tempFlowLayoutPanel);
                            // gán sự kiện hiển thị flowlayoutpanel khi ấn vào chatlistuser tương ứng
                            temp.MouseDown += click_show_panel;
                            // thêm chatlistuser vào panel bên trái 
                            ChatlistFlowPanel.Controls.Add(temp);
                        }));
                        if (keyValuePairs2.ContainsKey(userName))
                        {
                            Invoke(new Action(() =>
                            {
                                flowLayoutPanelListfriend.Controls.Remove(keyValuePairs2[userName]);
                            }));
                        }
                    }
                    else if (messageFromServer == "AcceptedSuccessfullyForSender")
                    {
                        string userName = reader.ReadLine();
                        Invoke(new Action(() =>
                        {
                            ChatlistUser temp = new ChatlistUser();
                            Image image = Image.FromFile("Resources\\4.jpg");
                            // tạo ra 1 chatlistuser
                            temp.username = userName;
                            temp.userimage = image;
                            // tạo ra 1 flowlayoutpanel tương ứng với chatlistuser ở trên
                            FlowLayoutPanel tempFlowLayoutPanel = createFlowlayoutPanel();
                            getRoomKey(username, userName);
                            keyValuePairs.Add(temp, getRoomKey(username, userName));
                            chatListUserToFlowLayoutPanelMap.Add(temp, tempFlowLayoutPanel);
                            // gán sự kiện hiển thị flowlayoutpanel khi ấn vào chatlistuser tương ứng
                            temp.MouseDown += click_show_panel;
                            // thêm chatlistuser vào panel bên trái 
                            ChatlistFlowPanel.Controls.Add(temp);
                        }));
                        if (keyValuePairs2.ContainsKey(userName))
                        {
                            Invoke(new Action(() =>
                            {
                                flowLayoutPanelListfriend.Controls.Remove(keyValuePairs2[userName]);
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        private Image StringToImage(string imageDataString)
        {
            try
            {
                // Chuyển đổi chuỗi base64 thành mảng byte
                byte[] imageBytes = Convert.FromBase64String(imageDataString);
                // Tạo một MemoryStream từ mảng byte
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    // Tạo một đối tượng hình ảnh từ MemoryStream
                    Image image = Image.FromStream(ms);
                    return image;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return null;
            }
        }
        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            writer.WriteLine("LogOut");
            LogIn lg = new LogIn();
            lg.Show();
            this.Close();
        }

        private void bunifuTextBox1_TextChanged(object sender, EventArgs e)
        {
            foreach (var item in username)
            {
                
            }
        }

        private void bunifuTextBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void bunifuImageButton1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bunifuTextBox2.Text))
            {
                foreach (ChatlistUser item in chatlistUsers)
                {
                    if (item != null)
                    {
                        item.Visible = true;
                    }
                }
            }
            foreach (ChatlistUser item in chatlistUsers)
            {
                if (item != null)
                {
                    if (item.username.IndexOf(bunifuTextBox2.Text) != -1)
                    {
                        item.Visible = true;
                    }
                    else
                    {
                        item.Visible = false;
                    }
                }
            }
        }
        private void bunifuButton4_Click(object sender, EventArgs e)
        {
            
        }

        private void bunifuTextBox4_Enter(object sender, EventArgs e)
        {
            FlowLayoutPanel temp = new FlowLayoutPanel();
            foreach (var item in chatListUserToFlowLayoutPanelMap)
            {
                if (item.Key.username == ContactNameConversation.Text)
                {
                    temp = item.Value;
                    break;
                }
            }
            if (temp != null)
            {
                /*FlowLayoutPanel temp = new FlowLayoutPanel();
                foreach (var item in chatListUserToFlowLayoutPanelMap)
                {
                    if (item.Key.username == ContactNameConversation.Text)
                    {
                        temp = item.Value;
                        break;
                    }
                }
                if (temp != null)
                {
                    foreach (Control ctl in temp.Controls)
                    {
                        if (bunifuButton4.Text.IndexOf(ctl.Text) != -1)
                        {
                            ctl.Visible = true;
                        }
                        else
                        {
                            ctl.Visible = false;
                        }
                    }
                }
                if (string.IsNullOrEmpty(bunifuTextBox4.Text))
                {
                    foreach (Control ctl in temp.Controls)
                    {
                        ctl.Visible = true;
                    }
                } */
                foreach (Control control in temp.Controls)
                {
                    if (control is SeMessage se && se.message.IndexOf(bunifuTextBox4.Text) != -1)
                    {
                        se.Visible = true;
                    }
                    else if (control is ReMessage re && re.messgae.IndexOf(bunifuTextBox4.Text) != -1)
                    {
                        re.Visible = true;
                    }
                    else
                    {
                        control.Visible = string.IsNullOrEmpty(bunifuTextBox4.Text);
                    }
                }
            }
        }

        private void bunifuTextBox4_TextChange(object sender, EventArgs e)
        {
            FlowLayoutPanel temp = new FlowLayoutPanel();
            foreach (var item in chatListUserToFlowLayoutPanelMap)
            {
                if (item.Key.username == ContactNameConversation.Text)
                {
                    temp = item.Value;
                    break;
                }
            }
            if (temp != null)
            {
                
                foreach (Control control in temp.Controls)
                {
                    if (control is SeMessage se && se.message.IndexOf(bunifuTextBox4.Text) != -1)
                    {
                        se.Visible = true;
                    }
                    else if (control is ReMessage re && re.messgae.IndexOf(bunifuTextBox4.Text) != -1)
                    {
                        re.Visible = true;
                    }
                    else
                    {
                        control.Visible = string.IsNullOrEmpty(bunifuTextBox4.Text);
                    }
                }
            }
        }
        private string ImageToString(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
        private void bunifuSendFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var item in chatListUserToFlowLayoutPanelMap)
                    {
                        if (item.Key.username == $"{ContactNameConversation.Text}")
                        {
                            SeImage se = new SeImage();
                            PictureBox pictureBox = new PictureBox();
                            pictureBox.Size = new Size(250, 250);
                            //pictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
                            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                            pictureBox.Image = Image.FromFile(ofd.FileName);
                            string savePath = "Resources\\" + Path.GetFileName(ofd.FileName); // Thay đổi đường dẫn tại đây

                            // Lưu ảnh vào đường dẫn
                            //pictureBox.Image.Save(savePath);

                            se.image = Image.FromFile(ofd.FileName);
                            se.image.Save(savePath);
                            Invoke(new Action(() =>
                            {

                                item.Value.Controls.Add(se);
                            }));
                            try
                            {

                                //string ImageDataString = ImageToString(pictureBox.Image);


                                StreamWriter writer = new StreamWriter(client.GetStream());
                                string imageData = ImageToBase64String(se.image);
                                //buffer = ImageToByteArray(pictureBox.Image);
                                writer.AutoFlush = true;
                                writer.WriteLine("Image");
                                writer.WriteLine(username + "|" + ContactNameConversation.Text);
                                //writer.WriteLine(Path.GetFileName(ofd.FileName));

                                //writer.WriteLine(ImageDataString);
                                writer.WriteLine(imageData);


                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    }
                }
            }
        }
        private void bunifuImageButton6_Click(object sender, EventArgs e)
        {


            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    foreach (var item in chatListUserToFlowLayoutPanelMap)
                    {
                        if (item.Key.username == $"{ContactNameConversation.Text}")
                        {
                            try
                            {
                                FileInfo fileInfo = new FileInfo(filePath);
                                string fileName = fileInfo.Name;
                                long fileSize = fileInfo.Length;

                                SeFile sef = new SeFile();
                                sef.filename = fileName;
                                Invoke(new Action(() =>
                                {

                                    item.Value.Controls.Add(sef);
                                }));
                                // Get the network stream from the client


                                // Create a StreamWriter to write metadata to the network stream
                                StreamWriter writer = new StreamWriter(client.GetStream());
                                
                                    writer.AutoFlush = true;
                                    writer.WriteLine("File");
                                    writer.WriteLine(username + "|" + ContactNameConversation.Text);
                                    writer.WriteLine(fileName);
                                    writer.WriteLine(fileSize.ToString());
                                
                                    byte[] buffer = new byte[52428800]; // 50MB buffer
                                    int bytesRead;
                                    NetworkStream stream = client.GetStream();

                                // Open the file and send its content using BinaryWriter
                                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                { 
                               
                                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        stream.Write(buffer, 0, bytesRead);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // Handle or log the exception
                                MessageBox.Show($"An error occurred while sending the file: {ex.Message}");
                            }
                        }
                    }
                }
            }
        }   
        
        public static string ImageToBase64String(Image image)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Lưu hình ảnh vào MemoryStream dưới dạng JPEG
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    // Chuyển đổi sang chuỗi base64
                    byte[] imageBytes = ms.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return null;
            }
        }
        private void bunifuImageButton4_Click(object sender, EventArgs e)
        {
            AddGroup form = new AddGroup();
            form.ShowDialog();
        }

        private void bunifuImageButton5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bunifuTextBox1.Text))
            {
                if (listFriends.Length != 0)
                {
                    foreach (UserFriend item in listFriends)
                    {
                        if (item != null)
                        {
                            item.Visible = true;
                        }
                    }
                }
                else
                {

                }
            }
            foreach (UserFriend item in listFriends)
            {
                if (item != null)
                {
                    if (item.username.IndexOf(bunifuTextBox1.Text) != -1)
                    {
                        item.Visible = true;
                    }
                    else
                    {
                        item.Visible = false;
                    }
                }
            }
        }

    }
}