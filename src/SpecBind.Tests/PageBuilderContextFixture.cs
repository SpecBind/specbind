// <copyright file="PageBuilderContextFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
    using System.Linq.Expressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for the page builder context and expression data context.
    /// </summary>
    [TestClass]
    public class PageBuilderContextFixture
    {
        /// <summary>
        /// Tests the expression data when constructed returns the constructed values.
        /// </summary>
        [TestMethod]
        public void TestExpressionDataWhenConstructedReturnsTheConstructedValues()
        {
            var expression = Expression.Constant(null, typeof(object));
            var expressionData = new ExpressionData(expression, expression.Type);

            Assert.AreSame(expression, expressionData.Expression);
            Assert.AreEqual(typeof(object), expressionData.Type);
        }

        /// <summary>
        /// Tests the expression data when constructed returns the constructed values.
        /// </summary>
        [TestMethod]
        public void TestExpressionDataWithNameWhenConstructedReturnsTheConstructedValues()
        {
            var expression = Expression.Constant(null, typeof(object));
            var expressionData = new ExpressionData(expression, expression.Type, "MyProperty");

            Assert.AreSame(expression, expressionData.Expression);
            Assert.AreEqual(typeof(object), expressionData.Type);
            Assert.AreEqual("MyProperty", expressionData.Name);
        }

        /// <summary>
        /// Tests the expression data ToString method returns the type.
        /// </summary>
        [TestMethod]
        public void TestExpressionDataToStringReturnsType()
        {
            var expression = Expression.Constant(null, typeof(object));
            var expressionData = new ExpressionData(expression, expression.Type);

            Assert.AreEqual("Type: System.Object", expressionData.ToString());
        }

        /// <summary>
        /// Tests the expression data ToString method returns the type and name when specified.
        /// </summary>
        [TestMethod]
        public void TestExpressionDataToStringWhenNameIsSpecifiedReturnsTypeAndName()
        {
            var expression = Expression.Constant(null, typeof(object));
            var expressionData = new ExpressionData(expression, expression.Type, "SomeProperty");

            Assert.AreEqual("Type: System.Object, Name: SomeProperty", expressionData.ToString());
        }

        /// <summary>
        /// Tests the page builder context when constructed returns the constructed values.
        /// </summary>
        [TestMethod]
        public void TestPageBuilderContextWhenConstructedReturnsTheConstructedValues()
        {
            var browser = new ExpressionData(null, typeof(object));
            var uriHelper = new ExpressionData(null, typeof(object));
            var document = new ExpressionData(null, typeof(object));
            var parentElement = new ExpressionData(null, typeof(object));

            var context = new PageBuilderContext(browser, uriHelper, parentElement, document);

            Assert.AreSame(browser, context.Browser);
            Assert.AreSame(document, context.Document);
            Assert.AreSame(parentElement, context.ParentElement);
            Assert.IsNull(context.RootLocator);
            Assert.IsNull(context.CurrentElement);
        }

        /// <summary>
        /// Tests the page builder context when constructed returns the constructed values.
        /// </summary>
        [TestMethod]
        public void TestCreateChildContextWhenCalledReturnsTheConstructedValuesAndNewContext()
        {
            var browser = new ExpressionData(null, typeof(object));
            var uriHelper = new ExpressionData(null, typeof(object));
            var document = new ExpressionData(null, typeof(object));
            var parentElement = new ExpressionData(null, typeof(object));
            var context = new PageBuilderContext(browser, uriHelper, parentElement, document);

            var child = new ExpressionData(null, typeof(object));
            var childContext = context.CreateChildContext(child);

            Assert.AreSame(browser, childContext.Browser);

            Assert.AreSame(child, childContext.Document);
            Assert.AreSame(parentElement, childContext.RootLocator);
            Assert.AreSame(document, childContext.ParentElement);

            Assert.IsNull(childContext.CurrentElement);
        }

        /// <summary>
        /// Tests the page builder context when multiples are created
        /// returns the first parent context as the root context.
        /// </summary>
        [TestMethod]
        public void TestCreateChildContextWhenMultipleContextsAreCreatedThenTheRootContextIsTheFirstParent()
        {
            var browser = new ExpressionData(null, typeof(object));
            var uriHelper = new ExpressionData(null, typeof(object));
            var document = new ExpressionData(null, typeof(object));
            var parentElement = new ExpressionData(null, typeof(object));
            var context = new PageBuilderContext(browser, uriHelper, parentElement, document);

            var child1 = new ExpressionData(null, typeof(object));
            var child2 = new ExpressionData(null, typeof(object));
            var childContext1 = context.CreateChildContext(child1);
            var childContext2 = childContext1.CreateChildContext(child2);

            Assert.AreSame(document, childContext1.ParentElement);
            Assert.AreSame(parentElement, childContext1.RootLocator);

            Assert.AreSame(child1, childContext2.ParentElement);
            Assert.AreSame(parentElement, childContext2.RootLocator);
        }
    }
}