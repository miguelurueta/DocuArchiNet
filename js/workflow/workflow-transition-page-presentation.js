(function (window, document) {
    "use strict";

    var api = {};
    var successTimer = null;
    var successMessageDuration = 6000;

    function asPositiveInteger(value) {
        var parsed = Number(value);
        return isFinite(parsed) && parsed > 0 && Math.floor(parsed) === parsed ? parsed : 0;
    }

    function find(selector) {
        return document.querySelector ? document.querySelector(selector) : null;
    }

    function taskRow(idTarea) {
        var rows = document.querySelectorAll ? document.querySelectorAll("[data-workflow-task-id]") : [];
        var index;
        for (index = 0; index < rows.length; index += 1) {
            if (String(rows[index].getAttribute("data-workflow-task-id")) === String(idTarea)) {
                return rows[index];
            }
        }
        return null;
    }

    function updateCounter() {
        var counter = find("[data-workflow-task-count]");
        var text;
        var match;
        var count;

        if (!counter) {
            return;
        }
        text = counter.textContent || "";
        match = text.match(/^(.*\()\s*(\d+)\s*(\)\s*)$/);
        if (!match) {
            return;
        }
        count = Math.max(0, Number(match[2]) - 1);
        counter.textContent = match[1] + count + match[3];
    }

    function setVisible(element, visible) {
        if (!element) {
            return;
        }
        element.hidden = !visible;
        element.setAttribute("aria-hidden", visible ? "false" : "true");
        if (element.style) {
            element.style.display = visible ? "block" : "none";
        }
    }

    function setVisibleForAll(selector, visible) {
        var elements = document.querySelectorAll ? document.querySelectorAll(selector) : [];
        var index;

        for (index = 0; index < elements.length; index += 1) {
            setVisible(elements[index], visible);
        }
    }

    function clearTaskContext() {
        var context = find("[data-workflow-task-context]");
        var viewer = find("[data-workflow-task-viewer]");
        var list = find("[data-workflow-task-list]");

        setVisible(context, false);
        setVisible(list, true);
        setVisibleForAll("[data-workflow-task-action]", false);
        setVisibleForAll("[data-workflow-task-toggle]", false);
        if (viewer) {
            viewer.removeAttribute("src");
        }
    }

    function restoreTaskListLayout() {
        var scroller = find("[data-workflow-task-scroll]");

        if (scroller && typeof scroller.scrollLeft === "number") {
            scroller.scrollLeft = 0;
        }
        if (typeof window.auto_zise_popup_workflow === "function") {
            window.auto_zise_popup_workflow("1");
        }
    }

    function showSuccess() {
        var message = find("[data-workflow-transition-success]");
        if (!message) {
            return;
        }
        if (successTimer !== null && typeof window.clearTimeout === "function") {
            window.clearTimeout(successTimer);
        }
        message.textContent = "La tarea fue enviada correctamente.";
        setVisible(message, true);
        message.removeAttribute("hidden");
        if (typeof window.setTimeout === "function") {
            successTimer = window.setTimeout(function () {
                setVisible(message, false);
                message.setAttribute("hidden", "hidden");
                message.textContent = "";
                successTimer = null;
            }, successMessageDuration);
        }
    }

    function applySuccess(detail) {
        var idTarea = asPositiveInteger(detail && detail.idTarea);
        var row;

        if (!idTarea) {
            return false;
        }
        row = taskRow(idTarea);
        if (row && row.parentNode) {
            row.parentNode.removeChild(row);
        }
        updateCounter();
        clearTaskContext();
        restoreTaskListLayout();
        showSuccess();
        return true;
    }

    api.applySuccess = applySuccess;
    window.WorkflowTransitionPagePresentation = api;
}(window, document));
