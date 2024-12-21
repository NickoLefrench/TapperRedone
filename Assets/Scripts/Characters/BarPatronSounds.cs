using FMS.TapperRedone.Interactables;

using UnityEngine;

namespace FMS.TapperRedone.Characters
{
    [RequireComponent(typeof(AudioSource))]
    public class BarPatronSounds : InteractableObject
    {
        public enum SoundType
        {
            HappyOrder,
            SadOrder,
        }

        [SerializeField] private AudioClip happyOrderClip;
        [SerializeField] private AudioClip sadOrderClip;

        private AudioSource _audioComponent;

        public void Start()
        {
            _audioComponent = GetComponent<AudioSource>();
        }

        public void PlaySound(SoundType soundType)
        {
            switch (soundType)
            {
            case SoundType.HappyOrder:
                PlaySoundInternal(happyOrderClip);
                break;
            case SoundType.SadOrder:
                PlaySoundInternal(sadOrderClip);
                break;
            }
        }

        private void PlaySoundInternal(AudioClip audioClip)
        {
            _audioComponent.Stop();
            _audioComponent.clip = audioClip;
            _audioComponent.Play();
        }
    }
}
