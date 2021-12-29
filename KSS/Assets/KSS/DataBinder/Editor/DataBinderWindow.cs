using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KSS.DataBind
{
    public class DataBinderWindow : EditorWindow
    {
        Vector2 scrollPos;
        [SerializeField] string key;

        private void OnEnable()
        {

        }

        private void OnGUI()
        {
            var bindableObjs = GetBindableObjsInScene();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Box("Object Name", GUILayout.Width(position.width / 3));
            GUILayout.Box("Value Type", GUILayout.Width(position.width / 3));
            GUILayout.Box("Binded Key", GUILayout.Width(position.width / 3));
            GUILayout.Box(" ");

            EditorGUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

            EditorGUILayout.BeginVertical();

            var currentEvent = Event.current;
            for (int i = 0; i < bindableObjs.Length; i++)
            {
                EditorGUI.BeginChangeCheck();
                var rect = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(bindableObjs[i].GetAttachedObject().name);
                EditorGUILayout.LabelField(bindableObjs[i].GetRequiredType().ToString());

                key = EditorGUILayout.TextField(bindableObjs[i].Key).Trim();
                EditorGUILayout.EndHorizontal();

                if ((currentEvent.type == EventType.MouseDown || currentEvent.type == EventType.Used) && rect.Contains(currentEvent.mousePosition))
                {
                    Selection.objects = new[] { bindableObjs[i].GetAttachedObject() };
                }
                Debug.Log(currentEvent.type);

                if (EditorGUI.EndChangeCheck())
                {
                    Selection.objects = new[] { bindableObjs[i].GetAttachedObject() };
                    EditorUtility.SetDirty(bindableObjs[i].GetAttachedObject());
                    bindableObjs[i].Key = key;
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            
        }

        
        private IBindableObj[] GetBindableObjsInScene()
        {
            var bindableObjs = new List<IBindableObj>();
            var rootObjs = new List<GameObject>();
            var numOfScenes = SceneManager.sceneCount;


            for (int i = 0; i < numOfScenes; i++)
            {
                rootObjs.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
            }

            foreach (var root in rootObjs)
            {
                bindableObjs.AddRange(root.GetComponentsInChildren<IBindableObj>(true));
            }

            return bindableObjs.ToArray();
        }
        [MenuItem("DataBinder/ShowBindedKeys")]
        public static void ShowBindedKeys()
        {
            var window = GetWindow<DataBinderWindow>("DataBinder", true);
        }
    }
}