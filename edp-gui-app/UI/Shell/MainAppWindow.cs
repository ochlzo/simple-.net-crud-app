namespace edp_gui_app;

public sealed partial class MainAppWindow : Form
{
    private const int SidebarWidth = 400;

    private readonly SiteOwnerAuthService _authService;
    private readonly LoginFlowController _flowController = new();
    private readonly TableLayoutPanel _shell;
    private readonly Panel _sidebar;
    private readonly Panel _contentHost;
    private readonly Control _landingPanel;
    private readonly Control _loginPanel;
    private readonly Control _signUpPanel;
    private readonly Control _ownerSitesPanel;
    private readonly Control _siteDetailsPanel;
    private readonly TextBox _emailTextBox;
    private readonly TextBox _passwordTextBox;
    private readonly Button _loginSubmitButton;
    private readonly TextBox _signUpNameTextBox;
    private readonly TextBox _signUpEmailTextBox;
    private readonly TextBox _signUpPasswordTextBox;
    private readonly Button _signUpSubmitButton;
    private readonly Label _loginStatusLabel;
    private readonly Label _signUpStatusLabel;
    private readonly TextBox _siteSearchTextBox;
    private readonly Button _refreshSitesButton;
    private readonly DataGridView _sitesGrid;
    private readonly Label _ownerSitesStatusLabel;
    private readonly Label _siteDetailsIdValueLabel;
    private readonly Label _siteDetailsNameValueLabel;

    public MainAppWindow(SiteOwnerAuthService authService)
    {
        _authService = authService;

        Text = "Site Management System";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(920, 520);

        _shell = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        _shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SidebarWidth));
        _shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _sidebar = BuildSidebar();
        _contentHost = new Panel
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
        (_ownerSitesPanel, _siteSearchTextBox, _refreshSitesButton, _sitesGrid, _ownerSitesStatusLabel) =
            BuildOwnerSitesPanel();
        (_siteDetailsPanel, _siteDetailsIdValueLabel, _siteDetailsNameValueLabel) = BuildSiteDetailsPanel();

        _contentHost.Controls.Add(_siteDetailsPanel);
        _contentHost.Controls.Add(_ownerSitesPanel);
        _contentHost.Controls.Add(_signUpPanel);
        _contentHost.Controls.Add(_loginPanel);
        _contentHost.Controls.Add(_landingPanel);

        _shell.Controls.Add(_sidebar, 0, 0);
        _shell.Controls.Add(_contentHost, 1, 0);
        Controls.Add(_shell);

        ResetOwnerWorkspace();
        ApplyState();
    }

    private void ApplyState()
    {
        var state = _flowController.CurrentState;
        var usingOwnerWorkspace = state is LoginViewState.OwnerSites or LoginViewState.SiteDetails;

        _sidebar.Visible = !usingOwnerWorkspace;
        _shell.ColumnStyles[0].Width = usingOwnerWorkspace ? 0 : SidebarWidth;
        _contentHost.Padding = usingOwnerWorkspace ? new Padding(18) : new Padding(28);

        _landingPanel.Visible = state == LoginViewState.Landing;
        _loginPanel.Visible = state == LoginViewState.Login;
        _signUpPanel.Visible = state == LoginViewState.SignUp;
        _ownerSitesPanel.Visible = state == LoginViewState.OwnerSites;
        _siteDetailsPanel.Visible = state == LoginViewState.SiteDetails;

        if (state == LoginViewState.SiteDetails)
        {
            UpdateSiteDetailsPanel();
        }

        AcceptButton = state switch
        {
            LoginViewState.Login => _loginSubmitButton,
            LoginViewState.SignUp => _signUpSubmitButton,
            _ => null
        };

        var activePanel = state switch
        {
            LoginViewState.Login => _loginPanel,
            LoginViewState.SignUp => _signUpPanel,
            LoginViewState.OwnerSites => _ownerSitesPanel,
            LoginViewState.SiteDetails => _siteDetailsPanel,
            _ => _landingPanel
        };
        activePanel.BringToFront();
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

            await EnterOwnerWorkspaceAsync(owner);
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

            await EnterOwnerWorkspaceAsync(result.Owner!);
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
        ResetOwnerWorkspace();
        _flowController.Logout();
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
        _ownerSitesPanel.Enabled = !isBusy;
        _siteDetailsPanel.Enabled = !isBusy;
        _refreshSitesButton.Enabled = !isBusy;
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
