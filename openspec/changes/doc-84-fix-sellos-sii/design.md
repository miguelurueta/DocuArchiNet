## Context

DOC-84: FIX-SELLOS-SII

## Jira Details

> Prompt 09 — Corrección de proyección de sellos en la lista Workflow
> Corrige la regresión visual de la importación de sellos o constancias de inscripción sin modificar el comportamiento aprobado de la importación ENLASE.
> Depende de F05–F07 y de la proyección ENLASE estabilizada en DOC-83. La solución debe ser aditiva: sellos obtiene una proyección tipada y un insertador JavaScript propios; ENLASE conserva íntegramente su recorrido actual.
> Este prompt correctivo reemplaza, exclusivamente para sellos y ENLASE, el fallback de recarga autoritativa permitido originalmente por el Prompt 06. No altera las demás responsabilidades ni criterios de reconciliación de ese prompt.
> Objetivo
> Después de confirmar la importación de un sello o constancia, insertar una sola fila completa y operable en GridView_list_documento_relacion_wf mediante JavaScript, conservando gabinete, identificador, radicado, formato físico, tipología, tarea, estado de firma e icono, sin ejecutar recarga parcial o completa, postback, DataBind ni una consulta adicional para reconstruir la fila.
> Diagnóstico obligatorio
> La implementación actual diferencia correctamente el almacenamiento de las capacidades, pero comparte indebidamente el adaptador visual:
> ENLASE usa LegacyEnlaseImportDocumentStorageAdapter, EnlaseProjection y el destino JavaScript rad.
> 
> Sellos usa LegacyImportDocumentStorageAdapter y el destino JavaScript wf.
> 
> Para wf, createLegacyGridAppender construye actualmente una fila parcial con gabinete, radicado y formato vacíos, y con icono genérico.
> 
> Al retornar éxito después de esa inserción parcial, se impide cualquier corrección posterior y quedan incompletos idd_wf, la tipología, el formato y las acciones del documento.
> 
> No resolver el problema reutilizando EnlaseProjection, modificando la proyección ENLASE ni recargando el GridView desde servidor.
> Rutas canónicas de implementación
> Modelo/Workflow/ImportarServicioWeb/
> └── ImportarServicioWebModels.vb
> 
> DTOs/Workflow/ImportarServicioWeb/
> └── ImportarServicioWebDtos.vb
> 
> Infrastructure/Workflow/ImportarServicioWeb/Storage/
> └── LegacyImportDocumentStorageAdapter.vb
> 
> Services/Workflow/ImportarServicioWeb/
> ├── ImportExecutionSteps.vb
> ├── ImportServiceOrchestrator.vb
> └── ServicioReconciliacionImportacion.vb
> 
> js/workflow/importar-servicio-web/
> ├── importar-servicio-web-progress-adapter.js
> ├── importar-servicio-web-reconciliation.js
> ├── importar-servicio-web-document-list-adapter.js
> ├── importar-servicio-web-workflow-document-list-adapter.js
> └── importar-servicio-web-ui.js
> 
> workflow/
> └── Webworkflow.aspx.vb
> 
> Tests/
> ├── importar-servicio-web-workflow-projection.test.cjs
> ├── importar-servicio-web-document-list-adapter.test.cjs
> ├── importar-servicio-web-reconciliation-ui.test.cjs
> ├── importar-servicio-web-enlase-persistence-contract.test.cjs
> └── importar-servicio-web-enlase-ui.test.cjsEl nombre del nuevo módulo JavaScript puede ajustarse a la convención real, pero debe quedar separado conceptualmente de la proyección ENLASE.
> 
> Registrar cualquier archivo nuevo en GestionDocumental-Docuarchi.net.vbproj y cargarlo antes de importar-servicio-web-ui.js.
> 
> Incrementar la versión de los recursos JavaScript afectados para invalidar caché; no reutilizar una versión ya desplegada.
> 
> No crear otra raíz frontend, otro GridView ni una segunda lista documental.
> 
> Ruta documental obligatoria
> Doc/Actualizacion/workflow/ImportarServicioWeb/SCRUMCORE-000-correccion-proyeccion-sellos-workflow/Sustituir SCRUMCORE-000 por el ticket real. Documentar allí diagnóstico, contrato de proyección, matriz de regresión, archivos modificados, pruebas y evidencia saneada.
> Contratos que deben permanecer separados
> ENLASE — invariante protegido
> PreAlmacenaDocumentoAnexosEnlaceIntegracionSII
>   -> LegacyEnlaseImportDocumentStorageAdapter
>   -> ProyeccionDocumentoEnlase
>   -> ImportEnlaseDocumentProjectionDto
>   -> enlaseProjection
>   -> insert_row_documento_relacionado(contratoCompleto, "rad", 1)
>   -> GridView_list_documento_relacionSellos o constancias — nueva frontera
> AlmacenaDocumentoTareaWorkflow(..., ByRef EstructuraDatosImagen)
>   -> LegacyImportDocumentStorageAdapter
>   -> ProyeccionDocumentoWorkflow
>   -> ImportWorkflowDocumentProjectionDto
>   -> workflowProjection
>   -> insert_row_documento_relacionado(contratoCompleto, "wf", 1)
>   -> GridView_list_documento_relacion_wfLas dos proyecciones pueden representar campos equivalentes, pero no deben compartir el DTO, el nombre de propiedad, el constructor de datos delimitados ni la selección de destino.
> Flujo paso a paso obligatorio
> La implementación y sus pruebas deben demostrar esta secuencia funcional completa:
> El usuario abre el importador desde la acción de sellos o constancias, no desde ENLASE.
> 
> El contexto confiable conserva proveedor, tarea, radicado y capacidad correspondiente a sellos.
> 
> El usuario selecciona el documento y la tipología autorizada.
> 
> Preflight e intención conservan DocumentTypeId, DocumentTypeName y TargetTaskId.
> 
> ExecuteImportIntent descarga, prepara y almacena el documento una sola vez.
> 
> AlmacenaDocumentoTareaWorkflow devuelve IdDocumento y EstructuraDatosImagen al adaptador existente.
> 
> LegacyImportDocumentStorageAdapter construye la nueva proyección Workflow completa sin consultar nuevamente servidor o base de datos.
> 
> La ejecución y reconciliación confirman documento y tarea, y preservan la proyección efímera.
> 
> El frontend valida los ocho campos y comprueba que TaskId continúa siendo la tarea visible.
> 
> JavaScript traduce la proyección al contrato histórico y llama una sola vez a insert_row_documento_relacionado(..., "wf", 1).
> 
> El adaptador verifica que la fila completa existe en GridView_list_documento_relacion_wf y que no está duplicada.
> 
> Solo después de esa comprobación se permite cerrar el modal; cualquier inconsistencia mantiene visible el resultado y no crea una fila parcial.
> 
> La prueba de flujo debe demostrar además que ninguna etapa activa el destino rad, toca la proyección ENLASE, dispara un postback o solicita una recarga del GridView.
> Implementa
> Modelo interno efímero ProyeccionDocumentoWorkflow o nombre equivalente, independiente de ProyeccionDocumentoEnlase.
> 
> DTO público aditivo ImportWorkflowDocumentProjectionDto en ImportItemResultDto, sin retirar ni reinterpretar campos existentes.
> 
> Construcción cerrada de la proyección dentro de LegacyImportDocumentStorageAdapter utilizando exclusivamente el stru_datos_image_lista retornado por el almacenamiento confirmado y el comando ya validado.
> 
> Propagación de la proyección por ResultadoFaseImportacion, ResultadoElementoImportacion, ejecución, respuesta pública, adaptador de progreso y reconciliación.
> 
> Preservación de la proyección efímera cuando la reconciliación sustituya el estado del elemento por su estado autoritativo.
> 
> Adaptador JavaScript exclusivo para Workflow que valide identidad de documento y tarea y traduzca la proyección tipada al contrato histórico solo en la frontera cliente.
> 
> Despacho explícito por capacidad: ENLASE usa únicamente su appender actual; sellos usa únicamente el nuevo appender Workflow.
> 
> Deduplicación por DocumentId dentro de GridView_list_documento_relacion_wf.
> 
> Inserción únicamente para estados confirmados y para la tarea que continúa visible.
> 
> Cierre del modal solo después de comprobar que la fila completa existe en el GridView correcto.
> 
> Fallo cerrado: una proyección incompleta no genera una fila parcial; la ventana permanece abierta e informa que el documento fue importado pero no pudo proyectarse con seguridad.
> 
> Contrato obligatorio de la proyección Workflow
> La traducción cliente debe generar los ocho campos consumidos por insert_row_documento_relacionado(...):
> gabinete|documentId|radicado|tipoFisico|tipologia|taskId|estadoFirma|claseIconoResolverlos sin una segunda consulta:
> gabinete: imagen.nombre_gabinete; respaldo seguro comando.NombreGabinete.
> 
> documentId: identificador confirmado retornado por el almacenamiento.
> 
> radicado: imagen.radicado; respaldo seguro comando.Radicado.
> 
> tipoFisico: imagen.DBT; cuando el legado no lo llena, usar imagen.extension.
> 
> tipologia: imagen.notipodocumento; respaldo seguro comando.DescripcionTipo.
> 
> taskId: comando.IdTareaWorkflow y debe coincidir con la tarea visible.
> 
> estadoFirma: imagen.estado_firma_digital.
> 
> claseIcono: imagen.icono_icono_awe_some; solo aceptar un respaldo conocido y explícito si el formato confirmado permite determinarlo sin consultar servidor.
> 
> Escapar los valores exclusivamente al construir la cadena delimitada en JavaScript. No transportar ni persistir dato_lista desde backend.
> Restricciones críticas
> No modificar LegacyEnlaseImportDocumentStorageAdapter.vb.
> 
> No modificar ProyeccionDocumentoEnlaseImportacion, ImportEnlaseDocumentProjectionDto, MapEnlaseProjection, enlaseData() ni el contrato del destino rad.
> 
> No cambiar cuándo ENLASE se considera confirmado, reconciliado, deduplicado o visible.
> 
> No invocar Button_actualiza_trevie_seleccion.click() como parte de la importación.
> 
> No utilizar refreshDocumentListPartial, PageRequestManager, window.location.reload(), postback, DataBind ni recarga total o parcial del GridView como solución o fallback.
> 
> No agregar consultas para volver a leer el documento recién almacenado; la proyección debe salir del resultado de almacenamiento ya disponible.
> 
> No modificar AlmacenaDocumentoTareaWorkflow(...), ClassAlmacenamiento ni consumidores legacy.
> 
> No reutilizar una proyección parcial con campos vacíos, icono genérico o nombre RAD-... cuando existe tipología seleccionada.
> 
> No insertar durante la espera, ante resultado incierto, para otra tarea ni antes de la confirmación autoritativa.
> 
> No cerrar el modal fingiendo éxito visual si la fila no pudo validarse.
> 
> Tareas atómicas sugeridas
> Caracterizar con pruebas el comportamiento aprobado de ENLASE y registrar una línea base que deba permanecer idéntica.
> 
> Agregar el modelo y DTO tipados exclusivos para Workflow.
> 
> Construir la proyección completa en LegacyImportDocumentStorageAdapter después del almacenamiento exitoso.
> 
> Propagar la proyección sin persistir cadenas delimitadas ni estado web.
> 
> Preservarla en progreso, reconciliación y recuperación de conflictos.
> 
> Crear el appender Workflow con validación cerrada, escape y deduplicación.
> 
> Separar en la UI el despacho de appenders por capacidad sin alterar la rama ENLASE.
> 
> Retirar del cierre de importación de sellos cualquier intento de recarga del GridView.
> 
> Actualizar versiones de recursos y pruebas de arquitectura/caché.
> 
> Ejecutar la matriz focal y documentar resultados, limitaciones y evidencia.
> 
> Criterios de aceptación
> Un sello importado aparece inmediatamente y una sola vez en GridView_list_documento_relacion_wf sin comunicación destinada a recargar la lista.
> 
> La fila muestra la tipología seleccionada, por ejemplo Constancia De Inscripción, y nunca sustituye ese valor por RAD-<radicado>.
> 
> El formato físico y el icono corresponden al documento almacenado.
> 
> El atributo idd_wf contiene los ocho campos completos y en el orden histórico esperado.
> 
> Abrir, eliminar, cambiar tipología, firmar, reemplazar y consultar versiones funcionan sobre la fila insertada.
> 
> Un lote de varios sellos inserta cada documento una sola vez y conserva el orden confirmado sin asumir que coincide con el orden visual externo.
> 
> Una proyección incompleta, una tarea distinta o un resultado incierto no insertan filas.
> 
> Ninguna prueba o implementación de este cambio hace clic en Button_actualiza_trevie_seleccion, ejecuta DataBind o recarga la página/lista.
> 
> ENLASE continúa insertando en GridView_list_documento_relacion mediante insert_row_documento_relacionado(..., "rad", 1) con su proyección completa actual.
> 
> Las suites focales de ENLASE conservan sus aserciones y pasan sin relajar contratos.
> 
> Pruebas obligatorias
> Contrato backend de los ocho campos Workflow y rechazo de campos obligatorios vacíos.
> 
> Fallback de DBT a extension sin confundir formato físico con tipología.
> 
> Preservación de tipología, estado de firma e icono.
> 
> Mapeo de ejecución, progreso y reconciliación de workflowProjection.
> 
> Inserción JavaScript con destino wf, escape de HTML y eliminación del carácter delimitador en valores visibles.
> 
> Deduplicación, lote múltiple y aislamiento por TaskId.
> 
> Operabilidad de todas las acciones que consumen idd_wf.
> 
> Prueba negativa que falle si se invoca cualquier mecanismo de recarga o postback después de importar.
> 
> Prueba negativa que falle si sellos utiliza enlaseProjection o si ENLASE utiliza workflowProjection.
> 
> Regresión completa de importar-servicio-web-enlase-* y de los contratos DOC-81/DOC-83.
> 
> Verificación de que la versión registrada del recurso cambió y coincide con el archivo desplegado.
> 
> Las pruebas locales deben ser deterministas, sin autenticación, red ni mutación externa. Antes de una E2E autenticada, leer AGENTS.md y tools/e2e/AGENT-RUNBOOK.md; no ejecutarla sin autorización explícita para ambiente, cuentas y datos descartables.
> Documentación técnica
> Actualizar exclusivamente el paquete de la Ruta documental obligatoria con:
> causa raíz y commit donde apareció la regresión;
> 
> diagrama comparativo ENLASE frente a sellos;
> 
> definición de ambas proyecciones y sus invariantes;
> 
> tabla de archivos creados o modificados;
> 
> evidencia de ausencia de recargas;
> 
> comandos y resultados de pruebas;
> 
> limitaciones y condiciones para E2E autorizada.
> 
> Antes de crear documentación nueva, ubicar y revisar la documentación existente de DOC-81/DOC-83 y cualquier paquete posterior que describa la proyección de lista documental. Actualizar las referencias vigentes afectadas por la corrección sin duplicarlas. Si una referencia esperada no existe o no debe modificarse por pertenecer a un cambio archivado, registrar expresamente su ausencia o inmutabilidad y enlazar desde el nuevo paquete la ruta canónica que la reemplaza.
> Entregable final
> Entregar código, pruebas y documentación coherentes con la implementación real. El resumen debe separar expresamente:
> comportamiento nuevo de sellos;
> 
> evidencia de que ENLASE permaneció intacto;
> 
> evidencia de que no existe recarga parcial o completa;
> 
> suites ejecutadas y cualquier validación bloqueada por falta de autorización.
> 
> Correcciones opsxj:prompt-review
> Estas reglas materializan una regresión reproducida y deben conservarse al convertir el prompt en propuesta, diseño y tareas atómicas. No relajar los invariantes de ENLASE ni aceptar una recarga como solución temporal.
> Rol esperado
> Actuar como desarrollador senior con experiencia en   WebForms,   y JavaScript legacy, preservando contratos ya estabilizados y separando estrictamente las capacidades ENLASE y sellos.
> Contexto obligatorio
> Antes de implementar, leer AGENTS.md, los Prompts 05–08, la evidencia DOC-83, los dos adaptadores de almacenamiento, los DTO/modelos de importación, insert_row_documento_relacionado(...) y las pruebas focales relacionadas. Revisar el diff del commit 0e8199c8 para comprender la regresión; no revertir el commit completo.
> Criterio de cierre
> No declarar la corrección terminada únicamente porque el documento se vea en pantalla. La fila debe contener el contrato completo, soportar sus acciones, no duplicarse, corresponder a la tarea visible y haberse insertado sin recarga. ENLASE debe conservar evidencia de regresión sin cambios funcionales.
> Requisitos positivos
> Mantener contratos tipados hasta la frontera JavaScript.
> 
> Reutilizar insert_row_documento_relacionado(...) sin modificar su implementación legacy.
> 
> Conservar una única lista documental por contexto.
> 
> Preferir validación cerrada a filas visibles pero inoperables.
> 
> Mantener el cambio mínimo, aditivo y reversible.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. Las decisiones funcionales y tecnicas se completan durante `opsxj:refine`; no se inyectan politicas de otro perfil tecnologico.


## Risks / Trade-offs

- El refinamiento debe identificar compatibilidad, riesgos y limites del modulo afectado antes de iniciar cambios.

## Migration Plan

1. Completar y aprobar `refinement.md` antes de marcar tareas de implementacion.
2. Sincronizar cada decision con design, spec y tasks mediante `opsxj:refine --sync`.

## Open Questions

- TBD
