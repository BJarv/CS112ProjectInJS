using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum JumpState
{
	GROUNDED,
	JUMPING,
	FALLING
}

public class CharControl : MonoBehaviour {
	
	SpawnManager spawnMan;
	public int PlayerNum = 0;
	public GameObject enemy;
	public float dashCD = .5f;
	ParticleSystem dashParts;

	//Character controls
	KeyCode moveLeft;
	KeyCode moveRight;
	KeyCode jump;
	KeyCode dashLeft;
	KeyCode dashRight;

	//Movement variables
	public float maxSpeed = 100f;
	public float addSpeed = 1000f;
	private JumpState Jump = JumpState.GROUNDED;
	
	//Ground stuff
	[HideInInspector] public Transform groundCheck;
	float raycastLength = 0.15f;
	public LayerMask whatIsGround;

	//Jumpforce variables
	public float PlusJumpForce = 5f;
	public float CurrJumpForce = 0f;
	public float MaxJumpForce = 10f;

	public int horizDirection = 1;

	float moveH;
	//control bools
	public bool colliding = false;
	public bool dashLock = false;
	public bool isDashing = false;
	public bool singleDamageDealt = false;

	//cooldown timers
	public float lockTimeStamp = 0;
	public float dashTimeStamp = 0;
	public float damageTimeStamp = 0;

	//Dash attack variables
	public float dashX = 5000f;
	public float dashY = 50f;


	GameObject player;
	PlayerStats playerStats;

	public AudioClip dashAud;
	public AudioClip[] hitAuds;


	void Awake () {
		if(PlayerNum == 0) {
			Debug.LogError("Player Number not set in inspector, set and retry");
		}
		spawnMan = GameObject.Find ("Code").GetComponent<SpawnManager>();
		playerStats = gameObject.GetComponent <PlayerStats> ();
		/*GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		for(int i = 0; i < players.Length; i++){
			if(players[i].GetComponent<CharControl>().PlayerNum != PlayerNum) {
				enemy = players[i];
				break;
			}
		}*/


		dashParts = transform.Find("dashParticles").gameObject.GetComponent<ParticleSystem>();

		//Debug.Log (this.name);
	}

	public void findHealth() {
		if(PlayerNum == 1){
			playerStats.healthSlider = GameObject.Find ("Canvas/Player1Health/Slider").gameObject.GetComponent<Slider>();
		}else if (PlayerNum == 2){
			playerStats.healthSlider = GameObject.Find ("Canvas/Player2Health/Slider").gameObject.GetComponent<Slider>();
		} else {
			Debug.LogError("no playernum set");
		}
	}

	void Start () {
		assignPlayerControls ();
		groundCheck = transform.Find ("groundCheck").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!playerStats.isDead) {
			float moveH = 0; //= Input.GetAxis ("Horizontal");

			if (Input.GetKey(moveRight) || Input.GetKey (moveLeft)) {
				if( Input.GetKey(moveRight) ) {
					moveH = 1;
				} else {
					moveH = -1;
				}
			}

			Flip (moveH);
			if (moveH > 0) {
				if (GetComponent<Rigidbody2D>().velocity.x <= maxSpeed)
					GetComponent<Rigidbody2D>().AddForce (new Vector2 (moveH * addSpeed, 0));
			} else if (moveH == 0 && isGrounded()) {
				//rigidbody2D.velocity = new Vector2 (0, rigidbody2D.velocity.y);
			} //set animation to idle here.
		    else if (moveH < 0) {
				if (GetComponent<Rigidbody2D>().velocity.x > -maxSpeed)
					GetComponent<Rigidbody2D>().AddForce (new Vector2 (moveH * addSpeed, 0));
			}
			DashAttack (); //Player can dash in any JumpState
			dealDamage();
		}
	}

	void Update() {
	  if (!playerStats.isDead) {
			switch (Jump) {
			case JumpState.GROUNDED: 
			//	dashLock = false;
				if (Input.GetKey (jump) && isGrounded ()) {
					Jump = JumpState.JUMPING;
				}
			//DashAttack();
				break;
			
			case JumpState.JUMPING: 
				if (Input.GetKey (jump) && CurrJumpForce < MaxJumpForce) {
					var timeDiff = Time.deltaTime * 10;
					var forceToAdd = PlusJumpForce * timeDiff;
					CurrJumpForce += forceToAdd;
					GetComponent<Rigidbody2D>().AddForce (new Vector2 (0, forceToAdd));
				} else {
					Jump = JumpState.FALLING;
					CurrJumpForce = 0;
				}
			//DashAttack();
				break;
			
			case JumpState.FALLING: 
				if (isGrounded () && GetComponent<Rigidbody2D>().velocity.y <= 0) {
					Jump = JumpState.GROUNDED;
				}
			//DashAttack ();
				break;
			
			}
			lockfuction();


		} else {
			playerStats.isDead = false;
			Debug.Log ("in dying update");
			spawnMan.killPlayer (gameObject);
		}
	}


	void DashAttack(){

		float xTotal = 0f;
		float yTotal = 0f;
		if (!dashLock && (Input.GetKey(dashLeft) || Input.GetKey (dashRight))) { //player can only dash once per jump
			AudioSource.PlayClipAtPoint(dashAud, transform.position);
			GetComponent<Rigidbody2D>().drag = 10f;
		    GetComponent<Rigidbody2D>().velocity = new Vector2 (0, GetComponent<Rigidbody2D>().velocity.y); //Vector2.zero
			dashParts.Play(); //play 
			if (Input.GetKey (dashRight)) {
				Flip (1);
				//rigidbody2D.AddForce (new Vector2 (100, 0));
				xTotal = dashX;
				//playerStats.currentHealth-=10;
			    lockTimeStamp = Time.time + dashCD;
				dashTimeStamp = Time.time + .4f;
				dashLock = true;
				isDashing = true;
			} else if (Input.GetKey (dashLeft)) {
				Flip (-1);
				//rigidbody2D.AddForce (new Vector2 (-100, 0));
				xTotal = -dashX;
				lockTimeStamp = Time.time + dashCD;
				dashTimeStamp = Time.time + .4f;
				//playerStats.currentHealth-=10;
				dashLock = true;
				isDashing = true;
			}
			//dashLock = true;

		//Jump = JumpState.FALLING;
		//CurrJumpForce = 0;

		GetComponent<Rigidbody2D>().AddForce (new Vector2 (xTotal, yTotal));
	}

	//	if(player.name.Equals ("Player1")
	}
	void assignPlayerControls(){
		if(PlayerNum == 1){
			moveLeft = KeyCode.A;
			moveRight = KeyCode.D;
			jump = KeyCode.W;
			dashLeft = KeyCode.Q;
			dashRight = KeyCode.E;
			Debug.Log ("Player 1 Controls Set");
		}
		else if (PlayerNum == 2){
			moveLeft = KeyCode.J;
			moveRight = KeyCode.L;
			jump = KeyCode.I;
			dashLeft = KeyCode.U;
			dashRight = KeyCode.O;
			Debug.Log ("Player 2 Controls Set");
		}
	}

	// 
	void lockfuction(){
		if (lockTimeStamp <= Time.time){
			dashLock = false;
		}
		if (dashTimeStamp <= Time.time) {
			GetComponent<Rigidbody2D>().drag = 0;
			isDashing = false;
		}
		if (damageTimeStamp <= Time.time) {
			singleDamageDealt = false;
		}
	}

	void dealDamage() {
		if ((colliding == true) && (isDashing == true) && (singleDamageDealt == false) && enemy != null) {
			AudioSource.PlayClipAtPoint(hitAuds[Random.Range(0, hitAuds.Length)], transform.position);
			enemy.GetComponent<PlayerStats>().currentHealth-=(5 * (Mathf.Abs (GetComponent<Rigidbody2D>().velocity.x)/10));
			singleDamageDealt = true;
			damageTimeStamp = Time.time + .5f;
		}
	}
	void Flip(float moveH)
	{
		if (moveH > 0)
			transform.localEulerAngles = new Vector3 (0, 0, 0);
		else if (moveH < 0)
			transform.localEulerAngles = new Vector3 (0, 180, 0);
	}

	public bool isGrounded()
	{
		return Physics2D.Raycast (groundCheck.position, -Vector2.up, raycastLength, whatIsGround);
	}


}
