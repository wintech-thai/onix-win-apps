/*
* This is auto generated class, DO NOT modify
*/
using System;
using System.IO;

namespace Onix.Client.Helper
{
   public static class CBuild
   {
        public static String BuildVersion = readAppVersion();
        public static String Product = "onix";

        public static string readAppVersion()
        {
            var version = "<version-here>";

            try
            {
                var lines = File.ReadAllLines("build.txt");
                if (lines.Length > 0)
                {
                    version = lines[0];
                }
            }
            catch (Exception ex)
            {
                //Do nothing
            }

            return version;
        }
    }
}