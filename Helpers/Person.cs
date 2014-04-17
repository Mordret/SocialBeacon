using System;

namespace AbstractClasses
{
	[Serializable]
	public class Person : IEquatable<Person>
	{
		string ip, firstName, lastName, country, city, phone, email, about;
		int age;

		public string UID
		{
			get
			{
				return
				(
					firstName.ToUpper()
					+ lastName.ToUpper()
					+ ((age > 9) ? age.ToString() : ("0" + age.ToString()))
					+ country.Substring(0,3)
				)
				.ToUpper();
			}
		}
		public string IP { get { return ip; } }
		public string FirstName { get { return firstName; } set { firstName = value; } }
		public string LastName { get { return lastName; } set { lastName = value; } }
		public int Age { get { return age; } set { age = value; } }
		public string Country {get { return country; }  set { country = value; } }
		public string City { get { return city; } set { city = value; } }
		public string Phone
		{
			get { return phone; }
			set
			{
                int test;
				if(int.TryParse(value,out test)) phone = value;
				else throw new Exception("Phone value is not valid!");
			}
		}
		public string Email { get { return email; } set { email = value; } }
		public string About { get { return about; } set { about = value; } }

		public Person (string ip, string firstName, string lastName, int age, string country, string city, string phone, string email, string about)
		{
			this.ip = ip;
			this.firstName = firstName;
			this.lastName = lastName;
			this.age = age;
			this.country = country;
			this.city = city;
			this.phone = phone;
			this.email = email;
			this.about = about;
		}

		public bool Equals(Person otherUser)
		{
			if(otherUser == null) return false;
			if(this.IP == otherUser.IP && this.UID == otherUser.UID && this.FirstName == otherUser.FirstName && this.LastName == otherUser.LastName)
				return true;
			return false;
		}
	}
}

