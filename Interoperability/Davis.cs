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
    public partial class Davis : Form
    {
        public Davis()
        {
            InitializeComponent();
        }

        public void setUniqueTelUploadText(string text)
        {
            try
            {
                if (this.uniqueTelUploadText.InvokeRequired)
                {
                    this.uniqueTelUploadText.BeginInvoke((MethodInvoker)delegate()
                    {
                        this.uniqueTelUploadText.Text = text;
                    });
                    return;
                }
                uniqueTelUploadText.Text = text;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("FAILED " + text); 
            }
        }

        public void setAvgTelUploadText(string text)
        {
            try
            {
                if (this.avgTelUploadText.InvokeRequired)
                {
                    this.avgTelUploadText.BeginInvoke((MethodInvoker)delegate()
                    {
                        this.avgTelUploadText.Text = text;
                    });
                    return;
                }
                avgTelUploadText.Text = text;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("FAILED " + text); 
            }
        }
    }
}
