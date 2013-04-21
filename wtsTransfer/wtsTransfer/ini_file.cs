using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace wtsTransfer
{
    class ini_file
    {
        string filePath;    //这是新生成用于读取的文件的位置。
        string realPath;
        bool flag;

        #region 调用windowsAPI  
       //用来读取INI 文件的内容  
       [DllImport("Kernel32.dll")]
        private static extern int GetPrivateProfileString(string strAppName, string strKeyName, string strDefault, byte[] buffer, int nSize, string strFileName);   
       //返回所读取的字符串值的真实长度  
       [DllImport("Kernel32.dll")]  
       private extern static int GetPrivateProfileStringA(string strAppName, string strKeyName, string sDefault, byte[] buffer, int nSize, string strFileName);  
       [DllImport("Kernel32.dll")]  
       private static extern int GetPrivateProfileInt(string strAppName, string strKeyName, int nDefault, string strFileName);  
       //获取ini文件所有的section  
       [DllImport("Kernel32.dll")]  
       private extern static int GetPrivateProfileSectionNamesA(byte[] buffer, int iLen, string fileName);  
       //获取指定Section的key和value          
       [System.Runtime.InteropServices.DllImport("Kernel32.dll")]  
       private static extern int GetPrivateProfileSection(string lpAppName,byte[] lpReturnedString, int nSize,string lpFileName);   
       //根据传入参数的不同进行写入或修改或删除操作（返回值 Long，非零表示成功，零表示失败）  
       [DllImport("Kernel32.dll")]  
       public static extern long WritePrivateProfileString(string strAppName, string strKeyName, string strKeyValue, string strFileName);  
       //添加一个section内容列表  
       [DllImport("Kernel32.dll")]  
       public static extern long WritePrivateProfileSection(string strAppName, string strkeyandvalue, string strFileName);  
       #endregion  

 
  
#region 供UI调用的方法   

       public ini_file(string inipath,bool selfmodify=false)
       {
           if (selfmodify)
           {
               realPath = inipath;
               //重新生成规范化文件，否则缺少文件头的文档一个section会无法读取。
               File.Delete(@"temp");
               filePath = Application.StartupPath + @"\temp";
               //MessageBox.Show(filePath);
               StreamReader sr = new StreamReader(inipath);
               StreamWriter sw = new StreamWriter(filePath);
               sw.Write(sr.ReadToEnd());
               sw.Close();
               sr.Close();
               File.Delete(@"temp");
           }
           else
           {
               realPath = inipath;
               filePath = inipath;
               StreamReader sr = new StreamReader(inipath);
               string ss = sr.ReadToEnd();
               sr.Close();
               StreamWriter sw = new StreamWriter(filePath);
               sw.Write(ss);
               sw.Close();
           }
       }
   
        /// <summary>  
        /// 判断该ini文件是否存在如果不存在新建一个该文件  
        /// </summary>  
        public void FileExists()  
        {  
            try 
            {  
                if (!File.Exists(this.filePath))  
                {  
                    using (FileStream fs = File.Create(this.filePath))  
                    {  
                        fs.Close();  
                    }  
                }  
             }  
            catch { }
        }

        /// <summary>  
        ///  打开文件的指定section修改多个含有特定字符串的key的值，非windows api，
        ///  一般用在一个section下有两个以上相同key的时候，此时用win api无效。(Indp=Independence)
        /// </summary>  
        /// <param name="sectionName"></param>  
        /// <returns></returns>  
        public bool WriteIniKeys_Indp(string section, string key, string value, string matchingstring)
        { 
            bool flag=false;
            StreamReader fsr = new StreamReader(filePath);
            string file = "";   //新文件样式
            string line = "";
            while ((line = fsr.ReadLine()) != null)
            {
                file = file + line + Environment.NewLine;
                if (line.Trim() == "[" + section + "]")
                {
                    line = fsr.ReadLine();
                    while (line.Trim() != "" && line.Trim()[0] != '[' && line != null)
                    {
                        if (line.Trim().IndexOf(key) == 0)
                        {
                            if (line.Substring(line.IndexOf("=") + 1).IndexOf(matchingstring) != -1)
                            {
                                line = key + "=" + value;
                                flag = true;
                            }
                        }
                        file = file + line + Environment.NewLine;
                        line = fsr.ReadLine();
                    }
                    file = file + line + Environment.NewLine;
                }
            }
            fsr.Close();
            StreamWriter fsw = new StreamWriter(filePath,false);
            fsw.Write(file);
            fsw.Close();


            //MessageBox.Show(flag.ToString()+" | "+filePath);
            return flag;
        }
   
        /// <summary>  
        /// 返回该配置文件中所有Section名称的集合  
        /// </summary>  
        /// <returns></returns>  
        public ArrayList ReadSections()  
        {  
            byte[] buffer = new byte[999999];  
            int rel = GetPrivateProfileSectionNamesA(buffer, buffer.GetUpperBound(0), this.filePath);  
            //int rel = GetPrivateProfileString(null, null, "", buffer, buffer.GetUpperBound(0), this.filePath);
            int iCnt, iPos;  
            ArrayList arrayList = new ArrayList();  
            string tmp;  
            if (rel > 0)  
            {  
                iCnt = 0; iPos = 0;
                for (iCnt = 0; iCnt < rel; iCnt++)  
                {
                    if (buffer[iCnt] == 0x00)  
                    {
                        tmp = System.Text.Encoding.UTF8.GetString(buffer, iPos, iCnt - iPos).Trim();  
                        iPos = iCnt + 1;  
                        if (tmp != "")  
                            arrayList.Add(tmp);  
                    }  
                }  
            }  
            return arrayList;  
        }  
           
        /// <summary>  
        ///  获取指定节点的所有KEY的名称  
        /// </summary>  
        /// <param name="sectionName"></param>  
        /// <returns></returns>  
        public ArrayList ReadKeys(string sectionName)  
        {  
   
            byte[] buffer = new byte[5120];  
            int rel = GetPrivateProfileStringA(sectionName, null, "", buffer, buffer.GetUpperBound(0), this.filePath);  
   
            int iCnt, iPos;  
            ArrayList arrayList = new ArrayList();  
            string tmp;  
            if (rel > 0)  
            {  
                iCnt = 0; iPos = 0;  
                for (iCnt = 0; iCnt < rel; iCnt++)  
                {  
                    if (buffer[iCnt] == 0x00)  
                    {
                        tmp = System.Text.Encoding.UTF8.GetString(buffer, iPos, iCnt - iPos).Trim();  
                        iPos = iCnt + 1;  
                        if (tmp != "")  
                            arrayList.Add(tmp);  
                    }  
                }  
            }  
            return arrayList;  
        }  
   
        /// <summary>  
        /// 读取指定节点下的指定key的value返回string  
        /// </summary>  
        /// <param name="section"></param>  
        /// <param name="key"></param>  
        /// <returns></returns>  
        public string GetIniKeyValueForStr(string section, string key)  
        {  
            if (section.Trim().Length <= 0 || key.Trim().Length <= 0) return string.Empty;  
            //StringBuilder strTemp = new StringBuilder(256);
            byte[] buffer = new byte[65535];
            GetPrivateProfileStringA(section, key, string.Empty, buffer, 256, this.filePath);
            string tmp = System.Text.Encoding.UTF8.GetString(buffer).Trim('\x0000'); //去除空字串
            return tmp.Trim();
            //return strTemp.ToString().Trim();  
        }  
   
        /// <summary>  
        /// 从指定的节点中获取一个整数值( Long，找到的key的值；如指定的key未找到，就返回默认值。如找到的数字不是一个合法的整数，函数会返回其中合法的一部分。如，对于“xyz=55zz”这个条目，函数返回55。)  
        /// </summary>  
        /// <param name="section"></param>  
        /// <param name="key"></param>  
        /// <returns></returns>  
        public int GetIniKeyValueForInt(string section, string key)  
        {  
            if (section.Trim().Length <= 0 || key.Trim().Length <= 0) return 0;  
            return GetPrivateProfileInt(section, key, 0, this.filePath);  
        }  
   
        /// <summary>  
        /// 读取指定节点下的所有key 和value  
        /// </summary>  
        /// <param name="section"></param>  
        /// <returns></returns>  
        public ArrayList GetIniSectionValue(string section)  
        {
            byte[] buffer = new byte[99999];  
            int rel = GetPrivateProfileSection(section, buffer, buffer.GetUpperBound(0), this.filePath);  
   
            int iCnt, iPos;  
            ArrayList arrayList = new ArrayList();  
            string tmp;  
            if (rel > 0)  
            {  
                iCnt = 0; iPos = 0;  
                for (iCnt = 0; iCnt < rel; iCnt++)  
                {  
                    if (buffer[iCnt] == 0x00)  
                    {
                        tmp = System.Text.Encoding.UTF8.GetString(buffer, iPos, iCnt - iPos).Trim();  
                        iPos = iCnt + 1;  
                        if (tmp != "")  
                            arrayList.Add(tmp);  
                    }  
                }  
            }  
            return arrayList;  
        }  
   
        /// <summary>  
        /// 往指定section的key中写入value  
        /// </summary>  
        /// <param name="section"></param>  
        /// <param name="key"></param>  
        /// <param name="value"></param>  
        /// <returns></returns>  
        public bool WriteIniKey(string section, string key, string value)  
        {  
            try 
            {  
                if (section.Trim().Length <= 0 || key.Trim().Length <= 0 || value.Trim().Length <= 0)  
                {  
                    flag = false;  
                }  
                else 
                {  
                     
                    if (WritePrivateProfileString(section, key, value, this.filePath) == 0)  
                    {  
                        flag = false;  
                    }  
                    else 
                    {  
                        flag = true;  
                    }  
                }  
            }  
            catch 
            {  
                flag = false;  
            }  
            return flag;  
        }  
   
        /// <summary>  
        /// 修改指定section的key的值  
        /// </summary>  
        /// <param name="section"></param>  
        /// <param name="key"></param>  
        /// <param name="value"></param>  
        /// <returns></returns>  
        public bool EditIniKey(string section, string key, string value)  
        {  
            try 
            {  
                if (section.Trim().Length <= 0 || key.Trim().Length <= 0 || value.Trim().Length <= 0)  
                {  
                    flag = false;  
                }  
                else 
                {  
                    if (WritePrivateProfileString(section, key, value, this.filePath) == 0)  
                    {  
                        flag = false;  
                    }  
                    else 
                    {  
                        flag = true;  
                    }  
                }  
            }  
            catch 
            {  
                flag = false;  
            }  
            return flag;  
        }  
   
        /// <summary>  
        /// 删除指定section的指定key  
        /// </summary>  
        /// <param name="section"></param>  
        /// <param name="key"></param>  
        /// <returns></returns>  
        public bool DeleteIniKey(string section, string key)  
        {  
            try 
            {  
                if (section.Trim().Length <= 0 || key.Trim().Length <= 0)  
                {  
                    flag = false;  
                }  
                else 
                {  
                    if (WritePrivateProfileString(section, key, null, this.filePath) == 0)  
                    {  
                        flag = false;  
                    }  
                    else 
                    {  
                        flag = true;  
                    }  
                }  
            }  
            catch 
            {  
                flag = false;  
            }  
            return flag;  
        }  
   
        /// <summary>  
        /// 删除指定section  
        /// </summary>  
        /// <param name="section"></param>  
        /// <returns></returns>  
        public bool DeleteIniSection(string section)  
        {  
            try 
            {  
                if (section.Trim().Length <= 0)  
                {  
                    flag = false;  
                }  
                else 
                {  
                    if (WritePrivateProfileString(section, null, null, this.filePath) == 0)  
                    {  
                        flag = false;  
                    }  
                    else 
                    {  
                        flag = true;  
                    }  
                }  
            }  
            catch 
            {  
                flag = false;  
            }  
            return flag;  
        }  
   
        /// <summary>  
        /// 给一个节点写入key和value列表  
        /// </summary>  
        /// <param name="section"></param>  
        /// <param name="ht"></param>  
        /// <returns></returns>  
        public bool WriteIniSectionAndValue(string section, Hashtable ht)  
        {  
            string lpString = "";  
            try 
            {  
                if (section.Trim().Length <= 0 || ht.Count == 0)  
                {  
                    flag = false;  
                }  
                else 
                {  
                    foreach (DictionaryEntry de in ht)  
                    {  
                        lpString += de.Key+"="+de.Value;                        
                        lpString += "\r\n";  
                    }  
                    if (WritePrivateProfileSection(section, lpString, this.filePath) == 0)  
                    {  
                        flag = false;  
                    }  
                    else 
                    {  
                        flag = true;  
                    }  
   
                }  
            }  
            catch 
            {  
                flag = false;  
            }  
            return flag;  
        }  
          
        /// <summary>  
        /// 给一个节点写入key 列表  
        /// </summary>  
        /// <param name="section"></param>  
        /// <param name="lstKeyValue"></param>  
        /// <returns></returns>  
        public bool WriteIniSectionName(string section, List<string> lstKeyValue)  
        {  
            string lpString = "";  
            try 
            {  
                if (section.Trim().Length <= 0 || lstKeyValue.Count == 0)  
                {  
                    flag = false;  
                }  
                else 
                {  
                    for (int i = 0; i < lstKeyValue.Count; ++i)  
                    {  
                        lpString += lstKeyValue[i];  
                        lpString += "\r\n";  
                    }  
                    if (WritePrivateProfileSection(section, lpString, this.filePath) == 0)  
                    {  
                        flag = false;  
                    }  
                    else 
                    {  
                        flag = true;  
                    }  
   
                }  
            }  
            catch 
            {  
                flag = false;  
            }  
            return flag;  
        }  
        #endregion
    }
}
