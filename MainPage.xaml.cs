namespace LoanCalculatorSimplified;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void btnLoanAmount_Click(object? sender, EventArgs e)
    {
        string chrInterestRate = txbInterestRate.Text;
        if (string.IsNullOrEmpty(chrInterestRate))
        {
            await MissingEntries();
            return;
        }
        chrInterestRate = chrInterestRate.TrimEnd('%');

        string chrNumberOfPayments = txbNumberOfPayments.Text;
        if (string.IsNullOrEmpty(chrNumberOfPayments))
        {
            await MissingEntries();
            return;
        }

        string chrPaymentAmount = txbPaymentAmount.Text;
        if (string.IsNullOrEmpty(chrPaymentAmount))
        {
            await MissingEntries();
            return;
        }
        chrPaymentAmount = chrPaymentAmount.TrimStart('$');

        double interestRate = Convert.ToDouble(chrInterestRate);
        double numberOfPayments = Convert.ToDouble(chrNumberOfPayments);
        double paymentAmount = Convert.ToDouble(chrPaymentAmount);

        if (interestRate < 0.1)
        {
            await OneTo100();
            txbTotalInterest.Text = "";
            txbInterestRate.Text = "";
            return;
        }
        if (numberOfPayments < 1)
        {
            await LessThanOne("Number of Payments");
            txbNumberOfPayments.Text = "";
            return;
        }
        if (paymentAmount < 1)
        {
            await LessThanOne("Payment Amount");
            txbPaymentAmount.Text = "";
            return;
        }
        if (interestRate > 100)
        {
            await OneTo100();
            txbInterestRate.Text = "";
            return;
        }

        try
        {
            double monthlyInterestRate = interestRate / 100 / 12;
            double loanAmount = paymentAmount / (monthlyInterestRate / (1 - Math.Pow((1 + monthlyInterestRate), -numberOfPayments)));

            txbLoanAmount.Text = string.Format("{0:$ ###,###,###.00}", loanAmount);

            CalculateTotalInterest();
        }
        catch (DivideByZeroException)
        {
            await DisplayAlert("DivideByZeroException", "Can't divide by zero", "OK");
        }
    }

    private async void btnInterestRate_Click(object? sender, EventArgs e)
    {
        double i = 0.001;

        string chrLoanAmount = txbLoanAmount.Text;
        if (string.IsNullOrEmpty(chrLoanAmount))
        {
            await MissingEntries();
            return;
        }
        chrLoanAmount = chrLoanAmount.TrimStart('$');

        string chrNumberOfPayments = txbNumberOfPayments.Text;
        if (string.IsNullOrEmpty(chrNumberOfPayments))
        {
            await MissingEntries();
            return;
        }

        string chrPaymentAmount = txbPaymentAmount.Text;
        if (string.IsNullOrEmpty(chrPaymentAmount))
        {
            await MissingEntries();
            return;
        }
        chrPaymentAmount = chrPaymentAmount.TrimStart('$');

        double loanAmount = Convert.ToDouble(chrLoanAmount);
        double numberOfPayments = Convert.ToDouble(chrNumberOfPayments);
        double paymentAmount = Convert.ToDouble(chrPaymentAmount);

        if (loanAmount < 1)
        {
            await LessThanOne("Loan Amount");
            txbLoanAmount.Text = "";
            return;
        }
        if (numberOfPayments < 1)
        {
            await LessThanOne("Number of Payments");
            txbNumberOfPayments.Text = "";
            return;
        }
        if (paymentAmount < 1)
        {
            await LessThanOne("Payment Amount");
            txbPaymentAmount.Text = "";
            return;
        }

        try
        {
            while (i <= 100)
            {
                double monthlyInterestRate = i / 100 / 12;
                double AoverP = paymentAmount / loanAmount;
                double InterestFormula = monthlyInterestRate / (1 - Math.Pow(1 + monthlyInterestRate, -numberOfPayments));

                if (AoverP > InterestFormula)
                {
                    i += 0.0001;
                }
                else
                {
                    i -= 0.0001;
                    i += 0.00005;
                    i = Math.Round(i, 4);

                    i = i / 100;  // Adjust i for % format
                    txbInterestRate.Text = string.Format("{0:##0.000%}", i);
                    if (i < 0.001 || i > 1)
                    {
                        txbInterestRate.Text = "";
                        await DisplayAlert("Interest Rate", "Allowable range for Interest Rate is 0.1 to 100 percent", "OK");
                        txbInterestRate.Text = "";
                        txbTotalInterest.Text = "";
                    }
                    break;
                }
                if (i > 100)
                {
                    await DisplayAlert("Interest Rate", "Allowable range for Interest Rate is 0.1 to 100 percent", "OK");
                    txbInterestRate.Text = "";
                    txbTotalInterest.Text = "";
                }
            }
            if (i >= 0.001 && i <= 1.0)
            {
                CalculateTotalInterest();
            }
        }
        catch (DivideByZeroException)
        {
            await DisplayAlert("DivideByZeroException", "Can't divide by zero", "OK");
        }
    }

    private async void btnNumberOfPayments_Click(object? sender, EventArgs e)
    {
        string chrLoanAmount = txbLoanAmount.Text;
        if (string.IsNullOrEmpty(chrLoanAmount))
        {
            await MissingEntries();
            return;
        }
        chrLoanAmount = chrLoanAmount.TrimStart('$');

        string chrInterestRate = txbInterestRate.Text;
        if (string.IsNullOrEmpty(chrInterestRate))
        {
            await MissingEntries();
            return;
        }
        chrInterestRate = chrInterestRate.TrimEnd('%');

        string chrPaymentAmount = txbPaymentAmount.Text;
        if (string.IsNullOrEmpty(chrPaymentAmount))
        {
            await MissingEntries();
            return;
        }
        chrPaymentAmount = chrPaymentAmount.TrimStart('$');

        double loanAmount = Convert.ToDouble(chrLoanAmount);
        double interestRate = Convert.ToDouble(chrInterestRate);
        double paymentAmount = Convert.ToDouble(chrPaymentAmount);

        if (loanAmount < 1)
        {
            await LessThanOne("Loan Amount");
            txbLoanAmount.Text = "";
            return;
        }
        if (interestRate < 0.1)
        {
            await OneTo100();
            txbTotalInterest.Text = "";
            txbInterestRate.Text = "";
            return;
        }
        if (paymentAmount < 1)
        {
            await LessThanOne("Payment Amount");
            txbPaymentAmount.Text = "";
            return;
        }
        if (interestRate > 100)
        {
            await OneTo100();
            txbInterestRate.Text = "";
            return;
        }

        try
        {
            double monthlyInterestRate = interestRate / 100 / 12;
            double numerator = Math.Log10(1 - (loanAmount * monthlyInterestRate / paymentAmount));
            double denominator = Math.Log10(1 + monthlyInterestRate);
            double numberOfPayments = -numerator / denominator;

            txbNumberOfPayments.Text = string.Format("{0:###.000}", numberOfPayments);

            CalculateTotalInterest();
        }
        catch (DivideByZeroException)
        {
            await DisplayAlert("DivideByZeroException", "Can't divide by zero", "OK");
        }
    }

    private async void btnPaymentAmount_Click(object? sender, EventArgs e)
    {
        string chrNumberOfPayments = txbNumberOfPayments.Text;
        if (string.IsNullOrEmpty(chrNumberOfPayments))
        {
            await MissingEntries();
            return;
        }

        string chrInterestRate = txbInterestRate.Text;
        if (string.IsNullOrEmpty(chrInterestRate))
        {
            await MissingEntries();
            return;
        }
        chrInterestRate = chrInterestRate.TrimEnd('%');

        string chrLoanAmount = txbLoanAmount.Text;
        if (string.IsNullOrEmpty(chrLoanAmount))
        {
            await MissingEntries();
            return;
        }
        chrLoanAmount = chrLoanAmount.TrimStart('$');

        double loanAmount = Convert.ToDouble(chrLoanAmount);
        double interestRate = Convert.ToDouble(chrInterestRate);
        double numberOfPayments = Convert.ToDouble(chrNumberOfPayments);

        if (loanAmount < 1)
        {
            await LessThanOne("Loan Amount");
            txbLoanAmount.Text = "";
            return;
        }
        if (interestRate < 0.1)
        {
            await OneTo100();
            txbTotalInterest.Text = "";
            txbInterestRate.Text = "";
            return;
        }
        if (numberOfPayments < 1)
        {
            await LessThanOne("Number of Payments");
            txbNumberOfPayments.Text = "";
            return;
        }
        if (interestRate > 100)
        {
            await OneTo100();
            txbInterestRate.Text = "";
            return;
        }

        try
        {
            double monthlyInterestRate = interestRate / 100 / 12;
            double paymentAmount = loanAmount * monthlyInterestRate /
                                   (1 - Math.Pow((1 + monthlyInterestRate), -numberOfPayments));

            txbPaymentAmount.Text = string.Format("{0:$ ###,###,###.00}", paymentAmount);

            CalculateTotalInterest();
        }
        catch (DivideByZeroException)
        {
            await DisplayAlert("DivideByZeroException", "Can't divide by zero", "OK");
        }
    }

    private void btnClear_Click(object? sender, EventArgs e)
    {
        txbLoanAmount.Text = "";
        txbInterestRate.Text = "";
        txbNumberOfPayments.Text = "";
        txbPaymentAmount.Text = "";
        txbTotalInterest.Text = "";
    }

    private void CalculateTotalInterest()
    {
        string chrLoanAmount = txbLoanAmount.Text.TrimStart('$');
        string chrPaymentAmount = txbPaymentAmount.Text.TrimStart('$');

        double loanAmount = Convert.ToDouble(chrLoanAmount);
        double numberOfPayments = Convert.ToDouble(txbNumberOfPayments.Text);
        double paymentAmount = Convert.ToDouble(chrPaymentAmount);

        double totalInterest = (paymentAmount * numberOfPayments) - loanAmount;

        txbTotalInterest.Text = string.Format("{0:$ ###,###,###.00}", totalInterest);
    }

    private async Task MissingEntries()
    {
        await DisplayAlert("Missing Entries", "Ensure that 3 of the 4 entries are filled-in before selecting a button", "OK");
    }

    private async Task LessThanOne(string fieldName)
    {
        await DisplayAlert(fieldName, "Allowable entry for " + fieldName + " is greater than or equal to 1", "OK");
    }

    private async Task OneTo100()
    {
        await DisplayAlert("Interest Rate", "Allowable entry for Interest Rate is 0.1 to 100", "OK");
    }
}
