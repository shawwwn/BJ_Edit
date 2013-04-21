using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace wtsTransfer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public int pos = 0;
        public StreamWriter inilist_sw = null;

        public void RestoreFileString(string file, string[] stringlist, string pretext, bool editprogressbar=false)
        {
            if (editprogressbar)
            {
                //用于设置进度条。
                progressBar1.Minimum = 0;
                progressBar1.Maximum = stringlist.Length;
                progressBar1.Step = 1;
                progressBar1.Value = 0;
            }

            //先读文件内容
            StreamReader sr = new StreamReader(file);
            string files = sr.ReadToEnd();
            sr.Close();
            
            //进行更改。
            for (int i = stringlist.Length - 1; i >= 0; i--)    //逆序排序，从大到小不会重复。
            {
                string[] tmps = stringlist[i].Split(new[] { '|' }, 2, StringSplitOptions.RemoveEmptyEntries);
                //tmps[0] - Index / tmps[1] - value
                files = files.Replace(pretext + tmps[0].Trim(), tmps[1].Trim());
                if (editprogressbar) { progressBar1.PerformStep(); }
            }
            StreamWriter wts_sw = new StreamWriter(file);
            wts_sw.Write(files);
            wts_sw.Close();
        }

        public bool HasChinese(string CString)
        {
            bool BoolValue = false;
            for (int i = 0; i < CString.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToChar(CString.Substring(i, 1))) > Convert.ToInt32(Convert.ToChar(128)))
                {
                    BoolValue = true;
                }

            }
            return BoolValue;
        }

        private void MapStringFromFdf(string fdfpath) //必须要新建了inilist_sw才能用。
        {
            fdf_file fdf = new fdf_file(fdfpath);
            StreamWriter sw = new StreamWriter(fdfpath, false);
            sw.Write(fdf.head+Environment.NewLine);
            for (int i = 0; i < fdf.body.Count; i++)
            {
                string line = (string)fdf.body[i];
                int start=line.IndexOf('"') + 1;
                if (line.Trim() != "" && start != 0 && HasChinese(line))
                {
                    string replacement = line.Substring(start, line.LastIndexOf('"') - start);
                    line = line.Replace(replacement, "INI_STRING " + pos.ToString());
                    inilist_sw.WriteLine(pos.ToString() + " | " + replacement);
                    pos++;
                }
                sw.WriteLine(line);
            }
            sw.Write(fdf.feet);
            sw.Close();
        }

        private void MapStringFromIni(string inipath, bool selfmodify = false) //必须要新建了inilist_sw才能用。selfmodify是在规范化文件的时候不直接操作原文件而是复制一个临时文件进行操作。
        {
            ini_file aa = new ini_file(inipath, selfmodify);
            ArrayList tmp = aa.ReadSections();
            for (int i = 0; i < tmp.Count; i++)
            {
                string section = ((string)tmp[i]);
                //ArrayList tmp2 = aa.ReadKeys(section);    //无法读取"号

                ArrayList tmp2 = aa.GetIniSectionValue(section);
                ArrayList repetition = new ArrayList();

                for (int j = 0; j < tmp2.Count; j++)
                {
                    string KeynValue = ((string)tmp2[j]).Trim();
                    //MessageBox.Show((string)tmp2[tmp2.Count-1]);
                    int divider = KeynValue.IndexOf("=");
                    if (divider == -1 || KeynValue.IndexOf("//") == 0) { continue; }    //其他杂七杂八的数据
                    string key = KeynValue.Substring(0, divider).Trim();
                    string value = KeynValue.Substring(key.Length + 1, KeynValue.Length - key.Length - 1).Trim();
                    if (HasChinese(value))
                    {
                        //MessageBox.Show(value);
                        string replacement = "INI_STRING " + pos.ToString();
                        if (value[0] == '"' && value[value.Length - 1] == '"')
                        {
                            replacement = '"' + replacement + '"';
                            value = value.Substring(1, value.Length - 2);
                        }
                        //发现上面有key跟当前key重复
                        if (repetition.Contains(key))
                        {   //独立函数
                            aa.WriteIniKeys_Indp(section, key, replacement, value);
                        }
                        else { aa.WriteIniKey(section, key, replacement); }
                        inilist_sw.WriteLine(pos.ToString() + " | " + value);
                        pos = pos + 1;
                    }
                    repetition.Add(key);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dr= openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "" && dr == DialogResult.OK)
            {
                wts_file wts = new wts_file(openFileDialog1.FileName);
                wts.SaveStripDataFile();
                wts.StringMapping();
                MessageBox.Show("Finish!");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string WidgetzInputFolder = Application.StartupPath+@"\Widgetizer\Input";
            if (!Directory.Exists(WidgetzInputFolder))
            {   //不存在Widgetizer，需用户自行定义。
                if (folderBrowserDialog1.ShowDialog() == DialogResult.Cancel) { return; }
                else { WidgetzInputFolder = folderBrowserDialog1.SelectedPath; }
            }
            //备份input文件夹
            if (Directory.Exists(@"Input_bak")) { Directory.Delete(@"Input_bak", true); }
            FileIO.CopyDirectory(WidgetzInputFolder, @"Input_bak");

            inilist_sw = new StreamWriter("inilist.txt", false, Encoding.UTF8);
            pos = 0;
            //遍历文件夹
            DirectoryInfo TheFolder = new DirectoryInfo(WidgetzInputFolder);
            FileInfo[] TheFiles=TheFolder.GetFiles();
            //用于设置进度条。
            progressBar1.Minimum = 0;
            progressBar1.Maximum = TheFiles.GetLength(0);
            progressBar1.Step = 1;
            progressBar1.Value = 0;
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                //进度条
                ((Form1)Application.OpenForms[0]).progressBar1.PerformStep();
                //MessageBox.Show(NextFile.FullName);
                switch (NextFile.Extension) {
                    case ".slk":    //slk不处理
                        break;
                    case ".fdf":    //fdf处理函数
                        MapStringFromFdf(NextFile.FullName);
                        break;
                    case ".txt":    //判断是哪种txt
                        MapStringFromIni(NextFile.FullName);
                        break;
                }
            }

            MessageBox.Show("Successfully transfered " + pos.ToString() + " strings.", "Finish!");
            //释放句柄
            inilist_sw.Close();
            inilist_sw.Dispose();
            inilist_sw = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //选择data；
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(openFileDialog2.FileName);
                
                string[] ss=sr.ReadToEnd().Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
                sr.Close();

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    RestoreFileString(openFileDialog1.FileName, ss, "GAME_STRING ", true);
                    MessageBox.Show("Finish!");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //选择inifilelist；
            openFileDialog2.FileName = "inilist.txt";
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(openFileDialog2.FileName);

                string[] ss = sr.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                sr.Close();

                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    //遍历文件夹
                    DirectoryInfo TheFolder = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                    FileInfo[] TheFiles = TheFolder.GetFiles();
                    //用于设置进度条。
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = TheFiles.GetLength(0);
                    progressBar1.Step = 1;
                    progressBar1.Value = 0;
                    foreach (FileInfo NextFile in TheFolder.GetFiles())
                    {
                        RestoreFileString(NextFile.FullName, ss, "INI_STRING "); //it also comes with the "GAME_STRING "
                        progressBar1.PerformStep();
                        Application.DoEvents();
                        this.Refresh();
                    }
                    MessageBox.Show("Finish!");
                }
            }
            openFileDialog2.FileName = "wts_data.txt";
        }
    }
}
