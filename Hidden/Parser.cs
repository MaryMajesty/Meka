using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka.ByteOperators;

namespace Meka
{
	namespace Hidden
	{
		public abstract class Parser<T>
		{
			internal abstract T ReadBytes(ByteReader _reader);
			internal abstract void WriteBytes(T _t, ByteWriter _writer);
		}
	}
}