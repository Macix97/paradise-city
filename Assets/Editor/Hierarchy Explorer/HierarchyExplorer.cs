using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace HierarchyExplorer
{

    public class HierarchyExplorer : EditorWindow
    {
        public enum SearchMode
        {
            ByName,
            ByTag,
            ByType,
            ByLayer,
            ByInstanceID,
        }

        public enum NameSearchMode
        {
            Exact,
            CaseInsensitive,
            SimpleRegex,
            Regex,
        }

        private static EditorWindow editorWindow;
        private static string typedName = string.Empty;

        private static NameSearchMode nameSearchMode;
        private static SearchMode searchMode;
        private static bool includeDisabled = false;
        private static bool includeSelfActive = true;

        private static string message;
        private static MessageType messageType;

        private static HashSet<Transform> cachedObjectsList;
        private static bool hierarchyChanged; //has the Hierarchy changed since the last search?

        [MenuItem("Tools/Hierarchy Explorer/Explorer %&e")]
        public static void HierarchyExplorerMain()
        {
            editorWindow = GetWindow(typeof(HierarchyExplorer));
            editorWindow.titleContent.text = "Hierarchy Explorer";
            editorWindow.minSize = new Vector2(256, 256);
            editorWindow.maxSize = new Vector2(1024, 512);

            searchMode = SearchMode.ByName;
            message = string.Empty;
            messageType = MessageType.None;

            hierarchyChanged = true;
            EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
        }

        [MenuItem("Tools/Hierarchy Explorer/Invert selection %&i")]
        public static void HierarchyExplorer_InvertSelection()
        {
            InvertSelectionHelper(false);
        }

        [MenuItem("Tools/Hierarchy Explorer/Select the children %&c")]
        public static void HierarchyExplorer_SelectChildren()
        {
            SelectChildrenHelper(false);
        }

        private void OnGUI()
        {
            AddModePopups();
            AddInputField();
            AddToggles();
            AddButtons();
            AddInfoBox();
        }

        private static void OnHierarchyChanged()
        {
            hierarchyChanged = true;
        }

        private static void SetMessage(string msg, MessageType type = MessageType.Info)
        {
            message = msg;
            messageType = type;
        }

        #region GUI

        private static void AddModePopups()
        {
            GUILayout.Space(5f);
            GUIContent searchModeContent = new GUIContent("Search mode", "Set search mode");
            searchMode = (SearchMode)EditorGUILayout.EnumPopup(searchModeContent, searchMode);
            GUILayout.Space(5f);

            if (searchMode == SearchMode.ByName)
            {
                GUIContent exactMatchContent = new GUIContent("Name search mode",
                    "Exact - don't use any regexes.\nSimple regex - use '?' as a one-character wildcard.\nRegex - use C# Regex class.");
                nameSearchMode = (NameSearchMode)EditorGUILayout.EnumPopup(exactMatchContent, nameSearchMode);
                GUILayout.Space(5f);
            }
        }

        private static void AddInputField()
        {
            //dirty - extract lowercase mode from enum value
            string mode = searchMode.ToString().Substring(2).ToLower();
            string caption = "Type object " + mode + ":";
            string description = "Enter the " + mode + "of object to be found";

            GUIContent content = new GUIContent(caption, description);
            typedName = EditorGUILayout.TextField(content, typedName);
            GUILayout.Space(5f);
        }

        private static void AddToggles()
        {
            GUIContent includeDisabledContent = new GUIContent("Include disabled", "Include inactive objects");
            includeDisabled = EditorGUILayout.Toggle(includeDisabledContent, includeDisabled);
            GUILayout.Space(5f);

            GUIContent includeSelfActiveContent = new GUIContent("Include self active", "Search, even if parent is inactive");
            includeSelfActive = EditorGUILayout.Toggle(includeSelfActiveContent, includeSelfActive);
            GUILayout.Space(5f);
        }

        private static void AddButtons()
        {
            switch (searchMode)
            {
                case SearchMode.ByName:
                    if (GUILayout.Button(new GUIContent("Find by name", "Replaces current selection")))
                    {
                        FindObjects();
                    }
                    if (GUILayout.Button(new GUIContent("Append to selection", "Adds found items current selection")))
                    {
                        AppendObjects();
                    }
                    break;
                case SearchMode.ByTag:
                    if (GUILayout.Button(new GUIContent("Find by tag", "Finds by tag. Case sensitive.")))
                    {
                        FindByTags();
                    }
                    break;
                case SearchMode.ByType:
                    if (GUILayout.Button(new GUIContent("Find by type", "Finds by type, provided the type derives from Component. Case insensitive.")))
                    {
                        FindByType();
                    }
                    break;
                case SearchMode.ByLayer:
                    if (GUILayout.Button(new GUIContent("Find by layer", "Finds by layer. Case sensitive.")))
                    {
                        FindByLayer();
                    }
                    break;
                case SearchMode.ByInstanceID:
                    if (GUILayout.Button(new GUIContent("Find by instance ID", "Finds by object's instance ID.")))
                    {
                        FindByInstanceID();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (GUILayout.Button(new GUIContent("Invert selection", "Inverts selection")))
            {
                InvertSelection();
            }
            if (GUILayout.Button(new GUIContent("Invert selection within children", "Inverts selection, but only within selected objects' parents.")))
            {
                InvertSelectionWithinChildren();
            }
            if (GUILayout.Button(new GUIContent("Select children of selected", "Select all children of currently selected objects")))
            {
                SelectChildren();
            }
        }

        private static void AddInfoBox()
        {
            EditorGUILayout.HelpBox(message, messageType);
        }

        #endregion

        #region Helper functions

        private static void InvertSelectionHelper(bool checkEnability)
        {
            List<Transform> allObjects = FindAllTransforms();
            List<GameObject> currentlySelectedObjects = new List<GameObject>(Selection.gameObjects);
            //remove objects that are already selected
            for (int i = 0; i < currentlySelectedObjects.Count; i++)
            {
                allObjects.Remove(currentlySelectedObjects[i].transform);
            }
            if (checkEnability)
            {
                //perform enability check
                allObjects.RemoveAll(transform => !EnablityCheck(transform));
            }
            else
            {
                //just remove objects inactive in hierarchy
                allObjects.RemoveAll(transform => !transform.gameObject.activeInHierarchy);
            }
            //set selection to objects as GameObject array
            Selection.objects = allObjects.ConvertAll(x => x.gameObject).ToArray();
        }

        private static void InvertSelectionWithinChildrenHelper()
        {
            List<GameObject> currentlySelectedObjects = new List<GameObject>(Selection.gameObjects);

            //get selected objects' parents
            HashSet<Transform> parents = new HashSet<Transform>(currentlySelectedObjects.ConvertAll(input => input.transform.parent));

            HashSet<Transform> newSelection = new HashSet<Transform>();
            foreach (Transform parent in parents)
            {
                if (parent == null) continue;

                foreach (Transform child in parent)
                {
                    //if transform is not selected, pick it
                    if (currentlySelectedObjects.Contains(child.gameObject) == false)
                    {
                        newSelection.Add(child);
                    }
                }
            }

            Selection.objects = newSelection.ToList().ConvertAll(x => x.gameObject).ToArray();
        }

        private static void SelectChildrenHelper(bool checkEnability)
        {
            Object[] currentSelection = Selection.objects;
            HashSet<Object> newSelection = new HashSet<Object>(); //using HashSet for faster adding

            foreach (Object selected in currentSelection)
            {
                GameObject transform = selected as GameObject;
                if (transform == null) continue;

                //GetComponentsInChildren includes the parent, too
                Transform[] newChildren = transform.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in newChildren)
                {
                    //depending on checkEnability, check if object should be added
                    if (checkEnability && !EnablityCheck(child) ||
                        !checkEnability && !child.gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    if (!newSelection.Contains(child))
                    {
                        newSelection.Add(child.gameObject);
                    }
                }
            }
            Selection.objects = newSelection.ToArray();
        }


        #endregion

        #region Button functions

        private static void FindObjects()
        {
            List<Object> objects = FindObjectsByName();
            if (objects == null || objects.Count == 0)
            {
                SetMessage("No matches");
                return;
            }
            Selection.objects = objects.ToArray();
            SetMessage("Matched "+ objects.Count + (objects.Count == 1 ? " object." : " objects."));
        }

        private static void AppendObjects()
        {
            List<Object> selectedObjects = new List<Object>(Selection.objects);
            selectedObjects.AddRange(FindObjectsByName());

            int addedObjectsCount = selectedObjects.Count - Selection.objects.Length;
            if (addedObjectsCount == 0)
            {
                SetMessage("Added " + addedObjectsCount + (addedObjectsCount == 1 ? " object." : " objects."));
            }
            else
            {
                SetMessage("Nothing added!");
            }

            Selection.objects = selectedObjects.ToArray();
        }

        private static void InvertSelection()
        {
            InvertSelectionHelper(true);
            SetMessage("Selection inverted.");
        }

        private static void InvertSelectionWithinChildren()
        {
            InvertSelectionWithinChildrenHelper();
            SetMessage("Selection inverted.");
        }

        private static void SelectChildren()
        {
            SelectChildrenHelper(true);
            SetMessage("Selected the children.");
        }

        private static void FindByType()
        {
            typedName = typedName.RemoveSpaces();
            string[] typedTypes = typedName.Split(',');

            List<Transform> allFoundObjects = new List<Transform>();

            foreach (string typedType in typedTypes)
            {
                List<Transform> allObjectsOnScene = FindAllTransforms();
                Type elementType = GetElementType(typedType);
                if (elementType != null && elementType.IsSubclassOf(typeof(Component)))
                {
                    allObjectsOnScene.RemoveAll(transform => !EnablityCheck(transform) ||
                                                            transform.GetComponent(elementType) == null);
                    allFoundObjects.AddRange(allObjectsOnScene);
                }
            }
            Object[] convertedObjects = allFoundObjects.ToList().ConvertAll(x => x.gameObject).Distinct().ToArray();
            if (convertedObjects.Length == 0)
            {
                SetMessage("No matches.");
                return;
            }
            Selection.objects = convertedObjects;
            SetMessage("Found " + convertedObjects.Length + (convertedObjects.Length == 1 ? " object." : " objects."));
        }

        private static void FindByTags()
        {
            List<string> tags = GetTagList();
            List<Transform> foundObjects = new List<Transform>();
            foreach (string tag in tags)
            {
                List<Transform> allObjectsOnScene = FindAllTransforms();
                allObjectsOnScene.RemoveAll(transform => !EnablityCheck(transform) ||
                                                         transform.tag != tag
                );
                foundObjects.AddRange(allObjectsOnScene);
            }
            Object[] convertedObjects = foundObjects.ToList().ConvertAll(x => x.gameObject).ToArray();
            if (convertedObjects.Length == 0)
            {
                SetMessage("No matches.");
                return;
            }
            Selection.objects = convertedObjects;
            SetMessage("Found " + convertedObjects.Length + (convertedObjects.Length == 1 ? " object." : " objects."));
        }

        private static void FindByLayer()
        {
            List<string> layers = GetLayersList();
            List<Transform> foundObjects = new List<Transform>();
            foreach (string layer in layers)
            {
                int layerNumber = LayerMask.NameToLayer(layer);
                List<Transform> allObjectsOnScene = FindAllTransforms();
                allObjectsOnScene.RemoveAll(transform => !EnablityCheck(transform) ||
                                                         transform.gameObject.layer != layerNumber
                );
                foundObjects.AddRange(allObjectsOnScene);
            }
            Object[] convertedObjects = foundObjects.ToList().ConvertAll(x => x.gameObject).ToArray();
            if (convertedObjects.Length == 0)
            {
                SetMessage("No matches.");
                return;
            }
            Selection.objects = convertedObjects;
            SetMessage("Found " + convertedObjects.Length + (convertedObjects.Length == 1 ? " object." : " objects."));
        }

        private static void FindByInstanceID()
        {
            int id;
            //id must be int
            if (int.TryParse(typedName, out id))
            {
                Transform foundObject = (Transform)EditorUtility.InstanceIDToObject(id);
                if (foundObject != null)
                {
                    SetMessage("Found object of name "+foundObject.name);
                    Selection.activeGameObject = foundObject.gameObject;
                }
                else
                {
                    SetMessage("Object of id "+id+" not found!", MessageType.Error);
                }
            }
            else
            {
                SetMessage("Not valid instance ID!", MessageType.Error);
            }
        }

        #endregion

        #region Utility Methods

        private static List<Object> FindObjectsByName()
        {
            List<Transform> allObjects = FindAllTransforms();

            //find all objects with specified name
            List<Transform> collection = allObjects.FindAll(CheckComponentName);

            //return them as Object collection
            return collection.ConvertAll(input => (Object)input.gameObject);
        }

        private static bool CheckComponentName(Component component)
        {
            //check for enabled, activeSelf etc.
            if (!EnablityCheck(component))
            {
                return false;
            }

            switch (nameSearchMode)
            {
                case NameSearchMode.Exact:
                    return component.name.Equals(typedName);
                case NameSearchMode.CaseInsensitive:
                    return component.name.Equals(typedName, StringComparison.OrdinalIgnoreCase);
                case NameSearchMode.SimpleRegex:
                    return SimpleRegex.IsMatch(component.name, typedName);
                case NameSearchMode.Regex:
                    try
                    {
                        Regex regex = new Regex(typedName);
                        return regex.IsMatch(component.name);
                    }
                    catch (ArgumentException ex)
                    {
                        Debug.LogError("HierarchyExplorer::CheckComponentName: Regex error: " + ex.Message);
                        return false;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Type GetElementType(string name)
        {
            //feel free to add other namespaces
            List<string> namespaces = new List<string> { "UnityEngine", "UnityEngine.UI" };
            foreach (string @namespace in namespaces)
            {
                string l = @namespace + "." + name + ", " + @namespace;
                var elementType = Type.GetType(l, false, true);
                if (elementType != null)
                {
                    return elementType;
                }
            }
            //last shot - try to find name without specifying namespace
            //useful for non-built-in scripts
            return Type.GetType(name, false, true);
        }

        private static List<string> GetTagList()
        {
            //extract list of tags from input
            typedName = typedName.RemoveSpaces();
            List<string> tags = new List<string>(typedName.Split(','));

            //remove undefined tags
            string[] tagList = UnityEditorInternal.InternalEditorUtility.tags;
            tags.RemoveAll(s => !tagList.Contains(s));

            return tags;
        }

        private static List<string> GetLayersList()
        {
            //extract list of tags from input
            typedName = typedName.RemoveSpaces();
            List<string> layers = new List<string>(typedName.Split(','));

            //remove undefined layers
            string[] layersList = UnityEditorInternal.InternalEditorUtility.layers;
            layers.RemoveAll(s => !layersList.Contains(s));

            return layers;
        }

        private static bool EnablityCheck(Component component)
        {
            return includeDisabled ||                                   //if true, include all
                includeSelfActive && component.gameObject.activeSelf    //if true, don't include objects with "Active" unchecked
                || component.gameObject.activeInHierarchy;              //just exclude objects marked gray in Hierarchy
        }

        private static List<Transform> FindAllTransforms()
        {
            if (cachedObjectsList != null && hierarchyChanged == false)
            {
                return cachedObjectsList.ToList();
            }

            cachedObjectsList = new HashSet<Transform>();
            GameObject[] objectsOnScene = GetObjectsOnCurrentScene();

            //add all parent and all it's
            foreach (GameObject @object in objectsOnScene)
            {
                //GetComponentsInChildren adds parent, too
                cachedObjectsList.UnionWith(@object.GetComponentsInChildren<Transform>(true));
            }

            hierarchyChanged = false;

            return cachedObjectsList.ToList();
        }

        private static GameObject[] GetObjectsOnCurrentScene()
        {
            return FindObjectsOfType<GameObject>();
        }

        #endregion

    }
}