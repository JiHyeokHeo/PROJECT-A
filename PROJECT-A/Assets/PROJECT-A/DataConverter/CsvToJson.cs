using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Tory.Editor
{
#if UNITY_EDITOR
    public class CsvToJson : EditorWindow
    {
        // Add a menu item named "Do Something" to MyMenu in the menu bar.
        [MenuItem("Tools/CsvToJson %#K")]
        public static void CsvDataConvertToJson()
        {
            // TODO : CSV 파일 추가 될때마다 추가

            
            Read("IngamePlayerDataDTO");
        }

        #region Simple CSV Reader
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        public static List<Dictionary<string, object>> Read(string fileName)
        {
            // 추후 데이터 이걸로 변경
            //persistentDataPath
            string checkDataPath = $"Assets/PROJECT-A/Anothers/Editor Saved Data/Json/{fileName}.json";
            TextAsset data = (TextAsset)AssetDatabase.LoadAssetAtPath(checkDataPath, typeof(TextAsset));
            if (data != null)
            {
                Debug.LogWarning("파일이 존재합니다");
                return null;
            }

            var list = new List<Dictionary<string, object>>();

            string dataPath = $"Assets/PROJECT-A/Anothers/Editor Saved Data/Csv/{fileName}.csv";

            data = (TextAsset)AssetDatabase.LoadAssetAtPath(dataPath, typeof(TextAsset));

            if (data == null)
            {
                Debug.LogWarning("파일을 로드하지 못했습니다");
                return null;
            }

            var lines = Regex.Split(data.text, LINE_SPLIT_RE);

            if (lines.Length <= 1) return list;

            var header = Regex.Split(lines[0], SPLIT_RE);
            for (var i = 1; i < lines.Length; i++)
            {

                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "") continue;

                var entry = new Dictionary<string, object>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    // 공백 제거
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                    object finalvalue = value;
                    int n;
                    float f;
                    if (int.TryParse(value, out n))
                    {
                        finalvalue = n;
                    }
                    else if (float.TryParse(value, out f))
                    {
                        finalvalue = f;
                    }

                    // csv 파일에 & 데이터를 넣을 시 리스트형으로 변환 // 포지션이냐 rotation 키워드가 포함 될 시  이걸 추후 Vec이나 Qua 이라는 키워드로 변경 하는 것도 좋을듯
                    if (header[j].Contains("Vec", StringComparison.OrdinalIgnoreCase))
                    {
                        entry[header[j]] = ParseVectorOrQuaternion(value, true);
                    }
                    else if (header[j].Contains("Quat", StringComparison.OrdinalIgnoreCase))
                    {
                        entry[header[j]] = ParseVectorOrQuaternion(value, false);
                    }
                    else if (value.Contains("&"))
                    {
                        entry[header[j]] = value.Split('&');
                    }
                    else
                    {
                        entry[header[j]] = finalvalue;
                    }
                }
                list.Add(entry);
            }


            string filePath = Application.dataPath;
            string jsonOutputPath = $"/PROJECT-A/Anothers/Editor Saved Data/Json/{fileName}.json";
            filePath += jsonOutputPath;

            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new Vector3Converter(),
                    new QuaternionConverter()
                }
            };

            var jsonObject = new Dictionary<string, object>
            {
                { "Values", list }
            };

            string json = JsonConvert.SerializeObject(jsonObject, Formatting.Indented, settings);
            File.WriteAllText(filePath, json);

            Debug.Log($"Convert Excel To Json : {filePath}");

            return list;
        }

        private static object ParseVectorOrQuaternion(string value, bool isVector)
        {
            // "x,y,z" 또는 "x,y,z,w" 형식인지 확인
            // () 삭제
            value = Regex.Replace(value, @"[()]", "");
            string[] components = value.Split(',');

            if (components.Length == 3) // Vector3
            {
                return new Vector3(
                    float.Parse(components[0]),
                    float.Parse(components[1]),
                    float.Parse(components[2])
                );
            }
            else if (components.Length == 4) // Quaternion
            {
                return new Quaternion(
                    float.Parse(components[0]),
                    float.Parse(components[1]),
                    float.Parse(components[2]),
                    float.Parse(components[3])
                );
            }
            else
            {
                if (isVector)
                {
                    return Vector3.zero;
                }
                else
                {
                    return Quaternion.identity;
                }
                
            }
        }
        #endregion

    }
#endif

    #region Vector3, QuaternionConverter
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("z");
            writer.WriteValue(value.z);
            writer.WriteEndObject();
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Vector3(
                (float)obj["x"],
                (float)obj["y"],
                (float)obj["z"]
            );
        }
    }

    public class QuaternionConverter : JsonConverter<Quaternion>
    {
        public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("z");
            writer.WriteValue(value.z);
            writer.WritePropertyName("w");
            writer.WriteValue(value.w);
            writer.WriteEndObject();
        }

        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Quaternion(
                (float)obj["x"],
                (float)obj["y"],
                (float)obj["z"],
                (float)obj["w"]
            );
        }
    }
    #endregion
}
