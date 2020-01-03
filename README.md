# Tank Game for AI Agents
This project is a Unity game about tanks shooting each other. The enemy tanks are AI agents, navigating the map, and trying to kill the player. 

Each tank moves using A* and is controlled using a state machine that decides which behaviors to tran-sition into based  on  some  input. This input is provided by  a  state-keeping scriptrunning on each tank that gathers information such as ”do I have a target” and ”canI fire at my target”. The state machine then uses that information to transition intothe appropriate behavior. Each of these behaviors are isolated scripts that only talksto the script controlling the tank, the pathfinding script, and the state-keeping scriptmentioned earlier that gathers information.  

### There are currently four states:  
- **Roam behavior** where the tank randomly selects a tile that it hasn’t been torecently in order to explore the map. In the case that it is surrounded by tilesit has recently been to, it selects the oldest one in the list.
- **Chase behavior** when it has seen a target and has a last know location it cango to, it runs pathfinding to get there (see fig 4, the yellow sphere marks thelast known location of the target). If it reaches the last know location withoutspotting its target, it returns to the roam behavior.
- **Fire behavior** when the tank has line of sight to its target and is within firingrange, the tank stops moving, aims at the target, and fires until the target isdead.
- **Escape behavior** when the tank has low health, it will start running away fromits target.  As all tanks heal over time, this option can function as anotherstrategy. When it has healed some, it will continue as normal.

