## 1. Refinement

- [x] 1.1 Confirmar contrato real de gabinete (source of truth):
  - `Radicado` (string) y reglas de normalización (`trim()`)
  - `NombreGabinete`
  - `EstadoExistenciaRadicado` (string) y valores reales (ej. `"SI"|"NO"` o `"YES"|"NO"`).
- [x] 1.2 Unificar validación de `EstadoExistenciaRadicado`:
  - Regla: bloquear query si el valor es `"NO"` (case-insensitive) o equivalente confirmado por contrato.
- [x] 1.3 Confirmar contrato del query: requiere `CampoRadicado="ENLASE"` + `Radicado` (y NO `Search`) vs `StructuredFilters`.
- [x] 1.4 Alinear mensajes de error funcional con producto (copy exacto).

## 2. Implementacion

- [x] 2.1 Extender/ajustar tipos del response de gabinete (`solicitaGabineteRadicadoWorkflow.types.ts`) para leer:
  - `Radicado?: string`
  - `NombreGabinete?: string`
  - `IdTareaWorkflow?: number`
  - `EstadoExistenciaRadicado?: string` (validación case-insensitive contra `"NO"` según contrato confirmado)
- [x] 2.2 Actualizar mapper root query `gestionRespuestaDocumentosRequestMapper.ts`:
  - aceptar `nombreGabinete?: string` y `radicado?: string` (radicado proviene del gabinete)
  - set `CampoRadicado="ENLASE"`
  - set `Radicado=<trim>` (si no existe: NO ejecutar query desde hook)
  - mantener `NombreGabinete` existente
  - NO usar `Search` como sustituto silencioso de `Radicado`
- [x] 2.3 Actualizar hook `useGestionRespuestaDocumentosTable.ts`:
  - resolver gabinete por `idTareaWf`
  - validar `Radicado` obligatorio (error funcional controlado si falta)
  - validar `EstadoExistenciaRadicado` (si `"NO"` -> no query + error controlado)
  - anti-stale: solo última ejecución aplica estado (nonce/seq)
  - limpiar estado al cambiar `idTareaWf` (evitar render stale)
- [x] 2.4 Mantener compatibilidad (no regresión):
  - `loadChildren` sigue consultando con contexto consistente
  - selección múltiple no se rompe
  - `ver_documento` sin cambios de contrato
  - `AppTreeTable`/`AppTable` sin cambios de API/contrato

## 3. Pruebas

- [x] 3.1 Unit (mapper): request incluye `CampoRadicado="ENLASE"` y `Radicado` trim; NO usa `Search` como fallback.
- [x] 3.2 Unit (hook): no query cuando Radicado vacío; no query cuando `EstadoExistenciaRadicado` bloquea (ej. `"NO"`); query cuando radicado ok.
- [x] 3.3 Concurrencia: simular `idTareaWf` A->B y validar que A no sobrescribe estado.
- [x] 3.4 Integración UI (vitest + RTL): cambio de tarea A->B no mantiene rows stale (sin mostrar docs previos durante carga).
- [x] 3.5 E2E Playwright (entorno real, si está configurado):
  - tareas distintas muestran documentos distintos
  - Radicado vacío -> error controlado (sin query)
  - selección múltiple intacta
  - `ver_documento` intacto
- [x] 3.6 Ejecutar `npm.cmd test` focal del módulo y registrar evidencia.

## 4. Cierre

- [x] 4.1 `npm.cmd run spec:validate`
- [x] 4.2 Documentación enterprise (obligatoria)
  - Ruta: `docs/modulos/gestioncorrespondencia/AppTreeTable/CorrecionListadoRepetidoDocumentos/`
  - Archivos:
    - `SCRUMCORE-230-Arquitectura.md`
    - `SCRUMCORE-230-Implementacion-Detallada.md`
    - `SCRUMCORE-230-Integracion-BackEnd.md` (matriz FE-BE + ejemplos payload)
    - `SCRUMCORE-230-Pruebas.md`
    - `SCRUMCORE-230-Metadata.md`
