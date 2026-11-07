using System.Collections.Generic;
using UnityEditor;

namespace SPGame.Editor
{
    /// <summary>
    /// Unity Editor 资源查找工具
    /// </summary>
    public static class EditorAssetFinder
    {
        private static readonly string[] s_searchInFolders = { "Assets" };
        #region Find Guids
        public static string[] FindSceneGuids()
        {
            return AssetDatabase.FindAssets(
                "t:SceneAsset",
                s_searchInFolders
            );
        }
        
        public static string[] FindPrefabGuids()
        {
            return AssetDatabase.FindAssets(
                "t:Prefab",
                s_searchInFolders
            );
        }
        
        public static string[] FindMaterialGuids()
        {
            return AssetDatabase.FindAssets(
                "t:Material",
                s_searchInFolders
            );
        }
        
        public static string[] FindTexture2DGuids()
        {
            return AssetDatabase.FindAssets(
                "t:Texture2D",
                s_searchInFolders
            );
        }
        #endregion

        #region Find Paths
        public static List<string> FindScenePaths()
        {
            string[] guids = FindSceneGuids();
            return GUIDsToAssetPaths(guids);
        }
        
        public static List<string> FindPrefabPaths()
        {
            string[] guids = FindPrefabGuids();
            return GUIDsToAssetPaths(guids);
        }
        
        public static List<string> FindMaterialPaths()
        {
            string[] guids = FindMaterialGuids();
            return GUIDsToAssetPaths(guids);
        }
        
        public static List<string> FindTexture2DPaths()
        {
            string[] guids = FindTexture2DGuids();
            return GUIDsToAssetPaths(guids);
        }

        private static List<string> GUIDsToAssetPaths(string[] guids)
        {
            List<string> paths = new List<string>(guids.Length);

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                paths.Add(path);
            }

            return paths;
        }
        #endregion

        #region Find Assets
        public static SceneAsset[] FindSceneAssets()
        {
            string[] guids = FindSceneGuids();
            string[] paths = new string[guids.Length];
            SceneAsset[] sceneAssets = new SceneAsset[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                sceneAssets[i] = AssetDatabase.LoadAssetAtPath<SceneAsset>(paths[i]);
            }

            return sceneAssets;
        } 
        #endregion
    }
}