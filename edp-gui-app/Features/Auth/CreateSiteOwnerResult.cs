namespace edp_gui_app;

public enum CreateSiteOwnerStatus
{
    Created,
    EmailAlreadyExists
}

public sealed record CreateSiteOwnerResult(CreateSiteOwnerStatus Status, SiteOwner? Owner);
