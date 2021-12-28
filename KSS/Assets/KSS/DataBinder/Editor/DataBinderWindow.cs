using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.IMGUI.Controls;

namespace KSS.DataBind
{
    public class DataBinderWindow : EditorWindow
    {
        bool initialized = false;
        BindedTreeView view;
        private void OnEnable()
        {
            view = new BindedTreeView(null, new TreeViewState());
        }
        private void OnGUI()
        {
            var objs = GetBindableObjsInScene();
            Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
            view.OnGUI(rect);
        }
        private void Initialize()
        {
            
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

    class BindedTreeView : UnityEditor.IMGUI.Controls.TreeView
    {
        IBindableObj[] objs;
        public BindedTreeView(IBindableObj[] bindableObjs, TreeViewState treeViewState) : base(treeViewState) 
        {
            objs = bindableObjs;
            Reload(); 
        }
        protected override TreeViewItem BuildRoot()
        {
            int uniqueId = 0;

            var root = new TreeViewItem { id = uniqueId++, depth = -1, displayName = nameof(IBindableObj) };

            var bindableObj = new TreeViewItem { id = uniqueId++, depth = 0, displayName = nameof(BindableObj) };
            var alwaysBindedObj = new TreeViewItem { id = uniqueId++, depth = 0, displayName = nameof(AlwaysBindedObj) };

            root.AddChild(bindableObj);
            root.AddChild(alwaysBindedObj);



            return root;
        }
    }
}