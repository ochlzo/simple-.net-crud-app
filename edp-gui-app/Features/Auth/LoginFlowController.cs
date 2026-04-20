namespace edp_gui_app;

public sealed class LoginFlowController
{
    public LoginViewState CurrentState { get; private set; } = LoginViewState.Landing;

    public SiteOwner? CurrentOwner { get; private set; }
    public OwnedSite? SelectedSite { get; private set; }

    public void ShowLanding()
    {
        CurrentState = LoginViewState.Landing;
        CurrentOwner = null;
        SelectedSite = null;
    }

    public void ShowLogin()
    {
        CurrentState = LoginViewState.Login;
        CurrentOwner = null;
        SelectedSite = null;
    }

    public void ShowSignUp()
    {
        CurrentState = LoginViewState.SignUp;
        CurrentOwner = null;
        SelectedSite = null;
    }

    public void ShowOwnerSites(SiteOwner owner)
    {
        CurrentOwner = owner;
        SelectedSite = null;
        CurrentState = LoginViewState.OwnerSites;
    }

    public void ShowOwnerSites()
    {
        SelectedSite = null;
        CurrentState = LoginViewState.OwnerSites;
    }

    public void ShowSiteDetails(OwnedSite site)
    {
        SelectedSite = site;
        CurrentState = LoginViewState.SiteDetails;
    }

    public void Logout()
    {
        ShowLanding();
    }
}
