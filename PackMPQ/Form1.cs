using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PackMPQ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region remove list
        public string[] listdontwant ={"war3map.w3e",
                                       "war3map.w3i",
                                       "war3map.wtg",
                                       "war3map.wct",
                                       "war3map.j",
                                       "scripts\\war3map.j",
                                       "war3map.shd",
                                       "war3mapmap.blp",
                                       "war3mapmap.tga",
                                       "war3mapmap.b00",
                                       "war3mappreview.tga",
                                       "war3mappreview.blp",
                                       "war3map.mmp",
                                       "war3mappath.tga",
                                       "war3mappath.blp",
                                       "war3map.wpm",
                                       "war3map.doo",
                                       "war3mapunits.doo",
                                       "war3map.w3r",
                                       "war3map.w3c",
                                       "war3map.imp",
                                       "war3mapskin.txt",
                                       "war3mapextra.txt"
                                      };
        #endregion

        private void PackAllFileToMpq(string path, string basepath, MpqLib.Mpq.CArchive mpq)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo fChild in dir.GetFiles("*")) //设置文件类型
            {
                string mpqpath = fChild.FullName.Substring(fChild.FullName.IndexOf(basepath) + basepath.Length + 1);
                //string mpqpath = fChild.FullName.Replace(basepath + @"\", "");
                mpq.ImportFile(mpqpath, fChild.FullName);
            }

            foreach (DirectoryInfo dChild in dir.GetDirectories("*")) //操作子目录
            {
                PackAllFileToMpq(dChild.FullName, basepath, mpq); //递归
            }
        }

        public string[] GenerateMapFilelist(MpqLib.Mpq.CArchive mpq)
        {
            string listcontent = "";
            byte[] fc=new byte[999999];
            mpq.ExportFile(@"(listfile)", fc);
            string[] filelist = System.Text.Encoding.ASCII.GetString(fc).Trim('\0').Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //StreamReader sr = new StreamReader("(listfile)");
            //string[] filelist = sr.ReadToEnd().Trim().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < filelist.Length; i++)
            {
                if (Array.IndexOf(listdontwant, filelist[i].Trim().ToLower()) == -1)
                {
                    listcontent += filelist[i] + "|";
                }
            }
            return listcontent.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            if (File.Exists(@"Output\Blizzard.j"))
            {
                //textBox1.Text = Application.StartupPath + @"\Output\Blizzard.j";
                textBox1.Text = @"Output\Blizzard.j";
            }
            if (!File.Exists(Application.StartupPath + @"\pjass\common.j"))
            {
                MessageBox.Show(@"'pjass\common.j' is missing, application can not initialize.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            if (Directory.Exists(@"Additional_Resources"))
            {
                textBox2.Text = @"Additional_Resources";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openBJDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openBJDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string bjpath = textBox1.Text;
            string cjpath = Application.StartupPath + @"\pjass\common.j";
            if (!File.Exists(bjpath)) { MessageBox.Show("invalid blizzard.j path!"); return; }
            if (openMapDialog.ShowDialog() != DialogResult.OK) { return; }
            MpqLib.Mpq.CArchive map = new MpqLib.Mpq.CArchive(openMapDialog.FileName);
            string[] listfile = GenerateMapFilelist(map);
            if (saveMpqDialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(saveMpqDialog.FileName)) { File.Delete(saveMpqDialog.FileName); }
                //==================
                MpqLib.Mpq.CArchive mpq = new MpqLib.Mpq.CArchive(saveMpqDialog.FileName, true, MpqLib.Mpq.EArchiveFormat.Version1, 10000);
                //添加地图内文件
                for (int i = 0; i < listfile.Length; i++)
                {
                    
                    MpqLib.Mpq.CFileStream file = new MpqLib.Mpq.CFileStream(map, listfile[i]);
                    byte[] fileb = new byte[file.Length];
                    fileb = file.Read((int)file.Length);
                    file.Close();
                    mpq.ImportFile(listfile[i], fileb);
                    fileb = null;
                    
                    /*
                    if (map.FileExists(listfile[i]))
                    {
                        if (File.Exists("temp")) { File.Delete("temp"); }
                        map.ExportFile(listfile[i], "temp");
                        mpq.ImportFile(listfile[i], "temp");
                    }
                    */
                }
                map.Close();
                //添加blizzard.j
                mpq.ImportFile(@"Scripts\Blizzard.j", bjpath);
                //添加blizzard.j
                mpq.ImportFile(@"Scripts\common.j", cjpath);
                //添加additional resources...
                PackAllFileToMpq(textBox2.Text.Trim(), textBox2.Text.Trim(), mpq);
                //关闭mpq
                mpq.Flush();
                mpq.Compact();
                mpq.Close();
                //测试地图
                MessageBox.Show("Mpq generated at:"+Environment.NewLine+saveMpqDialog.FileName,"Finish!");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void folderBrowserDialogAR_HelpRequest(object sender, EventArgs e)
        {

        }
            
        
    }
}
