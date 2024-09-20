using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JBooth.BetterShaders
{
   public class PipelineHDRP2020 : IPipelineAdapter
   {
      public StringBuilder GetTemplate(Options options, ShaderBuilder.RenderPipeline renderPipeline, BetterShaderUtility util, ref StringBuilder defines)
      {
         StringBuilder template = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020.txt"));

         var passGBuffer = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_PassGBuffer.txt"));
         var passShadow = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_PassShadow.txt"));
         var passDepthOnly = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_PassDepthOnly.txt"));
         var passDepthForwardOnly = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_PassDepthForwardOnly.txt"));

         var passForward = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_PassForward.txt"));
         var passMeta = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_PassMeta.txt"));
         var passSceneSelect = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_PassSceneSelection.txt"));
         var vert = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_Vert.txt"));
         var hdrpShared = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_shared.txt"));
         var hdrpInclude = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_include.txt"));

         var passMotion = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_PassMotionVector.txt"));
         var passPicking = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_PassPicking.txt"));
         var passTransparentDepth = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_PassTransparentDepthPrepass.txt"));
         var passFullDebug = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_PassFullScreenDebug.txt"));
         var passForwardUnlit = new StringBuilder(util.LoadTemplate("BetterShaders_Template_HDRP2020_PassForwardUnlit.txt"));

         if (options.enableTransparentDepthPrepass == Options.Bool.False)
         {
            passTransparentDepth.Length = 0;
         }
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
         else
         {
            passDepthForwardOnly.Clear();
         }

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
         template = template.Replace("%PASSDEPTHFORWARDONLY%", passDepthForwardOnly.ToString());
         template = template.Replace("%PASSFORWARD%", passForward.ToString());
         template = template.Replace("%PASSMETA%", passMeta.ToString());
         template = template.Replace("%PASSSCENESELECT%", passSceneSelect.ToString());
         template = template.Replace("%PASSMOTIONVECTOR%", passMotion.ToString());
         template = template.Replace("%PASSSCENEPICKING%", passPicking.ToString());
         template = template.Replace("%PASSTRANSPARENTDEPTHPREPASS%", passTransparentDepth.ToString());
         template = template.Replace("%PASSFULLSCREENDEBUG%", passFullDebug.ToString());
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
