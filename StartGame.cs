using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke("LoadLevel", 3f);
	}
	
	// Update is called once per frame
	void Update () {
		Application.LoadLevel("ZombieConga");
	}
}
