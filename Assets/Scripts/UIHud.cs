using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIHud : MonoBehaviour 
{
	[SerializeField] Button backToMain;
	[SerializeField] Button reset;
	[SerializeField] GameObject youWinScreen;

	void Awake() {
		backToMain.onClick.AddListener( () => {
			SceneManager.LoadScene(0);
		} );

		reset.onClick.AddListener( () => {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		} );
	}

	void Update () {
		youWinScreen.gameObject.SetActive(Grid.Singleton != null && Grid.Singleton.IsGoalComplete);
	}
}
