using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DStults.Json
{

	// https://talkdotnet.wordpress.com/2019/03/15/newtonsoft-json-deserializing-objects-that-have-private-setters/
	public class PrivateResolver : DefaultContractResolver
	{
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var prop = base.CreateProperty(member, memberSerialization);
			if (!prop.Writable)
			{
				var property = member as PropertyInfo;
				var hasPrivateSetter = property?.GetSetMethod(true) != null;
				prop.Writable = hasPrivateSetter;
			}
			return prop;
		}
	}

	// https://www.mking.net/blog/working-with-private-setters-in-json-net
	class PrivateSetterContractResolver : DefaultContractResolver
	{
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var jsonProperty = base.CreateProperty(member, memberSerialization);
			if (!jsonProperty.Writable)
			{
				if (member is PropertyInfo propertyInfo)
				{
					jsonProperty.Writable = propertyInfo.GetSetMethod(true) != null;
				}
			}

			return jsonProperty;
		}
	}

	// My personal succinct version, mostly based off the second version
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


}