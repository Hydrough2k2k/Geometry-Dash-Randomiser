using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static Geometry_Dash_Randomiser.Config;
using static Geometry_Dash_Randomiser.Sprite;

namespace Geometry_Dash_Randomiser {

      internal class Randomiser {

            public Randomiser(GameFileManager creator, int seed) {
                  gameFiles = creator;

                  if (seed == 0) {
                        random = new Random(Guid.NewGuid().GetHashCode());
                  } else {
                        random = new Random(seed);
                  }
            }

            GameFileManager gameFiles;
            Random random;

            public List<Sprite> RandomiseData() {
                  List<Sprite> randomisedSprites = new List<Sprite>();
                  List<Sprite> selectedSprites = new List<Sprite>();

                  Config config = Instance;

                  // This starts at 1, because 0 is a special case, read below the for loop
                  for (int i = 1; i <= Config.maxGroups; i++) {

                        // Grab all of the sprites with the given group ID if the sgroup is enabled, put them all in the list
                        selectedSprites = new List<Sprite>();

                        // Get all relevant icon types
                        if (config.iconTextures.Cube.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Cube));
                        if (config.iconTextures.Ship.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Ship));
                        if (config.iconTextures.Ball.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Ball));
                        if (config.iconTextures.Ufo.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.UFO));
                        if (config.iconTextures.Wave.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Wave));
                        if (config.iconTextures.Robot.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Robot));
                        if (config.iconTextures.Spider.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Spider));
                        if (config.iconTextures.Swing.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Swing));
                        if (config.iconTextures.Jetpack.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.IconType.Jetpack));

                        // Get all relevant textures of different groups
                        if (config.menuTextures.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.ResourceType.Menu));
                        if (config.shopTextures.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.ResourceType.Shop));
                        if (config.editorTextures.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.ResourceType.Editor));
                        if (config.tileTextures.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.ResourceType.Block));
                        if (config.portalTextures.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.ResourceType.Portal));
                        if (config.orbTextures.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.ResourceType.Orb));
                        if (config.orbTextures.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.ResourceType.Pad));
                        if (config.particleTextures.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.ResourceType.Particle));
                        if (config.effectTextures.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.ResourceType.Effect));
                        if (config.miscTextures.IsEnabledAndGroupIs(i))
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.ResourceType.Miscellaneous));

                        // Shuffle them, then add them to the randomised list
                        // If the max sprite multiplier is not unlimited, shuffle them with that limitation in mind. It's a lot slower
                        if (config.maxSpriteMultiplier < 1000f) {
                              selectedSprites = ShuffleSpritesRestricted(selectedSprites);

                        } else {
                              selectedSprites = ShuffleSprites(selectedSprites);
                        }
                        randomisedSprites.AddRange(selectedSprites);
                  }

                  // Finally get all of the groups that are enabled and have a group of 0
                  // Multiple groups with group ID 0 will not be pooled then shuffled, instead they are all shuffled in isolation
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.iconTextures.Cube, Sprite.IconType.Cube));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.iconTextures.Ship, Sprite.IconType.Ship));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.iconTextures.Ball, Sprite.IconType.Ball));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.iconTextures.Ufo, Sprite.IconType.UFO));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.iconTextures.Wave, Sprite.IconType.Wave));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.iconTextures.Robot, Sprite.IconType.Robot));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.iconTextures.Spider, Sprite.IconType.Spider));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.iconTextures.Swing, Sprite.IconType.Swing));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.iconTextures.Jetpack, Sprite.IconType.Jetpack));

                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.menuTextures, Sprite.ResourceType.Menu));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.shopTextures, Sprite.ResourceType.Shop));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.editorTextures, Sprite.ResourceType.Editor));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.tileTextures, Sprite.ResourceType.Block));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.portalTextures, Sprite.ResourceType.Portal));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.orbTextures, Sprite.ResourceType.Orb));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.padTextures, Sprite.ResourceType.Pad));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.particleTextures, Sprite.ResourceType.Particle));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.effectTextures, Sprite.ResourceType.Effect));
                  randomisedSprites.AddRange(ShuffleOrReturnOriginalSpritesOfType(config.miscTextures, Sprite.ResourceType.Miscellaneous));

                  return randomisedSprites;
            }

            List<Sprite> ShuffleOrReturnOriginalSpritesOfType(RandomisationSetting setting, Sprite.IconType iconType) {
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

            List<Sprite> ShuffleOrReturnOriginalSpritesOfType(RandomisationSetting setting, Sprite.ResourceType type) {
                  if (setting.enabled == false) {
                        // If the setting is disabled return all sprites unaltered
                        return gameFiles.getAllSpritesOfType(type);
                  } else if (setting.isEnabledAndGroupIsZero() == true) {
                        // If the group is 0 and the setting is enabled shuffle the sprites and return them
                        return ShuffleSpritesOfType(type);
                  }
                  // If neither condition came the the textures have already been randomised, nothing will be returned
                  return new List<Sprite>();
            }

            List<Sprite> ShuffleSpritesOfType(Sprite.ResourceType type) {
                  List<Sprite> sprites = gameFiles.getAllSpritesOfType(type);
                  if (Config.Instance.maxSpriteMultiplier < 1000f) {
                        return ShuffleSpritesRestricted(sprites);

                  } else {
                        return ShuffleSprites(sprites);
                  }
            }

            List<Sprite> ShuffleSpritesOfType(Sprite.IconType type) {
                  List<Sprite> sprites = gameFiles.getAllSpritesOfType(type);
                  if (Config.Instance.maxSpriteMultiplier < 1000f) {
                        return ShuffleSpritesRestricted(sprites);
                  } else {
                        return ShuffleSprites(sprites);
                  }
            }

            List<Sprite> ShuffleSprites(List<Sprite> sprites) {
                  List<Sprite> shuffledSprites = new List<Sprite>();
                  bool[] shuffled = new bool[sprites.Count];

                  for (int i = 0; i < sprites.Count; i++) {

                        int randomInt;
                        // Keep randomising until you find a sprite that has not been randomised yet
                        do {
                              randomInt = random.Next(0, sprites.Count);
                        } while (shuffled[randomInt] == true);

                        // Set it to randomised if allowDuplicated is false
                        if (Config.Instance.allowDuplicates == false)
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

            List<Sprite> ShuffleSpritesRestricted(List<Sprite> sprites) {
                  List<Sprite> shuffledSprites = new List<Sprite>();

                  int[] newSpriteOrder = GetNewSpriteOrderByArea(sprites.Select(s => s.getArea()).ToArray());

                  for (int i = 0; i < sprites.Count; i++) {

                        // Get all the stats you do not want to modify from the original sprite
                        Sprite newSprite = new Sprite(sprites[i].sourceFile, sprites[i].spriteName, sprites[i].type);

                        int newSpriteIndex = newSpriteOrder[i];

                        // Get everything else from the rolled sprite
                        newSprite.spriteOffset = sprites[newSpriteIndex].spriteOffset;
                        newSprite.spriteSize = sprites[newSpriteIndex].spriteSize;
                        newSprite.spriteSourceSize = sprites[newSpriteIndex].spriteSourceSize;
                        newSprite.textureRect = sprites[newSpriteIndex].textureRect;
                        newSprite.cropRect = sprites[newSpriteIndex].cropRect;
                        newSprite.textureRotated = sprites[newSpriteIndex].textureRotated;
                        newSprite.texture = (Bitmap)sprites[newSpriteIndex].texture.Clone();

                        // Add it to the randomised list
                        shuffledSprites.Add(newSprite);
                  }
                  return shuffledSprites;
            }

            int[] GetNewSpriteOrderByArea(float[] spriteAreas) {
                  bool[] shuffled = new bool[spriteAreas.Length];
                  int[] newSpriteOrder = new int[spriteAreas.Length];

                  for (int i = 0; i < spriteAreas.Length; i++) {
                        float maxArea = spriteAreas[i] * Config.Instance.maxSpriteMultiplier;
                        float minArea = spriteAreas[i] * (1 / Config.Instance.maxSpriteMultiplier);

                        int[] candidateSprites = spriteAreas
                              .Select((area, index) => new { index, area } )
                              .Where(sprite => sprite.area >= minArea && sprite.area <= maxArea && shuffled[sprite.index] == false)
                              .Select(sprite => sprite.index)
                              .ToArray();

                        int newSpriteIndex;

                        if (candidateSprites.Length != 0) {
                              int randomInt = random.Next(candidateSprites.Length);
                              newSpriteIndex = candidateSprites[randomInt];
                        } else {
                              newSpriteIndex = i;
                        }
                        newSpriteOrder[i] = newSpriteIndex;

                        if (Config.Instance.allowDuplicates == false)
                              shuffled[newSpriteIndex] = true;
                  }
                  return newSpriteOrder;
            }
      }
}
