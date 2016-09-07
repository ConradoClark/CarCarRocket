using UnityEngine;
using System.Collections;

[AddComponentMenu("Queendom-Tales/Effects/Particles/Particle Start Color Randomization Effect")]
public class ShurikenStartColorRandomization : MonoBehaviour {

    public ParticleSystem ps;
    public Color[] colors;

	void Start () {
	
	}
	
	void Update () {
        if (colors.Length <= 0) return;
        ps.startColor = colors[Random.Range(0, colors.Length)];
	}
}
