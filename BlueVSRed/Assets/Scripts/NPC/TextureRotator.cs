using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TextureRotator : MonoBehaviour {
    public Material material;

    [Range(-1.0f, 1.0f)]
    public float offset = 0.0f;

	// Use this for initialization
	void Start () {
        if (material == null)
        {
            material = GetComponentInChildren<Material>();
        }
	}
	// Update is called once per frame
	void Update () {
        material.mainTextureOffset = new Vector2(Time.time, offset);
	}
}
