using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuLoad : MonoBehaviour {

	public GameObject loadImage;

	public void Load(int id) {
		loadImage.SetActive (true);
		SceneManager.LoadScene (id);
	}
}
