///////////////////////////////////////////
///
/// Better Shaders
/// ©2021 Jason Booth
/// 

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif
using System.IO;

namespace JBooth.BetterShaders
{
#if __BETTERSHADERS__
   public class StackedShader : ScriptableObject
   {
      public List<Shader> shaders = new List<Shader>();
      public OptionOverrides optionOverrides = new OptionOverrides();


      [MenuItem("Assets/Create/Shader/Better Shader/Stacked Shader", priority = 300)]
      static void CreateMenuItemMinimal()
      {
         string directoryPath = "Assets";
         foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
         {
            directoryPath = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(directoryPath) && File.Exists(directoryPath))
            {
               directoryPath = Path.GetDirectoryName(directoryPath);
               break;
            }
         }
         directoryPath = directoryPath.Replace("\\", "/");
         if (directoryPath.Length > 0 && directoryPath[directoryPath.Length - 1] != '/')
            directoryPath += "/";
         if (string.IsNullOrEmpty(directoryPath))
            directoryPath = "Assets/";

         var fileName = string.Format("New Stacked Shader{0}", StackedShaderImporter.k_FileExtension);
         directoryPath = AssetDatabase.GenerateUniqueAssetPath(directoryPath + fileName);
         StackedShader content = ScriptableObject.CreateInstance<StackedShader>();
         File.WriteAllText(directoryPath, EditorJsonUtility.ToJson(content));
         AssetDatabase.Refresh();
      }
   }


#endif

}