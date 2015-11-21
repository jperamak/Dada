using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmmoDisplay : MonoBehaviour {

    private Hero _player;
    private int _maxBullets;
    private RangedWeapon _weapon;
    private HeroControllerV2 _heroController;
    private List<GameObject> _bullets;
    public GameObject ammoCountPrefab;


	// Use this for initialization
	void Start () {
        _player = transform.parent.GetComponent<Hero>();
        _heroController = _player.transform.GetComponent<HeroControllerV2>();
        _weapon = _player.RangedWeapon as RangedWeapon;
        _bullets = new List<GameObject>();
        _maxBullets = _weapon.MaxBullets;
        for (int i = 0; i < _maxBullets; i++)
        {
            GameObject g = Instantiate(ammoCountPrefab);
            g.transform.parent = transform;
            _bullets.Add(g);
        }
    }
    bool _flipped = false;
	// Update is called once per frame
	void Update () {
        _maxBullets = _weapon.MaxBullets;
        if (!_flipped && !_heroController.facingRight)
        {
            _flipped = true;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (_flipped && _heroController.facingRight)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            _flipped = false;
        }
        _weapon = _player.RangedWeapon as RangedWeapon;
        int bullets = _weapon.currentBullets;



        while (_bullets.Count < _maxBullets)
        {
            GameObject g = Instantiate(ammoCountPrefab);
            g.transform.parent = transform;
            _bullets.Add(g);
        }

        for (int i = 0; i < _maxBullets; i++)
        {
            float x = (-(_maxBullets - 1) / 2f + i) * ammoCountPrefab.transform.localScale.x *2;
            _bullets[i].transform.position = transform.position + new Vector3(x, 0, 0);
        }


        for (int i = 0; i < _maxBullets; i++)
            _bullets[i].SetActive(true);
        
        for (int i = _maxBullets - 1; i > bullets - 1; i--)
            _bullets[i].SetActive(false);

        
	}
}
