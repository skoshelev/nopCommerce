using Nop.Plugin.Api.Extensions;
using NUnit.Framework;

namespace Nop.Plugin.Api.Tests.ExtensionsTests.StringExtensions
{
    [TestFixture]
    public class StringExtensionsTests_ToStatus
    {
        [Test]
        [TestCase("invalid status")]
        [TestCase("publicshed")]
        [TestCase("un-published")]
        [TestCase("322345")]
        [TestCase("%^)@*%&*@_!+=")]
        public void WhenInvalidStatusPassed_ShouldReturnNull(string invalidStatus)
        {
            //Arange

            //Act
            bool? cut = invalidStatus.ToStatus();

            //Assert
            Assert.IsNull(cut);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void WhenNullOrEmptyStringPassed_ShouldReturnNull(string nullOrEmpty)
        {
            //Arange

            //Act
            bool? cut = nullOrEmpty.ToStatus();

            //Assert
            Assert.IsNull(cut);
        }

        [Test]
        [TestCase("published")]
        [TestCase("Published")]
        [TestCase("PublisheD")]
        public void WhenValidPublishedStatusPassed_ShouldReturnTrue(string validPublishedStatus)
        {
            //Arange

            //Act
            bool? cut = validPublishedStatus.ToStatus();

            //Assert
            Assert.IsTrue(cut.Value);
        }

        [Test]
        [TestCase("unpublished")]
        [TestCase("Unpublished")]
        [TestCase("UnPubLished")]
        public void WhenValidUnpublishedStatusPassed_ShouldReturnFalse(string validUnpublishedStatus)
        {
            //Arange

            //Act
            bool? cut = validUnpublishedStatus.ToStatus();

            //Assert
            Assert.IsFalse(cut.Value);
        }
    }
}