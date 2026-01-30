# Loan Calculator Simplified - iOS

A SwiftUI iOS app with PIN authentication for loan calculations.

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
    ├── Views/
    │   ├── LoginView.swift                # PIN entry/setup screen
    │   └── CalculatorView.swift           # Main calculator
    └── Services/
        └── AuthenticationService.swift    # PIN storage (Keychain)
```

## Building

1. Copy the `ios` folder to a Mac
2. Open `loancalculatorsimplified.xcodeproj` in Xcode
3. Select your development team in Signing & Capabilities
4. Build and run (Cmd + R)

## Features

- **PIN Authentication**: 4-digit PIN with secure Keychain storage
- **First-time Setup**: Create and confirm PIN on first launch
- **Loan Calculator**: Fill any 3 fields to calculate the 4th
  - Loan Amount
  - Interest Rate (annual %)
  - Number of Payments
  - Payment Amount
- **Results Display**: Shows calculated value, total paid, and total interest

## Security

- PIN is hashed with SHA256 before storage
- Uses iOS Keychain for secure PIN storage
- PIN accessible only when device is unlocked

## Bundle ID

`com.loancalcsimplified.app` (same as Android version)
