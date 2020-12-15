using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DStults.Json
{
	internal class Player
	{
		public string Name { get; private set; }
		public int HP { get; private set; }
		public int MaxHP { get; private set; }
		public Weapon Weapon { get; private set; }
		public Armor Armor { get; private set; }

		// Required later by polymorphic tests:
		protected Player() {}

		public Player(string name, int maxHp, Weapon weapon = null, Armor armor = null)
		{
			this.Name = name;
			this.MaxHP = maxHp;
			this.HP = this.MaxHP;
			this.Weapon = weapon;
			this.Armor = armor;
		}

		public override string ToString() => $"[BASE] {Name} {HP}/{MaxHP}";

	}

	internal class Item
	{
		public string Name { get; private set; }

		public Item(string name)
		{
			this.Name = name;
		}

		public override string ToString() => Name;
		
	}

	internal class Weapon : Item
	{
		public int Damage { get; private set; }

		public Weapon(string name, int damage) : base(name)
		{
			this.Damage = damage;
		}

		public override string ToString() => $"{Name}({Damage})";

	}

	internal class Armor : Item
	{
		public int Defense { get; private set; }

		public Armor(string name, int defense) : base(name)
		{
			this.Defense = defense;
		}

		public override string ToString() => $"{Name}({Defense})";

	}

	internal class PersonV1
	{
		public string Name { get; private set; }
	}

	internal class PersonV2a
	{
		public string Name { get; private set; }
		public int Age { get; private set; }

		public PersonV2a(string name)
		{
			this.Name = name;
		}

		public PersonV2a(int age)
		{
			this.Age = age;
		}

		public PersonV2a(string name, int age)
		{
			this.Name = name;
		}

		public override string ToString() => $"{Name}({Age})";
	}

	internal class PersonV2
	{
		public string Name { get; private set; }
		public int Age { get; private set; }

		public PersonV2(string name)
		{
			this.Name = name;
		}

		public override string ToString() => $"{Name}({Age})";

	}

	internal class PersonV3 : PersonV2
	{
		public PersonV3 Parent { get; private set; }

		public PersonV3(string name, PersonV3 parent = null) : base(name)
		{
			this.Parent = parent;
		}

		public string GetParent()
		{
			if (Parent == null)
				return "";
			return $" p:{Parent.Name}";
		}

		public override int GetHashCode()
		{
			if (this.Parent != null)
				return Parent.GetHashCode() ^ Name.GetHashCode() ^ Age;
			return Name.GetHashCode() ^ Age;
		}

		public override bool Equals(object obj)
		{
			 if ((obj == null) || ! this.GetType().Equals(obj.GetType())) return false;
			PersonV3 other = (PersonV3)obj;
			return this.GetHashCode() == other.GetHashCode();
		}

		public override string ToString() => $"{Name}{GetParent()}";

	}

	internal class PersonV4
	{
		public string Name { get; private set; }
		public PersonV4 Parent { get; private set; }
		public PersonV4 Partner { get; private set; }

		public PersonV4(string name, PersonV4 parent = null, PersonV4 partner = null)
		{
			this.Name = name;
			this.Parent = parent;
			this.Partner = partner;
		}

		public void SetParent(PersonV4 parent) => this.Parent = parent;

		public string GetParent()
		{
			if (Parent == null)
				return " (no p)";
			return $" p:{Parent.Name}";
		}

		public void SetPartner(PersonV4 partner) => this.Parent = partner;

		public string GetPartner()
		{
			if (Partner == null)
				return " (no r)";
			return $" r:{Partner.Name}";
		}

		public override string ToString() => $"{Name}{GetParent()}{GetPartner()}";
	}

	internal class PersonV4b
	{
		public string Name { get; private set; }
		public PersonV4b Parent { get; private set; }
		public PersonV4b Partner { get; private set; }

		public void SetParent(PersonV4b parent) => this.Parent = parent;

		public string GetParent()
		{
			if (Parent == null)
				return " (no p)";
			return $" p:{Parent.Name}";
		}

		public void SetPartner(PersonV4b partner) => this.Parent = partner;

		public string GetPartner()
		{
			if (Partner == null)
				return " (no r)";
			return $" r:{Partner.Name}";
		}

		public override string ToString() => $"{Name}{GetParent()}{GetPartner()}";
	}

	internal class PersonV5
	{
		public string Name { get; private set; }
		public int Age { get; private set; }
		public PersonV5 Parent { get; private set; }
		public PersonV5 Partner { get; private set; }
		public List<PersonV5> Children { get; private set; }

		public PersonV5(string name, PersonV5 parent = null, PersonV5 partner = null)
		{
			this.Name = name;
			this.Parent = parent;
			if (this.Parent != null) this.Parent.AddChild(this);
			this.Partner = partner;
			if (this.Partner != null) this.Partner.Partner = this;
		}

		public string GetParent()
		{
			if (Parent == null)
				return "";
			return $" p:{Parent.Name}";
		}

		public void AddChild(PersonV5 child) => this.Children.Add(child);
		public void RemoveChild(PersonV5 child) => this.Children.Remove(child);

		public override string ToString() => $"{Name}{GetParent()}";
	}

	internal class Employee
	{
		public string Name { get; private set; }
		public Employee Boss { get; private set; }
		public List<Employee> Subordinates { get; private set; } = new List<Employee>();

		// NOTE: NO DEFAULT CONSTRUCTOR

		public Employee(string name, Employee boss = null)
		{
			this.Name = name;
			this.Boss = boss;
			if (this.Boss != null) this.Boss.Subordinates.Add(this);
		}

		public void AddSubordinate(Employee employee) => Subordinates.Add(employee);
		public void RemoveSubordinate(Employee employee) => Subordinates.Remove(employee);
		public string GetSubordinates()
		{
			if (Subordinates.Count == 0) return "";
			StringBuilder sb = new StringBuilder(" is in charge of ");
			string comma = string.Empty;
			foreach (Employee e in Subordinates)
			{
				sb.Append(comma).Append(e.Name);
				comma = ", ";
			}
			return sb.ToString();
		}
		public void SetBoss(Employee employee) => Boss = employee;

		public override string ToString() => $"{Name}({Subordinates.Count})";

	}

	internal class Employee2
	{
		public string Name { get; private set; }
		public Employee2 Boss { get; private set; }
		public List<Employee2> Subordinates { get; private set; } = new List<Employee2>();

		// Note: Default constructor with attribute
		[JsonConstructor] Employee2() {}

		public Employee2(string name, Employee2 boss = null)
		{
			this.Name = name;
			this.Boss = boss;
			if (this.Boss != null) this.Boss.Subordinates.Add(this);
		}

		public void AddSubordinate(Employee2 employee) => Subordinates.Add(employee);
		public void RemoveSubordinate(Employee2 employee) => Subordinates.Remove(employee);
		public string GetSubordinates()
		{
			if (Subordinates.Count == 0) return "";
			StringBuilder sb = new StringBuilder(" is in charge of ");
			string comma = string.Empty;
			foreach (Employee2 e in Subordinates)
			{
				sb.Append(comma).Append(e.Name);
				comma = ", ";
			}
			return sb.ToString();
		}
		public void SetBoss(Employee2 employee) => Boss = employee;

		public override string ToString() => $"{Name}({Subordinates.Count})";

	}

	internal class PlayerAdvanced : Player
	{

		public Player Target { get; private set; }

		protected PlayerAdvanced() : base() {}
		
		public PlayerAdvanced(string name, int maxHp, Weapon weapon = null, Armor armor = null) : base(name, maxHp, weapon, armor)
		{}

		public void SetTarget(Player player) => Target = player;
		public void ClearTarget() => Target = null;

		public override string ToString()
		{
			if (Target != null)
				return $"[ADV] {Name} {HP}/{MaxHP} t:{Target.Name}";
			return $"[ADV] {Name} {HP}/{MaxHP} t:(none)";
		}

	}

	internal class DataSet {
		public List<Player> Players { get; private set; } = new List<Player>();
		public List<Location> Locations { get; private set; } = new List<Location>();

		public string DumpInfo()
		{
			StringBuilder sb = new StringBuilder();
			foreach(Player player in Players)
			{
				sb.Append($"    {player}\n");
			}
			foreach(Location location in Locations)
			{
				sb.Append($"    {location}\n");
			}
			return sb.ToString();
		}
		public override string ToString()
		{
			if (Players.Count == 0 && Locations.Count == 0)
				return "Empty";
			else if (Players.Count > 0 && Locations.Count == 0)
				return Players[0].Name + " (" + Players.Count + ")";
			else if (Players.Count == 0 && Locations.Count > 0)
				return Locations[0].Name + " (" + Locations.Count + ")";
			return Players[0].Name + " " + Locations[0].Name;
		}
	}

	internal class Location
	{
		public string Name { get; private set; }
		public string Description { get; private set; }
		public List<Location> Connections { get; private set; } = new List<Location>();
		public List<Player> Players { get; private set; } = new List<Player>();

		Location() {}

		public Location(string name, string description)
		{
			this.Name = name;
			this.Description = description;
		}

		public void AddPlayer(Player player) => Players.Add(player);
		public void RemovePlayer(Player player) => Players.Remove(player);

		public void AddConnection(Location location) => Connections.Add(location);
		public void RemoveConnection(Location location) => Connections.Remove(location);

		public override string ToString()
		{
			int descriptionLength = Description.Length > 10 ? 10 : Description.Length;
			return $"{Name}({Description.Substring(0, descriptionLength)}...)({Connections.Count})";
		}
	}

}
