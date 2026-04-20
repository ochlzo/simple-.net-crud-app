namespace edp_gui_app;

public sealed partial class MainAppWindow
{
    private (
        Control Panel,
        TextBox Search,
        Button Refresh,
        DataGridView Grid,
        Label Status) BuildOwnerSitesPanel()
    {
        var headerText = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Margin = new Padding(0)
        };
        headerText.Controls.Add(new Label
        {
            AutoSize = true,
            Text = "Manage sites that you own here.",
            Font = new Font(Font.FontFamily, 20, FontStyle.Bold)
        });
        headerText.Controls.Add(new Label
        {
            AutoSize = true,
            MaximumSize = new Size(520, 0),
            Text = "Review the properties under your account, search quickly, and open a site record for the next management step."
        });

        var refresh = new Button
        {
            AutoSize = true,
            Text = "Refresh",
            Padding = new Padding(10, 6, 10, 6)
        };
        refresh.Click += OnRefreshSitesClicked;

        var logout = new Button
        {
            AutoSize = true,
            Text = "Logout",
            Padding = new Padding(10, 6, 10, 6)
        };
        logout.Click += (_, _) => ShowLandingView();

        var actions = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        actions.Controls.Add(refresh);
        actions.Controls.Add(logout);

        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 2,
            Margin = new Padding(0, 0, 0, 18)
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        header.Controls.Add(headerText, 0, 0);
        header.Controls.Add(actions, 1, 0);

        var searchLabel = new Label
        {
            AutoSize = true,
            Text = "Search",
            Anchor = AnchorStyles.Left
        };

        var search = new TextBox
        {
            Dock = DockStyle.Fill,
            PlaceholderText = "Search by site name or site ID"
        };
        search.TextChanged += (_, _) => ApplyOwnedSiteFilter();

        var addSite = new Button
        {
            AutoSize = true,
            Text = "Add Site",
            Padding = new Padding(10, 6, 10, 6),
            Anchor = AnchorStyles.Right
        };
        addSite.Click += OnAddSiteClicked;

        var searchRow = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 3,
            Margin = new Padding(0, 0, 0, 12)
        };
        searchRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        searchRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        searchRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        searchRow.Controls.Add(searchLabel, 0, 0);
        searchRow.Controls.Add(search, 1, 0);
        searchRow.Controls.Add(addSite, 2, 0);

        var status = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(720, 0),
            Margin = new Padding(0, 0, 0, 12)
        };

        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            MultiSelect = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            DataSource = _sitesBindingSource
        };
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "SiteId",
            HeaderText = "Site ID",
            DataPropertyName = nameof(OwnedSite.SiteId),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        });
        grid.Columns.Add(new DataGridViewLinkColumn
        {
            Name = "SiteName",
            HeaderText = "Site Name",
            DataPropertyName = nameof(OwnedSite.SiteName),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            LinkBehavior = LinkBehavior.HoverUnderline,
            TrackVisitedState = false
        });
        grid.Columns.Add(new DataGridViewButtonColumn
        {
            Name = "EditSite",
            HeaderText = string.Empty,
            Text = "Edit",
            UseColumnTextForButtonValue = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        });
        grid.Columns.Add(new DataGridViewButtonColumn
        {
            Name = "DeleteSite",
            HeaderText = string.Empty,
            Text = "Delete",
            UseColumnTextForButtonValue = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        });
        grid.CellContentClick += OnSitesGridCellContentClick;

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            BackColor = Color.White,
            Padding = new Padding(26)
        };
        panel.RowStyles.Add(new RowStyle());
        panel.RowStyles.Add(new RowStyle());
        panel.RowStyles.Add(new RowStyle());
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.Controls.Add(header, 0, 0);
        panel.Controls.Add(searchRow, 0, 1);
        panel.Controls.Add(status, 0, 2);
        panel.Controls.Add(grid, 0, 3);

        return (panel, search, refresh, grid, status);
    }

    private (Control Panel, Label SiteIdValue, Label SiteNameValue) BuildSiteDetailsPanel()
    {
        var heading = new Label
        {
            AutoSize = true,
            Text = "Site Details",
            Font = new Font(Font.FontFamily, 20, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 8)
        };

        var description = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(560, 0),
            Text = "This is a placeholder page while the full site management workflow is being built.",
            Margin = new Padding(0, 0, 0, 18)
        };

        var siteIdValue = new Label { AutoSize = true, Text = "-" };
        var siteNameValue = new Label { AutoSize = true, Text = "-" };

        var details = new TableLayoutPanel
        {
            AutoSize = true,
            ColumnCount = 2,
            RowCount = 2,
            Margin = new Padding(0, 0, 0, 24)
        };
        details.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        details.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        details.Controls.Add(new Label { AutoSize = true, Text = "Site ID", Font = new Font(Font, FontStyle.Bold) }, 0, 0);
        details.Controls.Add(siteIdValue, 1, 0);
        details.Controls.Add(new Label { AutoSize = true, Text = "Site Name", Font = new Font(Font, FontStyle.Bold) }, 0, 1);
        details.Controls.Add(siteNameValue, 1, 1);

        var back = new Button
        {
            AutoSize = true,
            Text = "Back to Sites",
            Padding = new Padding(10, 6, 10, 6)
        };
        back.Click += (_, _) => ShowOwnerSitesView();

        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.White,
            Padding = new Padding(26)
        };
        layout.Controls.Add(heading);
        layout.Controls.Add(description);
        layout.Controls.Add(details);
        layout.Controls.Add(back);

        return (layout, siteIdValue, siteNameValue);
    }
}
