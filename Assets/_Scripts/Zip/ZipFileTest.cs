using UnityEngine;
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;
using System.Collections;
using System.Collections.Generic;

// 
// Use SharpZipLib for Compress files and Extract zip files.
// github: https://github.com/icsharpcode/SharpDevelop
//
// 1. take care of the file in different platforms.
// 2. compress files and folder.
// 3. extract zip file to target path.
// 4. deal with MacOSX extra files(DS_Store and *MACOSX* files.) 

public class ZipFileTest : MonoBehaviour
{
    private string compressInputFolder = string.Empty;
    private string compressOutputFolder = string.Empty;
    private string extractTargetZipFile = string.Empty;
    private string extractFolder = string.Empty;
    public UILabel mLbConsoleMsg;

    public GameObject btnCompress;
    public GameObject btnExtract;
    public GameObject btnMoveFile;

    public UIPopupList popList;

    private List<string> mTextureNames = new List<string>();
    private Dictionary<string, string> mTexturefilePath = new Dictionary<string, string>();

    public UITexture mTexture;
    public UILabel mTextureName;

    private string streamingPath = string.Empty;
    void Awake()
    {
#if UNITY_EDITOR
        streamingPath = Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
        streamingPath = "jar:file://"+ Application.dataPath + "!/assets/";
#elif UNITY_IOS
        streamingPath = "file:" + Application.dataPath + "/Raw/";
#else
        //Desktop (Mac OS or Windows)
        streamingPath = Application.dataPath + "/StreamingAssets/";
#endif
        compressInputFolder = streamingPath + "ToCompress";
        compressOutputFolder = Application.persistentDataPath + "/ZipFile.zip";
        extractTargetZipFile = Application.persistentDataPath + "/ZipFile.zip";
        extractFolder = Application.persistentDataPath + "/ExtraData";

        Debug.Log(" compress path is " + compressInputFolder + " " + compressOutputFolder);

        UIEventListener.Get(this.btnCompress).onClick = delegate
        {
            if (string.IsNullOrEmpty(this.compressInputFolder))
            {
                mLbConsoleMsg.text += "\n[99ff00]input file is null or empty.[-]";
                mLbConsoleMsg.color = Color.red;
                Debug.LogError("input file is null or empty.");
            }
            else
                this.CreateSample(compressOutputFolder, string.Empty, compressInputFolder);
        };

        UIEventListener.Get(this.btnExtract).onClick = delegate
        {
            if (string.IsNullOrEmpty(this.extractFolder))
            {
                mLbConsoleMsg.text += "\n[EE4000]extracFolder is null or empty.[-]";
                Debug.LogError(" extract folder is null or empty.");
            }
            else
            {
                this.ExtractZipFile(this.extractTargetZipFile, this.extractFolder);
            }
        };

        UIEventListener.Get(this.btnMoveFile).onClick = delegate
        {
            StartCoroutine(this.DownLoadDataFromStreamingAssetsToLocalFile());
        };

        EventDelegate.Add(popList.onChange, this.OnSelectTexture);
    }

    public void CreateSample(string outPathName, string password, string folderName)
    {
        FileStream fsOut = File.Create(outPathName);
        ZipOutputStream zipStream = new ZipOutputStream(fsOut);

        zipStream.SetLevel(3);
        int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

        mLbConsoleMsg.text += "\n[99ff00]output zip file to [-]" + outPathName;
        this.CompressFolder(folderName, zipStream, folderOffset);
        zipStream.IsStreamOwner = true;
        zipStream.Close();
    }

    private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
    {
        string[] files = Directory.GetFiles(path);

        foreach (string fileName in files)
        {
            // for the Mac system.
            if (fileName.Contains("meta")
            || fileName.Contains("DS_Store"))
                continue;
            FileStream inputStream = new FileStream(fileName, FileMode.Open);
            string entryName = fileName.Substring(folderOffset);
            entryName = ZipEntry.CleanName(entryName);
            ZipEntry newEntry = new ZipEntry(entryName);
            newEntry.Size = inputStream.Length;
            zipStream.PutNextEntry(newEntry);

            byte[] buffer = new byte[4096];
            StreamUtils.Copy(inputStream, zipStream, buffer);

            zipStream.CloseEntry();
            inputStream.Close();
            this.mLbConsoleMsg.text += ("\n[9ACD32]compress file :" + fileName + "[-]");
            Debug.Log(" compress file :" + fileName);
        }

        string[] folders = Directory.GetDirectories(path);
        foreach (string folder in folders)
            CompressFolder(folder, zipStream, folderOffset);
    }

    public void ExtractZipFile(string archiveFileName, string outFolder)
    {
        mLbConsoleMsg.text += "\n[9ACD32]extract file to " + outFolder + "[-]";
        this.mTexturefilePath.Clear();
        this.mTextureNames.Clear();

        ZipFile zf = null;
        try
        {
            FileStream fs = File.OpenRead(archiveFileName);
            zf = new ZipFile(fs);

            foreach (ZipEntry zipEntry in zf)
            {
                if (!zipEntry.IsFile) continue;
                string entryFileName = zipEntry.Name;

                // for the Mac system
                if (entryFileName.Contains("DS_Store")
                    || entryFileName.Contains("meta")
                    || entryFileName.Contains("MACOSX"))
                    continue;

                byte[] buffer = new byte[4096];
                Stream zipStream = zf.GetInputStream(zipEntry);

                string fullZipToPath = Path.Combine(outFolder, entryFileName);
                string dirctoryName = Path.GetDirectoryName(fullZipToPath);
                if (dirctoryName.Length > 0)
                    Directory.CreateDirectory(dirctoryName);

                FileStream streamWriter = File.Create(fullZipToPath);
                StreamUtils.Copy(zipStream, streamWriter, buffer);
                streamWriter.Close();

                this.mLbConsoleMsg.text += ("\n[8E388E]extract file:" + fullZipToPath + "[-]");
                Debug.Log(" extract file:" + entryFileName);

                this.mTextureNames.Add(entryFileName);
                this.mTexturefilePath[entryFileName] = fullZipToPath;
            }
        }
        catch (System.Exception ex)
        {
            if (zf != null)
            {
                zf.IsStreamOwner = true;
                zf.Close();
            }

            this.mLbConsoleMsg.text += ("\n[EE4000]exception " + ex.Message + "[-]");
            Debug.LogError(ex.Message);
        }

        this.UpdatePopContent();
    }

    private void OnSelectTexture()
    {
        string textureName = popList.value;
        this.mLbConsoleMsg.text += "\n on select texture name:" + textureName;
        this.mTextureName.text = textureName;
        DownLoadTexture(textureName);
    }

    private void UpdatePopContent()
    {
        this.popList.Clear();
        for (int i = 0; i < this.mTextureNames.Count; i++)
            this.popList.AddItem(this.mTextureNames[i]);

        if (this.mTextureNames.Count > 0)
            this.DownLoadTexture(this.mTextureNames[0]);
    }

    private void DownLoadTexture(string newTexture)
    {
        this.popList.value = newTexture;
        if (!this.mTexturefilePath.ContainsKey(newTexture))
            return;
        string url = "file:///" + this.mTexturefilePath[newTexture];
        this.mLbConsoleMsg.text += ("download file : " + url);
        StartCoroutine(this.DownLoadItems(url));
    }

    private Texture2D downLoadTexture;
    private bool isDownLoading = false;

    IEnumerator DownLoadItems(string url)
    {
        this.isDownLoading = true;
        this.popList.enabled = false;
        WWW www = new WWW(url);
        yield return www;
        this.downLoadTexture = www.texture;
        this.mTexture.mainTexture = this.downLoadTexture;
        www.Dispose();
        this.isDownLoading = false;
        this.popList.enabled = true;
    }
    void OnDestroy()
    {
        Destroy(this.downLoadTexture);
    }

    IEnumerator DownLoadDataFromStreamingAssetsToLocalFile()
    {
        string url = string.Empty;
#if UNITY_EDITOR
        url = "file:///" + this.streamingPath + "ReadyToCompress.zip";
#elif UNITY_ANDROID
        url = this.streamingPath + "ReadyToCompress.zip";;
#elif UNITY_IOS
        url = this.streamingPath + "ReadyToCompress.zip";
#else
        //Desktop (Mac OS or Windows)
        url = "file:///" + this.streamingPath + "ReadyToCompress.zip";
#endif
        Debug.Log(" down load from the streamingAssets url is " + url);
        WWW www = new WWW(url);
        yield return www;
        File.WriteAllBytes(this.compressOutputFolder, www.bytes);

        string message = " downloading over move to local file:" + this.compressOutputFolder;
        Debug.Log(message);

        this.mLbConsoleMsg.text += message;
    }
}
