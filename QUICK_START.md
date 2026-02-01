# ?? Quick Start Guide - Prop Strategy Builder

## ? Running the Application

### Visual Studio 2022
1. Press **F5** or click the green "Start" button
2. The application will launch showing the main window with 4 tabs

### Expected Result
You should see a beautiful Material Design window with:
- Purple header bar with "Prop Strategy Builder" title
- 4 navigation tabs on the left
- Footer status bar showing "Ready"

## ?? Using the Application (Current Features)

### 1. Preloaded Strategies Tab
- **View** the 8 prebuilt strategy names
- Each strategy shows as a card
- Buttons are placeholders (will be implemented in next phase)

### 2. Custom Strategy Builder Tab
- **Three main sections**:
  - Entry Conditions (with Add Condition button)
  - Exit Conditions (with Add Condition button)
  - Risk Management (with input fields)
- **Fill in** the Risk Management fields:
  - Stop Loss in pips
  - Take Profit in pips
  - Risk % per trade
  - Check/uncheck trailing stop

### 3. Prop Firm Settings Tab
- **Select** a prop firm from dropdown (FTMO, FundedNext, etc.)
- **Adjust** the drawdown limits:
  - Daily Drawdown %: Default 5.0
  - Max Drawdown %: Default 10.0
- **Set** trading parameters:
  - Max Open Trades: Default 3
  - Magic Number: Default 123456
- **Toggle** compliance options:
  - ? Enforce visible SL/TP (recommended ON)
  - ? Enable drawdown monitoring (recommended ON)
  - ? News filter

### 4. Generate & Export Tab
- **View** the sample MQL5 code in the preview area
- **Click "COPY CODE"** button ? Code is copied to clipboard
- **Test it**: Paste in Notepad to verify it works!
- Other buttons (Save .MQ5, Save .SET, Export All) are placeholders

## ?? What to Test

### ? Working Features
1. **Tab Navigation**
   - Click each tab icon to switch views
   - All tabs should load instantly

2. **Strategy List**
   - Scroll through 8 preloaded strategies
   - Each shows in a card with Load/Generate buttons

3. **Text Inputs**
   - Type in any text box (SL, TP, Risk %, etc.)
   - Values are stored in the ViewModel

4. **Dropdowns**
   - Click the Prop Firm dropdown
   - Select different firms (FTMO, FundedNext, etc.)

5. **Checkboxes**
   - Toggle trailing stop
   - Toggle compliance checkboxes
   - All maintain their state

6. **Copy to Clipboard** ?
   - Go to Generate & Export tab
   - Click "COPY CODE" button
   - Open Notepad and paste (Ctrl+V)
   - You should see the MQL5 code!

7. **Status Messages**
   - Watch the footer when you click "COPY CODE"
   - Status changes to "Code copied to clipboard successfully!"

## ?? Troubleshooting

### Application won't start
```bash
# In Package Manager Console
Update-Package -reinstall
dotnet clean
dotnet build
```

### Material Design not showing
- Check that packages are restored
- Rebuild the solution (Ctrl+Shift+B)
- Close and reopen Visual Studio

### Binding errors
- Check Output window for XAML binding warnings
- DataContext is set in MainWindow.xaml.cs constructor

## ?? File Structure at a Glance

```
Your Project/
??? Models/
?   ??? Strategy.cs ? Data classes
??? ViewModels/
?   ??? MainViewModel.cs ? MVVM logic (currently active)
??? Templates/
?   ??? EA_Template.mq5 ? MQL5 template for generation
??? MainWindow.xaml ? Main UI
??? App.xaml ? Theme setup
??? *.csproj ? Includes NuGet packages
```

## ?? Customizing the Theme

Want to change colors? Edit `App.xaml`:

```xml
<materialDesign:BundledTheme BaseTheme="Light"  ? Change to "Dark"
                           PrimaryColor="DeepPurple"  ? Try: "Blue", "Teal", "Red"
                           SecondaryColor="Lime" />   ? Try: "Orange", "Pink"
```

Available colors:
- Red, Pink, Purple, DeepPurple, Indigo, Blue
- LightBlue, Cyan, Teal, Green, LightGreen, Lime
- Yellow, Amber, Orange, DeepOrange

## ?? Testing Checklist

- [ ] Application launches without errors
- [ ] All 4 tabs are visible and clickable
- [ ] Header shows "Prop Strategy Builder" with icon
- [ ] Footer shows status messages
- [ ] Preloaded Strategies tab shows 8 strategy cards
- [ ] Custom Builder tab shows 3 sections
- [ ] Prop Firm Settings dropdown works
- [ ] Generate & Export shows code preview
- [ ] "COPY CODE" button copies to clipboard
- [ ] Status message updates when copying
- [ ] Window can be resized
- [ ] Window can be minimized/maximized

## ?? Learning the Code

### Understanding MVVM

**MainWindow.xaml** (View)
```xml
<TextBlock Text="{Binding StatusMessage}" />
```
?? **Binds to** ??

**MainViewModel.cs** (ViewModel)
```csharp
[ObservableProperty]
private string statusMessage;
```

### Understanding Commands

**XAML**
```xml
<Button Command="{Binding CopyCodeCommand}" />
```
?? **Executes** ??

**C#**
```csharp
[RelayCommand]
private void CopyCode() { ... }
```

The `[RelayCommand]` attribute auto-generates `CopyCodeCommand` for you!

## ?? Next Steps

Once you've tested the current features:

1. **Implement Code Generation Service**
   - Create `Services/CodeGenerationService.cs`
   - Use the template in `Templates/EA_Template.mq5`
   - Replace placeholders with actual strategy data

2. **Implement File Export**
   - Add file save dialogs
   - Generate .mq5 files
   - Generate .set parameter files

3. **Build Custom Strategy Logic**
   - Create indicator selection controls
   - Build condition builder UI
   - Implement strategy validation

4. **Add Preloaded Strategy Details**
   - Load actual strategy configurations
   - Display strategy descriptions
   - Implement "Load" button functionality

## ?? Tips

- **Explore the code**: Everything is well-commented
- **Use breakpoints**: Set breakpoints in `MainViewModel.cs` to see data flow
- **Watch the Output window**: See debug messages and binding information
- **Read the template**: `Templates/EA_Template.mq5` shows the structure you'll generate

## ?? Resources

- **Material Design**: https://materialdesigninxaml.net/
- **CommunityToolkit.Mvvm**: https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/
- **MQL5 Reference**: https://www.mql5.com/en/docs

---

## ?? You're Ready!

Press **F5** and start exploring your new application!

If you see the Material Design UI with all 4 tabs working, **you're all set!** ??

**Questions?** Check the `PROJECT_STATUS.md` file for detailed feature documentation.
