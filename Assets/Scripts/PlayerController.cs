using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float moveSpeed; 		// speed of the player
	private float moveVelocity;
	public float jumpHeight;		// how much the player jumps
	Rigidbody2D player;


	// for checking if we are on ground or not
	public Transform groundCheck;		// an empty child gameobj is given here
	public float groundCheckRadius;
	public LayerMask whatIsGround;
	private bool grounded;
	private bool doubleJump;

	// FIRING PROJECTILES
	public Transform firingPoint;		// gameobj pos
	public GameObject ninjaStar;		// gameobj

	// for player animations
	private Animator anim;

	// for continous shot
	public float shotDelay;
	private float shotDelayCounter;


	// to knock back the player when hitting on enemies or spikes

	public float knockBack;
	public float knockBackLength;
	public float knockBackCount; 
	public bool knockFromRight;


	// LADDER CLIMBING
	public bool onLadder;
	public float climbSpeed;
	private float climbVelocity;
	private float gravityStore;

	// Use this for initialization
	void Start () {
		player = GetComponent<Rigidbody2D> ();		// for getting the attached rigidbody component of the game object
		moveSpeed = 190;
		jumpHeight = 400;

		anim = GetComponent<Animator>();

		knockBackCount = 0;

		// for storing the current gravity of the player
		gravityStore = player.gravityScale;
	}


	// Update is called once per frame
	void FixedUpdate () {

	//	moveVelocity = 0f;		// in every loop the player velocity is set to 0 so the player WONT SLIDE over platform
		// we dont need to set the moveVelocity to 0f since we are using the getAxis() it will always reset the speed to 0 if we are not detecting any pressings

		// PLAYER GROUND CHECKING
		// physics2D.overlapCircle return true or false
		grounded = Physics2D.OverlapCircle( groundCheck.position, groundCheckRadius, whatIsGround);

		// for the jump animation
		anim.SetBool ("Grounded", grounded);

		if (grounded)
			doubleJump = false;

		// for JUMPING______________________// Input.GetKeyDown (KeyCode.Space)
		if ( Input.GetButtonDown("Jump") && grounded) {
			Jump();
		}

		// DOUBLE JUMPING___________________
		if ( Input.GetButtonDown("Jump") && !doubleJump && !grounded ) {
			Jump();
			doubleJump = true;
		}

/*	REMOVING THIS SINCE WE ARE GOING TO USE THE STANDARED INPUTS
		// PLAYER MOVEMENT__________________
		if (Input.GetKey(KeyCode.LeftArrow)) {
			// instead of new vector2( speed, 0) we use player.velocity.y so the player can jump diagonalyy
			//player.velocity = new Vector2( -moveSpeed * Time.deltaTime, player.velocity.y );
			moveVelocity = -moveSpeed;
		}
		if (Input.GetKey(KeyCode.RightArrow)) {	
			//player.velocity = new Vector2(moveSpeed * Time.deltaTime, player.velocity.y);
			moveVelocity = moveSpeed;
		}
*/

		moveVelocity = moveSpeed * Input.GetAxisRaw ("Horizontal");

		if(knockBackCount <= 0) {
			// PLAYER MOVEMENT.......
			player.velocity = new Vector2 (moveVelocity * Time.deltaTime, player.velocity.y);
			// moveVelocity idea is used so that the player wont slide over the platform
		}else {
			if (knockFromRight) {		// if enemy knocking us from RIGHT
				player.velocity = new Vector2( -knockBack * Time.deltaTime, knockBack*Time.deltaTime ); 
			}
			if (!knockFromRight) {		// if enemy knocking us from LEFT
				player.velocity = new Vector2( knockBack * Time.deltaTime, knockBack*Time.deltaTime ); 
			}

			knockBackCount -= Time.deltaTime;
		}
			
		//ANIMATING WALK_________________
		// setting a floating var Speed with the current velocity of the player so we can use this float to determine whether the 
		// player is moving or not and play the animations accordingly
		anim.SetFloat ("Speed", Mathf.Abs(player.velocity.x));		// mathf.Abs() is used to get a positive value always even if player moving left


		// Flipping player on walking left for better jumping animation e.t.c
		if (player.velocity.x > 0)
			player.transform.localScale = new Vector3 (1f, 1f, 1f);
		else if (player.velocity.x < 0)
			player.transform.localScale = new Vector3 (-1f, 1f, 1f);		// flip the player to left if he moves left....


		// FIRING_________________________
		if( Input.GetButtonDown("Fire1")){
			// making ninja star clones.... :)
			//Debug.Log("Dishum, Dishum");
			Instantiate(ninjaStar, firingPoint.transform.position, firingPoint.transform.rotation);

			shotDelayCounter = shotDelay;

		}

		if (Input.GetButton("Fire1")) {
			shotDelayCounter -= Time.deltaTime;

			if (shotDelayCounter <= 0) {
				shotDelayCounter = shotDelay;
				Instantiate (ninjaStar, firingPoint.transform.position, firingPoint.transform.rotation);
			}
		}

		// so that the sword is not shown every time
		if (anim.GetBool("Sword"))
			anim.SetBool("Sword",false);


		//SWORD play
		if( Input.GetButtonDown("Fire2")) {
			anim.SetBool ("Sword", true);
		}


		// LADDER CLIMBING ___________________
		if( onLadder){
			// setting the player gravity ot 0 so the player can climb the ladder
			player.gravityScale = 0;	

			climbVelocity = climbSpeed * Input.GetAxisRaw ("Vertical");
			player.velocity = new Vector2 (player.velocity.x, climbVelocity);
		}

		if (!onLadder) {
			player.gravityScale = gravityStore;	// resetting the gravity of the player
		}
	}	// END OF FIXEDUPDATE();

	// function for jumping...
	public void Jump() {
		// here the x pos is given as player.velocity.x bcos the player can fall diagonally
		player.velocity = new Vector2(player.velocity.x, jumpHeight * Time.deltaTime);
	}

	// on colliding with something
	void OnCollisionEnter2D( Collision2D other ){
		if (other.transform.tag == "MovingPlatform") {	// if standing on moving platfrom
			// parent of the player in now other.transform
			transform.parent = other.transform;
		}
	}

	void OnCollisionExit2D( Collision2D other ){
		if (other.transform.tag == "MovingPlatform") {	// if standing on moving platfrom
			// parent of the player in null so that the player wont move with the platform even if he has exitted.
			gameObject.transform.parent = null;
		}
	}
}	// END OF CLASS;
