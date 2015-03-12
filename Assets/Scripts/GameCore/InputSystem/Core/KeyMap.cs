using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using SimpleJSON;

namespace Dada.InputSystem{
public class KeyMap {

		private Dictionary<VirtualKey,List<KeyProperty>> _map;

		public KeyMap() : this(null){}
		private KeyMap(Dictionary<VirtualKey,List<KeyProperty>> map){
			if(map == null)
				_map = new Dictionary<VirtualKey, List<KeyProperty>>();
			else
				_map = map;
		}

		public List<KeyProperty> Get(VirtualKey key){
			if(_map.ContainsKey(key))
				return _map[key]; 
			return null;
		}

		public void SetKeyBind(VirtualKey key, KeyProperty val){
			if(_map.ContainsKey(key))
				ReplaceKeyBind(key,val);
			else
				AddKeyBind(key, val);
		}

		public void AddKeyBind(VirtualKey key, KeyProperty val){
			if(!_map.ContainsKey(key)){
				List<KeyProperty> newList = new List<KeyProperty>();
				_map[key] =  newList;

			}
			_map[key].Add(val);
		}

		public void ReplaceKeyBind(VirtualKey key, KeyProperty val){
			if(_map.ContainsKey(key)){
				_map[key].Clear();
				_map[key].Add(val);
			}
		}

		public void ClearKeyBind(VirtualKey key){
			if(_map.ContainsKey(key)){
				_map[key].Clear();
			}
		}


		public void Remap(KeyMap newMap){
			_map = DeepCopyDictionary(newMap._map);
		}

		public void Merge(KeyMap newMap){

			foreach(VirtualKey key in newMap._map.Keys){
				if(!_map.ContainsKey(key))
					_map.Add(key,new List<KeyProperty>());

				_map[key].AddRange(newMap._map[key]);
			}

		}

		public static Dictionary<string, KeyMap> JsonToKeyConfiguration(string jsonString){
			
			JSONNode configs = JSON.Parse(jsonString);
			Dictionary<string, KeyMap> mapping = new Dictionary<string, KeyMap>();
			
			
			foreach( string configName in configs.AsObject.Keys){

				KeyMap deviceMap = JsonToKeyMap(configs[configName].ToString());
				mapping.Add(configName,deviceMap);
			//	Debug.Log("Created keymap with "+deviceMap._map.Count +" elements");
			}

			return mapping;
		}

		public static KeyMap JsonToKeyMap(string jsonString){
			//Debug.Log(jsonString);
			JSONNode config = JSON.Parse(jsonString);
			
			KeyMap deviceMap = new KeyMap();
			
			//iterate through all virtualkeys
			foreach(string virtualKeyName in config.AsObject.Keys){
				IEnumerable<JSONNode> hardKeys = config[virtualKeyName].Children;

				VirtualKey vKey = VirtualKey.NONE;
				try {
					vKey = (VirtualKey) Enum.Parse(typeof(VirtualKey), virtualKeyName);  
				}
				catch (ArgumentException) {return null;}
				
				//iterate all hardkeys assigned to a virtual key
				foreach(JSONNode hardKey in hardKeys){
					
					string keyName = hardKey["KeyName"].Value;
					bool isAxis = hardKey["IsAxis"] != null ? hardKey["IsAxis"].AsBool : false;
					bool invert = hardKey["Inverted"] != null ? hardKey["Inverted"].AsBool : false;
					string condValue = hardKey["KeyTriggerCondition"];
					string orientValue = hardKey["AxisOrientation"];
					
					KeyTriggerCondition condition = KeyTriggerCondition.NON_ZERO;
					if(condValue != null){
						try {
							condition = (KeyTriggerCondition) Enum.Parse(typeof(KeyTriggerCondition), condValue);
						}
						catch (ArgumentException) {}
					}

					AxisOrientation orientation = AxisOrientation.NONE;
					if(orientValue != null){
						try {
							orientation = (AxisOrientation) Enum.Parse(typeof(AxisOrientation), orientValue);
						}
						catch (ArgumentException) {}
					}
					
					KeyProperty property = new KeyProperty(keyName,isAxis, invert, condition, orientation);
					deviceMap.AddKeyBind(vKey,property);
				}
			}
			return deviceMap;
		}

		private static List<T> DeepCopyList<T>(List<T> listToClone) where T: ICloneable{
			return listToClone.Select(item => (T)item.Clone()).ToList();
		}

		private static Dictionary<VirtualKey, List<KeyProperty>> DeepCopyDictionary(Dictionary<VirtualKey, List<KeyProperty>> original){
			Dictionary<VirtualKey, List<KeyProperty>> ret = new Dictionary<VirtualKey, List<KeyProperty>>();
			foreach (KeyValuePair<VirtualKey, List<KeyProperty>> entry in original)
				ret.Add(entry.Key, DeepCopyList<KeyProperty>(entry.Value));
			
			return ret;
		}

}
}