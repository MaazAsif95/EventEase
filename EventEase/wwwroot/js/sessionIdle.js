(function () {
    var handlers = {
        dotNetRef: null,
        timeoutMs: 0,
        timer: null,
        activityHandler: null
    };

    function resetTimer() {
        if (handlers.timer) clearTimeout(handlers.timer);
        handlers.timer = setTimeout(function () {
            if (handlers.dotNetRef) handlers.dotNetRef.invokeMethodAsync('OnIdleTimeout');
        }, handlers.timeoutMs);
    }

    function onActivity() {
        if (handlers.dotNetRef) handlers.dotNetRef.invokeMethodAsync('OnUserActivity');
        resetTimer();
    }

    window.sessionIdle = {
        start: function (dotNetRef, timeoutMs) {
            handlers.dotNetRef = dotNetRef;
            handlers.timeoutMs = timeoutMs || 300000;
            handlers.activityHandler = onActivity;
            ['mousemove', 'keydown', 'click', 'touchstart'].forEach(function (ev) { window.addEventListener(ev, handlers.activityHandler); });
            resetTimer();
        },
        stop: function () {
            if (handlers.activityHandler) ['mousemove', 'keydown', 'click', 'touchstart'].forEach(function (ev) { window.removeEventListener(ev, handlers.activityHandler); });
            if (handlers.timer) { clearTimeout(handlers.timer); handlers.timer = null; }
            handlers.dotNetRef = null;
        }
    };
})();
