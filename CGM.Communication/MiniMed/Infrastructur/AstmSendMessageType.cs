using System;

namespace CGM.Communication.MiniMed.Infrastructur
{
    public enum AstmSendMessageType
    {
        HIGH_SPEED_MODE_COMMAND = 0x0412,
        TIME_REQUEST = 0x0403,
        TIME_RESPONSE = 0x0407,
        READ_PUMP_STATUS_REQUEST = 0x0112,
        READ_PUMP_STATUS_RESPONSE = 0x013C,
        READ_BASAL_PATTERN_REQUEST = 0x0116,
        READ_BASAL_PATTERN_RESPONSE = 0x0123,
        READ_BOLUS_WIZARD_CARB_RATIOS_REQUEST = 0x012B,
        READ_BOLUS_WIZARD_CARB_RATIOS_RESPONSE = 0x012C,
        READ_BOLUS_WIZARD_SENSITIVITY_FACTORS_REQUEST = 0x012E,
        READ_BOLUS_WIZARD_SENSITIVITY_FACTORS_RESPONSE = 0x012F,
        READ_BOLUS_WIZARD_BG_TARGETS_REQUEST = 0x0131,
        READ_BOLUS_WIZARD_BG_TARGETS_RESPONSE = 0x0132,
        DEVICE_STRING_REQUEST = 0x013A,
        DEVICE_STRING_RESPONSE = 0x013B,
        DEVICE_CHARACTERISTICS_REQUEST = 0x0200,
        DEVICE_CHARACTERISTICS_RESPONSE = 0x0201,
        READ_HISTORY_REQUEST = 0x0304,
        READ_HISTORY_RESPONSE = 0x0305,
        END_HISTORY_TRANSMISSION = 0x030A,
        READ_HISTORY_INFO_REQUEST = 0x030C,
        READ_HISTORY_INFO_RESPONSE = 0x030D,
        UNMERGED_HISTORY_RESPONSE = 0x030E,
        INITIATE_MULTIPACKET_TRANSFER = 0xFF00,
        MULTIPACKET_SEGMENT_TRANSMISSION = 0xFF01,
        MULTIPACKET_RESEND_PACKETS = 0xFF02,
        ACK_MULTIPACKET_COMMAND = 0x00FE,
        Read_Preset_Boluses_Request = 0x0114,
        Read_Preset_Boluses_Response = 0x0121,
        Read_Preset_Temp_Basals_Request = 0x0115,
        Read_Preset_Temp_Basals_Response = 0x0122,
        Read_Timed_Notifications_Request = 0x0134,
        Read_Timed_Notifications_Response = 0x0135,
        Read_Low_Glucose_Sensor_Settings_Request = 0x0211,
        Read_Low_Glucose_Sensor_Settings_Response = 0x0212,
        Read_High_Glucose_Sensor_Settings_Request = 0x0215,
        Read_High_Glucose_Sensor_Settings_Response = 0x0216,
        Read_Basic_NGP_Parameters_Request = 0x0138,
        Read_Basic_NGP_Parameters_Response = 0x0139,
        //BOLUSES_REQUEST = 0x0114,
        REMOTE_BOLUS_REQUEST = 0x0100,
        //REQUEST_0x0124 = 0x0124
        //REQUEST_0x0405 = 0x0405
        //TEMP_BASAL_REQUEST = 0x0115
        //SUSPEND_RESUME_REQUEST = 0x0107
        //NGP_PARAMETER_REQUEST = 0x0138
        //NO_TYPE = 0x0,
        ////ACK_End_Multipacket_Command = 0x00FE,
        ////ACK_Set_Link_Key_Command = 0x00FE,
        //ACK_Start_Multipacket_Command = 0x00FE,
        //Begin_Extended_High_Speed_Mode_Session = 0x0412,
        //Device_Characteristics_Request = 0x0200,
        //Device_Characteristics_Response = 0x0201,
        ////End_Extended_High_Speed_Mode_Session = 0x0412,
        //End_Node_Device_Initilization_Response = 0x040D,
        //End_of_History_Transmission = 0x030A,
        //Initiate_Multipacket_Transfer_Command = 0xFF00,
        //Multipacket_Segment_Transmission_Command = 0xFF01,
        //Read_Basal_Pattern_Request = 0x0116,
        //Read_Basal_Pattern_Response = 0x0123,
        //Read_Basic_NGP_Parameters_Request = 0x0138,
        //Read_Basic_NGP_Parameters_Response = 0x0139,


        //Read_Glucose_Sensor_Settings_Request = 0x020B,
        //Read_Glucose_Sensor_Settings_Response = 0x020C,
        //Read_High_Glucose_Sensor_Settings_Request = 0x0215,
        //Read_High_Glucose_Sensor_Settings_Response = 0x0216,

        //Read_Low_Glucose_Sensor_Settings_Request = 0x0211,
        //Read_Low_Glucose_Sensor_Settings_Response = 0x0212,
        //Read_Preset_Boluses_Request = 0x0114,
        //Read_Preset_Boluses_Response = 0x0121,
        //Read_Preset_Temp_Basals_Request = 0x0115,
        //Read_Preset_Temp_Basals_Response = 0x0122,
        //Read_Pump_Status_Request = 0x0112,
        //Read_Pump_Status_Response = 0x013C,
        //Read_Timed_Notifications_Request = 0x0134,
        //Read_Timed_Notifications_Response = 0x0135,
        //Read_Trace_History = 0x0302,


        //unknowns
        //prefixs: 0x01,0x02
        //[0] 0x99	byte
        //[1] 0x9f	byte

        //prefix: 0x03
        //[0] 0x9a	byte
        //[1] 0xa5	byte

        //HIGH_SPEED_MODE_COMMAND: 0x0412,

        //TIME_REQUEST: 0x0403,

        //TIME_RESPONSE: 0x0407,

        //READ_PUMP_STATUS_REQUEST: 0x0112,

        //READ_PUMP_STATUS_RESPONSE: 0x013C,

        //READ_BASAL_PATTERN_REQUEST: 0x0116,

        //READ_BASAL_PATTERN_RESPONSE: 0x0123,

        //READ_BOLUS_WIZARD_CARB_RATIOS_REQUEST: 0x012B,

        //READ_BOLUS_WIZARD_CARB_RATIOS_RESPONSE: 0x012C,

        //READ_BOLUS_WIZARD_SENSITIVITY_FACTORS_REQUEST: 0x012E,

        //READ_BOLUS_WIZARD_SENSITIVITY_FACTORS_RESPONSE: 0x012F,

        //READ_BOLUS_WIZARD_BG_TARGETS_REQUEST: 0x0131,

        //READ_BOLUS_WIZARD_BG_TARGETS_RESPONSE: 0x0132,

        //DEVICE_STRING_REQUEST: 0x013A,

        //DEVICE_STRING_RESPONSE: 0x013B,

        //DEVICE_CHARACTERISTICS_REQUEST: 0x0200,

        //DEVICE_CHARACTERISTICS_RESPONSE: 0x0201,

        //READ_HISTORY_REQUEST: 0x0304,

        //READ_HISTORY_RESPONSE: 0x0305,

        //END_HISTORY_TRANSMISSION: 0x030A,

        //READ_HISTORY_INFO_REQUEST: 0x030C,

        //READ_HISTORY_INFO_RESPONSE: 0x030D,

        //UNMERGED_HISTORY_RESPONSE: 0x030E,

        //INITIATE_MULTIPACKET_TRANSFER: 0xFF00,

        //MULTIPACKET_SEGMENT_TRANSMISSION: 0xFF01,

        //MULTIPACKET_RESEND_PACKETS: 0xFF02,

        //ACK_MULTIPACKET_COMMAND: 0x00FE,

    }


}
