using UnityEngine;
using System.Collections;

public class TornadoEffect : MonoBehaviour {
    [Range(-1000.0f, 1000.0f)]public float Speed = 50.0f;

    void Start()
    {
        parent = transform.parent;
    }

    Transform parent;

	void Update () {
        transform.Rotate(parent.up, Time.deltaTime * Speed);
	}
}
