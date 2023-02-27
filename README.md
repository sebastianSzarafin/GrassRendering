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
- Light fading along with increasing the distance 
- Fog effect
- Mirror effect (accomplished with the water flow simulation)
---

## Description
As mentioned before, the scene consists of 3D objects. In particular of grass clumps created with 2D billboards, where each clump has some random factors (such as height) to make the look more realistic. Combined with a flowmap it generated a pretty decent wind effect.
dupa
