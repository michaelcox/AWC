using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AWC.Domain.Metadata;

namespace AWC.WebUI.Utils
{
    public class CustomMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            var options = attributes.OfType<OptionAttribute>();
            if (options.Any())
            {
                metadata.AdditionalValues["optionValues"]
                     = options.ToDictionary(x => x.Value, x => x.DisplayText);
                metadata.TemplateHint = "RadioButtons";
            }

            return metadata;
        }
    }
}