using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meka.ByteOperators;

namespace Meka
{
	namespace Hidden
	{
		public abstract class Bytable<T, TP> where TP : Parser<T>, new()
		{
			public static TP _Parser = new TP();

			public static T FromBytes(byte[] _bytes)
			{
				ByteReader r = new ByteReader(_bytes);
				if (!Compatibility._Old)
					r = new ByteReader(r.ReadBatch());
				return Bytable<T, TP>._Parser.ReadBytes(r);
			}
			public byte[] ToBytes()
			{
				ByteWriter w = new ByteWriter();
				Bytable<T, TP>._Parser.WriteBytes((T)(object)this, w);

				ByteWriter n = new ByteWriter();
				n.WriteBatch(w.Result);
				return n.Result;
			}
			public byte[] ToBytesEncrypted()
			{
				List<byte> @out = new List<byte>() { 0 };
				@out.AddRange(this.ToBytes());
				return @out.ToArray();
			}

			public static T FromBytesEncrypted(byte[] _bytes, string _password = null)
			{
				bool[] header = _bytes[0].ToBits();

				if (header[0])
				{
					Encryptions.Encryption e = Encryptions.Encryption._Modes[header.Sub(1).ToInt()];
					ByteReader r = new ByteReader(_bytes.Sub(1));
					byte[] buffer = r.ReadBatch();

					if (e.CheckPassword(buffer, _password))
						return FromBytes(e.Decrypt(r._Bytes.ToArray(), _password));
					else
						throw new Exception("Wrong password.");
				}
				else
					return FromBytes(_bytes.Sub(1));
			}

			public byte[] ToBytesEncrypted<T>(string _password = null) where T : Encryptions.Encryption, new()
			{
				if (_password == null)
				{
					List<byte> @out = new List<byte>(0);
					@out.AddRange(this.ToBytes());
					return @out.ToArray();
				}
				else
				{
					List<bool> header = new List<bool>() { true };
					header.AddRange(Encryptions.Encryption._GetModeIndex<T>().ToBits(7));

					ByteWriter w = new ByteWriter();
					Encryptions.Encryption e = new T();

					w.Write(header.ToArray().ToByte());
					w.Write(e.CreatePasswordBuffer(_password));

					List<byte> @out = new List<byte>();
					@out.AddRange(w.Result);
					@out.AddRange(e.Encrypt(this.ToBytes(), _password));

					return @out.ToArray();
				}
			}

			public static T LoadFromFile(string _path, string _password = null)
			{
				Compatibility._Old = false;
				byte[] bs = File.ReadAllBytes(_path);
				Bytable<T, TP> @out;

				try
				{
					@out = (Bytable<T, TP>)(object)FromBytesEncrypted(File.ReadAllBytes(_path), _password);

					if (_password == null)
					{
						int l = ((Bytable<T, TP>)(object)@out).ToBytesEncrypted().Length;
						if (l != bs.Length)
						{
							Compatibility._Old = true;
							@out = (Bytable<T, TP>)(object)FromBytesEncrypted(File.ReadAllBytes(_path), _password);
							//if (MessageBox.Show("Overwrite old file?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
							//	@out.SaveToFile(_path);
						}
					}
				}
				catch
				{
					Compatibility._Old = true;
					@out = (Bytable<T, TP>)(object)FromBytesEncrypted(File.ReadAllBytes(_path), _password);
					//if (MessageBox.Show("Overwrite old file?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
					//	@out.SaveToFile(_path);
				}

				return (T)(object)@out;
			}
			public void SaveToFile(string _path) { File.WriteAllBytes(_path, this.ToBytesEncrypted()); }

			public void SaveToFileEncrypted<T>(string _path, string _password) where T : Encryptions.Encryption, new() { File.WriteAllBytes(_path, this.ToBytesEncrypted<T>(_password)); }
		}
	}
}