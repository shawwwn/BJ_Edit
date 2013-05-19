using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace BJ_Edit
{
    class w3j_structure
    {
        public string[] globals;
        public string Func_InitGlobals="";
        public string CSCandTrig = "";
        public string Func_InitCustomTriggers = "";
        public string Func_RunInitializationTriggers = "";
        public string CallFuncs="";

        private string GetFuncName(string line)
        {
            if (line.Trim().IndexOf("function") == 0)
            {
                return (line.Substring(8, line.IndexOf(" takes") - 8)).Trim();
            }
            else
                return "";
        }

        private string GetVebName(string VebS)
        {
            if (VebS.IndexOf(" array ") == -1)
            {
                int len = VebS.IndexOf("=");
                if (len == -1) { len = VebS.Length - 1 - VebS.IndexOf(" "); } else { len = len - VebS.IndexOf(" "); }
                return (VebS.Substring(VebS.IndexOf(" "), len)).Trim();
            }
            else
            {
                return (VebS.Substring(VebS.IndexOf(" array ") + 7, VebS.Length - VebS.IndexOf(" array ") - 7));
            }
        }

        public w3j_structure(string w3jPath)
        {
            string line = "";
            StreamReader w3j = new StreamReader(w3jPath);
            string WEonly_Funcs="";
            string WEonly_Trigs="";
            
            while ((line = w3j.ReadLine()) != null)
            {
                // 读global 
                if (line.Trim() == "globals")
                {
                    string gtemp = "";
                    while ((line = w3j.ReadLine()) != "endglobals")
                    {
                        string temps = line.Trim();
                        if (temps.Length != 0 && temps.IndexOf("//") != 0)
                        {
                            gtemp += (temps+"\n");
                        }
                    }
                    gtemp=gtemp.TrimEnd();
                    this.globals = gtemp.Split('\n');
                }

                // 读全局变量赋值函数InitGlobals()
                if (GetFuncName(line) == "InitGlobals")
                {
                    this.Func_InitGlobals += (line+ Environment.NewLine);
                    while ((line = w3j.ReadLine()).Trim() != "endfunction")
                    {
                        this.Func_InitGlobals += (line + Environment.NewLine);
                    }
                    this.Func_InitGlobals += (line + Environment.NewLine);
                    //----------使指针移到到InitCustomTriggers处----------
                    while (true)
                    {
                        line=w3j.ReadLine();
                        if (line.IndexOf("//*  Triggers") == 0 || line.IndexOf("//*  Custom Script Code") == 0)
                        {
                            this.CSCandTrig+="//***************************************************************************"+Environment.NewLine+"//*"+Environment.NewLine;
                            this.CSCandTrig+=line+Environment.NewLine;
                            break;
                        }

                    }
                    //----------------------------------------------------

                    //读取下面的Custom Script Code & Triggers
                    while (GetFuncName(line = w3j.ReadLine())!="InitCustomTriggers")
                    {
                        //寻找标准丢弃trigger标志。
                        if (line.Trim().IndexOf(@"// Trigger:") == 0)
                        {
                            string trgFuncName=line.Trim().Substring(12);
                            if (trgFuncName.IndexOf("WEonly_") == 0)
                            {
                                //将函数名加入WEonly_Funcs列表
                                WEonly_Funcs = WEonly_Funcs + trgFuncName + Environment.NewLine;
                                //丢弃前面一行注释
                                CSCandTrig = CSCandTrig.Substring(0, CSCandTrig.LastIndexOf(Environment.NewLine, CSCandTrig.Length - 2)) + Environment.NewLine;
                                //丢弃函数
                                while (GetFuncName(line = w3j.ReadLine()) != "InitTrig_" + trgFuncName) { }
                                line = w3j.ReadLine().Trim();
                                //将触发变量名加入WEonly_Trigs列表
                                int len = 0;
                                if (line.LastIndexOf("=") != -1)
                                { len = line.LastIndexOf("=") - line.IndexOf(" ") - 1; }
                                else { len = line.Length - line.IndexOf(" "); }
                                WEonly_Trigs = WEonly_Trigs + line.Substring(line.IndexOf(" "), len) + Environment.NewLine;
                                while ((line = w3j.ReadLine()).Trim() != "endfunction") { }
                                line = w3j.ReadLine();
                            }
                        }
                        //正常读取
                        if (line.Trim().IndexOf(@"//TESH") != 0)
                            this.CSCandTrig += (line.TrimEnd() + Environment.NewLine);
                        else
                            this.CSCandTrig += Environment.NewLine;
                    }
                    //this.CSCandTrig = this.CSCandTrig.Trim();
                    CSCandTrig=CSCandTrig.Substring(0, CSCandTrig.LastIndexOf("endfunction")+11)+Environment.NewLine;
                }

                // 读InitCustomTriggers()
                if (GetFuncName(line) == "InitCustomTriggers")
                {
                    this.Func_InitCustomTriggers += (line + Environment.NewLine);
                    while ((line = w3j.ReadLine()).Trim() != "endfunction")
                    {
                        /*
                        //剔除在WEonly_Funcs里面的句子(bugged)
                        if (WEonly_Funcs.Trim() != "")
                        {
                            bool IsWEonly = false;
                            string FuncName = line.Trim().Substring(5, line.Trim().Length - 7);
                            string[] WEonly_Funcs_list = WEonly_Funcs.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < WEonly_Funcs_list.Length; i++)
                            { if ("InitTrig_" + WEonly_Funcs_list[i] == FuncName) { IsWEonly = true; } }
                            if (IsWEonly == false) { this.Func_InitCustomTriggers += (line + Environment.NewLine); }
                        }
                        */
                        //this.Func_InitCustomTriggers += (line + Environment.NewLine);
                    }
                    this.Func_InitCustomTriggers += (line + Environment.NewLine);
                }

                // 读RunInitializationTriggers()
                if (GetFuncName(line) == "RunInitializationTriggers")
                {
                    this.Func_RunInitializationTriggers += (line + Environment.NewLine);
                    while ((line = w3j.ReadLine()).Trim() != "endfunction")
                    {
                        //剔除在WEonly_Trigs里面的句子
                        if (WEonly_Trigs.Trim() != "")
                        {
                            bool IsWEonly = false;
                            string TrigName = line.Trim().Substring("call ConditionalTriggerExecute(".Length, line.Trim().Length - 1 - "call ConditionalTriggerExecute(".Length);
                            string[] WEonly_Trigs_list = WEonly_Trigs.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < WEonly_Trigs_list.Length; i++)
                            { if (WEonly_Trigs_list[i] == TrigName) { IsWEonly = true; } }
                            if (IsWEonly == false) { this.Func_RunInitializationTriggers += (line + Environment.NewLine); }
                        }
                    }
                    this.Func_RunInitializationTriggers += (line + Environment.NewLine);
                    break;
                }
            }
            //global区段里除去WEonly_Trigs里面的东西
            if (WEonly_Trigs.Trim() != "")
            {
                string newGlobals = "";
                string[] WEonly_Trigs_list = WEonly_Trigs.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < this.globals.Length; i++)
                {
                    bool IsWEonly=false;
                    string temp=globals[i].Trim();
                    if (temp.IndexOf("trigger ") == 0) 
                    {
                        temp = temp.Substring(8, temp.LastIndexOf("=") - 8);
                        for (int j = 0; j < WEonly_Trigs_list.Length; j++)
                        {
                            if (temp == WEonly_Trigs_list[j])
                            {
                                //MessageBox.Show(temp);    //Debug
                                IsWEonly = true;
                            }
                        }
                    }
                    if (IsWEonly == false) { newGlobals = newGlobals + globals[i] + Environment.NewLine; }
                }
                this.globals = newGlobals.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
            //主线程函数构成
            if (this.Func_InitGlobals != "")
            { this.CallFuncs += "call InitGlobals()" + Environment.NewLine; }
            if (this.Func_InitCustomTriggers != "")
            { this.CallFuncs += "call InitCustomTriggers()" + Environment.NewLine; }
            if (this.Func_RunInitializationTriggers != "")
            { this.CallFuncs += "call RunInitializationTriggers()" + Environment.NewLine; }
            w3j.Close();
        }

        public void AddPreName(string prename)
        {
            //重命名变量
            for (int i=0;i<this.globals.Length;i++)
            {
                string veb=GetVebName(this.globals[i]);
                this.globals[i]=this.globals[i].Replace(veb, prename + veb);
                //MessageBox.Show(this.globals[i]);
                this.CSCandTrig=this.CSCandTrig.Replace(veb, prename + veb);
                this.Func_InitCustomTriggers=this.Func_InitCustomTriggers.Replace(veb, prename + veb);
                this.Func_InitGlobals = this.Func_InitGlobals.Replace(veb, prename + veb);
                this.Func_RunInitializationTriggers=this.Func_RunInitializationTriggers.Replace(veb, prename + veb);
            }
            //重命名所有函数
            string[] tempnames = this.CSCandTrig.Split(Environment.NewLine.ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tempnames.Length; i++)
            {
                string funcname = GetFuncName(tempnames[i]);
                if (funcname != "")
                {
                    this.CSCandTrig=this.CSCandTrig.Replace(funcname, prename.ToUpper() + funcname);
                    this.Func_InitCustomTriggers = this.Func_InitCustomTriggers.Replace(funcname, prename.ToUpper() + funcname);
                }
            }
            //重命名InitCustomTriggers 
            this.Func_InitCustomTriggers=this.Func_InitCustomTriggers.Replace("InitCustomTriggers", prename.ToUpper() + "InitCustomTriggers");
            this.CallFuncs = this.CallFuncs.Replace("InitCustomTriggers", prename.ToUpper() + "InitCustomTriggers");

            //重命名RunInitializationTriggers 
            this.Func_RunInitializationTriggers = this.Func_RunInitializationTriggers.Replace("RunInitializationTriggers", prename.ToUpper() + "RunInitializationTriggers");
            this.CallFuncs = this.CallFuncs.Replace("RunInitializationTriggers", prename.ToUpper() + "RunInitializationTriggers");

            //重命名InitGlobals 
            this.Func_InitGlobals = this.Func_InitGlobals.Replace("InitGlobals", prename.ToUpper() + "InitGlobals");
            this.CallFuncs = this.CallFuncs.Replace("InitGlobals", prename.ToUpper() + "InitGlobals");
        }


    }
}
