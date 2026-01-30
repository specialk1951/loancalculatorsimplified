import SwiftUI

struct LoginView: View {
    @EnvironmentObject var authService: AuthenticationService
    @State private var pin = ""
    @State private var confirmPin = ""
    @State private var isConfirming = false
    @State private var errorMessage = ""
    @State private var showError = false

    var isSettingUp: Bool {
        !authService.isPinSet
    }

    var title: String {
        if isSettingUp {
            return isConfirming ? "Confirm PIN" : "Create PIN"
        }
        return "Enter PIN"
    }

    var subtitle: String {
        if isSettingUp {
            return isConfirming ? "Re-enter your PIN to confirm" : "Enter a 4-digit PIN to secure your app"
        }
        return "Enter your 4-digit PIN to continue"
    }

    var body: some View {
        VStack(spacing: 0) {
            // Header
            VStack(spacing: 8) {
                Text(title)
                    .font(.title)
                    .fontWeight(.bold)
                    .foregroundColor(.white)

                Text(subtitle)
                    .font(.subheadline)
                    .foregroundColor(.white.opacity(0.8))
            }
            .frame(maxWidth: .infinity)
            .padding(.vertical, 32)
            .background(Color.blue)

            VStack(spacing: 24) {
                // PIN Display
                HStack(spacing: 16) {
                    ForEach(0..<4, id: \.self) { index in
                        Circle()
                            .stroke(Color.blue, lineWidth: 2)
                            .background(
                                Circle()
                                    .fill(index < pin.count ? Color.blue : Color.clear)
                            )
                            .frame(width: 20, height: 20)
                    }
                }
                .padding(.top, 32)

                // Error Message
                if showError {
                    Text(errorMessage)
                        .font(.subheadline)
                        .foregroundColor(.red)
                        .padding(.horizontal, 16)
                        .padding(.vertical, 12)
                        .background(Color.red.opacity(0.1))
                        .cornerRadius(8)
                }

                // Number Pad
                VStack(spacing: 12) {
                    ForEach(0..<3, id: \.self) { row in
                        HStack(spacing: 24) {
                            ForEach(1..<4, id: \.self) { col in
                                let number = row * 3 + col
                                PinButton(number: "\(number)") {
                                    addDigit("\(number)")
                                }
                            }
                        }
                    }

                    HStack(spacing: 24) {
                        PinButton(number: "C", isSecondary: true) {
                            clearPin()
                        }

                        PinButton(number: "0") {
                            addDigit("0")
                        }

                        PinButton(number: "\u{232B}", isSecondary: true) {
                            backspace()
                        }
                    }
                }
                .padding(.top, 16)

                Spacer()
            }
            .padding()
            .background(Color(.systemBackground))
        }
    }

    private func addDigit(_ digit: String) {
        guard pin.count < 4 else { return }
        pin += digit
        showError = false

        if pin.count == 4 {
            processPin()
        }
    }

    private func clearPin() {
        pin = ""
        showError = false
    }

    private func backspace() {
        guard !pin.isEmpty else { return }
        pin.removeLast()
        showError = false
    }

    private func processPin() {
        if isSettingUp {
            if !isConfirming {
                confirmPin = pin
                pin = ""
                isConfirming = true
            } else {
                if pin == confirmPin {
                    if authService.setupPin(pin) {
                        // Success - auth service will update state
                    } else {
                        showError(message: "Failed to save PIN. Please try again.")
                        resetSetup()
                    }
                } else {
                    showError(message: "PINs do not match. Try again.")
                    resetSetup()
                }
            }
        } else {
            if authService.verifyPin(pin) {
                // Success - auth service will update state
            } else {
                showError(message: "Incorrect PIN. Try again.")
                pin = ""
            }
        }
    }

    private func resetSetup() {
        pin = ""
        confirmPin = ""
        isConfirming = false
    }

    private func showError(message: String) {
        errorMessage = message
        showError = true
    }
}

struct PinButton: View {
    let number: String
    var isSecondary: Bool = false
    let action: () -> Void

    var body: some View {
        Button(action: action) {
            Text(number)
                .font(.title)
                .fontWeight(.semibold)
                .foregroundColor(isSecondary ? .primary : .white)
                .frame(width: 72, height: 72)
                .background(isSecondary ? Color.blue.opacity(0.2) : Color.blue)
                .clipShape(Circle())
        }
    }
}

#Preview {
    LoginView()
        .environmentObject(AuthenticationService())
}
