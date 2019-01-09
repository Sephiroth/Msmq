using System;
using System.Runtime.InteropServices;

namespace CANLibrary
{
    internal class NativeMethod
    {
        [DllImport("controlcan.dll")]
        public static extern uint VCI_OpenDevice(uint DeviceType, uint DeviceInd, uint Reserved);

        [DllImport("controlcan.dll")]
        public static extern uint VCI_CloseDevice(uint DeviceType, uint DeviceInd);

        [DllImport("controlcan.dll")]
        public static extern uint VCI_InitCAN(uint DeviceType, uint DeviceInd, uint CANInd, ref VCI_INIT_CONFIG pInitConfig);

        [DllImport("controlcan.dll")]
        public static extern uint VCI_ReadBoardInfo(uint DeviceType, uint DeviceInd, ref VCI_BOARD_INFO pInfo);

        [DllImport("controlcan.dll")]
        public static extern uint VCI_ReadErrInfo(uint DeviceType, uint DeviceInd, uint CANInd, ref VCI_ERR_INFO pErrInfo);

        [DllImport("controlcan.dll")]
        public static extern uint VCI_ReadCANStatus(uint DeviceType, uint DeviceInd, uint CANInd, ref VCI_CAN_STATUS pCANStatus);

        [DllImport("controlcan.dll")]
        public static extern uint VCI_GetReference(uint DeviceType, uint DeviceInd, uint CANInd, uint RefType, ref byte pData);
        [DllImport("controlcan.dll")]
        public static extern uint VCI_SetReference(uint DeviceType, uint DeviceInd, uint CANInd, uint RefType, ref byte pData);
        [DllImport("ControlCAN")]
        public static extern Int32 VCI_SetReference(uint DevType, uint DevIndex, uint CANIndex, uint RefType, [MarshalAs(UnmanagedType.LPStr)]string pData);


        [DllImport("ControlCAN")]
        public static extern Int32 VCI_SetReference(uint DevType, uint DevIndex, uint CANIndex, uint RefType, out uint pData);

        [DllImport("controlcan.dll")]
        public static extern uint VCI_GetReceiveNum(uint DeviceType, uint DeviceInd, uint CANInd);
        [DllImport("controlcan.dll")]
        public static extern uint VCI_ClearBuffer(uint DeviceType, uint DeviceInd, uint CANInd);

        [DllImport("controlcan.dll")]
        public static extern uint VCI_StartCAN(uint DeviceType, uint DeviceInd, uint CANInd);
        [DllImport("controlcan.dll")]
        public static extern uint VCI_ResetCAN(uint DeviceType, uint DeviceInd, uint CANInd);

        [DllImport("controlcan.dll")]
        public static extern uint VCI_Transmit(uint DeviceType, uint DeviceInd, uint CANInd, ref VCI_CAN_OBJ pSend, uint Len);
        //[DllImport("controlcan.dll")]
        //static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);
        [DllImport("controlcan.dll")]
        public unsafe static extern uint VCI_Receive(uint DeviceType, uint DeviceInd, uint CANInd, VCI_CAN_OBJ* pReceive, uint Len, Int32 WaitTime);

    }
}
