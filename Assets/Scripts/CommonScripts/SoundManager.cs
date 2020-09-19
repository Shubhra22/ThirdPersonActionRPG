/*
Author: Shubhra Sarker
Company: JoystickLab
Email: ssuvro22@outlook.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoystickLab
{
    public class SoundManager : Manager<SoundManager>
    {
        #region Variables

        [SerializeField] private AudioClip[] footStepClips;
        [SerializeField] private AudioSource playerAudioSource;
        #endregion
        
        #region MainMethods
        // Start is called before the first frame update
        void Start()
        {
            
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
        #endregion
        
        #region CustomMethods

        public void SetFootSound()
        {
            int random = Random.Range(0, footStepClips.Length);
            playerAudioSource.PlayOneShot(footStepClips[random]);
        }
        #endregion
    }
}