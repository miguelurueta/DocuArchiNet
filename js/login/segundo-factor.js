(function (root, factory) {
    'use strict';
    var api = factory();
    if (typeof module === 'object' && module.exports) {
        module.exports = api;
    }
    if (root && root.document) {
        root.DocuArchiSegundoFactor = api.createBrowserClient(root);
    }
}(typeof window !== 'undefined' ? window : this, function () {
    'use strict';

    var PUBLIC_STATE_PENDING = 'PENDIENTE';
    var PUBLIC_STATE_COMPLETED = 'COMPLETADO';
    var RECOVERABLE_CODES = {
        SECOND_FACTOR_CODE_INVALID: true,
        SECOND_FACTOR_RESEND_COOLDOWN: true,
        SECOND_FACTOR_PENDING: true,
        SECOND_FACTOR_RESENT: true,
        SECOND_FACTOR_REQUIRED: true
    };
    var GENERIC_ERROR = 'No fue posible completar la verificación. Inicie sesión nuevamente.';

    function integerOrZero(value) {
        var number = Number(value);
        return isFinite(number) && number > 0 ? Math.floor(number) : 0;
    }

    function isPending(state) {
        return !!state && state.RequiereCodigo === true && state.EstadoPublico === PUBLIC_STATE_PENDING;
    }

    function isSafeLocalRoute(route) {
        return typeof route === 'string' &&
            /^~\/[A-Za-z0-9._/-]+$/.test(route) &&
            route.indexOf('..') < 0 && route.indexOf('//') < 0 &&
            route.indexOf('\\') < 0 && route.indexOf(':') < 0 &&
            route.indexOf('?') < 0 && route.indexOf('#') < 0;
    }

    function applicationRoot(endpoint, currentLocation) {
        var absolute = new URL(endpoint, currentLocation);
        var marker = '/webservice/WebServiceLoginSegundoFactor.asmx';
        var markerIndex = absolute.pathname.toLowerCase().indexOf(marker.toLowerCase());
        if (markerIndex < 0) {
            throw new Error('SECOND_FACTOR_ENDPOINT_INVALID');
        }
        absolute.pathname = absolute.pathname.substring(0, markerIndex + 1);
        absolute.search = '';
        absolute.hash = '';
        return absolute;
    }

    function resolveLocalRoute(route, endpoint, currentLocation) {
        if (!isSafeLocalRoute(route)) {
            return '';
        }
        return new URL(route.substring(2), applicationRoot(endpoint, currentLocation)).toString();
    }

    function createAsmxTransport(options) {
        var endpoint = options.endpoint;
        var request = options.request;
        return function (operation, payload) {
            var data = operation === 'Verificar' ? { codigo: String(payload.codigo || '') } : {};
            return request(endpoint + '/' + operation, data).then(function (envelope) {
                if (!envelope || typeof envelope !== 'object' || !Object.prototype.hasOwnProperty.call(envelope, 'd')) {
                    throw new Error('SECOND_FACTOR_CONTRACT_INVALID');
                }
                return envelope.d;
            });
        };
    }

    function createClient(options) {
        var transport = options.transport;
        var view = options.view;
        var now = options.now || Date.now;
        var schedule = options.setInterval || setInterval;
        var unschedule = options.clearInterval || clearInterval;
        var navigate = options.navigate;
        var endpoint = options.endpoint;
        var currentLocation = options.currentLocation;
        var busy = false;
        var timer = null;
        var expiresAt = 0;
        var resendAt = 0;
        var expirationChecked = false;

        function stopTimer() {
            if (timer !== null) {
                unschedule(timer);
                timer = null;
            }
        }

        function setBusy(value) {
            busy = value;
            view.setBusy(value);
        }

        function remaining(until) {
            return Math.max(0, Math.ceil((until - now()) / 1000));
        }

        function tick() {
            var expiry = remaining(expiresAt);
            var resend = remaining(resendAt);
            view.renderTime(expiry, resend);
            if (expiry === 0 && !busy && !expirationChecked) {
                expirationChecked = true;
                restore();
            }
        }

        function renderPending(state, message, isError) {
            var current = now();
            expiresAt = current + integerOrZero(state.ExpiraEnSegundos) * 1000;
            resendAt = current + integerOrZero(state.ReenvioDisponibleEnSegundos) * 1000;
            expirationChecked = false;
            view.showPending(String(state.DestinoEnmascarado || ''));
            view.showMessage(message || '', isError === true);
            stopTimer();
            timer = schedule(tick, 1000);
            tick();
        }

        function reset(message) {
            stopTimer();
            expiresAt = 0;
            resendAt = 0;
            expirationChecked = false;
            view.showLogin(message || '');
        }

        function failClosed(message) {
            reset(message || GENERIC_ERROR);
        }

        function restore() {
            if (busy) {
                return Promise.resolve(false);
            }
            setBusy(true);
            return transport('ObtenerEstado', {}).then(function (state) {
                if (isPending(state)) {
                    renderPending(state, '', false);
                    return true;
                }
                reset('');
                return false;
            }).catch(function () {
                failClosed(GENERIC_ERROR);
                return false;
            }).then(function (result) {
                setBusy(false);
                if (timer !== null) { tick(); }
                return result;
            });
        }

        function handleResult(result) {
            if (result && result.Exito === true && result.Codigo === 'SECOND_FACTOR_COMPLETED' &&
                    result.Estado && result.Estado.EstadoPublico === PUBLIC_STATE_COMPLETED) {
                var destination = resolveLocalRoute(result.RutaLocal, endpoint, currentLocation);
                if (!destination) {
                    failClosed(GENERIC_ERROR);
                    return false;
                }
                stopTimer();
                view.clearSensitive();
                navigate(destination);
                return true;
            }

            if (result && RECOVERABLE_CODES[result.Codigo] && isPending(result.Estado)) {
                renderPending(result.Estado, String(result.MensajeVisible || ''), result.Exito !== true);
                return false;
            }

            failClosed(result && result.MensajeVisible ? String(result.MensajeVisible) : GENERIC_ERROR);
            return false;
        }

        function mutate(operation, payload) {
            if (busy) {
                return Promise.resolve(false);
            }
            setBusy(true);
            return transport(operation, payload || {}).then(handleResult).catch(function () {
                failClosed(GENERIC_ERROR);
                return false;
            }).then(function (result) {
                setBusy(false);
                if (timer !== null) { tick(); }
                return result;
            });
        }

        return {
            iniciar: restore,
            restaurar: restore,
            verificar: function (codigo) {
                var normalized = String(codigo || '').replace(/\D/g, '').substring(0, 6);
                if (!/^\d{6}$/.test(normalized)) {
                    view.showMessage('Ingrese un código de seis dígitos.', true);
                    return Promise.resolve(false);
                }
                view.clearCode();
                return mutate('Verificar', { codigo: normalized });
            },
            reenviar: function () { return mutate('Reenviar', {}); },
            cancelar: function () { return mutate('Cancelar', {}); },
            detener: stopTimer
        };
    }

    function xhrRequest(windowObject, url, payload) {
        return new Promise(function (resolve, reject) {
            var xhr = new windowObject.XMLHttpRequest();
            xhr.open('POST', url, true);
            xhr.withCredentials = true;
            xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
            xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
            xhr.onreadystatechange = function () {
                if (xhr.readyState !== 4) { return; }
                if (xhr.status < 200 || xhr.status >= 300) { reject(new Error('SECOND_FACTOR_HTTP_FAILED')); return; }
                try { resolve(JSON.parse(xhr.responseText)); } catch (_) { reject(new Error('SECOND_FACTOR_JSON_INVALID')); }
            };
            xhr.onerror = function () { reject(new Error('SECOND_FACTOR_NETWORK_FAILED')); };
            xhr.send(JSON.stringify(payload));
        });
    }

    function createDomView(windowObject) {
        var documentObject = windowObject.document;
        var publicMessage = '';
        function byId(id) { return documentObject.getElementById(id); }
        function controls() {
            return [byId('docuarchi-segundo-factor-codigo'), byId('docuarchi-segundo-factor-validar'),
                byId('docuarchi-segundo-factor-reenviar'), byId('docuarchi-segundo-factor-cancelar')];
        }
        function clearSensitive() {
            var code = byId('docuarchi-segundo-factor-codigo');
            var password = byId('ContentPlacenter_TextBoxpasw');
            if (code) { code.value = ''; }
            if (password) { password.value = ''; }
        }
        return {
            bind: function (handlers) {
                var panel = byId('docuarchi-segundo-factor-panel');
                if (!panel || panel.getAttribute('data-bound') === 'true') { return; }
                panel.setAttribute('data-bound', 'true');
                byId('docuarchi-segundo-factor-codigo').addEventListener('input', function (event) {
                    event.target.value = event.target.value.replace(/\D/g, '').substring(0, 6);
                });
                byId('docuarchi-segundo-factor-validar').addEventListener('click', function () {
                    handlers.verify(byId('docuarchi-segundo-factor-codigo').value);
                });
                byId('docuarchi-segundo-factor-reenviar').addEventListener('click', handlers.resend);
                byId('docuarchi-segundo-factor-cancelar').addEventListener('click', handlers.cancel);
            },
            setBusy: function (value) {
                controls().forEach(function (control) { if (control) { control.disabled = value; } });
            },
            showPending: function (maskedDestination) {
                var login = byId('ContentPlacenter_Panel_sesion_privado_login');
                var panel = byId('docuarchi-segundo-factor-panel');
                if (!login || !panel) { throw new Error('SECOND_FACTOR_VIEW_INVALID'); }
                login.hidden = true;
                Array.prototype.forEach.call(login.querySelectorAll('input,select,button,a'), function (item) {
                    item.setAttribute('aria-disabled', 'true');
                    if ('disabled' in item) { item.disabled = true; }
                });
                byId('docuarchi-segundo-factor-destino').textContent = maskedDestination;
                byId('docuarchi-segundo-factor-login-error').textContent = '';
                panel.hidden = false;
                byId('docuarchi-segundo-factor-codigo').focus();
            },
            showLogin: function (message) {
                var login = byId('ContentPlacenter_Panel_sesion_privado_login');
                var panel = byId('docuarchi-segundo-factor-panel');
                clearSensitive();
                if (panel) { panel.hidden = true; }
                if (login) {
                    login.hidden = false;
                    Array.prototype.forEach.call(login.querySelectorAll('input,select,button,a'), function (item) {
                        item.removeAttribute('aria-disabled');
                        if ('disabled' in item) { item.disabled = false; }
                    });
                }
                var error = byId('docuarchi-segundo-factor-error');
                if (error) { error.textContent = ''; }
                publicMessage = '';
                var loginError = byId('docuarchi-segundo-factor-login-error');
                if (loginError) { loginError.textContent = message; }
                var user = byId('ContentPlacenter_TextBoxuser');
                if (message && user) { user.focus(); }
            },
            showMessage: function (message, isError) {
                publicMessage = isError ? '' : message;
                byId('docuarchi-segundo-factor-estado').textContent = publicMessage;
                byId('docuarchi-segundo-factor-error').textContent = isError ? message : '';
            },
            renderTime: function (expiry, resend) {
                var status = byId('docuarchi-segundo-factor-estado');
                var resendButton = byId('docuarchi-segundo-factor-reenviar');
                if (status) {
                    status.textContent = (publicMessage ? publicMessage + ' ' : '') + 'El código expira en ' + expiry + ' segundos.';
                }
                if (resendButton) {
                    resendButton.disabled = resend > 0;
                    resendButton.textContent = resend > 0 ? 'Reenviar código (' + resend + ' s)' : 'Reenviar código';
                }
            },
            clearCode: function () {
                var code = byId('docuarchi-segundo-factor-codigo');
                if (code) { code.value = ''; }
            },
            clearSensitive: clearSensitive
        };
    }

    function createBrowserClient(windowObject) {
        var activeClient = null;
        var endRequestRegistered = false;

        function buildClient() {
            var panel = windowObject.document.getElementById('docuarchi-segundo-factor-panel');
            if (!panel) { return null; }
            var endpoint = panel.getAttribute('data-endpoint');
            var view = createDomView(windowObject);
            var transport = createAsmxTransport({
                endpoint: endpoint,
                request: function (url, data) { return xhrRequest(windowObject, url, data); }
            });
            var client = createClient({
                endpoint: endpoint,
                currentLocation: windowObject.location.href,
                transport: transport,
                view: view,
                now: function () { return Date.now(); },
                setInterval: windowObject.setInterval.bind(windowObject),
                clearInterval: windowObject.clearInterval.bind(windowObject),
                navigate: function (url) { windowObject.location.assign(url); }
            });
            view.bind({ verify: client.verificar, resend: client.reenviar, cancel: client.cancelar });
            return client;
        }

        function iniciar() {
            if (activeClient) { activeClient.detener(); }
            activeClient = buildClient();
            return activeClient ? activeClient.iniciar() : Promise.resolve(false);
        }

        function registerLifecycle() {
            if (!endRequestRegistered && windowObject.Sys && windowObject.Sys.WebForms) {
                windowObject.Sys.WebForms.PageRequestManager.getInstance().add_endRequest(iniciar);
                endRequestRegistered = true;
            }
            iniciar();
        }

        if (windowObject.document.readyState === 'loading') {
            windowObject.document.addEventListener('DOMContentLoaded', registerLifecycle);
        } else {
            registerLifecycle();
        }

        return { iniciar: iniciar, restaurar: iniciar };
    }

    return {
        createAsmxTransport: createAsmxTransport,
        createClient: createClient,
        createBrowserClient: createBrowserClient,
        isSafeLocalRoute: isSafeLocalRoute,
        resolveLocalRoute: resolveLocalRoute
    };
}));
