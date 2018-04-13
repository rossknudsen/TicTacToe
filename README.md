# README #

This is the source code for my university assignment to construct a webserver using raw sockets.  The
architecture must include a webserver and an application server (in this case a game server).  

### How do I get set up? ###

You should be able to download the source code and run it without any configuration using Visual Studio
2017.  Earlier versions may work too, but have not been tested.

The webserver listens for connections on http://localhost:8080 and the game server listens on http://localhost:8081
so make sure these ports are available.  The Visual Studio solution should be set to start both the webserver and gameserver
projects at the same time (multiple startup projects) but please check this on your system.  Then you can navigate to 
http://localhost:8080/index.html to begin the game.

The source is currently Copyright to Ross Knudsen - this may change in the future.
