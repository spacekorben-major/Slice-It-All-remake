# Slice it all - remake

### EXERCISE

>Reproduce the core gameplay of Slice It All! using Unity3D.

>Give it a creative twist from the original game by adding or mixing another game mechanic in the core gameplay.
>Don’t implement side features (for example: skin shop, leaderboard, fake ads...).

>Original game:

>https://play.google.com/store/apps/details?id=com.tummygames.sliceit

>https://youtu.be/iavcunJln-A

------------
### Overview 

I tried the game and decided to add multiplayer to it.

The structure is pretty simple:

* I am using VContainer for dependency injection.
* As soon as the scene starts, main DI container instantiates all core game systems.

* Multiplayer solution is based on Unity Relay, Unity Network for GameObjects and Unity Lobbies. First, each client tries to fetch all hosted lobbies. If there's any, it joins, if there's none it hosts one. After hosting a lobby client puts his unity relay key to the lobby data. Anyone who joins the lobby connects to the host using relay.

* I use a custom made SignalBus that uses delegates to dispatch events. 
This approach has its own problems, but I find it usefull for small projects.

---------------------

### Gameplay:

It's a hypercasual game. You control a short sword and you need to cut as much fruits as you can. You can see your opponent also doing their best to score more then you. The player who scored more wins!

![ezgif com-video-to-gif](https://user-images.githubusercontent.com/69351628/230787126-d901b516-08d0-450a-bb02-22651e99d37c.gif)

Notice! FPS is low due to this being a 12-fps GIF :) Its fine in the game.

--------------------

### Known issues

* The camera is really basic and need more polish;
* The game has no sound;
* There's only one 10-second level and no level generation;
* There's no AI for bots if you can't find an opponent;
* There's no disconnection handling. If you opponent disconnects, everything breaks and you need to restart the app;
* Mesh cutting algorithm is really basic, especially the "filler part", If this were a real project I would need to improve it before adding more complex shapes;

---------------

### Some thoughts

The first mesh cut always produced a huge spike on the device. Even for simple cube it took about 80ms to cut it. After some more profiling on the device I discovered that most of that time is spent on JIT compilation for the cutting algorythm! There's nothing I can do! Or can I...

I added a new static class called MeshCutPreHeater wich creates a mesh with a single triangle in it and then triggers a cutting service to cut it. It takes about 3-4 ms on a device for a single triangle! Huge, but not noticable for the user. And I also do it during the inital loading. All subsequent cuts take no more then 1ms to complete, so that solves it.

-------------------

The project is tested and guaranteed to work on the following devices:

* OnePlus 10 Pro
* Xiaomi mi9t
* Samsung S20FE
* Galaxy A32
* Galaxy A60

