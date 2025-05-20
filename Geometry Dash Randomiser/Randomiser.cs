using System;
using System.Collections.Generic;
using System.Drawing;
using static Geometry_Dash_Randomiser.Config;
using static Geometry_Dash_Randomiser.Sprite.Type;
using static Geometry_Dash_Randomiser.Sprite.IconType;

namespace Geometry_Dash_Randomiser {

      internal class Randomiser {

            public Randomiser(GameFiles creator, int seed) {
                  gameFiles = creator;

                  if (seed == 0) {
                        random = new Random(Guid.NewGuid().GetHashCode());
                  } else {
                        random = new Random(seed);
                  }
            }

            GameFiles gameFiles;
            Random random;

            public static readonly int maxGroups = 10;

            public List<Sprite> RandomiseData() {
                  List<Sprite> randomisedSprites = new List<Sprite>();
                  List<Sprite> selectedSprites = new List<Sprite>();

                  // This starts at 1, because 0 is a special case, read below the for loop
                  for (int i = 1; i <= maxGroups; i++) {

                        // Grab all of the sprites with the given group ID if the sgroup is enabled, put them all in the list
                        selectedSprites = new List<Sprite>();

                        // Get all relevant icon types
                        if (Config.iconTextures.cube.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Cube));
                        if (Config.iconTextures.ship.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Ship));
                        if (Config.iconTextures.ball.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Ball));
                        if (Config.iconTextures.ufo.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.UFO));
                        if (Config.iconTextures.wave.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Wave));
                        if (Config.iconTextures.robot.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Robot));
                        if (Config.iconTextures.spider.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Spider));
                        if (Config.iconTextures.swing.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Swing));
                        if (Config.iconTextures.jetpack.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Jetpack));

                        // Get all relevant textures of different groups
                        if (Config.menuTextures.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Menu));
                        if (Config.shopTextures.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Shop));
                        if (Config.editorTextures.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Editor));
                        if (Config.tileTextures.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Block));
                        if (Config.portalTextures.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Portal));
                        if (Config.orbTextures.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Orb));
                        if (Config.orbTextures.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Pad));
                        if (Config.particleTextures.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Particle));
                        if (Config.effectTextures.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Effect));
                        if (Config.miscTextures.IsEnabledAndGroupMatches(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Miscellaneous));

                        // Shuffle them, then add them to the randomised list
                        selectedSprites = ShuffleSprites(selectedSprites);
                        randomisedSprites.AddRange(selectedSprites);
                  }

                  // Finally get all of the groups that are enabled and have a group of 0
                  // Multiple groups with group ID 0 will not be pooled then shuffled, instead they are all shuffled in isolation
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.iconTextures.cube, Sprite.IconType.Cube));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.iconTextures.ship, Sprite.IconType.Ship));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.iconTextures.ball, Sprite.IconType.Ball));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.iconTextures.ufo, Sprite.IconType.UFO));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.iconTextures.wave, Sprite.IconType.Wave));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.iconTextures.robot, Sprite.IconType.Robot));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.iconTextures.spider, Sprite.IconType.Spider));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.iconTextures.swing, Sprite.IconType.Swing));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.iconTextures.jetpack, Sprite.IconType.Jetpack));

                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.menuTextures, Sprite.Type.Menu));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.shopTextures, Sprite.Type.Shop));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.editorTextures, Sprite.Type.Editor));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.tileTextures, Sprite.Type.Block));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.portalTextures, Sprite.Type.Portal));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.orbTextures, Sprite.Type.Orb));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.padTextures, Sprite.Type.Pad));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.particleTextures, Sprite.Type.Particle));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.effectTextures, Sprite.Type.Effect));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(Config.miscTextures, Sprite.Type.Miscellaneous));

                  return randomisedSprites;
            }

            List<Sprite> ShuffleOrReturnOriginalSpritesOfType(RandSetting setting, Sprite.IconType iconType) {
                  if (setting.enabled == false) {
                        // If the setting is disabled return all sprites unaltered
                        return gameFiles.getAllSpritesOfType(iconType);
                  } else if (setting.isEnabledAndGroupIsZero() == true) {
                        // If the group is 0 and the setting is enabled shuffle the sprites and return them
                        return ShuffleSpritesOfType(iconType);
                  }
                  // If neither condition came tre the textures have already been randomised, nothing will be returned
                  return new List<Sprite>();
            }

            List<Sprite> ShuffleOrReturnOriginalSpritesOfType(RandSetting setting, Sprite.Type type) {
                  if (setting.enabled == false) {
                        // If the setting is disabled return all sprites unaltered
                        return gameFiles.getAllSpritesOfType(type);
                  } else if (setting.isEnabledAndGroupIsZero() == true) {
                        // If the group is 0 and the setting is enabled shuffle the sprites and return them
                        return ShuffleSpritesOfType(type);
                  }
                  // If neither condition came tre the textures have already been randomised, nothing will be returned
                  return new List<Sprite>();
            }

            List<Sprite> ShuffleSpritesOfType(Sprite.Type type) {
                  List<Sprite> sprites = gameFiles.getAllSpritesOfType(type);
                  return ShuffleSprites(sprites);
            }

            List<Sprite> ShuffleSpritesOfType(Sprite.IconType type) {
                  List<Sprite> sprites = gameFiles.getAllSpritesOfType(type);
                  return ShuffleSprites(sprites);
            }

            List<Sprite> ShuffleSprites(List<Sprite> sprites) {
                  List<Sprite> shuffledSprites = new List<Sprite>();
                  bool[] shuffled = new bool[sprites.Count];

                  for (int i = 0; i < sprites.Count; i++) {

                        // Keep randomising until you find a sprite that has not been randomised yet
                        int randomInt = random.Next(0, sprites.Count);
                        while (shuffled[randomInt] == true) {
                              randomInt = random.Next(0, sprites.Count);
                        }
                        // Set it to randomised. This will be conditional later when "Allow Duplicates will be added"
                        shuffled[randomInt] = true;

                        // Get all the stats you do not want to modify from the original sprite
                        Sprite newSprite = new Sprite(sprites[i].sourceFile, sprites[i].spriteName, sprites[i].type);

                        // Get everything else from the rolled sprite
                        newSprite.spriteOffset = sprites[randomInt].spriteOffset;
                        newSprite.spriteSize = sprites[randomInt].spriteSize;
                        newSprite.spriteSourceSize = sprites[randomInt].spriteSourceSize;
                        newSprite.textureRect = sprites[randomInt].textureRect;
                        newSprite.cropRect = sprites[randomInt].cropRect;
                        newSprite.textureRotated = sprites[randomInt].textureRotated;
                        newSprite.texture = (Bitmap)sprites[randomInt].texture.Clone();

                        // Add it to the randomised list
                        shuffledSprites.Add(newSprite);
                  }
                  return shuffledSprites;
            }
      }
}
