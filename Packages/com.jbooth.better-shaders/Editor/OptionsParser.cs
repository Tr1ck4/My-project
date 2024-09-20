///////////////////////////////////////////
///
/// Better Shaders
/// ©2021 Jason Booth
/// 

using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;

namespace JBooth.BetterShaders
{
#if __BETTERSHADERS__

   [System.Serializable]
   public class OptionOverrides
   {
      public string shaderName;
      public bool useCustomEditor;
      public string customEditor;
      public Shader fallback;
     
      public void DrawGUI()
      {
         shaderName = EditorGUILayout.TextField("Override Shader Name", shaderName);
         EditorGUILayout.BeginHorizontal();
         useCustomEditor = EditorGUILayout.Toggle(useCustomEditor, GUILayout.Width(18));
         var old = GUI.enabled;
         GUI.enabled = useCustomEditor;
         customEditor = EditorGUILayout.TextField("Custom Editor", customEditor);
         GUI.enabled = old;
         EditorGUILayout.EndHorizontal();
         fallback = (Shader)EditorGUILayout.ObjectField("Fallback", fallback, typeof(Shader), false);
      }
   }

   public class Options
   {
      public enum AlphaModes
      {
         Opaque,
         Blend,
         PreMultiply
      }

      public enum Workflow
      {
         Metallic,
         Specular,
         Unlit
      }

      public enum TessellationMode
      {
         None,
         Distance,
         Edge
      }
      public enum Bool
      {
         True,
         False
      }

      public class TemplateOverride
      {
         public ShaderBuilder.RenderPipeline renderPipeline;
         public string adapterName;
      }

      public string name;
      public string tags;
      public string dependency;
      public TessellationMode tessellation = TessellationMode.None;
      public string customEditor;
      public string fallback;
      public AlphaModes alpha = AlphaModes.Opaque;
      public Workflow workflow = Workflow.Metallic;
      public Bool disableShadowCasting = Bool.False;
      public Bool disableGBuffer = Bool.False;
      public string shaderTarget;
      public string v2fStrip;
      public string grabPass;
      public Bool stackable = Bool.True;
      public List<TemplateOverride> templateOverrides = new List<TemplateOverride>();
      public Bool enableTransparentDepthPrepass = Bool.False;
      public string texcoord0Mod = "";
      public string texcoord1Mod = "";
      public string texcoord2Mod = "";
      public string texcoord3Mod = "";
      public string vertexColorMod = "";
      public string extrav2f0Mod = "";
      public string extrav2f1Mod = "";
      public string extrav2f2Mod = "";
      public string extrav2f3Mod = "";
      public string extrav2f4Mod = "";
      public string extrav2f5Mod = "";
      public string extrav2f6Mod = "";
      public string extrav2f7Mod = "";
      public string vertexColorRequire = "";
      public string vertexTexcoord3Require = "";
      public string screenPosRequire = "";
      public string extrav2f0Require = "";
      public string extrav2f1Require = "";
      public string extrav2f2Require = "";
      public string extrav2f3Require = "";
      public string extrav2f4Require = "";
      public string extrav2f5Require = "";
      public string extrav2f6Require = "";
      public string extrav2f7Require = "";

      public static void GetOptionEnum<E>(string key, string line, ref E resOut)
       where E : struct
      {

         if (line.StartsWith(key))
         {
            int start = line.IndexOf("\"") + 1;
            int end = line.LastIndexOf("\"");
            if (end > start)
            {
               string enumVal = line.Substring(start, end - start);

               if (System.Enum.IsDefined(typeof(E), enumVal))
               {
                  resOut = (E)System.Enum.Parse(typeof(E),
                     enumVal, true);
                  return;
               }
               // ----------------------------------------
               foreach (var value in
                   System.Enum.GetNames(typeof(E)).Where(value =>
                      value.Equals(enumVal,
                      System.StringComparison.OrdinalIgnoreCase)))
               {
                  resOut = (E)System.Enum.Parse(typeof(E), value);
                  return;
               }
               Debug.LogError(key + " does not have an " + line + " option");
            }
         }
      }


      void GetOptionString(string key, string line, ref string o)
      {
         if (line.StartsWith(key))
         {
            int start = line.IndexOf("\"") + 1;
            int end = line.LastIndexOf("\"");
            if (end > start)
            {
               o = line.Substring(start, end - start).Trim();
            }
         }
      }


      void GetOptionStringAppend(string key, string line, ref string o, string append)
      {
         if (line.StartsWith(key))
         {
            int start = line.IndexOf("\"") + 1;
            int end = line.LastIndexOf("\"");
            if (end > start)
            {
               if (string.IsNullOrEmpty(o))
                  o = line.Substring(start, end - start).Trim();
               else
                  o += append + line.Substring(start, end - start).Trim();
            }
         }
      }

      void GetOptionStringNoQuote(string key, string line, ref string o)
      {
         if (line.StartsWith(key))
         {
            o = line.Substring(key.Length + 1).Trim();
         }
      }

      void GetOptionStringBrace(string key, string line, ref string o)
      {
         if (line.StartsWith(key))
         {
            int start = line.IndexOf("{") + 1;
            int end = line.LastIndexOf("}");
            if (end > start)
            {
               o = line.Substring(start, end - start).Trim();
            }
         }
      }

      void ParseTemplate(string[] rpnames, string l)
      {
         if (!l.Contains("Adapter"))
            return;
         foreach (var r in rpnames)
         {
            if (l.Contains(r))
            {
               var realNames = System.Enum.GetNames(typeof(ShaderBuilder.RenderPipeline));

               for (int i = 0; i < (int)ShaderBuilder.RenderPipeline.kNumPipelines; ++i)
               {
                  if (l.Contains(realNames[i]))
                  {
                     TemplateOverride t = new TemplateOverride();
                     t.renderPipeline = (ShaderBuilder.RenderPipeline)i;
                     string nm = l.Replace(r, "").Trim();
                     t.adapterName = nm;
                     templateOverrides.Add(t);
                  }
               }
            }
         }
         
      }

      internal Options(System.Text.StringBuilder optionBlock, OptionOverrides overrides = null)
      {
         if (optionBlock == null)
            return;
         string[] lines = optionBlock.ToString().StripComments().ToLines();

         string[] rpnames = System.Enum.GetNames(typeof(ShaderBuilder.RenderPipeline));
         for (int i = 0; i < rpnames.Length; ++i)
         {
            rpnames[i] = "Adapter" + rpnames[i];
         }

         for (int i = 0; i < lines.Length; ++i)
         {
            string l = lines[i].Trim();
            GetOptionString("ShaderName", l, ref name);
            GetOptionEnum<TessellationMode>("Tessellation", l, ref tessellation);
            GetOptionString("Fallback", l, ref fallback);
            GetOptionString("CustomEditor", l, ref customEditor);
            GetOptionString("ShaderTarget", l, ref shaderTarget);
            GetOptionEnum<AlphaModes>("Alpha", l, ref alpha);
            GetOptionEnum<Workflow>("Workflow", l, ref workflow);
            GetOptionEnum<Bool>("DisableShadowPass", l, ref disableShadowCasting);
            GetOptionEnum<Bool>("DisableGBufferPass", l, ref disableGBuffer);
            GetOptionStringBrace("Tags", l, ref tags);
            GetOptionStringBrace("Dependency", l, ref dependency);
            GetOptionStringBrace("StripV2F", l, ref v2fStrip);
            GetOptionStringBrace("GrabPass", l, ref grabPass);
            GetOptionEnum<Bool>("Stackable", l, ref stackable);
            GetOptionEnum<Bool>("EnableTransparentDepthPass", l, ref enableTransparentDepthPrepass);
            GetOptionString("VertexColorModifier", l, ref vertexColorMod);
            GetOptionString("TexCoord0Modifier", l, ref texcoord0Mod);
            GetOptionString("TexCoord1Modifier", l, ref texcoord1Mod);
            GetOptionString("TexCoord2Modifier", l, ref texcoord2Mod);
            GetOptionString("TexCoord3Modifier", l, ref texcoord3Mod);
            GetOptionString("ExtraV2F0Modifier", l, ref extrav2f0Mod);
            GetOptionString("ExtraV2F1Modifier", l, ref extrav2f1Mod);
            GetOptionString("ExtraV2F2Modifier", l, ref extrav2f2Mod);
            GetOptionString("ExtraV2F3Modifier", l, ref extrav2f3Mod);
            GetOptionString("ExtraV2F4Modifier", l, ref extrav2f4Mod);
            GetOptionString("ExtraV2F5Modifier", l, ref extrav2f5Mod);
            GetOptionString("ExtraV2F6Modifier", l, ref extrav2f6Mod);
            GetOptionString("ExtraV2F7Modifier", l, ref extrav2f7Mod);

            // we or together these, such that you can end up with #if _FOO || _BAR around one from
            // multiple stackables. Note, however, that we do not test to make sure any other stackable
            // uses these without the require block. Ideally, we would parse each option block for each
            // stackable, then if one of the stackables uses something like .VertexColor but another one
            // has a require, we throw an error and clear the optimization. 
            GetOptionStringAppend("VertexTexCoord3Require", l, ref vertexTexcoord3Require, " || ");
            GetOptionStringAppend("VertexColorRequire", l, ref vertexColorRequire, " || ");
            GetOptionStringAppend("ScreenPosRequire", l, ref screenPosRequire, " || ");
            GetOptionStringAppend("ExtraV2F0Require", l, ref extrav2f0Require, " || ");
            GetOptionStringAppend("ExtraV2F1Require", l, ref extrav2f1Require, " || ");
            GetOptionStringAppend("ExtraV2F2Require", l, ref extrav2f2Require, " || ");
            GetOptionStringAppend("ExtraV2F3Require", l, ref extrav2f3Require, " || ");
            GetOptionStringAppend("ExtraV2F4Require", l, ref extrav2f4Require, " || ");
            GetOptionStringAppend("ExtraV2F5Require", l, ref extrav2f5Require, " || ");
            GetOptionStringAppend("ExtraV2F6Require", l, ref extrav2f6Require, " || ");
            GetOptionStringAppend("ExtraV2F7Require", l, ref extrav2f7Require, " || ");

            ParseTemplate(rpnames, l);
         }

         if (overrides != null)
         {
            if (!string.IsNullOrEmpty(overrides.shaderName))
            {
               name = overrides.shaderName;
            }
            if (overrides.useCustomEditor)
            {
               if (!string.IsNullOrEmpty(overrides.customEditor))
               {
                  customEditor = overrides.customEditor;
               }
               else
               {
                  customEditor = "";
               }
            }
            if (overrides.fallback != null)
            {
               fallback = overrides.fallback.name;
            }
         }
      }

   }
#endif
}