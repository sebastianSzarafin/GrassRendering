# GrassRendering
<p align="center">
    <img width="700" alt="scenery" src="https://user-images.githubusercontent.com/74315304/221588943-ec04eddb-57fa-4641-972b-6fa647dd9907.png">
</p>

## Introduction
This is a project for *Computer Graphics 2022* course on Computer Science course. The application is implemented on GPU with usage of OpenGL library. It displays a small scenery which consists of 3D objects, some of them imported using [Assimp](https://learnopengl.com/Model-Loading/Assimp) package, and some of them written directly in code.

Starting this project there were defined crutial functionalities that needed to be implemented into the code, they are:
- Multiple moving and static objects
- Possibility to turn on/off vibrations of moving objects
- 3 different cameras:
  - fixed, observing the scene
  - fixed, following the moving object (in this case rubber ducks) 
  - FPP
- Day/night cycle
- Illumination with multiple light sources
- Light attenuation along with increasing the distance 
- Fog effect
- Mirror effect (accomplished with the water flow simulation)
---

## Description
### Grass
As mentioned before, the scene consists of 3D objects. In particular of grass clumps created with 2D billboards, where each clump has some random factors (such as height) to make the look more realistic. Combined with a flowmap it generated a pretty decent wind effect. Also, for optimalization purpouses, the bigger the distance from camera is, the less clumps are drawn.
### Water
Apart from grass, a significant milestone was the river simulation. The mirror was obtained by creating 2 additional textures based on the camera coordinates - reflection and refraction, showing respectively it's reflection and the terrain underneath. They need to be calculated during each frame render and mixed in a shader afterwards. To make it more "wavy", the same flowmap was applied.
### Lightning
To simulate some point lights, a basic Phong model was used. Simple spheres above the water, are only for visualisation. The lights turn themselfes on during nighttime.
### Fog
Depending from the time of the day, objects may or may not be more or less visible. That is thanks to the fog effect, calculated with function 
$$e^{-(distance \cdot density)^{gradient}}$$
based on the object's $distance$ from camera, and 2 factors - fog $density$ dependent from the day time, and $gradient$ which is responsible for how quickly visibility decreases with distance. The effect is strongest in the morning (right after the nighttime).
### Moving objects
Final detail was adding a bunch of floating ducks on the water. Each one draws a starting position, moves on a flatten sinusoid in a straight line to the end of the scene and loops back to the beginning.
