# Aetherhue

## What is Aetherhue?
It's a combination of simple scripts for manipulating textures that I've found useful during XIV modding, bundled into one tool.
It may or may not be useful to you but it's provided here as-is in case others do find some of the features useful.

## How do I use it?
Download it, and either run the executable directly and follow the prompts, or drag an image onto the executable and follow the prompts.

## What can it do?
The following scripts are currently available:

#### ID Map to XIV ID Map
Converts an ID map from another tool (such as Substance/Blender etc) into an XIV compatible ID map.
It creates a combined ID texture, separate Channel 1 and 2 Textures, and creates a colorset that can be pasted into Penumbra.

#### Overlay Images
Overlays images onto the base image, applying alpha blending and generating a new image.

#### Separate Diffuse and Alpha
Takes a diffuse image with alpha and separates it into diffuse image with no alpha and an opacity map.

#### Extend UV Islands
Extends UV islands out by the specified number of pixels where the background borders a different color. Useful if you have an ID map and you want to bleed the edges a little further to prevent seams.
