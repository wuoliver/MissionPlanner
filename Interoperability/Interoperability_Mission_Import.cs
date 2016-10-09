using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using interoperability;

namespace interoperability
{
    public partial class Interoperability_Mission_Import : Form
    {
        //The edit button will open up a human readable text box, with line by line coordinates, and tags, and everything.
        //So humans can edit each mission if they want. 

        public bool isOpened = false;
        List<Mission> Mission_List;


        public Interoperability_Mission_Import(List<Mission> _Mission_List)
        {
            InitializeComponent();
            Mission_List = _Mission_List;
            for(int i = 0; i < Mission_List.Count(); i++)
            {
                SelectMission_ComboBox.Items.Add(Mission_List[i].name);
            }

            //Sets initial value for combo box (Select Item)
            MissionItemImport_ComboBox.SelectedIndex = 0;
            SelectMission_ComboBox.SelectedIndex = 0;
        }

        /*public void Import_Mission_Item(string filename, string type)
        {
            //Set up file paths to save default login information 
            string path = Directory.GetCurrentDirectory() + @"\Interoperability";
            Directory.CreateDirectory(path);
            Console.WriteLine("The credentials directory is {0}", path);
            //Do not change file name. 
            path += @"\credentials.txt";

            try
            {
                if (File.Exists(path))
                {
                    //Create new filestream for streamreader to read
                    using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        Console.WriteLine("Credential File Exists, Opening File");
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            //Assuming nobody messed with the original file, we read each line into their respective variables
                            String[] credentials = new String[3];
                            for (int i = 0; i < 3; i++)
                            {
                                //Going to do some error checking in the future, in case people mess with file
                                credentials[i] = sr.ReadLine();
                            }
                            address = credentials[0];
                            username = credentials[1];
                            password = credentials[2];
                            Console.WriteLine("Address: " + address + "\nUsername: " + username + "\nPassword: " + password);
                            sr.Close();
                        }
                        fs.Close();
                    }
                }
                //File does not exist, we create a new file, and write the default server credentials 
                else
                {
                    using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                    {
                        Console.WriteLine("Credential File Does not Exist, Creating File");
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.WriteLine(Default_address);
                            sw.WriteLine(Default_username);
                            sw.WriteLine(Default_password);
                            address = Default_address;
                            username = Default_username;
                            password = Default_password;
                            sw.Close();
                        }
                        fs.Close();
                    }
                }
            }
            catch
            {
                Console.WriteLine("File opening fail");
                //Use default credentials for now
                address = Default_address;
                username = Default_username;
                password = Default_password;
                //Should do something...not sure what for now
            }

        }
        */

        private void ExportMission_Button_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    // Code to write the stream goes here.
                    myStream.Close();
                }
            }
        }

        private void ImportMission_Button_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            // Insert code to read the stream here.
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void Interoperability_Mission_Import_Shown(object sender, EventArgs e)
        {
            isOpened = true;
        }

        private void Interoperability_Mission_Import_FormClosed(object sender, FormClosedEventArgs e)
        {
            isOpened = false;
        }

        private void Done_Button_Click(object sender, EventArgs e)
        {
            isOpened = false;
            this.Close();
        }
    }
}
