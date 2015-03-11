using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpawnManager : MonoBehaviour {

	public GameObject playerPrefab;
	public Transform spawn1;
	public Transform spawn2;
	public int p1Stocks;
	public int p2Stocks;
	public Text p1StockCounter;
	public Text p2StockCounter;
	public float respawnTimer = 2f;
	public AudioClip deathAud;

	// Use this for initialization
	void Awake () {
		p1Stocks = 4;
		p2Stocks = 4;
		SpawnPlayer(1);
		SpawnPlayer(2);
	}
	
	// Update is called once per frame
	void Update () {
		p1StockCounter.text = "Stocks: " + p1Stocks;
		p2StockCounter.text = "Stocks: " + p2Stocks;

	}

	GameObject SpawnPlayer(int playerNum) {
		if(playerNum == 1) {
			GameObject player = (GameObject)Instantiate(playerPrefab, spawn1.position, Quaternion.identity);
			player.GetComponent<CharControl>().PlayerNum = playerNum;
			player.GetComponent<CharControl>().findHealth();
			player.GetComponentInChildren<ParticleSystem>().startColor = Color.red;
			player.GetComponent<SpriteRenderer>().color = Color.red;
			return player;
		} else if (playerNum == 2) {
			GameObject player = (GameObject)Instantiate(playerPrefab, spawn2.position, Quaternion.identity);
			player.GetComponent<CharControl>().PlayerNum = playerNum;
			player.GetComponent<CharControl>().findHealth();
			player.GetComponentInChildren<ParticleSystem>().startColor = Color.cyan;
			player.GetComponent<SpriteRenderer>().color = Color.cyan;
			return player;
		} else {
			Debug.LogError ("Spawn Player Failed");
			return null;
		}
	}
	public void killPlayer(GameObject playerToDie) {
		int deadPNum = playerToDie.GetComponent<CharControl>().PlayerNum;
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

	IEnumerator respawn(int playerNum) {
		yield return new WaitForSeconds(respawnTimer);
		SpawnPlayer(playerNum);
	}
}
