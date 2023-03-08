using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Analyzer
{
    public partial class Form1 : Form
    {
        public OpenFileDialog open = new OpenFileDialog();
        public Bitmap bitmap;
        public Bitmap[,] list = new Bitmap[2, 2];
        public DoInThread work;
        public Thread thread;
        public Thread thread2;
        public Thread thread3;
        public Thread thread4;

        public static float HSV(Color color)
        {
            float hue;
            float saturation;
            float value;

            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1f - (1f * min / max);
            value = max / 255f;
            float result = hue + saturation + value;


            Console.WriteLine("HUE: " + hue);
            Console.WriteLine("Saturation: " + saturation);
            Console.WriteLine("Value: " + value);
            Console.WriteLine("Result: " + result);
            Console.WriteLine();
            Console.ReadKey();
            return result;
        }        

        public void DO()
        {
            Console.WriteLine("Zacina 1");
            list[0, 0] = bitmap.Clone(new Rectangle(0, 0, bitmap.Width / 2, bitmap.Height / 2), bitmap.PixelFormat);                      
            Console.WriteLine("Zacina 2");
            list[0, 1] = bitmap.Clone(new Rectangle(bitmap.Width / 2, 0, bitmap.Width / 2, bitmap.Height / 2), bitmap.PixelFormat);      
            Console.WriteLine("Zacina 3");
            list[1, 0] = bitmap.Clone(new Rectangle(0, bitmap.Height / 2, bitmap.Width / 2, bitmap.Height / 2), bitmap.PixelFormat);     
            Console.WriteLine("Zacina 4");
            list[1, 1] = bitmap.Clone(new Rectangle(bitmap.Width / 2, bitmap.Height / 2, bitmap.Width / 2, bitmap.Height / 2), bitmap.PixelFormat);
            Console.WriteLine("Done");
            //this.Update();

        }
        public Form1()
        {

            InitializeComponent();

            

            //work = new DoInThread(bitmap);
            HideStuff();
            #region st
            /*
            open.ShowDialog();
            bitmap = (Bitmap)Bitmap.FromFile(open.FileName);
            DO();
            */
            /*
            pictureBox1.Width = (int)(bitmap.Width / 2);
            pictureBox1.Height = (int)(bitmap.Height/2);

            pictureBox2.Width = (int)(bitmap.Width / 2);
            pictureBox2.Height = (int)(bitmap.Height / 2);

            pictureBox3.Width = (int)(bitmap.Width / 2);
            pictureBox3.Height = (int)(bitmap.Height / 2);

            pictureBox4.Width = (int)(bitmap.Width / 2);
            pictureBox4.Height = (int)(bitmap.Height / 2);


            fullImage.Width = bitmap.Width;
            fullImage.Height = bitmap.Height;

            //fullImage.Update();

            fullImage.Invalidate();
            pictureBox1.Invalidate();
            pictureBox2.Invalidate();
            pictureBox3.Invalidate();
            pictureBox4.Invalidate();
            */
            //work = new DoInThread(list,bitmap,bitmap);
            //button1.Location.Offset(pictureBox1.Width + 20, 30);
            #endregion

            HidePictorebox();
            


        }

        private void HideStuff()
        {
            button1.Hide();
            button1.Enabled = false;

            button2.Hide();
            button2.Enabled = false;

            button3.Hide();
            button3.Enabled = false;

            button4.Hide();
            button4.Enabled = false;

            pictureBox1.Hide();
            pictureBox2.Hide();
            pictureBox3.Hide();
            pictureBox4.Hide();
            fullImage.Hide();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
            //int i = bitmap.Width / 2;
            int H = 0;//heigh - 95
            int W = 0;// Width 170

            #region
            /*
            int s = bitmap.Width;
            int f = bitmap.Height;
            int i = work.list[0, 0].Width;
            int u = work.list[0, 0].Height;

            int x = work.list[0, 0].Width + W;
            */
            /*
            e.Graphics.DrawImage(work.list[0,0], 0, 0);
            e.Graphics.DrawImage(work.list[0, 1],x, 0);//bitmap.Width / 2
            e.Graphics.DrawImage(work.list[1, 0], 0, work.list[0,0].Height+H);
            e.Graphics.DrawImage(work.list[1, 1], work.list[1, 0].Width + W, work.list[1, 0].Height + H);
            */
            #endregion

            if (bitmap != null)
            {
                e.Graphics.DrawImage(bitmap, ((1630 / 2) - (bitmap.Width / 2)), ((1080 / 2) - (bitmap.Height / 2)));

                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        var image = list[j, i];
                        var x = i * (1630 - image.Width);
                        var y = j * (1080 - image.Height);
                        e.Graphics.DrawImage(image, x, y);

                    }
                }

                //e.Graphics.DrawImage(list[0, 0], 0, 0);
                //e.Graphics.DrawImage(list[0, 1], (1630 -  (bitmap.Width / 2)), 0);
                //e.Graphics.DrawImage(list[1, 0], 0, (1080 - (bitmap.Height / 2)));
                //e.Graphics.DrawImage(list[1, 1], (1630 - (bitmap.Width / 2)), (1100 - (list[1, 1].Height)));


                //list[0, 0].Save("result1.jpg");
                //list[0, 1].Save("result2.jpg");
            }
            

            
            

            //e.Graphics.DrawImage(work.list[0, 0], 0, 0);


        }

        private void button1_Click(object sender, EventArgs e)
        {            
            Console.WriteLine("Magic starts here.");

            Thread thread = new Thread(new ThreadStart(work.DOStuff1));
            Thread thread2 = new Thread(new ThreadStart(work.DOStuff2));
            Thread thread3 = new Thread(new ThreadStart(work.DOStuff3));
            Thread thread4 = new Thread(new ThreadStart(work.DOStuff4));
            work.CreateFullImage();

            thread.Start();
            
            thread2.Start();
            thread3.Start();
            thread4.Start();
            
            while (work.isOn1) { int i = 1; }
            thread.Abort();
            
            
            while (work.isOn2) { int i = 1; }            
            thread2.Abort();
            
            
            
            while (work.isOn3) { int i = 1; }
            thread3.Abort();
            

            while (work.isOn4) { int i = 1; }
            thread4.Abort();

            work.CreateFullImage();
            //work.CreateResultImage();


            //this.Invalidate();
            pictureBox1.Invalidate();
            //pictureBox1.Update();
            fullImage.Invalidate();
            work.bitmap.Save("Hope.jpg");

            
          
            Console.WriteLine("DONE - Threads finished their job.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //this.Update();
            pictureBox1.Update();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //this.Invalidate();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int H = 0;//heigh - 95
            int W = 0;// Width 170

            int s = bitmap.Width;
            int f = bitmap.Height;
            int i = work.list[0, 0].Width;
            int u = work.list[0, 0].Height;

            int x = work.list[0, 0].Width + W;

            e.Graphics.DrawImage(work.list[0, 0], 0, 0);
            /*
            e.Graphics.DrawImage(work.list[0, 1], x, 0);//bitmap.Width / 2
            e.Graphics.DrawImage(work.list[1, 0], 0, work.list[0, 0].Height + H);
            e.Graphics.DrawImage(work.list[1, 1], work.list[1, 0].Width + W, work.list[1, 0].Height + H);
            
            list[0, 0].Save("result1.jpg");
            list[0, 1].Save("result2.jpg");
            */
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            /*
            for(int i1 = 0; i1 < work.list.GetLength(0); i1++)
            {
                for(int i2 = 0; i2 < work.list.GetLength(1); i2++)
                {
                    work.list[i1, i2].Dispose();
                }
            }
            work = null;
            */
            
        }

        private void fullImage_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.DrawImage(work.bitmap,0,0);
            fullImage.Image = bitmap;
        }

        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(work.list[1, 0], 0, 0);
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(work.list[0, 1], 0, 0);
        }

        private void pictureBox4_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(work.list[1, 1], 0, 0);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine("X> " + e.X + "       Y> " + e.Y);
            int x = e.X;
            int y = e.Y;
            if(x >= 1640 && x < 1860)
            {
                if(y >= 300 && y <= 350)
                {
                    //Open file
                    OpenFile();
                    
                }
                else if(y >= 400 && y <= 450)
                {
                    //Analyze
                    Analyze();
                }
                else if (y >= 530 && y <= 570)
                {
                    //Bot
                    Bot();
                }
                else if(y >= 670 && y <= 720)
                {
                    //Exit
                    Exit();
                }
            }

        }
        private void OpenFile()
        {
            open.ShowDialog();
            bitmap = (Bitmap)Bitmap.FromFile(open.FileName);
            DO();
            //fullImage.Image = bitmap;

            work = new DoInThread(list, bitmap, bitmap);
            thread = new Thread(new ThreadStart(work.DOStuff1));
            thread2 = new Thread(new ThreadStart(work.DOStuff2));
            thread3 = new Thread(new ThreadStart(work.DOStuff3));
            thread4 = new Thread(new ThreadStart(work.DOStuff4));
            work.CreateFullImage();
            //ShowPictureBox();
            pictureBox1.Width = (int)(bitmap.Width / 2);
            pictureBox1.Height = (int)(bitmap.Height / 2);

            pictureBox2.Width = (int)(bitmap.Width / 2);
            pictureBox2.Height = (int)(bitmap.Height / 2);

            pictureBox3.Width = (int)(bitmap.Width / 2);
            pictureBox3.Height = (int)(bitmap.Height / 2);

            pictureBox4.Width = (int)(bitmap.Width / 2);
            pictureBox4.Height = (int)(bitmap.Height / 2);


            fullImage.Width = bitmap.Width;
            fullImage.Height = bitmap.Height;

            //InvalidatePicturebox();
            Analyze();
            this.Invalidate();
            this.Update();
            
        }
        private void Analyze()
        {

            if (work == null)
            {
                MessageBox.Show("No image selected.", "System.OutOfMemoryException");
                return;
            }
            if(!work.isOn4)
            {
                MessageBox.Show("Already analyzed.", "System.OutOfMemoryException");
                return;
            }
            thread.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();

            while (work.isOn1) { int i = 1; }
            thread.Abort();


            while (work.isOn2) { int i = 1; }
            thread2.Abort();



            while (work.isOn3) { int i = 1; }
            thread3.Abort();


            while (work.isOn4) { int i = 1; }
            thread4.Abort();

            work.CreateFullImage();

            //InvalidatePicturebox();
            this.Invalidate();
            this.Update();
            work.bitmap.Save("Hope.jpg");
        }
        private void Bot()
        {
            // :D
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = "D:\\Amavet - Copy\\NeuralNet\\NeuralNeGame\\bin\\Debug\\NeuralNetGame.exe";
            process.StartInfo = startInfo;
            process.Start();
        }

        private void Exit()
        {
            this.Close();
        }
        private void InvalidatePicturebox()
        {
            
            pictureBox1.Invalidate();
            pictureBox1.Update();
            pictureBox2.Invalidate();
            pictureBox2.Update();
            pictureBox3.Invalidate();
            pictureBox3.Update();
            pictureBox4.Invalidate();
            pictureBox4.Update();
            fullImage.Invalidate();
            fullImage.Update();

        }
        private void ShowPictureBox()
        {
            pictureBox1.Show();
            pictureBox2.Show();
            pictureBox3.Show();
            pictureBox4.Show();
            fullImage.Show();
            InvalidatePicturebox();
        }
        private void HidePictorebox()
        {
            pictureBox1.Hide();
            pictureBox1.Enabled = false;
            pictureBox1.Visible = false;
            pictureBox2.Hide();
            pictureBox2.Enabled = false;
            pictureBox2.Visible = false;
            pictureBox3.Hide();
            pictureBox3.Enabled = false;
            pictureBox3.Visible = false;
            pictureBox4.Hide();
            pictureBox4.Enabled = false;
            pictureBox4.Visible = false;
            fullImage.Hide();
            fullImage.Enabled = false;
            fullImage.Visible = false;
        }
    }
}
