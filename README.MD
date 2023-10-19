# ParallelOrigin Client

The c# unity client of parallel origin.
Targets android-devices and provides in editor capabilities. 

## Run locally
### Requirements
- Unity 2021.3.9f1 with Android-Module
- Android SDK & Runtimes (In case you wanna build the project) 
- .Net 

### Preparation
- cd into `/ParallelOriginClient/Assets/Scripts/Core`
- `git submodule update --init --recursive`

### Running
- Open project in unity
- Switch build platform to Android
- Search for the `Main`-Script on the `Main`-GameObject
- Change IP-Address and Port to your local `ParallelOriginGameServer`-Instance
- Click run in the editor
- Login



