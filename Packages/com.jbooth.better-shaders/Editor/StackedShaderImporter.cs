///////////////////////////////////////////
///
/// Better Shaders
/// ©2021 Jason Booth
/// 

using System.Collections;
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
   [ScriptedImporter(0, StackedShaderImporter.k_FileExtension, 100)]
   public class StackedShaderImporter : ScriptedImporter
   {
      public const string k_FileExtension = ".stackedshader";


      internal static string BuildShaderString(StackedShader stack, string assetPath, out ShaderBlockReader mergeBlocks,
         ShaderBuilder.RenderPipeline? renderPipeline = null, OptionOverrides optionOverrides = null)
      {
         var builder = new ShaderBuilder();
         mergeBlocks = new ShaderBlockReader();

         for (int i = 0; i < stack.shaders.Count; ++i)
         {
            Shader s = stack.shaders[i];
            string sPath = AssetDatabase.GetAssetPath(s);
            if (s == null)
            {
               Debug.LogError("Null shader in shader stack, skipping");
               continue;
            }
            mergeBlocks.includes.Add(AssetDatabase.GetAssetPath(s));

         }
         mergeBlocks.ReadStack();
         var text = builder.Build(mergeBlocks, assetPath, renderPipeline, optionOverrides);
         
         return text;
      }

      public override void OnImportAsset(AssetImportContext ctx)
      {
         string fileContent = File.ReadAllText(ctx.assetPath);
         var stack = ObjectFactory.CreateInstance<StackedShader>();
         if (!string.IsNullOrEmpty(fileContent))
         {
            EditorJsonUtility.FromJsonOverwrite(fileContent, stack);
         }


         if (stack.shaders == null)
         {
            stack.shaders = new List<Shader>();
         }

         

         if (stack.shaders.Count > 0)
         {
            ShaderBlockReader blocks;
            var text = BuildShaderString(stack, ctx.assetPath, out blocks, null, stack.optionOverrides);
            var shader = ShaderUtil.CreateShaderAsset(ctx, text, true);
            foreach (var inc in blocks.includes)
            {
               ctx.DependsOnSourceAsset(inc);
            }

            if (ShaderUtil.ShaderHasError(shader))
            {
               var errors = ShaderUtil.GetShaderMessages(shader);
               BetterShaderImporter.RelayErrors(errors, text, blocks, ctx.assetPath);
            }
            else
            {
               ShaderUtil.ClearShaderMessages(shader);
            }

            ctx.AddObjectToAsset("MainAsset", shader);
            ctx.SetMainObject(shader);

            SubMaterialList ssl = new SubMaterialList();
            ssl.materials = blocks.blocks.subMats.ToArray();
            string json = JsonUtility.ToJson(ssl);
            TextAsset ta = new TextAsset(json);
            ta.hideFlags = HideFlags.HideInHierarchy;
            ctx.AddObjectToAsset("MaterialList", ta);

         }

          
      }


   }
#endif
}
