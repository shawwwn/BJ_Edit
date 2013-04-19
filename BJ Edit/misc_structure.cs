using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace BJ_Edit
{
    class misc_structure
    {
        public string[] hero;
        public misc_structure(string MiscPath)
        {
            string line = "";
            StreamReader misc = new StreamReader(MiscPath);

            while ((line = misc.ReadLine()) != null)
            {
                if (line.Trim() == "[HERO]")
                {
                    line = misc.ReadLine().Trim();
                    string s = line.Substring(line.IndexOf("DependencyOr=")+13);
                    hero = s.Split(',');
                    break;
                }
            }
            misc.Close();
        }
    }
}
