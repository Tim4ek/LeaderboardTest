# LeaderboardTest

How works

- Start from App.cs script.

- AppContex.cs is the main container for references that are injected into different classes.

- On startup, the lobby asset is asynchronously initialized and the button animation is played. Animation made with the help of DOTween.

- Lobby view and popup has different canvases, as an optimization.

- Also, for optimization, an atlas with common textures for buttons and popups was created.

- Lobby and popup prefabs are also located in different groups in addressables.
