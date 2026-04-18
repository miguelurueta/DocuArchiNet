## Context

`SCRUMCORE-115` corresponde a la fase 12 FE de `AppEditor`, enfocada en
habilitar almacenamiento temporal de imagenes locales con `IndexedDB`.

`AppEditor` ya soporta:
- insercion de imagen por URL y por archivo;
- resize persistido via `data-width`;
- alineacion horizontal via `data-align`;
- serializacion HTML reusable;
- integracion en flujos como gestion de correspondencia.

El problema actual es de persistencia temporal del lado cliente:
- las imagenes locales no tienen una infraestructura formal desacoplada del backend;
- el editor no administra un ciclo de vida claro para blobs locales;
- no existe rehidratacion basica de imagenes temporales dentro de la sesion;
- la base para una futura consolidacion al guardar aun no esta preparada.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/12-FE-AppEditor-imagenes-temporales-indexeddb.md`.

## Goals / Non-Goals

**Goals:**
- Crear un adaptador reusable de `IndexedDB` para imagenes temporales.
- Persistir blobs locales con ids `img_local_<uuid>`.
- Insertar imagenes locales en el editor usando `blob:` URLs validas.
- Mantener `data-local-image-id` y `data-source="local"` en el HTML.
- Gestionar explicitamente `URL.createObjectURL` y `URL.revokeObjectURL`.
- Permitir rehidratacion basica en sesion desde HTML con `data-local-image-id`.
- Mantener compatibilidad con URL remota, resize, alineacion y serializacion.

**Non-Goals:**
- No subir imagenes al backend.
- No generar URLs finales ni consolidacion remota.
- No guardar aun el documento completo en servidor.
- No resolver reconciliacion entre imagenes temporales y definitivas.
- No usar `localStorage` como alternativa principal.

## Decisions

1. **Persistencia temporal aislada en `infrastructure/indexeddb/`**
   - **Decision:** Crear un adaptador puro de `IndexedDB`, sin dependencia de React,
     en una carpeta dedicada dentro de `infrastructure`.
   - **Rationale:** El acceso a navegador y a blobs es concern de infraestructura.
     Mantenerlo separado facilita pruebas, evolucion y reemplazo.
   - **Alternatives considered:** Escribir la logica directo en `AppEditorToolbar`
     o `useAppEditor`. Se descarta por acoplar UI con persistencia de bajo nivel.

2. **Modelo `LocalImage` explicito y versionado desde el storage**
   - **Decision:** Persistir cada imagen temporal como un objeto con `id`,
     metadata y `blob`, y versionar la base IndexedDB desde el adaptador.
   - **Rationale:** El editor necesitara metadata estable para futuras fases
     de guardado, limpieza y consolidacion.
   - **Alternatives considered:** Guardar solo el blob y derivar el resto en
     runtime. Se descarta por perder trazabilidad y dificultar tests.

3. **`blob:` URL para render, `data-local-image-id` para persistencia**
   - **Decision:** Usar `blob:` URL exclusivamente para render en el navegador y
     persistir el identificador local en atributos `data-*`.
   - **Rationale:** `blob:` es valido para el DOM, pero no es estable para
     rehidratacion. El atributo `data-local-image-id` conserva la referencia
     semantica reutilizable.
   - **Alternatives considered:** Usar esquemas custom en `src` como
     `local-image://...`. Se descarta porque el navegador no los renderiza como
     imagenes reales y complica el flujo del editor.

4. **Orquestacion del flujo local en `application`**
   - **Decision:** Mover la secuencia "generar id -> guardar blob -> crear object
     url -> insertar nodo" a la capa `application`, manteniendo `presentation`
     enfocada en eventos y formularios.
   - **Rationale:** Esta secuencia ya es logica de caso de uso del editor, no solo
     detalle visual.
   - **Alternatives considered:** Resolver todo desde el handler del input file.
     Se descarta por dificultar mantenimiento y pruebas de integracion.

5. **Registro y limpieza explicita de Object URLs**
   - **Decision:** Mantener un registro interno de Object URLs activas para poder
     revocarlas al eliminar, reemplazar o desmontar.
   - **Rationale:** Las `blob:` URLs son recursos temporales del navegador y su
     fuga es un riesgo real en sesiones de edicion largas.
   - **Alternatives considered:** Dejar que el garbage collector resuelva
     implicitamente. Se descarta por ser poco fiable para un editor enriquecido.

6. **Rehidratacion best-effort dentro de la misma sesion**
   - **Decision:** Cuando el editor cargue HTML con `data-local-image-id`, intentar
     resolver el blob desde `IndexedDB` y regenerar una `blob:` URL; si no existe,
     fallar de forma segura sin romper el resto del documento.
   - **Rationale:** El requerimiento pide una rehidratacion basica, no una
     persistencia definitiva garantizada.
   - **Alternatives considered:** No rehidratar y tratar esas imagenes como
     perdidas. Se descarta por degradar demasiado la experiencia de borrador local.

## Risks / Trade-offs

- [Riesgo] `IndexedDB` puede no estar disponible o fallar en algunos entornos.
  Mitigacion: encapsular errores y degradar con fallo seguro sin romper el editor.

- [Riesgo] El ciclo de vida de `blob:` URLs puede dejar fugas si no se registra
  correctamente.
  Mitigacion: centralizar creacion/revocacion y cubrirlo con pruebas.

- [Riesgo] Rehidratar imagenes locales puede ser costoso si se hace de forma
  indiscriminada sobre documentos largos.
  Mitigacion: limitar la rehidratacion a imagenes con `data-local-image-id` y
  ejecutarla de forma dirigida.

- [Riesgo] La serializacion HTML puede mezclar imagenes remotas y locales.
  Mitigacion: distinguir siempre por `data-source="local"` y `data-local-image-id`.

- [Riesgo] Cambios en insercion de imagen local pueden romper el flujo actual
  de insercion remota o resize.
  Mitigacion: mantener rutas de codigo separadas y validar regresion completa.

## Migration Plan

- Crear el adaptador `IndexedDB` en `infrastructure/indexeddb/`.
- Definir el tipo `LocalImage` y el generador de ids locales.
- Integrar el adaptador en la orquestacion del editor (`useAppEditor` o helper de application).
- Ajustar el flujo de carga de archivo local en la toolbar para usar `IndexedDB`.
- Extender la extension de imagen si hace falta para preservar `data-local-image-id` y `data-source`.
- Implementar registro y limpieza de `blob:` URLs.
- Agregar rehidratacion basica al cargar contenido con imagenes locales.
- Actualizar pruebas unitarias e integracion.

## Open Questions

- ¿Conviene asociar desde ya un `documentDraftId` estable en `AppEditor` o dejar
  inicialmente solo `sessionId` como scope opcional?
- ¿La rehidratacion debe ejecutarse solo al montar el editor o tambien cuando
  cambie `value` en modo controlled con HTML que contenga imagenes locales?
