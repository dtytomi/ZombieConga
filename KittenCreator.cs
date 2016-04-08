using UnityEngine;
using System.Collections;

public class KittenCreator : MonoBehaviour {

	public float minSpawnTime = 0.75f;
	public float maxSpawnTime = 2f;
	public GameObject catPrefab;

	// Use this for initialization
	void Start () {
		Invoke ("SpawnCat", minSpawnTime);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void SpawnCat() {

		Camera camera = Camera.main;
		Vector3 cameraPos = camera.transform.position;
		float xMax = camera.aspect * camera.orthographicSize;
		float xRange = camera.aspect * camera.orthographicSize * 1.75f;
		float yMax = camera.orthographicSize - 0.5f;

		Vector3 catPos = 
			new Vector3 (cameraPos.x + Random.Range (xMax - xRange, yMax),
			            Random.Range (-yMax, yMax),
			            catPrefab.transform.position.z);

		Instantiate(catPrefab, catPos, Quaternion.identity);

		Invoke ("SpawnCat", Random.Range(minSpawnTime, maxSpawnTime));
	}
}
