# Arquitectura verificada

## Vista general

DOC-72 implementa una capa frontend sin build, cargada por WebForms en este orden: API → registro → core → UI. `importar-servicio-web-api.js` es el único módulo DOC-72 que usa transporte HTTP. `provider-registry.js` resuelve una identidad canónica. `core.js` mantiene estado y serializa la ejecución. `ui.js` crea el adaptador que une core con API y controla modal/foco.

El backend no es genérico en producción: `WebServiceImportarServicioWebModern.ValidRequest` acepta únicamente `SiiImportProvider.CanonicalProviderId`, y `ResolveProvider` construye el cliente SII desde configuración legacy. Por tanto, “genérico” describe la arquitectura del núcleo JavaScript, no la composición productiva actualmente desplegable.

## Responsabilidades reales

| Componente | Responsabilidad implementada | No hace |
| --- | --- | --- |
| `ImportarServicioWebApi` | POST JSON, credenciales same-origin, envelope ASMX `d`, ocho nombres de operación | No valida negocio ni decide proveedor |
| `ImportarServicioWebProviderRegistry` | normaliza ID, registra adaptador/capacidades, devuelve errores cerrados | No consulta backend ni selecciona SII por defecto |
| `ImportarServicioWebCore` | transiciones, consulta, una promesa de ejecución, notificación de snapshots | No crea intención, no ordena fases backend, no modifica documentos |
| `ImportarServicioWebUi` | bootstrap DOM, apertura/cierre, consulta inicial, render de lista, teclado/foco | No ofrece selección, preflight, creación, ejecución o reconciliación desde controles visibles |
| `Webworkflow` | registra assets y atributos bootstrap solo con gate activo | No contiene reglas de importación |
| `WebServiceImportarServicioWebModern` | valida gate/DTO/contexto, compone servicios y proyecta errores seguros | No es llamado si el gate está apagado; no acepta hoy proveedores distintos de SII |

## Dependencias y dirección

`UI → Core → Registry → Adapter(UI) → API → ASMX`. El adaptador creado por UI llama primero `resolveCapabilities` y luego `queryItems`. `core.execute` puede llamar `executeImportIntent`, pero ninguna acción del modal invoca actualmente `core.execute`; esa capacidad solo está disponible programáticamente y cubierta por prueba unitaria.

## Invariantes comprobados

- Gate versionado en `false`; listas de usuarios/grupos vacías.
- Sin modificación de `js/workflow/Webworkflow.js`, `JSProgresBar.js`, almacenamiento o endpoints legacy.
- Proveedor vacío/no migrado/desconocido falla sin fallback.
- Una ejecución concurrente reutiliza la misma `Promise` hasta `close`.
- El frontend no modela porcentaje ni progreso individual.
