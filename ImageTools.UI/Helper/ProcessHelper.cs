using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ImageTools.UI.Helper
{
    public class ProcessHelper
    {
        public static bool CloseProcess(string ProcName, bool isReverse = false)
        {
            bool result = false;
            System.Collections.ArrayList procList = new System.Collections.ArrayList();
            string tempName = "";
            var ss = Process.GetProcesses();
            if (isReverse) ss = ss.Reverse().ToArray();
            foreach (Process thisProc in ss)
            {
                tempName = thisProc.ProcessName;
                procList.Add(tempName);
                if (tempName == ProcName)
                {
                    if (!thisProc.CloseMainWindow())
                        thisProc.Kill(); //当发送关闭窗口命令无效时强行结束进程     
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// 强制杀死进程
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static bool KillProcess(params string[] processNameArr)
        {
            bool isAllKill = false;
            foreach (var item in processNameArr)
            {
                try
                {
                    isAllKill = false;
                    Process[] p2 = Process.GetProcessesByName(item);
                    foreach (var process in p2)
                    {
                        if (process.MainWindowTitle != "信息")
                        {
                            process.Kill();
                            isAllKill = true;
                        }
                    }

                    //p[0].Kill();
                    //MessageBox.Show("进程关闭成功！");
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("无法关闭此进程！");
                }
            }
            if (isAllKill == true) return true;
            return false;
        }
    }
}
