API.onKeyUp.connect(function (sender, keyEventArgs) 
{
    if (keyEventArgs.KeyValue == 50 && !API.isChatOpen()) 
	{
        API.triggerServerEvent("VEHICLE_ENGINE_TOGGLE");
    }
});