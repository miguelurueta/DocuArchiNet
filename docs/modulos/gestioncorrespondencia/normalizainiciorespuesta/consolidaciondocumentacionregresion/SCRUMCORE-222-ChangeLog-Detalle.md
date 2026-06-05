# SCRUMCORE-222 — ChangeLog Detallado (Implementación enterprise)

## Objetivo

Registrar de forma explícita y accionable cada cambio de implementación aplicado en el ticket **SCRUMCORE-222** (Consolidación, hardening, regresión y documentación de `GestionRespuesta`), con trazabilidad de:

- Archivo
- Componente funcional
- Cambio realizado
- Motivo técnico
- Impacto funcional
- Riesgo mitigado
- Artefacto de prueba asociado

> Este documento complementa los archivos principales de documentación enterprise del ticket y está pensado para auditoría técnica.

---

## Alcance confirmado del ticket

- Consolidar el refactor transversal iniciado en SCRUMCORE-219/220/221.
- Hardening sin cambios funcionales nuevos.
- Mantener contratos e endpoints.
- Validar estabilidad de:
  - `GestionRespuestaDocumentosContext`
  - `useGestionRespuestaDocumentos*`
  - `useListaDocumentosRadicadosTreeTable`
  - `DocumentosWorkbench`
- Alinear pruebas unitarias y regresión sin introducir deuda funcional.

---

## 1) Archivos modificados (core del ticket)

### 1.1 Contexto transversal documental

- `src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx`
  - **Cambio**:
    - Se extendió el contrato del contexto para exponer de forma transversal los datos de:
      - `idTareaWf`
      - `radicado`
      - `idRespuestaRadicado`
      - `nombreGabinete`
      - `gabineteLoading`
      - `gabineteError`
      - `reloadGabinete`
    - Se mantuvieron explícitamente `files` y `setFiles`.
  - **Motivo técnico**:
    - Evitar resolución local y duplicada de estado documental compartido.
  - **Impacto**:
    - Unificó el punto de acceso documental transversal para documentos/visor/acciones.
  - **Riesgo mitigado**:
    - Estado fragmentado entre capas.
    - Inconsistencia de gabinete entre consumidores.

- `src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx`
  - **Cambio**:
    - Se implementó estado interno para nombre de gabinete con ciclo de carga controlado:
      - bandera `gabineteLoading`.
      - manejo de `gabineteError`.
      - cancelación de request en cambios/re-montajes para evitar condiciones de carrera.
      - función `reloadGabinete` memoizada.
  - **Motivo técnico**:
    - Reglas de idempotencia del contexto y soporte de reintento seguro.
  - **Impacto**:
    - Menos requests redundantes y mejor estabilidad de estado.
  - **Riesgo mitigado**:
    - Doble fetch, estados stale, memory leaks.

### 1.2 Hook de consumidor de contexto

- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentos.ts`
  - **Cambio**:
    - Se ajustó firma/camino de consumo de estado documental ampliado.
    - Se mantuvo compatibilidad con uso preexistente.
  - **Motivo técnico**:
    - Forzar que consumidores lean estado de contexto y no del servicio directo.
  - **Impacto**:
    - Desacopla componentes de datos de transporte.
  - **Riesgo mitigado**:
    - Acoplamiento a implementación de transporte.

### 1.3 Hook de tabla de documentos / árbol documental

- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
  - **Cambio**:
    - Ajustes de estabilidad de integración con contexto transversal (alineación de estado derivado/propagación documental).
    - Validaciones de flujo compartido con estado de gabinete ya en contexto.
  - **Motivo técnico**:
    - Mantener coherencia entre árbol/tablas documentales y estado transversal.
  - **Impacto**:
    - Conserva operaciones documentales sin resolver gabinete local.
  - **Riesgo mitigado**:
    - Divergencia del mismo estado entre componentes distintos.

- `src/modules/gestionCorrespondencia/hooks/useListaDocumentosRadicadosTreeTable.ts`
  - **Cambio**:
    - Eliminación de resolución local de gabinete.
    - Consumo explícito de `useGestionRespuestaDocumentos()` para:
      - `nombreGabinete`
      - `gabineteLoading`
      - `gabineteError`
      - `reloadGabinete`
    - Conservación de contrato del hook (`load`, `loadChildren`, `rows`, `actions`, flags de estado).
    - Validación de estado de gabinete para acciones dependientes (ej. acción `ver_documento`).
  - **Motivo técnico**:
    - Cumplir rule: única fuente de verdad para gabinete en contexto transversal.
  - **Impacto**:
    - Se evita fetch duplicado de gabinete y se evita divergencia en acciones de documento.
  - **Riesgo mitigado**:
    - Doble fetch + mismatch entre acción y datos visibles.

### 1.4 Page/Wiring de `GestionRespuesta`

- `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx` *(si aplica en el flujo final del PR)*
  - **Cambio**:
    - Wiring del provider con los valores fuente de verdad:
      - `idTareaWf`
      - `radicado`
      - `idRespuestaRadicado`
    - Contexto inicializado como capa de orquestación documental transversal.
  - **Motivo técnico**:
    - Corregir origen del estado transversal evitando cálculos en consumidores.
  - **Impacto**:
    - Estado documental consistente desde origen al árbol/visor/acciones.
  - **Riesgo mitigado**:
    - Origen de estado “múltiple” en diferentes componentes.

### 1.5 Servicios y dependencias de gabinete

- `src/modules/gestionCorrespondencia/services/solicitaGabineteRadicadoWorkflow.service.ts`
  - **Cambio**:
    - Se confirmó uso centralizado por contexto para la resolución de gabinete (sin resolución ad-hoc en hooks de tabla).
  - **Motivo técnico**:
    - Separación de responsabilidades: servicio como ejecutor de request; contexto/consumer como poseedor de estado.
  - **Impacto**:
    - Consistencia de endpoint de gabinete y reducción de puntos de mantenimiento.
  - **Riesgo mitigado**:
    - Inconsistencia por distintos handlers o parámetros de llamada.

---

## 2) Casos de negocio tocados y controles de regresión

### 2.1 `ver_documento` y contexto de gabinete

- **Antes**: acción validaba/obtenía gabinete desde hook documental local.
- **Después**:
  - consulta `nombreGabinete` y estado de carga/error desde contexto.
  - si el gabinete no está disponible, retorna control de error funcional sin romper render.
  - no altera selección/estado de árbol al fallar dependencia auxiliar.
- **Resultado técnico**:
  - acción robusta ante latencia/errores de gabinete.

### 2.2 Mantener contrato de hook

- **Antes**: mismo contrato funcionalmente expuesto.
- **Después**: se conserva interfaz `load`, `loadChildren`, `loading`, `error`, `rows`, `actions`.
- **Resultado**:
  - compatibilidad con `AppTreeTable` y consumo existente.

### 2.3 Estado de archivos adjuntos

- `files` y `setFiles` quedan intactos en el contexto.
- Los flujos de adjuntos no ven cambios funcionales por el hardening.

---

## 3) Pruebas y evidencia asociada

### 3.1 Unitarias (ticket 222)

- `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`
  - Ajustes para validar integración con contexto transversal (estado de gabinete, errores, reintentos, render de tabla/documentos).
- `src/modules/gestionCorrespondencia/tests/useListaDocumentosRadicadosTreeTable.test.tsx`
  - Ajustes para validar que:
    - el hook ya no depende de resolución local de gabinete.
    - consume contexto y mantiene contrato de exportación.

### 3.2 Pruebas estáticas/compilación

- `tsc --noEmit`: OK para el alcance verificado.
- `eslint` sobre archivos modificados: OK.
- `npm run build`: bloqueado por deuda previa de tipados de dependencias de terceros (no atribuible al flujo de este ticket).
- `E2E`: pendientes por configuración de entorno (variables Playwright de auth no disponibles).

---

## 4) Trazabilidad a tareas del ticket (refinada por objetivo SCRUMCORE-222)

| Tarea | Estado | Evidencia |
|---|---|---|
| A. Auditoría baseline y riesgos | ✅ | Documento de arquitectura + matriz de pruebas |
| B. Hardening de estado/contexto | ✅ | Cambios en contexto + hook + tabla |
| B.1 Cancelación/idempotencia cacheo gabinete | ✅ | Estado `gabineteLoading/gabineteError` + `reloadGabinete` |
| B.2 Centralización de gabinete en contexto | ✅ | `useListaDocumentosRadicadosTreeTable` consume `useGestionRespuestaDocumentos` |
| B.3 Sin lógica de negocio nueva | ✅ | Conservación de contratos funcionales y endpoints |
| C. Cierre doc enterprise | ✅ | 5 artifacts principales + este changelog detallado |
| D. Validación regresión unidad/integración | ✅ (ejecutado en alcance unitario) | pruebas unitarias + lint + tsc |
| D.1 E2E completo y responsive | ⏳ | Pendiente de env/config |

---

## 5) Conformidad con restricciones del prompt

- No `any` nuevo introducido en cambios de hardening.
- No cambio de endpoints ni contratos funcionales públicos.
- No cambio de UI funcional deliberado.
- No duplicación de fetch de gabinete en hooks documentales.
- Contexto limitado a estado documental transversal (sin volverse “god context”).
- Error controlado sin romper render ni flujo del visor/árbol.

---

## 6) Pendientes explícitos post-cierre

1. Ejecutar E2E completo con variables de entorno de Playwright presentes.
2. Confirmar en entorno real móvil/tablet el ciclo de carga y retry de gabinete con red real.
3. Ejecutar una validación completa de regresión transversal en rama `main` post-merge.

---

## 7) Enlaces de control de calidad

- Documento principal de arquitectura: `SCRUMCORE-222-Arquitectura.md`
- Implementación detallada: `SCRUMCORE-222-Implementacion-Detallada.md`
- Integración backend: `SCRUMCORE-222-Integracion-BackEnd.md`
- Pruebas: `SCRUMCORE-222-Pruebas.md`
- Metadata: `SCRUMCORE-222-Metadata.md`
- Change-set (openspec): `openspec/changes/scrumcore-222...` (delta de change)

