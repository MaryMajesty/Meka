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
		public class MekaItemParser : Parser<MekaItem>
		{
			internal override MekaItem ReadBytes(ByteReader _reader)
			{
				MekaItem @out = new MekaItem();

				bool[] info = _reader.Read().ToBits();
				bool hasname = info[0];
				bool hascontent = info[1];
				bool hasdata = info[2];
				bool haschildren = info[3];

				@out.Horizontal = info[4];
				@out.Collapsed = info[5];

				if (hasname)
					@out.Name = _reader.ReadBatch().ToChars();
				if (hascontent)
					@out.Content = _reader.ReadBatch().ToChars();
				if (hasdata)
					@out.Data = _reader.ReadBatch();

				if (haschildren)
				{
					@out.Children = new List<MekaItem>();

					byte[] bs = _reader.ReadBatch();
					ByteReader cr = new ByteReader(bs);
					while (cr.CanRead)
						@out.Children.Add(cr.ReadCustom<MekaItem, Hidden.MekaItemParser>());
				}

				return @out;
			}

			internal override void WriteBytes(MekaItem _t, ByteWriter _writer)
			{
				bool[] info = new bool[] { _t.HasName, _t.HasContent, _t.HasData, _t.HasChildren, _t.Horizontal, _t.Collapsed, false, false };
				_writer.Write(info.ToByte());

				if (_t.HasName)
					_writer.WriteBatch(_t.Name.ToBytes());
				if (_t.HasContent)
					_writer.WriteBatch(_t.Content.ToBytes());
				if (_t.HasData)
					_writer.WriteBatch(_t.Data);

				if (_t.HasChildren)
				{
					ByteWriter cw = new ByteWriter();
					foreach (MekaItem child in _t.Children)
						cw.WriteCustom<MekaItem, Hidden.MekaItemParser>(child);
					_writer.WriteBatch(cw.Result);
				}
			}
		}
	}

	public class MekaItem : Hidden.Bytable<MekaItem, Hidden.MekaItemParser>
	{
		public string Name;
		public string Content;
		public byte[] Data;
		public List<MekaItem> Children;

		public bool Horizontal;
		public bool Collapsed;

		public MekaItem this[string _name]
		{
			get { return this.Children.First(item => item.Name == _name); }
		}
		public MekaItem this[int _index]
		{
			get { return this.Children[0]; }
		}

		#region Properties

		public bool HasName
		{
			get { return this.Name != null; }
		}
		public bool HasContent
		{
			get { return this.Content != null; }
		}
		public bool HasData
		{
			get { return this.Data != null; }
		}
		public bool HasChildren
		{
			get { return this.Children != null; }
		}
		public bool IsFile
		{
			get { return this.HasName && this.HasData; }
		}
		public string FileFormat
		{
			get { return this.Name.Substring(this.Name.LastIndexOf('.') + 1); }
		}

		#endregion

		public MekaItem() { }

		public MekaItem(string _name) { this.Name = _name; }

		public MekaItem(string _name, string _content)
		{
			this.Name = _name;
			this.Content = _content;
		}

		public MekaItem(byte[] _data) { this.Data = _data; }

		public MekaItem(string _name, byte[] _data)
		{
			this.Name = _name;
			this.Data = _data;
		}

		public MekaItem(string _name, List<MekaItem> _children)
		{
			this.Name = _name;
			this.Children = _children;
		}

		public MekaItem(string _name, params MekaItem[] _children) : this(_name, _children.ToList()) { }

		public MekaItem(string _name, string _fileformat, byte[] _data)
		{
			this.Name = _name;
			this.Content = _fileformat;
			this.Data = _data;
		}

		public bool Contains(string _name) { return this.Children != null && this.Children.Any(item => item.Name == _name); }

		public T To<T>()
		{
			Type t = typeof(T);
			if (t.IsEnum)
				return (T)Enum.Parse(t, this.Content);
			else
				return (T)t.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { this.Content });
		}
	}
}