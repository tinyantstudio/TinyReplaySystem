using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TinyReplay
{
    // replay parent object load prefab and other setting.
    // every replay object should have this script attached.
    public class ReplayParentObject : MonoBehaviour
    {
        public string prefabName;
        public ReplayEntityType entityType;
        public Dictionary<int, ReplayObject> allReplayObjects;

        private int entityBeginIndex = 10000;
        void Start()
        {
            this.CheckSetting();
        }

        private void CheckSetting()
        {
            ReplayParentObject[] parentObjs = this.GetComponentsInChildren<ReplayParentObject>();
            if (parentObjs.Length > 1)
                Debug.LogError("more than one Replay Parent Object script.");
        }

        public void InitReplayParentObject(int beginIndex)
        {
            this.entityBeginIndex = beginIndex + 1;
            ReplayObject[] objects = this.GetComponentsInChildren<ReplayObject>();
            if (this.allReplayObjects == null)
                this.allReplayObjects = new Dictionary<int, ReplayObject>();
            this.allReplayObjects.Clear();
            for (int i = 0; i < objects.Length; i++)
            {
                ReplayObject replayObjcet = objects[i];
                replayObjcet.entityIndex = (entityBeginIndex++);
                if (this.allReplayObjects.ContainsKey(replayObjcet.entityIndex))
                    Debug.LogError("Replay object has same entity index :" + replayObjcet.entityIndex);
                this.allReplayObjects[replayObjcet.entityIndex] = replayObjcet;
                Debug.Log(" replay object name is " + replayObjcet.gameObject.name + " index is " + replayObjcet.entityIndex);
            }
        }

        public ReplayObject GetReplayObject(int entityIndex)
        {
            if (this.allReplayObjects.ContainsKey(entityIndex))
                return this.allReplayObjects[entityIndex];
            return null;
        }
    }
}
