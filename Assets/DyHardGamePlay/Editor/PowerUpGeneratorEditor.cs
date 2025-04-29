using UnityEditor;

namespace RedGaint
{
    [CustomEditor(typeof(PowerUpGenerator))]
    public class PowerUpGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PowerUpGenerator generator = (PowerUpGenerator)target;

            if (generator.spawnMode == GlobalEnums.Mode.SingleShot)
            {
                generator.selectedPowerUpType = (GlobalEnums.PowerUpType)EditorGUILayout.EnumPopup("Selected Power-Up Type",
                        generator.selectedPowerUpType);
            }
        }
    }
}