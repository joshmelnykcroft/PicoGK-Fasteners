Build Plan

PicoGK-Fasteners should be a library that you add and allows you to create a fastener object.

creating a new object via FasternCustom will define the following:
 - standard
   head type
   thread size
   thread pitch
   length

alternatively, you can use FastenerID by passing the params a code that retrives standard sizes from a csv, which is filterable online to find the exact fastener you need.


to return voxel objects, the following methods can be called, each taking the position of the object to be the center axis under the head

 - .Screw()
   .ClearenceHole()
   .ScrewBasic()
   .ThreadedHole
   .ThreadedHoleBasic
   .CountersunkHole()
   .CounterBoredHole()
   .Nut()
   .Washer()
   .Stack()
   .WritetoBOM()

Each method (in addition to location) can have additional params passed to it that are specific to that method. 
Therefore, the new object params should only require basic info about size and thread, then allow for defaults to be set for common options, like clearence and grade.

To use both inputs, a private class is used via inheritance with all the methods passed to it ? not sure of the structure here.
