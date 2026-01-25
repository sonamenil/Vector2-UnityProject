using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class DefaultExclusiveObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		private static readonly IEqualityComparer<object> _objectComparer = EqualityComparer<object>.Default;

		public DefaultExclusiveObjectGraphVisitor(IObjectGraphVisitor nextVisitor)
			: base(nextVisitor)
		{
		}

		private static object GetDefault(Type type)
		{
			return (!type.IsValueType()) ? null : Activator.CreateInstance(type);
		}

		public override bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value)
		{
			return !_objectComparer.Equals(value, GetDefault(value.Type)) && base.EnterMapping(key, value);
		}

		public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value)
		{
			DefaultValueAttribute customAttribute = key.GetCustomAttribute<DefaultValueAttribute>();
			object y = ((customAttribute == null) ? GetDefault(key.Type) : customAttribute.Value);
			return !_objectComparer.Equals(value.Value, y) && base.EnterMapping(key, value);
		}
	}
}
