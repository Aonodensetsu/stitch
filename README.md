# Stitch

### Controller Logic Helper for Parameter Interactions

Stitch is a tool inspired by VRCFury (and integrating with it or Modular Avatar), which aims at the place VRCFury deliberately excludes from its scope - Controllers.  
Stitch requires VRCFury or Modular Avatar to be installed.

### Usage

Stitch is distributed as a VPM package, so just click: ![Add Stitch to VCC](https://img.shields.io/badge/Add_Stitch_to_VCC-blue?link=vcc%3A%2F%2Fvpm%2FaddRepo%3Furl%3Dhttps%3A%2F%2FAonodensetsu.github.io%2Fstitch%2Findex.json), then add it to any of your projects and you're all set!

Similarly to other non-destructive editors, click Add Component at the bottom of any of your avatar's objects, then choose Stitch to access its menu.

Stitch shows up as a simple menu with a list of Actions.

![main menu](media/main.png)

<details>
<summary>Technicality</summary>

Technically, Stitch is a UI for [Advanced BlendTree Techniques](https://vrc.school/docs/Other/Advanced-BlendTrees).  
The created parameters are [AAPs](https://vrc.school/docs/Other/AAPs).  
The possibilities of those and their limitations apply.

</details>

---

### Logic > And [∧]

![and action](media/and.png)

The And action performs the AND logic operation.  
The input values are restricted in the range 0 to 1.  
The output is on when both inputs are on.

### Logic > Gate

![gate action](media/gate.png)

The Gate action performs an arbitrary logic operation.  
The input values are restricted in the range 0 to 1.  
The output can take one of four values depending on the inputs.

00 - Left and right inputs are zero.  
01 - Left input is zero, right input is one.  
eg.

<details>
<summary>Common logic gates</summary>

The And action corresponds to a Gate of 0,0,0,1.  
The Or action corresponds to a Gate of 0,1,1,1.  
The Not action corresponds to a Gate of 1,0,0,0 with the same input parameter used twice.  
The Not action uses a more performant setup, use it instead of this.

</details>

### Logic > Not [¬]

![not action](media/not.png)

The Not action performs the NOT logic operation.  
The input value is restricted in the range 0 to 1.  
The output is on when the input is off.  

For values between zero and one, this is equivalent to 1 - n.  
This implementation is more performant than the Subtract action (and equivalent to Remap 0-1->1-0).

### Logic > Or [∨]

![or action](media/or.png)

The Or action perform the OR logic operation.  
The input value is restricted in the range 0 to 1.  
The output is on when either of the inputs is on.

### Math > Absolute

![absolute action](media/absolute.png)

The Absolute action returns the same value as the input for positive inputs, and the negated value for negative ones.  
The input value is restricted in the range -100 to 100.  

### Math > Add [+]

![add action](media/add.png)

The Add action sums the values of two parameters.  
The input values are restricted in the range -100 to 100.  
Either input (but not both) can be replaced with a constant number.

### Math > Compare

![compare action](media/compare.png)

The Compare actions provides common value comparisons between two values:  
- More Than  
- More Than or Equal  
- Less Than  
- Less Than or Equal  
- Equal  
- Inequal  

The comparisons have a tolerance of 1/140 (slightly less than VRChat's sync precision).  
The input values are restricted in the range -100 to 100.  
The output is on then the comparison is true.

### Math > Maximum

![maximum action](media/maximum.png)

The Maximum action returns the larger of the inputs.  
The input values are restricted in the range -100 to 100.  
Either input (but not both) can be replaced with a constant number.

### Math > Mean

![mean action](media/mean.png)

The Mean action returns the average (mean) value of two parameters.  
The input values are restricted in the range -100 to 100.  
Either input (but not both) can be replaced with a constant number.

### Math > Minimum

![minimum action](media/minimum.png)

The Minimum action returns the smaller of the inputs.  
The input values are restricted in the range -100 to 100.  
Either input (but not both) can be replaced with a constant number.

### Math > Multiply [×]

![multiply action](media/multiply.png)

The Multiply action multiplies the values of two parameters.  
The input values are restricted in the range -100 to 100.  
Either input (but not both) can be replaced with a constant number.

### Math > Subtract [−]

![subtract action](media/subtract.png)

The Subtract action subtracts the value of one input from the other.  
The input values are restricted in the range -100 to 100.  
This is equivalent to the Add action with the second parameter negated.  
Either input (but not both) can be replaced with a constant number.

### Parameter > Copy

![copy action](media/copy.png)

The Copy action duplicates the value of a parameter into another (or itself).  
The input value is restricted in the range -100 to 100.  
This is useful VERY rarely.

### Parameter > Default

![default action](media/default.png)

The Default action sets the starting value of the parameter created in the controller.

### Parameter > Frametime

![frametime action](media/frametime.png)

The Frametime action provides a duration in seconds since the previous frame.  
After every ~27 hours, will produce an invalid `-100` value for one frame.  
Only one shared frametime is created for all usages of the Action.

### Parameter > Global

![global action](media/global.png)

By default, Stitch will use the features of VRCFury or Modular Avatar to prevent conflicts between parameters.  
Use this action when a parameter needs to be shared between different setups.

### Parameter > Monostable

![monostable action](media/monostable.png)

The output value is 1 for a single frame whenever the input value changes.

### Parameter > Remap

![remap action](media/remap.png)

The Remap action allows modifying the range of values a parameter takes on.  
The input range must be written in ascending order (second input larger than first).  
The output range must not be length zero (fourth input different from third).  
The output takes on a value based on the percentage along the input range.

For example, a remap of 0-1 to 2-0 will change values:  
  0 -> 2  
  0.1 -> 1.8  
  0.2 -> 1.6  
  0.9 -> 0.2

### Parameter > Select

![select action](media/select.png)

The Select action output is a weighed choice of inputs.  
The first input (selector) is restricted in the range 0 to 1.  
The other inputs are restricted in the range -100 to 100.  
At most one input may be replaced by constant.  
The output takes a fraction of one input and the remaining part from the other based on the selector.

For example, using values 0.4, -1, 2:
  0.4 * -1 + 0.6 * 2 = 0.8

### Parameter > Smooth

![smooth action](media/smooth.png)

The Smooth action performs parameter smoothing over time.  
The output value will approach the input value smoothly over time based on the smoothing type and strength.  
The smoothing strength is restricted in the range 0 to 1.

### Parameter > Timer

![smooth action](media/timer.png)

The Timer action provides a count of seconds since the animator was enabled.  
Resets every ~27 hours.  
Only one shared timer is created for all usages of the Action.

---

In the case VRCFury and Modular Avatar are not installed, after setting up Stitch, clicking Build & Test (or Build & Publish) in the VRChat SDK panel will create a prefab with instructions to install one of the dependencies.  
Clicking on either button will try to install the selected dependency in VCC.  
A similar error message is also displayed on the Stitch components.

![install instruction](media/instruction.png)

Repo templated from https://github.com/vrchat-community/template-package.

