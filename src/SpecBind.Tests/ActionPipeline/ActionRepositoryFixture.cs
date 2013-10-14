// <copyright file="ActionRepositoryFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.ActionPipeline
{
    using System.Linq;

    using BoDi;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;

    /// <summary>
    /// A test fixture for the ActionRepository.
    /// </summary>
    [TestClass]
    public class ActionRepositoryFixture
    {
        /// <summary>
        /// Tests the get pre actions method without an initialize call returns an empty list.
        /// </summary>
        [TestMethod]
        public void TestGetPreActionsWithoutInitializeReturnsEmptyList()
        {
            var container = new Mock<IObjectContainer>(MockBehavior.Strict);

            var repository = new ActionRepository(container.Object);

            var result = repository.GetPreActions();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());

            container.VerifyAll();
        }

        /// <summary>
        /// Tests the get post actions method without an initialize call returns an empty list.
        /// </summary>
        [TestMethod]
        public void TestGetPostActionsWithoutInitializeReturnsEmptyList()
        {
            var container = new Mock<IObjectContainer>(MockBehavior.Strict);

            var repository = new ActionRepository(container.Object);

            var result = repository.GetPostActions();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());

            container.VerifyAll();
        }

        /// <summary>
        /// Tests the get locator actions method without an initialize call returns an empty list.
        /// </summary>
        [TestMethod]
        public void TestGetLocatorActionsWithoutInitializeReturnsEmptyList()
        {
            var container = new Mock<IObjectContainer>(MockBehavior.Strict);

            var repository = new ActionRepository(container.Object);

            var result = repository.GetLocatorActions();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());

            container.VerifyAll();
        }

        /// <summary>
        /// Tests that the Initialize method loads any extensions known to be in the project.
        /// </summary>
        [TestMethod]
        public void TestInitializeLoadsKnownActionsInClasses()
        {
            var container = new Mock<IObjectContainer>(MockBehavior.Strict);
            container.Setup(c => c.Resolve(typeof(HighlightPreAction), null))
                     .Returns(new HighlightPreAction(null, null));

            var repository = new ActionRepository(container.Object);

            repository.Initialize();

            container.VerifyAll();
        }
    }
}
