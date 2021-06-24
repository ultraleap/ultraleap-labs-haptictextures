## TexNet: Incorporating the Perception of Visual Roughness into the Design of Mid-Air Haptic Textures 
Ultrasonic mid-air haptic feedback enables the tactile exploration of virtual objects in digital environments. However, an object’s shape and texture is perceived multimodally, commencing before tactile contact is made. Visual cues, such as the spatial distribution of surface elements, play a critical first step in forming an expectation of how a texture should feel. When rendering surface texture virtually, its verisimilitude is dependent on whether these visually
inferred prior expectations are experienced during tactile exploration. To that end, our work proposes a method where the visual perception of roughness is integrated into the rendering algorithm of mid-air haptic texture feedback. We develop a machine learning model trained on crowd-sourced visual roughness ratings of texture images from the Penn Haptic Texture Toolkit (HaTT). We establish tactile roughness ratings for different mid-air haptic stimuli and
match these ratings to our model’s output, creating an end-to-end automated visuo-haptic rendering algorithm. We validate our approach by conducting a user study to examine the utility of the mid-air haptic feedback. This work can be used to automatically create tactile virtual surfaces where the visual perception of texture roughness guides the design of mid-air haptic feedback.

A copy of the publication, 'Incorporating the Perception of Visual Roughness into the Design of Mid-Air Haptic Textures' from which this work is described in detail, can be found at: https://dl.acm.org/doi/10.1145/3385955.3407927.

The `TexNetModel.ipynb` notebook includes an overview of the TexNet architecture and the code to reproduce the prediction of perceptual visual roughness values for images with the Penn Haptic Texture Toolkit image database: https://repository.upenn.edu/meam_papers/299/.

The `DataProcessing.ipynb` notebook enables users to filter and create a data frame based on data obtained from an Amazon Mechanical Turk Visual Perception Rating task, that can be used as training data for the TexNet model. 

Additional to the Python notebooks are individual scripts that allows the TexNet model to be run from a command line window.

5 different textural dimensions can be predicted and output from the model. These are: 
1. Roughness 
2. Bumpiness 
3. Stickiness 
4. Hardness
5. Warmness

In addition, the model can be trained on different input features: texture images, grey-level co-occurence matrices, and Haralick features.

## Running the experiment
Either run Jupyter from within the root directory of the repository then select the appropriate `.ipynb` file, or run it directly from the command line using the `texnet_runner.py` script. Running from a terminal window requires a number of elements to be referenced. These are discussed in more detail below.

If you wish to run directly from the command line the following information will need to be referenced.
```python
# --file: .csv file containing perceptual AMT data. (obtained from DataProcessing.ipynb)
# --image_data: path to Penn Haptic Texture Toolkit image repository.
```

In addition, there are a number of arguments that can be passed to adjust training parameters.
```python
# --test_texture_list: If a specific list of test textures is required then these can be input here. The output file from 'DataProcessing.ipynb' must be queried to find image names.
# --predictor_variable: Texture dimension to predict. Select from 'roughness', 'bumpiness', 'hardness', 'stickiness', 'warmness'. Can be median or mean: 'mean_{dimension}'.
# --haralick_training_on/off: Set as 'on' to train on Haralick data. 'on' by default.
# --matrix_training_on/off: Set as 'on' to train on GLCM data. 'on' by default.
# --image_training: Set as 'on' to train on the texture images directly. 'on' by default.
# --epochs: set how many epochs you require the model to train over. Default is 150.
# --batch_size: set custom batch_size. Default is 1.
# --image_size: adjust size of images to train on. Default 256.
# --distances: type(List) Pass a list of integer distances that GLCMs should be calculated for.
# --angles: type(List) Pass a list of integer angles (degrees) that GLCMs should be calculated for.
```

##### texnet_runner.py
```
python texnet_runner.py --file "path/to/DataProcessing.ipynb .csv file output" --image_dir "path/to/Penn Haptic Texture Toolkit image database/*.bmp" --image_size 256 --test_texture_list 'denim_square' 'cork_square' 'plastic_mesh_2_square' 'brick_2_square' 'bubble_envelope_square' 'silk_1_square' 'paper_plate_2_square' 'metal_mesh_square' 'glitter_paper_square' --predictor_variable 'median_roughness' --epochs 150 --batch_size 1
```

## Support, Contact & Contribution
For any questions regarding this project, please contact david.beattie@ultraleap.com. Alternatively, please branch this repo and leave comments so we can keep up to date with any requests and issues.

## Acknowledgements
This project has received funding from the European Union’s Horizon 2020 research and innovation programme under grant agreement No 801413, H-Reality.

Images were obtained from the University of Pennsylvania's Haptic Texture Toolkit (HaTT). More information can be found here, as well the associated license for usage of this research texture database: https://repository.upenn.edu/meam_papers/299/

## Disclaimer

This is not an officially supported Ultraleap product.

## License

TexNet: Ultraleap Image Texture Prediction Model © Ultraleap Limited 2020

Licensed under the Ultraleap closed source licence agreement; you may not use this file except in compliance with the License.

A copy of this License is included with this download as a separate document.

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.