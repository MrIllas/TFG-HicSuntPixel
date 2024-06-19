using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Character
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        CharacterManager _character;
        Animator _animator;

        int vertical;
        int horizontal;

        protected virtual void Awake()
        {
            _character = GetComponent<CharacterManager>();
            _animator = GetComponent<Animator>();

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

            _animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
            _animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);
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
            _animator.applyRootMotion = applyRootMotion;
            _animator.CrossFade(targetAnimation, 0.0f);

            // used to stop character attempting new actions
            _character.isPerformingAction = isPerformingAction;
            _character.canMove = canMove;
            _character.canRotate = canRotate;
        }

        public virtual void PlayTargetAttackActionAnimation
           (
               AttackType attackType,
               string targetAnimation,
               bool isPerformingAction,
               bool applyRootMotion = true,
               bool canRotate = false,
               bool canMove = false
           )
        {
            // Keep Track of last attack performed (For combos)
            // Keep track of current attack type (light, heavy)
            // Update animation set to current weapon animation
            // Decide if out attack can be parried

            _character._characterCombatManager.currentAttackType = attackType;

            _animator.applyRootMotion = applyRootMotion;
            _animator.CrossFade(targetAnimation, 0.0f);

            // used to stop character attempting new actions
            _character.isPerformingAction = isPerformingAction;
            _character.canMove = canMove;
            _character.canRotate = canRotate;


        }
    }
}