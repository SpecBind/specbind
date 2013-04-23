// <copyright file="MsTestCodedUiGeneratorProvider.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>
namespace Specflow.CodedUI
{
	using System.CodeDom;
	using System.Linq;

	using TechTalk.SpecFlow.Generator;
	using TechTalk.SpecFlow.Generator.UnitTestProvider;
	using TechTalk.SpecFlow.Utils;

	/// <summary>
	/// A unit test provider that added the appropriate attributes to each test.
	/// </summary>
	public class MsTestCodedUiGeneratorProvider : MsTest2010GeneratorProvider
	{
		private const string TestClassAttribute = @"Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute";
		private const string CodedUiTestClassAttribute = @"Microsoft.VisualStudio.TestTools.UITesting.CodedUITestAttribute";

		/// <summary>
		/// Initializes a new instance of the <see cref="MsTestCodedUiGeneratorProvider" /> class.
		/// </summary>
		/// <param name="codeDomHelper">The code DOM helper.</param>
		public MsTestCodedUiGeneratorProvider(CodeDomHelper codeDomHelper)
			: base(codeDomHelper)
		{
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

			foreach (var customAttribute in generationContext.TestClass.CustomAttributes
																	   .Cast<CodeAttributeDeclaration>()
																	   .Where(customAttribute => string.Equals(customAttribute.Name, TestClassAttribute)))
			{
				generationContext.TestClass.CustomAttributes.Remove(customAttribute);
				break;
			}

			generationContext.TestClass.CustomAttributes.Add(
				new CodeAttributeDeclaration(new CodeTypeReference(CodedUiTestClassAttribute)));
		}
	}
}