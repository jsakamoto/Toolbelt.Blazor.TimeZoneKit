using System;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.JSInterop;

namespace Toolbelt.Blazor.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods releated with TimeZoneKit.
    /// </summary>
    public static class TimeZoneKitExtension
    {
        /// <summary>
        /// Set "TimeZoneInfo.Local" to actual local time zone. (include "UseSystemTimeZones()")
        /// </summary>
        public static void UseLocalTimeZone(this IComponentsApplicationBuilder app)
        {
            app.UseSystemTimeZones();
            var jsRuntime = app.Services.GetService(typeof(IJSRuntime)) as IJSInProcessRuntime;
            if (jsRuntime != null)
            {
                var ianaTimeZoneName = jsRuntime.Invoke<string>("eval", "(function(){try { return ''+ Intl.DateTimeFormat().resolvedOptions().timeZone; } catch(e) {} return 'UTC';}())");
                TimeZoneKit.TimeZoneKit.SetLocalTimeZoneByIANAName(ianaTimeZoneName);
            }
        }

        /// <summary>
        /// Set "TimeZoneInfo.Local" to the time zone that specified id. (include "UseSystemTimeZones()")
        /// </summary>
        public static void UseLocalTimeZone(this IComponentsApplicationBuilder app, string timeZoneId)
        {
            app.UseSystemTimeZones();
            TimeZoneKit.TimeZoneKit.SetLocalTimeZone(timeZoneId);
        }

        /// <summary>
        /// Set "TimeZoneInfo.Local" to the time zone that specified IANA name. (include "UseSystemTimeZones()")
        /// </summary>
        public static void UseLocalTimeZoneByIANAName(this IComponentsApplicationBuilder app, string ianaTimeZoneName)
        {
            app.UseSystemTimeZones();
            TimeZoneKit.TimeZoneKit.SetLocalTimeZoneByIANAName(ianaTimeZoneName);
        }

        /// <summary>
        /// Ensure "TimeZoneInfo.GetSystemTimeZones()"
        /// </summary>
        public static void UseSystemTimeZones(this IComponentsApplicationBuilder app)
        {
            if (TimeZoneInfo.GetSystemTimeZones().Count == 0)
            {
                TimeZoneKit.TimeZoneKit.SetSystemTimeZones(TimeZoneKit.TimeZoneKit.CreateSystemTimeZones());
            }
        }
    }
}
