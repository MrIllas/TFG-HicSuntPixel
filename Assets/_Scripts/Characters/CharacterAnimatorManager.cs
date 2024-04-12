using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Character
{
    public class CharacterAnimatorManager : MonoBehaviour
    {

        CharacterManager _character;

        protected virtual void Awake()
        {
            _character = GetComponent<CharacterManager>();
        }

        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
        {
            _character.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
            _character.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
        }
    }
}