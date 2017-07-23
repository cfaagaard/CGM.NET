namespace CGM.Communication.MiniMed.Infrastructur
{
    public enum AstmCommandType : byte
    {
        OPEN_CONNECTION = 0x10,
        CLOSE_CONNECTION = 0x11,
        SEND_MESSAGE = 0x12,
        READ_INFO = 0x14,
        REQUEST_LINK_KEY = 0x16,
        SEND_LINK_KEY = 0x17,
        RECEIVE_MESSAGE = 0x80,
        SEND_MESSAGE_Response = 0x81,
        REQUEST_LINK_KEY_Response = 0x86
    }


}
