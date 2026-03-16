window.eventEase = {
    getAllEvents: function () {
        try {
            const indexKey = 'eventease:ids';
            const raw = localStorage.getItem(indexKey);
            if (!raw) return JSON.stringify([]);
            const ids = JSON.parse(raw);
            const list = [];
            for (let i = 0; i < ids.length; i++) {
                const key = 'eventease:event:' + ids[i];
                const j = localStorage.getItem(key);
                if (j) {
                    try { list.push(JSON.parse(j)); } catch (e) { }
                }
            }
            return JSON.stringify(list);
        }
        catch (e) {
            return JSON.stringify([]);
        }
    }
};
