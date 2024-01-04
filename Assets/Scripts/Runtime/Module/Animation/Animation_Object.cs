using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationSet", menuName = "BoyInABox/AnimationSet")]
public class Animation_Object : ScriptableObject
{
    #region Variables
    [Tooltip("默认状态切换过渡时间")]
    public float animationGlobalTransitionDuration = 0.25f;

    [Header("State Animation Blend Properties")]
    [Tooltip("设置状态切换过渡")]
    public List<StateAnimationBlend> stateAnimationBlends = new List<StateAnimationBlend>();

    [Header("Playable Animation Properties")]
    [Tooltip("设置可播放动画")]
    public List<PlayableAnimation> playableAnimations = new List<PlayableAnimation>();
    #endregion

    #region Properties
    [System.Serializable]
    public class StateAnimationBlend
    {
        [Tooltip("过渡名称")]
        public string blendName;

        [Header("From Animation Properties")]
        [Tooltip("由动画切换")]
        public string fromAnimation;

        [Tooltip("由动画切换过渡时间")]
        public float fromBlendAnimationTransitionDuration = 0.25f;

        [Tooltip("由动画切换偏移量")]
        public float fromBlendAnimationTransitionOffset = 0f;

        [Header("Blend Animation Properties")]
        [Tooltip("设置动画切换过渡动画名称。不能为空！当被设置为'Null'时，仅修改由动画切换的过渡时间(fromBlendAnimationTransitionDuration)以及由动画切换偏移量(fromBlendAnimationTransitionOffset)")]
        public string blendAnimation = "Null";

        [Header("To Animation Properties")]
        [Tooltip("到动画切换")]
        public string toAnimation;

        [Tooltip("到动画切换过渡时间")]
        public float toBlendAnimationTransitionDuration = 0.25f;

        [Tooltip("到动画切换偏移量")]
        public float toBlendAnimationTransitionOffset = 0f;
    }

    [System.Serializable]
    public class PlayableAnimation
    {
        [Tooltip("可播放动画名称")]
        public string playableName;

        [Tooltip("动画播放优先级")]
        public int animationPriority = 0;

        [Header("From Animation Properties")]
        [Tooltip("由动画切换过渡时间")]
        public float fromPlayableAnimationTransitionDuration = 0.25f;

        [Tooltip("由动画切换偏移量")]
        public float fromPlayableAnimationTransitionOffset = 0f;

        [Header("Blend Animation Properties")]
        [Tooltip("播放动画名称")]
        public string animationName;

        [Header("To Animation Properties")]
        [Tooltip("到动画切换过渡时间")]
        public float toPlayableAnimationTransitionDuration = 0.25f;

        [Tooltip("到动画切换偏移量")]
        public float toPlayableAnimationTransitionOffset = 0f;
    }
    #endregion
}
