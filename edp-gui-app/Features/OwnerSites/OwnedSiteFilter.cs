namespace edp_gui_app;

public static class OwnedSiteFilter
{
    public static IReadOnlyList<OwnedSite> Apply(IEnumerable<OwnedSite> sites, string? searchText)
    {
        var normalizedSearch = searchText?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedSearch))
        {
            return sites.ToArray();
        }

        return sites
            .Where(site =>
                site.SiteName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                site.SiteId.ToString().Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }
}
