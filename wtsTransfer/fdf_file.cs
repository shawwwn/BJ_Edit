using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace wtsTransfer
{
    class fdf_file
    {
        public string head = "";
        public string feet = "}"+Environment.NewLine;
        public ArrayList body = new ArrayList();

        public fdf_file(string fdfpath)
        {
            StreamReader sr = new StreamReader(fdfpath);
            string line = "";
            while ((line = sr.ReadLine()).Trim() != "StringList {")
            {
                if (line == null) { break; }
                head = head + line + Environment.NewLine;
            }
            head = head + line + Environment.NewLine;

            while ((line = sr.ReadLine()).Trim() != "}")
            {
                if (line == null) { break; }
                body.Add(line);
            }
            sr.Close();
            
        }
    }
}
