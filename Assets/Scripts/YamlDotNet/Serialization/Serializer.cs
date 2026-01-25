using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.EventEmitters;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.ObjectGraphTraversalStrategies;
using YamlDotNet.Serialization.ObjectGraphVisitors;
using YamlDotNet.Serialization.TypeInspectors;
using YamlDotNet.Serialization.TypeResolvers;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization
{
	public sealed class Serializer
	{
		private readonly SerializationOptions options;

		private readonly INamingConvention namingConvention;

		private readonly ITypeResolver typeResolver;

		internal IList<IYamlTypeConverter> Converters { get; private set; }

		public Serializer(SerializationOptions options = SerializationOptions.None, INamingConvention namingConvention = null)
		{
			this.options = options;
			this.namingConvention = namingConvention ?? new NullNamingConvention();
			Converters = new List<IYamlTypeConverter>();
			foreach (IYamlTypeConverter builtInConverter in YamlTypeConverters.BuiltInConverters)
			{
				Converters.Add(builtInConverter);
			}
			object dynamicTypeResolver;
			if (IsOptionSet(SerializationOptions.DefaultToStaticType))
			{
				ITypeResolver typeResolver = new StaticTypeResolver();
				dynamicTypeResolver = typeResolver;
			}
			else
			{
				dynamicTypeResolver = new DynamicTypeResolver();
			}
			this.typeResolver = (ITypeResolver)dynamicTypeResolver;
		}

		private bool IsOptionSet(SerializationOptions option)
		{
			return (options & option) != 0;
		}

		public void RegisterTypeConverter(IYamlTypeConverter converter)
		{
			Converters.Add(converter);
		}

		public void Serialize(TextWriter writer, object graph)
		{
			Serialize(new Emitter(writer), graph);
		}

		public void Serialize(TextWriter writer, object graph, Type type)
		{
			Serialize(new Emitter(writer), graph, type);
		}

		public void Serialize(IEmitter emitter, object graph)
		{
			if (emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}
			EmitDocument(emitter, new ObjectDescriptor(graph, (graph == null) ? typeof(object) : graph.GetType(), typeof(object)));
		}

		public void Serialize(IEmitter emitter, object graph, Type type)
		{
			if (emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			EmitDocument(emitter, new ObjectDescriptor(graph, type, type));
		}

		private void EmitDocument(IEmitter emitter, IObjectDescriptor graph)
		{
			IObjectGraphTraversalStrategy objectGraphTraversalStrategy = CreateTraversalStrategy();
			IEventEmitter eventEmitter = CreateEventEmitter(emitter);
			IObjectGraphVisitor visitor = CreateEmittingVisitor(emitter, objectGraphTraversalStrategy, eventEmitter, graph);
			emitter.Emit(new StreamStart());
			emitter.Emit(new DocumentStart());
			objectGraphTraversalStrategy.Traverse(graph, visitor);
			emitter.Emit(new DocumentEnd(true));
			emitter.Emit(new StreamEnd());
		}

		private IObjectGraphVisitor CreateEmittingVisitor(IEmitter emitter, IObjectGraphTraversalStrategy traversalStrategy, IEventEmitter eventEmitter, IObjectDescriptor graph)
		{
			IObjectGraphVisitor nextVisitor = new EmittingObjectGraphVisitor(eventEmitter);
			nextVisitor = new CustomSerializationObjectGraphVisitor(emitter, nextVisitor, Converters);
			if (!IsOptionSet(SerializationOptions.DisableAliases))
			{
				AnchorAssigner anchorAssigner = new AnchorAssigner();
				traversalStrategy.Traverse(graph, anchorAssigner);
				nextVisitor = new AnchorAssigningObjectGraphVisitor(nextVisitor, eventEmitter, anchorAssigner);
			}
			if (!IsOptionSet(SerializationOptions.EmitDefaults))
			{
				nextVisitor = new DefaultExclusiveObjectGraphVisitor(nextVisitor);
			}
			return nextVisitor;
		}

		private IEventEmitter CreateEventEmitter(IEmitter emitter)
		{
			WriterEventEmitter nextEmitter = new WriterEventEmitter(emitter);
			if (IsOptionSet(SerializationOptions.JsonCompatible))
			{
				return new JsonEventEmitter(nextEmitter);
			}
			return new TypeAssigningEventEmitter(nextEmitter, IsOptionSet(SerializationOptions.Roundtrip));
		}

		private IObjectGraphTraversalStrategy CreateTraversalStrategy()
		{
			ITypeInspector innerTypeDescriptor = new ReadablePropertiesTypeInspector(typeResolver);
			if (IsOptionSet(SerializationOptions.Roundtrip))
			{
				innerTypeDescriptor = new ReadableAndWritablePropertiesTypeInspector(innerTypeDescriptor);
			}
			innerTypeDescriptor = new NamingConventionTypeInspector(innerTypeDescriptor, namingConvention);
			innerTypeDescriptor = new YamlAttributesTypeInspector(innerTypeDescriptor);
			if (IsOptionSet(SerializationOptions.Roundtrip))
			{
				return new RoundtripObjectGraphTraversalStrategy(this, innerTypeDescriptor, typeResolver, 50);
			}
			return new FullObjectGraphTraversalStrategy(this, innerTypeDescriptor, typeResolver, 50);
		}
	}
}
