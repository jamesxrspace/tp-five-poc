using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TPFive.Game.Avatar.Factory
{
    public interface IAvatarFactory
    {
        /// <summary>
        /// Create an avatar by <see cref="AvatarFormat"/>.
        /// </summary>
        /// <param name="avatarFormat">xrspace avatar format.</param>
        /// <param name="token">cancellation token.</param>
        /// <returns>
        /// Item1 means successful or not.<br/>
        /// Item2 means the avatar <see cref="GameObject"/> root, If Item1 is false, Item2 will be null.
        /// </returns>
        UniTask<(bool, GameObject)> Create(AvatarFormat avatarFormat, CancellationToken token);

        /// <summary>
        /// Create an avatar by <see cref="AvatarFormat"/>.
        /// </summary>
        /// <param name="avatarFormat">xrspace avatar format.</param>
        /// <param name="optionBase">avatar factory options.</param>
        /// <param name="token">cancellation token.</param>
        /// <returns>
        /// Item1 means successful or not.<br/>
        /// Item2 means the avatar <see cref="GameObject"/> root, If Item1 is false, Item2 will be null.
        /// </returns>
        UniTask<(bool, GameObject)> Create(AvatarFormat avatarFormat, OptionBase optionBase, CancellationToken token);

        /// <summary>
        /// Create an avatar by <see cref="AvatarFactoryPreset.presetName"/> and <see cref="AvatarFormat"/>.
        /// </summary>
        /// <param name="presetName">name of avatar preset.</param>
        /// <param name="avatarFormat">xrspace avatar format.</param>
        /// <param name="token">cancellation token.</param>
        /// <returns>
        /// Item1 means successful or not.<br/>
        /// Item2 means the avatar <see cref="GameObject"/> root, If Item1 is false, Item2 will be null.
        /// </returns>
        UniTask<(bool, GameObject)> Create(string presetName, AvatarFormat avatarFormat, CancellationToken token);

        /// <summary>
        /// Create an avatar by <see cref="AvatarFactoryPreset.presetName"/> and <see cref="AvatarFormat"/>.
        /// </summary>
        /// <param name="presetName">name of avatar preset.</param>
        /// <param name="avatarFormat">xrspace avatar format.</param>
        /// <param name="optionBase">avatar factory options.</param>
        /// <param name="token">cancellation token.</param>
        /// <returns>
        /// Item1 means successful or not.<br/>
        /// Item2 means the avatar <see cref="GameObject"/> root, If Item1 is false, Item2 will be null.
        /// </returns>
        UniTask<(bool, GameObject)> Create(string presetName, AvatarFormat avatarFormat, OptionBase optionBase, CancellationToken token);

        /// <summary>
        /// Setup a <see cref="GameObject"/> be an avatar by <see cref="AvatarFormat"/>.
        /// </summary>
        /// <param name="go">want to be an avatar gameobject.</param>
        /// <param name="avatarFormat">xrspace avatar format.</param>
        /// <param name="token">cancellation token.</param>
        /// <returns>
        /// Item1 means successful or not.<br/>
        /// Item2 means the avatar <see cref="GameObject"/> root, If Item1 is false, Item2 will be null.
        /// </returns>
        UniTask<bool> Setup(GameObject go, AvatarFormat avatarFormat, CancellationToken token);

        /// <summary>
        /// Setup a <see cref="GameObject"/> be an avatar by <see cref="AvatarFormat"/>.
        /// </summary>
        /// <param name="go">want to be an avatar gameobject.</param>
        /// <param name="avatarFormat">xrspace avatar format.</param>
        /// <param name="optionBase">avatar factory options.</param>
        /// <param name="token">cancellation token.</param>
        /// <returns>
        /// Item1 means successful or not.<br/>
        /// Item2 means the avatar <see cref="GameObject"/> root, If Item1 is false, Item2 will be null.
        /// </returns>
        UniTask<bool> Setup(GameObject go, AvatarFormat avatarFormat, OptionBase optionBase, CancellationToken token);

        /// <summary>
        /// Setup a <see cref="GameObject"/> be an avatar by <see cref="AvatarFactoryPreset.presetName"/> and <see cref="AvatarFormat"/>.
        /// </summary>
        /// <param name="presetName">name of avatar preset.</param>
        /// <param name="go">want to be an avatar gameobject.</param>
        /// <param name="avatarFormat">xrspace avatar format.</param>
        /// <param name="token">cancellation token.</param>
        /// <returns>
        /// Item1 means successful or not.<br/>
        /// Item2 means the avatar <see cref="GameObject"/> root, If Item1 is false, Item2 will be null.
        /// </returns>
        UniTask<bool> Setup(string presetName, GameObject go, AvatarFormat avatarFormat, CancellationToken token);

        /// <summary>
        /// Setup a <see cref="GameObject"/> be an avatar by <see cref="AvatarFactoryPreset.presetName"/> and <see cref="AvatarFormat"/>.
        /// </summary>
        /// <param name="presetName">name of avatar preset.</param>
        /// <param name="go">want to be an avatar gameobject.</param>
        /// <param name="avatarFormat">xrspace avatar format.</param>
        /// <param name="optionBase">avatar factory options.</param>
        /// <param name="token">cancellation token.</param>
        /// <returns>
        /// Item1 means successful or not.<br/>
        /// Item2 means the avatar <see cref="GameObject"/> root, If Item1 is false, Item2 will be null.
        /// </returns>
        UniTask<bool> Setup(string presetName, GameObject go, AvatarFormat avatarFormat, OptionBase optionBase, CancellationToken token);
    }
}