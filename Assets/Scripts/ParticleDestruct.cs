using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestruct : MonoBehaviour {
	public ParticleSystem[] systems;
	public float countdown;

	void Update (){
		countdown -= Time.deltaTime;
		if( countdown < 0 ) {
			foreach( var system in systems ) {
				system.Stop( false, ParticleSystemStopBehavior.StopEmitting );
			}
			Invoke( "Teardown", 0.1f );
		}
	}

	public void SetColor ( NoteColor noteColor ) {
		var color = Visuals.Singleton.ConvertNoteColorToColor( noteColor );
		foreach( var system in systems ) {
			var main = system.main;
			var startColor = system.main.startColor;
			startColor.colorMin = color;
			color.a = 0.15f;
			startColor.colorMax = color;
			main.startColor = startColor;

			var particles = new ParticleSystem.Particle[ system.particleCount ];
			system.GetParticles( particles );
			for( int i = 0; i < particles.Length; i++ ) {
				var particle = particles[ i ];
				particle.startColor = Color.Lerp( startColor.colorMin, startColor.colorMax, Random.value );
				particles[ i ] = particle;
			}
			system.SetParticles( particles, particles.Length );
		}
	}

	void Teardown () {
		Destroy( gameObject );
	}
}
