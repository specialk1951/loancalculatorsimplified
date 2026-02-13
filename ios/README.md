# Loan Calculator Simplified - iOS

A SwiftUI iOS app for loan calculations.

## Requirements

- macOS with Xcode 15.0 or later
- iOS 16.0+ deployment target
- Apple Developer account (for device testing/App Store)

## Project Structure

```
ios/
├── loancalculatorsimplified.xcodeproj/    # Xcode project file
└── loancalculatorsimplified/
    ├── loancalculatorsimplifiedApp.swift  # App entry point
    ├── Info.plist                         # App configuration
    ├── Assets.xcassets/                   # Images and colors
    ├── Models/
    │   └── LoanCalculation.swift          # Loan math logic
    └── Views/
        └── CalculatorView.swift           # Main calculator
```

## Building

1. Copy the `ios` folder to a Mac
2. Open `loancalculatorsimplified.xcodeproj` in Xcode
3. Select your development team in Signing & Capabilities
4. Build and run (Cmd + R)

## Features

- **Loan Calculator**: Fill any 3 fields to calculate the 4th
  - Loan Amount
  - Interest Rate (annual %)
  - Number of Payments
  - Payment Amount
- **Results Display**: Shows calculated value, total paid, and total interest

## Bundle ID

`com.loancalcsimplified.app` (same as Android version)
