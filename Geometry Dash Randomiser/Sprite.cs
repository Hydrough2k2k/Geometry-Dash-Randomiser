using System.Drawing;

namespace Geometry_Dash_Randomiser {

      public class Sprite {

            public enum ResourceType { Unknown, Icon, Block, Portal, Orb, Pad, Editor, Menu, Particle, Effect, Miscellaneous, Shop }
            public enum IconType { Invalid, Cube, Ship, Ball, UFO, Wave, Robot, Spider, Swing, Jetpack }

            // What file did this come from, for example: "bird_01-uhd"
            // Might be retired in place of a container containing an array of sprites if it makes sense. SpriteSheet is a good name, maybe
            public string sourceFile { get; set; } = string.Empty;

            // Name of the sprite, for example: "bird_01_001.png"
            public string spriteName { get; set; } = string.Empty;

            public Point spriteOffset { get; set; } = new Point();
            public Size spriteSize { get; set; } = new Size();
            public Size spriteSourceSize { get; set; } = new Size();
            public Rectangle textureRect { get; set; } = new Rectangle();
            public Rectangle cropRect { get; set; } = new Rectangle();
            public bool textureRotated { get; set; } = false;
            public ResourceType type { get; set; } = ResourceType.Unknown;

            // This should only be used and accessed if type == Sprite.Type.Icon, and check if it is not invalid
            public IconType iconType { get; set; } = IconType.Invalid;

            // The cropped bitmap for the sprite
            public Bitmap texture = null;

            public Sprite() { }

            public Sprite(string sourceFile, string spriteName, Point spriteOffset,
                  Size spriteSize, Size spriteSourceSize, Rectangle textureRect,
                  bool textureRotated, ResourceType type, Bitmap texture) {

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

            public Sprite(string sourceFile, string spriteName, ResourceType type) {
                  this.sourceFile = sourceFile;
                  this.spriteName = spriteName;
                  this.type = type;
            }

            public float getArea() {
                  return spriteSize.Width * spriteSize.Height;
            }

            public float getBitmapArea() {
                  return texture.Width * texture.Height;
            }

            /// <summary>
            /// This will deduce the sprite type based on the spriteName and SourceFile strings
            /// </summary>
            public void AssignType() {
                  if (tryGetIconType() == true) {
                        type = ResourceType.Icon;
                  } else if (sourceFile.StartsWith("FireSheet_01")) {
                        type = ResourceType.Block;
                  } else if (sourceFile.StartsWith("GauntletSheet")) {
                        type = ResourceType.Menu;
                  } else if (sourceFile.StartsWith("GJ_GameSheetEditor")) {
                        type = ResourceType.Editor;
                  } else if (sourceFile.StartsWith("GJ_GameSheetGlow")) {
                        type = getTypeFromGlowSheets();
                  } else if (sourceFile.StartsWith("GJ_GameSheet")) {
                        type = getTypeFromGameSheets();
                  } else if (sourceFile.StartsWith("GJ_LaunchSheet")) {
                        type = ResourceType.Menu;
                  } else if (sourceFile.StartsWith("GJ_ParticleSheet")) {
                        type = ResourceType.Particle;
                  } else if (sourceFile.StartsWith("GJ_PathSheet")) {
                        type = ResourceType.Menu;
                  } else if (sourceFile.StartsWith("GJ_ShopSheet")) {
                        type = ResourceType.Shop;
                  } else if (sourceFile.StartsWith("PixelSheet")) {
                        type = ResourceType.Block;
                  } else if (sourceFile.StartsWith("SecretSheet")) {
                        type = ResourceType.Miscellaneous;
                  } else if (sourceFile.StartsWith("TowerSheet")) {
                        type = ResourceType.Miscellaneous;
                  } else if (sourceFile.StartsWith("TreasureRoomSheet")) {
                        type = ResourceType.Miscellaneous;
                  } else if (sourceFile.StartsWith("WorldSheet")) {
                        type = ResourceType.Miscellaneous;
                  }
            }

            ResourceType getTypeFromGlowSheets() {

                  if (spriteName.Contains("boost")) {
                        return ResourceType.Portal;
                  } else if (spriteName.Contains("bump") || spriteName.Contains("Bump")) {
                        return ResourceType.Pad;
                  } else if (spriteName.Contains("ring") || spriteName.Contains("Ring")) {
                        return ResourceType.Orb;
                  }
                  return ResourceType.Block;
            }

            ResourceType getTypeFromGameSheets() {
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

                  return ResourceType.Unknown;
            }

            ResourceType getTypeFromGameSheet_1() {
                  if (spriteName.Contains("teleportRing") || // TP Orb
                        spriteName.Contains("dashRing") || // Dash Orb
                        spriteName.Contains("spiderRing") || // Spider Orb
                        spriteName.Contains("gravJumpRing") || // Green Orb
                        spriteName.Contains("gravring") || // Blue Orb
                        spriteName.Contains("ring_0") || // Yellow, Pink, Red Orbs
                        spriteName.Contains("dropRing")) { // Black Orb

                        return ResourceType.Orb;
                  } else if (spriteName.Contains("bump") || // Yellow, Pink, Blue, Red Pads
                        spriteName.Contains("spiderBump")) { // TP Pad

                        return ResourceType.Pad;
                  } else if (spriteName.Contains("ParticleBtn")) {
                        return ResourceType.Editor;
                  }
                  return ResourceType.Block;
            }

            ResourceType getTypeFromGameSheet_2() {
                  if (spriteName.Contains("boost")) {
                        return ResourceType.Portal;
                  } else if (spriteName.Contains("portal")) {
                        return ResourceType.Portal;
                  } else if (spriteName.Contains("edit")) {
                        return ResourceType.Editor;
                  } else if (spriteName.Contains("keyframeIcon")) {
                        return ResourceType.Editor;
                  } else if (spriteName.Contains("floorLine") ||
                        spriteName.Contains("checkpoint") ||
                        spriteName.Contains("secretCoin") ||
                        spriteName.Contains("time")) {

                        return ResourceType.Miscellaneous;
                  }
                  return ResourceType.Block;
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

            ResourceType getTypeFromGameSheet_3() {
                  // The order matters, don't just group it by return type!
                  if (spriteName.Contains("arrow")) {
                        return ResourceType.Editor;
                  } else if (spriteName.Contains("checkpointBtn") ||
                        spriteName.StartsWith("GJ_delete") ||
                        spriteName.Contains("duplicate") ||
                        spriteName.Contains("everyplayBtn")) { // Important to keep these to make sure the items are sepatrated

                        return ResourceType.Menu;

                  } else if (spriteName.Contains("check")) {
                        return ResourceType.Editor;
                  } else if (spriteName.StartsWith("diff")) { // Difficulty icons "diffIcon" and "difficulty" will be caught by this
                        return ResourceType.Menu;
                  } else if (spriteName.Contains("delete")) {
                        return ResourceType.Editor;
                  } else if (spriteName.Contains("edit")) {
                        if (spriteName.Contains("editBtn")) {
                              return ResourceType.Menu;
                        } else {
                              return ResourceType.Editor;
                        }
                  }else if (spriteName.Contains("link")) {
                        return ResourceType.Editor;
                  } else if (spriteName.Contains("removeCheckBtn")) {
                        return ResourceType.Miscellaneous;
                  } else if (spriteName.Contains("PBtn")) {
                        return ResourceType.Miscellaneous;
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

                        return ResourceType.Editor;
                  }
                  return ResourceType.Menu;
            }

            // Done
            ResourceType getTypeFromGameSheet_4() {
                  if (spriteName.Contains("shine")) {
                        return ResourceType.Block;
                  } else if (spriteName.Contains("boom") ||
                        spriteName.Contains("spiderDash")) {

                        return ResourceType.Effect;
                  }
                  return ResourceType.Menu;
            }

            public bool tryGetIconType() {
                  if (sourceFile.StartsWith("bird")) {
                        this.iconType = IconType.UFO;

                  } else if (sourceFile.StartsWith("player_ball")) {
                        this.iconType = IconType.Ball;

                  } else if (sourceFile.StartsWith("player")) {
                        this.iconType = IconType.Cube;

                  } else if (sourceFile.StartsWith("dart")) {
                        this.iconType = IconType.Wave;

                  } else if (sourceFile.StartsWith("jetpack")) {
                        this.iconType = IconType.Jetpack;

                  } else if (sourceFile.StartsWith("robot")) {
                        this.iconType = IconType.Robot;

                  } else if (sourceFile.StartsWith("ship")) {
                        this.iconType = IconType.Ship;

                  } else if (sourceFile.StartsWith("spider")) {
                        this.iconType = IconType.Spider;

                  } else if (sourceFile.StartsWith("swing")) {
                        this.iconType = IconType.Swing;
                  }

                  // Return whether getting the icon type was successful 
                  if (this.iconType != IconType.Invalid)
                        return true;
                  return false;
            }
      }
}
