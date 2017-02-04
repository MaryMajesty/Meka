using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Meka
{
	namespace ByteOperators
	{
		public class ByteReader
		{
			public List<byte> _Bytes;

			public bool CanRead
			{
				get
				{
					if (Compatibility._Old)
						return this._Bytes.Count > 0;
					else
					{
						if (this._Bytes.Count == 0)
							return false;
						else
						{
							Tuple<bool, int> length = this.GetLength();
							return length.Item1 ? this._Bytes.Count >= length.Item2 : false;
						}
					}
				}
			}

			public ByteReader(byte[] _bytes)
			{
				this._Bytes = _bytes.ToList();
			}

			public byte Read()
			{
				byte @out = this._Bytes[0];
				this._Bytes.RemoveAt(0);
				return @out;
			}

			public byte[] Read(int _length)
			{
				List<byte> @out = new List<byte>();
				for (int i = 0; i < _length; i++)
					@out.Add(this._Bytes[i]);
				this._Bytes.RemoveRange(0, _length);
				return @out.ToArray();
			}

			public int ReadDynamic()
			{
				List<bool> bits = new List<bool>();
				while (true)
				{
					bool[] bs = this.Read().ToBits();
					for (int i = 1; i < 8; i++)
						bits.Add(bs[i]);
					if (!bs[0])
						break;
				}
				return bits.ToArray().ToInt();
			}

			public Tuple<bool, int> GetLength()
			{
				List<bool> bits = new List<bool>();
				int c = 0;
				int b = 0;
				while (true)
				{
					if (this._Bytes.Count == c)
						return new Tuple<bool, int>(false, 0);

					b++;

					bool[] bs = this._Bytes[c].ToBits();
					for (int i = 1; i < 8; i++)
						bits.Add(bs[i]);
					if (!bs[0])
						break;
					c++;
				}
				return new Tuple<bool, int>(true, b + bits.ToArray().ToInt());
			}

			public byte[] ReadBatch() { return Read(this.ReadDynamic()); }

			public T ReadCustom<T, TP>() where TP : Hidden.Parser<T>, new()
			{
				ByteReader r = this;
				if (!Compatibility._Old)
					r = new ByteReader(this.ReadBatch());

				TP p = new TP();
				return p.ReadBytes(r);
			}
			
			public void AddBytes(byte[] _bytes)
			{
				this._Bytes.AddRange(_bytes);
			}
		}
	}
}