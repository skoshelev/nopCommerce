using System;
using System.Collections.Generic;
using Nop.Plugin.Api.Extensions;
using Nop.Plugin.Api.Tests.ExtensionsTests.ObjectExtensions.DummyObjects;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ExtensionsTests.ObjectExtensions
{
    [TestFixture]
    public class ObjectExtensionsTests_ToObject
    {
        [Test]
        public void WhenCollectionIsNull_ShouldShouldNotCallAnyOfTheStringExtensionsMethods()
        {
            //Arange
            IStringExtensions stringExtensionsMock = MockRepository.GenerateMock<IStringExtensions>();

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsMock);

            ICollection<KeyValuePair<string, string>> nullCollection = null;

            //Act
            objectExtensions.ToObject<SomeTestingObject>(nullCollection);

            //Assert
            stringExtensionsMock.AssertWasNotCalled(x => x.ToInt(Arg<string>.Is.Anything));
            stringExtensionsMock.AssertWasNotCalled(x => x.ToIntNullable(Arg<string>.Is.Anything));
            stringExtensionsMock.AssertWasNotCalled(x => x.ToDateTimeNullable(Arg<string>.Is.Anything));
            stringExtensionsMock.AssertWasNotCalled(x => x.ToListOfInts(Arg<string>.Is.Anything));
            stringExtensionsMock.AssertWasNotCalled(x => x.ToStatus(Arg<string>.Is.Anything));
        }

        [Test]
        public void WhenCollectionIsNull_ShouldReturnInstanceOfAnObjectOfTheSpecifiedType()
        {
            //Arange
            IStringExtensions stringExtensionsStub = MockRepository.GenerateStub<IStringExtensions>();

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsStub);

            ICollection<KeyValuePair<string, string>> nullCollection = null;

            //Act
            SomeTestingObject someTestingObject = objectExtensions.ToObject<SomeTestingObject>(nullCollection);

            //Assert
            Assert.IsNotNull(someTestingObject);
            Assert.IsInstanceOf(typeof(SomeTestingObject), someTestingObject);
        }

        [Test]
        public void WhenCollectionIsNull_ShouldReturnInstanceOfAnObjectWithUnsetProperties()
        {
            //Arange
            IStringExtensions stringExtensionsStub = MockRepository.GenerateStub<IStringExtensions>();

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsStub);

            ICollection<KeyValuePair<string, string>> nullCollection = null;

            //Act
            SomeTestingObject someTestingObject = objectExtensions.ToObject<SomeTestingObject>(nullCollection);

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
            IStringExtensions stringExtensionsStub = MockRepository.GenerateStub<IStringExtensions>();

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsStub);

            ICollection<KeyValuePair<string, string>> emptyCollection = new List<KeyValuePair<string, string>>();

            //Act
            SomeTestingObject someTestingObject = objectExtensions.ToObject<SomeTestingObject>(emptyCollection);

            //Assert
            Assert.AreEqual(0, someTestingObject.IntProperty);
            Assert.AreEqual(null, someTestingObject.StringProperty);
            Assert.AreEqual(null, someTestingObject.DateTimeNullableProperty);
            Assert.AreEqual(null, someTestingObject.BooleanNullableStatusProperty);
        }
        [Test]
        public void WhenCollectionIsEmpty_ShoulNotCallAnyOfTheStringExtensionsMethods()
        {
            //Arange
            IStringExtensions stringExtensionsMock = MockRepository.GenerateMock<IStringExtensions>();

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsMock);

            ICollection<KeyValuePair<string, string>> emptyCollection = new List<KeyValuePair<string, string>>();

            //Act
            objectExtensions.ToObject<SomeTestingObject>(emptyCollection);

            //Assert
            stringExtensionsMock.AssertWasNotCalled(x => x.ToInt(Arg<string>.Is.Anything));
            stringExtensionsMock.AssertWasNotCalled(x => x.ToIntNullable(Arg<string>.Is.Anything));
            stringExtensionsMock.AssertWasNotCalled(x => x.ToDateTimeNullable(Arg<string>.Is.Anything));
            stringExtensionsMock.AssertWasNotCalled(x => x.ToListOfInts(Arg<string>.Is.Anything));
            stringExtensionsMock.AssertWasNotCalled(x => x.ToStatus(Arg<string>.Is.Anything));
        }

        [Test]
        [TestCase("IntProperty")]
        [TestCase("Int_Property")]
        [TestCase("int_property")]
        [TestCase("intproperty")]
        [TestCase("inTprOperTy")]
        public void WhenCollectionContainsValidIntProperty_ShouldCallTheToIntMethod(string intPropertyName)
        {
            //Arange
            int expectedInt = 5;
            IStringExtensions stringExtensionsMock = MockRepository.GenerateMock<IStringExtensions>();
            stringExtensionsMock.Expect(x => x.ToInt(Arg<string>.Is.Anything)).IgnoreArguments().Return(expectedInt);

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsMock);

            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(intPropertyName, expectedInt.ToString())
            };

            //Act
            objectExtensions.ToObject<SomeTestingObject>(collection);

            //Assert
            stringExtensionsMock.VerifyAllExpectations();
        }
        
        [Test]
        [TestCase("invalid int property name")]
        [TestCase("34534535345345345345345345345345345345345")]
        public void WhenCollectionContainsInvalidIntProperty_ShouldNotCallTheToIntMethod(string invalidIntPropertyName)
        {
            //Arange
            IStringExtensions stringExtensionsMock = MockRepository.GenerateMock<IStringExtensions>();

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsMock);

            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(invalidIntPropertyName, "5")
            };

            //Act
            objectExtensions.ToObject<SomeTestingObject>(collection);

            //Assert
            stringExtensionsMock.AssertWasNotCalled(x => x.ToInt(Arg<string>.Is.Anything));
        }

        [Test]
        [TestCase("StringProperty")]
        [TestCase("String_Property")]
        [TestCase("string_property")]
        [TestCase("stringproperty")]
        [TestCase("strInGprOperTy")]
        public void WhenCollectionContainsValidStringProperty_ShouldSetTheObjectStringPropertyValueToTheCollectionStringPropertyValue(string stringPropertyName)
        {
            //Arange
            IStringExtensions stringExtensionsStub = MockRepository.GenerateMock<IStringExtensions>();

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsStub);

            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(stringPropertyName, "some value")
            };

            //Act
            SomeTestingObject someTestingObject = objectExtensions.ToObject<SomeTestingObject>(collection);

            //Assert
            Assert.AreEqual("some value", someTestingObject.StringProperty);
        }

        [Test]
        [TestCase("invalid string property name")]
        public void WhenCollectionContainsInvalidStringProperty_ShouldReturnTheObjectWithItsStringPropertySetToTheDefaultValue(string invalidStringPropertyName)
        {
            //Arange
            IStringExtensions stringExtensionsStub = MockRepository.GenerateMock<IStringExtensions>();

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsStub);

            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(invalidStringPropertyName, "some value")
            };

            //Act
            SomeTestingObject someTestingObject = objectExtensions.ToObject<SomeTestingObject>(collection);

            //Assert
            Assert.IsNull(someTestingObject.StringProperty);
        }

        [Test]
        [TestCase("invalid string property name")]
        [TestCase("StringProperty")]
        [TestCase("String_Property")]
        [TestCase("string_property")]
        [TestCase("stringproperty")]
        [TestCase("strInGprOperTy")]
        public void WhenCollectionContainsValidOrInvalidStringProperty_ShouldNotCallAnyOfTheStringExtensionsMethods(string stringProperty)
        {
            //Arange
            IStringExtensions stringExtensionsMock = MockRepository.GenerateMock<IStringExtensions>();

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsMock);

            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(stringProperty, "some value")
            };

            //Act
            objectExtensions.ToObject<SomeTestingObject>(collection);

            //Assert
            stringExtensionsMock.AssertWasNotCalled(x => x.ToInt(Arg<string>.Is.Anything));
            stringExtensionsMock.AssertWasNotCalled(x => x.ToIntNullable(Arg<string>.Is.Anything));
            stringExtensionsMock.AssertWasNotCalled(x => x.ToDateTimeNullable(Arg<string>.Is.Anything));
            stringExtensionsMock.AssertWasNotCalled(x => x.ToListOfInts(Arg<string>.Is.Anything));
            stringExtensionsMock.AssertWasNotCalled(x => x.ToStatus(Arg<string>.Is.Anything));
        }

        [Test]
        [TestCase("DateTimeNullableProperty")]
        [TestCase("Date_Time_Nullable_Property")]
        [TestCase("date_time_nullable_property")]
        [TestCase("datetimenullableproperty")]
        [TestCase("dateTimeNullableProperty")]
        public void WhenCollectionContainsValidDateTimeProperty_ShouldCallTheToDateTimeNullableMethod(string dateTimePropertyName)
        {
            //Arange
            IStringExtensions stringExtensionsMock = MockRepository.GenerateMock<IStringExtensions>();
            stringExtensionsMock.Expect(x => x.ToDateTimeNullable(Arg<string>.Is.Anything)).IgnoreArguments().Return(DateTime.Now);

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsMock);

            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(dateTimePropertyName, "2016-12-12")
            };

            //Act
            objectExtensions.ToObject<SomeTestingObject>(collection);

            //Assert
            stringExtensionsMock.AssertWasCalled(x => x.ToDateTimeNullable(Arg<string>.Is.Anything));
        }

        [Test]
        [TestCase("invalid date time property name")]
        public void WhenCollectionContainsInvalidDateTimeNullableProperty_ShouldNotCallTheDateTimeNullableMethod(string invalidDateTimeNullablePropertyName)
        {
            //Arange
            IStringExtensions stringExtensionsMock = MockRepository.GenerateMock<IStringExtensions>();
            stringExtensionsMock.Expect(x => x.ToDateTimeNullable(Arg<string>.Is.Anything)).IgnoreArguments();

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsMock);

            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(invalidDateTimeNullablePropertyName, "2016-12-12")
            };

            //Act
            objectExtensions.ToObject<SomeTestingObject>(collection);

            //Assert
            stringExtensionsMock.AssertWasNotCalled(x => x.ToDateTimeNullable(Arg<string>.Is.Anything));
        }

        [Test]
        [TestCase("BooleanNullableStatusProperty")]
        [TestCase("BooleanNullableStatusProperty")]
        [TestCase("Boolean_Nullable_Status_Property")]
        [TestCase("Boolean_Nullable_Status_Property")]
        [TestCase("boolean_nullable_status_property")]
        [TestCase("boolean_nullable_status_property")]
        [TestCase("booleannullablestatusproperty")]
        [TestCase("booleannullablestatusproperty")]
        [TestCase("booLeanNullabLeStaTusProperty")]
        [TestCase("booLeanNullabLeStaTusProperty")]
        public void WhenCollectionContainsValidBooleanStatusPropertyAndPublishedValue_ShouldCallTheToStatusMethod(string booleanStatusPropertyName)
        {
            //Arange
            IStringExtensions stringExtensionsMock = MockRepository.GenerateMock<IStringExtensions>();
            stringExtensionsMock.Expect(x => x.ToStatus(Arg<string>.Is.Anything)).IgnoreArguments().Return(true);

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsMock);

            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(booleanStatusPropertyName, "some published value")
            };

            //Act
            objectExtensions.ToObject<SomeTestingObject>(collection);

            //Assert
            stringExtensionsMock.AssertWasCalled(x => x.ToStatus(Arg<string>.Is.Anything));
        }

        [Test]
        [TestCase("invalid boolean property name")]
        public void WhenCollectionContainsInvalidBooleanNullableStatusProperty_ShouldNotCallTheToStatusMethod(string invalidBooleanNullableStatusPropertyName)
        {
            //Arange
            IStringExtensions stringExtensionsMock = MockRepository.GenerateMock<IStringExtensions>();
            stringExtensionsMock.Expect(x => x.ToStatus(Arg<string>.Is.Anything)).IgnoreArguments();

            IObjectExtensions objectExtensions = new Extensions.ObjectExtensions(stringExtensionsMock);

            ICollection<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(invalidBooleanNullableStatusPropertyName, "true")
            };

            //Act
            objectExtensions.ToObject<SomeTestingObject>(collection);

            //Assert
            stringExtensionsMock.AssertWasNotCalled(x => x.ToStatus(Arg<string>.Is.Anything));
        }
    }   
}