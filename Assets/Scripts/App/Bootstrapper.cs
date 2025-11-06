using App.Config;
using App.Input;
using App.SaveData;
using App.Scenes;
using App.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App
{
    public class Bootstrapper : MonoBehaviour
    {
        private const string MainSceneName = "App";
        private const string GameSceneName = "Game";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Main()
        {
            if (SceneManager.GetSceneByName(MainSceneName).IsValid()) return;
            SceneManager.LoadScene(MainSceneName);
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            var serviceLocator = gameObject.AddComponent<ServiceLocator>();
            serviceLocator.Initialize();
            
            serviceLocator.Register(new SceneController());
            serviceLocator.Register(new IOController());
            serviceLocator.Register(new ConfigController());
            serviceLocator.Register(new SaveDataController());
            
            var inputController = gameObject.AddComponent<InputController>();
            inputController.Initialize();
            serviceLocator.Register(inputController);
            
            ServiceLocator.Instance.Get<SceneController>().LoadScene(GameSceneName, false);
            
            Destroy(this);
        }       
    }
}
