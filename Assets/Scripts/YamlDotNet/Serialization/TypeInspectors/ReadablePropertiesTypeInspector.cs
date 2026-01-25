using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YamlDotNet.Serialization.TypeInspectors
{
	public sealed class ReadablePropertiesTypeInspector : TypeInspectorSkeleton
	{
		private sealed class ReflectionPropertyDescriptor : IPropertyDescriptor
		{
			private readonly PropertyInfo _propertyInfo;

			private readonly ITypeResolver _typeResolver;

			public string Name
			{
				get
				{
					return _propertyInfo.Name;
				}
			}

			public Type Type
			{
				get
				{
					return _propertyInfo.PropertyType;
				}
			}

			public Type TypeOverride { get; set; }

			public int Order { get; set; }

			public bool CanWrite
			{
				get
				{
					return _propertyInfo.CanWrite;
				}
			}

			public ReflectionPropertyDescriptor(PropertyInfo propertyInfo, ITypeResolver typeResolver)
			{
				_propertyInfo = propertyInfo;
				_typeResolver = typeResolver;
			}

			public void Write(object target, object value)
			{
				_propertyInfo.SetValue(target, value, null);
			}

			public T GetCustomAttribute<T>() where T : Attribute
			{
				object[] customAttributes = _propertyInfo.GetCustomAttributes(typeof(T), true);
				return (T)customAttributes.FirstOrDefault();
			}

			public IObjectDescriptor Read(object target)
			{
				object value = _propertyInfo.GetValue(target, null);
				Type type = TypeOverride ?? _typeResolver.Resolve(Type, value);
				return new ObjectDescriptor(value, type, Type);
			}
		}

		private readonly ITypeResolver _typeResolver;

		public ReadablePropertiesTypeInspector(ITypeResolver typeResolver)
		{
			if (typeResolver == null)
			{
				throw new ArgumentNullException("typeResolver");
			}
			_typeResolver = typeResolver;
		}

		private static bool IsValidProperty(PropertyInfo property)
		{
			return property.CanRead && property.GetGetMethod().GetParameters().Length == 0;
		}

		public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
		{
			return type.GetPublicProperties().Where(IsValidProperty).Select((Func<PropertyInfo, IPropertyDescriptor>)((PropertyInfo p) => new ReflectionPropertyDescriptor(p, _typeResolver)));
		}
	}
}
