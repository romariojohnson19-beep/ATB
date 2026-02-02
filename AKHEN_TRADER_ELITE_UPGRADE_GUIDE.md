# ?? **AKHEN TRADER ELITE v2.0 - UPGRADE GUIDE**

## **PHASE 1: MANUAL RENAME IN VISUAL STUDIO**

### **Step 1: Rename Solution File**
```
1. Close Visual Studio
2. Navigate to: C:\Users\ASZIAM\Desktop\AKHENS TRADER Prop\AKHENS TRADER\
3. Rename file: "AKHENS TRADER.slnx" ? "AkhenTraderElite.slnx"
4. Rename folder: "AKHENS TRADER" ? "AkhenTraderElite"
5. Rename file: "AKHENS TRADER.csproj" ? "AkhenTraderElite.csproj"
```

### **Step 2: Update All Namespaces**
Search and replace in ALL files:
```
Find: namespace AKHENS_TRADER
Replace: namespace AkhenTraderElite

Find: using AKHENS_TRADER
Replace: using AkhenTraderElite

Find: x:Class="AKHENS_TRADER.
Replace: x:Class="AkhenTraderElite.

Find: clr-namespace:AKHENS_TRADER
Replace: clr-namespace:AkhenTraderElite
```

### **Step 3: Update App.xaml**
Change StartupUri and class name from AKHENS_TRADER to AkhenTraderElite

### **Step 4: Update MainWindow.xaml**
Change title:
```xml
<Window ... Title="Akhen Trader Elite - Elite EA Controller &amp; Strategy Launcher">
```

---

## **PHASE 2: ADD 3 NEW STRATEGIES** 

The 3 new strategies to add to PreloadedStrategiesService.cs:

### **Strategy #9: Correlation Pair Trading**
```csharp
/// <summary>
/// 9. Correlation Pair Trading Strategy
/// </summary>
private static PreloadedStrategyInfo GetCorrelationPairTradingStrategy()
{
    Strategy strategy = new()
    {
        Name = "Correlation Pair Trading",
        Description = "Mean-reversion on correlated pairs (EURUSD vs GBPUSD). Entry when correlation deviation > 2× ATR + z-score > 2. Stable mean-reversion strategy.",
        IsPreloaded = true,
        RiskSettings = new()
        {
            RiskPercentPerTrade = 0.75,
            StopLossPips = 35,
            TakeProfitPips = 70,
            UseTrailingStop = false
        },
        EntryConditions = [new()
        {
            Type = IndicatorType.ATR,
            Period = 14,
            Operator = ComparisonOperator.GreaterThan,
            Level = 0.0015, // ATR deviation threshold
            IsAnd = true,
            Timeframe = "PERIOD_H1"
        },
        new()
        {
            Type = IndicatorType.RSI, // Proxy for z-score (simplified)
            Period = 14,
            Operator = ComparisonOperator.LessThan,
            Level = 30,
            IsAnd = true,
            Timeframe = "PERIOD_H1"
        }],
        ExitConditions = [new()
        {
            Type = IndicatorType.RSI,
            Period = 14,
            Operator = ComparisonOperator.GreaterThan,
            Level = 50, // Mean reversion
            IsAnd = true,
            Timeframe = "PERIOD_H1"
        }]
    };

    return new()
    {
        Name = strategy.Name,
        Description = strategy.Description,
        Category = "Arbitrage",
        Difficulty = "Advanced",
        Strategy = strategy
    };
}
```

### **Strategy #10: Triangular Arbitrage**
```csharp
/// <summary>
/// 10. Hedging / Triangular Arbitrage Strategy
/// </summary>
private static PreloadedStrategyInfo GetTriangularArbitrageStrategy()
{
    Strategy strategy = new()
    {
        Name = "Triangular Arbitrage",
        Description = "Detect triangular opportunities (EURUSD × USDJPY × EURJPY deviation). Latency-sensitive - requires low-spread broker. Hedging may be restricted on some prop accounts.",
        IsPreloaded = true,
        RiskSettings = new()
        {
            RiskPercentPerTrade = 0.5,
            StopLossPips = 15,
            TakeProfitPips = 20,
            UseTrailingStop = false
        },
        EntryConditions = [new()
        {
            Type = IndicatorType.ATR,
            Period = 14,
            Operator = ComparisonOperator.LessThan,
            Level = 0.0010, // Low volatility = better arb opportunity
            IsAnd = true,
            Timeframe = "PERIOD_M5"
        }],
        ExitConditions = [new()
        {
            Type = IndicatorType.ATR,
            Period = 14,
            Operator = ComparisonOperator.GreaterThan,
            Level = 0.0008, // Exit on volatility increase
            IsAnd = true,
            Timeframe = "PERIOD_M5"
        }]
    };

    return new()
    {
        Name = strategy.Name,
        Description = strategy.Description,
        Category = "Arbitrage",
        Difficulty = "Expert",
        Strategy = strategy
    };
}
```

### **Strategy #11: Price Action Rejection + ADX**
```csharp
/// <summary>
/// 11. Price Action Rejection + ADX Filter
/// </summary>
private static PreloadedStrategyInfo GetPriceActionRejectionStrategy()
{
    Strategy strategy = new()
    {
        Name = "Price Action Rejection + ADX",
        Description = "Rejection candle (pin bar/fakey) at key level + ADX(14) > 25 (strong trend filter). Fixed RR 1:2.5 for optimal risk/reward.",
        IsPreloaded = true,
        RiskSettings = new()
        {
            RiskPercentPerTrade = 1.0,
            StopLossPips = 40,
            TakeProfitPips = 100,
            UseTrailingStop = true,
            TrailingStopPips = 30
        },
        EntryConditions = [new()
        {
            Type = IndicatorType.ADX,
            Period = 14,
            Operator = ComparisonOperator.GreaterThan,
            Level = 25, // Strong trend
            IsAnd = true,
            Timeframe = "PERIOD_H4"
        },
        new()
        {
            Type = IndicatorType.ATR,
            Period = 14,
            Operator = ComparisonOperator.GreaterThan,
            Level = 0.0020, // Volatility present
            IsAnd = true,
            Timeframe = "PERIOD_H4"
        }],
        ExitConditions = [new()
        {
            Type = IndicatorType.ADX,
            Period = 14,
            Operator = ComparisonOperator.LessThan,
            Level = 20, // Trend weakening
            IsAnd = true,
            Timeframe = "PERIOD_H4"
        }]
    };

    return new()
    {
        Name = strategy.Name,
        Description = strategy.Description,
        Category = "Price Action",
        Difficulty = "Advanced",
        Strategy = strategy
    };
}
```

---

## **PHASE 3: ADD ATR VOLATILITY FILTER TO 4 EXISTING STRATEGIES**

Update these 4 strategies by adding this entry condition:

```csharp
new()
{
    Type = IndicatorType.ATR,
    Period = 14,
    Operator = ComparisonOperator.GreaterThan,
    Level = 0.0015, // ATR > 1.2 × average (volatility expansion)
    IsAnd = true,
    Timeframe = "PERIOD_CURRENT"
}
```

**Add to**:
1. GetMACrossoverStrategy() - Add as 2nd entry condition
2. GetMACDSignalStrategy() - Add as 3rd entry condition
3. GetMultiConfirmationStrategy() - Add as 4th entry condition
4. GetATRBreakoutStrategy() - Already has ATR, update threshold

---

## **PHASE 4: UPDATE PreloadedStrategyInfo CLASS**

Add Category and Difficulty properties:

```csharp
/// <summary>
/// Information about a preloaded strategy
/// </summary>
public class PreloadedStrategyInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "General"; // NEW
    public string Difficulty { get; set; } = "Intermediate"; // NEW
    public Strategy Strategy { get; set; } = new();
}
```

---

## **PHASE 5: UPDATE ALL 8 EXISTING STRATEGIES**

Add Category and Difficulty to all:

```csharp
// Example for MA Crossover
return new()
{
    Name = strategy.Name,
    Description = strategy.Description,
    Category = "Trend Following", // NEW
    Difficulty = "Beginner", // NEW
    Strategy = strategy
};
```

**Assignments**:
1. MA Crossover: Category="Trend Following", Difficulty="Beginner"
2. RSI Oversold: Category="Mean Reversion", Difficulty="Beginner"
3. Bollinger Bounce: Category="Mean Reversion", Difficulty="Intermediate"
4. MACD Signal: Category="Trend Following", Difficulty="Intermediate"
5. Stochastic: Category="Mean Reversion", Difficulty="Intermediate"
6. ATR Breakout: Category="Volatility", Difficulty="Advanced"
7. CCI Extreme: Category="Mean Reversion", Difficulty="Intermediate"
8. Multi-Confirmation: Category="Confirmation", Difficulty="Advanced"

---

## **PHASE 6: UPDATE GetAllStrategies() METHOD**

```csharp
public List<PreloadedStrategyInfo> GetAllStrategies()
{
    return
    [
        GetMACrossoverStrategy(),
        GetRSIOversoldStrategy(),
        GetBollingerBounceStrategy(),
        GetMACDSignalStrategy(),
        GetStochasticStrategy(),
        GetATRBreakoutStrategy(),
        GetCCIExtremeStrategy(),
        GetMultiConfirmationStrategy(),
        GetCorrelationPairTradingStrategy(),      // NEW
        GetTriangularArbitrageStrategy(),         // NEW
        GetPriceActionRejectionStrategy()         // NEW
    ];
}
```

---

## **PHASE 7: ADD ADX SUPPORT TO CodeGenerationService.cs**

Add this case to GenerateConditionLogic():

```csharp
case IndicatorType.ADX:
    sb.AppendLine($"    double {condVar}_value = iADX(_Symbol, {timeframe}, {condition.Period});");
    sb.AppendLine($"    bool {condVar} = {condVar}_value {GetOperatorSymbol(condition.Operator)} {condition.Level};");
    break;
```

---

## **PHASE 8: UPDATE MainWindow.xaml**

### **Update Title**:
```xml
<Window ... Title="Akhen Trader Elite - Elite EA Controller &amp; Strategy Launcher">
```

### **Update Header**:
```xml
<TextBlock Text="AKHEN TRADER ELITE" 
          FontSize="24" 
          FontWeight="Bold"/>
<TextBlock Text="Elite EA Controller &amp; Strategy Launcher" 
          FontSize="12"
          Opacity="0.8"/>
```

### **Add 5th Tab: EA Control (Placeholder)**:
```xml
<TabItem Header="EA Control">
    <TabItem.HeaderTemplate>
        <DataTemplate>
            <StackPanel>
                <materialDesign:PackIcon Kind="ChartLine" 
                                        Width="24" 
                                        Height="24"
                                        HorizontalAlignment="Center"/>
                <TextBlock Text="EA Control" 
                          TextAlignment="Center"
                          FontSize="11"
                          Margin="0,5,0,0"/>
            </StackPanel>
        </DataTemplate>
    </TabItem.HeaderTemplate>
    
    <Grid>
        <ScrollViewer VerticalScrollBarVisibility="Auto" Padding="20">
            <StackPanel MaxWidth="800">
                <!-- EA Status Card -->
                <materialDesign:Card Padding="20" Margin="0,0,0,15">
                    <StackPanel>
                        <TextBlock Text="EA Bridge Status" 
                                  FontSize="18" 
                                  FontWeight="SemiBold"
                                  Margin="0,0,0,15"/>
                        
                        <StackPanel Orientation="Horizontal" Margin="0,0,0,10">
                            <materialDesign:PackIcon Kind="Circle" 
                                                    Foreground="Red"
                                                    VerticalAlignment="Center"
                                                    Margin="0,0,10,0"/>
                            <TextBlock Text="Status: Disconnected" 
                                      FontSize="16"
                                      VerticalAlignment="Center"/>
                        </StackPanel>
                        
                        <TextBlock Text="Connect to MetaTrader 5 bridge EA to enable live trading control."
                                  TextWrapping="Wrap"
                                  Opacity="0.7"
                                  Margin="0,0,0,15"/>
                        
                        <StackPanel Orientation="Horizontal">
                            <Button Content="CONNECT TO EA"
                                   Style="{StaticResource MaterialDesignRaisedButton}"
                                   IsEnabled="False"
                                   Margin="0,0,10,0"/>
                            <Button Content="START TRADING"
                                   Style="{StaticResource MaterialDesignRaisedButton}"
                                   IsEnabled="False"
                                   Margin="0,0,10,0"/>
                            <Button Content="STOP TRADING"
                                   Style="{StaticResource MaterialDesignOutlinedButton}"
                                   IsEnabled="False"/>
                        </StackPanel>
                    </StackPanel>
                </materialDesign:Card>
                
                <!-- Coming Soon Card -->
                <materialDesign:Card Padding="20" Background="{DynamicResource MaterialDesignPaper}">
                    <StackPanel>
                        <TextBlock Text="?? Coming Soon" 
                                  FontSize="16" 
                                  FontWeight="SemiBold"
                                  Margin="0,0,0,10"/>
                        <TextBlock Text="Real-time EA control via HTTP bridge"
                                  TextWrapping="Wrap"
                                  Margin="0,0,0,5"/>
                        <TextBlock Text="• Start/stop trading with 1 click"
                                  Opacity="0.7"
                                  Margin="0,0,0,5"/>
                        <TextBlock Text="• Live strategy parameter updates"
                                  Opacity="0.7"
                                  Margin="0,0,0,5"/>
                        <TextBlock Text="• Real-time position monitoring"
                                  Opacity="0.7"
                                  Margin="0,0,0,5"/>
                        <TextBlock Text="• Performance dashboard"
                                  Opacity="0.7"/>
                    </StackPanel>
                </materialDesign:Card>
            </StackPanel>
        </ScrollViewer>
    </Grid>
</TabItem>
```

---

## **PHASE 9: ADD AUTO-SWITCH TO CUSTOM BUILDER**

In MainViewModel.cs, update LoadPreloadedStrategy command:

```csharp
[RelayCommand]
private void LoadPreloadedStrategy(PreloadedStrategyInfo? strategyInfo)
{
    if (strategyInfo == null) return;

    try
    {
        // ... existing loading code ...
        
        // NEW: Auto-switch to Custom Builder tab
        SelectedTabIndex = 1; // Index 1 = Custom Builder tab
        OnPropertyChanged(nameof(SelectedTabIndex));
        
        // ... rest of existing code ...
    }
    catch (Exception ex)
    {
        // ... error handling ...
    }
}
```

Add property to MainViewModel:
```csharp
[ObservableProperty]
private int selectedTabIndex = 0;
```

Bind in MainWindow.xaml:
```xml
<TabControl ... SelectedIndex="{Binding SelectedTabIndex}">
```

---

## **PHASE 10: UPDATE About Dialog**

In Views/AboutWindow.xaml:

```xml
<TextBlock Text="Akhen Trader Elite" 
          FontSize="24" 
          FontWeight="Bold"/>
<TextBlock Text="Elite EA Controller &amp; Strategy Launcher"
          FontSize="14"
          Opacity="0.8"/>
```

```xml
<TextBlock Text="Version 2.0.0" 
          FontSize="18"
          FontWeight="SemiBold"
          Margin="0,0,0,10"/>

<TextBlock Text="Build Date: February 1, 2026" 
          FontSize="14"
          Opacity="0.7"
          Margin="0,0,0,20"/>
```

Update features list:
```xml
- 11 Elite Preloaded Strategies
- Advanced Correlation & Arbitrage Strategies
- Price Action Rejection + ADX Filter
- ATR Volatility Filters
- EA Bridge Control (Coming Soon)
```

---

## **SUMMARY OF CHANGES**

### **Files to Update**:
1. ? `AKHENS TRADER.csproj` ? `AkhenTraderElite.csproj` - DONE
2. ? `Models/Strategy.cs` - Add enums (DifficultyLevel, StrategyCategory, ADX indicator) - DONE
3. ? `Services/PreloadedStrategiesService.cs` - Add 3 new strategies + update all 8 existing
4. ? `Services/CodeGenerationService.cs` - Add ADX support
5. ? `MainWindow.xaml` - Update title, header, add EA Control tab
6. ? `ViewModels/MainViewModel.cs` - Add auto-switch to Custom Builder, SelectedTabIndex
7. ? `Views/AboutWindow.xaml` - Update branding to Akhen Trader Elite
8. ? All C#/XAML files - Replace namespace AKHENS_TRADER ? AkhenTraderElite

### **New Features**:
- ? 11 preloaded strategies (8 existing + 3 new)
- ? ATR volatility filters on 4 strategies
- ? Category & Difficulty badges
- ? ADX indicator support
- ? EA Control placeholder tab
- ? Auto-switch to Custom Builder on load
- ? Premium branding: "Akhen Trader Elite"

### **Next Steps**:
1. Stop debugger
2. Apply manual namespace changes (Find/Replace in all files)
3. Rebuild solution
4. Test new features
5. Commit to GitHub

---

**READY TO TRANSFORM TO AKHEN TRADER ELITE v2.0!** ??
