using Nop.Plugin.Api.Extensions;
using NUnit.Framework;

namespace Nop.Plugin.Api.Tests.ExtensionsTests.StringExtensions
{
    [TestFixture]
    public class StringExtensionsTests_ToStatus
    {
        private IStringExtensions _stringExtensions;

        [SetUp]
        public void SetUp()
        {
            _stringExtensions = new Extensions.StringExtensions();
        }

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
            bool? result = _stringExtensions.ToStatus(invalidStatus);

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
            bool? result = _stringExtensions.ToStatus(nullOrEmpty);

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
            bool? result = _stringExtensions.ToStatus(validPublishedStatus);

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
            bool? result = _stringExtensions.ToStatus(validUnpublishedStatus);

            //Assert
            Assert.IsFalse(result.Value);
        }
    }
}