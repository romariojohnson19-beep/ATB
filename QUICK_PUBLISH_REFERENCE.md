# ?? QUICK PUBLISH REFERENCE

## **ONE-COMMAND PUBLISH**

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```

**Output**: `bin\Release\net8.0-windows\win-x64\publish\AKHENS TRADER.exe`

---

## **VISUAL STUDIO STEPS**

1. **Right-click** project ? **Publish...**
2. **New** ? **Folder** ? **Finish**
3. **Settings**:
   - Configuration: `Release`
   - Deployment: `Self-contained`
   - Target runtime: `win-x64`
   - Single file: ? Checked
4. **Publish** ? Wait ? Done!

---

## **OUTPUT**

?? **Location**: `bin\Release\net8.0-windows\win-x64\publish\`  
?? **File**: `AKHENS TRADER.exe` (~100-150 MB)  
? **Standalone**: No .NET installation needed  
? **Double-click**: Runs immediately  

---

## **DISTRIBUTE**

1. **Copy** `AKHENS TRADER.exe` from publish folder
2. **Test** on different PC (optional)
3. **ZIP** for easy distribution
4. **Share** with users!

---

## **FIRST RUN WARNING**

Windows SmartScreen:
```
"Windows protected your PC"
```

**Fix**:
1. Click **"More info"**
2. Click **"Run anyway"**

**Normal** - not code-signed (needs $300+/year certificate)

---

## **TROUBLESHOOTING**

| Problem | Solution |
|---------|----------|
| Won't start | Run as administrator |
| Too large | Normal (self-contained = big) |
| Missing DLLs | Republish with `SelfContained=true` |
| Antivirus blocks | Add to whitelist |

---

## **VERSION UPDATE**

Change in `.csproj`:
```xml
<Version>1.0.1</Version>
```

Then republish!

---

**That's it! 3 commands/clicks to standalone .exe!** ??
