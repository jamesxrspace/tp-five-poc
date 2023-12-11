using System;
using System.Collections.Generic;
using TPFive.Game.Avatar.Attachment;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

namespace TPFive.Game.Avatar.Timeline.AvatarObjectControl
{
    /// <summary>
    /// It's a copy of <see cref="ControlPlayableAsset"/>
    /// Playable Asset that generates playables for controlling time-related elements on a GameObject.
    /// </summary>
    [Serializable]
    [NotKeyable]
    public class AvatarControlPlayableAsset : PlayableAsset, IPropertyPreview, ITimelineClipAsset
    {
        private const int KMaxRandInt = 10000;
        private static readonly List<PlayableDirector> EmptyDirectorsList = new List<PlayableDirector>(0);
        private static readonly List<ParticleSystem> EmptyParticlesList = new List<ParticleSystem>(0);
        private static readonly HashSet<ParticleSystem> SubEmitterCollector = new HashSet<ParticleSystem>();
        private static HashSet<PlayableDirector> processedDirectors = new HashSet<PlayableDirector>();
        private static HashSet<GameObject> createdPrefabs = new HashSet<GameObject>();

        /// <summary>
        /// GameObject in the scene to control, or the parent of the instantiated prefab.
        /// </summary>
        private ExposedReference<GameObject> _sourceGameObject;

        /// <summary>
        /// Indicates whether Particle Systems will be controlled.
        /// </summary>
        [SerializeField]
        private bool _updateParticle = true;

        /// <summary>
        /// Indicates whether playableDirectors are controlled.
        /// </summary>
        [SerializeField]
        private bool _updateDirector = true;

        /// <summary>
        /// Indicates whether Monobehaviours implementing ITimeControl will be controlled.
        /// </summary>
        [SerializeField]
        private bool _updateITimeControl = true;

        /// <summary>
        /// Indicate whether GameObject activation is controlled.
        /// </summary>
        [SerializeField]
        private bool _active = true;

        /// <summary>
        /// Prefab object that will be instantiated.
        /// </summary>
        [SerializeField]
        private GameObject _prefabGameObject;

        /// <summary>
        /// Random seed to supply particle systems that are set to use autoRandomSeed.
        /// </summary>
        /// <remarks>
        /// This is used to maintain determinism when playing back in timeline. Sub emitters will be assigned incrementing random seeds to maintain determinism and distinction.
        /// </remarks>
        [SerializeField]
        private uint _particleRandomSeed;

        /// <summary>
        /// Indicates whether to search the entire hierarchy for controllable components.
        /// </summary>
        [SerializeField]
        private bool _searchHierarchy;

        /// <summary>
        /// Indicates the active state of the GameObject when Timeline is stopped.
        /// </summary>
        [SerializeField]
        private ActivationControlPlayable.PostPlaybackState _postPlayback = ActivationControlPlayable.PostPlaybackState.Revert;

        private PlayableAsset _controlDirectorAsset;
        private double _duration = PlayableBinding.DefaultDuration;
        private bool _supportLoop;

        [SerializeField]
        private AnchorPointType _avatarAnchor;

        [SerializeField]
        private Vector3 _localPosition;

        [SerializeField]
        private Vector3 _localRotation;

        [SerializeField]
        private Vector3 _localScale = Vector3.one;

        public ExposedReference<GameObject> SourceGameObject => _sourceGameObject;

        public bool UpdateParticle => _updateParticle;

        public bool UpdateDirector => _updateDirector;

        public bool UpdateITimeControl => _updateITimeControl;

        public bool Active => _active;

        /// <summary>
        /// Gets and returns the duration in seconds needed to play the underlying director or particle system exactly once.
        /// </summary>
        /// <value>The duration in seconds.</value>
        public override double duration => _duration;

        /// <summary>
        /// Gets and returns the capabilities of TimelineClips that contain a ControlPlayableAsset.
        /// </summary>
        /// <value>The capabilities of TimelineClips.</value>
        public ClipCaps clipCaps
        {
            get { return ClipCaps.ClipIn | ClipCaps.SpeedMultiplier | (_supportLoop ? ClipCaps.Looping : ClipCaps.None); }
        }

        /// <summary>
        /// Gets a value indicating whether the last instance created control directors.
        /// </summary>
        /// <value>The last instance created control directors.</value>
        internal bool ControllingDirectors { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the last instance created control directors particles.
        /// </summary>
        /// <value>The last instance created control directors particles.</value>
        internal bool ControllingParticles { get; private set; }

        /// <summary>
        /// Creates the root of a Playable subgraph to control the contents of the game object.
        /// </summary>
        /// <param name="graph">PlayableGraph that will own the playable.</param>
        /// <param name="go">The GameObject that triggered the graph build.</param>
        /// <returns>The root playable of the subgraph.</returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            if (_prefabGameObject != null)
            {
                if (createdPrefabs.Contains(_prefabGameObject))
                {
                    Debug.LogWarningFormat("Control Track Clip ({0}) is causing a prefab to instantiate itself recursively. Aborting further instances.", name);
                    return Playable.Create(graph);
                }

                createdPrefabs.Add(_prefabGameObject);
            }

            Playable root = Playable.Null;
            var playables = new List<Playable>();

            GameObject sourceObject = null;
            if (_prefabGameObject != null)
            {
                // Try to get the AvatarAnchorPointProvider from the AvatarTimelinePlayable
                TryGetAvatarAnchorTransform(go, _avatarAnchor, out var parentTransform);
                var controlPlayable = PrefabControlPlayable.Create(graph, _prefabGameObject, parentTransform);
                sourceObject = controlPlayable.GetBehaviour().prefabInstance;
                var sourceObjectTransform = sourceObject.transform;
                sourceObjectTransform.SetLocalPositionAndRotation(_localPosition, Quaternion.Euler(_localRotation));
                sourceObjectTransform.localScale = _localScale;
                playables.Add(controlPlayable);
            }

            _duration = PlayableBinding.DefaultDuration;
            _supportLoop = false;

            ControllingParticles = false;
            ControllingDirectors = false;

            if (sourceObject != null)
            {
                var directors = UpdateDirector ? GetComponent<PlayableDirector>(sourceObject) : EmptyDirectorsList;
                var particleSystems = UpdateParticle ? GetControllableParticleSystems(sourceObject) : EmptyParticlesList;

                // update the duration and loop values (used for UI purposes) here
                // so they are tied to the latest gameObject bound
                UpdateDurationAndLoopFlag(directors, particleSystems);

                if (go.TryGetComponent<PlayableDirector>(out var director))
                {
                    _controlDirectorAsset = director.playableAsset;
                }

                if (go == sourceObject && _prefabGameObject == null)
                {
                    Debug.LogWarningFormat("Control Playable ({0}) is referencing the same PlayableDirector component than the one in which it is playing.", name);
                    _active = false;
                    if (!_searchHierarchy)
                    {
                        _updateDirector = false;
                    }
                }

                var otherPlayables = SearchAndGeneratePlayables(
                    sourceObject,
                    graph,
                    directors,
                    particleSystems,
                    _prefabGameObject != null);

                playables.AddRange(otherPlayables);

                // Connect Playables to Generic to Mixer
                root = ConnectPlayablesToMixer(graph, playables);
            }

            if (_prefabGameObject != null)
            {
                createdPrefabs.Remove(_prefabGameObject);
            }

            if (!root.IsValid())
            {
                root = Playable.Create(graph);
            }

            return root;
        }

        public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            // This method is no longer called by Control Tracks.
            if (director == null)
            {
                return;
            }

            // prevent infinite recursion
            if (processedDirectors.Contains(director))
            {
                return;
            }

            processedDirectors.Add(director);

            var gameObject = SourceGameObject.Resolve(director);
            if (gameObject != null)
            {
                if (UpdateParticle)
                {
                    PreviewParticles(driver, gameObject.GetComponentsInChildren<ParticleSystem>(true));
                }

                if (Active)
                {
                    PreviewActivation(driver, new[] { gameObject });
                }

                if (UpdateITimeControl)
                {
                    PreviewTimeControl(driver, director, GetControlableScripts(gameObject));
                }

                if (UpdateDirector)
                {
                    PreviewDirectors(driver, GetComponent<PlayableDirector>(gameObject));
                }
            }

            processedDirectors.Remove(director);
        }

        internal static IEnumerable<MonoBehaviour> GetControlableScripts(GameObject root)
        {
            if (root == null)
            {
                yield break;
            }

            foreach (var script in root.GetComponentsInChildren<MonoBehaviour>())
            {
                if (script is ITimeControl)
                {
                    yield return script;
                }
            }
        }

        internal static void PreviewParticles(IPropertyCollector driver, IEnumerable<ParticleSystem> particles)
        {
            foreach (var ps in particles)
            {
                driver.AddFromName<ParticleSystem>(ps.gameObject, "randomSeed");
                driver.AddFromName<ParticleSystem>(ps.gameObject, "autoRandomSeed");
            }
        }

        internal static void PreviewActivation(IPropertyCollector driver, IEnumerable<GameObject> objects)
        {
            foreach (var gameObject in objects)
            {
                driver.AddFromName(gameObject, "m_IsActive");
            }
        }

        internal static void PreviewTimeControl(IPropertyCollector driver, PlayableDirector director, IEnumerable<MonoBehaviour> scripts)
        {
            foreach (var script in scripts)
            {
                var propertyPreview = script as IPropertyPreview;
                if (propertyPreview != null)
                {
                    propertyPreview.GatherProperties(director, driver);
                }
                else
                {
                    driver.AddFromComponent(script.gameObject, script);
                }
            }
        }

        internal static void PreviewDirectors(IPropertyCollector driver, IEnumerable<PlayableDirector> directors)
        {
            foreach (var childDirector in directors)
            {
                if (childDirector == null)
                {
                    continue;
                }

                var timeline = childDirector.playableAsset as TimelineAsset;
                if (timeline == null)
                {
                    continue;
                }

                timeline.GatherProperties(childDirector, driver);
            }
        }

        internal IList<T> GetComponent<T>(GameObject gameObject)
            where T : Component
        {
            var components = new List<T>();
            if (gameObject != null)
            {
                if (_searchHierarchy)
                {
                    gameObject.GetComponentsInChildren(true, components);
                }
                else
                {
                    gameObject.GetComponents(components);
                }
            }

            return components;
        }

        internal void UpdateDurationAndLoopFlag(IList<PlayableDirector> directors, IList<ParticleSystem> particleSystems)
        {
            if (directors.Count == 0 && particleSystems.Count == 0)
            {
                return;
            }

            const double invalidDuration = double.NegativeInfinity;

            var maxDuration = invalidDuration;
            var supportsLoop = false;

            foreach (var director in directors)
            {
                if (director.playableAsset == null)
                {
                    continue;
                }

                var assetDuration = director.playableAsset.duration;

                if (director.playableAsset is TimelineAsset && assetDuration > 0.0)
                {
                    // Timeline assets report being one tick shorter than they actually are, unless they are empty
                    assetDuration = (double)((DiscreteTime)assetDuration).OneTickAfter();
                }

                maxDuration = Math.Max(maxDuration, assetDuration);
                supportsLoop = supportsLoop || director.extrapolationMode == DirectorWrapMode.Loop;
            }

            foreach (var particleSystem in particleSystems)
            {
                maxDuration = Math.Max(maxDuration, particleSystem.main.duration);
                supportsLoop = supportsLoop || particleSystem.main.loop;
            }

            _duration = double.IsNegativeInfinity(maxDuration) ? PlayableBinding.DefaultDuration : maxDuration;
            _supportLoop = supportsLoop;
        }

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        protected void OnEnable()
        {
            // can't be set in a constructor
            if (_particleRandomSeed == 0)
            {
                _particleRandomSeed = (uint)Random.Range(1, KMaxRandInt);
            }
        }

        private static Playable ConnectPlayablesToMixer(PlayableGraph graph, List<Playable> playables)
        {
            var mixer = Playable.Create(graph, playables.Count);

            for (int i = 0; i != playables.Count; ++i)
            {
                ConnectMixerAndPlayable(graph, mixer, playables[i], i);
            }

            mixer.SetPropagateSetTime(true);

            return mixer;
        }

        private static void SearchHierarchyAndConnectControlableScripts(IEnumerable<MonoBehaviour> controlableScripts, PlayableGraph graph, List<Playable> outplayables)
        {
            foreach (var script in controlableScripts)
            {
                outplayables.Add(TimeControlPlayable.Create(graph, (ITimeControl)script));
            }
        }

        private static void ConnectMixerAndPlayable(PlayableGraph graph, Playable mixer, Playable playable, int portIndex)
        {
            graph.Connect(playable, 0, mixer, portIndex);
            mixer.SetInputWeight(playable, 1.0f);
        }

        private static void GetControllableParticleSystems(Transform t, ICollection<ParticleSystem> roots, HashSet<ParticleSystem> subEmitters)
        {
            if (t.TryGetComponent<ParticleSystem>(out var ps))
            {
                if (!subEmitters.Contains(ps))
                {
                    roots.Add(ps);
                    CacheSubEmitters(ps, subEmitters);
                }
            }

            for (int i = 0; i < t.childCount; ++i)
            {
                GetControllableParticleSystems(t.GetChild(i), roots, subEmitters);
            }
        }

        private static void CacheSubEmitters(ParticleSystem ps, HashSet<ParticleSystem> subEmitters)
        {
            if (ps == null)
            {
                return;
            }

            for (int i = 0; i < ps.subEmitters.subEmittersCount; i++)
            {
                subEmitters.Add(ps.subEmitters.GetSubEmitterSystem(i));

                // don't call this recursively. subEmitters are only simulated one level deep.
            }
        }

        private void CreateActivationPlayable(GameObject root, PlayableGraph graph, List<Playable> outplayables)
        {
            var activation = ActivationControlPlayable.Create(graph, root, _postPlayback);
            if (activation.IsValid())
            {
                outplayables.Add(activation);
            }
        }

        private void SearchHierarchyAndConnectParticleSystem(IEnumerable<ParticleSystem> particleSystems, PlayableGraph graph, List<Playable> outplayables)
        {
            foreach (var particleSystem in particleSystems)
            {
                if (particleSystem == null)
                {
                    continue;
                }

                ControllingParticles = true;
                outplayables.Add(ParticleControlPlayable.Create(graph, particleSystem, _particleRandomSeed));
            }
        }

        private void SearchHierarchyAndConnectDirector(IEnumerable<PlayableDirector> directors, PlayableGraph graph, List<Playable> outplayables, bool disableSelfReferences)
        {
            foreach (var director in directors)
            {
                if (director == null)
                {
                    continue;
                }

                if (director.playableAsset != _controlDirectorAsset)
                {
                    outplayables.Add(DirectorControlPlayable.Create(graph, director));
                    ControllingDirectors = true;
                }

                // if this self references, disable the director.
                else if (disableSelfReferences)
                {
                    director.enabled = false;
                }
            }
        }

        private IList<ParticleSystem> GetControllableParticleSystems(GameObject go)
        {
            var roots = new List<ParticleSystem>();

            // searchHierarchy will look for particle systems on child objects.
            // once a particle system is found, all child particle systems are controlled with playables
            // unless they are subemitters
            if (_searchHierarchy || go.GetComponent<ParticleSystem>() != null)
            {
                GetControllableParticleSystems(go.transform, roots, SubEmitterCollector);
                SubEmitterCollector.Clear();
            }

            return roots;
        }

        private bool TryGetAvatarAnchorTransform(GameObject avatar, AnchorPointType type, out Transform anchorTransform)
        {
            anchorTransform = null;

            avatar.TryGetComponent<IAvatarTimelinePlayable>(out var avatarTimelinePlayable);

            if (avatarTimelinePlayable == null)
            {
                Debug.LogWarning("No AvatarTimelinePlayable is found in the gameObject.");

                return false;
            }

            var avatarAttachment = avatarTimelinePlayable.AnchorPointProvider;

            if (avatarAttachment == null)
            {
                Debug.LogWarning("The AvatarAnchorPointProvider is null.");

                return false;
            }

            avatarAttachment.TryGetAnchorPoint(type, out anchorTransform);

            if (anchorTransform == null)
            {
                Debug.LogWarning("The parenTransform is null.");

                return false;
            }

            return true;
        }

        private List<Playable> SearchAndGeneratePlayables(
            GameObject sourceObject,
            PlayableGraph graph,
            IEnumerable<PlayableDirector> directors,
            IEnumerable<ParticleSystem> particleSystems,
            bool disableSelfReferences)
        {
            var playables = new List<Playable>();

            if (Active)
            {
                CreateActivationPlayable(sourceObject, graph, playables);
            }

            if (UpdateDirector)
            {
                SearchHierarchyAndConnectDirector(directors, graph, playables, disableSelfReferences);
            }

            if (UpdateParticle)
            {
                SearchHierarchyAndConnectParticleSystem(particleSystems, graph, playables);
            }

            if (UpdateITimeControl)
            {
                SearchHierarchyAndConnectControlableScripts(GetControlableScripts(sourceObject), graph, playables);
            }

            return playables;
        }

#if UNITY_EDITOR
#pragma warning disable SA1204 // Static elements should appear before instance elements
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void OnInitializeOnEnterPlayMode()
        {
            SubEmitterCollector.Clear();
            processedDirectors = new HashSet<PlayableDirector>();
            createdPrefabs = new HashSet<GameObject>();
        }
#pragma warning restore SA1204 // Static elements should appear before instance elements
#endif
    }
}