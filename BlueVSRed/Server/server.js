var net = require('net');

var HOST = 'localhost';
var PORT = 8888;
var sockets = [];
var readyConnections = 0;
var connectionCounter = 0;
var counter = 3;
var gameTime = 0;

var server = net.createServer(function(socket){

        // User attributes.
        socket.id = connectionCounter;
        socket.isReady = false;
        socket.name = "Unknown";
        socket.player = {};
        socket.player.position = {};
        socket.player.position.x = 0;
        socket.player.position.y = 1.5;
        socket.player.position.z = 0;
        socket.player.health = 100;
        socket.player.mana = 100;

        connectionCounter++;

        // Set encoding to utf8.
        socket.setEncoding('utf8');

        // Save socket, so you can access it later on.
        sockets.push(socket);

        var receivedData = "";

        console.log("User: " + socket.remoteAddress + " connected at port: " + socket.remotePort);

        socket.on('data', function(data){
                // Buffer the received data.
                receivedData += data;

                // Split the data into substrings.
                var subData = receivedData.split("\n");

                // Check if we read all the data.
                if(subData[subData.length - 1].length != 0)
                        receivedData = subData[subData.length - 1];
                else
                        receivedData = "";

                // Process the data.
                for(var i = 0; i < subData.length - 1; i++){
                        var message = subData[i];
                        taskCreator(message);
                        //console.log("Received: " + message);


                }
        });

        socket.on('close', function(data){
                console.log("User: " + socket.remoteAddress + " at port: " + socket.remotePort + " closed the connection.");
                if(socket.isReady){
                        socket.isReady = false;
                        readyConnections--;
                }
                // Remove socket from the array.
                var i = sockets.indexOf(socket);
        sockets.splice(i, 1);
                updateLobby();

                var removePlayer = new playerMessage(socket.id, socket.name, socket.player.health, socket.player.mana, socket.player.position, socket.teamblue);
                var removePlayerContainer = JSON.stringify(new messageContainer("remove", removePlayer)) + "\n";
                for(var s = 0; s < sockets.length; s++)
                        if(sockets[s] != socket)
                                sockets[s].write(removePlayerContainer);
        });

        socket.on('timeout', function(data)
        {
                console.log("timeout received: " + data);
        });

        socket.on('error', function(data)
        {
                console.log("error received: " + data);
        });

        function taskCreator(message){
                try{
                        var data = JSON.parse(message);
                        switch(data.messageType){
                                case "init":
                                        console.log("Initialize request received.");
                                        initializeUser(data.message.name);
                                        break;
                                case "lobby":
                                        console.log("Lobby info request received.");
                                        if(!socket.isReady && data.message.isReady){
                                                readyConnections++;
                                                socket.isReady = true;
                                        }
                                        else if(socket.isReady && !data.message.isReady && readyConnections > 0){
                                                readyConnections--;
                                                socket.isReady = false;
                                        }
                                        updateLobby();
                                        break;
                                case "initplayer":
                                        initializePlayer();
                                        break;
                                case "playerpath":
                                        syncPlayerPath(data.message.id, data.message.path);
                                        break;
								case "recall":
										console.log("Recall received");
										sendSkillInfo(data.message.id, "recall");
										break;
								case "attack":
										console.log("Attack received");
										sendSkillInfo(data.message.id, "attack");
										break;
								case "q":
										console.log("q received");
										sendSkillInfo(data.message.id, "q");
										break;
								case "w":
										console.log("w received");
										sendSkillInfo(data.message.id, "w");
										break;
								case "e":
										console.log("e received");
										sendSkillInfo(data.message.id, "e");
										break;
																				
                                default:
                                        console.log("Unknown messagetype.");
                        }
                }
                catch(e){
                        console.log(e);
                }
}

        function syncPlayerPath(id, path){
                for(var s = 0; s < sockets.length; s++){
                        if(sockets[s] != socket) {
                                var currentPath = new pathMessage(id, path);
                                var currentPathContainer = JSON.stringify(new messageContainer("playerpath", currentPath)) + "\n";
                                sockets[s].write(currentPathContainer);
                        }
                }
        }
		
		function sendSkillInfo(id, messageName){
			for(var s = 0; s < sockets.length; s++){
                        if(sockets[s] != socket) {
                                var message = new skillMessage(id);
                                var skillContainer = JSON.stringify(new messageContainer(messageName, message)) + "\n";
                                sockets[s].write(skillContainer);
                        }
                }
		}

        function initializePlayer(){
                // Send the player data to the user.
                var init = new playerMessage(socket.id, socket.name, socket.player.health, socket.player.mana, socket.player.position, socket.teamblue);
                var initContainer = JSON.stringify(new messageContainer("initplayer", init)) + "\n";
                socket.write(initContainer);

                for(var s = 0; s < sockets.length; s++){
                        if(sockets[s] != socket) {
                                // Send the player data to all other players.
                                        var opponentContainer = JSON.stringify(new messageContainer("initopponent", init)) + "\n";
                                        sockets[s].write(opponentContainer);
                                }
                                // Send all other players to the user.
                                if(sockets[s] != socket) {
                                        var otherPlayer = new playerMessage(sockets[s].id, sockets[s].name, sockets[s].player.health, sockets[s].player.mana, sockets[s].player.position, sockets[s].teamblue);
                                        var otherPlayerContainer = JSON.stringify(new messageContainer("initopponent", otherPlayer)) + "\n";
                                        socket.write(otherPlayerContainer);
                        }
                }
        }

        function updateLobby(){
                var lobby = new lobbyMessage(false, 3);
                var initContainer = JSON.stringify(new messageContainer("lobby", lobby)) + "\n";

                for(var i = 0; i < sockets.length; i++){
                        sockets[i].write(initContainer);
                }

                if(readyConnections != 0 && readyConnections == sockets.length){
                        setInterval(function(){

                        var lobby = new lobbyMessage(true, counter);
                        var initContainer = JSON.stringify(new messageContainer("lobby", lobby)) + "\n";

                        counter--;

                        for(var i = 0; i < sockets.length; i++){
                                sockets[i].write(initContainer);
                        }
                        console.log(counter);
                        if(counter < 0) {
                                counter = 0;
                                clearInterval(this);
                                setInterval(function(){
                                        var pastTime = new timeMessage();
                                        gameTime++;
                                        var timeContainer = JSON.stringify(new messageContainer("time", pastTime)) + "\n";
                                        for(var i = 0; i < sockets.length; i++){
                                                sockets[i].write(timeContainer);
                                        }
                                }, 1000);
                        }
                        }, 1000);
                }
        }

        function initializeUser(username){
                var teamblue;
                if(sockets.length % 2 == 0)
                        teamblue = false;
                else
                        teamblue = true;
                socket.name = username;
                socket.teamblue = teamblue;
                var init = new initMessage(socket.id, teamblue);
                var initContainer = JSON.stringify(new messageContainer("init", init)) + "\n";
                socket.write(initContainer);
        }


});


function messageContainer(messageType, message){
        this.messageType = messageType;
        this.message = message;
        this.timeInSeconds = Date.now() / 1000.0;
}

function timeMessage(){
        this.timeInSeconds = gameTime;
}

function initMessage(id, teamblue){
        this.id = id;
        this.teamblue = teamblue;
}

function lobbyMessage(start, countDown){
        this.canStart = start;
        this.connections = sockets.length;
        this.readyConnections = readyConnections;
        this.countDown = countDown;
}

function pathMessage(id, path){
        this.id = id;
        this.path = path;
}

function skillMessage(id){
	this.id = id;
}

function playerMessage(id, pname, health, mana, position, teamblue){
        this.id = id;
        this.pname = pname;
        this.health = health;
        this.mana = mana;
        this.position = position;
        this.teamblue = teamblue;
}

function position(x, y, z){
        this.x = x;
        this.y = y;
        this.z = z;
}

server.listen(PORT, HOST);

console.log("Server is listening on: " + HOST + ":" + PORT);


