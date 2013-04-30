using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Wc3LocalizationTool
{
    public partial class Form1 : Form
    {
        public class wts_struct
        {
            public int index { get; set; }
            public string header { get; set; }
            public string description { get; set; }
            public string content { get; set; }

            public wts_struct()
            {
                this.index = 0;
                this.header = "";
                this.description = "";
                this.content = "";
            }
        }

        public wts_struct[] wts = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            saveFileDialog1.Filter = openFileDialog1.Filter;
            saveFileDialog1.FileName = openFileDialog1.FileName;
        }

        private void SortListBox()
        {
            if (wts!=null && wts.Length > 0)
            {
                listBox1.Enabled = false;
                listBox1.Items.Clear();
                IsFirstTime=true;
                Application.DoEvents();
                wts = bubbleUp(wts);
                for (int i = 0; i < wts.Length; i++)
                {
                    listBox1.Items.Add(wts[i].header);
                }
                listBox1.Enabled = true;
            }
            else
            { MessageBox.Show("未载入"); }
        }

        private wts_struct[] bubbleUp(wts_struct[] Array)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                for (int j = i + 1; j < Array.Length; j++)
                {
                    if (Array[i].index > Array[j].index)
                    {
                        wts_struct temp = Array[i];
                        Array[i] = Array[j];
                        Array[j] = temp;
                    }
                }
            }
            return Array;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) { return; }
            listBox1.Items.Clear();
            StreamReader sr = new StreamReader(openFileDialog1.FileName);
            string content = sr.ReadToEnd();
            sr.Close();
            Regex r = new Regex(@"STRING(?:.|\n)*?}", RegexOptions.IgnoreCase);
            Regex r2 = new Regex(@"{(?:.|\n)*?}", RegexOptions.IgnoreCase);
            MatchCollection mc = r.Matches(content);
            wts = new wts_struct[mc.Count];
            for (int i = 0; i < mc.Count; i++)
            {
                wts[i]=new wts_struct();
                string tmp = mc[i].Value;
                wts[i].index = int.Parse(tmp.Substring(6, tmp.IndexOf(Environment.NewLine) - 6).Trim());
                //listBox1.Items.Add(wts[i].index);
                wts[i].header = "STRING " + wts[i].index.ToString();
                listBox1.Items.Add(wts[i].header);  //添加到listbox
                
                string[] lines = tmp.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < lines.Length; j++)
                {
                    if (lines[j].IndexOf(@"//") == 0)
                    {
                        wts[i].description += lines[j] + Environment.NewLine;
                    }
                }
                wts[i].content = r2.Match(tmp).Value.Trim(new[] { '{', '}' }).Trim();
            }
            //=========完毕=========
            IsFirstTime = true;
            /*
            listBox1.DataSource = wts;
            listBox1.DisplayMember = "header";
            listBox1.ValueMember = "index";
            */

        }

        static int lastselectedindex;
        static bool IsFirstTime = true;

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.Enabled == false) { return; }
            if (!IsFirstTime)
            {
                wts[lastselectedindex].index = (int)indexnumBox.Value;
                wts[lastselectedindex].description = destextBox.Text;
                wts[lastselectedindex].content = contenttextBox.Text;
            }
            //==============================
            indexnumBox.Value = wts[listBox1.SelectedIndex].index;
            destextBox.Text = wts[listBox1.SelectedIndex].description;
            contenttextBox.Text = wts[listBox1.SelectedIndex].content;
            lastselectedindex = listBox1.SelectedIndex;
            IsFirstTime = false;
            listBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1_SelectedIndexChanged(null, null);
            if (listBox1.Items.Count <= 0 && wts==null) { MessageBox.Show("No file to be save!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, false, Encoding.UTF8);
                for (int i = 0; i < wts.Length; i++)
                {
                    sw.WriteLine(wts[i].header);
                    if (wts[i].description != "")
                    {
                        if (wts[i].description.IndexOf("//") != 0) { wts[i].description = "//" + wts[i].description; }
                        sw.Write(wts[i].description.Trim() + Environment.NewLine);
                    }
                    sw.WriteLine("{");
                    sw.Write(wts[i].content.Trim() + Environment.NewLine);
                    sw.WriteLine("}");
                    sw.WriteLine();
                }
                sw.Close();
            }
        }

        private void destextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void contenttextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void contenttextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                if (e.KeyCode == Keys.Enter && checkBox1.Checked == false) { return; }
                e.SuppressKeyPress = true;
                if (listBox1.Items.Count != 0 && listBox1.SelectedIndex + 1 < listBox1.Items.Count)
                {
                    listBox1.SelectedIndex++;
                    if (destextBox.Focused)
                    { destextBox.Select(2, destextBox.Text.Length - 2); }
                    else if (contenttextBox.Focused)
                    { contenttextBox.SelectAll(); }
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                destextBox.Focus();
                destextBox.Select(2, destextBox.Text.Length - 2);
            }
        }

        private void destextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                if (e.KeyCode == Keys.Enter && checkBox1.Checked == false) { return; }
                e.SuppressKeyPress = true;
                contenttextBox.Focus();
                contenttextBox.SelectAll();
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                if (listBox1.Items.Count != 0 && listBox1.SelectedIndex - 1 >= 0)
                {
                    listBox1.SelectedIndex--;
                    if (destextBox.Focused)
                    { destextBox.Select(2, destextBox.Text.Length - 2); }
                    else if (contenttextBox.Focused)
                    { contenttextBox.SelectAll(); }
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void contenttextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void destextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

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

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (wts == null) { return; }
            if (wts[e.Index] != null)
            {
                if (HasChinese(wts[e.Index].content))
                {
                    e.DrawBackground();
                    e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, Brushes.LightSeaGreen, e.Bounds);
                    e.DrawFocusRectangle();
                    return;
                }
            }
            e.DrawBackground();
            e.DrawFocusRectangle();
            e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (wts == null || wts.Length<=0) { MessageBox.Show("需要先载入wts。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (openFileDialog1.ShowDialog() != DialogResult.OK) { return; }
            //listBox1.ClearSelected();
            listBox1.Enabled = false;
            Application.DoEvents();
            StreamReader sr = new StreamReader(openFileDialog1.FileName);
            string content = sr.ReadToEnd();
            sr.Close();
            Regex r = new Regex(@"STRING(?:.|\n)*?}", RegexOptions.IgnoreCase);
            Regex r2 = new Regex(@"{(?:.|\n)*?}", RegexOptions.IgnoreCase);
            MatchCollection mc = r.Matches(content);
            wts_struct[] import = new wts_struct[mc.Count];
            int count = 0;
            for (int i = 0; i < mc.Count; i++)
            {
                import[i] = new wts_struct();
                string tmp = mc[i].Value;
                import[i].index = int.Parse(tmp.Substring(6, tmp.IndexOf(Environment.NewLine) - 6).Trim());
                //listBox1.Items.Add(wts[i].index);
                import[i].header = "STRING " + import[i].index.ToString();
                string[] lines = tmp.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < lines.Length; j++)
                {
                    if (lines[j].IndexOf(@"//") == 0)
                    {
                        import[i].description += lines[j] + Environment.NewLine;
                    }
                }
                import[i].content = r2.Match(tmp).Value.Trim(new[] { '{', '}' }).Trim();
                //查找并替换
                if (import[i].description.Trim() != "")
                {

                    int point = import[i].description.IndexOf(":") + 1;
                    string ID = import[i].description.Substring(point, import[i].description.IndexOf('(') - point).Trim();
                    //MessageBox.Show(ID);
                    point = import[i].description.LastIndexOf('[') + 1;
                    string subID = import[i].description.Substring(point, import[i].description.LastIndexOf(']') - point).Trim();
                    //MessageBox.Show(subID);

                    if (ID != "" && subID != "")
                    {
                        for (int k = 0; k < wts.Length; k++)
                        {
                            if (wts[k].description.Trim() != "")
                            {
                                point = wts[k].description.IndexOf(':') + 1;
                                string ID2 = wts[k].description.Substring(point, wts[k].description.IndexOf('(') - point).Trim();
                                point = wts[k].description.LastIndexOf('[') + 1;
                                string subID2 = wts[k].description.Substring(point, wts[k].description.IndexOf(']') - point).Trim();
                                if (ID == ID2 && subID == subID2)   //找到
                                {
                                    wts[k].header = import[i].header;
                                    wts[k].description = import[i].description;
                                    wts[k].content = import[i].content;
                                    wts[k].index = import[i].index;
                                    listBox1.Items[k] = import[i].header;
                                    count++;
                                    Application.DoEvents();
                                }
                            }
                        }
                    }
                }
            }
            listBox1.Enabled = true;
            IsFirstTime = true;
            MessageBox.Show("一共替换 " + count + " 个字符串。");
        }

        private void indexnumBox_ValueChanged(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Sorted = true;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            SortListBox();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (wts != null && wts.Length > 0)
            {
                string list = "";
                for (int i = 0; i < wts.Length; i++)
                {
                    if (HasChinese(wts[i].content))
                    {
                        list = list + wts[i].index.ToString() + ";";
                    }
                }
                if (list.Trim() != "")
                {
                    if (list.Length > 80)
                    {
                        MessageBox.Show("还有很多。");
                    }
                    else
                    {
                        MessageBox.Show("还有：" + Environment.NewLine + list);
                    }
                }
                else
                {
                    MessageBox.Show("没发现unicode字符。");
                }

            }
        }
    }
}
