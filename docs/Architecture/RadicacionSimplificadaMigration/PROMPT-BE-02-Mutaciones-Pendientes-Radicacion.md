# PROMPT ARQUITECTONICO - Backend Radicacion
# Fase BE-02 - APIs de mutacion para pendientes de radicacion

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL ESPERADO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Backend .NET senior especializado en:

- migracion legacy VB/WebForms hacia .NET API moderna;
- transacciones de negocio;
- workflow documental;
- radicacion y gestion documental;
- arquitectura Controller -> Service -> Repository;
- DapperCrudEngine y QueryOptions;
- contratos `AppResponses<T>`;
- manejo de claims;
- observabilidad y errores funcionales;
- paridad funcional legacy sin copiar implementacion insegura.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Crear las APIs modernas que reemplazan las mutaciones legacy del flujo de pendientes:

1. enviar un radicado activo de gestion documental a pendiente (`estado 0 -> 1`);
2. tomar/re-radicar un radicado pendiente para gestion documental (`estado 1 -> 0`);
3. orquestar creacion/relacion de workflow cuando el pendiente no tiene `id_tarea_workflow`;
4. retornar contexto suficiente para que frontend active o inactive `Documentos`.

Esta fase depende de:

```txt
PROMPT-BE-01-Consultas-Pendientes-Radicacion.md
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Prompt funcional origen:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-04-Pendientes-Radicacion-Gestion-Documental.md
```

Controllers modernos:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\Radicacion\Tramite\RadicacionController.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\Radicacion\Tramite\TramiteController.cs
```

Services/repositories modernos relacionados:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services\Service\Radicacion\Tramite\RegistrarRadicacionEntranteService.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\Radicador\PlantillaRadicado\RaRadEstadosModuloRadicacionR.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\Workflow\RutaTrabajo\RegistroRadicadoTareaWorkflowRepository.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services\Service\Workflow\RutaTrabajo\
```

Legacy origen:

```txt
Web_form_radicacion_simpilificada.js
  envia_tarea_pendiente_radicado(...)
  asigna_tarea_pendiente_radicado(...)

WebService_radicacion_Simplificada.asmx.vb
  Service_actualiza_estado_registro_radicado_pendiente(...)
  Service_solicita_estado_radicado_asignado_usuario_gestion_documentos(...)

WebServiceWorkflow.asmx.vb
  Service_registra_flujo_tarea_workflow_radicado_simple(...)

Class_ra_rad_estados_modulo_radicacion.vb
  Actualiza_estado_registro_modulo_radicacion(...)

ClassWorkflow.vb
  Registra_flujo_tarea_workflow_radicado_simple(...)
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## SEMANTICA DE ESTADOS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

```txt
estado = 0
  Activo/asignado para gestion documental.
  Habilita Documentos.
  Bloquea que el usuario tome otro pendiente.

estado = 1
  Pendiente.
  Se muestra en lista.
  No habilita Documentos.
```

No invertir esta semantica.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## API 1 - ENVIAR A PENDIENTE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Endpoint:

```txt
POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente
```

Uso:

Sube un radicado actualmente activo para gestion documental a lista de pendientes.

Transicion:

```txt
estado 0 -> estado 1
```

Legacy equivalente:

```txt
envia_tarea_pendiente_radicado(id_registro_estado)
Service_actualiza_estado_registro_radicado_pendiente(id_registro_estado, 1)
Actualiza_estado_registro_modulo_radicacion(id_registro_estado, 1)
```

Request:

```ts
type EnviarRadicadoPendienteRequestDto = {
  motivo?: string;
};
```

Response:

```ts
type EnviarRadicadoPendienteResponseDto = {
  idEstadoRadicado: number;
  consecutivoRadicado?: string;
  estadoAnterior: 0;
  estadoActual: 1;
  tieneTramiteDocumentalActivoEstado0: false;
  destinoPostRegistro: "resumen";
  mensaje: string;
};
```

Reglas:

- validar `idEstadoRadicado > 0`;
- validar que el registro exista;
- validar que pertenezca al usuario radicador relacionado al usuario gestionado por claims;
- si ya esta en `estado = 1`, responder exito idempotente o error funcional controlado; definir una sola politica y documentarla;
- actualizar solo `estado = 1`;
- no exigir `id_tarea_workflow`;
- no borrar documentos;
- no borrar gabinete;
- no ejecutar logica de frontend como `eliminar_gestion_soporte_documental`;
- retornar contexto para que frontend inactive `Documentos`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## API 2 - TOMAR PENDIENTE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Endpoint:

```txt
POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar
```

Uso:

Asigna al usuario actual un radicado que estaba pendiente, dejandolo activo para gestion documental.

Transicion:

```txt
estado 1 -> estado 0
```

Legacy equivalente:

```txt
asigna_tarea_pendiente_radicado(id_tarea_workflow, id_registro_estado)
Service_solicita_estado_radicado_asignado_usuario_gestion_documentos(...)
Service_registra_flujo_tarea_workflow_radicado_simple(...)
Service_actualiza_estado_registro_radicado_pendiente(id_registro_estado, 0)
```

Request:

```ts
type TomarRadicadoPendienteRequestDto = {
  idTareaWorkflow?: number | null;
};
```

Response:

```ts
type TomarRadicadoPendienteResponseDto = {
  idEstadoRadicado: number;
  idRadicado?: number;
  consecutivoRadicado: string;
  idTareaWorkflow: number;
  estadoAnterior: 1;
  estadoActual: 0;
  tieneTramiteDocumentalActivoEstado0: true;
  destinoPostRegistro: "documentos";
  metadataOperativa: {
    tramite?: string;
    remitente?: string;
    plantillaId?: number;
    workflowFueCreado: boolean;
  };
};
```

Reglas:

- validar `idEstadoRadicado > 0`;
- validar que el registro exista;
- validar que el registro este en `estado = 1`;
- validar que pertenezca al usuario radicador relacionado al usuario gestionado por claims;
- antes de tomar, consultar si el usuario ya tiene otro registro en `estado = 0`;
- si ya tiene activo, bloquear con error funcional:

```txt
Tarea asignada para gestion y asignacion, debe terminar la tarea actual o subirla a estado pendiente para continuar con la asignacion.
```

- si `id_tarea_workflow > 0`, actualizar `estado = 0`;
- si `id_tarea_workflow = 0`, crear/relacionar workflow y luego actualizar `estado = 0`;
- retornar `idTareaWorkflow` final;
- retornar `destinoPostRegistro = "documentos"`;
- no permitir activar `Documentos` si no se pudo dejar `estado = 0`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ORQUESTACION WORKFLOW CUANDO id_tarea_workflow = 0
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Legacy:

```txt
ClassWorkflow.Registra_flujo_tarea_workflow_radicado_simple
```

Secuencia legacy:

1. consulta estructura del estado radicado;
2. resuelve flujo por tipo de tramite;
3. resuelve actividad inicial;
4. resuelve tipo de modulo soporte documental;
5. calcula fecha;
6. registra flujo documento;
7. relaciona `id_tarea_workflow` en `ra_rad_estados_modulo_radicacion`;
8. actualiza `estado = 0`.

Regla moderna:

- reutilizar servicios/repositories de workflow existentes si ya cubren registro de tarea;
- no crear SQL manual si existe `RegistroRadicadoTareaWorkflowRepository`;
- si falta una pieza de workflow, crear service de orquestacion backend, no pasar la responsabilidad al frontend;
- la operacion debe ser transaccional: si falla workflow, no dejar `estado = 0`;
- registrar error controlado si no hay actividad inicial de flujo.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## UBICACION ESPERADA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Controller:

```txt
DocuArchi.Api\Controllers\Radicacion\Tramite\RadicacionPendientesController.cs
```

Service:

```txt
MiApp.Services\Service\Radicacion\Tramite\RadicacionPendientesCommandService.cs
```

Repository:

```txt
MiApp.Repository\Repositorio\Radicador\Tramite\RadicacionPendientesCommandRepository.cs
```

DTOs:

```txt
MiApp.DTOs\DTOs\Radicacion\Tramite\RadicacionPendientesCommandDtos.cs
```

Reutilizar antes de crear:

```txt
IRaRadEstadosModuloRadicacionR
IListaRadicadosPendientesRepository
RegistroRadicadoTareaWorkflowRepository
RegistrarRadicacionEntranteService.ActualizaEstadoModuloRadicacionAsync como referencia de validaciones
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## AJUSTE NECESARIO EN REPOSITORY EXISTENTE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Existe:

```txt
IRaRadEstadosModuloRadicacionR.ActualizaEstadoModuloRadicacio(
  string defaultDbAlias,
  long idRegistroEstado,
  int estado,
  long idTareaWorkflow)
```

Problema:

```txt
El metodo moderno exige idTareaWorkflow > 0.
El legacy permite actualizar estado sin id_tarea_workflow cuando se envia a pendiente.
```

Decision requerida:

- crear metodo nuevo para actualizar solo `estado`;
- o permitir `idTareaWorkflow` opcional sin romper consumidores actuales;
- no relajar validacion del metodo existente si eso rompe el flujo de registro entrante.

Opcion recomendada:

```txt
ActualizaEstadoModuloRadicacionEstadoAsync(defaultDbAlias, idRegistroEstado, estado)
```

Y otro metodo explicito cuando se deba relacionar tarea:

```txt
ActualizaEstadoModuloRadicacionConTareaAsync(defaultDbAlias, idRegistroEstado, estado, idTareaWorkflow)
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLAS TECNICAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

PROHIBIDO:

- consumir ASMX;
- usar Session legacy;
- concatenar SQL;
- aceptar `usuarioid` desde body;
- permitir que frontend cree workflow;
- activar documentos con `estado = 1`;
- cambiar estado sin verificar pertenencia del usuario;
- dejar cambios parciales si falla workflow;
- ocultar errores funcionales del backend.

OBLIGATORIO:

- leer `defaulalias` y `usuarioid` desde claims;
- resolver usuario radicador relacionado;
- validar plantilla default;
- validar registro por `id_estado_radicado`;
- usar `AppResponses<T>`;
- usar `QueryOptions` o repositorios ya existentes;
- mantener transaccion cuando haya workflow + update de estado;
- devolver errores funcionales claros;
- loguear requestId/alias/idEstadoRadicado;
- agregar XML comments;
- cubrir unit tests.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CRITERIOS DE ACEPTACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- Existe `POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente`.
- Existe `POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar`.
- Enviar a pendiente cambia `estado 0 -> 1`.
- Enviar a pendiente no exige `id_tarea_workflow`.
- Tomar pendiente bloquea si el usuario ya tiene activo `estado = 0`.
- Tomar pendiente con `id_tarea_workflow > 0` cambia `estado 1 -> 0`.
- Tomar pendiente con `id_tarea_workflow = 0` crea/relaciona workflow y cambia `estado 1 -> 0`.
- Si falla workflow, no queda activo `estado = 0`.
- Response de tomar retorna `destinoPostRegistro = "documentos"`.
- Response de enviar retorna `destinoPostRegistro = "resumen"`.
- No se consume legacy.
- No se concatena SQL.
- Tests cubren:
  - enviar pendiente success;
  - enviar pendiente id inexistente;
  - enviar pendiente sin `id_tarea_workflow`;
  - tomar pendiente success con tarea existente;
  - tomar pendiente success creando workflow;
  - tomar pendiente bloqueado por activo estado 0;
  - tomar pendiente falla workflow sin cambio parcial;
  - claim invalido;
  - alias faltante.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FUERA DE ALCANCE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

No implementar en esta fase:

- frontend React;
- tabla de pendientes;
- upload documental;
- visor PDF;
- cambio de tipologia;
- almacenamiento documental;
- endpoints de listado ya cubiertos por BE-01 salvo ajustes necesarios para pruebas de integracion.

