<a name="readme-top"></a>

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/AndreasLrx/LaFlammeDivain">
    <img src="Assets/Icons/logo.png" alt="Logo" width="350" height="auto">
  </a>

<h3 align="center">La Flamme Divain</h3>

  <p align="center">
    <br />
    <br />
    <br />
  </p>
</div>

<!-- ABOUT THE PROJECT -->

## About The Project

Before starting the app, we have created a [Game Design Document](https://www.figma.com/file/Ppd6RoeR3Xs6KQeoFx8FqF/La-flamme-Divain?type=design&node-id=0%3A1&t=gJZTL7G03ZN2wieZ-1) to have a global idea of the app.

This is the end of year Epitech project in the Pre-MSc cursus. In this project, students decides their own project to show the skills they acquired during the year.

We decided to create a rogue-lite video game.

Main technical points:

 - Player moveset
 - Prodecural generation
 - Scripted AI (enemies)
 - Supervised AI

 <p align="right">(<a href="#readme-top">Back to the top</a>)</p>

---

### Built With

Why ? Because we want to discover new technologies in that project

- [![C#][csharp-icon]][csharp-url]
- [![Unity][unity-icon]][unity-url]
- [![Figma][figma-icon]][figma-url]
- [![Git-LFS][git-lfs-icon]][git-lfs-url]

<p align="right">(<a href="#readme-top">Back to the top</a>)</p>

---

<!-- GETTING STARTED -->

## Getting Started

To get a local copy up and running follow these simple steps.

### Prerequisites

You need to install Unity, Visual Studio (Code) and Git LFS

- Unity
  - [Download Unity](https://unity3d.com/get-unity/download)
- Visual Studio (Code)
  - [Download Visual Studio](https://visualstudio.microsoft.com/downloads/)
  - [Download Visual Studio Code](https://code.visualstudio.com/download)
- Git LFS
  - [Download Git LFS](https://git-lfs.github.com)

### Installation

1. Install Unity version 2022.3.2f1
2. Install Visual Studio (or Visual Studio Code)
3. Install Git LFS

    ```sh
    curl -fsSL https://bun.sh/install | bash
    git lfs install
    ```

4. Clone the repo and check that git LFS elements are downloaded

   ```sh
    git clone git@github.com:AndreasLrx/LaFlammeDivain.git
    cd LaFlammeDivain
    git lfs checkout
    git lfs pull
   ```

5. Open the project with Unity
6. Set the external script editor to Visual Studio (or Visual Studio Code)
7. Open the project with Visual Studio (or Visual Studio Code)
8. Check that `com.unity.ide.visualstudio` is in the `Packages/manifest.json`, with the right version (2.0.20), and that the`com.unity.ide.vscode` is not in the `Packages/manifest.json` (the latter is deprecated, and both work with Visual Studio Code)
9. Open the `Menu` scene
10. Play the game, develop and enjoy !

### Build a live version (optional)

You can play a live build by selecting file->Build And Run, select a directory and wait for the build to finish.

<p align="right">(<a href="#readme-top">Back to the top</a>)</p>

<!-- ACKNOWLEDGMENTS -->

## Acknowledgments

Some useful links we used during the project and would like to give credit to.

- [Markdown badges](https://github.com/Ileriayo/markdown-badges)

<p align="right">(<a href="#readme-top">Back to the top</a>)</p>

---

# Thank you for reading this README

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->

[unity-icon]: https://img.shields.io/badge/unity-%23000000.svg?style=for-the-badge&logo=unity&logoColor=white
[unity-url]: https://unity.com
[csharp-icon]: https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white
[csharp-url]: https://learn.microsoft.com/dotnet/csharp/
[figma-icon]: https://img.shields.io/badge/figma-%23F24E1E.svg?style=for-the-badge&logo=figma&logoColor=white
[figma-url]: https://www.figma.com
[git-lfs-icon]: https://img.shields.io/badge/git-lfs-%23F05032.svg?style=for-the-badge&logo=git-lfs&logoColor=white
[git-lfs-url]: https://git-lfs.github.com
