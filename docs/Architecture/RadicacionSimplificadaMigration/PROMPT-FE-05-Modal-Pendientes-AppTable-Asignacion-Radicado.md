# PROMPT ARQUITECTÓNICO - Radicación Simplificada

# FE-05 - Flujo Transaccional de Pendientes mediante AppTable

---

# Contexto Arquitectónico

Esta fase debe implementarse respetando las decisiones adoptadas previamente:

- TD-FE-01 → Single Source of Truth para carga de datos.
- TD-FE-02 → RadicacionDocumentalContext único.
- FE-06 → Startup Guard responsable del bootstrap.
- TD-FE-04 → Navegación centralizada y eliminación de UI de prototipo.

No crear arquitecturas paralelas.

No duplicar estado documental.

---

# Objetivo

Migrar el modal de pendientes hacia la infraestructura estándar del proyecto utilizando `AppTable`, implementando el flujo transaccional de asignación de un radicado pendiente.

El objetivo no es reemplazar una tabla, sino integrar completamente el flujo:

```text
Listado → Selección → Asignación → Actualización del Context → Navegación
```

---

# Objetivo Arquitectónico

El modal deja de contener lógica de negocio.

Su única responsabilidad será representar el flujo operativo.

Toda la lógica de asignación debe vivir en hooks y servicios especializados.

---

# Problema Actual

Archivo actual:

```text
src/modules/radicacion/components/Modalpendiente.tsx
```

Problemas actuales:

- usa `antd/Table`;
- contiene datos mock hardcodeados;
- define columnas locales no ligadas al backend;
- el botón de opciones no ejecuta acción real;
- no consume listado moderno de pendientes;
- no valida estado activo `0`;
- no llama API de toma/asignación;
- no refresca contador/lista;
- no integra el resultado con el contexto documental;
- no navega al panel documental tras asignación exitosa.

El modal está conectado visualmente desde:

```text
src/modules/radicacion/hooks/RadicacionTabs.tsx
  tabBarExtraContent={{ right: <ModalPendiente /> }}
```

Por tanto, la migración debe conservar el punto de entrada visual y reemplazar la implementación interna.

---

# Flujo Arquitectónico

```text
Usuario

↓

Modal Pendientes

↓

AppTable

↓

onActionTriggered()

↓

useTomarRadicadoPendiente()

↓

radicacionPendientes.service

↓

Backend

↓

RadicacionDocumentalContext

↓

Router / rutas centralizadas

↓

Documentos
```

---

# Regla Funcional

Documentos solamente puede habilitarse cuando backend confirme:

```text
estadoActual == 0
```

No antes.

No por navegación.

No por selección de fila.

No por abrir el modal.

No por consultar el listado.

Si backend devuelve un estado distinto de `0`, no se actualiza el contexto como activo y no se navega a Documentos.

---

# Componentes

Mantener compatibilidad de importación:

```text
Modalpendiente.tsx
```

Internamente podrá delegar en:

```text
RadicacionPendientesModal.tsx
```

---

# Componentes Esperados

```text
types/
    radicacionPendientes.types.ts

services/
    radicacionPendientes.service.ts

adapters/
    radicacionPendientesTableRequestMapper.ts

hooks/
    useRadicacionPendientesTable.ts

hooks/
    useRadicacionPendientesContador.ts

hooks/
    useTomarRadicadoPendiente.ts

components/
    RadicacionPendientesModal.tsx
```

No crear una arquitectura paralela.

Si el repo ya tiene una convención distinta en `src/modules/radicacion`, respetarla.

---

# Contratos Backend de Referencia

APIs requeridas:

```text
GET  /api/tramite/tramites/apListaRadicadosPendientes
POST /api/tramite/tramites/apListaRadicadosPendientes
GET  /api/radicacion/pendientes/contador
POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar
```

Decisión de consumo:

```text
Usar POST /api/tramite/tramites/apListaRadicadosPendientes si BE-API-01 implementa paginación/búsqueda server para AppTable.

Usar GET /api/tramite/tramites/apListaRadicadosPendientes solo como compatibilidad temporal si el POST aún no existe.
```

El listado debe entregar, directa o indirectamente en la fila DynamicUiTable:

```text
id_estado_radicado
consecutivo_radicado
remitente
tramite
fecha_registro
id_tarea_workflow
```

La acción de fila debe estar disponible como:

```text
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

No modificar contratos backend desde esta fase.

---

# Responsabilidades

## Modal

Responsable únicamente de:

- abrir;
- cerrar;
- representar estados;
- mostrar AppTable.

No ejecuta reglas de negocio.

---

## AppTable

Responsable únicamente de:

- render;
- acciones;
- paginación;
- selección.

No conoce Radicación.

No conoce Pendientes.

No conoce Documentos.

---

## useTomarRadicadoPendiente

Responsable de:

- ejecutar la operación de negocio;
- validar respuesta;
- actualizar Context;
- refrescar listado;
- refrescar contador;
- resolver navegación.

Debe convertirse en el caso de uso oficial de "Tomar Pendiente".

---

## Service

Responsable exclusivamente del acceso HTTP.

No contiene lógica de negocio.

No se acopla al Modal.

---

## Context

Responsable únicamente de almacenar el estado documental.

Nunca consulta backend.

---

# Integración con AppTable

Debe reutilizar completamente la infraestructura existente.

Aplicar el patrón utilizado por Gestión Correspondencia.

```text
useAppTableQueryState

↓

useDynamicUiTableQuery

↓

Adapters

↓

<AppTable />
```

No duplicar lógica existente.

El hook de tabla debe:

- cargar al abrir el modal o bajo `enabled/open`;
- evitar traer toda la lista al montar `RadicacionTabs`;
- usar `useAppTableQueryState`;
- usar `useDynamicUiTableQuery` si el endpoint responde como DynamicUiTable;
- mapear columnas con los adapters existentes de AppTable;
- mapear filas con los adapters existentes de AppTable;
- no declarar columnas manuales si backend ya entrega configuración DynamicUiTable;
- soportar paginación server;
- refrescar después de una toma exitosa.

Identificador sugerido de tabla:

```text
radicacionPendientes
```

Si backend ya define otro `tableId`, usar el real.

---

# Acción de Tabla

La tabla únicamente emitirá:

```text
actionId = "asignacion-tarea"
```

La interpretación pertenece al módulo de Radicación.

No agregar lógica de negocio a AppTable.

La acción se recibe por:

```ts
onActionTriggered(params: AppTableActionTriggered<AppTableRow>)
```

El flujo debe:

1. normalizar `actionId`;
2. aceptar solo `"asignacion-tarea"`;
3. extraer `id_estado_radicado`;
4. extraer `id_tarea_workflow`;
5. extraer `consecutivo_radicado` para trazabilidad;
6. bloquear doble click mientras la mutation está en curso;
7. ejecutar `POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar`;
8. validar `response.estadoActual === 0`;
9. actualizar `RadicacionDocumentalContext`;
10. refrescar contador/lista;
11. cerrar modal;
12. navegar a Documentos usando rutas centralizadas.

Resolver campos de fila de forma tolerante por nombres, sin usar `any`:

```text
id_estado_radicado | idEstadoRadicado | IdEstadoRadicado
id_tarea_workflow | idTareaWorkflow | IdTareaWorkflow
consecutivo_radicado | consecutivoRadicado | ConsecutivoRadicado | RADICADO
```

Si falta `id_estado_radicado`, mostrar error funcional y no llamar API.

---

# Flujo Transaccional

```text
Usuario

↓

Selecciona acción

↓

useTomarRadicadoPendiente

↓

POST tomar

↓

¿estadoActual == 0?

SI

↓

Actualizar Context

↓

Refrescar contador

↓

Refrescar listado

↓

Cerrar modal

↓

Navegar a Documentos

NO

↓

Mostrar error

↓

Mantener modal abierto
```

---

# Actualización del Contexto

El Context debe recibir exactamente la misma estructura utilizada por FE-06.

No crear un modelo paralelo.

No transformar nuevamente el contrato.

Debe reutilizar el DTO de contexto documental.

Después de `tomar` exitoso, el contexto debe recibir:

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

Este objeto debe ser compatible con el contexto restaurado por:

```text
GET /api/radicacion/pendientes/estado-activo
```

FE-05 no debe crear un contexto paralelo al usado por FE-06.

---

# Navegación Post-Asignación

Después de `tomar` exitoso, navegar a Documentos usando las rutas centralizadas o helpers definidos por TD-FE-04.

Ruta objetivo:

```text
/dashboard/radicacion/registro/{idEstadoRadicado}/documentos
```

No hardcodear rutas en el Modal.

Si las rutas definitivas aún no existen, usar el adapter/helper disponible y dejar el punto de integración tipado.

No activar `Documentos` por estado local aislado del modal.

---

# Estados UI

Representar:

- cerrado;
- abierto;
- cargando;
- vacío;
- error;
- asignando;
- asignación exitosa;
- asignación bloqueada.

Evitar dobles clics.

Evitar múltiples mutations simultáneas.

Mientras una asignación está en curso:

- deshabilitar la acción de toma o bloquear reentradas;
- conservar la fila visible;
- no cerrar el modal hasta tener respuesta.

Si backend bloquea por tarea activa existente:

- mostrar el mensaje backend;
- mantener modal abierto;
- conservar el contexto actual;
- no navegar.

Mensaje funcional de bloqueo esperado:

```text
Tarea asignada para gestión y asignación, debe terminar la tarea actual o subirla a estado pendiente para continuar con la asignación.
```

---

# Restricciones

No utilizar:

- antd/Table;
- datos mock;
- columnas hardcodeadas si backend entrega DynamicUiTable;
- lógica de negocio dentro del Modal;
- lógica de negocio dentro de AppTable;
- stores paralelos;
- rutas hardcodeadas en el Modal;
- ASMX;
- jQuery;
- variables globales;
- datos legacy hardcodeados.

---

# Principios Arquitectónicos

Aplicar:

- Single Source of Truth.
- Composition over Inheritance.
- Smart Hooks / Dumb Components.
- Command Query Separation.
- Clean Architecture.
- Open/Closed.
- Backward Compatibility.

---

# Testing

## Unitarios

Validar:

- hook de tabla;
- hook de contador;
- hook de toma;
- service;
- extracción tolerante de campos de fila.

---

## Integración

Validar:

- Modal → AppTable;
- AppTable → Hook;
- Hook → Service;
- Hook → Context;
- Context → Router;
- lazy load al abrir modal;
- bloqueo por tarea activa.

---

## Regresión

Validar:

- navegación;
- contador;
- tabla;
- build;
- lint;
- TypeScript;
- ausencia de mocks en `Modalpendiente.tsx`;
- ausencia de `antd/Table` para esta lista.

---

# Criterios de Aceptación

- Modalpendiente ya no contiene datos mock.
- AppTable reemplaza completamente la tabla local.
- Toda la lógica de asignación vive en `useTomarRadicadoPendiente`.
- La lista carga al abrir el modal o bajo `enabled/open`, no al montar `RadicacionTabs`.
- El Context se actualiza únicamente después de una respuesta exitosa.
- Documentos sólo se habilita cuando `estadoActual = 0`.
- Si ya existe una tarea activa, se muestra el mensaje backend y no se navega.
- El modal cierra y refresca estado solo después de asignación exitosa.
- No existen estados duplicados.
- No existen stores paralelos.
- No se rompe la infraestructura existente de AppTable.
- No se introduce `antd/Table` para esta lista.

---

# Entregables

1. Lista de archivos modificados.

2. Resumen técnico:

- migración a AppTable;
- flujo transaccional;
- integración con Context;
- integración con FE-06;
- integración con rutas centralizadas de TD-FE-04.

3. Resultado de pruebas.

4. Riesgos residuales.

5. Próximas fases habilitadas.

---

# Instrucción Final

Implementar el flujo transaccional de asignación de radicados pendientes utilizando la infraestructura estándar de `AppTable`, encapsulando la lógica de negocio en hooks especializados, reutilizando el `RadicacionDocumentalContext` como única fuente de verdad y garantizando que la habilitación del panel **Documentos** ocurra exclusivamente después de una confirmación exitosa del backend, sin introducir duplicidad de estado, breaking changes ni regresiones.
