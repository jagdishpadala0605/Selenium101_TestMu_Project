using NUnit.Framework;
using OpenQA.Selenium;
using Selenium101.Driver;
using Selenium101.Pages;

namespace Selenium101.Tests;

[TestFixtureSource(nameof(BrowserConfigurations))]
[Parallelizable(ParallelScope.Fixtures)]
public sealed class TestMuAITests
{
    private readonly BrowserConfiguration configuration;

    private IWebDriver driver = null!;
    private SeleniumPlaygroundPage page = null!;

    public TestMuAITests(BrowserConfiguration configuration)
    {
        this.configuration = configuration;
    }

    private static IEnumerable<object[]> BrowserConfigurations()
    {
        yield return new object[]
        {
            new BrowserConfiguration(
                "chrome",
                "128.0",
                "Windows 10",
                "Chrome 128 - Windows 10")
        };

        yield return new object[]
        {
            new BrowserConfiguration(
                "MicrosoftEdge",
                "127.0",
                "macOS Ventura",
                "Edge 127 - macOS Ventura")
        };

        yield return new object[]
        {
            new BrowserConfiguration(
                "firefox",
                "130.0",
                "Windows 11",
                "Firefox 130 - Windows 11")
        };

        yield return new object[]
        {
            new BrowserConfiguration(
                "internet explorer",
                "11.0",
                "Windows 10",
                "Internet Explorer 11 - Windows 10")
        };
    }

    // ============================================================
    // SETUP
    // ============================================================

    [SetUp]
    public void SetUp()
    {
        driver = DriverSetup.CreateRemoteDriver(configuration);

        // Always keep implicit wait at zero when using explicit waits.
        driver.Manage().Timeouts().ImplicitWait =
            TimeSpan.Zero;

        page = new SeleniumPlaygroundPage(driver);

        page.Open();
    }

    // ============================================================
    // TEARDOWN
    // ============================================================

    [TearDown]
    public void TearDown()
    {
        try
        {
            driver?.Quit();
        }
        catch
        {
            // Do not hide the original test failure if
            // the remote session has already terminated.
        }
    }

    // ============================================================
    // SCENARIO 1
    // ============================================================

    [Test]
    public void Scenario1_SimpleFormDemo()
    {
        const string message =
            "Welcome to TestMu AI";

        page.ClickSimpleFormDemo();

        Assert.That(
            driver.Url,
            Does.Contain("simple-form-demo"),
            "The URL should contain simple-form-demo.");

        page.EnterMessage(message);

        page.ClickGetCheckedValue();

        var displayedMessage =
            page.GetDisplayedMessage(message);

        Assert.That(
            displayedMessage,
            Is.EqualTo(message),
            "The displayed message should match the entered message.");
    }

    // ============================================================
    // SCENARIO 2
    // ============================================================

    [Test]
    public void Scenario2_DragAndDropSlider()
    {
        page.ClickDragAndDropSliders();

        page.SetSliderTo95();

        var sliderValue =
            page.GetSliderValue();

        Assert.That(
            sliderValue,
            Is.EqualTo("95"),
            "The slider range value should be 95.");
    }

    // ============================================================
    // SCENARIO 3
    // ============================================================

    [Test]
    public void Scenario3_InputFormSubmit()
    {
        page.ClickInputFormSubmit();

        // --------------------------------------------------------
        // Submit empty form
        // --------------------------------------------------------

        page.SubmitInputForm();

        var validationMessage =
            page.GetNameValidationMessage();

        Assert.That(
            validationMessage,
            Does.Match("(?i)(required|fill\\s+(in|out))"),
            $"Unexpected browser validation message: '{validationMessage}'");

        // --------------------------------------------------------
        // Fill complete form
        // --------------------------------------------------------

        page.FillInputForm();

        page.SubmitInputForm();

        // --------------------------------------------------------
        // Verify successful submission
        // --------------------------------------------------------

        var successMessage =
            page.GetSuccessMessage();

        Assert.That(
            successMessage,
            Does.Contain(
                "Thanks for contacting us, we will get back to you shortly."),
            "The successful submission message should be displayed.");
    }
}