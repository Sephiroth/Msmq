using System;
using System.Collections.Generic;
using System.Threading;

namespace CANLibrary
{
    public class CANAPI2
    {

        public static uint m_bOpen = 0;
        public static string m_error = "";

        private static uint m_devtype = (uint)DEV_TYPE.VCI_CANETTCP; //VCI_CANETTCP
        private static uint m_devind = 1;
        private static uint m_canind = 1;

        private static VCI_CAN_OBJ[] m_recobj;

        //public static event Action<uint, byte[]> CANReceiveHandler = delegate { };

        /// <summary>
        /// 连接并启动CANET
        /// </summary>
        /// <param name="can_ip"></param>
        /// <param name="can_port"></param>
        public static void CanConnect(string can_ip, uint can_port)
        {
            try
            {
                if (m_bOpen == 1)
                {
                    return;
                    //NativeMethod.VCI_CloseDevice(m_devtype, m_devind);
                    //m_bOpen = 0;
                }
                //else
                //{
                uint srcport = 0;

                if (NativeMethod.VCI_OpenDevice(m_devtype, m_devind, srcport) == 0)
                {
                    throw new Exception("打开设备失败，设备类型或设备索引号不正确");
                }


                uint cmdtype = (uint)CMD.CMD_DESIP;
                if ((uint)STATUS.STATUS_OK != NativeMethod.VCI_SetReference(m_devtype, m_devind, m_canind, cmdtype, can_ip))
                {
                    throw new Exception("设置目标IP出错");
                }

                cmdtype = (uint)CMD.CMD_DESPORT;
                if ((uint)STATUS.STATUS_OK != NativeMethod.VCI_SetReference(m_devtype, m_devind, m_canind, cmdtype, out can_port))
                {
                    throw new Exception("设置目标端口出错");
                }

                cmdtype = (uint)CMD.CMD_TCP_TYPE;
                uint tcptype = (uint)TCPTYPE.TCP_CLIENT;
                if ((uint)STATUS.STATUS_OK != NativeMethod.VCI_SetReference(m_devtype, m_devind, m_canind, cmdtype, out tcptype))
                {
                    throw new Exception("设置TCP工作方式出错");
                }

                if (1 != NativeMethod.VCI_StartCAN(m_devtype, m_devind, m_canind))
                {
                    throw new Exception("CAN启动失败");
                }

                m_bOpen = 1;

                //开启定时器，接收下位机数据
                //Thread t = new Thread(CanReceiveThread);
                //t.IsBackground = true;
                //t.Start();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 断开CAN连接
        /// </summary>
        public static void CanDisConnect()
        {
            try
            {
                NativeMethod.VCI_CloseDevice(m_devtype, m_devind);
                m_bOpen = 0;
            }
            catch (Exception ex)
            {
                m_error += " DisConnect Error: " + ex.Message;
            }
        }

        unsafe public static void CanSend(uint canID, byte[] data)
        {
            try
            {
                VCI_CAN_OBJ[] sendobj = new VCI_CAN_OBJ[1];//sendobj.Init();
                sendobj[0].SendType = (byte)0; // 正常发送
                sendobj[0].RemoteFlag = (byte)0; // 0-数据帧，1-远程帧
                sendobj[0].ExternFlag = (byte)1; // 0-表针帧，1-扩展帧
                sendobj[0].ID = canID;
                sendobj[0].DataLen = (byte)data.Length;

                for (int i = 0; i < data.Length; i++)
                {
                    fixed (VCI_CAN_OBJ* sendobjs = &sendobj[0])
                        sendobjs[0].Data[i] = data[i];
                }

                uint res = NativeMethod.VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj[0], (uint)1);
                if (res == 0)
                {
                    throw new Exception("发送失败");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CanReceiveThread()
        {
            try
            {
                m_recobj = new VCI_CAN_OBJ[50];
                while (m_bOpen == 1)
                {
                    Thread.Sleep(100);
                    CanReceive();
                }
            }
            catch (Exception ex)
            {
                m_error += " ReceiveThread Error: " + ex.Message;
            }
        }

        unsafe public static void CanReceive()
        {
            try
            {
                UInt32 res = new UInt32();
                res = NativeMethod.VCI_GetReceiveNum(m_devtype, m_devind, m_canind);
                if (res < 1)
                    return;

                fixed (VCI_CAN_OBJ* pobj = &m_recobj[0])
                {
                    res = NativeMethod.VCI_Receive(m_devtype, m_devind, m_canind, pobj, 50, 100);
                }

                for (UInt32 i = 0; i < res; i++)
                {
                    uint frameID = m_recobj[i].ID;
                    byte len = (byte)(m_recobj[i].DataLen % 9);
                    byte[] dataBytes = new byte[len];
                    fixed (byte* bdata = m_recobj[i].Data)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            dataBytes[j] = bdata[j];
                        }
                    }
                    if (len > 0)
                    {
                        //if (CANReceiveHandler != null)
                        //{
                        //    CANReceiveHandler(frameID, dataBytes);
                        //    Thread.Sleep(100);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
