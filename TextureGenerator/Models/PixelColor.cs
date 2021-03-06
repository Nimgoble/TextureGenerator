﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
namespace TextureGenerator.Models
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PixelColor
    {
        // 32 bit BGRA
        [JsonIgnore]
        [FieldOffset(0)] public UInt32 ColorBGRA;
        // 8 bit components
        [FieldOffset(0)] public byte Blue;
        [FieldOffset(1)] public byte Green;
        [FieldOffset(2)] public byte Red;
        [FieldOffset(3)] public byte Alpha;
    }
}
