package io.gamemachine.routing;

import io.gamemachine.authentication.Authentication;
import io.gamemachine.core.ActorUtil;
import io.gamemachine.messages.ChatDestroy;
import io.gamemachine.messages.ClientConnection;
import io.gamemachine.messages.ClientManagerRegister;
import io.gamemachine.messages.ClientManagerUnregister;
import io.gamemachine.messages.ClientMessage;
import io.gamemachine.messages.Entity;

import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import akka.actor.ActorSelection;
import akka.actor.UntypedActor;

public class RequestHandler extends UntypedActor {

	private static final Logger logger = LoggerFactory.getLogger(RequestHandler.class);

	public static String name = "request_handler";
	private EntityRouter entityRouter;

	public RequestHandler() {
		entityRouter = new EntityRouter();
	}

	@Override
	public void onReceive(Object message) {
		if (message instanceof ClientMessage) {
			ClientMessage clientMessage = (ClientMessage) message;
			if (clientMessage.hasTrackData()) {
				entityRouter.routeTrackData(clientMessage.getTrackData());
			}
			
			if (clientMessage.getEntityCount() >= 1) {
				List<Entity> entities = clientMessage.getEntityList();
				entityRouter.route(entities, clientMessage.getPlayer());
			}
		} else {
			unhandled(message);
		}
	}

	public static void unregisterClient(ClientMessage clientMessage) {
		logger.debug("unregister client " + clientMessage.getPlayer().getId());
		String playerId = clientMessage.getPlayer().getId();
		String clientId = clientMessage.getClientConnection().getId();
		Authentication.unregisterPlayer(playerId);
		ClientManagerUnregister unregister = new ClientManagerUnregister();
		unregister.setRegisterType("client");
		unregister.setName(clientId);
		Entity entity = new Entity();
		entity.setId(clientId);
		entity.setClientManagerUnregister(unregister);
		entity.setClientConnection(clientMessage.clientConnection);
		entity.setPlayer(clientMessage.getPlayer());
		ActorSelection sel = ActorUtil.getSelectionByName("GameMachine::ClientManager");
		sel.tell(entity, null);

		ChatDestroy chatDestroy = new ChatDestroy();
		chatDestroy.setPlayerId(playerId);
		sel = ActorUtil.getSelectionByName("GameMachine::GameSystems::ChatManager");
		sel.tell(chatDestroy, null);
	}

	public static void registerClient(ClientMessage clientMessage) {
		logger.debug("register client " + clientMessage.getPlayer().getId());
		ClientConnection clientConnection = clientMessage.getClientConnection();
		String clientId = clientConnection.getId();
		ClientManagerRegister register = new ClientManagerRegister();
		register.setRegisterType("client");
		register.setName(clientId);
		Entity entity = new Entity();
		entity.setId(clientId);
		entity.setClientManagerRegister(register);
		entity.setClientConnection(clientConnection);
		entity.setPlayer(clientMessage.getPlayer());
		ActorUtil.ask("GameMachine::ClientManager", entity, 50);
	}

}
