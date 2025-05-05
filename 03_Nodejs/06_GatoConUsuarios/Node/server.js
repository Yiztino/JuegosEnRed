const WebSocket = require('ws');

const users = [];
const webSocketPort = 8080;
const partidasPrivadas = {};
const usuariosPersistentes = {};
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
        this.actual = 1;
        this.round = 0;
        this.score1 = 0;
        this.score2 = 0;
        this.winner = 0;
       
    }

    resetBoard(){
        this.board = [0, 0, 0, 0, 0, 0, 0, 0, 0];
        
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

    turn(playerNum, pos) {
        if(this.winner !== 0) return `error|Ya terminó la ronda. Ganó el jugador ${this.winner}`;
        if (playerNum === 0) return "error|jugador es 0";
        if (playerNum !== this.actual) return "error|no es tu turno";
        if (pos < 0 || pos >= 9) return "error|posición inválida";
        if (this.board[pos] !== 0) return "error|posición ocupada";

        this.board[pos] = playerNum;
       
        let winner = this.isWin();

        if (winner > 0) {
            this.winner = winner;
            winner === 1 ? this.score1++ : this.score2++;
            this.round++;
            this.actual = winner;
            
            return `win|${winner}`;
        }
         this.actual = this.actual === 1 ? 2 : 1;
        return "turnFinished|Continue";
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
        /*
        esto sirve en lugar de lo ed arriba?
        for (let [a, b, c] of wins) {
            if (this.board[a] && this.board[a] === this.board[b] && this.board[b] === this.board[c]) {
                return this.board[a];
            }
        }
        return 0;
        */
    }
}


//////////////REFERENTE AL WEB SOCKET/////////////////////


class User{
	constructor()
	{
		this._username="none";
		this._conn = null;
        this._playingState = false;
        this._invitedBy = null;
        this._partidaID = null;
	}

	set username( user ) { this._username = user; }
	set connection( con ) { this._conn = con; }
    set playingState ( state ) { this._playingState = state; }
    set invitedBy( user ) { this._invitedBy = user; }
    set partidaID ( id ) { this._partidaID = id; }

    get username () { return this._username; }
	get connection () { return this._conn; }
    get partidaID () { return this._partidaID; }
    get playingState () { return this._playingState; }
    get invitedBy () { return this._invitedBy; }

    static findClientByUsername(lst, username) {
        return lst.find(user => user.username === username) || null;
    }
}
//Crea servidor web
const wss = new WebSocket.Server({ port: webSocketPort },()=>{
    console.log(`Web Socket Server Started on port ${webSocketPort}`);
});

wss.on('connection', function connection(ws) {
	
	console.log('New connenction');

    const user = new User();
	user.connection = ws;
	users.push(user);
    
    // ws.on('open', (data) => {
	// 	console.log('Now Open');
	// });

	ws.on("message", function incoming(data){
    
        console.log('Message received: ',data.toString());
        //let response;
        let info = data.toString().split('|');
        let partida;

        switch (info[0]) {

            case "init":
                partida = partidasPrivadas[user.partidaID]
                if(partida){
                    partida.init();
                    broadcastToMatch(user.partidaID, "data|"+JSON.stringify(partida.getStatus()));
                }
                // response = partida.getStatus();
                // user.connection.send(JSON.stringify(response))
                
                break;
            case "status":
                
                partida = partidasPrivadas[user.partidaID]
                if(partida){
                    response = partida.getStatus();
                    user.connection.send("data|"+JSON.stringify(response))
                }
                
                break;
            case "resetBoard":
                
                partida = partidasPrivadas[user.partidaID];
                if (partida) {
                    partida.resetBoard();
                    partida.winner = 0;
                    partida.actual = 1;
                    broadcastToMatch(user.partidaID, "data|" + JSON.stringify(partida.getStatus()));
                }
                break;
            case "turn":
                
                partida = partidasPrivadas[user.partidaID];
                if(!partida){
                    user.connection.send("error|No estás en una partida");
                    break;
                }

                const playerNum = (partida.p1 === user.username) ? 1 : 2;
                const position = parseInt(info[1]) - 1;
                const result = partida.turn(playerNum, position)
                 
                if(result.startsWith("error|")){
                    user.connection.send(result);
                    return;
                }
                
                response = partida.getStatus();
                broadcastToMatch(user.partidaID, "data|"+JSON.stringify(response));
                
                break;
            case "updateUsername": 
                const newUsername = info[1];
                
                if (User.findClientByUsername(users, newUsername)) {
                    user.connection.send("message|Username already in use|");
                    break;
                }
                
                

                if (usuariosPersistentes[newUsername]) {
                    const dataPersistente = usuariosPersistentes[newUsername];
                    const partida = dataPersistente.partida;

                    if (partida !== null && !User.findClientByUsername(users, newUsername)) {
                        user.username = newUsername;
                        user.partidaID = dataPersistente.partidaID;
                        partidasPrivadas[user.partidaID] = partida;

                        
                        const oponente = (partida.p1 === user.username) ? partida.p2 : partida.p1;
                        const numJugador = (partida.p1 === user.username) ? 1 : 2;

                        
                        //user.connection.send("data|" + JSON.stringify(partida.getStatus()));

                        user.connection.send("message|Reconnected to your game|");
                        user.connection.send(`START_GAME|${oponente}|${numJugador}`);
                        user.connection.send("data|" + JSON.stringify(dataPersistente.partida.getStatus()));
                        break;
                    }
                }

                user.username = newUsername;
                usuariosPersistentes[newUsername] = {
                    partidaID: null,
                    partida: null
                };
                user.connection.send("message|Username updated|");
                break;

                // if (User.findClientByUsername(users, newUsername)) {
                //     user.connection.send("message|Username already in use|");
                // } else {
                //     user.username = newUsername;
                //     user.connection.send("message|Username updated|");
                // }
                //break;
            case 'getUsersList':
                const lista = users
                    .filter(u =>
                        u.username !== "none" &&
                        u.username !== user.username &&
                        u.connection.readyState === WebSocket.OPEN &&
                        !u.playingState)// no se muestra si estan en partida
                    .map(u => u.username);
                user.connection.send(`usersList|${JSON.stringify({ users: lista })}`);
                break;
                
            
            case 'sendGameInvite': 
                
                const targetUser = User.findClientByUsername(users, info[1]);
                if(targetUser && targetUser.connection.readyState === WebSocket.OPEN) {
                    targetUser.invitedBy = user.username;
                    targetUser.connection.send("invite|"+user.username);

                } else {
                     user.connection.send("error|User: " + info[1] + " not found");
                }
              
                break;
             case 'gameInviteResponse':

                if (!user.invitedBy) {
                    user.connection.send("error|No tienes invitador registrado");
                    break;
                }
               
                const inviterUser = User.findClientByUsername(users, user.invitedBy);

                

                if(!inviterUser){
                    user.connection.send("error|Inviter not found|"+ user.invitedBy);
                    break;
                };

                if (info[1] === "yes") {
                    const partidaID = `${inviterUser.username}_vs_${user.username}`;
        
                    const nuevaPartida = new Gato();

                    if (Math.random() < 0.5) {
                        nuevaPartida.p1 = inviterUser.username;
                        nuevaPartida.p2 = user.username;
                    } else {
                        nuevaPartida.p1 = user.username;
                        nuevaPartida.p2 = inviterUser.username;
                    }
                    nuevaPartida.init();
                    partidasPrivadas[partidaID] = nuevaPartida;
                    
                    
                    
                    inviterUser.playingState = true;
                    user.playingState = true;
        
                    inviterUser.partidaID = partidaID;
                    user.partidaID = partidaID;
                    //PAR LO DE REGRESAR A PARTIDA
                    usuariosPersistentes[inviterUser.username] = {
                        partidaID: partidaID,
                        partida: nuevaPartida
                    };

                    usuariosPersistentes[user.username] = {
                        partidaID: partidaID,
                        partida: nuevaPartida
                    };

                    inviterUser.connection.send(`START_GAME|${user.username}|${(nuevaPartida.p1 === inviterUser.username) ? 1 : 2}`);
                    user.connection.send(`START_GAME|${inviterUser.username}|${(nuevaPartida.p1 === user.username) ? 1 : 2}`);
                    inviterUser.connection.send("data|" + JSON.stringify(nuevaPartida.getStatus()));
                    user.connection.send("data|" + JSON.stringify(nuevaPartida.getStatus()));

                    //inviterUser.connection.send(`START_GAME|${user.username}|${}`);
                    //user.connection.send(`START_GAME|${inviterUser.username}`);
                } else if (info[1] === "no") {
                    
                    inviterUser.connection.send(`REJECTED|${user.username}`);
                    user.connection.send(`message|You rejected the invitation from ${inviterUser.username}`);
                }
                user.invitedBy = null;
                break;
            default:
                user.connection.send("error|Comando no reconocido");
                break;
            }
    });
    
    ws.on("close", function() {
        console.log(`Connection closed: ${user.username}`);
        // users.splice(users.indexOf(user))
        const index = users.indexOf(user);
        if(index !== -1){users.splice(index,1);}
       
    });
});

wss.on('listening',()=>{
   console.log('Now listening on port 8080...');
});

function broadcastToMatch(partidaID, message) {
    users.forEach(us => {
        if (us.partidaID === partidaID && us.connection.readyState === WebSocket.OPEN) {
            us.connection.send(message);
        }
    });
}