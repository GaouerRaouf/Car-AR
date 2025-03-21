mergeInto(LibraryManager.library, {

	StartWebGLTracker: function(ids, name)
	{
    	window.iTracker.startTracker(UTF8ToString(ids), UTF8ToString(name));
    },
    StopWebGLTracker: function()
	{
    	window.iTracker.stopTracker();
    },
    IsWebGLTrackerReady: function()
    {
        return window.iTracker.FOV != null;
    },
    SetWebGLTrackerSettings: function(settings)
	{
    	window.iTracker.setTrackerSettings(UTF8ToString(settings), "1.4.3.1");
    },
    GetTrackerFov: function()
    {
        return window.iTracker.FOV;
    },
    UnpauseWebGLCamera: function()
	{
    	window.iTracker.unpauseCamera();
    },
    PauseWebGLCamera: function()
	{
    	window.iTracker.pauseCamera();
    },
    DebugImageTarget: function(id)
    {
        window.iTracker.debugImageTarget(UTF8ToString(id));
    },
    IsWebGLImageTracked: function(id)
    {
        return window.iTracker.isImageTracked(id);
    },
    GetWebGLCameraFrame: function(type)
    {
        var data = window.iTracker.getCameraTexture(UTF8ToString(type));
        var bufferSize = lengthBytesUTF8(data) + 1;
        var buffer =  gameInstance.Module._malloc(bufferSize);
        stringToUTF8(data, buffer, bufferSize);
        return buffer;
    },
    GetWebGLCameraName: function()
    {
        var name = window.iTracker.WEBCAM_NAME;
        var bufferSize = lengthBytesUTF8(name) + 1;
        var buffer =  gameInstance.Module._malloc(bufferSize);
        stringToUTF8(name, buffer, bufferSize);
        return buffer;
    }
});
