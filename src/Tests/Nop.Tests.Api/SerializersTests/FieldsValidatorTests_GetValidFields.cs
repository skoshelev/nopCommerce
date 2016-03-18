using System.Collections.Generic;
using Nop.Plugin.Api.Tests.SerializersTests.DummyObjects;
using Nop.Plugin.Api.Validators;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.SerializersTests
{
    public class FieldsValidatorTests_GetValidFields
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void WhenEmptyFieldsParameterPassed_ShouldReturnEmptyDictionary(string emptyFields)
        {
            //Arange
            IFieldsValidator cut = MockRepository.GenerateStub<FieldsValidator>();
            
            //Act
            Dictionary<string, bool> result = cut.GetValidFields(emptyFields, typeof(SerializableObject));
            
            Assert.IsEmpty(result);
        }

        [Test]
        [TestCase("first_property,second_property")]
        [TestCase("firstproperty,secondproperty")]
        [TestCase("firstProperty,Secondproperty")]
        public void WhenValidFieldsParameterPassed_ShouldReturnNonEmptyDictionary(string validFields)
        {
            //Arange
            IFieldsValidator cut = MockRepository.GenerateStub<FieldsValidator>();

            //Act
            Dictionary<string, bool> result = cut.GetValidFields(validFields, typeof(DummyObject));

            Assert.IsNotEmpty(result);
        }

        [Test]
        [TestCase("first_property,second_property")]
        [TestCase("firstproperty,secondproperty")]
        [TestCase("firstProperty,Secondproperty")]
        public void WhenValidFieldsParameterPassed_ShouldReturnDictionaryWithValidFields(string validFields)
        {
            //Arange
            IFieldsValidator cut = MockRepository.GenerateStub<FieldsValidator>();

            //Act
            Dictionary<string, bool> result = cut.GetValidFields(validFields, typeof(DummyObject));

            Assert.True(result.ContainsKey("firstproperty"));
            Assert.True(result.ContainsKey("secondproperty"));
        }
        
        [Test]
        [TestCase("first_property,second_property,invalid")]
        [TestCase("firstproperty,secondproperty,invalid")]
        [TestCase("firstProperty,Secondproperty,invalid")]
        public void WhenValidAndInvalidFieldsParameterPassed_ShouldReturnDictionaryWithValidFieldsOnly(string mixedFields)
        {
            //Arange
            IFieldsValidator cut = MockRepository.GenerateStub<FieldsValidator>();

            //Act
            Dictionary<string, bool> result = cut.GetValidFields(mixedFields, typeof(DummyObject));

            Assert.AreEqual(2, result.Count);
            Assert.True(result.ContainsKey("firstproperty"));
            Assert.True(result.ContainsKey("secondproperty"));
        }

        [Test]
        [TestCase("invalid")]
        [TestCase("multiple,invalid,fields")]
        public void WhenInvalidFieldsParameterPassed_ShouldReturnEmptyDictionary(string invalidFields)
        {
            //Arange
            IFieldsValidator cut = MockRepository.GenerateStub<FieldsValidator>();

            //Act
            Dictionary<string, bool> result = cut.GetValidFields(invalidFields, typeof(DummyObject));

            Assert.IsEmpty(result);
        }

        [Test]
        [TestCase("invalid,,")]
        [TestCase(",,,*multiple,)in&^valid,f@#%$ields+_-,,,,,")]
        [TestCase(".")]
        [TestCase(",")]
        [TestCase("()")]
        [TestCase("'\"\"")]
        [TestCase(",,,,mail, 545, ''\"")]
        public void WhenInvalidFieldsWithSpecialSymbolsParameterPassed_ShouldReturnEmptyDictionary(string invalidFields)
        {
            //Arange
            IFieldsValidator cut = MockRepository.GenerateStub<FieldsValidator>();

            //Act
            Dictionary<string, bool> result = cut.GetValidFields(invalidFields, typeof(DummyObject));

            Assert.IsEmpty(result);
        }
    }
}