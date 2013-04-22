using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace wtsTransfer
{
    class wts_file
    {
        public class wts_structure
        {
            public int index = 0;
            public string str = "";
        }
        public string[] wts_list;
        public ArrayList wts_stringlist = new ArrayList();
        public string wtsFilePath = "";
        
        public wts_file(string wtsPath)
        {
            if (wtsPath == "" || !File.Exists(wtsPath)) { return; } else { wtsFilePath = wtsPath; }
            string ss = "";
            StreamReader wts = new StreamReader(wtsPath);
            ss = wts.ReadToEnd().Trim();
            wts_list = ss.Split(new[] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //用于设置进度条。
            ((Form1)Application.OpenForms[0]).progressBar1.Minimum = 0;
            ((Form1)Application.OpenForms[0]).progressBar1.Maximum = wts_list.Length;
            ((Form1)Application.OpenForms[0]).progressBar1.Step = 1;
            ((Form1)Application.OpenForms[0]).progressBar1.Value = 0;
            for (int i = 0; i < wts_list.Length; i++)
            {
                //进度条
                ((Form1)Application.OpenForms[0]).progressBar1.PerformStep();
                //MessageBox.Show(wts_list[i]);
                string[] section = wts_list[i].Trim().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                string sindex = "";
                wts_structure tmp = new wts_structure();
                for (int j = 0; j < section.Length; j++)
                {
                    section[j] = section[j].Trim();
                    if (section[j].IndexOf("STRING") == 0)
                    {
                        sindex = section[j].Substring(7);
                        if (int.Parse(sindex) != 0) { tmp.index = int.Parse(sindex); }
                    }
                    else if (section[j].IndexOf("//") == 0)
                    { 
                    }
                    else if (section[j].IndexOf("{") == 0)
                    {
                        j++;
                        while (j < section.Length && section[j].IndexOf("}") != 0)
                        {
                            tmp.str = tmp.str + section[j];
                            j++;
                        }
                        //MessageBox.Show("++++" + tmp.str + "====");
                    }
                }

                //判断数据是否有效
                if (tmp.index != 0 && tmp.str != "")
                {
                    wts_stringlist.Add(tmp);
                }
            }
            //MessageBox.Show(wts_stringlist.Count.ToString());   //总数
            //MessageBox.Show(((wts_structure)wts_stringlist[1992]).str);
            wts.Close();
        }
    
        public void SaveStripDataFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "wts_data.txt";
            sfd.Filter = "*.txt|text file";
            sfd.Title = "Save to..";
            sfd.ShowDialog();
            //MessageBox.Show(sfd.FileName);
            if (sfd.FileName != "")
            {
                StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8);
                for (int i = 0; i < wts_stringlist.Count; i++)
                {
                    wts_structure temp=(wts_structure)wts_stringlist[i];
                    sw.WriteLine(temp.index.ToString() + " | " + temp.str);
                }
                sw.Close();
                MessageBox.Show("Save file at:" + Environment.NewLine + sfd.FileName);
            }
            else { return; }
        }

        public void StringMapping()
        {
            if (wtsFilePath == "") { MessageBox.Show("Hasn't loaded any file yet."); return; }

            //StreamWriter sw = new StreamWriter(wtsFilePath.Substring(0, wtsFilePath.LastIndexOf('.')) + "_new.wts", false, Encoding.UTF8);
            StreamWriter sw = new StreamWriter(wtsFilePath, false, Encoding.UTF8);
            for (int i = 0; i < wts_stringlist.Count; i++)
            {
                wts_structure temp=(wts_structure)wts_stringlist[i];
                sw.WriteLine("STRING " + temp.index);
                sw.WriteLine("{");
                sw.WriteLine("GAME_STRING " + temp.index);
                sw.WriteLine("}");
                sw.WriteLine();
            }
            sw.Close();
        }
    }
}
