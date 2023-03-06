# GrassRendering
<p align="center">
    <img width="75%" alt="scenery" src="https://user-images.githubusercontent.com/74315304/221588943-ec04eddb-57fa-4641-972b-6fa647dd9907.png">
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
<p align="center">
    <img width="50%" alt="grass" src="https://user-images.githubusercontent.com/74315304/222485696-275f82d9-9bd0-47d1-b80a-7c8927871079.gif">
</p>

### Water
Apart from grass, a significant milestone was the river simulation. The mirror was obtained by creating 2 additional textures based on the camera coordinates - reflection and refraction, showing respectively it's reflection and the terrain underneath. They need to be calculated during each frame render and mixed in a shader afterwards. 
<p align="center">
    <img width="32%" alt="reflection" src="https://user-images.githubusercontent.com/74315304/222494597-e50c4aca-d785-4dbc-adc3-d18535a319ec.png">
    <img width="32%" alt="refraction" src="https://user-images.githubusercontent.com/74315304/222494607-562417f4-2ac2-4d8d-8f44-bbee03c42bbc.png">
    <img width="32%" alt="mirror" src="https://user-images.githubusercontent.com/74315304/222497184-c8ffb002-cc3e-4fd7-b1dd-71b64d6321ca.png">
</p>
When mixed by a factor based on an angle between camera position and a water tile's normal it was possible to attain Fresnel Effect. To make it more "wavy", the same flowmap was applied. <br><br>
<p align="center">
    <img width="49.5%" alt="fresnel" src="https://user-images.githubusercontent.com/74315304/222500775-18fa579b-3602-403a-bd3a-d52b08d74f04.gif">
    <img width="49.5%" alt="fresnel" src="https://user-images.githubusercontent.com/74315304/222502896-fbe48904-d307-4126-8689-30552e3aa488.gif">
</p>

### Lightning
To simulate some point lights, a basic Phong model was used. Simple spheres above the water, are only for visualisation. The lights turn themselfes on during nighttime.
<p align="center">
    <img width="50%" alt="fresnel" src="https://user-images.githubusercontent.com/74315304/222504532-d4922016-1c61-4d38-9812-c996682ae84d.gif">
</p>

### Fog
Depending from the time of the day, objects may or may not be more or less visible. That is thanks to the fog effect, calculated with function 
$$e^{-(distance \cdot density)^{gradient}}$$
based on the object's $distance$ from camera, and 2 factors - fog $density$ dependent from the day time, and $gradient$ which is responsible for how quickly visibility decreases with distance. The effect is strongest in the morning (right after the nighttime).
<p align="center">
    <img width="50%" alt="fresnel" src="https://user-images.githubusercontent.com/74315304/222506202-34a2493f-7741-4101-9c6f-3673ccc58cfa.gif">
</p>

### Moving objects
Final detail was adding a bunch of floating ducks on the water. Each one draws a starting position, moves on a flattened sinusoid in a straight line to the end of the scene and loops back to the beginning. Also, it is possible to make each one "shake" by rotating them by small angle across X axis with high frequency.
<p align="center">
    <img width="50%" alt="fresnel" src="https://user-images.githubusercontent.com/74315304/222507226-a23519a0-ace6-44bb-85b4-6e1f0b79f588.gif">
</p>
