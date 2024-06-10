# Platformer001 - Basic spritesheet aninmation with movement

Spritesheet animation with walking, running, jumping, and attacking animations \
Monogame classes used: ```Vector2```, ```Texture2D```, ```Keyboard```

There is a spritesheet: `knight_spritesheet.png` in the `Content` folder containing 9 animations with 6 frames per animation. Only the first 6 animations are implemented (`Ready`, `Walk`, `Run`, `Attack1`, `Attack2`, and `Jump`)

The animations were created using [Aseprite](https://https://aseprite.org/), along with the spritesheet mentioned above a JSON data file was also created using the Export Sprite Sheet functionality of Aseprite: `knight_spritesheet_array.json`. This file contains the coordinates within the spritesheet image for the individual frames. This json file is consumed by `AnimationManager`.
The `knight_spritesheet.aseprite` file has been included, if you open this file in Aseprite you can see the animations (each animation on a seperate row in the `knight_spritesheet.png` image) have been placed on different Aseprite layers with each layer corresponding to a value in the `AnimationType` enum and each of the 6 frames corresponding to a column in the `knight_spritesheet.png` image

![alt text](aseprite_screenshot.png "aseprite_screenshot")

Here we can see the Run animation:

![alt text](aseprite_animation.gif "aseprite_animation.gif")

---
### Controls:

| Action      | Key       |
| --------    | -------   |
| Walk left   | A         |
| Walk right  | D         |
| Run         | Shift (hold down while walking)    |
| Jump        | W         |
| Attack 1    | Space     |
| Attack 2    | Left Ctrl |

\
\
Directions:

![alt text](Vector2_Directions.png "Title")

