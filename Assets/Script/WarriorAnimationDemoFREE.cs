﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WarriorAnimationDemoFREE : MonoBehaviour 
{
	public Animator animator;
	public GameObject sentanL;
	public GameObject sentanR;
	public Collider colL;
	public Collider colR;
	float rotationSpeed = 30;
	public int playerHP;

	Vector3 inputVec;
	Vector3 targetDirection;

	
	//Warrior types
	public enum Warrior{Karate, Ninja, Brute, Sorceress, Knight, Mage, Archer, TwoHanded, Swordsman, Spearman, Hammer, Crossbow};

	public Warrior warrior;
	
	void Start()
	{
		//sentanL.SetActive (false);
		//sentanR.SetActive (false);
		//上の処理はミスったやつ
		playerHP = 50;
		colL.enabled = false;
		colR.enabled = false;
	
	}

	void Update()
	{
		foreach ( GameObject obj in GameObject.FindGameObjectsWithTag("Player")){
			Debug.Log (obj.transform.position);

		}
		//Get input from controls
		float z = Input.GetAxisRaw("Horizontal");
		float x = -(Input.GetAxisRaw("Vertical"));
		inputVec = new Vector3(x, 0, z);

		//Apply inputs to animator
		animator.SetFloat("Input X", z);
		animator.SetFloat("Input Z", -(x));

		if (x != 0 || z != 0 )  //if there is some input
		{
			//set that character is moving
			animator.SetBool("Moving", true);
			animator.SetBool("Running", true);
		}
		else
		{
			//character is not moving
			animator.SetBool("Moving", false);
			animator.SetBool("Running", false);
		}

		if (Input.GetButtonDown("Fire1"))//ここで攻撃してる
		{
			Attack ();
		}


		UpdateMovement();  //update character position and facing

		if (playerHP <= 0) 
		{
			//スコアをGameManagerのSetScore関数の引数に代入
			GameManager.instance.SetScore(GameManager.instance.enemycount);
			SceneManager.LoadScene ("Result");
		}
	}



	void Attack(){
		animator.SetTrigger("Attack1Trigger");
		if (warrior == Warrior.Brute)
			StartCoroutine (COStunPause(1.2f));
		else if (warrior == Warrior.Sorceress)
			StartCoroutine (COStunPause(1.2f));
		else
			StartCoroutine (COStunPause(.6f));
		//この下の処理でスマホの先端を出現させる
		//sentanL.SetActive(true);
		//sentanR.SetActive (true);
		colL.enabled = true;
		colR.enabled = true;

	}

	public IEnumerator COStunPause(float pauseTime)
	{
		yield return new WaitForSeconds(pauseTime);
	}

	//converts control input vectors into camera facing vectors
	void GetCameraRelativeMovement()
	{  
		Transform cameraTransform = Camera.main.transform;

		// Forward vector relative to the camera along the x-z plane   
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;

		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right= new Vector3(forward.z, 0, -forward.x);

		//directional inputs
		float v= Input.GetAxisRaw("Vertical");
		float h= Input.GetAxisRaw("Horizontal");

		// Target direction relative to the camera
		targetDirection = h * right + v * forward;
	}

	//face character along input direction
	void RotateTowardMovementDirection()  
	{
		if (inputVec != Vector3.zero)
		{
			
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * rotationSpeed);
			Debug.Log (transform.rotation);
		}
	}

	void UpdateMovement()
	{
		//get movement input from controls
		Vector3 motion = inputVec;

		//reduce input for diagonal movement
		motion *= (Mathf.Abs(inputVec.x) == 1 && Mathf.Abs(inputVec.z) == 1)?0.7f:1;

		RotateTowardMovementDirection();  
		GetCameraRelativeMovement();  
	}

	void OnGUI () 
	{
		if (GUI.Button (new Rect (25, 85, 100, 30), "Attack1")) 
		{
			animator.SetTrigger("Attack1Trigger");

			if (warrior == Warrior.Brute || warrior == Warrior.Sorceress)  //if character is Brute or Sorceress
				StartCoroutine (COStunPause(1.2f));
			else
				StartCoroutine (COStunPause(.6f));
		}
	}

	public void OffSentanCollider()
	{
		colL.enabled = false;
		colR.enabled = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "EnemySentan") {
			playerHP -= 1;
			Debug.Log (playerHP);

		}
		
	}
}