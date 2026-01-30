import Foundation

struct LoanResult {
    let calculatedField: String
    let calculatedValue: Double
    let totalPaid: Double
    let totalInterest: Double
}

class LoanCalculator {

    static func solveForLoanAmount(interestRate: Double, numPayments: Int, paymentAmount: Double) -> LoanResult? {
        let r = interestRate / 100.0 / 12.0
        let n = Double(numPayments)
        let pmt = paymentAmount

        guard r > 0, n > 0, pmt > 0 else { return nil }

        let pv = pmt * ((1.0 - pow(1.0 + r, -n)) / r)
        let totalPaid = pmt * n
        let totalInterest = totalPaid - pv

        return LoanResult(
            calculatedField: "Loan Amount",
            calculatedValue: pv,
            totalPaid: totalPaid,
            totalInterest: totalInterest
        )
    }

    static func solveForInterestRate(loanAmount: Double, numPayments: Int, paymentAmount: Double) -> LoanResult? {
        let pv = loanAmount
        let n = Double(numPayments)
        let pmt = paymentAmount

        guard pv > 0, n > 0, pmt > 0 else { return nil }
        guard pmt > pv / n else { return nil } // Payment too low

        // Bisection method
        var low = 0.0
        var high = 1.0
        var rate = 0.0

        for _ in 0..<100 {
            rate = (low + high) / 2.0

            let calculatedPmt = rate == 0
                ? pv / n
                : pv * rate * pow(1.0 + rate, n) / (pow(1.0 + rate, n) - 1.0)

            if abs(calculatedPmt - pmt) < 0.01 {
                break
            }

            if calculatedPmt < pmt {
                low = rate
            } else {
                high = rate
            }
        }

        let annualRate = rate * 12.0 * 100.0
        let totalPaid = pmt * n
        let totalInterest = totalPaid - pv

        return LoanResult(
            calculatedField: "Interest Rate (%)",
            calculatedValue: annualRate,
            totalPaid: totalPaid,
            totalInterest: totalInterest
        )
    }

    static func solveForNumPayments(loanAmount: Double, interestRate: Double, paymentAmount: Double) -> LoanResult? {
        let pv = loanAmount
        let r = interestRate / 100.0 / 12.0
        let pmt = paymentAmount

        guard pv > 0, r > 0, pmt > 0 else { return nil }
        guard pmt > pv * r else { return nil } // Payment too low

        let n = log(pmt / (pmt - pv * r)) / log(1.0 + r)
        let totalPaid = pmt * n
        let totalInterest = totalPaid - pv

        return LoanResult(
            calculatedField: "Number of Payments",
            calculatedValue: n,
            totalPaid: totalPaid,
            totalInterest: totalInterest
        )
    }

    static func solveForPaymentAmount(loanAmount: Double, interestRate: Double, numPayments: Int) -> LoanResult? {
        let pv = loanAmount
        let r = interestRate / 100.0 / 12.0
        let n = Double(numPayments)

        guard pv > 0, r > 0, n > 0 else { return nil }

        let pmt = pv * r * pow(1.0 + r, n) / (pow(1.0 + r, n) - 1.0)
        let totalPaid = pmt * n
        let totalInterest = totalPaid - pv

        return LoanResult(
            calculatedField: "Payment Amount",
            calculatedValue: pmt,
            totalPaid: totalPaid,
            totalInterest: totalInterest
        )
    }
}
