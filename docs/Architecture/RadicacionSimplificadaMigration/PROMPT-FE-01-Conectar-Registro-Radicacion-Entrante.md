# PROMPT ARQUITECTONICO - Radicacion Simplificada
# Fase FE-01 - Conectar registro de radicacion entrante desde React

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Frontend senior y desarrollador React especialista en:

- React 19;
- TypeScript estricto;
- arquitectura modular frontend;
- migracion legacy WebForms/jQuery hacia React;
- integracion API transaccional;
- formularios enterprise;
- contratos DTO frontend/backend;
- state orchestration;
- manejo de errores funcionales;
- testing con Vitest y Testing Library.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Conectar el modulo React `src/modules/radicacion` con la API moderna de registro de radicacion entrante:

```txt
POST /api/radicacion/registrar-entrante
```

El objetivo funcional de esta fase es que el boton `Radicar` del formulario React pueda:

1. tomar los valores capturados en `RadicacionForm`;
2. mapearlos al contrato `RegistrarRadicacionEntranteRequestDto`;
3. invocar la API moderna;
4. recibir `ConsecutivoRadicado`, `IdRadicado` e `IdEstadoRadicado`;
5. persistir esos datos en estado frontend post-radicacion;
6. dejar disponible el resultado para fases posteriores de navegacion contextual y gestion documental.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Documento fuente de analisis:

```txt
docs/Architecture/RadicacionSimplificadaMigration/Analisis-Migracion-Legacy-RadicadorSimplificado.md
```

Modulo frontend actual:

```txt
src/modules/radicacion
```

Controllers backend revisados:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\Radicacion\
```

API principal existente:

```txt
POST /api/radicacion/registrar-entrante
```

DTO backend fuente:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\Radicacion\Tramite\RegistrarRadicacionEntranteDtos.cs
```

Decision arquitectonica confirmada:

```txt
NO se debe migrar el ASMX legacy ni recrear la transaccion de registro en frontend.
La transaccion ya vive en backend moderno mediante RegistrarRadicacionEntranteAsync.
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ESTADO ACTUAL FRONTEND
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actualmente existen:

```txt
src/modules/radicacion/pages/RadicacionRoutePage.tsx
src/modules/radicacion/pages/RadicacionPage.tsx
src/modules/radicacion/components/RadicacionForm.tsx
src/modules/radicacion/hooks/useCamposPlantilla.ts
src/modules/radicacion/hooks/useAutocompleteCamposPlantilla.ts
src/modules/radicacion/hooks/useFlujosRelacionadosTramite.ts
src/modules/radicacion/hooks/useEstructuraRelacionTipoRestriccion.ts
src/modules/radicacion/services/mapCamposPlantillaToPlantillaRadicado.ts
src/modules/radicacion/services/radicacionPayloadSerializer.ts
src/modules/radicacion/services/radicacionMetadataMapper.ts
```

Problemas actuales:

- `RadicacionRoutePage` carga plantilla.
- `RadicacionPage` recibe `plantilla`, pero no la usa.
- `RadicacionForm` vuelve a ejecutar `useCamposPlantilla`.
- El boton `Radicar` solo ejecuta `form.submit()`.
- No existe service frontend para `POST /api/radicacion/registrar-entrante`.
- No existe mapper hacia `RegistrarRadicacionEntranteRequestDto`.
- No existe estado post-radicacion con `ConsecutivoRadicado`, `IdRadicado` e `IdEstadoRadicado`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTRATO BACKEND OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Request esperado por backend:

```ts
type RegistrarRadicacionEntranteRequestDto = {
  tipoModuloRadicacion: number;
  ASUNTO: string;
  Remitente: {
    Nombre: string;
    id_Dest_Ext: number;
  };
  Destinatario: {
    Destinatario: string;
    id_Remit_Dest_Int: number;
  };
  Tipo_tramite: {
    Descripcion: string;
    tipo_doc_entrante: number;
  };
  RE_flujo_trabajo: {
    NombreFlujo: string;
    id_tipo_flujo_workflow: number;
  };
  TipoRadicado: {
    TipoRadicacion: string;
    IdTipoRadicado: number;
  };
  TipoPlantillaRadicado: {
    TipoPlantillaRadicado: string;
    IdTipoPlantillaRdicado: number;
  };
  expedienteRelacionado: {
    Expediente: string;
    idExpediente: number;
  };
  radicadoRelacionados: Array<{
    consecutivoRelacionadohijo: string;
    idregistroradicadohijo: number;
    idplantillahijo: number;
  }>;
  ANEXOS_COR: string;
  FECHALIMITERESPUESTA: string;
  numeroFolios?: number | null;
  Campos: Array<{
    IdDetallePlantillaRadicado: number;
    NombreCampo: string;
    Valor: string;
  }>;
};
```

Regla complementaria para `numeroFolios`:

- El formulario debe renderizar `Número Folios` como campo fijo en la sección `Medio de Recepción del Trámite`.
- El campo debe ser requerido, numérico, entero y con mínimo `1`.
- Si la plantilla/carga de campos trae un campo equivalente (`Numero_Folios`, `Número Folios`, `NUMERO_FOLIOS`, `NumFolios` o variantes normalizables) con `value_campo`, el formulario debe precargarlo y bloquearlo para no sobrescribir el valor leído.
- Si la plantilla trae el campo equivalente sin valor, el formulario debe permitir diligenciarlo manualmente.
- Si la plantilla no trae el campo, el formulario debe mostrarlo igualmente porque backend puede exigir `numeroFolios`.
- Cuando el campo se renderiza fijo, no debe duplicarse en `Datos Especializados`.
- El mapper debe enviar el valor en `numeroFolios` y, si backend/plantilla lo requiere en `Campos`, debe reflejarlo como campo dinámico equivalente.

Regla complementaria para `Tipo_radicado_plantilla`:

- El select visible `TipoRadicado` (`Interna`, `Externa`, `No definido` o equivalentes) alimenta el bloque principal `TipoRadicado`.
- La misma opcion seleccionada en `TipoRadicado` debe alimentar `TipoPlantillaRadicado`: `TipoPlantillaRadicado.TipoPlantillaRadicado = selected.Value` e `IdTipoPlantillaRdicado = selected.idValue`.
- No enviar `TipoPlantillaRadicado.IdTipoPlantillaRdicado` en `0`; si `selected.idValue` no es mayor que `0`, el request no debe considerarse valido.
- Si backend valida el campo dinámico `Tipo_radicado_plantilla`, el mapper debe derivarlo desde el valor seleccionado en `tipoRadicado`.
- No usar `plantilla.IdPlantillaRadicado` como fuente de `TipoPlantillaRadicado.IdTipoPlantillaRdicado` en este flujo.
- Si `/api/PlantillaRadicado/listaPlantilla` no trae `Tipo_radicado_plantilla`, el frontend puede derivarlo para cumplir el contrato de registro, sin crear un segundo control visual.

Regla complementaria para `ModuloRegistro` y Q07:

- El frontend debe mantener la llamada `POST /api/radicacion/registrar-entrante?tipoModuloRadicacion=1`.
- No enviar `ModuloRegistro` ni `moduloRegistro` por query string.
- No agregar `ModuloRegistro` al payload de `RegistrarRadicacionEntranteRequestDto`; backend resuelve el modulo internamente para este endpoint.
- Si aparece el error transaccional `RAD_TXN_Q07` con `ModuloRegistro invalido para radicacion: RADICACION SIMPLIFICADA` usando `tipoModuloRadicacion=1`, el request frontend ya esta alineado y el ajuste queda en backend.
- Backend debe normalizar aliases defensivamente antes o dentro de `RegistroLogRespuestalBuilder.Build`:

```txt
RADICACION SIMPLIFICADA => RADICACION
RADICACIÓN SIMPLIFICADA => RADICACION
RADICACION => RADICACION
```

- Solo cambiar `tipoModuloRadicacion=1` si backend confirma formalmente otro valor para este flujo.

Regla complementaria para sincronizacion de validaciones frontend/backend:

- El backend es la fuente de verdad de validaciones de campos de plantilla.
- El frontend debe construir reglas de Ant Design Form desde la metadata recibida en `GET /api/PlantillaRadicado/listaPlantilla`.
- Para cada campo, usar como minimo:
  - `obligatorio_campo` para `required`;
  - `max_leng_campo` para longitud maxima cuando el valor enviado sea texto libre;
  - `tipo_campo` o `tipo_control` para diferenciar texto, numero, fecha y correo;
  - `disable_campo` para bloqueo visual;
  - `aleas_campo` o `name_campo` para mensajes visibles.
- No aplicar `max_leng_campo` al label visible de campos `SELECCION`; esos controles envian `idValue`.
- No aplicar `max_leng_campo` como longitud textual a campos numericos; validar formato/rango numerico segun corresponda.
- En campos `AUTOCOMPLETE`, si backend retorna `idValue`, el frontend debe enviar ese `idValue` y usar `texValue` solo como texto visible.
- Centralizar estas reglas en un helper reutilizable; no duplicar reglas sueltas dentro de cada componente.
- Si backend agrega `min_leng_campo`, `regex_campo`, `mensaje_validacion` u otra metadata equivalente, extender el helper central antes de tocar los renderers.

Response esperado:

```ts
type RegistrarRadicacionEntranteResponseDto = {
  ConsecutivoRadicado: string;
  ReturnRegistraRadicacion: {
    ConsecutivoRadicado: string;
    IdRadicado: number;
    IdEstadoRadicado: number;
  };
  EstadoAsignacion: string;
  Alertas: string[];
  MetadataOperativa: Record<string, unknown>;
};
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLAS ARQUITECTONICAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

PROHIBIDO:

- copiar JavaScript legacy;
- usar jQuery;
- usar WebForms;
- consumir endpoints `.asmx`;
- reconstruir consecutivos en frontend;
- calcular SQL o reglas transaccionales en frontend;
- hardcodear IDs de plantilla, tramite, destinatario o flujo;
- leer valores desde variables globales;
- duplicar `useCamposPlantilla` en ruta y formulario;
- introducir `any` nuevo;
- llamar `clienteApi` directamente desde componentes;
- ocultar errores funcionales retornados por `AppResponses`.

OBLIGATORIO:

- usar `clienteApi` solo dentro de services;
- mantener tipos estrictos;
- crear adapter de request separado del componente;
- preservar hooks existentes cuando ya resuelven una parte del flujo;
- tratar backend como fuente de verdad para registro;
- usar `AppResponses<T>` como contrato de respuesta;
- conservar `ReturnRegistraRadicacion.IdEstadoRadicado` en estado post-radicacion;
- cubrir errores de validacion y errores backend.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## UBICACION ESPERADA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Tipos:

```txt
src/modules/radicacion/types/radicacionRegistro.types.ts
```

Servicio:

```txt
src/modules/radicacion/services/radicacionRegistro.service.ts
```

Adapter:

```txt
src/modules/radicacion/adapters/radicacionRegistroRequest.mapper.ts
```

Hook:

```txt
src/modules/radicacion/hooks/useRegistrarRadicacion.ts
```

Estado post-radicacion, si se requiere separar:

```txt
src/modules/radicacion/hooks/useRadicacionPostRegistroState.ts
```

Tests:

```txt
src/modules/radicacion/tests/radicacionRegistroRequest.mapper.test.ts
src/modules/radicacion/tests/radicacionRegistro.service.test.ts
src/modules/radicacion/tests/useRegistrarRadicacion.test.tsx
src/modules/radicacion/components/RadicacionForm.spec.test.tsx
```

Si el repositorio tiene una convencion mas especifica, respetarla sin romper la separacion `types/services/adapters/hooks`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ALCANCE FUNCIONAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

### 1. Unificar carga de plantilla

Corregir la relacion:

```txt
RadicacionRoutePage -> RadicacionPage -> RadicacionForm
```

La plantilla cargada en `RadicacionRoutePage` debe llegar al formulario.

`RadicacionForm` no debe volver a ejecutar `useCamposPlantilla` si ya recibe los campos necesarios desde la pagina.

### 2. Crear contrato frontend de registro

Crear tipos frontend equivalentes al DTO backend.

Los tipos deben permitir:

- request;
- response;
- errores;
- estado post-radicacion.

### 3. Crear mapper de formulario a request

El adapter debe convertir:

- valores de Ant Design Form;
- metadata de `CampoPlantillaDTO`;
- opciones seleccionadas;
- remitente;
- destinatario;
- tramite;
- flujo;
- tipo radicado;
- expediente;
- radicados relacionados;
- campos dinamicos;

en `RegistrarRadicacionEntranteRequestDto`.

El mapper debe ser testeable sin React.

### 4. Crear service de registro

Debe invocar:

```txt
POST /api/radicacion/registrar-entrante
```

con query opcional:

```txt
tipoModuloRadicacion=1
```

Debe retornar el wrapper `AppResponses<RegistrarRadicacionEntranteResponseDto>`.

### 5. Crear hook de registro

`useRegistrarRadicacion` debe manejar:

- estado `idle`;
- estado `submitting`;
- estado `success`;
- estado `error`;
- errores funcionales del backend;
- datos post-radicacion.

### 6. Conectar boton Radicar

El boton debe:

1. validar formulario;
2. construir request;
3. llamar hook/service;
4. mostrar error si falla;
5. guardar estado post-radicacion si exito.

### 7. Exponer resultado post-registro

Dejar disponible un estado como:

```ts
type RadicacionPostRegistroState = {
  consecutivoRadicado: string;
  idRadicado: number;
  idEstadoRadicado: number;
  estadoAsignacion: string;
  metadataOperativa: Record<string, unknown>;
  requiereGestionDocumental?: boolean;
  tieneTramiteDocumentalActivoEstado0?: boolean;
  destinoPostRegistro?: "resumen" | "documentos";
};
```

Este estado sera consumido por fases posteriores, pero esta fase no debe implementar navegacion contextual ni panel documental completo.

Regla critica:

- si la respuesta backend o `MetadataOperativa` permite determinar que al radicar se genero un tramite documental activo con estado `0`, conservar `requiereGestionDocumental = true`, `tieneTramiteDocumentalActivoEstado0 = true` y `destinoPostRegistro = "documentos"`;
- si no existe tramite documental activo en estado `0`, conservar `requiereGestionDocumental = false`, `tieneTramiteDocumentalActivoEstado0 = false` y no activar el panel de documentos;
- si no existe una senial explicita, usar `destinoPostRegistro = "resumen"` y dejar que el resolver documental de FE-03 confirme el estado;
- no inferir esta condicion desde textos de UI, variables globales legacy o nombres visuales de controles.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FUERA DE ALCANCE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

No implementar en esta fase:

- shell contextual tipo `GestionCorrespondenciaRoute`;
- ruta hija `/dashboard/radicacion/registro/:idEstadoRadicado`;
- workbench documental completo;
- carga de documentos;
- visor PDF;
- cambio de tipologia documental;
- pendientes;
- envio workflow manual;
- digitalizacion con scanner;
- integracion con `AppDigitalizador`;
- redisenio visual grande;
- nuevos endpoints backend;
- migracion de ASMX;
- reemplazo total de `RadicacionForm`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CRITERIOS DE ACEPTACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- `RadicacionRoutePage` conserva skeleton/error de carga de plantilla.
- `RadicacionPage` deja de ignorar `plantilla`.
- `RadicacionForm` puede operar con datos recibidos desde la pagina.
- Existe service frontend para `POST /api/radicacion/registrar-entrante`.
- Existe adapter testeado para construir `RegistrarRadicacionEntranteRequestDto`.
- El boton `Radicar` invoca la API con datos reales del formulario.
- Si backend responde `success=false`, la UI muestra mensaje funcional.
- Si backend responde `success=true`, se conserva:
  - `ConsecutivoRadicado`;
  - `ReturnRegistraRadicacion.IdRadicado`;
  - `ReturnRegistraRadicacion.IdEstadoRadicado`.
- Si backend informa tramite documental activo en estado `0`, se conserva una bandera tipada para que FE-02/FE-03 activen directamente `Documentos`.
- Si no existe tramite documental activo en estado `0`, el estado post-registro no debe habilitar `Documentos`.
- No se consume ningun endpoint `.asmx`.
- No se introduce jQuery.
- No se introduce `any` nuevo.
- Tests cubren:
  - mapper con datos completos;
  - mapper con campos opcionales vacios;
  - service success;
  - service error;
  - hook success/error;
  - accion del boton `Radicar`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REFERENCIA DE LEGACY
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Legacy funcional que queda reemplazado por esta fase:

```text
Web_form_radicacion_simpilificada.js
  Service_REST_registro_radicacion_simplificada(...)

WebService_radicacion_Simplificada.asmx.vb
  Service_registro_radicacion_simplificada(...)

Class_ra_radicacion_simplificada.vb
  Registro_radicacion_simplificada(...)
```

La regla de migracion es conservar la semantica de negocio, no la implementacion legacy.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## NOTAS TECNICAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

El backend moderno `RegistrarRadicacionEntranteAsync` ya:

- resuelve usuario de gestion;
- resuelve usuario radicador;
- resuelve plantilla default;
- consulta campos dinamicos;
- valida campos;
- consulta configuracion de plantilla;
- valida workflow;
- registra radicacion;
- registra tarea workflow cuando aplica;
- actualiza estado modulo radicacion;
- retorna `ReturnRegistraRadicacion`.

Por tanto, el frontend debe enfocarse en:

```txt
captura -> normalizacion -> request DTO -> invocacion -> estado post-registro
```

No debe reimplementar la orquestacion transaccional del backend.

