Table of Contents
- [MonoGameSandbox](#monogamesandbox)
- [Projects](#projects)
  - [Platformer001](#platformer001)
  - [Platformer002 - Gravity and AABB Collision Detection](#platformer002---gravity-and-aabb-collision-detection)
  - [Platformer003](#platformer003)
  - [Platformer004](#platformer004)
  - [Platformer005](#platformer005)
  - [Platformer006](#platformer006)
  - [Platformer007 - Tiled integration](#platformer007---tiled-integration)
- [MonoGame](#monogame)
  - [Game Time](#game-time)
    - [Fixed Time](#fixed-time)
    - [Variable Time](#variable-time)
    - [Velocity Scaling Based on ElapsedGameTime](#velocity-scaling-based-on-elapsedgametime)
      - [Example](#example)
      - [Cumulative Movement Over One Second](#cumulative-movement-over-one-second)
        - [120 FPS:](#120-fps)
        - [30 FPS:](#30-fps)
      - [Real-World Analogy](#real-world-analogy)
  - [Sprite Maps](#sprite-maps)
  - [Collision Detection](#collision-detection)
    - [BoundingBox Collision Detection](#boundingbox-collision-detection)
    - [Bounding Circle Collision Detection](#bounding-circle-collision-detection)
    - [Pixel Collision Detection](#pixel-collision-detection)
    - [Collision Detection when Scaling and Rotating](#collision-detection-when-scaling-and-rotating)
    - [Using Matrices to simplify collision detection](#using-matrices-to-simplify-collision-detection)
    - [Swept AABB Collision Detection](#swept-aabb-collision-detection)
- [3rd Party Tools](#3rd-party-tools)
  - [Aseprite](#aseprite)
    - [Layers](#layers)
    - [User Data](#user-data)
    - [Exporting sprite sheets](#exporting-sprite-sheets)
  - [Tiled](#tiled)
    - [Setting up your tileset](#setting-up-your-tileset)
    - [Setting up your tilemap](#setting-up-your-tilemap)
      - [Rotating and flipping tiles](#rotating-and-flipping-tiles)
    - [Exporting your tilemap](#exporting-your-tilemap)
    - [Reading the tileset and tilemap in Monogame](#reading-the-tileset-and-tilemap-in-monogame)
- [Debugging](#debugging)
  
# MonoGameSandbox

# Projects

## Platformer001
TODO: add info for Platformer001

## Platformer002 - Gravity and AABB Collision Detection
TODO: add info for Platformer002

## Platformer003
TODO: add info for Platformer003

## Platformer004
TODO: add info for Platformer004

## Platformer005
TODO: add info for Platformer005

## Platformer006
TODO: add info for Platformer006

## Platformer007 - Tiled integration
Introduces Tiled integration - parsing of tileset (.tsx) and tilemap (.tmx) files 

# MonoGame

**A typical game loop:**

```
void gameLoop(){
	while (game != DONE) {
		getInput();
		physicsEngine.stepForward();
		updateWorld();
		render();
	}
	cleanup();
}
```

## Game Time

Monitor refresh rate: this is the number of times per second the monitor screen updates
Modern monitors refresh rates are typically around 60hz or higher

Tearing occurs when a games FPS is above the monitors refresh rate, for example if a game is rendering 60FPS and a monitor is only capable of 
refreshing the screen 30 times per second then some of the frames will be missed.
As long as the game's FPS is below or equal to the monitors refresh rate you will avoid tearing
tearing can be smoothed out with vertical syncing (v-sync)

### Fixed Time
For example setting `TargetElapsedTime` to 33ms will attempt to call the Update() method 30 times per second.
This is because 1000ms (1 second) divided by 30 equals 33ms 
MonoGame will 

### Variable Time
TODO: enter info for variable time

### Velocity Scaling Based on ElapsedGameTime

Multiplying velocity by time since the last call to `Update(GameTime)` (delta time) ensures that movement is consistent over time, regardless of frame rate fluctuations by scaling the velocity based on the time elapsed between frames.
This approach helps maintain a uniform speed over time even if the frame rate fluctuates which allows the game to handle varying frame rates smoothly and maintains uniform behavior, enhancing the overall user experience.

#### Example 
Velocity (v) is defined as the rate of change of position (x) with respect to time (t):\
v = x/t \
To find the new position (x) after a time step you use x = v*t
\
Assume a sprite with a velocity of 100 units per second.
Now consider a fluctuating frame rate: 

*High Frame Rate*
- Frame Rate: 120 FPS
- Delta Time(t): 1/20 seconds = 0.0083 seconds
- Change of position(x) = 100 x 0.0083 = 0.83 units

*Low Frame Rate*
- Frame Rate: 30 FPS
- Delta Time(t): 1/30 seconds = 0.0333 seconds
- Change of position(x) = 100 x 0.0333 = 3.333 units

#### Cumulative Movement Over One Second
Regardless of the frame rate, the total movement over one second should be consistent.

##### 120 FPS:
- Frames in 1 second: 120
- Movement per frame 0.83 units
- Total movement: 120 * 0.83 = 100 units

##### 30 FPS:
- Frames in 1 second: 30
- Movement per frame 3.33 units
- Total movement: 30 * 3.33 = 100 units

#### Real-World Analogy
Imagine you're walking on a moving walkway in an airport. If you walk at a steady pace, your speed is consistent. The moving walkway's speed might vary, but if you adjust your step size to the speed of the walkway, your overall travel distance remains consistent.

- **Consistent Step Size**: Walking at a fixed step size (like updating position without delta time) means your travel distance depends on the walkway's speed. If the walkway speeds up or slows down, your travel distance changes.

- **Adjusted Step Size**: Adjusting your step size based on the walkway's speed (like using delta time) means your travel distance remains the same regardless of the walkway's speed.

## Sprite Maps
TODO: add info for Sprite Maps

<hr/>

## Collision Detection

<hr/>

### BoundingBox Collision Detection

<hr/>

### Bounding Circle Collision Detection

<hr/>

### Pixel Collision Detection

<hr/>

### Collision Detection when Scaling and Rotating

<hr/>

### Using Matrices to simplify collision detection

The Matrix struct represents the right-handed 4x4 floating point matrix, which can store translation, position, scale, and rotation information of an image and can be used to translate or move vectors from one location to another

***There is one very important rule that you need to keep in mind, the order of matrix multiplications is important. In matrix multiplications, you should translate "*" to "after"***  

***In matrix multiplication, the order matters, and transformations are applied from right to left. Last statement first***  

***In general you want to first scale, then rotate, and finally translate***  

Below we will create a transformation matrix that represents a sprite drawn to the screen using a position, scale, rotation and origin, effectively it will mimic what the SpriteBatch.Draw() method does under the hood when called using this overload:
```
spriteBatch.Draw(_spriteTexture, _spritePosition, SpriteSourceRectangle, Color.White, _rotation, _origin, _spriteScale, SpriteEffects.None, 1);
```

Pseudocode for how matrix is calculated:

1. First: 		create a translation matrix using the sprite screen coordinates: Matrix.CreateTranslation(_spritePosition.X, _spritePosition.Y, 0)
2. Then : 		multiply this by a matrix containing the sprite scale: Matrix.CreateScale(spriteScale)
3. Then : 		multiply this by a matrix containing the sprite rotation angle: Matrix.CreateRotationZ(_rotation)
4. Finally: 	multiply this by a translation matrix created using the sprites origin: Matrix.CreateTranslation(-_origin.X, -_origin.Y, 0)

So in code the above is written like: (notice the reverse order, remember transformations are applied right to left, last statement first)

```
var matrix = Matrix.CreateTranslation(-_origin.X, -_origin.Y, 0) *
             Matrix.CreateRotationZ(_rotation) *
             Matrix.CreateScale(spriteScale) *
             Matrix.CreateTranslation(_spritePosition.X, _spritePosition.Y, 0);
```

Step 4 note: 
The origin is the zero point for a sprite, any origin greater than 0,0 (top left) will need to be subtracted from the positition translation (step 1) to get the final position of the sprite in the matrix.
So for example if our sprite texture dimensions are 64x64 and our origin is the centre of the sprite we can set the origin like this: \
```_origin = new Vector2(texture.Width / 2, texture.Height / 2);```

we will need to subtract the origin from the position translation to get the final position of the sprite, effectively what we do in step 4. is the same as what is being done in the line of code above.

This transormation matrix could now be used to move a vector from one location to another.

It can also be used to find the screen coordinate of any pixel of an image; conversly, given any screen coordinate the inverse of a matrix can be used to find the pixel coordinates of the pixel in an image
say we have two matrices matrix1 and matrix2 representing 2 different sprites (sprite1 and sprite2) position, scale, rotation and origin we can obtain the matrix that is the combination of both by simply multiplying them:
```
Matrix matrix1to2 = matrix1 * Matrix.Invert(matrix2);
```
Since the matrix1to2 matrix is the combination of the matrix1 and the inverse(mat2) matrices, transforming a pixel coordinate from sprite1 texture by this matrix will immediately give us the pixel coordinate in sprite2's texture

*Think of origin as the opposite of position: position MINUS origin EQUAS final position.*
*Origin in this case should be half the height and width of the sprite you are drawing, not the position of the parent. You will have to remember it will translate the sprite negatively that many pixels.*

<hr/>

### Swept AABB Collision Detection
\
When objects are moving fast and/or the program is running at a low frame rate AABB can assume that there was no collision and the moving object will move through another object like there was no collision at all. To avoid this, we need to somehow predict where the object travelled between each frame. This concept is called swept.
\
The velocity of an object is how far the object will move per second. 
If we multiply the velocity by the delta time, you will have the displacement that the object must move in this frame
\
A normal is the direction that an edge of an object is facing. Think of a perpendicular arrow pointing away from face at 90 degrees.
\




# 3rd Party Tools
## Aseprite
In order for your Aseprite spritesheets to work with the Aseprite.NET library you need to configure your spritesheets as described below

### Layers
Put your animations on seperate layers with the name of the layers corresponding to the different animations in your spritesheet eg for a walk animation name your layer "Walk".
You can also specify whether an animation should loop by adding a comma followed by the word "loop" after the layer name, eg: "Walk,loop" 

### User Data
You can also include user data in your spritesheet, for example you might want to specify that a frame within an Attack animation is a frame that when active in your game inflicts damage (eg the frame shows a a knight with his sword extended) so you could add the string "isattacking" to the User Data.
To add user data right click on a frame cell and select Cel Properties and enter a string in the User Data text field.
This will then be added to the outputted json file in meta.layers.

### Exporting sprite sheets
In Export Sprite Sheet on the Layout tab make sure that Sheet Type is is set to "By Rows", on the Sprite tab make sure Split Layers is selected, and on the Borders tab make sure Border Padding and Inner Padding is set to 0 and Spacing is set to 1, this will add a 1px border around each sprite in the exported sprite sheet image. 

Then on the Output tab make sure Output File and JSON Data are ticked and that Array is selected from the dropdown and that on that Layers is ticked next to Meta. In the Item Filename textbox make sure you have the "layer" and "frame" values in the format of: {layer}|{frame} this will give you an output that will specify for each frame the layer name and frame number in the frame filename property.

\
If all of the above was configured correctly you should have 2 files exported: a sprite sheet image with each sprite animation on a seperate row and a .json file that specifies the size and x,y coordinates of each frame within the image, something like this:

example_spritesheet_.json
```
{ "frames": [
    {
      "filename": "Walk,loop|0",
      "frame": { "x": 0, "y": 0, "w": 64, "h": 64 },
      "rotated": false,
      "trimmed": false,
      "spriteSourceSize": { "x": 0, "y": 0, "w": 64, "h": 64 },
      "sourceSize": { "w": 64, "h": 64 },
      "duration": 100
    },
    {
      "filename": "Walk,loop|1",
      "frame": { "x": 65, "y": 0, "w": 64, "h": 64 },
      "rotated": false,
      "trimmed": false,
      "spriteSourceSize": { "x": 0, "y": 0, "w": 64, "h": 64 },
      "sourceSize": { "w": 64, "h": 64 },
      "duration": 100
    },
    {
      "filename": "Attack|0",
      "frame": { "x": 0, "y": 65, "w": 64, "h": 64 },
      "rotated": false,
      "trimmed": false,
      "spriteSourceSize": { "x": 0, "y": 0, "w": 64, "h": 64 },
      "sourceSize": { "w": 64, "h": 64 },
      "duration": 100
    },
    {
      "filename": "Attack|1",
      "frame": { "x": 65, "y": 65, "w": 64, "h": 64 },
      "rotated": false,
      "trimmed": false,
      "spriteSourceSize": { "x": 0, "y": 0, "w": 64, "h": 64 },
      "sourceSize": { "w": 64, "h": 64 },
      "duration": 100
    }
    ...
  ],
  "meta": {
  "app": "https://www.aseprite.org/",
  "version": "1.3.7-x64",
  "image": "Spriteheet1.png",
  "format": "RGBA8888",
  "size": { "w": 389, "h": 129 },
  "scale": "1",
  "layers": [
   { "name": "Ready,loop", "opacity": 255, "blendMode": "normal" },
   { "name": "Attack", "opacity": 255, "blendMode": "normal", "cels": [{ "frame": 3, "data": "isattacking" }] }
  ]
 }
}
```


## Tiled

### Setting up your tileset
Make sure the tileset is named the same as the tileset image it is based on and that the image and the .tsj file are in the same location as this will effect the path value of the `image` property of the tileset json within the .tsj file and the embedded tileset of the tilemap json within the .tmj file (see exporting your tilemap below) e.g. if the tileset image source is named tileset1.png name your tileset tileset1 

### Setting up your tilemap
While working on the tilemap in Tiled you can keep the tilset(s) the tilemap uses as external (an external .tsj file) this allows flexibility in editing tilesets and for using them in multiple tilemaps rather than locking them into an individual tilemap. When you are ready to use the tilemap in Monogame you can embed the tileset in the exported .tmj file (see below) 
#### Rotating and flipping tiles
You can rotate tiles in your tilemap by pressing Z and Shift+Z and flip horizontal and vertical with X and Y keys. You need to do this *before you place the tiles

### Exporting your tilemap
Ensure `Embed tilesets` is enabled in the Preferences/General/Export Options section, this allows us to parse the tileset info (image size, tile dimensions etc) without requiring a seperate tileset json file.

### Reading the tileset and tilemap in Monogame
Tilemap files (.tmx, .tmj etc) specify a tilemap in a `data` array where each element of the array corresponds to a tile within a .tsx tileset source (the image containing the tileset specified in the `source` property). The array element value is the position within the tileset image, eg a value of 43 would be the 43rd tile in the tileset. To get the actual coordinates of the tile in the tileset image you will need to multiply the tile position value by the tile width (eg 32px). To get the y position (row) divide this result by the width of the tileset and multiply this by the tile height. The x position (column) is the remainder of this division which can be retrieved using the C# modulus operator.
\
eg to get the x and y position of a `32x32px` tile in the `43`rd position within a tileset of `width=608px` and `height=416px` do the following: 
- 43 * 32 = 1376
- x position: 1376 % 608 = 160  
- y position:  (1376 / 608) * 32 = 64 


How to integrate with Tiled:

- Ensure the tileset and tileset image files are in the project's `Content/Tilesets` folder and the tilemap files are in the `Content/Tilemaps` folder, and that Copy to Output Directory is set to Copy Always for all the files

- Start with the tilemap(s)
- go get the tilset (image and .tsj files) for the tilemap
- once the data for each tilemap layer is loaded in calculate the coordinates for each data element
  - replace the element value (the tile number from the tileset with the x,y position within the tileset image)
  - OR do the calculation on the fly for each element.







- Read tileset file (.tsx or .tsj file)
	- get tileset image source
	- get tile w & h
	- get tileset image w & h
	- get tilecount (?)
	- get column count (?)

- Load tilset image

- Load tilemap file (.tmx or .tmj file)
  - Get layers
    - Get BackgroundTiles layer
      - get layer w & h
      - get data array
    - Get CollidableTiles layer
      - get layer w & h
      - get data array

- 
	
	

<hr/>



# Debugging
Given that MonoGame is only a thin library, most of your game logic is probably not very tied in with the actual XNA code anyway. This should make it easy to make your game deeply testable outside of MonoGame.

Have you considered creating a lightweight version of your game logic that cuts out the content pipeline?

If you are really clean in separating game logic from presentation/UI then large parts of your code should be able to run in a unit-test/integration test like fashion. The only thing that MonoGame gives you is a `GameTime` struct on the `Update(..)` call. You dont need a `Game` object to make that call - you can provide that yourself during a test. This way bug hunting becomes a heck of a lot easier as you can just let the game run from a well-known state until you hit the bug or pass the test.

Likewise: If your game is grid based, you could use SadConsole as a lightweight alternative rendering method. This is especially great for turn based games, but can even be adapted to action games (with reduced visuals, of course. Just clip/round your positions to the nearest cell-positions/int-coordinates for crude visualisations).

For my style of games (strategy and old school turn based RPGs) this works remarkably well and even allows me to delay graphic decisions until I nailed the core game systems. As a bonus, most of my games end up having an ASCII debug mode even in the final version, as bug reports are so much easier to understand when you see a readable visual snapshot of the game state.

The Run/Debug command will only build whatever is a direct dependency of your run target. So if your project has a "Core" library project that holds the game logic, then you can have two UI projects in your solution that depend on "Core". One initializes a full game, including your Content directory, the other is a light weight thing with no extra content (like the aforementioned SadConsole renderer). As a bonus you can even add a unit-test project to test your logic/calculations in an automated way.

(Note that MonoGame 3.8 works nicely in library projects laid out that way. Once you have tasted the ability to do lightweight testing, working with fat game engines like Unity will feel like torture.)