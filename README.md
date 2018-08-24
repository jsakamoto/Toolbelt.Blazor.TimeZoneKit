# Toolbelt.Blazor.TimeZoneKit [![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.Blazor.TimeZoneKit.svg)](https://www.nuget.org/packages/Toolbelt.Blazor.TimeZoneKit/)

## Summary

This is a class library as a NuGet package for [Blazor](https://blazor.net/) browser application.

This package provides system time zones set, and local time zone initialization, for [Blazor](https://blazor.net/) browser application.

![fig. 1](.assets/fig-1.png)

### Note

At this time, the version of Blazor is v.0.5.x, it doesn't contain any time zone info, and local time zone is always UTC wherever.

See also: 

- [Blazor Issue #663 - TimeZone issues related to Mono?](https://github.com/aspnet/Blazor/issues/663)
- [Blazor Issue #1166 - DateTime always displays as UTC](https://github.com/aspnet/Blazor/issues/1166)
- [Mono Issue #6368 - WebAssembly: Missing Timezone implementation](https://github.com/mono/mono/issues/6368)

## How to install and use it?

**Step.1** - Install this package.

```shell
> dotnet add package Toolbelt.Blazor.TimeZoneKit
```

**Step.2** - call `UseLocalTimeZone()` extension method  in `Configure()` method of startup class.

```csharp
...
using Toolbelt.Blazor.TimeZoneKit;

public class Startup
{
    ...
    public void Configure(IBlazorApplicationBuilder app)
    {
        app.UseLocalTimeZone();
    ...
```

That's all!

### Note

In my test case, this package increase the size of your Blazor browser application contents to 154KB/20KB gzip transfer.

## License

[Mozilla Public License Version 2.0](LICENSE)
