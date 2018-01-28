using TMPro;
using UnityEngine;

[SelectionBase]
public class PitchRaiseGate : GridObject {
    public int raiseAmount;

	public TextMeshPro text;

	public Transform dir;

	public override void Init ( Grid grid ) {
		base.Init( grid );

		text.text = raiseAmount.ToString();

		if( raiseAmount < 0 ) {
			dir.rotation = Quaternion.AngleAxis( 180, Vector3.forward );
		} else {
			dir.rotation = Quaternion.AngleAxis( 0, Vector3.forward );
		}
	}

	public override void OnNoteEnter( Note note ) {
        note.pitch += raiseAmount;

		Waver( note.duration * (float)grid.frequency );

		var particles = Grid.MakeNoteCollideParticles( note );
		particles.transform.position = transform.position + Grid.GetParticleOffset( note.direction, 0.18f );
		particles.transform.rotation = Grid.GetDirectionRotation( Grid.GetOppositeDirection( note.direction ) );
	}

	private void OnValidate () {
		if( !Application.isPlaying ) text.text = raiseAmount.ToString();
	}
}
