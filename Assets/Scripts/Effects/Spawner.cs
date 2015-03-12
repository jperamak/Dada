using UnityEngine;
using System.Collections;

public class Spawner : Effect {

	public bool Loop = false;
	public int LoopNumber = 0;
	public float TimeToLive = 0;
	public float LoopIntervalTime = 0;
	public float SpawnDelay = 0;
	public float ItemTimeToLive = 0;
	public GameObject[] Items;

	private int _loopCount = 0;
	private float _expirationTime = -1;

	public override void Trigger (){Execute ();}

	protected override void Execute (){
		if(TimeToLive > 0)
			_expirationTime = Time.time + SpawnDelay + TimeToLive;

		if(Loop == false){
			Invoke("Spawn",SpawnDelay);
			Terminate(SpawnDelay+0.1f);
		}
		else
			StartCoroutine("SpawnLoop");
	}

	protected IEnumerable SpawnLoop(){

		//kill the coroutine: effect is expired
		if(_expirationTime == -1 || Time.time < _expirationTime){
			Terminate();
			yield break;
		}

		Spawn();
		if(LoopNumber != 0){
			_loopCount++;

			//kill coroutine: reached maximum number of loops
			if(_loopCount == LoopNumber){
				Terminate();
				yield break;
			}
		}
		yield return new WaitForSeconds(LoopIntervalTime);
	}

	protected void Spawn(){
		for(int i=0;i<Items.Length;i++){
			GameObject obj = Instantiate(Items[i], transform.position, transform.rotation) as GameObject;
			if(ItemTimeToLive > 0)
				Destroy(obj,ItemTimeToLive);
		}
	}

}
