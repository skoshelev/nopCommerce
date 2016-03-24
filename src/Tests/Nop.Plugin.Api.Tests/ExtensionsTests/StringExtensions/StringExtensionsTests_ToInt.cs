using Nop.Plugin.Api.Extensions;
using NUnit.Framework;

namespace Nop.Plugin.Api.Tests.ExtensionsTests.StringExtensions
{
    [TestFixture]
    public class StringExtensionsTests_ToInt
    {
        [Test]
        [TestCase("3ed")]
        [TestCase("sd4")]
        [TestCase("675435345345345345345345343456546")]
        [TestCase("-675435345345345345345345343456546")]
        [TestCase("$%%^%^$#^&&%#)__(^&")]
        [TestCase("2015-02-12")]
        [TestCase("12:45")]
        public void WhenInvalidIntPassed_ShouldReturnZero(string invalidInt)
        {
            //Arange

            //Act
            int cut = invalidInt.ToInt();

            //Assert
            Assert.AreEqual(0, cut);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void WhenNullOrEmptyStringPassed_ShouldReturnZero(string nullOrEmpty)
        {
            //Arange

            //Act
            int cut = nullOrEmpty.ToInt();

            //Assert
            Assert.AreEqual(0, cut);
        }

        [Test]
        [TestCase("3")]
        [TestCase("234234")]
        [TestCase("0")]
        [TestCase("-44")]
        [TestCase("000000005")]
        public void WhenValidIntPassed_ShouldReturnThatInt(string validInt)
        {
            //Arange
            int valid = int.Parse(validInt);

            //Act
            int cut = validInt.ToInt();

            //Assert
            Assert.AreEqual(valid, cut);
        }
    }
}