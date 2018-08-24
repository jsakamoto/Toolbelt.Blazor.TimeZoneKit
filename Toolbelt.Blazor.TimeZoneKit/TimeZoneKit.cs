using System;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.JSInterop;

namespace Toolbelt.Blazor.TimeZoneKit
{
    /// <summary>
    /// Provides system time zones set, and local time zone initialization.
    /// </summary>
    public static partial class TimeZoneKit
    {
        /// <summary>
        /// Set TimeZoneInfo.Local to specified time zone by IANA name.
        /// </summary>
        [JSInvokable("InitLocalTimeZone")]
        public static void SetLocalTimeZoneByIANAName(string ianaTimeZoneName)
        {
            var timeZoneId = TimeZoneKit.GetTimeZoneIdFromIANA(ianaTimeZoneName);
            TimeZoneKit.SetLocalTimeZone(timeZoneId);
        }

        /// <summary>
        /// Set TimeZoneInfo.Local to specified time zone.
        /// </summary>
        public static void SetLocalTimeZone(string timeZoneId)
        {
            foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
            {
                if (tz.Id == timeZoneId)
                {
                    SetLocalTimeZone(tz);
                    break;
                }
            }
        }

        /// <summary>
        /// Set TimeZoneInfo.Local to specified time zone.
        /// </summary>
        public static void SetLocalTimeZone(TimeZoneInfo localTimeZone)
        {
            var localFiled = typeof(TimeZoneInfo).GetField("local", BindingFlags.NonPublic | BindingFlags.Static);
            localFiled.SetValue(null, localTimeZone);
        }

        /// <summary>
        /// Set system time zones.
        /// </summary>
        public static void SetSystemTimeZones(TimeZoneInfo[] timeZones)
        {
            var systemTimeZones = typeof(TimeZoneInfo).GetField("systemTimeZones", BindingFlags.NonPublic | BindingFlags.Static);
            systemTimeZones.SetValue(null, new ReadOnlyCollection<TimeZoneInfo>(timeZones));
        }

        /// <summary>
        /// Convert IANA time zone name to .NET time zone id.
        /// </summary>
        public static string GetTimeZoneIdFromIANA(string ianaName)
        {
            var searchText = "\u0002" + ianaName + "\t";
            var headPos = TimeZoneKit.IANAtoTZIdMap.IndexOf(searchText);
            if (headPos == -1) return "UTC";

            var midPos = headPos + searchText.Length;
            var termPos = TimeZoneKit.IANAtoTZIdMap.IndexOf('\u0003', midPos);

            var tzid = TimeZoneKit.IANAtoTZIdMap.Substring(midPos, termPos - midPos);
            return tzid;
        }

    }
}
