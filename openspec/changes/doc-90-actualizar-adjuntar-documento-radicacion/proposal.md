## Why

ACTUALIZAR-ADJUNTAR-DOCUMENTO-RADICACION. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue DOC-90.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> Prompt 01 — Corregir adjunto de documentos en Radicación clásica sin regresionar Radicación Simplificada
> ROL ESPERADO
> Actúa como arquitecto y desarrollador senior especialista en   Web Forms,  , .NET Framework 4.6.1, JavaScript legacy y MySQL. Corrige la regresión que impide adjuntar documentos desde Radicación clásica (radicador/WebFormRadicacionEntrante.aspx) después de la separación de contexto implementada para Radicación Simplificada en DOC-85.
> La corrección debe distinguir inequívocamente los dos consumidores que hoy comparten ADJUNTARADICACION. Debe restaurar el comportamiento de Radicación clásica sin debilitar, omitir ni rodear las validaciones autoritativas de DOC-85 para Radicación Simplificada.
> UNIDAD DE ENTREGA Y GOBIERNO E2E
> Código, pruebas, documentación y validación forman una única unidad de entrega. No declarar el cambio terminado solo porque desaparezca el mensaje visual.
> Antes de ejecutar cualquier E2E autenticada o carga real, leer AGENTS.md y tools/e2e/AGENT-RUNBOOK.md. No ejecutar E2E, carga documental ni consultas contra un ambiente real sin autorización explícita para el ambiente, la cuenta y los recursos descartables.
> No imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión.
> 
> Las consultas de control deben ser exclusivamente SELECT parametrizados.
> 
> No activar ni reintroducir WorkflowCentroTrabajoModernActive, listas de usuarios o grupos.
> 
> Si falta autorización E2E, registrar el bloqueo; no inventar evidencia ni sustituir la E2E con mocks.
> 
> OBJETIVO
> Permitir nuevamente la carga de documentos desde Radicación clásica conservando su contexto autorizado, y mantener intacta la corrección DOC-85 de Radicación Simplificada.
> El resultado debe garantizar:
> Radicación clásica no entra accidentalmente en la ruta exclusiva de Radicación Simplificada.
> 
> Radicación Simplificada continúa exigiendo un id_estado_radicado positivo, autorizado y coherente con el radicado informativo.
> 
> Cada origen utiliza una ruta explícita y verificable; no se infiere el módulo por la presencia o ausencia accidental de parámetros.
> 
> Los demás consumidores del cargador compartido mantienen su comportamiento.
> 
> DIAGNÓSTICO CONFIRMADO
> El error visible en Radicación clásica es:
> El registro seleccionado no es válido para adjuntar el documento.El punto exacto de rechazo está en la sobrecarga de doce argumentos de ClassAlmacenamiento.UploadSaveFile(...):
> If IdRegistroEstadoRadicacion <= 0 Then
>     Return "El registro seleccionado no es válido para adjuntar el documento."
> End IfRecorrido defectuoso actual
> radicador/WebFormRadicacionEntrante.aspx
>   -> js/radicacion/WebFormRadicacionEntrante.js
>   -> ActivaAdjuntarDocumentoRadicacion
>   -> evento_adjunta = "ADJUNTARADICACION"
>   -> no configura IdRegistroEstadoRadicacion ni RadicadoRadicacion
>   -> FileUploadHandler.js aplica valores por defecto 0 y ""
>   -> multipart envía id_registro_estado_radicacion = 0
>   -> FileUploadHandler_.ashx.vb trata todo ADJUNTARADICACION como Simplificada
>   -> llama UploadSaveFile con doce argumentos
>   -> la ruta exclusiva de Simplificada rechaza el ID 0
>   -> no se almacena el documentoCausa raíz
> Radicación clásica y Radicación Simplificada comparten el mismo nombre de evento ADJUNTARADICACION, pero tienen contratos de contexto diferentes.
> DOC-85 convirtió la rama del handler asociada a ese evento en una llamada obligatoria de doce argumentos y movió la implementación a una ruta exclusiva de Radicación Simplificada. La pantalla clásica, que ya utilizaba el mismo evento, no fue incluida como consumidor funcional en la separación. Su JavaScript nunca configuró los dos campos nuevos, por lo que el handler envía los valores predeterminados.
> No atribuir este defecto al PDF, la tipología, la fecha, la conexión, los permisos de almacenamiento o la caché. El rechazo ocurre antes de esas etapas.
> EVIDENCIA HISTÓRICA A REVISAR
> Usar Git en modo de solo lectura para comparar como mínimo:
> 57d43b4f^  # handler antes de incorporar el contexto explícito
> 57d43b4f   # transporte inicial del contexto
> f7171925^  # UploadSaveFile antes de la separación DOC-85
> f7171925   # separación de sobrecargas y ruta DOC-85Confirmar que antes de DOC-85:
> la rama ADJUNTARADICACION del handler llamaba la sobrecarga legacy de diez argumentos;
> 
> la ruta clásica resolvía el registro desde el contexto de sesión existente;
> 
> js/radicacion/WebFormRadicacionEntrante.js ya usaba ADJUNTARADICACION;
> 
> la pantalla clásica no enviaba IdRegistroEstadoRadicacion ni RadicadoRadicacion.
> 
> La evidencia histórica sirve para recuperar la semántica, no para copiar indiscriminadamente código antiguo ni para revertir DOC-85.
> DISEÑO OBLIGATORIO
> 1. Separar explícitamente los orígenes
> Introducir una discriminación explícita entre:
> Radicación clásica
> Radicación SimplificadaLa alternativa preferida es conservar el nombre histórico ADJUNTARADICACION para Radicación clásica y asignar a Simplificada un evento específico, por ejemplo:
> ADJUNTARADICACION_SIMPLIFICADASi se adopta otro discriminador, debe ser tipado o constante, viajar explícitamente y validarse en servidor. No usar como único criterio:
> que id_registro_estado_radicacion sea cero o no exista;
> 
> el nombre de una función JavaScript como autorización;
> 
> un texto visible, nombre de archivo o elemento del DOM;
> 
> una combinación implícita de variables de sesión no documentada.
> 
> 2. Ruta de Radicación clásica
> Radicación clásica debe recuperar su comportamiento autorizado anterior usando su contexto real de servidor. La implementación debe:
> validar que el módulo y la sesión correspondan a Radicación clásica;
> 
> obtener y validar el registro seleccionado mediante la identidad de sesión vigente, incluida RA_ID_REGISTRO_RADICADO cuando corresponda al contrato histórico;
> 
> rechazar sesión inexistente, ID cero, registro inexistente o contexto incompatible;
> 
> resolver tarea, radicado, trámite, gabinete y tipología antes de almacenar;
> 
> reutilizar el almacenamiento existente;
> 
> conservar la respuesta uploadFiles y la inserción actual en GridView_list_documento_relacion;
> 
> evitar confiar en datos editables del navegador como fuente de autorización.
> 
> No restaurar un fallback global que permita a Radicación Simplificada aceptar un ID cero.
> 3. Ruta de Radicación Simplificada
> La ruta DOC-85 debe conservar:
> IdRegistroEstadoRadicacion > 0 obligatorio;
> 
> resolución parametrizada por registro y usuario;
> 
> ra_rad_estados_modulo_radicacion.consecutivo_radicado como fuente autoritativa;
> 
> comparación del radicado informativo cuando se envía;
> 
> ContextoAdjuntoRadicacion inmutable;
> 
> una única resolución del contexto por carga lógica;
> 
> ausencia de redescubrimiento del radicado desde DAT_ADIC_TAR;
> 
> rechazo previo al almacenamiento ante identidad, pertenencia o contexto inválidos.
> 
> No cambiar la ruta Simplificada para que vuelva a depender de sesión mutable como única autoridad.
> 4. Handler y sobrecargas
> FileUploadHandler_.ashx.vb debe seleccionar la sobrecarga adecuada mediante el origen explícito:
> Radicación clásica       -> contrato/ruta clásica
> Radicación Simplificada  -> sobrecarga de doce argumentos y ServicioAdjuntoRadicacionNo mantener una única rama ambigua que atienda ambas pantallas con contratos incompatibles.
> Conservar sin cambios funcionales las rutas de:
> GESTION_RESPUESTA;
> 
> WORKFLOWSELECCION;
> 
> WORKFLOWENLACE;
> 
> PRODUCCION;
> 
> ENLACE_RADICADO;
> 
> CONSULTA_RADICADO;
> 
> ENLASE, SII e importación de sellos.
> 
> 5. JavaScript y caché
> Actualizar únicamente los consumidores necesarios. Si cambia algún JavaScript referenciado por una página ASPX, actualizar su versión de recurso para impedir que el navegador mezcle contratos anteriores y nuevos.
> No introducir nuevos globales compartidos ni depender de que FilePerson conserve estado perteneciente a otra pantalla.
> FLUJO OBJETIVO
>                          ┌─ Radicación clásica
> UI de origen ─ contexto ─┤    -> ruta clásica autorizada
>                          │    -> almacenamiento existente
>                          │
>                          └─ Radicación Simplificada
>                               -> ID explícito obligatorio
>                               -> ServicioAdjuntoRadicacion
>                               -> contexto autoritativo DOC-85
>                               -> almacenamiento existenteLas dos rutas pueden reutilizar infraestructura común después de validar su contexto, pero no deben compartir una entrada ambigua.
> RUTAS CANÓNICAS DE REVISIÓN
> Leer completamente, como mínimo:
> AGENTS.md
> tools/e2e/AGENT-RUNBOOK.md
> 
> radicador/WebFormRadicacionEntrante.aspx
> radicador/WebFormRadicacionEntrante.aspx.vb
> js/radicacion/WebFormRadicacionEntrante.js
> 
> RadicadorSimplificado/Web_form_radicacion_simpilificada.aspx
> js/RadicadorSimplificado/Web_form_radicacion_simpilificada.js
> 
> generic_control/FileUploadHandler.js
> generic_control/FileUploadHandler_.ashx.vb
> workflow/ClassAlmacenamiento.vb
> 
> Modelo/RadicacionSimplificada/Adjuntos/
> Services/RadicacionSimplificada/Adjuntos/
> Infrastructure/Repositories/RadicacionSimplificada/Adjuntos/
> 
> tests/radicacion-simple-attachment-*.test.cjs
> tests/radicacion-simple-control-contract.test.cjs
> Doc/Actualizacion/RadicacionSimplificada/Adjunta/DOC-85-correccion-contexto-radicado-adjunto/README.mdRevisar también el estado y diff actuales del repositorio. Preservar cambios del usuario que no pertenezcan a esta corrección.
> PRUEBAS OBLIGATORIAS
> Caracterización del defecto
> Reproducir mediante prueba que Radicación clásica configura ADJUNTARADICACION sin los campos exclusivos de Simplificada.
> 
> Demostrar que el handler actual convierte ese caso en una llamada de doce argumentos con ID cero.
> 
> Demostrar que el rechazo ocurre antes del almacenamiento.
> 
> Contratos por origen
> Radicación clásica selecciona exclusivamente su ruta.
> 
> Radicación Simplificada selecciona exclusivamente la ruta DOC-85.
> 
> Un origen desconocido se rechaza de forma cerrada.
> 
> Un ID cero de Simplificada continúa siendo rechazado.
> 
> El handler no infiere el origen solamente por la ausencia de parámetros.
> 
> La respuesta conserva todos los campos actuales de uploadFiles.
> 
> Radicación clásica
> Contexto válido: almacena un documento una sola vez.
> 
> Estado de sesión sin registro: rechaza antes del almacenamiento.
> 
> Registro inexistente o incompatible: rechaza.
> 
> Tipología obligatoria ausente: rechaza con el mensaje funcional correspondiente.
> 
> La fila se proyecta en GridView_list_documento_relacion sin recarga completa.
> 
> Radicación Simplificada / DOC-85
> Conserva el escenario positivo con DAT_ADIC_TAR vacío.
> 
> Conserva el rechazo por ID cero.
> 
> Conserva el rechazo por registro no autorizado.
> 
> Conserva el rechazo por discrepancia del radicado informativo.
> 
> No llama SolicitaRadicadoTareaWorkflow(...) para redescubrir el radicado.
> 
> Resuelve el contexto autoritativo una sola vez.
> 
> Regresión compartida
> Ejecutar todas las pruebas radicacion-simple-attachment-*.test.cjs.
> 
> Extender el inventario para incluir consumidores JavaScript, no solo llamadas VB a UploadSaveFile(...).
> 
> Ejecutar bootstrap-table-global-contract.test.cjs y las suites relacionadas con el cargador compartido.
> 
> Compilar la solución o el proyecto completo con MSBuild.
> 
> Ejecutar git diff --check y revisar manualmente el diff final.
> 
> Las pruebas locales deben ser deterministas y no depender de red, autenticación real o escritura en bases de datos externas.
> MATRIZ MÍNIMA DE NO REGRESIÓN
> Origen/capacidad
> Identidad esperada
> Resultado
> Radicación clásica 
> Contexto autorizado del registro seleccionado en la sesión del módulo 
> Adjunta y proyecta una sola vez. 
> Radicación Simplificada 
> ID explícito + resolución autoritativa DOC-85 
> Adjunta sin depender de DAT_ADIC_TAR. 
> Simplificada con ID cero 
> Ninguna 
> Rechazo antes del almacenamiento. 
> Simplificada con discrepancia 
> Registro autoritativo 
> Rechazo antes del almacenamiento. 
> Gestión de respuestas 
> Contrato existente 
> Sin cambios. 
> Workflow seleccionado/enlace 
> Contrato existente 
> Sin cambios. 
> Producción documental 
> Expediente seleccionado 
> Sin cambios. 
> Consulta/Enlace de radicado 
> Contrato existente 
> Sin cambios. 
> ENLASE, SII y sellos 
> Adaptadores existentes 
> Sin cambios. 
> E2E REAL
> Integrar o extender la infraestructura existente bajo tools/e2e; no crear otro login, proyecto Playwright, archivo de secretos o almacenamiento paralelo de sesión.
> La validación autorizada debe cubrir al menos:
> Radicación clásica: seleccionar un radicado descartable, abrir el cargador real, adjuntar un fixture permitido, escoger tipología si aplica y verificar una única persistencia y una única fila visible.
> 
> Radicación Simplificada positiva: confirmar que DOC-85 continúa funcionando con su contexto autoritativo.
> 
> Radicación Simplificada negativa: alterar únicamente el radicado informativo y comprobar rechazo sin persistencia.
> 
> Las verificaciones de datos deben ser SELECT; no ejecutar limpieza mutante. La carga real requiere autorización explícita y recurso descartable.
> RESTRICCIONES CRÍTICAS
> No resolver el defecto eliminando la validación IdRegistroEstadoRadicacion <= 0 de Simplificada.
> 
> No permitir que Simplificada use silenciosamente RA_ID_REGISTRO_RADICADO cuando el contrato exige ID explícito.
> 
> No decidir el origen únicamente porque un campo venga vacío.
> 
> No usar el radicado visible del navegador como autorización.
> 
> No duplicar el motor de almacenamiento.
> 
> No cambiar globalmente SolicitaRadicadoTareaWorkflow(...).
> 
> No modificar tablas ni crear migraciones para esta corrección.
> 
> No introducir postback, DataBind, recarga parcial o recarga completa como mecanismo de actualización.
> 
> No modificar rutas no relacionadas para hacer pasar pruebas estructurales.
> 
> No reducir las garantías documentadas de DOC-85.
> 
> TAREAS ATÓMICAS SUGERIDAS
> Caracterizar en prueba los dos consumidores JavaScript de ADJUNTARADICACION.
> 
> Caracterizar la selección incorrecta de la sobrecarga de doce argumentos para la pantalla clásica.
> 
> Definir constantes o un discriminador explícito para ambos orígenes.
> 
> Separar el enrutamiento del handler sin modificar otros eventos.
> 
> Restaurar la ruta clásica con validación de sesión y registro.
> 
> Mantener la ruta DOC-85 sin relajaciones.
> 
> Ajustar los JavaScript estrictamente necesarios y versionar sus referencias ASPX.
> 
> Ampliar las pruebas para inventariar consumidores frontend y backend.
> 
> Ejecutar suites focales, regresión compartida y compilación.
> 
> Documentar archivos cambiados, decisiones, resultados y limitaciones.
> 
> Ejecutar E2E real solo después de autorización explícita.
> 
> CRITERIOS DE ACEPTACIÓN
> Radicación clásica adjunta documentos sin recibir el mensaje El registro seleccionado no es válido para adjuntar el documento. cuando su contexto es válido.
> 
> Radicación clásica no entra en ServicioAdjuntoRadicacion como si fuera Simplificada salvo que se diseñe y demuestre explícitamente un servicio común compatible con ambos contratos.
> 
> Radicación Simplificada sigue rechazando ID cero, registro ajeno, contexto incompleto y discrepancia informativa.
> 
> DAT_ADIC_TAR vacío no vuelve a bloquear el caso positivo DOC-85.
> 
> El origen se discrimina explícitamente y el servidor lo valida.
> 
> Cada carga produce como máximo una persistencia y una proyección visual.
> 
> El contrato uploadFiles permanece compatible.
> 
> Los otros eventos del handler conservan sus recorridos.
> 
> Las pruebas incluyen js/radicacion/WebFormRadicacionEntrante.js; no se limitan al inventario de llamadas VB.
> 
> La compilación termina sin errores nuevos.
> 
> La evidencia E2E solo se declara aprobada si fue ejecutada con autorización real.
> 
> DOCUMENTACIÓN TÉCNICA
> Crear un único paquete documental para el ticket aprobado en:
> Doc/Actualizacion/Radicacion/Adjuntar/<DOC-ID>-correccion-adjunto-radicacion-clasica/Documentar:
> diagnóstico y causa raíz;
> 
> comparación antes/después de DOC-85;
> 
> inventario completo de consumidores;
> 
> decisión para discriminar los dos orígenes;
> 
> flujo anterior y corregido;
> 
> controles de autorización por ruta;
> 
> matriz de no regresión;
> 
> archivos modificados;
> 
> pruebas y compilación;
> 
> resultado E2E saneado o bloqueo por falta de autorización;
> 
> estrategia de reversa.
> 
> No reescribir ni alterar el paquete histórico de DOC-85 para ocultar la regresión.
> ENTREGABLE FINAL
> El resumen de implementación debe indicar expresamente:
> Cómo se distinguió Radicación clásica de Radicación Simplificada.
> 
> Cómo se restauró la carga clásica sin confiar en datos manipulables del navegador.
> 
> Qué garantías de DOC-85 permanecieron intactas.
> 
> Qué pruebas cubren cada origen y los demás consumidores del handler.
> 
> Qué compilación y suites fueron ejecutadas.
> 
> Qué E2E real se ejecutó o qué autorización faltó.
> 
> CRITERIO DE CIERRE
> No cerrar el cambio únicamente porque Radicación clásica permita guardar el archivo. El cierre exige demostrar simultáneamente que:
> la ruta clásica almacena bajo el radicado, tarea, gabinete y tipología correctos;
> 
> Radicación Simplificada conserva todas las validaciones DOC-85;
> 
> los dos orígenes ya no comparten una entrada contractual ambigua;
> 
> no existe doble persistencia ni proyección duplicada;
> 
> los demás consumidores del cargador compartido no cambiaron de comportamiento.
> 
> GOBIERNO COMPLEMENTARIO DE VALIDACIÓN Y DOCUMENTACIÓN
> Antes de crear documentación, ubicar documentación existente sobre el cargador compartido y ambos flujos de radicación, y actualizarla cuando corresponda. Si no existe, registrar expresamente la ausencia y la ruta donde debe quedar el nuevo paquete documental.
> 
> Antes de autenticar, leer AGENTS.md y tools/e2e/AGENT-RUNBOOK.md. Ejecutar E2E o carga real únicamente con autorización explícita del ambiente, las cuentas y los datos o tareas descartables.
> 
> Usar secretos efímeros y no exponer, imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión. Las verificaciones de control deben ser solo SELECT parametrizados y se debe conservar evidencia saneada.
> 
> La E2E autorizada debe cubrir, cuando aplique al alcance, autorización y control de acceso, lectura sin mutación, escrituras autorizadas sobre recursos descartables, concurrencia y regresión de ambos orígenes del cargador.
> 
> Respetar los feature flags, gates y controles de usuarios o grupos existentes; no habilitarlos ni reintroducirlos arbitrariamente. En particular, no reintroducir WorkflowCentroTrabajoModernActive ni listas de usuarios o grupos como mecanismo de activación.
> 
> No cerrar sin validación autorizada. Si no existe autorización o configuración segura para la validación real, registrar un bloqueo explícito. No sustituir la evidencia faltante con mocks, simulaciones, resultados inventados ni evidencia ficticia.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: ACTUALIZACION, ADJUNTAR, DOCUMENTO, RADICACION

## Capabilities

### New Capabilities
- `actualizar-adjuntar-documento-radicacion`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

