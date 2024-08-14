using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

[InitializeOnLoad]
public static class StaticPrefabAutoRegister {
    static StaticPrefabAutoRegister() {
        RegisterJoinTypePrefabs();
        RegisterSpawnTypePrefabs();
    }

    private static void RegisterJoinTypePrefabs() {
        var joinTypeClasses = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(JoinTypePrefabAttribute), false).Length > 0);

        foreach (var type in joinTypeClasses) {
            var attribute = (JoinTypePrefabAttribute)type.GetCustomAttributes(typeof(JoinTypePrefabAttribute), false).FirstOrDefault();
            if (attribute != null) {
                // Find a prefab with the same name as the class
                var prefab = FindPrefabWithClassName(type.Name);
                if (prefab != null) {
                    // Assign the prefab to your static dictionary or a ScriptableObject
                    // This example assumes a static dictionary in a Manager class
                    PrefabManager.RegisterJoinTypePrefab(attribute.TypeName, prefab);
                } else {
                    Debug.LogWarning($"No prefab found matching class name: {type.Name}");
                }
            }
        }
    }

    private static void RegisterSpawnTypePrefabs() {
        var spawnTypeClasses = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(SpawnTypePrefabAttribute), false).Length > 0);

        foreach (var type in spawnTypeClasses) {
            var attribute = (SpawnTypePrefabAttribute)type.GetCustomAttributes(typeof(SpawnTypePrefabAttribute), false).FirstOrDefault();
            if (attribute != null) {
                // Find a prefab with the same name as the class
                var prefab = FindPrefabWithClassName(type.Name);
                if (prefab != null) {
                    // Assign the prefab to your static dictionary or a ScriptableObject
                    PrefabManager.RegisterSpawnTypePrefab(attribute.TypeName, prefab);
                } else {
                    Debug.LogWarning($"No prefab found matching class name: {type.Name}");
                }
            }
        }
    }

    private static GameObject FindPrefabWithClassName(string className) {
        string[] guids = AssetDatabase.FindAssets(className + " t:Prefab");
        foreach (var guid in guids) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab != null) {
                // You might want to add additional checks here to ensure this prefab is the correct one
                return prefab;
            }
        }
        return null;
    }
}

public static class PrefabManager {
    private static Dictionary<string, GameObject> joinTypePrefabs = new Dictionary<string, GameObject>();
    private static Dictionary<string, GameObject> spawnTypePrefabs = new Dictionary<string, GameObject>();

    public static void RegisterJoinTypePrefab(string typeName, GameObject prefab) {
        if (!joinTypePrefabs.ContainsKey(typeName)) {
            joinTypePrefabs[typeName] = prefab;
        }
    }

    public static void RegisterSpawnTypePrefab(string typeName, GameObject prefab) {
        if (!spawnTypePrefabs.ContainsKey(typeName)) {
            spawnTypePrefabs[typeName] = prefab;
        }
    }

    public static GameObject GetJoinTypePrefab(string typeName) {
        return joinTypePrefabs.TryGetValue(typeName, out var prefab) ? prefab : null;
    }

    public static GameObject GetSpawnTypePrefab(string typeName) {
        return spawnTypePrefabs.TryGetValue(typeName, out var prefab) ? prefab : null;
    }
}
