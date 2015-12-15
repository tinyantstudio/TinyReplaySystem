using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TinyReplay
{
    /// <summary>
    /// Tiny Replay manager for control replay system.
    /// AndyKun
    /// </summary>
    public class TinyReplayManager : MonoBehaviour
    {
        public static string replaySystemVersion = "1.0";
        // current animation replay in local.
        public List<TinyReplaySaveData> curLocalAnimationReplayData;
        // current system state.
        private ReplaySystemState mCurReplaySystemState = ReplaySystemState.None;

        private TinyReplayRecordController recordController = null;
        private TinyReplayPlayController playController = null;

        // Debug replay system information.
        public UILabel lbTimePosition;
        public UILabel lbReplaySystemMsg;

        // for character or record target template prefab.
        private GameObject mPlayerPrefab;
        private GameObject mMonsterPrefab;

        private const int entityParentBeginIndex = 10000;
        private int curEntityParentBeginIndex = 10000;
        
        public int CurEntityParentBeginIndex
        {
            get { return this.curEntityParentBeginIndex; }
            set { this.curEntityParentBeginIndex = value; }
        }

        private static TinyReplayManager instance;

        public static TinyReplayManager GetInstance
        {
            get { return instance; }
            private set { }
        }

        void Awake()
        {
            instance = this;
            this.InitTinyReplaySystem();
        }

        /// <summary>
        /// Init replay system.
        /// </summary>
        public void InitTinyReplaySystem()
        {
            this.recordController = this.GetComponent<TinyReplayRecordController>();
            this.playController = this.GetComponent<TinyReplayPlayController>();
        }


        /// <summary>
        /// start recording.
        /// </summary>
        public void StartRecording()
        {
            if(this.playController.IsReplaying)
            {
                Debug.LogWarning("system is replaying can not start record.");
                return;
            }
            StartCoroutine(this.PrepareToRecord());
        }

        private IEnumerator PrepareToRecord()
        {
            // set entity index to begin.
            this.CurEntityParentBeginIndex = entityParentBeginIndex;
            
            // load character and clear up the animation stage.
            AnimationStage.GetInstance.ClearAnimationStage();
            Transform stageRoot = AnimationStage.StageRoot.transform;

            // make sure destory and detach all child.
            yield return new WaitForEndOfFrame();

            // test add some character and record target.
            // add player to animation stage.
            GameObject player = this.GetNewPlayerTemplate();
            this.AddChildToTarget(stageRoot, player.transform, Vector3.zero);

            player = this.GetNewPlayerTemplate();
            this.AddChildToTarget(stageRoot, player.transform, new Vector3(-300, 0, 0));

            player = this.GetNewMonsterTemplate();
            this.AddChildToTarget(stageRoot, player.transform, new Vector3(300, 0, 0));

            // real start the recording.
            this.recordController.StartRecording();
        }


        /// <summary>
        /// save record animation to local file.
        /// </summary>
        public void SaveRecordToFile()
        {
            if(this.playController.IsReplaying)
            {
                Debug.LogWarning("system is replaying now can not save file.");
                return;
            }
            this.recordController.StopRecording();
        }

        /// <summary>
        /// load replay file data from local file.
        /// </summary>
        public void LoadRecordFromFile()
        {
            if(this.recordController.IsRecording)
            {
                Debug.LogWarning("system is recording now can not load replay file.");
                return;
            }
            StartCoroutine(this.PrepareToReplay());
        }

        private IEnumerator PrepareToReplay()
        {
            AnimationStage.GetInstance.ClearAnimationStage();
            yield return new WaitForEndOfFrame();
            this.playController.LoadReplayDataFromFile();
        }

        // Debug replay system.
        public void RefreshTimePosition(int timePos)
        {
            this.lbTimePosition.text = timePos.ToString();
        }

        public void OnRecordBegin(string message)
        {
            this.lbReplaySystemMsg.text = message;
        }

        public void OnRecordOver(string message)
        {
            this.lbReplaySystemMsg.text = message;
        }

        public void OnReplayBegin()
        {
            this.lbReplaySystemMsg.text = "Replaying.";
        }

        public void OnReplayEnd()
        {
            this.lbReplaySystemMsg.text = "ReplayDone.";
        }


        // some helper functions.
        private void AddChildToTarget(Transform trsParent, Transform trsTarget, Vector3 targetPos)
        {
            trsTarget.parent = trsParent;
            trsTarget.localScale = Vector3.one;
            trsTarget.localPosition = targetPos;
            trsTarget.rotation = Quaternion.identity;
        }

        private GameObject GetNewPlayerTemplate()
        {
            if (this.mPlayerPrefab == null)
                this.mPlayerPrefab = Resources.Load<GameObject>("AnimationPrefab/" + "Monster");
            return GameObject.Instantiate(this.mPlayerPrefab);
        }

        private GameObject GetNewMonsterTemplate()
        {
            if (this.mMonsterPrefab == null)
                this.mMonsterPrefab = Resources.Load<GameObject>("AnimationPrefab/" + "Player");
            return GameObject.Instantiate(this.mMonsterPrefab);
        }

        private void ClearUpTransform(Transform target)
        {
            for (int i = 0; i < target.childCount; i++)
            {
                Transform trs = target.GetChild(i);
                GameObject.Destroy(trs.gameObject);
            }
            target.DetachChildren();
        }
    }
}
