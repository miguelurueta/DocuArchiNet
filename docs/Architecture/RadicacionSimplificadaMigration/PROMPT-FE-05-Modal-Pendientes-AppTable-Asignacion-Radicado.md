# PROMPT ARQUITECTONICO - Radicacion Simplificada
# Fase FE-05 - Modal de pendientes con AppTable y asignacion de radicado

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Frontend senior especialista en:

- React 19 y TypeScript estricto;
- migracion quirurgica de componentes legacy hacia componentes compartidos;
- integracion de `AppTable` sobre AG Grid;
- consumo de APIs REST con contratos tipados;
- DynamicUiTable y acciones por fila;
- flujos transaccionales de radicacion documental;
- navegacion contextual post-registro;
- pruebas unitarias y de integracion frontend.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar el componente de lista de tareas/radicados pendientes dentro del modal existente de radicacion, reemplazando la tabla local/mock por `src/app/Components/UI/AppTable`.

Desde la tabla debe ser posible activar la asignacion/toma del radicado pendiente mediante la accion de fila:

```txt
asignacion-tarea
```

La accion no debe activar documentos por si sola. Debe llamar la API de toma/asignacion y solo despues de una respuesta exitosa con `estadoActual = 0` debe habilitar el contexto documental y navegar al panel `Documentos`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Documentos de arquitectura:

```txt
docs/Architecture/RadicacionSimplificadaMigration/Analisis-Migracion-Legacy-RadicadorSimplificado.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-02-Navegacion-Contextual-Post-Radicacion.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-03-Panel-Documental-Post-Radicacion.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-04-Pendientes-Radicacion-Gestion-Documental.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-BE-API-01-Listado-Radicados-Pendientes.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-BE-API-03-Contador-Pendientes-Radicacion.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-BE-API-05-Tomar-Radicado-Pendiente.md
```

Componentes y patrones existentes:

```txt
src/modules/radicacion/components/Modalpendiente.tsx
src/modules/radicacion/hooks/RadicacionTabs.tsx
src/modules/radicacion/components/RadicacionForm.tsx

src/app/Components/UI/AppTable/AppTable.tsx
src/app/Components/UI/AppTable/AppTable.types.ts
src/app/Components/UI/AppTable/AppTableQueryWrapper.tsx
src/app/Components/UI/AppTable/hooks/useDynamicUiTableQuery.ts
src/app/Components/UI/AppTable/hooks/useAppTableQueryState.ts
src/app/Components/UI/AppTable/adapters/appGridToAppTableColumns.ts
src/app/Components/UI/AppTable/adapters/appGridToAppTableRows.ts
src/app/Components/UI/AppTable/services/dynamicUiTable.service.ts

src/modules/gestionCorrespondencia/hooks/useGestionCorrespondenciaTable.ts
src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx
src/modules/gestionCorrespondencia/adapters/gestionCorrespondenciaTableRequestMapper.ts
```

Regla de referencia:

```txt
GestionCorrespondencia ya demuestra el patron:
useAppTableQueryState
  -> useDynamicUiTableQuery
  -> mapAppGridRowsToAppTableRows
  -> mapAppGridColumnsToAppTableColumns
  -> <AppTable onActionTriggered={...} />
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ESTADO ACTUAL DEL FRONTEND
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Archivo actual:

```txt
src/modules/radicacion/components/Modalpendiente.tsx
```

Problemas actuales:

- usa `antd/Table`;
- contiene datos mock hardcodeados;
- define columnas locales no ligadas al backend;
- el boton de opciones no ejecuta accion real;
- no consume listado moderno de pendientes;
- no valida estado activo `0`;
- no llama API de toma/asignacion;
- no refresca contador/lista;
- no integra el resultado con el contexto post-radicacion;
- no navega al panel documental tras asignacion exitosa.

El modal si esta conectado visualmente:

```txt
src/modules/radicacion/hooks/RadicacionTabs.tsx
  tabBarExtraContent={{ right: <ModalPendiente /> }}
```

Por tanto, la migracion debe conservar el punto de entrada visual y reemplazar la implementacion interna.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## DEPENDENCIAS BACKEND
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APIs requeridas:

```txt
GET  /api/tramite/tramites/apListaRadicadosPendientes
POST /api/tramite/tramites/apListaRadicadosPendientes
GET  /api/radicacion/pendientes/contador
POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar
```

Decision de consumo:

```txt
Usar POST /api/tramite/tramites/apListaRadicadosPendientes si BE-API-01 implementa paginacion/busqueda server para AppTable.
Usar GET /api/tramite/tramites/apListaRadicadosPendientes solo como compatibilidad temporal si el POST aun no existe.
```

El listado debe entregar, directa o indirectamente en la fila DynamicUiTable:

```txt
id_estado_radicado
consecutivo_radicado
remitente
tramite
fecha_registro
id_tarea_workflow
```

La accion de fila debe estar disponible como:

```txt
actionId = "asignacion-tarea"
```

Contrato esperado para tomar pendiente:

```ts
type TomarRadicadoPendienteRequestDto = {
  idTareaWorkflow?: number | null;
};

type TomarRadicadoPendienteResponseDto = {
  idEstadoRadicado: number;
  idRadicado?: number;
  consecutivoRadicado: string;
  idTareaWorkflow: number;
  estadoAnterior: 1;
  estadoActual: 0;
  requiereGestionDocumental: true;
  tieneTramiteDocumentalActivoEstado0: true;
  destinoPostRegistro: "documentos";
  contextoDocumental?: {
    idGabinete?: number | null;
    nombreGabinete?: string | null;
    idTipoTramite?: number | null;
    nombreTramite?: string | null;
    utilEstadoPendienteRad?: boolean;
  } | null;
  metadataOperativa: {
    tramite?: string;
    remitente?: string;
    plantillaId?: number;
    workflowFueCreado: boolean;
  };
};
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLA FUNCIONAL CRITICA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

```txt
Documentos permanece inactivo para consulta y para navegacion simple.
Solo se activa cuando el usuario toma/re-radica un pendiente y backend confirma estadoActual = 0.
```

Implicaciones:

- abrir el modal no activa documentos;
- consultar la lista no activa documentos;
- hacer click sobre una fila no activa documentos;
- solo `POST /tomar` exitoso activa documentos;
- si backend bloquea por tarea activa existente, el modal queda abierto y muestra el error;
- si backend devuelve un estado distinto de `0`, no navegar a `Documentos`.

Mensaje funcional de bloqueo esperado:

```txt
Tarea asignada para gestion y asignacion, debe terminar la tarea actual o subirla a estado pendiente para continuar con la asignacion.
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ARQUITECTURA OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Mantener compatibilidad de importacion del componente actual.

Opcion recomendada:

```txt
src/modules/radicacion/components/Modalpendiente.tsx
  exporta el componente existente, pero delega en implementacion nueva
```

Crear piezas tipadas:

```txt
src/modules/radicacion/types/radicacionPendientes.types.ts
src/modules/radicacion/services/radicacionPendientes.service.ts
src/modules/radicacion/adapters/radicacionPendientesTableRequestMapper.ts
src/modules/radicacion/hooks/useRadicacionPendientesTable.ts
src/modules/radicacion/hooks/useRadicacionPendientesContador.ts
src/modules/radicacion/hooks/useTomarRadicadoPendiente.ts
src/modules/radicacion/components/RadicacionPendientesModal.tsx
```

Si el repo ya tiene una convencion distinta en `src/modules/radicacion`, respetarla. No crear una segunda arquitectura paralela.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTRATO DEL HOOK DE TABLA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

El hook debe seguir el patron de `useGestionCorrespondenciaTable`.

Firma sugerida:

```ts
type RadicacionPendientesTableResult<T extends AppTableRow = AppTableRow> = {
  rows: T[];
  columns: ColDef<T>[];
  total: number;
  page: number;
  pageSize: number;
  queryState: AppTableQueryState;
  onQueryChange: (patch: Partial<AppTableQueryState>) => void;
  loading: boolean;
  error: Error | null;
  isEmpty: boolean;
  hasLoadedOnce: boolean;
  refetch: () => void;
};
```

Requisitos:

- usar `useAppTableQueryState`;
- usar `useDynamicUiTableQuery` si el endpoint responde como DynamicUiTable;
- mapear columnas con `mapAppGridColumnsToAppTableColumns`;
- mapear filas con `mapAppGridRowsToAppTableRows`;
- no declarar columnas manuales si backend ya entrega configuracion DynamicUiTable;
- soportar paginacion server;
- cargar al abrir el modal o bajo `enabled/open`, evitando traer toda la lista al montar `RadicacionTabs`;
- refrescar despues de una toma exitosa.

Identificador sugerido de tabla:

```txt
radicacionPendientes
```

Si backend ya define otro `tableId`, usar el real.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## IMPLEMENTACION DEL MODAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

El modal debe:

- conservar el boton visible `Pendientes`;
- mostrar contador si `GET /api/radicacion/pendientes/contador` esta disponible;
- abrir con una lista paginada server;
- renderizar `AppTable`, no `antd/Table`;
- usar `AppTableQueryWrapper` si se requiere paginacion/filtros;
- permitir busqueda si el endpoint DynamicUiTable la soporta;
- manejar loading, error, vacio y refrescar;
- mantener ancho adecuado para tabla operativa;
- cerrar solo por cancelacion del usuario o asignacion exitosa.

Uso esperado de `AppTable`:

```tsx
<AppTable
  rows={table.rows}
  columns={table.columns}
  total={table.total}
  loading={table.loading && table.hasLoadedOnce}
  paginationMode="server"
  layoutMode="fill"
  responsivePresentation={{ enabled: true, cardsBelow: 768 }}
  rowSelection="single"
  rowSelectionCheckboxes={false}
  rowSelectionHeaderCheckbox={false}
  onActionTriggered={handleTableAction}
/>
```

No usar:

```txt
antd/Table
datos mock
columnas hardcodeadas si backend entrega DynamicUiTable
acciones inline sin contrato tipado
any
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ACCION DE ASIGNACION DESDE LA TABLA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

La accion se recibe por:

```ts
onActionTriggered(params: AppTableActionTriggered<AppTableRow>)
```

Flujo:

```txt
1. normalizar actionId
2. aceptar solo "asignacion-tarea"
3. extraer id_estado_radicado
4. extraer id_tarea_workflow
5. extraer consecutivo_radicado para trazabilidad
6. bloquear doble click mientras mutation esta en curso
7. ejecutar POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar
8. validar respuesta estadoActual = 0
9. actualizar contexto post-radicacion
10. cerrar modal
11. refrescar contador/lista
12. navegar a Documentos
```

Resolver campos de fila de forma tolerante por nombres, sin usar `any`:

```txt
id_estado_radicado | idEstadoRadicado | IdEstadoRadicado
id_tarea_workflow  | idTareaWorkflow  | IdTareaWorkflow
consecutivo_radicado | consecutivoRadicado | ConsecutivoRadicado | RADICADO
```

Si falta `id_estado_radicado`, mostrar error funcional y no llamar API.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## NAVEGACION POST-ASIGNACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Despues de `tomar` exitoso:

```txt
/dashboard/radicacion/registro/{idEstadoRadicado}/documentos
```

El estado/contexto de radicacion debe recibir:

```ts
{
  idEstadoRadicado: response.idEstadoRadicado,
  idRadicado: response.idRadicado,
  consecutivoRadicado: response.consecutivoRadicado,
  idTareaWorkflow: response.idTareaWorkflow,
  estadoActual: 0,
  requiereGestionDocumental: true,
  tieneTramiteDocumentalActivoEstado0: true,
  destinoPostRegistro: "documentos",
  contextoDocumental: response.contextoDocumental,
  metadataOperativa: response.metadataOperativa
}
```

Este objeto debe ser compatible con el contexto restaurado por `GET /api/radicacion/pendientes/estado-activo`. FE-05 no debe crear un contexto paralelo al usado por FE-06.

Si el shell contextual de FE-02/FE-03 ya tiene provider/hook propio, integrarse ahi. No crear un segundo store global.

Si todavia no existe provider de contexto post-radicacion, dejar el punto de integracion tipado y probado, pero no activar `Documentos` por estado local aislado del modal.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## SERVICIO FRONTEND
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Crear service con responsabilidades atomicas:

```ts
getRadicacionPendientesTable(...)
getRadicacionPendientesContador(...)
tomarRadicadoPendiente(idEstadoRadicado, payload)
```

Reglas:

- usar el cliente HTTP existente del repo;
- respetar contratos `AppResponse` si el repo los usa;
- normalizar errores en una capa, no en el componente;
- no acoplar el service a `Modal`;
- no usar rutas ASMX ni endpoints legacy.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ESTADOS UI
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

El modal debe representar:

- cerrado;
- abierto cargando primera pagina;
- abierto con datos;
- abierto sin pendientes;
- abierto con error recuperable;
- asignando una fila;
- asignacion exitosa con cierre y navegacion;
- asignacion bloqueada por tarea activa existente.

Mientras una asignacion esta en curso:

- deshabilitar la accion de toma o bloquear reentradas;
- conservar la fila visible;
- no cerrar el modal hasta tener respuesta.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## PRUEBAS REQUERIDAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Crear pruebas enfocadas:

```txt
src/modules/radicacion/components/RadicacionPendientesModal.spec.test.tsx
src/modules/radicacion/hooks/useRadicacionPendientesTable.spec.test.ts
src/modules/radicacion/hooks/useTomarRadicadoPendiente.spec.test.ts
src/modules/radicacion/services/radicacionPendientes.service.test.ts
```

Casos minimos:

- renderiza boton `Pendientes`;
- no carga tabla antes de abrir el modal si se implementa lazy load;
- al abrir, consume listado de pendientes;
- renderiza `AppTable` con filas/columnas mapeadas;
- ejecuta `tomar` cuando `onActionTriggered.actionId = "asignacion-tarea"`;
- no ejecuta `tomar` para otra accion;
- extrae `id_estado_radicado` y `id_tarea_workflow` desde la fila;
- muestra error si falta `id_estado_radicado`;
- no navega si API falla;
- no navega si respuesta no confirma `estadoActual = 0`;
- cierra modal y navega a `/dashboard/radicacion/registro/{idEstadoRadicado}/documentos` si la toma es exitosa;
- refresca contador/lista despues de exito;
- respeta el bloqueo cuando backend informa que ya existe tarea activa.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CRITERIOS DE ACEPTACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- `Modalpendiente.tsx` ya no contiene datos mock.
- El modal usa `AppTable` desde `src/app/Components/UI/AppTable`.
- El listado viene de `POST /api/tramite/tramites/apListaRadicadosPendientes` si existe paginacion server; `GET` queda como compatibilidad temporal.
- El badge/contador viene de `GET /api/radicacion/pendientes/contador` si esta disponible.
- La accion `asignacion-tarea` llama `POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar`.
- La accion transporta `id_estado_radicado`, `id_tarea_workflow` y `consecutivo_radicado`.
- `Documentos` solo se activa con respuesta exitosa `estadoActual = 0`.
- Si ya existe una tarea activa, se muestra el mensaje backend y no se navega.
- El modal cierra y refresca estado solo despues de asignacion exitosa.
- No se introduce `antd/Table` para esta lista.
- No se introduce ASMX, jQuery, variables globales ni datos legacy hardcodeados.
- Hay pruebas que cubren tabla, accion, error, bloqueo y navegacion.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FUERA DE ALCANCE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

No implementar en este corte:

- carga real de documentos;
- digitalizacion;
- visor PDF;
- cambio de tipologia documental;
- envio a pendiente desde el formulario;
- endpoint backend nuevo;
- reemplazo completo del shell de radicacion;
- redisenio visual completo de `RadicacionTabs`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## NOTA DE MIGRACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Este prompt es el puente entre las APIs atomicas backend de pendientes y la experiencia operativa frontend.

Debe ejecutarse despues de:

```txt
BE-API-01 listado
BE-API-03 contador
BE-API-05 tomar pendiente
```

Puede ejecutarse antes de `enviar a pendiente` si la asignacion desde listado ya esta disponible. Sin embargo, el flujo completo de subir/bajar pendientes solo queda cerrado cuando tambien exista:

```txt
BE-API-04 enviar pendiente
```
