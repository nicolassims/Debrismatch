using System;
using UnityEngine;

namespace Sounds
{
    [CreateAssetMenu(menuName = "SoundState")]
    public class SoundState : ScriptableObject
    {
        [SerializeField] public AudioClip intro;
        [SerializeField] public AudioClip[] mainTracks;
        [SerializeField] public AudioClip[] layers;
        [SerializeField] public float songBpm;
        [SerializeField] public float[] startTracksAtTimes;
        [SerializeField] public int startOnTrack = 0;
        [SerializeField] public bool loop;
        [SerializeField] public float targetVolume;
        [SerializeField] public TransitionPair[] transitions;

        public float SecPerBeat()
        {
            return 60f / songBpm;
        }

        public SoundState GetTransitionDestination(string tName)
        {
            foreach (TransitionPair p in transitions)
            {
                if (p.name.Equals(tName))
                {
                    return p.nextState;
                }
            }

            throw new ArgumentException("No transition found named " + tName);
        }
    }

    // SoundStates that can be transitioned to from this state
    [Serializable]
    public struct TransitionPair
    {
        [SerializeField] public string name;
        [SerializeField] public SoundState nextState;
    }
}
