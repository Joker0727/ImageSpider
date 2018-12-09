using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageSpider
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            #region 合并dll到程序中
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;//合并dll到程序中
            #endregion

            if (CheckRepeatStart())
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        /// <summary>
        /// 合并dll到程序中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string resourceName = "ImageSpider." + new AssemblyName(args.Name).Name + ".dll";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
        /// <summary>
        /// 判断程序是否重复打开
        /// </summary>
        /// <returns></returns>
        public static bool CheckRepeatStart()
        {
            #region 防止程序重复打开
            int processCount = 0;
            Process[] pa = Process.GetProcesses();//获取当前进程数组。
            foreach (Process PTest in pa)
            {
                if (PTest.ProcessName == Process.GetCurrentProcess().ProcessName)
                    processCount += 1;
            }
            if (processCount > 1)
            {
                //如果程序已经运行，则给出提示。并退出本进程。
                DialogResult dr;
                dr = MessageBox.Show(Process.GetCurrentProcess().ProcessName + "程序已经在运行！", "退出程序", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //可能你不需要弹出窗口，在这里可以屏蔽掉
                return true; //Exit;
            }
            #endregion
            return false;
        }
    }
}
