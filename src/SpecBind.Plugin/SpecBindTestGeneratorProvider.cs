// <copyright file="SpecBindTestGeneratorProvider.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace SpecBind.Plugin
{
    using System;
    using System.CodeDom;
    using System.Linq;

    using TechTalk.SpecFlow.Generator;
    using TechTalk.SpecFlow.Generator.UnitTestProvider;
    using TechTalk.SpecFlow.Utils;

    /// <summary>
    /// A unit test provider that added the appropriate attributes to each test.
    /// </summary>
    public class SpecBindTestGeneratorProvider : MsTest2010GeneratorProvider
    {
        private const string TestClassAttribute = @"Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute";
        private const string CodedUiTestClassAttribute = @"Microsoft.VisualStudio.TestTools.UITesting.CodedUITestAttribute";
        private const string DeploymentItemAttribute = "Microsoft.VisualStudio.TestTools.UnitTesting.DeploymentItemAttribute";

        private readonly ISpecBindConfigurationProvider configurationProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecBindTestGeneratorProvider" /> class.
        /// </summary>
        /// <param name="codeDomHelper">The code DOM helper.</param>
        /// <param name="configurationProvider">The configuration provider.</param>
        public SpecBindTestGeneratorProvider(CodeDomHelper codeDomHelper, ISpecBindConfigurationProvider configurationProvider)
            : base(codeDomHelper)
        {
            this.configurationProvider = configurationProvider;
        }

        /// <summary>
        /// Sets the test class.
        /// </summary>
        /// <param name="generationContext">The generation context.</param>
        /// <param name="featureTitle">The feature title.</param>
        /// <param name="featureDescription">The feature description.</param>
        public override void SetTestClass(TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
        {
            base.SetTestClass(generationContext, featureTitle, featureDescription);

            // Get the driver assembly name.
            var providerName = this.configurationProvider.GetBrowserDriverType();

            // Only generate Coded UI attribute if using the Coded UI provider.
            if (string.Equals(Constants.CodedUiDriverAssembly, providerName, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var customAttribute in generationContext.TestClass.CustomAttributes
                                                                       .Cast<CodeAttributeDeclaration>()
                                                                       .Where(customAttribute => string.Equals(customAttribute.Name, TestClassAttribute)))
                {
                    generationContext.TestClass.CustomAttributes.Remove(customAttribute);
                    break;
                }

                generationContext.TestClass.CustomAttributes.Add(
                    new CodeAttributeDeclaration(new CodeTypeReference(CodedUiTestClassAttribute)));
<<<<<<< HEAD
	        }

            // Add deployment item in each test for the driver.
			generationContext.TestClass.CustomAttributes.Add(
				new CodeAttributeDeclaration(
                    new CodeTypeReference(DeploymentItemAttribute),
=======
            }
            
            // Add deployment item in each test for the driver.
            generationContext.TestClass.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(DeploymentItemAttribute), 
>>>>>>> 5ad7ae3... Updated SpecBind to use SpecFlow 2.0.0
                    new CodeAttributeArgument(new CodePrimitiveExpression(providerName))));
        }
    }
}