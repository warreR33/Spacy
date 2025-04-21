using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] damageClips;
    public AudioClip[] deathClips;
    public Vector2 damagePitchRange = new Vector2(0.9f, 1.1f);

    public void PlayDamageSound()
    {
        if (damageClips.Length == 0 || audioSource == null) return;
        audioSource.pitch = Random.Range(damagePitchRange.x, damagePitchRange.y);
        audioSource.PlayOneShot(damageClips[Random.Range(0, damageClips.Length)]);
    }

    public IEnumerator PlayDeathSequence(System.Action onEnd)
    {
        if (deathClips.Length > 0 && audioSource != null)
        {
            audioSource.pitch = 1f;
            audioSource.clip = deathClips[Random.Range(0, deathClips.Length)];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }

        onEnd?.Invoke();
    }
}
