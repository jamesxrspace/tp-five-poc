using System.Collections;
using Microsoft.Extensions.Logging;
using UnityEngine;
using XR.Avatar;
using XR.AvatarEditing.Core;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal sealed class AvatarStyleAccessor : BaseAvatarStyleAccessor
    {
        private readonly ILogger logger;
        private readonly StylizeEditor stylizeEditor;
        private readonly int avatarLayerIndex;

        // Workaround fix changing icon lighting
        private bool changing = false;

        public AvatarStyleAccessor(
            ILoggerFactory loggerFactory,
            StylizeEditor stylizeEditor,
            int avatarLayerIndex = -1)
            : base()
        {
            logger = loggerFactory.CreateLogger<AvatarStyleAccessor>();
            this.stylizeEditor = stylizeEditor;
            this.avatarLayerIndex = avatarLayerIndex;
        }

        public override void Save()
        {
            stylizeEditor.SaveCurrentToTemp();
            stylizeEditor.SaveTempToOrigin();

            OnSave?.Invoke();
        }

        public override void Revert()
        {
            stylizeEditor.RevertAllChange(stylizeEditor.OriginalSettings, avatarLayerIndex);
            stylizeEditor.SaveOriginToTemp();
            InitStyleAccessors();

            OnRevert?.Invoke();
        }

        protected override void InitStyleAccessors()
        {
            InitStyleSetters();
            InitStyleGetters();
        }

        private void InitStyleSetters()
        {
            // Appearance -> Head
            RegisterSetter("face_preset", (styleId, value) =>
            {
                stylizeEditor.setupHeadMesh.SelectMorpherGroup(DefaultAvatarStyleValueSource.StyleIdToMorpherGroupName(styleId), (string)value);
            });
            RegisterSetter("skin_color", (_, value) => stylizeEditor.SetSkinColor((Color)value));

            // Appearacne -> Eyes -> Eyes
            RegisterSetter("eyeliner_color", (styleId, value) =>
            {
                stylizeEditor.SetFaceTextureColor(DefaultAvatarStyleValueSource.StyleIdToEMergeTexturePart(styleId), (Color)value);
            });
            RegisterSetter("eyes_preset", (styleId, value) =>
            {
                stylizeEditor.setupHeadMesh.SelectMorpherGroup(DefaultAvatarStyleValueSource.StyleIdToMorpherGroupName(styleId), (string)value);
            });

            // Appearacne -> Eyes -> Pupil
            RegisterSetter("eye", (_, value) => stylizeEditor.SetEyeTexture((string)value));

            // Appearacne -> Eyes -> Eyebrow
            RegisterSetter("eyebrow_color", (styleId, value) =>
            {
                stylizeEditor.SetFaceTextureColor(DefaultAvatarStyleValueSource.StyleIdToEMergeTexturePart(styleId), (Color)value);
            });
            RegisterSetter("eyebrow_preset", (styleId, value) =>
            {
                stylizeEditor.setupHeadMesh.SelectMorpherGroup(DefaultAvatarStyleValueSource.StyleIdToMorpherGroupName(styleId), (string)value);
            });

            // Appearacne -> Nose
            RegisterSetter("nose_preset", (styleId, value) =>
            {
                stylizeEditor.setupHeadMesh.SelectMorpherGroup(DefaultAvatarStyleValueSource.StyleIdToMorpherGroupName(styleId), (string)value);
            });

            // Appearance -> Mouth
            RegisterSetter("lip_preset", (styleId, value) =>
            {
                stylizeEditor.setupHeadMesh.SelectMorpherGroup(DefaultAvatarStyleValueSource.StyleIdToMorpherGroupName(styleId), (string)value);
            });

            // Appearance -> HairStyle
            RegisterSetter("hair_color", (_, value) => stylizeEditor.SetHairColor((Color)value));
            RegisterSetter("top", (styleId, value) =>
            {
                stylizeEditor.StartCoroutine(ChangePart(styleId, (string)value, stylizeEditor));
            });

            // Appearance -> Beard
            RegisterSetter("beard_color", (styleId, value) =>
            {
                stylizeEditor.SetFaceTextureColor(DefaultAvatarStyleValueSource.StyleIdToEMergeTexturePart(styleId), (Color)value);
            });
            RegisterSetter("beard_style", (_, value) => stylizeEditor.SetFaceTexture((string)value, EMergeTexturePart.M01));

            // Appearance -> Body -> Body
            RegisterSetter("body_height", (_, value) => stylizeEditor.SetHeight((float)value));
            RegisterSetter("body_shape", (_, value) => stylizeEditor.SetShape((int)value));

            // Appearance -> Body -> Decoration
            RegisterSetter("body_scar", (_, value) => stylizeEditor.SetBodyTexture((string)value, EMergeTexturePart.M12));

            // Appearance -> Tatoo -> Head
            RegisterSetter("face_tattoo", (_, value) => stylizeEditor.SetFaceTexture((string)value, EMergeTexturePart.M06));

            // Appearance -> Tatoo -> Decoration
            RegisterSetter("face_scar", (_, value) => stylizeEditor.SetFaceTexture((string)value, EMergeTexturePart.M07));

            // Appearance -> Tatoo -> Body
            RegisterSetter("body_tattoo1", (_, value) => stylizeEditor.SetBodyTexture((string)value, EMergeTexturePart.M09));

            // Appearance -> Tatoo -> Arm
            RegisterSetter("body_tattoo2", (_, value) => stylizeEditor.SetBodyTexture((string)value, EMergeTexturePart.M10));

            // Appearance -> Tatoo -> Feet
            RegisterSetter("body_tattoo3", (_, value) => stylizeEditor.SetBodyTexture((string)value, EMergeTexturePart.M11));

            // Apparel -> Face
            RegisterSetter("facedeck", (styleId, value) =>
            {
                stylizeEditor.StartCoroutine(ChangeAccessory(styleId, (string)value, stylizeEditor));
            });

            // Apparel -> Coat
            RegisterSetter("coat", (styleId, value) =>
            {
                stylizeEditor.StartCoroutine(ChangePart(styleId, (string)value, stylizeEditor));
            });

            // Apparel -> Pants
            RegisterSetter("pants", (styleId, value) => stylizeEditor.StartCoroutine(ChangePart(styleId, (string)value, stylizeEditor)));

            // Apparel -> Socks
            RegisterSetter("body_socks", (styleId, value) => stylizeEditor.SetBodyTexture((string)value, EMergeTexturePart.M13));

            // Apparel -> Foot
            RegisterSetter("foot", (styleId, value) => stylizeEditor.StartCoroutine(ChangePart(styleId, (string)value, stylizeEditor)));

            // Apparel -> Suit
            RegisterSetter("suit", (styleId, value) => stylizeEditor.StartCoroutine(ChangePart(styleId, (string)value, stylizeEditor)));

            // Apparel -> Back
            RegisterSetter("back", (styleId, value) => stylizeEditor.StartCoroutine(ChangePart(styleId, (string)value, stylizeEditor)));

            // Makeup -> blush
            RegisterSetter("blush", (_, value) => stylizeEditor.SetFaceTexture((string)value, EMergeTexturePart.M04));

            // Makeup -> eyeshadow
            RegisterSetter("eyeshadow", (_, value) => stylizeEditor.SetFaceTexture((string)value, EMergeTexturePart.M02));

            // Makeup -> lipstick
            RegisterSetter("lipstick", (_, value) => stylizeEditor.SetFaceTexture((string)value, EMergeTexturePart.M05));

            // Makeup -> nail
            RegisterSetter("nail", (styleId, value) => stylizeEditor.StartCoroutine(ChangePart(styleId, (string)value, stylizeEditor)));
        }

        private void InitStyleGetters()
        {
            // Appearance
            RegisterGetter("face_preset", (_) => string.Empty);
            RegisterGetter("skin_color", (_) => StylizeEditor.StringToColor(stylizeEditor.CurrentSettings.SkinColor));
            RegisterGetter("eyeliner_color", (_) =>
            {
                return StylizeEditor.StringToColor(stylizeEditor.CurrentSettings.FaceTextures.GetPartColor(EMergeTexturePart.M03));
            });
            RegisterGetter("eyes_preset", (_) => string.Empty);
            RegisterGetter("eye", (_) => stylizeEditor.CurrentSettings.FaceTextures.GetPartTexture(EMergeTexturePart.M08));
            RegisterGetter("eyebrow_color", (_) =>
            {
                return StylizeEditor.StringToColor(stylizeEditor.CurrentSettings.FaceTextures.GetPartColor(EMergeTexturePart.M00));
            });
            RegisterGetter("eyebrow_preset", (_) => string.Empty);
            RegisterGetter("nose_preset", (_) => string.Empty);
            RegisterGetter("lip_preset", (_) => string.Empty);
            RegisterGetter("hair_color", (_) => StylizeEditor.StringToColor(stylizeEditor.CurrentSettings.HairColor));
            RegisterGetter("top", (_) => stylizeEditor.CurrentSettings.BodyParts.HairName);
            RegisterGetter("beard_color", (_) =>
            {
                return StylizeEditor.StringToColor(stylizeEditor.CurrentSettings.FaceTextures.GetPartColor(EMergeTexturePart.M01));
            });
            RegisterGetter("beard_style", (_) => stylizeEditor.CurrentSettings.FaceTextures.GetPartTexture(EMergeTexturePart.M01));
            RegisterGetter("body_height", (_) => stylizeEditor.CurrentSettings.Height);
            RegisterGetter("body_shape", (_) => stylizeEditor.CurrentSettings.Shape);
            RegisterGetter("body_scar", (_) => stylizeEditor.CurrentSettings.FaceTextures.GetPartTexture(EMergeTexturePart.M12));
            RegisterGetter("face_tattoo", (_) => stylizeEditor.CurrentSettings.FaceTextures.GetPartTexture(EMergeTexturePart.M06));
            RegisterGetter("face_scar", (_) => stylizeEditor.CurrentSettings.FaceTextures.GetPartTexture(EMergeTexturePart.M07));
            RegisterGetter("body_tattoo1", (_) => stylizeEditor.CurrentSettings.FaceTextures.GetPartTexture(EMergeTexturePart.M09));
            RegisterGetter("body_tattoo2", (_) => stylizeEditor.CurrentSettings.FaceTextures.GetPartTexture(EMergeTexturePart.M10));
            RegisterGetter("body_tattoo3", (_) => stylizeEditor.CurrentSettings.FaceTextures.GetPartTexture(EMergeTexturePart.M11));

            // Apparel
            RegisterGetter("facedeck", (_) => stylizeEditor.CurrentSettings.Attachments.GetPart(EAttachmentPart.A0));
            RegisterGetter("coat", (_) => stylizeEditor.CurrentSettings.BodyParts.CoatName);
            RegisterGetter("pants", (_) => stylizeEditor.CurrentSettings.BodyParts.PantsName);
            RegisterGetter("body_socks", (_) => stylizeEditor.CurrentSettings.FaceTextures.GetPartTexture(EMergeTexturePart.M13));
            RegisterGetter("foot", (_) => stylizeEditor.CurrentSettings.BodyParts.ShoesName);
            RegisterGetter("suit", (_) => stylizeEditor.CurrentSettings.BodyParts.SuitName);
            RegisterGetter("back", (_) => stylizeEditor.CurrentSettings.BodyParts.BackpackName);

            // Makeup
            RegisterGetter("blush", (_) => stylizeEditor.CurrentSettings.FaceTextures.GetPartTexture(EMergeTexturePart.M04));
            RegisterGetter("eyeshadow", (_) => stylizeEditor.CurrentSettings.FaceTextures.GetPartTexture(EMergeTexturePart.M02));
            RegisterGetter("lipstick", (_) => stylizeEditor.CurrentSettings.FaceTextures.GetPartTexture(EMergeTexturePart.M05));
            RegisterGetter("nail", (_) => stylizeEditor.CurrentSettings.BodyParts.NailName);
        }

        private IEnumerator ChangePart(string styleId, string itemName, StylizeEditor editor)
        {
            if (changing)
            {
                yield break;
            }

            var part = DefaultAvatarStyleValueSource.StyleIdToSkinPartType(styleId);

            if (!changing)
            {
                changing = true;

                if (part == ESkinPart.S1 || part == ESkinPart.S3)
                {
                    Set("suit", "None");
                }
                else if (part == ESkinPart.S8)
                {
                    Set("coat", "None");
                    Set("pants", "None");
                }

                changing = false;
            }

            var data = new CrossData();
            yield return editor.SetSkinPartCoroutine(itemName, part, data, avatarLayerIndex);
            if (data.lastError != null)
            {
                logger.LogError(data.lastError.Error());
            }
            else
            {
                editor.SaveCurrentToTemp();
            }
        }

        private IEnumerator ChangeAccessory(string styleId, string itemName, StylizeEditor editor)
        {
            var part = DefaultAvatarStyleValueSource.StyleIdToAttachmentPartType(styleId);
            var data = new CrossData();
            yield return editor.SetAttachmentCoroutine(itemName, part, data, avatarLayerIndex);
            if (data.lastError != null)
            {
                logger.LogError(data.lastError.Error());
            }
            else
            {
                editor.SaveCurrentToTemp();
            }
        }
    }
}