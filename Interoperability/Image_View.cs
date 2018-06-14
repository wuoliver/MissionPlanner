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
    public partial class Image_View : Form
    {
        public Image_View(Bitmap image)
        {
            InitializeComponent();


            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = image;

        }


    }

    }

