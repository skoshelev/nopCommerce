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
            bool? result = invalidStatus.ToStatus();

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void WhenNullOrEmptyStringPassed_ShouldReturnNull(string nullOrEmpty)
        {
            //Arange

            //Act
            bool? result = nullOrEmpty.ToStatus();

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase("published")]
        [TestCase("Published")]
        [TestCase("PublisheD")]
        public void WhenValidPublishedStatusPassed_ShouldReturnTrue(string validPublishedStatus)
        {
            //Arange

            //Act
            bool? result = validPublishedStatus.ToStatus();

            //Assert
            Assert.IsTrue(result.Value);
        }

        [Test]
        [TestCase("unpublished")]
        [TestCase("Unpublished")]
        [TestCase("UnPubLished")]
        public void WhenValidUnpublishedStatusPassed_ShouldReturnFalse(string validUnpublishedStatus)
        {
            //Arange

            //Act
            bool? result = validUnpublishedStatus.ToStatus();

            //Assert
            Assert.IsFalse(result.Value);
        }
    }
}