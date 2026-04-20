namespace edp_gui_app;

public sealed partial class MainAppWindow
{
    private Form BuildAddSiteDialog(int ownerId)
    {
        return BuildSiteNameDialog(
            "Add Site",
            "Create",
            string.Empty,
            siteName => _authService.CreateSiteAsync(ownerId, siteName));
    }

    private Form BuildEditSiteDialog(int ownerId, OwnedSite site)
    {
        return BuildSiteNameDialog(
            "Edit Site",
            "Update",
            site.SiteName,
            siteName => _authService.UpdateSiteAsync(site.SiteId, ownerId, siteName));
    }

    private Form BuildSiteNameDialog(
        string title,
        string submitText,
        string initialSiteName,
        Func<string, Task> submitAsync)
    {
        var siteNameLabel = new Label
        {
            AutoSize = true,
            Text = "Site Name",
            Anchor = AnchorStyles.Left
        };

        var siteNameTextBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Text = initialSiteName
        };

        var errorLabel = new Label
        {
            AutoSize = true,
            ForeColor = Color.Firebrick,
            MaximumSize = new Size(320, 0)
        };

        var submitButton = new Button
        {
            AutoSize = true,
            Text = submitText,
            Padding = new Padding(10, 6, 10, 6)
        };

        var cancelButton = new Button
        {
            AutoSize = true,
            Text = "Cancel",
            Padding = new Padding(10, 6, 10, 6),
            DialogResult = DialogResult.Cancel
        };

        var buttons = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 8, 0, 0)
        };
        buttons.Controls.Add(submitButton);
        buttons.Controls.Add(cancelButton);

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(18)
        };
        content.Controls.Add(siteNameLabel, 0, 0);
        content.Controls.Add(siteNameTextBox, 0, 1);
        content.Controls.Add(errorLabel, 0, 2);
        content.Controls.Add(buttons, 0, 3);

        var dialog = new Form
        {
            Text = title,
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            ShowInTaskbar = false,
            ClientSize = new Size(360, 150)
        };
        dialog.Controls.Add(content);
        dialog.AcceptButton = submitButton;
        dialog.CancelButton = cancelButton;

        submitButton.Click += async (_, _) =>
        {
            var siteName = siteNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(siteName))
            {
                errorLabel.Text = "Enter a site name.";
                return;
            }

            submitButton.Enabled = false;
            cancelButton.Enabled = false;
            errorLabel.Text = string.Empty;

            try
            {
                await submitAsync(siteName);
                dialog.DialogResult = DialogResult.OK;
                dialog.Close();
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message;
                submitButton.Enabled = true;
                cancelButton.Enabled = true;
            }
        };

        return dialog;
    }
}
