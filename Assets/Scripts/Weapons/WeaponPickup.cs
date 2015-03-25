using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour {

    public GameObject WeaponPrefab;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("waappen");
        var h = other.gameObject.GetComponent<Hero>();
        Weapon w = (Instantiate(WeaponPrefab) as GameObject).GetComponent<Weapon>();
        w.transform.parent = h.gameObject.transform.FindChild("Hand1");
        w.transform.localPosition = Vector2.zero;
        w.SetOwner(h.gameObject);
        h.GiveWeapon(w);
        Destroy(this.gameObject);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
