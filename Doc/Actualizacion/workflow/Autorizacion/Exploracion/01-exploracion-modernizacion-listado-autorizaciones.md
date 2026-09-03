# Exploración de modernización del listado de autorizaciones

## Estado del documento

- Tipo: exploración técnica y funcional.
- Alcance: listado e historial de autorizaciones del Centro de Trabajo Workflow.
- Estado: diagnóstico inicial; no constituye autorización de implementación.
- Evidencia visual: `Grabación 2026-09-03 143718-detalle-autotizacion.mp4`.
- Duración analizada: 21,73 segundos.
- Resolución: 1622 × 890, 30 fps.

## Objetivo

Comprender el recorrido actual de consulta y descarga de autorizaciones, identificar sus dependencias Web Forms, riesgos funcionales y técnicos, y definir una orientación preliminar para una modernización segura sin afectar consumidores existentes.

## Recorrido observado

1. El usuario abre el menú `Historial` de una tarea seleccionada.
2. Activa `Lista de autorizaciones`.
3. El sistema ejecuta un postback mediante un control servidor oculto.
4. Se abre el modal legacy titulado `Autorizantes`.
5. El modal muestra el total de registros y una tabla con autorizante, cargo, fecha, radicado, trámite, actividad, actividad del usuario, flujo y beneficiario.
6. Cada fila presenta una acción verde para descargar el XML individual.
7. El pie del modal presenta una acción `Descargar` para obtener el consolidado.
8. Al descargar el XML individual, el navegador abandona visualmente el modal y muestra el XML crudo en pantalla.
9. Al regresar, el modal recupera el listado.

La grabación confirma el recorrido de apertura, consulta y descarga individual. No demuestra paginación con múltiples páginas, ordenamiento, estado vacío, descarga consolidada, error de servidor ni comportamiento responsive.

## Superficie legacy identificada

| Responsabilidad | Elemento actual |
|---|---|
| Acceso desde la tarea | `Historial` → `Lista de autorizaciones` |
| Disparador servidor | `ImageButton_ista_autorizacio` |
| Modal | `Panel_lista_autorizacion` y `ModalPopupExtender_edition_lista_autorizacion` |
| Contenido parcial | `UpdatePanel_contenido_lista_autorizacion` |
| Tabla | `data_grid_listado_solicitudes` (`GridView`) |
| Selección de descarga | `Hidden_selec_list` |
| Postback de descarga | `Button_dowload_xml` |
| Descarga consolidada | `Button_descarga_consolidado_aprobacion` |
| Lógica de presentación | `workflow/Webworkflow.aspx.vb` |
| JavaScript principal | `js/workflow/Webworkflow.js` |
| JavaScript de descarga | `js/workflow/Webworkflow_2.js` |
| Dominio y acceso a datos legacy | `workflow/Class_autoriza_tarea_worklfow.vb` |
| Tabla consultada | `wf_autoriza_tarea` |

## Flujo técnico actual

```text
Usuario
  │
  ├─ Historial → Lista de autorizaciones
  │
  ▼
Control servidor oculto / postback
  │
  ▼
Webworkflow.aspx.vb
  │
  ▼
Class_autoriza_tarea_worklfow.Lista_autorizaciones_tarea
  │
  ├─ Construye SQL
  ├─ Consulta wf_autoriza_tarea
  ├─ Actualiza Label y GridView
  └─ Inserta controles HTML de descarga por fila
  │
  ▼
ModalPopupExtender + UpdatePanel + GridView
```

La descarga individual transporta el identificador mediante `Hidden_selec_list`, dispara `Button_dowload_xml` y delega la generación a `Descarga_archivo_xml`. El consolidado usa `Solicita_consolidado_autorizacion`.

## Hallazgos

### Prioridad crítica

#### Descarga XML fuera del contexto de la aplicación

La grabación muestra el XML crudo en la superficie del navegador. Esto rompe la continuidad del recorrido, expone estructura técnica y permite que extensiones del navegador procesen el contenido. La descarga futura debe entregarse como archivo adjunto, con nombre y tipo controlados, sin navegar al cuerpo XML.

#### Identidad de tarea dependiente de sesión

El consumidor principal obtiene la tarea desde `Session("ID_TAREA_SELECCIONDA")`. Existe además una variante que usa `HiddenSeleccion`. Esta combinación puede producir inconsistencias entre pestañas, postbacks parciales o selecciones recientes. Un contrato moderno debe recibir `idTarea` explícito y revalidarlo en servidor.

#### SQL construido por concatenación

La consulta concatena el identificador de tarea, el nombre de columna y la dirección de orden. La tarea debe parametrizarse y los campos de orden deben resolverse mediante una lista blanca de valores internos.

### Prioridad alta

#### Presentación acoplada al acceso a datos

`Lista_autorizaciones_tarea` recibe y modifica directamente `Label`, `GridView` y `UpdatePanel`. No existe un contrato independiente que pueda reutilizar una UI moderna o probarse sin controles Web Forms.

#### Estado distribuido

El flujo conserva criterios y orden en variables de sesión y distribuye el comportamiento entre markup, code-behind y dos archivos JavaScript. La ruta de descarga por fila depende específicamente de `Webworkflow_2.js`.

#### Revalidación de descarga

La interfaz transporta el identificador de autorización mediante un campo oculto. El servidor debe considerar ese identificador una intención no confiable y verificar nuevamente acceso a la tarea, pertenencia de la autorización y permiso de descarga.

#### Errores internos visibles

Diversos `Catch` envían `ex.Message` a la interfaz mediante scripts. La solución futura debe traducir fallos a códigos funcionales saneados y registrar el detalle solo en el mecanismo interno autorizado.

#### Consumidor compartido

`Class_autoriza_tarea_worklfow` también es consumida por `WebFormConsultaTareasWorkflow`. No puede retirarse como parte de una modernización exclusiva del Centro de Trabajo sin inventario, adaptación y regresión del segundo consumidor.

### Prioridad media

- El modal usa aproximadamente 90 % del ancho, incluso para un solo registro.
- Una fila requiere desplazamiento horizontal debido a la cantidad de columnas.
- `ACTIVIDAD` y `ACTIVIDAD_USUARIO` no expresan claramente su diferencia funcional.
- La paginación fija de siete registros depende de `GridView` y `ViewState`.
- No se observan estados modernos de carga, vacío, error o reintento.
- La acción individual se representa solo mediante un icono y depende de `title`.
- El gran espacio vacío reduce la jerarquía visual y dificulta reconocer las acciones principales.

## Ambigüedad funcional

La interfaz se presenta como historial o lista de autorizaciones, pero la consulta actual filtra `estado_autorizacion = 1`. En consecuencia, parece mostrar únicamente autorizaciones vigentes y no un historial completo.

Antes de diseñar el contrato deben resolverse estas preguntas:

1. ¿La superficie debe mostrar autorizaciones vigentes o toda la historia?
2. ¿Deben aparecer autorizaciones anuladas y quién/cuándo las anuló?
3. ¿Una misma persona puede autorizar varias veces la misma tarea o actividad?
4. ¿El contador superior representa autorizaciones vigentes, históricas o pendientes?
5. ¿La descarga individual y el consolidado incluyen registros anulados?
6. ¿XML sigue siendo un formato orientado al usuario o solo a integración/auditoría?
7. ¿Qué roles pueden consultar y descargar autorizaciones?

## Orientación arquitectónica preliminar

```text
Acceso moderno de Historial
          │
          ▼
Modal accesible y estable
          │
          ▼
Contrato ASMX moderno de solo lectura
          │
          ├─ idTarea explícito
          ├─ autorización reconstruida en servidor
          ├─ paginación y orden estable
          ├─ SQL parametrizado
          └─ respuesta mínima y saneada
          │
          ▼
Servicio de aplicación
          │
          ▼
Repositorio de autorizaciones
          │
          ▼
wf_autoriza_tarea
```

La interfaz no debe autorizar por sí misma ninguna consulta o descarga. El navegador expresa la intención y el servidor reconstruye la tarea, usuario, actividad, permisos y pertenencia del registro.

## Contrato de lectura preliminar

Un DTO de listado podría contener únicamente:

- `IdAutorizacion`
- `Autorizante`
- `Cargo`
- `FechaAutorizacion`
- `ActividadTarea`
- `ActividadAutorizante`
- `Flujo`
- `Estado`
- `PuedeDescargar`

Radicado, trámite y beneficiario deberían aparecer una sola vez en el contexto del modal cuando sean invariantes para la tarea. La inclusión por fila debe justificarse si pueden variar históricamente.

## Experiencia visual sugerida

- Acceso visible desde `Historial`, conservando el lenguaje del Centro de Trabajo.
- Modal superpuesto de tamaño estable con scroll interno.
- Encabezado `Historial de autorizaciones`, contexto resumido de la tarea y contador.
- Estado de carga con controles temporalmente deshabilitados.
- Estado vacío con explicación clara.
- Estado de error saneado y opción de reintento.
- Tabla en escritorio y presentación adaptable en pantallas estrechas.
- Filtros por estado y autorizante cuando el volumen lo justifique.
- Orden estable por fecha e identificador.
- Descarga individual accesible por teclado y con texto comprensible.
- Descarga consolidada como acción secundaria.
- Conservación de página, filtros y modal después de descargar.
- Cierre por botón, Escape y retorno del foco al disparador.

## Estrategia de descarga sugerida

1. Recibir identificadores explícitos de tarea y autorización.
2. Revalidar sesión autenticada y acceso a la tarea.
3. Verificar que la autorización pertenece a la tarea.
4. Generar el documento sin revelar rutas físicas.
5. Responder como archivo adjunto mediante `Content-Disposition`.
6. Utilizar un nombre de archivo estable y saneado.
7. Mantener abierto el modal y preservar su estado.

Debe evaluarse PDF para lectura humana y reservar XML para integración o auditoría técnica. Esta decisión es funcional y no debe tomarse solo desde implementación.

## Alcance seguro por etapas

1. Confirmar la semántica de historial, estados y descargas.
2. Inventariar consumidores de `Class_autoriza_tarea_worklfow` y de los controles legacy.
3. Separar consulta y reglas de los controles Web Forms.
4. Introducir repositorio parametrizado y contrato moderno de lectura.
5. Modernizar el consumidor del Centro de Trabajo.
6. Modernizar descarga individual y consolidada.
7. Validar regresión de `WebFormConsultaTareasWorkflow`.
8. Retirar controles legacy solo cuando sus referencias sean cero.
9. Retirar la clase legacy únicamente después de migrar todos sus consumidores.

## Cobertura esperada para un cambio futuro

- Rechazo anónimo.
- Tarea inexistente, ajena, inactiva o no autorizada.
- Identidad explícita y aislamiento entre pestañas.
- Lectura sin mutación de tarea, estado ni auditoría.
- Autorizaciones vigentes y anuladas según la decisión funcional.
- Estado vacío, una fila y múltiples páginas.
- Ordenamiento permitido y rechazo de columnas arbitrarias.
- Descarga individual válida y rechazo de autorización cruzada.
- Descarga consolidada.
- Respuestas y nombres de archivo saneados.
- Modal responsive, foco, teclado, Escape y scroll interno.
- Conservación del estado después de descargar.
- Regresión del consumidor `WebFormConsultaTareasWorkflow`.

Las pruebas autenticadas o descargas reales futuras requerirán ambiente, cuenta, tarea/datos descartables y autorización explícita. Esta exploración no autoriza ninguna ejecución real.

## Conclusión preliminar

La modernización no debe limitarse a cambiar estilos del modal. La deuda central está en el acoplamiento entre sesión, SQL, controles Web Forms, descarga y reglas de acceso. La dirección recomendada es establecer primero un contrato seguro de lectura y descarga, adaptar después el Centro de Trabajo y conservar temporalmente los consumidores legacy que continúen activos.

El alcance tiene suficiente complejidad y riesgo transversal para tratarse como un DOC independiente, con propuesta, diseño, especificación, tareas, pruebas focales y E2E integrada cuando exista autorización.
