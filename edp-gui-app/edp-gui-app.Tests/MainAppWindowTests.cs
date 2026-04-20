using edp_gui_app;

namespace edp_gui_app.Tests;

[TestClass]
public sealed class MainAppWindowTests
{
    [TestMethod]
    public void MainAppWindow_CanBeConstructed()
    {
        using var window = new MainAppWindow(new SiteOwnerAuthService("Server=127.0.0.1;Database=test;"));

        Assert.IsNotNull(window);
    }
}
