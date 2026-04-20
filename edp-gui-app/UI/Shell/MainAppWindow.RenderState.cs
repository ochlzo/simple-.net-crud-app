namespace edp_gui_app;

public sealed partial class MainAppWindow
{
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

    private void SetBusy(bool isBusy)
    {
        _loginPanel.Enabled = !isBusy;
        _signUpPanel.Enabled = !isBusy;
        _ownerSitesPanel.Enabled = !isBusy;
        _siteDetailsPanel.Enabled = !isBusy;
        _refreshSitesButton.Enabled = !isBusy;
        UseWaitCursor = isBusy;
    }
}
