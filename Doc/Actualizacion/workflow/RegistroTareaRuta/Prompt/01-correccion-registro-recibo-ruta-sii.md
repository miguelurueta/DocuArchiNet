# Prompt 01 — Corrección integral del registro de recibo SII en una ruta Workflow

## Rol esperado

Actúa como arquitecto y desarrollador senior especialista en ASP.NET Web Forms, VB.NET, .NET Framework, JavaScript legacy, servicios ASMX y MySQL. Corrige integralmente la operación **Registrar tarea para ruta** de `WebFormGestionFlujoTrabajoCamaras.aspx`, preservando los demás formularios y consumidores compartidos de Workflow.

No reduzcas el cambio a ocultar el error JavaScript visible. La entrega debe restablecer el contrato del formulario, mantener autoritativo el recibo realmente consultado, validar la correspondencia de los datos derivados, cerrar las entradas inseguras del servicio y demostrar que el registro es consistente y único.

## Objetivo

Permitir que un usuario autorizado consulte un recibo SII y registre una tarea en una ruta con los datos exactos de esa consulta, sin errores JavaScript, selecciones implícitas, mezcla de estados anteriores, SQL construido con valores concatenados ni persistencia parcial.

La operación corregida debe garantizar:

- formato canónico y validado del recibo;
- correspondencia entre recibo, radicado, matrícula, razón social y trámite consultados;
- selección explícita y válida de actividad;
- autorización server-side para `UTIL_SII_REGISTRO_TAREA_RUTA`;
- consultas y escrituras parametrizadas;
- transacción local Workflow para tarea + outbox y relación Docuarchi idempotente;
- respuesta pública saneada y confirmación inequívoca al usuario;
- compatibilidad demostrada con los demás formularios de la página.

## Diagnóstico confirmado

### 1. Ruptura actual del contrato HTML

`Button_registro_actividad_ruta` ejecuta:

```txt
event_element_click_promise
  -> valida_solicita_datos_control_general_async("conten_registro_ruta")
  -> segunda iteración del validador
  -> attributes["atrib_campo_beetwen"].value
  -> Cannot read properties of undefined (reading 'value')
```

Los seis controles de `conten_registro_ruta` no declaran `atrib_campo_beetwen`, aunque el validador compartido lo lee obligatoriamente. El servicio de registro no llega a invocarse.

### 2. Actividad vacía aceptada como válida

`Solicita_class_actividades_workflow_ruta(...)` agrega una opción vacía con valor `0`. El validador general solo rechaza una cadena vacía, por lo que `0` atraviesa la validación y falla más tarde al buscar la sede.

### 3. Mezcla posible de dos recibos

Después de consultar un recibo, el campo continúa editable. Si el usuario cambia el número o el prefijo y pulsa **Aceptar** sin consultar nuevamente, el cliente puede enviar el recibo nuevo junto con radicado, matrícula, razón social y trámite obtenidos del recibo anterior. El servidor confía en esos valores y no recompone el contexto desde una fuente autoritativa.

### 4. Normalización defectuosa

`zeroFillFReciboSII(...)` acepta el prefijo en cualquier posición, no valida exclusivamente dígitos, no normaliza espacios o mayúsculas y conserva `"-"` como relleno para largos no contemplados. Puede producir valores como `S-123456789` o mezclar prefijos.

No asumir un formato por memoria. Caracterizar primero el contrato vigente de SII y los recibos válidos usados por el repositorio. La implementación debe expresar ese contrato mediante pruebas deterministas.

### 5. Trámite seleccionado silenciosamente

La carga compara el nombre retornado en `id_value` con `subtipotramite`. Si no encuentra coincidencia, no falla: el navegador conserva el primer trámite de la lista y permite registrarlo como si fuera el correspondiente al recibo.

### 6. Promesas que pueden quedar pendientes

El cliente desreferencia directamente `data.d[0]`, `Class_parram_consultarRadicado` y las listas de opciones. Una respuesta vacía, nula o incompleta puede lanzar una excepción dentro del callback `success` y dejar la operación sin resolver, con progreso visible o controles bloqueados.

### 7. Datos alterados y SQL concatenado

La razón social se modifica con `replace("'", "")` y `replace("/", "")`, eliminando solo la primera coincidencia y alterando nombres válidos. En servidor, recibo, código de barras, razón social, matrícula y otros valores se concatenan en `SELECT` e `INSERT`.

### 8. Persistencia fuera de una única transacción

`RegistraValidaRelacionExpedienteRadicadoExterno(...)` puede insertar la relación expediente–radicado antes de abrir la transacción utilizada por `Registra_flujo_workflow_externo(...)`. Si el registro del Workflow falla, la relación previa no participa en el rollback.

### 9. Autorización solo visual

La interfaz consulta permisos para mostrar la pestaña, pero `Service_registro_tarea_ruta_sii(...)` no demuestra dentro del método la sesión válida y el permiso `util_sii_registro_tarea_ruta`. Ocultar un control no sustituye la autorización server-side.

### 10. Cierre visual ambiguo y caché

En éxito se limpian los controles, pero no existe una confirmación inequívoca. Además, los dos JavaScript relevantes se cargan sin una versión de recurso, por lo que una corrección puede no llegar al navegador por caché.

## Rutas canónicas que deben revisarse

```txt
workflow/WebFormGestionFlujoTrabajoCamaras.aspx
js/workflow/WebFormGestionFlujoTrabajoCamaras.js
js/java_general/general_control_java.js
webservice/WebServiceWorkflow.asmx.vb
workflow/ClassGestionTareasFlujoTrabajo.vb
workflow/Class_Listado_Actividades_workflow.vb
workflow/Class_permisos_usuarios_workflow.vb
Gestion/ClassRaRelacionRadicadoExternoExpediente.vb
Integracionccv/Class_ws_tipotramitesii_determina_gabinete.vb
GestionDocumental-Docuarchi.net.vbproj
tests/
tools/e2e/
```

Antes de implementar, localizar todos los consumidores de las funciones que se pretendan modificar y revisar el diff completo del repositorio. No asumir que una función con nombre parecido es exclusiva de esta pantalla.

## Decisiones arquitectónicas obligatorias

### Frontera 1 — Formulario local

La corrección inmediata del contrato HTML debe ser local a `conten_registro_ruta`:

- agregar explícitamente `atrib_campo_beetwen="0"` a cada `INPUT`, `SELECT` o `TEXTAREA` que pertenezca a ese espacio y sea recorrido por el validador;
- no relajar globalmente `valida_solicita_datos_control_general_async(...)` como atajo;
- no realizar reemplazos masivos sobre otros espacios de nombres;
- proteger mediante prueba estructural el conjunto exacto de atributos requerido por los seis controles.

Si se descubre que el contrato compartido realmente define `atrib_campo_beetwen` como opcional, esa normalización debe proponerse y probarse como un cambio transversal separado; no incorporarla silenciosamente a esta corrección.

### Frontera 2 — Contexto autoritativo en cliente

Mantener un estado explícito de la última consulta válida de ruta, como mínimo con:

```txt
reciboCanonico
codigoBarras
matricula
razonSocial
idTramite
subtipoTramite
consultaVigente
```

No confiar en controles deshabilitados como fuente de autoridad. Los controles representan la interfaz; el contexto confirmado representa la consulta.

Cuando cambien el valor del recibo o su prefijo:

1. invalidar inmediatamente la consulta vigente;
2. limpiar radicado, matrícula, razón social, trámite y actividad;
3. impedir el registro hasta una nueva consulta exitosa.

Antes de enviar, comparar el recibo visible canonizado con el recibo confirmado. Una discrepancia debe fallar cerrada sin invocar el servicio.

### Frontera 3 — Contrato de consulta

La consulta solo se considera exitosa cuando:

- `data.d` contiene exactamente un resultado utilizable;
- `Error_gestion` es `YES`;
- existe `Class_parram_consultarRadicado`;
- radicado, recibo y demás campos obligatorios cumplen el contrato;
- existe una coincidencia inequívoca entre el subtipo SII y el catálogo de trámites;
- existen actividades válidas para selección.

No seleccionar silenciosamente el primer trámite. Si no existe una coincidencia única, limpiar el formulario derivado, invalidar el contexto y mostrar un error funcional saneado.

Toda rama del adaptador AJAX debe resolver o rechazar de forma controlada. Utilizar `try/catch` dentro de los callbacks cuando corresponda y garantizar la restauración de progreso y controles mediante el `finally` superior.

### Frontera 4 — Comando server-side autoritativo

`Service_registro_tarea_ruta_sii(...)` debe dejar de aceptar como verdad un arreglo genérico construido libremente por el navegador. Adaptar el contrato existente de manera compatible hacia un comando tipado y validado para esta capacidad.

El servidor debe:

1. comprobar sesión Workflow válida;
2. resolver el usuario desde sesión, nunca desde el cliente;
3. comprobar `util_sii_registro_tarea_ruta = 1` reutilizando la infraestructura de permisos existente;
4. validar formato y longitud del recibo;
5. rechazar identificadores de trámite o actividad nulos, `0`, negativos o inexistentes;
6. reconstruir o revalidar la correspondencia del recibo con los datos consultados por un mecanismo autoritativo existente;
7. validar que trámite y actividad pertenecen al catálogo permitido;
8. devolver códigos funcionales saneados sin SQL, rutas, conexión ni traza interna.

Si conservar temporalmente `Class_config_general_service` es obligatorio para compatibilidad, limitarlo al adaptador ASMX. No propagar esa representación genérica como modelo de dominio nuevo.

### Frontera 5 — Persistencia parametrizada con outbox entre bases

Parametrizar todos los valores en:

- verificación de existencia del recibo;
- relación expediente–radicado externo;
- registro público;
- datos adicionales de la ruta;
- inicio y estado de tarea;
- log aplicable.

Los identificadores dinámicos de tabla no pueden tratarse como parámetros SQL. Deben resolverse exclusivamente desde configuración confiable del servidor, validarse contra una lista o patrón cerrado y nunca proceder del navegador.

La exploración oficial confirmó que Workflow usa `Dbase_Conction_Mysql` y Docuarchi usa `Dbase_Conction_Mysql_DA`. Por decisión aprobada, no usar transacción distribuida ni compensación destructiva. Confirmar tarea y evento en `workflow_registro_ruta_sii_outbox` dentro de una sola transacción Workflow; después materializar la relación en Docuarchi bajo bloqueo nombrado e idempotencia por recibo. Una caída remota debe conservar `RETRYABLE` y responder `REGISTERED_RELATION_PENDING`. Un reintento del mismo recibo procesa el evento existente sin crear otra tarea.

El esquema outbox requiere scripts versionados apply/preflight/rollback. El preflight debe ser solo `SELECT`; el rollback debe negarse si existen eventos.

La verificación de duplicado debe quedar respaldada por una garantía de base de datos o por un mecanismo equivalente resistente a concurrencia. Un `SELECT` previo seguido de `INSERT` no es suficiente por sí solo. No crear índices en un ambiente real sin script versionado, reversión y aprobación.

## Flujo corregido obligatorio

```txt
[Usuario autorizado]
        |
        v
[Escribe prefijo + número]
        |
        v
[Cliente canoniza y valida]
        |
        +-- inválido --> [mensaje + cero llamadas]
        |
        v
[ASMX consulta recibo SII]
        |
        +-- respuesta incompleta/error --> [limpia derivados + invalida contexto]
        |
        v
[Coincidencia única de trámite]
        |
        +-- no existe/ambigua --> [bloquea registro]
        |
        v
[Guarda contexto confirmado y presenta datos]
        |
        v
[Usuario selecciona actividad válida > 0]
        |
        v
[Aceptar: compara UI contra contexto confirmado]
        |
        +-- recibo cambió --> [invalida + exige consultar]
        |
        v
[ASMX valida sesión + permiso + comando]
        |
        v
[Servidor revalida contexto y catálogos]
        |
        v
[Transacción Workflow]
        +--> registro público
        +--> inicio de tarea
        +--> datos adicionales
        +--> estado de tarea
        +--> log aplicable
        +--> evento outbox
        |
        +-- cualquier fallo --> [rollback total]
        |
        v
[Commit Workflow]
        |
        v
[Despacho idempotente Docuarchi]
        +--> relación compatible/no aplica --> [confirma evento]
        +--> caída remota --> [RETRYABLE + pendiente visible]
        +--> relación incompatible --> [REVIEW_REQUIRED, sin sobrescribir]
        |
        v
[Respuesta saneada de éxito o pendiente]
        |
        v
[Confirmación visible + limpieza controlada]
```

## Implementación requerida

1. Caracterizar el formato real de recibo SII y escribir primero pruebas de canonización válidas e inválidas.
2. Completar exclusivamente el contrato de los seis controles `conten_registro_ruta`.
3. Crear una normalización pura y determinista para el recibo de ruta; no seguir agregando casos al `switch` defectuoso.
4. Agregar invalidación del contexto ante cambios en recibo o prefijo.
5. Mantener separado el contexto consultado de los valores visuales.
6. Validar actividad explícita y rechazar `0` o `-1` antes del validador general y también en servidor.
7. Exigir coincidencia única de trámite; eliminar el fallback implícito al primer elemento.
8. Fortalecer el adaptador AJAX contra respuestas vacías, nulas, múltiples o incompletas.
9. Incorporar confirmación visible de éxito sin doble envío.
10. Versionar `WebFormGestionFlujoTrabajoCamaras.js` y cualquier otro recurso JavaScript modificado con un identificador nuevo de caché.
11. Agregar validación de sesión y permiso dentro del servicio mutante.
12. Introducir un comando tipado o adaptador tipado en la frontera del servicio sin romper consumidores existentes.
13. Parametrizar las consultas y escrituras alcanzadas por esta operación.
14. Implementar la migración outbox aprobada, transacción local Workflow y despacho idempotente Docuarchi sin modificar APIs legacy compartidas.
15. Proteger duplicados frente a concurrencia conforme al esquema real.
16. Agregar pruebas focales, de integración y E2E real autorizada.
17. Registrar todos los archivos nuevos en el proyecto VB.NET cuando aplique.

## Restricciones críticas y antirregresión

- No modificar globalmente `general_control_java.js` para hacer desaparecer el error de este formulario.
- No cambiar los espacios `conten_registro_flujo`, `conten_registro_flujo_tarea_sii` ni sus contratos salvo que exista un ticket y pruebas independientes.
- No modificar importación ENLASE, importación de sellos, Radicación Simplificada, adjuntos ni `FileUploadHandler_.ashx`.
- No agregar recarga completa, postback, `DataBind`, `window.location.reload()` ni temporizadores para ocultar estados.
- No aceptar campos deshabilitados, variables globales o parámetros del navegador como identidad autoritativa.
- No corregir SQL mediante eliminación de comillas, barras u otros caracteres en JavaScript.
- No concatenar valores en SQL.
- No registrar una relación expediente–radicado fuera del resultado atómico y después reportar la tarea como fallida.
- No cambiar el esquema de base de datos fuera de scripts versionados con preflight y rollback.
- No registrar secretos, cookies, cadenas de conexión, respuestas SII completas ni datos personales en evidencia o logs.
- No presentar un formulario limpio como única evidencia de éxito.
- No ejecutar E2E autenticada ni escritura real sin autorización explícita para ambiente, cuenta y datos descartables.

## Pruebas locales obligatorias

### Contrato del formulario

- Los seis controles de `conten_registro_ruta` poseen todos los atributos requeridos, incluido `atrib_campo_beetwen="0"`.
- El conjunto de controles y nombres de campo serializados es exactamente: `recibo`, `codigo_barras`, `matricula`, `rscocial`, `id_tramite` e `id_actividad`.
- La corrección no modifica atributos ni comportamiento de los otros espacios del formulario.

### Canonización

- Casos válidos mínimos, máximos y ya canonizados según el contrato SII confirmado.
- Espacios laterales y normalización de mayúsculas si el contrato lo permite.
- Rechazo de letra interna, prefijo contradictorio, caracteres no numéricos, largo inválido, guion espurio y valor vacío.
- Idempotencia: canonizar dos veces devuelve exactamente el mismo resultado.

### Estado autoritativo

- Cambiar número o prefijo después de consultar invalida y limpia todos los datos derivados.
- Aceptar sin consulta vigente no invoca el servicio de registro.
- Una discrepancia entre recibo visible y confirmado falla cerrada.
- Una segunda consulta reemplaza completamente el contexto anterior sin mezclar campos.

### Catálogos y respuesta

- Actividad `""`, `0` y `-1` son rechazadas.
- Subtipo sin trámite coincidente bloquea la operación.
- Más de una coincidencia también bloquea la operación.
- `data.d` vacío, elemento nulo, objeto incompleto y listas nulas producen error controlado y restauran la interfaz.
- Error HTTP, parser error, timeout y abort resuelven la operación con mensaje saneado.

### Backend

- Sesión ausente o usuario sin `UTIL_SII_REGISTRO_TAREA_RUTA` no puede registrar.
- El usuario del comando procede de sesión.
- Recibo, trámite y actividad inválidos son rechazados antes de escribir.
- Datos manipulados o inconsistentes con la consulta autoritativa no se persisten.
- Todos los valores se transmiten como parámetros MySQL.
- Un fallo en cualquier escritura revierte también la relación expediente–radicado.
- Dos solicitudes concurrentes para el mismo recibo producen como máximo una tarea y una respuesta funcional de duplicado.
- Los errores públicos no exponen SQL, rutas, conexiones o trazas.

### Regresión

- Registrar tarea para flujo conserva su contrato actual.
- Registrar tareas RUE y virtuales conserva su contrato actual.
- El servicio de consulta del recibo sigue cargando los datos esperados.
- `bootstrap-table-global-contract.test.cjs` continúa aprobando.
- Compilación completa de `GestionDocumental-Docuarchi.net.vbproj` con MSBuild.
- `git diff --check` y revisión manual de cambios fuera de alcance.

No crear una prueba que solo copie el HTML o simule el resultado final. Las pruebas deben ejecutar las funciones reales que canonizan, invalidan, validan y construyen el comando.

## E2E real obligatoria para el cierre

Integrar la prueba en la infraestructura existente de `tools/e2e`; no crear otro proyecto Playwright, login, `.env`, almacenamiento de cookies ni arnés paralelo.

Antes de diseñarla o ejecutarla, leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`. La ejecución requiere autorización explícita para:

- ambiente de pruebas;
- cuenta con permiso de registro para ruta;
- recibos SII positivos y negativos autorizados;
- actividad y trámite permitidos;
- creación real de una tarea descartable;
- consultas de evidencia con cuenta de solo lectura.

La E2E positiva debe:

1. autenticar mediante la sesión compartida existente;
2. abrir `WebFormContenedorPageWF.aspx` y la pestaña real **Registrar tarea para ruta**;
3. consultar un recibo descartable autorizado;
4. comprobar que los datos visibles corresponden a la respuesta real;
5. seleccionar una actividad válida;
6. pulsar **Aceptar** por la interfaz;
7. verificar una sola llamada mutante y una confirmación visible;
8. comprobar mediante `SELECT` parametrizados que existe una sola tarea y que recibo, radicado, matrícula, trámite y actividad son los esperados;
9. comprobar que no ocurrió navegación, postback o recarga total/parcial.

La E2E negativa debe demostrar, como mínimo:

- recibo modificado después de consultar: bloqueo en cliente y cero escrituras;
- actividad `0`: bloqueo y cero escrituras;
- contexto manipulado en la petición: rechazo server-side y cero escrituras;
- usuario sin permiso: rechazo server-side y cero escrituras;
- respuesta de trámite sin coincidencia, cuando exista un dato autorizado para reproducirla: bloqueo sin registro.

Las consultas de evidencia son solo `SELECT`; no utilizar la cuenta de evidencia para limpiar datos. Las credenciales y cookies deben permanecer efímeras y nunca imprimirse ni persistirse. Al finalizar cualquier corrida, incluso fallida, `WorkflowCentroTrabajoModernActive` debe permanecer en `false`, con usuarios y grupos vacíos.

Si faltan autorización, cuentas o datos descartables, registrar el bloqueo y no ejecutar. Las pruebas locales pueden aprobar, pero el cambio no puede declararse validado de extremo a extremo ni cerrado.

## Criterios de aceptación

- Desaparece el error `Cannot read properties of undefined (reading 'value')` sin relajar el validador compartido.
- Los seis controles cumplen el contrato y generan exactamente los seis campos esperados.
- No puede registrarse una actividad vacía o con identificador `0`/`-1`.
- Un recibo editado después de consultar invalida todo el contexto derivado.
- Nunca se registra un recibo con datos obtenidos de otro recibo.
- Un subtipo sin coincidencia única no selecciona el primer trámite.
- El cliente no queda bloqueado ante respuestas nulas o incompletas.
- El servicio verifica sesión y permiso aunque se invoque directamente.
- El servidor revalida el contexto y no confía en valores visuales.
- No existen valores concatenados en el SQL alcanzado por la operación.
- La relación expediente–radicado y la tarea se confirman o revierten como una unidad.
- Una carrera de duplicados no crea dos tareas para el mismo recibo.
- El usuario recibe una confirmación visible e inequívoca.
- Los JavaScript modificados llegan con una versión de caché nueva.
- Los demás formularios y módulos protegidos conservan sus pruebas sin cambios funcionales.
- La compilación y todas las suites focales aprueban.
- La E2E real positiva y negativa aprueban con evidencia saneada, o el cambio permanece formalmente bloqueado por falta de autorización.

## Ruta documental obligatoria

Crear un único paquete para el ticket real dentro de:

```txt
Doc/Actualizacion/workflow/RegistroTareaRuta/<DOC-ID>-correccion-registro-recibo-ruta-sii/
```

Sustituir `<DOC-ID>` por el identificador aprobado. Documentar:

- causa raíz y recorrido exacto anterior;
- contrato canónico del recibo confirmado;
- modelo de estado autoritativo del cliente;
- contrato de servicio y autorización;
- límites transaccionales antes y después;
- inventario de SQL parametrizado;
- matriz de archivos y consumidores;
- pruebas, comandos y resultados;
- evidencia E2E saneada o bloqueo operativo;
- estrategia de despliegue, caché y reversión.

No reescribir paquetes históricos ni duplicar la documentación en otra raíz.

## Entregable final

Entregar código, pruebas y documentación coherentes con el comportamiento real. El resumen debe separar expresamente:

1. corrección local del formulario;
2. protección del contexto autoritativo;
3. validaciones y autorización server-side;
4. parametrización y atomicidad;
5. evidencia de no regresión;
6. resultado de la E2E autorizada o bloqueo que impide cerrar.

## Contexto obligatorio antes de implementar

Leer completamente:

- `AGENTS.md`;
- este prompt;
- `tools/e2e/AGENT-RUNBOOK.md` antes de cualquier E2E autenticada;
- las rutas canónicas enumeradas;
- todos los consumidores de las funciones que se modifiquen;
- las pruebas existentes relacionadas;
- el diff actual del repositorio.

No implementar mediante sustituciones textuales globales. Delimitar cada modificación por función, espacio de nombres y capacidad, y comprobar después que ningún cambio local quedó conectado a otro formulario.

## Criterio de cierre

No declarar la corrección terminada porque el mensaje JavaScript desaparezca o porque el formulario se limpie. Debe demostrarse que el recibo consultado es el mismo que se registra, que los catálogos son válidos, que el servidor autoriza y revalida, que todas las escrituras son seguras y atómicas, que no hay duplicados y que la operación real fue verificada con evidencia autorizada.

## Pruebas obligatorias

Código, pruebas focales, E2E integrada, validación autorizada y evidencia saneada forman una única unidad de entrega de DOC-87. La E2E no puede separarse en otro ticket para declarar cerrada esta corrección.

Además de las pruebas locales detalladas anteriormente, la cobertura E2E debe demostrar, cuando el ambiente autorizado lo permita:

- autorización y rechazo de acceso sin permiso;
- consulta real sin mutación;
- escritura autorizada única;
- rechazo de estado manipulado;
- concurrencia sin tareas duplicadas;
- regresión de los formularios Workflow relacionados.

No usar mocks, simulaciones, resultados inventados ni evidencia ficticia para sustituir la validación real de cierre. Si faltan autorización, cuenta o datos descartables, registrar el bloqueo explícito y mantener el cambio abierto.

Usar secretos efímeros. No exponer, imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión. Las verificaciones de datos serán solo `SELECT` y se conservará evidencia saneada.

La cobertura E2E incluye autorización y control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada.

Respetar feature flags, gates, usuarios, grupos y controles de seguridad sin habilitarlos arbitrariamente. Después de cualquier ejecución, incluso fallida, el gate deberá quedar en `false`, con usuarios y grupos vacíos.

## Documentacion tecnica

Antes de crear documentación nueva, ubicar la documentación existente de la operación y actualizarla. Si no existe, registrar expresamente la ausencia y crear el paquete canónico en `Doc/Actualizacion/workflow/RegistroTareaRuta/DOC-87-correccion-registro-recibo-ruta-sii/`.

Actualizar también los cinco documentos exigidos por el manifiesto OPSXJ bajo `Doc/Tecnica/Opsxj/doc-87-correccion-integral-registro-recibo-sii/`, sin duplicar decisiones ni contradecir el paquete canónico.
