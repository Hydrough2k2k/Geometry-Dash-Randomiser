using System.ComponentModel.Design;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace Geometry_Dash_Randomiser {

      public class Sprite {

            public enum Type { Unknown, Icon, Block, Portal, OrbsAndPads, Editor, Menu, Particle, Effect, Miscellaneous, Shop }

            // What file did this come from, for example: "bird_01-uhd"
            // Might be retired in place of a container containing an array of sprites if it makes sense. SpriteSheet is a good name, maybe
            public string sourceFile { get; set; } = string.Empty;

            // Name of the sprite, for example: "bird_01_001.png"
            public string spriteName { get; set; } = string.Empty;

            // replace with Point eventually maybe
            public Point spriteOffset { get; set; } = new Point();
            public Point spriteSize { get; set; } = new Point();
            public Point spriteSourceSize { get; set; } = new Point();
            public Rectangle textureRect { get; set; } = new Rectangle();
            public bool textureRotated { get; set; } = false;
            public Type type { get; set; } = Sprite.Type.Unknown;

            // The cropper bitmap for the sprite
            public Bitmap texture = null;

            public Sprite() { }

            public Sprite(string sourceFile, string spriteName, Point spriteOffset,
                  Point spriteSize, Point spriteSourceSize, Rectangle textureRect,
                  bool textureRotated, Type type, Bitmap texture) {

                  this.sourceFile = sourceFile;
                  this.spriteName = spriteName;
                  this.spriteOffset = spriteOffset;
                  this.spriteSize = spriteSize;
                  this.spriteSourceSize = spriteSourceSize;
                  this.textureRect = textureRect;
                  this.textureRotated = textureRotated;
                  this.type = type;
                  this.texture = texture;
            }

            public Sprite(string sourceFile, string spriteName, Type type) {
                  this.sourceFile = sourceFile;
                  this.spriteName = spriteName;
                  this.type = type;
            }

            //public void SwapTextureData(ref Sprite other) {
            //      // keep only type, name and source file, everything else is (x, y) = (y, x);

            //}

            /// <summary>
            /// This will deduce the sprite type based on the spriteName string
            /// </summary>
            public void AssignType() {
                  if (this.isIconType()) {
                        type = Type.Icon;
                  } else if (sourceFile.StartsWith("FireSheet_01")) {
                        type = Type.Block;
                  } else if (sourceFile.StartsWith("GauntletSheet")) {
                        type = Type.Menu;
                  } else if (sourceFile.StartsWith("GJ_GameSheetEditor")) {
                        type = Type.Editor;
                  } else if (sourceFile.StartsWith("GJ_GameSheetGlow")) {
                        type = getTypeFromGlowSheets();
                  } else if (sourceFile.StartsWith("GJ_GameSheet")) {
                        type = getTypeFromGameSheets();
                  } else if (sourceFile.StartsWith("GJ_LaunchSheet")) {
                        type = Type.Menu;
                  } else if (sourceFile.StartsWith("GJ_ParticleSheet")) {
                        type = Type.Particle;
                  } else if (sourceFile.StartsWith("GJ_PathSheet")) {
                        type = Type.Menu;
                  } else if (sourceFile.StartsWith("GJ_ShopSheet")) {
                        type = Type.Shop;
                  } else if (sourceFile.StartsWith("PixelSheet")) {
                        type = Type.Block;
                  } else if (sourceFile.StartsWith("SecretSheet")) {
                        type = Type.Miscellaneous;
                  } else if (sourceFile.StartsWith("TowerSheet")) {
                        type = Type.Miscellaneous;
                  } else if (sourceFile.StartsWith("TreasureRoomSheet")) {
                        type = Type.Miscellaneous;
                  } else if (sourceFile.StartsWith("WorldSheet")) {
                        type = Type.Miscellaneous;
                  }
            }

            Type getTypeFromGlowSheets() {
                  if (spriteName.Contains("boost")) {
                        return Type.Portal;
                  } else if (spriteName.Contains("bump") ||
                        spriteName.Contains("Bump") ||
                        spriteName.Contains("ring") ||
                        spriteName.Contains("Ring")) {

                        return Type.OrbsAndPads;
                  }
                  return Type.Block;
            }

            Type getTypeFromGameSheets() {
                  if (sourceFile.StartsWith("GJ_GameSheet04")) {
                        return getTypeFromGameSheet_4();
                  } else if (sourceFile.StartsWith("GJ_GameSheet03")) {
                        return getTypeFromGameSheet_3();
                  } else if (sourceFile.StartsWith("GJ_GameSheet02")) {
                        return getTypeFromGameSheet_2();
                  } else if (sourceFile.StartsWith("GJ_GameSheet")) {
                        return getTypeFromGameSheet_1();
                  }
                  // TO-DO: Flag error for there's an unknown file

                  return Type.Unknown;
            }

            // Done
            Type getTypeFromGameSheet_1() {
                  if (spriteName.Contains("teleportRing") || // TP Orb
                        spriteName.Contains("dashRing") || // Dash Orb
                        spriteName.Contains("spiderRing") || // Spider Orb
                        spriteName.Contains("gravJumpRing") || // Green Orb
                        spriteName.Contains("gravring") || // Blue Orb
                        spriteName.Contains("ring_0") || // Yellow, Pink, Red Orbs
                        spriteName.Contains("bump") || // Yellow, Pink, Blue, Red Pads
                        spriteName.Contains("spiderBump") || // TP Pad
                        spriteName.Contains("dropRing")) { // Black Orb

                        return Type.OrbsAndPads;
                  } else if (spriteName.Contains("ParticleBtn")) {
                        return Type.Editor;
                  }
                  return Type.Block;
            }

            // Done
            Type getTypeFromGameSheet_2() {
                  if (spriteName.Contains("boost")) {
                        return Type.Portal;
                  } else if (spriteName.Contains("portal")) {
                        return Type.Portal;
                  } else if (spriteName.Contains("edit")) {
                        return Type.Editor;
                  } else if (spriteName.Contains("keyframeIcon")) {
                        return Type.Editor;
                  } else if (spriteName.Contains("floorLine") ||
                        spriteName.Contains("checkpoint") ||
                        spriteName.Contains("secretCoin") ||
                        spriteName.Contains("time")) {

                        return Type.Miscellaneous;
                  }
                  return Type.Block;
            }

            //  - Unknowns for GameSheet3:
            // GJ_fxOn/OffBtn
            // GJ_longBtn
            // GJ_navDotBtn
            // GJ_orderUp
            // GJ_pause
            // GJ_plain
            // GJ_plusBtn 1, 2 and 3
            // GJ_hideBtn

            // Done
            Type getTypeFromGameSheet_3() {
                  // The order matters, don't just group it by return type!
                  if (spriteName.Contains("arrow")) {
                        return Type.Editor;
                  } else if (spriteName.Contains("checkpointBtn") ||
                        spriteName.StartsWith("GJ_delete") ||
                        spriteName.Contains("duplicate") ||
                        spriteName.Contains("everyplayBtn")) { // Important to keep these to make sure the items are sepatrated

                        return Type.Menu;

                  } else if (spriteName.Contains("check")) {
                        return Type.Editor;
                  } else if (spriteName.StartsWith("diff")) { // Difficulty icons "diffIcon" and "difficulty" will be caught by this
                        return Type.Menu;
                  } else if (spriteName.Contains("delete")) {
                        return Type.Editor;
                  } else if (spriteName.StartsWith("edit")) {
                        return Type.Editor;
                  } else if (spriteName.Contains("editBtn")) {
                        return Type.Menu;
                  } else if (spriteName.Contains("edit")) {
                        return Type.Editor;
                  } else if (spriteName.Contains("link")) {
                        return Type.Editor;
                  } else if (spriteName.Contains("removeCheckBtn")) {
                        return Type.Miscellaneous;
                  } else if (spriteName.Contains("PBtn")) {
                        return Type.Miscellaneous;
                  }

                  // Start filtering editor buttons
                  if (spriteName.Contains("BPM") ||
                        spriteName.Contains("audio") ||
                        spriteName.Contains("ball") || 
                        spriteName.Contains("bird") ||
                        spriteName.Contains("change") ||
                        spriteName.Contains("color") ||
                        spriteName.Contains("copy") ||
                        spriteName.Contains("create") ||
                        spriteName.Contains("dart") ||
                        spriteName.Contains("deSel") ||
                        spriteName.Contains("duplicateObject") ||
                        spriteName.Contains("Layer") ||
                        spriteName.Contains("groupID") ||
                        spriteName.Contains("help") ||
                        spriteName.Contains("hsv") ||
                        spriteName.Contains("icon") ||
                        spriteName.Contains("info") ||
                        spriteName.Contains("jetpack") ||
                        spriteName.Contains("musicLibrary") ||
                        spriteName.Contains("ncs") ||
                        spriteName.Contains("normalBtn") ||
                        spriteName.Contains("orderUp") ||
                        spriteName.Contains("paintBtn") ||
                        spriteName.Contains("paste") ||
                        spriteName.Contains("EditorBtn") ||
                        spriteName.Contains("MusicBtn") ||
                        spriteName.Contains("redoBtn") ||
                        spriteName.Contains("robot") ||
                        spriteName.Contains("rotationControlBtn") ||
                        spriteName.Contains("savedSongsBtn") ||
                        spriteName.Contains("select") ||
                        spriteName.Contains("ship") ||
                        spriteName.Contains("spider") ||
                        spriteName.Contains("swing") ||
                        spriteName.Contains("tabOff") ||
                        spriteName.Contains("tabOn") ||
                        spriteName.Contains("trashBtn") ||
                        spriteName.Contains("undoBtn") ||
                        spriteName.Contains("zoom") ||
                        spriteName.Contains("warp") ||
                        spriteName.Contains("pause")) {

                        return Type.Editor;
                  }
                  return Type.Menu;
            }

            // Done
            Type getTypeFromGameSheet_4() {
                  if (spriteName.Contains("shine")) {
                        return Type.Block;
                  } else if (spriteName.Contains("boom") ||
                        spriteName.Contains("spiderDash")) {

                        return Type.Effect;
                  }
                  return Type.Menu;
            }

            public bool isIconType() {
                  if (type == Type.Icon) {
                        return true;

                  } else if (type != Type.Unknown) {
                        return false;

                  } else if (sourceFile.StartsWith("bird") ||
                        sourceFile.StartsWith("player_ball") ||
                        sourceFile.StartsWith("player") ||
                        sourceFile.StartsWith("dart") ||
                        sourceFile.StartsWith("jetpack") ||
                        sourceFile.StartsWith("robot") ||
                        sourceFile.StartsWith("ship") ||
                        sourceFile.StartsWith("spider") ||
                        sourceFile.StartsWith("swing")) {

                        return true;
                  }
                  return false;
            }
      }
}
