using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      public class FontRandomisationSettings {

            public enum FontStyleShufflingMode {
                  /// <summary> Each font's style remains consistent between it's characters when possible, but the styles will be shuffled around </summary>
                  PerFont,

                  /// <summary> Each character gets a random sprite from a different font </summary>
                  PerLetter
            }

            // Not Implemented
            public enum LetterRandomiationMode {
                  /// <summary> Each character could be swapped with any other character, there is no restriction </summary>
                  Unrestricted,

                  /// <summary> Each group of characters (letter, numbers, symbols) will be shuffles within themselves </summary>
                  Grouped
            }

            public FontRandomisationSettings(bool enabled, bool shuffleFontStyles, FontStyleShufflingMode shufflingMode, bool randomiseLetters) {
                  this.enabled = enabled;
                  this.shuffleFontStyles = shuffleFontStyles;
                  this.shufflingMode = shufflingMode;
                  this.randomiseLetters = randomiseLetters;
            }

            public bool enabled { get; set; } = false;
            public bool shuffleFontStyles { get; set; } = false;
            public FontStyleShufflingMode shufflingMode { get; set; }
            public bool randomiseLetters { get; set; } = false;
      }
}
