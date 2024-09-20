///////////////////////////////////////////
///
/// Better Shaders
/// ©2021 Jason Booth
/// 


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

   [CustomEditor(typeof(BetterShaderImporter))]
   [CanEditMultipleObjects]
   public class BetterShaderImporterEditor : ScriptedImporterEditor
   {
      public static ShaderBuilder.RenderPipeline renderPipeline = ShaderBuilder.RenderPipeline.Standard;
      

      public override void OnInspectorGUI()
      {
         serializedObject.Update();
         renderPipeline = (ShaderBuilder.RenderPipeline)EditorGUILayout.EnumPopup("Export Pipeline", renderPipeline);
         
         if (GUILayout.Button("Export Shader"))
         {
           ExportShader(renderPipeline,".shader", null, AssetDatabase.GetAssetPath(target));
         }
         
         if (GUILayout.Button("Export All Shaders as TextAssets"))
         {
            EditorUtility.DisplayProgressBar("Exporting shaders","Standard pipeline",0);
            ExportShader(ShaderBuilder.RenderPipeline.Standard,".txt", null, AssetDatabase.GetAssetPath(target));
            EditorUtility.DisplayProgressBar("Exporting shaders","URP2019",0.20f);
            ExportShader(ShaderBuilder.RenderPipeline.URP2019,".txt", null, AssetDatabase.GetAssetPath(target));
            EditorUtility.DisplayProgressBar("Exporting shaders","HDRP2019",0.40f);
            ExportShader(ShaderBuilder.RenderPipeline.HDRP2019,".txt", null, AssetDatabase.GetAssetPath(target));
            EditorUtility.DisplayProgressBar("Exporting shaders", "URP2020 SRP", 0.60f);
            ExportShader(ShaderBuilder.RenderPipeline.URP2020, ".txt", null, AssetDatabase.GetAssetPath(target));
            EditorUtility.DisplayProgressBar("Exporting shaders", "HDRP2020 SRP", 0.80f);
            ExportShader(ShaderBuilder.RenderPipeline.HDRP2020, ".txt", null, AssetDatabase.GetAssetPath(target));

            EditorUtility.ClearProgressBar();
         }
         serializedObject.ApplyModifiedProperties();
         base.ApplyRevertGUI();

      }

      public static string BuildExportShader(ShaderBuilder.RenderPipeline selectedRenderPipeline, OptionOverrides overrides, string assetPath)
      {
         try
         {
            // Read all blocks
            var blocks = new ShaderBlockReader();
            blocks.Read(assetPath);

            // Build the actual shader text from the blocks
            var builder = new ShaderBuilder();
            var text = builder.Build(blocks, assetPath, selectedRenderPipeline, overrides);
            return text;

         }
         catch (System.Exception e)
         {
            Debug.LogException(e);
         }
         return null;
      }

      static void ExportShader(ShaderBuilder.RenderPipeline selectedRenderPipeline, string extension, OptionOverrides overrides, string assetPath)
      {
         var text = BuildExportShader(selectedRenderPipeline, overrides, assetPath);
         assetPath = assetPath.Replace("." + BetterShaderImporter.k_FileExtension, "_" + selectedRenderPipeline.ToString() + extension);
         File.WriteAllText(assetPath, text);
         AssetDatabase.Refresh();
      }
      
   }
   
  
#endif
}
