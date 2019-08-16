window.blazorJSPongInterop = {
    setOnbeforeunload: function (instance) {
        window.onbeforeunload = function () {
            instance.invokeMethodAsync('DisposePongComponent');
        };
    },
    unsetOnbeforeunload: function (instance) {
        window.onbeforeunload = null;
    }
};