using UnityEngine;

using UnityEngine.SceneManagement;

using System.Collections;

using System.Collections.Generic;



namespace SA

{

    public class MySceneManager : MonoBehaviour

    {
        public GameScene[] gameScenes;
        public GameObject menuCamera;

        Dictionary<string, int> lvls_dict = new Dictionary<string, int>();

        public string referencesScene = "references";
        public string startScene = "s1";

        const string LogTag = "[MySceneManager]";
        void Start()
        {
            for (int i = 0; i < gameScenes.Length; i++)
            {
                if (!lvls_dict.ContainsKey(gameScenes[i].primaryScene))
                    lvls_dict.Add(gameScenes[i].primaryScene, i);
            }

            UIManager.singleton.mainMenu.SetActive(false);
            //StartCoroutine(LoadSceneAsyncRoutine(referencesScene, LoadSceneMode.Additive));
        }
        int StringToIndex(string id)
        {
            int index = -1;
            lvls_dict.TryGetValue(id, out index);
            return index;
        }
        public void PressStartGame()

        {
            Debug.Log($"{LogTag} PressStartGame -> startScene='{startScene}'");
            StartCoroutine(StartGameRoutine());
        }
        IEnumerator StartGameRoutine()

        {
            UIManager.singleton.mainMenu.SetActive(false);

            yield return new WaitForSeconds(0.5f);
            menuCamera.SetActive(false);

            GameScene gs = GetGameScene(startScene);

            if (gs == null)

                yield break;

            yield return LoadGameSceneRoutine(gs, preloadForward: true, preloadBackward: true);
            yield return new WaitForSeconds(0.5f);

            // gestureGrid เปิดอยู่ใน scene — ปิดก่อนเปิด gameUI ไม่งั้น UI Gestures จะกระพริบ 1 frame
            if (GesturesManager.singleton != null)
                GesturesManager.singleton.HandleGestures(false);

            SessionManager.singleton.InitGame();

        }
        IEnumerator LoadGameSceneRoutine(GameScene gs, bool preloadForward, bool preloadBackward)
        {
            if (gs.isLoaded)

            {

                Debug.Log($"{LogTag} Skip load — '{gs.primaryScene}' already loaded.");

                yield break;

            }

            if (!string.IsNullOrEmpty(gs.lightData))
                yield return LoadSceneAsyncRoutine(gs.lightData, LoadSceneMode.Additive);
            yield return LoadSceneAsyncRoutine(gs.primaryScene, LoadSceneMode.Additive);

            gs.isLoaded = true;

            Debug.Log($"{LogTag} GameScene ready: '{gs.primaryScene}'");

            if (preloadForward && !string.IsNullOrEmpty(gs.forward))

            {

                GameScene forward = GetGameScene(gs.forward);

                if (forward != null && !forward.isLoaded)

                    yield return LoadGameSceneRoutine(forward, preloadForward: false, preloadBackward: false);

            }

            if (preloadBackward && !string.IsNullOrEmpty(gs.backward))

            {
                GameScene backward = GetGameScene(gs.backward);

                if (backward != null && !backward.isLoaded)

                    yield return LoadGameSceneRoutine(backward, preloadForward: false, preloadBackward: false);
            }
        }
        IEnumerator LoadSceneAsyncRoutine(string sceneName, LoadSceneMode mode)

        {

            if (string.IsNullOrEmpty(sceneName))

            {

                Debug.LogWarning($"{LogTag} Skip load — scene name is empty.");

                yield break;

            }



            Scene existing = SceneManager.GetSceneByName(sceneName);

            if (existing.isLoaded)

            {

                Debug.Log($"{LogTag} Already loaded: '{sceneName}'");

                yield break;

            }



            Debug.Log($"{LogTag} Loading '{sceneName}' mode={mode}...");

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);

            if (op == null)

            {

                Debug.LogError($"{LogTag} LoadSceneAsync returned null for '{sceneName}'. Is it in Build Settings?");

                yield break;

            }



            while (!op.isDone)

                yield return null;



            Debug.Log($"{LogTag} Loaded '{sceneName}' (progress={op.progress})");

        }



        public void LoadScene(string sceneName)

        {

            if (singleton == null)

            {

                Debug.LogError($"{LogTag} LoadScene('{sceneName}') failed — singleton is null.");

                return;

            }



            Debug.Log($"{LogTag} LoadScene request: '{sceneName}'");

            GameScene gs = GetGameScene(sceneName);

            if (gs == null)

                return;



            if (gs.isLoaded)

            {

                Debug.Log($"{LogTag} LoadScene skipped — '{sceneName}' already marked loaded.");

                return;

            }



            StartCoroutine(LoadGameSceneRoutine(gs, preloadForward: false, preloadBackward: false));

        }



        public void UnloadScene(string sceneName)

        {

            Debug.Log($"{LogTag} UnloadScene request: '{sceneName}'");

            GameScene gs = GetGameScene(sceneName);

            if (gs == null)

                return;



            if (!gs.isLoaded)

            {

                Debug.Log($"{LogTag} UnloadScene skipped — '{sceneName}' not loaded.");

                return;

            }



            gs.isLoaded = false;

            Scene existing = SceneManager.GetSceneByName(sceneName);

            if (!existing.isLoaded)

            {

                Debug.LogWarning($"{LogTag} UnloadScene — Unity scene '{sceneName}' was not loaded.");

                return;

            }



            AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);

            if (op == null)

                Debug.LogError($"{LogTag} UnloadSceneAsync returned null for '{sceneName}'.");

            else

                Debug.Log($"{LogTag} Unloading '{sceneName}'...");

        }



        GameScene GetGameScene(string id)

        {

            int index = StringToIndex(id);

            if (index < 0)

            {

                Debug.LogError($"{LogTag} Scene '{id}' not found in gameScenes. Registered: {string.Join(", ", lvls_dict.Keys)}");

                return null;

            }



            return gameScenes[index];

        }



        public static MySceneManager singleton;

        void Awake()

        {

            singleton = this;

        }

    }



    [System.Serializable]

    public class GameScene

    {

        public string primaryScene;

        public bool isLoaded;

        public string forward;

        public string backward;

        public string lightData;

    }

}


