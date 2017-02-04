using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meka
{
	namespace Encryptions
	{
		public abstract class Encryption
		{
			internal static List<Encryption> _Modes = new List<Encryption>();
			internal static int _GetModeIndex<T>() where T : Encryption { return _Modes.FindIndex(item => item is T); }

			public abstract byte[] CreatePasswordBuffer(string _password);
			public abstract bool CheckPassword(byte[] _bytes, string _password);

			public abstract byte[] Encrypt(byte[] _bytes, string _password);
			public abstract byte[] Decrypt(byte[] _bytes, string _password);
		}
	}
}