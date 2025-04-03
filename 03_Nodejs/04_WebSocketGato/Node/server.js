const WebSocket = require('ws');


class Gato{
    constructor(){
        this.board = [0, 0, 0, 0, 0, 0, 0, 0, 0];
        this.p1 = "id1";
        this.p2 = "id2";
        this.actual = 1;
        this.round = 0;
        this.score1 = 0;
        this.score2 = 0;
        this.winner = 0;
    }

    init(){
        this.board = [0, 0, 0, 0, 0, 0, 0, 0, 0];
        this.p1 = "id1";
        this.p2 = "id2";
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

const game = new Gato();

//REFERENTE AL WEB SOCKET

const clients = [];
const wss = new WebSocket.Server({ port: 8080 },()=>{
    console.log('Server Started on port 8080');
});

wss.on('connection', function connection(ws) {
	
	console.log('New connenction');
    
    clients.push(ws);// Agregar la conexión (cliente) a la lista

	// let cliente = new Cliente ();
	
    ws.on('open', (data) => {
		console.log('Now Open');
	});

	ws.on("message", (message) => {
        const [command, player, pos] = message.toString().split(":");
        let response;
        switch (command) {
            case "init":
                game.init();
                response = game.getStatus();
                break;
            case "status":
                response = game.getStatus();
                break;
            case "turn":
                const playerNum = parseInt(player);
                // const position = parseInt(pos) - 1;
                const position = parseInt(pos) - 1;
                game.turn(playerNum, position);
                
                response = game.getStatus();
                clients.forEach(client => {
                    if (client.readyState === WebSocket.OPEN) {
                        client.send(JSON.stringify(response)); // Aquí se envía a todos los clientes
                    }
                });
                break;
            default:
                response = { error: "Comando no válido" };
                break;
        }

        clients.forEach(client => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(JSON.stringify(response));
            }
        });
    });
    
    ws.on("close", () => {
        let index = clients.indexOf(ws);
        if (index > -1) {
            clients.splice(index, 1);
        }
        console.log("User disconnected");
    });
		// wss.clients.forEach(client => {
        //     if (client.readyState === WebSocket.OPEN) {
        //         client.send(JSON.stringify(response));
        //     }
        // });
		//ws.send("The server response: "+data); // Para mandar el mensaje al cliente que lo envió

		// let info = data.toString().split('|');

		// switch (info[0])
		// {
		// 	case '200':
		// 		cliente.username = info[1];
		// 		ws.send("UserName upDated: "+cliente.username);
		// 		break;
			
		// 		default:
		// 			// Mandar a todos los clientes conectados el mensaje con el username de quien lo envió
		// 			clients.forEach(client => {
		// 				if(client.readyState === WebSocket.OPEN)
		// 				{
		// 					client.send(cliente.username + " says: " + data); // si falla, cambiar a: `data.toString()`
		// 				}
		// 			});
		// 			break;
		// }
	//});

	// Al cerrar la conexión, quitar de la lista de clientes
	// ws.on('close', () => { 
	// 	let index = clients.indexOf(ws);
	// 	if(index > -1)
	// 	{
	// 		clients.splice(index, 1);
	// 		ws.send("UserName disconnected");
	// 	}
	// });
});

wss.on('listening',()=>{
   console.log('Now listening on port 8080...');
});