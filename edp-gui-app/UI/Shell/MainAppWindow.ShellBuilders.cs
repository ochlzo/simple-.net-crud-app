namespace edp_gui_app;

public sealed partial class MainAppWindow
{
    private Panel BuildSidebar()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(32, 64, 96),
            Padding = new Padding(24)
        };

        var badge = new Label
        {
            AutoSize = true,
            Text = "Dashboard",
            ForeColor = Color.Gainsboro,
            Font = new Font(Font.FontFamily, 10, FontStyle.Bold)
        };

        var title = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(320, 0),
            Text = "Site Management System",
            ForeColor = Color.White,
            Font = new Font(Font.FontFamily, 24, FontStyle.Bold),
            Margin = new Padding(0, 18, 0, 16)
        };

        var description = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(300, 0),
            Text = "Manage site owner access and review the sites tied to each owner from one desktop window.",
            ForeColor = Color.Gainsboro
        };

        var stack = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };
        stack.Controls.Add(badge);
        stack.Controls.Add(title);
        stack.Controls.Add(description);
        panel.Controls.Add(stack);

        return panel;
    }

    private static FlowLayoutPanel BuildCardPanel(string heading, string description)
    {
        var headingLabel = new Label
        {
            AutoSize = true,
            Text = heading,
            Font = new Font(FontFamily.GenericSansSerif, 20, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 10)
        };

        var descriptionLabel = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(420, 0),
            Text = description,
            Margin = new Padding(0, 0, 0, 18)
        };

        var stack = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.White,
            Padding = new Padding(26)
        };
        stack.Controls.Add(headingLabel);
        stack.Controls.Add(descriptionLabel);
        return stack;
    }
}
