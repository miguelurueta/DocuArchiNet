(function (env) {
    "use strict";
    var bar = null, observer = null, frame = null, height = -1;
    function refresh() {
        if (frame !== null) { return; }
        frame = env.requestAnimationFrame(function () {
            frame = null;
            if (!bar || !bar.isConnected || bar.clientHeight === height) { return; }
            height = bar.clientHeight;
            if (typeof env.auto_zise === "function") { env.auto_zise(); }
        });
    }
    function mount() {
        var current = env.document.getElementById("menu_var");
        if (current !== bar) {
            if (observer) { observer.disconnect(); observer = null; }
            bar = current; height = -1;
            if (bar && env.ResizeObserver) { observer = new env.ResizeObserver(refresh); observer.observe(bar); }
        }
        refresh();
    }
    function install() {
        mount();
        env.addEventListener("resize", refresh);
        if (env.jQuery) { env.jQuery(env.document).on("shown.bs.collapse hidden.bs.collapse", "#navbarNavDropdown", refresh); }
        if (env.Sys && env.Sys.Application) { env.Sys.Application.add_load(mount); }
    }
    if (env.document.readyState === "loading") { env.document.addEventListener("DOMContentLoaded", install, {once: true}); } else { install(); }
}(window));
