# Site Owner Login And Sign-Up Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a same-window WinForms auth flow that starts on a `Site Management System` landing view, supports both login and sign-up, and shows a welcome state after successful login or account creation.

**Architecture:** Keep one WinForms shell, extend the existing MySQL auth service with a small owner-creation path, and keep a small state controller so the landing, login, sign-up, and welcome transitions stay explicit and testable. The form will render each state inside one content region instead of opening another window.

**Tech Stack:** .NET 10 WinForms, MySqlConnector, MSTest

---

### Task 1: Extend testable screen state logic

**Files:**
- Modify: `LoginViewState.cs`
- Modify: `LoginFlowController.cs`
- Modify: `edp-gui-app.Tests/LoginFlowControllerTests.cs`

- [ ] **Step 1: Write the failing state tests for sign-up**

Cover:
- initial state is `Landing`
- clicking login transitions to `Login`
- clicking sign-up transitions to `SignUp`
- back transitions to `Landing`
- successful login or sign-up transitions to `Welcome`

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test .\edp-gui-app.Tests\edp-gui-app.Tests.csproj --filter LoginFlowControllerTests`
Expected: FAIL because `SignUp` support does not exist yet

- [ ] **Step 3: Implement the minimal controller changes**

Add a `SignUp` state and a `ShowSignUp()` transition while keeping welcome-name handling unchanged.

- [ ] **Step 4: Run the focused test to verify it passes**

Run: `dotnet test .\edp-gui-app.Tests\edp-gui-app.Tests.csproj --filter LoginFlowControllerTests`
Expected: PASS

### Task 2: Add owner-creation database behavior with TDD

**Files:**
- Modify: `SiteOwnerAuthService.cs`
- Modify: `edp-gui-app.Tests/SiteOwnerAuthServiceTests.cs`

- [ ] **Step 1: Write the failing database tests**

Cover:
- creating a new owner with a unique email succeeds
- creating a second owner with the same email returns a duplicate-email result

- [ ] **Step 2: Run the focused database tests to verify they fail**

Run: `dotnet test .\edp-gui-app.Tests\edp-gui-app.Tests.csproj --filter SiteOwnerAuthServiceTests`
Expected: FAIL because the creation API does not exist yet

- [ ] **Step 3: Implement the minimal owner-creation path**

Add a create-owner result type and a service method that checks for an existing `owner_email`, inserts when unique, and returns the created owner.

- [ ] **Step 4: Run the focused database tests to verify they pass**

Run: `dotnet test .\edp-gui-app.Tests\edp-gui-app.Tests.csproj --filter SiteOwnerAuthServiceTests`
Expected: PASS

### Task 3: Refactor the form to the approved same-window auth flow

**Files:**
- Modify: `MainAppWindow.cs`
- Modify: `MainAppWindow.Panels.cs`

- [ ] **Step 1: Extend the shell with a sign-up panel**

Keep the dashboard title visible and add a `SignUp` panel in the same content region.

- [ ] **Step 2: Add the new navigation actions**

- landing shows `Login` and `Sign Up`
- login shows `No account? Go to signup`
- sign-up shows `Already have an account? Go to login`
- login/sign-up back actions return to `Landing`

- [ ] **Step 3: Wire the async submit handlers**

- login validates email/password and uses `AuthenticateAsync`
- sign-up validates name/email/password and uses the new create-owner method
- both success paths switch to `Welcome`
- both failure paths stay in-window with inline status text

- [ ] **Step 4: Prevent duplicate submits**

Disable the relevant auth actions while a login or sign-up request is in flight.

### Task 4: Verify the complete app

**Files:**
- Modify: `MainAppWindow.cs`
- Modify: `MainAppWindow.Panels.cs`

- [ ] **Step 1: Run the full test suite**

Run: `dotnet test .\edp-gui-app.Tests\edp-gui-app.Tests.csproj`
Expected: all tests pass

- [ ] **Step 2: Run a final build**

Run: `dotnet build .\edp-gui-app.csproj`
Expected: build succeeds with the revised same-window auth flow

- [ ] **Step 3: Smoke-start the desktop app**

Run the built EXE briefly.
Expected: the window starts without an immediate crash
