using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CommandLineSwitchParser;
using TimeZoneConverter;
using static System.TimeZoneInfo;

namespace Toolbelt.Blazor.TimeZoneKit.GenerateSourceCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = CommandLineSwitch.Parse<CommandLineOptions>(ref args);
            options.BaseDirectory = options.BaseDirectory.Trim('"');
            if (string.IsNullOrWhiteSpace(options.BaseDirectory))
            {
                Console.WriteLine("Usage: dotnet exec Toolbelt.Blazor.TimeZoneKit.GenerateSourceCod.dll - b <base directory>");
                return;
            }

            GenerateIANAtoTZIdMap(options.BaseDirectory);
            GenerateSystemTimeZones(options.BaseDirectory);
        }

        private static void GenerateIANAtoTZIdMap(string baseDir)
        {
            var map = new StringBuilder();
            var ianaNames = TZConvert.KnownIanaTimeZoneNames;
            foreach (var ianaName in ianaNames)
            {
                var tzid = TZConvert.IanaToWindows(ianaName);
                map.Append("\\u0002" + ianaName + "\t" + tzid + "\\u0003");
            }

            var buff = new List<string>();
            buff.Add("namespace Toolbelt.Blazor.TimeZoneKit");
            buff.Add("{");
            buff.Add("    public static partial class TimeZoneKit");
            buff.Add("    {");
            buff.Add("        private const string IANAtoTZIdMap = \"" + map.ToString() + "\";");
            buff.Add("    }");
            buff.Add("}");

            var path = Path.Combine(baseDir, "TimeZoneKit.IANAtoTZIdMap.cs");
            File.WriteAllLines(path, buff);
        }

        private static void GenerateSystemTimeZones(string baseDir)
        {
            var adjustmentRulesField = typeof(TimeZoneInfo).GetField("_adjustmentRules", BindingFlags.Instance | BindingFlags.NonPublic);
            var baseUtcOffsetDeltaField = typeof(AdjustmentRule).GetField("_baseUtcOffsetDelta", BindingFlags.Instance | BindingFlags.NonPublic);

            var buff = new List<string>();
            buff.Add("using System;");
            buff.Add("using static System.TimeZoneInfo;");
            buff.Add("");
            buff.Add("namespace Toolbelt.Blazor.TimeZoneKit");
            buff.Add("{");
            buff.Add("    public static partial class TimeZoneKit");
            buff.Add("    {");
            buff.Add("        /// <summary>");
            buff.Add("        /// Create system time zones array.");
            buff.Add("        /// </summary>");
            buff.Add("        public static TimeZoneInfo[] CreateSystemTimeZones()");
            buff.Add("        {");
            buff.Add("            BeginCreateSystemTimeZones();");
            buff.Add("");
            buff.Add("            var sysTimeZones = new[] {");

            foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
            {
                var rules = adjustmentRulesField.GetValue(tz) as AdjustmentRule[];
                buff.Add("                " +
                    $"TZ(\"{tz.Id}\", {tz.BaseUtcOffset.Ticks}, \"{tz.DisplayName}\", \"{tz.StandardName}\", \"{tz.DaylightName}\", {(rules == null ? "null)," : "new AdjustmentRule[] {")}");

                if (rules != null)
                {
                    foreach (var r in rules)
                    {
                        var baseUtcOffsetDelta = (TimeSpan)baseUtcOffsetDeltaField.GetValue(r);
                        var t1 = CreateCode(r.DaylightTransitionStart);
                        var t2 = CreateCode(r.DaylightTransitionEnd);
                        buff.Add("                    " +
                            $"R({r.DateStart.ToBinary()}, {r.DateEnd.ToBinary()}, {r.DaylightDelta.Ticks}, {t1}, {t2}, {baseUtcOffsetDelta.Ticks}),");
                    }
                    buff.Add("                " + "}),");
                }
            }

            buff.Add("            };");
            buff.Add("");
            buff.Add("            EndCreateSystemTimeZones();");
            buff.Add("");
            buff.Add("            return sysTimeZones;");
            buff.Add("        }");
            buff.Add("    }");
            buff.Add("}");

            var path = Path.Combine(baseDir, "TimeZoneKit.CreateSystemTimeZones.cs");
            File.WriteAllLines(path, buff);
        }

        private static string CreateCode(TransitionTime t)
        {
            if (t.IsFixedDateRule)
                return $"T({t.TimeOfDay.ToBinary()}, {t.Month}, {t.Day})";
            else
                return $"T({t.TimeOfDay.ToBinary()}, {t.Month}, {t.Week}, DayOfWeek.{t.DayOfWeek})";
        }
    }
}
