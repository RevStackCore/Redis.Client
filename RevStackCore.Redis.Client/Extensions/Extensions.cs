using System;
namespace RevStackCore.Redis.Client
{
	public static partial class Extensions
	{
		public static string ToTypedUrnKey(this string key, string type)
		{
			return "urn:" + type.ToLower() + ":" + key;
		}

	}
}
