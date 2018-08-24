using System;
using System.Reflection;
using static System.TimeZoneInfo;

namespace Toolbelt.Blazor.TimeZoneKit
{
    public static partial class TimeZoneKit
    {
        private static FieldInfo BaseUtcOffsetDeltaField;

        private static void BeginCreateSystemTimeZones()
        {
            var fields = typeof(TimeZoneInfo.AdjustmentRule).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.Name.EndsWith("aseUtcOffsetDelta")) { BaseUtcOffsetDeltaField = field; break; }
            }
        }

        private static void EndCreateSystemTimeZones()
        {
            BaseUtcOffsetDeltaField = null;
        }

        private static TimeZoneInfo TZ(string id, long baseUtcOffset, string displayName, string standardName, string daylightName, AdjustmentRule[] rules)
        {
            return TimeZoneInfo.CreateCustomTimeZone(id, TimeSpan.FromTicks(baseUtcOffset), displayName, standardName, daylightName, rules);
        }

        private static AdjustmentRule R(long dateStart, long dateEnd, long daylightDelta, TransitionTime daylightTransitionStart, TransitionTime daylightTransitionEnd, long baseUtcOffsetDelta)
        {
            var rule = AdjustmentRule.CreateAdjustmentRule(
                DateTime.FromBinary(dateStart),
                DateTime.FromBinary(dateEnd),
                TimeSpan.FromTicks(daylightDelta),
                daylightTransitionStart, daylightTransitionEnd);
            BaseUtcOffsetDeltaField.SetValue(rule, TimeSpan.FromTicks(baseUtcOffsetDelta));
            return rule;
        }

        private static TransitionTime T(long timeOfDay, int month, int day)
        {
            return TransitionTime.CreateFixedDateRule(DateTime.FromBinary(timeOfDay), month, day);
        }

        private static TransitionTime T(long timeOfDay, int month, int week, DayOfWeek dayOfWeek)
        {
            return TransitionTime.CreateFloatingDateRule(DateTime.FromBinary(timeOfDay), month, week, dayOfWeek);
        }
    }
}
