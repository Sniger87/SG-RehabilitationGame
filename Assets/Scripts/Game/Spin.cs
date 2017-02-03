using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {
	public float speed = 100;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(45, 45, 45), speed * Time.deltaTime);
	}
}
