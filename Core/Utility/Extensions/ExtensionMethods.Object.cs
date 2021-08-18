using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace AARPG.Core.Utility.Extensions{
	public static partial class ExtensionMethods{
		private static readonly Dictionary<Type, PropertyInfo[]> objectToProperties = new();

		public static void InitializeWithDefaultValueAttributes<T>(T obj) where T : class{
			var type = obj.GetType();
			if(!objectToProperties.ContainsKey(type))
				objectToProperties.Add(type, type.GetProperties());

			var properties = objectToProperties[type];
			for(int i = 0; i < properties.Length; i++){
				var property = properties[i];
				if(property.GetCustomAttribute(typeof(DefaultValueAttribute)) is DefaultValueAttribute def)
					property.SetValue(obj, def.Value);
			}
		}

		public static void InitializeWithDefaultValueAttributes<T>(ref T obj) where T : struct{
			var type = obj.GetType();
			if(!objectToProperties.ContainsKey(type))
				objectToProperties.Add(type, type.GetProperties());

			var properties = objectToProperties[type];
			for(int i = 0; i < properties.Length; i++){
				var property = properties[i];
				if(property.GetCustomAttribute(typeof(DefaultValueAttribute)) is DefaultValueAttribute def)
					property.SetValue(obj, def.Value);
			}
		}
	}
}
