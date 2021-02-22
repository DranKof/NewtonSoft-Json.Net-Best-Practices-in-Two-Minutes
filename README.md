# NewtonSoft Json.Net Best Practices in Two Minutes
 A bunch of useful snippets scoured from hours of testing various serialization/deserialization methods and which ones require the least code-maintenance to work.

# Add the package:
```
dotnet add package Newtonsoft.Json
```
- This lets you use the library.

# Create a private setter contract resolver class

```cs
class PrivateSetResolver : DefaultContractResolver
{
	protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
	{
		JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);
		if (!jsonProperty.Writable && member is PropertyInfo propertyInfo)
			jsonProperty.Writable = propertyInfo.GetSetMethod(true) != null;
		return jsonProperty;
	}
}
```
- This will be used below. It's the key ingredient allowing the continued use of { get; private set; } automatic properties best practice. Otherwise you have to start to make everything public, violating principles of encapsulation.

# Use these settings for serialization and deserialization:

```cs
JsonSerializerSettings settings = new JsonSerializerSettings
{
	ContractResolver = new PrivateSetResolver(),
	PreserveReferencesHandling = PreserveReferencesHandling.Objects,
	TypeNameHandling = TypeNameHandling.Objects,
	ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
};
```
- **ContractResolver = new PrivateSetResolver()**
    - This utilizes what we made above.
    - It lets you write to { get; private set; } properties.
- **PreserveReferencesHandling = PreserveReferencesHandling.Objects**
    - Preserves reference links and prevents circular references
- **TypeNameHandling = TypeNameHandling.Objects**
    - Automates polymorphic handling -- Auto often misses things, all hasn't worked as well in my tests so far
- **ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor**
    - Prevents you from needing to add [JsonConstructor] to no-arg constructors...but you still must add no-arg constructors.

# Add private no-arg constructors

In order for Json.Net to be able to create objects that cross-reference in circular references, they must have access to a no-arg constructor:
```cs
// For standalone classes...
internal class Person
{
	public string Name { get; private set; }
	
	// Add this in (omitted accessibility key word means it's 'private'):
	Person() {}
	
	public Person(string name)
	{
		this.Name = name;
	}
}
```

For classes with inheritance, the process is required for every layer (*sigh*):
```cs
internal class Person
{
	public string Name { get; private set; }
	
	// Add this in, and make sure it's 'protected':
	protected Person() {}
	
	public Person(string name)
	{
		this.Name = name;
	}
}

internal class Employee : Person
{
	public int BadgeNumber { get; private set; }
	
	// Add this in, protected or private (omitting works, too, as long as this is the final derivative class):
	protected Employee() : base() {}
	
	public Person(string name, int badgeNumber) : base(name)
	{
		this.BadgeNumber = badgeNumber;
	}
}
```

# Create an Overarching Data Class

Create a class that contains all your data. You could use a list, but that only works well when you're dealing with one data type. The easiest way is just one class with lists for each of the various data types you're using, something like this:
```cs
internal class DataSet {
	public List<Person> People { get; private set; } = new List<Person>();
	public List<Location> Locations { get; private set; } = new List<Location>();
}
```
- Once you have your data class ready, then you will be ready to serialize and deserialize it in a single line of code!

# Serialize

Use the "settings" class above, and deploy it in the serialization command like so:
```cs
string json = JsonConvert.SerializeObject(myData1, Formatting.Indented, settings);
```

# Deserialize

Whether you stream it in via a file or over the network, bring it right back into a data mega-object like so:
```cs
DataSet myData2 = JsonConvert.DeserializeObject<DataSet>(json, settings);
```

# And that's it.

If you find or know of a better way, please let me know!
