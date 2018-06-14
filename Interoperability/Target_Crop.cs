using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace interoperability
{
    public partial class Target_Crop : Form
    {
        public Target_Crop()
        {
            InitializeComponent();

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "jpg files (*.jpg)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap MyImage;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    MyImage = new Bitmap(openFileDialog1.FileName);
                    pictureBox1.Image = MyImage;
                 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error, Invalid Image File.\n" + ex.Message, "Interoperability Control Panel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        int xUp, yUp, xDown, yDown;

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle rec = new Rectangle(xDown, yDown, e.X - xDown, e.Y - yDown);

            using (Pen pen = new Pen(Color.YellowGreen, 3))
            {

                pictureBox1.CreateGraphics().DrawRectangle(pen, rec);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            xDown = e.X;
            yDown = e.Y;
        }

        Rectangle rectCropArea;

        void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            xUp = e.X;
            yUp = e.Y;

            Rectangle rec = new Rectangle(xDown, yDown, xDown - xUp, yDown - yUp);

          

            using (Pen pen = new Pen(Color.YellowGreen, 3))
            {

                pictureBox1.CreateGraphics().DrawRectangle(pen, rec);
            }

            xDown = xDown * pictureBox1.Image.Width / pictureBox1.Width;
            yDown = yDown * pictureBox1.Image.Height / pictureBox1.Height;

            xUp = xUp * pictureBox1.Image.Width / pictureBox1.Width;
            yUp = yUp * pictureBox1.Image.Height / pictureBox1.Height;

            rectCropArea = new Rectangle(xDown, yDown, Math.Abs(xUp - xDown), Math.Abs(yUp - yDown));
        }

    }
}
