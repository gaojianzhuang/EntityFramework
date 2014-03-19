// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity
{
    // TODO: This is temporary convention to add each property to the model
    public class SimpleTemporaryConvention
    {
        public virtual void Apply([NotNull] Model model)
        {
            Check.NotNull(model, "model");

            foreach (var entityType in model.EntityTypes)
            {
                foreach (var propertyInfo in entityType.Type.GetRuntimeProperties()
                    .Where(p => !p.IsStatic() 
                        && !p.GetIndexParameters().Any() 
                        && p.PropertyType.Namespace.StartsWith("System")
                        && !p.PropertyType.Namespace.StartsWith("System.Collections")))
                {
                    var property = entityType.AddProperty(propertyInfo);

                    if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        entityType.SetKey(property);

                        if (property.PropertyType == typeof(Guid))
                        {
                            property.ValueGenerationStrategy = ValueGenerationStrategy.Client;
                        }
                    }
                }
            }
        }
    }
}