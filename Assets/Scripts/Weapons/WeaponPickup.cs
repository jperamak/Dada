using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour {

    public GameObject WeaponPrefab;

    void OnCollisionEnter2D(Collision2D c)
    {

        if (c.collider.gameObject.tag == "Player")
        {
            var h = c.collider.gameObject.GetComponent<Hero>();
            Weapon w = (Instantiate(WeaponPrefab) as GameObject).GetComponent<Weapon>();
            w.transform.parent = h.gameObject.transform.FindChild("Hand1");
            if (h.transform.localScale.x < 0)
                w.transform.localScale = new Vector3( -w.transform.localScale.x, w.transform.localScale.y, w.transform.localScale.z);
            w.transform.localPosition = Vector2.zero;
            w.SetOwner(h.gameObject);
            h.GiveWeapon(w);
            Destroy(this.gameObject);
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
