using System;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Models.CategoriesParameters
{
    public class BaseCategoriesParametersModel
    {
        public BaseCategoriesParametersModel()
        {
            ProductId = 0;
            CreatedAtMin = null;
            CreatedAtMax = null;
            UpdatedAtMin = null;
            UpdatedAtMax = null;
            PublishedStatus = null;
        }

        public DateTime? CreatedAtMin { get; set; }

        public DateTime? CreatedAtMax { get; set; }

        public DateTime? UpdatedAtMin { get; set; }

        public DateTime? UpdatedAtMax { get; set; }

        public bool? PublishedStatus { get; set; }

        public int ProductId { get; set; }
    }
}