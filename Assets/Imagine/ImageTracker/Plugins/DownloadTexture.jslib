var DownloadTexture = {
  DownloadWebGLTexture: function(img, size, name, ext) {
    
    console.log(size);

    var binary = '';
    for (var i = 0; i < size; i++)
      binary += String.fromCharCode(HEAPU8[img + i]);

    var filename = UTF8ToString(name);
    var fileext = UTF8ToString(ext);
    if(fileext == ".png")
        var dataUrl = 'data:image/png;base64,' + btoa(binary);
    else if(fileext == ".jpeg")
        var dataUrl = 'data:image/jpeg;base64,' + btoa(binary);

    //download file
    var link = document.createElement("a");
    link.download = filename + fileext;
    link.href = dataUrl;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    delete link;
  },
};
mergeInto(LibraryManager.library, DownloadTexture);