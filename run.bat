@echo off
echo Building Programmer For Feetech Servos...
dotnet build ProgrammerForFeetechServos\ProgrammerForFeetechServos.csproj

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Build successful! Starting application...
    echo.
    dotnet run --project ProgrammerForFeetechServos\ProgrammerForFeetechServos.csproj
) else (
    echo.
    echo Build failed! Check errors above.
    pause
)
