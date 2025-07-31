PicoGK-Fasteners 

## Description
### PicoGK-Fasteners is a fastener and fastener utility library for your PicoGK project.

With this library, you can add fasteners or their respective holes as voxel bodies by defining a fastener (size, length, etc.) and then calling that fastener's methods to generate the type of voxel body you want. More in the How to Use section below.

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

Start by adding PicoGK-Fasteners to your project
 
> git clone blah blah recursive 

Include fasteners to your project by adding 
> using PicoGK-Fasteners;

To create your first fastener object, choose one of two constructors:

The first constructor creates generic fastener given a few inputs.

> Fastener oMyBasicFastener = new( // list inputs here)

This is not a accurate representation of a fastener you'd buy off the shelf, but should be close enough for you to carry on with your  project.

The second constructor creates a more accurate representation of your fastener based on measurements you provide.

> Fastener oMyDefinedFastener = new(
// list inputs here)

Once you have your fastener defined, you can call one of the following methods to create a variety of voxel objects that you can add or subtract to your project.

#### Fasteners

- .ScrewBasic
- .ScrewThreaded
- .Nut
- .Washer
- .Stack 

#### Holes

.HoleThreaded
.HoleClearance
.HoleBasic
.HoleCounterbored
.HoleCountersunk
.Tapdrill


## Roadmap to release 1.0

- [] Impliment some sort of BOM export
- [] Add an offset to countersunk holes so that screw heads can sit below the surface
- [] Tidy up code 
- [] 


## Credits
Thanks to Leap71 for creating and sharing PicoGK , as well as writing the Coding for Engineers book.


