using System;
using System.Collections.Generic;
using System.Drawing;

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

            public readonly int maxGroups = 10;

            public List<Sprite> RandomiseData() {
                  List<Sprite> randomisedSprites = new List<Sprite>();
                  List<Sprite> selectedSprites = new List<Sprite>();

                  // This starts at 1, because 0 is a special case, read below the for loop
                  for (int i = 1; i <= maxGroups; i++) {
                        selectedSprites = new List<Sprite>();

                        // Grab all of the sprites with the given group ID, put them all in the list
                        if (Config.iconTextures.group == i && Config.iconTextures.enabled)
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Icon));

                        if (Config.menuTextures.group == i && Config.menuTextures.enabled)
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Menu));

                        if (Config.shopTextures.group == i && Config.shopTextures.enabled)
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Shop));

                        if (Config.editorTextures.group == i && Config.editorTextures.enabled)
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Editor));

                        if (Config.tileTextures.group == i && Config.tileTextures.enabled)
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Block));

                        if (Config.portalTextures.group == i && Config.portalTextures.enabled)
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Portal));

                        if (Config.orbTextures.group == i && Config.orbTextures.enabled)
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.OrbsAndPads));

                        if (Config.particleTextures.group == i && Config.particleTextures.enabled)
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Particle));

                        if (Config.effectTextures.group == i && Config.effectTextures.enabled)
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Effect));

                        if (Config.miscTextures.group == i && Config.miscTextures.enabled)
                              selectedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Miscellaneous));

                        // Shuffle them, then add them to the randomised list
                        selectedSprites = ShuffleSprites(selectedSprites);
                        randomisedSprites.AddRange(selectedSprites);
                  }

                  // Finally get all of the groups that are enabled and have a group of 0
                  // Multiple groups with group ID 0 will not be pooled then shuffled, instead they are all shuffled in isolation
                  if (Config.iconTextures.enabled) {
                        if (Config.iconTextures.group == 0)
                              randomisedSprites.AddRange(ShuffleSpritesOfType(Sprite.Type.Icon));
                  } else {
                        randomisedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Icon));
                  }

                  if (Config.menuTextures.enabled) {
                        if (Config.menuTextures.group == 0)
                              randomisedSprites.AddRange(ShuffleSpritesOfType(Sprite.Type.Menu));
                  } else {
                        randomisedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Menu));
                  }

                  if (Config.shopTextures.enabled) {
                        if (Config.shopTextures.group == 0)
                              randomisedSprites.AddRange(ShuffleSpritesOfType(Sprite.Type.Shop));
                  } else {
                        randomisedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Shop));
                  }

                  if (Config.editorTextures.enabled) {
                        if (Config.editorTextures.group == 0)
                              randomisedSprites.AddRange(ShuffleSpritesOfType(Sprite.Type.Editor));
                  } else {
                        randomisedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Editor));
                  }

                  if (Config.tileTextures.enabled) {
                        if (Config.tileTextures.group == 0)
                              randomisedSprites.AddRange(ShuffleSpritesOfType(Sprite.Type.Block));
                  } else {
                        randomisedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Block));
                  }

                  if (Config.portalTextures.enabled) {
                        if (Config.portalTextures.group == 0)
                              randomisedSprites.AddRange(ShuffleSpritesOfType(Sprite.Type.Portal));
                  } else {
                        randomisedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Portal));
                  }

                  if (Config.orbTextures.enabled) {
                        if (Config.orbTextures.group == 0)
                              randomisedSprites.AddRange(ShuffleSpritesOfType(Sprite.Type.OrbsAndPads));
                  } else {
                        randomisedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.OrbsAndPads));
                  }

                  if (Config.particleTextures.enabled) {
                        if (Config.particleTextures.group == 0)
                              randomisedSprites.AddRange(ShuffleSpritesOfType(Sprite.Type.Particle));
                  } else {
                        randomisedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Particle));
                  }

                  if (Config.effectTextures.enabled) {
                        if (Config.effectTextures.group == 0)
                              randomisedSprites.AddRange(ShuffleSpritesOfType(Sprite.Type.Effect));
                  } else {
                        randomisedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Effect));
                  }

                  if (Config.miscTextures.enabled) {
                        if (Config.miscTextures.group == 0)
                              randomisedSprites.AddRange(ShuffleSpritesOfType(Sprite.Type.Miscellaneous));
                  } else {
                        randomisedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Miscellaneous));
                  }

                  return randomisedSprites;

                  // Sample Switch/case code that might replace the hell above
                  //switch (Config.iconTextures.enabled) {
                  //      case true:
                  //            if (Config.iconTextures.group == 0)
                  //                  randomisedSprites.AddRange(ShuffleSpritesOfType(Sprite.Type.Icon));
                  //            break;
                  //      case false:
                  //            randomisedSprites.AddRange(gameFiles.getAllSpritesOfType(Sprite.Type.Icon));
                  //            break;
                  //}
            }

            List<Sprite> ShuffleSpritesOfType(Sprite.Type type) {
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
                        newSprite.textureRotated = sprites[randomInt].textureRotated;
                        newSprite.texture = (Bitmap)sprites[randomInt].texture.Clone();

                        // Add it to the randomised list
                        shuffledSprites.Add(newSprite);
                  }
                  return shuffledSprites;
            }
      }
}
