﻿using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {

	private bool attacking = false;

	private float attackTimer = 0f;
	private float attackCD = 0.3f;

	public Collider2D attackTrigger;



	void Awake() {
		attackTrigger.enabled = false;
	}

	void Update() {
		if (Input.GetKeyDown ("c") && !attacking) {
			attacking = true;
			attackTimer = attackCD;

			attackTrigger.enabled = true;
		}

		if (attacking) {
			if (attackTimer > 0) {
				attackTimer -= Time.deltaTime;
			} else {
				attacking = false;
				attackTrigger.enabled = false;
			}
		}
	}
}