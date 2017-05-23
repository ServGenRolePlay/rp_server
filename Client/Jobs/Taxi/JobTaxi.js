API.onServerEventTrigger.connect(function (eventName, args) {
    switch (eventName) {
        case "markOnMap":
            API.setWaypoint(args[0], args[1]);
            break;
        case "finishTaxiClient":
            API.removeWaypoint();
            break;
    }
});
