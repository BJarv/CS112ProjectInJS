using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour {

	public float startingHealth = 100f;
	public float currentHealth;
	public Slider healthSlider;
	public bool isDead = false;

	// Use this for initialization
	void Start () {
		currentHealth = startingHealth; 
	}
	
	// Update is called once per frame
	void Update () {
		healthSlider.value = currentHealth;
		if (currentHealth <= 0)
			isDead = true;
	}
}
