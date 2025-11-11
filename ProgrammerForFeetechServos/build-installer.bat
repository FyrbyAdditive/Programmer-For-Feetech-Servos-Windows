@echo off
REM Build script for Programmer For Feetech Servos
REM Copyright (c) 2025 Fyrby Additive Manufacturing & Engineering

echo ========================================
echo Programmer For Feetech Servos - Build
echo ========================================
echo.

cd /d "%~dp0"

REM Run the PowerShell build script
powershell.exe -ExecutionPolicy Bypass -File "build-installer.ps1"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Build completed successfully!
    echo Check the installer folder for output files.
) else (
    echo.
    echo Build failed! Check the errors above.
)

echo.
pause
