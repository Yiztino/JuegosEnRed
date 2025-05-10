# CODIGOS

## INIT

- Recibe un mensaje del tipo "init|<ID>".

- Busca una partida privada con ese ID.

- Si existe, la inicializa.

- Notifica a todos los jugadores de esa partida sobre el estado actual.

## STATUS

- Recibe un mensaje del tipo "status|<ID>".

- Busca una partida privada con ese ID.

- Si existe, obtiene su estado actual.

- Envía ese estado solo al usuario que lo solicitó.

## RESETBOARD

- Recibe un mensaje del tipo "resetBoard|<ID>".

- Busca una partida privada con ese ID.

- Si existe, reinicia el tablero con resetBoard().

- Reinicia también otras variables clave:

- - winner = 0 (reinicia al ganador)

- - actual = 1 (reinicia el turno o jugador actual)

- Notifica a todos los jugadores de la partida el nuevo estado del juego.

## TURN 

- Recibe un mensaje del tipo "turn|<ID>|<posición>".

- Busca una partida privada con ese ID.

- Si no existe, envía un error al usuario: "error|No estás en una partida".

- Determina si el jugador es el jugador 1 o el jugador 2, comparando su username.

- Convierte la posición recibida a un número (restando 1).

- Intenta realizar el turno con partida.turn(...).

- Si ocurre un error (el resultado empieza con "error|"), se envía ese error al usuario y se detiene ahí.

- Si todo sale bien, obtiene el nuevo estado de la partida y lo envía a todos los jugadores de esa partida.

## UPDATEUSERNAME

- Recibe un mensaje del tipo "updateUsername|<nuevoNombre>".

- - Verifica si ya existe un usuario conectado con ese nombre:

- Si sí, envía un mensaje de error al usuario: "Username already in use" y termina ahí.

- Si el nombre está registrado en usuariosPersistentes (usuarios desconectados que dejaron una partida activa):

- - Recupera la partida antigua asociada al nuevo nombre.

- - Si la partida existe y el nombre aún no está en uso, entonces:

- - - Reconecta al jugador a esa partida.

- - - Actualiza su username y partidaID.

- - - Restaura la partida en partidasPrivadas.

- - - Determina el oponente y su número de jugador.

- - - Envía mensajes para:

- - - - Confirmar reconexión.

- - - - Iniciar el juego (START_GAME).

- - - - Enviar el estado actual de la partida.

- - - Termina el bloque.

- Si no había partida previa o ya terminó:

- - Actualiza simplemente el nombre del usuario.

- - Guarda el nombre en usuariosPersistentes con datos vacíos (sin partida asociada).

- - Informa al usuario que su nombre fue actualizado con éxito.

## GETUSERLIST

- Recibe un mensaje del tipo "getUsersList".

- Crea una lista de usuarios conectados que cumplen estas condiciones:

- - Tienen un nombre distinto de "none" (es decir, están identificados).

- - No son el usuario actual (no se muestra a sí mismo).

- - Tienen la conexión abierta (WebSocket.OPEN).

- - (Opcionalmente comentado): podrían excluirse si están en una partida (!u.playingState), pero eso está desactivado en este código.

- Envía al usuario que pidió la lista un mensaje con los nombres válidos, en este formato:
"usersList|{"users":["nombre1","nombre2",...]}"

## GETMATCHESLIST

- Recibe un mensaje del tipo "getMatchesList".

- Busca en partidasPrivadas todas las partidas en las que el usuario actual esté participando (p1 o p2).

- Extrae solo los IDs de esas partidas.

- Crea un objeto con esos IDs bajo la clave matchID.

- Envía al usuario que hizo la solicitud un mensaje con la lista de partidas en las que participa, en este formato:
"matchesList|{"matchID":["id1","id2",...]}"

## SENDGAMEINVITE

- Recibe un mensaje del tipo "sendGameInvite|<nombreDelUsuarioObjetivo>".

- Busca al usuario objetivo por nombre usando findClientByUsername.

- Si el usuario existe y su conexión está activa (WebSocket.OPEN):

- - Guarda quién lo está invitando (invitedBy = user.username).

- - Le envía al usuario objetivo un mensaje de invitación:
"invite|<nombreDelUsuarioQueInvita>"

- Si no se encuentra al usuario o su conexión está cerrada:

- - Envía un mensaje de error al usuario que intentó invitar:
"error|User: <nombreObjetivo> not found"

## GAMEINVITERESPONSE

- Recibe un mensaje del tipo "gameInviteResponse|<respuesta>".

- Verifica si el usuario tiene a alguien registrado como invitador (user.invitedBy):

- - Si no tiene invitador, envía un mensaje de error: "error|No tienes invitador registrado".

- Si el usuario tiene un invitador registrado:

- - Busca al usuario que lo invitó usando findClientByUsername.

- - Si no se encuentra al invitador, envía un mensaje de error: "error|Inviter not found|<nombreInvitador>".

- Si la respuesta es "yes" (acepta la invitación):

- - Crea una nueva partida (en este caso, un juego de "Gato").

- - Asigna aleatoriamente a los jugadores como p1 o p2.

- - Inicializa la partida y la guarda en partidasPrivadas.

- - Actualiza los datos de los usuarios en usuariosPersistentes y los marca como participantes en la nueva partida.

- - Envía a ambos usuarios el mensaje "START_GAME" con los detalles del oponente y el número de jugador.

- - Envía el estado inicial de la partida a ambos usuarios.

- Si la respuesta es "no" (rechaza la invitación):

- - El invitado borra la referencia al invitador y envía un mensaje de rechazo al invitador: "REJECTED|
<nombreInvitador>".

- - El invitado también recibe un mensaje informando que rechazó la invitación.

## GAMESELECTED

- Recibe un mensaje del tipo "gameSelected|<IDdePartida>".

- Busca la partida en partidasPrivadas usando el partidaID.

- - Si la partida no se encuentra, envía un mensaje de error: "error|Partida no encontrada".

- Valida que el usuario esté realmente en la partida:

- - Si el usuario no es ni p1 ni p2 de la partida, envía un mensaje de error: "error|No perteneces a esta partida".

- Si todo es válido:

- - Asocia al usuario con la partida en usuariosPersistentes.

- - Determina el número de jugador (1 o 2) y el oponente.

- - Envía el mensaje "START_GAME" a ambos jugadores para iniciar el juego:

- - - Avisa al usuario quién es su oponente y su número de jugador.

- - Envía el estado actual de la partida ("data|<estado>") a ambos jugadores.



