# NewtonSoft Json.Net Best Practices in Two Minutes
 A bunch of useful snippets scoured from hours of testing various serialization/deserialization methods and which ones require the least code-maintenance to work.

# Add the package:
`dotnet add package Newtonsoft.Json`
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

#	Explanation:

**ContractResolver = new PrivateSetResolver()**

- Let's you write to { get; private set; } properties.
  
**PreserveReferencesHandling = PreserveReferencesHandling.Objects**

- Preserves reference links and prevents circular references
  
**TypeNameHandling = TypeNameHandling.Objects**

- Automates polymorphic handling -- Auto often misses things, all hasn't worked as well in my tests so far

**ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor**

- Prevents you from needing to add [JsonConstructor] to no-arg constructors

