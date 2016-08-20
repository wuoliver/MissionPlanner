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
    public partial class Interoperability : Form
    {
        Action<int> restartInteroperabilityCallback;
        protected int pollRate = 10;
        interoperability.Settings settings_gui;

        public Interoperability(Action<int> _restartInteroperabilityCallback)
        {
            InitializeComponent();
            restartInteroperabilityCallback = _restartInteroperabilityCallback;

            pollRateInput.Text = pollRate.ToString();
        }

        public int getPollRate()
        {
            return pollRate;
        }

        public Settings getSettings_GUI()
        {
            return settings_gui;
        }
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
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

        private void applyPollRateButton_Click(object sender, EventArgs e)
        {
            try
            {
                pollRate = Int32.Parse(this.pollRateInput.Text);
            }
            catch (FormatException error)
            {
                this.pollRateInput.Text = pollRate.ToString();
            }
        }

        private void Server_Settings_Click(object sender, EventArgs e)
        {   
            if (!Settings.isOpened)
            {
                settings_gui = new Settings(restartInteroperabilityCallback);
                settings_gui.Show();
            }

        }

    }
}
