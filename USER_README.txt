=================================================================
  PROP STRATEGY BUILDER v1.0.0
  MT5 Expert Advisor Strategy Generator for Prop Traders
  Copyright © 2026 FuglyMan TokenArena
=================================================================

QUICK START
-----------
1. Double-click "AKHENS TRADER.exe"
2. App launches immediately - no installation needed!

FIRST TIME RUNNING
------------------
Windows may show this warning:
  "Windows protected your PC"
  "Unknown publisher"

This is NORMAL and SAFE. Here's what to do:
  1. Click "More info"
  2. Click "Run anyway"
  3. App will launch

Why this happens:
  - The app isn't code-signed (requires expensive certificate)
  - Windows shows this for all unverified publishers
  - Your app is completely safe - you created it!

SYSTEM REQUIREMENTS
-------------------
? Windows 10 or Windows 11 (64-bit)
? ~200 MB free disk space
? No other software needed!
? No .NET installation required!
? No internet connection needed!

FEATURES
--------
? 8 Pre-Made Trading Strategies
  - MA Crossover
  - RSI Oversold/Overbought
  - Bollinger Band Bounce
  - MACD Signal
  - Stochastic
  - ATR Breakout
  - CCI Extreme
  - Multi-Confirmation

? Visual Strategy Builder
  - Build strategies without coding
  - 10 technical indicators
  - Multiple timeframes (M1, M5, H1, H4, D1, etc.)
  - Dynamic entry/exit conditions
  - AND/OR logic support

? Prop Firm Compliance
  - FTMO preset
  - FundedNext preset
  - The5%ers preset
  - DNA Funded preset
  - MyForexFunds preset
  - Custom settings

? Code Generation
  - Complete MQL5 Expert Advisor code
  - Prop firm risk management
  - Drawdown monitoring
  - Visible SL/TP enforcement
  - Professional code structure

? Export Options
  - Save .mq5 files (MT5 source code)
  - Save .set files (MT5 parameters)
  - Export complete project folders
  - Save/Load strategy configurations (JSON)

HOW TO USE
----------
1. PRELOADED STRATEGIES TAB
   - Browse 8 ready-made strategies
   - Click LOAD to use any strategy
   - Modify settings as needed

2. CUSTOM BUILDER TAB
   - Create your own strategy from scratch
   - Name your strategy
   - Add entry conditions (when to enter trades)
   - Add exit conditions (when to exit trades)
   - Set risk management (SL, TP, Risk %)
   - Click SAVE CONFIG to save for later
   - Click LOAD CONFIG to load saved strategies

3. PROP FIRM SETTINGS TAB
   - Choose your prop firm (FTMO, FundedNext, etc.)
   - Click APPLY PRESET to load firm's rules
   - Customize daily DD, max DD, max trades
   - Enable/disable drawdown monitoring

4. GENERATE & EXPORT TAB
   - View generated MQL5 code
   - Click COPY CODE to copy to clipboard
   - Click SAVE .MQ5 to save source file
   - Click SAVE .SET to save parameters
   - Click EXPORT ALL to save complete project

INSTALLING YOUR EA IN MT5
--------------------------
After generating your EA:

1. Open MetaTrader 5
2. Press F4 to open MetaEditor
3. File ? Open ? Select your .mq5 file
4. Click Compile (F7)
5. Close MetaEditor
6. In MT5: Navigator (Ctrl+N) ? Expert Advisors
7. Drag your EA onto a chart
8. Load the .set file (if you saved one)
9. Review settings and click OK
10. EA starts trading!

IMPORTANT WARNINGS
------------------
?? ALWAYS test on DEMO account first!
?? Review generated code before live trading
?? Verify prop firm rules match your EA settings
?? Monitor your EA closely, especially during news
?? This tool generates code - YOU are responsible for testing it

TROUBLESHOOTING
---------------
Problem: App won't start
Solution: Try running as administrator (right-click ? Run as administrator)

Problem: Windows blocks the app
Solution: Add to antivirus whitelist or Windows Defender exceptions

Problem: File size is large (~150 MB)
Solution: This is normal - app includes entire .NET runtime for portability

Problem: Save/Load dialogs don't open
Solution: Check file permissions - run as administrator if needed

Problem: Generated code has errors in MT5
Solution: Check your strategy logic - ensure all conditions are valid

SUPPORT & UPDATES
-----------------
Version: 1.0.0
Build Date: 2026-01-31
Developer: FuglyMan TokenArena

For updates and support:
- Check for new versions periodically
- Report bugs or issues to developer
- Provide feedback for improvements

DISCLAIMER
----------
This software is provided "as-is" without warranty of any kind.

Trading involves risk. Past performance does not guarantee future results.
Always test strategies on demo accounts before live trading.

Prop firm rules vary - verify your EA complies with your firm's requirements.
The developer is not responsible for losses, prop firm breaches, or account
terminations resulting from use of generated Expert Advisors.

By using this software, you acknowledge that you understand and accept all
risks associated with algorithmic trading.

LICENSE
-------
Copyright © 2026 FuglyMan TokenArena

This software is licensed for personal and commercial use.
You may generate and distribute Expert Advisors created with this tool.
You may NOT redistribute this software itself without permission.

TECHNICAL DETAILS
-----------------
Framework: .NET 8.0
UI: WPF (Windows Presentation Foundation)
Design: Material Design In XAML Toolkit
Architecture: MVVM (Model-View-ViewModel)
Output: MQL5 (MetaQuotes Language 5)
Target: MetaTrader 5 Platform

=================================================================
  Thank you for using Prop Strategy Builder!
  Build profitable, prop-compliant EAs visually!
=================================================================
