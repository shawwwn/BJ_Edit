using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading; /// 线程空间

namespace BJ_Edit
{
    /// <summary>
    /// 语言库 class
    /// </summary>
    public class SetLanguage
    {
        /// <summary>
        /// 设置当前程序的界面语言
        /// </summary>
        /// <param name="lang">语言 </param>
        /// <param name="form">窗体</param>
        /// <param name="frmtype">窗体类型</param>
        public static void SetLang(string lang, Form form, Type frmtype, ToolTip tp = null)
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
            if (form != null)
            {
                ComponentResourceManager resources = new ComponentResourceManager(frmtype);
                resources.ApplyResources(form, "$this");
                AppLang(form, resources, tp);
            }
        }
        #region AppLang for Control
        /// <summary>
        /// 遍历窗体所有控件，针对其设置当前界面语言
        /// </summary>
        /// <param name="contrl"></param>
        /// <param name="resoureces"></param>
        private static void AppLang(Control control, ComponentResourceManager resources, ToolTip tp = null)
        {
            if (control is MenuStrip)
            {
                //将资源应用与对应的属性
                resources.ApplyResources(control, control.Name);
                MenuStrip ms = (MenuStrip)control;
                if (ms.Items.Count > 0)
                {
                    foreach (ToolStripMenuItem c in ms.Items)
                    {
                        //调用 遍历菜单 设置语言
                        AppLang(c, resources);
                    }
                }
            }

            foreach (Control c in control.Controls)
            {
                resources.ApplyResources(c, c.Name);
                if (tp != null)
                {
                    //MessageBox.Show(c.Name);
                    tp.SetToolTip(c, resources.GetString(c.Name + ".ToolTip"));
                }
                AppLang(c, resources);
            }
        }
        #endregion

        #region AppLang for menuitem
        /// <summary>
        /// 遍历菜单
        /// </summary>
        /// <param name="item"></param>
        /// <param name="resources"></param>
        private static void AppLang(ToolStripMenuItem item, System.ComponentModel.ComponentResourceManager resources)
        {
            if (item is ToolStripMenuItem)
            {
                resources.ApplyResources(item, item.Name);
                ToolStripMenuItem tsmi = (ToolStripMenuItem)item;
                if (tsmi.DropDownItems.Count > 0)
                {
                    foreach (ToolStripMenuItem c in tsmi.DropDownItems)
                    {
                        //if (tsmi != ToolStripSeparator)
                        //{ }
                        AppLang(c, resources);
                    }
                }
            }
        }
        #endregion
    }
}