using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;


namespace BJ_Edit
{
    class mc
    {
        public string[] RaceName;

        public void GenerateConstants(string[] racename)
        {
            this.RaceName = racename;
            for (int i = 0; i < this.RaceName; i++)
            {

            }

        }
        public void AddRace(string[] racename)
        {
            this.RaceName = racename;

        }
    }
}
