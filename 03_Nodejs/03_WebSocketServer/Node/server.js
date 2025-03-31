const WebSocket  = require('ws');
const clients = [];
const wss = new WebSocket.Server({ port: 8080 }, ()=> {
    console.log('Server Started');
})

wss.on('connection', function connection(ws){
    console.log('Se conectó un cliente');

    ws.on('open', (data) => {
        console.log('New Connection');
    });


    ws.on('message', (data) =>{
        console.log('Data received: %s', data);
        //ws.send("The server respones: "+data);

        clients.forEach(client => {
            if(client.readyState === WebSocket.OPEN)
            {
                client.send(data);
            }
        });
    });
});