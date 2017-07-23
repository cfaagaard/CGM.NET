namespace CGM.Communication.MiniMed.Infrastructur
{
    public enum AstmCommandAction : byte
    {
        NO_TYPE = 0x0,
        CHANNEL_NEGOTIATE = 0x03,
        PUMP_REQUEST = 0x05,
        PUMP_RESPONSE = 0x55,
        BAYERINFO= 0x31,
        OPEN_CONNECTION=0xa4
    }


}
