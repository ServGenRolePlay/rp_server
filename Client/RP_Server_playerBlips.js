API.onResourceStart.connect(function () {
});

API.onResourceStop.connect(function () {
});

API.onServerEventTrigger.connect(function (eventName, args) {
    if(eventName == "SHOW_BLIP_HANDLE")
    {
        var ourBlip = args[0];
        API.sendChatMessage("Client: " + ourBlip.ToString());
        API.setBlipTransparency(ourBlip, 0);
        //API.setEntityTransparency(ourBlip, 0);
    }
});

API.onUpdate.connect(function() {
	/*var player = API.getLocalPlayer();
	if (API.hasEntitySyncedData(player, "PLAYER_BLIP")) {
	    var ourBlip = API.getEntitySyncedData(player, "PLAYER_BLIP");
	    if (ourBlip != null) {
	        //API.setBlipTransparency(ourBlip, 0);
	        API.setEntityTransparency(ourBlip, 0);
	    }
	}*/
});