using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Core.Tokens;

namespace YamlDotNet.Core
{
	[Serializable]
	public class Scanner : IScanner
	{
		private const int MaxVersionNumberLength = 9;

		private const int MaxBufferLength = 8;

		private static readonly IDictionary<char, char> simpleEscapeCodes = new SortedDictionary<char, char>
		{
			{ '0', '\0' },
			{ 'a', '\a' },
			{ 'b', '\b' },
			{ 't', '\t' },
			{ '\t', '\t' },
			{ 'n', '\n' },
			{ 'v', '\v' },
			{ 'f', '\f' },
			{ 'r', '\r' },
			{ 'e', '\u001b' },
			{ ' ', ' ' },
			{ '"', '"' },
			{ '\'', '\'' },
			{ '\\', '\\' },
			{ 'N', '\u0085' },
			{ '_', '\u00a0' },
			{ 'L', '\u2028' },
			{ 'P', '\u2029' }
		};

		private readonly Stack<int> indents = new Stack<int>();

		private readonly InsertionQueue<Token> tokens = new InsertionQueue<Token>();

		private readonly Stack<SimpleKey> simpleKeys = new Stack<SimpleKey>();

		private readonly CharacterAnalyzer<LookAheadBuffer> analyzer;

		private Cursor cursor;

		private bool streamStartProduced;

		private bool streamEndProduced;

		private int indent = -1;

		private bool simpleKeyAllowed;

		private int flowLevel;

		private int tokensParsed;

		private bool tokenAvailable;

		private Token previous;

		public bool SkipComments { get; private set; }

		public Token Current { get; private set; }

		public Mark CurrentPosition
		{
			get
			{
				return cursor.Mark();
			}
		}

		public Scanner(TextReader input, bool skipComments = true)
		{
			analyzer = new CharacterAnalyzer<LookAheadBuffer>(new LookAheadBuffer(input, 8));
			cursor = new Cursor();
			SkipComments = skipComments;
		}

		public bool MoveNext()
		{
			if (Current != null)
			{
				ConsumeCurrent();
			}
			return InternalMoveNext();
		}

		internal bool InternalMoveNext()
		{
			if (!tokenAvailable && !streamEndProduced)
			{
				FetchMoreTokens();
			}
			if (tokens.Count > 0)
			{
				Current = tokens.Dequeue();
				tokenAvailable = false;
				return true;
			}
			Current = null;
			return false;
		}

		internal void ConsumeCurrent()
		{
			tokensParsed++;
			tokenAvailable = false;
			previous = Current;
			Current = null;
		}

		private char ReadCurrentCharacter()
		{
			char result = analyzer.Peek(0);
			Skip();
			return result;
		}

		private char ReadLine()
		{
			if (analyzer.Check("\r\n\u0085"))
			{
				SkipLine();
				return '\n';
			}
			char result = analyzer.Peek(0);
			SkipLine();
			return result;
		}

		private void FetchMoreTokens()
		{
			while (true)
			{
				bool flag = false;
				if (tokens.Count == 0)
				{
					flag = true;
				}
				else
				{
					StaleSimpleKeys();
					foreach (SimpleKey simpleKey in simpleKeys)
					{
						if (simpleKey.IsPossible && simpleKey.TokenNumber == tokensParsed)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					break;
				}
				FetchNextToken();
			}
			tokenAvailable = true;
		}

		private static bool StartsWith(StringBuilder what, char start)
		{
			return what.Length > 0 && what[0] == start;
		}

		private void StaleSimpleKeys()
		{
			foreach (SimpleKey simpleKey in simpleKeys)
			{
				if (simpleKey.IsPossible && (simpleKey.Line < cursor.Line || simpleKey.Index + 1024 < cursor.Index))
				{
					if (simpleKey.IsRequired)
					{
						Mark mark = cursor.Mark();
						throw new SyntaxErrorException(mark, mark, "While scanning a simple key, could not find expected ':'.");
					}
					simpleKey.IsPossible = false;
				}
			}
		}

		private void FetchNextToken()
		{
			if (!streamStartProduced)
			{
				FetchStreamStart();
				return;
			}
			ScanToNextToken();
			StaleSimpleKeys();
			UnrollIndent(cursor.LineOffset);
			analyzer.Buffer.Cache(4);
			if (analyzer.Buffer.EndOfInput)
			{
				FetchStreamEnd();
				return;
			}
			if (cursor.LineOffset == 0 && analyzer.Check('%'))
			{
				FetchDirective();
				return;
			}
			if (cursor.LineOffset == 0 && analyzer.Check('-') && analyzer.Check('-', 1) && analyzer.Check('-', 2) && analyzer.IsWhiteBreakOrZero(3))
			{
				FetchDocumentIndicator(true);
				return;
			}
			if (cursor.LineOffset == 0 && analyzer.Check('.') && analyzer.Check('.', 1) && analyzer.Check('.', 2) && analyzer.IsWhiteBreakOrZero(3))
			{
				FetchDocumentIndicator(false);
				return;
			}
			if (analyzer.Check('['))
			{
				FetchFlowCollectionStart(true);
				return;
			}
			if (analyzer.Check('{'))
			{
				FetchFlowCollectionStart(false);
				return;
			}
			if (analyzer.Check(']'))
			{
				FetchFlowCollectionEnd(true);
				return;
			}
			if (analyzer.Check('}'))
			{
				FetchFlowCollectionEnd(false);
				return;
			}
			if (analyzer.Check(','))
			{
				FetchFlowEntry();
				return;
			}
			if (analyzer.Check('-') && analyzer.IsWhiteBreakOrZero(1))
			{
				FetchBlockEntry();
				return;
			}
			if (analyzer.Check('?') && (flowLevel > 0 || analyzer.IsWhiteBreakOrZero(1)))
			{
				FetchKey();
				return;
			}
			if (analyzer.Check(':') && (flowLevel > 0 || analyzer.IsWhiteBreakOrZero(1)))
			{
				FetchValue();
				return;
			}
			if (analyzer.Check('*'))
			{
				FetchAnchor(true);
				return;
			}
			if (analyzer.Check('&'))
			{
				FetchAnchor(false);
				return;
			}
			if (analyzer.Check('!'))
			{
				FetchTag();
				return;
			}
			if (analyzer.Check('|') && flowLevel == 0)
			{
				FetchBlockScalar(true);
				return;
			}
			if (analyzer.Check('>') && flowLevel == 0)
			{
				FetchBlockScalar(false);
				return;
			}
			if (analyzer.Check('\''))
			{
				FetchFlowScalar(true);
				return;
			}
			if (analyzer.Check('"'))
			{
				FetchFlowScalar(false);
				return;
			}
			if ((!analyzer.IsWhiteBreakOrZero(0) && !analyzer.Check("-?:,[]{}#&*!|>'\"%@`")) || (analyzer.Check('-') && !analyzer.IsWhite(1)) || (flowLevel == 0 && analyzer.Check("?:") && !analyzer.IsWhiteBreakOrZero(1)))
			{
				FetchPlainScalar();
				return;
			}
			Mark start = cursor.Mark();
			Skip();
			Mark end = cursor.Mark();
			throw new SyntaxErrorException(start, end, "While scanning for the next token, find character that cannot start any token.");
		}

		private bool CheckWhiteSpace()
		{
			return analyzer.Check(' ') || ((flowLevel > 0 || !simpleKeyAllowed) && analyzer.Check('\t'));
		}

		private bool IsDocumentIndicator()
		{
			if (cursor.LineOffset == 0 && analyzer.IsWhiteBreakOrZero(3))
			{
				bool flag = analyzer.Check('-') && analyzer.Check('-', 1) && analyzer.Check('-', 2);
				bool flag2 = analyzer.Check('.') && analyzer.Check('.', 1) && analyzer.Check('.', 2);
				return flag || flag2;
			}
			return false;
		}

		private void Skip()
		{
			cursor.Skip();
			analyzer.Buffer.Skip(1);
		}

		private void SkipLine()
		{
			if (analyzer.IsCrLf(0))
			{
				cursor.SkipLineByOffset(2);
				analyzer.Buffer.Skip(2);
			}
			else if (analyzer.IsBreak(0))
			{
				cursor.SkipLineByOffset(1);
				analyzer.Buffer.Skip(1);
			}
			else if (!analyzer.IsZero(0))
			{
				throw new InvalidOperationException("Not at a break.");
			}
		}

		private void ScanToNextToken()
		{
			while (true)
			{
				if (CheckWhiteSpace())
				{
					Skip();
					continue;
				}
				ProcessComment();
				if (analyzer.IsBreak(0))
				{
					SkipLine();
					if (flowLevel == 0)
					{
						simpleKeyAllowed = true;
					}
					continue;
				}
				break;
			}
		}

		private void ProcessComment()
		{
			if (analyzer.Check('#'))
			{
				Mark mark = cursor.Mark();
				Skip();
				while (analyzer.IsSpace(0))
				{
					Skip();
				}
				StringBuilder stringBuilder = new StringBuilder();
				while (!analyzer.IsBreakOrZero(0))
				{
					stringBuilder.Append(ReadCurrentCharacter());
				}
				if (!SkipComments)
				{
					bool isInline = previous != null && previous.End.Line == mark.Line && !(previous is StreamStart);
					tokens.Enqueue(new Comment(stringBuilder.ToString(), isInline, mark, cursor.Mark()));
				}
			}
		}

		private void FetchStreamStart()
		{
			simpleKeys.Push(new SimpleKey());
			simpleKeyAllowed = true;
			streamStartProduced = true;
			Mark mark = cursor.Mark();
			tokens.Enqueue(new StreamStart(mark, mark));
		}

		private void UnrollIndent(int column)
		{
			if (flowLevel == 0)
			{
				while (indent > column)
				{
					Mark mark = cursor.Mark();
					tokens.Enqueue(new BlockEnd(mark, mark));
					indent = indents.Pop();
				}
			}
		}

		private void FetchStreamEnd()
		{
			cursor.ForceSkipLineAfterNonBreak();
			UnrollIndent(-1);
			RemoveSimpleKey();
			simpleKeyAllowed = false;
			streamEndProduced = true;
			Mark mark = cursor.Mark();
			tokens.Enqueue(new StreamEnd(mark, mark));
		}

		private void FetchDirective()
		{
			UnrollIndent(-1);
			RemoveSimpleKey();
			simpleKeyAllowed = false;
			Token item = ScanDirective();
			tokens.Enqueue(item);
		}

		private Token ScanDirective()
		{
			Mark start = cursor.Mark();
			Skip();
			Token result;
			switch (ScanDirectiveName(start))
			{
			default:
			{
				int num = 1;
				if (num == 1)
				{
					result = ScanTagDirectiveValue(start);
					break;
				}
				throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a directive, find uknown directive name.");
			}
			case "YAML":
				result = ScanVersionDirectiveValue(start);
				break;
			}
			while (analyzer.IsWhite(0))
			{
				Skip();
			}
			ProcessComment();
			if (!analyzer.IsBreakOrZero(0))
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a directive, did not find expected comment or line break.");
			}
			if (analyzer.IsBreak(0))
			{
				SkipLine();
			}
			return result;
		}

		private void FetchDocumentIndicator(bool isStartToken)
		{
			UnrollIndent(-1);
			RemoveSimpleKey();
			simpleKeyAllowed = false;
			Mark mark = cursor.Mark();
			Skip();
			Skip();
			Skip();
			Token item = ((!isStartToken) ? ((Token)new DocumentEnd(mark, mark)) : ((Token)new DocumentStart(mark, cursor.Mark())));
			tokens.Enqueue(item);
		}

		private void FetchFlowCollectionStart(bool isSequenceToken)
		{
			SaveSimpleKey();
			IncreaseFlowLevel();
			simpleKeyAllowed = true;
			Mark mark = cursor.Mark();
			Skip();
			Token item = ((!isSequenceToken) ? ((Token)new FlowMappingStart(mark, mark)) : ((Token)new FlowSequenceStart(mark, mark)));
			tokens.Enqueue(item);
		}

		private void IncreaseFlowLevel()
		{
			simpleKeys.Push(new SimpleKey());
			flowLevel++;
		}

		private void FetchFlowCollectionEnd(bool isSequenceToken)
		{
			RemoveSimpleKey();
			DecreaseFlowLevel();
			simpleKeyAllowed = false;
			Mark mark = cursor.Mark();
			Skip();
			Token item = ((!isSequenceToken) ? ((Token)new FlowMappingEnd(mark, mark)) : ((Token)new FlowSequenceEnd(mark, mark)));
			tokens.Enqueue(item);
		}

		private void DecreaseFlowLevel()
		{
			if (flowLevel > 0)
			{
				flowLevel--;
				simpleKeys.Pop();
			}
		}

		private void FetchFlowEntry()
		{
			RemoveSimpleKey();
			simpleKeyAllowed = true;
			Mark start = cursor.Mark();
			Skip();
			tokens.Enqueue(new FlowEntry(start, cursor.Mark()));
		}

		private void FetchBlockEntry()
		{
			if (flowLevel == 0)
			{
				if (!simpleKeyAllowed)
				{
					Mark mark = cursor.Mark();
					throw new SyntaxErrorException(mark, mark, "Block sequence entries are not allowed in this context.");
				}
				RollIndent(cursor.LineOffset, -1, true, cursor.Mark());
			}
			RemoveSimpleKey();
			simpleKeyAllowed = true;
			Mark start = cursor.Mark();
			Skip();
			tokens.Enqueue(new BlockEntry(start, cursor.Mark()));
		}

		private void FetchKey()
		{
			if (flowLevel == 0)
			{
				if (!simpleKeyAllowed)
				{
					Mark mark = cursor.Mark();
					throw new SyntaxErrorException(mark, mark, "Mapping keys are not allowed in this context.");
				}
				RollIndent(cursor.LineOffset, -1, false, cursor.Mark());
			}
			RemoveSimpleKey();
			simpleKeyAllowed = flowLevel == 0;
			Mark start = cursor.Mark();
			Skip();
			tokens.Enqueue(new Key(start, cursor.Mark()));
		}

		private void FetchValue()
		{
			SimpleKey simpleKey = simpleKeys.Peek();
			if (simpleKey.IsPossible)
			{
				tokens.Insert(simpleKey.TokenNumber - tokensParsed, new Key(simpleKey.Mark, simpleKey.Mark));
				RollIndent(simpleKey.LineOffset, simpleKey.TokenNumber, false, simpleKey.Mark);
				simpleKey.IsPossible = false;
				simpleKeyAllowed = false;
			}
			else
			{
				if (flowLevel == 0)
				{
					if (!simpleKeyAllowed)
					{
						Mark mark = cursor.Mark();
						throw new SyntaxErrorException(mark, mark, "Mapping values are not allowed in this context.");
					}
					RollIndent(cursor.LineOffset, -1, false, cursor.Mark());
				}
				simpleKeyAllowed = flowLevel == 0;
			}
			Mark start = cursor.Mark();
			Skip();
			tokens.Enqueue(new Value(start, cursor.Mark()));
		}

		private void RollIndent(int column, int number, bool isSequence, Mark position)
		{
			if (flowLevel <= 0 && indent < column)
			{
				indents.Push(indent);
				indent = column;
				Token item = ((!isSequence) ? ((Token)new BlockMappingStart(position, position)) : ((Token)new BlockSequenceStart(position, position)));
				if (number == -1)
				{
					tokens.Enqueue(item);
				}
				else
				{
					tokens.Insert(number - tokensParsed, item);
				}
			}
		}

		private void FetchAnchor(bool isAlias)
		{
			SaveSimpleKey();
			simpleKeyAllowed = false;
			tokens.Enqueue(ScanAnchor(isAlias));
		}

		private Token ScanAnchor(bool isAlias)
		{
			Mark start = cursor.Mark();
			Skip();
			StringBuilder stringBuilder = new StringBuilder();
			while (analyzer.IsAlphaNumericDashOrUnderscore(0))
			{
				stringBuilder.Append(ReadCurrentCharacter());
			}
			if (stringBuilder.Length == 0 || (!analyzer.IsWhiteBreakOrZero(0) && !analyzer.Check("?:,]}%@`")))
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While scanning an anchor or alias, did not find expected alphabetic or numeric character.");
			}
			if (isAlias)
			{
				return new AnchorAlias(stringBuilder.ToString(), start, cursor.Mark());
			}
			return new Anchor(stringBuilder.ToString(), start, cursor.Mark());
		}

		private void FetchTag()
		{
			SaveSimpleKey();
			simpleKeyAllowed = false;
			tokens.Enqueue(ScanTag());
		}

		private Token ScanTag()
		{
			Mark start = cursor.Mark();
			string text;
			string text2;
			if (analyzer.Check('<', 1))
			{
				text = string.Empty;
				Skip();
				Skip();
				text2 = ScanTagUri(null, start);
				if (!analyzer.Check('>'))
				{
					throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a tag, did not find the expected '>'.");
				}
				Skip();
			}
			else
			{
				string text3 = ScanTagHandle(false, start);
				if (text3.Length > 1 && text3[0] == '!' && text3[text3.Length - 1] == '!')
				{
					text = text3;
					text2 = ScanTagUri(null, start);
				}
				else
				{
					text2 = ScanTagUri(text3, start);
					text = "!";
					if (text2.Length == 0)
					{
						text2 = text;
						text = string.Empty;
					}
				}
			}
			if (!analyzer.IsWhiteBreakOrZero(0))
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a tag, did not find expected whitespace or line break.");
			}
			return new Tag(text, text2, start, cursor.Mark());
		}

		private void FetchBlockScalar(bool isLiteral)
		{
			RemoveSimpleKey();
			simpleKeyAllowed = true;
			tokens.Enqueue(ScanBlockScalar(isLiteral));
		}

		private Token ScanBlockScalar(bool isLiteral)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			int num = 0;
			int num2 = 0;
			int currentIndent = 0;
			bool flag = false;
			Mark start = cursor.Mark();
			Skip();
			if (analyzer.Check("+-"))
			{
				num = (analyzer.Check('+') ? 1 : (-1));
				Skip();
				if (analyzer.IsDigit(0))
				{
					if (analyzer.Check('0'))
					{
						throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a block scalar, find an intendation indicator equal to 0.");
					}
					num2 = analyzer.AsDigit(0);
					Skip();
				}
			}
			else if (analyzer.IsDigit(0))
			{
				if (analyzer.Check('0'))
				{
					throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a block scalar, find an intendation indicator equal to 0.");
				}
				num2 = analyzer.AsDigit(0);
				Skip();
				if (analyzer.Check("+-"))
				{
					num = (analyzer.Check('+') ? 1 : (-1));
					Skip();
				}
			}
			while (analyzer.IsWhite(0))
			{
				Skip();
			}
			ProcessComment();
			if (!analyzer.IsBreakOrZero(0))
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a block scalar, did not find expected comment or line break.");
			}
			if (analyzer.IsBreak(0))
			{
				SkipLine();
			}
			Mark end = cursor.Mark();
			if (num2 != 0)
			{
				currentIndent = ((indent < 0) ? num2 : (indent + num2));
			}
			currentIndent = ScanBlockScalarBreaks(currentIndent, stringBuilder3, start, ref end);
			while (cursor.LineOffset == currentIndent && !analyzer.IsZero(0))
			{
				bool flag2 = analyzer.IsWhite(0);
				if (!isLiteral && StartsWith(stringBuilder2, '\n') && !flag && !flag2)
				{
					if (stringBuilder3.Length == 0)
					{
						stringBuilder.Append(' ');
					}
					stringBuilder2.Length = 0;
				}
				else
				{
					stringBuilder.Append(stringBuilder2.ToString());
					stringBuilder2.Length = 0;
				}
				stringBuilder.Append(stringBuilder3.ToString());
				stringBuilder3.Length = 0;
				flag = analyzer.IsWhite(0);
				while (!analyzer.IsBreakOrZero(0))
				{
					stringBuilder.Append(ReadCurrentCharacter());
				}
				stringBuilder2.Append(ReadLine());
				currentIndent = ScanBlockScalarBreaks(currentIndent, stringBuilder3, start, ref end);
			}
			if (num != -1)
			{
				stringBuilder.Append(stringBuilder2);
			}
			if (num == 1)
			{
				stringBuilder.Append(stringBuilder3);
			}
			ScalarStyle style = ((!isLiteral) ? ScalarStyle.Folded : ScalarStyle.Literal);
			return new Scalar(stringBuilder.ToString(), style, start, end);
		}

		private int ScanBlockScalarBreaks(int currentIndent, StringBuilder breaks, Mark start, ref Mark end)
		{
			int num = 0;
			end = cursor.Mark();
			while (true)
			{
				if ((currentIndent == 0 || cursor.LineOffset < currentIndent) && analyzer.IsSpace(0))
				{
					Skip();
					continue;
				}
				if (cursor.LineOffset > num)
				{
					num = cursor.LineOffset;
				}
				if ((currentIndent == 0 || cursor.LineOffset < currentIndent) && analyzer.IsTab(0))
				{
					throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a block scalar, find a tab character where an intendation space is expected.");
				}
				if (!analyzer.IsBreak(0))
				{
					break;
				}
				breaks.Append(ReadLine());
				end = cursor.Mark();
			}
			if (currentIndent == 0)
			{
				currentIndent = Math.Max(num, Math.Max(indent + 1, 1));
			}
			return currentIndent;
		}

		private void FetchFlowScalar(bool isSingleQuoted)
		{
			SaveSimpleKey();
			simpleKeyAllowed = false;
			tokens.Enqueue(ScanFlowScalar(isSingleQuoted));
		}

		private Token ScanFlowScalar(bool isSingleQuoted)
		{
			Mark start = cursor.Mark();
			Skip();
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			StringBuilder stringBuilder4 = new StringBuilder();
			while (true)
			{
				if (IsDocumentIndicator())
				{
					throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a quoted scalar, find unexpected document indicator.");
				}
				if (analyzer.IsZero(0))
				{
					throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a quoted scalar, find unexpected end of stream.");
				}
				bool flag = false;
				while (!analyzer.IsWhiteBreakOrZero(0))
				{
					if (isSingleQuoted && analyzer.Check('\'') && analyzer.Check('\'', 1))
					{
						stringBuilder.Append('\'');
						Skip();
						Skip();
						continue;
					}
					if (analyzer.Check((!isSingleQuoted) ? '"' : '\''))
					{
						break;
					}
					if (!isSingleQuoted && analyzer.Check('\\') && analyzer.IsBreak(1))
					{
						Skip();
						SkipLine();
						flag = true;
						break;
					}
					if (!isSingleQuoted && analyzer.Check('\\'))
					{
						int num = 0;
						char c = analyzer.Peek(1);
						switch (c)
						{
						case 'x':
							num = 2;
							break;
						case 'u':
							num = 4;
							break;
						case 'U':
							num = 8;
							break;
						default:
						{
							char value;
							if (simpleEscapeCodes.TryGetValue(c, out value))
							{
								stringBuilder.Append(value);
								break;
							}
							throw new SyntaxErrorException(start, cursor.Mark(), "While parsing a quoted scalar, find unknown escape character.");
						}
						}
						Skip();
						Skip();
						if (num <= 0)
						{
							continue;
						}
						uint num2 = 0u;
						for (int i = 0; i < num; i++)
						{
							if (!analyzer.IsHex(i))
							{
								throw new SyntaxErrorException(start, cursor.Mark(), "While parsing a quoted scalar, did not find expected hexdecimal number.");
							}
							num2 = (uint)((num2 << 4) + analyzer.AsHex(i));
						}
						if ((num2 >= 55296 && num2 <= 57343) || num2 > 1114111)
						{
							throw new SyntaxErrorException(start, cursor.Mark(), "While parsing a quoted scalar, find invalid Unicode character escape code.");
						}
						stringBuilder.Append((char)num2);
						for (int j = 0; j < num; j++)
						{
							Skip();
						}
					}
					else
					{
						stringBuilder.Append(ReadCurrentCharacter());
					}
				}
				if (analyzer.Check((!isSingleQuoted) ? '"' : '\''))
				{
					break;
				}
				while (analyzer.IsWhite(0) || analyzer.IsBreak(0))
				{
					if (analyzer.IsWhite(0))
					{
						if (!flag)
						{
							stringBuilder2.Append(ReadCurrentCharacter());
						}
						else
						{
							Skip();
						}
					}
					else if (!flag)
					{
						stringBuilder2.Length = 0;
						stringBuilder3.Append(ReadLine());
						flag = true;
					}
					else
					{
						stringBuilder4.Append(ReadLine());
					}
				}
				if (flag)
				{
					if (StartsWith(stringBuilder3, '\n'))
					{
						if (stringBuilder4.Length == 0)
						{
							stringBuilder.Append(' ');
						}
						else
						{
							stringBuilder.Append(stringBuilder4.ToString());
						}
					}
					else
					{
						stringBuilder.Append(stringBuilder3.ToString());
						stringBuilder.Append(stringBuilder4.ToString());
					}
					stringBuilder3.Length = 0;
					stringBuilder4.Length = 0;
				}
				else
				{
					stringBuilder.Append(stringBuilder2.ToString());
					stringBuilder2.Length = 0;
				}
			}
			Skip();
			return new Scalar(stringBuilder.ToString(), (!isSingleQuoted) ? ScalarStyle.DoubleQuoted : ScalarStyle.SingleQuoted);
		}

		private void FetchPlainScalar()
		{
			SaveSimpleKey();
			simpleKeyAllowed = false;
			tokens.Enqueue(ScanPlainScalar());
		}

		private Token ScanPlainScalar()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			StringBuilder stringBuilder4 = new StringBuilder();
			bool flag = false;
			int num = indent + 1;
			Mark mark = cursor.Mark();
			Mark end = mark;
			while (!IsDocumentIndicator() && !analyzer.Check('#'))
			{
				while (!analyzer.IsWhiteBreakOrZero(0))
				{
					if (flowLevel > 0 && analyzer.Check(':') && !analyzer.IsWhiteBreakOrZero(1))
					{
						throw new SyntaxErrorException(mark, cursor.Mark(), "While scanning a plain scalar, find unexpected ':'.");
					}
					if ((analyzer.Check(':') && analyzer.IsWhiteBreakOrZero(1)) || (flowLevel > 0 && analyzer.Check(",:?[]{}")))
					{
						break;
					}
					if (flag || stringBuilder2.Length > 0)
					{
						if (flag)
						{
							if (StartsWith(stringBuilder3, '\n'))
							{
								if (stringBuilder4.Length == 0)
								{
									stringBuilder.Append(' ');
								}
								else
								{
									stringBuilder.Append(stringBuilder4);
								}
							}
							else
							{
								stringBuilder.Append(stringBuilder3);
								stringBuilder.Append(stringBuilder4);
							}
							stringBuilder3.Length = 0;
							stringBuilder4.Length = 0;
							flag = false;
						}
						else
						{
							stringBuilder.Append(stringBuilder2);
							stringBuilder2.Length = 0;
						}
					}
					stringBuilder.Append(ReadCurrentCharacter());
					end = cursor.Mark();
				}
				if (!analyzer.IsWhite(0) && !analyzer.IsBreak(0))
				{
					break;
				}
				while (analyzer.IsWhite(0) || analyzer.IsBreak(0))
				{
					if (analyzer.IsWhite(0))
					{
						if (flag && cursor.LineOffset < num && analyzer.IsTab(0))
						{
							throw new SyntaxErrorException(mark, cursor.Mark(), "While scanning a plain scalar, find a tab character that violate intendation.");
						}
						if (!flag)
						{
							stringBuilder2.Append(ReadCurrentCharacter());
						}
						else
						{
							Skip();
						}
					}
					else if (!flag)
					{
						stringBuilder2.Length = 0;
						stringBuilder3.Append(ReadLine());
						flag = true;
					}
					else
					{
						stringBuilder4.Append(ReadLine());
					}
				}
				if (flowLevel == 0 && cursor.LineOffset < num)
				{
					break;
				}
			}
			if (flag)
			{
				simpleKeyAllowed = true;
			}
			return new Scalar(stringBuilder.ToString(), ScalarStyle.Plain, mark, end);
		}

		private void RemoveSimpleKey()
		{
			SimpleKey simpleKey = simpleKeys.Peek();
			if (simpleKey.IsPossible && simpleKey.IsRequired)
			{
				throw new SyntaxErrorException(simpleKey.Mark, simpleKey.Mark, "While scanning a simple key, could not find expected ':'.");
			}
			simpleKey.IsPossible = false;
		}

		private string ScanDirectiveName(Mark start)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (analyzer.IsAlphaNumericDashOrUnderscore(0))
			{
				stringBuilder.Append(ReadCurrentCharacter());
			}
			if (stringBuilder.Length == 0)
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a directive, could not find expected directive name.");
			}
			if (!analyzer.IsWhiteBreakOrZero(0))
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a directive, find unexpected non-alphabetical character.");
			}
			return stringBuilder.ToString();
		}

		private void SkipWhitespaces()
		{
			while (analyzer.IsWhite(0))
			{
				Skip();
			}
		}

		private Token ScanVersionDirectiveValue(Mark start)
		{
			SkipWhitespaces();
			int major = ScanVersionDirectiveNumber(start);
			if (!analyzer.Check('.'))
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a %YAML directive, did not find expected digit or '.' character.");
			}
			Skip();
			int minor = ScanVersionDirectiveNumber(start);
			return new VersionDirective(new Version(major, minor), start, start);
		}

		private Token ScanTagDirectiveValue(Mark start)
		{
			SkipWhitespaces();
			string handle = ScanTagHandle(true, start);
			if (!analyzer.IsWhite(0))
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a %TAG directive, did not find expected whitespace.");
			}
			SkipWhitespaces();
			string prefix = ScanTagUri(null, start);
			if (!analyzer.IsWhiteBreakOrZero(0))
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a %TAG directive, did not find expected whitespace or line break.");
			}
			return new TagDirective(handle, prefix, start, start);
		}

		private string ScanTagUri(string head, Mark start)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (head != null && head.Length > 1)
			{
				stringBuilder.Append(head.Substring(1));
			}
			while (analyzer.IsAlphaNumericDashOrUnderscore(0) || analyzer.Check(";/?:@&=+$,.!~*'()[]%"))
			{
				if (analyzer.Check('%'))
				{
					stringBuilder.Append(ScanUriEscapes(start));
				}
				else
				{
					stringBuilder.Append(ReadCurrentCharacter());
				}
			}
			if (stringBuilder.Length == 0)
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While parsing a tag, did not find expected tag URI.");
			}
			return stringBuilder.ToString();
		}

		private char ScanUriEscapes(Mark start)
		{
			List<byte> list = new List<byte>();
			int num = 0;
			do
			{
				if (!analyzer.Check('%') || !analyzer.IsHex(1) || !analyzer.IsHex(2))
				{
					throw new SyntaxErrorException(start, cursor.Mark(), "While parsing a tag, did not find URI escaped octet.");
				}
				int num2 = (analyzer.AsHex(1) << 4) + analyzer.AsHex(2);
				if (num == 0)
				{
					num = (((num2 & 0x80) == 0) ? 1 : (((num2 & 0xE0) == 192) ? 2 : (((num2 & 0xF0) == 224) ? 3 : (((num2 & 0xF8) == 240) ? 4 : 0))));
					if (num == 0)
					{
						throw new SyntaxErrorException(start, cursor.Mark(), "While parsing a tag, find an incorrect leading UTF-8 octet.");
					}
				}
				else if ((num2 & 0xC0) != 128)
				{
					throw new SyntaxErrorException(start, cursor.Mark(), "While parsing a tag, find an incorrect trailing UTF-8 octet.");
				}
				list.Add((byte)num2);
				Skip();
				Skip();
				Skip();
			}
			while (--num > 0);
			char[] chars = Encoding.UTF8.GetChars(list.ToArray());
			if (chars.Length != 1)
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While parsing a tag, find an incorrect UTF-8 sequence.");
			}
			return chars[0];
		}

		private string ScanTagHandle(bool isDirective, Mark start)
		{
			if (!analyzer.Check('!'))
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a tag, did not find expected '!'.");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(ReadCurrentCharacter());
			while (analyzer.IsAlphaNumericDashOrUnderscore(0))
			{
				stringBuilder.Append(ReadCurrentCharacter());
			}
			if (analyzer.Check('!'))
			{
				stringBuilder.Append(ReadCurrentCharacter());
			}
			else if (isDirective && (stringBuilder.Length != 1 || stringBuilder[0] != '!'))
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While parsing a tag directive, did not find expected '!'.");
			}
			return stringBuilder.ToString();
		}

		private int ScanVersionDirectiveNumber(Mark start)
		{
			int num = 0;
			int num2 = 0;
			while (analyzer.IsDigit(0))
			{
				if (++num2 > 9)
				{
					throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a %YAML directive, find extremely long version number.");
				}
				num = num * 10 + analyzer.AsDigit(0);
				Skip();
			}
			if (num2 == 0)
			{
				throw new SyntaxErrorException(start, cursor.Mark(), "While scanning a %YAML directive, did not find expected version number.");
			}
			return num;
		}

		private void SaveSimpleKey()
		{
			bool isRequired = flowLevel == 0 && indent == cursor.LineOffset;
			if (simpleKeyAllowed)
			{
				SimpleKey t = new SimpleKey(true, isRequired, tokensParsed + tokens.Count, cursor);
				RemoveSimpleKey();
				simpleKeys.Pop();
				simpleKeys.Push(t);
			}
		}
	}
}
