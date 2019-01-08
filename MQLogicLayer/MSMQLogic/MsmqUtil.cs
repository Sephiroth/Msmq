using System;
using System.Messaging;

namespace MQLogicLayer.MSMQLogic
{
    public class MsmqUtil
    {
        public MessageQueue MSMQ;

        private string queueName;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorInfo { get; private set; }

        /// <summary>
        /// MsmqUtil构造器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="isCreate">true:删除重建;false:存在则返回，不存在则创建</param>
        public MsmqUtil(string queueName, bool isCreate = false)
        {
            this.queueName = queueName;
            this.InitMq(isCreate);
        }

        /// <summary>
        /// 初始化msmq对象
        /// </summary>
        /// <param name="isCreate">是否重新创建这个队列</param>
        public void InitMq(bool isCreate)
        {
            MessageQueue[] mqList = MessageQueue.GetPrivateQueuesByMachine(System.Environment.MachineName);
            if (mqList.Length > 0)
            {
                foreach (var mq in mqList)
                {
                    if (mq.FormatName.Contains(queueName))
                    {
                        if (isCreate)
                        {
                            MessageQueue.Delete(queueName);
                            MSMQ = MessageQueue.Create(queueName, false);
                        }
                        else { MSMQ = new MessageQueue(queueName); }
                        return;
                    }
                }
                MSMQ = MessageQueue.Create(queueName, false);
            }
            else { MSMQ = MessageQueue.Create(queueName, false); }
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="frameID"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Send(object obj)
        {
            bool rs = false;
            try
            {
                if (obj != null)
                {
                    MSMQ.Send(obj);
                    rs = true;
                    ErrorInfo = null;
                }
            }
            catch (Exception e) { ErrorInfo = e.Message; }
            return rs;
        }

        public object Receive()
        {
            object rs = null;
            Message msg = MSMQ.Receive();
            if (msg != null)
            {
                rs = msg.Body;
            }
            return rs;
        }
    }
}
