using System;
using System.IO;
using Base;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter
{
    public class CreateCharacterSystem : MonoBehaviour
    {
        [SerializeField] private int totalSlots = 3;
        [SerializeField] private SkinResources skinResources;
        [SerializeField] private SelectCharacterPanel selectCharacterPanel;
        [SerializeField] private SetPlayerNamePanel setPlayerNamePanel;
        [SerializeField] private CreateCharacterPanel createCharacterPanel;
        [SerializeField]
        private UserData userData;
        private string userDataPath = "UserData";
        public static CreateCharacterSystem Instance { get; private set; }
        [field: SerializeField]
        public CharacterData NewCharacter { get; private set; }
        private void Start()
        {
            Instance = this;
            userData = LoadUserData();
            selectCharacterPanel.Init(userData.CharacterDatas, totalSlots,skinResources);
            selectCharacterPanel.SetActive(true);
            selectCharacterPanel.DoFade(0,1,0.5f,true);
        }
        private UserData LoadUserData()
        {
            var path = Path.Combine(Application.persistentDataPath, userDataPath);
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            var bin = File.ReadAllBytes(path);
            var reader = new Reader(bin);
            UserData userData = null;
            try
            {
                userData = reader.Read<UserData>();
            }
            catch (Exception ex)
            {
                userData = new UserData();
            }
            Save(path,userData);
            return userData;
        }

        private void Save(string path,UserData userData)
        {
            var writer = new Writer(userData.ByteSize());
            writer.Write(userData);
            File.WriteAllBytes(path,writer.Buffer);
        }
        public void AddCharacterData(CharacterData characterData)
        {
            if (userData.CharacterDatas.Count >= totalSlots) return;
            userData.CharacterDatas.Add(characterData);
            var path = Path.Combine(Application.persistentDataPath, userDataPath);
            Save(path,userData);
        }

        public void BeginCreateCharacter()
        {
            NewCharacter = new CharacterData();
            ToSetPlayerNamePanel(selectCharacterPanel);
        }
        public void ToSetPlayerNamePanel(IPanel panel)
        {
            panel.DoFade(1,0,0.5f,false, () =>
            {
                panel.SetActive(false);
            });
            setPlayerNamePanel.SetActive(true);
            setPlayerNamePanel.DoFade(0,1,0.5f,true);
        }
        public void BackToSelectCharacter()
        {
            selectCharacterPanel.SetActive(true);
            selectCharacterPanel.DoFade(0,1,0.5f,true);
            setPlayerNamePanel.DoFade(1,0,0.5f,false, () =>
            {
                setPlayerNamePanel.SetActive(false);
            });
        }
        public void NextToCreateCharacterPanel()
        {
            setPlayerNamePanel.DoFade(1,0,0.5f,false, () =>
            {
                setPlayerNamePanel.SetActive(false);
            });
            createCharacterPanel.SetActive(true);
            createCharacterPanel.Init(skinResources);
            createCharacterPanel.DoFade(0,1,0.5f,true);
        }

        [Button]
        public void ClearData()
        {
            var path = Path.Combine(Application.persistentDataPath, userDataPath);
            File.Delete(path);
        }
    }
}
