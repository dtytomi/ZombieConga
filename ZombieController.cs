using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieController : MonoBehaviour {

	public float moveSpeed;
	public float turnSpeed; 
	public AudioClip catContactSound;
	public AudioClip enemyContactSound;

	private Vector3 moveDirection;
	private List<Transform> congaLine = new List<Transform>();
	private bool isInvincible = false;
	private float timeSpentInvincible;
	private int lives = 3;

	[SerializeField]
	private PolygonCollider2D[] colliders;
	private int currentColliderIndex = 0;

	// Use this for initialization
	void Start () {
		moveDirection = Vector3.right;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 currentPosition = transform.position;

		if ( Input.GetButton("Fire1")) {
			
			Vector3 moveToward = Camera.main.ScreenToWorldPoint( Input.mousePosition);
			
			moveDirection = moveToward - currentPosition;
			moveDirection.z = 0;
			moveDirection.Normalize();
		}

		Vector3 target = moveDirection * moveSpeed + currentPosition;
		transform.position = Vector3.Lerp (currentPosition, target, Time.deltaTime);

		float targetAngle = Mathf.Atan2 (moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
		transform.rotation = 
			Quaternion.Slerp (transform.rotation,
			                  Quaternion.Euler (0, 0, targetAngle),
			                  turnSpeed * Time.deltaTime);

		
		EnforceBounds();

		if(isInvincible) {

			timeSpentInvincible += Time.deltaTime;

			if( timeSpentInvincible < 3f ) {

				float remainder = timeSpentInvincible % .3f;
				renderer.enabled = remainder > .15f;
			}
			else {
				renderer.enabled = true;
				isInvincible = false;
			}
		}

	}

	public void SetColliderForSprite( int spriteNum ){

		colliders[currentColliderIndex].enabled = false;
		currentColliderIndex = spriteNum;
		colliders[currentColliderIndex].enabled = true;
	}

	void OnTriggerEnter2D( Collider2D other ) {

		if (other.CompareTag("cat")) {
			audio.PlayOneShot(catContactSound);
			Transform followTarget = congaLine.Count == 0 ? transform : congaLine[congaLine.Count-1];
			other.transform.parent.GetComponent<CatController>().JoinConga(followTarget, moveSpeed, turnSpeed);
		 	congaLine.Add(other.transform);
		} 
		else if ( !isInvincible && other.CompareTag("enemy")) {
			audio.PlayOneShot(enemyContactSound);
			isInvincible = true;
			timeSpentInvincible = 0;

			for(int i = 0; i < 2 && congaLine.Count > 0; i++ ){

				int lastIdx = congaLine.Count-1;
				Transform cat =congaLine[lastIdx];
				congaLine.RemoveAt(lastIdx);
				cat.parent.GetComponent<CatController>().ExitConga();
			}
		}

		if (congaLine.Count >= 5) {
			Debug.Log("You Won");
			Application.LoadLevel("WinScene");
		}

		if (--lives <= 0 ) {
			Debug.Log("You Lose");
			Application.LoadLevel("LoseScene");
		}
	}

	private void EnforceBounds() {
		
		Vector3 newPosition = transform.position;
		Camera mainCamera = Camera.main;
		Vector3 cameraPosition = mainCamera.transform.position;
		
		float xDist = mainCamera.aspect * mainCamera.orthographicSize;
		float xMax = cameraPosition.x + xDist;
		float xMin = cameraPosition.x - xDist;

		float yMax = mainCamera.orthographicSize;
		
		if (newPosition.x < xMin || newPosition.x > xMax) {
			newPosition.x = Mathf.Clamp( newPosition.x, xMin, xMax);
			moveDirection.x  = -moveDirection.x ;	
		}

		if (newPosition.y < -yMax || newPosition.y > yMax) {
			newPosition.y = Mathf.Clamp( newPosition.y, -yMax, yMax );
			moveDirection.y = -moveDirection.y;
		}
		
		transform.position = newPosition;
	}
}
