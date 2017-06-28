using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ParticleSystem Main;
    public ParticleSystem[] Ring;
    public SparkParticles Sparks;
    public ParticleSystem Flash;
    public ParticleSystem[] Debris;
    public float shakeDuration = 1f;
    public float magnitude = 0.2f;
    public AudioSource explosionSound;
    void Start() {
        Play();
    }

    public void Play()
    {
        Main.Play();
        for (int i = 0; i < Ring.Length; i++)
            Ring[i].Play();
        Sparks.Play();
        Flash.Play();
        for (int i = 0; i < Debris.Length; i++)
        {
            Debris[i].Play();
        }
        StartCoroutine(Shake());
        explosionSound.Play();
    }

    void FixedUpdate() {
        if (!Debris[0].IsAlive())
            Destroy(this.gameObject);
    }

    IEnumerator Shake()
    {

        float elapsed = 0.0f;

        Vector3 originalCamPos = Camera.main.transform.position;

        while (elapsed < shakeDuration)
        {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / shakeDuration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map value to [-1, 1]
            float z = Random.value * 2.0f - 1.0f;
            float x = Random.value * 2.0f - 1.0f;
            z *= magnitude * damper;
            x *= magnitude * damper;

            Camera.main.transform.position = new Vector3(x + originalCamPos.x, originalCamPos.y, z + originalCamPos.z);

            yield return null;
        }

        Camera.main.transform.position = originalCamPos;
    }
}

