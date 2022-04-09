using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace ScreenMemo
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetSystemMetrics(int nIndex);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        private const uint MOUSEMOVE = 0x0001; // 마우스 이동
        private const uint LBUTTONDOWN = 0x0002; // 왼쪽 마우스 버튼 눌림
        private const uint LBUTTONUP = 0x0004; // 왼쪽 마우스 버튼 떼어짐
        private const uint ABSOLUTEMOVE = 0x8000; // 전역 위치

        int SM_CXSCREEN = 0;
        int SM_CYSCREEN = 1;

        int click_flag = 0;
        int pen_flag = 1;
        int erase_flag = 0;
        List<Point> points;
        SolidBrush background_brush;
        Point first_point;
        Pen pen;
        
        SolidBrush brush;
        public Form1()
        {
            InitializeComponent();
        }
        private void init_varialbe()
        {

            button1.BackColor = Color.Black;
            
            pen = new Pen(Color.Black);
            pen.Width = int.Parse(comboBox1.Text);
            brush = new SolidBrush(Color.Black);
            first_point = new Point();
            
            
            points = new List<Point>();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
            init_varialbe();
            Capture();
            SetToolLocation();
            mouse_pencile();
        }
        private void SetToolLocation()
        {
            int width = GetSystemMetrics(SM_CXSCREEN);
            int height = GetSystemMetrics(SM_CYSCREEN);
            groupBox1.SetBounds(width - groupBox1.Width-30, height - groupBox1.Height - 300, groupBox1.Width, groupBox1.Height);
        }
        private void Capture()
        {
            KeyPreview = true;
            SetProcessDPIAware();
            int width = GetSystemMetrics(SM_CXSCREEN);
            int height = GetSystemMetrics(SM_CYSCREEN);

            this.SetBounds(0, 0, width, height);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(0, 0, 0, 0, new Size(width, height));

            this.BackgroundImage = bmp;
            this.TransparencyKey = Color.Aqua;

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
            else if(e.KeyCode == Keys.Q)
            {
                groupBox1.Visible = !groupBox1.Visible;
            }
            else if (e.KeyCode == Keys.S)
            {
                savebmp();
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            click_flag = 1;
            int pen_width = int.Parse(comboBox1.Text);
            if (pen_flag == 1 && erase_flag == 0)
            {
                first_point.X = e.X;
                first_point.Y = e.Y;
                Graphics g = CreateGraphics();

                g.FillEllipse(brush, new Rectangle(e.X - pen_width / 2, e.Y - pen_width / 2, pen_width, pen_width));
            }
            else if (erase_flag == 1 && pen_flag == 0)
            {
                
                first_point.X = e.X;
                first_point.Y = e.Y;
                
                Region region = new Region(new Rectangle(e.X - 12 / 2, e.Y - 12 / 2, 12, 12));
                Invalidate(region);
               //g.FillRectangle(background_brush,e.X - pen_width / 2, e.Y - pen_width / 2, pen_width, pen_width);
                
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {            
            click_flag = 0;            
        }
        

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {            
            if(click_flag==1)
            {
                if (pen_flag == 1 && erase_flag==0)
                {
                    Graphics g = CreateGraphics();
                    g.DrawLine(pen, first_point.X, first_point.Y, e.X, e.Y);
                    first_point.X = e.X;
                    first_point.Y = e.Y;
                }
                else if(erase_flag ==1 && pen_flag==0)
                {
                    first_point.X = e.X;
                    first_point.Y = e.Y;

                    Region region = new Region(new Rectangle(e.X - 30 / 2, e.Y - 30 / 2, 30, 30));
                    Invalidate(region);
                }
            }         
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void groupBox1_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pen_flag = 1;
            erase_flag = 0;
            if(colorDialog1.ShowDialog()==DialogResult.OK)
            {
                pen.Color = colorDialog1.Color;
                brush.Color =colorDialog1.Color;

                button1.BackColor = colorDialog1.Color;
                
            }
            mouse_pencile();
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            erase_flag = 1;
            pen_flag = 0;
            mouse_erase();
            
        }
        private void mouse_erase()
        {
            Bitmap bmp = new Bitmap(20, 20);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.White, 0, 0, 20, 20);
            g.DrawRectangle(Pens.Black, new Rectangle(0, 0, 20, 20));


            IntPtr handle = bmp.GetHicon();
            Cursor cursor = new Cursor(handle);
            this.Cursor = cursor;
        }
        private void mouse_pencile()
        {
            Bitmap bmp = new Bitmap("pencil_PNG3855.png");
            IntPtr handle = bmp.GetHicon();
            Cursor cursor = new Cursor(handle);
            this.Cursor = cursor;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pen.Width = int.Parse(comboBox1.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            erase_flag = 0;
            pen_flag = 1;
            mouse_pencile();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            savebmp();

        }
        private void savebmp()
        {
            int width = GetSystemMetrics(SM_CXSCREEN);
            int height = GetSystemMetrics(SM_CYSCREEN);

            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(0, 0, 0, 0, new Size(width, height));
            string s = DateTime.Now.ToString("yyyy.MM.dd HH시mm분ss초");
            MessageBox.Show(s);
            string save_str = string.Format("capture\\"+s+".bmp");
            bmp.Save(save_str);
            MessageBox.Show("저장 성공");
            groupBox1.Visible = true;
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://blog.naver.com/dhdh0482");
        }
    }
}
