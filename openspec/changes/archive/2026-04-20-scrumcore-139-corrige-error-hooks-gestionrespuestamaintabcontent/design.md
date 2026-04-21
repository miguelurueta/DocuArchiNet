## Context

La pantalla `GestionRespuestaMainTabContent` presenta un error de `build/lint` por redeclaración de hooks (ej. `useEffect`) debido a imports duplicados en el archivo:

- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`

El objetivo del cambio es dejar el archivo/pantalla sin errores de `build`/`lint` en el pipeline del proyecto, sin introducir cambios funcionales.

## Goals / Non-Goals

**Goals:**
- Eliminar redeclaraciones por imports duplicados (principalmente desde `"react"`).
- Dejar el archivo/pantalla sin errores de `build`/`lint` asociados al cambio (imports duplicados, unused imports/vars, orden de imports, tipos, etc.).
- Mantener el comportamiento funcional intacto (cambio puramente estructural/estático).

**Non-Goals:**
- Refactor de lógica de negocio, UX o cambios de comportamiento de la pantalla.
- Cambios de arquitectura de módulos/rutas.
- Resolver problemas de tooling/entorno local (por ejemplo políticas de PowerShell, permisos del sistema, `spawn EPERM` de esbuild) si no ocurren en CI.

## Decisions

- **Corrección mínima y localizada al archivo afectado.**
  - Remover el import duplicado desde `"react"` y mantener un único import para `useEffect`, `useId`, `useState` (y cualquier hook requerido).

- **Normalizar imports si el linter lo exige.**
  - Si existen imports duplicados desde el mismo paquete (por ejemplo `@ant-design/icons`), consolidarlos para evitar warnings/errores de lint.
  - Evitar cambios de orden/estilo si no son necesarios para pasar lint.

- **Validación en pipeline.**
  - La definición de “sin errores” es la ejecución exitosa de los comandos del pipeline del proyecto (lint/build/test según aplique).

## Risks / Trade-offs

- **Riesgo:** Al corregir imports duplicados aparezcan nuevos errores de lint (unused imports/vars, reglas de ordenamiento).
  - **Mitigación:** Ejecutar el set de validaciones del proyecto y ajustar únicamente lo necesario para cumplirlas.

- **Riesgo:** El error inicial enmascare otros errores existentes previos en la misma pantalla.
  - **Mitigación:** Mantener el scope limitado a “archivo/pantalla” y documentar errores preexistentes fuera del cambio.

- **Trade-off:** Consolidar imports puede tocar líneas no directamente relacionadas.
  - **Mitigación:** Mantener el diff pequeño y mecánico (solo consolidación / deduplicación).

