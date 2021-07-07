# Mid-Air Haptic Textures: Translating Image Features to Produce Haptic Feedback
This project is part of an internal research project exploring the possibility of rendering surface textures using an Ultraleap haptic array. This project allows users to generate haptic sensations from images by modulating intensity via image displacement map greyscale values. The draw frequency of the sensation is also automatically set based on the output from a neural network that takes as input various statistical features calculated from the pixel values in an image. These features correspond to how much roughness is contained within the image. This roughness is then varied by adjusting the draw frequency of the haptic feedback. In addition, both visual and haptic feedback are directly linked which ensures congruency for the user whilst exploring a visuo-haptic texture.

## About
This repository contains the following:

* A Unity project that demonstrates the generation of haptic textures.
* C# scripts responsible for processing and rendering different haptic textural sensations.
* Two simple (2D & 3D) demo scenes presenting the application of haptic textures in 2D and 3D environments.
* A small repository of sample textures.
* Prefab gameobjects with corresponding texture components attached: 
	- A [Haptics] component with some pre-attached haptics script and preset values. 

This Unity project can be run on both Ultrahaptics "STRATOS" series hardware (Inspire & Explore).

#### What does this project do?
Contained within this project are scripts that enable the generation of haptic textures. Images and their associated displacement maps can be imported and used to modulate the intensity of a haptic sensation. Moreover, this project contains a pre-trained neural network that is used to automatically calculate the draw frequency of each image-based haptic sensation to convey different sensations of roughness.

You can add your own textured images provided they have an appropriate displacement map.

Visual feedback and haptic feedback are coupled when using this particular rendering method, which results in excellent congruency between these modalities.

Ultrasonic mid-air haptic textures can be produced within 2D and 3D environments, offering the possibility to explore many different use cases for numerous applications. 

## Prerequisites
To explore this project and create your own haptic textures some requirements must first be considered:

#### Hardware

* Ultrahaptics STRATOS platform hardware device (Inspire or Explore)
* Leapmotion Hand Tracking device

#### Software
* Ultrahaptics SDK 2.6.5: https://developer.ultrahaptics.com/downloads/sdk/
* Leap Motion SDK
	- (Windows: v4 https://developer-archive.leapmotion.com/downloads/external/v4-developer-beta/windows)
 	- (Mac: v2 https://developer.leapmotion.com/sdk/v2)
* Leap Motion Core Assets Unity Package: https://developer.leapmotion.com/unity
* This project was built using Unity 2020.3.2f1, but should work with other Unity versions (not tested).
* Will import with a dependency on Unity's 'Python for Unity' experimental package. This will be installed through the Unity package manager by default. 

## Usage

#### Setting up the project
1. Download the project and copy it into your relevant workspace. 
2. Navigate to the appropriate folder in your Unity Hub / Unity Standalone and open the project.
3. Once open you will see a host of errors, but don't panic. The steps below will address these:
	- Import the Leap Motion Core Assets Unity package, and the Ultrahaptics Unity package into the project (found in your Ultrahaptics SDK folder). When prompted, press "Import".
4. The project should now be ready to use!
	- **TIP:** *It is worth double-checking that the LeapHandController Gameobject in the scene contains the HandData.cs script found in: /Assets/Scripts/HandData*

#### Creating a new scene
The following describes how to set up a new scene in a similar fashion to our demo scenes to interact with Haptic Textures.

1. Add the Leap Hand Controller prefab to the scene (Assets/LeapMotion/Prefabs/Misc/LeapHandController) and set it up per your preference.
2. Add the HandData script to the Leap Hand Controller game object and set the 'Leap Provider' as the LeapHandController prefab. Set up the 'Hand Model Manager' script so that the model pool contains 2 elements, where the first element has references to the 'LoPoly Rigged Hand' and the second element references the 'RigidRoundHand' model. Ensure the 'IsEnabled' box is checked.
3. Add the [Haptics] prefab to the scene (Assets/Prefabs/[Haptics])
4. Within [Haptics], in the 'Haptic Renderer' script, set the number of hands you require in the scene. Assign the "ScanPosition" (we currently use bone2 of the middle finger on the left hand and right hand), and assign a location on the hand where you want the haptics to be played.
	**TIP:** *Scan position in Haptic Renderer is the position on the hand where we raycast from onto the texture, to find the displacement. Hand Positions in Haptic Runner are where we then project the haptic feedback. Feel free to change and experiment with these values!*
6. Add the model you want to apply textures to the scene. 
7. The model needs the following:
     - A mesh renderer with a basic material and standard shader (with the image as albedo, height map + normal map etc).
     - A Texture Attributes script.

#### Texture Importing
The scene contains several different textures that are located in: /Assets/Textures/. These are ready to be used and can be applied to any game object. 

However, it is possible to create your own textures!

To do so follow these steps:

1. Create a new folder within the /Assets/Textures/ folder entitled the texture/image you wish to use.
2. In this folder, copy in your texture image, and displacement map (grey-scale image) [Ensure Read/Write Enabled is checked on the import settings - Advanced!], along with any additional material component you wish to use (metallic, normal map etc).
	- **TIP:** *Make sure to enhance the contrast and saturation on your displacement map, so the separation between white and black values is distinguished. This will improve haptic rendering!*
4. Create an empty game object in your Unity scene and attach to it a MeshRenderer object then assign to it a basic material object with a standard shader. Attach each of the corresponding images to the material i.e. place the image into the "Albedo" selection window.
5. Attach a 'Texture Attributes' script to the empty game object. within the 'Texture Attributes' script, press the 'Calculate' button and the script will automatically determine the appropriate draw frequency for the given roughness contained within the image. There are additional sliders to further adjust both draw frequency and intensity.

* **Draw Frequency** - This value can be adjusted from 20 - 80. A higher value will yield a smoother sensation.
* **Intensity Minimum/Maximum** - These values will allow you to adjust the variance between the intensity values used for the haptic sensation. It is best to keep the maximum setting at 1. Bringing the minimum value closer to 1 will result in a flatter texture, and again feel smooth.

## License
This project is open-sourced under Ultraleap's proprietary closed-source license. The textures used within this repo were obtained at https://www.cc0textures.com.

## Support, Contact & Contribution
For any questions regarding this project, please contact david.beattie@ultraleap.com. Alternatively, please branch this repo and leave comments so we can keep up to date with any requests and issues.

## Acknowledgements
This project has received funding from the European Union’s Horizon 2020 research and innovation programme under grant agreement No 801413, H-Reality.