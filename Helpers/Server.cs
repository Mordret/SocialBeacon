using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractClasses
{
    [Serializable]
	public class Server : IEquatable<Server>
	{
		string ip, name;

		public string IP { get { return ip; } }
		public string Name { get { return name; } }

		public Server(string ip, string name)
		{
			this.ip = ip;
			this.name = name;
		}

		public bool Equals(Server otherServer)
		{
			if(otherServer == null) return false;
			if(this.IP == otherServer.IP && this.Name == otherServer.Name)
				return true;
			return false;
		}
	}
}

