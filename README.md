# Selenium 101 - TestMu AI Selenium Grid Assignment

This project implements the three requested Selenium scenarios in **C# + .NET 8 + NUnit 4 + Selenium WebDriver 4** using the Selenium 4 Remote WebDriver connection to TestMu AI.

## Project structure

```text
Selenium101_TestMu_Project/
├── Driver/
│   └── DriverSetup.cs
├── Pages/
│   └── SeleniumPlaygroundPage.cs
├── Tests/
│   ├── AssemblyInfo.cs
│   └── TestMuAITests.cs
├── Selenium101.csproj
├── Selenium101.sln
├── test.runsettings
└── README.md
```

## Requested browser/platform matrix

The cloud test fixture is data-driven and creates four independent NUnit fixtures:

| Browser | Version | Platform |
|---|---:|---|
| Chrome | 128.0 | Windows 10 |
| Microsoft Edge | 127.0 | macOS Ventura |
| Firefox | 130.0 | Windows 11 |
| Internet Explorer | 11.0 | Windows 10 |

TestMu AI documents support for Chrome, Firefox, Edge Chromium and IE on the relevant desktop operating systems. The exact browser/OS availability can change, so verify the requested combinations in the TestMu AI Capabilities Generator before the final run.

## Framework and packages

The project targets .NET 8 and currently uses these stable package versions:

- Microsoft.NET.Test.Sdk 18.9.0
- NUnit 4.6.1
- NUnit3TestAdapter 6.2.0
- Selenium.WebDriver 4.47.0

Do not use the NUnit 5 beta for this assignment.

## Credentials

The credentials are intentionally not stored in source code.

### PowerShell

```powershell
$env:LT_USERNAME="YOUR_USERNAME"
$env:LT_ACCESS_KEY="YOUR_ACCESS_KEY"
```

### Command Prompt

```cmd
set LT_USERNAME=YOUR_USERNAME
set LT_ACCESS_KEY=YOUR_ACCESS_KEY
```

TestMu AI's Selenium documentation uses:

```text
https://{username}:{accessKey}@hub.lambdatest.com/wd/hub
```

for the RemoteWebDriver connection.

## Restore

From the project folder:

```cmd
dotnet restore
```

## Run the complete cloud assignment

```cmd
dotnet test --settings test.runsettings --filter FullyQualifiedName~TestMuAITests
```

All four browser configurations are generated from the `BrowserConfigurations()` data source.

## Parallel execution

`TestMuAITests` uses:

```csharp
[Parallelizable(ParallelScope.All)]
```

Therefore:

- the four browser/platform fixtures can execute in parallel;
- the three scenario methods can execute in parallel;
- each test instance gets its own WebDriver session;
- setup and teardown are separate NUnit lifecycle methods and are not decorated with `[Test]`.

`AssemblyInfo.cs` also limits NUnit to four parallel workers.

## 20-second timeout

Each scenario contains:

```csharp
[CancelAfter(20000)]
```

This is the NUnit 4 cooperative 20-second cancellation timeout for .NET 8. NUnit documents `CancelAfter` as the cooperative cancellation mechanism for modern .NET, while the legacy `Timeout` mechanism relies on thread abort and is not supported on modern .NET targets.

The RemoteWebDriver session-creation timeout is separate and is intentionally longer so that cloud session negotiation does not fail because of a short HTTP connection timeout.

## Scenario implementation

### Scenario 1 - Simple Form Demo

- Opens the Selenium Playground.
- Clicks `Simple Form Demo`.
- Verifies the URL contains `simple-form-demo`.
- Stores `Welcome to TestMu AI` in a C# variable.
- Enters the variable into the message box.
- Clicks `Get Checked Value`.
- Verifies the result exactly matches the entered message.

### Scenario 2 - Drag & Drop Sliders

- Opens `Drag & Drop Sliders`.
- Locates the slider initially set to 15.
- Moves it to 95 using the HTML range control's keyboard interaction.
- Verifies the final range value is exactly `95`.

The final value is what the assignment asks to validate. Keyboard movement of an HTML range input is used because it is substantially more deterministic than pixel-based mouse dragging across different remote browser resolutions.

### Scenario 3 - Input Form Submit

- Opens `Input Form Submit`.
- Clicks Submit without entering information.
- Reads the browser's HTML5 `validationMessage`.
- Asserts `Please fill in this field.`.
- Fills all fields.
- Selects `United States` with `SelectElement.SelectByText()`.
- Submits the form.
- Verifies `Thanks for contacting us, we will get back to you shortly.`.

## Locator requirement

More than three locator strategies are used:

1. **XPath**
   - Navigation links
   - Submit button
   - Success message

2. **ID**
   - Message box
   - Email
   - Company
   - City
   - Address fields
   - State
   - ZIP

3. **Name**
   - Name
   - Password
   - Country

4. **CSS Selector**
   - Slider
   - Success message fallback

This exceeds the required minimum of three locator strategies.

## Local execution

The assignment is designed to run on the TestMu AI Selenium Grid. The project therefore uses `RemoteWebDriver` for the test fixture.

## Important IE note

Selenium's own documentation states that standalone Internet Explorer support was officially discontinued in June 2022. However, TestMu AI's current supported-browser documentation still lists IE 11 on Windows 10. Because IE 11 is explicitly required by this assignment, the project retains an `InternetExplorerOptions` configuration for the TestMu AI cloud session.

If TestMu AI rejects the exact IE capability in your account, use the current capability generated by TestMu AI's Capabilities Generator while keeping the test code unchanged.

## Git

Do not commit credentials.

```cmd
git add .
git commit -m "Update Selenium 101 TestMu AI assignment"
git push
```
