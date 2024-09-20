///////////////////////////////////////////
///
/// Better Shaders
/// ©2021 Jason Booth
/// 

using System;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JBooth.BetterShaders
{
#if __BETTERSHADERS__
   public class ShaderBuilder
   {
      StringBuilder Strip(string code, StringBuilder template, string param, string[] keys)
      {
         bool contains = false;
         foreach (var k in keys)
         {
            if (code.Contains(k))
            {
               contains = true;
               break;
            }
         }
         if (contains)
         {
            return template.Replace(param, "");
         }
         else
         {
            return template.Replace(param, "//");
         }
      }

      StringBuilder Strip(string code, StringBuilder template, string param, string key, string stripOption = null)
      {
         if (code.Contains(key) && (stripOption == null || !stripOption.Contains(key.Substring(1))))
         {
            return template.Replace(param, "");
         }
         else
         {
            return template.Replace(param, "//");
         }
      }

      StringBuilder Strip(string code, StringBuilder template, string stripOption)
      {
         const int maxLayers = 30;

         template = Strip(code, template, "%SCREENPOS%", new string[] { ".screenPos", "screenUV" });
         template = Strip(code, template, "%UV0%", ".texcoord0");
         template = Strip(code, template, "%UV1%", ".texcoord1");
         template = Strip(code, template, "%UV2%", ".texcoord2");
         template = Strip(code, template, "%UV3%", ".texcoord3");
         template = Strip(code, template, "%V2FUV0%", ".texcoord0", stripOption);
         template = Strip(code, template, "%V2FUV1%", ".texcoord1", stripOption);
         template = Strip(code, template, "%V2FUV2%", ".texcoord2", stripOption);
         template = Strip(code, template, "%V2FUV3%", ".texcoord3", stripOption);
         template = Strip(code, template, "%VFACE%", ".isFrontFace");
         template = Strip(code, template, "%LOCALSPACEPOSITION%", ".localSpacePosition");
         template = Strip(code, template, "%LOCALSPACENORMAL%", ".localSpaceNormal");
         template = Strip(code, template, "%LOCALSPACETANGENT%", ".localSpaceTangent");
         template = Strip(code, template, "%VERTEXCOLOR%", ".vertexColor");
         template = Strip(code, template, "%V2FVERTEXCOLOR%", ".vertexColor", stripOption);

         template = Strip(code, template, "%EXTRAV2F0%", ".extraV2F0");
         template = Strip(code, template, "%EXTRAV2F1%", ".extraV2F1");
         template = Strip(code, template, "%EXTRAV2F2%", ".extraV2F2");
         template = Strip(code, template, "%EXTRAV2F3%", ".extraV2F3");
         template = Strip(code, template, "%EXTRAV2F4%", ".extraV2F4");
         template = Strip(code, template, "%EXTRAV2F5%", ".extraV2F5");
         template = Strip(code, template, "%EXTRAV2F6%", ".extraV2F6");
         template = Strip(code, template, "%EXTRAV2F7%", ".extraV2F7");
         template = Strip(code, template, "%VERTEXID%", ".vertexID");

         for (int i = 0; i < maxLayers; ++i)
         {
            template = Strip(code, template, "%MODIFYVERTEX" + i + "%", " Ext_ModifyVertex" + i + " ");
            template = Strip(code, template, "%DISPLACEVERTEX" + i + "%", " Ext_ModifyTessellatedVertex" + i + " ");
            template = Strip(code, template, "%SURFACEFUNCTION" + i + "%", " Ext_SurfaceFunction" + i + " ");
            template = Strip(code, template, "%FINALCOLORFORWARD" + i + "%", " Ext_FinalColorForward" + i + " ");
            template = Strip(code, template, "%FINALCOLORGBUFFERSTANDARD" + i + "%", " Ext_FinalGBufferStandard" + i + " ");
         }



         return template;
      }

      StringBuilder ReplaceOrRemove(StringBuilder template, string key, string tag, string option)
      {
         if (!string.IsNullOrEmpty(option))
         {
            return template.Replace(key, tag + " \"" + option + "\"");
         }
         else
         {
            return template.Replace(key, "");
         }
      }

      StringBuilder ReplaceOrRemove(StringBuilder template, string key, string option)
      {
         if (!string.IsNullOrEmpty(option))
         {
            return template.Replace(key, option);
         }
         else
         {
            return template.Replace(key, "");
         }
      }
      public enum RenderPipeline
      {
         Standard = 0,
         URP2019,
         URP2020,
         HDRP2019,
         HDRP2020,
         kNumPipelines
      }

      IPipelineAdapter GetAdapter(Options options, RenderPipeline rp)
      {
         List<System.Type> adapterTypes = new List<System.Type>();
         foreach (var a in System.AppDomain.CurrentDomain.GetAssemblies())
         {
            var adaptTypes = (from System.Type type in a.GetLoadableTypes()
                              where (type.GetInterfaces().Contains(typeof(IPipelineAdapter)))
                              select type).ToArray();

            adapterTypes.AddRange(adaptTypes);
         }

         // see if we have an override
         if (options.templateOverrides != null && options.templateOverrides.Count > 0)
         {
            foreach (var tmp in options.templateOverrides)
            {
               if (tmp.renderPipeline == rp)
               {
                  foreach (var t in adapterTypes)
                  {
                     if (t.Name == tmp.adapterName)
                     {
                        IPipelineAdapter adapter = System.Activator.CreateInstance(t) as IPipelineAdapter;
                        return adapter;
                     }
                  }
               }
            }
         }

         // no override..
         if (rp == RenderPipeline.Standard)
         {
            return new PipelineStandard();
         }
         else if (rp == RenderPipeline.URP2019)
         {
            return new PipelineURP2019();
         }
         else if (rp == RenderPipeline.URP2020)
         {
            return new PipelineURP2020();
         }
         else if (rp == RenderPipeline.HDRP2019)
         {
            return new PipelineHDRP2019();
         }
         else if (rp == RenderPipeline.HDRP2020)
         {
            return new PipelineHDRP2020();
         }
         else
         {
            Debug.LogError("Render pipeline " + rp + " is not handled");
            return new PipelineStandard();
         }
      }

      StringBuilder DoInterpolatorRequire(StringBuilder template, string option, string key, string modkey)
      {
         if (string.IsNullOrEmpty(option))
         {
            return template.Replace(key, "//");
         }
         template = template.Replace(modkey, option);
         return template.Replace(key, "");
      }

      StringBuilder DoAllInterpolatorRequire(StringBuilder template, Options options)
      {
         template = DoInterpolatorRequire(template, options.vertexColorRequire, "%VERTEXCOLORREQUIRE%", "%VERTEXCOLORREQUIREKEY%");
         template = DoInterpolatorRequire(template, options.vertexTexcoord3Require, "%TEXCOORD3REQUIRE%", "%TEXCOORD3REQUIREKEY%");
         template = DoInterpolatorRequire(template, options.screenPosRequire, "%SCREENPOSREQUIRE%", "%SCREENPOSREQUIREKEY%");
         template = DoInterpolatorRequire(template, options.extrav2f0Require, "%EXTRAV2F0REQUIRE%", "%EXTRAV2F0REQUIREKEY%");
         template = DoInterpolatorRequire(template, options.extrav2f1Require, "%EXTRAV2F1REQUIRE%", "%EXTRAV2F1REQUIREKEY%");
         template = DoInterpolatorRequire(template, options.extrav2f2Require, "%EXTRAV2F2REQUIRE%", "%EXTRAV2F2REQUIREKEY%");
         template = DoInterpolatorRequire(template, options.extrav2f3Require, "%EXTRAV2F3REQUIRE%", "%EXTRAV2F3REQUIREKEY%");
         template = DoInterpolatorRequire(template, options.extrav2f4Require, "%EXTRAV2F4REQUIRE%", "%EXTRAV2F4REQUIREKEY%");
         template = DoInterpolatorRequire(template, options.extrav2f5Require, "%EXTRAV2F5REQUIRE%", "%EXTRAV2F5REQUIREKEY%");
         template = DoInterpolatorRequire(template, options.extrav2f6Require, "%EXTRAV2F6REQUIRE%", "%EXTRAV2F6REQUIREKEY%");
         template = DoInterpolatorRequire(template, options.extrav2f7Require, "%EXTRAV2F7REQUIRE%", "%EXTRAV2F7REQUIREKEY%");
         return template;

      }

      StringBuilder BuildTemplate(ShaderBlockReader parser, Options options, RenderPipeline rp, BetterShaderUtility util, ref StringBuilder defines)
      {
         StringBuilder template = null;
         IPipelineAdapter adapter = GetAdapter(options, rp);

         template = adapter.GetTemplate(options, rp, util, ref defines);

         // intert header
         StringBuilder header = new StringBuilder();
         header.AppendLine("////////////////////////////////////////");
         header.AppendLine("// Generated with Better Shaders");
         header.AppendLine("//");
         header.AppendLine("// Auto-generated shader code, don't hand edit!");
         header.AppendLine("//");
         header.AppendLine("//   Unity Version: " + Application.unityVersion);
         header.AppendLine("//   Render Pipeline: " + rp.ToString());
         header.AppendLine("//   Platform: " + Application.platform);
         header.AppendLine("////////////////////////////////////////\n\n");

         template = template.Insert(0, header);

         // add shared code
         var shared = util.LoadTemplate("BetterShaders_shared.txt");
         // process blackboard, if it is used
         if (parser.blocks.blackboard != null)
         {
            shared = shared.Replace("%BLACKBOARD%", parser.blocks.blackboard.ToString());
         }
         else
         {
            shared = shared.Replace("%BLACKBOARD%", "");
         }

         if (!string.IsNullOrEmpty(options.grabPass))
         {
            shared = shared.Replace("%GRABTEXTURE%", options.grabPass.Replace("\"", ""));
            defines.AppendLine("#define _GRABPASSUSED 1");
            defines.AppendLine("#define REQUIRE_OPAQUE_TEXTURE");
         }

         template = template.Replace("%TEMPLATE_SHARED%", shared);
         template = ReplaceOrRemove(template, "%CUSTOMEDITOR%", "CustomEditor", options.customEditor);
         template = ReplaceOrRemove(template, "%FALLBACK%", "Fallback", options.fallback);
         template = ReplaceOrRemove(template, "%DEPENDENCY%", "Dependency", options.dependency);
         template = ReplaceOrRemove(template, "%TEXCOORD0MOD%", options.texcoord0Mod);
         template = ReplaceOrRemove(template, "%TEXCOORD1MOD%", options.texcoord1Mod);
         template = ReplaceOrRemove(template, "%TEXCOORD2MOD%", options.texcoord2Mod);
         template = ReplaceOrRemove(template, "%TEXCOORD3MOD%", options.texcoord3Mod);
         template = ReplaceOrRemove(template, "%VERTEXCOLORMOD%", options.vertexColorMod);
         template = ReplaceOrRemove(template, "%EXTRAV2F0MOD%", options.extrav2f0Mod);
         template = ReplaceOrRemove(template, "%EXTRAV2F1MOD%", options.extrav2f1Mod);
         template = ReplaceOrRemove(template, "%EXTRAV2F2MOD%", options.extrav2f2Mod);
         template = ReplaceOrRemove(template, "%EXTRAV2F3MOD%", options.extrav2f3Mod);
         template = ReplaceOrRemove(template, "%EXTRAV2F4MOD%", options.extrav2f4Mod);
         template = ReplaceOrRemove(template, "%EXTRAV2F5MOD%", options.extrav2f5Mod);
         template = ReplaceOrRemove(template, "%EXTRAV2F6MOD%", options.extrav2f6Mod);
         template = ReplaceOrRemove(template, "%EXTRAV2F7MOD%", options.extrav2f7Mod);
         template = DoAllInterpolatorRequire(template, options);
         return template;
      }

      internal void StripPass(ShaderBlockReader.BlockData blocks, ref StringBuilder shader, string key, string name)
      {

         if (blocks.customPassBlocks.ContainsKey(name))
         {
            shader = shader.Replace(key, blocks.customPassBlocks[name].ToString());
         }
         else
         {
            shader.Replace(key, "");
         }
      }

      internal string Build(ShaderBlockReader parser, string assetPath, RenderPipeline? forcedRP = null,
         OptionOverrides optionOverrides = null)
      {
         BetterShaderUtility util = new BetterShaderUtility();
         var defines = parser.blocks.defines;

#if USING_HDRP
#if UNITY_2020_2_OR_NEWER
         RenderPipeline rp = RenderPipeline.HDRP2020;
#else
         RenderPipeline rp = RenderPipeline.HDRP2019;
#endif
#elif USING_URP


#if UNITY_2020_2_OR_NEWER
         RenderPipeline rp = RenderPipeline.URP2020;
#else
         RenderPipeline rp = RenderPipeline.URP2019;
#endif
#else
         RenderPipeline rp = RenderPipeline.Standard;
#endif

         if (forcedRP != null)
         {
            rp = forcedRP.Value;
         }

         if (rp == RenderPipeline.HDRP2019 || rp == RenderPipeline.HDRP2020)
         {
            defines.AppendLine( "\n   #define _HDRP 1");
         }
         else if (rp == RenderPipeline.URP2019 || rp == RenderPipeline.URP2020)
         {
            defines.AppendLine( "\n   #define _URP 1");
         }
         else
         {
            defines.AppendLine("\n   #define _STANDARD 1");
         }

         Options options = new Options(parser.blocks.options, optionOverrides);

         // build the template

         var shader = BuildTemplate(parser, options, rp, util, ref defines);

         
         // user code
         StringBuilder code = (parser.blocks.code != null) ?  parser.blocks.code : new StringBuilder();
         StringBuilder properties = (parser.blocks.properties != null) ? parser.blocks.properties : new StringBuilder();
         StringBuilder cbuffer = (parser.blocks.cbuffer != null) ? parser.blocks.cbuffer : new StringBuilder();

         string codeNoComments = code.ToString().StripComments();
         // If no shadername block exists, use the filename instead.
         var shaderName = options.name;
         if (string.IsNullOrEmpty(shaderName))
            shaderName = "BetterShaders/" + System.IO.Path.GetFileNameWithoutExtension(assetPath);

         string shaderTarget = "3.0";
         if (codeNoComments.Contains(".vertexID"))
         {
            shaderTarget = "3.5";
         }
         if (rp == RenderPipeline.HDRP2019 || rp == RenderPipeline.HDRP2020)
         {
            shaderTarget = "4.5";
         }
         if (options.tessellation != Options.TessellationMode.None)
         {
            shaderTarget = "4.6";
         }

         if (!string.IsNullOrEmpty(options.shaderTarget))
         {
            shaderTarget = options.shaderTarget;
         }

         if (options.tessellation != Options.TessellationMode.None)
         {
            if (codeNoComments.Contains(" GetTessFactors") && codeNoComments.Contains(" Ext_ModifyTessellatedVertex"))
            {
               StringBuilder tess = new StringBuilder(util.LoadTemplate("BetterShaders_tessellation.txt"));
               DoAllInterpolatorRequire(tess, options);

               defines.AppendLine("\n      #define _TESSELLATION_ON 1");
               if (options.tessellation == Options.TessellationMode.Edge)
               {
                  defines.AppendLine("      #define _TESSEDGE 1");
               }
               shader = shader.Replace("%TESSELLATION%", tess.ToString());
               if (System.Convert.ToSingle(shaderTarget, System.Globalization.CultureInfo.InvariantCulture) < 4.6)
               {
                  shaderTarget = "4.6";
               }
               shader = shader.Replace("%PRAGMAS%", "   #pragma hull Hull\n   #pragma domain Domain\n   #pragma vertex TessVert\n   #pragma fragment Frag\n   #pragma require tesshw\n");
            }
            else
            {
               Debug.LogWarning("Could not compile tessellation into " + assetPath + " because ComputeTessellationAmount or ModifyTessellatedVertex function is missing");
               shader = shader.Replace("%TESSELLATION%", "");
               shader = shader.Replace("%PRAGMAS%", "   #pragma vertex Vert\n   #pragma fragment Frag");
            }
         }
         else
         {
            shader = shader.Replace("%TESSELLATION%", "");
            shader = shader.Replace("%PRAGMAS%", "   #pragma vertex Vert\n   #pragma fragment Frag");
         }


         shader = shader.Replace("%SHADERTARGET%", shaderTarget);

         if (options.workflow == Options.Workflow.Specular)
         {
            defines.AppendLine("\n#define _USESPECULAR 1");
            defines.AppendLine("\n#define _SPECULAR_SETUP");
            defines.AppendLine("#define _MATERIAL_FEATURE_SPECULAR_COLOR 1");  // for hdrp
         }
         else if (options.workflow == Options.Workflow.Unlit)
         {
            defines.AppendLine("\n#define _UNLIT 1");
         }

         if (codeNoComments.Contains(".outputDepth"))
         {
            defines.AppendLine("#define _DEPTHOFFSET_ON");
         }

         // urp requires a define, the rest don't..
         if (rp != RenderPipeline.Standard)
         {
            if (codeNoComments.Contains("GetSceneNormal") ||
               codeNoComments.Contains("GetSceneDepth") ||
               codeNoComments.Contains("GetLinear01Depth") ||
               codeNoComments.Contains("GetLinearEyeDepth") ||
               codeNoComments.Contains("WorldPositionFromDepthBuffer"))
            {
               defines.AppendLine("#define REQUIRE_DEPTH_TEXTURE");
            }
         }

         if (codeNoComments.Contains(".texcoord1"))
         {
            defines.AppendLine("#define _USINGTEXCOORD1 1");
         }
         if (codeNoComments.Contains(".texcoord2"))
         {
            defines.AppendLine("#define _USINGTEXCOORD2 1");
         }


         StringBuilder shaderDesc = new StringBuilder(util.LoadTemplate("BetterShaders_shaderdesc.txt"));
         shaderDesc = DoAllInterpolatorRequire(shaderDesc, options);

         shader = shader.Replace("%SHADERDESC%", shaderDesc.ToString());
         shader = shader.Replace("%SHADERNAME%", shaderName);
         shader = shader.Replace("%PROPERTIES%", properties.ToString());

         // insert chaining at the end of the user code, before template code.
         code.Append("\n");
         StringBuilder chains = new StringBuilder(util.LoadTemplate("BetterShaders_chains.txt"));
         DoAllInterpolatorRequire(chains, options);

         code.AppendLine(chains.ToString());

         if (rp != RenderPipeline.Standard)
         {
            code.Insert(0, "#ifdef unity_WorldToObject\n#undef unity_WorldToObject\n#endif\n#ifdef unity_ObjectToWorld\n#undef unity_ObjectToWorld\n#endif\n#define unity_ObjectToWorld GetObjectToWorldMatrix()\n#define unity_WorldToObject GetWorldToObjectMatrix()\n");
         }

         shader = shader.Replace("%CODE%", code.ToString());
         
         shader = shader.Replace("%CBUFFER%", cbuffer.ToString());

         // custom cbuffer and instanced properties, merged/mutated.
         StringBuilder customCBuffers = new StringBuilder();
         StringBuilder customInstanceProps = new StringBuilder();
         foreach (var key in parser.blocks.customCBuffers.Keys)
         {
            customCBuffers.Append("CBUFFER_START(");
            customCBuffers.Append(key);
            customCBuffers.AppendLine(")");
            customCBuffers.AppendLine(parser.blocks.customCBuffers[key].ToString());
            customCBuffers.AppendLine("CBUFFER_END");
         }
         shader = shader.Replace("%CUSTOMCBUFFER%", customCBuffers.ToString());

         foreach (var key in parser.blocks.instancePropertyBuffer.Keys)
         {
            customInstanceProps.Append("UNITY_INSTANCING_BUFFER_START(");
            customInstanceProps.Append(key);
            customInstanceProps.AppendLine(")");
            customInstanceProps.AppendLine(parser.blocks.instancePropertyBuffer[key].ToString());
            customInstanceProps.Append("UNITY_INSTANCING_BUFFER_END(");
            customInstanceProps.Append(key);
            customInstanceProps.AppendLine(")");
         }
         shader = shader.Replace("%CUSTOMINSTANCEPROPS%", customInstanceProps.ToString());

         if (parser.blocks.customPassBlocks.ContainsKey("all"))
         {
            StripPass(parser.blocks, ref shader, "%PASSFORWARD%", "all");
            StripPass(parser.blocks, ref shader, "%PASSFORWARDADD%", "all");
            StripPass(parser.blocks, ref shader, "%PASSGBUFFER%", "all");
            StripPass(parser.blocks, ref shader, "%PASSDEPTH%", "all");
            StripPass(parser.blocks, ref shader, "%PASSSHADOW%", "all");
            StripPass(parser.blocks, ref shader, "%PASSSELECT%", "all");
            StripPass(parser.blocks, ref shader, "%PASSMETA%", "all");
            StripPass(parser.blocks, ref shader, "%PASSMOTION%", "all");
         }
         else
         {
            StripPass(parser.blocks, ref shader, "%PASSFORWARD%", "forward");
            StripPass(parser.blocks, ref shader, "%PASSFORWARDADD%", "forwardadd");
            StripPass(parser.blocks, ref shader, "%PASSGBUFFER%", "gbuffer");
            StripPass(parser.blocks, ref shader, "%PASSDEPTH%", "depth");
            StripPass(parser.blocks, ref shader, "%PASSSHADOW%", "shadow");
            StripPass(parser.blocks, ref shader, "%PASSSELECT%", "select");
            StripPass(parser.blocks, ref shader, "%PASSMETA%", "meta");
            StripPass(parser.blocks, ref shader, "%PASSMOTION%", "motion");
         }

         if (codeNoComments.Contains(".isFrontFace"))
         {
            defines.Append("#define NEED_FACING 1");
         }

         shader = Strip(codeNoComments, shader, options.v2fStrip);
         

         if (rp != RenderPipeline.Standard)
         {
            shader = shader.Replace("fixed", "half");
            //shader = shader.Replace("UNITY_MATRIX_MVP", "mul(GetWorldToHClipMatrix(), GetObjectToWorldMatrix())");
           // shader = shader.Replace("UNITY_MATRIX_MV", "mul(GetWorldToViewMatrix(), GetObjectToWorldMatrix())");
            shader = shader.Replace("UNITY_MATRIX_M", "GetObjectToWorldMatrix()");
            shader = shader.Replace("UNITY_MATRIX_I_M", "GetWorldToObjectMatrix()");
            shader = shader.Replace("UNITY_MATRIX_VP", "GetWorldToHClipMatrix()");
            shader = shader.Replace("UNITY_MATRIX_V", "GetWorldToViewMatrix()");
            // don't replace UNITY_MATRIX_PREV_VP by mistake
            shader = shader.Replace("UNITY_MATRIX_P,", "GetViewToHClipMatrix(),");
            shader = shader.Replace("UNITY_MATRIX_P ", "GetViewToHClipMatrix() ");
            shader = shader.Replace("UNITY_MATRIX_P)", "GetViewToHClipMatrix())");
         }

         if (parser.blocks.customPass.Length > 1)
         {
            shader = shader.Replace("%CUSTOMPREPASS%", parser.blocks.customPass.ToString());
         }

         // we do defines last, that way people like lennart can do wacky stuff like redefine unity_ObjectToWorld..
         shader = shader.Replace("%DEFINES%", defines.ToString());
         shader = shader.Replace("%STACKIDX%", "");
         shader = shader.Replace("\r\n", "\n");


         

         return shader.ToString();
      }
   }
   
   public static class AssemblyExtensions
   {
      /// <summary>
      /// Assembly.GetTypes() can throw in some cases.  This extension will catch that exception and return only the types which were successfully loaded from the assembly.
      /// </summary>
      public static IEnumerable<Type> GetLoadableTypes(this Assembly asm)
      {
         try
         {
            return asm.GetTypes();
         }
         catch (ReflectionTypeLoadException e)
         {
            return e.Types.Where(t => t != null);
         }
      }
   }
#endif
}