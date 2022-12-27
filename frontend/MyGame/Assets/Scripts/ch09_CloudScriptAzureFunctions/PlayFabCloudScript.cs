using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabCloudScript : MonoBehaviour
{
    IDictionary<string, string> displayNamesCache = new Dictionary<string, string>();
    public string GetDisplayNameFromCache(string entityId)
    {
        string displayName;
        if (displayNamesCache.TryGetValue(entityId, out displayName)) return displayName;
        else return null;
    }
    public void UpdateDisplayNameCache(string entityId)
    {
        //ExecuteCloudScript(entityId); // with CloudScript (legacy)
        ExecuteCloudScriptFunctions(entityId); // with CloudScript Functions
    }

    private void ExecuteCloudScript(string entityId)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "GetPlayFabIdFromEntityId",
            FunctionParameter = new { Id = entityId },
        },
        result =>
        {
            GetDisplayName(result.FunctionResult.ToString());
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void ExecuteCloudScriptFunctions(string entityId)
    {
        PlayFabCloudScriptAPI.ExecuteFunction(new PlayFab.CloudScriptModels.ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFab.PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFab.PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "GetPlayFabIdFromEntityId",
            FunctionParameter = new Dictionary<string, object>() { { "entityId", entityId } },
        },
        result =>
        {
            GetDisplayName(result.FunctionResult.ToString());
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }


    private void GetDisplayName(string playFabId)
    {
        PlayFabClientAPI.GetPlayerCombinedInfo(new GetPlayerCombinedInfoRequest
        {
            PlayFabId = playFabId,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetUserAccountInfo = true,
                ProfileConstraints = new PlayerProfileViewConstraints
                {
                    ShowDisplayName = true
                }
            }
        },
        result =>
        {
            var displayName = result.InfoResultPayload.AccountInfo.TitleInfo.DisplayName;
            var entityId = result.InfoResultPayload.AccountInfo.TitleInfo.TitlePlayerAccount.Id;

            displayNamesCache[entityId] = displayName;
            ChatUI.UpdateDisplayName(entityId, displayName);
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }
}
