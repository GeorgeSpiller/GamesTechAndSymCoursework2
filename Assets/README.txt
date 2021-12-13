Bellow are some details on wheere you may find some examples of things mentioned in the specification.
Please note this is not an exhaustive list, and there will be additional sections that may also contain
smaller examples.

10 marks available for Newtonian Physics 
Assets\Scripts\Player\PlayerMovmentPhysicsBased.cs
This is the main example of Newtonian Physics, as the player (during 'encounters', aka each puzzle level, 
first person) moves around using forces.
This also handels the player colliding with gells. This 'collisision' uses triggers that then change the forces
that are applied to the player.
Drag forces acting on the player are changed based on their collision with the ground (separate drag for 
ground based physics and air based physics).

10 marks available for Collision Detection 
Assets\Scripts\Finish.cs
basic example of collision detection (one of many; see player, gell blob, gell patch objects)
Assets\Scripts\Gell\CreateGell.cs
This handels the merging of gell patches on the floor, manipulating the collision volume(s) of neighbouring
patches based on their distances to eachother, and merging patches together into bigger ones where appropriate.
Assets\Scripts\Gell\GellCollision.cs
This handels queueing momentum changes to the players velocity when they enter or exit a collection of 
gell patches. 

10 marks available for Collision Response and Feedback 
Assets\Scripts\Player\PlayerMovmentPhysicsBased.cs
Examples of changing player acceleration, drag based on interactions with other objects (eg. gells) by using
various different forces (eg. changes in acceleration, Impulse forces). 
Assets\Scripts\Gell\CreateGell.cs
This handels the merging of gell patches on the floor, manipulating the collision volume(s) of neighbouring
patches based on their distances to eachother, and merging patches together into bigger ones where appropriate.
Please also see the player object, who has a specficic physics material:
Assets\Materials\PhysicsMaterials\PlayerPhysicsmat.physicMaterial
that helps handel in air collision with walls
Assets\Scripts\Gell\GellableCube.cs
Changes the vlues of the cubes physics material based on what color 'gell' hasbeen shot at it. seen on lvl8

8 marks available for state-based behaviours 
# 4 marks for basic state based behaviour (menu system, simple opponents) 
Assets\Scripts\Control\EscapeMenu.cs
escape screen that pauses physics interactions.
Assets\Scripts\Control\ManageGameState.cs
Allows states to persist accross level completion. NOTE: I have turned of data being saved accross the game 
restarting to help testing/marking (consistant replayablility). Also there is an issue with this working on build
which prevents the player from entering levels, and therefore can only be demonstrated within the editor
Assets\Scripts\AI\Memoryless\RandomAI.cs
Example of a 'Memoryless' schotastic AI.
Assets\Scripts\AI\FSM\FSM_AI.cs
FSM AI that transistions to different states based on various game enviromental factors
all scripts in Assets\Scripts\AI\Hierachal_FSM
Hierachal FSM AI that transistions to different FSM's based on various game enviromental factors

12 marks available for probability and game design 
Assets\Scripts\AI\Memoryless\RandomAI.cs
Example of a 'Memoryless' schotastic AI.
Assets\Scripts\AI\FSM\FSM_AI.cs
FSM AI that transistions to different states based on various game enviromental factors
all scripts in Assets\Scripts\AI\Hierachal_FSM
Hierachal FSM AI that transistions to different FSM's based on various game enviromental factors
feedback loops can be seen through the level progression: each level gets progressivly harder, needing for
the player to utilise theory learnt from the previous levels in the next levels (eg. lvl2 to lvl3)