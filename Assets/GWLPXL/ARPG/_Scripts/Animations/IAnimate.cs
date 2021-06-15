using UnityEngine;
namespace GWLPXL.ARPGCore.Animations.com
{


    /// <summary>
    /// not used anymore in the new system, deprecated. Will delete in future versions
    /// </summary>
    public interface IAnimate
    {
        Animator GetAnimator();
        void SetAnimatorSpeed(float newavalue);
        void SetDead(bool isDead);
        void SetHurt(bool isHurt);
        void SetLooping(bool isLooping);
        void SetBasicAttackIndex(int newIndex);
        void TriggerAbilityAnimation(string trigger, int index, bool canLoop);
        void TriggerBasicAttackAnimation(string trigger, int index, bool canLoop);

        float GetCurrentAnimationLength();
        void SetMovement(float movement);
        bool GetDelay();
        void DelayAnimation();

    }
}