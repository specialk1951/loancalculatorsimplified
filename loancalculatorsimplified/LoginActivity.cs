using Android.Content;
using Android.Views;
using System.Security.Cryptography;
using System.Text;

namespace Loan_Calculator
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class LoginActivity : Activity
    {
        private TextView? textTitle;
        private TextView? textSubtitle;
        private TextView? textPinDisplay;
        private TextView? textError;
        private Button? btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9, btn0;
        private Button? btnClear, btnBackspace;

        private string currentPin = "";
        private string? firstPin = null; // Used during PIN setup for confirmation
        private bool isSettingUp = false;
        private const string PREFS_NAME = "LoanCalcPrefs";
        private const string PIN_KEY = "pin_hash";

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_login);

            InitializeViews();
            SetupButtonHandlers();

            // Check if PIN is already set
            isSettingUp = !IsPinSet();
            UpdateUI();
        }

        private void InitializeViews()
        {
            textTitle = FindViewById<TextView>(Resource.Id.textTitle);
            textSubtitle = FindViewById<TextView>(Resource.Id.textSubtitle);
            textPinDisplay = FindViewById<TextView>(Resource.Id.textPinDisplay);
            textError = FindViewById<TextView>(Resource.Id.textLoginError);

            btn1 = FindViewById<Button>(Resource.Id.btn1);
            btn2 = FindViewById<Button>(Resource.Id.btn2);
            btn3 = FindViewById<Button>(Resource.Id.btn3);
            btn4 = FindViewById<Button>(Resource.Id.btn4);
            btn5 = FindViewById<Button>(Resource.Id.btn5);
            btn6 = FindViewById<Button>(Resource.Id.btn6);
            btn7 = FindViewById<Button>(Resource.Id.btn7);
            btn8 = FindViewById<Button>(Resource.Id.btn8);
            btn9 = FindViewById<Button>(Resource.Id.btn9);
            btn0 = FindViewById<Button>(Resource.Id.btn0);
            btnClear = FindViewById<Button>(Resource.Id.btnPinClear);
            btnBackspace = FindViewById<Button>(Resource.Id.btnBackspace);
        }

        private void SetupButtonHandlers()
        {
            btn1!.Click += (s, e) => AddDigit("1");
            btn2!.Click += (s, e) => AddDigit("2");
            btn3!.Click += (s, e) => AddDigit("3");
            btn4!.Click += (s, e) => AddDigit("4");
            btn5!.Click += (s, e) => AddDigit("5");
            btn6!.Click += (s, e) => AddDigit("6");
            btn7!.Click += (s, e) => AddDigit("7");
            btn8!.Click += (s, e) => AddDigit("8");
            btn9!.Click += (s, e) => AddDigit("9");
            btn0!.Click += (s, e) => AddDigit("0");
            btnClear!.Click += (s, e) => ClearPin();
            btnBackspace!.Click += (s, e) => Backspace();
        }

        private void AddDigit(string digit)
        {
            if (currentPin.Length < 4)
            {
                currentPin += digit;
                UpdatePinDisplay();
                HideError();

                if (currentPin.Length == 4)
                {
                    ProcessPin();
                }
            }
        }

        private void ClearPin()
        {
            currentPin = "";
            UpdatePinDisplay();
            HideError();
        }

        private void Backspace()
        {
            if (currentPin.Length > 0)
            {
                currentPin = currentPin.Substring(0, currentPin.Length - 1);
                UpdatePinDisplay();
                HideError();
            }
        }

        private void UpdatePinDisplay()
        {
            string display = "";
            for (int i = 0; i < 4; i++)
            {
                if (i < currentPin.Length)
                    display += "\u25CF "; // Filled circle
                else
                    display += "\u25CB "; // Empty circle
            }
            textPinDisplay!.Text = display.Trim();
        }

        private void ProcessPin()
        {
            if (isSettingUp)
            {
                if (firstPin == null)
                {
                    // First entry during setup
                    firstPin = currentPin;
                    currentPin = "";
                    textSubtitle!.Text = GetString(Resource.String.pin_confirm);
                    UpdatePinDisplay();
                }
                else
                {
                    // Confirmation entry
                    if (currentPin == firstPin)
                    {
                        SavePin(currentPin);
                        NavigateToMain();
                    }
                    else
                    {
                        ShowError(GetString(Resource.String.pin_mismatch));
                        firstPin = null;
                        currentPin = "";
                        textSubtitle!.Text = GetString(Resource.String.pin_setup_subtitle);
                        UpdatePinDisplay();
                    }
                }
            }
            else
            {
                // Verify PIN
                if (VerifyPin(currentPin))
                {
                    NavigateToMain();
                }
                else
                {
                    ShowError(GetString(Resource.String.pin_incorrect));
                    currentPin = "";
                    UpdatePinDisplay();
                }
            }
        }

        private void UpdateUI()
        {
            if (isSettingUp)
            {
                textTitle!.Text = GetString(Resource.String.pin_setup_title);
                textSubtitle!.Text = GetString(Resource.String.pin_setup_subtitle);
            }
            else
            {
                textTitle!.Text = GetString(Resource.String.pin_enter_title);
                textSubtitle!.Text = GetString(Resource.String.pin_enter_subtitle);
            }
            UpdatePinDisplay();
        }

        private bool IsPinSet()
        {
            var prefs = GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
            return prefs?.Contains(PIN_KEY) ?? false;
        }

        private void SavePin(string pin)
        {
            var prefs = GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
            var editor = prefs?.Edit();
            editor?.PutString(PIN_KEY, HashPin(pin));
            editor?.Apply();
        }

        private bool VerifyPin(string pin)
        {
            var prefs = GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
            var storedHash = prefs?.GetString(PIN_KEY, null);
            return storedHash != null && storedHash == HashPin(pin);
        }

        private string HashPin(string pin)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pin + "LoanCalcSalt"));
                return Convert.ToBase64String(bytes);
            }
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

        private void NavigateToMain()
        {
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            Finish();
        }
    }
}
