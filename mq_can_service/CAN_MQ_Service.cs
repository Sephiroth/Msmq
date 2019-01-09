using CANLibrary;
using log4net;
using MQLogicLayer.MSMQLogic;
using MQLogicLayer.Util;
using System;
using System.Configuration;
using System.Messaging;
using System.Reflection;
using System.ServiceProcess;

namespace mq_can_service
{
    public partial class CAN_MQ_Service : ServiceBase
    {
        /// <summary>
        /// 发送队列名称
        /// </summary>
        const string SEND_NAME = "can_send_queue";
        /// <summary>
        /// 接收队列名称
        /// </summary>
        const string RCV_NAME = "can_rcv_queue";

        static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public const string NAME = "CAN_MQ_Service";

        /// <summary>
        /// 消息给can的队列
        /// </summary>
        public MsmqUtil sendMQ;

        /// <summary>
        /// 存入can接收消息的队列
        /// </summary>
        public MsmqUtil rcvMQ;

        public CAN_MQ_Service()
        {
            InitializeComponent();
            ServiceName = NAME;
            AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            InitMQ();
            ConnectCAN();
        }

        protected override void OnStop()
        {
        }

        private void InitMQ()
        {
            //string can_send_queue = ConfigurationManager.AppSettings["can_send_queue"];
            //string can_rcv_queue = ConfigurationManager.AppSettings["can_rcv_queue"];
            sendMQ = new MsmqUtil(string.Format(@".\Private$\{0}", SEND_NAME), false);
            rcvMQ = new MsmqUtil(string.Format(@".\Private$\{0}", RCV_NAME), false);
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
        private void sendMqReceiveCompleted(object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = null;
            try
            {
                mq = source as MessageQueue;
                Message msg = mq.EndReceive(asyncResult.AsyncResult);
                if (msg.Body != null)
                {
                    Tuple<byte, byte[]> rs = DataConvert.DataToCan(msg.Body as byte[]);
                    Logger.Info(string.Format("CanInfo:帧ID:{0};内容:{1}", rs.Item1, BytesUtil.Bytes2HexString(rs.Item2)));
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
