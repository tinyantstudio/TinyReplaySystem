using UnityEngine;
using System.Collections;
using System.IO;
using System;
using Newtonsoft.Json;

namespace TinyReplay
{
    /// <summary>
    /// Record Controller.
    /// </summary>
    public class TinyReplayRecordController : MonoBehaviour
    {
        private float mTimeElapsed = 0.0f;
        private float mPerRecordInterval = 0.0f;
        private float mStartRecordTime = 0.0f;

        private bool mIsRecording = false;
        private bool mIsStartRecording = false;

        public bool IsRecording
        {
            get { return (this.mIsRecording || this.mIsStartRecording); }
        }
        private int mCurTimePos = 0;

        private TinyReplayProgress mProgressController;
        private StreamWriter mStreamWriter;

        private int mMaxTimePosition = 5400;// 30 * 60 * 3
        public static TinyReplayRecordController instance;
        void Awake()
        {
            instance = this;
        }

        public void SaveDataToLocalFile(string strToSave)
        {
            mStreamWriter.WriteLine(strToSave);
            this.mProgressController.mTotalSaveDataLineCount++;
        }

        public void StartRecording()
        {
            string fileName = TinyReplaySystemDefine.GetStrEntityStateSaveFilePath();
            // open file for entity state to write.
            FileInfo fileInfor = new FileInfo(fileName);
            mStreamWriter = fileInfor.CreateText();

            this.mIsRecording = true;
            this.mIsStartRecording = true;
            this.mTimeElapsed = 0.0f;
            this.mPerRecordInterval = 0.033f;
            this.mCurTimePos = 0;
            this.mStartRecordTime = Time.realtimeSinceStartup;
            this.InitRecordProgress();

            Debug.Log("@start recording." + Time.realtimeSinceStartup);
            // spriteDone.color = Color.gray;

            // Debug information.
            string debugMsg = string.Format("Recording...PerSecondInterval:{0}, MaxTimePos:{1}.", this.mPerRecordInterval, this.mMaxTimePosition);
            TinyReplayManager.GetInstance.OnRecordBegin(debugMsg);
        }

        // stop recording save data to local file.
        public void StopRecording()
        {
            this.mIsRecording = false;
            this.mIsStartRecording = false;
            // save the last time position entity state.
            Debug.Log("@stop recoring save record file.");
            this.SaveRecordDataToLocalFile();
        }

        private void InitRecordProgress()
        {
            if (this.mProgressController == null)
                this.mProgressController = new TinyReplayProgress();
            this.mProgressController.ResetReplayProgress();

            // init the replay progress.
            this.mProgressController.mPerRecordInterval = this.mPerRecordInterval;
            this.mProgressController.audioURL = "HelloWorld.mp3";

            // reset the replay progress controller.
            GameObject[] allReplayObjs = GameObject.FindGameObjectsWithTag("RepalyObjectParent");
            for (int i = 0; i < allReplayObjs.Length; i++)
            {
                ReplayParentObject replayParent = allReplayObjs[i].GetComponent<ReplayParentObject>();
                if (replayParent == null)
                {
                    Debug.LogError(" replay parent not have ReplayParentObject script.");
                    continue;
                }
                int parentIndex = TinyReplayManager.GetInstance.CurEntityParentBeginIndex;
                replayParent.InitReplayParentObject(parentIndex);
                this.mProgressController.AddNewReplayParentEntity(replayParent, parentIndex);
                TinyReplayManager.GetInstance.CurEntityParentBeginIndex = (parentIndex + 10000);

                Debug.Log("next replay parent index is " + TinyReplayManager.GetInstance.CurEntityParentBeginIndex);
                foreach (var replayChildObject in replayParent.allReplayObjects)
                    this.mProgressController.AddNewReplayEntity(replayChildObject.Value);

                // save the first time pos entity state.
                this.mProgressController.InsertEntityNewState(this.mCurTimePos);
            }
        }

        public void PauseRecording()
        {
            this.mIsRecording = false;
        }

        public void ResumeRecording()
        {
            this.mIsRecording = true;
        }

        void Update()
        {
            if (!this.mIsRecording)
                return;
            if (Time.realtimeSinceStartup - this.mStartRecordTime >= this.mPerRecordInterval)
            {
                this.mCurTimePos++;
                // Debug.Log(" begin next record." + this.mCurTimePos);
                this.mProgressController.InsertEntityNewState(this.mCurTimePos);
                this.mStartRecordTime = Time.realtimeSinceStartup;
                this.mProgressController.mMaxTimePosition = this.mCurTimePos;

                if (this.mCurTimePos >= mMaxTimePosition)
                    this.StopRecording();

                TinyReplayManager.GetInstance.RefreshTimePosition(this.mCurTimePos);
            }
        }

        private void SaveFile()
        {
            Debug.Log("@start time " + Time.realtimeSinceStartup);
            string fileName = TinyReplaySystemDefine.GetStrReplayProcessFilePath();
            Debug.Log("@save file to:" + fileName);

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            // **** write all file just save string text to file.
            string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(this.mProgressController, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            FileInfo fileInfor = new FileInfo(fileName);
            using (StreamWriter sw = fileInfor.CreateText())
            {
                sw.Write(jsonStr);
            }

            Debug.Log("@end time " + Time.realtimeSinceStartup);

            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed;
            double takeTime = timeSpan.TotalSeconds;
            double takeTime2 = timeSpan.TotalMilliseconds;

            Debug.Log("@Test save record file take time is " + takeTime + " current Time is " + takeTime2);

            // Debug information.
            string debugMsg = string.Format("Record over. save file:{0}.", fileName);
            TinyReplayManager.GetInstance.OnRecordOver(debugMsg);

        }
        private void SaveRecordDataToLocalFile()
        {
            mStreamWriter.Flush();
            mStreamWriter.Close();
            this.SaveFile();
        }
    }
}
