﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IKillable {

	[HideInInspector]public Status playerStatus;

	[SerializeField] private GameObject gameOverText;
	[SerializeField] private LayerMask groundMask;
	[SerializeField] private ScreenController screenController;
	[SerializeField] private AudioClip damageSound;

	private Vector3 direction;
	private PlayerMovement playerMovement;
	private CharacterAnimation playerAnimation;

	void Start () {
		// Starts the game without been paused
		Time.timeScale = 1;

		playerMovement = GetComponent<PlayerMovement>();
		playerAnimation = GetComponent<CharacterAnimation>();
		playerStatus = GetComponent<Status>();
	}

	void Update () {

		// player movement inputs. Stores the X and Z direction using the pressed keys
		float xAxis = Input.GetAxis("Horizontal");
		float zAxis = Input.GetAxis("Vertical");

		// creates a Vector3 with the new direction
		direction = new Vector3 (xAxis, 0, zAxis);

		// player animations transition
		playerAnimation.Movement(direction.magnitude);

		// if the player isn't alive anymore 
		// and the mouse button was clicked, restart the game
		if (playerStatus.health <= 0) {
			if (Input.GetButtonDown ("Fire1"))
				SceneManager.LoadScene("Game");
		}
	}
		
	void FixedUpdate () {
		// moves the player by second using physics
		// use physics (rigidbody) to compute the player movement is better than transform.position 
		// because prevents the player to "bug" when colliding with other objects
		playerMovement.Movement(direction, playerStatus.speed);

		playerMovement.PlayerRotation(groundMask);
	}

	/// <summary>
	/// Loses health based on the damage value. 
	/// If health is equal to or less than 0 the game ends.
	/// </summary>
	/// <param name="damage">Damage taken.</param>
	public void LoseHealth (int damage) {
		playerStatus.health -= damage;
		screenController.UpdateHealthSlider();

		// plays the damage sound
		AudioController.instance.PlayOneShot(damageSound);

		if (playerStatus.health <= 0)
			Die();
	}

	/// <summary>
	/// Pauses the game and display the Game Over message on the screen.
	/// </summary>
	public void Die () {
		Time.timeScale = 0;
		gameOverText.SetActive(true);
	}
}
