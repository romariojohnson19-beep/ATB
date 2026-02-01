# Prop Strategy Builder - Setup Instructions

## ? Completed Steps

1. **Project Structure Created**
   - ? Models/ - Data models for strategies, conditions, risk management
   - ? ViewModels/ - MVVM view models
   - ? Views/ - User controls and views (currently using MainWindow)
   - ? Services/ - Future: Code generation, file export services
   - ? Helpers/ - Future: Utility classes
   - ? Templates/ - Future: MQL5 code templates

2. **Core Files Generated**
   - ? MainWindow.xaml - Complete UI with 4 tabs (Material Design)
   - ? MainWindow.xaml.cs - Code-behind with ViewModel initialization
   - ? App.xaml - Material Design theme configuration
   - ? App.xaml.cs - Application startup logic
   - ? ViewModels/MainViewModel.cs - Main MVVM ViewModel with commands
   - ? Models/Strategy.cs - Core data models

## ?? Required NuGet Packages

You need to install these packages via NuGet Package Manager or Package Manager Console:

### Method 1: Package Manager Console
Open Tools ? NuGet Package Manager ? Package Manager Console and run:

```powershell
Install-Package MaterialDesignThemes -Version 5.1.0
Install-Package MaterialDesignColors -Version 3.1.0
Install-Package CommunityToolkit.Mvvm -Version 8.3.2
```

### Method 2: .NET CLI (from project directory)
```bash
dotnet add package MaterialDesignThemes --version 5.1.0
dotnet add package MaterialDesignColors --version 3.1.0
dotnet add package CommunityToolkit.Mvvm --version 8.3.2
```

### Method 3: Edit .csproj file directly
Add this inside `<ItemGroup>`:

```xml
<PackageReference Include="MaterialDesignThemes" Version="5.1.0" />
<PackageReference Include="MaterialDesignColors" Version="3.1.0" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.3.2" />
```

## ?? Current UI Features

### Tab 1: Preloaded Strategies
- Displays 8 preloaded strategy cards
- Load and Generate buttons for each strategy
- Material Design cards with elevation

### Tab 2: Custom Strategy Builder
- Entry Conditions section
- Exit Conditions section
- Risk Management section with SL/TP inputs
- Trailing stop checkbox

### Tab 3: Prop Firm Settings
- Dropdown for selecting prop firms (FTMO, FundedNext, etc.)
- Daily/Max drawdown inputs
- Max open trades and magic number
- Compliance checkboxes (visible SL/TP, drawdown monitoring, news filter)

### Tab 4: Generate & Export
- Code preview area with Consolas font
- Sample MQL5 code displayed
- Four action buttons:
  - Copy to Clipboard (working)
  - Save as .MQ5 (placeholder)
  - Save as .SET (placeholder)
  - Export Project (placeholder)

## ?? ViewModel Features

**MainViewModel.cs** includes:
- `PreloadedStrategies` - ObservableCollection of strategy names
- `GeneratedCode` - String for MQL5 code preview
- `StatusMessage` - Footer status updates
- Commands: `CopyCodeCommand`, `SaveMq5Command`, `SaveSetCommand`, `ExportProjectCommand`

## ?? Next Development Steps

1. **Create Service Layer**
   - `Services/CodeGenerationService.cs` - Generate MQL5 code from Strategy model
   - `Services/FileExportService.cs` - Handle .mq5, .set, and project exports
   - `Services/PropFirmService.cs` - Manage prop firm presets

2. **Expand ViewModels**
   - `ViewModels/PreloadedStrategiesViewModel.cs` - Manage preloaded strategies tab
   - `ViewModels/CustomBuilderViewModel.cs` - Manage custom strategy builder
   - `ViewModels/PropFirmSettingsViewModel.cs` - Manage prop firm settings

3. **Create Detailed Views**
   - `Views/StrategyCard.xaml` - UserControl for strategy cards
   - `Views/IndicatorConditionControl.xaml` - UserControl for adding conditions
   - `Views/RiskManagementControl.xaml` - UserControl for risk settings

4. **Implement Code Generation**
   - MQL5 template files in `Templates/` folder
   - Parameter substitution logic
   - .SET file generation

5. **Add Validation**
   - Input validation for numeric fields
   - Strategy validation before generation
   - Error messages and warnings

6. **Testing & Polish**
   - Test all commands
   - Add tooltips and help text
   - Implement About dialog
   - Add app icon

## ?? Running the Application

After installing the NuGet packages:

1. Rebuild the solution (Ctrl+Shift+B)
2. Run the application (F5)
3. You should see the Material Design UI with all four tabs functional

## ?? Key Technologies Used

- **.NET 8.0** - Latest LTS version
- **C# 12** - Modern language features
- **WPF** - Windows Presentation Foundation
- **MVVM Pattern** - Clean separation of concerns
- **CommunityToolkit.Mvvm** - Source generators for MVVM boilerplate
- **Material Design** - Beautiful, modern UI components

## ?? Architecture Highlights

- **MVVM Pattern**: Strict separation between UI (Views), logic (ViewModels), and data (Models)
- **ObservableProperty**: Automatic INotifyPropertyChanged implementation
- **RelayCommand**: Type-safe command binding
- **Material Design**: Consistent, professional UI with ColorZone, Cards, Icons

## ?? Important Notes

- The app does NOT connect to MT5 - it only generates code
- No credentials are stored or transmitted
- All processing happens locally
- Generated code includes prop-compliant features (visible SL/TP, drawdown monitoring)

---

**Status**: Foundation complete ?  
**Next**: Install NuGet packages and test the application
