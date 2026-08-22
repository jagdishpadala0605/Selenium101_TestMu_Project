using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Selenium101.Pages;

public sealed class SeleniumPlaygroundPage
{
    private const string PlaygroundUrl =
         "https://www.testmuai.com/selenium-playground/";

    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    // ============================================================
    // Simple Form Demo
    // ============================================================

    private readonly By simpleFormLink =
        By.XPath("//a[normalize-space()='Simple Form Demo']");

    private readonly By messageBox =
        By.Id("user-message");

    private readonly By getCheckedValueButton =
        By.Id("showInput");

    private readonly By messageResult =
        By.Id("message");

    // ============================================================
    // Drag & Drop Sliders
    // ============================================================

    private readonly By slidersLink =
        By.XPath("//a[normalize-space()='Drag & Drop Sliders']");

    private readonly By sliderInputs =
        By.CssSelector("input[type='range']");

    // ============================================================
    // Input Form Submit
    // ============================================================

    private readonly By inputFormLink =
        By.XPath("//a[normalize-space()='Input Form Submit']");

    private readonly By name =
        By.Name("name");

    private readonly By email =
        By.Id("inputEmail4");

    private readonly By password =
        By.Name("password");

    private readonly By company =
        By.Id("company");

    private readonly By website =
        By.Id("websitename");

    private readonly By country =
        By.Name("country");

    private readonly By city =
        By.Id("inputCity");

    private readonly By address1 =
        By.Id("inputAddress1");

    private readonly By address2 =
        By.Id("inputAddress2");

    private readonly By state =
        By.Id("inputState");

    private readonly By zip =
        By.Id("inputZip");

    private readonly By submitButton =
        By.XPath("//button[normalize-space()='Submit']");

    private readonly By successMessage =
        By.XPath(
            "//*[contains(normalize-space(.), " +
            "'Thanks for contacting us, we will get back to you shortly.')]");

    // ============================================================
    // Constructor
    // ============================================================

    public SeleniumPlaygroundPage(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(
            new SystemClock(),
            driver,
            TimeSpan.FromSeconds(20),
            TimeSpan.FromMilliseconds(500));

        wait.IgnoreExceptionTypes(
            typeof(NoSuchElementException),
            typeof(StaleElementReferenceException));
    }

    // ============================================================
    // Open Playground
    // ============================================================

    public void Open()
    {
        driver.Navigate().GoToUrl(PlaygroundUrl);

        WaitForPageReady();
    }

    // ============================================================
    // Scenario 1 - Simple Form Demo
    // ============================================================

    public void ClickSimpleFormDemo()
    {
        Click(simpleFormLink);

        wait.Until(d =>
            d.Url.Contains(
                "simple-form-demo",
                StringComparison.OrdinalIgnoreCase));
    }

    public void EnterMessage(string message)
    {
        var element = Find(messageBox);

        element.Clear();
        element.SendKeys(message);
    }

    public void ClickGetCheckedValue()
    {
        Click(getCheckedValueButton);
    }

    public string GetDisplayedMessage(string expectedMessage)
    {
        return wait.Until(d =>
        {
            try
            {
                var element = d.FindElement(messageResult);

                var text = element.Text?.Trim();

                if (string.Equals(
                        text,
                        expectedMessage,
                        StringComparison.Ordinal))
                {
                    return text;
                }

                return null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        })!;
    }

    // ============================================================
    // Scenario 2 - Drag & Drop Sliders
    // ============================================================

    public void ClickDragAndDropSliders()
    {
        Click(slidersLink);

        wait.Until(d =>
            d.Url.Contains(
                "drag-and-drop-sliders",
                StringComparison.OrdinalIgnoreCase));
    }

    public void SetSliderTo95()
    {
        /*
         * IMPORTANT:
         *
         * Do not cache the slider element for the entire operation.
         *
         * The TestMu slider page can update/re-render the range element,
         * which causes StaleElementReferenceException.
         *
         * Therefore we locate the slider again before every interaction.
         */

        wait.Until(d =>
        {
            try
            {
                return d.FindElements(sliderInputs)
                    .Any(x =>
                        (x.GetAttribute("value") ?? string.Empty) == "15");
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });

        // Re-find the slider immediately before clicking.
        IWebElement slider = FindSliderWithValue("15");

        ScrollIntoView(slider);

        try
        {
            slider.Click();
        }
        catch (StaleElementReferenceException)
        {
            // DOM changed - get a fresh element.
            slider = FindSliderWithValue("15");
            ScrollIntoView(slider);
            slider.Click();
        }

        /*
         * Use keyboard movement on the range control.
         *
         * 15 -> End = 100
         * 100 -> five ArrowLeft = 95
         */

        try
        {
            slider.SendKeys(Keys.End);

            for (int i = 0; i < 5; i++)
            {
                slider.SendKeys(Keys.ArrowLeft);
            }
        }
        catch (StaleElementReferenceException)
        {
            /*
             * If the control was re-rendered, find the new slider.
             *
             * The exact current value may now be 100 or another value,
             * so use JavaScript as a reliable final synchronization.
             */
            SetSliderValueUsingJavaScript(95);
        }

        // Final verification.
        wait.Until(d =>
        {
            try
            {
                return d.FindElements(sliderInputs)
                    .Any(x =>
                        (x.GetAttribute("value") ?? string.Empty) == "95");
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });
    }

    public string GetSliderValue()
    {
        return wait.Until(d =>
        {
            try
            {
                var sliders = d.FindElements(sliderInputs);

                foreach (var slider in sliders)
                {
                    var value =
                        slider.GetAttribute("value");

                    if (value == "95")
                    {
                        return value;
                    }
                }

                return null;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        })!;
    }

    private IWebElement FindSliderWithValue(string expectedValue)
    {
        return wait.Until(d =>
        {
            try
            {
                foreach (var element in d.FindElements(sliderInputs))
                {
                    if ((element.GetAttribute("value") ?? string.Empty)
                        == expectedValue)
                    {
                        return element;
                    }
                }

                return null;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        })!;
    }

    private void SetSliderValueUsingJavaScript(int value)
    {
        wait.Until(d =>
        {
            try
            {
                var sliders = d.FindElements(sliderInputs);

                var slider = sliders.FirstOrDefault();

                if (slider == null)
                    return false;

                ((IJavaScriptExecutor)d).ExecuteScript(
                    @"
                    arguments[0].value = arguments[1];

                    arguments[0].dispatchEvent(
                        new Event('input', { bubbles: true })
                    );

                    arguments[0].dispatchEvent(
                        new Event('change', { bubbles: true })
                    );
                    ",
                    slider,
                    value);

                return true;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });
    }

    // ============================================================
    // Scenario 3 - Input Form Submit
    // ============================================================

    public void ClickInputFormSubmit()
    {
        Click(inputFormLink);

        wait.Until(d =>
            d.Url.Contains(
                "input-form-submit",
                StringComparison.OrdinalIgnoreCase));
    }

    public string GetNameValidationMessage()
    {
        return wait.Until(d =>
        {
            try
            {
                var element = d.FindElement(name);

                var message =
                    element.GetAttribute("validationMessage");

                return string.IsNullOrWhiteSpace(message)
                    ? null
                    : message.Trim();
            }
            catch (NoSuchElementException)
            {
                return null;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        })!;
    }

    public void FillInputForm()
    {
        Type(name, "Jagdish");

        Type(email, "jagdish@test.com");

        Type(password, "Password@123");

        Type(company, "Test Company");

        Type(website, "https://example.com");

        var countryElement = Find(country);

        var select = new SelectElement(countryElement);

        select.SelectByText("United States");

        Type(city, "Bengaluru");

        Type(address1, "Address 1");

        Type(address2, "Address 2");

        Type(state, "Karnataka");

        Type(zip, "560001");
    }

    public void SubmitInputForm()
    {
        Click(submitButton);
    }

    public string GetSuccessMessage()
    {
        return wait.Until(d =>
        {
            try
            {
                var element = d.FindElement(successMessage);

                var text = element.Text?.Trim();

                return string.IsNullOrWhiteSpace(text)
                    ? null
                    : text;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        })!;
    }

    // ============================================================
    // Common Methods
    // ============================================================

    private void WaitForPageReady()
    {
        wait.Until(d =>
        {
            try
            {
                return d.FindElement(simpleFormLink).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });
    }

    private IWebElement Find(By locator)
    {
        return wait.Until(d =>
        {
            try
            {
                var element = d.FindElement(locator);

                return element.Displayed
                    ? element
                    : null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        })!;
    }

    private void Click(By locator)
    {
        wait.Until(d =>
        {
            try
            {
                /*
                 * IMPORTANT:
                 * Find the element INSIDE the wait.
                 *
                 * Do not locate it once and keep reusing it.
                 */
                var element = d.FindElement(locator);

                if (!element.Displayed || !element.Enabled)
                    return false;

                try
                {
                    element.Click();
                    return true;
                }
                catch (ElementClickInterceptedException)
                {
                    ((IJavaScriptExecutor)d).ExecuteScript(
                        "arguments[0].scrollIntoView({block:'center'});",
                        element);

                    return false;
                }
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });
    }

    private void Type(By locator, string value)
    {
        wait.Until(d =>
        {
            try
            {
                var element = d.FindElement(locator);

                if (!element.Displayed || !element.Enabled)
                    return false;

                element.Clear();
                element.SendKeys(value);

                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });
    }

    private void ScrollIntoView(IWebElement element)
    {
        ((IJavaScriptExecutor)driver).ExecuteScript(
            "arguments[0].scrollIntoView({block:'center', inline:'center'});",
            element);
    }
}
