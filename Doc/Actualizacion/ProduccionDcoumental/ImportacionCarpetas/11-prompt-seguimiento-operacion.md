# Prompt 11 — Seguimiento, reanudación y operación

## Medición real de almacenamiento —2026-11-17

Lote50: tres archivos medidos y confirmados,7→10 documentos guardados. El promedio de las solicitudes fue79,432s; el escritor habitual tardó0,152/0,023/0,020s. El mayor costo estuvo en cargar/reconstruir metadatos:2.000 llamadas por documento y72,321% del tiempo del coordinador. [Resultados, versión y límites](Validacion/doc120-medicion-tres-20261117.md). DLL instrumentada instalada con respaldo; no se aplicó una optimización. La prueba quedó cerrada y no continúa automáticamente los990 pendientes.

## Consulta sin referencia local — corrección2026-11-17

El botón **Consultar último lote** ya no exige elegir perfil ni tener una referencia en sessionStorage. Cuando falta, busca la última importación confirmada de la cuenta/empresa en el servidor compatible; valida el acceso al destino original y abre su seguimiento. Consultar no reanuda ni ejecuta efectos. Si hay una referencia guardada, conserva su consulta directa. Puedes volver a tu selección sin perder sus archivos o perfil.

Si la cuenta no tiene importaciones, lo indica. Si el servidor todavía no admite la búsqueda, mantiene **Consultar importación** para ingresar una referencia conocida; no necesitas preparar otra importación. La búsqueda automática requiere instalar la DLL de esta corrección, con autorización separada. Cache token `doc120-ultimo-20261117-1`. [Diagnóstico, pruebas y estado de despliegue](Validacion/doc120-consulta-ultimo-20261117.md).

## Espera y reanudación general — implementación local2026-11-17

Recursos del cliente: `doc120-reanudacion-20261117-1`. Recepción y guardado esperan la respuesta del servidor sin el timeout genérico del navegador a30s. Al terminar de transmitir bytes se muestra **Esperando confirmación del servidor**; todavía no significa archivo recibido o documento guardado. Se conserva archivo/ruta, posición y último avance confirmado. Un fallo de red, respuesta inválida o error HTTP sigue obligando a consultar; esperar no impide que IIS u otra dependencia cierre su solicitud.

**Pausar importación** deja terminar la solicitud iniciada y no comienza otra. Después, **Continuar importación** revisa los pendientes del mismo lote. Cerrar/recargar detiene la continuación automática; una respuesta perdida no revierte un guardado ni habilita repetirlo.

Con la nueva DLL desplegada, consultar un lote DETENIDO sin actividad ni incertidumbre ofrece **Reanudar importación**. Ese clic autoriza levantar la detención con su revisión vigente; espera acuse, consulta de nuevo y continúa mediante la cola existente. No crea otro plan/lote, no repite recibidos, expedientes o documentos confirmados. Solo se pide vincular originales que realmente falten por recibir. Si aparece una nueva detención, actividad o incertidumbre, no se continúa automáticamente.

Una solicitud de reanudación sin respuesta permite **Consultar resultado** y una nueva decisión; no se reenvía sola. Un servidor que no anuncie esta capacidad muestra **Importación detenida** con consulta disponible. La reanudación especial de creación conserva su control anterior y no sirve para levantar una detención general.

Consultar importación, último lote, filtros y CSV conservan su función de lectura. La selección de pendientes usa IDs/estados del inventario completo, no el orden o la página visible. [Contrato](Diseno/espera-respuesta-reanudacion-lote-doc120.md), [evidencia local](Validacion/doc120-espera-reanudacion-20261117.md) y [ventana real propuesta](Validacion/doc120-reanudacion-plan-real-20261117.md).

**Estado de entrega:** código compilado y probado localmente; esta continuación no instala la nueva DLL ni recupera el lote50. Los párrafos siguientes conservan sus cortes históricos; la nueva reanudación requiere el servidor compatible. No hay script SQL nuevo en esta ampliación.

## Lote50 — condición real comprobada2026-11-17

Diagnóstico8803b6f4 de5 SELECT confirmó detención persistente,1.000 recibidos, expediente confirmado y4 documentos completados;996 tienen almacenamiento NO_INICIADO y valores efectivos registrados. No hay intentos activos/inciertos. La interfaz vigente todavía carece de reanudación general para DETENIDO: repetir Continuar/Consultar no levanta la señal. Conservar el lote50/plan90/versión45 y sus resultados; esta lectura no ejecutó una recuperación ni autorizó editar estados por SQL. [Evidencia y límites físicos/operativos](Validacion/doc120-detencion-lote50-20261117.md). La implementación de reanudación permanece por desarrollar.

## Aviso de parada por archivo — 2026-11-17, corte posterior

Recargar con Ctrl+F5 para cargar recursos `doc120-parada-20261117-1`. Una parada muestra causa comprobada, fase, archivo/ruta, posición dentro del inventario y último avance confirmado; tiempo agotado no significa guardado fallido. Consultando resultado es una lectura distinta del almacenamiento. La confirmación observada se conserva; intento activo y fallo de consulta mantienen la pausa, sin repetir el efecto. Datos para soporte conserva códigos/referencias seguros. Consultar el mismo lote antes de usar únicamente sus acciones disponibles; no crear otra importación para resolver una espera sin respuesta. [Verificación local y límites](Validacion/doc120-parada-diagnostico-20261117.md):89/89 y4 GET estáticos200, sin autenticación o recuperación nueva del lote50.

## Corrección de consulta tras revisar1.000 archivos — 2026-11-17, corte posterior

DLL actualizada con `beae0afc`: sellado guarda el snapshot canónico y lector acepta revisiones anteriores cuyas reglas tengan otro orden, manteniendo la huella y el rechazo de valores alterados. Sin SQL ni importaciones nuevas. **Continuar revisión** usa el mismo plan si la ventana conserva sesión/selección; si el reinicio de la aplicación obliga a ingresar o recargar, puede hacer falta reselección/nueva revisión por pérdida de referencias en memoria. [Pruebas y límites](Validacion/doc120-lectura-plan-sellado-20261117.md). Retorno recomendado vigente `beae0afc`, sustituye `af1ff073`; los cortes inferiores son históricos.

## Despliegue local confirmado — 2026-11-17

SQL08 aplicado manualmente por el usuario; esquema real aceptado con10 SELECT de metadatos. DLL/PDB actualizados en IIS local, canal JSON de nueva consulta rechaza correctamente el acceso anónimo; sin importar archivos. **Recargar con Ctrl+F5**. El retorno compatible requiere `af1ff073` o posterior; sustituye el mínimo `d5bae464` por la corrección del detector de índices. [Resultado, huellas y límites](Validacion/doc120-despliegue-esquema-20261117.md). Los apartados inferiores conservan su corte previo al despliegue; la aceptación autenticada de1.000 PDF continúa pendiente.

## Preparación segmentada — entrega local2026-11-17

Con la DLL/SQL08 desplegados y compatibles, el usuario puede seleccionar la carpeta completa de1.000 PDF y pulsar **Preparar revisión** una vez. El módulo divide automáticamente los metadatos por cantidad/tamaño; muestra revisados/total y permite **Pausar revisión** y **Continuar revisión**. No envía contenido PDF en esa fase ni crea un plan por parte. Las fichas por carpeta, valores e incidencias se paginan conservando totales globales.

Tras revisar, **Confirmar e importar** mantiene un lote y un expediente por raíz, con envío y efectos secuenciales. Los contadores distinguen recibidos, expedientes y documentos guardados. **Pausar** deja concluir la solicitud en curso; cerrar o recargar cancela la continuación automática. Consultar importación, último lote, filtros y CSV permanecen disponibles y no ejecutan efectos.

Para retomar un lote confirmado: consultarlo, elegir la raíz pendiente y **Vincular carpeta de originales**. El módulo informa vinculados/faltantes/modificados/ambiguos/sobrantes y omite recibidos. Solo **Continuar importación** empieza los pendientes; no vuelve a confirmar ni agrega archivos nuevos al lote. Si la selección original sigue en memoria, no obliga a reseleccionar. En un borrador sin IDs de selección recuperables después de recarga, consultar es posible, pero continuar requiere preparar otra revisión: no se asocian originales por orden o nombre solo.

[Verificación local y límites](Validacion/doc120-segmentada-20261117.md):200 casos únicos, compilaciones aisladas y lectores compatibles. SQL/DLL **sin aplicar al sitio**; capacidad real y rollback transaccional pendientes. [Ventana propuesta](Validacion/doc120-capacidad-plan-real-20261117.md). El apartado siguiente describe el aviso anterior, que sigue aplicando a servidores sin el nuevo contrato o a un elemento indivisible que exceda el límite.

## Aviso de tamaño de revisión — 2026-11-16

Si la lista seleccionada supera el límite JSON publicado por el servidor, Preparar revisión detiene la solicitud antes de enviarla. El aviso indica cantidad de PDF, caracteres observados y máximo permitido, confirma que se conserva selección/perfil y explica que hace falta ajustar la capacidad para esa cantidad. No envía los PDF ni afirma haber guardado una revisión. Ese control previo requiere desplegar la DLL actualizada del host; esta entrega solo compila en una ubicación aislada.

Un servidor anterior sin ese metadato sigue pudiendo devolver un error ASMX reconocible de tamaño o HTTP413: el aviso explica el tamaño, pero conserva incertidumbre sobre si la revisión se guardó. Una respuesta HTML/genérica o un error desconocido no se atribuye a capacidad por tener muchos archivos; se informa que no se pudo identificar la causa. Ningún Message/StackTrace se muestra ni se repite la petición automáticamente. [Evidencia y alcance](Validacion/doc120-mensaje-tamano-20261116.md).

## Recorrido vigente — DOC-120, 2026-11-15

Claridad tras video121039: junto a Agregar carpeta se explica que el permiso «Subir» pertenece al navegador y permite leer la selección; el contenido PDF se envía al confirmar la importación. No se agrega otro diálogo. Revisar presenta cantidades una sola vez y fichas por carpeta con código y nombre informativo completos. La referencia de revisión sigue disponible en Detalles de configuración; incidencias y datos previstos conservan sus controles. Ver [corte de presentación](Validacion/doc120-revision-claridad-20261115.md).

1. **Seleccionar**: elegir perfil publicado y agregar carpetas. Cada selección agrega una raíz con su identidad, aunque se repita el nombre.
2. **Revisar**: preparar y comprobar carpeta, código previsto, nombre informativo e incidencias. **Confirmar e importar** registra el plan y coordina recepción, creación, preparación de datos y almacenamiento con las operaciones existentes.
3. **Importar**: recorre los recursos elegibles de todo el lote, independientemente de filtros y páginas. Cada siguiente efecto exige acuse válido y lectura coherente. Se distinguen PDF recibidos, expedientes creados y documentos guardados; los bytes de transporte no equivalen a documentos almacenados.

**Pausar importación** espera la solicitud iniciada e impide enviar la siguiente; no revierte su efecto. Cerrar o recargar no deja una cola ejecutándose en segundo plano. **Consultar importación**, **Consultar último lote**, filtros, páginas y CSV se conservan. Abrir una consulta durante la ejecución primero aplica la pausa; consultar y exportar no reanudan operaciones.

Para recuperar: consultar el mismo lote y pulsar **Continuar importación**. Se reconstruyen los pendientes por IDs y estado del servidor, sin reconfirmar ni repetir resultados confirmados. Tras recargar, **Vincular PDF pendiente** solicita el original que falte; seleccionarlo no lo envía. Una respuesta perdida exige **Consultar resultado**. Los intentos activos, inciertos y reservas retomables conservan su verificación/reanudación explícita; el tiempo transcurrido no permite replay.

El cierre muestra **Resultado de la importación**, **Ver expediente** o **Ver expedientes creados**, y **Cerrar**. El acceso distingue IDs con códigos iguales. Ante nodo oculto o ambiguo, conserva el resultado y avisa para abrir el destino original sin repetir la importación.

El código puede repetirse frente a expedientes anteriores y entre carpetas del lote; el nombre es informativo y cada creación recibe su ID. El editor muestra **Permitir códigos repetidos**. Las versiones históricas se leen sin reescribir snapshots; los planes bloqueados por la comprobación anterior requieren nueva revisión. La política configurable futura todavía no está disponible.

**Ver acciones** conserva operaciones individuales avanzadas; **Ver avance del lote** vuelve al recorrido coordinado. Las descripciones siguientes del envío individual mantienen su alcance de página, sin limitar el nuevo inicio único. Evidencia: [doc120-coordinacion-20261115.md](Validacion/doc120-coordinacion-20261115.md).

## Mensajes del recorrido operativo — DOC-120,2026-11-14

El aviso identifica la fase, el motivo comprobado y la siguiente acción permitida. Las incidencias muestran el campo y, cuando se conoce, formato, segmento desde1 o longitud observada/máxima; no vuelcan valores documentales privados. En una coincidencia de código se puede mostrar el código que ya figura en los datos previstos de esa misma carpeta. Una referencia de diagnóstico aparece solo si llegó del servidor. No se deduce del tiempo transcurrido que una operación haya fallado o pueda repetirse.

Un plan histórico puede conservar la incidencia **Código del expediente** existente o repetido. La comprobación anterior ya no se aplica a nuevas revisiones: prepara otra revisión explícitamente. Se muestra el código solo con un único valor resuelto de la misma raíz; no se consultan ni exponen expedientes anteriores por coincidencia de código.

| Fase | Resultado que puede mostrar | Cómo continuar |
| --- | --- | --- |
| Preparar revisión | Revisión guardada o bloqueada, con incidencias por carpeta/archivo. Por ejemplo: FECHA_INICIAL requiere el segmento3 del nombre. | Corregir selección/regla y preparar otra revisión explícitamente. No se han recibido archivos ni creado expedientes por preparar. Si se pierde la respuesta, la revisión puede haberse guardado; se conserva la selección, sin inventar una referencia de lote. |
| Confirmar plan | Rechazo comprobado; confirmación pendiente de verificar; o lote confirmado cuya consulta falló. | Si no hubo acuse, usar Verificar confirmación pendiente con el plan conservado. Si llegó lote confirmado, consultar ese mismo lote, sin reconfirmar por el fallo de lectura. |
| Enviar archivos | Archivo actual y bytes transferidos; recibos confirmados; archivo que interrumpió la cola y archivos sin intentar. El alcance es la cola seleccionada de la página filtrada. | La cola se detiene ante un fallo. Consultar antes de reenviar una recepción sin confirmar; reseleccionar solo pendientes elegibles. Los recibos confirmados se conservan aunque falle la consulta posterior. |
| Crear/retomar expediente | Incidencia de comando, catálogo, revisión o reserva; creación sin confirmación; o creación confirmada con siguiente acción de índices. | Usar solo las acciones que ofrezca la consulta. Retomar conserva su confirmación y consentimiento. Si únicamente falla el árbol, actualizar la vista: no crear otra vez. |
| Preparar índices/almacenar | Índices preparados, documento confirmado, rechazo comprobado o resultado incierto; archivo, carpeta y lote mantienen estados separados. | Índices preparados todavía requiere Almacenar archivo. Un acuse de almacenamiento no se pierde por un error de consulta. Resultado incierto requiere consulta y verificación permitida, sin repetir el escritor. |

Los bytes de la solicitud no prueban recepción; RECIBIDO no prueba almacenamiento. Un resultado parcial no convierte los archivos sin intentar en rechazados. Tras recargar se conservan referencias de consulta, pero el navegador exige volver a seleccionar los originales pendientes: no se restaura un objeto File desde el servidor. El detalle del aviso se puede enfocar con Tab y recorrer si es largo; consultas, filtros, páginas y CSV siguen disponibles según autoridad.

El catálogo público reutiliza código/mensaje/incidencias del sobre vigente, con alternativa segura para respuestas antiguas o desconocidas. No se muestran SQL, contraseñas, tokens, rutas físicas ni excepciones libres. La traza usa los listeners existentes; la prueba local acredita un listener de prueba, sin garantizar dónde está configurado el destino de traza de cada IIS. Mapa: `Validacion/doc120-mensajes-mapa-20261114.json`. Evidencia de ejecución local: `Validacion/doc120-mensajes-20261114.json`.

## Mostrar un expediente ya confirmado en el árbol

Después de terminar o recuperar una importación, Ctrl+F5 y Consultar importación/Consultar último lote → Actualizar estado vuelven a proyectar los expedientes confirmados de la página consultada en su destino original. Si el nivel estaba vacío se añade el primer hijo con su expansor y búsqueda; los ya representados no se duplican. No vuelvas a confirmar el plan ni enviar archivos. Si el destino no está visible, localízalo y actualiza; el asistente avisa si la vista no puede mostrarlos. Ver Validacion/doc120-primer-expediente-arbol-20261114.md.

## Retomar una creación reservada — opción permanente

1. Abre Consultar importación o Consultar último lote y actualiza el estado.
2. Si el servidor reconoce una reserva de creación que puede retomarse, selecciona su carpeta y pulsa **Retomar creación**. El aviso indica si también se levantará la detención consultada. Cancelar no envía la solicitud.
3. Confirma una vez. Se conservan la importación y los archivos ya recibidos. Los permisos, catálogos y archivos temporales se comprueban de nuevo.
4. Cuando el expediente quede confirmado, continúa con **Preparar índices** y después con el almacenamiento mediante sus acciones separadas.

Si no se confirma la respuesta, usa Actualizar estado. Una creación iniciada o incierta no se reinicia; Verificar intento solo aparece cuando el servidor lo permite. Si falta staging o cambia una revisión, la reanudación se bloquea. No crees otra importación ni reenvíes archivos recibidos para eludir ese bloqueo. Filtros y exportación conservan su funcionamiento.

Implementación verificada localmente con108/108 pruebas y compilación; recuperación autenticada del lote39 pendiente. Ver Validacion/doc120-retomar-creacion-permanente-20261114.md. Los cortes siguientes son históricos.

## Corrección de fechas de origen cortas — 2026-11-14

Una fecha de origen compacta se conserva resuelta en yyyy-MM-dd. La validación de esa salida ya usa sus diez caracteres, sin cambiar el formato/límite de entrada ni editar el perfil. La corrección no reanuda intentos que quedaron reservados: Actualizar solo consulta y Verificar no convierte RESERVADO en verificable. Lote39/intento96 sigue conservado con su detención; la propuesta técnica de recuperación todavía necesita implementación y autorización específicas. No crear otro lote ni reenviar su PDF para sustituir la recuperación. Ver Validacion/doc120-fecha-canonica-corregida-20261114.json.

## Continuación y bloqueo de la parte final — vigente2026-11-14

Después de enviar los pendientes, **Crear expediente** solicita una creación para la carpeta indicada. Solo al confirmarse aparece **Preparar índices** para su archivo; después, **Almacenar archivo**. Son operaciones distintas. **Importación completada** depende del estado confirmado del lote; recibir el PDF no lo almacena.

Si la creación no devuelve confirmación, el aviso permanece visible aunque estés al final de las tablas. Consulta el resultado una vez. Si el servidor conserva **CREANDO/EN_CURSO**, el flujo queda detenido para ese intento: **Revisar bloqueo** lleva al detalle con lote, carpeta e intento sin ejecutar peticiones; **Actualizar estado** solo vuelve a consultar, no reanuda. No se ofrece otra creación ni se habilita Verificar intento por tiempo. Si el servidor publica un INCIERTO verificable, **Verificar intento** comprueba su evidencia sin repetir efectos y puede conservar el bloqueo.

El video194059 muestra este bloqueo en lote39/carpeta60; no consta todavía la causa ni si el escritor llegó a crear el expediente. Diagnóstico preparado: `node tools/e2e/scripts/inspect-doc120-final-state.cjs --lot 39 --root 60 --authorized`. **Ejecutar únicamente después de autorización específica**, con DSN RA del ambiente local autorizado y cuenta MySQL SELECT en consola enmascarada:5 SELECT máximos,0 HTTP,0 modificaciones. La salida saneada queda en .tmp/doc120-final-state, excluida de Git; nunca introducir claves en el chat. La prueba local79/79 no sustituye esa lectura ni acredita recuperación real. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-flujo-final-20261114.json.


## Controles de la maqueta — interfaz vigente2026-11-13

Consultar importación y Consultar último lote permanecen en la barra superior del asistente, visibles al desplazar cualquier etapa. Sin referencia guardada, último lote aparece deshabilitado con explicación. Consultar una referencia abre su seguimiento sin enviar ni confirmar. **Volver a mi importación** recupera la preparación original en memoria; si ya tenía un lote confirmado, relee sus datos antes de habilitar acciones. La consulta requiere reseleccionar sus propios pendientes: no toma los archivos locales de otra preparación por coincidencias de identificación. Cerrar una consulta con preparación sin confirmar mantiene la advertencia de descarte; recargar la página todavía exige reseleccionar Files porque no se guardan en el navegador.

**Ver acciones** en una carpeta o archivo destaca su recurso, sin efectos. El botón del pie solicita únicamente la operación indicada. **Enviar archivo / Enviar N archivos** se limita a pendientes seleccionados en este equipo de la página filtrada; las fases restantes son explícitas. Referencias y estados aparecen en el contexto y en los nombres accesibles, mientras los rótulos visibles de las acciones son breves. Filtros, configuración, incidencias y datos previstos se despliegan; más perfiles, eliminación de archivos y paginación permanecen disponibles.

Verificación local71/71, comparación visual116 propiedades, sin nueva sesión real. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-fidelidad-maqueta-20261113.json.


## Interfaz guiada vigente2026-11-13

1. **Seleccionar**: elige una versión publicada del perfil y agrega una carpeta por selección. Puedes agregar varias, quitar carpetas o archivos y recorrer su inventario. Preparar revisión no envía archivos.
2. **Revisar**: consulta incidencias/valores y confirma el plan cuando el servidor lo permita. Cambiar selección exige preparar de nuevo.
3. **Importar**: destaca fase y recurso actuales, con pausa y continuación explícitas. Confirmar e importar recorre el lote elegible completo; Ver acciones conserva la operación individual. Recepción no equivale a documento almacenado.

Actualizar estado conserva filtros/páginas y muestra fecha/contador tras completar las lecturas. Filtros de carpetas y archivos y configuración se despliegan por separado. Exportar esta página descarga únicamente la página de archivos con sus filtros. Las consultas por referencia, último lote y confirmación pendiente conservan destino/autorización originales. Cerrar antes de confirmar descarta la selección; después conserva referencia y puede exigir reseleccionar originales pendientes, sin reenviar recibidos o inciertos automáticamente. Una detención conserva efectos ya confirmados.

Implementación verificada localmente68/68; evidencia Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-asistente-guiado-20261113.json. Sin nueva aceptación IIS ni cambios de perfil Egresos/datos.


## Recepción previa a creación — DOC-120, 2026-11-13

En Resultados, Enviar pendientes de esta página aparece antes de los filtros. Primero recibe los archivos requeridos por el perfil: todos los de la carpeta para EXPEDIENTE, todos los del lote para LOTE, o al menos un archivo elegible por carpeta en PARCIAL. Crear expediente aparece tras consultar la disponibilidad de recepción calculada por servidor; los filtros y la página visible no reducen el ámbito exigido. La creación vuelve a verificar autoridad, revisión y staging. Continúa con Preparar índices y Almacenar archivo. Tras recargar debes reseleccionar el PDF pendiente antes de enviarlo; conserva el lote existente, sin volver a confirmar el plan. Esta corrección no recibe archivos automáticamente ni resuelve intentos inciertos.

## Navegación operativa DOC-119, 2026-11-12

En Producción Documental, el menú superior **Importación** ofrece **Importar carpetas…** y **Consultar importación…** a quien tiene permiso operativo, aunque no administre perfiles. La sección **Administración** conserva por separado los permisos existentes de perfiles/proveedores/fuentes. Los accesos ya no dependen de una franja permanente.

Para importar desde arriba, selecciona un nivel y abre Importar carpetas. Desde el contextual, pulsa el nivel de destino: se usa ese nivel aunque otro esté seleccionado. Expedientes, documentos y fondo no se convierten en niveles por su nombre. El asistente muestra la ruta autorizada del servidor; una ventana abierta mantiene su destino. El servidor revalida permisos en cada operación.

Para consultar, abre Consultar importación desde el menú superior (no requiere selección previa) o desde un nivel elegible del contextual. Introduce la referencia del lote y pulsa Consultar importación o Enter. El lote conserva su destino original; no se reasigna al nodo seleccionado ni se crea un historial por nivel. La vista permite consultar el último lote y verificar una confirmación pendiente. Recargar conserva únicamente pistas de IDs en el contexto de la sesión; no conserva los archivos locales. Una verificación sin resultado mantiene el aviso y no repite ConfirmarPlan.

Los filtros de carpeta, efecto y recepción se conservan al paginar. El inventario original y el total filtrado se muestran separados. **Exportar esta página** descarga solo la página y los filtros representados; no anuncia exportación total ni un snapshot global entre páginas. Consultar, filtrar y descargar CSV son lecturas; RECIBIDO no significa documento almacenado.

Para un archivo pendiente, usa Reseleccionar en su fila. La elección se vincula al recurso y raíz por IDs, y compara nombre/tamaño/fecha; no demuestra igualdad de contenido. Reseleccionar no envía el PDF. **Enviar pendientes de esta página** es una acción distinta y explícita, con validación de contenido y autoridad en servidor. Los recibidos, inciertos o recursos en vuelo no se reemplazan por coincidencia de nombre.

Ante incertidumbre, consulta el estado o verifica la referencia pendiente antes de repetir una acción. Cerrar libera archivos/URLs del navegador y conserva pistas; no promete cancelar efectos del servidor. Solicitar detención depende del estado y de la capacidad vigente, no realiza rollback global. La operación sin limpieza fue aceptada expresamente: no hay nuevo botón/job/plazo de borrado y se conservan staging, partes e historial.

El contextual cabe en el viewport y permite desplazarse hasta sus últimas opciones. Con foco en el recurso, usa Shift+F10 o la tecla de menú; navega con flechas/Inicio/Fin y cierra con Escape para devolver el foco al invocador. La acción conserva el recurso que abrió el menú.

Evidencia operativa: SEG-01 real completo y SEG-02 real con límites, aceptado con cobertura mixta por el responsable. Inserción nueva y raíces físicas homónimas conservan cobertura local, sin anunciar observación en IIS. La UI definitiva se verifica con Chromium/cliente real y servicios sintéticos, compilación WebForms y regresiones de consumidores; ese alcance no sustituye una nueva prueba autenticada ni autoriza efectos adicionales.

## Consolidación de verificación DOC-120

La interfaz descrita corresponde a la base8c0773d4 entregada por DOC-119; sus fuentes permanecen iguales en la consolidación local DOC-120. Consulta [matriz-verificacion.md](Validacion/matriz-verificacion.md) y [resultados.md](Validacion/resultados.md) para el alcance por caso. Las pruebas locales no equivalen a nueva sesión autenticada, medición de capacidad o ejecución SII. Los límites de capacidad/UI real y la aceptación de salida se revisan expresamente antes del cierre; se mantiene la operación sin limpieza y la conservación de datos.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior en ASP.NET WebForms, VB.NET, JavaScript y MySQL 5.1, respetando las fronteras existentes y el alcance de esta entrega.

## OBJETIVO Y ENCARGO

Lee guía 00, prompt 03, diseño 04 y componentes reales 05/07/10. Conecta la experiencia de resultados y las tareas operativas al estado persistido. No reconstruir resultados desde la memoria del navegador ni implementar otro coordinador.

## Trabajo

Extender los componentes de la ventana 05; no reemplazarlos ni duplicar sus etapas. Los comandos de continuar/cancelar/reconciliar llaman a 10, mientras las consultas/CSV pertenecen a esta entrega. Separar staging retenido de las copias consumibles de 09; la retención nunca presupone que un método legacy conserva el archivo entregado.

- Consultar lotes/resultados con autorización por usuario/ámbito y recurso. Definir paginación, filtros por raíz/estado y totales estables. Mostrar política, nivel, versión, errores por campo, IDs confirmados y fases pendientes.
- Separar recibido de almacenado/incorporado. Porcentajes documentales usan archivos confirmados; incluir rechazados/cancelados/inciertos en el total original. Exportar resultados autorizados protegiendo fórmulas CSV y campos externos.
- Permitir recuperar un lote existente mediante su ID y permisos. Para archivos pendientes de recepción, pedir selección local y comprobar correspondencia; nombre/tamaño/fecha no prueban igualdad de contenido. No almacenar rutas absolutas del dispositivo para leerlas desde servidor.
- Acciones de continuar, cancelar y verificar deben derivarse de estados y capacidades del servicio. No ofrecer reintentar almacenamiento ante resultado incierto sin reconciliación. Explicar estados con mensajes útiles sin SQL, secretos ni rutas internas.
- Al cerrar la ventana, conservar identidad del lote confirmado; abrir una preparación nueva no lo mueve ni elimina. La recepción pendiente no continúa sin los archivos locales disponibles.
- Actualizar la rama del nivel de origen mediante mecanismos existentes, IDs de expediente/relación confirmados y deduplicación. Respetar la navegación actual; no seleccionar otro nodo automáticamente para mostrar un resultado.
- Definir retención de temporales e historial con parámetros aprobados. Implementar limpieza en seco/listado previo y eliminación acotada a rutas propias, sin tocar archivos documentales permanentes ni resultados inciertos. Evitar carreras con recepción/procesamiento activos; registrar resultado y reintento de limpieza.
- Reutilizar mecanismo de mantenimiento existente si es compatible. Si no existe, entregar operación/manual controlado documentado; no crear un temporizador web que prometa mantenimiento durable.
- Añadir observabilidad por lote/archivo/fase, tiempos y códigos sin nombres sensibles innecesarios. Documentar cómo investigar discrepancias entre disco, expediente e índice.

## CRITERIOS DE ACEPTACIÓN

Pruebas de acceso a lote ajeno, resultados paginados, exportación, recarga/reselección, operación incierta, actualización del árbol sin duplicación y limpieza concurrente/protegida. Acreditar que consultar estado no ejecuta efectos documentales. Entregar interfaz conectada, consultas, mantenimiento seguro y guía operativa con recuperación, retención y límites conocidos. Si faltan decisiones de retención, no habilitar borrado automático con un plazo inventado. SQL MySQL 5.1 manual para cambios justificados.

## CONTEXTO Y CONTRATO DE INTEGRACIÓN

Leer `00-guia-ejecucion-prompts.md` y los artefactos vigentes de `Diseno/`; distinguir propuestas de componentes existentes. Para esta entrega, documentar el contrato request/response, autoridad, errores, estados y efectos permitidos. Flujo de referencia: Cliente → entrada ASMX/ASHX (responsabilidad de Controller) → Service → Repository/adaptadores → respuesta; no imponer ASP.NET MVC a WebForms. Los adaptadores internos de 08/09 no se exponen como operaciones públicas independientes.

## PRECONDICIONES DE RUTAS

Antes de implementar, inventariar los archivos exactos en `Diseno/design.md`, con propietario (prompt), existentes/nuevos, responsabilidad y consumidores. Es un mapa de implementación complementario a `Diseno/01-modelo-datos.md` y `Diseno/02-contratos-operacion.md`, no otro diseño de contratos. Si existe un cambio OpenSpec, referenciar ese mapa desde su `design.md` sin duplicarlo. Verificar estas ubicaciones propuestas contra el repositorio; no afirmar que los archivos nuevos ya existen ni crear todas las capas por ejecutar esta entrega.

| Responsabilidad | Ubicación común propuesta | Propiedad |
|---|---|---|
| Modelos y puertos | `Modelo/ProduccionDocumental/ImportacionCarpetas/` | 04 define; cada entrega añade solo su contrato. |
| DTOs de entrada/salida | `DTOs/ProduccionDocumental/ImportacionCarpetas/` | 02/06/07/10/11 según operación. |
| Servicios y reglas | `Services/ProduccionDocumental/ImportacionCarpetas/` | 02 perfil; 06 plan; 07 recepción; 10 coordinación; 11 consulta/mantenimiento. |
| Repositorios | `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/` | 02 perfil; 07-A operación. Los demás consumen sus interfaces. |
| Adaptadores documentales | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/` | 08 expedientes; 09 almacenamiento e incorporación. |
| Servicio web nuevo | `webservice/WebServiceImportacionCarpetas.asmx` y `webservice/WebServiceImportacionCarpetas.asmx.vb` | Una fachada de las operaciones anteriores; sin reglas documentales en transporte. |
| Recepción multipart nueva | `generic_control/ImportacionCarpetasUpload.ashx` y `generic_control/ImportacionCarpetasUpload.ashx.vb` | 07-B; delega en su servicio; no sustituye el handler compartido. |
| Integración visual existente | `Gestion/WebFormProducionDocumental.aspx`, su code-behind `.aspx.vb` y `js/gestion/WebFormProducionDocumental.js` | 05 integra; 11 extiende resultados sobre la misma ventana. |
| JavaScript específico nuevo | `js/gestion/ImportacionCarpetas/` | 05 estado/transporte/renderizado; 11 seguimiento; nombres exactos en el inventario. |

Registrar los archivos nuevos de compilación y contenido en `GestionDocumental-Docuarchi.net.vbproj` según el tipo de recurso. Mantener namespaces y referencias compatibles con .NET Framework 4.6.1. Las rutas nuevas son destino de implementación, no evidencia de componentes existentes. El host opcional del 13 requiere su propio inventario tras decidir alojamiento; no inventar un ejecutable desplegado.

## RESTRICCIONES CRÍTICAS

No debe modificarse SII: código, tablas, configuración, contratos ni consumidores. Preservar carga individual y contratos legacy; no cambiar pruebas/huellas para ocultar regresiones. No crear otro árbol ni cambiar el nivel al elegir perfil. No usar autorregistro, tareas ficticias, valores fijos del ejemplo ni temporizadores del prototipo como implementación productiva.

Mantener COMPLETO_OBLIGATORIO configurable por EXPEDIENTE o LOTE; exigir elección al publicar y congelarla en plan/lote. PARCIAL_PERMITIDO usa NO_APLICA. No prometer rollback global: conservar efectos confirmados y reconciliar incertidumbre. Cada prompt implementa solo su responsabilidad según la guía 00; no duplicar motor, repositorios ni endpoints.

El equipo aplica SQL manualmente: MySQL 5.1, sin migraciones automáticas. Los scripts de perfil pertenecen a 02 y los operativos a 07; otros cambios justificados usan incrementos coordinados. No ejecutar SQL por tener conexión ni declarar aplicado un script por existir.

## PRUEBAS OBLIGATORIAS

Consultar no muta; lote ajeno se rechaza; CSV escapa fórmulas; recargar mantiene identidad/destino; limpieza no elimina archivos activos, permanentes o inciertos.

Ejecutar pruebas unitarias o QA manual reproducible según el impacto, además de integración para persistencia y efectos. Registrar comandos, entorno, resultado y limitaciones. Para cambios VB/proyecto, compilar con MSBuild usando la configuración real de `GestionDocumental-Docuarchi.net.vbproj`; no sustituirlo por un build .NET moderno. Si faltan herramientas/referencias, registrar la limitación. Para JavaScript, comprobar sintaxis con `node --check <archivo-real>` y comportamiento con la infraestructura existente. Las suites DOC-88 aplicables son evidencia estática, no prueba de persistencia real.

La validación E2E del comportamiento entregado es parte integral del mismo cambio: código, pruebas, validación autorizada y evidencia saneada forman su unidad de entrega. El prompt 12 consolida regresión transversal; no sustituye las pruebas de cada componente. Para servicios/adaptadores internos, validar primero contratos y fallos mediante unitarias/integración; no crear endpoints de prueba para eludir el coordinador. Si el recorrido depende de componentes posteriores, registrar integración pendiente y no declarar cerrado el comportamiento integrado.

Reutilizar exclusivamente autenticación, configuración, validadores y utilidades de `tools/e2e`; no crear login, arnés, proyecto Playwright ni .env paralelos. Leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md` antes de autenticar. Ejecutar pruebas reales solo con autorización explícita del ambiente, cuentas y datos descartables; este prompt no concede esa autorización. Cubrir control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada con esta entrega.

Usar secretos efímeros; no imprimir ni guardar credenciales, cookies, tokens o cadenas de conexión. Consultas de control solo SELECT; conservar evidencia saneada. Respetar gates de seguridad existentes, sin habilitar arbitrariamente funciones o permisos para probar. No reintroducir flags ni listas de usuarios/grupos como activación de la experiencia moderna. No cerrar sin validación requerida: ante ambiente o autorización ausente, registrar bloqueo explícito, casos no ejecutados y motivo; mocks y simulaciones no sustituyen evidencia real.

## E2E POR CAPACIDAD Y CRITERIO DE CIERRE

### Reutilizaci?n obligatoria de infraestructura E2E

Antes de escribir pruebas, inventariar y reutilizar `tools/e2e/package.json`, `tools/e2e/playwright.config.cjs`, `tools/e2e/tests/support/`, `tools/e2e/scripts/`, `tools/e2e/fixtures/` y `tools/e2e/AGENT-RUNBOOK.md`. Ubicar las nuevas especificaciones en `tools/e2e/tests/` y registrar solo los comandos necesarios en el paquete existente. Usar los reporteros y directorios de evidencia ya configurados; conservar saneamiento de secretos y datos sensibles tambi?n en capturas y trazas.

Reutilizar `tools/e2e/tests/support/authenticated-workflow-session.cjs` cuando su contrato soporte el acceso requerido; verificarlo antes, sin fabricar tareas Workflow. Si falta soporte para Producci?n, ampliar de forma compatible el soporte compartido y su configuraci?n/runner existente, con pruebas de regresi?n de los consumidores afectados. Reutilizar validadores de autorizaci?n, perfiles y utilidades aplicables identificados en el inventario; documentar qu? se reutiliza y qu? extensi?n m?nima necesita esta capacidad. No copiar suites completas ni introducir login, almacenamiento de sesi?n, .env, dependencias, instalaci?n de navegadores, runner o configuraci?n Playwright paralelos. Un script delgado nuevo dentro de `tools/e2e/scripts/` solo puede seleccionar escenarios y delegar en los mecanismos compartidos; no reimplementar autenticaci?n ni autorizaci?n.

No cambiar globalmente workers, reintentos, timeouts, permisos o configuraci?n para lograr pruebas verdes. Cualquier extensi?n compartida debe mantener los contratos de las suites existentes y acreditar regresi?n pertinente. Las pruebas DOC-88 de pol?tica son evidencia est?tica complementaria, no reemplazan los escenarios E2E reales. El inventario de reutilizaci?n y la ausencia de infraestructura duplicada son criterios de aceptaci?n de esta entrega; para 04 se documentan y para 13 solo se implementan al activar su alcance opcional.

Implementar estos escenarios como parte de la entrega, reutilizando `tools/e2e`. Son pruebas futuras: esta documentación no acredita suites creadas ni corridas. Leer [la estrategia de comprobación progresiva](Validacion/01-estrategia-e2e-capacidades.md).

| ID | Precondiciones | Pasos del recorrido | Aserciones obligatorias |
|---|---|---|---|
| SEG-01 | Lote con recibidos, completados y rechazados. | Consultar paginación/filtros y exportar CSV; repetir con usuario ajeno. | Totales del inventario original, estados diferenciados y fórmulas CSV escapadas; acceso ajeno rechazado; consultas sin mutación. |
| SEG-02 | Lote recuperable con bytes locales pendientes. | Recargar página, recuperar lote y reseleccionar archivos; verificar y actualizar árbol. | Destino e IDs conservados; validar correspondencia de contenido; árbol sin duplicados ni cambio de selección actual. |
| SEG-03 | Política de retención aprobada y staging de prueba. | Listar candidatos en seco; autorizar limpieza y ejecutarla mientras otro archivo está activo. | Solo temporales propios elegibles eliminados; activos, inciertos y documentos permanentes intactos; evidencia de cada resultado. |

Para cada caso registrar estado EJECUTADO_OK, FALLIDO o NO_EJECUTADO, SHA probado, ambiente, datos sintéticos, pasos/comando real, evidencia saneada de UI/HTTP y comprobaciones persistidas de solo lectura. No declarar aprobada una capacidad únicamente con mocks o revisión estática. Un caso bloqueado debe identificar dependencia y responsable; no se omite del informe.

Entregar especificación automatizada y comando reproducible dentro de la infraestructura existente; el nombre final se inventaría en `Diseno/design.md`. Si la autorización no permite ejecutarla, entregar suite y evidencia local pertinente, manteniendo la validación real pendiente. No habilitar permisos ni ejecutar cargas por inferencia de este prompt.

## DOCUMENTACIÓN TÉCNICA

Actualizar documentación existente en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`: `Diseno/` para contratos y decisiones, y `Validacion/` para evidencia. Mantener guía 00 y consumidores sincronizados si cambia un contrato. No abrir una segunda fuente de verdad. Registrar archivos y funciones afectados, dependencia propietaria, casos ejecutados/no ejecutados y riesgos pendientes. Una revisión estructural PASS no certifica implementación ni ausencia de regresiones.

## ENTREGABLE FINAL

Conectar seguimiento, consultas, CSV y recuperación al coordinador existente, con actualización del árbol y limpieza segura. Entregar los artefactos especificados en este prompt y una relación verificable de cambios, pruebas, resultados y pendientes. No declarar funcionalidades completas por entregar únicamente stubs, mocks o documentación.
