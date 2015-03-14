#pragma strict
import UnityEngine.UI;

public var startingHealth : float = 100f;
public var currentHealth : float;
public var healthSlider : Slider;
public var isDead = false;

function Start () {
	currentHealth = startingHealth;
}

function Update () {
	healthSlider.value = currentHealth;
	if (currentHealth <= 0)
		isDead = true;
}