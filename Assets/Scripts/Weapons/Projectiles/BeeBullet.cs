using UnityEngine;
using System.Collections;

public class BeeBullet : Projectile {

    public float sleepTime;
    private int _awake = 0;
    private float _drag;
    public float dragMod = 2;
    public float range, chaseFollow, chaseSpeed;
    private Transform _target;

    private Rigidbody2D _rigidbody;
	// Use this for initialization
	void Start () {
        sleepTime += Time.time;
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            TriggerEffects();
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log("moo");
        TriggerEffects();
    }

	// Update is called once per frame
	void Update () {
        if (_awake == 0 && Time.time > sleepTime)
        {
            _drag = _rigidbody.drag + dragMod;
            _rigidbody.drag = _drag;

            if (_drag > 20)
            {
                _rigidbody.drag = 0;
                _awake++;
            }
        }
        if (_awake == 1)
        {
            _rigidbody.velocity = Quaternion.EulerAngles(0, 0, Random.Range(-45f, 45f)) * _rigidbody.velocity;
            Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, range);
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].gameObject.tag == "Player")
                {
                    _target = objects[i].gameObject.transform;
                    _awake++;
                }
            }
        }
        if (_awake == 2)
        {
            Vector3 dir = Vector3.Lerp(_rigidbody.velocity, _target.position - transform.position, chaseFollow * Time.deltaTime);
            _rigidbody.velocity = dir.normalized * chaseSpeed;
        }
	}
}
