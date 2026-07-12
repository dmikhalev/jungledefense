using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SoundLibrary",
    menuName = "Jungle Defense/Audio/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [Serializable]
    public class SoundEntry
    {
        public SoundType type;
        public AudioChannel channel = AudioChannel.World;

        [Tooltip("One clip is enough. If several are assigned, one is selected randomly without immediate repetition.")]
        public AudioClip[] clips;

        [Range(0f, 1f)]
        public float volume = 1f;

        [Tooltip("Additional random volume multiplier applied each time the sound is played.")]
        public Vector2 volumeRange = Vector2.one;

        [Tooltip("Random pitch range applied each time the sound is played.")]
        public Vector2 pitchRange = Vector2.one;

        [Tooltip("Minimum time in seconds before this sound type can be played again. Useful for rapid tower shots.")]
        [Min(0f)]
        public float minimumInterval;

        public bool loop;
    }

    [SerializeField] private SoundEntry[] sounds;

    public bool TryGet(SoundType type, out SoundEntry entry)
    {
        if (sounds != null)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                SoundEntry candidate = sounds[i];

                if (candidate != null && candidate.type == type)
                {
                    entry = candidate;
                    return true;
                }
            }
        }

        entry = null;
        return false;
    }
}
