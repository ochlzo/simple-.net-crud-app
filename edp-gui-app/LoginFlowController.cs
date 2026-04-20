namespace edp_gui_app;

public sealed class LoginFlowController
{
    public LoginViewState CurrentState { get; private set; } = LoginViewState.Landing;

    public string? WelcomeOwnerName { get; private set; }

    public void ShowLanding()
    {
        CurrentState = LoginViewState.Landing;
        WelcomeOwnerName = null;
    }

    public void ShowLogin()
    {
        CurrentState = LoginViewState.Login;
        WelcomeOwnerName = null;
    }

    public void ShowSignUp()
    {
        CurrentState = LoginViewState.SignUp;
        WelcomeOwnerName = null;
    }

    public void ShowWelcome(string ownerName)
    {
        CurrentState = LoginViewState.Welcome;
        WelcomeOwnerName = ownerName;
    }
}
