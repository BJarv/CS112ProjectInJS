#pragma strict
import UnityEngine.UI;

enum JumpState
{
	GROUNDED,
	JUMPING,
	FALLING
}

var spawnMan : SpawnManager;
public var PlayerNum : int = 0;
var enemy : GameObject;
public var dashCD : float = .5f;
var dashParts : ParticleSystem;

//Character Controls
private var moveLeft : KeyCode;
private var moveRight : KeyCode;
private var jump : KeyCode;
private var dashLeft : KeyCode;
private var dashRight : KeyCode;

//Movement Variables
var maxSpeed : float = 100f;
var addSpeed : float = 1000f;
var Jump : JumpState = JumpState.GROUNDED;

//Ground helpers
var groundCheck : Transform;
var raycastLength : float = 0.15f;
public var whatIsGround : LayerMask;

//Jumpforce variables
public var PlusJumpForce : float = 5f;
public var CurrJumpForce : float = 5f;
public var MaxJumpForce : float = 5f;

public var horizDirection : int = 1;

var moveH : float;

//control bools
public var colliding : bool = false;
public var dashLock : bool = false;
public var isDashing : bool = false;
public var singleDamageDealt : bool = false;

//cooldown timers
public var lockTimeStamp : float = 0f;
public var dashTimeStamp : float = 0f;
public var damageTimeStamp : float = 0f;

//dash attack variables
public var dashX : float = 5000f;
public var dashY : float = 50f;

var player : GameObject;
var playerStats : PlayerStats;

public var dashAud : AudioClip;
public var hitAuds : AudioClip[];


function Start () {
	if(PlayerNum == 0) {
		Debug.LogError("Player Number not set in inspector, set and retry");
	}
	spawnMan = GameObject.Find("Code").GetComponent(SpawnManager);
	playerStats = gameObject.GetComponent(PlayerStats);
	dashParts = transform.Find("dashParticles").gameObject.GetComponent(ParticleSystem);
	groundCheck = transform.Find("groundCheck");
	assignPlayerControls();

}

public function findHealth() {
	if(PlayerNum == 1) {
		playerStats.healthSlider = GameObject.Find("Canvas/Player1Health/Slider").gameObject.GetComponent(Slider);
	} else if (PlayerNum == 2) {
		playerStats.healthSlider = GameObject.Find("Canvas/Player2Health/Slider").gameObject.GetComponent(Slider);
	}
}

function FixedUpdate() {
	if(!playerStats.isDead) {
		if(Input.GetKey(moveRight)) {
			moveH = 1;
		} else if (Input.GetKey(moveLeft)) {
			moveH = -1;
		} else {
			moveH = 0;
		}
		Flip(moveH);
		if(moveH > 0) {
			if (GetComponent(Rigidbody2D).velocity.x <= maxSpeed){
				GetComponent(Rigidbody2D).AddForce (Vector2(moveH * addSpeed, 0));
			}	
		} else if (moveH == 0 && isGrounded()) {
			//set speed 0?
		} else if (moveH < 0) {
			if(GetComponent(Rigidbody2D).velocity.x > -maxSpeed) {
				GetComponent(Rigidbody2D).AddForce(Vector2(moveH * addSpeed, 0));
			}
		}
		DashAttack();
		dealDamage();
	}
}

function Update () {
	if(!playerStats.isDead) {
		switch(Jump) {
			case JumpState.GROUNDED:
				if(Input.GetKey(jump) && isGrounded()) {
					Jump = JumpState.JUMPING;
				}
				break;
			
			case JumpState.JUMPING:
				if(Input.GetKey(jump) && CurrJumpForce < MaxJumpForce) {
					var forceToAdd : float = PlusJumpForce * Time.deltaTime * 10f;
					CurrJumpForce += forceToAdd;
					GetComponent(Rigidbody2D).AddForce(Vector2(0, forceToAdd));
				} else {
					Jump = JumpState.FALLING;
					CurrJumpForce = 0;
				}
				break;
			
			case JumpState.FALLING:
				if(isGrounded() && GetComponent(Rigidbody2D).velocity.y <= 0) {
					Jump = JumpState.GROUNDED;
				}
				break;
		}
		lockFunction();
	} else {
		playerStats.isDead = false;
		spawnMan.killPlayer(gameObject);
	}
}

function DashAttack() {
	var xTotal : float = 0f;
	var yTotal : float = 0f;
	if(!dashLock && (Input.GetKey(dashLeft) || Input.GetKey(dashRight))) {
		AudioSource.PlayClipAtPoint(dashAud, transform.position);
		GetComponent(Rigidbody2D).drag = 10f;
		GetComponent(Rigidbody2D).velocity = Vector2(0, GetComponent(Rigidbody2D).velocity.y);
		dashParts.Play();
		if(Input.GetKey(dashRight)) {
			Flip(1);
			xTotal = dashX;
			lockTimeStamp = Time.time + dashCD;
			dashTimeStamp = Time.time + .4f;
			dashLock = true;
			isDashing = true;
		} else if(Input.GetKey(dashLeft)){
			Flip(-1);
			xTotal = -dashX;
			lockTimeStamp = Time.time + dashCD;
			dashTimeStamp = Time.time + .4f;
			dashlock = true;
			isDashing = true;
		}
	}
	GetComponent(Rigidbody2D).AddForce(Vector2(xTotal, yTotal));
}

function assignPlayerControls() {
	if(PlayerNum == 1){
		moveLeft = KeyCode.A;
		moveRight = KeyCode.D;
		jump = KeyCode.W;
		dashLeft = KeyCode.Q;
		dashRight = KeyCode.E;
	}
	else if (PlayerNum == 2){
		moveLeft = KeyCode.J;
		moveRight = KeyCode.L;
		jump = KeyCode.I;
		dashLeft = KeyCode.U;
		dashRight = KeyCode.O;
	}
}

function lockFunction() {
	if (lockTimeStamp <= Time.time){
		dashLock = false;
	}
	if (dashTimeStamp <= Time.time) {
		GetComponent(Rigidbody2D).drag = 0;
		isDashing = false;
	}
	if (damageTimeStamp <= Time.time) {
		singleDamageDealt = false;
	}
}

function dealDamage() {
	if(colliding && isDashing && !singleDamageDealt && (enemy != null)) {
		AudioSource.PlayClipAtPoint(hitAuds[Random.Range(0, hitAuds.Length)], transform.position);
		enemy.GetComponent(PlayerStats).currentHealth -= (5 * (Mathf.Abs(GetComponent(Rigidbody2D).velocity.x)/10f));
		singleDamageDealt = true;
		damageTimeStamp = Time.time + 0.5f;
	}
}

function Flip(moveH : float) {
	if(moveH > 0) {
		transform.localEulerAngles = Vector3.zero;
	} else if (moveH < 0) {
		transform.localEulerAngles = Vector3(0, 180, 0);
	}
}

public function isGrounded() : bool {
	return Physics2D.Raycast(groundCheck.position, -Vector2.up, raycastLength, whatIsGround);
}














