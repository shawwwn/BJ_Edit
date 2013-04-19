using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace BJ_Edit
{
    class Bj_structure_original
    {
        public ArrayList Global_org = new ArrayList();
        public int GlobalCount_org = 0;

        public struct function
        {
            public string name;
            public string content;
        }
        public ArrayList Function_org = new ArrayList();
        public int FunctionCount_org = 0;

        public Bj_structure_original(string bj_orgPath)
        {
            string line;
            StreamReader bj_org = new StreamReader(bj_orgPath);
            while ((line = bj_org.ReadLine()) != null)
            {

                //===========读取声明===========
                if (line.Trim() == "globals")
                {
                    int i = 0;
                    while ((line = bj_org.ReadLine()) != "endglobals")
                    {
                        string temps = line.Trim();
                        if (temps.Length != 0 && temps.IndexOf("//") != 0)
                        {
                            this.Global_org.Add(line);
                            i++;
                        }
                    }
                    this.GlobalCount_org = i;
                }
                //===========读取函数===========
                if (line.IndexOf("function") == 0)
                {
                    this.FunctionCount_org++;

                    function f = new function();
                    f.name = line.Substring(8, line.IndexOf(" takes") - 8);
                    f.content = f.content + (line + System.Environment.NewLine);
                    while ((line = bj_org.ReadLine()) != "endfunction")
                    {
                        f.content = f.content + (line + System.Environment.NewLine);
                    }
                    f.content = f.content + (line + System.Environment.NewLine);

                    this.Function_org.Add(f);
                }

            }
            bj_org.Close();
        }


    }
}
