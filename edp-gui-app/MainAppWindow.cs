namespace edp_gui_app;

public sealed partial class MainAppWindow : Form
{
    private readonly SiteOwnerAuthService _authService;
    private readonly LoginFlowController _flowController = new();
    private readonly Control _landingPanel;
    private readonly Control _loginPanel;
    private readonly Control _signUpPanel;
    private readonly Control _welcomePanel;
    private readonly TextBox _emailTextBox;
    private readonly TextBox _passwordTextBox;
    private readonly Button _loginSubmitButton;
    private readonly TextBox _signUpNameTextBox;
    private readonly TextBox _signUpEmailTextBox;
    private readonly TextBox _signUpPasswordTextBox;
    private readonly Button _signUpSubmitButton;
    private readonly Label _loginStatusLabel;
    private readonly Label _signUpStatusLabel;
    private readonly Label _welcomeLabel;

    public MainAppWindow(SiteOwnerAuthService authService)
    {
        _authService = authService;

        Text = "Site Management System";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(820, 460);

        var shell = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400));
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 150));

        var contentHost = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(28),
            BackColor = Color.WhiteSmoke
        };

        _landingPanel = BuildLandingPanel();
        (_loginPanel, _emailTextBox, _passwordTextBox, _loginSubmitButton, _loginStatusLabel) =
            BuildLoginPanel();
        (_signUpPanel, _signUpNameTextBox, _signUpEmailTextBox, _signUpPasswordTextBox, _signUpSubmitButton,
            _signUpStatusLabel) = BuildSignUpPanel();
        (_welcomePanel, _welcomeLabel) = BuildWelcomePanel();

        contentHost.Controls.Add(_welcomePanel);
        contentHost.Controls.Add(_signUpPanel);
        contentHost.Controls.Add(_loginPanel);
        contentHost.Controls.Add(_landingPanel);

        shell.Controls.Add(BuildSidebar(), 0, 0);
        shell.Controls.Add(contentHost, 1, 0);
        Controls.Add(shell);

        ApplyState();
    }

    private void ApplyState()
    {
        _landingPanel.Visible = _flowController.CurrentState == LoginViewState.Landing;
        _loginPanel.Visible = _flowController.CurrentState == LoginViewState.Login;
        _signUpPanel.Visible = _flowController.CurrentState == LoginViewState.SignUp;
        _welcomePanel.Visible = _flowController.CurrentState == LoginViewState.Welcome;

        var activePanel = _flowController.CurrentState switch
        {
            LoginViewState.Login => _loginPanel,
            LoginViewState.SignUp => _signUpPanel,
            LoginViewState.Welcome => _welcomePanel,
            _ => _landingPanel
        };

        if (_flowController.CurrentState == LoginViewState.Welcome)
        {
            var ownerName = _flowController.WelcomeOwnerName ?? "site owner";
            _welcomeLabel.Text = $"Welcome, {ownerName}.";
        }

        AcceptButton = _flowController.CurrentState switch
        {
            LoginViewState.Login => _loginSubmitButton,
            LoginViewState.SignUp => _signUpSubmitButton,
            _ => null
        };

        if (activePanel is not null)
        {
            activePanel.BringToFront();
        }
    }

    private async void OnSubmitLoginClicked(object? sender, EventArgs e)
    {
        var email = _emailTextBox.Text.Trim();
        var password = _passwordTextBox.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ShowLoginStatus("Email and password are required.", Color.Firebrick);
            return;
        }

        try
        {
            SetBusy(true);
            ShowLoginStatus("Checking credentials...", Color.DimGray);

            var owner = await _authService.AuthenticateAsync(email, password);
            if (owner is null)
            {
                ShowLoginStatus("Invalid email or password.", Color.Firebrick);
                return;
            }

            _flowController.ShowWelcome(owner.OwnerName);
            ApplyState();
        }
        catch (Exception ex)
        {
            ShowLoginStatus($"Login failed: {ex.Message}", Color.Firebrick);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnSubmitSignUpClicked(object? sender, EventArgs e)
    {
        var ownerName = _signUpNameTextBox.Text.Trim();
        var email = _signUpEmailTextBox.Text.Trim();
        var password = _signUpPasswordTextBox.Text;

        if (string.IsNullOrWhiteSpace(ownerName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            ShowSignUpStatus("Name, email, and password are required.", Color.Firebrick);
            return;
        }

        try
        {
            SetBusy(true);
            ShowSignUpStatus("Creating account...", Color.DimGray);

            var result = await _authService.CreateOwnerAsync(ownerName, email, password);
            if (result.Status == CreateSiteOwnerStatus.EmailAlreadyExists)
            {
                ShowSignUpStatus("Email already exists.", Color.Firebrick);
                return;
            }

            _flowController.ShowWelcome(result.Owner!.OwnerName);
            ApplyState();
        }
        catch (Exception ex)
        {
            ShowSignUpStatus($"Sign-up failed: {ex.Message}", Color.Firebrick);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void ShowLandingView()
    {
        ResetAuthForms();
        _flowController.ShowLanding();
        ApplyState();
    }

    private void ShowLoginView()
    {
        ResetLoginStatus();
        SetBusy(false);
        _flowController.ShowLogin();
        ApplyState();
        _emailTextBox.Focus();
    }

    private void ShowSignUpView()
    {
        ResetSignUpStatus();
        SetBusy(false);
        _flowController.ShowSignUp();
        ApplyState();
        _signUpNameTextBox.Focus();
    }

    private void ResetAuthForms()
    {
        ResetLoginForm();
        ResetSignUpForm();
    }

    private void ResetLoginForm()
    {
        _emailTextBox.Clear();
        _passwordTextBox.Clear();
        ResetLoginStatus();
    }

    private void ResetSignUpForm()
    {
        _signUpNameTextBox.Clear();
        _signUpEmailTextBox.Clear();
        _signUpPasswordTextBox.Clear();
        ResetSignUpStatus();
    }

    private void ResetLoginStatus()
    {
        ShowLoginStatus(string.Empty, Color.Firebrick);
    }

    private void ResetSignUpStatus()
    {
        ShowSignUpStatus(string.Empty, Color.Firebrick);
    }

    private void SetBusy(bool isBusy)
    {
        _loginPanel.Enabled = !isBusy;
        _signUpPanel.Enabled = !isBusy;
        UseWaitCursor = isBusy;
    }

    private void ShowLoginStatus(string text, Color color)
    {
        _loginStatusLabel.Text = text;
        _loginStatusLabel.ForeColor = color;
    }

    private void ShowSignUpStatus(string text, Color color)
    {
        _signUpStatusLabel.Text = text;
        _signUpStatusLabel.ForeColor = color;
    }
}
