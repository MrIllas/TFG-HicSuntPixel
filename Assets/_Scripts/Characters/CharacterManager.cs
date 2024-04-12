using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterManager : MonoBehaviour
    {
        public CharacterController _characterController;
        [HideInInspector] public Animator animator;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);

            _characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
        }

        protected void Start()
        {
            OnSpawn();
        }

        protected virtual void Update()
        {

        }

        protected virtual void OnSpawn()
        {

        }
    }
}

