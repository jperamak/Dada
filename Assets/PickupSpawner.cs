using UnityEngine;
using System.Collections;

public class PickupSpawner : MonoBehaviour {

    public GameObject[] WeaponPickupPrefabs;
    public float SpawnTime;

    private float timer;
	// Use this for initialization
	void Start () {
        timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer >= SpawnTime)
        {
            Instantiate(WeaponPickupPrefabs[Random.RandomRange(0, WeaponPickupPrefabs.Length)], transform.position, Quaternion.identity);
            timer = 0;
        }
    }
}
