﻿namespace UglyToad.Pdf.Filters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using ContentStream;
    using ContentStream.TypedAccessors;
    using Cos;
    using Logging;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// See section 3.3.3 of the spec (version 1.7) for details on the FlateDecode filter.
    /// The flate decode filter may have a predictor function to further compress the stream.
    /// </remarks>
    public class FlateFilter : IFilter
    {
        // Defaults are from table 3.7 in the spec (version 1.7)
        private const int DefaultColors = 1;
        private const int DefaultBitsPerComponent = 8;
        private const int DefaultColumns = 1;

        private readonly IDecodeParameterResolver decodeParameterResolver;
        private readonly IPngPredictor pngPredictor;
        private readonly ILog log;

        public FlateFilter(IDecodeParameterResolver decodeParameterResolver, IPngPredictor pngPredictor, ILog log)
        {
            this.decodeParameterResolver = decodeParameterResolver;
            this.pngPredictor = pngPredictor;
            this.log = log;
        }

        public byte[] Decode(byte[] input, ContentStreamDictionary streamDictionary, int filterIndex)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var parameters = decodeParameterResolver.GetFilterParameters(streamDictionary, filterIndex);

            var predictor = parameters.GetIntOrDefault(CosName.PREDICTOR);

            try
            {
                var decompressed = Decompress(input);

                if (predictor == -1)
                {
                    return decompressed;
                }

                var colors = Math.Min(parameters.GetIntOrDefault(CosName.COLORS, DefaultColors), 32);
                var bitsPerComponent = parameters.GetIntOrDefault(CosName.BITS_PER_COMPONENT, DefaultBitsPerComponent);
                var columns = parameters.GetIntOrDefault(CosName.COLUMNS, DefaultColumns);

                var result = pngPredictor.Decode(decompressed, predictor, colors, bitsPerComponent, columns);

                return result;
            }
            catch (Exception ex)
            {
                log.Error("Could not decode a flate stream due to an error.", ex);
            }

            return input;
        }

        private byte[] Decompress(byte[] input)
        {
            try
            {
                using (var memoryStream = new MemoryStream(input))
                {
                    // The first 2 bytes are the header which DelfateStream does not support.
                    memoryStream.ReadByte();
                    memoryStream.ReadByte();

                    using (var deflate = new DeflateStream(memoryStream, CompressionMode.Decompress))
                    {
                        var bytes = new List<byte>();

                        var x = deflate.ReadByte();
                        while (x != -1)
                        {
                            bytes.Add((byte)x);
                            x = deflate.ReadByte();
                        }

                        var result = new byte[bytes.Count];

                        for (var i = 0; i < bytes.Count; i++)
                        {
                            result[i] = bytes[i];
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                log?.Error("Could not decode the input using the deflate stream. Input was: " + input, ex);
                throw;
            }
        }
    }
}