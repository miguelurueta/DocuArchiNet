# Worklog: SCRUMCORE-185 (Ajustes AppEditor / Gestion Respuesta)

Fecha: 2026-04-27

## Objetivo

Dejar el editor de respuesta con una experiencia "paginada" consistente (A4 visual), y robustecer el comportamiento al pegar contenido largo (incluyendo cadenas sin espacios como rutas).

## Cambios realizados

### 1. AppEditor por defecto en modo paginado visual

- Se cambia el default de `paginationMode` en `AppEditor` a `visual`.
- Motivacion: que al consumir `AppEditor` sin props adicionales, la UX sea tipo hoja real (A4) con margenes y salto entre paginas.
- Compatibilidad: donde se requiere el modo continuo, se debe pasar explicitamente `paginationMode="none"`.

### 2. Pegado de texto sin espacios (rutas) respeta margenes

- En modo paginado (`.editorContentPaged`), se agregan reglas CSS para permitir el wrap de cadenas largas:
  - `overflow-wrap: anywhere;`
  - `word-break: break-word;`
  - `hyphens: auto;`
- Motivacion: cuando se pega texto como rutas repetidas sin espacios, el navegador no puede envolver lineas sin estas reglas, lo que produce desbordes y aparenta "no respetar margenes" y dificulta el corte por altura.

### 3. Gestion Respuesta: surface usa AppEditor (no AppEditorPdf)

- En `GestionRespuestaMainTabContent` se reemplaza `AppEditorPdf` por `AppEditor`.
- Se remueve `AppSteps` del flujo visual de gestion de respuesta.
- El guardado se mantiene con `AppEditorSaveAction` + `useAppEditorSaveState`.

## Notas

- La paginacion es por layout (altura, tipografia, interlineado, imagenes, etc.), no por "cantidad fija de caracteres".
- Si un caso requiere editor continuo, debe declararse con `paginationMode="none"`.

