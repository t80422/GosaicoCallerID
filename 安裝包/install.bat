@echo off
CD /D "%~dp0"

echo "start install..."
xcopy /s /i /Y "./gosaicoCallerID" "c:\gosaicoCallerID"
echo "copy finished!!"

echo "setup auto start~~"
rem win10 11
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run" /v gosaicoCallerID /d "c:\gosaicoCallerID\gosaicoCallerID.exe" -f
rem win7
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run" /v gosaicoCallerID /d "c:\gosaicoCallerID\gosaicoCallerID.exe" -f
echo "setup finished!!"

pause