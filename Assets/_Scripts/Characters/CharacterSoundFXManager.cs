using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterSoundFXManager : MonoBehaviour
    {
        AudioSource _audioSource;

        protected virtual void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayRollSoundFX()
        {
            _audioSource.PlayOneShot(WorldSoundFXManager.instance.dashSFX);
        }
    }
}

