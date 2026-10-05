(function (global) {
    "use strict";

    var snapshot = null;
    var submitting = false;

    function canonicalize(prefix, value) {
        var normalizedPrefix = String(prefix || "").trim().toUpperCase();
        var normalizedValue = String(value || "").trim().toUpperCase();
        if (!/^[SR]$/.test(normalizedPrefix)) {
            return null;
        }
        if (/^[SR][0-9]{9}$/.test(normalizedValue)) {
            return normalizedValue.charAt(0) === normalizedPrefix ? normalizedValue : null;
        }
        if (!/^[0-9]{1,9}$/.test(normalizedValue)) {
            return null;
        }
        return normalizedPrefix + normalizedValue.padStart(9, "0");
    }

    function positiveInteger(value) {
        var parsed = Number(value);
        return Number.isInteger(parsed) && parsed > 0 ? parsed : null;
    }

    function normalizeProcedureName(value) {
        return String(value || "").trim().toUpperCase();
    }

    function normalizeQuery(receipt, response) {
        var rows = response && Array.isArray(response.d) ? response.d : [];
        if (rows.length !== 1 || !rows[0] || rows[0].Error_gestion !== "YES") {
            return { ok: false, message: rows.length === 1 && rows[0] ? String(rows[0].Error_gestion || "Respuesta SII inválida.") : "La consulta SII no retornó un único resultado." };
        }
        var row = rows[0];
        var sii = row.Class_parram_consultarRadicado;
        var receiptData = row.Class_parram_consultarRecibo;
        var procedures = Array.isArray(row.Class_service_ilist_drowlist) ? row.Class_service_ilist_drowlist : [];
        var activities = Array.isArray(row.Class_service_ilist_drowlist_actividad) ? row.Class_service_ilist_drowlist_actividad : [];
        if (!sii || !String(sii.radicado || "").trim() || !String(sii.nombre || "").trim()) {
            return { ok: false, message: "La respuesta SII está incompleta." };
        }
        var validProcedures = procedures.filter(function (item) {
            var parts = String(item && item.id_value || "").split("|");
            return positiveInteger(parts[0]) !== null;
        });
        var subtype = String(sii.subtipotramite || "").trim();
        var receiptType = String(receiptData && receiptData.tipotramite || "").trim();
        var effectiveProcedureType = subtype || receiptType;
        var normalizedProcedureType = normalizeProcedureName(effectiveProcedureType);
        var matches = normalizedProcedureType ? validProcedures.filter(function (item) {
            return normalizeProcedureName(String(item && item.id_value || "").split("|")[1]) === normalizedProcedureType;
        }) : [];
        var validActivities = activities.filter(function (item) { return positiveInteger(item && item.id_value) !== null; });
        if (matches.length !== 1) {
            return { ok: false, message: "El trámite SII no tiene una coincidencia única en Workflow." };
        }
        if (validActivities.length === 0) {
            return { ok: false, message: "No existen actividades válidas para el trámite." };
        }
        return {
            ok: true,
            snapshot: Object.freeze({
                receipt: receipt,
                barcode: String(sii.radicado).trim(),
                enrollment: String(sii.matricula || "").trim(),
                name: String(sii.nombre).trim(),
                procedureId: positiveInteger(String(matches[0].id_value).split("|")[0]),
                procedureIds: matches.map(function (item) { return positiveInteger(String(item.id_value).split("|")[0]); }),
                procedureSubtype: subtype,
                procedureType: effectiveProcedureType,
                activityIds: validActivities.map(function (item) { return positiveInteger(item.id_value); })
            }),
            procedures: matches,
            activities: validActivities,
            requiresProcedureSelection: false
        };
    }

    function setSnapshot(value) { snapshot = value || null; }
    function invalidate() { snapshot = null; }
    function current() { return snapshot; }

    function buildCommand(prefix, visibleReceipt, procedureId, activityId) {
        var receipt = canonicalize(prefix, visibleReceipt);
        var procedure = positiveInteger(procedureId);
        var activity = positiveInteger(activityId);
        if (!snapshot || receipt !== snapshot.receipt) {
            return { ok: false, message: "El recibo cambió. Consulte nuevamente antes de registrar." };
        }
        if (snapshot.procedureIds.indexOf(procedure) < 0 || snapshot.activityIds.indexOf(activity) < 0) {
            return { ok: false, message: "Seleccione un trámite y una actividad válidos de la última consulta." };
        }
        return { ok: true, command: { recibo: receipt, id_tramite: procedure, id_actividad: activity } };
    }

    function tryBeginSubmit() {
        if (submitting) { return false; }
        submitting = true;
        return true;
    }
    function endSubmit() { submitting = false; }

    global.RegistroTareaRutaSii = Object.freeze({
        canonicalize: canonicalize,
        normalizeQuery: normalizeQuery,
        setSnapshot: setSnapshot,
        invalidate: invalidate,
        current: current,
        buildCommand: buildCommand,
        tryBeginSubmit: tryBeginSubmit,
        endSubmit: endSubmit
    });
}(typeof window !== "undefined" ? window : globalThis));
