# SCRUMCORE-230 — Implementación Detallada

## 1) Archivos tocados (rutas reales)

**Tipos**
- `src/modules/gestionCorrespondencia/types/solicitaGabineteRadicadoWorkflow.types.ts`

**Mapper / payloads**
- `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.ts`

**Orquestación / hook**
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`

**UI (validación integración)**
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx` (sin cambios funcionales del ticket; solo consume el hook)

## 2) Fix #1 — Tipos gabinete (source of truth)
Objetivo: poder leer desde el FE los campos que gobiernan el filtro.

En `solicitaGabineteRadicadoWorkflow.types.ts` se soporta (según respuesta BE):
- `NombreGabinete`
- `Radicado`
- `EstadoExistenciaRadicado`
- `IdTareaWorkflow`

Decisión:
- La validación se hace en el hook; el tipo se mantiene permissive (`?:`) porque el backend puede omitir campos en errores o escenarios edge.

## 3) Fix #2 — Mapper de `ListaDocumentosRadicados/query` (filtro real)
Objetivo: garantizar que el request tenga el filtro de radicado **de manera contractual**.

Cambios en `gestionRespuestaDocumentosRequestMapper.ts`:
- Root query (`buildListaDocumentosRadicadosRootQuery`):
  - `CampoRadicado` se fija siempre a `"ENLASE"`.
  - `Radicado` se envía como `radicado.trim()`.
  - `Search` NO se usa como reemplazo silencioso de `Radicado`.
- Children query (`buildListaDocumentosRadicadosChildrenQuery`):
  - preserva `radicado` cuando aplica para que la jerarquía no “salte” a datos generales.

Resultado:
- Sin `Radicado`, el hook no ejecuta el query (ver sección 4).
- Con `Radicado`, el payload fuerza el filtro en backend por contrato.

## 4) Fix #3 — Hook (validación estricta + anti-stale + estabilidad UX)
Objetivo: que el listado solo consulte cuando tiene contexto válido y que nunca aplique resultados de otra tarea.

### 4.1) Source of truth
En `load()`:
1. Valida `idTareaWf` (> 0).
2. Llama `getSolicitaGabinetePorTareaWorkflow(idTareaWf)`.
3. Obtiene:
   - `nombreGabinete`
   - `radicado`
   - `estadoExistenciaRadicado`

### 4.2) Validación estricta antes de consultar documentos
Si `idTareaWf` es válido:
- `radicado.trim()` vacío:
  - NO consulta `ListaDocumentosRadicados/query`
  - retorna error funcional controlado:
    - `"No fue posible cargar documentos: el radicado de la tarea es obligatorio."`
- `EstadoExistenciaRadicado === "NO"` (case-insensitive):
  - NO consulta `ListaDocumentosRadicados/query`
  - retorna error funcional controlado:
    - `"No fue posible cargar documentos: el radicado no existe para la tarea."`

### 4.3) Anti-stale (concurrencia)
Se usa `loadSeqRef`:
- al entrar a `load()`: `const seq = ++loadSeqRef.current`
- después de cada await relevante:
  - si `seq !== loadSeqRef.current`, el resultado es obsoleto.

### 4.4) “Carga cancelada por cambio de tarea” (por qué aparecía y cómo se corrigió)
**Qué estaba pasando**
- Cuando el usuario navega/cambia de tarea, el componente puede disparar un `load()` mientras simultáneamente cambia el `idTareaWf`.
- Si un `load()` queda obsoleto, el hook devolvía:
  - `{ ok:false, message:"Carga cancelada por cambio de tarea." }`
- `AppTreeTable` interpretaba eso como error visible.

**Corrección aplicada**
Para evitar:
- el flash de error,
- y el estado “Sin documentos adjuntos” por limpiar rows,
se agregó un buffer de “último éxito”:
- `lastSuccessfulRowsRef: useRef<AppTreeTableRow[]>([])`

Regla:
- Si un `load()` queda obsoleto (`seq !== loadSeqRef.current`), se retorna:
  - `{ ok:true, rows: lastSuccessfulRowsRef.current }`

Con esto:
- La UI no muestra error por cancelación.
- La tabla no se queda vacía por una cancelación intermedia.
- El siguiente `load()` (tarea nueva) sí traerá los documentos reales del backend.

### 4.5) Limpieza al cambiar tarea
En `useEffect([idTareaWf])` se resetean:
- `latestRowRef` (cache de rows DTO)
- `gabineteRef` (contexto de gabinete)
- columnas, selección y contadores
- `lastSuccessfulRowsRef.current = []` para no “arrastrar” docs de otra tarea cuando ya cambió la tarea.

Nota:
- Se evita incrementar `loadSeqRef` dentro del effect para no generar cancelaciones falsas cuando `load()` ocurre antes de que el effect corra.

## 5) Selectores / CSS
Este ticket no introduce cambios de estilo global. Cualquier ajuste visual previo queda fuera del alcance de este documento.

