using System;

namespace CGM.Communication.MiniMed.Infrastructur
{
    public enum AstmSendMessageType 
    {
        NO_TYPE = 0x0,
        //ACK_End_Multipacket_Command = 0x00FE,
        //ACK_Set_Link_Key_Command = 0x00FE,
        ACK_Start_Multipacket_Command = 0x00FE,
        Begin_Extended_High_Speed_Mode_Session = 0x0412,
        Device_Characteristics_Request = 0x0200,
        Device_Characteristics_Response = 0x0201,
        //End_Extended_High_Speed_Mode_Session = 0x0412,
        End_Node_Device_Initilization_Response = 0x040D,
        End_of_History_Transmission = 0x030A,
        Initiate_Multipacket_Transfer_Command = 0xFF00,
        Multipacket_Segment_Transmission_Command = 0xFF01,
        Read_Basal_Pattern_Request = 0x0116,
        Read_Basal_Pattern_Response = 0x0123,
        Read_Basic_NGP_Parameters_Request = 0x0138,
        Read_Basic_NGP_Parameters_Response = 0x0139,
        Read_Bolus_Wizard_BG_Targets_Request = 0x0131,
        Read_Bolus_Wizard_BG_Targets_Response = 0x0132,
        Read_Bolus_Wizard_Carb_Ratios_Request = 0x012B,
        Read_Bolus_Wizard_Carb_Ratios_Response = 0x012C,
        Read_Bolus_Wizard_Sensitivity_Factors_Request = 0x012E,
        Read_Bolus_Wizard_Sensitivity_Factors_Response = 0x012F,
        Read_Glucose_Sensor_Settings_Request = 0x020B,
        Read_Glucose_Sensor_Settings_Response = 0x020C,
        Read_High_Glucose_Sensor_Settings_Request = 0x0215,
        Read_High_Glucose_Sensor_Settings_Response = 0x0216,
        Read_History_Info_Response = 0x030D,
        Read_History_Info = 0x030C,
        Read_History = 0x0304,
        Read_History_Response = 0x01FF,
        Read_Low_Glucose_Sensor_Settings_Request = 0x0211,
        Read_Low_Glucose_Sensor_Settings_Response = 0x0212,
        Read_Preset_Boluses_Request = 0x0114,
        Read_Preset_Boluses_Response = 0x0121,
        Read_Preset_Temp_Basals_Request = 0x0115,
        Read_Preset_Temp_Basals_Response = 0x0122,
        Read_Pump_Status_Request = 0x0112,
        Read_Pump_Status_Response = 0x013C,
        Read_Timed_Notifications_Request = 0x0134,
        Read_Timed_Notifications_Response = 0x0135,
        Read_Trace_History = 0x0302,
        Time_Request = 0x0403,
        Time_Response = 0x0407,

        //unknowns
        //prefixs: 0x01,0x02
        //[0] 0x99	byte
        //[1] 0x9f	byte

        //prefix: 0x03
        //[0] 0x9a	byte
        //[1] 0xa5	byte

    }


}
