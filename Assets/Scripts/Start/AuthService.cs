using System.Collections.Generic;
using System.Linq;
using AllMaps.NgoDao;
using CreateCharacter;
using Network;
using Network.Callback;
using UnityEngine;
using Network.Extensions;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.SceneManagement;
using CongPhap = Skill.CongPhap;
using NgoDao = AllMaps.NgoDao.NgoDao;
using Network.Core;

namespace Start
{
    public class AuthService : MonoBehaviour, IAuthCallback
    {
        [SerializeField] private string username;
        [SerializeField] private string loggedIn;
        [SerializeField] private StartScene startScene;
        public void Login()
        {
            startScene.SetStatusText(loggedIn);
            Client.Instance.Send(MessageTag.LoginDeveloper, username);
        }

        public void OnLoginResult(LoginResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                startScene.SetStatusText(result.Message);
                if (result.Success)
                {
                    Client.Instance.SetUserData(result.UserData);
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("CreateCharacter");
                    
                }
            });
        }
    }
}