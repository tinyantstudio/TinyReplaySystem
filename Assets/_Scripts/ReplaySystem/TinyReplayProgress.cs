using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TinyReplay
{
    /// <summary>
    /// save the current tiny replay or record progress.
    /// </summary>
    public class TinyReplayProgress
    {
        // current replay system version to select replay version.
        public string replaySystemVersion = string.Empty;
        // save all the record or replay entity. 
        public List<TinyReplayEntity> allEntity;
        // auto resource path.
        public string audioURL = string.Empty;
        // progress time interval.
        public float mPerRecordInterval = 0.5f;
        // max time position.
        public int mMaxTimePosition = 0;
        // total line in entity state file.
        public long mTotalSaveDataLineCount = 0;

        // save replay entity to dictionary
        [Newtonsoft.Json.JsonIgnoreAttribute]
        public Dictionary<int, TinyReplayEntity> allEntityDic = new Dictionary<int, TinyReplayEntity>();

        public void ResetReplayProgress()
        {
            this.replaySystemVersion = TinyReplayManager.replaySystemVersion;
            if (this.allEntity == null)
                this.allEntity = new List<TinyReplayEntity>();
            this.allEntity.Clear();
            this.allEntityDic.Clear();
            this.audioURL = string.Empty;
        }

        public void AddNewReplayEntity(ReplayObject replayObject)
        {
            TinyReplayEntity newEntity = this.CreateNewTinyReplayEntity(replayObject.entityType);
            newEntity.InitReplayEntity(replayObject.gameObject, string.Empty, replayObject.entityIndex);
            this.allEntity.Add(newEntity);
        }

        public void AddNewReplayParentEntity(ReplayParentObject replayObject, int parentIndex)
        {
            TinyReplayEntity newEntity = this.CreateNewTinyReplayEntity(replayObject.entityType);
            newEntity.InitReplayEntity(replayObject.gameObject, replayObject.prefabName, parentIndex);
            this.allEntity.Add(newEntity);
        }

        private TinyReplayEntity CreateNewTinyReplayEntity(ReplayEntityType type)
        {
            TinyReplayEntity newEntity = null;
            if (type == ReplayEntityType.Player)
                newEntity = new TinyReplayPlayerEntity();
            else
                newEntity = new TinyReplayEntity();
            return newEntity;
        }

        // save entity state to local file.
        public void InsertEntityNewState(int timePos)
        {
            for (int i = 0; i < this.allEntity.Count; i++)
                this.allEntity[i].InsertNewEntityState(timePos);
        }

        // parsing state data.
        public int ParsingStateData(string newData)
        {
            int timePos = -1;
            // find effective way to split and parse strings
            string[] strDatas = newData.Split(';');
            int entityIndex = int.Parse(strDatas[0]);
            if (this.allEntityDic.ContainsKey(entityIndex))
            {
                timePos = int.Parse(strDatas[1]);
                this.allEntityDic[entityIndex].ParsingStateData(timePos, strDatas[2]);
            }
            else
                Debug.LogError("@ entity dictionary has no entityIndex:" + entityIndex);

            return timePos;
        }
    }
}
