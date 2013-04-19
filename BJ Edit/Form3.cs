using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace BJ_Edit
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            //初始化窗体
            txtEdit.Visible = false;
            txtEdit.Text = "";
            textBox1.Text = "";
            listBox1.Items.Clear();
            //==========
            string Arguments = "";
            string F3_BJpath = Form1.BJpath;
            if (F3_BJpath != "" && System.IO.File.Exists(F3_BJpath)) { MessageBox.Show("Syntax Checking:" + Environment.NewLine + Environment.NewLine + F3_BJpath); }
            else 
            {
                MessageBox.Show("no file is waiting to be check..");
                this.Close();
            }
            string F3_BJPathFileName = F3_BJpath.Substring(F3_BJpath.LastIndexOf('\\') == -1 ? 0 : F3_BJpath.LastIndexOf('\\') + 1);
            if (F3_BJpath.IndexOf(" ") != -1) { F3_BJpath = "\"" + F3_BJpath + "\""; }
            if (F3_BJPathFileName.ToLower() != "blizzard.j")
            {
                Arguments = @"Original\Blizzard.j " + F3_BJpath;
            }
            else { Arguments = F3_BJpath; }
            //启动pjass
            Process p = new Process();
            p.StartInfo.FileName = @"pjass\pjass.exe";//要执行的程序名称
            p.StartInfo.Arguments = @"pjass\common.j pjass\common.ai " + Arguments;
            p.StartInfo.UseShellExecute = false;//要重定向 IO 流，Process 对象必须将 UseShellExecute 属性设置为 False
            p.StartInfo.RedirectStandardInput = true;//可能接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序
            string output = p.StandardOutput.ReadToEnd();
            //处理输出信息
            string[] errlist = output.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            this.textBox1.Text = "";
            for (int i = 0; i < errlist.Length; i++)
            {
                if (errlist[i].Substring(0, 6) == "Parse " || errlist[i].IndexOf(" failed with ") != -1)
                {
                    this.textBox1.Text += errlist[i] + Environment.NewLine;
                }
                else
                {
                    listBox1.Items.Add("line" + errlist[i].Substring(errlist[i].IndexOf(":")));
                }
            }
            //this.textBox1.Text=output;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            int itemSelected = listBox1.SelectedIndex;
            if (itemSelected == -1) { return; }
            string itemText = listBox1.Items[itemSelected].ToString();

            Rectangle rect = listBox1.GetItemRectangle(itemSelected);
            txtEdit.Parent = listBox1;
            txtEdit.Bounds = rect;
            txtEdit.Multiline = true;
            txtEdit.Size = new System.Drawing.Size(rect.Width, rect.Height);
            txtEdit.Visible = true;
            txtEdit.Text = itemText;
            txtEdit.Focus();
            txtEdit.SelectAll();
        }

        private void txtEdit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)   //Enter键
            {
                this.listBox1.Items[this.listBox1.SelectedIndex] = this.txtEdit.Text;
                this.txtEdit.Visible = false;
            }
            if (e.KeyChar == 27)   //Esc键
                this.txtEdit.Visible = false;
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            this.txtEdit.Visible = false;
        }
    }
}
