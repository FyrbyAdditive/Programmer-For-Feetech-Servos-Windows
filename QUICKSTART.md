# Quick Start Guide

## Installation

### Option 1: Use the Installer (Recommended)
1. Download `ProgrammerForFeetechServos-Setup-1.0.0.msi` from the installer folder
2. Double-click the MSI file to run the installer
3. Follow the installation wizard
4. Launch from Start Menu: "Programmer For Feetech Servos"

### Option 2: Run from Source
**Prerequisites:**
- .NET 8.0 SDK - https://dotnet.microsoft.com/download/dotnet/8.0
- Windows 10 or later
- USB serial adapter (for connecting to Feetech servos)

**Quick Run:**
- Double-click `run.bat` in the project folder, or
- Run: `dotnet run --project ProgrammerForFeetechServos\ProgrammerForFeetechServos.csproj`

## First Time Setup

1. **Launch the application**
   - The main window will open showing "No servos found"

2. **Select your COM port**
   - Click the "Port" dropdown in the toolbar
   - Select your USB serial adapter (e.g., COM3, COM4)
   - If your port doesn't appear, click the refresh button (🔄)

3. **Connect to servos**
   - Click the "Connect" button
   - Connection status indicator will turn green
   - Application will automatically scan for servos

4. **Wait for scan to complete**
   - Progress bar will show scan progress
   - Found servos will appear in the left panel

## Using the Application

### Changing a Servo ID

1. **Select a servo** from the list on the left
2. **Click "Change Servo ID"** button
3. **Enter the new ID** (must be 1-252)
4. **Click "Change ID"** to confirm
5. **Wait for confirmation** - the servo will restart with new ID
6. **Verify** - servo will appear with new ID after automatic rescan

### Rescanning for Servos

- Click the "🔄 Refresh Scan" button at the bottom of the servo list
- Wait for scan to complete (takes a few seconds)

### Disconnecting

- Click the "Disconnect" button in the toolbar
- Connection status will turn gray

## Troubleshooting

### No COM ports appear
- Ensure your USB serial adapter is plugged in
- Check Windows Device Manager for COM port
- Click the refresh button (🔄) next to port dropdown
- May need to install USB serial drivers

### No servos found
- Check power supply to servos (external power required!)
- Verify TX/RX connections on adapter
- Try different baud rate if needed
- Ensure servos are actually powered on
- Check that cables are securely connected

### Application won't start
- Ensure .NET 8.0 Runtime is installed
- Check if port is in use by another application
- Try running as administrator

### Connection lost during operation
- Application will auto-detect and notify you
- Check USB cable connection
- Try reconnecting

## Getting Help

- **In-app help**: Click "Help" button in toolbar for detailed documentation
- **About**: Click "About" button for version and contact information

## Tips

💡 **Label your servos** - After changing IDs, label servos with tape to track them

💡 **One at a time** - If servos have duplicate IDs, connect and program one at a time

💡 **External power** - Always use external power supply for servos (USB power is insufficient)

💡 **Test after changes** - Verify the new ID works before installing servo in your project

⚠️ **Changes are permanent** - ID changes are stored in EEPROM and persist across power cycles

## Quick Reference

| Action | Location |
|--------|----------|
| Select COM port | Toolbar dropdown |
| Connect | Toolbar button |
| Scan servos | Bottom of servo list |
| Change ID | Click servo, then "Change Servo ID" button |
| Help | Toolbar "Help" button |
| About | Toolbar "About" button |

---

**Need more help?** Click the "Help" button in the application for comprehensive documentation!
