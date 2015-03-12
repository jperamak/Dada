using UnityEngine;
using System.Collections;

public delegate void Callback();
public delegate void PlayerReady(Player player);
public delegate void KilledCallback(GameObject victim, GameObject killer = null);
