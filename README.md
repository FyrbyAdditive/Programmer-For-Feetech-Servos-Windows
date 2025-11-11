# Programmer For Feetech Servos - Windows Edition

A modern Windows WPF application for programming and configuring Feetech servos. This application provides an intuitive interface for scanning servos on a serial bus and changing their IDs.

## Features

- **Automatic Servo Detection**: Scans IDs 1-252 to detect connected servos
- **Easy ID Changes**: Simple interface to change servo IDs with validation
- **Real-time Connection Monitoring**: Automatic disconnect detection and handling
- **Modern UI**: Clean, responsive WPF interface with Material Design-inspired styling
- **Async Operations**: All operations run asynchronously with cancellation support
- **Comprehensive Help**: Built-in help system with hardware setup guides
- **Progress Indicators**: Visual feedback during scanning and ID changes
- **MSI Installer**: Professional installer package for easy deployment

## Installation

### End Users

Download and run the MSI installer:
```
installer/ProgrammerForFeetechServos-Setup-1.0.0.msi
```

The installer will:
- Install the application to Program Files
- Create a Start Menu shortcut
- Register for easy uninstallation

### Developers

See [Building from Source](#building-from-source) below.

## System Requirements

- **OS**: Windows 10 or later (64-bit)
- **Runtime**: .NET 8.0 (included in installer for end users)
- **Hardware**: 
  - USB to TTL serial adapter (compatible with 3.3V or 5V logic)
  - Feetech servos (STS/SMS/SCS series)
  - External power supply for servos (5-12V depending on servo model)

## Quick Start

1. **Install** the application using the MSI installer
2. **Connect** your USB serial adapter to your computer
3. **Power** your servos with an external power supply
4. **Launch** the app from the Start Menu
5. **Select** your COM port from the dropdown
6. **Click** Connect to scan for servos
7. **Change** servo IDs as needed

For detailed instructions, see [QUICKSTART.md](QUICKSTART.md)

## Building from Source

### Prerequisites

1. .NET 8.0 SDK - https://dotnet.microsoft.com/download/dotnet/8.0
2. Visual Studio 2022 or VS Code with C# extension
3. Windows 10 or later
4. WiX Toolset 6.x (optional, for building installer)

### Build Steps

```powershell
# Navigate to the project directory
cd "C:\Users\timel\VSCode\Programmer For Feetech Servos Windows"

# Restore dependencies
dotnet restore ProgrammerForFeetechServos\ProgrammerForFeetechServos.csproj

# Build the project
dotnet build ProgrammerForFeetechServos\ProgrammerForFeetechServos.csproj -c Release

# Run the application
dotnet run --project ProgrammerForFeetechServos\ProgrammerForFeetechServos.csproj
```

### Publishing

Create a single-file executable:
```powershell
cd ProgrammerForFeetechServos
dotnet publish -c Release -r win-x64 --self-contained `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -o bin\publish\win-x64-single
```

### Building the Installer

```powershell
# Install WiX if not already installed
dotnet tool install --global wix

# Build the MSI installer
cd ProgrammerForFeetechServos
wix build Package.wxs -out ..\installer\ProgrammerForFeetechServos-Setup-1.0.0.msi
```

## Credits

**Copyright © 2025 Timothy Ellis, Fyrby Additive Manufacturing & Engineering**

- Ported from macOS SwiftUI application
- Uses [Feetech Servo SDK for C#](https://github.com/FyrbyAdditive/feetech-servo-sdk-csharp)

## License

All rights reserved. This application and its source code are proprietary to Fyrby Additive Manufacturing & Engineering.

## Support

For issues or questions:
- GitHub Issues: [Project Repository]
- Website: https://fyrbyadditive.com

---

**Built with ❤️ using C#, WPF, and .NET 8.0**
