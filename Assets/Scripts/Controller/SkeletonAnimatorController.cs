using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAnimatorController : MonoBehaviour
{

    private SkeletonAnimation skeletonAnimation;
    // Start is called before the first frame update
    void Start()
    {
        skeletonAnimation= GetComponent<SkeletonAnimation>();
        var animationState = skeletonAnimation.AnimationState;
        animationState.Complete += OnSpineAnimationComplete;//������ɺ�Ļص�
    }

    public void Eat()
    {
        skeletonAnimation.AnimationState.SetAnimation(0,"eat",false);
    }

    private void OnSpineAnimationComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "eat")//break�¼���
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
        }
    }
}
