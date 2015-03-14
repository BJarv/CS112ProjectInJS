#pragma strict
import UnityEngine.UI;

public var playerPrefab : GameObject ;
public var spawn1 : Transform;
public var spawn2 : Transform;
public var p1Stocks : int;
public var p2Stocks : int;
public var p1StockCounter : Text;
public var p2StockCounter : Text;
public var respawnTimer : float = 2f;
public var deathAud : AudioClip;

function Awake () {
	p1Stocks = 4;
	p2Stocks = 4;
	SpawnPlayer(1);
	SpawnPlayer(2);
}

function Update () {
	p1StockCounter.text = "Stocks: " + p1Stocks;
	p2StockCounter.text = "Stocks: " + p2Stocks;
}

function SpawnPlayer(playerNum : int) {
	if(playerNum == 1) {
		var player1 : GameObject = Instantiate(playerPrefab, spawn1.position, Quaternion.identity);
		player1.GetComponent(CharControl).PlayerNum = playerNum;
		player1.GetComponent(CharControl).findHealth();
		player1.GetComponentInChildren(ParticleSystem).startColor = Color.red;
		player1.GetComponent(SpriteRenderer).color = Color.red;
		return player1;
	} else if (playerNum == 2) {
		var player2 : GameObject = Instantiate(playerPrefab, spawn2.position, Quaternion.identity);
		player2.GetComponent(CharControl).PlayerNum = playerNum;
		player2.GetComponent(CharControl).findHealth();	
		player2.GetComponentInChildren(ParticleSystem).startColor = Color.cyan;
		player2.GetComponent(SpriteRenderer).color = Color.cyan;
		return player2;
	} else {
		Debug.LogError ("Spawn Player Failed");
		return null;
	}
}

function killPlayer(playerToDie : GameObject) {
	var deadPNum : int; 
	deadPNum = playerToDie.GetComponent(CharControl).PlayerNum;
	if(deadPNum == 1) {
		p1Stocks--;
		if(p1Stocks < 1) {
			Application.LoadLevel("Player2Wins");
		} else {
			StartCoroutine ("respawn", 1);
		}
	} else if(deadPNum == 2) {
		p2Stocks--;
		if(p2Stocks < 1) {
			Application.LoadLevel("Player1Wins");
		} else {
			StartCoroutine ("respawn", 2);
		}
	}
	AudioSource.PlayClipAtPoint(deathAud, transform.position);
	Destroy(playerToDie);
}

function respawn(playerNum : int) {
	yield new WaitForSeconds(respawnTimer); // yeild is different in JS?
	SpawnPlayer(playerNum);
}