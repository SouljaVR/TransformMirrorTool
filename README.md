# Unity Transform Mirror Tool

A Unity Editor tool for mirroring transformations between GameObjects in real-time. Perfect for working with character rigs, symmetrical objects, or any scenario where you need to mirror or copy movements between objects.

![MirrorTransformDemo](https://github.com/user-attachments/assets/77a2e656-10a2-47aa-ba75-a0a0abd095d7)

## Features

- Real-time mirroring of position and rotation in world space
- Works reliably with complex hierarchies and FBX imports
- Mirror or directly copy transformations on each axis (X, Y, Z)
- Bypass settings to lock specific position or rotation axes
- Save and load presets for different mirroring configurations
- Quick pair creation from hierarchy selection
- Swap source/target objects with one click
- Full undo/redo support

## Installation

1. Import the unity package from releases

**OR**

1. Clone this repository or download the source code
2. Copy the scripts into your Unity project's `Assets` folder
3. The tool will be available under the "Tools > BigSoulja" menu in Unity

Required Files:
- `TransformMirrorTool.cs`
- `TransformMirrorPreset.cs`
- `TransformMirrorPresetAsset.cs`
- `TransformPairData.cs`

## Usage

### Basic Setup

1. Open the tool via `Tools > BigSoulja > Transform Mirror Tool`
2. Add a new pair using any **one** of these methods:
   - Click "Add Pair" in the tool window, then add your gameobjects into the base and source fields.
     
     **OR**

   - Select two objects in the hierarchy, right-click and choose "Add Pair to Transform Mirror Tool". This option will only appear if **two** gameobjects are selected.
3. Assign your source and target objects
4. Click "Start Mirroring" to begin real-time mirroring. Moving the base object will copy/mirror the same movement on the target object. The target object cannot be moved manually while mirroring is enabled. This system works when recording animations too.
5. Click stop mirroring when you are done. The transforms will reset back to their original states.

### Mirror Settings

For each pair, you can configure:

- **Mirror Axes**: Choose which axes (X, Y, Z) should be mirrored. If deselected it will move both objects in the same direction instead.
- **Bypass Settings**: 
  - Position Bypass: Lock specific position axes from changing on the target
  - Rotation Bypass: Lock specific rotation axes from changing on the target

### Working with Pairs

- **Restore**: Reset objects to their initial transforms
- **Swap Pair**: Switch the base and target objects
- **Remove Pair**: Delete a pair from the tool

### Preset System

Save and load mirroring configurations:

1. Enter a preset name
2. Click "Save Preset" to store the current configuration
3. Use the dropdown to select and load saved presets

## Tips & Best Practices

1. **Complex Hierarchies**: The tool works in world space, making it reliable for complex hierarchies and FBX imports with different local axes.

2. **Character Rigs**: When working with character rigs:
   - Start with mirroring enabled on all axes
   - Use bypass settings if certain axes need to be locked
   - Test movements to ensure correct mirroring behavior

3. **Performance**: 
   - Remove unused pairs to maintain editor performance
   - Stop mirroring when not actively using the tool

## Things to note:

- Mirroring is based on world space transformations
- Scale changes are mirrored proportionally
- Preset system relies on object names for reference

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License with Additional Terms - see below for details.

## Support

For issues, questions, or suggestions:
1. Open an issue in this repository
2. Discord: bigsoulja

# MIT License with Additional Terms

Copyright (c) [2024] [BigSoulja/SouljaVR]

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, and/or sublicense copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

1. The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

2. Commercial Use Restriction: The Software may not be included, in whole or in part, in any commercial, paid, or premium products or services without explicit written permission from the copyright holder. "Commercial use" means any use intended for or directed toward commercial advantage or monetary compensation.

3. Attribution Requirement: Any use, modification, or distribution of the Software must include clear and visible attribution to the original author and project. This attribution must include:
   - The original project name
   - A link to the original repository
   - The original author's name
   - The original copyright notice

4. Derivative Works: Any derivative works must be distributed under the same license terms and conditions as this license.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
