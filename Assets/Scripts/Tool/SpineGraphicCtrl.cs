using Spine.Unity;
using UnityEngine;
using System.Collections;
using System;

namespace IGS_GAME_EX
{
    public class SpineGraphicCtrl : MonoBehaviour
    {
        [SerializeField]
        protected SkeletonGraphic m_TargetSpine = null;

        [SpineAnimation]
        public string m_sAnimationName = "";

        [SerializeField]
        private bool m_bIsLoop = false;

        [SerializeField]
        private bool m_bOnEnableToPlay = true;

        [SerializeField]
        private float m_TimeScale = 1f;

        private Action<string> Event_OnComplete;
        private Action<string, string> Event_OnSpineEvent;

        private void OnEnable()
        {
            if (m_bOnEnableToPlay)
            {
                ResetSpineAnima();
            }
        }

        public bool IsLoop { get { return m_bIsLoop; } set { m_bIsLoop = value; } }
        public SkeletonGraphic GetSpineGraphic { get { return m_TargetSpine; } }

        private void OnDestroy()
        {
            m_TargetSpine.AnimationState.Complete -= OnComplete;
        }

        /// <summary>
        /// 開出去給需要的人Reset動畫
        /// </summary>
        public void ResetSpineAnima(float fStartPlayTime = -1)
        {
            m_TargetSpine.timeScale = m_TimeScale;
            if (m_TargetSpine.AnimationState == null)
                m_TargetSpine.Initialize(false);
            Spine.TrackEntry entry = m_TargetSpine.AnimationState.SetAnimation(0, m_sAnimationName, m_bIsLoop);
            if (fStartPlayTime > 0)
            {
                entry.TrackTime = fStartPlayTime;
            }
            m_TargetSpine.Update(0);       //Set動畫之後Call一下Update讓Spine顯示動畫第0個frame的樣子
        }

        /// <summary>
        /// Add動畫(會在上一個動畫播完後接續下去播出)
        /// </summary>
        public void AddSpineAnima(string sAnimKey, bool bIsLoop, float fDelayTime = 0)
        {
            m_TargetSpine.timeScale = m_TimeScale;
            if (m_TargetSpine.AnimationState == null)
                m_TargetSpine.Initialize(false);
            m_TargetSpine.AnimationState.AddAnimation(0, sAnimKey, bIsLoop, fDelayTime);
        }

        /// <summary>
        /// 停止Spine動畫
        /// </summary>
        public void StopSpineAnima()
        {
            if (m_TargetSpine.AnimationState != null)
                m_TargetSpine.AnimationState.SetEmptyAnimation(0, 0);
            //m_TargetSpine.AnimationState.ClearTrack(0);
        }

        /// <summary>
        /// 確認這個動畫Key存在嗎
        /// </summary>
        public bool CheckSpineAnimaExist(string AnimName)
        {
            if (m_TargetSpine.AnimationState == null)
                m_TargetSpine.Initialize(false);
            return m_TargetSpine.Skeleton.Data.FindAnimation(AnimName) != null;
        }

        /// <summary>
        /// 取得現在動畫表演到哪個時間
        /// </summary>
        public float GetAnimationTime()
        {
            if (m_TargetSpine.AnimationState != null)
                return m_TargetSpine.AnimationState.GetCurrent(0).TrackTime;
            else
                return 0;
        }

        public void SetSkin(string skinName)
        {
            m_TargetSpine.Skeleton.SetSkin(skinName);
        }

        /// <summary>
        /// 設定這次動畫結束後會執行的CompleteEvent
        /// </summary>
        public void SetCompleteEvent(Action<string> CompleteCallback)
        {
            Event_OnComplete = CompleteCallback;
            if (Event_OnComplete != null)
            {
                m_TargetSpine.AnimationState.Complete -= OnComplete;
                m_TargetSpine.AnimationState.Complete += OnComplete;
            }
            else
            {
                m_TargetSpine.AnimationState.Complete -= OnComplete;
            }
        }
        /// <summary>
        /// 設定這次動畫中途有設Key Event需要回傳
        /// </summary>
        public void SetSpineKeyEvent(Action<string, string> Callback)
        {
            Event_OnSpineEvent = Callback;
            if (Event_OnSpineEvent != null)
            {
                m_TargetSpine.AnimationState.Event -= OnSpineKeyEvent;
                m_TargetSpine.AnimationState.Event += OnSpineKeyEvent;
            }
            else
            {
                m_TargetSpine.AnimationState.Event -= OnSpineKeyEvent;
            }
        }

        private void OnComplete(Spine.TrackEntry Entry)
        {
            if (Event_OnComplete != null)
                Event_OnComplete(Entry.Animation.Name);
        }

        private void OnSpineKeyEvent(Spine.TrackEntry Entry, Spine.Event e)
        {
            if (Event_OnSpineEvent != null)
                Event_OnSpineEvent(Entry.Animation.Name, e.Data.Name);
        }
    }
}