# Invisible Ink

A booking system for a piercing studio, written in C# / WPF (.NET 6). Receptionists book appointments and record customer health checks, and each piercer sees their own schedule.

The app uses a local **SQLite** database (`invisibleink.db`, created next to the exe on first run). The schema and test data are in [`InvisibleInk/DemoDatabase.cs`](InvisibleInk/DemoDatabase.cs). Delete the file to reset it.

## Build and run

Requires the .NET SDK (6 or later) on Windows.

```
dotnet run --project InvisibleInk
```

## Build the demo package

This produces a single self-contained `InvisibleInk.exe` in `demo/`. It runs on any 64-bit Windows 10/11 PC with nothing else installed.

```
dotnet publish InvisibleInk/InvisibleInk.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o demo
```

The exe isn't committed because it is over GitHub's 100 MB file limit. See [`demo/README.txt`](demo/README.txt) for running the demo and [`demo/LOGINS.txt`](demo/LOGINS.txt) for user IDs.

## Logins

Log in with a User ID: `1` Receptionist, `2`–`7` piercers (Nick, Oxley, Dave, Claudia, Skelly, Spudgun), `8` Admin.
