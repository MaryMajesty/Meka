using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meka
{
	namespace ByteOperators
	{
		public class ByteWriter
		{
			private List<byte> _Bytes = new List<byte>();
			
			public byte[] Result
			{
				get { return this._Bytes.ToArray(); }
			}
			
			public void Write(byte _byte) { this._Bytes.Add(_byte); }

			public void Write(byte[] _bytes) { this._Bytes.AddRange(_bytes); }

			public void WriteBatch(byte[] _bytes)
			{
				WriteDynamic(_bytes.Length);
				Write(_bytes);
			}

			public void WriteDynamic(int _length)
			{
				bool[] lb = _length.ToBits();

				for (int i = lb.Length - 1; i >= 0; i--)
				{
					if (lb[i] == true)
					{
						lb = lb.Sub(0, i + 1);
						break;
					}
				}

				List<bool> lengthbits = new List<bool>();
				foreach (bool bit in lb)
					lengthbits.Add(bit);
				if (lb.Length % 7 > 0)
				{
					for (int i = 0; i < 7 - lb.Length % 7; i++)
						lengthbits.Add(false);
				}

				List<byte> lengthbytes = new List<byte>();
				for (int i = 0; i < lengthbits.Count / 7; i++)
				{
					List<bool> bits = new List<bool>();
					bits.Add(i < lengthbits.Count / 7 - 1);
					for (int x = 0; x < 7; x++)
						bits.Add(lengthbits[i * 7 + x]);

					lengthbytes.Add(bits.ToArray().ToByte());
				}

				Write(lengthbytes.ToArray());
			}

			public void WriteCustom<T, TP>(T _item) where TP : Hidden.Parser<T>, new()
			{
				TP p = new TP();
				ByteWriter w = new ByteWriter();
				p.WriteBytes(_item, w);
				this.WriteBatch(w.Result);
			}
		}
	}
}