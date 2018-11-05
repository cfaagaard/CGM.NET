using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Model
{
    public enum Alerts : Int16
    {
        Electric_Malfunction_Found_25=25,
        Alert_Before_High_817 = 817,
        Alert_Before_Low_805 = 805,
        Alert_On_High_816 = 816,
        Alert_On_Low_802 = 802,
        Alert_On_Low_while_suspended_803 = 803,
        Basal_Delivery_Resumed_Alert_glucose_still_low_maximum_suspend_reached_814 = 814,
        Basal_Delivery_Resumed_Alert_maximum_suspend_reached_808 = 808,
        Basal_Delivery_Resumed_Alert_quiet_806 = 806,
        Basal_Delivery_Resumed_Alert_settings_change_815 = 815,
        Battery_Depleted_11 = 11,
        Battery_Out_Limit_Exceeded_6 = 6,
        Device_Alarm_100 = 100, //time out for the accept of bolus (some wizard-thing? not pushing the last key?)
        Device_Alarm_109 = 109, //change cannula/insulin 
        Device_Alarm_110 = 110,
        Device_Alarm_111 = 111,
        Device_Alarm_113 = 113,
        Device_Alarm_58 = 58,
        Device_Alarm_Button_Pressed_3minutes=61,
        Device_Alarm_70 = 70,
        //Device_Alarm_73 = 73,
        //Device_Alarm_84 = 84,
        Device_Alarm_Change_Battery_73 = 73,
        Device_Alarm_Insert_Battery_84 = 84,
        Low_Battery_104 = 104,
        Low_Reservoir_105 = 105,
        No_Delivery_7 = 7,
        No_Delivery_After_Estimate_8 = 8,
        No_Reservoir_66 = 66,
        Sensor_Alarm_786 = 786, //no calibration
        Sensor_Alarm_787 = 787,
        Sensor_Alarm_788 = 788, //"BG not received."
        Sensor_Alarm_789 = 789,
        Sensor_Alarm_790 = 790, //
        Sensor_Alarm_791 = 791, //question: did the sensor blink when connected
        Sensor_Alarm_795 = 795,
        Sensor_Alarm_797 = 797,
        Sensor_Alarm_798 = 798,
        Sensor_Alarm_799 = 799, //sensor found will alert when to calibrate
        Sensor_Alert_Calibrate_Now_775 = 775,
        Sensor_Alert_Calibration_Error_776 = 776,
        Sensor_Alert_Calibration_Reminder_869 = 869,
        Sensor_Alert_Change_Sensor_777 = 777,
        Sensor_Alert_Change_Sensor_778 = 778,
        Sensor_Alert_Lost_Sensor_780 = 780,
        Sensor_Alert_Lost_Sensor_781 = 781,
        Sensor_Alert_Low_Transmitter_870 = 870,
        Sensor_Alert_Rising_Rate_of_Change_784 = 784,
        Sensor_Alert_Sensor_End_794 = 794,
        Sensor_Alert_Sensor_Error_801 = 801,
        Sensor_Alert_Weak_Signal_796 = 796,
        //Suspend_Before_Low_Alarm, _patient_unresponsive, _medical_device_emergency_812 = 812,
        Suspend_Before_Low_Alarm_811 = 811,
        Suspend_Before_Low_Alarm_patient_unresponsive_medical_device_emergency_812 = 812,
        Suspend_Before_Low_Alarm_quiet_810 = 810,
        Suspend_On_Low_Alarm_809 = 809,
        No_Warning_0 = 0,
    }

    public enum SgvAlert: Int16
    {
        Sensor_Cal_Error_772=772,
        Sensor_Cal_Needed_770 = 770,
        Sensor_Cal_Pending_778 = 778,
        Sensor_Change_Cal_Error_779 = 779,
        Sensor_Change_Sensor_Error_773 = 773,
        Sensor_End_Of_Life_774 = 774,
        Sensor_Error_771 = 771,
        Sensor_Init_769 = 769,
        Sensor_Not_Ready_775 = 775,
        Sensor_Reading_High_776 = 776,
        Sensor_Reading_Low_777 = 777,
        Sensor_Time_Unknown_780 = 780,
        No_Warning_0 = 0,
    }

    //Sensor_Alert_Change_774 = 774,
    //ShallWarmup_780= 780
    //pazan
    //769: SENSOR_INIT;
    //770: SENSOR_CAL_NEEDED;
    //771: SENSOR_ERROR;
    //772: SENSOR_CAL_ERROR;
    //773: SENSOR_CHANGE_SENSOR_ERROR;
    //774: SENSOR_END_OF_LIFE;
    //775: SENSOR_NOT_READY;
    //776: SENSOR_READING_HIGH;
    //777: SENSOR_READING_LOW;
    //778: SENSOR_CAL_PENDING;
    //779: SENSOR_CHANGE_CAL_ERROR;
    //780: SENSOR_TIME_UNKNOWN;

    //from pogman
    //Alert Before High (817)
    //Alert Before Low (805)
    //Alert On High (816)
    //Alert On Low (802)
    //Alert On Low while suspended (803)
    //Basal Delivery Resumed Alert, maximum suspend reached (808)
    //Basal Delivery Resumed Alert, quiet (806)
    //Basal Delivery Resumed Alert, settings change (815)
    //Battery Depleted (11)
    //Battery Out Limit Exceeded (6)
    //Device Alarm (100)
    //Device Alarm (110)
    //Device Alarm (113)
    //Device Alarm (73)
    //Device Alarm (84)
    //Low Battery (104)
    //Low Reservoir (105)
    //No Delivery (7)
    //No Reservoir (66)
    //Sensor Alarm (787)
    //Sensor Alarm (788)
    //Sensor Alarm (789)
    //Sensor Alarm (790)
    //Sensor Alarm (791)
    //Sensor Alarm (795)
    //Sensor Alarm (797)
    //Sensor Alarm (798)
    //Sensor Alarm (799)
    //Sensor Alert: Calibrate Now (775)
    //Sensor Alert: Calibration Reminder (869)
    //Sensor Alert: Lost Sensor (780)
    //Sensor Alert: Lost Sensor (781)
    //Sensor Alert: Rising Rate of Change (784)
    //Sensor Alert: Sensor End (794)
    //Sensor Alert: Sensor Error (801)
    //Sensor Alert: Weak Signal (796)
    //Suspend Before Low Alarm, patient unresponsive, medical device emergency (812)
    //Suspend Before Low Alarm, quiet (810)
    //Suspend On Low Alarm (809)
    //Sensor Alarm (786)
    //Sensor Alert: Calibration Error (776)
    //No Delivery After Estimate (8)
    //Sensor Alert: Change Sensor (777)
    //Device Alarm (111)
    //Sensor Alert: Low Transmitter (870)
    //Device Alarm (58)
    //Device Alarm (70)

}
