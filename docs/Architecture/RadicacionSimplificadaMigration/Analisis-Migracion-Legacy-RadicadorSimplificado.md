# Analisis de migracion legacy - RadicadorSimplificado

Fecha: 2026-07-02

## Alcance

Este documento captura la exploracion arquitectonica del legacy ubicado en:

```text
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\RadicadorSimplificado\
```

Y lo compara contra la implementacion actual en:

```text
src/modules/radicacion
```

El objetivo es preservar el analisis tecnico para orientar una migracion progresiva, sin copiar el modelo WebForms/jQuery al frontend React.

## Archivos legacy revisados

```text
RadicadorSimplificado/
  Class_ra_radicacion_simplificada.vb
  Web_form_radicacion_simpilificada.aspx
  Web_form_radicacion_simpilificada.aspx.designer.vb
  Web_form_radicacion_simpilificada.aspx.vb

js/RadicadorSimplificado/
  Web_form_radicacion_simpilificada.js

webservice/
  WebService_radicacion_Simplificada.asmx.vb
```

## Diagnostico arquitectonico

El modulo legacy no es solo una pantalla de captura. Es un flujo transaccional completo compuesto por:

```text
ASPX markup
  -> contenedores, toolbars, modales, tablas bootstrap, panel documental

Code-behind
  -> inicializacion minima de scripts

JavaScript procedural
  -> orquestacion de eventos, llamadas ASMX, estado global, manipulacion DOM

Servicios ASMX
  -> fachada HTTP sobre clases VB de dominio

Class_ra_radicacion_simplificada
  -> regla transaccional critica de radicacion
```

El code-behind `Web_form_radicacion_simpilificada.aspx.vb` casi no contiene negocio. Su responsabilidad principal es registrar el script inicial:

```vb
$(document).ready(function () {$().inicio();});
```

La conducta funcional vive principalmente en:

```text
Web_form_radicacion_simpilificada.js
Class_ra_radicacion_simplificada.vb
WebService_radicacion_Simplificada.asmx.vb
```

## Flujo legacy de inicio

El JavaScript ejecuta en `window.load`:

```text
load_page_radicacion_simplificada()
```

Ese flujo hace, en orden general:

1. Inicializa eventos de botones y resize.
2. Inicializa cliente workflow.
3. Carga interfaz dinamica de radicacion simplificada.
4. Solicita nombre y metadata de plantilla default.
5. Consulta numero de radicados pendientes.
6. Valida si el usuario tiene radicado asignado.
7. Si hay radicado asignado, carga estructura de estado y soporte documental.
8. Consulta opciones de plantilla.

Equivalentes legacy importantes:

```text
Service_Inicializa_cliente_workflow_radicacion_simple
Service_solicita_estructura_radicacion_simplificada
Service_solicita_nombre_plantilla_radicacion_simplificada
Service_solicita_numero_radicados_pendientes
Service_Solicita_radicado_existencia_radicado_asignado
Service_solicita_estructura_estado_radicado_radicacion_simple
Service_solicita_opciones_plantilla_radicacion
```

## Estado global legacy

El JS conserva estado de modulo en variables globales:

```js
CONS_ID_SCRIPT_SOLICITANTE
CONS_ID_PLANTILLA_RAD
CONS_ID_TIPO_PLANTILLA_RAD
CONS_UTIL_ESTADO_PENDIENTE
CONS_TIPO_PLANTILLA_RAD
CONST_NOMBRE_PLANTILLA_RAD
CONST_ID_REGISTRO_ESTADO
CONST_STRU_RAD_ASIN
CONST_ID_IMAGEN_RAD
CONST_ESTADO_ENVIO_SEND_MAIL
GroupManagerTomSlect
_CDeRelacionEstadoRetriccion
```

En React, estas variables no deben migrarse como globals. Deben convertirse en estado tipado, idealmente separado en:

```ts
RadicacionBootstrapState
RadicacionFormState
RadicacionRegistroState
RadicacionDocumentosState
RadicacionWorkflowState
RadicacionPendientesState
```

## Metodo critico legacy

El metodo mas importante es:

```vb
Registro_radicacion_simplificada(...)
```

Responsabilidades detectadas:

1. Resuelve la plantilla por nombre.
2. Determina tipo de plantilla.
3. Valida autorizacion por dias/hora de radicacion.
4. Lee `RE_flujo_trabajo` desde los campos enviados.
5. Valida existencia de actividad inicial workflow.
6. Carga opciones de plantilla.
7. Carga campos adicionales de plantilla.
8. Carga scripts de validacion asociados a campos.
9. Copia valores de formulario a campos de plantilla.
10. Calcula fecha limite de respuesta.
11. Extrae remitente externo.
12. Extrae destinatario interno y cargo desde texto con formato `nombre <cargo>`.
13. Aplica validaciones por script.
14. Asigna defaults para campos obligatorios especiales.
15. Valida obligatorios.
16. Valida formato de fechas.
17. Resuelve area del destinatario.
18. Determina si el tramite genera modulo de respuesta.
19. Resuelve sede del usuario radicador.
20. Calcula prefijo de consecutivo.
21. Determina tipo de envio documental.
22. Determina si sube radicado a workflow.
23. Bloquea consecutivo con `FOR UPDATE`.
24. Inicializa consecutivo anual si aplica.
25. Valida si el usuario ya tiene radicado asignado.
26. Genera consecutivo de radicado y codigo de barras.
27. Inserta en tabla dinamica de plantilla.
28. Actualiza consecutivo en sistema.
29. Inserta `ra_registro_general_radicacion`.
30. Inserta `ra_respuesta_radicado` si aplica.
31. Inserta log de respuesta si aplica.
32. Inserta `ra_rad_estados_modulo_radicacion` si aplica.
33. Registra flujo documental si aplica.
34. Actualiza relacion de tarea workflow.
35. Actualiza estado flow del radicado.
36. Retorna `codigo_radicado`, `asignar_radicado`, `id_registro_estado` y `error_gestion`.

Conclusion: este metodo debe permanecer como operacion backend atomica. React no debe replicar consecutivos, SQL dinamico, transacciones ni workflow.

## Estado actual en React

El modulo actual ya tiene:

```text
src/modules/radicacion/
  pages/RadicacionRoutePage.tsx
  pages/RadicacionPage.tsx
  hooks/useCamposPlantilla.ts
  hooks/useAutocompleteCamposPlantilla.ts
  hooks/useFlujosRelacionadosTramite.ts
  hooks/useEstructuraRelacionTipoRestriccion.ts
  components/RadicacionForm.tsx
  services/mapCamposPlantillaToPlantillaRadicado.ts
  services/radicacionMetadataMapper.ts
  services/radicacionPayloadSerializer.ts
```

Capacidades ya presentes:

- Ruta `/dashboard/radicacion`.
- Carga de campos de plantilla desde `/api/PlantillaRadicado/listaPlantilla`.
- Skeleton y error state inicial.
- Formulario visual de radicacion.
- Campos especializados por nombre.
- Autocomplete generico, remitente y destinatario.
- Restriccion de destinatario por tramite.
- Consulta de flujos relacionados por tramite.
- Motor dinamico preparado mediante `useRadicacionDynamicForm`.

## Problema de integracion actual

`RadicacionRoutePage` carga y transforma la plantilla:

```ts
mapCamposPlantillaToPlantillaRadicado(data)
```

Pero `RadicacionPage` no la usa:

```ts
void plantilla;
void onSubmit;
```

Ademas, `RadicacionForm` vuelve a ejecutar `useCamposPlantilla()`. Esto genera doble consumidor de la misma fuente y deja desconectado el motor dinamico.

## Brechas principales

| Area | React actual | Legacy | Accion recomendada |
| --- | --- | --- | --- |
| Bootstrap del modulo | Parcial | Completo | Crear `useRadicacionBootstrap` |
| Plantilla default | Parcial | Completa | Unificar contrato |
| Formulario | Visual especifico | Dinamico WebForms/JS | Mantener shell y conectar metadata |
| Autocomplete | Parcial | Completo | Validar payloads por campo |
| Restricciones | Parcial | Completo | Consolidar DTO |
| Fecha limite respuesta | No completa | Backend | Backend fuente de verdad |
| Registro radicado | No migrado | Critico | Crear servicio/hook de registro |
| Consecutivo | No migrado | Critico | Solo backend atomico |
| Respuesta radicado | No migrado | Critico si tramite aplica | Backend |
| Workflow | Parcial/no conectado | Completo | Migrar despues del registro |
| Pendientes | Modal parcial | Completo | Crear `useRadicacionPendientes` |
| Documentos | CapDocument/base | Completo | Reusar AppDigitalizador/AppUpload/AppVisor |
| Tipologia documental | No completa | Completa | Servicio + panel documental |

## Estrategia de migracion

La estrategia recomendada es estrangulamiento funcional, no transliteracion del JS legacy.

### Fase 1 - Bootstrap moderno

Crear una capa de inicio que reemplace `load_page_radicacion_simplificada()`.

Responsabilidades:

- Cargar plantilla.
- Resolver metadata default.
- Consultar opciones de plantilla.
- Consultar numero de pendientes.
- Consultar radicado asignado al usuario.
- Exponer estado tipado para la pagina.

Propuesta:

```text
hooks/useRadicacionBootstrap.ts
services/radicacionPlantilla.service.ts
types/radicacionBootstrap.types.ts
```

### Fase 2 - Contrato real de registro

El DTO actual `RadicacionPayloadDTO` es insuficiente para el backend legacy. El backend espera algo equivalente a `Class_config_general_service`.

Debe crearse un mapper:

```text
adapters/radicacionLegacyPayload.mapper.ts
```

Debe producir campos con informacion como:

```ts
name_campo
texto_campo
value_campo
tipo_control
serviceName
TomPParameterTomSelelect
```

### Fase 3 - Registro desde React

Crear:

```text
services/radicacionRegistro.service.ts
hooks/useRegistrarRadicacion.ts
types/radicacionRegistro.types.ts
```

El hook debe:

1. Validar formulario.
2. Serializar valores al contrato backend real.
3. Llamar registro.
4. Recibir `codigo_radicado`, `asignar_radicado`, `id_registro_estado`.
5. Actualizar estado post-radicacion.
6. Activar panel documental.

Este es el primer corte funcional de valor.

### Fase 4 - Estado post-radicacion

Reemplazar globals legacy por un estado explicito:

```ts
type RadicacionSessionState =
  | "idle"
  | "loading-template"
  | "editing"
  | "submitting"
  | "registered"
  | "documenting"
  | "pending"
  | "workflow-send"
  | "error";
```

Contexto sugerido:

```ts
type RadicacionSessionContext = {
  idPlantilla: number;
  nombrePlantilla: string;
  tipoPlantilla: string;
  codigoRadicado?: string;
  idRegistroEstado?: string | number;
  idTareaWorkflow?: string | number;
  nombreGabinete?: string;
  radicado?: string;
};
```

### Fase 5 - Documentos

No migrar Bootstrap Table ni manipulacion DOM.

Reusar infraestructura existente:

- `AppDigitalizador`
- `AppUpload`
- `AppVisorEmbedPdf`
- patrones de `gestionCorrespondencia/components/documentosWorkbench`

Servicios legacy a cubrir:

```text
Service_almacenamiento_documentos_digitalizados_rad_simplificada
Service_actualiza_tipologia_rad_simplificada
Service_solicita_url_documento_soporte_documental_rad_simple
Service_elimina_documento_enlace_radicado_workflow
service_source_list_item_control_general_documento_radicado
```

### Fase 6 - Pendientes

Crear:

```text
hooks/useRadicacionPendientes.ts
services/radicacionPendientes.service.ts
components/RadicacionPendientesModal.tsx
```

Servicios legacy equivalentes:

```text
Service_Solicita_radicados_pendientes_radicacion
Service_solicita_numero_radicados_pendientes
Service_actualiza_estado_registro_radicado_pendiente
Service_solicita_estado_radicado_asignado_usuario_gestion_documentos
```

### Fase 7 - Workflow

Crear:

```text
hooks/useRadicacionWorkflow.ts
services/radicacionWorkflow.service.ts
components/RadicacionWorkflowSendModal.tsx
```

Servicios legacy equivalentes:

```text
Service_Inicializa_cliente_workflow_radicacion_simple
Service_solicita_listado_actividades_para_envio_tarea_a_flujo
Service_enviar_tarea_flujo_trabajo_radicacion_simple
Service_registra_flujo_tarea_workflow_radicado_simple
```

## Arquitectura objetivo

```text
src/modules/radicacion/
  pages/
    RadicacionRoutePage.tsx
    RadicacionPage.tsx

  components/
    RadicacionForm.tsx
    RadicacionWorkbench.tsx
    RadicacionPendientesModal.tsx
    RadicacionDocumentosPanel.tsx

  hooks/
    useRadicacionBootstrap.ts
    useRadicacionFormState.ts
    useRegistrarRadicacion.ts
    useRadicacionPendientes.ts
    useRadicacionDocumentos.ts
    useRadicacionWorkflow.ts

  services/
    radicacionPlantilla.service.ts
    radicacionRegistro.service.ts
    radicacionPendientes.service.ts
    radicacionDocumentos.service.ts
    radicacionWorkflow.service.ts

  adapters/
    radicacionLegacyPayload.mapper.ts
    radicacionEstado.mapper.ts
    radicacionDocumento.mapper.ts

  types/
    radicacionBootstrap.types.ts
    radicacionRegistro.types.ts
    radicacionPendientes.types.ts
    radicacionDocumentos.types.ts
    radicacionWorkflow.types.ts
```

## Secuencia recomendada

```text
Usuario
  -> /dashboard/radicacion
  -> useRadicacionBootstrap
  -> RadicacionForm
  -> useRegistrarRadicacion
  -> backend registra transaccion atomica
  -> React recibe codigo/id estado
  -> RadicacionDocumentosPanel
  -> carga/visor/tipologia/documentos
  -> pendientes o workflow segun estado
```

## Reglas de arquitectura

1. React no debe calcular consecutivos.
2. React no debe construir SQL dinamico.
3. React no debe decidir atomicidad de registro.
4. Backend debe ser fuente de verdad para fecha limite, workflow y respuesta.
5. Frontend debe modelar estados y efectos, no replicar el procedural jQuery.
6. Cada endpoint legacy debe entrar como service + type + adapter.
7. Las variables globales legacy deben convertirse en estado local/contextual tipado.
8. El primer corte funcional debe ser registrar y obtener `codigo_radicado` + `id_registro_estado`.

## Riesgos

- Dependencia fuerte de sesion ASP.NET en legacy.
- Contratos ASMX devuelven estructuras no normalizadas.
- Inconsistencia de nombres: `Destinatario_Cor` vs `DESTINATARIO_COR`.
- El formulario React actual es visualmente util, pero no representa todo el contrato legacy.
- Duplicacion actual de `useCamposPlantilla`.
- Fecha limite y validaciones por script estan incompletas en frontend.
- Pendientes, documentos y workflow tienen acoplamiento historico alto.

## Proximo corte sugerido

Crear una especificacion/cambio para:

```text
Migrar registro funcional de radicacion simplificada desde React
```

Alcance minimo:

1. Unificar carga de plantilla.
2. Conectar `RadicacionPage` con `plantilla`.
3. Crear mapper a contrato backend real.
4. Crear service de registro.
5. Implementar `useRegistrarRadicacion`.
6. Actualizar boton `Radicar`.
7. Persistir en estado React `codigo_radicado` e `id_registro_estado`.
8. Dejar preparado el panel documental post-registro.

## Inventario de APIs modernas revisadas

Ruta backend revisada:

```text
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\Radicacion\
```

Controllers encontrados:

```text
Configuracion/
  ConfiguracionPlantillaController.cs

PlantillaRadicado/
  PlantillaRadicacionController.cs

Tramite/
  RadicacionController.cs
  TramiteController.cs
  SolicitaGabineteRadicadoWorkflowController.cs
  SolicitaExistenciaRadicadoRutaWorkflowController.cs
  SolicitaEstructuraTipoDocEntranteController.cs
  SolicitaEstructuraRutaWorkflowController.cs
  RelacionCamposRutaWorklflowController.cs
```

### RadicacionController

Base route:

```text
api/radicacion
```

Endpoints:

```text
POST api/radicacion/registrar-entrante
POST api/radicacion/validar-entrante
GET  api/radicacion/flujo-inicial?idTipoTramite={id}
```

Responsabilidad:

- Es el controller principal para registrar radicacion entrante.
- Reemplaza el nucleo de `Service_registro_radicacion_simplificada`.
- Usa claims `defaulalias` y `usuarioid`.
- Obtiene IP por `IIpHelper`.
- Delega en `IRegistrarRadicacionEntranteService`.

DTO principal:

```ts
RegistrarRadicacionEntranteRequestDto {
  tipoModuloRadicacion
  ASUNTO
  Remitente
  Destinatario
  Tipo_tramite
  RE_flujo_trabajo
  TipoRadicado
  TipoPlantillaRadicado
  expedienteRelacionado
  radicadoRelacionados
  ANEXOS_COR
  FECHALIMITERESPUESTA
  numeroFolios
  Campos
}
```

Respuesta principal:

```ts
RegistrarRadicacionEntranteResponseDto {
  ConsecutivoRadicado
  ReturnRegistraRadicacion {
    ConsecutivoRadicado
    IdRadicado
    IdEstadoRadicado
  }
  EstadoAsignacion
  Alertas
  MetadataOperativa
}
```

Impacto en la estrategia:

- El primer corte frontend ya no debe apuntar a replicar legacy ASMX.
- Debe construir un adapter desde `RadicacionForm` hacia `RegistrarRadicacionEntranteRequestDto`.
- El estado post-radicacion debe tomar `IdEstadoRadicado` desde `ReturnRegistraRadicacion`.

### TramiteController

Base route:

```text
api/tramite
```

Endpoints:

```text
GET api/tramite/tramites/empsolicitaListaflujosRelacionadosTramite?idTipoDocEntrante={id}
GET api/tramite/tramites/solicitaEstructuraRelacionTipoRestriccion?idTipoTramite={id}
GET api/tramite/tramites/solicitaTotalDiasVencimientoTramite?idPlantilla={id}&idTipoTramite={id}
GET api/tramite/tramites/solicitaListaDiasFeriados
GET api/tramite/tramites/solicitaFechaLimiteRespuesta?idTipoTramite={id}
GET api/tramite/tramites/apListaRadicadosPendientes
```

Responsabilidad:

- Agrupa consultas auxiliares del tramite.
- Ya alimenta hooks React existentes:
  - `useFlujosRelacionadosTramite`
  - `useEstructuraRelacionTipoRestriccion`
- Cubre parte faltante del legacy:
  - fecha limite de respuesta
  - dias feriados
  - radicados pendientes

Impacto:

- `FECHALIMITERESPUESTA` no debe calcularse en frontend.
- Debe conectarse `solicitaFechaLimiteRespuesta` cuando cambie `Tipo_tramite`.
- `apListaRadicadosPendientes` puede reemplazar el listado legacy de pendientes con `AppTable`/`DynamicUiTableDto`.

### PlantillaRadicacionController

Base route:

```text
api/PlantillaRadicado
```

Endpoints:

```text
GET  api/PlantillaRadicado/listaPlantilla
POST api/PlantillaRadicado/autoCompleteTercero
POST api/PlantillaRadicado/caracterizacionDestinatario
POST api/PlantillaRadicado/solicitaAutoCompleteDestinatarioRestriccion
POST api/PlantillaRadicado/solicitaAutoCompleteCampos
POST api/PlantillaRadicado/solicitaAutoCompleteTokenRadicado
POST api/PlantillaRadicado/solicitaAutoCompleteTokenExpedienteRadicado
```

Responsabilidad:

- Construye la estructura dinamica de campos de radicacion.
- Resuelve autocompletes de remitente, destinatario, campos dinamicos, radicado y expediente.
- Expone caracterizacion del destinatario.

Impacto:

- Ya cubre la mayor parte de `Service_solicita_estructura_radicacion_simplificada` y autocompletes legacy.
- El frontend debe dejar de duplicar carga de plantilla entre `RadicacionRoutePage` y `RadicacionForm`.
- Los campos `buscarRadicado` y `expedienteRelacionado` pueden conectarse a los autocompletes token existentes.

### ConfiguracionPlantillaController

Base route:

```text
api/configuracionPlantilla
```

Endpoint:

```text
GET api/configuracionPlantilla/solicitaConfiguracionPlantilla?idPlantilla={id}&tipoRadicacionPlantilla={id}
```

Responsabilidad:

- Retorna configuracion de plantilla por plantilla y tipo de radicacion.
- En el servicio de registro se usa para resolver `Descripcion_tipo_radicacion` y modulo de registro.

Impacto:

- Debe formar parte del bootstrap o prevalidacion si la UI necesita conocer opciones antes de registrar.
- Para registro final, backend ya lo consulta internamente.

### SolicitaEstructuraTipoDocEntranteController

Base route:

```text
api/radicacion/tramite
```

Endpoint:

```text
GET api/radicacion/tramite/tipo-doc-entrante/{idTipoDocEntrante}
```

Responsabilidad:

- Devuelve estructura del tipo de documento/tramite entrante.
- En registro se usa para decidir comportamiento del flujo documental/workflow.

Impacto:

- Puede alimentar UI avanzada, pero no es obligatorio para el primer corte si `registrar-entrante` ya valida internamente.

### RelacionCamposRutaWorklflowController

Base route:

```text
api/radicacion
```

Endpoint:

```text
GET api/radicacion/tramite/solicita-campos-relacion-ruta-plantilla?idPlantillaRadicado={id}&idRuta={id}
```

Responsabilidad:

- Consulta relacion entre campos de plantilla de radicacion y ruta workflow.

Impacto:

- Pertenece a fase workflow/post-radicacion.
- No bloquea el primer corte de registro si el backend registra workflow internamente.

### SolicitaEstructuraRutaWorkflowController

Base route:

```text
api/workflow/ruta-trabajo
```

Endpoint:

```text
GET api/workflow/ruta-trabajo/solicita-estructura-ruta
```

Responsabilidad:

- Devuelve rutas workflow activas.

Impacto:

- Reemplaza parte de inicializacion legacy de ruta.
- Util para selector o validacion UI de rutas workflow.

### SolicitaExistenciaRadicadoRutaWorkflowController

Base route:

```text
api/workflow/ruta-trabajo
```

Endpoint:

```text
GET api/workflow/ruta-trabajo/solicita-existencia-radicado?consecutivoRadicado={radicado}&nombreRuta={ruta}
```

Responsabilidad:

- Verifica si un radicado existe en una ruta workflow.
- El servicio `RegistrarRadicacionEntranteAsync` ya lo usa como parte del flujo post-registro.

Impacto:

- No debe ser llamado manualmente por React en el primer corte.
- Sirve para pantallas de consulta/diagnostico o workflow avanzado.

### SolicitaGabineteRadicadoWorkflowController

Base route:

```text
api/workflow/ruta-trabajo
```

Endpoints:

```text
GET api/workflow/ruta-trabajo/radicados/{consecutivoRadicado}/gabinete
GET api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete
```

Responsabilidad:

- Resuelve metadata documental/gabinete por radicado o por tarea workflow.
- Ya existe consumo equivalente en `gestionCorrespondencia` para documentos de respuesta.

Impacto:

- Es clave para fase documental de radicacion.
- Despues de `registrar-entrante`, React deberia usar `ConsecutivoRadicado` o `IdTareaWorkflow` si esta en metadata para abrir panel documental.

## Cambio de estrategia por APIs existentes

Antes del inventario, la hipotesis era crear un servicio de registro nuevo o adaptar el legacy ASMX. Despues de revisar controllers modernos:

```text
La funcion de registro ya existe:
POST api/radicacion/registrar-entrante
```

Por tanto, la estrategia se ajusta:

1. No migrar `Service_registro_radicacion_simplificada` directamente.
2. No crear endpoint nuevo de registro salvo brecha probada.
3. Crear en React:
   - `radicacionRegistro.service.ts`
   - `useRegistrarRadicacion.ts`
   - `radicacionRegistroRequest.mapper.ts`
4. Conectar `RadicacionForm` al DTO moderno.
5. Usar `ReturnRegistraRadicacion.IdEstadoRadicado` para estado post-radicacion.
6. Usar APIs auxiliares ya existentes para fecha limite, pendientes, gabinete y workflow.

## APIs prioritarias para conectar desde React

Orden recomendado:

1. `GET api/PlantillaRadicado/listaPlantilla`
   - Ya conectado.
   - Debe unificarse para evitar doble carga.

2. `GET api/tramite/tramites/empsolicitaListaflujosRelacionadosTramite`
   - Ya conectado.

3. `GET api/tramite/tramites/solicitaEstructuraRelacionTipoRestriccion`
   - Ya conectado.

4. `GET api/tramite/tramites/solicitaFechaLimiteRespuesta`
   - Falta conectar al cambio de tramite.

5. `POST api/radicacion/validar-entrante`
   - Prevalidacion ligera opcional.

6. `POST api/radicacion/registrar-entrante`
   - Corte funcional principal.

7. `GET api/workflow/ruta-trabajo/radicados/{consecutivoRadicado}/gabinete`
   - Inicio del panel documental post-registro.

8. `GET api/tramite/tramites/apListaRadicadosPendientes`
   - Fase pendientes.

