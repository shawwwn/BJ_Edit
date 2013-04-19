using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace BJ_Edit
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            System.Globalization.CultureInfo UICulture = new System.Globalization.CultureInfo("en-GB");
            Thread.CurrentThread.CurrentUICulture = UICulture;
            InitializeComponent();
        }

        public string[] race =new string[] { "Human", "Orc", "Undead","NightElf" };
        public string[] raceai = new string[] { "human.ai", "orc.ai", "undead.ai", "elf.ai" };
        public string[] hero = new string[] { "Hamg", "Hmkg", "Hpal", "Hblm", "Obla", "Ofar", "Otch", "Oshd", "Edem", "Ekee", "Emoo", "Ewar", "Udea", "Udre", "Ulic", "Ucrl", "Npbm", "Nbrn", "Nngs", "Nplh", "Nbst", "Nalc", "Ntin", "Nfir" };
        public string[] defaultrace = new string[4] { "Human", "Orc", "Undead", "NightElf" };
        public ArrayList raceinfo_list = new ArrayList();
        public string racetext = "";
        public bool isjustload = false;


        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ((Form1)this.Owner).button10_Click(null, EventArgs.Empty);
            ((Form1)this.Owner).button11_Click(null, EventArgs.Empty);
            ((Form1)this.Owner).button3_Click(null, EventArgs.Empty);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Text="";
            for (int i=0;i<race.Length;i++)
            {
                textBox1.Text += this.race[i] + ",";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] temp = textBox1.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            race = temp;
            ArrayList tmp = new ArrayList();
            int j = -1;
            for (int i = 0; i < race.Length; i++)
            {
                MultiRace.raceinfo r;
                j=HasData(race[i]);
                if (j == -1)    //若无对应，即时生成一个默认的
                {
                    if (Array.IndexOf(defaultrace, race[i]) != -1) { continue; }    //已经在默认四族里面的
                    r = new MultiRace.raceinfo();
                    r.raceName = race[i];
                    r.peonID = "opeo";
                    r.baseID = "ogre";
                    r.aiPath = r.raceName.ToLower().Trim() + ".ai";
                    r.peonCount = 5;
                }
                else { r = (MultiRace.raceinfo)raceinfo_list[j]; }
                r.aiPath = r.aiPath.Trim();
                r.baseID = r.baseID.Trim();
                r.peonID = r.peonID.Trim();
                tmp.Add(r);
            }
            raceinfo_list = tmp;
            this.Hide();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)   //Enter键
            {
                
            }
        }

        private void ApplyData(string racename)
        {
            for (int i = 0; i < raceinfo_list.Count; i++)
            {
                if (((MultiRace.raceinfo)raceinfo_list[i]).raceName == racename)
                {
                    textBox3.Text = ((MultiRace.raceinfo)raceinfo_list[i]).baseID;
                    textBox4.Text = ((MultiRace.raceinfo)raceinfo_list[i]).aiPath;
                    textBox2.Text = ((MultiRace.raceinfo)raceinfo_list[i]).peonID;
                    numericUpDown1.Value = ((MultiRace.raceinfo)raceinfo_list[i]).peonCount;
                    panel1.Enabled = true;
                    return;
                }
            }
            //无法找到相应数据

        }

        private int HasData(string racename)
        {
            for (int i = 0; i < raceinfo_list.Count; i++)
            {
                if (((MultiRace.raceinfo)raceinfo_list[i]).raceName == racename) 
                {
                    return i;
                }
            }
            return -1;
        }

        private void SaveData(string racename)
        {
            MultiRace.raceinfo tmp = new MultiRace.raceinfo();
            tmp.aiPath = textBox4.Text;
            tmp.baseID = textBox3.Text;
            tmp.peonID = textBox2.Text;
            tmp.peonCount = (int)numericUpDown1.Value;
            tmp.raceName = racename;
            if (HasData(racename) != -1)
            { raceinfo_list[HasData(racename)] = tmp; }
            else
            { raceinfo_list.Add(tmp); }
        }


        private void textBox1_Click(object sender, EventArgs e)
        {
            int SelectedValue = this.textBox1.SelectionStart;
            //MessageBox.Show(SelectedValue.ToString());
            if (SelectedValue == textBox1.Text.Length) { checkBox1.Enabled = false; return; }
            int upbound=textBox1.Text.Length, lowbound=0;
            //获得上标
            for (int i = SelectedValue; i < textBox1.Text.Length; i++)
            { if (textBox1.Text[i] == ',') { upbound = i; break; } }
            //获得下标
            for (int i = SelectedValue; i >= 0; i--)
            {
                if (textBox1.Text[i] == ',')
                {
                    if (i == SelectedValue) { continue; }
                    lowbound = i + 1;
                    break;
                }
            }
            if (upbound <= lowbound) { checkBox1.Enabled = false; return; }    //防止出错
            textBox1.SelectionStart = lowbound;
            textBox1.SelectionLength = upbound - lowbound;
            //textBox1.Focus();
            racetext = textBox1.SelectedText;
            if (racetext == "")
            { checkBox1.Enabled = false; return; }  //防止出错
            else { checkBox1.Enabled = true; }

            //判断是否有raceinfo
            if (HasData(racetext) != -1)
            {
                checkBox1.Checked = false;
                ApplyData(racetext);    //读取raceinfo
                if (Array.IndexOf(defaultrace, textBox1.SelectedText) == -1) { checkBox1.Enabled = false; }
            }
            //判断是否是默认四族
            else if (Array.IndexOf(defaultrace, textBox1.SelectedText) != -1)
            {
                checkBox1.Checked = true;   //是则可以允许使用默认数据
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                numericUpDown1.Value = 0;
            }
            else       //不是则禁掉checkbox
            {
                checkBox1.Checked = false;
                checkBox1.Enabled = false;
                panel1.Enabled = true;
                textBox2.Text = "opeo";
                textBox3.Text = "ogre";
                textBox4.Text = racetext.ToLower() + ".ai";
                numericUpDown1.Value = 5;
                SaveData(racetext);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                //删除选中数据
                if (HasData(racetext) != -1) { raceinfo_list.RemoveAt(HasData(racetext)); }
                panel1.Enabled = false; 
            }
            else
            { panel1.Enabled = true; }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            SaveData(racetext);
            MessageBox.Show("Update successfully.");
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            this.isjustload = checkBox2.Checked;
        }
    }
}
