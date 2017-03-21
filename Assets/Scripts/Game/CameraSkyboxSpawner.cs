using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSkyboxSpawner : MonoBehaviour
{
    public Material[] SkyboxMaterial;

    // Use this for initialization
    private void Start()
    {
        Skybox skybox = null;
        skybox = this.GetComponent<Skybox>();
        if (skybox != null)
        {
            skybox.material = SkyboxMaterial[RandomSkyboxID()];
        }
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private int RandomSkyboxID()
    {
        int skyboxesLength = SkyboxMaterial.Length;
        if (skyboxesLength <= 1)
        {
            return 0;
        }
        int randomID = UnityEngine.Random.Range(0, skyboxesLength);
        return randomID;
    }
}
