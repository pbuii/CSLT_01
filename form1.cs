using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSLT
{
    public partial class Form1 : Form
    {
        // Khai báo các biến trạng thái trò chơi và điều khiển người chơi
        bool isGameStarted = false; 
        bool goLeft, goRight; 
        int playerSpeed = 14; 
        int rubbishSpeed = 3; 
        int score; 
        int missed; 

        // Khai báo các biến cho các loại thùng rác
        int canType; 
        private Image can1; 
        private Image can2; 
        private Image can3;

        // Khai báo các biến cho các loại rác khác nhau
        private Image r1; 
        private Image r2; 
        private Image r3; 
        private Image r4; 

        private Image nr1; 
        private Image nr2; 
        private Image nr3; 
        private Image nr4; 

        private Image o1; 
        private Image o2; 
        private Image o3; 
        private Image o4; 

        // Khai báo mảng 2 chiều để lưu trữ các kết hợp hình ảnh rác khác nhau
        private Image[,] rubbishes; 

        // Khai báo đối tượng phát âm thanh hiệu ứng và nhạc nền
        private SoundPlayer catchSound; 
        private SoundPlayer missSound; 
        private SoundPlayer backgroundMusic; 

        // Tạo một đối tượng số ngẫu nhiên
        Random random = new Random(); 
        public Form1()
        {
            InitializeComponent();
            restartGame();
        }

        // Xử lý sự kiện click nút "Bắt đầu"
        private void Start_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu trò chơi chưa bắt đầu
            if (!isGameStarted)
            {
                // Phát nhạc nền
                backgroundMusic.Play();

                // Giả sử pictureBox2 là ảnh nút "Bắt đầu", ẩn nó đi
                Sorting.Visible = false;

                // Ẩn nút "Bắt đầu"
                Start.Visible = false;

                // Thiết lập trạng thái trò chơi thành đã bắt đầu
                isGameStarted = true;

                // Gọi hàm restartGame() để khởi tạo lại trò chơi
                restartGame();

                // Bắt đầu bộ hẹn giờ trò chơi
                MainTimer.Start();
            }
        }
        // Xử lý sự kiện hẹn giờ chính của trò chơi (được gọi mỗi khi hẹn giờ chạy)
        private void MainGameTimerEvent(object sender, EventArgs e)
        {
            // Kiểm tra nếu trò chơi đã bắt đầu
            if (isGameStarted)
            {
                // Xử lý di chuyển của người chơi dựa trên các phím được nhấn
                if (goLeft && UEHer.Left >= 5) // Kiểm tra nếu phím trái được nhấn và còn đủ chỗ để di chuyển sang trái
                {
                    UEHer.Left -= playerSpeed; // Di chuyển hình ảnh người chơi sang trái
                }
                if (goRight && UEHer.Left <= this.ClientSize.Width - UEHer.Width - 5) // Kiểm tra nếu phím phải được nhấn và còn đủ chỗ để di chuyển sang phải
                {
                    UEHer.Left += playerSpeed; // Di chuyển hình ảnh người chơi sang phải
                }

                // Lặp qua tất cả các PictureBox có tag "rubbish"
                foreach (Control x in this.Controls)
                {
                    if (x is PictureBox && ((string)x.Tag == "rubbish:0") || ((string)x.Tag == "rubbish:1") || ((string)x.Tag == "rubbish:2")) // Kiểm tra xem control x có phải PictureBox với tag "rubbish" và loại rác cụ thể
                    {
                        ((PictureBox)x).Top += rubbishSpeed; // Di chuyển hình ảnh rác xuống theo tốc độ rác

                        // Lấy loại rác từ tag
                        int rubbishtype;
                        bool boo = int.TryParse(((string)x.Tag).Split(':')[1], out rubbishtype);

                        // Kiểm tra xem rác đã ra khỏi màn hình (phía dưới của form) chưa
                        if (((PictureBox)x).Top > this.ClientSize.Height)
                        {
                            ((PictureBox)x).Top = random.Next(80, 300) * -1; // Vị trí lại hình ảnh rác ngoài màn hình ở phía trên
                            ((PictureBox)x).Left = random.Next(5, this.ClientSize.Width - 5 - ((PictureBox)x).Width); // Đặt một vị trí ngang mới ngẫu nhiên cho rác
                            ((PictureBox)x).BackColor = Color.Transparent; // Làm cho hình ảnh rác trong suốt trở lại
                            missed++; // Tăng số lần bỏ lỡ
                        }

                        // Kiểm tra va chạm giữa người chơi và rác
                        if (UEHer.Bounds.IntersectsWith(x.Bounds))
                        {
                            if (rubbishtype == canType) // Kiểm tra xem loại rác có khớp với loại lon của người chơi không
                            {
                                score++; // Tăng điểm nếu bắt đúng
                                catchSound.Play(); // Phát hiệu ứng âm thanh bắt được
                            }
                            else
                            {
                                missed++; // Tăng số lần bỏ lỡ nếu bắt sai
                                missSound.Play(); // Phát hiệu ứng âm thanh bỏ lỡ
                            }

                            // Cập nhật hình ảnh rác với loại và hình ảnh mới ngẫu nhiên
                            int type = random.Next(0, 3); // Lấy một loại rác ngẫu nhiên (0, 1 hoặc 2)
                            int item = random.Next(0, 4); // Lấy một biến thể vật phẩm rác ngẫu nhiên (0, 1, 2 hoặc 3)
                            ((PictureBox)x).Tag = $"rubbish:{type}"; // Cập nhật tag với loại rác mới
                            ((PictureBox)x).Image = rubbishes[type, item]; // Đặt hình ảnh PictureBox thành loại rác và biến thể tương ứng

                            ((PictureBox)x).Top = random.Next(80, 300) * -1; // Vị trí lại hình ảnh rác ngoài màn hình ở phía trên
                            ((PictureBox)x).Left = random.Next(5, this.ClientSize.Width - 5 - x.Width); // Đặt một vị trí ngang mới ngẫu nhiên cho rác
                            x.BackColor = Color.Transparent; // Làm cho hình ảnh rác trong suốt trở lại
                        }
                    }
                }
                // Cập nhật nhãn điểm và số lần bỏ lỡ
                txtScore.Text = "Score: " + score;
                txtMissed.Text = "Missed: " + missed;

                // Kiểm tra điều kiện kết thúc trò chơi (bỏ lỡ 10 lần)
                if (missed == 10)
                {
                    MainTimer.Enabled = false; // Dừng bộ hẹn giờ trò chơi
                    GameOver.Visible = true; // Hiển thị nhãn "Game Over"
                    FinalScore.Text = "Your score is: " + score; // Cập nhật văn bản điểm cuối cùng
                    FinalScore.Visible = true; // Hiển thị văn bản điểm cuối cùng
                    Exit.Visible = true; // Hiển thị nút "Thoát"
                }
            }
        }

        // Xử lý sự kiện khi người dùng nhả phím
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            // Kiểm tra nếu phím bấm là phím mũi tên phải
            if (e.KeyCode == Keys.Right)
            {
                // Đặt cờ di chuyển sang phải về false
                goRight = false;
            }

            // Kiểm tra nếu phím bấm là phím mũi tên trái
            if (e.KeyCode == Keys.Left)
            {
                // Đặt cờ di chuyển sang trái về false
                goLeft = false;
            }
        }

        // Xử lý sự kiện khi người dùng nhấn phím
        private void Form1_KeyDown_1(object sender, KeyEventArgs e)
        {
            // Kiểm tra nếu phím trái được nhấn
            if (e.KeyCode == Keys.Left)
            {
                // Kiểm tra xem còn đủ khoảng trống để di chuyển sang trái
                if (UEHer.Left >= 5)
                {
                    // Đặt cờ hiệu di chuyển sang trái
                    goLeft = true;
                }
            }

            // Kiểm tra nếu phím phải được nhấn
            if (e.KeyCode == Keys.Right)
            {
                // Kiểm tra xem còn đủ khoảng trống để di chuyển sang phải
                if (UEHer.Left <= this.ClientSize.Width - UEHer.Width - 5)
                {
                    // Đặt cờ hiệu di chuyển sang phải
                    goRight = true;
                }
            }

            // Xử lý khi người dùng chọn loại thùng rác 1 (D1)
            if (e.KeyCode == Keys.D1)
            {
                // Đặt hình ảnh người chơi là thùng rác 1
                UEHer.Image = can1;
                // Cập nhật loại thùng rác hiện tại
                canType = 0;
            }

            // Xử lý khi người dùng chọn loại thùng rác 2 (D2)
            if (e.KeyCode == Keys.D2)
            {
                // Đặt hình ảnh người chơi là thùng rác 2
                UEHer.Image = can2;
                // Cập nhật loại thùng rác hiện tại
                canType = 1;
            }

            // Xử lý khi người dùng chọn loại thùng rác 3 (D3)
            if (e.KeyCode == Keys.D3)
            {
                // Đặt hình ảnh người chơi là thùng rác 3
                UEHer.Image = can3;
                // Cập nhật loại thùng rác hiện tại
                canType = 2;
            }
        }

        // Phương thức này tải các dữ liệu liên quan
        private void Form1_Load(object sender, EventArgs e)
        {
            // Tải hình ảnh
            r1 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\r1.png");
            r2 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\r2.png");
            r3 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\r3.png");
            r4 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\r4.png");

            nr1 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\nr1.png");
            nr2 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\nr2.png");
            nr3 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\nr3.png");
            nr4 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\nr4.png");


            o1 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\o1.png");
            o2 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\o2.png");
            o3 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\o3.png");
            o4 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\o4.png");

            // Tải âm thanh
            catchSound = new SoundPlayer(@"C:\Users\nhatl\source\repos\CSLT\Imagess\catch.wav");
            missSound = new SoundPlayer(@"C:\Users\nhatl\source\repos\CSLT\Imagess\miss1.wav");
            backgroundMusic = new SoundPlayer(@"C:\Users\nhatl\source\repos\CSLT\Imagess\background1.wav");

            // Khởi tạo mảng 2 chiều lưu trữ hình ảnh các loại rác
            rubbishes = new Image[3, 4]{
                { r1, r2, r3, r4},
                {o1, o2, o3, o4},
                {nr1, nr2, nr3, nr4},
            };

            // Tải hình ảnh các loại thùng rác
            can1 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\can1.png");
            can2 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\can2.png");
            can3 = Image.FromFile(@"C:\Users\nhatl\source\repos\CSLT\Imagess\can3.png");
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            // Dừng nhạc nền
            backgroundMusic.Stop();

            // Thiết lập lại trạng thái trò chơi
            isGameStarted = false;

            // Ẩn các thành phần liên quan đến Game Over
            GameOver.Visible = false;
            FinalScore.Visible = false;
            Exit.Visible = false;

            // Hiển thị lại các thành phần ban đầu
            Start.Visible = true;
            Sorting.Visible = true;

            // Reset điểm số và số lần bỏ lỡ
            score = 0;
            missed = 0;
        }

        private void restartGame()
        {

            // Ẩn nút "Thoát" (có thể tùy chỉnh)
            Exit.Visible = false;

            // Thiết lập loại rác mặc định (có thể tùy chỉnh)
            canType = 1;

            // Khởi tạo lại trạng thái di chuyển (bỏ chọn trái/phải)
            goLeft = false;
            goRight = false;

            // Bắt đầu bộ hẹn giờ trò chơi
            MainTimer.Start();

            // Lặp qua tất cả các PictureBox có tag "rubbish"
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && ((string)x.Tag == "rubbish:0") || ((string)x.Tag == "rubbish:1") || ((string)x.Tag == "rubbish:2"))
                {
                    // Vị trí lại các rác ở ngoài màn hình phía trên
                    x.Top = random.Next(80, 300) * -1;
                    // Đặt lại vị trí ngang ngẫu nhiên cho các rác
                    x.Left = random.Next(5, this.ClientSize.Width - 5 - x.Width);
                }
            }
        }
    }
}
