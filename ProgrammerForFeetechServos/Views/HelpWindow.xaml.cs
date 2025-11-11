// Copyright © 2025 Timothy Ellis, Fyrby Additive Manufacturing & Engineering

using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Documents;

namespace ProgrammerForFeetechServos.Views;

public partial class HelpWindow : Window
{
    public HelpWindow()
    {
        InitializeComponent();
        Loaded += HelpWindow_Loaded;
    }

    private void HelpWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            LoadContent("Overview");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading help content: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void HelpTopicsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (HelpTopicsListBox?.SelectedItem is ListBoxItem item && item.Tag is string tag)
        {
            LoadContent(tag);
        }
    }

    private void LoadContent(string section)
    {
        // ContentPanel may be null during InitializeComponent
        if (ContentPanel == null)
            return;
            
        ContentPanel.Children.Clear();

        switch (section)
        {
            case "Overview":
                LoadOverview();
                break;
            case "SystemRequirements":
                LoadSystemRequirements();
                break;
            case "GettingStarted":
                LoadGettingStarted();
                break;
            case "UsingTheApp":
                LoadUsingTheApp();
                break;
            case "Troubleshooting":
                LoadTroubleshooting();
                break;
            case "Tips":
                LoadTips();
                break;
        }
    }

    private void LoadOverview()
    {
        AddTitle("Overview");
        AddBody("Programmer For Feetech Servos is a Windows application designed to easily program and configure Feetech servos. It provides a simple interface for changing servo IDs.");
        AddSeparator();
        AddSubtitle("Key Features");
        AddFeature("🔍", "Automatic Scanning", "Automatically detects servos on the bus");
        AddFeature("✏", "Easy ID Changes", "Simple interface to change servo IDs");
        AddFeature("💻", "Model Detection", "Identifies servo models automatically");
        AddFeature("⚡", "Real-time Status", "Live connection and servo status updates");
    }

    private void LoadSystemRequirements()
    {
        AddTitle("System Requirements");
        AddSubtitle("Hardware Requirements");
        AddBullet("USB serial adapter compatible with Feetech servos");
        AddBullet("Feetech servo(s) compatible with the same protocol including the STS and SCS series");
        AddSeparator();
        AddSubtitle("Tested Adapters");
        AddBody("The application has been tested with the following servo adapters, and probably works with most others:");
        
        // Create horizontal panel for adapters
        var adapterPanel = new WrapPanel { Orientation = Orientation.Horizontal };
        ContentPanel.Children.Add(adapterPanel);
        
        AddAdapterCardToPanel(adapterPanel, "Waveshare Bus Servo Adapter (A)", "WaveshareBusServoAdapterA.png", "https://www.waveshare.com/bus-servo-adapter-a.htm");
        AddAdapterCardToPanel(adapterPanel, "Waveshare Bus Servo Driver HAT (A)", "WaveshareBusServoDriverHATA.jpg", "https://www.waveshare.com/bus-servo-driver-hat-a.htm");
        
        AddSeparator();
        AddSubtitle("Tested Servos");
        AddBody("Currently the following servos have been tested, however others as stated above should work:");
        AddServoCard("STS 3215", "STS3250.png", "https://www.feetechrc.com/525603.html");
        AddServoCard("STS 3250", "STS3250.png", "https://www.feetechrc.com/562636.html");
    }

    private void LoadGettingStarted()
    {
        AddTitle("Getting Started");
        AddSubtitle("Installation & First Time Setup");
        AddStep(1, "Download the application installer");
        AddStep(2, "Run the installer and follow the installation wizard");
        AddStep(3, "Launch the application from the Start Menu or Desktop shortcut");
        AddSeparator();
        AddWarningBox("Windows Security Warning", 
            "When you first launch the application, Windows Defender SmartScreen may show a warning because the app is not from the Microsoft Store. This is normal for new applications. Click 'More info' and then 'Run anyway' to proceed.");
    }

    private void LoadUsingTheApp()
    {
        AddTitle("Using the Application");
        AddSubtitle("Connecting Your Servo");
        AddStep(1, "Connect a Feetech servo to the adapter");
        AddStep(2, "Connect a power supply matching the voltage of your servos to the adapter board");
        AddStep(3, "Connect a USB cable from the adapter board to your computer");
        AddStep(4, "Run the application");
        AddInfoBox("If carried out as above and everything is working properly, you should see the application start up and scan the bus for servos automatically. This takes a few seconds. Your servo should be listed in the application on the left hand side.");
        AddSeparator();
        AddSubtitle("Interface Overview");
        AddFeature("📋", "Servo List", "Shows all detected servos with their current IDs and model numbers");
        AddFeature("🔌", "Port Selection", "Use the port dropdown in the toolbar to select your USB serial adapter");
        AddFeature("🔄", "Refresh Button", "Rescan the bus to detect newly connected servos");
        AddFeature("✏", "Change ID Button", "Select a servo and click this button to assign a new ID");
        AddSeparator();
        AddSubtitle("Changing a Servo ID");
        AddStep(1, "Select the servo from the list on the left");
        AddStep(2, "Click the \"Change Servo ID\" button");
        AddStep(3, "Enter the new ID (1-252)");
        AddStep(4, "Click \"Change ID\" to confirm");
        AddStep(5, "Wait for the operation to complete");
        AddSuccessBox("The servo will restart with the new ID and be removed from the list. Click Refresh to rescan and see the servo with its new ID.");
    }

    private void LoadTroubleshooting()
    {
        AddTitle("Troubleshooting");
        AddSubtitle("⚠ Servo Not Detected");
        AddBullet("Check all cable connections are secure");
        AddBullet("Verify the servo has power");
        AddBullet("Try different baud rates");
        AddBullet("Ensure the serial port is not in use by another application");
        AddWarningBox("Important Note", 
            "Remember that servos can have the same IDs, especially if they are new. If you have two plugged in and one is showing, try plugging them in one at a time and changing one of their IDs first.");
        AddSeparator();
        AddSubtitle("🔌 Connection Issues");
        AddBullet("Try disconnecting and reconnecting the USB cable");
        AddBullet("Restart the application");
        AddBullet("Try a different USB port on your computer");
        AddBullet("Check if the adapter appears in the port list");
        AddBullet("Verify the adapter is receiving power (check LED indicators if present)");
        AddSeparator();
        AddSubtitle("💻 COM Port Not Found");
        AddBullet("Ensure the USB serial adapter drivers are installed");
        AddBullet("Check Windows Device Manager to verify the COM port is recognized");
        AddBullet("Try unplugging and replugging the USB adapter");
    }

    private void LoadTips()
    {
        AddTitle("Tips & Best Practices");
        AddTipCard("⚡", "Power Supply", "#FFE0B2", "#FF9800", 
            "Always power servos from an external power supply matching the voltage of your servos via the adapter. Never rely on USB power alone for servos.");
        AddTipCard("🏷", "Track Your IDs", "#BBDEFB", "#2196F3", 
            "Keep track of your servo IDs somehow, perhaps by labeling them with tape or stickers. This will save you time when troubleshooting or assembling projects.");
        AddTipCard("1️⃣", "One at a Time", "#C8E6C9", "#4CAF50", 
            "Program servos one at a time if servos are likely to have matching IDs. This avoids conflicts and confusion during the ID assignment process.");
        AddTipCard("✓", "Test After Changes", "#E1BEE7", "#9C27B0", 
            "Test servos afterwards to ensure they function as expected. Verify the new ID is working correctly before installing in your project.");
        AddTipCard("💾", "Remember: Changes are Permanent", "#FFE0B2", "#FF9800", 
            "ID changes are stored in the servo's EEPROM and persist across power cycles. Make sure you're assigning the correct ID before confirming.");
    }

    // Helper methods to add UI elements
    private void AddTitle(string text)
    {
        ContentPanel.Children.Add(new TextBlock
        {
            Text = text,
            Style = (Style)FindResource("SectionTitle")
        });
    }

    private void AddSubtitle(string text)
    {
        ContentPanel.Children.Add(new TextBlock
        {
            Text = text,
            Style = (Style)FindResource("SubsectionTitle")
        });
    }

    private void AddBody(string text)
    {
        ContentPanel.Children.Add(new TextBlock
        {
            Text = text,
            Style = (Style)FindResource("BodyText"),
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"))
        });
    }

    private void AddBullet(string text)
    {
        ContentPanel.Children.Add(new TextBlock
        {
            Text = $"• {text}",
            Style = (Style)FindResource("BulletText")
        });
    }

    private void AddStep(int number, string text)
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 4, 0, 4) };
        panel.Children.Add(new TextBlock
        {
            Text = $"{number}.",
            FontWeight = FontWeights.Bold,
            FontSize = 13,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3")),
            Width = 24
        });
        panel.Children.Add(new TextBlock
        {
            Text = text,
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap
        });
        ContentPanel.Children.Add(panel);
    }

    private void AddFeature(string icon, string title, string description)
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 8, 0, 8) };
        panel.Children.Add(new TextBlock
        {
            Text = icon,
            FontSize = 20,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3")),
            Width = 30,
            VerticalAlignment = VerticalAlignment.Top
        });
        var textPanel = new StackPanel();
        textPanel.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 14,
            FontWeight = FontWeights.SemiBold
        });
        textPanel.Children.Add(new TextBlock
        {
            Text = description,
            FontSize = 12,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575")),
            TextWrapping = TextWrapping.Wrap
        });
        panel.Children.Add(textPanel);
        ContentPanel.Children.Add(panel);
    }

    private void AddSeparator()
    {
        ContentPanel.Children.Add(new Separator { Margin = new Thickness(0, 16, 0, 16) });
    }

    private void AddInfoBox(string text)
    {
        var border = new Border
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3F2FD")),
            BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3")),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 12, 0, 12)
        };
        border.Child = new TextBlock
        {
            Text = text,
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap
        };
        ContentPanel.Children.Add(border);
    }

    private void AddSuccessBox(string text)
    {
        var border = new Border
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8F5E9")),
            BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 12, 0, 12)
        };
        border.Child = new TextBlock
        {
            Text = text,
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap
        };
        ContentPanel.Children.Add(border);
    }

    private void AddWarningBox(string title, string text)
    {
        var border = new Border
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF3E0")),
            BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9800")),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 12, 0, 12)
        };
        var panel = new StackPanel();
        panel.Children.Add(new TextBlock
        {
            Text = $"⚠ {title}",
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 8)
        });
        panel.Children.Add(new TextBlock
        {
            Text = text,
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"))
        });
        border.Child = panel;
        ContentPanel.Children.Add(border);
    }

    private void AddTipCard(string icon, string title, string bgColorHex, string borderColorHex, string description)
    {
        var border = new Border
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColorHex)),
            BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(borderColorHex)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 8, 0, 8)
        };
        var panel = new StackPanel { Orientation = Orientation.Horizontal };
        panel.Children.Add(new TextBlock
        {
            Text = icon,
            FontSize = 32,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(borderColorHex)),
            Width = 50,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0, 12, 0),
            VerticalAlignment = VerticalAlignment.Top
        });
        var textPanel = new StackPanel { Width = 700 };
        textPanel.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 6),
            TextWrapping = TextWrapping.Wrap
        });
        textPanel.Children.Add(new TextBlock
        {
            Text = description,
            FontSize = 13,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575")),
            TextWrapping = TextWrapping.Wrap
        });
        panel.Children.Add(textPanel);
        border.Child = panel;
        ContentPanel.Children.Add(border);
    }

    private void AddAdapterCard(string name, string imageName, string url)
    {
        var border = new Border
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5")),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 8, 0, 8),
            MaxWidth = 300
        };
        var panel = new StackPanel();
        
        try
        {
            var image = new Image
            {
                Source = new System.Windows.Media.Imaging.BitmapImage(new Uri($"pack://application:,,,/Resources/Images/{imageName}")),
                Height = 150,
                Stretch = Stretch.Uniform,
                Margin = new Thickness(0, 0, 0, 12)
            };
            panel.Children.Add(image);
        }
        catch { }

        panel.Children.Add(new TextBlock
        {
            Text = name,
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 8)
        });

        var linkText = new TextBlock { TextAlignment = TextAlignment.Center };
        var hyperlink = new Hyperlink(new Run("View Product")) 
        { 
            NavigateUri = new Uri(url),
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3"))
        };
        hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
        linkText.Inlines.Add(hyperlink);
        panel.Children.Add(linkText);

        border.Child = panel;
        ContentPanel.Children.Add(border);
    }

    private void AddAdapterCardToPanel(Panel parentPanel, string name, string imageName, string url)
    {
        var border = new Border
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5")),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 8, 8, 8),
            Width = 300
        };
        var panel = new StackPanel();
        
        try
        {
            var image = new Image
            {
                Source = new System.Windows.Media.Imaging.BitmapImage(new Uri($"pack://application:,,,/Resources/Images/{imageName}")),
                Height = 150,
                Stretch = Stretch.Uniform,
                Margin = new Thickness(0, 0, 0, 12)
            };
            panel.Children.Add(image);
        }
        catch { }

        panel.Children.Add(new TextBlock
        {
            Text = name,
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 8)
        });

        var linkText = new TextBlock { TextAlignment = TextAlignment.Center };
        var hyperlink = new Hyperlink(new Run("View Product")) 
        { 
            NavigateUri = new Uri(url),
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3"))
        };
        hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
        linkText.Inlines.Add(hyperlink);
        panel.Children.Add(linkText);

        border.Child = panel;
        parentPanel.Children.Add(border);
    }

    private void AddServoCard(string name, string imageName, string url)
    {
        var border = new Border
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5")),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(12),
            Margin = new Thickness(0, 8, 0, 8),
            MaxWidth = 400
        };
        var panel = new StackPanel { Orientation = Orientation.Horizontal };
        
        try
        {
            var image = new Image
            {
                Source = new System.Windows.Media.Imaging.BitmapImage(new Uri($"pack://application:,,,/Resources/Images/{imageName}")),
                Width = 80,
                Height = 80,
                Stretch = Stretch.Uniform,
                Margin = new Thickness(0, 0, 12, 0)
            };
            panel.Children.Add(image);
        }
        catch { }

        var textPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        textPanel.Children.Add(new TextBlock
        {
            Text = name,
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 4)
        });

        var linkText = new TextBlock();
        var hyperlink = new Hyperlink(new Run("Product Information")) 
        { 
            NavigateUri = new Uri(url),
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3"))
        };
        hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
        linkText.Inlines.Add(hyperlink);
        textPanel.Children.Add(linkText);

        panel.Children.Add(textPanel);
        border.Child = panel;
        ContentPanel.Children.Add(border);
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }
}
