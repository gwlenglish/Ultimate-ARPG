
using GWLPXL.ARPGCore.com;

using System.Collections;
using UnityEngine;

namespace GWLPXL.ARPGCore.Animations.com
{
    /// <summary>
    /// not used anymore in the new system, deprecated. Will delete in future versions
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimations : MonoBehaviour, IAnimate, ITick
    {

        #region fields
        Animator animator = null;
        bool delay = false;
        [SerializeField]
        string isHurt = "IsHurt";
        [SerializeField]
        string IsDead = "IsDead";
        [SerializeField]
        string abilityIndex = "AbilityIndex";
        [SerializeField]
        string basicattackIndex = "BasicAttackIndex";
        [SerializeField]
        string isLooping = "IsLooping";
        [SerializeField]
        string Movement = "Movement";

        int normalizedSpeed = 1;
        float animatorSpeed = 1;
        #endregion

        IActorHub mover = null;

      

        #region private calls
        void Awake()
        {
            animator = GetComponent<Animator>();

            mover = transform.root.GetComponent<IActorHub>();

        }
        #endregion


        #region public
        public void TriggerBasicAttackAnimation(string trigger, int index, bool canLoop)
        {
            animator.SetTrigger(trigger);
            animator.SetInteger(basicattackIndex, index);
            SetLooping(canLoop);
        }
        public void TriggerAbilityAnimation(string trigger, int index, bool canLoop)
        {
            animator.SetTrigger(trigger);
            animator.SetInteger(abilityIndex, index);
            SetLooping(canLoop);
        }

        public void SetLooping(bool isLooping)
        {
            animator.SetBool(this.isLooping, isLooping);
        }
        public Animator GetAnimator()
        {
            return animator;
        }
        public void SetHurt(bool _isHurt)
        {
            animator.SetBool(isHurt, _isHurt);
        }
        public void SetDead(bool isDead)
        {

            animator.SetBool(IsDead, isDead);
        }


        public float GetCurrentAnimationLength()
        {


            AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
            if (clips == null || clips.Length == 0)
            {
                return 0;
            }
            float length = clips[0].clip.length;
            AnimatorTransitionInfo transinfo = animator.GetAnimatorTransitionInfo(0);
            float translength = transinfo.duration;
            length = length + translength;
            //ARPGDebugger.DebugMessage("Animator delay length: " + length);
            return length;
        }

        public void SetMovement(float movement)
        {

            animator.SetFloat(Movement, movement);
        }

        public bool GetDelay()
        {

            return delay;
        }

        public void DelayAnimation()
        {
            StartCoroutine(AnimationDelay());
        }
        #endregion

        #region coroutines
        IEnumerator AnimationDelay()
        {
            delay = true;
            yield return null;
            while (animator.GetCurrentAnimatorStateInfo(0).IsName(Movement) == false)
            {
                yield return null;
            }
            delay = false;
        }

        public void SetAnimatorSpeed(float newvalue)
        {
            animator.speed = newvalue;
            ARPGCore.DebugHelpers.com.ARPGDebugger.DebugMessage("Animator speed " + animator.speed, this);
        }

        public void AddTicker() => TickManager.Instance.AddTicker(this);

       
        public void DoTick()//helps sync the animator to the agent
        {
            //a source of great consternation
            Vector3 worldDeltaPosition = mover.MyTransform.position - transform.position;
            // Pull character towards agent
            if (worldDeltaPosition.magnitude > .01f)
            {
                transform.position = mover.MyTransform.position - 0.9f * worldDeltaPosition;

            }
           // SetLooping(mouseInput.GetMouseButtonOneDown());
            //     transform.rotation = Quaternion.Slerp(navmeshmover.GetAgent().transform.rotation, this.transform.localRotation, Time.deltaTime);
        }

        public void RemoveTicker() => TickManager.Instance.RemoveTicker(this);


        public float GetTickDuration() => Time.deltaTime;

        public void SetBasicAttackIndex(int newIndex)
        {
            animator.SetInteger(basicattackIndex, newIndex);
        }


        #endregion
    }
}