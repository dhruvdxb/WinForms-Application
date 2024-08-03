using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace ScreenColor
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern int GetPixel(IntPtr hdc, int nXPos, int nYPos);

        bool HoldMousebtnPressed = false;
        bool isColorLocked = false;
 
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Thread thread = new Thread(BackGroundThread) { IsBackground = true };
            thread.Start();
        }

        void BackGroundThread()
        {
            while (true)
            {
                if (HoldMousebtnPressed && !isColorLocked)
                {
                    Point cursorPosition = Cursor.Position;
                    Color selectedColor = GetColorAt(cursorPosition);
                    string colorHtml = ColorTranslator.ToHtml(selectedColor);

                   
                    this.Invoke(new Action(() =>
                    {
                        panel1.BackColor = selectedColor;
                        textBox1.Text = colorHtml;

                    
                        int red = selectedColor.R;
                        int green = selectedColor.G;
                        int blue = selectedColor.B;

                    
                        labelRed.Text = $"Red: {red}";
                        labelGreen.Text = $"Green: {green}";
                        labelBlue.Text = $"Blue: {blue}";
                    }));
                }
            }
        }

        Color GetColorAt(Point location)
        {
           
            IntPtr hdc = GetDC(IntPtr.Zero);

           
            int pixelColor = GetPixel(hdc, location.X, location.Y);

            
            ReleaseDC(IntPtr.Zero, hdc);

          
            int red = (pixelColor & 0x00FF0000) >> 16;
            int green = (pixelColor & 0x0000FF00) >> 8;
            int blue = (pixelColor & 0x000000FF);

           
            return Color.FromArgb(red, green, blue);
        }

        private void HoldMousebtn_MouseDown(object sender, MouseEventArgs e)
        {
            HoldMousebtnPressed = true;
            Cursor = Cursors.Cross;

        }

        private void HoldMousebtn_MouseUp(object sender, MouseEventArgs e)
        {
            HoldMousebtnPressed = false;
            Cursor = Cursors.Default;

        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.L))
            {
                isColorLocked = !isColorLocked;
                MessageBox.Show(isColorLocked ? "Color locked." : "Color unlocked."); 
                return true; 
            }
            return base.ProcessCmdKey(ref msg, keyData); 
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            if (panel1.BackColor != null)
            {
                Color selectedColor = panel1.BackColor;
                string colorHtml = ColorTranslator.ToHtml(selectedColor);

              
                int red = selectedColor.R;
                int green = selectedColor.G;
                int blue = selectedColor.B;

              
                string clipboardText = $"Color Information:\n" +
                                       $"HEX: {colorHtml}\n" +
                                       $"RGB: {red}, {green}, {blue}";

                Clipboard.SetText(clipboardText);
                MessageBox.Show("Color information copied to clipboard.");
            }
        }
    }
}

