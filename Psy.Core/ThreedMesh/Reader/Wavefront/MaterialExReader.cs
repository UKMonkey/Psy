using System;
using System.Collections.Generic;
using System.IO;

namespace Psy.Core.ThreedMesh.Reader.Wavefront
{
    public static class MaterialExReader
    {
         public static List<MaterialEx> ReadFromStream(Stream stream)
         {
             var result = new List<MaterialEx>();

             var streamReader = new StreamReader(stream);

             string materialName = "";
             string lightMode = "";
             string friendlyName = "";

             string line;
             while ((line = streamReader.ReadLine()) != null)
             {
                 var parts = line.Split(' ');
                 var key = parts[0].ToLower();

                 switch (key)
                 {
                     case "mtl":
                         if (!string.IsNullOrEmpty(materialName))
                         {
                             // save current material.
                             result.Add(new MaterialEx
                             {
                                 AmbientLightSource = ParseAmbientLightSource(lightMode),
                                 FriendlyName = friendlyName,
                                 MaterialName = materialName
                             });
                         }
                         materialName = parts[1];
                         break;
                     case "friendlyname":
                         friendlyName = parts[1];
                         break;
                     case "lightmode":
                         lightMode = parts[1];
                         break;
                 }
             }

             if (!string.IsNullOrEmpty(materialName))
             {
                 // save current material.
                 result.Add(new MaterialEx
                 {
                     AmbientLightSource = ParseAmbientLightSource(lightMode),
                     FriendlyName = friendlyName,
                     MaterialName = materialName
                 });
             }

             return result;
         }

        private static AmbientLightSource ParseAmbientLightSource(string lightMode)
        {
            return (AmbientLightSource) Enum.Parse(typeof (AmbientLightSource), lightMode);
        }
    }
}