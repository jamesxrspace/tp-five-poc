using System;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// This class is copied from an open source project: https://github.com/deadlyfingers/UnityWav.
/// WAV utility for recording and audio playback functions in Unity.
/// Version: 1.0 alpha 1
///
/// - Use "ToAudioClip" method for loading wav file / bytes.
/// Loads .wav (PCM uncompressed) files at 8,16,24 and 32 bits and converts data to Unity's AudioClip.
///
/// - Use "FromAudioClip" method for saving wav file / bytes.
/// Converts an AudioClip's float data into wav byte array at 16 bit.
/// </summary>
/// <remarks>
/// We removed the original AuioClip(using UnityEngine) and replace it as our custom class AudioClipData.
/// </remarks>
namespace TPFive.Game.Record
{
    // TBD: [TF3R-138] [Unity][Optimize] replace wavutility class to wave parser
    public class WavUtility
    {
        private const int WavFileHeaderSize = 44;
        private const ushort BitDepth = 16;
        private const int BlockSize16Bit = 2;

        public static AudioClipData ToAudioClipData(byte[] fileBytes)
        {
            // string riff = Encoding.ASCII.GetString (fileBytes, 0, 4);
            // string wave = Encoding.ASCII.GetString (fileBytes, 8, 4);
            int subchunk1 = BitConverter.ToInt32(fileBytes, 16);
            ushort audioFormat = BitConverter.ToUInt16(fileBytes, 20);

            // NB: Only uncompressed PCM wav files are supported.
            string formatCode = FormatCode(audioFormat);
            Debug.AssertFormat(audioFormat == 1 || audioFormat == 65534, "Detected format code '{0}' {1}, but only PCM and WaveFormatExtensable uncompressed formats are currently supported.", audioFormat, formatCode);

            ushort channels = BitConverter.ToUInt16(fileBytes, 22);
            int sampleRate = BitConverter.ToInt32(fileBytes, 24);

            // int byteRate = BitConverter.ToInt32 (fileBytes, 28);
            // UInt16 blockAlign = BitConverter.ToUInt16 (fileBytes, 32);
            ushort bitDepth = BitConverter.ToUInt16(fileBytes, 34);

            int headerOffset = 16 + 4 + subchunk1 + 4;
            int subchunk2 = BitConverter.ToInt32(fileBytes, headerOffset);

            // Debug.LogFormat ("riff={0} wave={1} subchunk1={2} format={3} channels={4} sampleRate={5} byteRate={6} blockAlign={7} bitDepth={8} headerOffset={9} subchunk2={10} filesize={11}", riff, wave, subchunk1, formatCode, channels, sampleRate, byteRate, blockAlign, bitDepth, headerOffset, subchunk2, fileBytes.Length);
            float[] data = bitDepth switch
            {
                8 => Convert8BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2),
                16 => Convert16BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2),
                24 => Convert24BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2),
                32 => Convert32BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2),
                _ => throw new Exception($"{bitDepth} bit depth is not supported."),
            };

            AudioClipData audioClipData = new AudioClipData(data, (int)channels, sampleRate);
            return audioClipData;
        }

        public static byte[] FromAudioClipData(AudioClipData audioClipData)
        {
            MemoryStream stream = new MemoryStream();

            // get bit depth
            ushort bitDepth = BitDepth;

            // NB: Only supports 16 bit
            // Debug.AssertFormat (bitDepth == 16, "Only converting 16 bit is currently supported. The audio clip data is {0} bit.", bitDepth);

            // total file size = 44 bytes for header format and audioClip.samples * factor due to float to Int16 / sbyte conversion
            int fileSize = (audioClipData.Samples * audioClipData.Channels * BlockSize16Bit) + WavFileHeaderSize;

            // chunk descriptor (riff)
            WriteFileHeader(ref stream, fileSize);

            // file header (fmt)
            WriteFileFormat(ref stream, audioClipData.Channels, audioClipData.Frequency, bitDepth);

            // data chunks (data)
            WriteFileData(ref stream, audioClipData, bitDepth);

            byte[] bytes = stream.ToArray();

            // Validate total bytes
            Debug.AssertFormat(bytes.Length == fileSize, "Unexpected AudioClip to wav format byte count: {0} == {1}", bytes.Length, fileSize);

            stream.Dispose();

            return bytes;
        }

        public static byte[] FromAudioClipData(AudioClipData audioClipData, string destPath, out string fileName)
        {
            byte[] bytes = FromAudioClipData(audioClipData);
            fileName = string.Format("{0}/{1}.{2}", destPath, DateTime.UtcNow.ToString("yyMMdd-HHmmss-fff"), "wav");
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            File.WriteAllBytes(fileName, bytes);
            return bytes;
        }

        public static float[] TrimmedAudioClip(AudioClip audioClip, int startPosition, int endPosition)
        {
            var soundData = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(soundData, 0);
            return soundData[(startPosition * audioClip.channels) .. (endPosition * audioClip.channels)];
        }

        private static int WriteFileHeader(ref MemoryStream stream, int fileSize)
        {
            int count = 0;
            int total = 12;

            // riff chunk id
            byte[] riff = Encoding.ASCII.GetBytes("RIFF");
            count += WriteBytesToMemoryStream(ref stream, riff, "ID");

            // riff chunk size
            int chunkSize = fileSize - 8; // total size - 8 for the other two fields in the header
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(chunkSize), "CHUNK_SIZE");

            byte[] wave = Encoding.ASCII.GetBytes("WAVE");
            count += WriteBytesToMemoryStream(ref stream, wave, "FORMAT");

            // Validate header
            Debug.AssertFormat(count == total, "Unexpected wav descriptor byte count: {0} == {1}", count, total);

            return count;
        }

        private static int WriteFileFormat(ref MemoryStream stream, int channels, int sampleRate, ushort bitDepth)
        {
            int count = 0;
            int total = 24;

            byte[] id = Encoding.ASCII.GetBytes("fmt ");
            count += WriteBytesToMemoryStream(ref stream, id, "FMT_ID");

            int subchunk1Size = 16; // 24 - 8
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(subchunk1Size), "SUBCHUNK_SIZE");

            ushort audioFormat = 1;
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(audioFormat), "AUDIO_FORMAT");

            ushort numChannels = Convert.ToUInt16(channels);
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(numChannels), "CHANNELS");

            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(sampleRate), "SAMPLE_RATE");

            int byteRate = sampleRate * channels * BytesPerSample(bitDepth);
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(byteRate), "BYTE_RATE");

            ushort blockAlign = Convert.ToUInt16(channels * BytesPerSample(bitDepth));
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(blockAlign), "BLOCK_ALIGN");

            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(bitDepth), "BITS_PER_SAMPLE");

            // Validate format
            Debug.AssertFormat(count == total, "Unexpected wav fmt byte count: {0} == {1}", count, total);

            return count;
        }

        private static int WriteFileData(ref MemoryStream stream, AudioClipData audioClipData, ushort bitDepth)
        {
            int count = 0;
            int total = 8;

            byte[] bytes = ConvertAudioClipDataToInt16ByteArray(audioClipData.Buffer);

            byte[] id = Encoding.ASCII.GetBytes("data");
            count += WriteBytesToMemoryStream(ref stream, id, "DATA_ID");

            int subchunk2Size = Convert.ToInt32(audioClipData.Samples * audioClipData.Channels * BlockSize16Bit);
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(subchunk2Size), "SAMPLES");

            // Validate header
            Debug.AssertFormat(count == total, "Unexpected wav data id byte count: {0} == {1}", count, total);

            // Write bytes to stream
            count += WriteBytesToMemoryStream(ref stream, bytes, "DATA");

            // Validate audio data
            Debug.AssertFormat(bytes.Length == subchunk2Size, "Unexpected AudioClip to wav subchunk2 size: {0} == {1}", bytes.Length, subchunk2Size);

            return count;
        }

        private static byte[] ConvertAudioClipDataToInt16ByteArray(float[] data)
        {
            MemoryStream dataStream = new MemoryStream();

            int x = sizeof(short);

            short maxValue = short.MaxValue;

            int i = 0;
            while (i < data.Length)
            {
                dataStream.Write(BitConverter.GetBytes(Convert.ToInt16(data[i] * maxValue)), 0, x);
                ++i;
            }

            byte[] bytes = dataStream.ToArray();

            // Validate converted bytes
            Debug.AssertFormat(data.Length * x == bytes.Length, "Unexpected float[] to Int16 to byte[] size: {0} == {1}", data.Length * x, bytes.Length);

            dataStream.Dispose();

            return bytes;
        }

        private static int WriteBytesToMemoryStream(ref MemoryStream stream, byte[] bytes, string tag = "")
        {
            int count = bytes.Length;
            stream.Write(bytes, 0, count);

            // Debug.LogFormat ("WAV:{0} wrote {1} bytes.", tag, count);
            return count;
        }

        private static float[] Convert8BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 8-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            float[] data = new float[wavSize];

            sbyte maxValue = sbyte.MaxValue;

            int i = 0;
            while (i < wavSize)
            {
                data[i] = (float)source[i] / maxValue;
                ++i;
            }

            return data;
        }

        private static float[] Convert16BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 16-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            int x = sizeof(short); // block size = 2
            int convertedSize = wavSize / x;

            float[] data = new float[convertedSize];

            short maxValue = short.MaxValue;

            int offset = 0;
            int i = 0;
            while (i < convertedSize)
            {
                offset = (i * x) + headerOffset;
                data[i] = (float)BitConverter.ToInt16(source, offset) / maxValue;
                ++i;
            }

            Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

            return data;
        }

        private static float[] Convert24BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 24-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            int x = 3; // block size = 3
            int convertedSize = wavSize / x;

            int maxValue = int.MaxValue;

            float[] data = new float[convertedSize];

            byte[] block = new byte[sizeof(int)]; // using a 4 byte block for copying 3 bytes, then copy bytes with 1 offset

            int offset = 0;
            int i = 0;
            while (i < convertedSize)
            {
                offset = (i * x) + headerOffset;
                Buffer.BlockCopy(source, offset, block, 1, x);
                data[i] = (float)BitConverter.ToInt32(block, 0) / maxValue;
                ++i;
            }

            Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

            return data;
        }

        private static float[] Convert32BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 32-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            int x = sizeof(float); // block size = 4
            int convertedSize = wavSize / x;

            int maxValue = int.MaxValue;

            float[] data = new float[convertedSize];

            int offset = 0;
            int i = 0;
            while (i < convertedSize)
            {
                offset = (i * x) + headerOffset;
                data[i] = (float)BitConverter.ToInt32(source, offset) / maxValue;
                ++i;
            }

            Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

            return data;
        }

        private static int BytesPerSample(ushort bitDepth)
        {
            return bitDepth / 8;
        }

        private static string FormatCode(ushort code) => code switch
        {
            1 => "PCM",
            2 => "ADPCM",
            3 => "IEEE",
            7 => "μ-law",
            65534 => "WaveFormatExtensable",
            _ => string.Empty,
        };
    }
}