using UnityEngine;

[SelectionBase]
public class Emitter : GridObject {

	[SerializeField] private int emitPerBeats;
	private int beatsLeft;

	void Awake() {
		beatsLeft = emitPerBeats;
	}

    public override void OnNoteEnter( Note note ) {
		  grid.DestroyNote( note );
    }

	public bool CheckReady() {
		beatsLeft--;
		if(beatsLeft == 0)
		{
			beatsLeft = emitPerBeats;
			return true;
		}
		else
		{
			return false;
		}
	}
}
