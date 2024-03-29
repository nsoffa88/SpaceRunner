﻿using UnityEngine;
using System.Collections;

public class SlugController : MonoBehaviour {

	private GameObject hero;
	private HealthSystem health;
	private HealthSystem heroHealth;
	private Transform heroTrans;
	private Rigidbody2D rb2d;

	private bool facingRight = false;

	private float knockbackTime;

	private float moveTimer;

	public Transform wallCheck;
	private bool hittingWall = false;
	float wallRadius = 0.2f;
	public LayerMask wallLayer;

	private bool notAtEdge;
	public Transform edgeCheck;

	private Animator anim;
	private AudioSource playSpot;
	public AudioClip[] sounds;
	private float playTime = 0f;
		
	void Start () {
		health = GetComponent<HealthSystem> ();
		hero = GameObject.FindGameObjectWithTag ("Player");
		heroHealth = hero.GetComponent<HealthSystem> ();
		heroTrans = hero.GetComponent<Transform> ();
		rb2d = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		playSpot = GetComponent<AudioSource> ();
	}

	void Update() {
		if (knockbackTime < Time.time)
			anim.SetFloat ("Speed", Mathf.Abs(rb2d.velocity.x));
		else
			anim.SetFloat ("Speed", 0);

		if (Mathf.Abs(rb2d.velocity.x) > 0.1f && notAtEdge && playTime < Time.time) {
			playSpot.pitch = Random.Range(1f,1.1f);
			playSpot.PlayOneShot (sounds [0]);
			playTime = Time.time + sounds [0].length;
		}
	}

	void FixedUpdate() {
		//Checking for edges/walls to turn around
		hittingWall = Physics2D.OverlapCircle (wallCheck.position, wallRadius, wallLayer);
		notAtEdge = Physics2D.OverlapCircle (edgeCheck.position, wallRadius, wallLayer);

		if ((hittingWall || !notAtEdge) && knockbackTime < Time.time)
			Flip ();

		//Knockback Management
		if (health.tookDamage) {
			Knockback (400f);
			health.tookDamage = false;
			moveTimer = 0;
		}

		if (knockbackTime > Time.time) {
			knockbackTime -= Time.deltaTime;
		}

		//Movement Management
		moveTimer++;

		if (moveTimer > 40 && !facingRight && knockbackTime < Time.time) {
			rb2d.AddForce (new Vector2(-10f - moveTimer/1.5f, rb2d.velocity.y));
		}
		if (moveTimer > 40 && facingRight && knockbackTime < Time.time) {
			rb2d.AddForce (new Vector2(10f + moveTimer/1.5f, rb2d.velocity.y));
		}

		if (moveTimer >= 50) {
			moveTimer = 0;
		}

		//Ignoring collision while Player is invincible
		if(heroHealth.invincible){
			Physics2D.IgnoreCollision (hero.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D> ());
		}
	}

	public void Knockback(float knockPower) {
		if (heroTrans.position.x > transform.position.x) {
			rb2d.AddForce (new Vector2 (-knockPower, 1.5f * knockPower));
		} else {
			rb2d.AddForce (new Vector2 (knockPower, 1.5f * knockPower));
		}
		knockbackTime = Time.time + 2f;
	}

	void Flip() {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void OnTriggerStay2D(Collider2D enter) {
		if (!heroHealth.invincible){
			if (enter.CompareTag ("Player")) {
				heroHealth.Damage (1);
			}
		}
	}

}
