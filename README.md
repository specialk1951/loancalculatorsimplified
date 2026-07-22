# Loan Calculator Simplified

<img src="AppIcon-1024.png" alt="App Icon" width="100">

A cross-platform loan calculator built with .NET MAUI targeting Android, iOS, macOS, and Windows. Now with Android 16 (API 36) support.

Enter any 3 of the 4 loan variables and the app solves for the missing one:

- **Loan Amount**
- **Interest Rate** (annual %)
- **Number of Payments**
- **Payment Amount**

Results include the calculated value, total amount paid, and total interest paid.

## Platforms

### Android

Built with .NET MAUI (`net9.0-android36.0`). Targets Android 16 (API 36), minimum Android 5.0 (API 21).

**Requirements:** Visual Studio 2022+ with the .NET MAUI / Android workload and Android SDK Platform 36 installed.

```bash
dotnet build -t:Run -f net9.0-android36.0
```

### iOS

Built with SwiftUI (iOS 16+). Uses a clean Models/Views architecture.

See [`ios/README.md`](ios/README.md) for full details.

**Requirements:** macOS with Xcode 15+.

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- .NET MAUI workload (`dotnet workload install maui`)
- Android SDK with API 36 (Android 16) platform installed (for Android deployment)

## Project Structure

```
LoanCalculatorSimplified/
├── LoanCalculatorSimplified.sln
├── LoanCalculatorSimplified/
│   ├── LoanCalculatorSimplified.csproj
│   ├── Platforms/Android/AndroidManifest.xml
│   ├── MainPage.xaml
│   └── Resources/
├── ios/
│   ├── loancalculatorsimplified.xcodeproj/
│   └── loancalculatorsimplified/
│       ├── Models/LoanCalculation.swift
│       └── Views/CalculatorView.swift
```

## Bundle ID

`com.specialk9.loancalculatorsimplifiedii`
