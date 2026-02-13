import SwiftUI

struct CalculatorView: View {
    @State private var loanAmount = ""
    @State private var interestRate = ""
    @State private var numPayments = ""
    @State private var paymentAmount = ""

    @State private var result: LoanResult?
    @State private var errorMessage = ""
    @State private var showError = false

    var body: some View {
        NavigationView {
            ScrollView {
                VStack(spacing: 0) {
                    // Header
                    VStack(alignment: .leading, spacing: 4) {
                        Text("Loan Calculator Simplified")
                            .font(.title2)
                            .fontWeight(.bold)
                            .foregroundColor(.white)

                        Text("Fill in any 3 fields to calculate the 4th")
                            .font(.subheadline)
                            .foregroundColor(.white.opacity(0.8))
                    }
                    .frame(maxWidth: .infinity, alignment: .leading)
                    .padding(24)
                    .background(Color.blue)

                    // Form
                    VStack(spacing: 16) {
                        InputField(title: "Loan Amount ($)", placeholder: "Enter loan amount or leave empty", text: $loanAmount, keyboardType: .decimalPad)

                        InputField(title: "Interest Rate (% per year)", placeholder: "Enter interest rate or leave empty", text: $interestRate, keyboardType: .decimalPad)

                        InputField(title: "Number of Payments", placeholder: "Enter number of payments or leave empty", text: $numPayments, keyboardType: .numberPad)

                        InputField(title: "Payment Amount ($)", placeholder: "Enter payment amount or leave empty", text: $paymentAmount, keyboardType: .decimalPad)

                        // Buttons
                        HStack(spacing: 12) {
                            Button(action: calculate) {
                                Text("Calculate")
                                    .fontWeight(.semibold)
                                    .foregroundColor(.white)
                                    .frame(maxWidth: .infinity)
                                    .padding()
                                    .background(Color.green)
                                    .cornerRadius(8)
                            }

                            Button(action: clear) {
                                Text("Clear")
                                    .fontWeight(.semibold)
                                    .foregroundColor(.white)
                                    .padding()
                                    .background(Color.red)
                                    .cornerRadius(8)
                            }
                        }

                        // Error Message
                        if showError {
                            Text(errorMessage)
                                .font(.subheadline)
                                .foregroundColor(.red)
                                .padding()
                                .frame(maxWidth: .infinity)
                                .background(Color.red.opacity(0.1))
                                .cornerRadius(8)
                        }

                        // Results
                        if let result = result {
                            ResultsView(result: result)
                        }
                    }
                    .padding(20)
                    .background(Color(.systemBackground))
                }
                .background(
                    RoundedRectangle(cornerRadius: 12)
                        .fill(Color(.systemBackground))
                        .shadow(radius: 4)
                )
                .padding()
            }
            .background(Color.blue.opacity(0.1))
            .navigationBarTitleDisplayMode(.inline)
        }
    }

    private func calculate() {
        showError = false
        result = nil

        let hasLoanAmount = !loanAmount.trimmingCharacters(in: .whitespaces).isEmpty
        let hasInterestRate = !interestRate.trimmingCharacters(in: .whitespaces).isEmpty
        let hasNumPayments = !numPayments.trimmingCharacters(in: .whitespaces).isEmpty
        let hasPaymentAmount = !paymentAmount.trimmingCharacters(in: .whitespaces).isEmpty

        let filledCount = [hasLoanAmount, hasInterestRate, hasNumPayments, hasPaymentAmount].filter { $0 }.count

        guard filledCount == 3 else {
            showError(message: "Please fill in exactly 3 fields and leave 1 empty")
            return
        }

        do {
            if !hasLoanAmount {
                guard let rate = Double(interestRate),
                      let payments = Int(numPayments),
                      let payment = Double(paymentAmount) else {
                    throw CalculationError.invalidInput
                }
                result = LoanCalculator.solveForLoanAmount(interestRate: rate, numPayments: payments, paymentAmount: payment)
                if let r = result {
                    loanAmount = String(format: "%.2f", r.calculatedValue)
                }
            } else if !hasInterestRate {
                guard let loan = Double(loanAmount),
                      let payments = Int(numPayments),
                      let payment = Double(paymentAmount) else {
                    throw CalculationError.invalidInput
                }
                result = LoanCalculator.solveForInterestRate(loanAmount: loan, numPayments: payments, paymentAmount: payment)
                if let r = result {
                    interestRate = String(format: "%.2f", r.calculatedValue)
                }
            } else if !hasNumPayments {
                guard let loan = Double(loanAmount),
                      let rate = Double(interestRate),
                      let payment = Double(paymentAmount) else {
                    throw CalculationError.invalidInput
                }
                result = LoanCalculator.solveForNumPayments(loanAmount: loan, interestRate: rate, paymentAmount: payment)
                if let r = result {
                    numPayments = String(format: "%.0f", r.calculatedValue)
                }
            } else {
                guard let loan = Double(loanAmount),
                      let rate = Double(interestRate),
                      let payments = Int(numPayments) else {
                    throw CalculationError.invalidInput
                }
                result = LoanCalculator.solveForPaymentAmount(loanAmount: loan, interestRate: rate, numPayments: payments)
                if let r = result {
                    paymentAmount = String(format: "%.2f", r.calculatedValue)
                }
            }

            if result == nil {
                showError(message: "Invalid input values. Please check your entries.")
            }
        } catch {
            showError(message: "Invalid input values. Please check your entries.")
        }
    }

    private func clear() {
        loanAmount = ""
        interestRate = ""
        numPayments = ""
        paymentAmount = ""
        result = nil
        showError = false
    }

    private func showError(message: String) {
        errorMessage = message
        showError = true
    }
}

enum CalculationError: Error {
    case invalidInput
}

struct InputField: View {
    let title: String
    let placeholder: String
    @Binding var text: String
    var keyboardType: UIKeyboardType = .default

    var body: some View {
        VStack(alignment: .leading, spacing: 4) {
            Text(title)
                .font(.subheadline)
                .fontWeight(.semibold)
                .foregroundColor(.secondary)

            TextField(placeholder, text: $text)
                .keyboardType(keyboardType)
                .padding()
                .background(Color(.systemGray6))
                .cornerRadius(8)
                .overlay(
                    RoundedRectangle(cornerRadius: 8)
                        .stroke(Color(.systemGray4), lineWidth: 1)
                )
        }
    }
}

struct ResultsView: View {
    let result: LoanResult

    var body: some View {
        VStack(alignment: .leading, spacing: 12) {
            Text("Results")
                .font(.headline)

            HStack {
                Text("\(result.calculatedField):")
                    .foregroundColor(.secondary)
                Spacer()
                Text(formattedValue)
                    .font(.title2)
                    .fontWeight(.bold)
                    .foregroundColor(.blue)
            }

            Divider()

            HStack {
                Text("Total Amount Paid:")
                    .foregroundColor(.secondary)
                Spacer()
                Text("$\(String(format: "%.2f", result.totalPaid))")
                    .fontWeight(.semibold)
            }

            HStack {
                Text("Total Interest Paid:")
                    .foregroundColor(.secondary)
                Spacer()
                Text("$\(String(format: "%.2f", result.totalInterest))")
                    .fontWeight(.semibold)
            }
        }
        .padding()
        .background(Color.blue.opacity(0.1))
        .cornerRadius(8)
    }

    var formattedValue: String {
        if result.calculatedField == "Interest Rate (%)" {
            return String(format: "%.2f%%", result.calculatedValue)
        } else if result.calculatedField == "Number of Payments" {
            return String(format: "%.0f", result.calculatedValue)
        } else {
            return "$\(String(format: "%.2f", result.calculatedValue))"
        }
    }
}

#Preview {
    CalculatorView()
}
