# MultiPlayer Zombaland

A multiplayer zombie game built with Unity and **Photon PUN 2** networking.

## Prerequisites

Before cloning and setting up this project, ensure you have the following installed:

- **Unity** (version specified in your project - check `ProjectSettings/ProjectVersion.txt`)
- **Git**
- **.NET Framework** (for C# development)
- **Visual Studio** (recommended for C# editing)

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd MultiPlayerZombaland
```

### 2. Open in Unity

1. Open **Unity Hub**
2. Click **"Add"** and select the `MultiPlayerZombaland` folder
3. Open the project with the appropriate Unity version

### 3. Install Photon PUN 2

This project uses **Photon PUN 2** for multiplayer networking.

- The Photon PUN 2 plugin should be included in the project
- If not already configured, import it from the Asset Store or via the Photon setup wizard
- Configure your **Photon AppID**:
  1. Go to `Window` → `Photon PUN 2 Settings`
  2. Enter your AppID (obtain from [photonengine.com](https://www.photonengine.com))

### 4. Load the Demo Scene

1. Navigate to `Assets/Scenes` (or your project's scenes folder)
2. Open the main demo scene to test multiplayer functionality

## Project Structure

```
MultiPlayerZombaland/
├── Assets/
│   ├── Scenes/           # Game scenes
│   ├── Scripts/          # C# scripts
│   ├── Prefabs/          # Reusable game objects
│   ├── Resources/        # Runtime-loaded assets
│   └── Plugins/          # Photon PUN 2 and dependencies
├── Assembly-CSharp.csproj
├── PhotonUnityNetworking.csproj
└── README.md
```

## Development Workflow

1. **Create a feature branch**: `git checkout -b feature/your-feature-name`
2. **Make your changes** in Unity or Visual Studio
3. **Test locally** (including multiplayer testing with Photon)
4. **Commit and push** your changes
5. **Create a pull request** for review

## Photon PUN 2 Basics

- **PhotonNetwork**: Main class for network operations
- **IPunObservable**: Implement this for networked object synchronization
- **OnPhotonSerializeView**: Handle networked data serialization
- Refer to [Photon PUN 2 Documentation](https://doc.photonengine.com/en-us/pun/current/getting-started/pun-intro)

## Troubleshooting

- **Photon connection issues**: Check your AppID in Photon settings
- **Missing scripts**: Ensure all `.csproj` files are properly loaded
- **Build errors**: Rebuild the solution in Visual Studio if needed

## Support

For Photon PUN 2 support, visit: https://www.photonengine.com/

---

ShaKk add cheating feature and test the whole finding for bug
Thiện fix bug and design new zombie
Kiệt fix zombie nav mesh and code script for new zombie

Happy developing! 🧟‍♂️