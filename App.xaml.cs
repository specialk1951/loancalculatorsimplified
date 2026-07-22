namespace LoanCalculatorSimplified;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());
		window.Title = "Loan Calculator Simplified";
		window.Width = 480;
		window.Height = 700;
		window.MinimumWidth = 400;
		window.MinimumHeight = 600;
		return window;
	}
}