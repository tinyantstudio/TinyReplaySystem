using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TinyReplay
{
    /// <summary>
    /// Replay entity for record and replay.
    /// </summary>
    public class TinyReplayEntity
    {
        // prefab name
        // if entity is parent just load prefab.
        // if not template name is the gameobject name in parent prefab.
        public string mTemplateName;
        // entity index(index = 1000 is the parent object.)
        public int entityIndex;
        // entity target.
        protected GameObject entityTarget;

        // for SynchronizeProperties.
        protected Queue<TinyReplayObjectState> mReadyToReplayData;
        protected Stack<TinyReplayObjectState> mActiveReplayDataStackPool;

        // for GC optimize.
        protected Color mCacheColor = Color.white;
        protected Vector3 mCacheForPosition = new Vector3(100000.0f, 0f, 0f);
        protected Vector3 mCacheForRotation = new Vector3(100000.0f, 0f, 0f);
        protected Transform mTrs;
        protected TinyReplayObjectState mCacheState = new TinyReplayObjectState();

        public virtual string GetReplayEntityType()
        {
            return "ReplayEntityBase";
        }

        // init before insert new state to entity.
        public virtual void InitReplayEntity(GameObject obj, string templateName, int entityIndex)
        {
            this.entityTarget = obj;
            this.mTrs = this.entityTarget.transform;
            this.mTemplateName = templateName;
            this.entityIndex = entityIndex;
            Debug.Log("@index is " + entityIndex);
        }

        // prepare entity for replay. 
        // find gameobject target 
        // set UISprite or UITexture and other Compontent.
        public virtual void PrepareForReplay(GameObject gb)
        {
            this.entityTarget = gb;
            this.mTrs = this.entityTarget.transform;

            // init for replay data.
            if (this.mActiveReplayDataStackPool == null)
                this.mActiveReplayDataStackPool = new Stack<TinyReplayObjectState>();
            if (this.mReadyToReplayData == null)
                this.mReadyToReplayData = new Queue<TinyReplayObjectState>();
            this.mActiveReplayDataStackPool.Clear();
            this.mReadyToReplayData.Clear();

            if (gb == null)
                Debug.LogError("@error for preparefor replay game object.");
        }

        protected virtual int GetChangePropertiesType()
        {
            int saveType = 0;
            if (!this.mCacheForPosition.Equals(this.mTrs.localPosition))
            {
                this.mCacheForPosition = this.mTrs.localPosition;
                saveType |= (1 << ((int)SaveTargetPropertiesType.Position));
            }
            if (!this.mCacheForRotation.Equals(this.mTrs.localEulerAngles))
            {
                this.mCacheForRotation = this.mTrs.localEulerAngles;
                saveType |= (1 << (int)SaveTargetPropertiesType.Rotation);
            }
            return saveType;
        }
        // insert save entity state
        public void InsertNewEntityState(int timePos)
        {
            SaveReplayEntityState(timePos);
        }

        protected virtual void SaveReplayEntityState(int timePos)
        {
            this.mCacheState.ResetState();
            int saveType = this.GetChangePropertiesType();
            string saveStateData = TinyReplayObjectState.SaveCurStateProperties(this.entityIndex,
                timePos,
                mTrs,
                null,
                saveType);

            if (saveType != 0)
                TinyReplayRecordController.instance.SaveDataToLocalFile(saveStateData);
        }

        // synchronize entity state with local save file.
        public void SynchronizeEntity(int timePosition)
        {
            if (this.mReadyToReplayData.Count <= 0)
                return;
            TinyReplayObjectState curState = this.mReadyToReplayData.Peek();
            // if state in the deque's data timePosition is equal to timePosition just synchronize entity.
            if (curState.TimePos == timePosition)
            {
                curState = this.mReadyToReplayData.Dequeue();
                this.RealSynchronizeEntity(curState);
                this.mActiveReplayDataStackPool.Push(curState);
            }
        }

        protected virtual void RealSynchronizeEntity(TinyReplayObjectState newState)
        {
            newState.SynchronizeProperties(this.mTrs);
        }

        // parsing state data push to state deque.
        public void ParsingStateData(int timePos, string parsingData)
        {
            TinyReplayObjectState newState = this.GetUnUsedState();
            newState.ParsingProperties(this.entityIndex, timePos, parsingData);
            this.mReadyToReplayData.Enqueue(newState);
            // Debug.Log("   first time?");
        }

        private TinyReplayObjectState GetUnUsedState()
        {
            TinyReplayObjectState objectStateData = null;
            if (this.mActiveReplayDataStackPool.Count > 0)
                objectStateData = this.mActiveReplayDataStackPool.Pop();
            else
                objectStateData = new TinyReplayObjectState();
            return objectStateData;
        }
    }

    public class TinyReplayPlayerEntity : TinyReplayEntity
    {
        public string splineJson = string.Empty;
        public string splineAni = string.Empty;
        public string splineAsset = string.Empty;

        public string textureURL = string.Empty;

        private UITexture mTexture;
        public override string GetReplayEntityType()
        {
            return "ReplayPlayerEntity";
        }

        // init before insert new state to entity.
        public override void InitReplayEntity(GameObject obj, string templateName, int entityIndex)
        {
            base.InitReplayEntity(obj, templateName, entityIndex);
            this.mTexture = obj.GetComponent<UITexture>();
            if (this.mTexture == null)
                Debug.Log(" entity UITexture is null.");
        }

        // prepare entity for replay.
        // find gameobject target 
        // set UISprite or UITexture and other Compontent.
        public override void PrepareForReplay(GameObject obj)
        {
            base.PrepareForReplay(obj);
            this.mTexture = obj.GetComponent<UITexture>();
            if (this.mTexture == null)
                Debug.LogError("@replay player entity texture is null.");
        }

        protected override void SaveReplayEntityState(int timePos)
        {
            this.mCacheState.ResetState();
            int saveType = this.GetChangePropertiesType();
            if (!this.mTexture.color.Equals(this.mCacheColor))
            {
                this.mCacheColor = this.mTexture.color;
                saveType |= (1 << (int)SaveTargetPropertiesType.Color);
            }
            string saveStateData = TinyReplayObjectState.SaveCurStateProperties(this.entityIndex,
                timePos,
                mTrs,
                this.mTexture,
                saveType);

            if (saveType != 0)
                TinyReplayRecordController.instance.SaveDataToLocalFile(saveStateData);
        }

        protected override void RealSynchronizeEntity(TinyReplayObjectState newState)
        {
            base.RealSynchronizeEntity(newState);
            newState.SynchronizeColorProperties(this.mTexture, this.mCacheColor);
        }
    }
}