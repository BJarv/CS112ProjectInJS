using UnityEngine;
using System.Collections;

public class InAttackRange : MonoBehaviour {
	private CharControl player;
	public GameObject camera;

	//public int damageVariable = 10;

	// Use this for initialization
	void Start () {
		player = transform.parent.GetComponent<CharControl>();

	}
	
	// Update is called once per frame
	void Update () {
		if (player.colliding) {
			//camera.transform.position = new Vector2 (camera.transform.position.x, camera.transform.position.y + 1);
		}
	}

	void OnTriggerEnter2D (Collider2D objectCollidedWith) {
		if (objectCollidedWith.tag == "Player") { 
			player.enemy = objectCollidedWith.gameObject;
			player.colliding = true;

		}
	}

	void OnTriggerExit2D (Collider2D objectCollidedWith) {
		if (objectCollidedWith.tag == "Player") {
			player.colliding = false;
			player.enemy = null;
		}
	}
}
