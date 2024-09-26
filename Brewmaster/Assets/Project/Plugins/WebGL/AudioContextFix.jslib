mergeInto(LibraryManager.library, {
  FixAudioContext: function() {
    if (typeof window.AudioContext !== "undefined" || typeof window.webkitAudioContext !== "undefined") {
      var context = new (window.AudioContext || window.webkitAudioContext)();
      if (context.state === "suspended") {
        var resume = function () {
          context.resume().then(function () {
            console.log("Audio Context Resumed");
            document.body.removeEventListener("mousedown", resume);
            document.body.removeEventListener("touchstart", resume);
            document.body.removeEventListener("keydown", resume);
          });
        };
        document.body.addEventListener("mousedown", resume);
        document.body.addEventListener("touchstart", resume);
        document.body.addEventListener("keydown", resume);
      }
    }
  }
});
