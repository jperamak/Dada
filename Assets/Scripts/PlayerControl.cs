using UnityEngine;
using Dada.InputSystem;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.
	[HideInInspector]
	public AbstractController controller;

	public float moveForce = 365.0f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 3f;				// The fastest the player can travel in the x axis.
	public AudioClip[] jumpClips;			// Array of clips for when the player jumps.
	public float jumpForce = 1000f;			// Amount of force added when the player jumps.
	public AudioClip[] taunts;				// Array of clips for when the player taunts.
	public float tauntProbability = 50f;	// Chance of a taunt happening.
	public float tauntDelay = 1f;			// Delay for when the taunt should happen.


	private int tauntIndex;					// The index of the taunts array indicating the most recent taunt.
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.
    private int walljump = 0;               // whether or not the player can walljump
    private Animator anim;					// Reference to the player's animator component.
    private int points = 0;

    private CameraFollow _camera;


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag != "player")
        {
            Rigidbody2D o = other.gameObject.GetComponent<Rigidbody2D>();

            if (o && o.GetPointVelocity(transform.position).magnitude > 15f && o.mass > 5)
                //Debug.Log(o.GetPointVelocity(transform.position));
                GetComponent<PlayerHealth>().TakeDamage(null);
        }
    }

	void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		anim = GetComponent<Animator>();

        _camera = FindObjectOfType<CameraFollow>();
        _camera.AddPlayer(transform);


		if(controller == null)
			controller = DadaInput.Controller;
	}


	void Update()
	{
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, LayerMask.GetMask(new string[] { "Ground", "Rubble" }));

        var sliding = false;
        if (Physics2D.Linecast(transform.position,transform.position + new Vector3(-0.5f, 0, 0), 1 << LayerMask.NameToLayer("Ground")) || 
            Physics2D.Linecast(transform.position, transform.position + new Vector3(0.5f, 0, 0), 1 << LayerMask.NameToLayer("Ground")) )
        {
            anim.SetBool("Slide", true);
            sliding = true;
        }

		if (controller.GetButtonDown(VirtualKey.JUMP) && sliding )
            walljump = 1;
        else if (controller.GetButtonDown(VirtualKey.JUMP) && sliding)
            walljump = 2;
        else
        {
            walljump = 0;
            if (!sliding)
                anim.SetBool("Slide", false);
        }
		// If the jump button is pressed and the player is grounded then the player should jump.
		if(controller.GetButtonDown(VirtualKey.JUMP) && grounded)
			jump = true;
	}


	void FixedUpdate ()
	{
		// Cache the horizontal input.
		float h = controller.XAxis;

        // quick fix for joystick deadzone
        if (Mathf.Abs(h) < 0.2f)
            h = 0;

		// The Speed animator parameter is set to the absolute value of the horizontal input.
		anim.SetFloat("Speed", grounded ? Mathf.Abs(h) : 0);

		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * rigidbody2D.velocity.x < maxSpeed)
			// ... add a force to the player.
			rigidbody2D.AddForce(Vector2.right * h * moveForce);

		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);

		// If the input is moving the player right and the player is facing left...
		if(h > 0 && !facingRight)
			// ... flip the player.
			Flip();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if(h < 0 && facingRight)
			// ... flip the player.
			Flip();
         
        if (grounded)
            anim.SetBool("Slide", false);

		// If the player should jump...
		if(jump)
		{
			// Set the Jump animator trigger parameter.
            anim.SetBool("Slide", false);
			anim.SetTrigger("Jump");

			// Play a random jump audio clip.
			int i = Random.Range(0, jumpClips.Length);
			AudioSource.PlayClipAtPoint(jumpClips[i], transform.  position);

			// Add a vertical force to the player.
			rigidbody2D.AddForce(new Vector2(0f, jumpForce));

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
		}
        if (walljump > 0 && !grounded)
        {
            anim.SetBool("Slide", false);
            anim.SetTrigger("Jump");
            // Play a random jump audio clip.
            int i = Random.Range(0, jumpClips.Length);
            AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

            // Add a vertical force to the player.
            if (walljump == 1)
            {
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.AddForce(new Vector2(jumpForce, jumpForce));
            }
            else
            {
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.AddForce(new Vector2(-jumpForce, jumpForce));
            }
        }
	}

    public void GivePoints()
    {
        points++;
        GameObject scores = GameObject.Find("Scores" + controller.Number);
        scores.GetComponent<GUIText>().text = ""+points;
    }
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    public void Die()
    {
        _camera.RemovePlayer(transform);
        Destroy(this.gameObject);
    }

	public IEnumerator Taunt()
	{
		// Check the random chance of taunting.
		float tauntChance = Random.Range(0f, 100f);
		if(tauntChance > tauntProbability)
		{
			// Wait for tauntDelay number of seconds.
			yield return new WaitForSeconds(tauntDelay);

			// If there is no clip currently playing.
			if(!audio.isPlaying)
			{
				// Choose a random, but different taunt.
				tauntIndex = TauntRandom();

				// Play the new taunt.
				audio.clip = taunts[tauntIndex];
				audio.Play();
			}
		}
	}

	int TauntRandom()
	{
		// Choose a random index of the taunts array.
		int i = Random.Range(0, taunts.Length);

		// If it's the same as the previous taunt...
		if(i == tauntIndex)
			// ... try another random taunt.
			return TauntRandom();
		else
			// Otherwise return this index.
			return i;
	}
}
