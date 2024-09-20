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
   [ScriptedImporter(0, BetterShaderImporter.k_FileExtension)]
   public class BetterShaderImporter : ScriptedImporter
   {
      public const string k_FileExtension = "surfshader";

      static string ReadLine(string text, int lineNumber, int around = 0)
      {
         var reader = new StringReader(text);
         string line = null;

         int currentLineNumber = 0;
         int start = lineNumber - around;
         int end = lineNumber + around;
         if (start < 0) start = 0;

         while (currentLineNumber < start)
         {
            currentLineNumber += 1;
            line = reader.ReadLine();
         }
         string ret = "";
         while (line != null && currentLineNumber < end)
         {
            currentLineNumber += 1;
            line = reader.ReadLine();
            if (line != null)
            {
               ret += line + "\n";
            }
         }

         return ret;
      }

      static int CountStringOccurrences(string text, string pattern)
      {
         // Loop through all instances of the string 'text'.
         int count = 0;
         int i = 0;
         while ((i = text.IndexOf(pattern, i)) != -1)
         {
            i += pattern.Length;
            count++;
         }
         return count;
      }


      internal static void RelayErrors(ShaderMessage[] errors, string shaderText, ShaderBlockReader blocks, string path)
      {
         foreach (var e in errors)
         {
            string line = ReadLine(shaderText, e.line, 2);
            List<string> suspects = new List<string>();
            // first check includes
            foreach (var i in blocks.includes)
            {
               var b = File.ReadAllText(i);
               if (b.Contains(line))
               {
                  int index = b.IndexOf(line);
                  string substr = b.Substring(0, Mathf.Clamp(index + line.Length, 0, b.Length));
                  int lineNum = CountStringOccurrences(substr, "\n");
                  suspects.Add("Found in include " + i + " at line " + (lineNum - 2).ToString());
               }

            }
            if (suspects.Count == 0)
            {
               foreach (var inc in blocks.includes)
               {
                  string text = File.ReadAllText(inc);
                  if (text.Contains(line))
                  {
                     suspects.Add("Found in block " + inc + " at line " + (text.IndexOf(line) + 2).ToString());
                  }
               }
            }

            string sus = "";
            foreach (var s in suspects)
            {
               sus += s;
            }

            string rawError = "Shader Error in : " + path + "\n" + e.message + "\n" + e.file + "\n";
            if (sus.Length > 1)
            {
               rawError += sus;
            }
            rawError += "\n" + line + "\n" + e.messageDetails;
            if (e.severity == UnityEditor.Rendering.ShaderCompilerMessageSeverity.Error)
            {
               Debug.LogError(rawError);
            }
            else
            {
               Debug.LogWarning(rawError);
            }
         }
      }

      public override void OnImportAsset(AssetImportContext ctx)
      {
         ShaderBlockReader blocks = null;
         var text = BuildShader(ctx, out blocks);

         var shader = ShaderUtil.CreateShaderAsset(ctx,text,true);

         // In case the shader could not be compiled, use an "error shader" instead.
         if (ShaderUtil.ShaderHasError(shader))
         {
            var errors = ShaderUtil.GetShaderMessages(shader);
            RelayErrors(errors, text, blocks, ctx.assetPath);
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

      string BuildShader(AssetImportContext ctx, out ShaderBlockReader blocks)
      {
         try
         {
            // Read all blocks
            blocks = new ShaderBlockReader();
            blocks.Read(ctx.assetPath);

            // Mark included files as dependencies
            foreach (var include in blocks.includes)
               ctx.DependsOnSourceAsset(include);

            // Build the actual shader text from the blocks
            var builder = new ShaderBuilder();
            var text = builder.Build(blocks, ctx.assetPath);

            return text;
         }
         catch (System.Exception e)
         {
            Debug.LogException(e);
         }
         blocks = null;
         return "error";
      }

      static void CreateMenuShader(string name)
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

         var fileName = string.Format("New Better Shader.{0}", k_FileExtension);
         directoryPath = AssetDatabase.GenerateUniqueAssetPath(directoryPath + fileName);
         BetterShaderUtility util = new BetterShaderUtility();
         var content = util.LoadTemplate(name);
         ProjectWindowUtil.CreateAssetWithContent(directoryPath, content);
      }

      [MenuItem("Assets/Create/Shader/Better Shader/Minimal", priority = 310)]
      static void CreateMenuItemMinimal()
      {
         CreateMenuShader("BetterShaders_New_Slim.txt");
      }

      [MenuItem("Assets/Create/Shader/Better Shader/Documented", priority = 311)]
      static void CreateMenuItemDoc()
      {
         CreateMenuShader("BetterShaders_New_Document.txt");
      }

      [MenuItem("Assets/Create/Shader/Better Shader/Standard", priority = 312)]
      static void CreateMenuItemDocStandard()
      {
         CreateMenuShader("BetterShaders_New_Standard.txt");
      }

      [MenuItem("Assets/Create/Shader/Better Shader/Tessellation", priority = 315)]
      static void CreateMenuItemDocTessellation()
      {
         CreateMenuShader("BetterShaders_New_Tessellation.txt");
      }
   }
#endif
}
