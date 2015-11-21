using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour {

    public GameObject WeaponPrefab;
	public SoundEffect PickupSound;

    void OnTriggerEnter2D(Collider2D c)
    {

        if (c.gameObject.tag == "Player")
        {
            var h = c.gameObject.GetComponent<Hero>();
            Weapon w;
            if (WeaponPrefab != null)
            {
                w = (Instantiate(WeaponPrefab) as GameObject).GetComponent<Weapon>();
                w.transform.parent = h.gameObject.transform.FindChild("Hand1");
                if (h.transform.localScale.x < 0)
                    w.transform.localScale = new Vector3(-w.transform.localScale.x, w.transform.localScale.y, w.transform.localScale.z);
                w.transform.localPosition = Vector2.zero;
                w.SetOwner(h.gameObject);
                h.GiveWeapon(w);
            }
            else
                (h.RangedWeapon as RangedWeapon).Reset();
			PickupSound.PlayEffect();
            Destroy(this.gameObject);
        }
    }

	// Use this for initialization
	void Start () {
		PickupSound = DadaAudio.GetSoundEffect(PickupSound);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
