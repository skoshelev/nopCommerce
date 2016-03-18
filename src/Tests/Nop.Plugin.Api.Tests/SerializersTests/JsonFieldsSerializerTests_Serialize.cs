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
        public void WhenEmptyFieldsParameterPassed_ShouldReturnJsonForThePassedObject(string emptyFieldsParameter)
        {
            //Arange
            IFieldsValidator validator = MockRepository.GenerateStub<FieldsValidator>();

            IJsonFieldsSerializer cut = new JsonFieldsSerializer(validator);

            var serializableObject = new SerializableObject();
            serializableObject.Items.Add(new DummyObject()
            {
                FirstProperty = "first property value",
                SecondProperty = "second property value"
            });
            
            //Act
            string serializedObjectJson = cut.Serialize(serializableObject, emptyFieldsParameter);

            SerializableObject serializableObjectFromJsonResult = JsonConvert.DeserializeObject<SerializableObject>(serializedObjectJson);

            //Assert
            Assert.AreEqual(serializableObject.Items.Count, serializableObjectFromJsonResult.Items.Count);
            Assert.AreEqual(serializableObject.Items[0], serializableObjectFromJsonResult.Items[0]);
            Assert.AreEqual(serializableObject.GetPrimaryPropertyName(), serializableObjectFromJsonResult.GetPrimaryPropertyName());
            Assert.AreEqual(serializableObject.GetPrimaryPropertyType(), serializableObjectFromJsonResult.GetPrimaryPropertyType());
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
        [TestCase("")]
        [TestCase(null)]
        public void WhenNoFieldsParametersPassed_ShouldReturnSerializedJsonStringForTheSerializableObject(string emptyFields)
        {
            SerializableObject serializableObject = new SerializableObject();
            serializableObject.Items.Add(new DummyObject()
            {
                FirstProperty = "first property value",
                SecondProperty = "second property value",
            });

            //Arange
            IFieldsValidator validator = MockRepository.GenerateStub<IFieldsValidator>();
           
            IJsonFieldsSerializer cut = new JsonFieldsSerializer(validator);

            //Act
            string json = cut.Serialize(serializableObject, emptyFields);

            SerializableObject serializableObjectFromResultJson = JsonConvert.DeserializeObject<SerializableObject>(json);

            //Assert
            Assert.AreEqual(serializableObject.Items.Count, serializableObjectFromResultJson.Items.Count);
            Assert.AreEqual(serializableObject.Items[0], serializableObjectFromResultJson.Items[0]);
            Assert.AreEqual(serializableObject.GetPrimaryPropertyName(), serializableObjectFromResultJson.GetPrimaryPropertyName());
            Assert.AreEqual(serializableObject.GetPrimaryPropertyType(), serializableObjectFromResultJson.GetPrimaryPropertyType());
        }

        [Test]
        public void WhenSomeFieldsParametersPassed_ShouldCallTheValidator()
        {
            string someFields = "some,fields";
            ISerializableObject serializableObject = new SerializableObject();

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
        public void WhenValidFieldsParametersPassed_ShouldCallTheValidator()
        {
            string someFields = "some,fields";
            ISerializableObject serializableObject = new SerializableObject();

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
        public void WhenNoValidFieldsPassedInTheFieldsParameter_ShouldReturnEmptyCollectionJson()
        {
            ISerializableObject serializableObject = new SerializableObject();
            string expectedJson = string.Format("{{\r\n  \"{0}\": []\r\n}}", serializableObject.GetPrimaryPropertyName());

            //Arange
            IFieldsValidator validator = MockRepository.GenerateStub<IFieldsValidator>();
            validator.Stub(
                x =>
                    x.GetValidFields(Arg<string>.Is.Anything, Arg<Type>.Is.Anything))
                        .Return(new Dictionary<string, bool>());

            IJsonFieldsSerializer cut = new JsonFieldsSerializer(validator);

            //Act
            string json = cut.Serialize(serializableObject, null);

            //Assert
            Assert.AreEqual(expectedJson, json);
        }
    }
}