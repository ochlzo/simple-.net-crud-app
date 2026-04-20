using edp_gui_app;

namespace edp_gui_app.Tests;

[TestClass]
public sealed class LoginFlowControllerTests
{
    [TestMethod]
    public void NewController_StartsOnLanding()
    {
        var controller = new LoginFlowController();

        Assert.AreEqual(LoginViewState.Landing, controller.CurrentState);
        Assert.IsNull(controller.WelcomeOwnerName);
    }

    [TestMethod]
    public void ShowLogin_MovesToLoginState()
    {
        var controller = new LoginFlowController();

        controller.ShowLogin();

        Assert.AreEqual(LoginViewState.Login, controller.CurrentState);
    }

    [TestMethod]
    public void ShowSignUp_MovesToSignUpState()
    {
        var controller = new LoginFlowController();

        controller.ShowSignUp();

        Assert.AreEqual(LoginViewState.SignUp, controller.CurrentState);
    }

    [TestMethod]
    public void ShowLanding_FromLogin_ReturnsToLanding()
    {
        var controller = new LoginFlowController();
        controller.ShowLogin();

        controller.ShowLanding();

        Assert.AreEqual(LoginViewState.Landing, controller.CurrentState);
        Assert.IsNull(controller.WelcomeOwnerName);
    }

    [TestMethod]
    public void ShowLanding_FromSignUp_ReturnsToLanding()
    {
        var controller = new LoginFlowController();
        controller.ShowSignUp();

        controller.ShowLanding();

        Assert.AreEqual(LoginViewState.Landing, controller.CurrentState);
        Assert.IsNull(controller.WelcomeOwnerName);
    }

    [TestMethod]
    public void ShowWelcome_StoresOwnerNameAndMovesToWelcome()
    {
        var controller = new LoginFlowController();

        controller.ShowWelcome("Maria Santos");

        Assert.AreEqual(LoginViewState.Welcome, controller.CurrentState);
        Assert.AreEqual("Maria Santos", controller.WelcomeOwnerName);
    }
}
