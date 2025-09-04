using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TST
{
    public enum SceneType
    {
        None,
        Empty,

        // Content Scenes
        Title,
        Ingame,
    }

    public class Main : SingletonBase<Main>
    {
        //���� �ʱ�ȭ ����
        private bool isInitialized = false;

        private void Start()
        {
            Initialize();
#if UNITY_EDITOR
            Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (activeScene.name.Equals("Main"))
            {
                ChangeScene(SceneType.Title);
            }
#else
            ChangeScene(SceneType.Title);
#endif
        }

        public void Initialize()
        {
            if (isInitialized)
                return;

            isInitialized = true;

            var newEventSystemPrefab = Resources.Load<GameObject>("TST.EventSystem");
            var newInstEventSystem = Instantiate(newEventSystemPrefab);
            DontDestroyOnLoad(newInstEventSystem);

            // �ʿ��� �⺻ �ý��� �ʱ�ȭ
            UIManager.Singleton.Initialize();
            UserDataModel.Singleton.Initialize();
            EffectManager.Singleton.Initialize();
            //SoundManager.Singleton.Initialize();
        }

        public void SetBootStrapperState<T>() where T : SceneBase
        {
            GameObject newSceneBase = new GameObject(typeof(T).Name);
            newSceneBase.transform.SetParent(transform);
            currentSceneController = newSceneBase.AddComponent<T>();
        }

        public void SystemQuit()
        {
            // TODO : ���࿡ ���� ���� ���� �ڵ����� ó���ؾ��� ������ �ִٸ�, ���⼭ ó���� ��.



            // ���� ����.
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #region Main Scene Management Core

        [SerializeField] private SceneType currentScene = SceneType.None;

        public bool IsOnProgressSceneChange { get; private set; } = false;

        public SceneBase CurrentSceneController => currentSceneController;
        private SceneBase currentSceneController;

        public void ChangeScene(SceneType sceneType, bool isForceLoad = false, System.Action onSceneLoadCompleted = null)
        {
            // �̹� ���� ���̸� �����ϵ��� ó��
            if (currentScene == sceneType && false == isForceLoad)
                return;

            switch (sceneType)
            {
                case SceneType.Title:
                    //ChangeScene<TitleScene>(SceneType.Title, onSceneLoadCompleted);
                    currentScene = SceneType.Title;
                    break;
                case SceneType.Ingame:
                    //ChangeScene<IngameMapScene>(SceneType.Ingame, onSceneLoadCompleted);
                    currentScene = SceneType.Ingame;
                    break;
            }
        }

        private void ChangeScene<T>(SceneType sceneType, System.Action onSceneLoadCompleted = null) where T : SceneBase
        {
            if (IsOnProgressSceneChange)
                return;

            StartCoroutine(ChangeSceneAsync<T>(sceneType, onSceneLoadCompleted));
        }

        private IEnumerator ChangeSceneAsync<T>(SceneType sceneType, System.Action onSceneLoadCompleted = null) where T : SceneBase
        {
            IsOnProgressSceneChange = true;
            UIManager.Singleton.HideAllUI();
            //UIManager.Show<LoadingUI>(UIList.LoadingUI);

            // ������ Ȥ�ó� Current SceneBase �� �����Ѵٸ� End ó���� ���ش�.
            if (currentSceneController != null)
            {
                yield return StartCoroutine(currentSceneController.OnEnd());
                Destroy(currentSceneController.gameObject);
                currentSceneController = null;
            }

            // Empty ������ ���� ��ȯ�Ѵ�.
            AsyncOperation asyncToEmpty = SceneManager.LoadSceneAsync(SceneType.Empty.ToString(), LoadSceneMode.Single);
            yield return new WaitUntil(() => asyncToEmpty.isDone);

            // ���ο� ��� ���� SceneBase�� �߰��Ѵ�.
            GameObject newSceneBase = new GameObject(typeof(T).Name);
            newSceneBase.transform.SetParent(transform);
            currentSceneController = newSceneBase.AddComponent<T>();

            // ���ο� ��� ���� SceneBase�� OnStart�� ȣ���Ѵ�.
            yield return StartCoroutine(currentSceneController.OnStart());

            // �ε� UI�� �ݾ��ش�.
            //UIManager.Hide<LoadingUI>(UIList.LoadingUI);

            // �Ѱܹ޾Ҵ� �ݹ� �Լ��� �ҷ��� �ش�.
            onSceneLoadCompleted?.Invoke();
            IsOnProgressSceneChange = false;
        }



        #endregion
    }
}
