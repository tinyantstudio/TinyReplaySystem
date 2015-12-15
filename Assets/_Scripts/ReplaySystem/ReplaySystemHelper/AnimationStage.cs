using UnityEngine;
using System.Collections;


namespace TinyReplay
{
    // animation stage root.
    public class AnimationStage : MonoBehaviour
    {
        private static GameObject mGbAnimationStageRoot;

        public static GameObject StageRoot
        {
            get
            {
                if (mGbAnimationStageRoot == null)
                    Debug.LogError("@animation stage rool is null ,please check!");
                return mGbAnimationStageRoot;
            }
        }

        private static AnimationStage instance;
        public static AnimationStage GetInstance
        {
            get { return instance; }
        }
        void Awake()
        {
            mGbAnimationStageRoot = this.gameObject;
            instance = this;
        }

        public void ClearAnimationStage()
        {
            // clear up the animation stage.
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform trs = this.transform.GetChild(i);
                GameObject.Destroy(trs.gameObject);
            }
            this.transform.DetachChildren();
        }
    }
}