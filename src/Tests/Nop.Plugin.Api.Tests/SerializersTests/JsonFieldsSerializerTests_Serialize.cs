using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Tests.SerializersTests.DummyObjects;
using Nop.Plugin.Api.Validators;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.SerializersTests
{
    [TestFixture]
    public class JsonFieldsSerializerTests_Serialize
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void WhenEmptyFieldsParametersPassed_ShouldNotCallTheValidator(string emptyFields)
        {
            ISerializableObject serializableObject = new DummySerializableObject();

            //Arange
            IFieldsValidator validator = MockRepository.GenerateMock<IFieldsValidator>();


            IJsonFieldsSerializer cut = new JsonFieldsSerializer(validator);

            //Act
            cut.Serialize(serializableObject, emptyFields);

            //Assert
            validator.AssertWasNotCalled(x => x.GetValidFields(null, null), y => y.IgnoreArguments());
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void WhenEmptyFieldsParameterPassed_ShouldSerializeEverythingFromThePassedObject(string emptyFieldsParameter)
        {
            //Arange
            IFieldsValidator validator = MockRepository.GenerateStub<FieldsValidator>();

            IJsonFieldsSerializer cut = new JsonFieldsSerializer(validator);

            var serializableObject = new DummySerializableObject();
            serializableObject.Items.Add(new DummyObject()
            {
                FirstProperty = "first property value",
                SecondProperty = "second property value"
            });

            //Act
            string serializedObjectJson = cut.Serialize(serializableObject, emptyFieldsParameter);


            //Assert 
            DummySerializableObject dummySerializableObjectFromJson =
                JsonConvert.DeserializeObject<DummySerializableObject>(serializedObjectJson);

            Assert.AreEqual(serializableObject.Items.Count, dummySerializableObjectFromJson.Items.Count);
            Assert.AreEqual(serializableObject.Items[0], dummySerializableObjectFromJson.Items[0]);
            Assert.AreEqual("first property value", dummySerializableObjectFromJson.Items[0].FirstProperty);
            Assert.AreEqual("second property value", dummySerializableObjectFromJson.Items[0].SecondProperty);
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenNullObjectToSerializePassed_ShouldThrowArgumentNullException()
        {
            //Arange
            IFieldsValidator validator = MockRepository.GenerateStub<FieldsValidator>();

            IJsonFieldsSerializer cut = new JsonFieldsSerializer(validator);

            //Act
            cut.Serialize(Arg<ISerializableObject>.Is.Null, Arg<string>.Is.Anything);
        }
       
        [Test]
        public void WhenSomeFieldsParametersPassed_ShouldCallTheValidatorWithTheSameFields()
        {
            string someFields = "some,fields";
            ISerializableObject serializableObject = new DummySerializableObject();

            //Arange
            IFieldsValidator validator = MockRepository.GenerateMock<IFieldsValidator>();
            validator.Expect(
                x =>
                    x.GetValidFields(someFields, serializableObject.GetPrimaryPropertyType()))
                        .Return(new Dictionary<string, bool>());

            IJsonFieldsSerializer cut = new JsonFieldsSerializer(validator);

            //Act
            cut.Serialize(serializableObject, someFields);

            //Assert
            validator.VerifyAllExpectations();
        }

        [Test]
        public void WhenNoValidFieldsInTheFieldsParameterPassed_ShouldReturnEmptyCollectionJson()
        {
            DummySerializableObject dummySerializableObject = new DummySerializableObject();
            dummySerializableObject.Items.Add(new DummyObject()
            {
                FirstProperty = "first property value",
                SecondProperty = "second property value",
            });


            // Arange
            IFieldsValidator validator = MockRepository.GenerateStub<IFieldsValidator>();
            validator.Stub(
                x =>
                    x.GetValidFields(Arg<string>.Is.Anything, Arg<Type>.Is.Anything))
                        .Return(new Dictionary<string, bool>());

            IJsonFieldsSerializer cut = new JsonFieldsSerializer(validator);

            // Act
            string json = cut.Serialize(dummySerializableObject, "not valid fields");

            // Assert
            //string expectedJson = string.Format("{{\r\n  \"{0}\": []\r\n}}", dummySerializableObject.GetPrimaryPropertyName());
            //Assert.AreEqual(expectedJson, json);
            DummySerializableObject dummySerializableObjectFromJson =
                JsonConvert.DeserializeObject<DummySerializableObject>(json);

            Assert.AreEqual(0, dummySerializableObjectFromJson.Items.Count);
        }

        [Test]
        public void WhenValidFieldsParameterPassed_ShouldSerializeTheseFieldsJson()
        {
            DummySerializableObject dummySerializableObject = new DummySerializableObject();
            dummySerializableObject.Items.Add(new DummyObject()
            {
                FirstProperty = "first property value",
                SecondProperty = "second property value",
            });

            var validatorDictionary = new Dictionary<string, bool>();
            validatorDictionary.Add("firstproperty", true);

            // Arange
            IFieldsValidator validator = MockRepository.GenerateStub<IFieldsValidator>();
            validator.Stub(
                x =>
                    x.GetValidFields(Arg<string>.Is.Anything, Arg<Type>.Is.Anything))
                        .Return(validatorDictionary);

            IJsonFieldsSerializer cut = new JsonFieldsSerializer(validator);

            // Act
            string json = cut.Serialize(dummySerializableObject, "firstproperty");

            // Assert
            DummySerializableObject dummySerializableObjectFromJson =
                JsonConvert.DeserializeObject<DummySerializableObject>(json);

            Assert.AreEqual(1, dummySerializableObjectFromJson.Items.Count);
            Assert.AreEqual("first property value", dummySerializableObjectFromJson.Items[0].FirstProperty);
        }

        [Test]
        public void WhenValidFieldsParameterPassed_ShouldNotSerializeOtherFieldsJson()
        {
            DummySerializableObject dummySerializableObject = new DummySerializableObject();
            dummySerializableObject.Items.Add(new DummyObject()
            {
                FirstProperty = "first property value",
                SecondProperty = "second property value",
            });

            var validatorDictionary = new Dictionary<string, bool>();
            validatorDictionary.Add("firstproperty", true);

            // Arange
            IFieldsValidator validator = MockRepository.GenerateStub<IFieldsValidator>();
            validator.Stub(
                x =>
                    x.GetValidFields(Arg<string>.Is.Anything, Arg<Type>.Is.Anything))
                        .Return(validatorDictionary);

            IJsonFieldsSerializer cut = new JsonFieldsSerializer(validator);

            // Act
            string json = cut.Serialize(dummySerializableObject, "firstproperty");

            // Assert
            DummySerializableObject dummySerializableObjectFromJson =
                JsonConvert.DeserializeObject<DummySerializableObject>(json);

            Assert.AreEqual(1, dummySerializableObjectFromJson.Items.Count);
            Assert.IsNull(dummySerializableObjectFromJson.Items[0].SecondProperty);
        }
    }
}