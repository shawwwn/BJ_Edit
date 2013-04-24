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
using System.Globalization;

namespace BJ_Edit
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            System.Globalization.CultureInfo UICulture = new System.Globalization.CultureInfo("en-GB");
            Thread.CurrentThread.CurrentUICulture = UICulture;
            InitializeComponent();
        }

        Bj_structure_original BJorg = new Bj_structure_original(@"Original\Blizzard.j");
        Bj_structure BJ = null;
        ArrayList EditItems = new ArrayList();
        ArrayList AddItems = new ArrayList();
        bool IsNewBJ = true;
        w3j_structure W3J;
        Form2 frm2 = new Form2();
        Form3 frm3 = new Form3();

        public static string BJpath="";

        public void ChangeLabel(int flag)
        {
            //flag=1为正在处理，flag=0为处理完毕
            if (flag == 1)
            { label3.Text = "processing..."; }
            else if (flag == 0)
            { label3.Text = ""; }
            this.Refresh(); //Application.DoEvents();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.AddOwnedForm(frm2);
           /*
            Form2 frm = new Form2();
            this.AddOwnedForm(frm);
            frm.Show();
            */
            label3.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangeLabel(1);
            EditItems.Clear();
            AddItems.Clear();
            IsNewBJ = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            //Fname.DataSource= (string[])BJorg.Global_org.ToArray(typeof(string));

            Fname.Items.Clear();
            Fcontent.Text = "";
            Fdescriptions.Text = "";
            for (int i=0; i<BJorg.FunctionCount_org;i++)
            {
                Fname.Items.Add(((Bj_structure_original.function)BJorg.Function_org[i]).name);
            }
            ChangeLabel(0);
        }

        private void Fname_SelectedIndexChanged(object sender, EventArgs e)
        {
            Fcontent.Text = "";
            Fdescriptions.Text = "";
            if (IsNewBJ == true)
            {
                if (BJ.substance[Fname.SelectedIndex].GetType() == typeof(Bj_structure.function))  //fucntions
                {
                    Fcontent.Text = ((Bj_structure.function)BJ.substance[Fname.SelectedIndex]).content;
                    Fdescriptions.Text = ((Bj_structure.function)BJ.substance[Fname.SelectedIndex]).description;
                }
                else if (BJ.substance[Fname.SelectedIndex].GetType() == typeof(Bj_structure.comment))  //comments
                {
                    Fcontent.Text = ((Bj_structure.comment)BJ.substance[Fname.SelectedIndex]).comments;
                }
                else if (BJ.substance[Fname.SelectedIndex].GetType() == typeof(ArrayList))  // globals
                {
                    ArrayList temp = ((ArrayList)BJ.substance[Fname.SelectedIndex]);
                    string temps = "";
                    for (int i = 0; i < temp.Count; i++)
                    {

                        temps += temp[i].ToString() + System.Environment.NewLine;
                    }
                    Fcontent.Text = temps.TrimEnd() + System.Environment.NewLine;
                }
                else if (BJ.substance[Fname.SelectedIndex].GetType() == typeof(string))  //w3j additions
                {
                    Fcontent.Text = ((string)BJ.substance[Fname.SelectedIndex]);
                }

            }
            else
            {
                for (int i = 0; i < BJorg.Function_org.Count; i++)
                {
                    if (Fname.SelectedItem.ToString() == ((Bj_structure_original.function)BJorg.Function_org[i]).name)
                    {
                        Fcontent.Text = ((Bj_structure_original.function)BJorg.Function_org[i]).content;
                    }
                }
            }
            /*
            for (int i = 0; i < BJ.substance.Count; i++)
            {
                if (BJ.substance[i].GetType() == typeof(Bj_structure.function))
                {
                    if (Fname.SelectedItem.ToString() == ((Bj_structure.function)BJ.substance[i]).name)
                    {
                        Fcontent.Text = ((Bj_structure.function)BJ.substance[i]).content;
                        Fdescriptions.Text = ((Bj_structure.function)BJ.substance[i]).description;
                    }
                }
                else if (BJ.substance[i].GetType() == typeof(Bj_structure.comment))
                {
                   
                    Fcontent.Text = ((Bj_structure.comment)BJ.substance[i]).comments;
                }
            }
            
            for (int i = 0; i < BJorg.Function_org.Count; i++)
            {
                if (Fname.SelectedItem.ToString() == ((Bj_structure_original.function)BJorg.Function_org[i]).name)
                {
                    Fcontent.Text = ((Bj_structure_original.function)BJorg.Function_org[i]).content;
                }
            }
            */
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        public void button3_Click(object sender, EventArgs e)
        {
            ChangeLabel(1);
            EditItems.Clear();
            AddItems.Clear();
            if (!System.IO.Directory.Exists("Output")) { System.IO.Directory.CreateDirectory("Output"); }
            StreamWriter BJ_writer = new StreamWriter(@"Output\Blizzard.j");
            for (int i=0; i<BJ.substance.Count; i++)
            {
                if (BJ.substance[i].GetType() == typeof(Bj_structure.comment))  //Comments
                {
                    string s = ((Bj_structure.comment)BJ.substance[i]).comments.Trim();
                    BJ_writer.Write(s+System.Environment.NewLine+System.Environment.NewLine); //空出一行
                }
                else if (BJ.substance[i].GetType() == typeof(ArrayList))    //Globals
                {
                    bool Ghead = true;
                    BJ_writer.WriteLine("globals" + System.Environment.NewLine); //空出一行
                    for (int j = 0; j < ((ArrayList)BJ.substance[i]).Count; j++)
                    {
                        string s=((ArrayList)BJ.substance[i])[j].ToString();
                        while (s.IndexOf("MC_MC_", StringComparison.OrdinalIgnoreCase) != -1) { s = s.Replace("mc_", "MC_").Replace("MC_MC_", "MC_"); }
                        //去除开头空白
                        if (Ghead)
                        {
                            if (s.Trim().Length != 0) Ghead = false;
                            else continue;
                        }
                        BJ_writer.WriteLine(s);
                    }
                    BJ_writer.WriteLine("endglobals" + System.Environment.NewLine); //空出一行
                }
                else if (BJ.substance[i].GetType() == typeof(Bj_structure.function))    //functions
                {
                    string s = ((Bj_structure.function)BJ.substance[i]).description.Trim();
                    /*
                    if (s.LastIndexOf(System.Environment.NewLine) == s.Length - 2)
                    { 
                        s = s.Substring(0, s.Length - 2);
                        BJ_writer.WriteLine();
                    }
                    else BJ_writer.WriteLine();
                    */
                    BJ_writer.WriteLine(s);
                    s = ((Bj_structure.function)BJ.substance[i]).content.Trim();
                    while (s.IndexOf("MC_MC_", StringComparison.OrdinalIgnoreCase) != -1) { s = s.Replace("mc_", "MC_").Replace("MC_MC_", "MC_"); }
                    BJ_writer.WriteLine(s + System.Environment.NewLine); //空出一行
                }
                else if (BJ.substance[i].GetType() == typeof(string))    //w3j addtions
                {
                    string s = ((string)BJ.substance[i]).Trim();
                    while (s.IndexOf("MC_MC_", StringComparison.OrdinalIgnoreCase) != -1) { s = s.Replace("mc_", "MC_").Replace("MC_MC_", "MC_"); }
                    BJ_writer.Write(s + Environment.NewLine + Environment.NewLine);
                }
            }
            BJ_writer.Close();
            button14.Enabled = true;
            BJpath = @"Output\Blizzard.j";
            MessageBox.Show("File has been saved at :" + Environment.NewLine + Environment.NewLine + "Output\\Blizzard.j", "Finish!");
            button4.PerformClick();
            ChangeLabel(0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ChangeLabel(1);
            EditItems.Clear();
            AddItems.Clear();
            for (int i = 0; i < BJ.substance.Count; i++)
            {
                //---判断函数---
                if (BJ.substance[i].GetType() == typeof(Bj_structure.function))
                {
                    bool InBJorg = false;
                    string funcname=((Bj_structure.function)BJ.substance[i]).name;
                    for (int j=0; j < BJorg.Function_org.Count; j++)
                    {
                        if (funcname == ((Bj_structure_original.function)BJorg.Function_org[j]).name)
                        {
                            InBJorg = true;
                            if (((Bj_structure.function)BJ.substance[i]).content.Trim() != ((Bj_structure_original.function)BJorg.Function_org[j]).content.Trim())
                            {
                                EditItems.Add(i);
                            }
                        }
                    }
                    if (InBJorg == false)
                    {
                        AddItems.Add(i);
                    }
                }
                //-------------
                if (BJ.substance[i].GetType() == typeof(string))
                { AddItems.Add(i); }
                else if (BJ.substance[i].GetType() == typeof(ArrayList))
                { EditItems.Add(i); }
            }
            ChangeLabel(0);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ChangeLabel(1);
            EditItems.Clear();
            AddItems.Clear();
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName.Trim()!="" && File.Exists(openFileDialog1.FileName))
            {
                BJ = new Bj_structure(openFileDialog1.FileName);
                ListBJNew();
            }
            BJpath = openFileDialog1.FileName;
            button14.Enabled = true;
            ChangeLabel(0);
        }

        private void ListBJNew()
        {
            IsNewBJ = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            if (File.Exists(@"Input\war3map.wts")) { button11.Enabled = true; } else { button11.Enabled = false; }
            if (File.Exists(@"Input\war3map.j")) { button10.Enabled = true; } else { button10.Enabled = false; }
            if (File.Exists(@"Input\war3mapMisc.txt")) { button13.Enabled = true; } else { button13.Enabled = false; }
            Fname.Items.Clear();
            Fcontent.Text = "";
            Fdescriptions.Text = "";
            for (int i = 0; i < BJ.substance.Count; i++)
            {
                if (BJ.substance[i].GetType() == typeof(Bj_structure.comment))  //Comments
                {
                    Fname.Items.Add("- Comment #" + ((Bj_structure.comment)BJ.substance[i]).index.ToString() + " -");
                }
                else if (BJ.substance[i].GetType() == typeof(ArrayList))  //Globals
                {
                    Fname.Items.Add("- GLOBALS -");
                }
                else if (BJ.substance[i].GetType() == typeof(Bj_structure.function))  //Functions
                {
                    Fname.Items.Add(((Bj_structure.function)BJ.substance[i]).name);
                }
                else if (BJ.substance[i].GetType() == typeof(string))  //Functions
                {
                    string FuncName = ((string)BJ.substance[i]);
                    FuncName = FuncName.Substring(0, FuncName.IndexOf(Environment.NewLine));
                    if (FuncName == "//<----- Sync Gamecache") { Fname.Items.Add("- SyncFuncs -"); }
                    else { Fname.Items.Add("- W3J -"); }
                }
            }
            button4.PerformClick();
        }

        private void Fname_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (EditItems.Contains(e.Index))
            {
                e.DrawBackground();
                e.Graphics.DrawString(Fname.Items[e.Index].ToString(), e.Font, Brushes.Red, e.Bounds);
                e.DrawFocusRectangle();
            }
            else if (AddItems.Contains(e.Index))
            {
                e.DrawBackground();
                e.Graphics.FillRectangle(Brushes.LightGreen, e.Bounds);
                e.Graphics.DrawString(Fname.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds);
                e.DrawFocusRectangle();
            }
            else
            {
                e.DrawBackground();
                e.DrawFocusRectangle();
                e.Graphics.DrawString(Fname.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds);
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            EditItems.Clear();
            AddItems.Clear();
            if (BJ.substance[Fname.SelectedIndex].GetType()==typeof(Bj_structure.function))
            {
            Bj_structure.function temp =((Bj_structure.function)BJ.substance[Fname.SelectedIndex]);
            temp.description = Fdescriptions.Text;
            temp.content = Fcontent.Text;
            BJ.substance[Fname.SelectedIndex] = temp;
            }
            else if (BJ.substance[Fname.SelectedIndex].GetType() == typeof(Bj_structure.comment))
            {
                Bj_structure.comment temp = ((Bj_structure.comment)BJ.substance[Fname.SelectedIndex]);
                temp.comments = Fcontent.Text;
                BJ.substance[Fname.SelectedIndex] = temp;
            }
            else if (BJ.substance[Fname.SelectedIndex].GetType() == typeof(string))
            {
                BJ.substance[Fname.SelectedIndex] = Fcontent.Text;
            }
            else if (BJ.substance[Fname.SelectedIndex].GetType() == typeof(ArrayList))
            {
                MessageBox.Show("Unsupported Item!");
                return;
            }
            MessageBox.Show("Success");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                EditItems.Clear();
                AddItems.Clear();
                Bj_structure.function item = new Bj_structure.function();
                item.name = Fcontent.Text.Substring(8, Fcontent.Text.IndexOf(" takes") - 8);
                item.description = Fdescriptions.Text;
                item.content = Fcontent.Text;
                int ti = Fname.SelectedIndex;
                BJ.substance.Insert(ti + 1, item);
                ListBJNew();
                Fname.SelectedIndex = ti + 1;
            }
            catch
            {
                MessageBox.Show("Opps, something went wrong.");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            EditItems.Clear();
            AddItems.Clear();
            Bj_structure.comment item = new Bj_structure.comment();
            item.comments = Fcontent.Text;
            item.index = BJ.commentCount;
            BJ.commentCount++;
            int ti = Fname.SelectedIndex;
            BJ.substance.Insert(ti + 1, item);
            ListBJNew();
            Fname.SelectedIndex = ti + 1;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                EditItems.Clear();
                AddItems.Clear();
                int ti = Fname.SelectedIndex;
                BJ.substance.RemoveAt(ti);
                ListBJNew();
                Fname.SelectedIndex = ti;
            }
            catch
            {
                MessageBox.Show("Opps, something went wrong.");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        public void button10_Click(object sender, EventArgs e)
        {
            ChangeLabel(1);
            EditItems.Clear();
            AddItems.Clear();
            //BJ = new Bj_structure(@"Input\Blizzard.j");
            W3J = new w3j_structure(@"Input\war3map.j");
            W3J.AddPreName("MC_");
            int InitFuncIndex = 0;

            BJ.global.Add("    // war3map.j additions");
            for (int i = 0; i < W3J.globals.Length; i++)
            {
                BJ.global.Add("    " + W3J.globals[i]);
            }

            for (int i = 0; i < BJ.substance.Count; i++)
            { 
                if (BJ.substance[i].GetType() == typeof(Bj_structure.function))
                {
                    if (((Bj_structure.function)BJ.substance[i]).name.Trim() == "MeleeStartingVisibility")
                    {
                        Bj_structure.function temp = (Bj_structure.function)BJ.substance[i];
                        temp.content = temp.content.Insert(temp.content.LastIndexOf("endfunction"), Environment.NewLine+"//MC_InitFuncs"+Environment.NewLine + W3J.CallFuncs);
                        BJ.substance[i] = temp;
                        InitFuncIndex = i;
                        break;
                    }
                }
            }

            BJ.substance.Insert(InitFuncIndex, W3J.Func_RunInitializationTriggers);
            BJ.substance.Insert(InitFuncIndex, W3J.Func_InitCustomTriggers);
            BJ.substance.Insert(InitFuncIndex, W3J.CSCandTrig);
            BJ.substance.Insert(InitFuncIndex, W3J.Func_InitGlobals);

            ListBJNew();
            ChangeLabel(0);
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            Fname.Items.Clear();
            Fdescriptions.Text = "";
            Fcontent.Text = "";
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button11.Enabled = false;
            button10.Enabled = false;
            button13.Enabled = false;
            W3J = null;
            BJ = null;
        }

        public void button11_Click(object sender, EventArgs e)
        {
            ChangeLabel(1);
            EditItems.Clear();
            AddItems.Clear();
            wts_structure wts = new wts_structure(@"Input\war3map.wts");
            /*
            MessageBox.Show(wts.TrigStr.Count.ToString());
            MessageBox.Show(((wts_structure.wtsS)wts.TrigStr[0]).S);
            MessageBox.Show(((wts_structure.wtsS)wts.TrigStr[0]).TrigS);
            */
            for (int i = 0; i < BJ.substance.Count; i++)
            {
                if (BJ.substance[i].GetType() == typeof(string))
                {
                    for (int j = 0; j < wts.TrigStr.Count; j++)
                    {
                        string s =(string)BJ.substance[i];
                        s = s.Replace(((wts_structure.wtsS)wts.TrigStr[j]).S, ((wts_structure.wtsS)wts.TrigStr[j]).TrigS);
                        BJ.substance[i] = s;
                    }
                }
            }
            ListBJNew();
            ChangeLabel(0);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ChangeLabel(1);
            EditItems.Clear();
            AddItems.Clear();
            if (!System.IO.Directory.Exists(@"Input")) { System.IO.Directory.CreateDirectory("Input"); }
            if (!System.IO.File.Exists(@"Input\Blizzard.j")) { System.IO.File.Copy(@"Original\Blizzard.j", @"Input\Blizzard.j"); }
            BJ = new Bj_structure(@"Input\Blizzard.j");
            MultiRace mc = new MultiRace();
            frm2.ShowDialog();
            if (((Form2)this.OwnedForms[0]).isjustload==false)
            {
                mc.GenerateConstants(((Form2)this.OwnedForms[0]).race);
                mc.AddConstants(ref BJ);

                mc.AddCommentToBottom(ref BJ, "Mod Craft Initialization");
                mc.ModifyStartingUnits(ref BJ, ((Form2)this.OwnedForms[0]).raceinfo_list, true);
                mc.ModifyMeleeAI(ref BJ, ((Form2)this.OwnedForms[0]).raceinfo_list, true);

                mc.MoveFunctionsToButtom(ref BJ, "MeleeStartingVisibility");
                mc.MoveFunctionsToButtom(ref BJ, "MeleeStartingUnits");
                mc.MoveFunctionsToButtom(ref BJ, "MeleeStartingUnitsForPlayer");
                mc.MoveFunctionsToButtom(ref BJ, "MeleeStartingAI");
                mc.MoveFunctionsToButtom(ref BJ, "MeleeStartingHeroLimit");

                mc.AddSyncFuncs(ref BJ, false);
            }
            ListBJNew();
            ChangeLabel(0);
        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //this.Hide();
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            ChangeLabel(1);
            misc_structure misc = new misc_structure(@"Input\war3mapMisc.txt");
            Bj_structure.function temp = (Bj_structure.function)BJ.substance[BJ.substance.Count-1];
            temp.content = "function MeleeStartingHeroLimit takes nothing returns nothing"+Environment.NewLine;
            temp.content += "    local integer index" + Environment.NewLine;
            temp.content += "" + Environment.NewLine;
            temp.content += "    set index = 0" + Environment.NewLine;
            temp.content += "    loop" + Environment.NewLine;
            temp.content += "        // max heroes per player" + Environment.NewLine;
            temp.content += "        call SetPlayerMaxHeroesAllowed(bj_MELEE_HERO_LIMIT, Player(index))" + Environment.NewLine;
            temp.content += "" + Environment.NewLine;
            temp.content += "        // each player is restricted to a limit per hero type as well" + Environment.NewLine;
            for (int i = 0; i < misc.hero.Length; i++)
            {
                temp.content += "        call ReducePlayerTechMaxAllowed(Player(index), '" + misc.hero[i] + "', bj_MELEE_HERO_TYPE_LIMIT)" + Environment.NewLine;
            }
            temp.content += "" + Environment.NewLine;
            temp.content += "        set index = index + 1" + Environment.NewLine;
            temp.content += "        exitwhen index == bj_MAX_PLAYERS" + Environment.NewLine;
            temp.content += "    endloop" + Environment.NewLine;
            temp.content += "endfunction" + Environment.NewLine;

            //MessageBox.Show(temp.content);
            BJ.substance[BJ.substance.Count - 1] = temp;
            ListBJNew();
            ChangeLabel(0);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            ChangeLabel(1);
            frm3.ShowDialog();
            ChangeLabel(0);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {
            
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.FlatAppearance.BorderSize = 0;
            Point point = new Point(btn.Left + this.Left + 5, btn.Top + this.Top + 40);
            contextMenuStrip1.Show(point);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetLanguage.SetLang("en-GB", this, this.GetType());
            SetLanguage.SetLang("en-GB", frm2, frm2.GetType(), this.toolTip1);
            SetLanguage.SetLang("en-GB", frm3, frm3.GetType(), frm2.toolTip1);
            label3.Text = "";
        }

        private void Fcontent_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetLanguage.SetLang("zh-CHS", this, this.GetType(), this.toolTip1);
            SetLanguage.SetLang("zh-CHS", frm2, frm2.GetType(), frm2.toolTip1);
            SetLanguage.SetLang("zh-CHS", frm3, frm3.GetType());
            label3.Text="";
        }

        private void button16_Click_1(object sender, EventArgs e)
        {
            openFileDialog2.FileName = "";
            ChangeLabel(1);
            openFileDialog2.ShowDialog();
            if (openFileDialog2.FileName.Trim() != "" && File.Exists(openFileDialog2.FileName))
            {
                //MessageBox.Show(openFileDialog2.FileName);
                try
                {
                    //if (File.Exists(@"Input\Blizzard.j")) { File.Delete(@"Input\Blizzard.j"); }
                    if (File.Exists(@"Input\war3map.j")) { File.Delete(@"Input\war3map.j"); }
                    if (File.Exists(@"Input\war3map.wts")) { File.Delete(@"Input\war3map.wts"); }
                    if (File.Exists(@"Input\war3mapMisc.txt")) { File.Delete(@"Input\war3mapMisc.txt"); }

                    MpqLib.Mpq.CArchive archive = new MpqLib.Mpq.CArchive(openFileDialog2.FileName);
                    if (!System.IO.Directory.Exists("Input")) { System.IO.Directory.CreateDirectory("Input"); }

                    if (archive.FileExists("war3map.j")) { archive.ExportFile("war3map.j", @"Input\war3map.j"); }
                    if (archive.FileExists("war3map.wts")) { archive.ExportFile("war3map.wts", @"Input\war3map.wts"); }
                    if (archive.FileExists("war3mapMisc.txt")) { archive.ExportFile("war3mapMisc.txt", @"Input\war3mapMisc.txt"); }
                    archive.Close();
                    MessageBox.Show("Files have been extracted at: Input\\");
                }
                catch
                {
                    MessageBox.Show("Opps, Something wrong happened during the extracting process.");
                }
            }
            ChangeLabel(0);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button17_Click(object sender, EventArgs e)
        {

        }

        private void button17_Click_1(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("PackMPQ.exe");
            }
            catch
            {
                MessageBox.Show("Can not execute 'PackMPQ.exe', file may be missing..","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        
    }
}
