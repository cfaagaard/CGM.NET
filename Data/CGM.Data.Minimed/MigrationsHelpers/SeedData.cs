using CGM.Communication.MiniMed.Infrastructur;
using CGM.Communication.MiniMed.Model;
using CGM.Data.Minimed.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGM.Data.Minimed.Migrations
{
   public static class SeedData
    {
        public static List<EventType> EventType()
        {
            List<EventType> eventTypes = new List<EventType>();
            foreach (byte eventtypevalue in (byte[])Enum.GetValues(typeof(EventTypeEnum)))
            {
                EventType eventType = new EventType();
                eventType.EventTypeId = eventtypevalue;
                eventType.EventTypeFullName = ((EventTypeEnum)eventtypevalue).ToString();
                var list = new List<string>( eventType.EventTypeFullName.Split('_'));
                List<string> list2 = new List<string>();
                list.ForEach(e => list2.Add(e.First().ToString().ToUpper() + e.Substring(1).ToLower()));
                eventType.EventTypeName = string.Join(" ", list2);

                eventTypes.Add(eventType);
            }
            return eventTypes;
        }

        public static List<PumpEventAlert> PumpEventAlerts()
        {
            List<PumpEventAlert> eventAlerts = new List<PumpEventAlert>();
            foreach (Int16 alert in (Int16[])Enum.GetValues(typeof(Alerts)))
            {
                PumpEventAlert pumpEventAlert = new PumpEventAlert();
                pumpEventAlert.PumpEventAlertId = (int)alert;
                pumpEventAlert.PumpEventAlertFullName = ((Alerts)alert).ToString();
                var list = new List<string>(pumpEventAlert.PumpEventAlertFullName.Split('_'));
                list.RemoveAt(list.Count - 1);
                pumpEventAlert.PumpEventAlertName = string.Join(" ", list);

                var test = eventAlerts.FirstOrDefault(e => e.PumpEventAlertId == pumpEventAlert.PumpEventAlertId);
                if (test!=null)
                {
                    throw new Exception("Already exists.");
                }

                eventAlerts.Add(pumpEventAlert);
            }
            return eventAlerts;
        }


        public static List<SensorReadingAlert> SensorReadingAlerts()
        {
            List<SensorReadingAlert> eventAlerts = new List<SensorReadingAlert>();
            foreach (Int16 alert in (Int16[])Enum.GetValues(typeof(SgvAlert)))
            {
                SensorReadingAlert sensorReadingAlert = new SensorReadingAlert();
                sensorReadingAlert.SensorReadingAlertId = (int)alert;
                sensorReadingAlert.SensorReadingAlertFullName = ((SgvAlert)alert).ToString();
                var list = new List<string>(sensorReadingAlert.SensorReadingAlertFullName.Split('_'));
                list.RemoveAt(list.Count - 1);
                sensorReadingAlert.SensorReadingAlertName = string.Join(" ", list);

                var test = eventAlerts.FirstOrDefault(e => e.SensorReadingAlertId == sensorReadingAlert.SensorReadingAlertId);
                if (test != null)
                {
                    throw new Exception("Already exists.");
                }

                eventAlerts.Add(sensorReadingAlert);
            }
            return eventAlerts;
        }
    }
}
