using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace mq_can_service
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceController[] serviceArr = ServiceController.GetServices();
            bool exit = false;
            foreach (var serv in serviceArr)
            {
                if (serv.ServiceName.Contains("CAN_MQ_Service"))
                {
                    exit = true;
                    break;
                }
            }
            if (exit) // 服务已存在则退出
            {
                return;
            }

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new CAN_MQ_Service()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
