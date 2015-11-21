using UnityEngine;
using Dada.InputSystem;
using System.Collections.Generic;

public enum VirtualKey{ SHOOT, MELEE, JUMP, SPECIAL, MENU, START, BACK, PAUSE, SUBMIT, CANCEL, X_AXIS, Y_AXIS, MOVE_AXIS, AIM_AXIS, UP, DOWN, LEFT, RIGHT, SELECT, NONE};
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
	public static int ControllerCount{get{return Instance._joyList.Count; }}
			
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
			TextAsset txt = (TextAsset)Resources.Load("_Core/keymap", typeof(TextAsset));
			string json = txt.text;
			
			Dictionary<string,KeyMap> keyMapConfig = KeyMap.JsonToKeyConfiguration(json);
			

			DadaInput.Initialize(keyMapConfig);

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
		List<string> fixedNames = new List<string>();

		//In some cases Unity detects a non-existing joystick with empty name.
		//This code fixes that eventuality 
		foreach(string s in names){
			if(s.Length != 0){
				fixedNames.Add(s);
			}
		}

		names = fixedNames.ToArray();
		for (int i=0; i< names.Length; i++)
			Debug.Log( names[i] );

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
	
		List<ConsoleController> controllers = new List<ConsoleController>();
		for(int i=0;i<_controllerNames.Length;i++){
			kMap = MakeMap(_controllerNames[i]);
			ConsoleController c = new ConsoleController(kMap,_controllerNames[i],i);
			controllers.Add(c);
			_joyList.Add(c);
		}
		if(controllers.Count > 0)
			GamepadSync.Initialize(controllers);
#if UNITY_EDITOR
		_joyList.Add(new KeyboardController(_rawKeyMaps["Keyboard"],_joyList.Count));
#endif
		if(_joyList.Count > 0)
			_joy = new CompositeController(_joyList);
		else
			_joy = new NullController(0);

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