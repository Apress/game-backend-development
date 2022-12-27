using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class PlayFabAuth : MonoBehaviour
{
    public void PlayFabLoginWithCustomID(string displayName)
    {
        var guid = Guid.NewGuid().ToString();

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
        {
            CustomId = guid,
            CreateAccount = true
        },
        result =>
        {
            Debug.Log("Login with CustomID succeeded.");
            GetComponent<PlayFabSettings>().entityId = result.EntityToken.Entity.Id;
            UpdateDisplayName(displayName);
            GetComponent<PlayFabSettings>().displayName = displayName;
        },
        error =>
        {
            Debug.Log("Login with CustomID failed. " + error.ErrorMessage);
        });
    }

    public void PlayFabLoginWithUsernameAndPassword(string name, string password, string displayName)
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = name,
            Password = password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        },
        result =>
        {
            Debug.Log("Login with PlayFab succeeded.");
            GetComponent<PlayFabSettings>().entityId = result.EntityToken.Entity.Id;
            GetComponent<PlayFabSettings>().displayName = result.InfoResultPayload.PlayerProfile.DisplayName;

        },
        error =>
        {
            Debug.Log("Login with PlayFab failed. " + error.ErrorMessage);
            if (error.Error == PlayFab.PlayFabErrorCode.AccountNotFound)
                RegisterPlayFabUser(name, password, displayName);
        });
    }

    private void RegisterPlayFabUser(string name, string password, string displayName)
    {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest()
        {
            Username = name,
            Password = password,
            RequireBothUsernameAndEmail = false

        },
        result =>
        {
            Debug.Log("PlayFab user is registered.");
            GetComponent<PlayFabSettings>().entityId = result.EntityToken.Entity.Id;
            UpdateDisplayName(displayName);
            GetComponent<PlayFabSettings>().displayName = displayName;

        },
        error =>
        {
            Debug.Log("PlayFab user registration failed. " + error.ErrorMessage);
        });
    }

    private void UpdateDisplayName(string displayName)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = displayName
            },
            (UpdateUserTitleDisplayNameResult result) =>
            {
                Debug.Log("Display name updated.");
            },
            (PlayFabError error) =>
            {
                Debug.LogError(error.GenerateErrorReport());
            });
    }
}
