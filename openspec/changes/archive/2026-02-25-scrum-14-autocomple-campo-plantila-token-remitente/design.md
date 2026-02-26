## Context

En `RadicacionForm.tsx` existe el control con `data-ident="pl-radicacion-spe-REMITENTE_COR"` y actualmente no aprovecha de forma consistente la metadata de `camposPlantilla` para resolver autocompletado por `name_campo`. El requerimiento de `SCRUM-14` pide localizar el campo con `name_campo="REMITENTE_COR"` e implementar consulta de autocompletado contra `/api/PlantillaRadicado/autoCompleteTercero`.

Restricciones relevantes:
- Mantener arquitectura del modulo de radicacion sin introducir dependencias nuevas.
- Mantener TypeScript estricto y patrones actuales de hooks/servicios.
- Preservar accesibilidad y comportamiento declarativo (`data-ident`, `aria-*`).

Stakeholders: usuarios de radicacion y equipo funcional que depende de la seleccion guiada de remitente.

## Goals / Non-Goals

**Goals:**
- Resolver de forma deterministica el campo `REMITENTE_COR` desde `camposPlantilla` por `name_campo`.
- Habilitar autocompletado en ese campo usando `/api/PlantillaRadicado/autoCompleteTercero`.
- Mantener compatibilidad visual/comportamental con otros campos dinamicos de autocompletado.
- Dejar cobertura de pruebas para flujo exitoso y manejo de error.

**Non-Goals:**
- Rediseñar toda la seccion de remitente o el contrato completo de `camposPlantilla`.
- Cambiar endpoints existentes distintos a `autoCompleteTercero`.
- Modificar reglas de negocio de seleccion de destinatario o otros campos no relacionados.

## Decisions

### Decision 1: Resolver metadata por `name_campo` normalizado
- **Decision:** Buscar en `camposPlantilla` el registro cuyo `name_campo` sea `REMITENTE_COR`, con comparacion case-insensitive y trim para robustez.
- **Rationale:** `name_campo` es el identificador funcional estable dentro de la plantilla; evita acoplar la logica a labels o posiciones.
- **Alternatives considered:**
  - Buscar por `data-ident`: descartado porque el `data-ident` es derivado de UI y no fuente primaria de metadata.
  - Buscar por indice fijo: descartado por fragilidad ante cambios de plantilla.

### Decision 2: Reusar patron de autocompletado existente del modulo
- **Decision:** Integrar la consulta a `/api/PlantillaRadicado/autoCompleteTercero` reutilizando el mismo flujo de hook/servicio de autocompletado que ya usa radicacion para otros campos.
- **Rationale:** Minimiza riesgo de regresion, mantiene consistencia de manejo de loading/error y reduce codigo duplicado.
- **Alternatives considered:**
  - Llamado `fetch` inline en el componente: descartado por mezclar UI y acceso a datos.
  - Crear un flujo totalmente nuevo de estado: descartado por complejidad innecesaria.

### Decision 3: Mantener fallback de ingreso manual ante error de API
- **Decision:** Si la API falla o no retorna opciones, el usuario puede seguir digitando manualmente sin bloquear el formulario.
- **Rationale:** Evita interrupciones operativas y sigue el comportamiento tolerante ya usado en otros autocompletados.
- **Alternatives considered:**
  - Bloquear campo en error: descartado por impacto funcional negativo.

### Decision 4: Validar con pruebas de comportamiento en Vitest
- **Decision:** Agregar/ajustar tests en el modulo de radicacion para cubrir: match de `REMITENTE_COR`, invocacion del endpoint correcto y degradacion por error.
- **Rationale:** El problema es de integracion UI+metadata+API; pruebas de comportamiento detectan regresiones reales.
- **Alternatives considered:**
  - Solo pruebas unitarias aisladas del helper: insuficiente para garantizar integracion en formulario.

## Risks / Trade-offs

- **[Risk] Plantillas con variaciones de nombre/casing en `name_campo`** -> **Mitigation:** normalizacion (`trim` + `toUpperCase`) y test con variantes.
- **[Risk] Regresion en campos de autocompletado existentes** -> **Mitigation:** ejecutar suite de radicacion y mantener cambios acotados a `REMITENTE_COR`.
- **[Risk] Latencia/fallo del endpoint `autoCompleteTercero`** -> **Mitigation:** debouncing/reintento segun patron existente y fallback manual sin bloqueo.

## Migration Plan

1. Implementar localizacion del campo `REMITENTE_COR` desde `camposPlantilla`.
2. Conectar el campo a servicio/hook de `/api/PlantillaRadicado/autoCompleteTercero`.
3. Validar UI (opciones, seleccion, mensaje de error, fallback manual).
4. Ejecutar pruebas del modulo de radicacion y registrar evidencia en `tasks.md`.
5. Desplegar sin migraciones de datos (cambio solo frontend).

**Rollback:** revertir commit del cambio en el componente/hook de radicacion; no requiere rollback de base de datos.

## Open Questions

- Confirmar payload exacto esperado por `/api/PlantillaRadicado/autoCompleteTercero` para evitar transformaciones duplicadas.
- Validar si `REMITENTE_COR` debe soportar seleccion unica o multiples tokens en todos los clientes.
- Confirmar texto funcional de mensajes de error/placeholder para este campo.
