using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Myrmec;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.ComponentModel;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        int matchtype = 0;
        string path = "";
        int notepadcheck = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            dataGridView1.Rows.Clear();
            folderBrowserDialog1.SelectedPath= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            DialogResult result = folderBrowserDialog1.ShowDialog();
            textBox1.Text = folderBrowserDialog1.SelectedPath;
            path = folderBrowserDialog1.SelectedPath;
            string[] allfiles = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.*", SearchOption.AllDirectories);

            System.Text.Encoding enc = System.Text.Encoding.ASCII;

            Sniffer sniffer = new Sniffer();

            sniffer.Populate(FileTypes.Common);
            sniffer.Add(new Record("plist", "62 70")); //adding plist
            sniffer.Add(new Record("jpg", "FF D8 FF E1 09 50 68")); //
            sniffer.Add(new Record("data", "0C 00 00 00 0B 00 00"));
            sniffer.Add(new Record("JSON", "7B"));
            sniffer.Add(new Record("Binary Cookies", "63 6F 6F"));
            sniffer.Add(new Record("SQLite DB", "53 51 4c 69"));
            sniffer.Add(new Record("SQLite WAL", "37 7F 06"));
            sniffer.Add(new Record("SQLite SHM", "18 E2 2D"));
            sniffer.Add(new Record("Serialised Data", "52 B7 0E 08"));
            sniffer.Add(new Record("Android Manifest", "31 0A 63 6F"));
            foreach (var file in allfiles)
            {
                string typelist = "";
                FileInfo info = new FileInfo(file);
                //Console.WriteLine(file);
                DataGridViewRow row = new DataGridViewRow();
                byte[] fileHead = ReadFileHead(file);
                //Console.WriteLine(sniffer.Match(fileHead));
                List<string> results = sniffer.Match(fileHead, false);
                foreach (var i in results)
                {
                    typelist = typelist + "," + i;
                    matchtype = 0;
                }
                if (typelist == "" && file.Contains("NetworkCache"))
                {
                    typelist = "NetworkCacheBlob";
                    matchtype = 1;
                }
                if (typelist =="" && info.Extension.Contains("json"))
                {
                    typelist = "JSON";
                    matchtype = 1;
                }
                if (typelist == "" && info.Extension.Contains(".log"))
                {
                    typelist = "Log";
                    matchtype = 1;
                }
                if (typelist == "" && file.Contains("com.apple.WebKit.WebContent") && (info.Extension.Contains("data") | info.Extension.Contains("maps")))
                {
                    typelist = "WebKitData";
                    matchtype = 1;
                }
                if (typelist == "")
                {
                    typelist = "Unknown";
                    matchtype = 1;
                }
                if (info.Length == 0)
                {
                    typelist = "Empty File";
                    matchtype = 0;
                }
                

                dataGridView1.Rows.Add(file.Replace(folderBrowserDialog1.SelectedPath, ""), typelist, ByteArrayToString(fileHead));
                //dataGridView1.Rows[XmlReadMode - 1].Cells[2].Style.ForeColor = Color.Purple;
                typelist = "";
            }
        }

      
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            Console.WriteLine("added");
            if (matchtype == 0)
            {
                dataGridView1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.OliveDrab;
                dataGridView1.Rows[e.RowIndex].Cells[1].Style.ForeColor = Color.White;
            }
            else
            {
                dataGridView1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Tomato;
                dataGridView1.Rows[e.RowIndex].Cells[1].Style.ForeColor = Color.White;
            }
        }

        private byte[] ReadFileHead(string file)
        {
            var stream = new FileStream(file, FileMode.Open);
            byte[] target = new byte[20];
            stream.Read(target, 0, 20);
            stream.Dispose();
            return target;
        }


        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists("C:\\Program Files\\VOW Software\\plist Editor Pro\\plistEditor.exe"))
            {
                MessageBox.Show("Plist editor missing, go download it , I'll open the page, download 'plist Editor Pro'");
                System.Diagnostics.Process.Start("https://www.icopybot.com/download.htm");
            }

            if (!File.Exists("C:\\Program Files (x86)\\Frhed\\Frhed.exe"))
            {
                MessageBox.Show("Hex editor missing, go download it , I'll open the page");
                System.Diagnostics.Process.Start("http://frhed.sourceforge.net/en/");
            }

            
            if (!File.Exists("C:\\Program Files\\Notepad++\\Notepad++.exe"))
            {
                notepadcheck = 64;
            }

            if (!File.Exists("C:\\Program Files\\Notepad++ (x86)\\Notepad++.exe"))
            {
                notepadcheck = 32;
            }

            if (notepadcheck == 0)
            {
                MessageBox.Show("Notepad++ missing, you filthy casual, go download it , I'll open the page, make sure to get the 64Bit version");
                System.Diagnostics.Process.Start("https://notepad-plus-plus.org/download/v7.7.1.html");
            }

            if (!File.Exists("C:\\Program Files\\DB Browser for SQLite\\DB Browser for SQLite.exe"))
            {
                MessageBox.Show("SQLIte Browser missing go download it , I'll open the page, make sure to get the 64Bit version");
                System.Diagnostics.Process.Start("https://sqlitebrowser.org/dl/");
            }

        }



        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow RW in dataGridView1.SelectedRows)
            {

                dataGridView1.Rows.RemoveAt(RW.Index);
            }
       

        }



        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }


     




        private void NotepadToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Console.WriteLine(dataGridView1.CurrentCell.RowIndex);
            Console.WriteLine(dataGridView1.CurrentCell.ColumnIndex);
            string filetype;
            string filepath;
            filetype = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value.ToString();
            filepath = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
            Console.WriteLine(filetype);

            Process notepad = new Process();
            if (notepadcheck == 64)
            {
                notepad.StartInfo.FileName = "C:\\Program Files\\Notepad++\\Notepad++.exe";
            }
            else
            {
                notepad.StartInfo.FileName = "C:\\Program Files (x86)\\Notepad++\\Notepad++.exe";
            }
            notepad.StartInfo.Arguments = path + filepath;
            notepad.Start();
        }

        private void PlistViewerToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Console.WriteLine(dataGridView1.CurrentCell.RowIndex);
            Console.WriteLine(dataGridView1.CurrentCell.ColumnIndex);
            string filetype;
            string filepath;
            filetype = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value.ToString();
            filepath = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
            Console.WriteLine(filetype);

            Process notepad = new Process();
            notepad.StartInfo.FileName = "C:\\Program Files\\VOW Software\\plist Editor Pro\\plistEditor.exe";
            notepad.StartInfo.Arguments = path + filepath;
            notepad.Start();
        }

        private void HexViewerToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Console.WriteLine(dataGridView1.CurrentCell.RowIndex);
            Console.WriteLine(dataGridView1.CurrentCell.ColumnIndex);
            string filetype;
            string filepath;
            filetype = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value.ToString();
            filepath = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
            Console.WriteLine(filetype);

            Process notepad = new Process();
            notepad.StartInfo.FileName = "C:\\Program Files (x86)\\Frhed\\Frhed.exe";
            notepad.StartInfo.Arguments = path + filepath;
            notepad.Start();
        }

        private void SqliteDBToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Console.WriteLine(dataGridView1.CurrentCell.RowIndex);
            Console.WriteLine(dataGridView1.CurrentCell.ColumnIndex);
            string filetype;
            string filepath;
            filetype = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value.ToString();
            filepath = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
            Console.WriteLine(filetype);

            Process notepad = new Process();
            notepad.StartInfo.FileName = "C:\\Program Files\\DB Browser for SQLite\\DB Browser for SQLite.exe";
            notepad.StartInfo.Arguments = path + filepath;
            notepad.Start();
        }

        private void ImageViewer_Click(object sender, EventArgs e)
        {
            Console.WriteLine(dataGridView1.CurrentCell.RowIndex);
            Console.WriteLine(dataGridView1.CurrentCell.ColumnIndex);
            string filetype;
            string filepath;
            filetype = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value.ToString();
            filepath = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
            Console.WriteLine(filetype);

            Process notepad = new Process();
            notepad.StartInfo.FileName = "C:\\Windows\\System32\\rundll32.exe";
            notepad.StartInfo.Arguments = " \"C:\\Program Files (x86)\\Windows Photo Viewer\\PhotoViewer.dll\",ImageView_Fullscreen " + path + filepath;
            notepad.Start();
        }
    }



}


