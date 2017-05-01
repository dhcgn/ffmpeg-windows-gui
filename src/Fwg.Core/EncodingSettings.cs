using System;
using System.Text;

namespace Fwg.Core
{
    public class EncodingSettings
    {
        public enum CodecEnum
        {
            h265,
            Aac,
            Copy,
            None
        }

        public enum MediaEnum
        {
            Audio,
            Video
        }

        public enum PresetsEnum
        {
            Ultrafast,
            Superfast,
            Veryfast,
            Faster,
            Fast,
            Medium,
            Slow,
            Slower,
            Veryslow,
            Placebo
        }

        public enum RateControlEnum
        {
            ConstantRateFactor,
            ConstantBitRate,
            Remove,
            None
        }

        public MediaEnum Media { get; set; }

        public int Value { get; private set; }

        public PresetsEnum Present { get; private set; }

        public RateControlEnum RateControl { get; private set; }

        public CodecEnum Codec { get; private set; }

        public string GetArgument()
        {
            var sb = new StringBuilder();
            switch (this.Codec)
            {
                case CodecEnum.h265:
                    sb.AppendLine("-c:v libx265 `");
                    break;

                case CodecEnum.Aac:
                    sb.AppendLine("-c:a aac `");
                    break;

                case CodecEnum.Copy:
                    sb.AppendLine("-c:v copy `");
                    break;
                case CodecEnum.None:
                    sb.AppendLine("-an `");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(CodecEnum));
            }

            if (this.Media == MediaEnum.Video)
            {
                sb.AppendLine($"-preset {this.Present.ToString().ToLower()} `");
            }

            switch (this.RateControl)
            {
                case RateControlEnum.ConstantRateFactor:
                    sb.AppendLine($"-crf {this.Value} `");
                    break;
                case RateControlEnum.ConstantBitRate:
                    sb.AppendLine($"-b:a {this.Value}k `");
                    break;
                case RateControlEnum.None:
                case RateControlEnum.Remove:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(RateControlEnum));
            }

            return sb.ToString();
        }

        public static EncodingSettings CreateAudioSettings(CodecEnum codec, RateControlEnum rateControl, int value)
        {
            switch (codec)
            {
                case CodecEnum.Aac:
                    switch (rateControl)
                    {
                        case RateControlEnum.ConstantBitRate:
                            if (value < 0)
                                throw new ArgumentException("ConstantBitRate for ACC must be beetwenn greater than 0 kbps");
                            break;
                        case RateControlEnum.None:
                            if (rateControl != RateControlEnum.None)
                                throw new ArgumentException($"{nameof(RateControlEnum)}:{RateControlEnum.None} is only allowed with {nameof(RateControlEnum)}:{rateControl}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(rateControl), rateControl, null);
                    }
                    break;
                case CodecEnum.None:
                    if (rateControl != RateControlEnum.Remove)
                        throw new ArgumentException($"{nameof(CodecEnum)}:{codec} is only allowed with {nameof(RateControlEnum)}:{RateControlEnum.Remove}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(codec), codec, null);
            }

            return new EncodingSettings
            {
                Codec = codec,
                RateControl = rateControl,
                Present = PresetsEnum.Medium,
                Value = value,
                Media = MediaEnum.Audio
            };
        }

        public static EncodingSettings CreateVideoSettings(CodecEnum codec, RateControlEnum rateControl, PresetsEnum present, int value)
        {
            switch (codec)
            {
                case CodecEnum.h265:
                    switch (rateControl)
                    {
                        case RateControlEnum.ConstantRateFactor:
                            if (value < 0 || value > 51)
                                throw new ArgumentException("CRF must be beetwenn 0 (best) and 51 (worst)");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(rateControl), rateControl, null);
                    }
                    break;
                case CodecEnum.Copy:
                    if (rateControl != RateControlEnum.None)
                        throw new ArgumentException($"{nameof(CodecEnum)}:{CodecEnum.Copy} is only allowed with {nameof(RateControlEnum)}:{rateControl}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(codec), codec, null);
            }

            return new EncodingSettings
            {
                Codec = codec,
                RateControl = rateControl,
                Present = present,
                Value = value,
                Media = MediaEnum.Video
            };
        }
    }
}