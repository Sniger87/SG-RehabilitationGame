using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Profiles;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject StartPrefab;
    public GameObject EndPrefab;
    public GameObject EmptyPrefab;
    public GameObject[] Prefabs;
    public GameObject[] Backgrounds;
    public GameObject Wall;
    public GameObject Coin;
    public Material[] GroundMaterials;
    public Material[] ObstacleMaterials;

    public static SpawnManager Instance;

    private Transform playerTransform;
    private List<GameObject> listOfPrefabs = new List<GameObject>();

	//KI relevant
	private int chanceEmpty = 0;
	private int[] distro;

    private float spawnAxisZ = 0f;
    //amount of prefabs per area
    private int amountPrefabs = 50;
    // average length of a prefab
    private float lastPrefabLength = 0.0f;
    //number of prefabs visible
    private int preRenders = 10;
    //safety area for despawn
    private float offset = 15.0f;
    //ID of last used prefab
    private int prefabID = -1;
    //number of coins
    private int amountCoins = 25;
    // count of tries for Random
    private int maxRandomTries = 10;
    // id for ground material
    private int groundMaterialID = 0;

    private StringBuilder stringBuilder;

    public int AmountCoins
    {
        get;
        private set;
    }

    public int AmountObstacles
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
		//KI
		distro = KI.Instance.calculateDistribution ();
		chanceEmpty = KI.Instance.calculateEmpty (amountPrefabs - 3);
		Debug.Log (distro [1]);
		Debug.Log (chanceEmpty);

        // TODO: umbauen!
        // wird zurzeit noch hier gesetzt, da amountCoins und amountPrefabs abgezogen wird
        this.AmountCoins = this.amountCoins;
        // TODO: Wenn KI bereit, die EmptyTiles abziehen
        // -3 wegen Start, End und einem Empty
        this.AmountObstacles = this.amountPrefabs - 3;

        groundMaterialID = RandomGroundMaterialID();

        this.stringBuilder = new StringBuilder();
        this.stringBuilder.AppendLine("x;y;width;height;");

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            throw new NullReferenceException("Player object is Null");
        }

        playerTransform = playerObject.transform;

        // spawn start prefab
        Spawn(StartPrefab);
        // spawn other prefabs
        for (int i = 1; i < preRenders; i++)
        {
            Spawn();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (amountPrefabs == 0 && playerTransform.position.z - offset > (spawnAxisZ - preRenders * lastPrefabLength))
        {
            // spawn end prefab
            Spawn(EmptyPrefab);
            Spawn(EndPrefab);
            RemoveOld();
            amountPrefabs = -1;
        }
        //spawn and remove prefabs
        else if (amountPrefabs > 0 && playerTransform.position.z - offset > (spawnAxisZ - preRenders * lastPrefabLength))
        {
            Spawn();
            RemoveOld();
        }
    }

    private void Spawn(GameObject prefabToLoad = null)
    {
        GameObject prefab;
        GameObject borders;
        GameObject background;
        GameObject coinSpawn;

        //spawn random prefab
        if (prefabToLoad == null)
        {
            if (chanceEmpty > 0)
            {
                int rand = UnityEngine.Random.Range(0, 100);
                if (rand < chanceEmpty)
                {
                    //spawn empty prefab
                    prefab = Instantiate(EmptyPrefab,
                        Vector3.forward * spawnAxisZ, Quaternion.identity, transform);
                }
                else
                {
                    //spawn according to KI
                    prefab = Instantiate(Prefabs[RandomPrefabID()],
                        Vector3.forward * spawnAxisZ, Quaternion.identity, transform);
                }
            }
            else
            {
                //spawn without empty according to KI
                prefab = Instantiate(Prefabs[RandomPrefabID()],
                    Vector3.forward * spawnAxisZ, Quaternion.identity, transform);
            }

        }
        else
        {
            prefab = Instantiate(prefabToLoad,
                Vector3.forward * spawnAxisZ, Quaternion.identity, transform);
        }

        // Set Random Material
        Renderer renderer = prefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = GroundMaterials[groundMaterialID];
        }

        SetChildTransformMaterial(prefab.transform);

        // Get Length of the Prefab
        float prefabLength = -1.0f;
        float prefabWidth = -1.0f;
        Collider c = prefab.GetComponent<Collider>();
        if (c != null)
        {
            prefabLength = c.bounds.size.z;
            prefabWidth = c.bounds.size.x;
            AppendPositionData(c.bounds, prefab.transform);
        }
        else
        {
            Renderer r = prefab.GetComponent<Renderer>();
            if (r != null)
            {
                prefabLength = r.bounds.size.z;
                prefabWidth = r.bounds.size.x;
                AppendPositionData(r.bounds, prefab.transform);
            }
        }
        if (prefabLength <= 0.0f)
        {
            throw new InvalidOperationException("No Collider or Renderer detected on the Prefab");
        }

        //add wall, background as child of prefab for deletion
        //add invisible wall
        borders = Instantiate(Wall,
            Vector3.forward * spawnAxisZ, Quaternion.identity, prefab.transform);

        //add background
        background = Instantiate(Backgrounds[RandomBackgroundID()],
            new Vector3(-prefabWidth - 25, -0.5f, 1 * spawnAxisZ), Quaternion.identity, prefab.transform);

        background = Instantiate(Backgrounds[RandomBackgroundID()],
            new Vector3(prefabWidth - 5, -0.5f, 1 * spawnAxisZ), Quaternion.identity, prefab.transform);

        //spawn coin
        if (amountCoins > 0 && prefabToLoad == null)
        {
            //randomly add a coin
            int flipcoin = UnityEngine.Random.Range(0, 2);

            if (((amountPrefabs > amountCoins) && (flipcoin == 1)) || (amountPrefabs <= amountCoins))
            {
                //determine random position
                Vector3 coinPosition = Vector3.zero;
                int randomTries = 0;
                do
                {
                    randomTries++;
                    coinPosition = new Vector3(UnityEngine.Random.Range(-4f, 4f), 1.0f, spawnAxisZ + UnityEngine.Random.Range(-prefabLength / 2f, prefabLength / 2f));
                } while ((Physics.OverlapSphere(coinPosition, 0.95f)).Length != 0 && randomTries < maxRandomTries);

                if (randomTries <= maxRandomTries)
                {
                    //Debug.Log ("Random tries: " +  randomTries.ToString() + "Coin spawned at: " + coinPosition.ToString ());
                    coinSpawn = Instantiate(Coin, coinPosition, Quaternion.identity, prefab.transform);
                    amountCoins--;
                }
            }
        }

        amountPrefabs--;
        // scale beachten!
        this.lastPrefabLength = (prefab.transform.localScale.z < 1.0f) ? prefabLength : (prefabLength / 2.0f);
        spawnAxisZ += lastPrefabLength;
        listOfPrefabs.Add(prefab);
    }

    private int RandomBackgroundID()
    {
        int backgroundsLength = Backgrounds.Length;
        if (backgroundsLength <= 1)
        {
            return 0;
        }
        int randomID = UnityEngine.Random.Range(0, backgroundsLength);
        return randomID;
    }

    private int RandomObstacleMaterialID()
    {
        int materialsLength = ObstacleMaterials.Length;
        if (materialsLength <= 1)
        {
            return 0;
        }
        int randomID = UnityEngine.Random.Range(0, materialsLength);
        return randomID;
    }

    private int RandomGroundMaterialID()
    {
        int materialsLength = GroundMaterials.Length;
        if (materialsLength <= 1)
        {
            return 0;
        }
        int randomID = UnityEngine.Random.Range(0, materialsLength);
        return randomID;
    }

    private int RandomPrefabID()
    {
        //length of array of prefabs
        int prefabsLength = Prefabs.Length;
        if (prefabsLength <= 1)
        {
            return 0;
        }

        int randomTries = 0;
        int randomID = prefabID;
        while (randomID == prefabID && randomTries < maxRandomTries)
        {
            randomTries++;
            randomID = UnityEngine.Random.Range(0, prefabsLength);
        }
        prefabID = randomID;
        return randomID;
    }

    private int WeightedPrefabID()
    {

        int total = distro[0] + distro[1] + distro[2];
        int rand = UnityEngine.Random.Range(0, total);
        if (rand <= distro[0])
        {
            //don't chose left prefab
        }
        else if (rand <= distro[1] + distro[0])
        {
            //don't chose center prefab
        }
        else
        {
            //don't chose right prefab
        }
        return 0;
    }

    private void RemoveOld()
    {
        //Destroy and Delist old prefabs
        Destroy(listOfPrefabs[0]);
        listOfPrefabs.RemoveAt(0);
    }

    private void SetChildTransformMaterial(Transform transform)
    {
        Transform childTransform = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            childTransform = transform.GetChild(i);
            if (childTransform.tag == "Obstacle")
            {
                break;
            }
            childTransform = null;
        }
        if (childTransform != null)
        {
            Renderer renderer = childTransform.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = ObstacleMaterials[RandomObstacleMaterialID()];
            }
        }
    }

    private void AppendPositionData(Bounds bounds, Transform transform)
    {
        if (this.stringBuilder != null)
        {
            Transform childTransform = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                childTransform = transform.GetChild(i);
                if (childTransform.tag == "Obstacle")
                {
                    break;
                }
                childTransform = null;
            }
            if (childTransform != null)
            {
                Nullable<Bounds> childBounds = GetBoundsFromChild(childTransform);
                if (childBounds != null && childBounds.HasValue)
                {
                    float childHeight = childBounds.Value.size.z;
                    float childWidth = childBounds.Value.size.x;
                    float height = bounds.size.z;
                    float width = bounds.size.x;
                    float x = bounds.center.x - width / 2.0f;
                    float z = bounds.center.z - height / 2.0f;
                    float childX = childBounds.Value.center.x - childWidth / 2.0f;
                    float childZ = childBounds.Value.center.z - childHeight / 2.0f;
                    string line = string.Format("{0};{1};{2};{3};", childX, childZ, childWidth, childHeight);
                    this.stringBuilder.AppendLine(line);
                }
            }
        }
    }

    private Nullable<Bounds> GetBoundsFromChild(Transform childTransform)
    {
        Collider c = childTransform.GetComponent<Collider>();
        if (c != null)
        {
            return c.bounds;
        }
        else
        {
            Renderer r = childTransform.GetComponent<Renderer>();
            if (r != null)
            {
                return r.bounds;
            }
        }
        return null;
    }

    private void OnDestroy()
    {
        if (this.stringBuilder != null)
        {
            ProfileManager.Current.WriteLevelPositionFile(this.stringBuilder.ToString());
            this.stringBuilder = null;
        }
    }

}
