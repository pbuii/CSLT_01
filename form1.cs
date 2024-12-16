using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSLT
{
    public partial class Form1 : Form
    {
        PictureBox organicCan, plasticCan, otherCan;
        int currentCanIndex = 0;
        int[] canIndices = { 0, 1, 2 };
        bool goLeft, goRight;
        int playerSpeed = 14;
        int rubbishSpeed = 8;
        int score = 0;
        int missed = 0

        Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            restartGame();
        }

        private voide Form1_Load(object sender, EventArgs e) 
        {
            // Load image
            Image plastic1 = Image.FromFile(@"CSLT\\Images\\plastic1");
            Image plastic2 = Image.FromFile(@"CSLT\\Images\\plastic2");
            Image plastic3 = Image.FromFile(@"CSLT\\Images\\plastic3");
            Image organic1 = Image.FromFile(@"CSLT\\Images\\organic1");
            Image organic2 = Image.FromFile(@"CSLT\\Images\\organic2");
            Image organic3 = Image.FromFile(@"CSLT\\Images\\organic3");
            Image other1 = Image.FromFile(@"CSLT\\Images\\other1");
            Image other2 = Image.FromFile(@"CSLT\\Images\\other2");
            Image other3 = Image.FromFile(@"CSLT\\Images\\other3");

            // Image 2d array
            Image[,] ImageArray = {{plastic1, plastic2, plastic3}, {organic1, organic2, organic3}, {other1, other2, other3}};
        }
        
        private int GetRubbishType(PictureBox rubbish)
        {
            string[] tagParts = ((string)rubbish.Tag).Split(':');
            return int.Parse(tagParts[1]);
        }   
        private void MainGameTimerEvent(object sender, EventArgs e) 
        {
            if (goLeft && UEher.Left >= 5)
            {
                UEHer.Left -= playerSpeed;
            }
            if (goRight && UEHer.Left <= this.ClientSize.Width - UEHer.Width - 5)
            {
                UEHer.Left += playerSpeed;
            }

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string).x.Tag == "rubbish")
                {
                    x.Top += rubbishSpeed;

                    if (x.Top + x.Height >= this.ClientSize.Height)
                    {
                        X.Top = random.Next(80,300) * -1;
                        X.Left = random.Next(5, this.ClientSize.Width - 5 - x.Width)
                    }
                    if (x.Bounds.IntersectsWith(Controls[canIndices[currentCanIndex]].Bounds) && GetRubbishType(x) == currentCanIndex)
                    {
                        // Correct trash can, increase score
                        score++;
                        x.Top = random.Next(80, 300) * -1;
                        x.Left = random.Next(5, this.ClientSize.Width - 5 - x.Width);
                    }
                    else if (x.Top + x.Height >= this.ClientSize.Height || GetRubbishType(x) != currentCanIndex)
                    {
                        // Missed the trash, decrease score or increase missed count
                        missed++;
                        // reset rubbish position
                        x.Top = random.Next(80, 300) * -1;
                        x.Left = random.Next(5, this.ClientSize.Width - 5 - x.Width);
                    }
                }
                scoreLabel.Text = "Score: " + score;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                if (UEHer.Left >= 5)
                {
                    goLeft = true;
                }
            }
            if (e.KeyCode == Keys.Right)
            {
                if (UEHer.Left <= this.ClientSize.Width - 5)
                {
                    goRight = true;
                }
            }
                if (e.KeyCode == Keys.D1)
                    {
                        currentCanIndex = 0;
                    }
                else if (e.KeyCode == Keys.D2)
                    {
                        currentCanIndex = 1;
                    }
                else if (e.KeyCode == Keys == Keys.D3)
                    {
                        currentCanIndex = 2;
                    }
        }


        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
        }

        private void restartGame()
        {
            foreach (Control x in this.Controls) 
            {
                if(x is PictureBox && (string)x.Tag == "rubbish") 
                {
                    // Access a random rubbish item
                    int type = random.Next(0, 3);
                    int item = random.Next(0, 3);
                    x.Tag = $"rubbish:{type}"                    
                    x.Image = ImageArray[type, item]
                    
                    // Random location
                    x.Top = random.Next(80,300) * -1;
                    x.Left = random.Next(5, this.ClientSize.Width - 5 - x.Width);
                }
            }

            UEHer.Left = this.ClientSize.Width / 2;
            playerSpeed = 8;
            currentCanIndex = 0;
            
            goLeft = false;
            goRight = false;

            MainTimer.Start();  
        }
    }
}