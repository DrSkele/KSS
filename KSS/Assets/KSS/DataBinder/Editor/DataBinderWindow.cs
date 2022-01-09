using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KSS.DataBind
{
    public class DataBinderWindow : EditorWindow
    {
        Vector2 scrollPos;
        [SerializeField] string searchInput = string.Empty;
        [SerializeField] string replaceInput = string.Empty;
        [SerializeField] bool doExpand = false;

        private void OnEnable()
        {

        }

        private void OnGUI()
        {
            

            using (var findAndReplace = new EditorGUILayout.VerticalScope())
            {
                using (var searchBar = new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(5);
                    doExpand = GUILayout.Toggle(doExpand, "", EditorStyles.foldout, GUILayout.Width(20));
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var input = EditorGUILayout.TextField(searchInput, GUILayout.ExpandWidth(true)).Trim();
                        if (check.changed)
                        {
                            searchInput = input;
                        }
                    }
                }
                if (doExpand)
                {
                    using (var replaceBar = new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Space(35);
                        var replacer = EditorGUILayout.TextField(replaceInput, GUILayout.ExpandWidth(true)).Trim();
                        bool buttonPush = GUILayout.Button("Change All");

                        if (buttonPush && !string.IsNullOrEmpty(searchInput) && !string.IsNullOrEmpty(replacer))
                        {
                            ChangeKey(searchInput, replacer);
                        }
                    }
                }
            }
            using (var definition = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Box("Object", GUILayout.ExpandWidth(true));
                GUILayout.Box("Value Type", GUILayout.ExpandWidth(true));
                GUILayout.Box("Binded Key", GUILayout.ExpandWidth(true));
                GUILayout.Space(10);
            }
            var bindableObjs = GetBindableObjsInScene(searchInput);
            using (var vertical = new EditorGUILayout.VerticalScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    scrollPos = scrollView.scrollPosition;

                    for (int i = 0; i < bindableObjs.Length; i++)
                    {
                        using (var row = new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                        {
                            string objectFieldName = "object_" + i;
                            using (new EditorGUI.DisabledScope(false))
                            {
                                GUI.SetNextControlName(objectFieldName);//Applies given name to Next GUI Field. In this case, ObjectField.
                                EditorGUILayout.ObjectField("", bindableObjs[i], typeof(GameObject), true);
                            }
                            EditorGUILayout.LabelField(bindableObjs[i].GetRequiredType().ToString());
                            string textFieldName = "bindedKey_" + i;
                            using (var check = new EditorGUI.ChangeCheckScope())//check if the key is changed.
                            {
                                GUI.SetNextControlName(textFieldName);
                                var key = EditorGUILayout.TextField(bindableObjs[i].Key).Trim();
                                if (check.changed)
                                {
                                    Undo.RecordObject(bindableObjs[i], nameof(BindableObj));
                                    bindableObjs[i].Key = key;
                                }
                            }
                            if (((EditorGUIUtility.editingTextField && GUI.GetNameOfFocusedControl().Equals(textFieldName)) 
                                || GUI.GetNameOfFocusedControl().Equals(objectFieldName))
                                && Selection.activeGameObject != bindableObjs[i].gameObject )//check if objectfield or key textfield is selected.
                            {
                                EditorGUIUtility.PingObject(bindableObjs[i].gameObject);//show on hierarchy and inspector.
                                Selection.objects = new[] { bindableObjs[i].gameObject };
                            }
                        }
                    }
                }
            }
        }


        private BindableObj[] GetBindableObjsInScene(string key)
        {
            var bindableObjs = new List<BindableObj>();
            var rootObjs = new List<GameObject>();
            var numOfScenes = SceneManager.sceneCount;

            for (int i = 0; i < numOfScenes; i++)
            {
                rootObjs.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
            }

            foreach (var root in rootObjs)
            {
                var childBindables = root.GetComponentsInChildren<BindableObj>(true);
                bindableObjs.AddRange((string.IsNullOrEmpty(key))? 
                    childBindables : childBindables.Where(bindable => bindable.Key.Contains(key)));
            }

            return bindableObjs.ToArray();
        }

        private void ChangeKey(string key, string replacer)
        {
            var bindablesWithKey = GetBindableObjsInScene(key);

            Debug.Log(bindablesWithKey.Count());

            foreach (var bindable in bindablesWithKey)
            {
                Undo.RecordObject(bindable, nameof(BindableObj));
                bindable.Key = replacer;
            }
        }

        [MenuItem("DataBinder/ShowBindedKeys")]
        public static void ShowBindedKeys()
        {
            var window = GetWindow<DataBinderWindow>("DataBinder", true);
        }
    }
}