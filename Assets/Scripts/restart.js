#pragma strict

public var restartAfter : float = 2f;

function Awake () {
	StartCoroutine("LoadMenu");
}

function LoadMenu() {
	yield new WaitForSeconds(restartAfter);
	Application.LoadLevel("Menu");
}