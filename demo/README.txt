INVISIBLEINK - DEMO PACKAGE
===========================

InvisibleInk is a booking system for a piercing studio. Receptionists book
appointments and record customer health checks, and each piercer sees their
own schedule.

This folder has everything needed to run it on another Windows PC:

  InvisibleInk.exe   The app. .NET and the database engine are built in.
  reset-demo.bat     Wipes all changes and starts again with fresh test data.
  LOGINS.txt         User IDs, passwords and database details.
  invisibleink.db    The database. Created automatically the first time the
                     app runs, so it won't be there until then.


REQUIREMENTS
------------
Windows 10 or 11 (64-bit). Nothing to install: no SQL Server, Docker, .NET
or internet needed.


RUNNING THE DEMO
----------------
1. Copy this whole "demo" folder to the PC, e.g. onto the Desktop
   (about 150 MB). Don't put it in Program Files, because the app needs to
   be able to write its database file.
2. Double-click InvisibleInk.exe.
   - Windows may show "Windows protected your PC". Click "More info", then
     "Run anyway". This happens because the exe isn't code-signed.
3. Log in with a User ID from LOGINS.txt (e.g. 1 = Receptionist, 2 = Nick).


TIP: RESET BEFORE THE DEMO
--------------------------
The test bookings run from 3 weeks before to 4 weeks after the day the
database is created. On the day of the demo, run reset-demo.bat, then open
the app. "Today" then has bookings for every piercer, with past and future
bookings around it.


TEST DATA
---------
- 8 staff logins (see LOGINS.txt)
- 20 piercing types with prices
- 25 customers with health check notes
- About 560 bookings over 7 weeks, on each piercer's working days, with
  3-4 for every piercer on the day the database was created


SUGGESTED DEMO WALKTHROUGH
--------------------------
Receptionist (User ID 1):
  - Type part of a name (e.g. "Sm") in Client Name to search existing customers.
  - Pick a piercing to see its price. Choose a time first, then a date, to
    see which staff are available. Then add the booking. Booking the same
    piercer, date and time twice is blocked.
  - Booking Management: show all bookings, then update or delete one using
    the number in the ID column as the booking reference.
  - Customer Information form: add a new customer's health check.

Piercer (User IDs 2-7):
  - Their schedule loads on login.
  - "Appointments Today" shows today's bookings.
  - Choose From/To dates to filter their schedule.


TROUBLESHOOTING
---------------
The app closes straight away or shows a database error
    Make sure the folder isn't read-only and isn't inside Program Files.
    Running reset-demo.bat to recreate the database can also help.

"User Not Found"
    Use a number from 1 to 8 (see LOGINS.txt).


FOR DEVELOPERS
--------------
Source: InvisibleInk.sln in the folder above this one.
- Schema and test data: InvisibleInk\DemoDatabase.cs. It runs on startup
  and only creates the database if invisibleink.db doesn't exist.
- Rebuild this demo folder from the solution folder:
    dotnet publish InvisibleInk\InvisibleInk.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o demo
