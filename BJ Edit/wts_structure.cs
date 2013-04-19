using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;


namespace BJ_Edit
{
    class wts_structure
    {
        public struct wtsS
        {
            public string TrigS;
            public string S;
        }

        public ArrayList TrigStr=new ArrayList();
        public int count=0;

        public wts_structure(string wtsPath)
        {
            string line = "";
            StreamReader wts = new StreamReader(wtsPath);

            while ((line = wts.ReadLine()) != null)
            {
                if (line.IndexOf("STRING ") != -1)
                {
                    wtsS w= new wtsS();
                    w.S=line.Substring(7).Trim();
                    w.S = "TRIGSTR_" + w.S.PadLeft(3, '0');

                    if (wts.ReadLine().IndexOf("//") == 0)  //下移一行
                        continue;

                    while ((line = wts.ReadLine()).Trim() != "}")
                    {
                        w.TrigS += line + Environment.NewLine;
                    }
                    w.TrigS = w.TrigS.Trim();
                    TrigStr.Add(w);
                }
            }

            wts.Close();
        }
    }
}
