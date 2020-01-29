using System;
using Microsoft.AspNetCore.Blazor.Hosting;
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
        public static WebAssemblyHost UseLocalTimeZone(this WebAssemblyHost host)
        {
            host.UseSystemTimeZones();
            var jsRuntime = host.Services.GetService(typeof(IJSRuntime)) as IJSInProcessRuntime;
            if (jsRuntime != null)
            {
                var ianaTimeZoneName = jsRuntime.Invoke<string>("eval", "(function(){try { return ''+ Intl.DateTimeFormat().resolvedOptions().timeZone; } catch(e) {} return 'UTC';}())");
                TimeZoneKit.TimeZoneKit.SetLocalTimeZoneByIANAName(ianaTimeZoneName);
            }
            return host;
        }

        /// <summary>
        /// Set "TimeZoneInfo.Local" to the time zone that specified id. (include "UseSystemTimeZones()")
        /// </summary>
        public static WebAssemblyHost UseLocalTimeZone(this WebAssemblyHost host, string timeZoneId)
        {
            host.UseSystemTimeZones();
            TimeZoneKit.TimeZoneKit.SetLocalTimeZone(timeZoneId);
            return host;
        }

        /// <summary>
        /// Set "TimeZoneInfo.Local" to the time zone that specified IANA name. (include "UseSystemTimeZones()")
        /// </summary>
        public static WebAssemblyHost UseLocalTimeZoneByIANAName(this WebAssemblyHost host, string ianaTimeZoneName)
        {
            host.UseSystemTimeZones();
            TimeZoneKit.TimeZoneKit.SetLocalTimeZoneByIANAName(ianaTimeZoneName);
            return host;
        }

        /// <summary>
        /// Ensure "TimeZoneInfo.GetSystemTimeZones()"
        /// </summary>
        public static WebAssemblyHost UseSystemTimeZones(this WebAssemblyHost host)
        {
            if (TimeZoneInfo.GetSystemTimeZones().Count == 0)
            {
                TimeZoneKit.TimeZoneKit.SetSystemTimeZones(TimeZoneKit.TimeZoneKit.CreateSystemTimeZones());
            }
            return host;
        }
    }
}
