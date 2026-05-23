## 1. Refinement

- [ ] 1.1 Confirmar contrato real de gabinete: `Radicado` y `EstadoExistenciaRadicado` (tipo/campos exactos).
- [ ] 1.2 Confirmar contrato del query: requiere `CampoRadicado="ENLASE"` + `Radicado` vs `StructuredFilters`.
- [ ] 1.3 Alinear mensajes de error funcional con producto (copy exacto).

## 2. Implementacion

- [ ] 2.1 Extender/ajustar tipos del response de gabinete (`solicitaGabineteRadicadoWorkflow.types.ts`) para leer `Radicado` + `EstadoExistenciaRadicado`.
- [ ] 2.2 Actualizar mapper root query `gestionRespuestaDocumentosRequestMapper.ts`:
  - set `CampoRadicado="ENLASE"`
  - set `Radicado=<trim>` (source of truth: gabinete)
  - mantener `NombreGabinete` existente
- [ ] 2.3 Actualizar hook `useGestionRespuestaDocumentosTable.ts`:
  - validar `Radicado` obligatorio
  - validar `EstadoExistenciaRadicado !== "NO"`
  - anti-stale: solo Ãºltima ejecuciÃ³n aplica estado (nonce/seq)
  - limpiar estado al cambiar `idTareaWf`
- [ ] 2.4 Mantener compatibilidad:
  - `loadChildren` sigue consultando con contexto consistente
  - selecciÃ³n mÃºltiple no se rompe
  - `ver_documento` sin cambios de contrato

## 3. Pruebas

- [ ] 3.1 Unit (mapper): request incluye `CampoRadicado="ENLASE"` y `Radicado` trim; no usa `Search` como fallback.
- [ ] 3.2 Unit (hook): no query cuando Radicado vacÃ­o; no query cuando EstadoExistenciaRadicado="NO"; query cuando radicado ok.
- [ ] 3.3 Concurrencia: simular `idTareaWf` A->B y validar que A no sobrescribe estado.
- [ ] 3.4 Ejecutar `npm.cmd test` focal del mÃ³dulo y registrar evidencia.

## 4. Cierre

- [ ] 4.1 `npm.cmd run spec:validate`
- [ ] 4.2 DocumentaciÃ³n enterprise (ruta a confirmar por el repo/ticket):
  - Arquitectura
  - ImplementaciÃ³n detallada
  - IntegraciÃ³n FE-BE (payloads)
  - Pruebas
  - Metadata
