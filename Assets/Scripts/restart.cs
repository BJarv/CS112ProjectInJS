using UnityEngine;
using System.Collections;

public class restart : MonoBehaviour {

	public float restartAfter = 2f;
	// Use this for initialization
	void Awake () {
		StartCoroutine("LoadMenu");
	}

	IEnumerator LoadMenu() {
		yield return new WaitForSeconds(restartAfter);
		Application.LoadLevel ("Menu");
	}

	// Update is called once per frame
	void Update () {
	
	}
}
