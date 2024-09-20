using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JBooth.BetterShaders
{
   public class PipelineURP2020 : IPipelineAdapter
   {
      public StringBuilder GetTemplate(Options options, ShaderBuilder.RenderPipeline renderPipeline, BetterShaderUtility util, ref StringBuilder defines)
      {
         StringBuilder template = new StringBuilder(util.LoadTemplate("BetterShaders_Template_URP2020.txt"));

         var passforward = new StringBuilder(util.LoadTemplate("BetterShaders_Template_URP2020_PassForward.txt"));
         var passShadow = new StringBuilder(util.LoadTemplate("BetterShaders_Template_URP2020_PassShadow.txt"));
         var passMeta = new StringBuilder(util.LoadTemplate("BetterShaders_Template_URP2020_PassMeta.txt"));
         var passDepthOnly = new StringBuilder(util.LoadTemplate("BetterShaders_Template_URP2020_PassDepthOnly.txt"));
         var vert = new StringBuilder(util.LoadTemplate("BetterShaders_Template_URP2020_Vert.txt"));
         var urpInclude = new StringBuilder(util.LoadTemplate("BetterShaders_Template_URP2020_include.txt"));
         var passDepthNormals = new StringBuilder(util.LoadTemplate("BetterShaders_Template_URP2020_PassDepthNormals.txt"));

         // Unity didn't ship this for 2020 after all..
         //var passGBuffer = new StringBuilder(util.LoadTemplate("BetterShaders_Template_URP2020_PassGBuffer.txt"));
         StringBuilder passGBuffer = new StringBuilder();

         if (options.disableShadowCasting == Options.Bool.True)
         {
            passShadow.Clear();
         }
         if (options.disableGBuffer == Options.Bool.True)
         {
            passGBuffer.Clear();
         }

         // do alpha
         if (options.alpha != Options.AlphaModes.Opaque)
         {
            passShadow.Length = 0;
            passDepthOnly.Length = 0;
            passGBuffer.Length = 0;
            passforward = passforward.Replace("%FORWARDBASEBLEND%", "Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha\nCull Back\n ZTest LEqual\nZWrite Off");
            if (options.alpha == Options.AlphaModes.PreMultiply)
            {
               defines.AppendLine("\n   #define _ALPHAPREMULTIPLY_ON 1");
            }
            else
            {
               defines.AppendLine("\n   #define _ALPHABLEND_ON 1");
            }

            passforward = passforward.Insert(0, "\nZWrite Off ColorMask RGB\n\n");
            defines.AppendLine("#define _ALPHABLEND_ON 1\n#define _SURFACE_TYPE_TRANSPARENT 1");

         }
         else
         {
            passforward = passforward.Replace("%FORWARDBASEBLEND%", "Blend One Zero, One Zero\nCull Back\nZTest LEqual\nZWrite On");
         }

         template = template.Replace("%PASSFORWARD%", passforward.ToString());
         template = template.Replace("%PASSMETA%", passMeta.ToString());
         template = template.Replace("%PASSDEPTHONLY%", passDepthOnly.ToString());
         template = template.Replace("%PASSSHADOW%", passShadow.ToString());
         template = template.Replace("%PASSDEPTHNORMALS%", passDepthNormals.ToString());
         template = template.Replace("%PASSGBUFFER%", passGBuffer.ToString());
         template = template.Replace("%VERT%", vert.ToString());
         template = template.Replace("%URPINCLUDE%", urpInclude.ToString());


         // 2020 URP tags are different than 2019
         string tagString = "";
         if (options.tags != null)
         {
            tagString = options.tags;
            tagString = "\"RenderPipeline\"=\"UniversalPipeline\" " + tagString;
         }
         else
         {
            tagString = "\"RenderPipeline\"=\"UniversalPipeline\" \"RenderType\" = \"Opaque\" \"UniversalMaterialType\" = \"Lit\" \"Queue\" = \"Geometry\"";
         }

         if (options.alpha != Options.AlphaModes.Opaque)
         {
            tagString = tagString.Replace("Geometry", "Transparent");
            tagString = tagString.Replace("Opaque", "Transparent");
            tagString = tagString.Replace("Transparent", "Transparent");
         }

         template = template.Replace("%TAGS%", tagString);
         template.Replace("%SUBSHADERTAGS%", "");

         return template;
      }
   }
}
