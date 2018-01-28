using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIHud : MonoBehaviour 
{
	[SerializeField] Button backToMain;
	[SerializeField] Button reset;
	[SerializeField] Button play;
	[SerializeField] Text playText;
	[SerializeField] GameObject youWinScreen;

	void Awake() {
		backToMain.onClick.AddListener( () => {
			SceneManager.LoadScene(0);
		} );

		reset.onClick.AddListener( () => {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		} );

		play.onClick.AddListener( () => {
			Grid.Singleton.SetPlaying( !Grid.Singleton.isPlaying );
			if( Grid.Singleton.isPlaying ) {
				playText.text = "Stop";
			} else {
				playText.text = "Play";
			}
		} );
	}

	void Update () {
		youWinScreen.gameObject.SetActive(Grid.Singleton != null && Grid.Singleton.IsGoalComplete);
	}
}
