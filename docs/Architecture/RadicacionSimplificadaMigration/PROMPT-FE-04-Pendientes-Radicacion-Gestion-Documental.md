# PROMPT ARQUITECTONICO - Radicacion Simplificada
# Fase FE-04 - Pendientes de radicacion y toma de tramite documental

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Frontend/Backend senior especialista en:

- migracion legacy WebForms/jQuery hacia React;
- React 19 y TypeScript estricto;
- .NET API por capas Controller -> Service -> Repository;
- DapperCrudEngine y QueryOptions;
- flujos de estado transaccional;
- modulos documentales enterprise;
- workflow documental;
- contratos AppResponses;
- DynamicUiTable;
- migracion quirurgica con fidelidad funcional.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Migrar la seccion legacy que permite:

1. enviar un radicado activo de gestion documental a estado pendiente;
2. listar radicados pendientes;
3. tomar/asignar un radicado pendiente para gestion documental;
4. bajar el radicado de pendiente a activo;
5. activar el panel `Documentos` solamente cuando el radicado quede asignado para tramite documental activo.

Regla funcional central:

```txt
El panel Documentos NO se activa por existir radicado, consecutivo o gabinete.
Solo se activa cuando el usuario toma/re-radica un tramite pendiente y el sistema deja
el registro de estado en estado = 0 para gestion documental activa.
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Documentos fuente:

```txt
docs/Architecture/RadicacionSimplificadaMigration/Analisis-Migracion-Legacy-RadicadorSimplificado.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-02-Navegacion-Contextual-Post-Radicacion.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-03-Panel-Documental-Post-Radicacion.md
```

Legacy revisado:

```txt
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\RadicadorSimplificado\Web_form_radicacion_simpilificada.aspx
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\js\RadicadorSimplificado\Web_form_radicacion_simpilificada.js
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\webservice\WebService_radicacion_Simplificada.asmx.vb
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\radicador\Class_ra_rad_estados_modulo_radicacion.vb
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\ClassWorkflow.vb
```

Implementacion moderna ya existente:

```txt
GET /api/tramite/tramites/apListaRadicadosPendientes

D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\Radicacion\Tramite\TramiteController.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services\Service\Radicacion\Tramite\ListaRadicadosPendientesService.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\Radicador\Tramite\ListaRadicadosPendientesRepository.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\Radicador\PlantillaRadicado\RaRadEstadosModuloRadicacionR.cs
```

Inventario real en Controllers:

```txt
EXISTE:
GET /api/tramite/tramites/apListaRadicadosPendientes
  Controller:
  D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\Radicacion\Tramite\TramiteController.cs

NO EXISTE ACTUALMENTE:
POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente
POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar
GET  /api/radicacion/pendientes/estado-activo
GET  /api/radicacion/pendientes/contador
```

Conclusion de revision:

- el listado de pendientes ya tiene entrada publica moderna;
- las transiciones de estado `0 -> 1` y `1 -> 0` no estan expuestas como API;
- la validacion "usuario ya tiene radicado activo estado 0" no esta expuesta como API;
- la creacion/relacion de workflow cuando `id_tarea_workflow = 0` no esta expuesta como API especifica para tomar pendientes;
- por tanto FE-04 requiere backend antes de cerrar el flujo frontend.

Frontend actual relacionado:

```txt
src/modules/radicacion/components/Modalpendiente.tsx
src/modules/radicacion/components/RadicacionForm.tsx
src/modules/radicacion/hooks/RadicacionTabs.tsx
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## SEMANTICA LEGACY DE ESTADOS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Tabla principal:

```txt
ra_rad_estados_modulo_radicacion
```

Campos criticos:

```txt
id_estado_radicado
system_plantilla_radicado_id_Plantilla
id_radicado
consecutivo_radicado
fecha_registro
estado
remitente
id_usuario_radicado
id_tarea_workflow
tipo_doc_entrante_id_Tipo_Doc_Entrante
tipo_plantilla_radicado
log_error_wf_asing
```

Semantica:

```txt
estado = 0
  Radicado asignado/activo para gestion documental.
  Este es el unico estado que habilita el panel Documentos.

estado = 1
  Radicado pendiente.
  Aparece en la lista de pendientes.
  No habilita Documentos hasta que el usuario lo tome/re-radique.

estado = 2
  El comentario legacy lo menciona como "Radicado pendiente",
  pero no se evidencio uso funcional claro en el flujo principal revisado.
  No migrar como regla activa sin confirmacion adicional.
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FLUJO LEGACY QUIRURGICO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

### 1. Mostrar panel de pendientes

Legacy:

```txt
Service_solicita_opciones_plantilla_radicacion
  -> util_estado_pendiente_rad
```

JS:

```txt
if (data.d[0].util_estado_pendiente_rad == 1) {
  Panel_pendiente_radicado.style.display = "flex";
}
```

Regla:

- si la plantilla no habilita `util_estado_pendiente_rad`, no mostrar controles de pendientes;
- no hardcodear esta habilitacion en frontend;
- debe venir de API/configuracion de plantilla.

### 2. Enviar radicado activo a pendiente

Legacy JS:

```txt
envia_tarea_pendiente_radicado(id_registro_estado)
  -> Service_actualiza_estado_registro_radicado_pendiente(id_registro_estado, 1)
  -> Service_solicita_estructura_estado_radicado_radicacion_simple_vacia(0)
  -> eliminar_gestion_soporte_documental(CONST_STRU_RAD_ASIN)
  -> Service_solicita_numero_radicados_pendientes(CONS_ID_PLANTILLA_RAD)
```

Legacy VB:

```txt
Actualiza_estado_registro_modulo_radicacion(id_registro_estado, estado)
  UPDATE ra_rad_estados_modulo_radicacion
  SET estado = @estado
  WHERE id_estado_radicado = @id_registro_estado
```

Regla moderna:

- estado destino `1`;
- limpiar contexto activo de documentos;
- cerrar/inactivar `Documentos`;
- refrescar contador/lista de pendientes;
- mantener metadata minima del radicado para trazabilidad;
- no borrar documentos ni gabinete desde frontend.

### 3. Listar pendientes

Legacy:

```sql
SELECT
  id_estado_radicado,
  rre.consecutivo_radicado AS RADICADO,
  rre.remitente AS REMITENTE,
  tde.Descripcion_Doc AS TRAMITE,
  rre.fecha_registro AS FECHA,
  rre.id_tarea_workflow AS id_tarea_wf
FROM ra_rad_estados_modulo_radicacion AS rre
LEFT OUTER JOIN tipo_doc_entrante AS tde
  ON tde.id_Tipo_Doc_Entrante = rre.tipo_doc_entrante_id_Tipo_Doc_Entrante
WHERE rre.id_usuario_radicado = @idUsuarioRadicador
  AND rre.system_plantilla_radicado_id_Plantilla = @idPlantilla
  AND rre.estado = 1
ORDER BY id_estado_radicado DESC
```

Backend moderno existente:

```txt
GET /api/tramite/tramites/apListaRadicadosPendientes
```

Brecha detectada:

- el DTO moderno expone `id_estado_radicado`, `consecutivo_radicado`, `remitente`, `fecha_registro`;
- el flujo legacy necesita tambien `id_tarea_workflow` y `TRAMITE`;
- el DynamicUiTable actual define accion `asignacion-tarea`, pero la accion solo transporta `id_estado_radicado`.

Regla de migracion:

- ampliar contrato de lista para incluir `id_tarea_workflow` y descripcion de tramite;
- mantener `DynamicUiTable`;
- conservar accion por fila `asignacion-tarea`;
- no volver a construir tablas AntD con datos mock.

### 4. Tomar/re-radicar pendiente para gestion documental

Legacy JS:

```txt
asigna_tarea_pendiente_radicado(id_tarea_workflow, id_registro_estado)
  -> Service_solicita_estado_radicado_asignado_usuario_gestion_documentos(CONS_ID_PLANTILLA_RAD)
  -> si estado_asignado == YES: bloquear
  -> si id_tarea_workflow == 0: Service_registra_flujo_tarea_workflow_radicado_simple(id_registro_estado)
  -> si id_tarea_workflow != 0: Service_actualiza_estado_registro_radicado_pendiente(id_registro_estado, 0)
  -> Service_solicita_estructura_estado_radicado_radicacion_simple(...)
  -> asigna_gestion_soporte_documental(CONST_STRU_RAD_ASIN)
  -> refrescar contador
  -> cerrar modal
  -> activar panel documental
```

Legacy backend:

```txt
Solicita_estado_radicado_asignado_usuario_gestion_documentos
  SELECT id_estado_radicado
  FROM ra_rad_estados_modulo_radicacion
  WHERE estado = 0
    AND id_usuario_radicado = @idUsuarioRadicador
    AND system_plantilla_radicado_id_Plantilla = @idPlantilla
```

Regla:

- un usuario no puede tomar otro pendiente si ya tiene un radicado activo en `estado = 0`;
- la UI debe mostrar error funcional equivalente:

```txt
Tarea asignada para gestion y asignacion, debe terminar la tarea actual o subirla a estado pendiente para continuar con la asignacion.
```

### 5. Crear tarea workflow cuando falta id_tarea_workflow

Legacy:

```txt
if (id_tarea_workflow == 0) {
  Service_registra_flujo_tarea_workflow_radicado_simple(id_registro_estado)
}
```

`ClassWorkflow.Registra_flujo_tarea_workflow_radicado_simple`:

1. consulta estructura del estado radicado;
2. resuelve flujo relacionado al tipo de tramite;
3. resuelve actividad inicial del flujo;
4. resuelve tipo de modulo soporte documental;
5. calcula fecha;
6. registra flujo documento;
7. relaciona `id_tarea_workflow` con `id_estado_radicado`;
8. actualiza `estado = 0`.

Regla moderna:

- la asignacion desde pendientes debe ser una operacion de backend orquestada;
- frontend no debe decidir ni crear workflow;
- si `id_tarea_workflow = 0`, backend debe crear/relacionar workflow y luego bajar a `estado = 0`;
- si `id_tarea_workflow > 0`, backend debe bajar a `estado = 0`;
- ambos caminos deben retornar contexto post-asignacion para abrir `Documentos`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## BRECHAS EN EL REPO MODERNO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

### Backend

Existe:

```txt
GET /api/tramite/tramites/apListaRadicadosPendientes
IRaRadEstadosModuloRadicacionR.ActualizaEstadoModuloRadicacio(...)
```

Falta o debe ajustarse:

- endpoint para enviar actual activo a pendiente;
- endpoint para tomar/asignar pendiente;
- endpoint o service para validar si el usuario ya tiene activo `estado = 0`;
- contador de pendientes o reutilizacion clara del listado para badge;
- incluir `id_tarea_workflow` y tramite en el DTO de lista;
- corregir `ActualizaEstadoModuloRadicacio`, porque hoy exige `idTareaWorkflow > 0`, pero el update legacy de estado no requiere tarea cuando solo se sube a pendiente;
- orquestador backend para el caso `id_tarea_workflow = 0`.

### Frontend

Existe:

```txt
Modalpendiente.tsx
RadicacionForm.tsx -> boton "Enviar a Pendientes"
RadicacionTabs.tsx -> tabBarExtraContent con ModalPendiente
```

Problemas:

- `Modalpendiente.tsx` usa datos mock;
- no consume `GET /api/tramite/tramites/apListaRadicadosPendientes`;
- no ejecuta accion `asignacion-tarea`;
- el boton `Enviar a Pendientes` no tiene `onClick`;
- no hay service/hook de pendientes;
- no hay bloqueo de `Documentos` basado en `estado = 0`;
- no existe contexto de radicado activo tomado desde pendientes.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## API MODERNA PROPUESTA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Mantener `GET /api/tramite/tramites/apListaRadicadosPendientes` para listado.

Este endpoint ya existe y debe reutilizarse. No crear otro listado paralelo salvo que el contrato actual no pueda ampliarse sin romper consumidores.

Agregar o exponer bajo Radicacion/Tramite:

```txt
GET  /api/radicacion/pendientes/estado-activo
POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente
POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar
GET  /api/radicacion/pendientes/contador
```

Contrato sugerido:

```ts
type RadicacionPendienteEstadoActivoDto = {
  tieneActivoEstado0: boolean;
  idEstadoRadicadoActivo?: number;
  consecutivoRadicado?: string;
};

type TomarRadicadoPendienteRequestDto = {
  idEstadoRadicado: number;
  idTareaWorkflow?: number | null;
};

type TomarRadicadoPendienteResponseDto = {
  idEstadoRadicado: number;
  idRadicado?: number;
  consecutivoRadicado: string;
  idTareaWorkflow: number;
  estado: 0;
  tieneTramiteDocumentalActivoEstado0: true;
  destinoPostRegistro: "documentos";
  metadataOperativa: Record<string, unknown>;
};

type EnviarRadicadoPendienteResponseDto = {
  idEstadoRadicado: number;
  estado: 1;
  tieneTramiteDocumentalActivoEstado0: false;
  destinoPostRegistro: "resumen";
};
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLAS ARQUITECTONICAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

PROHIBIDO:

- consumir `.asmx`;
- copiar jQuery legacy;
- usar datos mock en `Modalpendiente`;
- activar `Documentos` por solo existir radicado;
- activar `Documentos` por `estado = 1`;
- permitir que un usuario tome pendiente si ya tiene activo `estado = 0`;
- crear workflow desde frontend;
- depender de variables globales tipo `CONST_ID_REGISTRO_ESTADO` o `CONST_STRU_RAD_ASIN`;
- duplicar consultas SQL ya migradas sin revisar servicios existentes;
- introducir `any` nuevo.

OBLIGATORIO:

- backend debe ser fuente de verdad para transiciones `estado 0/1`;
- frontend debe tratar pendientes como estado remoto;
- lista de pendientes debe salir de API moderna;
- accion `asignacion-tarea` debe invocar endpoint moderno de toma;
- al tomar pendiente exitosamente, navegar a `/dashboard/radicacion/registro/:idEstadoRadicado/documentos`;
- `Documentos` solo se activa con `tieneTramiteDocumentalActivoEstado0 = true`;
- al enviar a pendiente, limpiar/inactivar panel documental;
- refrescar contador/lista despues de cada mutacion;
- mostrar errores funcionales del backend.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ARQUITECTURA FRONTEND ESPERADA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Tipos:

```txt
src/modules/radicacion/types/radicacionPendientes.types.ts
```

Services:

```txt
src/modules/radicacion/services/radicacionPendientes.service.ts
```

Hooks:

```txt
src/modules/radicacion/hooks/useRadicacionPendientes.ts
src/modules/radicacion/hooks/useTomarRadicadoPendiente.ts
src/modules/radicacion/hooks/useEnviarRadicadoPendiente.ts
```

Componentes:

```txt
src/modules/radicacion/components/RadicacionPendientesModal.tsx
src/modules/radicacion/components/RadicacionPendientesBadge.tsx
```

Integracion:

```txt
RadicacionTabs
  -> RadicacionPendientesBadge / Modal

RadicacionForm
  -> boton Enviar a Pendientes

RadicacionPostRegistroPage
  -> habilita Documentos solo si estado activo confirmado
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ARQUITECTURA BACKEND ESPERADA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Controller:

```txt
DocuArchi.Api\Controllers\Radicacion\Tramite\RadicacionPendientesController.cs
```

Service:

```txt
MiApp.Services\Service\Radicacion\Tramite\RadicacionPendientesService.cs
```

Repository:

```txt
MiApp.Repository\Repositorio\Radicador\Tramite\RadicacionPendientesRepository.cs
```

Reutilizar antes de crear:

```txt
IListaRadicadosPendientesService
IListaRadicadosPendientesRepository
IRaRadEstadosModuloRadicacionR
```

El service debe orquestar:

```txt
Enviar a pendiente:
  validar idEstadoRadicado
  actualizar estado = 1
  retornar contexto inactivo documental

Tomar pendiente:
  validar idEstadoRadicado
  validar que usuario no tenga estado = 0 activo
  obtener fila pendiente
  si id_tarea_workflow = 0 crear/relacionar workflow
  actualizar estado = 0
  retornar contexto activo documental
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CRITERIOS DE ACEPTACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- `Modalpendiente.tsx` deja de usar datos mock o es reemplazado.
- La lista de pendientes consume `GET /api/tramite/tramites/apListaRadicadosPendientes`.
- La lista incluye `id_estado_radicado`, `consecutivo_radicado`, `remitente`, `tramite`, `fecha_registro`, `id_tarea_workflow`.
- El contador de pendientes se actualiza despues de tomar/enviar pendiente.
- `Enviar a Pendientes` actualiza el estado remoto a `1` y desactiva `Documentos`.
- Tomar pendiente valida que no exista otro `estado = 0` activo para el usuario.
- Si no existe `id_tarea_workflow`, backend crea/relaciona workflow antes de activar documentos.
- Tomar pendiente actualiza el estado remoto a `0`.
- Solo despues de tomar pendiente exitosamente se activa `Documentos`.
- Si no hay tramite activo `estado = 0`, `Documentos` queda inactivo.
- No se consume ningun endpoint `.asmx`.
- No se introduce jQuery.
- Tests cubren:
  - listado sin resultados;
  - listado con accion `asignacion-tarea`;
  - enviar a pendiente success/error;
  - tomar pendiente con `id_tarea_workflow > 0`;
  - tomar pendiente con `id_tarea_workflow = 0`;
  - bloqueo por usuario con activo `estado = 0`;
  - `Documentos` inactivo sin estado `0`;
  - navegacion a `Documentos` despues de toma exitosa.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## NOTA DE MIGRACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Esta fase no debe migrar carga documental real, visor, tipologia ni almacenamiento.

La responsabilidad de esta fase es controlar correctamente el ciclo:

```txt
activo estado 0 -> subir a pendiente estado 1 -> listar pendiente -> tomar pendiente -> activo estado 0 -> habilitar Documentos
```

El estado `0` no significa "pendiente"; significa que el tramite esta activo/asignado para gestion documental. Esa precision es obligatoria para no activar el panel documental en momentos incorrectos.

