using UnityEngine;

[SelectionBase]
public class Output : GridObject {
    public override void OnNoteEnter( Note note ) {
        Debug.Log( "Played note at pitch " + note.pitch + " and duration " + note.duration );
        // grid.DestroyNote( note );
        MusicPlayer.Singleton.PlayNote(note.pitch, note.duration);
        grid.MarkGoal(note);

		var particles = Grid.MakeNoteCollideParticles( note );
		particles.transform.position = transform.position + Grid.GetParticleOffset( note.direction, 0.15f );
        particles.transform.rotation = Grid.GetDirectionRotation( Grid.GetOppositeDirection( note.direction ) );

		Waver( note.duration * (float)grid.frequency );
	}
}