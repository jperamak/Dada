using UnityEngine;
using Dada.InputSystem;
using System.Collections.Generic;

public enum VirtualKey{ SHOOT, MELEE, JUMP, SPECIAL, MENU, START, BACK, PAUSE, SUBMIT, CANCEL, X_AXIS, Y_AXIS, MOVE_AXIS, AIM_AXIS, UP, DOWN, LEFT, RIGHT, NONE};
public enum InputMethod{ AUTO, JOYSTICK, KEYBOARD, COMPOSITE };

public class DadaInput {

	private static DadaInput _inst;
	private static DadaInput Instance{
		get{
			if(_inst == null)
				AutoInit();
			return _inst;
		} 
	}

	private AbstractController _joy;
	private List<AbstractController> _joyList;
	private string[] _controllerNames;
	private Dictionary<string,KeyMap> _rawKeyMaps;
	private KeyMap _customKeyMap;

	public static float XAxis{get{ return Instance._joy.XAxis; }}
	public static float YAxis{get{ return Instance._joy.YAxis; }}
	public static bool AnyKey{get{ return Instance._joy.AnyKey; }}

	public static bool Inverted{get{return Instance._joy.Inverted; } set{ Instance._joy.Inverted = value; }}
	public static float Sensibility{get{return Instance._joy.Sensibility; } set{ Instance._joy.Sensibility = Mathf.Clamp(value,0.5f, 2.0f); }}

	public static AbstractController Controller{get{ return Instance._joy; }}
	public static string[] ControllerNames{get{return Instance._controllerNames; }}
	public static int ConrtollerCount{get{return Instance._joyList.Count; }}
			
	public static float GetAxis(VirtualKey key){ return Instance._joy.GetAxis(key); }
	public static bool GetButton(VirtualKey key){ return Instance._joy.GetButton(key); }
	public static bool GetButtonDown(VirtualKey key){ return Instance._joy.GetButtonDown(key); }
	public static bool GetButtonUp(VirtualKey key){ return Instance._joy.GetButtonUp(key); }


	//TODO: now supports only one joystick
	public static AbstractController GetJoystick(int number){
		if(number >= Instance._joyList.Count)
			return null;
		return Instance._joyList[number];
	}


	private DadaInput(){
		_rawKeyMaps = new Dictionary<string, KeyMap>();
		_joyList = new List<AbstractController>();
	}

	public static void AutoInit(){
		if(_inst == null){
			TextAsset txt = (TextAsset)Resources.Load("keymap", typeof(TextAsset));
			string json = txt.text;
			
			Dictionary<string,KeyMap> keyMapConfig = KeyMap.JsonToKeyConfiguration(json);
			
			#if UNITY_EDITOR
			DadaInput.Initialize(keyMapConfig);
			#else
			DadaInputInput.Initialize(keyMapConfig,InputMethod.JOYSTICK);
			#endif
		}
	}

	public static void Initialize(Dictionary<string, KeyMap> map){Initialize(map, null, InputMethod.AUTO);}
	public static void Initialize(Dictionary<string, KeyMap> map, InputMethod method){Initialize(map, null, method);}
	public static void Initialize(Dictionary<string, KeyMap> map, KeyMap customMap, InputMethod method){

		if(_inst == null)
			_inst = new DadaInput();

		if(map == null)
			Instance._rawKeyMaps = new Dictionary<string, KeyMap>();
		else
			Instance._rawKeyMaps = map;

		Instance._customKeyMap = customMap;
		CheckJoysticks(method);
	}

	public static void CheckJoysticks(InputMethod forceMethod = InputMethod.AUTO){

		string[] names = UnityEngine.Input.GetJoystickNames();
		if(Instance._controllerNames == null || !ArraysEqual<string>(names,Instance._controllerNames)){
			Instance._controllerNames = names;
			Instance.Configure(forceMethod);
		}
		else if(forceMethod != InputMethod.AUTO)
			Instance.Configure(forceMethod);
	}

	public static void SetCustomKey(VirtualKey oldKey, KeyProperty newVal){
		if(Instance._customKeyMap == null)
			Instance._customKeyMap = new KeyMap();
		Instance._customKeyMap.SetKeyBind(oldKey,newVal);
	}

	public static void SetCustomKey(KeyMap newMap){
		Instance._customKeyMap.Remap(newMap);
	}

	public static AbstractController DetectKeypress(VirtualKey key){
		List<AbstractController> list = Instance._joyList;
		if(list.Count > 0)
			for(int i=0; i<list.Count; i++)
				if(list[i].GetButtonDown(key))
					return list[i];

		if(Instance._joy.GetButtonDown(key))
			return Instance._joy;

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