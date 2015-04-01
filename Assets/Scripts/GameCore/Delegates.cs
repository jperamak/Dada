using UnityEngine;
using System.Collections;

public delegate void Callback();
public delegate void PlayerReady(Player player);
public delegate void KilledCallback(GameObject victim, GameObject killer = null);

public class Pair<T,U>{
	public T First{get; set;}
	public U Second{get; set;}

	public Pair(){
		First = default(T);
		Second = default(U);
	}
	public Pair(T first, U second){
		First = first;
		Second = second;
	}

}