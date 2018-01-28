using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeGate : GridObject {
	public SpriteRenderer gfx;

	public NoteColor[] colorsToChangeTo;

	NoteColor currentColor;

	float lockTimer = 0;

	public override void Init ( Grid grid ) {
		base.Init( grid );
		OnStop();
	}

	public override void OnNoteEnter ( Note note ) {
		note.color = currentColor;

		gfx.color = Visuals.Singleton.ConvertNoteColorToColor( currentColor );

		var particles = Grid.MakeNoteCollideParticles( note );
        particles.transform.position = transform.position + Grid.GetParticleOffset( note.direction, 0.18f );
        particles.transform.rotation = Grid.GetDirectionRotation( Grid.GetOppositeDirection( note.direction ) );

		lockTimer = note.duration;
		Waver( note.duration * ( 1 + (float)grid.frequency ) );
	}

	public override void OnStop () {
		base.OnStop();
		lockTimer = 0;
		currentColor = colorsToChangeTo[ 0 ];
		gfx.color = Visuals.Singleton.ConvertNoteColorToColor( currentColor );
	}

	public override void Tick () {
		var nc = colorsToChangeTo[ this.grid.BeatsTicked % colorsToChangeTo.Length ];
		if( lockTimer > 0 ) {
			lockTimer--;
			currentColor = nc;
			return;
		}

		if( nc != currentColor ) {
			Pulse();
			currentColor = nc;
			gfx.color = Visuals.Singleton.ConvertNoteColorToColor( currentColor );
		}
	}
}
