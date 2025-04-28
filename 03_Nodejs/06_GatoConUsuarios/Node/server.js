const WebSocket = require('ws');

const users = [];
const webSocketPort = 8080;
const partidasPrivadas = {};
class Gato{
    constructor(){
        this.board = [0, 0, 0, 0, 0, 0, 0, 0, 0];
        this.p1 = "";
        this.p2 = "";
        this.actual = 1;
        this.round = 0;
        this.score1 = 0;
        this.score2 = 0;
        this.winner = 0;
    }

    init(){
        this.board = [0, 0, 0, 0, 0, 0, 0, 0, 0];
        this.p1 = "";
        this.p2 = "";
        this.actual = 1;
        this.round = 0;
        this.score1 = 0;
        this.score2 = 0;
        this.winner = 0;
       
    }

    getStatus(){
        return{
            board: this.board,
            actual: this.actual,
            round: this.round,
            score1: this.score1,
            score2: this.score2,
            winner: this.winner
            
        }
    }

    turn(player, pos) {
       
        // let player = this.getPlayer(id);
        if (player === 0) return "error: jugador es 0";
        if (player !== this.actual) return "error: no es tu turno";
        if (pos < 0 || pos >= 9) return "error: posición inválida";
        if (this.board[pos] !== 0) return "error: posición ocupada";

        this.board[pos] = player;
        this.actual = this.actual === 1 ? 2 : 1;

        let winner = this.isWin();
        if (winner > 0) {
            this.winner = winner;
            winner === 1 ? this.score1++ : this.score2++;
            this.round++;
            
            return `Ganó el jugador ${winner}`;
        }
        return "OK, sigue el juego";
    }
    isWin() {
        const winPatterns = [
            [0, 1, 2], [3, 4, 5], [6, 7, 8],
            [0, 3, 6], [1, 4, 7], [2, 5, 8],
            [0, 4, 8], [2, 4, 6]
        ];
        for (let pattern of winPatterns) {
            if (this.board[pattern[0]] !== 0 &&
                this.board[pattern[0]] === this.board[pattern[1]] &&
                this.board[pattern[1]] === this.board[pattern[2]]) {
                return this.board[pattern[0]];
            }
        }
        return 0;
    }
}

//const game = new Gato();

//REFERENTE AL WEB SOCKET


class User{
	constructor()
	{
		this._username="none";
		this._conn = null;
        this._playingState = false;
        this._invitedBy = null;
       // this._inviting = null;
        this._partidaID = null;
	}

	set username( user )
	{
		this._username = user;
	}

	set connection( con )
	{
		this._conn = con;
	}

    set playingState ( state )
    {
        this._playingState = state;
    }
    set invitedBy( user )
	{
		this._invitedBy = user;
	}

    // set inviting ( inviting )
    // {
    //     this._inviting = inviting;
    // }
    set partidaID ( id )
    {
        this._partidaID = id;
    }

	get username ()
	{
		return this._username;
	}
	get connection ()
	{
		return this._conn;
	}
    get partidaID ()
    {
        return this._partidaID;
    }
    get playingState ()
    {
        return this._playingState
    }
    get invitedBy ()
	{
		return this._invitedBy;
	}
	// get inviting ()
	// {
	// 	return this._inviting;
	// }

	// static findClientByUsername (lst, username)
	// {
	// 	lst.forEach(user => {
	// 		if(user.username === username)
	// 		{
	// 			return user;
	// 		}
	// 	});
	// 	return null;
	// }
    static findClientByUsername(lst, username) {
        return lst.find(user => user.username === username) || null;
    }
}

const wss = new WebSocket.Server({ port: webSocketPort },()=>{
    console.log('Web Socket Server Started on port 8080');
});

wss.on('connection', function connection(ws) {
	
	console.log('New connenction');

    let user = new User();
	user.connection = ws;
	users.push(user);
    //clients.push(ws);// Agregar la conexión (cliente) a la lista

	// let cliente = new Cliente ();
	
    ws.on('open', (data) => {
		console.log('Now Open');
	});

	ws.on("message", (data) => {
        console.log('Message received: %s',data);
        // const [command, player, pos] = message.toString().split(":");
        let response;
        let info = data.toString().split('|');
        let partida;

        switch (info[0]) {

            case "init":
                partida = partidasPrivadas[user.partidaID]
                partida.init();
                response = partida.getStatus();
                user.connection.send(JSON.stringify(response))
                
                break;
            case "status":
                partida = partidasPrivadas[user.partidaID]
                response = partida.getStatus();
                user.connection.send(JSON.stringify(response))
                break;
            case "turn":
                
                partida = partidasPrivadas[user.partidaID]
                if(!partida){user.connection.send("No estás en una partida");
                    break;
                }

                //const playerNum = parseInt(info[1]);
                const playerNum = (partida.p1 === user.username) ? 1 : 2;
                //const position = parseInt(info[2]) - 1;
                const position = parseInt(info[1]) - 1;
                partida.turn(playerNum, position);
                
                response = partida.getStatus();

                users.forEach(us => {
                    if ( us.partidaID === user.partidaID && us.connection.readyState === WebSocket.OPEN) {
                        us.connection.send(JSON.stringify(response)); // Aquí se envía a todos los clientes
                    }
                });
                break;
            case "updateUsername": 
                const newUsername = info[1];
                if (User.findClientByUsername(users, newUsername)) {
                    user.connection.send("message|Username already in use|");
                } else {
                    user.username = newUsername;
                    // let json = '{"message": "Username updated to' + user.username + '"}';
                    // user.connection.send(json);
                   user.connection.send("message|Username updated|");
                }
                break;
                
                // u=true;
                // users.forEach(us => {
                //     if(us.username === info[1]){
                //         u = false;
                //         user.connection.send("El nombre de usuario ya está ocupado")
                //     }
                // });
                // if(u == true){
                //     user.username = info[1];
                // }
                
                // user.connection.send("Username upDated: "+user.username);
                // break;
            case 'getUsersList':
                let lista = [];
                users.forEach(us => {
                    if(us.connection.readyState === WebSocket.OPEN)
                    {
                        if(!(us.username === "none" && user.username)){

                            lista.push(us.username);
                            
                        }
                        //lista = lista + us.username;
                        //us.send(cliente.username + " says: " + data); // si falla, cambiar a: `data.toString()`
                    }
                });
                let json = 'usersList|{"users":'+JSON.stringify(lista)+"}";
                user.connection.send(json);
                //response = lista;
                break;
            
            case 'sendGameInvite': 
                // u = false;
                let invitingUser = users.find(us => us.username === info[1]);
                if(invitingUser && invitingUser.connection.readyState === WebSocket.OPEN) {
                    invitingUser.invitedBy = user.username;
                    //user.inviting = invitingUser;
                    invitingUser.connection.send("message|Invitation Received|"+user.username);

                } else {
                     user.connection.send("User: " + info[1] + " not found");
                }
                // users.forEach(us => {
                //     if(us.username === info[1])
                //     {  
                //         u=true;
                //         user.inviting = us;
                //         us.invitedBy = user;
                //         us.connection.send("El usuario: "+user.username+" manda invitacion de juego" + inviter)
                        
                //     }
                // })
                // if(u == false){
                //     user.connection.send()
                // }
                break;
                case 'gameInviteResponse':

                    const inviterUsername = user.invitedBy;
                    const inviterUser = User.findClientByUsername(users, inviterUsername);

                    if(!inviterUser){
                        user.connection.send("Inviter not found");
                        break;
                    };

                    if (info[1] === "yes") {
                        const partidaID = inviterUser.username + "_vs_" + user.username;
            
                        const nuevaPartida = new Gato();
                        let playerTurn;
                        if (Math.random() < 0.5) {
                            nuevaPartida.p1 = inviterUser.username;
                            playerTurn = 1;
                            nuevaPartida.p2 = user.username;
                        } else {
                            nuevaPartida.p1 = user.username;
                            playerTurn = 2;
                            nuevaPartida.p2 = inviterUser.username;
                        }
                        nuevaPartida.init();
                        partidasPrivadas[partidaID] = nuevaPartida;
                        
                        
                        
                        inviterUser.playingState = true;
                        user.playingState = true;
            
                        inviterUser.partidaID = partidaID;
                        user.partidaID = partidaID;
                        if(playerTurn == 1){
                            inviterUser.connection.send(`START_GAME|${user.username}|${1}`);
                        }
                        //inviterUser.connection.send(`START_GAME|${user.username}|${}`);
                        user.connection.send(`START_GAME|${inviterUser.username}`);
                        
                        user.invitedBy = null;
                        //inviter.inviting = null;
                        
                    } else if (info[1] === "no") {
                        
                        inviterUser.connection.send(`REJECTED|${user.username}`);
                        user.connection.send(`You rejected the invitation from ${inviter.username}`);
                        
                        
                        
                    }
                    user.invitedBy = null;
                       
                    break;
            case '404': 
                break;
            default:
                // response = { error: "Comando no válido" };
                // break;
                // Mandar a todos los clientes conectados el mensaje con el username de quien lo envió
				users.forEach(us => {
					if(us.readyState === WebSocket.OPEN)
					{
						us.send(us.username + " says: " + data); // si falla, cambiar a: `data.toString()`
					}
				});
				break;
        }

        users.forEach(us => {
            if (us.readyState === WebSocket.OPEN) {
                us.send(JSON.stringify(response));
            }
        });
    });
    
    ws.on("close", () => {
        let index = users.indexOf(ws);
        if (index > -1)
        {
            user.username = "none";
            users.splice(index, 1);
            user.connection.send("User: "+user.username+ " disconnected");
        }
    });
});

wss.on('listening',()=>{
   console.log('Now listening on port 8080...');
});