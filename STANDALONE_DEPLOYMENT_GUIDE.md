# ?? STANDALONE EXECUTABLE - DEPLOYMENT GUIDE

## ? PROJECT CONFIGURED FOR STANDALONE DISTRIBUTION!

Your Prop Strategy Builder is now ready to be published as a standalone Windows executable that **doesn't require Visual Studio or .NET runtime installation**.

---

## ?? WHAT'S BEEN CONFIGURED

### **Updated `AKHENS TRADER.csproj`**:

```xml
<!-- Publishing Configuration -->
<RuntimeIdentifier>win-x64</RuntimeIdentifier>           <!-- Target 64-bit Windows -->
<SelfContained>true</SelfContained>                      <!-- Include .NET runtime -->
<PublishSingleFile>true</PublishSingleFile>              <!-- Single .exe output -->
<PublishTrimmed>false</PublishTrimmed>                   <!-- Keep all assemblies (WPF safe) -->
<IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>

<!-- Assembly Information -->
<AssemblyTitle>Prop Strategy Builder</AssemblyTitle>
<Version>1.0.0</Version>
<Description>Visual MT5 EA Strategy Generator for Prop Traders</Description>
```

### **Key Settings Explained**:

| Setting | Value | Why |
|---------|-------|-----|
| **SelfContained** | `true` | Includes .NET 8 runtime (no install needed) |
| **PublishSingleFile** | `true` | Single .exe file (easier distribution) |
| **PublishTrimmed** | `false` | Keeps all assemblies (WPF + MaterialDesign safe) |
| **RuntimeIdentifier** | `win-x64` | 64-bit Windows (most common) |

---

## ?? METHOD 1: VISUAL STUDIO PUBLISH (RECOMMENDED)

### **Step-by-Step Instructions:**

#### **1. Open Publish Dialog**
1. **Right-click** on `AKHENS TRADER` project in Solution Explorer
2. Select **"Publish..."**

#### **2. Create Publish Profile** (First time only)
1. Click **"New"** or **"Add a publish profile"**
2. Choose **"Folder"** as the target
3. Click **"Next"**

#### **3. Configure Publish Settings**
1. **Folder location**: Leave default or choose custom path
   - Default: `bin\Release\net8.0-windows\win-x64\publish\`
2. Click **"Finish"**

#### **4. Configure Profile Settings**
1. Click **"Show all settings"** or **"More options"**
2. Verify settings:
   - **Configuration**: `Release`
   - **Target framework**: `net8.0-windows`
   - **Deployment mode**: `Self-contained` ?
   - **Target runtime**: `win-x64` ?
   - **Produce single file**: `Checked` ?
3. Click **"Save"**

#### **5. Publish**
1. Click the big **"Publish"** button
2. Wait for build and publish (30-60 seconds)
3. ? Done! Output folder opens automatically

#### **6. Output Location**
```
?? bin\Release\net8.0-windows\win-x64\publish\
   ??? ?? AKHENS TRADER.exe  (Single executable ~100-150 MB)
```

---

## ? METHOD 2: COMMAND LINE PUBLISH (ADVANCED)

### **Option A: Single Command (Recommended)**

Open **Developer Command Prompt** or **PowerShell** in project directory:

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```

**Output**: `bin\Release\net8.0-windows\win-x64\publish\AKHENS TRADER.exe`

### **Option B: With Specific Output Path**

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o "C:\MyPublish"
```

**Output**: `C:\MyPublish\AKHENS TRADER.exe`

### **Option C: 32-bit Windows (if needed)**

```bash
dotnet publish -c Release -r win-x86 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```

---

## ?? TESTING THE STANDALONE EXECUTABLE

### **1. Find the Published .exe**
Navigate to:
```
bin\Release\net8.0-windows\win-x64\publish\
```

### **2. Copy to Test Location**
Copy the **entire publish folder** to a different location (e.g., Desktop, USB drive)

### **3. Run Without Visual Studio**
1. **Close Visual Studio** completely
2. **Double-click** `AKHENS TRADER.exe`
3. ? App should launch immediately (no dependencies needed)

### **4. Test All Features**
- ? Load preloaded strategies
- ? Build custom strategies
- ? Generate MQL5 code
- ? Save/Load configurations (JSON)
- ? Export .mq5 files
- ? Export project folders

### **5. Test on Another Computer** (Optional)
- Copy the .exe to a different Windows PC (without .NET or Visual Studio)
- ? Should run perfectly!

---

## ?? DISTRIBUTION OPTIONS

### **Option 1: Single .exe File**
**Pros**:
- ? Easiest to distribute
- ? Single file to email/upload
- ? ~100-150 MB in size

**How**:
1. Publish as shown above
2. Share just `AKHENS TRADER.exe`
3. Users double-click to run

### **Option 2: ZIP Archive**
**Pros**:
- ? Professional distribution
- ? Can include README, docs
- ? Smaller download (compressed)

**How**:
1. Create folder: `PropStrategyBuilder_v1.0`
2. Copy `AKHENS TRADER.exe`
3. Add `README.txt` with instructions
4. Add any documentation
5. Zip the folder ? `PropStrategyBuilder_v1.0.zip`
6. Distribute the ZIP

### **Option 3: Installer (Future)**
**For professional distribution**:
- Use **Inno Setup** or **WiX Toolset**
- Creates Windows installer (.msi or setup.exe)
- Adds Start Menu shortcuts
- Handles uninstall
- More advanced (not needed for v1.0)

---

## ?? IMPORTANT NOTES

### **File Size**
- **Single .exe**: ~100-150 MB
- **Why so large?** Includes entire .NET 8 runtime + MaterialDesign themes
- **Normal**: Self-contained apps are always large
- **Trade-off**: Size vs. no installation required

### **Windows SmartScreen Warning**
**First run on new PC might show**:
```
"Windows protected your PC"
"Unknown publisher"
```

**How to bypass**:
1. Click **"More info"**
2. Click **"Run anyway"**

**Why this happens**:
- Your .exe isn't code-signed
- Normal for unverified publishers
- **To fix**: Purchase code signing certificate ($300+/year)
- **For personal use**: Just ignore the warning

### **Antivirus False Positives**
Some antivirus software may flag self-contained .NET apps:
- **Reason**: Packed executable + network code (file dialogs, etc.)
- **Solution**: Add to antivirus whitelist
- **Not a real virus**: Just how self-contained apps work

### **MaterialDesign Themes**
- ? Included automatically
- ? No extra files needed
- ? All themes and icons embedded

---

## ??? TROUBLESHOOTING

### **Problem**: "App won't start on another PC"
**Solution**:
- Ensure Windows 10/11 (64-bit)
- Ensure .exe wasn't corrupted during transfer
- Try running as administrator

### **Problem**: "File is too large (>200 MB)"
**Solution**:
- Normal for self-contained WPF apps
- Alternative: Framework-dependent publish (requires .NET 8 installation)

### **Problem**: "Publish fails with errors"
**Solution**:
1. Clean solution (`Build ? Clean Solution`)
2. Rebuild (`Build ? Rebuild Solution`)
3. Try publish again

### **Problem**: "Missing DLLs on another PC"
**Solution**:
- Ensure `SelfContained=true` in .csproj
- Republish
- Copy **entire publish folder**, not just .exe

---

## ?? PUBLISHING CHECKLIST

Before distributing to users:

- [ ] ? Published in **Release** mode (not Debug)
- [ ] ? Tested .exe on **different computer** without Visual Studio
- [ ] ? Tested all major features (generate, save, export)
- [ ] ? Added README or instructions for users
- [ ] ? Noted file size (~100-150 MB) in distribution notes
- [ ] ? Warned users about SmartScreen warning (first run)
- [ ] ? Zipped for easy distribution (optional)

---

## ?? QUICK START FOR END USERS

### **User Instructions** (include in README.txt):

```
PROP STRATEGY BUILDER v1.0

INSTALLATION:
1. Extract the ZIP file (if zipped)
2. No installation needed!

HOW TO RUN:
1. Double-click "AKHENS TRADER.exe"
2. If Windows SmartScreen appears:
   - Click "More info"
   - Click "Run anyway"
3. App launches immediately

SYSTEM REQUIREMENTS:
- Windows 10 or 11 (64-bit)
- No other software needed
- ~200 MB disk space

FEATURES:
- 8 preloaded trading strategies
- Visual strategy builder
- MQL5 code generation
- Export to MetaTrader 5
- Prop firm compliance presets

SUPPORT:
- Version: 1.0.0
- Contact: [Your Email/Website]
```

---

## ?? ADVANCED: ALTERNATIVE PUBLISH MODES

### **Framework-Dependent (Smaller File)**
If users have .NET 8 installed:

```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

**Result**: ~10 MB (requires .NET 8 runtime on target PC)

### **ReadyToRun (Faster Startup)**
For faster cold-start performance:

```xml
<PublishReadyToRun>true</PublishReadyToRun>
```

Add to `<PropertyGroup>` in .csproj

### **Enable Compression**
Slightly smaller .exe (slower extraction):

```xml
<EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
```

---

## ?? SUCCESS CRITERIA

Your app is ready for distribution when:

? **Single .exe file** created  
? **Runs without Visual Studio** on your PC  
? **Runs on different PC** without .NET installation  
? **All features work** (generate, save, load, export)  
? **File size acceptable** (~100-150 MB)  
? **SmartScreen warning understood** (expected behavior)  

---

## ?? FINAL OUTPUT STRUCTURE

```
?? PropStrategyBuilder_v1.0_Distribution/
   ??? ?? AKHENS TRADER.exe           (Main executable - 100-150 MB)
   ??? ?? README.txt                  (User instructions)
   ??? ?? LICENSE.txt                 (Optional - your license)
```

**Distribute**: ZIP this folder ? Share with users!

---

## ?? CONGRATULATIONS!

Your **Prop Strategy Builder** is now:
- ? **Standalone executable** (no dependencies)
- ? **Self-contained** (.NET 8 runtime included)
- ? **Single file** (easy distribution)
- ? **Production-ready** (v1.0.0)
- ? **User-friendly** (double-click to run)

**Your app can now be distributed to anyone with Windows 10/11!** ??

---

**Need to update version for future releases?**
1. Change `<Version>1.0.1</Version>` in .csproj
2. Republish
3. Distribute new version

**That's it! You're done!** ??
