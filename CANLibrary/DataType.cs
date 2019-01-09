using System;
using System.Runtime.InteropServices;

namespace CANLibrary
{
    public enum DEV_TYPE
    {
        VCI_PCI5121 = 1,
        VCI_PCI9810 = 2,
        VCI_USBCAN1 = 3,
        VCI_USBCAN2 = 4,
        VCI_USBCAN2A = 4,
        VCI_PCI9820 = 5,
        VCI_CAN232 = 6,
        VCI_PCI5110 = 7,
        VCI_CANLITE = 8,
        VCI_ISA9620 = 9,
        VCI_ISA5420 = 10,
        VCI_PC104CAN = 11,
        VCI_CANETE = 12,
        VCI_DNP9810 = 13,
        VCI_PCI9840 = 14,
        VCI_PC104CAN2 = 15,
        VCI_PCI9820I = 16,
        VCI_PEC9920 = 18,
        VCI_CANETTCP = 17,
        VCI_PCI5010U = 19,
        VCI_USBCANEU = 20,
        VCI_USBCAN2EU = 21,
    }

    // CAN错误码
    public enum CAN_ERR
    {
        ERR_CAN_OVERFLOW = 0x0001, //CAN控制器内部FIFO溢出
        ERR_CAN_ERRALARM = 0x0002, //CAN控制器错误报警
        ERR_CAN_PASSIVE = 0x0004, //CAN控制器消极错误
        ERR_CAN_LOSE = 0x0008, //CAN控制器仲裁丢失
        ERR_CAN_BUSERR = 0x0010, //CAN控制器总线错误
        ERR_CAN_BUSCLOSE = 0x0020, //CAN控制器总线关闭
    }

    // 通用错误码
    public enum DEV_ERR
    {
        ERR_DEVICEOPENED = 0x0100, //设备已经打开
        ERR_DEVICEOPEN = 0x0200, //打开设备错误
        ERR_DEVICENOTOPEN = 0x0400, //设备没有打开
        ERR_BUFFEROVERFLOW = 0x0800, //缓冲区溢出
        ERR_DEVICENOTEXIST = 0x1000, //此设备不存在
        ERR_LOADKERNELDLL = 0x2000, //装载动态库失败
        ERR_CMDFAILED = 0x4000, //执行命令失败错误码
        ERR_BUFFERCREATE = 0x8000, //内存不足
    }

    // 函数调用返回状态值
    public enum STATUS
    {
        STATUS_OK = 1,
        STATUS_ERR = 0,
    }
    
    // 函数调用返回状态值
    public enum CMD
    {
        CMD_DESIP = 0,
        CMD_DESPORT = 1,
        CMD_SRCPORT = 2,
        CMD_CHGDESIPANDPORT = 2,
        CMD_TCP_TYPE = 4,//tcp 工作方式，服务器:1 或是客户端:0
    }

    public enum TCPTYPE
    {
        TCP_CLIENT = 0,
        TCP_SERVER = 1,
    }

    public enum REF
    {
        REFERENCE_BAUD = 1,
        REFERENCE_SET_TRANSMIT_TIMEOUT = 2,
        REFERENCE_ADD_FILTER = 3,
        REFERENCE_SET_FILTER = 4,
    };

    public struct VCI_BOARD_INFO
    {
        public UInt16 hw_Version;
        public UInt16 fw_Version;
        public UInt16 dr_Version;
        public UInt16 in_Version;
        public UInt16 irq_Num;
        public byte can_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] str_Serial_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] str_hw_Type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved;
    }

    //2.定义CAN信息帧的数据类型。
    //public struct VCI_CAN_OBJ
    //{
    //    public UInt32 ID;
    //    public UInt32 TimeStamp;
    //    public byte TimeFlag;
    //    public byte SendType;
    //    public byte RemoteFlag;//是否是远程帧
    //    public byte ExternFlag;//是否是扩展帧
    //    public byte DataLen;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    //    public byte[] Data;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    //    public byte[] Reserved;


    //    public void Init()
    //    {
    //        Data = new byte[8];
    //        Reserved = new byte[3];
    //    }
    //}
    //2.定义CAN信息帧的数据类型。
    unsafe public struct VCI_CAN_OBJ  //使用不安全代码
    {
        public uint ID;
        public uint TimeStamp;
        public byte TimeFlag;
        public byte SendType;
        public byte RemoteFlag;//是否是远程帧
        public byte ExternFlag;//是否是扩展帧
        public byte DataLen;

        public fixed byte Data[8];

        public fixed byte Reserved[3];


        public void Init()
        {
            //Data = new byte[8];
            //Reserved = new byte[3];
        }
    }

    //3.定义CAN控制器状态的数据类型。
    public struct VCI_CAN_STATUS
    {
        public byte ErrInterrupt;
        public byte regMode;
        public byte regStatus;
        public byte regALCapture;
        public byte regECCapture;
        public byte regEWLimit;
        public byte regRECounter;
        public byte regTECounter;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved;
    }

    //4.定义错误信息的数据类型。
    public struct VCI_ERR_INFO
    {
        public uint ErrCode;
        public byte Passive_ErrData1;
        public byte Passive_ErrData2;
        public byte Passive_ErrData3;
        public byte ArLost_ErrData;
    }

    //5.定义初始化CAN的数据类型
    public struct VCI_INIT_CONFIG
    {
        public uint AccCode;
        public uint AccMask;
        public uint Reserved;
        public byte Filter;
        public byte Timing0;
        public byte Timing1;
        public byte Mode;
    }

    public struct CHGDESIPANDPORT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] szpwd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] szdesip;
        public Int32 desport;

        public void Init()
        {
            szpwd = new byte[10];
            szdesip = new byte[20];
        }
    }
}
