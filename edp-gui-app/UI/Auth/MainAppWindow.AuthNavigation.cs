namespace edp_gui_app;

public sealed partial class MainAppWindow
{
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
}
