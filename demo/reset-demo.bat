@echo off
cd /d "%~dp0"
echo This deletes all bookings/customers added during demos.
echo Fresh test data (with today's date) is created the next time you open the app.
choice /m "Continue"
if errorlevel 2 exit /b 0
taskkill /IM InvisibleInk.exe /F >nul 2>&1
del /q invisibleink.db >nul 2>&1
echo Reset done.
pause
