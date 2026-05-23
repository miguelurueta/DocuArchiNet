## 1. Refinement

- [ ] 1.1 Confirmar contrato real de gabinete (source of truth):
  - `Radicado` (string) y reglas de normalizaciÃ³n (`trim()`)
  - `NombreGabinete`
  - `EstadoExistenciaRadicado` (string) y valores reales (ej. `"SI"|"NO"` o `"YES"|"NO"`).
- [ ] 1.2 Unificar validaciÃ³n de `EstadoExistenciaRadicado`:
  - Regla: bloquear query si el valor es `"NO"` (case-insensitive) o equivalente confirmado por contrato.
- [ ] 1.3 Confirmar contrato del query: requiere `CampoRadicado="ENLASE"` + `Radicado` (y NO `Search`) vs `StructuredFilters`.
- [ ] 1.4 Alinear mensajes de error funcional con producto (copy exacto).

## 2. Implementacion

- [ ] 2.1 Extender/ajustar tipos del response de gabinete (`solicitaGabineteRadicadoWorkflow.types.ts`) para leer:
  - `Radicado?: string`
  - `NombreGabinete?: string`
  - `IdTareaWorkflow?: number`
  - `EstadoExistenciaRadicado?: string` (validaciÃ³n case-insensitive contra `"NO"` segÃºn contrato confirmado)
- [ ] 2.2 Actualizar mapper root query `gestionRespuestaDocumentosRequestMapper.ts`:
  - aceptar `nombreGabinete?: string` y `radicado?: string` (radicado proviene del gabinete)
  - set `CampoRadicado="ENLASE"`
  - set `Radicado=<trim>` (si no existe: NO ejecutar query desde hook)
  - mantener `NombreGabinete` existente
  - NO usar `Search` como sustituto silencioso de `Radicado`
- [ ] 2.3 Actualizar hook `useGestionRespuestaDocumentosTable.ts`:
  - resolver gabinete por `idTareaWf`
  - validar `Radicado` obligatorio (error funcional controlado si falta)
  - validar `EstadoExistenciaRadicado` (si `"NO"` -> no query + error controlado)
  - anti-stale: solo Ãºltima ejecuciÃ³n aplica estado (nonce/seq)
  - limpiar estado al cambiar `idTareaWf` (evitar render stale)
- [ ] 2.4 Mantener compatibilidad (no regresiÃ³n):
  - `loadChildren` sigue consultando con contexto consistente
  - selecciÃ³n mÃºltiple no se rompe
  - `ver_documento` sin cambios de contrato
  - `AppTreeTable`/`AppTable` sin cambios de API/contrato

## 3. Pruebas

- [ ] 3.1 Unit (mapper): request incluye `CampoRadicado="ENLASE"` y `Radicado` trim; NO usa `Search` como fallback.
- [ ] 3.2 Unit (hook): no query cuando Radicado vacÃ­o; no query cuando `EstadoExistenciaRadicado` bloquea (ej. `"NO"`); query cuando radicado ok.
- [ ] 3.3 Concurrencia: simular `idTareaWf` A->B y validar que A no sobrescribe estado.
- [ ] 3.4 IntegraciÃ³n UI (vitest + RTL): cambio de tarea A->B no mantiene rows stale (sin mostrar docs previos durante carga).
- [ ] 3.5 E2E Playwright (entorno real, si estÃ¡ configurado):
  - tareas distintas muestran documentos distintos
  - Radicado vacÃ­o -> error controlado (sin query)
  - selecciÃ³n mÃºltiple intacta
  - `ver_documento` intacto
- [ ] 3.6 Ejecutar `npm.cmd test` focal del mÃ³dulo y registrar evidencia.

## 4. Cierre

- [ ] 4.1 `npm.cmd run spec:validate`
- [ ] 4.2 DocumentaciÃ³n enterprise (obligatoria)
  - Ruta: `docs/modulos/gestioncorrespondencia/AppTreeTable/CorrecionListadoRepetidoDocumentos/`
  - Archivos:
    - `SCRUMCORE-230-Arquitectura.md`
    - `SCRUMCORE-230-Implementacion-Detallada.md`
    - `SCRUMCORE-230-Integracion-BackEnd.md` (matriz FE-BE + ejemplos payload)
    - `SCRUMCORE-230-Pruebas.md`
    - `SCRUMCORE-230-Metadata.md`
