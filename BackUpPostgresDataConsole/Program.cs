using Newtonsoft.Json;
using ReadIni;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BackUpPostgresDataConsole
{
    /*
     * Desc:定时执行批处理
     * Auth:dsy
     * Date:2022-4-6 10:27:02
     */

    class Program
    {
        static void Main(string[] args)
        {
            var ini = new IniFile(System.Environment.CurrentDirectory + "\\" + "config.ini");
            string targetDir = AppDomain.CurrentDomain.BaseDirectory;

            //线程后台运行
            Task.Run(() =>
            {
                //线程无限循环运行
                while (true)
                {
                    //当前时间
                    DateTime time = DateTime.Now;

                    //时分
                    int hour = time.Hour;
                    int minite = time.Minute;

                    //当晚23点10分 运行备份命令
                    if(hour == 23 && minite == 10)
                    {
                        //读取配置文件 批处理的内容
                        string batStr = ini.IniReadValue("Cmd", "batlist");
                        //批处理列表
                        List<string> batList = JsonConvert.DeserializeObject<List<string>>(batStr);

                        //执行命令 备份数据库
                        for (int i = 0; i < batList.Count; i++)
                        {
                            //遍历执行bat文件命令
                            ExcuteProc(targetDir, batList[i]);
                        }
                    }

                    //60秒
                    Thread.Sleep(1000 * 60);
                }
            });

            //停止，使程序不要退出
            Console.ReadLine();
        }

        /// <summary>
        /// 执行批处理命令
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="fileName"></param>
        public static void ExcuteProc(string targetDir,string fileName)
        {
            Process proc = null;
            try
            {
                proc = new Process();
                proc.StartInfo.WorkingDirectory = targetDir;
                proc.StartInfo.FileName = fileName;
                //proc.StartInfo.Arguments = string.Format("10");//this is argument
                //proc.StartInfo.CreateNoWindow = true;
                //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;//这里设置DOS窗口不显示，经实践可行
                proc.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }
        }
    }
}
