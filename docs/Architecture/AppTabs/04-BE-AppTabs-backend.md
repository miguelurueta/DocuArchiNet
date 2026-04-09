# PROMPT ARQUITECTONICO  Ticket 04 BE
# Contrato backend para estado/configuracion de tabs (futuro)

Rol esperado:
Arquitecto de software senior backend (APIs enterprise, contratos)


OBJETIVO

Definir contratos backend opcionales para persistir el estado de tabs y cargar configuraciones dinamicas por ruta/usuario.


CONTEXTO EXISTENTE

- arquitectura: `docs/Architecture/AppTabs/AppTabs-Architecture.md`
- integracion backend es opcional y desacoplada


UBICACION (OBLIGATORIA)

```
Backend / Documentacion de API
```


RESTRICCIONES (OBLIGATORIAS)

- no acoplar el componente AppTabs al backend
- contrato versionable


REGLAS DE IMPLEMENTACION (OBLIGATORIAS)

- `POST /ui/tabs/state`
  - payload: `{ userId, route, activeKey, timestamp }`
  - respuesta: `{ status: "ok", saved: true }`
  - ejemplo:
    - request:
      - `{ "userId": "u123", "route": "/pagina/historial", "activeKey": "history", "timestamp": "2026-04-09T00:00:00Z" }`
    - response:
      - `{ "status": "ok", "saved": true }`

- `GET /ui/tabs/state?route=...`
  - respuesta: `{ activeKey }`
  - ejemplo:
    - response:
      - `{ "activeKey": "history" }`

- `GET /ui/tabs/config?route=...`
  - respuesta: `{ items: [{ key, label, order, disabled? }] }`
  - ejemplo:
    - response:
      - `{ "items": [{ "key": "info", "label": "Información", "order": 1 }, { "key": "history", "label": "Historial", "order": 2, "disabled": true }] }`

Seguridad (obligatorio)

- requiere auth (token/session)
- validar pertenencia de `userId` al tenant


CRITERIOS DE ACEPTACION

- contratos claros y versionables
- respuesta usable por el contenedor sin transformaciones adicionales
