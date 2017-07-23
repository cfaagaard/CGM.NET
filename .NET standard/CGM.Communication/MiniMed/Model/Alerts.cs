using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.MiniMed.Model
{
    public enum Alerts: Int16
    {
        No_Warning_0 = 0,
        Alert_On_High_816 = 816,
        Alert_Before_High_817 = 817,
        Alert_On_Low_while_suspended_803 = 803,
        Battery_Depleted_11 = 11,
        Device_Alarm_100 = 100,
        Device_Alarm_109 = 109,
        Device_Alarm_73 = 73,
        Device_Alarm_84 = 84,
        No_Delivery_7 = 7,
        Low_Battery_104 = 104,
        Low_Reservoir_105 = 105,
        Sensor_Alert_Calibrate_Now_775 = 775,
        Sensor_Alert_Calibration_Error_776 = 776,
        Sensor_Alert_Calibration_Reminder_869 = 869,
        Sensor_Alert_Low_Transmitter_870 = 870,
        Sensor_Alert_Sensor_Error_801 = 801,
        Sensor_Alert_Change_Sensor_778 = 778,
        Sensor_Alert_Lost_Sensor_780 = 780,
        Sensor_Alert_Lost_Sensor_781 = 781,
        Sensor_Alarm_786 = 786,
        Sensor_Alarm_787 = 787,
        Sensor_Alarm_788 = 788,
        Sensor_Alarm_795 = 795,
        Sensor_Alarm_798 = 798,
        Sensor_Alarm_799 = 799,
        Suspend_Before_Low_Alarm_quiet_810 = 810,
        Suspend_Before_Low_Alarm_811 = 811,
        Basal_Delivery_Resumed_Alert_quiet_806 = 806,
        Suspend_Before_Low_Alarm_patient_unresponsive_medical_device_emergency_812 = 812,
        Basal_Delivery_Resumed_Alert_maximum_suspend_reached_808 = 808,
        Basal_Delivery_Resumed_Alert_glucose_still_low_maximum_suspend_reached_814 = 814

    }
}
