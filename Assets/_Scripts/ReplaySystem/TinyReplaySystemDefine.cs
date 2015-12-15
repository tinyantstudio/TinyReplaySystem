using UnityEngine;
using System.Collections;

namespace TinyReplay
{
    /// <summary>
    /// Replay system global define.
    /// </summary>
    public class TinyReplaySystemDefine
    {
        public static string strEntityStateSaveFileName = "entityStateData.txt";
        public static string strReplayProcessFileName = "replayProcess.json";

        public static string GetStrEntityStateSaveFilePath()
        {
            string fileName = string.Empty;
#if UNITY_IOS
            fileName = Application.persistentDataPath + "/" + TinyReplaySystemDefine.strEntityStateSaveFileName;
#else
            fileName = Application.persistentDataPath + TinyReplaySystemDefine.strEntityStateSaveFileName;
#endif
            return fileName;
        }

        public static string GetStrReplayProcessFilePath()
        {
            string fileName = string.Empty;
#if UNITY_IOS
            fileName = Application.persistentDataPath + "/" + TinyReplaySystemDefine.strReplayProcessFileName;
#else
            fileName = Application.persistentDataPath + TinyReplaySystemDefine.strReplayProcessFileName;
#endif
            return fileName;
        }
    }


    public enum ReplaySystemState
    {
        None = 0,
        Recording,      // recording
        Replaying,      // replaying
    }

    // entity type.
    public enum ReplayEntityType
    {
        None,
        Player,
    }

    public enum SaveTargetPropertiesType
    {
        None = 0,
        Rotation,
        Color,
        Position,
    }


    public class Tester
    {
        public static void TestForSaveObject()
        {
            //int saveType = 0;
            //saveType |= (1 << ((int)SaveTargetPropertiesType.Possition));
            //string saveData = TinyReplayObjectBaseState.SavePropertiesPosition(0, 100, this.gameObject.transform, null, saveType);
            //Debug.Log("saveData:" + saveData);

            //saveType |= (1 << (int)SaveTargetPropertiesType.Rotation);
            //saveData = TinyReplayObjectBaseState.SavePropertiesPosition(0, 100, this.gameObject.transform, null, saveType);
            //Debug.Log("saveData:" + saveData);

            //saveType |= (1 << (int)SaveTargetPropertiesType.Color);
            //saveData = TinyReplayObjectBaseState.SavePropertiesPosition(0, 100, this.gameObject.transform, targetTexture, saveType);
            //Debug.Log("saveData:" + saveData);
        }
    }
}

