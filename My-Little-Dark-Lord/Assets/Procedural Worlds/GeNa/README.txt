WELCOME:
-------------

Welcome to GeNa - the procedural content generation system for Unity.

CONCEPTS:
--------------

GeNa is a system that makes it easy to procedurally place assets within your scene. 

Adding assets is done with GeNa spawners. GeNa spawners can add terrain grass, terrain trees, and any sort of prefab, and use a variety of algorithms to choose how these assets will be placed or spawned into your scene.

GeNa will spawn your resources onto the designated target - which can be either a mesh or a terrain. The target is selected by either shift left clicking or control left clicking on it.


BASIC USAGE:
------------------

1. Create your scene and add a GeNa spawner either by selecting GameObject -> GeNa -> Add Spawner, or by right clicking a blank space in your scene hierarchy and selecting GeNa -> Add Spawner.


2. Add the assets you want to spawn by clicking the Add Grass button to add one of your terrain grasses, or the Add Tree button to add one of your terrain trees, or by dragging and dropping either a SpeedTree .spm file, or a prefab, or a selection of prefabs that have already been instantiated in your scene onto the 'Add Prefabs or SpeedTrees here' box.

GeNa will add the resource and it can be viewed in the 'Prototype' section. Each prototype maintains a set of meta data that controls how it can be placed into your scene. TIP : You can control the height that trees and prefabs spawn above or below the target by modifying the Min and Max Position Offsets under the Detail folder for each prototype.


3. Visualise where your prototype can be spawned into the scene. To activate the visualiser hit Shift and the left mouse button on the target mesh (which needs a collider to be detected) or target terrain. 

The green dots represent where the resource can be spawned - and this can be tailored via the "Spawn Criteria" section. The way the spawner works is to sample the location you clicked on and then select other locations with similar features as potential spawn locations.

* The Check Height criteria selects for heights within the height range of where you selected. So if for example you shift or control click on something at 50m in your environment and have a height range of 20m, then heights from 40m to 60m will be selected.

* The Check Slope criteria selects for slopes within the slope range of where you selected. So if for example you shift or control click on something at 20 degrees in your environment - and have a slope range of 20 degress, then slopes from 10 degrees to 30 degrees will be selected.

* The Check Textures criteria selects for the textures you clicked on.

* The Check Mask criteria selects for the noise or image based mask you supply.


4. Spawn your resource into your scene by hitting Ctrl + Left Mouse Click.


5. Celebrate!!


NOTES:

The main control shortcuts are described at the top of the spawner.

To understand what each setting does, hover over it with the mouse.