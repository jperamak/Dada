using UnityEngine;
using UnityEngine.UI;
using Dada.InputSystem;
using System.Collections.Generic;

public enum BootLevel{JOYSTICKS, ALEVEL}
public class TestBoot : MonoBehaviour {
	
	public BootLevel LevelToBoot;
	public GameObject[] Objects;
	private Transform[] Boxes;
	private Transform trans;
	private bool[] confirmedPlayers;
	private float delay, startTime;


	
	void Awake () {
		
		//Executed for all the scenes
		StartForAll();
		
		//executed only for certain scenes
		switch(LevelToBoot){
		case BootLevel.JOYSTICKS:
			StartJoysticksTest();
			break;

		case BootLevel.ALEVEL:
			StartALevel();
			break;
		}
	}

	void Update(){
		switch(LevelToBoot){
		case BootLevel.JOYSTICKS:
			UpdateJoysticksTest();
			break;
		}
	}

	private void StartALevel(){
		for(int i=0;i<DadaInput.ConrtollerCount;i++){
			GameObject hero = Instantiate(Objects[1], Objects[0].transform.position, Quaternion.identity) as GameObject;
			hero.GetComponent<PlayerControl>().controller = DadaInput.GetJoystick(i);
		}
	}

	private void UpdateJoysticksTest(){

		if(startTime > 0 && startTime+delay < Time.time){

			AbstractController c = DadaInput.DetectKeypress(VirtualKey.START);

			if(c != null && c.Number < confirmedPlayers.Length){
				if(!confirmedPlayers[c.Number]){
					confirmedPlayers[c.Number] = true;
					GameObject player = Instantiate(Objects[0], Boxes[c.Number].position,Quaternion.identity) as GameObject;
					player.GetComponent<PlayerControl>().controller = c;
					trans.Find("PlayersTest").GetChild(c.Number).GetComponentInChildren<Text>().text = "Player "+(c.Number+1)+": "+c.Name;
				}
			}
		}
	}

	public void SetPlayerNum(int num){
		confirmedPlayers = new bool[num];
		trans.Find("PlayerNumSelection").gameObject.SetActive(false);
		for(int i=0;i<num; i++)
			Boxes[i].gameObject.SetActive(true);

		Transform t = trans.GetChild(0);
		for(int i=num;i<t.childCount; i++)
			t.GetChild(i).gameObject.SetActive(false);

		startTime = Time.time;
		delay = 0.2f;
	}

	private void StartForAll(){
		TextAsset txt = (TextAsset)Resources.Load("keymap", typeof(TextAsset));
		string json = txt.text;
		
		//string json = SimpleFS.ReadFile("Scripts/Common/InputSystem/Res/keymap.json");
		Dictionary<string,KeyMap> keyMapConfig = KeyMap.JsonToKeyConfiguration(json);
		
		#if UNITY_EDITOR
		DadaInput.Initialize(keyMapConfig);
		#else
		DadaInputInput.Initialize(keyMapConfig,InputMethod.JOYSTICK);
		#endif
	}
	
	private void StartJoysticksTest(){
		trans = transform.Find("/Canvas");
		Transform parentBox = transform.Find("/Boxes");
		Boxes = new Transform[4];
		for(int i=0;i<parentBox.childCount; i++){
			Boxes[i] = parentBox.GetChild(i);
			Boxes[i].gameObject.SetActive(false);
		}

		Button[] buttons = trans.GetComponentsInChildren<Button>();
		buttons[0].Select();
	}

}
