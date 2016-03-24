using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Api.Extensions;
using NUnit.Framework;

namespace Nop.Plugin.Api.Tests.ExtensionsTests.StringExtensions
{
    [TestFixture]
    public class StringExtensionsTests_IsListOfInts
    {
        [Test]
        [TestCase("a,b,c,d")]
        [TestCase(",")]
        [TestCase("invalid")]
        [TestCase("1 2 3 4 5")]
        [TestCase("&*^&^^*()_)_-1-=")]
        [TestCase("5756797879978978978978978978978978978, 234523523423423423423423423423423423423423")]
        public void WhenAllPartsOfTheListAreInvalid_ShouldReturnNull(string invalidList)
        {
            //Arange

            //Act
            IList<int> result = invalidList.ToListOfInts();

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
            IList<int> result = nullOrEmpty.ToListOfInts();

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase("1,2,3")]
        [TestCase("1, 4, 7")]
        [TestCase("0,-1, 7, 9 ")]
        [TestCase("   0,1  , 7, 9   ")]
        public void WhenValidListPassed_ShouldReturnThatList(string validList)
        {
            //Arange
            List<int> expectedList = validList.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            //Act
            IList<int> result = validList.ToListOfInts();

            //Assert
            CollectionAssert.AreEqual(expectedList, result);
        }

        [Test]
        [TestCase("1,2, u,3")]
        [TestCase("a, b, c, 1")]
        [TestCase("0,-1, -, 7, 9 ")]
        [TestCase("%^#^^,$,#,%,8")]
        [TestCase("0")]
        [TestCase("097")]
        [TestCase("087, 05667, sdf")]
        [TestCase("017, 345df, 05867")]
        [TestCase("67856756, 05867, 76767ergdf")]
        [TestCase("690, 678678678678678678678678678678678678676867867")]
        public void WhenSomeOfTheItemsAreValid_ShouldReturnThatListContainingOnlyTheValidItems(string mixedList)
        {
            //Arange
            List<int> expectedList = new List<int>();
            var collectionSplited = mixedList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            int tempInt;
            foreach (var item in collectionSplited)
            {
                if (int.TryParse(item, out tempInt))
                {
                    expectedList.Add(tempInt);
                }
            }

            //Act
            IList<int> result = mixedList.ToListOfInts();

            //Assert
            CollectionAssert.IsNotEmpty(result);
            CollectionAssert.AreEqual(expectedList, result);
        }

        [Test]
        [TestCase("f,d, u,3")]
        [TestCase("0")]
        [TestCase("097")]
        [TestCase("67856756, 05ert867, 76767ergdf")]
        [TestCase("690, 678678678678678678678678678678678678676867867")]
        public void WhenOnlyOneOfTheItemsIsValid_ShouldReturnListContainingOnlyThatItem(string mixedList)
        {
            //Arange
            List<int> expectedList = new List<int>();
            var collectionSplited = mixedList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            int tempInt;
            foreach (var item in collectionSplited)
            {
                if (int.TryParse(item, out tempInt))
                {
                    expectedList.Add(tempInt);
                }
            }

            //Act
            IList<int> result = mixedList.ToListOfInts();

            //Assert
            Assert.AreEqual(1, result.Count);
            CollectionAssert.IsNotEmpty(result);
            CollectionAssert.AreEqual(expectedList, result);
        }
    }
}