using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meka
{
	namespace ByteOperators
	{
		public static class ByteFlow
		{
			public static bool[] ToBits(this byte @this)
			{
				List<bool> @out = new List<bool>();
				for (int i = 7; i >= 0; i--)
				{
					@out.Add(@this >= Math.Pow(2, i));
					if (@this >= Math.Pow(2, i))
						@this -= (byte)Math.Pow(2, i);
				}
				@out.Reverse();
				return @out.ToArray();
			}

			public static bool[] ToBits(this int @this, int _bits = 32)
			{
				BitArray b = new BitArray(new int[] { @this });
				bool[] @out = new bool[32];
				b.CopyTo(@out, 0);

				return @out.Sub(32 - _bits);
			}

			public static byte ToByte(this bool[] @this)
			{
				return (byte)@this.ToInt();
			}

			public static int ToInt(this bool[] @this)
			{
				int @out = 0;
				for (int i = 0; i < @this.Length; i++)
					@out += @this[i] ? (int)Math.Pow(2, i) : 0;
				return @out;
			}

			public static T[] Sub<T>(this T[] @this, int _start, int _length)
			{
				List<T> @out = new List<T>();
				for (int i = _start; i < _start + _length; i++)
					@out.Add(@this[i]);
				return @out.ToArray();
			}

			public static T[] Sub<T>(this T[] @this, int _start)
			{
				return @this.Sub(_start, @this.Length - _start);
			}

			public static string ToZero(this byte @this)
			{
				return new string(@this.ToBits().Select(item => item ? '1' : '0').ToArray());
			}

			public static string ToZeroes(this byte[] @this)
			{
				return string.Join(" ", @this.Select(item => item.ToZero()).ToArray());
			}

			public static string ToZeroes(this bool[] @this)
			{
				return string.Join("", @this.Select(item => item ? "1" : "0"));
			}



			public static string ToChars(this byte[] @this) { return new string(@this.Select(item => (char)item).ToArray()); }

			public static byte[] ToBytes(this string @this) { return @this.Select(item => (byte)item).ToArray(); }

			public static byte[] ToBytes(this bool[] @this)
			{
				List<byte> @out = new List<byte>();
				for (int n = 0; n < @this.Length / 8; n++)
				{
					byte b = 0;
					for (int i = 0; i < 8; i++)
					{
						if (@this[n * 8 + i])
							b += (byte)Math.Pow(2, i);
					}
					@out.Add(b);
				}
				return @out.ToArray();
			}
		}
	}
}