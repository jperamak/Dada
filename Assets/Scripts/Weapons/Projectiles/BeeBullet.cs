using UnityEngine;
using System.Collections;

public class BeeBullet : Projectile {

    public float sleepTime;
    private int _awake = 0;
    private float _drag;
    public float dragMod = 2;
    public float range, chaseFollow, chaseSpeed;
    private Transform _target;
    public float lifeTime = 5f;
    private bool _fading = false;

    public SoundEffect flySound, aggroSound;

    private float startTime;
    private SpriteRenderer spriteRend;

    protected GameObject _targetHit;

    private Rigidbody2D _rigidbody;
	// Use this for initialization
	void Start () {

        flySound = DadaAudio.GetSoundEffect(flySound);
        aggroSound = DadaAudio.GetSoundEffect(aggroSound);


        startTime = Time.time;
        sleepTime += Time.time;
        spriteRend = GetComponent<SpriteRenderer>();

        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            _targetHit = coll.gameObject;
            TriggerEffects();
        }
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
            if (flySound != null)
                flySound.PlayEffect();
            if (aggroSound != null)
                aggroSound.Stop();

            _rigidbody.velocity = Quaternion.EulerAngles(0, 0, Random.Range(-45f, 45f)) * _rigidbody.velocity.normalized * 4f;
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
            if (flySound != null)
                flySound.Stop();
            if (aggroSound != null)
                aggroSound.PlayEffect();

            if (_target != null)
            {
                Vector3 dir = Vector3.Lerp(_rigidbody.velocity, _target.position - transform.position, chaseFollow * Time.deltaTime);
                _rigidbody.velocity = dir.normalized * chaseSpeed;
                float angle = Mathf.Atan2(_rigidbody.velocity.y, _rigidbody.velocity.x);
                transform.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
            }
            else
                _awake--;
        }

        if (Time.time >= startTime + lifeTime)
        {
            if (!_fading)
            { 
                Destroy(gameObject, 2);
                _fading = true;
            }
            float timeIntoFading = Time.time - startTime - lifeTime;
            

            if (timeIntoFading > 0.0f && lifeTime > 0.0f)
                SetAlpha(1.0f - timeIntoFading / 2f);
        }
	}


    void SetAlpha(float value) // from 0 .. 1
    {
        if (spriteRend == null)
            return;
        Color color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, value);
        spriteRend.material.color = color;
    }

    public virtual void TriggerEffects()
    {

        if (_effects != null)
        {
            for (int i = 0; i < _effects.Length; i++)
            {
                _effects[i].Owner = Owner;
                _effects[i].OnEnd += OnEffectFinshed;
                _effects[i].Trigger(_targetHit);
            }
        }
    }
}
