using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Animation_Controller  : MonoBehaviour
{
    #region Variables
    Animator m_animator;

    [Header("Animation Properties")]
    [Tooltip("动画风格修改")]
    public AnimatorOverrideController animationStyle;
    RuntimeAnimatorController m_animationOrigin;

    [Header("Animation Set Properties")]
    [Tooltip("设置状态切换预设，动画管理设置层级为该列表排序")]
    public List<Animation_Object> AnimationSets;

    Dictionary<int, Animation_Processer> m_processersDic = new Dictionary<int, Animation_Processer>();
    #endregion

    #region BuiltIn Methods
    void Awake()
    {
        InitAnimationController();
    }

    void Update()
    {
        UpdateAnimationProcess();
        UpdateAnimationStyle();
    }
    #endregion

    #region Init
    void InitAnimationController()
    {
        m_animator = GetComponent<Animator>();
        m_animationOrigin = m_animator.runtimeAnimatorController;

        // 初始化生成处理脚本
        for(int i = 0; i < AnimationSets.Count; i++) 
        {
            Animation_Processer tmpProcesser = new Animation_Processer();
            // 注册脚本
            tmpProcesser.Init();
            m_processersDic.Add(i, tmpProcesser);
            tmpProcesser.Register(m_animator, AnimationSets[i], i);
        }
    }
    #endregion

    #region Update Animation Process
    void UpdateAnimationProcess()
    {
        for(int i = 0;i < m_processersDic.Count; i++) 
        {
            m_processersDic[i].ProcessAnimation();
        }
    }

    void UpdateAnimationStyle()
    {
        if (animationStyle)
            m_animator.runtimeAnimatorController = animationStyle;
        else
            m_animator.runtimeAnimatorController = m_animationOrigin;
    }
    #endregion

    #region Play Methods
    /// <summary>
    /// 播放状态动画(Update)
    /// </summary>
    /// <param name="animationName"></param>
    public void PlayStateAnimation(string animationName, int layer)
    {
        m_processersDic[layer].PlayStateAnimation(animationName);
    }

    /// <summary>
    /// 播放插入动画(不会改变状态动画)(Trigger)
    /// </summary>
    /// <param name="playableIndex"></param>
    public void PlayPlayableAnimation(string animationName, int layer)
    {
        m_processersDic[layer].PlayPlayableAnimation(animationName);
    }
    #endregion

    #region Animation Parameter Handle Methods
    
    #endregion
}
