﻿using System;
using UnityEditor;
using UnityEngine;

public class GameDataEditor : EditorWindow
{
    public GameData gameData;

    private int indice = 1;
    private int indiceButton = 1;
    private Color OriginalBg = GUI.backgroundColor;
    private Color OriginalCont = GUI.contentColor;
    private Color OriginalColor = GUI.color;
    private const string STR_PercorsoConfig2 = "PercorsoConfigurazione";
    private const string STR_DatabaseDiGioco2 = "/dataBaseDiGioco.asset";
    private static bool preferenzeCaricate = false;
    private static string percorso;


    [PreferenceItem("Alleanze")]
    private static void preferenzeDiGameGUI()
    {
        if (!preferenzeCaricate)
        {
            percorso = EditorPrefs.GetString(STR_PercorsoConfig2);
            preferenzeCaricate = true;
        }
        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string tmpStr = "Assets";
            string tmpPercosro = EditorUtility.OpenFolderPanel("Percorso del Database", tmpStr, "");
            if (tmpPercosro != string.Empty)
            {
                percorso = "Assets" + tmpPercosro.Substring(Application.dataPath.Length);
                EditorPrefs.SetString(STR_PercorsoConfig2, percorso);
            }
        }
        GUILayout.Label(percorso);
        GUILayout.EndHorizontal();
    }
    [MenuItem("Window/ToolsGame/Configurazione Diplomazia %&D")]
    private static void Init()
    {
        EditorWindow.GetWindow<GameDataEditor>("Editor Alleanze");
    }
    private void OnEnable()
    {
        if (EditorPrefs.HasKey(STR_PercorsoConfig2))
        {
            percorso = EditorPrefs.GetString(STR_PercorsoConfig2);
            gameData = AssetDatabase.LoadAssetAtPath<GameData>(percorso + STR_DatabaseDiGioco2);
        }
    }

    private void OnGUI()
    {
        if (gameData != null)
        {
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUILayout.Label("Editor by DFT Students", GUI.skin.GetStyle("Label"));
            GUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            GestisciDiplomazia();
        }
        else
        {
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            if (GUILayout.Button("Crea il DataBase"))
            {
                string tmpStr = "Assets";
                if (percorso == null || percorso == string.Empty)
                {
                    string tmpPercosro = EditorUtility.OpenFolderPanel("Percorso per Database", tmpStr, "");
                    if (tmpPercosro != string.Empty)
                    {
                        percorso = "Assets" + tmpPercosro.Substring(Application.dataPath.Length);
                        EditorPrefs.SetString(STR_PercorsoConfig2, percorso);
                    }
                }
                if (percorso != string.Empty)
                {
                    gameData = ScriptableObject.CreateInstance<GameData>();
                    AssetDatabase.CreateAsset(gameData, percorso + STR_DatabaseDiGioco2);
                    AssetDatabase.Refresh();
                    ProjectWindowUtil.ShowCreatedAsset(gameData);
                }
                resettaParametri();
            }
            EditorGUILayout.HelpBox("DataBase Mancante", MessageType.Error);
            GUILayout.EndHorizontal();
        }
    }
    private void GestisciDiplomazia()
    {
        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
        GUIStyle stileEtichetta = new GUIStyle(GUI.skin.GetStyle("Label"));
        stileEtichetta.alignment = TextAnchor.MiddleCenter;
        stileEtichetta.fontStyle = FontStyle.Bold;
        stileEtichetta.normal.textColor = Color.black;
        stileEtichetta.fontSize = 14;
        GUIStyle stileEtichetta2 = new GUIStyle(GUI.skin.GetStyle("Label"));
        stileEtichetta2.alignment = TextAnchor.MiddleLeft;
        stileEtichetta2.fontStyle = FontStyle.Bold;
        stileEtichetta2.normal.textColor = Color.black;
        stileEtichetta2.fontSize = 11;
        GUILayout.Label("Gestione Diplomazia", stileEtichetta);
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
        if (GUILayout.Button("Resetta", GUILayout.Width(100f)))
        {
            resettaParametri();
            EditorUtility.SetDirty(gameData);
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
        GUILayout.Label(new GUIContent("Matrice Amicizie"), stileEtichetta, GUILayout.Width(130));
        //codice necessario in caso di aggiunta o rimozione di un tag:
        if (gameData.tipoEssere.Length != UnityEditorInternal.InternalEditorUtility.tags.Length - 5)
        {
            int vecchio = gameData.tipoEssere.Length;
            int differenzaLunghezze = UnityEditorInternal.InternalEditorUtility.tags.Length - 5 - gameData.tipoEssere.Length;
            Array.Resize<string>(ref gameData.tipoEssere, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
            Array.Resize<classeAmicizie>(ref gameData.matriceAmicizie, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
            for (int i = 0; i < gameData.matriceAmicizie.Length; i++)
            {
                if (gameData.matriceAmicizie[i] == null)
                    gameData.matriceAmicizie[i] = new classeAmicizie();

                Array.Resize<GameData.Amicizie>(ref gameData.matriceAmicizie[i].elementoAmicizia, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
            }
            if (differenzaLunghezze > 0)
            {
                for (int i = vecchio; i < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; i++)
                {
                    gameData.tipoEssere[i] = UnityEditorInternal.InternalEditorUtility.tags[i + 5];
                    for (int j = 0; j < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; j++)
                    {
                        gameData.matriceAmicizie[i].elementoAmicizia[j] = GameData.Amicizie.Alleato;
                        gameData.matriceAmicizie[j].elementoAmicizia[i] = GameData.Amicizie.Alleato;
                        EditorUtility.SetDirty(gameData);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }
        //codice necessario per l'aggiornamento dei dati in caso qualcosa venga modificato
        for (int i = 0; i < gameData.tipoEssere.Length; i++)
        {
            EditorGUILayout.LabelField(gameData.tipoEssere[i], stileEtichetta2, GUILayout.Width(130));
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.BeginVertical((EditorStyles.objectFieldThumb));
        for (int i = 0; i < gameData.tipoEssere.Length; i++)
        {
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            EditorGUILayout.LabelField(gameData.tipoEssere[i], stileEtichetta2, GUILayout.Width(130));
            for (int j = 0; j < gameData.tipoEssere.Length; j++)
            {
                GameData.Amicizie tmpAmicizia = (GameData.Amicizie)EditorGUILayout.EnumPopup(gameData.matriceAmicizie[i].elementoAmicizia[j], GUILayout.Width(130f));
                if (tmpAmicizia != gameData.matriceAmicizie[i].elementoAmicizia[j])
                {
                    gameData.matriceAmicizie[i].elementoAmicizia[j] = tmpAmicizia;
                    gameData.matriceAmicizie[j].elementoAmicizia[i] = tmpAmicizia;
                    EditorUtility.SetDirty(gameData);
                    AssetDatabase.SaveAssets();
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    private void resettaParametri()
    {
        for (int r = 0; r < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; r++)
        {
            gameData.tipoEssere[r] = UnityEditorInternal.InternalEditorUtility.tags[r + 5];
        }
        for (int r = 0; r < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; r++)
        {
            for (int c = 0; c < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; c++)
            {
                gameData.matriceAmicizie[r].elementoAmicizia[c] = GameData.Amicizie.Alleato;
            }
        }
    }
}