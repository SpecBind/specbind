// <copyright file="NativeAttributeBuilder.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Selenium
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    /// <summary>
    /// A static class that constructs locators based on the Selenium Page attribute.
    /// </summary>
    public static class NativeAttributeBuilder
    {
        /// <summary>
        /// Gets the locator from the attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>The created locator; otherwise <c>null</c>.</returns>
        public static By GetLocator(FindsByAttribute attribute)
        {
            var how = attribute.How;
            var usingValue = attribute.Using;
            switch (how)
            {
                case How.Id:
                    return By.Id(usingValue);
                case How.Name:
                    return By.Name(usingValue);
                case How.TagName:
                    return By.TagName(usingValue);
                case How.ClassName:
                    return By.ClassName(usingValue);
                case How.CssSelector:
                    return By.CssSelector(usingValue);
                case How.LinkText:
                    return By.LinkText(usingValue);
                case How.PartialLinkText:
                    return By.PartialLinkText(usingValue);
                case How.XPath:
                    return By.XPath(usingValue);
                default:
                    return null;
            }
        }
    }
}