// Copyright 2022-2026 Niantic Spatial.

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARSubsystems;

namespace NianticSpatial.NSDK.AR.Utilities.Textures
{
    internal static class XRTextureDescriptorUtils
    {
        internal static XRTextureDescriptor Create(
            IntPtr nativeTexture,
            int width,
            int height,
            int mipmapCount,
            TextureFormat format,
            int propertyNameId,
            int depth,
            TextureDimension dimension)
        {
#if ARF_6_1_OR_NEWER
            return new XRTextureDescriptor(
                nativeTexture,
                width,
                height,
                mipmapCount,
                format,
                propertyNameId,
                depth,
                ToXRTextureType(dimension));
#else
            return new XRTextureDescriptor(
                nativeTexture,
                width,
                height,
                mipmapCount,
                format,
                propertyNameId,
                depth,
                dimension);
#endif
        }

#if ARF_6_1_OR_NEWER
        private static XRTextureType ToXRTextureType(TextureDimension dimension)
        {
            switch (dimension)
            {
                case TextureDimension.Tex2D:
                    return XRTextureType.Texture2D;
                case TextureDimension.Tex3D:
                    return XRTextureType.Texture3D;
                case TextureDimension.Cube:
                    return XRTextureType.Cube;
                default:
                    return XRTextureType.None;
            }
        }
#endif
    }
}
