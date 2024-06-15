using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Character
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        CharacterManager _character;

        int vertical;
        int horizontal;

        protected virtual void Awake()
        {
            _character = GetComponent<CharacterManager>();

            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
        {
            float horizontalAmount = horizontalValue;
            float verticalAmount = verticalValue;

            if (isSprinting) 
            {
                verticalAmount = 2;
            }

            _character._animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
            _character._animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);
        }

        public virtual void PlayTargetActionAnimation
            (
                string targetAnimation,
                bool isPerformingAction,
                bool applyRootMotion = true,
                bool canRotate = false,
                bool canMove = false
            )
        {
            _character._animator.applyRootMotion = applyRootMotion;
            _character._animator.CrossFade(targetAnimation, 0.0f);

            // used to stop character attempting new actions
            _character.isPerformingAction = isPerformingAction;
            _character.canMove = canMove;
            _character.canRotate = canRotate;
        }
    }
}