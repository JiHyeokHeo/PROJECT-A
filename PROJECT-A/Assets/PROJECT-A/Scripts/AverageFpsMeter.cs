// AverageFpsMeter.cs
// ���̱⸸ �ϸ� ����. Editor/DevBuild���� F1�� HUD On/Off.
// windowSeconds ������ "��Ȯ ��� FPS"�� ���� ����� ����մϴ�.
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[DisallowMultipleComponent]
public class AverageFpsMeter : MonoBehaviour
{
    [Header("Average Window")]
    [Tooltip("�ֱ� N�� ���� ��� FPS ���")]
    [SerializeField, Min(0.1f)] float windowSeconds = 5f;

    [Header("HUD (Editor/DevBuild)")]
    [SerializeField] bool showHud = true;
    [SerializeField] KeyCode toggleKey = KeyCode.F1;
    [SerializeField] Vector2 hudAnchor = new Vector2(10, 10);
    [SerializeField, Min(0.05f)] float hudRefreshInterval = 0.25f;

    // ���� ����(�����̵� ������)
    readonly Queue<float> _q = new Queue<float>(256);
    float _sum; // ������ �� dt ��

    // ���� ����
    int _totalFrames;
    float _totalTime;

    // ��� ��� ĳ��
    public float WindowAverageFps { get; private set; }
    public float WindowAverageMs { get; private set; }
    public float SessionAverageFps { get; private set; }
    public float SessionAverageMs { get; private set; }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
    GUIStyle _style;
    readonly StringBuilder _sb = new StringBuilder(128);
    string _cachedHudText = "";
    float _hudTimer;
#endif

    void Awake()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        _style = new GUIStyle()
        {
            alignment = TextAnchor.UpperLeft,
            fontSize = 13,
            richText = false
        };
#endif
        DontDestroyOnLoad(gameObject); // ���ϸ� �����ϼ���
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        _totalTime += dt;
        _totalFrames++;

        // �����̵� ������ ����(�ð����� ��Ȯ ���)
        _q.Enqueue(dt);
        _sum += dt;
        while (_sum > windowSeconds && _q.Count > 1)
            _sum -= _q.Dequeue();

        // ��� ���
        if (_sum > 0f && _q.Count > 0)
        {
            WindowAverageFps = _q.Count / _sum;
            WindowAverageMs = (_sum / _q.Count) * 1000f;
        }
        else
        {
            WindowAverageFps = 0f;
            WindowAverageMs = 0f;
        }

        if (_totalTime > 0f && _totalFrames > 0)
        {
            SessionAverageFps = _totalFrames / _totalTime;
            SessionAverageMs = (_totalTime * 1000f) / _totalFrames;
        }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (Input.GetKeyDown(toggleKey)) showHud = !showHud;

        // HUD �ؽ�Ʈ�� ���� �ֱ�θ� ����(�Ҵ� ���̱�)
        _hudTimer += dt;
        if (_hudTimer >= hudRefreshInterval)
        {
            _hudTimer = 0f;
            _sb.Length = 0;
            _sb.Append("Avg FPS (").Append(windowSeconds.ToString("0.0")).Append("s): ")
              .Append(WindowAverageFps.ToString("F1")).Append("  | ms: ").Append(WindowAverageMs.ToString("F1")).Append('\n')
              .Append("Session : ").Append(SessionAverageFps.ToString("F1")).Append("  | ms: ").Append(SessionAverageMs.ToString("F1")).Append('\n')
              .Append("Toggle  : ").Append(toggleKey);
            _cachedHudText = _sb.ToString();
        }
#endif
    }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
    void OnGUI()
    {
        if (!showHud) return;
        var content = new GUIContent(_cachedHudText);
        Vector2 size = _style.CalcSize(content);
        Rect r = new Rect(hudAnchor.x, hudAnchor.y, size.x + 12f, size.y + 12f);
        GUI.Box(r, content, _style);
    }
#endif

    // �ܺο��� ��� �ʱ�ȭ�ϰ� ���� �� ȣ��
    public void ResetAverages()
    {
        _q.Clear();
        _sum = 0f;
        _totalFrames = 0;
        _totalTime = 0f;
        WindowAverageFps = WindowAverageMs = SessionAverageFps = SessionAverageMs = 0f;
    }
}