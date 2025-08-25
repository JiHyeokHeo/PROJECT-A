using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using static TST.UserItemDTO;

namespace TST
{
    public interface ILoader<Key, Value>
    {
        public Dictionary<Key,Value> MakeDict();
    }

    [System.Serializable]
    public class SaveLoadDataWrapper<T> : ILoader<int, T> where T : RootDataDTO
    {
        public List<T> Values = new List<T>();

        public Dictionary<int, T> MakeDict()
        {
            Dictionary<int, T> dict = new Dictionary<int, T>();
            foreach(T value in Values)
                dict.Add(value.ID, value);
            return dict;
        }
    }

    public class UserDataModel : SingletonBase<UserDataModel>
    {
        [field: SerializeField] public Dictionary<int, IngamePlayerDataDTO> IngamePlayerData { get; private set; } = new Dictionary<int, IngamePlayerDataDTO> ();
        [field: SerializeField] public Dictionary<int, IngameMonsterDataDTO> ingameMonsterData { get; private set; } = new Dictionary<int, IngameMonsterDataDTO> ();

        [field: SerializeField] public UserItemDTO UserItemData { get; private set; } = new UserItemDTO();
        //[field: SerializeField] public PlayerEquipmentDTO PlayerEquipmentData { get; private set; } = new PlayerEquipmentDTO();

        //public event System.Action<UserItemDTO.UserItemData> OnUserItemChangedEvent;
        //public event System.Action<ItemEquipmentCategory, int, int> OnPlayerEquipmentChanagedEvent; // BeforeSlotId, AfterSlotId

        public void Initialize()
        {
            IngamePlayerData = LoadData<IngamePlayerDataDTO>().MakeDict();
            ingameMonsterData = LoadData<IngameMonsterDataDTO>().MakeDict();
        }

        public void ChangeData<T>(int id, T data) where T : RootDataDTO
        {
            var dictionary = GetDictionaryForType<T>();
            if (dictionary != null)
            {
                dictionary[id] = data;
                Debug.Log($"Saved data for ID {id} of type {typeof(T).Name}");
            }
            else
            {
                Debug.LogError($"Unsupported type: {typeof(T).Name}");
            }
        }
  
        // �Լ� ��� �߰��ؾ���
        private Dictionary<int, T> GetDictionaryForType<T>() where T : RootDataDTO
        {
            if (typeof(T) == typeof(IngamePlayerDataDTO))
            {
                return IngamePlayerData as Dictionary<int, T>;
            }
            else if (typeof(T) == typeof(IngameMonsterDataDTO))
            {
                return ingameMonsterData as Dictionary<int, T>;
            }

            return null;
        }

        private void SaveAllInGameData()
        {
            // TODO : Dictionary ��� �߰��ɶ����� �þ����
            SaveData(IngamePlayerData);
            //SaveData(ingameMonsterData);
        }

        private void OnDisable()
        {
            // ������ ���� �뵵
            SaveAllInGameData();
        }
        
        //public void AddItemToInventory(ItemData itemData, int count = 1)
        //{
        //    // TODO : UserItemData�� ���� ������ �߰�
        //    // TODO : ������ ���� �������� �ִ°�? ������ ī��Ʈ�� ����, ������ ���� �߰�
        //    // TODO : ������ ���� �������� ������, �ش� ������ Count�� MaxCount ���� �Ѿ�°�? �Ѿ���� ���ο� ���Կ� �߰�
        //    for (int i = 0; i < count; i++) 
        //    {
        //        UserItemDTO.UserItemData changedData = null;
        //        int existedItemDataIndex = UserItemData.Items.FindLastIndex(x => x.itemID.Equals(itemData.ItemID));
        //        if (existedItemDataIndex >= 0)
        //        {
        //            bool isExistGameData = GameDataModel.Singleton.GetItemData(itemData.ItemID, out var itemGameData);

        //            // TODO : ������ ���� �����Ͱ� ���°Ϳ� ���� ����ó��
        //            Assert.IsTrue(isExistGameData, $"ItemData {itemData.ItemID} is not exist in GameDataModel");

        //            int limitStack = itemGameData.ItemMaxStack;
        //            // 5�� �ְ� 7�� �߰� = �ִ� 10��
        //            if (UserItemData.Items[existedItemDataIndex].itemCount + 1 <= limitStack)
        //            {
        //                UserItemData.Items[existedItemDataIndex].itemCount += 1;
        //                changedData = UserItemData.Items[existedItemDataIndex];
        //            }
        //            else
        //            {
        //                changedData = new UserItemDTO.UserItemData()
        //                {
        //                    slotID = UserItemData.Items.Count,
        //                    itemID = itemData.ItemID,
        //                    itemCount = 1
        //                };
        //                UserItemData.Items.Add(changedData);
        //            }
        //        }
        //        else
        //        {
        //            changedData = new UserItemDTO.UserItemData()
        //            {
        //                slotID = UserItemData.Items.Count,
        //                itemID = itemData.ItemID,
        //                itemCount = 1
        //            };
        //            UserItemData.Items.Add(changedData);
        //        }

        //        // TODO : ������ ����
        //        // TODO : UserDataModel �� OnUserItemChangedEvent �� ȣ��������.
        //        OnUserItemChangedEvent?.Invoke(changedData);
        //    }
        //}

        
        //public bool UseInventoryItem(ItemData itemData, int useCount)
        //{
        //    var itemDataTemp = UserItemData.Items.Find(x => x.itemID.Equals(itemData.ItemID));
        //    if (itemDataTemp == null)
        //        return false;

        //    bool isSucceed = RecursiveSearch(itemData, useCount);

        //    return true;
        //}

        //public void UnEquipItem(ItemEquipmentCategory category, int slotId)
        //{
        //    if (PlayerEquipmentData.equipmentItems[category] != slotId)
        //        return;

        //    int beforeSlotID = slotId;
        //    int afterSlotID = -1;
        //    PlayerEquipmentData.equipmentItems[category] = -1;

        //    // �÷��̾� ��� ���� �̺�Ʈ �߻�
        //    OnPlayerEquipmentChanagedEvent.Invoke(category, beforeSlotID, afterSlotID);
        //}

        //// ������ ������ ��� ���Ƴ����� �ִ�.!
        //public void EquipItem(ItemEquipmentCategory category, int slotId)
        //{
        //    int beforeSlotID = -1;
        //    int afterSlotID = slotId;

        //    if (PlayerEquipmentData.equipmentItems[category] >= 0) // ������ ������ �������� �ִ� ���
        //    {
        //        // UnEquip Item
        //        beforeSlotID = PlayerEquipmentData.equipmentItems[category];
        //        PlayerEquipmentData.equipmentItems[category] = -1;
        //    }

        //    // ���� ���޹��� �κ��丮 SlotId ���� EquipData�� ���� �����
        //    PlayerEquipmentData.equipmentItems[category] = slotId;

        //    // �÷��̾� ��� ���� �̺�Ʈ �߻�
        //    OnPlayerEquipmentChanagedEvent?.Invoke(category, beforeSlotID, afterSlotID);
        //}

        //private bool RecursiveSearch(ItemData itemData, int useCount)
        //{
        //    int existedItemDataIndex = UserItemData.Items.FindLastIndex(x => x.itemID.Equals(itemData.ItemID));

        //    UserItemDTO.UserItemData changedData = null;

        //    if (existedItemDataIndex < 0)
        //        return false;

        //    if (existedItemDataIndex >= 0)
        //    {
        //        bool isExistGameData = GameDataModel.Singleton.GetItemData(itemData.ItemID, out var itemGameData);

        //        changedData = UserItemData.Items[existedItemDataIndex];
        //        Assert.IsTrue(isExistGameData, $"ItemData {itemData.ItemID} is not exist in GameDataModel");

        //        int minimumZone = 0;
        //        if (UserItemData.Items[existedItemDataIndex].itemCount - useCount > minimumZone)
        //        {
        //            UserItemData.Items[existedItemDataIndex].itemCount -= useCount;

        //            changedData = UserItemData.Items[existedItemDataIndex];
        //        }
        //        else
        //        {
        //            int maxcnt = UserItemData.Items[existedItemDataIndex].itemCount;

        //            // ������ ���� �� 
        //            int remainCount = UserItemData.Items[existedItemDataIndex].itemCount - useCount;
        //            // �ϴ� ����� �ε��� ��ŭ�� ����.
        //            UserItemData.Items[existedItemDataIndex].itemCount = 0;
        //            changedData = UserItemData.Items[existedItemDataIndex];

        //            // �ϴ� ������ �� �����Ϳ� ���� ���� ������
        //            OnUserItemChangedEvent?.Invoke(changedData);

        //            UserItemData.Items.RemoveAt(existedItemDataIndex);
        //            RecursiveSearch(itemData, -remainCount); 
        //        }
        //    } 

        //    if (existedItemDataIndex >= 0)
        //    { 
        //        OnUserItemChangedEvent?.Invoke(changedData);
        //        return true;
        //    }

        //    return false;
        //}

        #region SAVE / LOAD Core Method
        private SaveLoadDataWrapper<T> LoadData<T>() where T : RootDataDTO
        {
#if UNITY_EDITOR
            string path = $"Assets/PROJECT-A/Anothers/Editor Saved Data/Json/{typeof(T).Name}.json";
#else
            string path = $"{Application.persistentDataPath}/{typeof(T).Name}.json";
#endif
            if (FileManager.ReadFileData(path, out string loadedEditorData))
            {
                // JSON ������ȭ
                var wrapper = JsonConvert.DeserializeObject<SaveLoadDataWrapper<T>>(loadedEditorData);

                return wrapper;
            }
            
            Debug.Log($"Failed to Load Data {typeof(T).Name}");
            return null;
        }

        private void SaveData<T>(Dictionary<int, T> newData) where T : RootDataDTO
        {
#if UNITY_EDITOR
            string jsonPath = $"Assets/PROJECT-A/Anothers/Editor Saved Data/Json/{typeof(T).Name}.json";
            string csvPath = $"Assets/PROJECT-A/Anothers/Editor Saved Data/Csv/{typeof(T).Name}.csv";
#else
            string jsonPath = $"{Application.persistentDataPath}/{typeof(T).Name}.json";
            string csvPath = $"{Application.persistentDataPath}/{typeof(T).Name}.csv";
#endif
            // JSON ����
            SaveLoadDataWrapper<T> wrapper = new SaveLoadDataWrapper<T>();
            foreach (var dic in newData)
            {
                wrapper.Values.Add(newData[dic.Key]);
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new ParentFirstContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new Vector3Converter(),
                    new QuaternionConverter()
                },
                Formatting = Formatting.Indented
            };

            if (wrapper == null || !wrapper.Values.Any())
                return;

            var jsonData = JsonConvert.SerializeObject(wrapper, settings);
            FileManager.WriteFileFromString(jsonPath, jsonData);
            Debug.Log($"Save Data to JSON Success: {jsonData}");

            // CSV ����
            if (SaveToCsv(wrapper.Values, csvPath))
                Debug.Log($"Save Data to CSV Success: {csvPath}");
        }

        private static bool SaveToCsv<T>(IEnumerable<T> dataCollection, string filePath) where T : RootDataDTO
        {
            if (dataCollection == null || !dataCollection.Any())
            {
                Debug.LogError("Data collection is null or empty.");
                return false;
            }

            var csvBuilder = new StringBuilder();

            // �θ� Ŭ�������� ���������� �Ӽ� ����
            var properties = GetPropertiesInHierarchy(typeof(T));

            // ��� ����
            csvBuilder.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // ������ �߰�
            foreach (var data in dataCollection)
            {
                var values = properties.Select(p =>
                {
                    var value = p.GetValue(data);

                    if (value == null)
                        return ""; // Null ���� �� ���ڿ��� ó��

                    if (value is IEnumerable enumerable && !(value is string))
                    {
                        return $"\"{string.Join("&", enumerable.Cast<object>())}\""; // ����Ʈ�� '&'�� ����
                    }
                    else if (value is Vector3 vector)
                    {
                        return $"\"({vector.x},{vector.y},{vector.z})\""; // Vector3 ����
                    }
                    else if (value is Quaternion quaternion)
                    {
                        return $"\"({quaternion.x},{quaternion.y},{quaternion.z},{quaternion.w})\""; // Quaternion ����
                    }
                    else
                    {
                        return value?.ToString()?.Replace(",", " ").Replace("\"", "\"\""); // ��ǥ�� ū����ǥ �̽�������
                    }
                });

                csvBuilder.AppendLine(string.Join(",", values));
            }

            // ���� ����
            try
            {
                FileManager.WriteFileFromString(filePath, csvBuilder.ToString());
            }
            catch (IOException ex)
            {
                Debug.LogError($"File is locked or cannot be accessed: {filePath}. Error: {ex.Message}");
                return false;
            }

            return true;
        }

        // �θ� Ŭ�������� �Ӽ� ����
        private static List<FieldInfo> GetPropertiesInHierarchy(Type type)
        {
            var properties = new List<FieldInfo>();
            var types = new List<Type>();

            while (type != null && type != typeof(object))
            {
                types.Add(type);
                type = type.BaseType; // �θ� Ŭ������ �̵�
            }

            // �θ���� ������
            types.Reverse();

            for (int i = 0; i < types.Count; i++)
            {
                properties.AddRange(types[i].GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            }

            return properties;
        }
 
        #endregion

        #region Serialize & Deserialize & FindParentProperty
        public class ParentFirstContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var properties = base.CreateProperties(type, memberSerialization);

                // �θ� Ŭ������ �Ӽ��� ���� ����
                return properties
                    .OrderBy(p => GetInheritanceDepth(p.DeclaringType)) // ��� ���̿� ���� ����
                    .ThenBy(p => p.Order ?? int.MaxValue) // JsonProperty(Order) �Ӽ� ����
                    .ToList();
            }

            private int GetInheritanceDepth(Type type)
            {
                int depth = 0;
                while (type.BaseType != null)
                {
                    depth++;
                    type = type.BaseType;
                }
                return depth;
            }
        }

        public class Vector3Converter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var vector = (Vector3)value;
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(vector.x);
                writer.WritePropertyName("y");
                writer.WriteValue(vector.y);
                writer.WritePropertyName("z");
                writer.WriteValue(vector.z);
                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var obj = JObject.Load(reader);
                return new Vector3(
                    (float)obj["x"],
                    (float)obj["y"],
                    (float)obj["z"]
                );
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Vector3);
            }
        }

        public class QuaternionConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var quaternion = (Quaternion)value;
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(quaternion.x);
                writer.WritePropertyName("y");
                writer.WriteValue(quaternion.y);
                writer.WritePropertyName("z");
                writer.WriteValue(quaternion.z);
                writer.WritePropertyName("w");
                writer.WriteValue(quaternion.w);
                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var obj = JObject.Load(reader);
                return new Quaternion(
                    (float)obj["x"],
                    (float)obj["y"],
                    (float)obj["z"],
                    (float)obj["w"]
                );
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Quaternion);
            }
        }
        #endregion
    }
}
