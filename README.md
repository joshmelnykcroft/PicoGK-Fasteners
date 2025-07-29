#PicoGK-Fasteners 
---
## Description
### PicoGK-Fasteners is a fastener and fastener utility library for your PicoGK project.

With this library, you can add fasteners or their respective holes as voxel bodies by defining a fastener and then calling that fastener's methods to generate the type of voxel body you want. More in the How to Use section below.

PicoGK-Fasteners can currently generate the following types of voxel objects:
 
#### Fasteners
- Hex Heads 
- Countersunk 
- Button Head 
- Socket Head Cap Screw (SHCS)
All with the following driver options:
- Hex
- Philips
- Robinson
- None

#### Holes 
- Clearance Holes 
- Countersunk Holes
- Counterbored Holes
- Tapped holes
- Tap Drill Holes

#### Other Fasteners 
- Washers 
- Nuts 
- Fastener Stacks (Fastener/Washer/Gap/Washer/Nut)

Most methods take a LocalFrame for its position, and possibly some other info specific to that method. 

## Dependencies
 - PicoGK
 - ShapeKernel

## How to Use 

## Still To Do 

- [] Impliment some sort of BOM export
- [] Add an offset to countersunk holes so that screw heads can sit below the surface
- [] Tidy up code 
- [] 


## Credits


