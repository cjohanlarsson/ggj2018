using UnityEngine;

[SelectionBase]
public class Emitter : GridObject {

	[SerializeField] private int emitPerBeats;
	public float duration = 0.5f;

	private int beatsLeft;

	public int maxEmitted;

	public int remainingEmitted;

	public override void Init( Grid grid ) {
		base.Init( grid );
		OnStop();
	}

	public override void OnStop () {
		beatsLeft = emitPerBeats;
		remainingEmitted = maxEmitted;
	}

    public override void OnNoteEnter( Note note ) {
		  grid.DestroyNote( note );
    }

	public bool CheckReady() {
		if( remainingEmitted <= 0 ) return false;
		beatsLeft--;
		if(beatsLeft == 0)
		{
			beatsLeft = emitPerBeats;
			remainingEmitted -= 1;
			return true;
		}
		else
		{
			return false;
		}
	}
}
