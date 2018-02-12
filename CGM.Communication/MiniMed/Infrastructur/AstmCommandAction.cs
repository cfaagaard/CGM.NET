namespace CGM.Communication.MiniMed.Infrastructur
{
    public enum AstmCommandAction : byte
    {
        NO_TYPE = 0x0,
        INITIALIZE = 0x01,
        SCAN_NETWORK = 0x02,
        JOIN_NETWORK = 0x03,
        LEAVE_NETWORK = 0x04,
        TRANSMIT_PACKET = 0x05,
        READ_DATA = 0x06,
        READ_STATUS = 0x07,
        READ_NETWORK_STATUS = 0x08,
        SET_SECURITY_MODE = 0x0c,
        READ_STATISTICS = 0x0d,
        SET_RF_MODE = 0x0e,
        CLEAR_STATUS = 0x10,
        SET_LINK_KEY = 0x14,
        COMMAND_RESPONSE = 0x55
        //NO_TYPE = 0x0,
        //CHANNEL_NEGOTIATE = 0x03,
        //PUMP_REQUEST = 0x05,
        //PUMP_RESPONSE = 0x55,
        //BAYERINFO= 0x31,
        //OPEN_CONNECTION=0xa4


    }


}
