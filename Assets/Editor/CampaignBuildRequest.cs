using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Allows a local build request while the project is already open in Unity.
[InitializeOnLoad]
public static class CampaignBuildRequest
{
    static CampaignBuildRequest() { EditorApplication.update += Poll; }
    private static void Poll()
    {
        const string request = "Library/CampaignBuild.request";
        if(EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlaying || BuildPipeline.isBuildingPlayer || !File.Exists(request)) return;
        File.Delete(request);
        var setup = EditorSceneManager.GetSceneManagerSetup();
        try
        {
            if(!EditorSceneManager.SaveOpenScenes()) throw new Exception("Could not save open scenes before build");
            CampaignValidation.BuildWindows();
            File.WriteAllText("Library/CampaignBuild.result","PASS");
        }
        catch(Exception error)
        {
            Debug.LogException(error);
            File.WriteAllText("Library/CampaignBuild.result","FAIL: "+error.Message);
        }
        finally { EditorSceneManager.RestoreSceneManagerSetup(setup); }
    }
}
