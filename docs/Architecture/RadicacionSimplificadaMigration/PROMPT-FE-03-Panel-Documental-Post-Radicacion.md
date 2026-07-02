# PROMPT ARQUITECTONICO - Radicacion Simplificada
# Fase FE-03 - Panel documental post-radicacion

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Frontend senior especialista en:

- React 19;
- TypeScript estricto;
- modulos documentales enterprise;
- integracion con APIs de gabinete/radicado;
- composicion de tabs;
- reutilizacion de componentes existentes;
- migracion legacy documental sin copiar UI WebForms;
- manejo de estados async;
- accesibilidad y UX de trabajo repetitivo.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar el primer panel documental post-radicacion dentro del shell contextual de `radicacion`.

Esta fase parte de:

```txt
FE-01: registro exitoso y estado post-registro
FE-02: ruta/panel contextual /dashboard/radicacion/registro/:idEstadoRadicado
```

El objetivo es que el panel contextual pueda resolver gabinete/documentos usando el `ConsecutivoRadicado` retornado por registro y preparar una experiencia documental moderna.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Documentos fuente:

```txt
docs/Architecture/RadicacionSimplificadaMigration/Analisis-Migracion-Legacy-RadicadorSimplificado.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-01-Conectar-Registro-Radicacion-Entrante.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-02-Navegacion-Contextual-Post-Radicacion.md
```

Patrones reutilizables:

```txt
src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx
src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx
src/modules/gestionCorrespondencia/components/documentosWorkbench
src/modules/gestionCorrespondencia/services/solicitaGabineteRadicadoWorkflow.service.ts
```

Componentes compartidos disponibles:

```txt
src/app/Components/UI/AppTabs
src/app/Components/UI/AppUpload
src/app/Components/UI/AppDigitalizador
src/app/Components/UI/AppVisorEmbedPdf
```

API de gabinete disponible:

```txt
GET /api/workflow/ruta-trabajo/radicados/{consecutivoRadicado}/gabinete
GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## DECISION ARQUITECTONICA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Para radicacion, el panel documental debe iniciar por `ConsecutivoRadicado`, no por `idTareaWf`.

Motivo:

```txt
No todo radicado necesariamente genera workflow.
El registro siempre debe retornar ConsecutivoRadicado e IdEstadoRadicado.
```

Si posteriormente se obtiene `idTareaWorkflow`, puede enriquecer el contexto, pero no debe bloquear el panel documental inicial.

Decision adicional:

```txt
Si el sistema determina que al radicar existe un tramite documental activo en estado 0,
el panel post-radicacion debe activar y abrir directamente la pestana Documentos.
Si no existe tramite documental activo en estado 0, Documentos debe quedar inactivo.
```

Esta regla aplica despues del registro y tambien cuando el usuario reingresa al modulo despues de haber salido sin completar la gestion documental.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ALCANCE FUNCIONAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

### 1. Crear contexto documental de radicacion

Crear:

```txt
src/modules/radicacion/context/RadicacionDocumentosContext.tsx
```

Contrato sugerido:

```ts
type RadicacionDocumentosContextState = {
  idEstadoRadicado?: number;
  idRadicado?: number;
  consecutivoRadicado?: string;
  idTareaWorkflow?: number;
  requiereGestionDocumental?: boolean;
  tieneTramiteDocumentalActivoEstado0?: boolean;
  gestionDocumentalPendiente?: boolean;
  nombreGabinete?: string;
  gabineteLoading: boolean;
  gabineteError?: string;
  reloadGabinete: () => Promise<void>;
};
```

Debe inspirarse en `GestionRespuestaDocumentosContext`, pero resolver por radicado cuando sea posible.

### 2. Crear servicio de gabinete para radicacion

Crear o reutilizar servicio:

```txt
src/modules/radicacion/services/radicacionGabinete.service.ts
```

Debe soportar:

```ts
getRadicacionGabinetePorRadicado(consecutivoRadicado)
getRadicacionGabinetePorTareaWorkflow(idTareaWorkflow)
```

No llamar `clienteApi` desde componentes.

### 3. Crear pagina panel post-registro

Crear:

```txt
src/modules/radicacion/pages/RadicacionPostRegistroPage.tsx
```

Debe renderizar una estructura por tabs:

```txt
Resumen
Documentos
```

La pestana inicial debe resolverse asi:

```ts
type RadicacionPostRegistroInitialTab = "resumen" | "documentos";
```

Reglas:

- si la ruta termina en `/documentos` y existe `tieneTramiteDocumentalActivoEstado0 = true`, abrir `Documentos`;
- si el contexto trae `destinoPostRegistro = "documentos"` y `tieneTramiteDocumentalActivoEstado0 = true`, abrir `Documentos`;
- si el resolver detecta `gestionDocumentalPendiente = true` con tramite documental en estado `0`, abrir `Documentos`;
- en cualquier otro caso, abrir `Resumen`.

`Resumen` debe mostrar informacion minima del registro:

- radicado;
- id radicado;
- id estado radicado;
- estado asignacion;
- alertas si existen.

`Documentos` debe preparar la zona documental con estados:

- inactivo por no existir tramite documental activo en estado `0`;
- cargando gabinete;
- gabinete resuelto;
- gabinete no encontrado;
- error funcional;
- sin documentos todavia.

### 4. Preparar integracion documental

Esta fase puede dejar placeholders funcionales, pero deben estar tipados y conectados al contexto.

Debe quedar claro donde se integraran:

- `AppDigitalizador`;
- `AppUpload`;
- `AppVisorEmbedPdf`;
- lista de documentos;
- cambio de tipologia documental.

### 5. Resolver entrada directa a documentos

El panel debe tener un resolver tipado que determine si el radicado requiere gestion documental inmediata.

Regla de activacion:

```txt
Documentos solo se activa cuando existe al menos un tramite documental activo en estado 0.
Si no existe, no se habilita ni para consulta ni para carga.
```

Fuentes validas:

- `destinoPostRegistro` recibido desde FE-01;
- `MetadataOperativa` del registro;
- respuesta del endpoint de gabinete por `ConsecutivoRadicado`;
- consulta de tramites documentales pendientes que confirme estado `0`;
- resolver backend futuro, si existe, que indique documentos pendientes.

Fuentes no validas:

- localStorage como fuente unica;
- texto visible de la UI;
- banderas globales legacy;
- suposiciones basadas solo en que existe `ConsecutivoRadicado`.

### 6. Manejo de errores

Si el endpoint de gabinete responde `EstadoExistenciaRadicado = "NO"`:

- mostrar mensaje funcional;
- no romper el panel;
- permitir reintentar;
- mantener visible la metadata de radicacion.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FUERA DE ALCANCE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

No implementar todavia:

- upload real;
- digitalizacion real;
- scanner;
- visor PDF completo;
- tabla final de documentos;
- cambio de tipologia;
- eliminacion documental;
- firma digital;
- versionamiento documental;
- auto-vinculacion a expediente;
- envio workflow;
- endpoints backend nuevos.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLAS ARQUITECTONICAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

PROHIBIDO:

- copiar tablas Bootstrap legacy;
- copiar handlers jQuery;
- usar `.asmx`;
- usar variables globales tipo `CONST_STRU_RAD_ASIN`;
- acoplar documentos a `RadicacionForm`;
- llamar APIs desde componentes;
- asumir que existe `idTareaWorkflow`;
- ocultar errores del gabinete;
- introducir `any` nuevo.

OBLIGATORIO:

- usar service + hook/context;
- resolver gabinete por radicado como primer camino;
- mantener tipos estrictos;
- mostrar estados async explicitos;
- preservar metadata post-radicacion aunque falle gabinete;
- abrir directamente `Documentos` solo cuando el contexto o resolver determine tramite documental activo en estado `0`;
- mantener `Documentos` inactivo si no existe tramite documental activo en estado `0`;
- preparar integracion con componentes compartidos existentes.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## MAPEO LEGACY
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Legacy relacionado:

```text
Service_REST_solicita_estructura_estado_radicado_radicacion_simple
Service_REST_almacenamiento_documentos_digitalizados_rad_simplificada
Service_REST_actualiza_tipologia_rad_simplificada
Service_REST_olicita_url_documento_soporte_documental_rad_simple
Service_REST_elimina_documento_enlace_radicado
Service_REST_source_list_tipos_documentales_radicacion_simple
```

Esta fase NO reemplaza todos esos servicios. Solo prepara el punto moderno de entrada documental desde el radicado registrado.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CRITERIOS DE ACEPTACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- Existe contexto documental de radicacion.
- Existe servicio para resolver gabinete por radicado.
- El panel post-registro muestra tabs `Resumen` y `Documentos`.
- `Resumen` muestra metadata post-registro.
- `Documentos` intenta resolver gabinete por `ConsecutivoRadicado`.
- Si el radicado tiene tramite documental activo en estado `0`, el panel inicia directamente en `Documentos`.
- Si no existe tramite documental activo en estado `0`, `Documentos` permanece inactivo y no permite consulta ni carga.
- Si el usuario reingresa despues de salir del modulo, el resolver solo puede dirigirlo a `Documentos` si confirma tramite documental activo en estado `0`.
- Error de gabinete no rompe el panel.
- No se requiere `idTareaWorkflow` para abrir documentos.
- No se consume `.asmx`.
- No se introduce jQuery.
- Tests cubren:
  - gabinete success;
  - gabinete `EstadoExistenciaRadicado = NO`;
  - error backend;
  - render de resumen;
  - render de documentos en loading/error/ready.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## NOTA DE CONTINUIDAD
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Esta fase deja la plataforma lista para una fase posterior:

```txt
FE-04 - Integrar AppDigitalizador/AppUpload al panel documental de radicacion
```

No anticipar esa integracion en esta fase si obliga a mezclar demasiados cambios.

