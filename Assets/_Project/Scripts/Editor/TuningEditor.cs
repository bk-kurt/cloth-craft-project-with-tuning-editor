using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TuningEditor : EditorWindow
    {
        [SerializeField] private PlayerData playerData;
        
        private Vector2 _scrollPosition;
        private GUIStyle largerLabelStyle; 
        private GUIStyle myLabelStyle; 
        
        [MenuItem("Project Tools/Tuning Editor")]
        public static void Open()
        {
            InitializeWindowAppearance();
        }

        private static void InitializeWindowAppearance()
        {
            TuningEditor window = GetWindow<TuningEditor>();
            window.titleContent = new GUIContent("Tuning Editor");

            window.minSize = new Vector2(400, window.minSize.y);
            window.maxSize = new Vector2(800, window.maxSize.y);

            window.largerLabelStyle = new GUIStyle(EditorStyles.helpBox);
            window.largerLabelStyle.fontSize = 12;
            window.largerLabelStyle.normal.textColor = Color.white;

            window.myLabelStyle = new GUIStyle(EditorStyles.helpBox);
            window.myLabelStyle.fontSize = 12;
            window.myLabelStyle.normal.textColor = Color.green;
        }

        private void OnGUI()
        {
            if (playerData == null)
            {
                EditorGUILayout.LabelField("Player data not assigned.");
                return;
            }

            DrawEditorLayout();
        }

        private void DrawEditorLayout()
        {
            GUILayout.Label("Edit Preferences", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            GUILayout.Space(20);
            
            // Edit float values using sliders
            DrawPriceAdjustmentSection();
            
            GUILayout.Space(40);
            
            DrawExecutionTimeAdjustmentSection();
            
            GUILayout.Space(20);
            
            DrawEquationSection();
            
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Save Changes"))
            {
                EditorUtility.SetDirty(playerData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        
        private void DrawPriceAdjustmentSection()
        {
            GUILayout.Label("Sold Price Multiplier    [0-1000]", EditorStyles.boldLabel);
            playerData.soldClothPricePriceMultiplier =
                EditorGUILayout.IntSlider("Cloth", playerData.soldClothPricePriceMultiplier, 0, 1000);
            GUILayout.Space(5);
            playerData.sewMachinePriceMultiplier =
                EditorGUILayout.IntSlider("Sew Machine", playerData.sewMachinePriceMultiplier, 0, 1000);
            playerData.coloringPotPriceMultiplier =
                EditorGUILayout.IntSlider("Coloring Pot", playerData.coloringPotPriceMultiplier, 0, 1000);
        }
        
        private void DrawExecutionTimeAdjustmentSection()
        {
            GUILayout.Label("Execution Time Base Value (sec)   [0.0-5.0]", EditorStyles.boldLabel);
            playerData.sewMachineExecutionTimeBase = EditorGUILayout.Slider("Sew Machine",
                playerData.sewMachineExecutionTimeBase, 0f, 5f);
            playerData.coloringPotExecutionTimeBase = EditorGUILayout.Slider("Coloring Pot",
                playerData.coloringPotExecutionTimeBase, 0f, 5f);
            
            GUILayout.Space(10);
            
            GUILayout.Label("Execution Time Multiplier   [0.0-2.0]", EditorStyles.boldLabel);
            playerData.sewMachineExecutionTimeMultiplier = EditorGUILayout.Slider("Sew Machine",
                playerData.sewMachineExecutionTimeMultiplier, 0f, 2f);
            playerData.coloringPotExecutionTimeMultiplier = EditorGUILayout.Slider("Coloring Pot",
                playerData.coloringPotExecutionTimeMultiplier, 0f, 2f);
        }

        
        private void DrawEquationSection()
        {
            GUILayout.Label("How is execution time calculated for an executor? (sew machine & coloring pot.)?", myLabelStyle);
            GUILayout.Label("base time + time multiplier * pow2(executer level)", EditorStyles.label);
            GUILayout.Label(
                "Example = Execution time for a level 2 bra sewing machine:\n\n  " + playerData.sewMachineExecutionTimeBase +
                " + " + playerData.sewMachineExecutionTimeMultiplier + " * 2 * 2 = " + (playerData.sewMachineExecutionTimeBase +
                    playerData.sewMachineExecutionTimeMultiplier * 2 * 2) + "sn", largerLabelStyle);
            
            GUILayout.Space(10);
            
            GUILayout.Label("How is the value of a finished cloth calculated?", myLabelStyle);
            GUILayout.Label(
                "color level * cloth type level * price multiplier  \n * is completing a mission or not? 1 (no) or 2 (yes)",
                EditorStyles.label);
            GUILayout.Label(
                "Example = Selling price of a purple colored bra \n \n purple(4) * bra(2) * " +
                playerData.soldClothPricePriceMultiplier + " * 2 (yes) = " + 16 * playerData.soldClothPricePriceMultiplier,
                largerLabelStyle);
        }

        
    }
}