# Site Owner Login And Sign-Up Design

**Goal:** Update the current WinForms app so it opens on a dashboard-style landing view, shows `Site Management System`, supports both login and sign-up in the same window, and routes successful authentication or account creation to the welcome state.

**Confirmed Facts**
- The repo currently contains a .NET 10 WinForms app.
- The current connection string points to `site_management` on `127.0.0.1:3306`.
- The `site_owner` table exists and has these columns:
  - `owner_id`
  - `owner_name`
  - `password`
  - `owner_email`
- The repo already contains a small auth service and real-database tests.

**Chosen Approach**
- Keep a single WinForms window.
- Keep the existing MySQL connection details and parameterized queries.
- Use simple state-driven UI switching inside one form.
- Keep files small and focused.

**UI States**
- `landing`
  - show `Site Management System`
  - show a short intro message
  - show a `Login` button
  - show a `Sign Up` button
- `login`
  - reveal the login panel in the same window only after the user clicks `Login`
  - show email and password inputs
  - show submit and back actions
  - show an inline link: `No account? Go to signup`
- `signup`
  - reveal the sign-up panel in the same content region
  - show `owner_name`, `owner_email`, and `password` inputs
  - show submit and back actions
  - show an inline link: `Already have an account? Go to login`
- `welcome`
  - replace the auth area with a simple welcome state
  - show `Welcome, <owner_name>`

**State Flow**
1. App opens in the `landing` state.
2. User clicks `Login`.
3. The form switches to the `login` state in the same window.
4. The user enters email and password.
5. The form validates empty fields before querying.
6. The auth service runs:
   `SELECT owner_id, owner_name, owner_email FROM site_owner WHERE owner_email = @email AND password = @password LIMIT 1`
7. If credentials are invalid, the form stays in `login` and shows `Invalid email or password`.
8. If credentials are valid, the form switches to `welcome` and shows `Welcome, <owner_name>`.
9. The user can also click `Sign Up` from `landing` or the inline link from `login`.
10. The form switches to the `signup` state in the same window.
11. The user enters `owner_name`, `owner_email`, and `password`.
12. The form validates empty fields before querying.
13. The sign-up service first checks whether `owner_email` already exists in `site_owner`.
14. If the email already exists, the form stays in `signup` and shows an inline duplicate-email error.
15. If the email is new, the service inserts:
    `INSERT INTO site_owner (owner_name, owner_email, password) VALUES (@name, @email, @password)`
16. After a successful insert, the form switches to `welcome` and shows `Welcome, <owner_name>`.
17. The sign-up view includes an inline link back to `login`.
18. If the user clicks `Back` from the login or sign-up state, the form returns to `landing`.

**Layout Direction**
- Keep a stable dashboard shell feel across states.
- The main title area remains visible instead of navigating to a separate window.
- The auth section is hidden on startup and only appears after clicking `Login` or `Sign Up`.
- The welcome state reuses the same content region that previously showed the login or sign-up form.

**Error Handling**
- Empty email or password: block the query and show a validation message in the login area.
- Empty name, email, or password on sign-up: block the query and show a validation message in the sign-up area.
- Invalid credentials: stay on the login state and show a short error message.
- Duplicate `owner_email`: stay on the sign-up state and show a short inline error message.
- Database exceptions: stay in the same window and show a short failure message without crashing.

**Testing**
- Keep the existing automated database tests against the real connection.
- Add focused state-controller coverage for the new `signup` state.
- Preserve coverage for:
  - valid credentials return the matching owner
  - invalid password returns no owner
  - creating a new owner inserts a row and returns the created owner
  - creating a duplicate owner email returns a duplicate-email result

**Security Note**
- This design matches the current schema exactly and compares the submitted password with the value stored in the `password` column.
- If the database stores plain text passwords, this is suitable only for a sample app, not for production authentication.
