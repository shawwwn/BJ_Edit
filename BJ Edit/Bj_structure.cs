using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace BJ_Edit
{
    class Bj_structure
    {
        public ArrayList substance = new ArrayList();
        public ArrayList global = new ArrayList();
        public int commentCount = 0;

        public struct function
        {
            public string description;
            public string name;
            public string content;
        }

        public struct comment
        {
            public int index;
            public string comments;
        }

        public bool IsAComment(string sLine)
        {
            string s = sLine.Trim();
            if (s.IndexOf("//") == 0) return true;
            return false;
        }

        public Bj_structure(string bjPath)
        {
            commentCount = 0;
            bool IsFunc = false;
            StreamReader bj = new StreamReader(bjPath);

            //读取全部内容放在一个个位置中
            string[] tempbj=bj.ReadToEnd().Replace("\r\n","\n").Split('\n');
            bj.Close();

            for (int i = 0; i < tempbj.Length; i++)
            {
                tempbj[i]=tempbj[i].Trim();

                // 是global
                if (tempbj[i].IndexOf("globals") == 0)
                {

                    i++;
                    string s = tempbj[i].Trim();
                    while (s.IndexOf("endglobals") != 0)
                    {
                        this.global.Add(tempbj[i]);
                        i++;
                        s = tempbj[i].Trim();
                    }
                    this.substance.Add(this.global);
                }

                // 是以//开头的的情况
                if (IsAComment(tempbj[i]) == true)
                {
                    IsFunc = false;  //还原flag
                    comment c = new comment();
                    c.comments = c.comments + (tempbj[i]+System.Environment.NewLine);

                    while (tempbj[i].Length != 0)
                    {
                        i++;
                        tempbj[i]=tempbj[i].Trim();
                        if (tempbj[i].IndexOf("function") == 0)
                        {
                            IsFunc = true;
                            function f = new function();
                            f.description = c.comments;
                            f.name = tempbj[i].Substring(8, tempbj[i].IndexOf(" takes") - 8);
                            f.content = f.content + (tempbj[i].TrimEnd() + System.Environment.NewLine);
                            i++;
                            string s=tempbj[i].Trim();
                            while (s != "endfunction")
                            {
                                f.content = f.content + (tempbj[i] + System.Environment.NewLine);
                                i++;
                                s = tempbj[i].TrimEnd();
                            }
                            f.content = f.content + (tempbj[i].TrimEnd() + System.Environment.NewLine);
                            this.substance.Add(f);
                            break;
                        }
                        c.comments = c.comments + (tempbj[i] + System.Environment.NewLine);
                    }
                    if (!IsFunc)
                    {
                        c.index = commentCount;
                        this.substance.Add(c);
                        commentCount++;
                    }
                }

                // 是函数
                if (tempbj[i].IndexOf("function")==0)
                {
                    function f = new function();
                    f.description = "";
                    f.name = tempbj[i].Substring(8, tempbj[i].IndexOf(" takes") - 8);
                    f.content=f.content+(tempbj[i]+System.Environment.NewLine);
                    i++;
                    string s=tempbj[i].Trim();
                    while (s.IndexOf("endfunction")!=0)
                    {
                        f.content = f.content + (tempbj[i] + System.Environment.NewLine);
                        i++;
                        s = tempbj[i].Trim();
                    }
                    f.content = f.content + (tempbj[i] + System.Environment.NewLine);
                    this.substance.Add(f);
                }
            }
        }
        
    }
}
