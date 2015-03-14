#pragma strict
import UnityEngine.UI;

public var startingHealth : float = 100f;
public var currentHealth : float;
public var healthSlider : Slider;
public var isDead = false;
var controller : CharControl;

function Start () {
	controller = GetComponent(CharControl);
	if(controller.PlayerNum == 1) {
		healthSlider = GameObject.Find("Canvas/Player1Health/Slider").gameObject.GetComponent(Slider);
	} else if (controller.PlayerNum == 2) {
		healthSlider = GameObject.Find("Canvas/Player2Health/Slider").gameObject.GetComponent(Slider);
	}
	currentHealth = startingHealth;
}

function Update () {
	if(healthSlider != null) {
		healthSlider.value = currentHealth;
		if (currentHealth <= 0)
			isDead = true;
	}
}