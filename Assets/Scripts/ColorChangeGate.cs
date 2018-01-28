using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeGate : GridObject {
	public SpriteRenderer gfx;

	public NoteColor[] colorsToChangeTo;

	NoteColor currentColor;

	float lockTimer = 0;

	public override void OnNoteEnter ( Note note ) {
		note.color = currentColor;

        var particles = Grid.MakeNoteCollideParticles( note );
        particles.transform.position = transform.position + Grid.GetParticleOffset( note.direction, 0.20f );
        particles.transform.rotation = Grid.GetDirectionRotation( Grid.GetOppositeDirection( note.direction ) );

		lockTimer = note.duration;
		Waver( note.duration * (float)grid.frequency );
	}

	public override void Tick () {
		var nc = colorsToChangeTo[ this.grid.BeatsTicked % colorsToChangeTo.Length ];
		if( lockTimer > 0 ) {
			lockTimer--;
			return;
		}

		if( nc != currentColor ) {
			Pulse();
			currentColor = nc;
			gfx.color = Visuals.Singleton.ConvertNoteColorToColor( currentColor );
		}
	}
}
