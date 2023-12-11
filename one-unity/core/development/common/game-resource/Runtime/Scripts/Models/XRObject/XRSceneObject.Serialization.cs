using System.Linq;
using SolidUtilities.UnityEngineInternals;
using TPFive.Game.Resource.Serialization;

namespace TPFive.Game.Resource
{
    /// <summary>
    /// Define some convenient method from XRSceneObject in this file.
    /// </summary>
    public partial class XRSceneObject
    {
        /// <summary>
        /// Collect the data of XRSceneObject and its components into XRObject.
        /// </summary>
        /// <returns>XRObject data for save or pass.</returns>
        public XRObject ToXRObject()
        {
            // Write Transform into XRObject.
            XRObject.WriteComponent(transform.GetType().Name, transform.ToJson());

            // Write other components into XRObject.
            foreach (var comp in transform.GetComponentsInChildren<IComponent>())
            {
                XRObject.WriteComponent(comp.GetType().Name, comp);
            }

            return XRObject;
        }

        /// <summary>
        /// Write XRObject data into XRSceneObject and each components.
        /// </summary>
        /// <param name="xrobject">The XRObject what you want to write.</param>
        public void FromXRObject(XRObject xrobject)
        {
            XRObject = xrobject;

            name = xrobject.ObjectName;

            // Apply Transform to GameObject.
            xrobject.ReadComponent(transform.GetType().Name, out var json);
            transform.FromJson(json);

            // Apply other components to GameObject.
            foreach (var comp in transform.GetComponentsInChildren<IComponent>())
            {
                xrobject.ReadComponent(comp.GetType().Name, comp);
            }
        }
    }
}