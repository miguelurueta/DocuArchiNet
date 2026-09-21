# Arquitectura

El feature se divide en API, registro, core y UI. `api.js` es la única frontera HTTP; el registro resuelve identidades canónicas y capacidades; core controla estados y ejecución única; UI renderiza y gestiona accesibilidad. La secuencia mutadora permanece exclusivamente en `ImportServiceOrchestrator` del backend.

No existe dependencia con conceptos internos de SII, `JSProgresBar`, scripts globales legacy ni almacenamiento documental.
