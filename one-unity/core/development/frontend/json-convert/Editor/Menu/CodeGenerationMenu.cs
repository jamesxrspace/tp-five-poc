using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSharpDataModel;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using Debug = UnityEngine.Debug;

namespace CodeGeneration.Editor
{
    public class CodeGenerationMenu
    {
        private static readonly string PkgName = "io.xrspace.jsonconvert";

        [MenuItem("CodeGen/Convert C# model to json schema")]
        internal static void GenerateDartModel()
        {
            try
            {
                if (!GetPkgPath(out var pkgPath))
                {
                    return;
                }

                Assembly assembly = CSharpModelAssembly.Value;
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsVisible)
                    {
                        continue;
                    }

                    if (type == CSharpModelAssembly.IgnoreType)
                    {
                        continue;
                    }

                    // Convert C# data model to JSON schema
                    ConvertToJSONSchema(type);
                }

                Debug.Log("All C# data models are converted to JSON schema.");
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private static bool GetPkgPath(out string pkgPath)
        {
            pkgPath = null;

            // Try retrieving the list of information of all dependent packages of this project.
            var listReqst = Client.List(true);

            // Wait for the request to complete
            while (!listReqst.IsCompleted)
            {
                Task.Delay(100).Wait();
            }

            // Check if some error occurs.
            if (listReqst.Error is not null)
            {
                Debug.LogError(listReqst.Error.message);
                return false;
            }

            // Try fetching the information of the given package.
            var pkgInfo = listReqst.Result.FirstOrDefault((UnityEditor.PackageManager.PackageInfo info) => info.name.Equals(PkgName));
            if (pkgInfo is null)
            {
                Debug.LogError($"Failed retrieving information of the given package '{PkgName}'");
                return false;
            }

            pkgPath = pkgInfo.resolvedPath;
            return true;
        }

        /// <summary>
        /// Convert C# data model to JSON schema by NJsonSchema for .NET
        /// </summary>
        private static void ConvertToJSONSchema(Type type)
        {
            // Temp block
            if (type.Name == "<PrivateImplementationDetails>")
            {
                return;
            }

            // Generate JSON schema
            NJsonSchema.JsonSchema schema = NJsonSchema.JsonSchema.FromType(type);

            // Remove definition property
            var json = JObject.Parse(schema.ToJson());
            json.Property("definitions")?.Remove();

            // Replace "$ref": "XXX" to "$ref": "XXX.json#/"
            var referenceTokens = json.SelectTokens("$..$ref");
            foreach (var value in referenceTokens)
            {
                string removePrefix = Regex.Replace(value.ToString(), "^#/definitions/", string.Empty);
                value.Replace(JToken.FromObject($"{removePrefix}.schema#/"));
            }

            GetPkgPath(out var pkgPath);

            // Write JSON to a file
            var schemaDirectory = Path.Combine(pkgPath, "Editor/", "JSONSchema").Replace('\\', '/');
            Directory.CreateDirectory(schemaDirectory);

            var schemaFilePath = Path.Combine(schemaDirectory, $"{type.Name}.schema").Replace('\\', '/');
            File.WriteAllText(schemaFilePath, json.ToString());
        }
    }
}