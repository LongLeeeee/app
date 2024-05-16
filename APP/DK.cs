using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace APP
{
    public partial class DK : Form
    {
        public class Data
        {
            public string email { get; set; }
            public string password { get; set; }
            public string userName { get; set; }
        }
        public DK(TcpClient tcpClient)
        {
            InitializeComponent();
            this.tcpClient = tcpClient;
            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream());
            writer.AutoFlush = true;
            this.OTP = GenerateCode();
        }
        string OTP;
        TcpClient tcpClient;
        StreamReader reader;
        StreamWriter writer;
        private void baoloi()
        {
            label7.Visible = true;
        }

        private void DK_Load(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            LogIn form = new LogIn();
            form.Visible = true;
            this.Close();

        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            new Thread(() => Application.Run(new LogIn())).Start();
            this.Close();
        }
        private void Register()
        {
            Data signup = new Data()
            {
                userName = bunifuTextBox1.Text,
                email = bunifuTextBox3.Text,
                password = bunifuTextBox4.Text,
            };
            // gửi thông điệp yêu cầu đăng kí và thông tin đắng kí
            string signupString = JsonConvert.SerializeObject(signup);
            writer.WriteLine("Register");
            writer.WriteLine(signupString);

            //Nhận phản hồi từ server
            string responseFromServer = reader.ReadLine();
            string username = responseFromServer.Substring(0, responseFromServer.IndexOf(":"));
            string response = responseFromServer.Substring(responseFromServer.IndexOf(":") + 1);
            if (response.CompareTo("Register successfully") == 0)
            {
                Invoke(new Action(() =>
                {
                    Chat chat = new Chat(tcpClient, username);
                    chat.Show();
                    this.Hide();
                }));
            }
            else
            {
                Invoke(new Action(() =>
                {
                    MessageBox.Show("Khong thanh cong");
                    bunifuTextBox1.Clear();
                    bunifuTextBox2.Clear();
                    bunifuTextBox3.Clear();
                    bunifuTextBox4.Clear();
                }));
            }
        }

        private void bunifuTextBox4_TextChanged(object sender, EventArgs e)
        {
            string password = bunifuTextBox4.Text;

            if (password.Length < 8)
            {
                lblPasswordError.Text = "Mật khẩu cần ít nhất 8 ký tự.";
                return;
            }
            else if (!password.Any(char.IsUpper))
            {
                lblPasswordError.Text = "Mật khẩu cần ít nhất một ký tự hoa.";
                return;
            }
            else if (!password.Any(char.IsLower))
            {
                lblPasswordError.Text = "Mật khẩu cần ít nhất một ký tự thường.";
                return;
            }
            else if (!password.Any(char.IsDigit))
            {
                lblPasswordError.Text = "Mật khẩu cần ít nhất một số.";
                return;
            }
            else
            {
                // Nếu mật khẩu đáp ứng tất cả các điều kiện, xóa thông báo lỗi
                lblPasswordError.Text = "";
            }
        }

        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Mã OTP sẽ được gửi qua Email của bạn!");
            new Thread(() => SendEmail(bunifuTextBox3.Text.Trim(), OTP)).Start();
        }
        private void SendEmail(string toEmail, string code)
        {
            // Địa chỉ email và mật khẩu của người gửi
            string fromAddress = "22520985@gm.uit.edu.vn";
            string password = "1991419869";
                
            // Nội dung email
            string subject = "OTP";
            string body = $"Chào bạn,\n\nOTP của bạn là: {code}";

            try
            {
                // Tạo đối tượng MailMessage
                MailMessage mail = new MailMessage(fromAddress, toEmail, subject, body);

                // Cấu hình SmtpClient
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress, password)
                };

                // Gửi email
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu gửi email không thành công
                MessageBox.Show($"Gửi email thất bại: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public string GenerateCode()
        {
            const string chars = "0123456789";
            var random = new Random();
            var code = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
            return code;
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            // kiểm các textbox
            if (string.IsNullOrEmpty(bunifuTextBox1.Text) || string.IsNullOrEmpty(bunifuTextBox2.Text) ||
                string.IsNullOrEmpty(bunifuTextBox3.Text) || string.IsNullOrEmpty(bunifuTextBox4.Text)
                || (bunifuTextBox2.Text != bunifuTextBox4.Text))
            {
                baoloi();
                bunifuTextBox1.Clear();
                bunifuTextBox2.Clear();
                bunifuTextBox3.Clear();
                bunifuTextBox4.Clear();
                return;
            }
            // tạo thread để giao tiếp với server
            Thread threadSignin = new Thread(Register);
            threadSignin.Start();
            threadSignin.IsBackground = true;
        }
    }
}
