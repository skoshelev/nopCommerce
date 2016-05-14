using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper.Internal;

namespace Nop.Plugin.Api.Helpers
{
    public class MappingHelper : IMappingHelper
    {
        public void SetValues(Dictionary<string, object> propertiesAsDictionary, object objectToBeUpdated, Type propertyType)
        {
            // TODO: handle the special case where some field was not set before, but values are send for it.
            if (objectToBeUpdated == null) return;

            foreach (var property in propertiesAsDictionary)
            {
                SetValue(objectToBeUpdated, property, propertyType);
            }
        }

        private void SetValue(object objectToBeUpdated, KeyValuePair<string, object> property, Type propertyType)
        {
            string propertyName = property.Key.Replace("_", string.Empty);
            object propertyValue = property.Value;

            var objectProperty = propertyType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            // This case handles nested properties.
            if (propertyValue != null && propertyValue is Dictionary<string, object>)
            {
                // We need to use GetValue method to get the actual instance of the property. objectProperty is the property info.
                SetValues((Dictionary<string, object>)propertyValue, objectProperty.GetValue(objectToBeUpdated), objectProperty.PropertyType);
                // We expect the nested properties to be classes which are refrence types.
                return;
            }
            // This case hadles collections.
            else if (propertyValue != null && propertyValue is ICollection<object>)
            {
                ICollection<object> propertyValueAsCollection = propertyValue as ICollection<object>;

                propertyValueAsCollection.Each(
                    x =>

                        CreateOrUpdateItemInCollection(x as Dictionary<string, object>,
                                                       objectProperty.GetValue(objectToBeUpdated) as IList,
                                                       objectProperty.PropertyType.GetGenericArguments()[0])
                    );

                return;
            }

            // This is where the new value is beeing set to the object property using the SetValue function part of System.Reflection.
            if (objectProperty != null)
            {
                if (propertyValue == null)
                {
                    objectProperty.SetValue(objectToBeUpdated, null);
                }
                else if (propertyValue is IConvertible)
                {
                    objectProperty.SetValue(objectToBeUpdated,
                        Convert.ChangeType(propertyValue, objectProperty.PropertyType));
                }
                else
                {
                    objectProperty.SetValue(objectToBeUpdated, propertyValue);
                }
            }
        }

        private void CreateOrUpdateItemInCollection(Dictionary<string, object> newProperties, IList collection, Type collectionElementsType)
        {
            if (collectionElementsType.Namespace != "System" && collection != null)
            {
                if (newProperties.ContainsKey("id"))
                {
                    // Every element in collection, that is not System type should have an id.
                    int id = int.Parse(newProperties["id"].ToString());

                    object itemToBeUpdated = null;

                    // Check if there is already an item with this id in the collection.
                    foreach (var item in collection)
                    {
                        if (int.Parse(item.GetType()
                            .GetProperty("id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                            .GetValue(item)
                            .ToString()) == id)
                        {
                            itemToBeUpdated = item;
                            break;
                        }
                    }

                    if (itemToBeUpdated == null)
                    {
                        // We should create a new item and put it in the collection.
                        AddNewItemInCollection(newProperties, collection, collectionElementsType);
                    }
                    else
                    {
                        // We should update the existing element.
                        SetValues(newProperties, itemToBeUpdated, collectionElementsType);
                    }
                }
                // It is a new item.
                else
                {
                    AddNewItemInCollection(newProperties, collection, collectionElementsType);
                }
            }
        }

        private void AddNewItemInCollection(Dictionary<string, object> newProperties, IList collection, Type collectionElementsType)
        {
            var newInstance = Activator.CreateInstance(collectionElementsType);

            var properties = collectionElementsType.GetProperties();

            SetEveryDatePropertyThatIsNotSetToDateTimeUtcNow(newProperties, properties);

            SetValues(newProperties, newInstance, collectionElementsType);

            collection.Add(newInstance);
        }

        // We need this method, because the default value of DateTime is not in the sql server DateTime range and we will get an exception if we use it.
        private void SetEveryDatePropertyThatIsNotSetToDateTimeUtcNow(Dictionary<string, object> newProperties, PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(DateTime))
                {
                    bool keyFound = false;

                    // We need to loop through the keys, because the key may contain underscores in its name, which won't match the property name.
                    foreach (var key in newProperties.Keys)
                    {
                        if (key.Replace("_", string.Empty)
                            .Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            keyFound = true;
                            break;
                        }
                    }

                    if (!keyFound)
                    {
                        // Create the item with the DateTime.NowUtc.
                        newProperties.Add(property.Name, DateTime.UtcNow);
                    }
                }
            }
        }
    }
}