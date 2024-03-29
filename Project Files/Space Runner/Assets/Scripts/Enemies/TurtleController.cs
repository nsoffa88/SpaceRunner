﻿using UnityEngine;
using System.Collections;

public class TurtleController : MonoBehaviour {

	private GameObject hero;
	private HealthSystem health;
	private HealthSystem heroHealth;
	private Transform heroTrans;
	private Rigidbody2D rb2d;

	private bool facingRight = false;
	private float knockbackTime;
	private int moveTimer;

	public Transform wallCheck;
	private bool hittingWall = false;
	float wallRadius = 0.2f;
	public LayerMask wallLayer;

	public GameObject spark;
	public Transform sparkPoint;

	private bool notAtEdge;
	public Transform edgeCheck;

	private Animator anim;
	private AudioSource playSpot;
	public AudioClip[] sounds;
	private float playTime = 0f;

	void Start() {
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

		anim.SetInteger ("MoveTimer", moveTimer);
		if (Mathf.Abs(rb2d.velocity.x) < 0.1f && playTime < Time.time) {
			playSpot.pitch = Random.Range(1f,2f);
			playSpot.PlayOneShot (sounds [0]);
			playTime = Time.time + (sounds [0].length * 2);
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
			Knockback (150f);
			health.tookDamage = false;
			moveTimer = 0;
		}

		if (knockbackTime > Time.time) {
			knockbackTime -= Time.deltaTime;
		}

		//Movement Management
		moveTimer++;

		if (moveTimer > 60 && !facingRight && knockbackTime < Time.time) {
			rb2d.velocity = new Vector2 (-2f, rb2d.velocity.y);
		}
		if (moveTimer > 60 && facingRight && knockbackTime < Time.time) {
			rb2d.velocity = new Vector2 (2f, rb2d.velocity.y);
		}

		if (moveTimer >= 80) {
			moveTimer = 0;
		}

		if (moveTimer == 8) {
			Attack ();
		}

		//Ignore collision while hero is invincible
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

	public void Attack(){
		GameObject sparkClone;
		sparkClone = Instantiate(spark, sparkPoint.transform.position, sparkPoint.transform.rotation, gameObject.transform) as GameObject;
		sparkClone.transform.parent = gameObject.transform.parent;
		sparkClone.GetComponent<Rigidbody2D> ().velocity = new Vector2 (-4f, 0f);
		sparkClone = Instantiate(spark, sparkPoint.transform.position, sparkPoint.transform.rotation, gameObject.transform) as GameObject;
		sparkClone.transform.parent = gameObject.transform.parent;
		sparkClone.GetComponent<Rigidbody2D> ().velocity = new Vector2 (+4f, 0f);
	}
}
