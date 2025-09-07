using Morhu.Util;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Morhu.Infrustructure
{
    public class SceneLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public SceneLoader(ICoroutineRunner coroutineRunner) => 
            _coroutineRunner = coroutineRunner;

        public void Load(SceneName sceneName, Action onLoaded) => 
            _coroutineRunner.StartCoroutine(LoadScene(sceneName, onLoaded));

        private IEnumerator LoadScene(SceneName scene, Action onLoaded)
        {
            var sceneName = scene.ToString();

            if (SceneManager.GetActiveScene().name == sceneName)
            {
                onLoaded?.Invoke();
                yield break;
            }

            AsyncOperation wait = SceneManager.LoadSceneAsync(sceneName);

            while (!wait.isDone)
                yield return wait;

            onLoaded?.Invoke();
        }
    }
}
