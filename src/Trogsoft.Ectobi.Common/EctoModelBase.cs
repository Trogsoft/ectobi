using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.Common
{
    public abstract class EctoModelBase<T> : IEctoModel<T>
    {
        public abstract Task<Success<T>> GetPopulatedModel();

        public async Task<Success<List<EctoModelProperty>>> GetProperties()
        {
            List<EctoModelProperty> properties = new List<EctoModelProperty>();

            var typeProperties = typeof(T).GetProperties();
            foreach (var typeProperty in typeProperties)
            {
                var prop = new EctoModelProperty
                {
                    TextId = typeProperty.Name,
                    Name = typeProperty.Name
                };

                var displayName = typeProperty.GetCustomAttribute<DisplayNameAttribute>();
                if (displayName != null)
                    prop.Name = displayName.DisplayName;

                var description = typeProperty.GetCustomAttribute<DescriptionAttribute>();
                if (description != null)
                    prop.Description = description.Description;

                var flags = typeProperty.GetCustomAttribute<ModelFlagsAttribute>();
                if (flags != null)
                    prop.Flags = flags.Flags;

                properties.Add(prop);
            }

            return new Success<List<EctoModelProperty>>(properties);
        }
    }
}
