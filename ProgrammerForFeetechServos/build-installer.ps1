# Build script for Programmer For Feetech Servos - MSI Installer
# Copyright © 2025 Fyrby Additive Manufacturing & Engineering

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building MSI Installer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "ProgrammerForFeetechServos.csproj"
$publishPath = "bin\publish\win-x64"
$outputDir = "..\installer"
$appVersion = "1.0.0"

# Clean
Write-Host "Cleaning..." -ForegroundColor Yellow
if (Test-Path $publishPath) { Remove-Item -Path $publishPath -Recurse -Force }
if (Test-Path $outputDir) { Remove-Item -Path $outputDir -Recurse -Force }
New-Item -ItemType Directory -Path $outputDir -Force | Out-Null

# Build
Write-Host "Building Release..." -ForegroundColor Yellow
dotnet build $projectPath -c Release
if ($LASTEXITCODE -ne 0) { exit 1 }

# Publish
Write-Host "Publishing..." -ForegroundColor Yellow
dotnet publish $projectPath -c Release -r win-x64 --self-contained true -o $publishPath
if ($LASTEXITCODE -ne 0) { exit 1 }

# Download and run Inno Setup to create installer
Write-Host "Creating installer with Inno Setup..." -ForegroundColor Yellow
$isccPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if (Test-Path $isccPath) {
    & $isccPath "installer.iss"
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "Installer created successfully!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        $installerFile = Get-ChildItem "$outputDir\*.exe" | Select-Object -First 1
        if ($installerFile) {
            Write-Host "Installer: $($installerFile.FullName)" -ForegroundColor Cyan
            Write-Host "Size: $([math]::Round($installerFile.Length / 1MB, 2)) MB" -ForegroundColor Cyan
        }
    }
} else {
    Write-Host ""
    Write-Host "Inno Setup not found!" -ForegroundColor Red
    Write-Host "Download from: https://jrsoftware.org/isdl.php" -ForegroundColor Yellow
    Write-Host "After installing, run this script again." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Opening download page..." -ForegroundColor Yellow
    Start-Process "https://jrsoftware.org/isdl.php"
    exit 1
}
