using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace BJ_Edit
{
    class MultiRace
    {
        public class raceinfo
        {
            public string raceName;
            public string baseID;
            public string peonID;
            public int peonCount;
            public string aiPath;
        }

        public string[] RaceName;
        public string[] RaceNameConstants;

        private int HasFunc(ref Bj_structure inputbj, string functionname)
        {
            for (int i = 0; i < inputbj.substance.Count; i++)
            {
                if (inputbj.substance[i].GetType() == typeof(Bj_structure.function) && ((Bj_structure.function)inputbj.substance[i]).name.Trim() == functionname)
                {
                    return i; 
                }
            }
            return -1;
        }

        private void EditFunc(ref Bj_structure inputbj, string functionname, string content, string comment = "")
        {
            for (int i = 0; i < inputbj.substance.Count; i++)
            {
                if (inputbj.substance[i].GetType() == typeof(Bj_structure.function) && ((Bj_structure.function)inputbj.substance[i]).name.Trim() == functionname)
                {
                    Bj_structure.function temp = (Bj_structure.function)inputbj.substance[i];
                    temp.content = content;
                    if (comment != "") { temp.description = comment; }
                    inputbj.substance[i] = temp;
                }
            }
        }

        public void GenerateConstants(string[] racen)
        {
            this.RaceName = (string[])racen.Clone();
            this.RaceNameConstants = (string[])racen.Clone();
            for (int i = 0; i < this.RaceNameConstants.Length; i++)
            {
                RaceNameConstants[i] = "    constant integer   MC_RACE_" + RaceNameConstants[i].ToUpper();
                RaceNameConstants[i] += ("= "+(i+1).ToString()).PadLeft(50 - RaceNameConstants[i].Length, ' ');
            }
        }

        public void AddConstants(ref Bj_structure inputbj)
        {
            string s = "";
            s += "    //***************************************************************************"+Environment.NewLine;
            s += "    //*" + Environment.NewLine;
            s += "    //*  Globals of ModCraft" + Environment.NewLine;
            s += "    //*" + Environment.NewLine;
            s += "    //***************************************************************************" + Environment.NewLine + Environment.NewLine;
            s += "    // race constant" + Environment.NewLine;
            for (int i = 0; i < this.RaceNameConstants.Length; i++)
            {
                s += RaceNameConstants[i] + Environment.NewLine;
            }

            for (int i = 0; i < inputbj.substance.Count; i++)
            {
                if (inputbj.substance[i].GetType() == typeof(ArrayList))
                {
                    ((ArrayList)inputbj.substance[i]).Add(Environment.NewLine);
                    ((ArrayList)inputbj.substance[i]).Add(s);
                    break;
                }
            }
        }

        public void MoveFunctionsToButtom(ref Bj_structure inputbj, string functionname)
        {
            int count = inputbj.substance.Count;
            //MessageBox.Show(count.ToString());
            for (int i = 0; i < count; i++)
            {
                if (inputbj.substance[i].GetType() == typeof(Bj_structure.function) && ((Bj_structure.function)inputbj.substance[i]).name.Trim() == functionname)
                {
                    Bj_structure.function temp=((Bj_structure.function)inputbj.substance[i]);
                    temp.description += "// MC_EDITED";
                    inputbj.substance.RemoveAt(i);
                    inputbj.substance.Add(temp);
                    i--;
                    count--;
                }
            }
        }

        public void ClearFunctions(ref Bj_structure inputbj, string functionname, bool IncludeComments = true)
        {
            int count = inputbj.substance.Count;
            //MessageBox.Show(count.ToString());
            for (int i = 0; i < count; i++)
            {
                if (inputbj.substance[i].GetType() == typeof(Bj_structure.function) && ((Bj_structure.function)inputbj.substance[i]).name.Trim() == functionname)
                {
                    Bj_structure.function temp = ((Bj_structure.function)inputbj.substance[i]);
                    if (IncludeComments) temp.description = "";
                    temp.content = "";
                    inputbj.substance[i] = temp;
                }
            }
        }

        public void AddCommentToBottom(ref Bj_structure inputbj, string comment)
        {
            Bj_structure.comment cs=new Bj_structure.comment();
            string s="";
            s += "//***************************************************************************" + Environment.NewLine;
            s += "//*" + Environment.NewLine;
            s += "//*  " + comment + Environment.NewLine;
            s += "//*" + Environment.NewLine;
            s += "//***************************************************************************" + Environment.NewLine;
            s += Environment.NewLine;
            cs.comments=s;
            cs.index = inputbj.commentCount;
            inputbj.commentCount++;
            inputbj.substance.Add(cs);
        }

        private string GenerateContentForRaceFunc(string funcname, raceinfo ri)
        {
            string s = Template.startingunittemplate;
            s = s.Replace("[FuncName]", funcname);
            string tmp=ri.baseID;
            if (tmp == "" || tmp.Length > 4) { tmp = "ogre"; }
            s = s.Replace("[baseID]", tmp);
            tmp = ri.peonID;
            if (tmp == "" || tmp.Length > 4) { tmp = "opeo"; }
            s = s.Replace("[peonID]", tmp);
            return s;
        }

        private string GenerateConmentForRaceFunc(raceinfo ri)
        {
            string s = Template.startingunitcomment;
            string tmp = ri.baseID;
            if (tmp == "" || tmp.Length > 4) { tmp = "ogre"; }
            s = s.Replace("[baseID]", tmp);
            tmp = ri.peonID;
            if (tmp == "" || tmp.Length > 4) { tmp = "opeo"; }
            s = s.Replace("[peonID]", tmp);
            s = s.Replace("[RaceName]", ri.raceName);
            return s;
        }

        public void ModifyStartingUnits(ref Bj_structure inputbj, ArrayList raceinfolist, bool debug=false)
        {
            this.ClearFunctions(ref inputbj,"MeleeStartingUnits",false);
            string s = Template.head + Environment.NewLine;
            s += "                    set indexRace = GetRandomInt(1," + RaceNameConstants.Length.ToString() + ")" + Environment.NewLine;
            s += "                " + Template.body + Environment.NewLine;

            
            for (int i = 0; i < this.RaceNameConstants.Length; i++)
            {
                if (i > 0) s += "            elseif (indexRace == " + "MC_RACE_" + this.RaceName[i].ToUpper() + ") then" + Environment.NewLine; 
                else s += "            if (indexRace == " + "MC_RACE_" + this.RaceName[i].ToUpper() + ") then" + Environment.NewLine;
                //这里修正/创建自定义函数；
                string RaceStartFuncName = "MeleeStartingUnits" + this.RaceName[i];
                if (HasFunc(ref inputbj, RaceStartFuncName) != -1) 
                {
                    //有没有数据，有就清空
                    MoveFunctionsToButtom(ref inputbj, RaceStartFuncName);
                    //取数据，看是否有改动数据
                    for (int j = 0; j < raceinfolist.Count; j++)
                    {
                        if (((raceinfo)raceinfolist[j]).raceName.ToLower() == RaceName[i].ToLower())
                        {
                            ClearFunctions(ref inputbj, RaceStartFuncName);
                            //修改函数
                            EditFunc(ref inputbj, RaceStartFuncName, GenerateContentForRaceFunc(RaceStartFuncName, (raceinfo)raceinfolist[j]), GenerateConmentForRaceFunc((raceinfo)raceinfolist[j]));
                            break;
                        }
                    }
                }
                else 
                {
                    //创建函数
                    Bj_structure.function ff = new Bj_structure.function();
                    ff.name = " "+RaceStartFuncName;
                    for (int j = 0; j < raceinfolist.Count; j++)
                    {
                        if (((raceinfo)raceinfolist[j]).raceName.ToLower() == RaceName[i].ToLower())
                        {
                            ff.content = GenerateContentForRaceFunc(RaceStartFuncName, (raceinfo)raceinfolist[j]);
                            ff.description = GenerateConmentForRaceFunc((raceinfo)raceinfolist[j]);
                            //添加
                            inputbj.substance.Add(ff);
                            break;
                        }
                    }
                }
                s += "                call " + RaceStartFuncName + "(indexPlayer, indexStartLoc, true, true, true)" + Environment.NewLine;
            }

            s += "            " + Template.feet + Environment.NewLine;
            //修改BJ
            EditFunc(ref inputbj, "MeleeStartingUnits", s);
        }

        public void ModifyMeleeAI(ref Bj_structure inputbj,ArrayList raceinfolist, bool debug = false)
        {
            this.ClearFunctions(ref inputbj, "MeleeStartingAI", false);
            string s = Template.head2 + Environment.NewLine + "                ";
            bool HasData = false;
            string aip = "";
            for (int i = 0; i < this.RaceName.Length; i++)
            {
                string add = Template.body2.Replace("[RACE]", this.RaceName[i].ToUpper());
                HasData = false;
                aip = "";
                //取数据，看是否原先有
                for (int j = 0; j < raceinfolist.Count; j++)
                {
                    if (((raceinfo)raceinfolist[j]).raceName.ToLower() == RaceName[i].ToLower() && (((raceinfo)raceinfolist[j]).aiPath != "" || ((raceinfo)raceinfolist[j]).aiPath != ".ai"))
                    {
                        aip= ((raceinfo)raceinfolist[j]).aiPath;
                        if (aip.Length >= 3) { aip = aip.Substring(0, aip.Length - 3); }
                        HasData = true;
                        break;
                    }
                }
                if (HasData && aip != "")
                {
                    add = add.Replace("[race]", aip);
                }
                else
                {
                    if (RaceName[i].ToLower() == "nightelf") { add = add.Replace("[race]", "elf"); }
                    else { add = add.Replace("[race]", this.RaceName[i].ToLower()); }   //特殊
                    if (RaceName[i].ToLower() == "undead")
                    {
                        //食尸鬼巡逻位置
                        add = add.Substring(0, add.LastIndexOf(Environment.NewLine)) + Environment.NewLine + "                    call RecycleGuardPosition(bj_ghoul[index])" + add.Substring(add.LastIndexOf(Environment.NewLine), add.Length - add.LastIndexOf(Environment.NewLine));
                    }
                }
                s += add;
            }
            s += Environment.NewLine + "                    ";
            s += Template.feet2;
            //MessageBox.Show(s);   //debug用

            //修改BJ
            EditFunc(ref inputbj, "MeleeStartingAI", s);
        }

        public void AddSyncFuncs(ref Bj_structure inputbj, bool debug = false)
        {
            //加入call sync函数
            int InitFuncIndex = 0;
            for (int i = 0; i < inputbj.substance.Count; i++)
            {
                if (inputbj.substance[i].GetType() == typeof(Bj_structure.function))
                {
                    if (((Bj_structure.function)inputbj.substance[i]).name.Trim() == "MeleeStartingVisibility")
                    {
                        Bj_structure.function temp = (Bj_structure.function)inputbj.substance[i];
                        temp.content = temp.content.Insert(temp.content.LastIndexOf("endfunction"), Environment.NewLine + "//MC_InitRaceIndexGC_CallFuncs" + Environment.NewLine + Template.rigc_callfuncs + Environment.NewLine);
                        inputbj.substance[i] = temp;
                        InitFuncIndex = i;
                        break;
                    }
                }
            }
            inputbj.substance.Insert(InitFuncIndex, Template.sync_body);
            inputbj.global.Add("    //Sync Gamecache");
            inputbj.global.Add("    " + "gamecache MC_udg_raceindexgc= null");
            inputbj.global.Add("    " + "integer array      MC_raceindex" + Environment.NewLine);
        }

    }
}
