using Spine.Unity;
using UnityEngine;
using System.Collections;
using System;

namespace IGS_GAME_EX
{
    /*
     * Enable時播放Spine動畫
     * byYC
    */

    public class SpineAnimationCtrl : MonoBehaviour
    {
        [SpineAnimation]
        public string m_sAnimationName = "";
     
        [SerializeField]
        protected SkeletonAnimation m_TargetSpine = null;

        [SerializeField]
        private bool m_bIsLoop = false;

        [SerializeField]
        private bool m_bOnEnableToPlay = true;

        [SerializeField]
        private float m_TimeScale = 1f;

        private Action<string> Event_OnComplete;

        private void OnEnable()
        {
            if (m_bOnEnableToPlay) 
            {
                ResetSpineAnima ();
            }
        }

        public bool IsLoop { get { return m_bIsLoop; }  set { m_bIsLoop = value; } }
        public SkeletonAnimation GetSpineAnime { get { return m_TargetSpine; } }

        private void OnDestroy()
        {
            m_TargetSpine.state.Complete -= OnComplete;
        }
        /// <summary>
        /// 開出去給需要的人Reset動畫
        /// </summary>
        public void ResetSpineAnima(float fStartPlayTime = -1)
        {
            m_TargetSpine.timeScale = m_TimeScale;
            if (m_TargetSpine.state == null)
                m_TargetSpine.Initialize(false);
            Spine.TrackEntry entry = m_TargetSpine.state.SetAnimation (0, m_sAnimationName, m_bIsLoop); 
            if (fStartPlayTime > 0)
            {
                entry.TrackTime = fStartPlayTime;
            }
            m_TargetSpine.Update(0);       //Set動畫之後Call一下Update讓Spine顯示動畫第0個frame的樣子
            if (Event_OnComplete != null)
            {
                m_TargetSpine.state.Complete -= OnComplete;
                m_TargetSpine.state.Complete += OnComplete;
            }
        }

        /// <summary>
        /// Add動畫(會在上一個動畫播完後接續下去播出)
        /// </summary>
        public void AddSpineAnima(string sAnimKey, bool bIsLoop, float fDelayTime = 0)
        {
            m_TargetSpine.timeScale = m_TimeScale;
            if (m_TargetSpine.state == null)
                m_TargetSpine.Initialize(false);
            m_TargetSpine.state.AddAnimation(0, sAnimKey, bIsLoop, fDelayTime);
        }

        /// <summary>
        /// 停止Spine動畫
        /// </summary>
        public void StopSpineAnima()
        {
            m_TargetSpine.timeScale = 0;
            if (m_TargetSpine.state == null)
                m_TargetSpine.Initialize(false);
            m_TargetSpine.Update (0);       //Set動畫之後Call一下Update讓Spine顯示動畫第0個frame的樣子
        }

        /// <summary>
        /// 確認這個動畫Key存在嗎
        /// </summary>
        public bool CheckSpineAnimaExist(string AnimName)
        {
            if (m_TargetSpine.state == null)
                m_TargetSpine.Initialize(false);
            return m_TargetSpine.Skeleton.Data.FindAnimation(AnimName) != null;
        }

        /// <summary>
        /// 取得現在動畫表演到哪個時間
        /// </summary>
        public float GetAnimationTime()
        {
            if (m_TargetSpine.state != null)
                return m_TargetSpine.state.GetCurrent(0).TrackTime;
            else
                return 0;
        }

        /// <summary>
        /// 設定這次動畫結束後會執行的CompleteEvent
        /// </summary>
        public void SetCompleteEvent(Action<string> CompleteCallback)
        {
            Event_OnComplete = CompleteCallback;
        }

        /// <summary>
        /// Assign new skeleton animation to target spine animation.
        /// </summary>
        /// <param name="targetSpine"></param>
        public void SetTargetSpine(SkeletonAnimation targetSpine)
        {
            this.m_TargetSpine = targetSpine;
        }

        private void OnComplete(Spine.TrackEntry Entry)
        {
            if (Event_OnComplete != null)
                Event_OnComplete(Entry.Animation.Name);
        }
    }
}
