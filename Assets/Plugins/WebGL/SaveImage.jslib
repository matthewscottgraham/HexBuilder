mergeInto(LibraryManager.library, {
    DownloadFile: function (fileNamePtr, dataPtr, dataLength) {
        var fileName = UTF8ToString(fileNamePtr);
        var data = HEAPU8.subarray(dataPtr, dataPtr + dataLength);

        var blob = new Blob([data], { type: "image/jpeg" });
        var url = URL.createObjectURL(blob);

        var a = document.createElement("a");
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);

        URL.revokeObjectURL(url);
    }
});