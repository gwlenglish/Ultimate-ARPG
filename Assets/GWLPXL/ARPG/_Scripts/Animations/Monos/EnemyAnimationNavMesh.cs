
using GWLPXL.ARPGCore.DebugHelpers.com;
using System.Collections;
using UnityEngine;

namespace GWLPXL.ARPGCore.Animations.com
{
    /// <summary>
    /// not used anymore in the new system, deprecated. Will delete in future versions
    /// </summary>
    public class EnemyAnimationNavMesh : MonoBehaviour, IAnimate
    {
        [SerializeField]
        protected Animator animator;
        protected bool delay = false;
        [SerializeField]
        float additionalCooldown = 1f;
        [SerializeField]
        string isHurt = "IsHurt";
        [SerializeField]
        string IsDead = "IsDead";
        [SerializeField]
        string abilityIndex = "AbilityIndex";
        [SerializeField]
        string basicattackIndex = "BasicAttackIndex";
        //[SerializeField]
        //string isLooping = "IsLooping";
        [SerializeField]
        string Movement = "Movement";

        float animatorSpeed = 1f;

        void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            if (animator == null)
            {
                Debug.LogError(this.gameObject + " needs an Animator Component in order to animate....");
                return;
            }
           
        }


        public void SetDead(bool isDead)
        {
            if (animator == null) return;
            animator.SetBool(IsDead, isDead);
        }

        public void TriggerAbilityAnimation(string trigger, int index, bool canLoop)
        {
            if (animator == null) return;

            //ARPGDebugger.DebugMessage("Enemy animations called with trigger and index " + trigger + " " + index, this);
            animator.SetTrigger(trigger);
            animator.SetInteger(abilityIndex, index);
        }



        public float GetCurrentAnimationLength()
        {
            if (animator == null) return 0f;

            AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
            if (clips == null || clips.Length == 0)
            {
                return 0;
            }
            float length = clips[0].clip.length;
            AnimatorTransitionInfo transinfo = animator.GetAnimatorTransitionInfo(0);
            float translength = transinfo.duration;
            length = length + translength;
            return length;
        }

        public void SetMovement(float movement)
        {
            if (animator == null) return;

            animator.SetFloat(Movement, movement);
        }

        /// <summary>
        /// deprecated
        /// </summary>
        /// <returns></returns>
        public bool GetDelay()
        {
            if (animator == null) return false;

            return delay;
        }

        /// <summary>
        /// no longer used, deprecated
        /// </summary>
        public void DelayAnimation()
        {
            StartCoroutine(AnimationDelay());
        }
        IEnumerator AnimationDelay()
        {
            delay = true;
            yield return null;
            while (animator.GetCurrentAnimatorStateInfo(0).IsName(Movement) == false)
            {
                yield return null;
            }
            yield return new WaitForSeconds(additionalCooldown);
            delay = false;



        }

        public Animator GetAnimator()
        {
            return animator;
        }

        public void SetHurt(bool _isHurt)
        {
            if (animator == null) return;

            animator.SetBool(isHurt, _isHurt);
        }

        public void TriggerBasicAttackAnimation(string trigger, int index, bool canLoop)
        {
            animator.SetTrigger(trigger);
            animator.SetInteger(basicattackIndex, index);
            SetLooping(canLoop);
        }

        public void SetLooping(bool isLooping)
        {
         //
        }
        public void SetAnimatorSpeed(float newvalue)
        {
            animator.speed = newvalue;
        }

        public void SetBasicAttackIndex(int newIndex)
        {
            animator.SetInteger(basicattackIndex, newIndex);
        }
    }
}