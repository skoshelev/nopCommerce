using System;
using System.Collections.Generic;
using Nop.Plugin.Api.Extensions;
using Nop.Plugin.Api.Tests.ExtensionsTests.ObjectExtensions.DummyObjects;
using NUnit.Framework;

namespace Nop.Plugin.Api.Tests.ExtensionsTests.ObjectExtensions
{
    [TestFixture]
    public class ObjectExtensionsTests_ToObject
    {
        [Test]
        public void WhenCollectionIsNull_ShouldReturnInstanceOfAnObjectOfTheSpecifiedType()
        {
            //Arange
            ICollection<KeyValuePair<string, string>> nullCollection = null;

            //Act
            SomeTestingObject someTestingObject = nullCollection.ToObject<SomeTestingObject>();

            //Assert
            Assert.IsNotNull(someTestingObject);
            Assert.IsInstanceOf(typeof(SomeTestingObject), someTestingObject);
        }

        [Test]
        public void WhenCollectionIsNull_ShouldReturnInstanceOfAnObjectWithUnsetProperties()
        {
            //Arange
            ICollection<KeyValuePair<string, string>> nullCollection = null;

            //Act
            SomeTestingObject someTestingObject = nullCollection.ToObject<SomeTestingObject>();

            //Assert
            Assert.AreEqual(0, someTestingObject.IntProperty);
            Assert.AreEqual(null, someTestingObject.StringProperty);
            Assert.AreEqual(null, someTestingObject.DateTimeNullableProperty);
            Assert.AreEqual(null, someTestingObject.BooleanNullableStatusProperty);
        }

        [Test]
        public void WhenCollectionIsEmpty_ShouldReturnInstanceOfAnObjectWithUnsetProperties()
        {
            //Arange
            ICollection<KeyValuePair<string, string>> emptyCollection = new List<KeyValuePair<string, string>>();

            //Act
            SomeTestingObject someTestingObject = emptyCollection.ToObject<SomeTestingObject>();

            //Assert
            Assert.AreEqual(0, someTestingObject.IntProperty);
            Assert.AreEqual(null, someTestingObject.StringProperty);
            Assert.AreEqual(null, someTestingObject.DateTimeNullableProperty);
            Assert.AreEqual(null, someTestingObject.BooleanNullableStatusProperty);
        }

        [Test]
        [TestCase("IntProperty")]
        [TestCase("Int_Property")]
        [TestCase("int_property")]
        [TestCase("intproperty")]
        [TestCase("inTprOperTy")]
        public void WhenCollectionContainsValidIntProperty_ShouldReturnTheObjectWithThisIntPropertySetToThePassedValue(string intPropertyName)
        {
            //Arange
            ICollection<KeyValuePair<string, string>> nullCollection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(intPropertyName, "5")
            };

            //Act
            SomeTestingObject someTestingObject = nullCollection.ToObject<SomeTestingObject>();

            //Assert
            Assert.AreEqual(5, someTestingObject.IntProperty);
        }

        [Test]
        [TestCase("invalid int property name")]
        [TestCase("34534535345345345345345345345345345345345")]
        public void WhenCollectionContainsInvalidIntProperty_ShouldReturnTheObjectWithItsIntPropertySetToTheDefaultValue(string invalidIntPropertyName)
        {
            //Arange
            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(invalidIntPropertyName, "5")
            };

            //Act
            SomeTestingObject someTestingObject = collection.ToObject<SomeTestingObject>();

            //Assert
            Assert.AreEqual(0, someTestingObject.IntProperty);
        }

        [Test]
        [TestCase("StringProperty")]
        [TestCase("String_Property")]
        [TestCase("string_property")]
        [TestCase("stringproperty")]
        [TestCase("strInGprOperTy")]
        public void WhenCollectionContainsValidStringProperty_ShouldReturnTheObjectWithThisStringPropertySetToThePassedValue(string stringPropertyName)
        {
            //Arange
            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(stringPropertyName, "some value")
            };

            //Act
            SomeTestingObject someTestingObject = collection.ToObject<SomeTestingObject>();

            //Assert
            Assert.AreEqual("some value", someTestingObject.StringProperty);
        }

        [Test]
        [TestCase("invalid string property name")]
        public void WhenCollectionContainsInvalidStringProperty_ShouldReturnTheObjectWithItsStringPropertySetToTheDefaultValue(string invalidStringPropertyName)
        {
            //Arange
            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(invalidStringPropertyName, "some value")
            };

            //Act
            SomeTestingObject someTestingObject = collection.ToObject<SomeTestingObject>();

            //Assert
            Assert.IsNull(someTestingObject.StringProperty);
        }
        
        [Test]
        [TestCase("DateTimeNullableProperty")]
        [TestCase("Date_Time_Nullable_Property")]
        [TestCase("date_time_nullable_property")]
        [TestCase("datetimenullableproperty")]
        [TestCase("dateTimeNullableProperty")]
        public void WhenCollectionContainsValidDateTimeProperty_ShouldReturnTheObjectWithThisDateTimePropertySetToThePassedValue(string dateTimePropertyName)
        {
            //Arange
            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(dateTimePropertyName, "2016-12-12")
            };

            //Act
            SomeTestingObject someTestingObject = collection.ToObject<SomeTestingObject>();

            //Assert
            Assert.AreEqual(new DateTime(2016, 12, 12), someTestingObject.DateTimeNullableProperty);
        }

        [Test]
        [TestCase("invalid date time property name")]
        public void WhenCollectionContainsInvalidDateTimeNullableProperty_ShouldReturnTheObjectWithItsDateTimeNullablePropertySetToTheDefaultValue(string invalidDateTimeNullablePropertyName)
        {
            //Arange
            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(invalidDateTimeNullablePropertyName, "2016-12-12")
            };

            //Act
            SomeTestingObject someTestingObject = collection.ToObject<SomeTestingObject>();

            //Assert
            Assert.IsNull(someTestingObject.DateTimeNullableProperty);
        }

        [Test]
        [TestCase("BooleanNullableStatusProperty", "published")]
        [TestCase("BooleanNullableStatusProperty", "PublisheD")]
        [TestCase("Boolean_Nullable_Status_Property", "published")]
        [TestCase("Boolean_Nullable_Status_Property", "PublisheD")]
        [TestCase("boolean_nullable_status_property", "published")]
        [TestCase("boolean_nullable_status_property", "Published")]
        [TestCase("booleannullablestatusproperty", "published")]
        [TestCase("booleannullablestatusproperty", "PublisheD")]
        [TestCase("booLeanNullabLeStaTusProperty", "published")]
        [TestCase("booLeanNullabLeStaTusProperty", "PubliShed")]
        public void WhenCollectionContainsValidBooleanStatusPropertyAndPublishedValue_ShouldReturnTheObjectWithThisBooleanStatusPropertySetToTrue(string booleanStatusPropertyName, string validPublishedValue)
        {
            //Arange
            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(booleanStatusPropertyName, validPublishedValue)
            };

            //Act
            SomeTestingObject someTestingObject = collection.ToObject<SomeTestingObject>();

            //Assert
            Assert.IsTrue(someTestingObject.BooleanNullableStatusProperty.Value);
        }

        [TestCase("BooleanNullableStatusProperty", "unpublished")]
        [TestCase("BooleanNullableStatusProperty", "unPublisHed")]
        [TestCase("Boolean_Nullable_Status_Property", "unpublished")]
        [TestCase("Boolean_Nullable_Status_Property", "unPublisHed")]
        [TestCase("boolean_nullable_status_property", "unPublIshed")]
        [TestCase("boolean_nullable_status_property", "unpublished")]
        [TestCase("booleannullablestatusproperty", "unpublished")]
        [TestCase("booleannullablestatusproperty", "unPublisHed")]
        [TestCase("booLeanNullabLeStaTusProperty", "unpublished")]
        [TestCase("booLeanNullabLeStaTusProperty", "UnpubLished")]
        public void WhenCollectionContainsValidBooleanStatusPropertyAndUnpublishedValue_ShouldReturnTheObjectWithThisBooleanStatusPropertySetToFalse(string booleanStatusPropertyName, string validUnublishedValue)
        {
            //Arange
            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(booleanStatusPropertyName, validUnublishedValue)
            };

            //Act
            SomeTestingObject someTestingObject = collection.ToObject<SomeTestingObject>();

            //Assert
            Assert.IsFalse(someTestingObject.BooleanNullableStatusProperty.Value);
        }

        [Test]
        [TestCase("invalid boolean property name")]
        public void WhenCollectionContainsInvalidBooleanNullableStatusProperty_ShouldReturnTheObjectWithItsBooleanNullableStatusPropertySetToTheDefaultValue(string invalidBooleanNullableStatusPropertyName)
        {
            //Arange
            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(invalidBooleanNullableStatusPropertyName, "true")
            };

            //Act
            SomeTestingObject someTestingObject = collection.ToObject<SomeTestingObject>();

            //Assert
            Assert.IsNull(someTestingObject.BooleanNullableStatusProperty);
        }
    }   
}