2.0.0:
- VRCFury or Modular Avatar are now required
  - this is because I properly fixed namespacing issues by separating the resulting tree based on where the components are in the avatar - like MA or VF do too
  - you would have one controller to add manually per object where you use Stitch, just install one of the probably two most common packages
- the manual install instructions are now dependency install instructions
- added an error message to the Stitch component if dependencies are missing
- added a unique id to every Stitch component
  - it is shown in the component ui
  - and also the generated blendtree, allowing for debugging
  - the id may sometimes change unexpectedly when copying objects around, but it will stay unique
- the clips are now created in-memory and cached instead of being real assets in a folder
  - they also now have 2 keyframes and are exactly one frame long, which is needed for optimization
- the animator itself is also stored in-memory
  - since everything is in-memory, the Temp folder is not needed
  - that makes the package no longer need to be visible in the packages list
- the action trees are now named based on their type (an Add action would say 'result=a+b')
- the Default action is fixed (oops)
- since both dependencies support it, the Global action is now always available
- the Remap action now checks for inequal values on the right side

1.3.0:
- Modular Avatar support
- localization support for the remaining visible elements (action buttons, debug messages)
- moved the add/remove action buttons above the list
- controller layer renamed to Stitch instead of the default Base Layer, for easier debug
- creates object under the avatar root to not propagate MA/VF parameter renames
- enlarged the Actions dropdown in the readme :p

1.2.0:
- added the Smooth action (linear and exponential variants)

1.1.0:
- added input validation (oops)
- added constant input to math operations
- changed the Weight parameter to constant 1
- added the (arbitrary) Gate action
- added the Remap action
- added some padding to elements

1.0.0:
- initial working version of the package
  - added the Add action
  - added the Or action
  - added the Not action
  - added the Add action
  - added the Subtract action
  - added the Multiply action
  - added the Global action (in VRCFury integration)
  - integrated with the VRCFury non-destructive build system
  - added a manual deployment option if no supported build system available

