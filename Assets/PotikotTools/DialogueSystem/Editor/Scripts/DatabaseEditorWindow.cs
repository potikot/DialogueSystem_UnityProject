using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class DatabaseEditorWindow : EditorWindow
    {
        private string _dialogueId = "";
        private string _tag = "";
        private Database _database;

        [MenuItem("Tools/DialogueSystem/Database")]
        public static void ShowWindow()
        {
            GetWindow<DatabaseEditorWindow>("Database Tools");
        }

        private void OnEnable()
        {
            _database = Components.Database;
        }

        private void OnGUI()
        {
            GUILayout.Label("Dialogue Operations", EditorStyles.boldLabel);
            
            // Поле для ввода dialogueId и кнопка LoadDialogueAsync
            _dialogueId = EditorGUILayout.TextField("Dialogue ID", _dialogueId);
            if (GUILayout.Button("Load Dialogue Async"))
            {
                Task.Run(async () => 
                {
                    DL.Log("F");
                    bool result = await _database.LoadDialogueAsync(_dialogueId);
                    Debug.Log(result ? "Loaded successfully!" : "Failed to load!");
                });
            }

            EditorGUILayout.Space();

            // Поле для ввода tag и кнопка LoadDialogueGroupAsync
            _tag = EditorGUILayout.TextField("Tag", _tag);
            if (GUILayout.Button("Load Dialogue Group Async"))
            {
                Task.Run(async () => 
                {
                    DL.Log("G");
                    bool result = await _database.LoadDialogueGroupAsync(_tag);
                    Debug.Log(result ? "Group loaded successfully!" : "Failed to load group!");
                });
            }
        }
    }
}