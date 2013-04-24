using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Threading;

namespace wtsTransfer
{
    public partial class Form1 : Form
    {
        #region Units文件夹下还原列表
        string[] unitsfilelist = {"CampaignAbilityStrings.txt",
                                  "CampaignUnitStrings.txt",
                                  "CampaignUpgradeStrings.txt",
                                  "CommandStrings.txt",
                                  "CommonAbilityStrings.txt",
                                  "GlobalStrings.fdf",
                                  "HumanAbilityStrings.txt",
                                  "HumanUnitStrings.txt",
                                  "HumanUpgradeStrings.txt",
                                  "ItemAbilityStrings.txt",
                                  "ItemStrings.txt",
                                  "NeutralAbilityStrings.txt",
                                  "NeutralUnitStrings.txt",
                                  "NeutralUpgradeStrings.txt",
                                  "NightElfAbilityStrings.txt",
                                  "NightElfUnitStrings.txt",
                                  "NightElfUpgradeStrings.txt",
                                  "OrcAbilityStrings.txt",
                                  "OrcUnitStrings.txt",
                                  "OrcUpgradeStrings.txt",
                                  "UndeadAbilityStrings.txt",
                                  "UndeadUnitStrings.txt",
                                  "UndeadUpgradeStrings.txt",
                                  "UnitGlobalStrings.txt"
                                 };
        #endregion
        public Form1()
        {
            System.Globalization.CultureInfo UICulture = new System.Globalization.CultureInfo("en-GB");
            Thread.CurrentThread.CurrentUICulture = UICulture;
            InitializeComponent();
        }

        public int pos = 0;
        public StreamWriter inilist_sw = null;
        Image btnimg1;

        private void PackAllFileToMpq(string path, string basepath, MpqLib.Mpq.CArchive mpq)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo fChild in dir.GetFiles("*")) //设置文件类型
            {
                string mpqpath = fChild.FullName.Replace(basepath + @"\", "");
                mpq.ImportFile(mpqpath, fChild.FullName);
            }

            foreach (DirectoryInfo dChild in dir.GetDirectories("*")) //操作子目录
            {
                PackAllFileToMpq(dChild.FullName, basepath, mpq); //递归
            }
        }

        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }

        public byte[] ReplaceBytes(byte[] src, string replace, string replacewith)
        {
            string hex = BitConverter.ToString(src);
            hex = hex.Replace("-", "");
            hex = hex.Replace(replace, replacewith);
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public void TransferStatus(bool IsTransfering)
        {
            button1.Enabled = !IsTransfering;
            button2.Enabled = !IsTransfering;
            button3.Enabled = !IsTransfering;
            button4.Enabled = !IsTransfering;
            button5.Enabled = !IsTransfering;
            button7.Enabled = !IsTransfering;
            button8.Enabled = !IsTransfering;
            button9.Enabled = !IsTransfering;
        }

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
                if (editprogressbar) { progressBar1.PerformStep(); Application.DoEvents(); }
            }
            StreamWriter wts_sw = new StreamWriter(file);
            wts_sw.Write(files);
            wts_sw.Close();
        }

        public void RestoreFileStringFromW3i(string w3ifile, string[] stringlist, string pretext, bool editprogressbar = false)
        {
            if (editprogressbar)
            {
                //用于设置进度条。
                progressBar1.Minimum = 0;
                progressBar1.Maximum = stringlist.Length;
                progressBar1.Step = 1;
                progressBar1.Value = 0;
            }

            //读取w3i
            FileStream fs = new FileStream(w3ifile, FileMode.Open, FileAccess.ReadWrite);
            byte[] ss=new byte[fs.Length];
            fs.Read(ss, 0, (int)fs.Length);
            fs.Close();
            //开始替换
            for (int i = stringlist.Length - 1; i >= 0; i--)    //逆序排序，从大到小不会重复。
            {
                string[] tmps = stringlist[i].Split(new[] { '|' }, 2, StringSplitOptions.RemoveEmptyEntries);
                byte[] source = System.Text.Encoding.UTF8.GetBytes(pretext + tmps[0].Trim());
                byte[] target = System.Text.Encoding.UTF8.GetBytes(tmps[1].Trim());
                ss = ReplaceBytes(ss, ToHexString(source), ToHexString(target));
                if (editprogressbar) { progressBar1.PerformStep(); Application.DoEvents(); }    //进度条
            }
            FileStream fsw = new FileStream(w3ifile, FileMode.Create, FileAccess.Write);
            fsw.Write(ss,0,ss.Length);
            fsw.Close();
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

        private void refreshfrm()
        {
            label1.Text = "";
            if (File.Exists("inilist.txt") && new FileInfo("inilist.txt").Length != 0)
            {
                StreamReader tempr = new StreamReader("inilist.txt");
                if (tempr.ReadToEnd().Trim() == "")
                {
                    button8.Image = null;
                }
                else
                { button8.Image = btnimg1; }
                tempr.Close();
                tempr.Dispose();
            }
            else
            {
                button8.Image = null;
            }
            if (checkBox1.Checked)
            { this.Height = 350; }
            else
            { this.Height = 190; }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            btnimg1 = button8.Image;
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            refreshfrm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string WidgetzInputFolder = "";
            if (WidgetizerFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                WidgetzInputFolder = WidgetizerFolderBrowserDialog.SelectedPath+@"\Input";
            }
            else { return; }
            //备份input文件夹
            TransferStatus(true);
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
                progressBar1.PerformStep();
                Application.DoEvents();
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
            TransferStatus(false);
            //释放句柄
            inilist_sw.Close();
            inilist_sw.Dispose();
            inilist_sw = null;
            refreshfrm();
            MessageBox.Show("Successfully transfered " + pos.ToString() + " strings.", "Finish!");
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
            if (openFileDialog2.ShowDialog() != DialogResult.OK) { return; }
            string inidata = openFileDialog2.FileName;
            openFileDialog2.FileName = "wts_data.txt";
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                string wtsdata = openFileDialog2.FileName;
                StreamReader sr = new StreamReader(inidata);
                string[] iniss = sr.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                sr.Close();
                StreamReader sr2 = new StreamReader(wtsdata);
                string[] wtsss = sr2.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                sr2.Close();

                if (WidgetizerFolderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    TransferStatus(true);
                    //遍历文件夹
                    DirectoryInfo TheFolder = new DirectoryInfo(WidgetizerFolderBrowserDialog.SelectedPath+@"\Input");
                    FileInfo[] TheFiles = TheFolder.GetFiles();
                    //用于设置进度条。
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = TheFiles.GetLength(0);
                    progressBar1.Step = 1;
                    progressBar1.Value = 0;
                    foreach (FileInfo NextFile in TheFolder.GetFiles())
                    {
                        if (NextFile.Extension == ".txt" || NextFile.Extension == ".fdf")
                        {
                            RestoreFileString(NextFile.FullName, iniss, "INI_STRING "); //it also comes with the "GAME_STRING "
                            Application.DoEvents();
                            RestoreFileString(NextFile.FullName, wtsss, "GAME_STRING ");
                            Application.DoEvents();
                        }
                        progressBar1.PerformStep();
                        Application.DoEvents();
                    }
                    TransferStatus(false);
                    refreshfrm();
                    MessageBox.Show("Finish!");
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(openFileDialog2.FileName);
                string[] ss = sr.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                sr.Close();

                if (openFileDialog3.ShowDialog() == DialogResult.OK)
                {
                    TransferStatus(true);
                    RestoreFileStringFromW3i(openFileDialog3.FileName, ss, "GAME_STRING ", true);
                }
                TransferStatus(false);
                refreshfrm();
                MessageBox.Show("Finish!");
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void Language_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.FlatAppearance.BorderSize = 0;
            Point point = new Point(btn.Left + this.Left + 5, btn.Top + this.Top + 40);
            contextMenuStrip1.Show(point);
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BJ_Edit.SetLanguage.SetLang("en-GB", this, this.GetType(),toolTip1);
            refreshfrm();
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BJ_Edit.SetLanguage.SetLang("zh-CHS", this, this.GetType(),toolTip1);
            refreshfrm();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (openMapDialog.ShowDialog() == DialogResult.OK)
            {
                TransferStatus(true);
                if (Directory.Exists("map")) { Directory.Delete("map", true); }
                Directory.CreateDirectory(@"map\Units");
                #region ---解压mpq---
                //开始解压文件
                label1.Text = "Extracting Files...";
                //用于设置进度条。
                progressBar1.Minimum = 0;
                progressBar1.Maximum = unitsfilelist.GetLength(0);
                progressBar1.Step = 1;
                progressBar1.Value = 0;
                MpqLib.Mpq.CArchive map = new MpqLib.Mpq.CArchive(openMapDialog.FileName);
                for (int i = 0; i < unitsfilelist.Length; i++)
                {
                    progressBar1.PerformStep();
                    string filename = @"Units\" + unitsfilelist[i];
                    if (map.FileExists(filename))
                    {
                        map.ExportFile(filename, @"map\Units\" + unitsfilelist[i]);
                    }
                }
                if (map.FileExists(@"war3map.w3i"))   //解w3i
                { map.ExportFile(@"war3map.w3i", @"map\" + @"war3map.w3i"); }

                if (map.FileExists(@"war3map.wts"))   //解wts
                { map.ExportFile(@"war3map.wts", @"map\" + @"war3map.wts"); }

                //map.Close();
                #endregion
                #region ---读取stringlist---
                label1.Text = "Load string list...";
                string inidata = @"inilist.txt";
                string wtsdata = @"wts_data.txt";
                if (!File.Exists(inidata)) { MessageBox.Show("inilist.txt missing"); return; }
                StreamReader sr = new StreamReader(inidata);
                string[] iniss = sr.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                sr.Close();
                if (!File.Exists(wtsdata)) { MessageBox.Show("wts_data.txt missing"); return; }
                StreamReader sr2 = new StreamReader(wtsdata);
                string[] wtsss = sr2.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                sr2.Close();
                #endregion
                #region ---还原w3i---
                if (File.Exists(@"map\war3map.w3i"))
                {
                    label1.Text = "Restoring war3map.w3i...";
                    RestoreFileStringFromW3i(@"map\war3map.w3i", wtsss, "GAME_STRING ", true);
                }
                #endregion
                #region ---还原wts---
                if (File.Exists(@"map\war3map.wts"))
                {
                    label1.Text = "Restoring war3map.wts...";
                    RestoreFileString(@"map\war3map.wts", wtsss, "GAME_STRING ", true);
                }
                #endregion
                #region ---还原slk---
                label1.Text = "Restoring slk...";
                //遍历文件夹
                DirectoryInfo TheFolder = new DirectoryInfo(@"map\Units");
                FileInfo[] TheFiles = TheFolder.GetFiles();
                //用于设置进度条。
                progressBar1.Minimum = 0;
                progressBar1.Maximum = TheFiles.GetLength(0);
                progressBar1.Step = 1;
                progressBar1.Value = 0;
                foreach (FileInfo NextFile in TheFolder.GetFiles())
                {
                    if (NextFile.Extension == ".txt" || NextFile.Extension == ".fdf")
                    {
                        RestoreFileString(NextFile.FullName, iniss, "INI_STRING "); //it also comes with the "GAME_STRING "
                        Application.DoEvents();
                        RestoreFileString(NextFile.FullName, wtsss, "GAME_STRING ");
                        Application.DoEvents();
                    }
                    progressBar1.PerformStep();
                    Application.DoEvents();
                }
                #endregion
                #region ---重新压入地图---
                //war3map.j导入
                if (File.Exists("war3map.j")) { File.Copy("war3map.j", @"map\war3map.j", true); }
                //war3mapMisc.txt导入
                if (File.Exists("war3mapMisc.txt")) { File.Copy("war3mapMisc.txt", @"map\war3mapMisc.txt", true); }
                label1.Text = "Import to map...";
                PackAllFileToMpq(Application.StartupPath + @"\map", Application.StartupPath + @"\map", map);
                map.Flush();
                map.Compact();
                map.Close();
                #endregion
                label1.Text = "Finish.";
                TransferStatus(false);
                refreshfrm();
                MessageBox.Show("Finish!");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            refreshfrm();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (button8.Image != null)
            {
                if (MessageBox.Show("It seems that you had already run this before.." + Environment.NewLine + Environment.NewLine + "Click 'OK' to continue and overwrite the current data.", "reminder", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK)
                {
                    return;
                }
            }
            button2.PerformClick();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (openMapDialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists("war3map_edited.wts")) { File.Delete("war3map_edited.wts"); }
                if (File.Exists("war3map.j")) { File.Delete("war3map.j"); }
                if (File.Exists("war3mapMisc.txt")) { File.Delete("war3mapMisc.txt"); }
                //导出文件
                MpqLib.Mpq.CArchive map = new MpqLib.Mpq.CArchive(openMapDialog.FileName);
                if (map.FileExists("war3map.j")) { map.ExportFile("war3map.j", "war3map.j"); }
                if (map.FileExists("war3mapMisc.txt")) { map.ExportFile("war3mapMisc.txt", "war3mapMisc.txt"); }
                if (map.FileExists("war3map.wts")) { map.ExportFile("war3map.wts", "war3map_edited.wts"); }
                else
                {
                    label1.Text = "war3map.wts doesn't exist in this map.";
                    map.Close(); 
                    return;
                }
                TransferStatus(true);
                label1.Text = "Transfering strings from war3map.wts...";
                wts_file wts = new wts_file("war3map_edited.wts");
                wts.SaveStripDataFile(true);
                wts.StringMapping();
                //打包进地图
                map.ImportFile("war3map.wts", "war3map_edited.wts");
                map.Flush();
                map.Compact();
                map.Close();
                if (File.Exists("war3map_edited.wts")) { File.Delete("war3map_edited.wts"); }
                TransferStatus(false);
                refreshfrm();
                label1.Text = "Finish.";
                MessageBox.Show("Finish!");
            }
        }
    }
}
