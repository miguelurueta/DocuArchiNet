## 1. Refinement

- [x] 1.1 Revisar Jira SCRUMCORE-287 y separar alcance de SCRUMCORE-284 tipologias workflow.
- [x] 1.2 Revisar integracion actual de Gestion Respuesta con `AppUploadDocumental`.
- [x] 1.3 Confirmar punto real de integracion: `loadGestionRespuestaUploadConfig`.
- [x] 1.4 Refinar `proposal.md`, `design.md`, `spec.md` y `tasks.md` antes de publish.

## 2. Types And Contract

- [x] 2.1 Crear `src/modules/gestionCorrespondencia/types/configuracionUploadCorrespondencia.types.ts`.
- [x] 2.2 Modelar `ConfiguracionUploadCorrespondenciaBackendItem` con PascalCase y camelCase.
- [x] 2.3 Modelar `ConfiguracionUploadCorrespondenciaResponse`.
- [x] 2.4 Modelar `ConfiguracionUploadCorrespondencia` con `nameProceso`, `accept`, `allowedExtensions` y `maxSizeBytes`.
- [x] 2.5 Mantener TypeScript estricto sin `any`.

## 3. Service Layer

- [x] 3.1 Crear `src/modules/gestionCorrespondencia/services/configuracionUploadCorrespondencia.service.ts`.
- [x] 3.2 Implementar `normalizeUploadExtensions(raw: string): string[]` como funcion pura.
- [x] 3.3 Implementar `getConfiguracionUploadCorrespondencia(options?: { signal?: AbortSignal })`.
- [x] 3.4 Consumir `/api/gestor-documental/configuracion-upload` con `nameProceso=CORRESPO` usando `clienteApi`.
- [x] 3.5 Normalizar respuestas PascalCase y camelCase.
- [x] 3.6 Seleccionar la primera fila activa (`EstadoProceso === 1` / `estadoProceso === 1`).
- [x] 3.7 Usar la primera fila como fallback si no existe fila activa pero hay data.
- [x] 3.8 Fallar con error funcional si `success=false`, `data=[]`, no hay extensiones usables o `LengUpload <= 0`.
- [x] 3.9 Soportar `AbortSignal`.

## 4. Hook State

- [x] 4.1 Crear `src/modules/gestionCorrespondencia/hooks/useConfiguracionUploadCorrespondencia.ts`.
- [x] 4.2 Exponer `config`, `loading`, `error`, `empty` y `reload`.
- [x] 4.3 Cargar al montar cuando `enabled=true`.
- [x] 4.4 No cargar cuando `enabled=false`.
- [x] 4.5 Usar `AbortController`.
- [x] 4.6 Ignorar respuestas stale.
- [x] 4.7 Permitir retry con `reload`.

## 5. Gestion Respuesta Integration

- [x] 5.1 Actualizar `gestionRespuestaUploadDocumental.service.ts` para dejar de usar extensiones/tamano hardcodeados como fuente final.
- [x] 5.2 Hacer que `loadGestionRespuestaUploadConfig` delegue en `getConfiguracionUploadCorrespondencia`.
- [x] 5.3 Preservar flags existentes: `multiple`, `requiereTipologia`, `requiereFechaCarga`, `fechaCargaObligatoria` y `validationMode`.
- [x] 5.4 Verificar que `AppUploadDocumental` recibe `accept` y `maxSizeBytes` desde backend por el contrato `loadConfig`.
- [x] 5.5 Mantener fuera de alcance tipologias, metadata por archivo, `renderMetadata`, chunks y payload de almacenamiento.
- [x] 5.6 Mantener fail-closed cuando no exista configuracion backend usable.

## 6. Tests

- [x] 6.1 Crear/ajustar `src/modules/gestionCorrespondencia/tests/configuracionUploadCorrespondencia.service.test.ts`.
- [x] 6.2 Cubrir request con `nameProceso=CORRESPO`.
- [x] 6.3 Cubrir normalizacion PascalCase y camelCase.
- [x] 6.4 Cubrir normalizacion de extensiones: trim, lowercase, punto faltante, vacios y duplicados.
- [x] 6.5 Cubrir seleccion de fila activa y fallback a primera fila.
- [x] 6.6 Cubrir errores por `success=false`, `data=[]`, extensiones vacias y `LengUpload <= 0`.
- [x] 6.7 Crear/ajustar `src/modules/gestionCorrespondencia/tests/useConfiguracionUploadCorrespondencia.test.tsx`.
- [x] 6.8 Cubrir loading, success, empty, error, disabled mode, reload, abort y stale response.
- [x] 6.9 Ajustar pruebas de `GestionRespuestaUploadDocumental` o loader para validar que `accept` y `maxSizeBytes` vienen del servicio.

## 7. Documentation

- [x] 7.1 Crear documentacion enterprise en `docs/Architecture/GestionCorrrespondecia/Integracion-AppUploadDocumental/`.
- [x] 7.2 Incluir metadata SCRUMCORE-287.
- [x] 7.3 Documentar endpoint, params, contrato, normalizacion, mapeo `ExtensionUpload -> accept` y `LengUpload -> maxSizeBytes`.
- [x] 7.4 Documentar estados loading/error/empty/retry y politica fail-closed.
- [x] 7.5 Documentar limites: no tipologias, no storage, no backend, no cambios shared innecesarios.
- [x] 7.6 Registrar evidencia de pruebas ejecutadas.

## 8. Validation And Close

- [x] 8.1 Ejecutar pruebas focales del servicio/hook/integracion.
- [x] 8.2 Ejecutar validacion TypeScript/lint disponible si aplica al alcance.
- [x] 8.3 Ejecutar `openspec validate scrumcore-287-configuracion-upload-gestioncorrespondencia --strict`.
- [x] 8.4 Confirmar sin `any` nuevo.
- [x] 8.5 Confirmar backend no modificado.
- [x] 8.6 Confirmar endpoints no modificados.
- [x] 8.7 Confirmar que `AppUploadDocumental`, `AppUploadBatchView` y `AppUpload` no fueron reemplazados.
