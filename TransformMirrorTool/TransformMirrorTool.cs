using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class TransformPair
{
    public GameObject baseObject;
    public GameObject targetObject;

    // Initial transforms in world space
    [HideInInspector]
    public Vector3 initialBasePosition;
    [HideInInspector]
    public Quaternion initialBaseRotation;
    [HideInInspector]
    public Vector3 initialBaseScale;
    
    [HideInInspector]
    public Vector3 initialTargetPosition;
    [HideInInspector]
    public Quaternion initialTargetRotation;
    [HideInInspector]
    public Vector3 initialTargetScale;

    // Last known base transform in world space
    [HideInInspector]
    public Vector3 lastBasePosition;
    [HideInInspector]
    public Quaternion lastBaseRotation;
    [HideInInspector]
    public Vector3 lastBaseScale;

    // Axis control flags
    public bool mirrorX = true;
    public bool mirrorY = true;
    public bool mirrorZ = true;

    // Bypass flags
    public bool bypassXPosition = false;
    public bool bypassYPosition = false;
    public bool bypassZPosition = false;
    public bool bypassXRotation = false;
    public bool bypassYRotation = false;
    public bool bypassZRotation = false;

    public void CaptureInitialTransforms()
    {
        if (baseObject != null)
        {
            // Store world space transforms
            initialBasePosition = baseObject.transform.position;
            initialBaseRotation = baseObject.transform.rotation;
            initialBaseScale = baseObject.transform.localScale;
            
            lastBasePosition = initialBasePosition;
            lastBaseRotation = initialBaseRotation;
            lastBaseScale = initialBaseScale;
        }
        
        if (targetObject != null)
        {
            // Store world space transforms
            initialTargetPosition = targetObject.transform.position;
            initialTargetRotation = targetObject.transform.rotation;
            initialTargetScale = targetObject.transform.localScale;
        }
    }

    public void RestoreInitialTransforms()
    {
        if (baseObject != null)
        {
            Undo.RecordObject(baseObject.transform, "Restore Base Transform");
            baseObject.transform.position = initialBasePosition;
            baseObject.transform.rotation = initialBaseRotation;
            baseObject.transform.localScale = initialBaseScale;
            EditorUtility.SetDirty(baseObject.transform);
        }

        if (targetObject != null)
        {
            Undo.RecordObject(targetObject.transform, "Restore Target Transform");
            targetObject.transform.position = initialTargetPosition;
            targetObject.transform.rotation = initialTargetRotation;
            targetObject.transform.localScale = initialTargetScale;
            EditorUtility.SetDirty(targetObject.transform);
        }
    }
}

public class TransformMirrorTool : EditorWindow
{
    private List<TransformPair> transformPairs = new List<TransformPair>();
    private Vector2 scrollPos;
    private bool isMirroring = false;

    // Preset System Variables
    private string presetName = "New_Preset";
    private List<string> availablePresets = new List<string>();
    private int selectedPresetIndex = -1;
    private string presetsFolder = "Assets/TransformMirrorPresets";

    [MenuItem("Tools/BigSoulja/Transform Mirror Tool")]
    public static void ShowWindow()
    {
        GetWindow<TransformMirrorTool>("Transform Mirror Tool");
    }

    private void OnEnable()
    {
        if (!AssetDatabase.IsValidFolder(presetsFolder))
        {
            AssetDatabase.CreateFolder("Assets", "TransformMirrorPresets");
            AssetDatabase.Refresh();
        }

        LoadAvailablePresets();
    }

    private void OnGUI()
    {
        GUILayout.Label("Transform Mirror Tool by BigSoulja", EditorStyles.boldLabel);
        GUILayout.Space(10);

        #region Preset Management
        GUILayout.Label("Preset Management", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        presetName = EditorGUILayout.TextField("Preset Name", presetName);
        if (GUILayout.Button("Save Preset", GUILayout.Width(100)))
        {
            SavePreset();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Available Presets:", GUILayout.Width(100));
        if (availablePresets.Count > 0)
        {
            selectedPresetIndex = Mathf.Clamp(EditorGUILayout.Popup(selectedPresetIndex, availablePresets.ToArray(), GUILayout.Width(200)), 0, availablePresets.Count - 1);
            if (GUILayout.Button("Load Preset", GUILayout.Width(100)))
            {
                if (selectedPresetIndex >= 0 && selectedPresetIndex < availablePresets.Count)
                {
                    LoadPreset(availablePresets[selectedPresetIndex]);
                }
            }
        }
        else
        {
            GUILayout.Label("No Presets Found");
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reload Presets", GUILayout.Width(150)))
        {
            LoadAvailablePresets();
        }

        GUILayout.Space(20);
        #endregion

        #region Transform Pairs Management
        GUILayout.Label("Transform Pairs", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(600));
        for (int i = 0; i < transformPairs.Count; i++)
        {
            DrawPair(transformPairs[i], i);
            GUILayout.Space(5);
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("+ Add Pair"))
        {
            AddEmptyPair();
        }

        GUILayout.Space(10);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 12
        };

        GUI.backgroundColor = isMirroring ? Color.red : Color.green;
        if (GUILayout.Button(isMirroring ? "Stop Mirroring" : "Start Mirroring", buttonStyle, GUILayout.Height(40)))
        {
            if (isMirroring)
                StopMirroring();
            else
                StartMirroring();
        }
        GUI.backgroundColor = Color.white;
        #endregion
    }

    private void DrawPair(TransformPair pair, int index)
    {
        EditorGUILayout.BeginVertical("box");

        GUILayout.BeginHorizontal();
        GUILayout.Label($"Pair {index + 1}", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("-", GUILayout.Width(25)))
        {
            RemovePair(pair);
            return;
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Object Fields
        GUILayout.BeginHorizontal();
        GUILayout.Label("Source Object", GUILayout.Width(80));
        pair.baseObject = (GameObject)EditorGUILayout.ObjectField(pair.baseObject, typeof(GameObject), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Target Object", GUILayout.Width(80));
        pair.targetObject = (GameObject)EditorGUILayout.ObjectField(pair.targetObject, typeof(GameObject), true);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Restore and Swap Buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Restore", GUILayout.Width(80)))
        {
            pair.RestoreInitialTransforms();
        }
        if (GUILayout.Button("Swap Pair", GUILayout.Width(80)))
        {
            SwapPair(pair);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Mirror Settings Section
        GUILayout.BeginVertical("box");
        GUILayout.Label("Mirror Settings", EditorStyles.boldLabel);

        // Mirror Axis Toggles
        GUILayout.BeginHorizontal();
        GUILayout.Label("Mirror X", GUILayout.Width(70));
        pair.mirrorX = EditorGUILayout.Toggle(pair.mirrorX);
        GUILayout.Space(10);
        GUILayout.Label("Mirror Y", GUILayout.Width(70));
        pair.mirrorY = EditorGUILayout.Toggle(pair.mirrorY);
        GUILayout.Space(10);
        GUILayout.Label("Mirror Z", GUILayout.Width(70));
        pair.mirrorZ = EditorGUILayout.Toggle(pair.mirrorZ);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.Space(10);

        // Bypass Settings Section
        GUILayout.BeginVertical("box");
        GUILayout.Label("Bypass Settings", EditorStyles.boldLabel);

        // Position Bypass Toggles
        EditorGUILayout.LabelField("Position Bypass", EditorStyles.boldLabel);
        pair.bypassXPosition = EditorGUILayout.Toggle("Bypass X Position", pair.bypassXPosition);
        pair.bypassYPosition = EditorGUILayout.Toggle("Bypass Y Position", pair.bypassYPosition);
        pair.bypassZPosition = EditorGUILayout.Toggle("Bypass Z Position", pair.bypassZPosition);

        GUILayout.Space(5);

        // Rotation Bypass Toggles
        EditorGUILayout.LabelField("Rotation Bypass", EditorStyles.boldLabel);
        pair.bypassXRotation = EditorGUILayout.Toggle("Bypass X Rotation", pair.bypassXRotation);
        pair.bypassYRotation = EditorGUILayout.Toggle("Bypass Y Rotation", pair.bypassYRotation);
        pair.bypassZRotation = EditorGUILayout.Toggle("Bypass Z Rotation", pair.bypassZRotation);

        GUILayout.EndVertical();

        EditorGUILayout.EndVertical();
    }

    private void AddEmptyPair()
    {
        TransformPair newPair = new TransformPair();
        transformPairs.Add(newPair);
    }

    private void RemovePair(TransformPair pair)
    {
        if (isMirroring)
        {
            pair.RestoreInitialTransforms();
        }
        transformPairs.Remove(pair);
    }

    private void StartMirroring()
    {
        foreach (var pair in transformPairs)
        {
            if (pair.baseObject == null || pair.targetObject == null)
                continue;

            pair.CaptureInitialTransforms();
        }

        EditorApplication.update += OnEditorUpdate;
        isMirroring = true;

        Debug.Log("Mirroring started.");
    }

    private void StopMirroring()
    {
        EditorApplication.update -= OnEditorUpdate;
        isMirroring = false;

        foreach (var pair in transformPairs)
        {
            if (pair.baseObject != null && pair.targetObject != null)
            {
                pair.RestoreInitialTransforms();
            }
        }

        Debug.Log("Mirroring stopped and transforms restored to original.");
    }

    private void OnEditorUpdate()
    {
        if (!isMirroring) return;

        // Check if we're recording an animation
        bool isRecording = AnimationMode.InAnimationMode();

        foreach (var pair in transformPairs)
        {
            if (pair.baseObject == null || pair.targetObject == null)
                continue;

            Transform baseTransform = pair.baseObject.transform;
            Transform targetTransform = pair.targetObject.transform;

            // Check if base transform has changed
            if (baseTransform.position != pair.lastBasePosition ||
                baseTransform.rotation != pair.lastBaseRotation ||
                baseTransform.localScale != pair.lastBaseScale)
            {
                // Handle Position Mirroring in World Space
                Vector3 currentWorldPos = baseTransform.position;
                Vector3 worldPosDelta = currentWorldPos - pair.initialBasePosition;
                Vector3 newTargetPosition = pair.initialTargetPosition;

                // Apply position changes based on mirror settings
                if (!pair.bypassXPosition)
                    newTargetPosition.x += pair.mirrorX ? -worldPosDelta.x : worldPosDelta.x;
                if (!pair.bypassYPosition)
                    newTargetPosition.y += pair.mirrorY ? -worldPosDelta.y : worldPosDelta.y;
                if (!pair.bypassZPosition)
                    newTargetPosition.z += pair.mirrorZ ? -worldPosDelta.z : worldPosDelta.z;

                // Handle Rotation Mirroring in World Space
                Quaternion currentWorldRot = baseTransform.rotation;
                Quaternion worldRotDelta = currentWorldRot * Quaternion.Inverse(pair.initialBaseRotation);
                Vector3 worldRotDeltaEuler = worldRotDelta.eulerAngles;
                Vector3 targetRotationEuler = pair.initialTargetRotation.eulerAngles;

                // Apply rotation changes based on mirror settings
                if (!pair.bypassXRotation)
                    targetRotationEuler.x += pair.mirrorX ? -worldRotDeltaEuler.x : worldRotDeltaEuler.x;
                if (!pair.bypassYRotation)
                    targetRotationEuler.y += pair.mirrorY ? -worldRotDeltaEuler.y : worldRotDeltaEuler.y;
                if (!pair.bypassZRotation)
                    targetRotationEuler.z += pair.mirrorZ ? -worldRotDeltaEuler.z : worldRotDeltaEuler.z;

                // Handle Scale
                Vector3 currentScale = baseTransform.localScale;
                Vector3 scaleDelta = new Vector3(
                    currentScale.x / pair.initialBaseScale.x,
                    currentScale.y / pair.initialBaseScale.y,
                    currentScale.z / pair.initialBaseScale.z
                );

                Vector3 newScale = new Vector3(
                    pair.initialTargetScale.x * (pair.mirrorX ? scaleDelta.x : scaleDelta.x),
                    pair.initialTargetScale.y * (pair.mirrorY ? scaleDelta.y : scaleDelta.y),
                    pair.initialTargetScale.z * (pair.mirrorZ ? scaleDelta.z : scaleDelta.z)
                );

				// Record changes for animation if we're recording
				if (isRecording)
				{
					Undo.RecordObject(targetTransform, "Mirror Transform");
					EditorUtility.SetDirty(targetTransform);
					UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
					SceneView.RepaintAll();
				}

                // Apply transforms
                Undo.RecordObject(targetTransform, "Mirror Transform");
                
                targetTransform.position = newTargetPosition;
                targetTransform.rotation = Quaternion.Euler(targetRotationEuler);
                targetTransform.localScale = newScale;
                
                EditorUtility.SetDirty(targetTransform);

                // Update last known transforms
                pair.lastBasePosition = currentWorldPos;
                pair.lastBaseRotation = currentWorldRot;
                pair.lastBaseScale = currentScale;
            }
        }
    }

    #region Preset System

    private void LoadAvailablePresets()
    {
        availablePresets.Clear();
        string[] guids = AssetDatabase.FindAssets("t:TransformMirrorPresetAsset", new[] { presetsFolder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TransformMirrorPresetAsset presetAsset = AssetDatabase.LoadAssetAtPath<TransformMirrorPresetAsset>(path);
            if (presetAsset != null && !string.IsNullOrEmpty(presetAsset.preset.presetName))
            {
                availablePresets.Add(presetAsset.preset.presetName);
            }
        }

        if (availablePresets.Count == 0)
        {
            selectedPresetIndex = -1;
        }
        else if (selectedPresetIndex >= availablePresets.Count)
        {
            selectedPresetIndex = availablePresets.Count - 1;
        }

        Repaint();
    }

    private void SavePreset()
    {
        if (string.IsNullOrEmpty(presetName))
        {
            EditorUtility.DisplayDialog("Invalid Preset Name", "Please enter a valid preset name.", "OK");
            return;
        }

        string presetPath = Path.Combine(presetsFolder, $"{presetName}.asset");

        // Check for existing preset
        bool shouldOverwrite = true;
        if (File.Exists(presetPath))
        {
            shouldOverwrite = EditorUtility.DisplayDialog("Preset Exists",
                $"A preset named '{presetName}' already exists. Do you want to overwrite it?",
                "Yes", "No");
            if (!shouldOverwrite) return;
        }

        // Create or get existing preset asset
        TransformMirrorPresetAsset presetAsset = AssetDatabase.LoadAssetAtPath<TransformMirrorPresetAsset>(presetPath);
        if (presetAsset == null)
        {
            presetAsset = ScriptableObject.CreateInstance<TransformMirrorPresetAsset>();
            AssetDatabase.CreateAsset(presetAsset, presetPath);
        }

        // Update preset data
        presetAsset.preset.presetName = presetName;
        presetAsset.preset.pairs.Clear();

        foreach (var pair in transformPairs)
        {
            TransformPairData pairData = new TransformPairData
            {
                baseObjectName = pair.baseObject != null ? pair.baseObject.name : "",
                targetObjectName = pair.targetObject != null ? pair.targetObject.name : "",
                mirrorX = pair.mirrorX,
                mirrorY = pair.mirrorY,
                mirrorZ = pair.mirrorZ,
                bypassXPosition = pair.bypassXPosition,
                bypassYPosition = pair.bypassYPosition,
                bypassZPosition = pair.bypassZPosition,
                bypassXRotation = pair.bypassXRotation,
                bypassYRotation = pair.bypassYRotation,
                bypassZRotation = pair.bypassZRotation,
                initialBasePosition = pair.initialBasePosition,
                initialBaseRotation = pair.initialBaseRotation,
                initialBaseScale = pair.initialBaseScale,
                initialTargetPosition = pair.initialTargetPosition,
                initialTargetRotation = pair.initialTargetRotation,
                initialTargetScale = pair.initialTargetScale
            };

            presetAsset.preset.pairs.Add(pairData);
        }

        EditorUtility.SetDirty(presetAsset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        LoadAvailablePresets();
    }

    private void LoadPreset(string presetNameToLoad)
    {
        if (string.IsNullOrEmpty(presetNameToLoad)) return;

        string presetPath = Path.Combine(presetsFolder, $"{presetNameToLoad}.asset");
        if (!File.Exists(presetPath))
        {
            EditorUtility.DisplayDialog("Preset Not Found", $"Preset '{presetNameToLoad}' does not exist.", "OK");
            return;
        }

        TransformMirrorPresetAsset presetAsset = AssetDatabase.LoadAssetAtPath<TransformMirrorPresetAsset>(presetPath);
        if (presetAsset == null) return;

        // Clear current pairs
        transformPairs.Clear();

        // Load pairs from preset
        foreach (var pairData in presetAsset.preset.pairs)
        {
            GameObject baseObj = GameObject.Find(pairData.baseObjectName);
            GameObject targetObj = GameObject.Find(pairData.targetObjectName);

            if (baseObj == null || targetObj == null)
            {
                Debug.LogWarning($"Could not find objects '{pairData.baseObjectName}' or '{pairData.targetObjectName}' in the scene.");
                continue;
            }

            TransformPair newPair = new TransformPair
            {
                baseObject = baseObj,
                targetObject = targetObj,
                mirrorX = pairData.mirrorX,
                mirrorY = pairData.mirrorY,
                mirrorZ = pairData.mirrorZ,
                bypassXPosition = pairData.bypassXPosition,
                bypassYPosition = pairData.bypassYPosition,
                bypassZPosition = pairData.bypassZPosition,
                bypassXRotation = pairData.bypassXRotation,
                bypassYRotation = pairData.bypassYRotation,
                bypassZRotation = pairData.bypassZRotation,
                initialBasePosition = pairData.initialBasePosition,
                initialBaseRotation = pairData.initialBaseRotation,
                initialBaseScale = pairData.initialBaseScale,
                initialTargetPosition = pairData.initialTargetPosition,
                initialTargetRotation = pairData.initialTargetRotation,
                initialTargetScale = pairData.initialTargetScale
            };

            transformPairs.Add(newPair);
        }
    }

    #endregion

    #region Contextual Menu Integration

    [MenuItem("GameObject/Add Pair to Transform Mirror Tool", false, 1000)]
    private static void AddPairContextMenu()
    {
        TransformMirrorTool window = GetWindow<TransformMirrorTool>("Transform Mirror Tool");
        window.AddSelectedPair();
    }

    [MenuItem("GameObject/Add Pair to Transform Mirror Tool", true)]
    private static bool AddPairContextMenu_Validate()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length != 2) return false;

        TransformMirrorTool window = GetWindow<TransformMirrorTool>("Transform Mirror Tool");
        foreach (var pair in window.transformPairs)
        {
            if (pair.baseObject == selectedObjects[0] || pair.targetObject == selectedObjects[0] ||
                pair.baseObject == selectedObjects[1] || pair.targetObject == selectedObjects[1])
            {
                return false;
            }
        }

        return true;
    }

    private void AddSelectedPair()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length != 2)
        {
            Debug.LogWarning("Please select exactly two GameObjects to add as a pair.");
            return;
        }

        // Check if objects are already in a pair
        foreach (var pair in transformPairs)
        {
            if (pair.baseObject == selectedObjects[0] || pair.targetObject == selectedObjects[0] ||
                pair.baseObject == selectedObjects[1] || pair.targetObject == selectedObjects[1])
            {
                Debug.LogWarning("One or both of the selected GameObjects are already in a pair.");
                return;
            }
        }

        TransformPair newPair = new TransformPair
        {
            baseObject = selectedObjects[0],
            targetObject = selectedObjects[1]
        };
        newPair.CaptureInitialTransforms();
        transformPairs.Add(newPair);

        Debug.Log($"Added pair: {selectedObjects[0].name} ↔ {selectedObjects[1].name}");
    }

    #endregion

    #region Swap Functionality

    private void SwapPair(TransformPair pair)
    {
        if (pair.baseObject == null || pair.targetObject == null)
        {
            Debug.LogWarning("Cannot swap pair because one or both GameObjects are null.");
            return;
        }

        // Swap GameObjects
        GameObject tempObj = pair.baseObject;
        pair.baseObject = pair.targetObject;
        pair.targetObject = tempObj;

        // Swap initial transforms
        Vector3 tempPos = pair.initialBasePosition;
        Quaternion tempRot = pair.initialBaseRotation;
        Vector3 tempScale = pair.initialBaseScale;

        pair.initialBasePosition = pair.initialTargetPosition;
        pair.initialBaseRotation = pair.initialTargetRotation;
        pair.initialBaseScale = pair.initialTargetScale;

        pair.initialTargetPosition = tempPos;
        pair.initialTargetRotation = tempRot;
        pair.initialTargetScale = tempScale;

        // Update last known transforms
        if (pair.baseObject != null)
        {
            pair.lastBasePosition = pair.baseObject.transform.position;
            pair.lastBaseRotation = pair.baseObject.transform.rotation;
            pair.lastBaseScale = pair.baseObject.transform.localScale;
        }

        Debug.Log($"Swapped pair: {pair.baseObject.name} ↔ {pair.targetObject.name}");
    }

    #endregion
}
