#pragma strict

private var player : CharControl;
public var camera : GameObject;

function Start () {
	player = transform.parent.GetComponent(CharControl);
}

function Update () {

}

function OnTriggerEnter2D (objectCollidedWith : Collider2D) {
	if (objectCollidedWith.tag == "Player") { 
		player.enemy = objectCollidedWith.gameObject;
		player.colliding = true;
	}
}

function OnTriggerExit2D (objectCollidedWith : Collider2D) {
	if (objectCollidedWith.tag == "Player") {
		player.colliding = false;
		player.enemy = null;
	}
}