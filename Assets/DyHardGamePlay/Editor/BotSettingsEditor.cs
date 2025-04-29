using RedGaint;
using UnityEditor;
using UnityEngine;

public class BotSettingsEditor : MonoBehaviour
{
    private const string botSettingsPath = "Assets/BotSettings.asset";

    [MenuItem("Tools/Bot Settings")]
    public static void OpenBotSettings()
    {
        // Try to load the BotSettings asset
        BotSettings settings = AssetDatabase.LoadAssetAtPath<BotSettings>(botSettingsPath);

        // If the asset doesn't exist, create it
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<BotSettings>();
            AssetDatabase.CreateAsset(settings, botSettingsPath);
            AssetDatabase.SaveAssets();

            Debug.Log("BotSettings asset created at: " + botSettingsPath);
        }

        // Focus the asset in the Project window
        Selection.activeObject = settings;
        EditorGUIUtility.PingObject(settings);
    }
}
