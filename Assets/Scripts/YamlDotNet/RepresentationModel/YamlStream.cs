using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	public class YamlStream : IEnumerable, IEnumerable<YamlDocument>
	{
		private readonly IList<YamlDocument> documents = new List<YamlDocument>();

		public IList<YamlDocument> Documents
		{
			get
			{
				return documents;
			}
		}

		public YamlStream()
		{
		}

		public YamlStream(params YamlDocument[] documents)
			: this((IEnumerable<YamlDocument>)documents)
		{
		}

		public YamlStream(IEnumerable<YamlDocument> documents)
		{
			foreach (YamlDocument document in documents)
			{
				this.documents.Add(document);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(YamlDocument document)
		{
			documents.Add(document);
		}

		public void Load(TextReader input)
		{
			documents.Clear();
			Parser parser = new Parser(input);
			EventReader eventReader = new EventReader(parser);
			eventReader.Expect<StreamStart>();
			while (!eventReader.Accept<StreamEnd>())
			{
				YamlDocument item = new YamlDocument(eventReader);
				documents.Add(item);
			}
			eventReader.Expect<StreamEnd>();
		}

		public void Save(TextWriter output, bool useAnchors = true)
		{
			IEmitter emitter = new Emitter(output);
			emitter.Emit(new StreamStart());
			foreach (YamlDocument document in documents)
			{
				document.Save(emitter, useAnchors);
			}
			emitter.Emit(new StreamEnd());
		}

		public void Accept(IYamlVisitor visitor)
		{
			visitor.Visit(this);
		}

		public IEnumerator<YamlDocument> GetEnumerator()
		{
			return documents.GetEnumerator();
		}
	}
}
