(function (root) {
    "use strict";
    var filters = { all: true, available: true, imported: true, novelty: true };
    function create(options) {
        options = options || {}; var pageSize = Math.max(1, Number(options.pageSize) || 25), items = [], filter = "all", page = 1, selected = {};
        function visible() { return items.filter(function (item) { if (filter === "available") { return item.importable; } if (filter === "imported") { return item.importStatus === "IMPORTED"; } if (filter === "novelty") { return item.importStatus === "NOVELTY"; } return true; }); }
        function snapshot() { var all = visible(), start = (page - 1) * pageSize; return { filter: filter, page: page, total: all.length, items: all.slice(start, start + pageSize), selected: Object.keys(selected) }; }
        return { replace: function (next) { items = (next || []).slice(0); filter = "all"; page = 1; selected = {}; return snapshot(); }, setFilter: function (next) { if (!filters[next]) { throw new Error("SII_FILTER_INVALID"); } filter = next; page = 1; return snapshot(); }, setPage: function (next) { page = Math.max(1, Number(next) || 1); return snapshot(); }, select: function (key, checked) { var item = items.filter(function (candidate) { return candidate.externalKey === key; })[0]; if (!item || !item.importable) { delete selected[key]; } else if (checked) { selected[key] = true; } else { delete selected[key]; } return snapshot(); }, selectAll: function () { visible().forEach(function (item) { if (item.importable) { selected[item.externalKey] = true; } }); return snapshot(); }, snapshot: snapshot };
    }
    var api = { create: create }; root.ImportarServicioWebSiiList = api; if (typeof module !== "undefined" && module.exports) { module.exports = api; }
}(typeof window !== "undefined" ? window : globalThis));
