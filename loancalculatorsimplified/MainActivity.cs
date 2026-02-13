using Android.Views;

namespace Loan_Calculator
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private EditText? editLoanAmount;
        private EditText? editInterestRate;
        private EditText? editNumPayments;
        private EditText? editPaymentAmount;
        private Button? btnCalculate;
        private Button? btnClear;
        private TextView? textError;
        private LinearLayout? layoutResult;
        private TextView? textCalcLabel;
        private TextView? textCalcValue;
        private TextView? textTotalPaid;
        private TextView? textTotalInterest;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            editLoanAmount = FindViewById<EditText>(Resource.Id.editLoanAmount);
            editInterestRate = FindViewById<EditText>(Resource.Id.editInterestRate);
            editNumPayments = FindViewById<EditText>(Resource.Id.editNumPayments);
            editPaymentAmount = FindViewById<EditText>(Resource.Id.editPaymentAmount);
            btnCalculate = FindViewById<Button>(Resource.Id.btnCalculate);
            btnClear = FindViewById<Button>(Resource.Id.btnClear);
            textError = FindViewById<TextView>(Resource.Id.textError);
            layoutResult = FindViewById<LinearLayout>(Resource.Id.layoutResult);
            textCalcLabel = FindViewById<TextView>(Resource.Id.textCalcLabel);
            textCalcValue = FindViewById<TextView>(Resource.Id.textCalcValue);
            textTotalPaid = FindViewById<TextView>(Resource.Id.textTotalPaid);
            textTotalInterest = FindViewById<TextView>(Resource.Id.textTotalInterest);

            btnCalculate!.Click += (sender, e) => CalculateLoan();
            btnClear!.Click += (sender, e) => ResetCalculator();
        }

        private void CalculateLoan()
        {
            HideError();
            HideResult();

            string loanAmountText = editLoanAmount!.Text?.Trim() ?? "";
            string interestRateText = editInterestRate!.Text?.Trim() ?? "";
            string numPaymentsText = editNumPayments!.Text?.Trim() ?? "";
            string paymentAmountText = editPaymentAmount!.Text?.Trim() ?? "";

            bool hasLoanAmount = !string.IsNullOrEmpty(loanAmountText);
            bool hasInterestRate = !string.IsNullOrEmpty(interestRateText);
            bool hasNumPayments = !string.IsNullOrEmpty(numPaymentsText);
            bool hasPaymentAmount = !string.IsNullOrEmpty(paymentAmountText);

            int filledCount = (hasLoanAmount ? 1 : 0) + (hasInterestRate ? 1 : 0) +
                              (hasNumPayments ? 1 : 0) + (hasPaymentAmount ? 1 : 0);

            if (filledCount != 3)
            {
                ShowError("Please fill in exactly 3 fields and leave 1 empty");
                return;
            }

            try
            {
                if (!hasLoanAmount)
                {
                    SolveForLoanAmount(interestRateText, numPaymentsText, paymentAmountText);
                }
                else if (!hasInterestRate)
                {
                    SolveForInterestRate(loanAmountText, numPaymentsText, paymentAmountText);
                }
                else if (!hasNumPayments)
                {
                    SolveForNumPayments(loanAmountText, interestRateText, paymentAmountText);
                }
                else
                {
                    SolveForPaymentAmount(loanAmountText, interestRateText, numPaymentsText);
                }
            }
            catch
            {
                ShowError("Invalid input values. Please check your entries.");
            }
        }

        private void SolveForLoanAmount(string interestRateText, string numPaymentsText, string paymentAmountText)
        {
            double r = double.Parse(interestRateText) / 100.0 / 12.0;
            int n = (int)double.Parse(numPaymentsText);
            double pmt = double.Parse(paymentAmountText);

            if (r == 0 || n == 0 || pmt == 0)
            {
                ShowError("Please fill in interest rate, number of payments, and payment amount");
                return;
            }

            double pv = pmt * ((1.0 - Math.Pow(1.0 + r, -n)) / r);
            double totalPaid = pmt * n;
            double totalInterest = totalPaid - pv;

            editLoanAmount!.Text = pv.ToString("F2");
            ShowResult("Loan Amount:", pv.ToString("F2"), totalPaid, totalInterest);
        }

        private void SolveForInterestRate(string loanAmountText, string numPaymentsText, string paymentAmountText)
        {
            double pv = double.Parse(loanAmountText);
            int n = (int)double.Parse(numPaymentsText);
            double pmt = double.Parse(paymentAmountText);

            if (pv == 0 || n == 0 || pmt == 0)
            {
                ShowError("Please fill in loan amount, number of payments, and payment amount");
                return;
            }

            if (pmt <= pv / n)
            {
                ShowError("Payment amount is too low");
                return;
            }

            // Bisection method to find monthly interest rate
            double low = 0;
            double high = 1;
            double rate = 0;

            for (int i = 0; i < 100; i++)
            {
                rate = (low + high) / 2.0;

                double calculatedPmt = rate == 0
                    ? pv / n
                    : pv * rate * Math.Pow(1.0 + rate, n) / (Math.Pow(1.0 + rate, n) - 1.0);

                if (Math.Abs(calculatedPmt - pmt) < 0.01)
                    break;

                if (calculatedPmt < pmt)
                    low = rate;
                else
                    high = rate;
            }

            double annualRate = rate * 12.0 * 100.0;
            double totalPaid = pmt * n;
            double totalInterest = totalPaid - pv;

            editInterestRate!.Text = annualRate.ToString("F2");
            ShowResult("Interest Rate (%):", annualRate.ToString("F2"), totalPaid, totalInterest);
        }

        private void SolveForNumPayments(string loanAmountText, string interestRateText, string paymentAmountText)
        {
            double pv = double.Parse(loanAmountText);
            double r = double.Parse(interestRateText) / 100.0 / 12.0;
            double pmt = double.Parse(paymentAmountText);

            if (pv == 0 || r == 0 || pmt == 0)
            {
                ShowError("Please fill in loan amount, interest rate, and payment amount");
                return;
            }

            if (pmt <= pv * r)
            {
                ShowError("Payment amount is too low to pay off the loan");
                return;
            }

            double n = Math.Log(pmt / (pmt - pv * r)) / Math.Log(1.0 + r);
            double calculatedN = Math.Round(n, 3);
            double totalPaid = pmt * calculatedN;
            double totalInterest = totalPaid - pv;

            editNumPayments!.Text = calculatedN.ToString("F3");
            ShowResult("Number of Payments:", calculatedN.ToString("F3"), totalPaid, totalInterest);
        }

        private void SolveForPaymentAmount(string loanAmountText, string interestRateText, string numPaymentsText)
        {
            double pv = double.Parse(loanAmountText);
            double r = double.Parse(interestRateText) / 100.0 / 12.0;
            int n = (int)double.Parse(numPaymentsText);

            if (pv == 0 || r == 0 || n == 0)
            {
                ShowError("Please fill in loan amount, interest rate, and number of payments");
                return;
            }

            double pmt = pv * r * Math.Pow(1.0 + r, n) / (Math.Pow(1.0 + r, n) - 1.0);
            double totalPaid = pmt * n;
            double totalInterest = totalPaid - pv;

            editPaymentAmount!.Text = pmt.ToString("F2");
            ShowResult("Payment Amount:", pmt.ToString("F2"), totalPaid, totalInterest);
        }

        private void ShowError(string message)
        {
            textError!.Text = message;
            textError.Visibility = ViewStates.Visible;
        }

        private void HideError()
        {
            textError!.Visibility = ViewStates.Gone;
        }

        private void ShowResult(string label, string value, double totalPaid, double totalInterest)
        {
            textCalcLabel!.Text = label;
            textCalcValue!.Text = value;
            textTotalPaid!.Text = "$" + totalPaid.ToString("F2");
            textTotalInterest!.Text = "$" + totalInterest.ToString("F2");
            layoutResult!.Visibility = ViewStates.Visible;
        }

        private void HideResult()
        {
            layoutResult!.Visibility = ViewStates.Gone;
        }

        private void ResetCalculator()
        {
            editLoanAmount!.Text = "";
            editInterestRate!.Text = "";
            editNumPayments!.Text = "";
            editPaymentAmount!.Text = "";
            HideError();
            HideResult();
        }
    }
}
