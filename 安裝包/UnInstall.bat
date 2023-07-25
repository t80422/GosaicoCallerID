@echo off
echo "Start UnInstall..."
rem win10 11
reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run" /v gosaicoCallerID /f
rem win7
reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run" /v gosaicoCallerID /f
echo "Finished!!"
pause