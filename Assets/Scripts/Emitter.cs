using UnityEngine;

[SelectionBase]
public class Emitter : GridObject {

	[SerializeField] private int emitPerBeats;
	private int beatsLeft;

	public int maxEmitted;

	void Awake() {
		beatsLeft = emitPerBeats;
	}

    public override void OnNoteEnter( Note note ) {
		  grid.DestroyNote( note );
    }

	public bool CheckReady() {
		if( maxEmitted <= 0 ) return false;
		beatsLeft--;
		if(beatsLeft == 0)
		{
			beatsLeft = emitPerBeats;
			maxEmitted -= 1;
			return true;
		}
		else
		{
			return false;
		}
	}
}
