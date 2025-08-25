#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;


namespace A
{
    public class SpineMaterialExchange : EditorWindow
    {
        // 탐지 가능한 프로퍼티/키워드
        static readonly string[] StraightAlphaPropNames = new[]
        {
            "_StraightAlphaInput", "_StraightAlpha", "_StraightAlphaTexture", "_StraightAlphaTex"
        };

        static readonly string[] StraightAlphaKeywords = new[]
        {
            "_STRAIGHT_ALPHA_INPUT", "STRAIGHT_ALPHA_INPUT", "STRAIGHT_ALPHA_TEXTURE"
        };

        enum DropType { Folder, Material }
        class DroppedItem
        {
            public DropType Type;
            public string Path;            // Folder: 폴더 경로 / Material: 에셋 경로
            public Object Obj;             // 폴더면 null일 수 있음
            public bool Include = true;    // 체크 해제 시 스캔 제외
        }

        readonly List<DroppedItem> _dropped = new();
        readonly HashSet<Material> _materials = new();

        Vector2 _scrollDropped, _scrollMats;
        bool _showDropped = true, _showMats = true;

        [MenuItem("Tools/Spine Material StraightAlpha")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(SpineMaterialExchange));
        }

        void OnGUI()
        {
            GUILayout.Label("Spine Straight Alpha Texture/Input Batch Toggle", EditorStyles.boldLabel);

            DrawDropArea(); // 드래그-드랍 영역

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField($"Dropped: {_dropped.Count}  |  Found Materials: {_materials.Count}", GUILayout.MaxWidth(320));
                EditorGUI.BeginDisabledGroup(_materials.Count == 0);
                if (GUILayout.Button("Set ON", GUILayout.Height(22))) ApplyToAll(true);
                if (GUILayout.Button("Set OFF", GUILayout.Height(22))) ApplyToAll(false);
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("Clear", GUILayout.Height(22)))
                {
                    _dropped.Clear();
                    _materials.Clear();
                }
            }

            // 드랍된 원본 목록
            _showDropped = EditorGUILayout.Foldout(_showDropped, "Dropped Items (folders & .mat)");
            if (_showDropped)
            {
                using var sv = new EditorGUILayout.ScrollViewScope(_scrollDropped, GUILayout.MinHeight(120));
                _scrollDropped = sv.scrollPosition;

                for (int i = 0; i < _dropped.Count; i++)
                {
                    var it = _dropped[i];
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        it.Include = EditorGUILayout.Toggle(it.Include, GUILayout.Width(18));

                        if (it.Type == DropType.Material)
                        {
                            EditorGUILayout.ObjectField(it.Obj, typeof(Material), false);
                        }
                        else
                        {
                            EditorGUILayout.LabelField($"[Folder] {it.Path}");
                        }

                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(22)))
                        {
                            _dropped.RemoveAt(i);
                            RebuildMaterialSet();
                            GUI.FocusControl(null);
                            break;
                        }
                    }
                }
            }

            // 스캔된 머티리얼 목록
            _showMats = EditorGUILayout.Foldout(_showMats, "Materials Found");
            if (_showMats)
            {
                using var sv2 = new EditorGUILayout.ScrollViewScope(_scrollMats, GUILayout.MinHeight(160));
                _scrollMats = sv2.scrollPosition;

                foreach (var m in _materials.OrderBy(m => m.name))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.ObjectField(m, typeof(Material), false);
                        GUILayout.Label(HasAnyStraightAlphaProperty(m) ? "✓ StraightAlpha prop" : "— (no prop)", GUILayout.Width(160));
                    }
                }
            }
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // UI: 드래그-드랍 영역 + 처리
        void DrawDropArea()
        {
            var rect = GUILayoutUtility.GetRect(0, 70, GUILayout.ExpandWidth(true));
            GUI.Box(rect, "여기에 폴더나 .mat 파일을 드래그-드랍 하세요");

            var e = Event.current;
            if (!rect.Contains(e.mousePosition)) return;

            if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (e.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    bool changed = false;

                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        var path = AssetDatabase.GetAssetPath(obj);
                        if (string.IsNullOrEmpty(path)) continue;

                        if (AssetDatabase.IsValidFolder(path))
                        {
                            AddFolder(path, ref changed);
                            continue;
                        }

                        if (obj is Material mat)
                        {
                            AddMaterial(path, mat, ref changed);
                            continue;
                        }

                        // 다른 에셋이면 상위 폴더를 등록
                        var parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
                        if (!string.IsNullOrEmpty(parent) && AssetDatabase.IsValidFolder(parent))
                            AddFolder(parent, ref changed);
                    }

                    if (changed) RebuildMaterialSet();
                    e.Use();
                }
            }
        }

        void AddFolder(string folderPath, ref bool changed)
        {
            if (_dropped.Any(d => d.Type == DropType.Folder && d.Path == folderPath)) return;
            _dropped.Add(new DroppedItem { Type = DropType.Folder, Path = folderPath, Include = true });
            changed = true;
        }

        void AddMaterial(string path, Material mat, ref bool changed)
        {
            if (_dropped.Any(d => d.Type == DropType.Material && d.Path == path)) return;
            _dropped.Add(new DroppedItem { Type = DropType.Material, Path = path, Obj = mat, Include = true });
            changed = true;
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // 스캔 & 적용
        void RebuildMaterialSet()
        {
            _materials.Clear();

            // 포함된 항목만 스캔
            var includeFolders = _dropped.Where(d => d.Include && d.Type == DropType.Folder).Select(d => d.Path).ToArray();
            if (includeFolders.Length > 0)
            {
                var guids = AssetDatabase.FindAssets("t:Material", includeFolders);
                foreach (var guid in guids)
                {
                    var p = AssetDatabase.GUIDToAssetPath(guid);
                    var m = AssetDatabase.LoadAssetAtPath<Material>(p);
                    if (m != null) _materials.Add(m);
                }
            }

            foreach (var it in _dropped)
            {
                if (!it.Include || it.Type != DropType.Material) continue;
                var mat = it.Obj as Material;
                if (mat != null) _materials.Add(mat);
            }

            Repaint();
        }

        void ApplyToAll(bool on)
        {
            int total = 0, changed = 0, skipped = 0;

            foreach (var mat in _materials)
            {
                total++;
                if (TrySetStraightAlpha(mat, on)) changed++;
                else skipped++;
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[SpineStraightAlpha] {(on ? "ON" : "OFF")} - Total:{total}, Changed:{changed}, Skipped:{skipped}");
            ShowNotification(new GUIContent($"Done • {changed} changed, {skipped} skipped"));
        }

        static bool TrySetStraightAlpha(Material mat, bool on)
        {
            bool hadProp = false;
            bool edited = false;

            foreach (var prop in StraightAlphaPropNames)
            {
                if (!mat.HasProperty(prop)) continue;
                hadProp = true;

                float newVal = on ? 1f : 0f;
                if (!Mathf.Approximately(mat.GetFloat(prop), newVal))
                {
                    Undo.RecordObject(mat, $"Set {prop} {(on ? "ON" : "OFF")}");
                    mat.SetFloat(prop, newVal);
                    edited = true;
                }
            }

            if (hadProp)
            {
                foreach (var kw in StraightAlphaKeywords)
                    if (on) mat.EnableKeyword(kw); else mat.DisableKeyword(kw);

                if (edited) EditorUtility.SetDirty(mat);
            }
            return hadProp && edited;
        }

        static bool HasAnyStraightAlphaProperty(Material m) =>
            StraightAlphaPropNames.Any(m.HasProperty);
    }
}
#endif
