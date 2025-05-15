using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class LightSettingsTool : EditorWindow
{
    // Filter options for the type of light
    private LightType lightTypeFilter = LightType.Point;

    // Settings to adjust
    private bool enableShadows = true;
    private LightShadows shadowType = LightShadows.Soft;
    private LightShadowResolution shadowResolution = LightShadowResolution.Medium;
    private LightmapBakeType lightMode = LightmapBakeType.Mixed;

    [MenuItem("Tools/Daelonik Artworks/Light Settings Tool")]
    public static void ShowWindow()
    {
        GetWindow<LightSettingsTool>("Light Settings Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Light Settings Tool", EditorStyles.boldLabel);

        // Light type filter selection
        lightTypeFilter = (LightType)EditorGUILayout.EnumPopup("Light Type Filter", lightTypeFilter);

        // Shadows toggle and type
        enableShadows = EditorGUILayout.Toggle("Enable Shadows", enableShadows);
        shadowType = (LightShadows)EditorGUILayout.EnumPopup("Shadow Type", shadowType);

        // Shadow resolution
        shadowResolution = (LightShadowResolution)EditorGUILayout.EnumPopup("Shadow Resolution", shadowResolution);

        // Light mode
        lightMode = (LightmapBakeType)EditorGUILayout.EnumPopup("Light Mode", lightMode);

        // Apply settings buttons
        if (GUILayout.Button("Apply Settings to All"))
        {
            ApplyLightSettingsToAll();
        }

        if (GUILayout.Button("Apply Settings on Selected"))
        {
            ApplyLightSettingsToSelected();
        }
    }

    private void ApplyLightSettingsToAll()
    {
        // Find all lights in the scene
        Light[] lights = FindObjectsOfType<Light>();

        List<Light> filteredLights = new List<Light>();
        foreach (Light light in lights)
        {
            // Filter lights by the selected type
            if (light.type == lightTypeFilter)
            {
                filteredLights.Add(light);
            }
        }

        // Apply settings to each filtered light
        ApplySettingsToLights(filteredLights);

        Debug.Log($"{filteredLights.Count} {lightTypeFilter} lights updated in the scene.");
    }

    private void ApplyLightSettingsToSelected()
    {
        // Get selected GameObjects in the hierarchy
        GameObject[] selectedObjects = Selection.gameObjects;

        List<Light> filteredLights = new List<Light>();
        foreach (GameObject obj in selectedObjects)
        {
            // Get lights in the selected GameObject and its children
            Light[] lights = obj.GetComponentsInChildren<Light>();

            foreach (Light light in lights)
            {
                // Filter lights by the selected type
                if (light.type == lightTypeFilter)
                {
                    filteredLights.Add(light);
                }
            }
        }

        // Apply settings to each filtered light
        ApplySettingsToLights(filteredLights);

        Debug.Log($"{filteredLights.Count} {lightTypeFilter} lights updated in selected objects.");
    }

    private void ApplySettingsToLights(List<Light> lights)
    {
        foreach (Light light in lights)
        {
            Undo.RecordObject(light, "Light Settings Change");

            light.shadows = enableShadows ? shadowType : LightShadows.None;
            light.shadowResolution = shadowResolution;
            light.lightmapBakeType = lightMode;

            EditorUtility.SetDirty(light);
        }
    }
}
