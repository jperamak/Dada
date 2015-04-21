using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour
{
    [SerializeField]
    private List<AudioSource> _audioClips = new List<AudioSource> { null }; //initial size 1
    public IEnumerable<AudioSource> AudioClips { get { return _audioClips; } }

    public float minPitch = 1f;
    public float maxPitch = 1f;

    public float minVolume = 1f;
    public float maxVolume = 1f;

    public ClipCyclingMode Mode = ClipCyclingMode.Random;

    private int _currentClip;

    void Start()
    {
        _audioClips = GetComponents<AudioSource>().ToList<AudioSource>();
    }

    public void PlayEffect()
    {
        switch (Mode)
        {
            case ClipCyclingMode.Single:
                PlayEffect(_audioClips.First());
                break;
            case ClipCyclingMode.InOrder:
                if (_currentClip >= _audioClips.Count)
                    _currentClip = 0;
                PlayEffect(_audioClips[_currentClip]);
                _currentClip++;
                break;
            case ClipCyclingMode.Random:
                PlayEffect(_audioClips[Random.Range(0, _audioClips.Count)]);
                break;
        }
    }

    public void Stop()
    {
        _audioClips.ForEach(c => c.Stop());
    }

    private void PlayEffect(AudioSource effect)
    {
        if ( effect != null )
        {
            effect.Play();
        }
    }   
}

public enum ClipCyclingMode
{
    Single,
    InOrder,
    Random
}

public class ReadOnlyAttribute : PropertyAttribute
{

}
