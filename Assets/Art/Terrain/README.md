# Meadow terrain surface

Static ground appearance for the normal grass stage in `Ingame Scene`.
The scene continues to use Unity's built-in URP `TerrainLit` material.

- `Textures/MeadowGrass_Albedo.png`: short olive-green grass.
- `Textures/StonySoil_Albedo.png`: earth with small embedded stones.
- `Layers/MeadowGrass.terrainlayer`: primary grass, tiled every 3.5 metres.
- `Layers/MeadowGrassVariation.terrainlayer`: a darker, offset version of the same grass texture, tiled every 5.7 metres.
- `Layers/StonySoil.terrainlayer`: exposed soil, tiled every 4.5 metres.

The three layers are painted into `Assets/New Terrain.asset`: grass covers the
flat areas, soil is stronger on steep slopes, and broad irregular patches break
up the surface. Heightmap, collider, sky and gameplay scripts are unchanged.
The grass layer color multipliers compensate for the scene's dim dusk lighting.
The textures use repeat wrapping, mipmaps, trilinear filtering, 8x anisotropic
filtering and a 1024-pixel import limit. No runtime surface update is required.

## Texture creation

Created with the built-in `image_gen` tool on 2026-09-30. The generated PNGs were
copied into this folder and were not sourced from a third-party asset pack.
Unity generated and manages all `.meta` files.

### MeadowGrass_Albedo prompt

Use case: stylized-concept. Asset type: tileable Unity Terrain ground albedo texture, one square 1024x1024 image. Primary request: a seamless short meadow grass ground surface viewed perfectly straight down, for the normal uncorrupted stage of a grounded stylized zombie survival game. Small narrow matte grass blades in irregular natural clumps cover nearly all the ground; subtle brown loam in tiny gaps. Restrained natural olive, sage and moss greens, medium light albedo so dim in-game lighting remains readable. Fine-scale organic details, cohesive semi-realistic hand-painted game material, no chunky low-poly polygons. Uniform flat neutral illumination ONLY: no baked directional lighting, shadows, AO, highlights or vignetting. Even density and color distribution across the whole image, seamless wrapping left/right and top/bottom, no standout central shape, no obvious repeating rows. Cover full frame with ground. No perspective, horizon, tall grass, flowers, stones, text, frame, labels, watermark. Opaque background.

### StonySoil_Albedo prompt

Use case: stylized-concept. Asset type: tileable Unity Terrain ground albedo texture, one square 1024x1024 image. Primary request: a seamless earthy ground texture of compact light brown soil with scattered small weathered gray-brown mineral chips and irregular exposed sandstone grain, viewed perfectly straight down. For mixing with short olive grass on a grounded stylized zombie survival game terrain and covering steep banks. Fine granular earth dominates with embedded small angular pebbles and subtle natural mottling. Warm taupe, restrained ochre brown and neutral gray; medium light albedo. Cohesive semi-realistic hand-painted game material. Uniform flat neutral illumination ONLY: no baked directional light, shadows, AO, highlights or vignette. Even texture density throughout; seamlessly wrapping left/right and top/bottom; no central focal feature, no conspicuous big rock or repeated pattern. No vegetation, pavement, red clay, deep cracks, perspective, horizon, text, frame, labels, watermark. Opaque background.
