public class TrashGate : GridObject {
    public int useCount;

    int remainingUses;

    public override void Init( Grid grid ) {
        base.Init( grid );
        OnStop();
    }

    public override void OnStop () {
        remainingUses = useCount;
    }

    public override void OnNoteEnter( Note note ) {
        if( useCount > 0 ) {
            useCount--;
        } else {
            grid.DestroyNote( note );
        }
        var particles = Grid.MakeNoteCollideParticles( note );
        particles.transform.position = transform.position + Grid.GetParticleOffset( note.direction, 0.15f );
        particles.transform.rotation = Grid.GetDirectionRotation( Grid.GetOppositeDirection( note.direction ) );
		Waver( note.duration * (float)grid.frequency - 0.04f );
	}
}