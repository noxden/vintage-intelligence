This package currently is only tested for a Quest2 build, so this setup explains how to get it running for a Quest2 only. If you want to make it work for Vive etc, you may have to make your own XR Player rig.
But this is independent from the Multiplayer parts of this application. Just make sure that the XRGrabInteractableNetwork script and the NetworkPlayer script works with your XRRig setup of choice.


There are some dependencies that you should install for the scene to work without errors:

- Photon Voice 2 (which includes Photon PUN 2)
	(https://assetstore.unity.com/packages/tools/audio/photon-voice-2-130518)
- XR Interaction Toolkit and its Samples Starter Assets
	(Add by name 'com.unity.xr.interaction.toolkit' in Package Manager, then import Samples Starter Assets as well)
- XR Plugin Management
	(Add from Unity Registry in Package Manager)

In Edit> Project Settings> XR Plug-in Management set the Plug-in Provider to Oculus in all tabs.
In File> Build Settings, switch the Platform to Android.

The Floor object needs to be set to the layer Teleport (which should be on layer6 ideally).

Don't forget to fill out the server settings of Photon (Window> Photon Unity Networking> Highlight Server Settings). It needs an App Id PUN and App Id Voice. 
If you want to build the app, make sure to change your player setting as per usual (like setting the scripting backend to IL2CPP etc).


For any more questions ask Krista :)