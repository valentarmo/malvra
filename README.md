# Malvra

HoloLens app that aims to help in several aims to help in floriculture tasks, 
it's made in unity using [MRTK](https://github.com/microsoft/MixedRealityToolkit-Unity).

The current app's functionality consist on building a virtual mesh that indicates
the spots where a plant should be planted according to its type. The mesh creation
algorithm was made with circle packing in mind, which results in an almost optimal
distribution of planting spots, increasing the amount of plants per square meter.

### Deploy
#### Requirements 
- Unity
- Visual Studio IDE with C++ and UWP components
- HoloLens Device or HoloLens Emulator

#### Unity Build
Open the project in unity, go to `File > Build` Settings and change to Universal
Windows Platform. Set the following configuration:

* Target device -> Any device
* Architecture -> x86
* Build Type -> D3D
* Target SDK -> Latest installed
* Minimum platform version -> 10.0.10240.0
* Visual Studio Version -> Latest installed
* Build and Run on -> Local Machine
* Build configuration -> Release
* Checkboxes -> Unchecked
* Compression Method -> Default

Now click on Build

#### Deploy on the emulator
Open the generated Visual Studio solution, pick the emulator from the debugging
dropdown menu and deploy

#### Deploy to a HoloLens device
Open the generated Visual Studio solution, pick remote machine from the debugging
dropdown menu, now go to project properties, and go to the debugging seccion,
once there put your device's IP in the field labeled remote machine name. Deploy.

You can find your HoloLens IP under `Settings > Networking > Hardware properties`
