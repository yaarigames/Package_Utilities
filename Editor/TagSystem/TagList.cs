using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SAS.TagSystem.Editor
{
    [CreateAssetMenu(fileName = "Tag List", menuName = "SAS/Tag List")]
    [System.Serializable]
    public class TagList : ScriptableObject
    {
        public const string KeysIdentifier = "Key List";
        public string[] values = new string[] { };

        public static TagList Instance(string assetName = "Tag List")
        {
            var basePath = "Assets/Editor Default Resources/TagList/";
            var filePath = $"Assets/Editor Default Resources/TagList/{assetName}.asset";
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            var asset = AssetDatabase.LoadAssetAtPath<TagList>(filePath);
            if (asset == null)
            {
                asset = CreateInstance<TagList>();
                AssetDatabase.CreateAsset(asset, $"{filePath}");
                AssetDatabase.SaveAssets();
            }

            var tagListAssets = EditorGUIUtility.Load(filePath) as TagList;
            return tagListAssets;
        }

        public static string[] GetList(string assetName = "Tag List")
        {
            return Instance(assetName).values;
        }

        public void Add(string value, string assetName = "Tag List")
        {
            if (Array.IndexOf(values, value) == -1)
                values = values.Concat(new string[] { value }).ToArray();
        }

        public void Remove(int index)
        {
            values = values.Where(e => e != values[index]).ToArray();
        }

        public void AddRange(List<string> values, string assetName = "Tag List")
        {
            this.values = this.values.Concat(values).ToArray();
            this.values = this.values.Distinct().ToArray();
            EditorUtility.SetDirty(this);
        }
    }
}
