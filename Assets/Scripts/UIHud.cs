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

	[SerializeField] RectTransform goalMusicSheet;
	[SerializeField] GameObject goalVertLine;
	[SerializeField] GameObject goalHoriLine;
	[SerializeField] GameObject goalNote;

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

	void Start() {
		CreateMusicSheet();
	}

	void Update () {
		youWinScreen.gameObject.SetActive(Grid.Singleton != null && Grid.Singleton.IsGoalComplete);
		var goals = Grid.Singleton.goals;
		for(int i=0;i<goals.Count;i++)
			notes[i].color = goals[i].Complete ? Color.black : new Color(0.7f,0.7f,0.7f,1f);
	}

	List<Image> notes = new List<Image>();
	void CreateMusicSheet() {
		if(Grid.Singleton != null) {
			
			this.goalHoriLine.SetActive(true);
			this.goalVertLine.SetActive(true);
			this.goalNote.SetActive(true);

			int padding = 4;
			int horiPadding = 10;
			int height = 64 - (padding * 2);
			int width = 300 - (horiPadding * 2);

			int numBeats = 0;
			int minPitch = int.MaxValue;
			int maxPitch = int.MinValue;

			foreach(var g in Grid.Singleton.goals) {
				numBeats = Mathf.Max( g.beatIndex, numBeats );
				minPitch = Mathf.Min( g.pitch , minPitch );
				maxPitch = Mathf.Max( g.pitch , maxPitch );
			}

			minPitch--;
			maxPitch++;

			int numPitches = (maxPitch - minPitch) + 1;
			numPitches = Mathf.Max( 3, numPitches );

			numBeats = Mathf.Max(8, numBeats);

			for(int i=0;i<numPitches;i++) {
				var go = GameObject.Instantiate( goalHoriLine , goalVertLine.transform.parent );
				var p = go.transform.localPosition;
				p.y = padding + (i * height / (numPitches-1));
				go.transform.localPosition = p;
			}

			for(int i=0;i<numBeats;i++) {
				var go = GameObject.Instantiate( goalVertLine , goalVertLine.transform.parent );
				var p = go.transform.localPosition;
				p.x = horiPadding + (i * width / (numBeats-1));
				go.transform.localPosition = p;
			}

			foreach(var g in Grid.Singleton.goals) {
				var go = GameObject.Instantiate( goalNote , goalNote.transform.parent );
				var p = go.transform.localPosition;
				p.x = horiPadding + (g.beatIndex * width / (numBeats-1));
				p.y = padding + ( (g.pitch - minPitch) * height / (numPitches-1) );
				go.transform.localPosition = p;
				notes.Add(go.GetComponent<Image>());
			}

			this.goalHoriLine.SetActive(false);
			this.goalVertLine.SetActive(false);
			this.goalNote.SetActive(false);

		}
		else
		{
			Debug.LogError("No Grid Found!" );
		}
	}
}
