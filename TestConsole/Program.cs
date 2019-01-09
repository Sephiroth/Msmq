using CANLibrary;
using log4net;
using MQLogicLayer.MSMQLogic;
using MQLogicLayer.Util;
using System;
using System.Configuration;
using System.Messaging;
using System.Reflection;
using System.Threading;

namespace TestConsole
{
    class Program
    {
        const string SEND_NAME = "can_send_queue";
        const string RCV_NAME = "can_rcv_queue";

        public MsmqUtil sendMQ;

        public MsmqUtil rcvMQ;

        static void Main(string[] args)
        {
            try
            {   // WM-WinServer
                Program p = new Program();
                p.InitMQ();
                //MessageQueue MSMQ = new MessageQueue(@"FormatName:DIRECT=TCP:192.168.0.82\Private$\can_send_queue");
                //MSMQ.ReceiveCompleted += sendMqReceiveCompleted;
                //MSMQ.BeginReceive();
                p.sendMQ.MSMQ.Send(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 0x22 });
                //p.ConnectCAN();
                Console.Read();

            }
            catch (Exception exp)
            {
            }
        }

        static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private void InitMQ()
        {
            string can_send_queue = ConfigurationManager.AppSettings["can_send_queue"];
            string can_rcv_queue = ConfigurationManager.AppSettings["can_rcv_queue"];
            sendMQ = new MsmqUtil(string.Format(@"FormatName:DIRECT=TCP:{0}\Private$\{1}", can_send_queue, SEND_NAME), true);
            rcvMQ = new MsmqUtil(string.Format(@"FormatName:DIRECT=TCP:{0}\Private$\{1}", can_rcv_queue, RCV_NAME), true);
            sendMQ.MSMQ.ReceiveCompleted += sendMqReceiveCompleted;
            sendMQ.MSMQ.BeginReceive();
        }

        public void ConnectCAN()
        {
            try
            {
                string ip = ConfigurationManager.AppSettings["canIP"];
                string port = ConfigurationManager.AppSettings["canPort"];
                CANAPI.CanConnect(ip, Convert.ToUInt32(port));
            }
            catch (Exception exp)
            {
                Logger.Error("启动CAN失败:", exp);
            }
            CANAPI.CANReceiveHandler += MsgHandler;
        }

        /// <summary>
        /// CAN接收的报文写到can_rcv_queue
        /// </summary>
        /// <param name="frameID"></param>
        /// <param name="data"></param>
        private void MsgHandler(uint frameID, byte[] data)
        {
            if (rcvMQ.Send(DataConvert.HandleData(Convert.ToByte(frameID), data)) == false)
            {
                Logger.Error("发送到can_rcv_queue失败:", new Exception(rcvMQ.ErrorInfo));
            }
        }

        /// <summary>
        /// 监听sendMQ接收到的报文响应
        /// </summary>
        /// <param name="source"></param>
        /// <param name="asyncResult"></param>
        private static void sendMqReceiveCompleted(object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = null;
            try
            {
                mq = source as MessageQueue;
                Message msg = mq.EndReceive(asyncResult.AsyncResult);
                if (msg.Body != null)
                {
                    Tuple<byte, byte[]> rs = DataConvert.DataToCan(msg.Body as byte[]);
                    Logger.Error("", new Exception(string.Format("CanInfo:帧ID:{0};内容:{1}", rs.Item1, BytesUtil.Bytes2HexString(rs.Item2))));
                    CANAPI.CanSend(rs.Item1, rs.Item2);
                }
            }
            catch (Exception exp)
            {
                Logger.Error("sendMqReceiveCompleted", exp);
            }
            try
            {
                if (mq != null)
                {
                    mq.BeginReceive();
                }
            }
            catch (Exception exp)
            {
                Logger.Error("sendMqReceiveCompleted", exp);
            }
            return;
        }
    }
}