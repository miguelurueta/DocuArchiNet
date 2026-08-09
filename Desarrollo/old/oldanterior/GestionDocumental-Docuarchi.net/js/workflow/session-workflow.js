(function () {
    var redireccionandoSesion = false;
    var servidorWorkflowInterrumpido = false;
    var timerRecuperacionServidor = null;
    var timerValidacionSesion = null;
    var sondeoRecuperacionEnCurso = false;
    var validacionSesionEnCurso = false;
    var INTERVALO_RECUPERACION_SERVIDOR = 15000;
    var INTERVALO_VALIDACION_SESION = 60000;

    function respuestaEsLogin(xhr) {
        if (!xhr) {
            return false;
        }

        var texto = xhr.responseText || "";
        var url = xhr.responseURL || "";

        return xhr.status === 401 ||
            xhr.status === 403 ||
            url.toLowerCase().indexOf("/gestor.aspx") >= 0 ||
            texto.indexOf('id="TextBoxuser"') >= 0 ||
            texto.indexOf("INICIAR SESIÓN") >= 0;
    }

    function respuestaEsIndisponibilidad(error) {
        if (!error || typeof error === "string") {
            return false;
        }

        var estado = Number(error.status || error.httpStatusCode || error.statusCode) || 0;
        return estado === 0 || estado === 503;
    }

    function errorPageRequestEsSesion(error) {
        if (!error) {
            return false;
        }

        var estado = error.httpStatusCode || error.statusCode || 0;
        var mensaje = error.message || "";

        return estado === 401 ||
            estado === 403 ||
            mensaje.indexOf('id="TextBoxuser"') >= 0 ||
            mensaje.indexOf("INICIAR SESIÓN") >= 0;
    }

    function mostrarAvisoSesionFinalizada() {
        if (!window.jQuery) {
            return false;
        }

        var popupSesion = jQuery("#modal_alert_sesion_time_out");

        if (popupSesion.length) {
            jQuery("#title_lert_sesion_time_out").text("Su sesión ha caducado o fue cerrada. Será dirigido a la página principal.");
            popupSesion.modal({ backdrop: "static", keyboard: false });
            return true;
        }

        return false;
    }

    function redirigirPorSesion() {
        if (redireccionandoSesion) {
            return;
        }

        redireccionandoSesion = true;
        clearTimeout(timerRecuperacionServidor);
        clearTimeout(timerValidacionSesion);
        if (!mostrarAvisoSesionFinalizada()) {
            window.top.location.replace("../gestor.aspx");
        }
    }

    function advertirYRedirigirPorInterrupcion() {
        if (redireccionandoSesion) {
            return;
        }

        alert("El servidor Workflow se reinició o estuvo fuera de servicio. Por seguridad, su sesión ya no es válida; será dirigido a la página principal para iniciar sesión nuevamente.");
        redirigirPorSesion();
    }

    function programarSondeoRecuperacion() {
        clearTimeout(timerRecuperacionServidor);
        timerRecuperacionServidor = setTimeout(sondearRecuperacionServidor, INTERVALO_RECUPERACION_SERVIDOR);
    }

    function sondearRecuperacionServidor() {
        if (!servidorWorkflowInterrumpido || redireccionandoSesion || sondeoRecuperacionEnCurso) {
            return;
        }

        sondeoRecuperacionEnCurso = true;
        jQuery.ajax({
            type: "GET",
            url: "../workflow/refresh_session.ashx?workflowProbe=" + new Date().getTime(),
            cache: false,
            global: false,
            success: function () {
                // IIS volvió a responder. InProc perdió el contexto Workflow: se exige un nuevo ingreso.
                advertirYRedirigirPorInterrupcion();
            },
            error: function (xhr) {
                if (respuestaEsLogin(xhr)) {
                    advertirYRedirigirPorInterrupcion();
                    return;
                }
                programarSondeoRecuperacion();
            },
            complete: function () {
                sondeoRecuperacionEnCurso = false;
            }
        });
    }

    function registrarInterrupcionServidor() {
        if (redireccionandoSesion || servidorWorkflowInterrumpido) {
            return;
        }

        servidorWorkflowInterrumpido = true;
        programarSondeoRecuperacion();
    }

    function respuestaIndicaSesionInactiva(respuesta) {
        if (respuesta === null || typeof respuesta === "undefined") {
            return true;
        }

        if (typeof respuesta === "string") {
            var texto = respuesta.replace(/^\s+|\s+$/g, "");
            if (texto === "-1") {
                return true;
            }
            try {
                respuesta = JSON.parse(texto);
            } catch (e) {
                return false;
            }
        }

        return respuesta && (respuesta.active === false || respuesta.active === "false");
    }

    function programarValidacionSesion(retraso) {
        clearTimeout(timerValidacionSesion);
        if (redireccionandoSesion || servidorWorkflowInterrumpido) {
            return;
        }
        timerValidacionSesion = setTimeout(validarSesionWorkflow, retraso);
    }

    function validarSesionWorkflow() {
        if (redireccionandoSesion || servidorWorkflowInterrumpido || validacionSesionEnCurso) {
            return;
        }

        validacionSesionEnCurso = true;
        jQuery.ajax({
            type: "GET",
            url: "../workflow/refresh_session.ashx?workflowSession=" + new Date().getTime(),
            cache: false,
            global: false,
            success: function (respuesta) {
                if (respuestaIndicaSesionInactiva(respuesta)) {
                    redirigirPorSesion();
                }
            },
            error: function (xhr) {
                if (respuestaEsLogin(xhr)) {
                    redirigirPorSesion();
                    return;
                }
                if (respuestaEsIndisponibilidad(xhr)) {
                    registrarInterrupcionServidor();
                }
            },
            complete: function () {
                validacionSesionEnCurso = false;
                programarValidacionSesion(INTERVALO_VALIDACION_SESION);
            }
        });
    }

    window.mostrarErrorWorkflow = function (error) {
        if (respuestaEsLogin(error)) {
            redirigirPorSesion();
            return;
        }

        if (respuestaEsIndisponibilidad(error)) {
            registrarInterrupcionServidor();
            return;
        }

        if (typeof error === "string") {
            alert(error);
            return;
        }

        var estado = Number(error && error.status) || 0;
        alert("No fue posible completar la operación. Código HTTP: " + (estado || "no disponible") + ".");
    };

    if (window.jQuery) {
        jQuery(document).ajaxError(function (event, xhr) {
            if (respuestaEsLogin(xhr)) {
                redirigirPorSesion();
                return;
            }

            if (respuestaEsIndisponibilidad(xhr)) {
                registrarInterrupcionServidor();
            }
        });

        jQuery(document).ajaxSuccess(function () {
            if (servidorWorkflowInterrumpido) {
                // Cualquier respuesta posterior a una caída invalida el estado cliente previo.
                advertirYRedirigirPorInterrupcion();
            }
        });
    }

    function registrarManejadorPostBackAsincrono() {
        if (!(window.Sys && Sys.WebForms)) {
            window.setTimeout(registrarManejadorPostBackAsincrono, 50);
            return;
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(
            function (sender, args) {
                var error = args.get_error();

                if (errorPageRequestEsSesion(error)) {
                    args.set_errorHandled(true);
                    redirigirPorSesion();
                    return;
                }

                if (respuestaEsIndisponibilidad(error)) {
                    args.set_errorHandled(true);
                    registrarInterrupcionServidor();
                }
            });
    }

    window.addEventListener("pagehide", function () {
        clearTimeout(timerRecuperacionServidor);
        clearTimeout(timerValidacionSesion);
    });

    registrarManejadorPostBackAsincrono();
    programarValidacionSesion(INTERVALO_VALIDACION_SESION);
}());
