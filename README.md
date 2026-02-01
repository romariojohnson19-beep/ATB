# ?? Prop Strategy Builder

> A Windows desktop application for generating MetaTrader 5 Expert Advisors tailored for proprietary trading firm challenges.

![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue)
![C# 12](https://img.shields.io/badge/C%23-12-green)
![WPF](https://img.shields.io/badge/WPF-Material%20Design-purple)
![License](https://img.shields.io/badge/license-MIT-blue)

## ?? Overview

**Prop Strategy Builder** helps traders create prop-firm-compliant trading strategies for MetaTrader 5 without writing a single line of MQL5 code. The application generates complete Expert Advisor files with built-in drawdown monitoring, risk management, and compliance features required by major prop firms like FTMO, FundedNext, The5%ers, and more.

### ?? Important

This application **DOES NOT**:
- Connect to any trading account or broker
- Store or transmit MT5 login credentials
- Execute trades or test strategies
- Require internet connection

This application **ONLY**:
- Generates MQL5 source code files (.mq5)
- Creates parameter files (.set)
- Exports project folders for manual import to MT5

## ? Features

### ?? Modern UI
- **Material Design** interface with beautiful animations
- **4-tab navigation** system for organized workflow
- **Responsive** and intuitive controls
- **Dark/Light theme** support

### ?? Preloaded Strategies
- MA Crossover Strategy
- RSI Divergence Strategy
- Bollinger Band Breakout
- MACD Signal Strategy
- Simple Trend Follow
- Support & Resistance Bounce
- Ichimoku Cloud Strategy
- Price Action Patterns

### ??? Custom Strategy Builder
- **Entry Conditions**: Multiple indicators (MA, RSI, MACD, Bollinger, Stochastic, etc.)
- **Exit Conditions**: Flexible exit rules with logical operators
- **Risk Management**: Configurable SL, TP, trailing stops, risk per trade
- **Time Filters**: Trading hours and news avoidance
- **Logical Operators**: AND/OR conditions for complex strategies

### ?? Prop Firm Presets
- **FTMO**
- **FundedNext**
- **The5%ers**
- **DNA Funded**
- **MyForexFunds**
- **Custom** (define your own rules)

Each preset auto-configures:
- Daily drawdown limits
- Maximum drawdown limits
- Max concurrent trades
- Compliance requirements

### ?? Export Options
- **Copy to Clipboard**: Quick code copying
- **Save as .MQ5**: Full Expert Advisor source code
- **Save as .SET**: Parameter files for MT5
- **Export Project**: Complete folder with all files

## ?? Getting Started

### Prerequisites
- Windows 10/11
- .NET 8.0 SDK or Runtime
- Visual Studio 2022 (for development)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/prop-strategy-builder.git
cd prop-strategy-builder
```

2. **Restore packages**
```bash
dotnet restore
```

3. **Build the project**
```bash
dotnet build
```

4. **Run the application**
```bash
dotnet run
```

Or simply open in Visual Studio and press **F5**.

## ?? Quick Usage

1. **Choose a Strategy**
   - Navigate to "Preloaded Strategies" tab
   - Select from 8 ready-made strategies
   - Or go to "Custom Builder" to create your own

2. **Configure Risk Management**
   - Set Stop Loss and Take Profit in pips
   - Define risk percentage per trade
   - Enable trailing stop if desired

3. **Select Prop Firm**
   - Go to "Prop Firm Settings" tab
   - Choose your prop firm from dropdown
   - Rules are auto-loaded (daily DD, max DD, etc.)

4. **Generate Code**
   - Navigate to "Generate & Export" tab
   - Preview the generated MQL5 code
   - Click "COPY CODE" or save as .MQ5 file

5. **Import to MT5**
   - Open MT5 MetaEditor
   - Create new EA or open existing
   - Paste the generated code
   - Compile and attach to chart

## ??? Architecture

```
???????????????????????????????????????????????????
?                   Views (XAML)                  ?
?  MainWindow.xaml, StrategyCard.xaml, etc.     ?
???????????????????????????????????????????????????
                      ? ?
                      ? ? Bindings
                      ? ?
???????????????????????????????????????????????????
?               ViewModels (C#)                   ?
?  MainViewModel, CustomBuilderViewModel, etc.   ?
???????????????????????????????????????????????????
                      ? ?
                      ? ? Data
                      ? ?
???????????????????????????????????????????????????
?                 Models (C#)                     ?
?  Strategy, IndicatorCondition, RiskMgmt, etc.  ?
???????????????????????????????????????????????????
                      ? ?
                      ? ? Services
                      ? ?
???????????????????????????????????????????????????
?                Services (C#)                    ?
?  CodeGenerationService, FileExportService, etc. ?
???????????????????????????????????????????????????
```

### MVVM Pattern
- **Models**: Data structures (Strategy, PropFirmPreset, etc.)
- **Views**: XAML UI files with Material Design
- **ViewModels**: Business logic with CommunityToolkit.Mvvm
- **Services**: Code generation, file I/O, prop firm presets

## ?? Technologies

| Technology | Purpose | Version |
|------------|---------|---------|
| .NET | Framework | 8.0 |
| C# | Language | 12 |
| WPF | UI Framework | Built-in |
| Material Design | UI Theme | 5.1.0 |
| CommunityToolkit.Mvvm | MVVM Helpers | 8.3.2 |

## ?? Code Examples

### Creating a Custom Strategy

```csharp
var strategy = new Strategy
{
    Name = "My Custom Strategy",
    Description = "RSI + MA combination",
    EntryConditions = new List<IndicatorCondition>
    {
        new IndicatorCondition
        {
            IndicatorType = "RSI",
            Parameters = new Dictionary<string, object> { { "Period", 14 } },
            Condition = "LessThan",
            Threshold = 30
        }
    },
    RiskSettings = new RiskManagement
    {
        StopLossPips = 50,
        TakeProfitPips = 100,
        RiskPercentPerTrade = 1.0
    }
};
```

### Using Commands in ViewModel

```csharp
[RelayCommand]
private void CopyCode()
{
    Clipboard.SetText(GeneratedCode);
    StatusMessage = "Code copied!";
}
```

## ?? Configuration

### Changing Theme Colors

Edit `App.xaml`:

```xml
<materialDesign:BundledTheme 
    BaseTheme="Light"              <!-- Light or Dark -->
    PrimaryColor="DeepPurple"      <!-- Main color -->
    SecondaryColor="Lime" />       <!-- Accent color -->
```

### Adding Custom Prop Firm Presets

Edit `Services/PropFirmService.cs`:

```csharp
public PropFirmPreset GetPreset(string firmName)
{
    return firmName switch
    {
        "MyPropFirm" => new PropFirmPreset
        {
            FirmName = "MyPropFirm",
            DailyDrawdownPercent = 5.0,
            MaxDrawdownPercent = 10.0,
            // ... other settings
        },
        _ => GetDefaultPreset()
    };
}
```

## ?? Documentation

- **[Quick Start Guide](QUICK_START.md)** - Get up and running fast
- **[Project Status](PROJECT_STATUS.md)** - Current implementation status
- **[Setup Instructions](SETUP_INSTRUCTIONS.md)** - Detailed setup guide
- **[MQL5 Template](Templates/EA_Template.mq5)** - Base EA template

## ?? Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Development Guidelines
- Follow MVVM pattern
- Use CommunityToolkit.Mvvm attributes
- Follow Material Design principles
- Comment complex logic
- Test on Windows 10 and 11

## ?? Known Issues

- [ ] Save .MQ5 functionality (placeholder)
- [ ] Save .SET functionality (placeholder)
- [ ] Export project functionality (placeholder)
- [ ] Custom strategy builder UI (in progress)

## ?? Roadmap

### Phase 1: Foundation ? COMPLETE
- [x] Project structure
- [x] Material Design UI
- [x] MVVM architecture
- [x] Basic navigation
- [x] Sample code preview

### Phase 2: Core Features ?? IN PROGRESS
- [ ] Code generation service
- [ ] File export service
- [ ] Custom strategy builder
- [ ] Indicator selection
- [ ] Condition builder

### Phase 3: Advanced Features ?? PLANNED
- [ ] Strategy validation
- [ ] Backtesting integration
- [ ] Strategy optimization
- [ ] Multiple symbol support
- [ ] Strategy marketplace

### Phase 4: Polish ?? PLANNED
- [ ] Localization
- [ ] Themes customization
- [ ] User preferences
- [ ] Strategy templates
- [ ] Documentation site

## ?? Security & Privacy

- ? **No network connections** - Everything runs locally
- ? **No credential storage** - Never asks for MT5 login
- ? **No telemetry** - No usage tracking
- ? **No ads** - Completely free and clean
- ? **Open source** - Fully auditable code

## ?? Legal & Disclaimer

This software is provided "as is" without warranty of any kind. Use at your own risk.

**IMPORTANT DISCLAIMERS**:
- Not financial advice - For educational purposes only
- No guarantee of profits - Trading involves risk
- Test thoroughly - Always backtest before live trading
- Compliance is your responsibility - Verify with your prop firm
- No affiliation - Not associated with any prop firm or broker

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ????? Author

**Your Name**
- GitHub: [@yourusername](https://github.com/yourusername)
- Email: your.email@example.com

## ?? Acknowledgments

- [MaterialDesignInXamlToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) - Beautiful UI components
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) - MVVM helpers
- [MetaQuotes](https://www.mql5.com/) - MQL5 language and MT5 platform

## ?? Support

- ?? [Report a Bug](https://github.com/yourusername/prop-strategy-builder/issues)
- ?? [Request a Feature](https://github.com/yourusername/prop-strategy-builder/issues)
- ?? [Join Discussions](https://github.com/yourusername/prop-strategy-builder/discussions)
- ?? Email: support@yourdomain.com

---

<p align="center">
  Made with ?? for the trading community
  <br>
  ? Star this repo if you find it useful!
</p>
