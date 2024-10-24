using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEditorInternal;
using UnityEditor.Compilation;

namespace SAS.Utilities.TagSystem.Editor
{
    [InitializeOnLoad]
    public class TagGenerator : EditorWindow
    {
        private static List<string> _enumValues = new List<string>();
        private static ReorderableList _tagsList;

        static TagGenerator()
        {
            EditorApplication.delayCall += GenerateEnumFile;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
        }

        private static void OnAssemblyCompilationFinished(string assemblyPath, CompilerMessage[] messages)
        {
            bool hasErrors = false;

            foreach (var message in messages)
            {
                if (message.type == CompilerMessageType.Error)
                {
                    hasErrors = true;
                    Debug.LogError($"Compilation Error in {assemblyPath}: {message.message}");
                }
            }

            if (hasErrors)
            {
                GenerateEnumFile();
            }
        }

        [MenuItem("Assets/Create/SAS/Tag Generator")]
        private static void ShowWindow()
        {
            GetWindow<TagGenerator>("Tag Generator");
            _enumValues = new List<string>();

            foreach (string name in Enum.GetNames(typeof(Tag)))
            {
                _enumValues.Add(name);
            }

            CreateReorderableList();
        }

        static private void CreateReorderableList()
        {
            _tagsList = new ReorderableList(_enumValues, typeof(Tag), false, true, true, true);
            _tagsList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Tag Values");
            };

            _tagsList.onAddCallback = (list) =>
            {
                _enumValues.Insert(_enumValues.Count(), GetUniqueName("Value", _enumValues));
            };

            _tagsList.onRemoveCallback = (list) =>
            {
                if (list.index > 0)
                {
                    _enumValues.RemoveAt(list.index);
                }
            };

            _tagsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (_enumValues.Count > 0)
                {
                    if (index == 0)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        _enumValues[index] = EditorGUI.TextField(rect, $"Value {index + 1}", _enumValues[index]);
                        EditorGUI.EndDisabledGroup();
                    }
                    else
                    {
                        var currValue = _enumValues[index];
                        var newValue = EditorGUI.DelayedTextField(rect, $"Value {index + 1}", currValue);
                        if (newValue != currValue)
                        {
                            newValue = GetUniqueName(newValue, _enumValues);
                            _enumValues[index] = newValue;
                        }
                    }
                }
            };
        }

        private void OnGUI()
        {
            if (_tagsList == null)
            {
                _enumValues = new List<string>();

                foreach (string name in Enum.GetNames(typeof(Tag)))
                {
                    _enumValues.Add(name);
                }

                CreateReorderableList();
            }
            _tagsList?.DoLayoutList();
            if (GUILayout.Button("Save Tag File"))
            {
                Save();
            }
        }

        private static void GenerateEnumFile()
        {
            EditorApplication.delayCall -= GenerateEnumFile;

            string enumName = "Tag";
            var folderPath = "Assets/SASTag";
            var tagEnumFilePath = $"{folderPath}/{enumName}.cs";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string assemblyDefinitionRefName = "UtilitiesAssemblyDefinitionRef";
            string assemblyDefinitionRefPath = $"{folderPath}/{assemblyDefinitionRefName}.asmref";

            if (!File.Exists(assemblyDefinitionRefPath))
            {
                EmptyTempTagFile("TempTag");
                string guid = GetAssemblyUtilitiesDefinitionAssetGuid();
                string content = "{\r\n    \"reference\":\"GUID:" + guid + "\"\r\n}";


                File.WriteAllText(assemblyDefinitionRefPath, content);
                AssetDatabase.Refresh();
                Debug.Log("Tag file generated at: " + assemblyDefinitionRefPath);
            }
            else
                EmptyTempTagFile("TempTag");



            if (!File.Exists(tagEnumFilePath))
            {
                File.WriteAllText(tagEnumFilePath, string.Empty);
                File.WriteAllText(tagEnumFilePath, GetFileContent(enumName, new List<string>()));
                AssetDatabase.Refresh();
                Debug.Log("Tag file generated at: " + tagEnumFilePath);
            }
        }

        private static void Save()
        {
            string enumName = "Tag";
            var basePath = "Assets/SASTag/";
            var filePath = $"{basePath}{enumName}.cs";

            File.WriteAllText(filePath, string.Empty);
            File.WriteAllText(filePath, GetFileContent(enumName, _enumValues));
            AssetDatabase.Refresh();
        }


        private static string GetFileContent(string enumName, List<string> enumValues)
        {
            if (!enumValues.Contains("None"))
            {
                enumValues.Insert(0, "None");
            }

            string namespaceName = "SAS.Utilities.TagSystem";

            string content = "namespace " + namespaceName + "\n";
            content += "{\n\t";
            content += "public enum " + enumName + "\n";
            content += "\t{\n";

            foreach (string value in enumValues)
            {
                content += "\t\t" + value + ",\n";
            }

            content += "\t}\n}";

            return content;
        }

        private static string GetUniqueName(string nameBase, List<string> usedNames)
        {
            nameBase = nameBase.Replace(" ", "");
            string name = nameBase;
            int counter = 1;
            while (usedNames.Contains(name.Trim()))
            {
                name = nameBase + counter;
                counter++;
            }
            return name;
        }

        private static string GetAssemblyUtilitiesDefinitionAssetGuid()
        {
            string[] guids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset");

            if (guids.Length == 0)
            {
                Debug.Log("No Assembly Definition Assets found in the project.");
                return "";
            }

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                AssemblyDefinitionAsset assemblyDefinition = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assetPath);

                if (assemblyDefinition != null && assemblyDefinition.name.Equals("AssemblyUtilities"))
                {
                    return guid;
                }
            }
            return "";
        }

        private static void EmptyTempTagFile(string fileName)
        {
            string[] guids = AssetDatabase.FindAssets(fileName);

            if (guids.Length == 0)
            {
                Debug.Log("No file found with the provided name.");
                return;
            }

            foreach (string guid in guids)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(guid);
                string fileContents = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(fileContents))
                {
                    File.WriteAllText(filePath, String.Empty);
                    Debug.Log("Empty TempTag File: " + filePath);
                }
                return;
            }
        }

    }
}
