using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JBooth.BetterShaders
{
   public class PipelineHDRP2019 : IPipelineAdapter
   {
      public StringBuilder GetTemplate(Options options, ShaderBuilder.RenderPipeline renderPipeline, BetterShaderUtility util, ref StringBuilder defines)
      {
         StringBuilder template = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2019.txt"));

         var passGBuffer = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2019_PassGBuffer.txt"));
         var passShadow = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2019_PassShadow.txt"));
         var passDepthOnly = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2019_PassDepthOnly.txt"));
         var passForward = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2019_PassForward.txt"));
         var passMeta = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2019_PassMeta.txt"));
         var passSceneSelect = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2019_PassSceneSelection.txt"));
         var vert = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2019_Vert.txt"));
         var hdrpShared = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2019_shared.txt"));
         var hdrpInclude = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2019_include.txt"));
         var passForwardUnlit = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2019_PassForwardUnlit.txt"));

         if (options.disableShadowCasting == Options.Bool.True)
         {
            passShadow.Clear();
         }
         if (options.disableGBuffer == Options.Bool.True)
         {
            passGBuffer.Clear();
         }
         if (options.workflow == Options.Workflow.Unlit)
         {
            passForward = passForwardUnlit;
            passGBuffer.Clear();
         }


         // do alpha

         if (options.alpha != Options.AlphaModes.Opaque)
         {
            passShadow.Clear();
            passDepthOnly.Clear();
            passGBuffer.Clear();
            if (options.alpha == Options.AlphaModes.PreMultiply)
            {
               defines.AppendLine("#define _BLENDMODE_PRE_MULTIPLY 1");
            }
            else
            {
               defines.AppendLine("#define _BLENDMODE_ALPHA 1");
            }
            defines.AppendLine("#define _SURFACE_TYPE_TRANSPARENT 1");
            passForward = passForward.Replace("%FORWARDBASEBLEND%", "Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha\nCull Back\n ZTest LEqual\nZWrite Off");

         }
         else
         {
            passForward = passForward.Replace("%FORWARDBASEBLEND%", "");
         }

         template = template.Replace("%PASSSHADOW%", passShadow.ToString());
         template = template.Replace("%PASSGBUFFER%", passGBuffer.ToString());
         template = template.Replace("%PASSDEPTHONLY%", passDepthOnly.ToString());
         template = template.Replace("%PASSFORWARD%", passForward.ToString());
         template = template.Replace("%PASSMETA%", passMeta.ToString());
         template = template.Replace("%PASSSCENESELECT%", passSceneSelect.ToString());

         template = template.Replace("%HDRPSHARED%", hdrpShared.ToString());
         template = template.Replace("%HDRPINCLUDE%", hdrpInclude.ToString());

         template = template.Replace("%VERT%", vert.ToString());


         // HDRP tags are different, blerg..
         string tagString = "";
         if (options.tags != null)
         {
            tagString = options.tags;
            tagString = "\"RenderPipeline\" = \"HDRenderPipeline\" " + tagString;
            tagString.Replace("Opaque", "HDLitShader");
         }
         else
         {
            tagString = "\"RenderPipeline\" = \"HDRenderPipeline\" \"RenderType\" = \"HDLitShader\" \"Queue\" = \"Geometry+225\"";
         }

         if (options.alpha != Options.AlphaModes.Opaque)
         {
            tagString = tagString.Replace("Geometry+225", "Transparent");
         }

         template = template.Replace("%TAGS%", tagString);

         template.Replace("%SUBSHADERTAGS%", "");
         return template;
      }
   }
}
