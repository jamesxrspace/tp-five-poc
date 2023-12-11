using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Game.Resource
{
    /// <summary>
    /// Define some convenient method in this file.
    /// </summary>
    public partial class XRObject
    {
        public string AssetKey() => $"{Uid}.asset";

        public void WriteComponent(string key, IComponent comp)
        {
            WriteComponent(key, comp.Serialize());
        }

        public void WriteComponent(string key, string json)
        {
            Components ??= new Dictionary<string, object>();

            try
            {
                if (Components.ContainsKey(key))
                {
                    Components[key] = json;
                }
                else
                {
                    Components.Add(key, json);
                }
            }
            catch (Exception)
            {
                Debug.LogError($"WriteComponent fail with Key={key}");
                throw;
            }
        }

        public void ReadComponent(string key, IComponent comp)
        {
            try
            {
                ReadComponent(key, out var json);
                comp.Deserialize(json);
            }
            catch (Exception)
            {
                Debug.LogError($"ReadComponent fail with Key={key}");
                throw;
            }
        }

        public void ReadComponent(string key, out string json)
        {
            json = string.Empty;

            if (Components == null)
            {
                return;
            }

            try
            {
                json = (string)Components[key];
            }
            catch (Exception)
            {
                Debug.LogError($"ReadComponent fail with Key={key}");
                throw;
            }
        }
    }
}