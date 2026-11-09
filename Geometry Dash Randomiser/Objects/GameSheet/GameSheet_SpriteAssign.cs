using System.Linq;
using static Geometry_Dash_Randomiser.GameSheet.SpriteNameComparer;

namespace Geometry_Dash_Randomiser {

      public partial class GameSheet {

            private void AssignAllSpriteTypes() {
                  if (FileType == GameFileType.None)
                        return;

                  switch (this.FileType) {
                        case GameFileType.Resource:
                              AssignAllSpriteTypes(this.FileName);
                              break;

                        case GameFileType.Icon:
                              AssignAllIconTypes();
                              break;

                        case GameFileType.Font: // This will have it's own logic when GameSheetManager and FontManager will be merged
                        default:
                              Logger.Error($"Cannot assign sprite typpes to invalid file type: {FileType}");
                              break;
                  }
            }

            private void AssignAllSpriteTypes(string fileName) {
                  string shortFileName = fileName.RemoveQualityExtension();

                  // This will be used when this file type contains only 1 type of sprite and we don't need any specific conparison logic
                  ResourceType bulkSetType = GetBulkResourceType(shortFileName);

                  if (bulkSetType != ResourceType.Unknown) {
                        Sprites.ToList().ForEach(sprite => sprite.Type = bulkSetType);
                        return;
                  }

                  switch (shortFileName) {
                        case "GJ_GameSheetGlow":
                              AssignSpriteTypes(GlowSheet_Comparers, ResourceType.Block);
                              return;

                        case "GJ_GameSheet":
                              AssignSpriteTypes(GS1_Comparers, ResourceType.Block);
                              return;

                        case "GJ_GameSheet02":
                              AssignSpriteTypes(GS2_Comparers, ResourceType.Block);
                              return;

                        case "GJ_GameSheet03":
                              AssignSpriteTypes(GS3_Comparers, ResourceType.Menu);
                              return;

                        case "GJ_GameSheet04":
                              AssignSpriteTypes(GS4_Comparers, ResourceType.Menu);
                              return;

                        default:
                              Logger.Write(Logger.Mode.Warn, $"Unhandled resource file \"{shortFileName}\". The file's contents won't be randomized.");
                              break;
                  }
            }

            private ResourceType GetBulkResourceType(string fileName) {
                  if (fileName.StartsWith("GJ_ShopSheet")) {
                        return ResourceType.Shop;

                  } else if (fileName.StartsWith("PlayerExplosion")) {
                        return ResourceType.Miscellaneous;
                  }

                  switch (fileName) {
                        case "CCControlColourPickerSpriteSheet":
                              return ResourceType.Editor;

                        case "DungeonSheet":
                              return ResourceType.Menu;

                        case "FireSheet_01":
                              return ResourceType.Block;

                        case "GauntletSheet":
                        case "GJ_FontSheet":
                              return ResourceType.Menu;

                        case "GJ_GameSheetEditor":
                              return ResourceType.Editor;

                        case "GJ_LaunchSheet":
                              return ResourceType.Menu;

                        case "GJ_ParticleSheet":
                              return ResourceType.Particle;

                        case "GJ_PathSheet":
                              return ResourceType.Menu;

                        case "PixelSheet_01":
                              return ResourceType.Block;

                        case "SecretSheet":
                        case "TowerSheet":
                        case "TreasureRoomSheet":
                        case "WorldSheet":
                              return ResourceType.Miscellaneous;

                        default:
                              // If we hit this case, then the file might be unrecognised, or might be in the other switch case
                              return ResourceType.Unknown;
                  }
            }

            private void AssignAllIconTypes() {
                  AssignSpriteTypes(IconNameComparers, ResourceType.Unknown);
            }

            private void AssignSpriteTypes(SpriteNameComparer[] comparers, ResourceType defaultResourceType)
                  => AssignSpriteTypes(Sprites, comparers, defaultResourceType);

            private static void AssignSpriteTypes(ResourceSprite[] sprite, SpriteNameComparer[] comparers, ResourceType defaultResourceType) {
                  for (int s = 0; s < sprite.Length; s++) {
                        bool found = false;

                        // Go through each comparer to try to get the type of the sprite
                        for (int c = 0; c < comparers.Length; c++) {
                              if (comparers[c].DoesMatch(sprite[s].Name)) {
                                    sprite[s].Type = comparers[c].Result;
                                    found = true;
                                    break;
                              }
                        }

                        // If the type was not found from the comparers, assign the default resource type to the sprite
                        if (found == false) {
                              sprite[s].Type = defaultResourceType;
                        }
                  }
            }

            /// <summary>
            /// Comparers for GlowSheet. The order of the comparers matters, as the first match will be used to assign the type.
            /// </summary>
            private static SpriteNameComparer[] GlowSheet_Comparers => new SpriteNameComparer[] {
                  new SpriteNameComparer(ComparisonMode.Contains, "boost", ResourceType.Portal),
                  new SpriteNameComparer(ComparisonMode.Contains, new string[] { "bump", "Bump" }, ResourceType.Pad),
                  new SpriteNameComparer(ComparisonMode.Contains, new string[] { "ring", "Ring" }, ResourceType.Orb)
            };

            /// <summary>
            /// Comparers for GameSheet1. The order of the comparers matters, as the first match will be used to assign the type.
            /// </summary>
            private static SpriteNameComparer[] GS1_Comparers => new SpriteNameComparer[] {
                  new SpriteNameComparer(ComparisonMode.Contains, new string[] {
                        "teleportRing", "dashRing", "spiderRing", "gravJumpRing", "gravring", "ring_0", "dropRing"}, ResourceType.Orb),
                  new SpriteNameComparer(ComparisonMode.Contains, new string[] { "bump", "spiderBump" }, ResourceType.Pad),
                  new SpriteNameComparer(ComparisonMode.Contains, "ParticleBtn", ResourceType.Editor)
            };

            /// <summary>
            /// Comparers for GameSheet2. The order of the comparers matters, as the first match will be used to assign the type.
            /// </summary>
            private static SpriteNameComparer[] GS2_Comparers => new SpriteNameComparer[] {
                  new SpriteNameComparer(ComparisonMode.Contains, "boost", ResourceType.Portal),
                  new SpriteNameComparer(ComparisonMode.Contains, "portal", ResourceType.Portal),
                  new SpriteNameComparer(ComparisonMode.Contains, "edit", ResourceType.Editor),
                  new SpriteNameComparer(ComparisonMode.Contains, "keyframeIcon", ResourceType.Editor),
                  new SpriteNameComparer(ComparisonMode.Contains, new string[] {
                        "floorLine", "checkpoint", "secretCoin", "time" }, ResourceType.Miscellaneous)
            };

            /// <summary>
            /// Comparers for GameSheet3. The order of the comparers matters, as the first match will be used to assign the type.
            /// </summary>
            private static SpriteNameComparer[] GS3_Comparers => new SpriteNameComparer[] {
                  new SpriteNameComparer(ComparisonMode.Contains, "arrow", ResourceType.Editor),
                  new SpriteNameComparer(ComparisonMode.Contains, "checkpointBtn", ResourceType.Menu),
                  new SpriteNameComparer(ComparisonMode.StartsWith, "GJ_delete", ResourceType.Menu),
                  new SpriteNameComparer(ComparisonMode.Contains, "duplicate", ResourceType.Menu),
                  new SpriteNameComparer(ComparisonMode.Contains, "everyplayBtn", ResourceType.Menu),
                  new SpriteNameComparer(ComparisonMode.Contains, "check", ResourceType.Editor),
                  new SpriteNameComparer(ComparisonMode.StartsWith, "diff", ResourceType.Menu),
                  new SpriteNameComparer(ComparisonMode.Contains, "delete", ResourceType.Editor),
                  new SpriteNameComparer(ComparisonMode.Contains, "editBtn", ResourceType.Menu),
                  new SpriteNameComparer(ComparisonMode.Contains, "edit", ResourceType.Editor),
                  new SpriteNameComparer(ComparisonMode.Contains, "link", ResourceType.Editor),
                  new SpriteNameComparer(ComparisonMode.Contains, "removeCheckBtn", ResourceType.Miscellaneous),
                  new SpriteNameComparer(ComparisonMode.Contains, "PBtn", ResourceType.Miscellaneous),
                  new SpriteNameComparer(ComparisonMode.Contains, new string[] {
                        "BPM", "audio", "ball", "bird", "change", "color", "copy", "create", "dart",
                        "deSel", "duplicateObject", "Layer", "groupID", "help", "hsv", "icon", "info",
                        "jetpack", "musicLibrary", "ncs", "normalBtn", "orderUp", "paintBtn", "paste",
                        "EditorBtn", "MusicBtn", "redoBtn", "robot", "rotationControlBtn", "savedSongsBtn",
                        "select", "ship", "spider", "swing", "tabOff", "tabOn", "trashBtn", "undoBtn",
                        "zoom", "warp", "pause" }, ResourceType.Editor)
            };

            /// <summary>
            /// Comparers for GameSheet4. The order of the comparers matters, as the first match will be used to assign the type.
            /// </summary>
            private static SpriteNameComparer[] GS4_Comparers => new SpriteNameComparer[] {
                  new SpriteNameComparer(ComparisonMode.Contains, "shine", ResourceType.Block),
                  new SpriteNameComparer(ComparisonMode.Contains, "boom", ResourceType.Effect),
                  new SpriteNameComparer(ComparisonMode.Contains, "spiderDash", ResourceType.Effect)
            };

            /// <summary>
            /// Comparers for icon sprites. The order of the comparers matters, as the first match will be used to assign the type.
            /// </summary>
            private static SpriteNameComparer[] IconNameComparers => new SpriteNameComparer[] {
                  new SpriteNameComparer(ComparisonMode.StartsWith, "bird" , ResourceType.UFO),
                  new SpriteNameComparer(ComparisonMode.StartsWith, "player_ball", ResourceType.Ball),
                  new SpriteNameComparer(ComparisonMode.StartsWith, "player", ResourceType.Cube),
                  new SpriteNameComparer(ComparisonMode.StartsWith, "dart", ResourceType.Wave),
                  new SpriteNameComparer(ComparisonMode.StartsWith, "jetpack", ResourceType.Jetpack),
                  new SpriteNameComparer(ComparisonMode.StartsWith, "robot", ResourceType.Robot),
                  new SpriteNameComparer(ComparisonMode.StartsWith, "ship", ResourceType.Ship),
                  new SpriteNameComparer(ComparisonMode.StartsWith, "spider", ResourceType.Spider),
                  new SpriteNameComparer(ComparisonMode.StartsWith, "swing", ResourceType.Swing)
            };

            public struct SpriteNameComparer {

                  public enum ComparisonMode {
                        StartsWith,
                        Contains,
                        EndsWith
                  }

                  public SpriteNameComparer(ComparisonMode mode, string[] value, ResourceType result) {
                        _mode = mode;
                        _values = value;
                        _result = result;
                  }

                  public SpriteNameComparer(ComparisonMode mode, string value, ResourceType result) {
                        _mode = mode;
                        _values = new string[] { value };
                        _result = result;
                  }

                  public ComparisonMode Mode => _mode;
                  public string[] Values => _values;
                  public ResourceType Result => _result;

                  public bool DoesMatch(string spriteName) {
                        switch (_mode) {
                              case ComparisonMode.StartsWith:
                                    return spriteName.StartsWithAny(_values);

                              case ComparisonMode.Contains:
                                    return spriteName.ContainsAny(_values);

                              case ComparisonMode.EndsWith:
                                    return spriteName.EndsWithAny(_values);

                              default:
                                    Logger.Error($"Unrecognised Comparison Mode {_mode.ToString()} passed.");
                                    return false;
                        }
                  }

                  private readonly ComparisonMode _mode;
                  private readonly string[] _values;
                  public readonly ResourceType _result;
            }
      }
}
