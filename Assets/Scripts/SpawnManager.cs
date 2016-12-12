using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour {

	public GameObject[] prefabs;
	public GameObject wall;

	private Transform player;
	private List<GameObject> listOfPrefabs = new List<GameObject> ();

	private float spawnAxisZ = -5.0f;
	private float prefabLength = 25.0f;
	//number of prefabs visible
	private int preRenders = 5;
	//safety area for despawn
	private float offset = 50.0f;
	//ID of last used prefab
	private int prefabID = 1;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		//spawn Start prefab
		Spawn (0);
		for (int i = 1; i < preRenders; i++) {
			Spawn ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		//spawn and remove prefabs
		if (player.position.z - offset > (spawnAxisZ - preRenders * prefabLength)) {
			Spawn();
			RemoveOld ();
		}
	}

	private void Spawn(int prefabIndex = -1) {
		GameObject prefab;
		GameObject borders;

		//spawn random prefab
		if (prefabIndex == -1) {
			prefab = Instantiate (prefabs [RandomPrefabID ()]) as GameObject;
		} else {
			prefab = Instantiate (prefabs [prefabIndex]) as GameObject;
		}
		prefab.transform.SetParent (transform);
		prefab.transform.position = Vector3.forward * spawnAxisZ;
		//add invisible wall
		borders = Instantiate (wall) as GameObject;
		borders.transform.SetParent (transform);
		borders.transform.position = Vector3.forward * spawnAxisZ;
		//add wall as child of prefab for deletion
		borders.transform.SetParent (prefab.transform);

		spawnAxisZ += prefabLength;
		listOfPrefabs.Add (prefab);
	
	}

	private int RandomPrefabID() {
		//length of array of prefabs
		int prefabsL = prefabs.Length;
		if (prefabsL <= 1) {
			return 0;
		}

		int randomID = prefabID;
		while (randomID == prefabID) {
			randomID = Random.Range (1, prefabsL);
		}
		prefabID = randomID;
		return randomID;

	}

	private void RemoveOld() {
		//Destroy and Delist old prefabs
		Destroy (listOfPrefabs [0]);
		listOfPrefabs.RemoveAt (0);
	}

}
