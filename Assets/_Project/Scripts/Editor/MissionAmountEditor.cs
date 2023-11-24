using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class MissionAmountEditor : EditorWindow
    {
        [SerializeField] private PlayerData playerData;
        private Vector2 _scrollPosition;

        [MenuItem("Project Tools/Mission Amount Editor")]
        public static void Open()
        {
            MissionAmountEditor window = GetWindow<MissionAmountEditor>();
            window.titleContent = new GUIContent("Mission Amount Editor For Each Day");
        }

        private void OnGUI()
        {
            if (playerData == null)
            {
                EditorGUILayout.LabelField("PlayerData not assigned.");
                return;
            }

            GUILayout.Label("Edit Mission Amounts", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            for (int i = 0; i < playerData.fixedMissionAmountsForDays.Count; i++)
            {
                int missionAmount = EditorGUILayout.IntField("Day " + i, playerData.fixedMissionAmountsForDays[i]);

                if (missionAmount > 4)
                {
                    Debug.LogWarning("Max 4 missions per day can be defined!");
                }
                // Clamp the mission amount to a maximum of 4
                missionAmount = Mathf.Clamp(missionAmount, 0, 4);

            

                playerData.fixedMissionAmountsForDays[i] = missionAmount;
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Save Changes"))
            {
                EditorUtility.SetDirty(playerData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
