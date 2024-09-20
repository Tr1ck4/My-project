///////////////////////////////////////////
///
/// Better Shaders
/// ©2021 Jason Booth
/// 

using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


namespace JBooth.BetterShaders
{
#if __BETTERSHADERS__
   class BetterShaderAssetPostProcessor : AssetPostprocessor
   {
      static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
      {
         RegisterShaders(importedAssets);
         
      }

      static void RegisterShaders(string[] paths)
      {
         foreach (var assetPath in paths)
         {
            if (!assetPath.EndsWith(BetterShaderImporter.k_FileExtension, StringComparison.InvariantCultureIgnoreCase) &&
               !assetPath.EndsWith(StackedShaderImporter.k_FileExtension, StringComparison.InvariantCultureIgnoreCase))
            {
               continue;
            }

            var mainObj = AssetDatabase.LoadMainAssetAtPath(assetPath) as Shader;

            if (mainObj != null)
            {
               ShaderUtil.ClearShaderMessages(mainObj);
               if (!ShaderUtil.ShaderHasError(mainObj))
               {
                  ShaderUtil.RegisterShader(mainObj);
               }
            }

            foreach (var obj in AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath))
            {
               if (obj is Shader)
               {
                  Shader s = obj as Shader;
                  ShaderUtil.ClearShaderMessages(s);
                  if (!ShaderUtil.ShaderHasError(s))
                  {
                     ShaderUtil.RegisterShader((Shader)obj);
                  }
               }
            }
         }
      }
   }
#endif
}