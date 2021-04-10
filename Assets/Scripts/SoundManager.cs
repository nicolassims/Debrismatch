using System;
using System.Collections;
using System.Collections.Generic;
using Sounds;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;

// This behavior keeps track of sounds to play
// and music to loop simultaneously.
public class SoundManager : MonoBehaviour
{
    // public constants
    public const float defaultVolume = 0.6f;
    
    // constant indices to use when accessing sounds/channels
    private const int SFX = 0;
    private const int BGM = 1;
    private const int LAYER = 2;
    private static readonly int[] SNDS = new[] {SFX, BGM, LAYER};

    // struct to store settings per channel
    [Serializable]
    public struct ChannelSettings
    {
        [SerializeField] public bool shouldPlay;
        [SerializeField] public bool loop;
        [SerializeField] public float volume;
    }

    // sounds to play
    public SoundState soundState;
    
    // sources thru which to play sounds
    [SerializeField] private AudioSource[] sfxChannels;
    [SerializeField] private AudioSource[] bgmChannels; 
    [SerializeField] private AudioSource[] layerChannels; 

    // internal state
    [FormerlySerializedAs("_currentBgmChannel")] [SerializeField] private int currentBgmChannel;
    //private SoundStateTransition _transition = SoundStateTransition.Crossfade;
    private string _transitionToTake = "";
    public bool shouldPlay;
    [SerializeField] private bool[] layersShouldPlay;
    
    // timing
    public float songPosition; // current position in seconds
    public float songPositionInBeats; // current position in beats
    public float dspSongTime; // how many seconds have passed since the song began
    private bool _onBeat = false;
    [FormerlySerializedAs("transitionTime")] public float fadeTime; // secs it takes for a sound to fade in/out

    // Start is called before the first frame update
    void Start()
    {
        // Ensure required channels are linked
        if (sfxChannels.Length < 2 || bgmChannels.Length < 2)
        {
            throw new Exception("Missing required audio channels");
        }

        layersShouldPlay = new bool[layerChannels.Length];
    }
    
    /**
     * Returns a new array which represents the given array
     * extended to a given size.
     * New indices are filled by calling a given function.
     */
    private static T[] ExtendArray<T>(T[] array, int targetSize, Func<T> defaultValue)
    {
        var newArray = new T[targetSize];
        for (var i = 0; i < targetSize; i += 1)
        {
            if (i < array.Length)
            {
                newArray[i] = array[i];
            }
            else
            {
                newArray[i] = defaultValue();
            }
        }

        return newArray;
    }

    // Update is called once per frame
    void Update()
    {
        if (soundState != null)
        {
            if (!GetCurrentBgmChannel().isPlaying)
            {
                PerformTransition("NEXT");
            }
            UpdateBgmVolumes();
            UpdateLayerVolumes();
            UpdateSongPosition();
            if (_onBeat)
            {
                Debug.Log("beat " + songPositionInBeats);
                // Transition if needed
                if (!_transitionToTake.Equals(""))
                {
                    PerformTransition(_transitionToTake);
                }
            }
        }
    }

    private void PerformTransition(string tName)
    {
        Debug.Log("Transition to " + tName + " now!");
        SoundState nextState = soundState.GetTransitionDestination(tName);
        LoadSoundState(nextState);
        _transitionToTake = ""; // reset transition
    }

    /*
     * Updates all bgm channels to reflect where they should be
     * regarding volume and current playing sounds.
     */
    private void UpdateBgmVolumes()
    {
        // Change volume towards target volumes for channels
        for (var i = 0; i < bgmChannels.Length; i += 1)
        {
            var channel = bgmChannels[i];
            if (!channel.isPlaying)
            {
                continue;
            }
            if (i == currentBgmChannel && shouldPlay)
            {
                // bring active channel towards target volume
                if (channel.volume < soundState.targetVolume)
                {
                    channel.volume = Mathf.Clamp(channel.volume + TransitionStep(), 0, soundState.targetVolume);
                }
            }
            else
            {
                // bring inactive channel towards 0
                if (channel.volume > 0)
                {
                    channel.volume = Mathf.Clamp(channel.volume - TransitionStep(), 0, soundState.targetVolume);
                }
            }
            // stop channel if at 0 volume and shouldn't play
            if (channel.volume == 0 && !shouldPlay)
            {
                channel.Stop();
            }
        }
    }

    /*
     * Returns the value that the volume must be changed by each step during a transition.
     */
    private float TransitionStep()
    {
        return (soundState.targetVolume / fadeTime) * Time.deltaTime;
    }

    // TODO: Implement
    private void UpdateLayerVolumes()
    {
        // Change volume towards target volumes for channels
        for (var i = 0; i < layerChannels.Length; i += 1)
        {
            var channel = layerChannels[i];
            var channelShouldPlay = layersShouldPlay[i];
            if (!channel.isPlaying)
            {
                continue;
            }
            if (channelShouldPlay)
            {
                // bring active channel towards target volume
                if (channel.volume < soundState.targetVolume)
                {
                    channel.volume = Mathf.Clamp(channel.volume + TransitionStep(), 0, soundState.targetVolume);
                }
            }
            else
            {
                // bring inactive channel towards 0
                if (channel.volume > 0)
                {
                    channel.volume = Mathf.Clamp(channel.volume - TransitionStep(), 0, soundState.targetVolume);
                }
            }
            // stop channel if at 0 volume and shouldn't play
            if (channel.volume == 0 && !shouldPlay)
            {
                channel.Stop();
            }
        }
    }

    private void RecordSongStartTime()
    {
        dspSongTime = (float) AudioSettings.dspTime;
    }

    private void UpdateSongPosition()
    {
        // Determine seconds since song began
        songPosition = (float) (AudioSettings.dspTime - dspSongTime);
        // determine beats since song began
        var newBeat = (float) Math.Floor(songPosition / soundState.SecPerBeat());
        _onBeat = Math.Abs(newBeat - songPositionInBeats) > 0.001f;
        songPositionInBeats = newBeat;
    }

    public void PlaySfx(AudioClip clip, bool loop = false, float vol = defaultVolume)
    {
        var sfxChannel = GetAvailableSfxChannel();
        sfxChannel.loop = loop;
        sfxChannel.clip = clip;
        sfxChannel.volume = vol;
        sfxChannel.Play();
    }

    public void Loop(AudioClip clip)
    {
        PlaySfx(clip, true);
    }

    // Gets the first sfx channel that isn't currently playing a sound.
    // Whether the settings say it should play is not considered.
    private AudioSource GetAvailableSfxChannel()
    {
        foreach (var channel in sfxChannels)
        {
            if (!channel.isPlaying)
            {
                return channel;
            }
        }

        return sfxChannels[0]; // returns first channel by default if all are playing
    }

    private AudioSource GetCurrentBgmChannel()
    {
        return bgmChannels[currentBgmChannel];
    }

    private void CycleCurrentBgm()
    {
        currentBgmChannel = (currentBgmChannel + 1) % bgmChannels.Length;
    }

    // TODO: Remove
    // public void StartBGM(string soundName)
    // {
    //     _sounds[BGM][_currentBgmChannel] = GetAudioClip(soundName);
    //     _settings[BGM][_currentBgmChannel].shouldPlay = true;
    // }

    // Stops a looping sfx channel.
    // If index -1 is given, stops all channels.
    public void Stop(int index)
    {
        if (index == -1)
        {
            foreach (AudioSource channel in sfxChannels)
            {
                channel.Stop();
                channel.loop = false;
            }
        }
        else
        {
            AudioSource src = sfxChannels[index];
            src.Stop();
            src.loop = false;
        }
    }

    public void LoadSoundState(SoundState state)
    {
        soundState = state;
        shouldPlay = true;
        // Stop all tracks
        StopAllBgmImmediate();
        currentBgmChannel = soundState.startOnTrack;
        RecordSongStartTime();
        
        if (soundState.intro == null)
        {
            // Load up all main tracks that can fit
            for (var i = 0; i < Math.Min(bgmChannels.Length, soundState.mainTracks.Length); i += 1)
            {
                var track = soundState.mainTracks[i];
                var channel = bgmChannels[i];
                channel.clip = track;
                channel.time = soundState.startTracksAtTimes[i];
                channel.loop = soundState.loop;
                channel.volume = (i == currentBgmChannel) ? soundState.targetVolume : 0;
                channel.Play();
            }
            // Load up all layers
            for (var i = 0; i < Math.Min(layerChannels.Length, soundState.layers.Length); i += 1)
            {
                var track = soundState.layers[i];
                var channel = layerChannels[i];
                channel.clip = track;
                channel.time = 0;
                channel.loop = soundState.loop;
                channel.volume = (layersShouldPlay[i]) ? soundState.targetVolume : 0;
                channel.Play();
            }
        }
        else
        {
            PlaySfx(soundState.intro, false, soundState.targetVolume); // one-shot the intro
            // Schedule all main tracks to play once the intro ends
            for (var i = 0; i < Math.Min(bgmChannels.Length, soundState.mainTracks.Length); i += 1)
            {
                var track = soundState.mainTracks[i];
                var channel = bgmChannels[i];
                channel.clip = track;
                channel.time = soundState.startTracksAtTimes[i];
                channel.loop = soundState.loop;
                channel.volume = (i == currentBgmChannel) ? soundState.targetVolume : 0;
                channel.PlayScheduled(dspSongTime + soundState.intro.length);
            }
            // Schedule all layers
            for (var i = 0; i < Math.Min(layerChannels.Length, soundState.layers.Length); i += 1)
            {
                var track = soundState.layers[i];
                var channel = layerChannels[i];
                channel.clip = track;
                channel.time = 0;
                channel.loop = soundState.loop;
                channel.volume = (layersShouldPlay[i]) ? soundState.targetVolume : 0;
                channel.PlayScheduled(dspSongTime + soundState.intro.length);
            }
        }
    }

    private void StopAllBgmImmediate()
    {
        foreach (AudioSource channel in bgmChannels)
        {
            channel.Stop();
        }
        foreach (AudioSource channel in layerChannels)
        {
            channel.Stop();
        }
    }

    // Causes all bgm channels to fade out.
    public void StopAllBgm()
    {
        shouldPlay = false;
    }

    public void SetMainTrack(int trackIndex)
    {
        currentBgmChannel = trackIndex;
    }

    public void SetPlayLayer(int layerIndex, bool play, bool immediate)
    {
        layersShouldPlay[layerIndex] = play;
        if (immediate)
        {
            layerChannels[layerIndex].volume = play ? soundState.targetVolume : 0;
        }
    }

    // Queues a transition to be performed on the next beat.
    // The given name should match a transition on the current SoundState.
    public void TriggerTransition(string tName)
    {
        _transitionToTake = tName;
    }
}

// Used to indicate to the SoundManager what kind of transition to do
public enum SoundStateTransition
{
    Crossfade,
    NextBeat,
    Immediate,
    DoNotTransition
}
