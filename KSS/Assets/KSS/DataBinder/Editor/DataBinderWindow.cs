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

            using (var horizontal = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Box("Object", GUILayout.ExpandWidth(true));
                GUILayout.Box("Value Type", GUILayout.ExpandWidth(true));
                GUILayout.Box("Binded Key", GUILayout.ExpandWidth(true));
                GUILayout.Space(10);
            }
            using (var vertical = new EditorGUILayout.VerticalScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    scrollPos = scrollView.scrollPosition;

                    for (int i = 0; i < bindableObjs.Length; i++)
                    {
                        using (var horizontal = new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
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
                                key = EditorGUILayout.TextField(bindableObjs[i].Key).Trim();
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


        private BindableObj[] GetBindableObjsInScene()
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
                bindableObjs.AddRange(root.GetComponentsInChildren<BindableObj>(true));
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