using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace Selenium101.Driver;

public sealed record BrowserConfiguration(
    string Browser,
    string BrowserVersion,
    string Platform,
    string TestName);

public static class DriverSetup
{
    private const string HubUrl =
        "https://hub.lambdatest.com/wd/hub";

    public static IWebDriver CreateRemoteDriver(
        BrowserConfiguration config)
    {
        var username =
            Environment.GetEnvironmentVariable("LT_USERNAME");

        var accessKey =
            Environment.GetEnvironmentVariable("LT_ACCESS_KEY");

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new InvalidOperationException(
                "LT_USERNAME environment variable is not set.");
        }

        if (string.IsNullOrWhiteSpace(accessKey))
        {
            throw new InvalidOperationException(
                "LT_ACCESS_KEY environment variable is not set.");
        }

        var options =
            CreateBrowserOptions(config);

        var ltOptions =
            new Dictionary<string, object>
            {
                ["username"] = username,
                ["accessKey"] = accessKey,

                ["build"] =
                    "Selenium 101 - TestMu AI Assignment",

                ["name"] =
                    $"{config.TestName} | " +
                    $"{config.Browser} " +
                    $"{config.BrowserVersion} | " +
                    $"{config.Platform}",

                ["video"] = true,
                ["visual"] = true,
                ["network"] = true,
                ["console"] = true
            };

        options.AddAdditionalOption(
            "LT:Options",
            ltOptions);

        return new RemoteWebDriver(
            new Uri(HubUrl),
            options.ToCapabilities(),
            TimeSpan.FromSeconds(90));
    }

    private static DriverOptions CreateBrowserOptions(
        BrowserConfiguration config)
    {
        return config.Browser.ToLowerInvariant() switch
        {
            "chrome" =>
                CreateChromeOptions(config),

            "microsoftedge" or "edge" =>
                CreateEdgeOptions(config),

            "firefox" =>
                CreateFirefoxOptions(config),

            "internet explorer" or "ie" =>
                CreateInternetExplorerOptions(config),

            _ => throw new ArgumentException(
                $"Unsupported browser: {config.Browser}",
                nameof(config))
        };
    }

    private static ChromeOptions CreateChromeOptions(
        BrowserConfiguration config)
    {
        var options = new ChromeOptions
        {
            BrowserVersion = config.BrowserVersion,
            PlatformName = config.Platform
        };

        options.AddArgument("--start-maximized");

        return options;
    }

    private static EdgeOptions CreateEdgeOptions(
        BrowserConfiguration config)
    {
        var options = new EdgeOptions
        {
            BrowserVersion = config.BrowserVersion,
            PlatformName = config.Platform
        };

        options.AddArgument("--start-maximized");

        return options;
    }

    private static FirefoxOptions CreateFirefoxOptions(
        BrowserConfiguration config)
    {
        return new FirefoxOptions
        {
            BrowserVersion = config.BrowserVersion,
            PlatformName = config.Platform
        };
    }

    private static InternetExplorerOptions
        CreateInternetExplorerOptions(
            BrowserConfiguration config)
    {
        var options =
            new InternetExplorerOptions
            {
                BrowserVersion = config.BrowserVersion,
                PlatformName = config.Platform
            };

        options
            .IntroduceInstabilityByIgnoringProtectedModeSettings = true;

        options.IgnoreZoomLevel = true;

        return options;
    }

    public static IWebDriver CreateLocalChrome()
    {
        var options = new ChromeOptions();

        options.AddArgument("--start-maximized");

        return new ChromeDriver(options);
    }
}