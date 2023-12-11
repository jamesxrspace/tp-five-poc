using System;
using Microsoft.Extensions.Logging;
using VContainer;

namespace TPFive.Room
{
    /// <summary>
    /// IOpenRoomCmd holds information necessary for room-opening operation, which is all encapsulated as IOpenRoomCmdContent.
    /// </summary>
    public interface IOpenRoomCmd
    {
        /// <summary>
        /// Gets a value indicating whether IOpenRoomCmd has content or not.
        /// </summary>
        /// <value>
        /// true: IOpenRoomCmd has content already.
        /// false: IOpenRoomCmd doesn't have content yet.
        /// </value>
        bool HasContent { get; }

        /// <summary>
        /// Set content of IOpenRoomCmd. The method should throw exception when there is content already there.
        /// </summary>
        /// <param name="spaceID"> id of the space defing the room. </param>
        /// <param name="sceneKey"> unity addressable key of the scene used in the room.</param>
        /// <param name="roomID">
        /// id of the Photon session used for the room.
        /// When the room id isn't empty and a session of that id exists there already, Photon will join that session.
        /// Otherwise, depending on the GameMode used while starting Fusion, Photon will create a new session of the specified id, or simply aboard.
        /// When the room id is empty, Photon will join a room using match-making mechanism.
        /// </param>
        /// <param name="region"> the region in which the Photon session will reside.</param>
        void Set(string spaceID, string sceneKey, string roomID = "", PhotonRegion region = PhotonRegion.JP);

        /// <summary>
        /// Get content of an IOpenRoomCmd and clear the content.
        /// </summary>
        /// <param name="content"> This parameter holds the retrieved content. </param>
        /// <returns>
        ///  true: the content has been retrieved.
        ///  false: there is no content there.
        /// </returns>
        bool Fetch(out IOpenRoomCmdContent content);
    }

    public interface IOpenRoomCmdContent
    {
        string SpaceID { get; }

        string SceneKey { get; }

        string RoomID { get; }

        PhotonRegion Region { get; }
    }

    public class OpenRoomCmd : IOpenRoomCmd
    {
        private static readonly string EditorRoomIDKey = "roomID";
        private static readonly string EditorPhotonRegionKey = "photonRegion";
        private readonly ILogger logger;
        private IOpenRoomCmdContent content;

        [Inject]
        public OpenRoomCmd(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<OpenRoomCmd>();
        }

        public bool HasContent => content is not null;

        public void Set(string spaceID, string sceneKey, string roomID = "", PhotonRegion region = PhotonRegion.NONE)
        {
            if (content is not null)
            {
                throw new Exception($"{nameof(OpenRoomCmd)}.{nameof(Set)}: Content is set multiple times before entering room.");
            }

            if (string.IsNullOrEmpty(spaceID))
            {
                throw new Exception($"{nameof(OpenRoomCmd)}.{nameof(Set)}: The given space id is null or empty.");
            }

            if (string.IsNullOrEmpty(sceneKey))
            {
                throw new Exception($"{nameof(OpenRoomCmd)}.{nameof(Set)}: The given scene key is null or empty.");
            }

            if (!region.IsValid())
            {
                throw new Exception($"{nameof(OpenRoomCmd)}.{nameof(Set)}: The given region({region}) is invalid.");
            }

            content = new Content(spaceID, sceneKey, ResolveRoomID(roomID), ResolvePhotonRegion(region));
        }

        public bool Fetch(out IOpenRoomCmdContent content)
        {
            content = this.content;
            this.content = null;
            return content != null;
        }

        private static string ResolveRoomID(string runtimeRoomID = null)
        {
#if UNITY_EDITOR
            return UnityEditor.EditorPrefs.GetString(EditorRoomIDKey, runtimeRoomID);
#else
            return runtimeRoomID;
#endif
        }

        private static PhotonRegion ResolvePhotonRegion(PhotonRegion runtimePhotonRegion = PhotonRegion.NONE)
        {
#if UNITY_EDITOR
            return UnityEditor.EditorPrefs.GetString(EditorPhotonRegionKey, runtimePhotonRegion.ToName()).FromName();
#else
            return runtimePhotonRegion;
#endif
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("TPFive/Room/Set Custom Room ID")]
        private static void SetEditorCustomRoomID()
        {
            Game.Editor.TextInputWindow.Show("Custom RoomID", "Please input custom room id below", ResolveRoomID(), value =>
            {
                UnityEditor.EditorPrefs.SetString(EditorRoomIDKey, value);
            });
        }

        [UnityEditor.MenuItem("TPFive/Room/Set Custom Photon Region")]
        private static void SetEditorCustomPhotonRegion()
        {
            Game.Editor.TextInputWindow.Show("Custom Region", "Please input custom photon region below", ResolvePhotonRegion().ToName(), value =>
            {
                UnityEditor.EditorPrefs.SetString(EditorPhotonRegionKey, value);
            });
        }
#endif

        public class Content : IOpenRoomCmdContent
        {
            public Content(string spaceID, string sceneKey, string roomID, PhotonRegion region)
            {
                SpaceID = spaceID;
                SceneKey = sceneKey;
                RoomID = roomID;
                Region = region;
            }

            public string SpaceID { get; private set; }

            public string SceneKey { get; private set; }

            public string RoomID { get; private set; }

            public PhotonRegion Region { get; private set; }
        }
    }
}