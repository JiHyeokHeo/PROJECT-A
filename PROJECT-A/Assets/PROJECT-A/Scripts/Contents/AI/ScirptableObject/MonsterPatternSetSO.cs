using System;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum EPatternID
{
    Rush,
    Smash,
    ColdBeam,
    Melt,
    None,
}

public enum EMonsterID
{
    CopyBara = 10000000,
}

namespace A
{
    // 패턴에 관련된 에셋
    [CreateAssetMenu(menuName = "PROJECT.A/Monster/PatternSetSO", fileName = "PatternSetSO")]
    public class MonsterPatternSetSO : ScriptableObject
    {
        [Header("패턴 기본 정보")]
        public int monsterID;
        public EPatternID PatternID; // 추후 뭐 인트로 바꿉시다
        public bool hasCoolDown;
        public float CoolDown;

        [Tooltip("0 = 독립, 1 이상 = 공유 그룹 ID")]
        public int CooldownGroupId; 
        public float AttackRange;
        [Range(0, 1)] public float Weight = 0.3f; // 가중도 

        // 옵션
        [SerializeReference]
        public SpecificMonsterPatternData AddedData;
    }

    [System.Serializable]
    public class SpecificMonsterPatternData
    {
        // 발동 체력
        [Range(0, 1)] public float executeHpRatio;
        public GameObject ProjectilePrefab;
    }

    [CustomEditor(typeof(MonsterPatternSetSO))]
    public class MonsterPatternSetSOEditor : Editor
    {
        private MonsterPatternSetSO patternTarget;

        private void OnEnable()
        {
            patternTarget = (MonsterPatternSetSO)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (patternTarget.AddedData == null)
            {
                if (GUILayout.Button("Specific Data 추가"))
                {
                    Undo.RecordObject(patternTarget, "Add Specific Data");
                    patternTarget.AddedData = new SpecificMonsterPatternData();
                    EditorUtility.SetDirty(patternTarget);
                }
            }
            else
            {
                if (GUILayout.Button("Specific Data 제거"))
                {
                    Undo.RecordObject(patternTarget, "Remove Specific Data");
                    patternTarget.AddedData = null;
                    EditorUtility.SetDirty(patternTarget);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
