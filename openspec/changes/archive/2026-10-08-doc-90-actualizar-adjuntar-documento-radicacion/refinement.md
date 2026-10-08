<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-90-actualizar-adjuntar-documento-radicacion

## Fuente y alcance

- Ticket: `DOC-90` — ACTUALIZAR-ADJUNTAR-DOCUMENTO-RADICACION.
- Superficie funcional corregida: Radicación Entrante clásica (`radicador/WebFormRadicacionEntrante.aspx`).
- Perfil tecnológico: ASP.NET Web Forms, VB.NET, .NET Framework 4.6.1, JavaScript legacy y MySQL.
- Frontera protegida: Radicación Simplificada y su contrato autoritativo DOC-85 son no regresión, no superficies de implementación.

## Contexto inspeccionado

- `js/radicacion/WebFormRadicacionEntrante.js`, `ActivaAdjuntarDocumentoRadicacion`, usa `ADJUNTARADICACION` sin los campos exclusivos de DOC-85.
- `generic_control/FileUploadHandler_.ashx.vb` dirige actualmente ese evento a la sobrecarga de doce argumentos y por ello entrega ID cero desde Radicación Entrante.
- `workflow/ClassAlmacenamiento.vb`, sobrecarga de doce argumentos, rechaza correctamente el ID cero y pertenece exclusivamente al contrato de Radicación Simplificada.
- El diff `57d43b4f^..57d43b4f` cambió la llamada compartida de diez a doce argumentos.
- El diff `f7171925^..f7171925` separó la sobrecarga DOC-85 y retiró de la sobrecarga de diez argumentos la rama que resolvía `RA_ID_REGISTRO_RADICADO` desde la sesión clásica.
- `radicador/ClassRadicador.vb` y `radicador/Class_ra_rad_estados_modulo_radicacion.vb` establecen el registro clásico seleccionado en `RA_ID_REGISTRO_RADICADO`.
- `generic_control/FileUploadHandler.js` ya proyecta `insert_row_documento_relacionado` sin depender del nombre del evento, por lo que no requiere modificación para la corrección clásica.
- El handler compartido atiende actualmente `GESTION_PQRS`, `ADJUNTAVERSION`, `REMPLAZAVERSION`, `INTRUESII`, `INTVIRTUALSII`, `MIGRACION`, `GESTION_RESPUESTA`, `WORKFLOWSELECCION`, `WORKFLOWENLACE`, `ADJUNTARADICACION`, `PRODUCCION`, `SUBE_RESPUESTA`, `SUBE_ANEXO` y `RADICA_WORKFLOW`; fuera de la nueva rama clásica, sus bloques deben permanecer equivalentes.
- Las llamadas a `UploadSaveFile` también existen en `workflow/Webworkflow.aspx.vb` y `webservice/WebServiceRadicacion.asmx.vb`; sus firmas y recorridos quedan fuera del cambio funcional.
- `inicio_tab_radicador`, `nuevo_radicado_tab` y `terminar_radicado_tab` usaban `Hidden_numero_rad_pend` para habilitar Soporte sin una asignación vigente; el contador de pendientes no es identidad de una tarea asignada.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Asignar `ADJUNTARADICACION_CLASICA` únicamente a `evento_adjunta` de Radicación Entrante, conservando `NameLoadProceso` para reutilizar su configuración documental. | `ActivaAdjuntarDocumentoRadicacion` separa configuración y evento dentro de `_OPtionFileLoad`. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Agregar al handler una rama clásica explícita que invoque la sobrecarga de diez argumentos y conserve la respuesta `uploadFiles`. | Antes de `57d43b4f`, la rama clásica utilizaba diez argumentos y el mismo mapeo de respuesta. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Restaurar en la sobrecarga de diez argumentos la ruta clásica basada en el módulo, la plantilla y `RA_ID_REGISTRO_RADICADO` de sesión, con rechazo previo y propagación de la tarea resuelta. | La rama retirada en `f7171925` resolvía `stru_registro_estado`; el contrato del ticket exige además validar el módulo clásico y la compatibilidad de la plantilla. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Mantener sin cambios el consumidor Simplificada, `FileUploadHandler.js`, la sobrecarga de doce argumentos, `ServicioAdjuntoRadicacion` y sus repositorios. | La ruta DOC-85 funciona con `ADJUNTARADICACION`, ID explícito y contexto autoritativo. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Versionar el JavaScript clásico y demostrar la corrección con contratos focales, regresión DOC-85, compilación y E2E autorizada. | `WebFormRadicacionEntrante.aspx` referencia hoy el script clásico sin versión. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Congelar el comportamiento de todos los demás eventos y consumidores del cargador mediante un inventario automatizado de ramas, firmas, sesión y respuesta. | El handler contiene catorce eventos existentes y hay ocho llamadas distribuidas entre handler, Workflow y WebServiceRadicacion. | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Mantener Recepción activa y Soporte desactivado sin radicado restaurado; habilitar Soporte tras el resultado `YES` de la asignación y sincronizar radicado/pendientes dentro de sus `UpdatePanel`. | `CheckStatus` valida el resultado servidor; `UpdatePanel_boton_tool` ya se actualiza explícitamente y el contador requiere un panel visible independiente. | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Radicación Entrante declara un origen clásico inequívoco sin enviar identidad autoritativa desde el navegador. | WHEN abre el cargador THEN mantiene `NameLoadProceso = ADJUNTARADICACION` y usa `evento_adjunta = ADJUNTARADICACION_CLASICA`. | El servidor no infiere el origen por campos ausentes. |
| RQ-02 | El handler selecciona la sobrecarga clásica y devuelve el contrato vigente. | WHEN recibe el evento clásico THEN invoca una sola vez la firma de diez argumentos y completa los campos existentes de `uploadFiles`. | Las demás ramas del handler conservan su comportamiento. |
| RQ-03 | La ruta clásica usa módulo, plantilla y registro autorizados de sesión y falla antes de almacenar si el contexto no es válido. | WHEN la sesión corresponde a Radicación Entrante y registro/plantilla coinciden THEN resuelve tarea y radicado; WHEN falta, difiere o es inválido THEN no llama el prealmacenamiento. | No se acepta un ID editable del navegador ni se crea fallback para Simplificada. |
| RQ-04 | DOC-85 permanece funcional y sin cambios de implementación. | WHEN se ejecutan sus contratos THEN conserva ID positivo, resolución única, comparación informativa y ausencia de fallback a sesión o `DAT_ADIC_TAR`. | Los archivos protegidos de Simplificada mantienen su contenido salvo el handler compartido estrictamente necesario. |
| RQ-05 | La UI clásica evita caché obsoleta y la evidencia demuestra persistencia/proyección únicas. | WHEN la carga clásica es válida THEN muestra una sola fila sin postback; suites, compilación y diff pasan; la E2E real solo se acredita con autorización. | Sin autorización se registra bloqueo y no se inventa evidencia. |
| RQ-06 | Los módulos ajenos conservan sus eventos, firmas y recorridos. | WHEN se compara el inventario antes/después THEN solo aparece la rama aditiva `ADJUNTARADICACION_CLASICA`; las ramas y llamadas existentes permanecen equivalentes. | Evita regresiones en PQRS, versiones, SII, migración, respuestas, Workflow, Producción, anexos y enlace de radicados. |
| RQ-07 | Sin radicado asignado, Recepción permanece activa y las acciones de Soporte son inaccesibles aunque existan pendientes; el selector de pendientes sigue visible y sincronizado. | WHEN inicia, crea un radicado nuevo o termina la tarea sin asignación THEN desactiva Soporte y conserva `A1`; WHEN el servidor confirma la asignación THEN habilita Soporte; radicado y contador viajan en los paneles parciales correspondientes. | Evita adjuntos sin contexto, permite seleccionar la tarea y elimina lecturas de DOM obsoletas sin modificar Radicación Simplificada. |

## Resultado del refinamiento

- Estado: aprobado con Radicación Entrante como única superficie funcional de cambio.
- Radicación Simplificada queda limitada a validación de no regresión.
- Los demás módulos del cargador compartido quedan cubiertos por una matriz e inventario de no regresión.
- El contador de pendientes queda separado del estado de asignación activa y no habilita acciones documentales.
- Siguiente control: `npm.cmd --prefix Tools/opsxj run opsxj:refine -- DOC-90 --sync`.
