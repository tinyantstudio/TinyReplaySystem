using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System;

namespace TinyReplay
{
    /// <summary>
    /// Play animation controller.
    /// </summary>
    public class TinyReplayPlayController : MonoBehaviour
    {
        private TinyReplayProgress mProgressController;

        private float mPerReplayInterval = 0.0f;
        private float mStartPlayTime = 0.0f;
        private int mCurTimePos = 0;
        private bool mIsReplaying = false;
        // read need data over just replay the animation.
        private bool mIsStartReplay = false;

        // per load line data count.
        private int mMinLoadLineDataCount = 60;
        private int mStartReplayTimePos = 60;
        private int mFirstTimePosLineDataCount = 0;
        private bool mFirstUpdateOver = false;

        public bool IsReplaying
        {
            get { return this.mIsReplaying || this.mIsStartReplay; }
        }

        public void LoadReplayDataFromFile()
        {
            StartCoroutine(this._LoadReplayDataFromFile());
        }

        IEnumerator _LoadReplayDataFromFile()
        {
            string fileName = TinyReplaySystemDefine.GetStrReplayProcessFilePath();
            Debug.Log("@load file name is " + fileName);
            Debug.Log("@start load time:" + Time.realtimeSinceStartup);

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // load process data.
            FileInfo fileInfor = new FileInfo(fileName);
            if (fileInfor.Exists)
            {
                using (StreamReader sr = fileInfor.OpenText())
                {
                    string jsonProgress = sr.ReadToEnd();
                    this.mProgressController = Newtonsoft.Json.JsonConvert.DeserializeObject<TinyReplayProgress>(jsonProgress, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
                    this.mPerReplayInterval = this.mProgressController.mPerRecordInterval;
                }
            }
            else
            {
                Debug.LogError("replay file is not exist, please check it.");
                yield break;
            }

            // set the min line data count.
            if (this.mPerReplayInterval <= 0.0f)
            {
                this.mPerReplayInterval = 0.033f;
                Debug.LogError("@per replay interval is less than 0.0f.");
            }

            int perSecondCount = Mathf.RoundToInt(1f / this.mPerReplayInterval);
            int entityCount = this.mProgressController.allEntity.Count;
            this.mMinLoadLineDataCount = perSecondCount * 2 * entityCount;
            this.mStartReplayTimePos = Mathf.Min(this.mMinLoadLineDataCount * 4, this.mProgressController.mMaxTimePosition * entityCount);
            this.mFirstTimePosLineDataCount = entityCount;
            this.mFirstUpdateOver = false;
            this.mIsReplaying = false;
            this.mIsStartReplay = false;
            Debug.Log("@per second count is " + perSecondCount);
            Debug.Log("@min load line data count is " + this.mMinLoadLineDataCount);
            Debug.Log("@start replay animation timepos is " + this.mStartReplayTimePos);
            Debug.Log("@first time pos line data count is " + this.mFirstTimePosLineDataCount);
            // init the animation entity.
            // 加载角色模板，下载本地贴图，全部完成开始播放动画.
            // 第一帧缩略图，至于最高层，当所有的角色加载完毕并且第一帧得到了更新，删除当前的缩略图开始动画
            this.LoadEntityDataBeforeReplay();

            fileName = TinyReplaySystemDefine.GetStrEntityStateSaveFilePath();
            fileInfor = new FileInfo(fileName);
            string lineData = string.Empty;
            int dataCount = 0;
            if (fileInfor.Exists)
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    while ((lineData = sr.ReadLine()) != null)
                    {
                        dataCount++;
                        this.PrasingStateData(lineData, dataCount);
                        if (dataCount >= this.mFirstTimePosLineDataCount && !this.mFirstUpdateOver)
                        {
                            Debug.Log("@update the first timePos state.");
                            this.mFirstUpdateOver = true;
                            // init the first timePos.
                            for (int i = 0; i < entityCount; i++)
                                this.mProgressController.allEntity[i].SynchronizeEntity(0);
                        }
                        this.CheckToReplayAnimation(dataCount);
                        // lbLineIndex.text = dataCount.ToString();
                        // per 2 second data count.
                        if (dataCount % this.mMinLoadLineDataCount == 0)
                        {
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("replay progress data file is not exist.");
                yield break;
            }

            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed;
            double takeTime = timeSpan.TotalSeconds;
            double takeTime2 = timeSpan.TotalMilliseconds;

            //打印程序运行的时间
            Debug.Log("@LoadRecordFile__________take time is " + takeTime + " current Time is " + takeTime2);
            Debug.Log("@end load time:" + Time.realtimeSinceStartup);
            yield return 0;
        }

        private void LoadEntityDataBeforeReplay()
        {
            Debug.Log("@animation entity count is " + this.mProgressController.allEntity.Count);
            List<TinyReplayEntity> allEntity = this.mProgressController.allEntity;
            StringBuilder sb = null;

            GameObject templateObject = null;
            ReplayParentObject parentScript = null;

            for (int i = 0; i < allEntity.Count; i++)
            {
                TinyReplayEntity replayEntity = allEntity[i];
                sb = new StringBuilder();
                sb.AppendFormat("@entity information: prefab name is {0} entityIndex is {1}.", allEntity[i].mTemplateName, allEntity[i].entityIndex);
                Debug.Log(sb.ToString());

                // add TinyReplayEntity to progressController.
                this.mProgressController.allEntityDic[replayEntity.entityIndex] = replayEntity;

                if (replayEntity.entityIndex % 10000 == 0)
                {
                    GameObject loadedPrefab = Resources.Load<GameObject>("AnimationPrefab/" + replayEntity.mTemplateName);

                    // load the prefab and disable some script. like MovePosition, ChangeRotation etc.
                    // we just set the target state Properties use saved file.
                    // and you can disable/enable or just destory these unused control scripts.

                    // 删除对象的控制文件，以为在播放阶段，直接通过保存的对象信息更新对象位置缩放等属性信息
                    // 如果在文件中存储了一些控制命令，比如播放xx动画，可以解析该命令直接通过脚本控制对象执行动画操作
                    // 保证对象控制逻辑和同步逻辑不冲突.
                    // 有些情况下比如角色的动作过于复杂，也可以保留动作控制脚本，在Record的时候记录当前动画状态，到达TimePos时间点过后，直接按照当前的
                    // 动画状态播放角色动画
                    // 必须保证同步文件和控制脚本控制的对象不发生冲突.

                    if (loadedPrefab != null)
                    {
                        templateObject = GameObject.Instantiate(loadedPrefab) as GameObject;

                        // Delete some control script attached to the prefab template, such as UIDragObject etc.
                        UIDragObject[] disableScript = templateObject.GetComponentsInChildren<UIDragObject>();
                        for (int disIndex = 0; disIndex < disableScript.Length; disIndex++)
                        {
                            // or just destory the script you want to disable.
                            // we use UIDragObject script to drag object in recording, when replaying the animation we don't want
                            // the charater can be draged , so we destory or disable the UIDragObject script when replaying.
                            // that's why we add some control logic in these function.
                            // 在录制动画过程中用到了Drag脚本控制，但是在播放的过程不希望当前的角色能够Drag所以直接的删除或者disable相应的控制脚本
                            disableScript[disIndex].enabled = false;
                        }

                        parentScript = templateObject.GetComponent<ReplayParentObject>();
                        templateObject.SetActive(true);
                        if (parentScript != null)
                            parentScript.InitReplayParentObject(replayEntity.entityIndex);

                        // add to animation stage.
                        templateObject.transform.parent = AnimationStage.StageRoot.transform;
                        templateObject.transform.localScale = Vector3.one;
                        templateObject.transform.localPosition = Vector3.zero;
                        templateObject.transform.rotation = Quaternion.identity;
                        // synchronize first time postiton state.
                        replayEntity.PrepareForReplay(templateObject);
                    }
                    else
                        Debug.LogError("load prefab is null.");
                }
                else
                {
                    // for the child.
                    if (parentScript == null)
                        Debug.LogError("@parent script is null.");
                    ReplayObject replayObject = parentScript.GetReplayObject(replayEntity.entityIndex);
                    if (replayObject == null)
                        Debug.LogError("replay object null:" + replayEntity.entityIndex);
                    Debug.Log("@ replay object name is " + replayObject.name);
                    replayEntity.PrepareForReplay(replayObject.gameObject);
                }
            }
        }

        private void StartReplaying()
        {
            this.mStartPlayTime = Time.realtimeSinceStartup;
            this.mCurTimePos = 0;
            this.mPerReplayInterval = this.mProgressController.mPerRecordInterval;
            this.mIsReplaying = true;
            Debug.Log("@start replay animation.");

            // Debug information.
            TinyReplayManager.GetInstance.OnReplayBegin();
        }

        void Update()
        {
            if (!this.mIsReplaying || !this.mIsStartReplay)
                return;

            // Debug information.    
            TinyReplayManager.GetInstance.RefreshTimePosition(this.mCurTimePos);

            if (this.mCurTimePos >= this.mProgressController.mMaxTimePosition)
            {
                this.OnReplayOver();
                return;
            }
            if (Time.realtimeSinceStartup - this.mStartPlayTime >= this.mPerReplayInterval)
            {
                this.mCurTimePos++;
                this.mStartPlayTime = Time.realtimeSinceStartup;
                int entityCount = this.mProgressController.allEntity.Count;
                for (int i = 0; i < entityCount; i++)
                    this.mProgressController.allEntity[i].SynchronizeEntity(this.mCurTimePos);
            }
        }

        // parsing the line state dato to target entity.
        private void PrasingStateData(string lineStateData, int lineIndex)
        {
            int timePos = this.mProgressController.ParsingStateData(lineStateData);
            if ((timePos >= this.mStartReplayTimePos || (lineIndex >= this.mProgressController.mTotalSaveDataLineCount)) && this.mFirstUpdateOver)
            {
                this.mIsStartReplay = true;
                this.StartReplaying();
            }
        }

        private void CheckToReplayAnimation(int lineIndex)
        {
            if (this.mFirstUpdateOver && lineIndex >= this.mProgressController.mTotalSaveDataLineCount)
            {
                this.mIsStartReplay = true;
                this.StartReplaying();
            }
        }


        private void OnReplayOver()
        {
            Debug.Log("@replay is over.");
            this.mIsReplaying = false;
            this.mIsStartReplay = false;
            // Debug information.
            TinyReplayManager.GetInstance.OnReplayEnd();
        }
    }
}