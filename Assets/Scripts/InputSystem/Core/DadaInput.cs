using UnityEngine;
using Dada.InputSystem;
using System.Collections.Generic;

public enum VirtualKey{ SHOOT, MELEE, JUMP, SPECIAL, MENU, START, BACK, PAUSE, SUBMIT, CANCEL, X_AXIS, Y_AXIS, MOVE_AXIS, AIM_AXIS, NONE};
public enum InputMethod{ AUTO, JOYSTICK, KEYBOARD, COMPOSITE };

public class DadaInput {

	private static DadaInput _instance;

	private AbstractController _joy;
	private List<AbstractController> _joyList;
	private string[] _controllerNames;
	private Dictionary<string,KeyMap> _rawKeyMaps;
	private KeyMap _customKeyMap;

	public static float XAxis{get{ return _instance._joy.XAxis; }}
	public static float YAxis{get{ return _instance._joy.YAxis; }}
	public static bool AnyKey{get{ return _instance._joy.AnyKey; }}

	public static bool Inverted{get{return _instance._joy.Inverted; } set{ _instance._joy.Inverted = value; }}
	public static float Sensibility{get{return _instance._joy.Sensibility; } set{ _instance._joy.Sensibility = Mathf.Clamp(value,0.5f, 2.0f); }}

	public static AbstractController Controller{get{ return _instance._joy; }}
	public static string[] ControllerNames{get{return _instance._controllerNames; }}
	public static int ConrtollerCount{get{return _instance._joyList.Count; }}
			
	public static float GetAxis(VirtualKey key){ return _instance._joy.GetAxis(key); }
	public static bool GetButton(VirtualKey key){ return _instance._joy.GetButton(key); }
	public static bool GetButtonDown(VirtualKey key){ return _instance._joy.GetButtonDown(key); }
	public static bool GetButtonUp(VirtualKey key){ return _instance._joy.GetButtonUp(key); }


	//TODO: now supports only one joystick
	public static AbstractController GetJoystick(int number){
		if(number >= _instance._joyList.Count)
			return null;
		return _instance._joyList[number];
	}


	private DadaInput(){
		_rawKeyMaps = new Dictionary<string, KeyMap>();
		_joyList = new List<AbstractController>();
	}

	public static void Initialize(Dictionary<string, KeyMap> map){Initialize(map, null, InputMethod.AUTO);}
	public static void Initialize(Dictionary<string, KeyMap> map, InputMethod method){Initialize(map, null, method);}
	public static void Initialize(Dictionary<string, KeyMap> map, KeyMap customMap, InputMethod method){

		if(_instance == null)
			_instance = new DadaInput();

		if(map == null)
			_instance._rawKeyMaps = new Dictionary<string, KeyMap>();
		else
			_instance._rawKeyMaps = map;

		_instance._customKeyMap = customMap;
		CheckJoysticks(method);
	}

	public static void CheckJoysticks(InputMethod forceMethod = InputMethod.AUTO){

		string[] names = UnityEngine.Input.GetJoystickNames();
		if(_instance._controllerNames == null || !ArraysEqual<string>(names,_instance._controllerNames)){
			_instance._controllerNames = names;
			_instance.Configure(forceMethod);
		}
		else if(forceMethod != InputMethod.AUTO)
			_instance.Configure(forceMethod);
	}

	public static void SetCustomKey(VirtualKey oldKey, KeyProperty newVal){
		if(_instance._customKeyMap == null)
			_instance._customKeyMap = new KeyMap();
		_instance._customKeyMap.SetKeyBind(oldKey,newVal);
	}

	public static void SetCustomKey(KeyMap newMap){
		_instance._customKeyMap.Remap(newMap);
	}

	public static AbstractController DetectKeypress(VirtualKey key){
		List<AbstractController> list = _instance._joyList;
		if(list.Count > 0)
			for(int i=0; i<list.Count; i++)
				if(list[i].GetButtonDown(key))
					return list[i];

		if(_instance._joy.GetButtonDown(key))
			return _instance._joy;

		return null;
	}

	private void Configure(InputMethod forceMethod){

		KeyMap kMap;

		switch(forceMethod){

#if UNITY_EDITOR
		case InputMethod.KEYBOARD:
			_joy = new KeyboardController(_rawKeyMaps["Keyboard"],0);
			break;
#endif
		case InputMethod.JOYSTICK:
			if(_controllerNames.Length == 0){
				_joy = new NullController(0);
			}
			else if(_controllerNames.Length == 1){
				kMap = MakeMap(_controllerNames[0]);
				_joy = new ConsoleController(kMap,_controllerNames[0]);
			}
			else{
				for(int i=0;i<_controllerNames.Length;i++){
					kMap = MakeMap(_controllerNames[i]);
					_joyList.Add(new ConsoleController(kMap,_controllerNames[i],i));
					_joy = new CompositeController(_joyList);
				}
			}
			break;
		case InputMethod.COMPOSITE:
		case InputMethod.AUTO:
			for(int i=0;i<_controllerNames.Length;i++){
				kMap = MakeMap(_controllerNames[i]);
				_joyList.Add(new ConsoleController(kMap,_controllerNames[i],i));
			}
#if UNITY_EDITOR
			_joyList.Add(new KeyboardController(_rawKeyMaps["Keyboard"],_joyList.Count));
#endif
			_joy = new CompositeController(_joyList);
			break;
		default:
			_joy = new NullController(0);
			break;
		}
	}
	
	private KeyMap MakeMap(string name){
		if(_customKeyMap != null)
			return _customKeyMap;

		if(_rawKeyMaps.ContainsKey(name))
			return _rawKeyMaps[name];

		return _rawKeyMaps["JoystickDefault"];
	}
	
	private static bool ArraysEqual<T>(T[] a1, T[] a2){
		if (ReferenceEquals(a1,a2))
			return true;
		
		if (a1 == null || a2 == null)
			return false;
		
		if (a1.Length != a2.Length)
			return false;
		
		EqualityComparer<T> comparer = EqualityComparer<T>.Default;
		for (int i = 0; i < a1.Length; i++){
			if (!comparer.Equals(a1[i], a2[i])) return false;
		}
		return true;
	} 
		



}