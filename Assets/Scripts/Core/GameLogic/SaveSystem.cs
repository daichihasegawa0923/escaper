using System;
using System.Collections.Generic;
using System.IO;
using Escaper.Core.Events;
using UnityEngine;

namespace Escaper.Core.GameLogic
{
    /// <summary>
    /// セーブデータを管理するクラス！めっちゃ安全にデータを保存できるよ！
    /// </summary>
    public class SaveSystem
    {
        private static SaveSystem _instance;
        private string _savePath;
        private Dictionary<string, object> _saveData;

        public static SaveSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SaveSystem();
                }
                return _instance;
            }
        }

        private SaveSystem()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "savedata.json");
            _saveData = new Dictionary<string, object>();
            LoadData();
        }

        /// <summary>
        /// データを保存するよ！
        /// </summary>
        public void SaveData(string key, object data)
        {
            _saveData[key] = data;
            string json = JsonUtility.ToJson(new SerializableDictionary<string, object>(_saveData));
            File.WriteAllText(_savePath, json);
            GameEventSystem.Publish("DataSaved", key);
        }

        /// <summary>
        /// データを読み込むよ！
        /// </summary>
        public T LoadData<T>(string key)
        {
            if (_saveData.TryGetValue(key, out object data))
            {
                return (T)data;
            }
            return default;
        }

        /// <summary>
        /// すべてのデータを読み込むよ！
        /// </summary>
        private void LoadData()
        {
            if (File.Exists(_savePath))
            {
                string json = File.ReadAllText(_savePath);
                var data = JsonUtility.FromJson<SerializableDictionary<string, object>>(json);
                _saveData = data.ToDictionary();
            }
        }

        /// <summary>
        /// データを削除するよ！
        /// </summary>
        public void DeleteData(string key)
        {
            if (_saveData.ContainsKey(key))
            {
                _saveData.Remove(key);
                string json = JsonUtility.ToJson(new SerializableDictionary<string, object>(_saveData));
                File.WriteAllText(_savePath, json);
                GameEventSystem.Publish("DataDeleted", key);
            }
        }

        /// <summary>
        /// すべてのデータを削除するよ！
        /// </summary>
        public void DeleteAllData()
        {
            _saveData.Clear();
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
            }
            GameEventSystem.Publish("AllDataDeleted", null);
        }
    }

    /// <summary>
    /// Dictionaryをシリアライズするためのヘルパークラス！
    /// </summary>
    [Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        public List<TKey> Keys = new List<TKey>();
        public List<TValue> Values = new List<TValue>();

        public SerializableDictionary() { }

        public SerializableDictionary(Dictionary<TKey, TValue> dict)
        {
            foreach (var kvp in dict)
            {
                Keys.Add(kvp.Key);
                Values.Add(kvp.Value);
            }
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            var dict = new Dictionary<TKey, TValue>();
            for (int i = 0; i < Keys.Count; i++)
            {
                dict.Add(Keys[i], Values[i]);
            }
            return dict;
        }
    }
}