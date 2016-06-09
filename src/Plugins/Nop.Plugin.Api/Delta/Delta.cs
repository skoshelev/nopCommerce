using System.Collections.Generic;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Delta
{
    public class Delta<TDto> where TDto : class, new()
    {
        private TDto _dto;

        private IMappingHelper _mappingHelper = EngineContext.Current.Resolve<IMappingHelper>();

        public TDto Dto
        {
            get
            {
                if (_dto == null)
                {
                    _dto = new TDto();
                }

                return _dto;
            }
            set { _dto = value; }
        }
        
        public Delta(Dictionary<string, object> passedJsonPropertyValuePaires)
        {
            FillDto(passedJsonPropertyValuePaires);
        }

        public void Merge<TEntity>(TEntity entity)
        {
            Dictionary<string, object> changedProperties = _mappingHelper.GetChangedProperties();

            var entityProperties = entity.GetType().GetProperties();

            // Set the changed properties
            foreach (var property in entityProperties)
            {
                // its a changed property so we need to update its value.
                if (changedProperties.ContainsKey(property.Name))
                {
                    // The value-type validation will happen in the model binder so here we expect the values to correspond to the types.
                    _mappingHelper.ConverAndSetValueIfValid(entity, property, changedProperties[property.Name]);
                  
                    // The remove operation is O(1) complexity. We are doing this for optimization purposes. 
                    // So we can break the loop if there are no more changed properties.
                    changedProperties.Remove(property.Name);
                }

                if (changedProperties.Count == 0) break;
            }
        }

        private void FillDto(Dictionary<string, object> passedJsonPropertyValuePaires)
        {
            _mappingHelper.SetValues(passedJsonPropertyValuePaires, Dto, typeof(TDto));
        }
    }
}