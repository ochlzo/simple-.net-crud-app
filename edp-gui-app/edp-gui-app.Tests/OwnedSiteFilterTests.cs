using edp_gui_app;

namespace edp_gui_app.Tests;

[TestClass]
public sealed class OwnedSiteFilterTests
{
    private static readonly IReadOnlyList<OwnedSite> Sites =
    [
        new OwnedSite(12, "North Tower"),
        new OwnedSite(34, "Harbor Point"),
        new OwnedSite(56, "South Annex")
    ];

    [TestMethod]
    public void Apply_ReturnsAllSites_WhenSearchIsEmpty()
    {
        var filtered = OwnedSiteFilter.Apply(Sites, string.Empty);

        CollectionAssert.AreEqual(Sites.ToArray(), filtered.ToArray());
    }

    [TestMethod]
    public void Apply_FiltersBySiteName()
    {
        var filtered = OwnedSiteFilter.Apply(Sites, "harbor");

        CollectionAssert.AreEqual(new[] { Sites[1] }, filtered.ToArray());
    }

    [TestMethod]
    public void Apply_FiltersBySiteIdText()
    {
        var filtered = OwnedSiteFilter.Apply(Sites, "56");

        CollectionAssert.AreEqual(new[] { Sites[2] }, filtered.ToArray());
    }
}
