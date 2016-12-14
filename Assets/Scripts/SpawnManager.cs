using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour {

	public GameObject[] prefabs;
	public GameObject[] backgrounds;
	public GameObject wall;
	public GameObject coin;

	private Transform player;
	private List<GameObject> listOfPrefabs = new List<GameObject> ();

	private float spawnAxisZ = -5.0f;
	//amount of prefabs per area
	private int amntPrefabs = 10;
	private float prefabLength = 25.0f;
	//number of prefabs visible
	private int preRenders = 7;
	//safety area for despawn
	private float offset = 75.0f;
	//ID of last used prefab
	private int prefabID = 2;
	//number of coins
	private int amntCoins = 3;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		//spawn Start prefab
		Spawn (0);
		for (int i = 2; i < preRenders; i++) {
			Spawn ();
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (amntPrefabs == 0 && player.position.z - offset > (spawnAxisZ - preRenders * prefabLength)) {
			Spawn (1);
			RemoveOld ();
			amntPrefabs = -1;
		} else {
			//spawn and remove prefabs
			if (amntPrefabs > 0 && player.position.z - offset > (spawnAxisZ - preRenders * prefabLength)) {
				Spawn ();
				RemoveOld ();
			}
		}
	}

	private void Spawn(int prefabIndex = -1) {
		GameObject prefab;
		GameObject borders;
		GameObject background;
		GameObject coinSpawn;

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

		//add background
		background = Instantiate(backgrounds[RandomBackgroundID()]) as GameObject;

		//add wall, background as child of prefab for deletion
		background.transform.SetParent(prefab.transform);
		borders.transform.SetParent (prefab.transform);

		background.transform.position = new Vector3(0, -1, 1 * spawnAxisZ + prefabLength/2);
		borders.transform.position = Vector3.forward * spawnAxisZ;

		//spawn coin
		if (amntCoins > 0) {
			int flipcoin = Random.Range (0, 2);
			if (((amntPrefabs > amntCoins) && (flipcoin == 1)) || (amntPrefabs <= amntCoins)) {
				coinSpawn = Instantiate (coin) as GameObject;
				coinSpawn.transform.SetParent (transform);
				coinSpawn.transform.position = new Vector3(0, 0.6f, 1 * spawnAxisZ + 5);
				coinSpawn.transform.SetParent (prefab.transform);
				amntCoins--;
			}		
		}

		amntPrefabs--;
		spawnAxisZ += prefabLength;
		listOfPrefabs.Add (prefab);
	
	}

	private int RandomBackgroundID() {
		int backgroundsL = backgrounds.Length;
		if (backgroundsL <= 1) {
			return 0;
		}
		int randomID = Random.Range (0, backgroundsL);
		return randomID;

	}

	private int RandomPrefabID() {
		//length of array of prefabs
		int prefabsL = prefabs.Length;
		if (prefabsL <= 1) {
			return 0;
		}

		int randomID = prefabID;
		while (randomID == prefabID) {
			randomID = Random.Range (2, prefabsL);
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
