# importacion-excel-06-interfaz-menu Specification

## Purpose

Permitir a las personas autorizadas administrar plantillas y cargar libros Excel desde Producción Documental, revisar su validación y recuperar el resultado durable, conservando los permisos, límites y datos de las capacidades existentes.

## Requirements

### Requirement: RQ-01 — Acceso exclusivo y bootstrap autorizado

El sistema SHALL ofrecer y autorizar la carga Excel por su propio permiso vigente, sin exigir capacidades administrativas o de importación de carpetas adicionales. Trazabilidad: D-01.

#### Scenario: Permiso exclusivo
- **WHEN** una cuenta tiene únicamente el permiso del cargador
- **THEN** encuentra el acceso y abre el recorrido sin un nivel seleccionado ni permisos de otras capacidades

#### Scenario: Acceso denegado o revocado
- **WHEN** falta el permiso o se revoca durante el uso
- **THEN** el servidor rechaza la operación, el cliente cierra su capacidad y no envía nuevas mutaciones

#### Scenario: Apertura válida y capacidad cerrada
- **WHEN** se abre la pantalla con límites inválidos para nuevas cargas
- **THEN** se conserva la consulta autorizada de resultados y se impiden nuevas recepciones

### Requirement: RQ-02 — Menú común y coexistencia

El sistema SHALL integrar el acceso Excel en el menú de Importación vigente, manteniendo un único menú y los accesos existentes según sus permisos. Trazabilidad: D-02.

#### Scenario: Menú único
- **WHEN** conviven Excel y las capacidades ya instaladas
- **THEN** cada acceso aparece una sola vez en el menú común y conserva su permiso

#### Scenario: Reinicialización
- **WHEN** se repite el montaje o se actualiza parcialmente la página
- **THEN** se preserva un único acceso Excel y no se duplican listeners ni operaciones

#### Scenario: Ausencia de otras capacidades
- **WHEN** Excel es la única capacidad disponible
- **THEN** su acceso funciona y no depende de crear una importación de carpetas

### Requirement: RQ-03 — Cliente del contrato Excel y transporte cerrado

El sistema SHALL transportar las solicitudes y respuestas Excel actuales sin perder los datos de validación fallida ni repetir automáticamente operaciones de resultado desconocido. Trazabilidad: D-03.

#### Scenario: Preparación con incidencias
- **WHEN** el servidor devuelve rechazo con el detalle de validación
- **THEN** el cliente presenta el detalle y conserva el ID en vez de tratarlo como una respuesta ilegible

#### Scenario: Respuesta inválida o timeout
- **WHEN** una mutación pierde su respuesta o recibe contenido no interpretable
- **THEN** se conserva su referencia y se exige consultar sin reintento automático

#### Scenario: Transporte propio
- **WHEN** se envía una operación del cargador
- **THEN** usa rutas y referencias autorizadas, sin permitir un destino de conexión o una operación arbitrarios

### Requirement: RQ-04 — Administración de plantillas existente y CAS

El sistema SHALL permitir administrar las plantillas mediante sus contratos actuales, conservando revisiones, transformaciones admitidas y compatibilidad de las plantillas históricas. Trazabilidad: D-04.

#### Scenario: Selección paginada
- **WHEN** se busca o selecciona una plantilla
- **THEN** se muestra su detalle vigente, destino, formato y clave única dentro de la conexión autorizada

#### Scenario: Edición concurrente
- **WHEN** otro editor cambia la plantilla antes de guardar
- **THEN** se rechaza el guardado obsoleto sin sobrescribir la revisión nueva

#### Scenario: Guardado incierto
- **WHEN** se pierde la respuesta de un alta o edición
- **THEN** la interfaz conserva el formulario, informa incertidumbre y evita repetir la mutación automáticamente

### Requirement: RQ-05 — Recepción única, límites y hojas reales

El sistema SHALL recibir un único libro compatible y permitir elegir una hoja disponible, mostrando los límites efectivos y sometiendo su validación completa al servidor. Trazabilidad: D-05.

#### Scenario: Archivo incompatible
- **WHEN** la selección supera el tamaño visible o tiene extensión incompatible con la plantilla
- **THEN** se informa el problema y no se inicia una recepción incompatible

#### Scenario: Hojas recibidas
- **WHEN** la recepción termina correctamente
- **THEN** se muestran las hojas acreditadas y se prepara únicamente una selección válida

#### Scenario: Respuesta de recepción perdida
- **WHEN** se pierde el acuse del archivo recibido
- **THEN** se conserva el ID para consulta y no se envía otra copia automáticamente

### Requirement: RQ-06 — Intención y preparación vigentes

El sistema SHALL preparar exactamente la intención seleccionada y habilitar la confirmación sólo para una validación completa vigente, invalidándola ante cambios relevantes. Trazabilidad: D-06.

#### Scenario: Preparación íntegra
- **WHEN** el servidor acredita lectura y validación completas sin incidencias
- **THEN** se habilita confirmar únicamente esa intención y sus hashes

#### Scenario: Selección modificada
- **WHEN** se cambia un dato que determina la carga después de prepararla
- **THEN** se deshabilita confirmar y se descarta el consentimiento anterior

#### Scenario: Preparación obsoleta
- **WHEN** se intenta sustituir una preparación con una huella anterior
- **THEN** se rechaza el cambio y se exige revisar el estado vigente

### Requirement: RQ-07 — Vista previa e incidencias acotadas

El sistema SHALL presentar una vista previa y un listado de incidencias acotados, conservando el significado de los valores y distinguiendo muestras de totales. Trazabilidad: D-07.

#### Scenario: Valores significativos
- **WHEN** la muestra contiene ceros iniciales, decimales amplios, fechas y nulos
- **THEN** se muestran sin pérdida de precisión, interpretación HTML ni cambio de fecha

#### Scenario: Incidencias truncadas
- **WHEN** hay más incidencias que las enviadas
- **THEN** se muestra el total y la limitación de la muestra con sus ubicaciones conocidas

#### Scenario: Muestra parcial
- **WHEN** se presenta una preparación válida de muchas filas
- **THEN** la pantalla distingue filas observadas, muestra visible y validación completa

### Requirement: RQ-08 — Estado de interacción y respuestas tardías

El sistema SHALL mantener una interacción coherente ante concurrencia de respuestas, doble activación y recreación de la página, sin atribuir cancelación a un cierre de ventana. Trazabilidad: D-08.

#### Scenario: Respuesta antigua
- **WHEN** una selección nueva precede a la respuesta de la anterior
- **THEN** la respuesta tardía no cambia controles, vista previa ni intención confirmable

#### Scenario: Doble activación
- **WHEN** se activa rápidamente dos veces una mutación
- **THEN** se envía una sola solicitud de esa vista

#### Scenario: Cierre durante actividad
- **WHEN** se cierra el diálogo o se reemplaza su host mientras el servidor trabaja
- **THEN** no se promete cancelación, se conserva el ID y los callbacks antiguos no alteran la vista nueva

### Requirement: RQ-09 — Confirmación explícita y resultado durable

El sistema SHALL confirmar únicamente la preparación vigente y exigir consentimiento específico para reemplazar, mostrando el resultado durable acreditado por el servidor. Trazabilidad: D-09.

#### Scenario: Reemplazo consentido
- **WHEN** el usuario confirma la sustitución para la intención mostrada
- **THEN** se envía una sola confirmación con sus hashes y consentimiento específico

#### Scenario: Reemplazo no consentido
- **WHEN** falta aceptación o cambia la intención después de aceptarla
- **THEN** no se envía una confirmación destructiva

#### Scenario: Resultado durable
- **WHEN** el servidor acredita la confirmación o un rollback conocido
- **THEN** la interfaz muestra el estado y conteo acreditados sin convertir un fallo en éxito

### Requirement: RQ-10 — Recuperación contextual sin secretos

El sistema SHALL conservar una referencia de carga para recuperar su resultado autorizado sin duplicar efectos, aislarla por contexto y evitar persistir secretos o el libro en el navegador. Trazabilidad: D-10.

#### Scenario: Acuse perdido
- **WHEN** se pierde una respuesta de confirmación
- **THEN** se consulta el mismo ID y se muestra el resultado observado sin reimportar

#### Scenario: Recarga o cambio de contexto
- **WHEN** se recarga la página o cambia la cuenta, empresa o módulo
- **THEN** se recuperan sólo referencias del contexto vigente y no se confirman cargas desde referencias incompletas

#### Scenario: Almacenamiento no disponible
- **WHEN** el navegador rechaza guardar referencias
- **THEN** se conserva la referencia en memoria, se ofrece consulta manual y se informa la limitación antes de recargar

### Requirement: RQ-11 — Diálogo accesible y visibilidad

El sistema SHALL permitir operar el diálogo mediante teclado y mantener sus controles accesibles y visibles en el área disponible. Trazabilidad: D-11.

#### Scenario: Recorrido de teclado
- **WHEN** se abre, recorre y cierra el diálogo por teclado
- **THEN** el foco queda contenido, las etiquetas son identificables y vuelve al acceso adecuado

#### Scenario: Área reducida o zoom
- **WHEN** se usa un viewport reducido o zoom aumentado
- **THEN** las acciones y mensajes siguen alcanzables sin quedar recortados

#### Scenario: Estados anunciados
- **WHEN** una operación cambia de estado o devuelve incidencias
- **THEN** el cambio se anuncia de manera accesible sin depender sólo de color o animación

### Requirement: RQ-12 — Información operativa y salida segura

El sistema SHALL exponer únicamente información operativa necesaria y renderizar datos y errores de forma segura, sin revelar secretos ni diagnósticos internos. Trazabilidad: D-12.

#### Scenario: Dato con marcado
- **WHEN** un nombre, celda o incidencia contiene texto con apariencia HTML
- **THEN** se muestra como texto y no ejecuta contenido

#### Scenario: Error inesperado
- **WHEN** el transporte o servidor devuelve un fallo no reconocido
- **THEN** se presenta un mensaje operativo y una acción de consulta o recarga sin detalles internos

#### Scenario: Información para decidir
- **WHEN** se prepara un reemplazo o se consulta un resultado
- **THEN** se muestran destino, clave, intención y referencia útiles sin rutas privadas, tokens ni cadenas de conexión

### Requirement: RQ-13 — SQL manual y reutilización de instalación

El sistema SHALL reutilizar la instalación compatible y entregar cualquier adecuación o preparación administrativa necesaria como SQL revisable de ejecución manual separada de las pruebas. Trazabilidad: D-13.

#### Scenario: Instalación compatible
- **WHEN** el preflight acredita los recursos actuales
- **THEN** la interfaz los utiliza sin instalar ni cambiar el esquema automáticamente

#### Scenario: Adecuación identificada
- **WHEN** se necesita corregir configuración o preparar nuevos datos de prueba
- **THEN** se entrega un archivo nuevo y el recorrido dependiente espera aplicación manual confirmada y comprobación posterior

#### Scenario: Prueba sin instalación
- **WHEN** se ejecuta la aceptación de interfaz
- **THEN** no instala, siembra, limpia ni concede permisos como parte del test

### Requirement: RQ-14 — Aceptación UI con E2E existentes

El sistema SHALL acreditar los recorridos de interfaz con las E2E y recursos existentes, distinguiendo ejecución local de real y exigiendo autorización específica para cuentas, ambiente y efectos. Trazabilidad: D-14.

#### Scenario: Lectura y permiso exclusivo
- **WHEN** se autoriza un recorrido real de sólo lectura con cuentas acotadas
- **THEN** se comprueban menú y acceso exclusivo/restringido sin confirmar cargas ni editar configuración

#### Scenario: Escritura de interfaz autorizada
- **WHEN** se autoriza el alcance descartable de escritura DOC126 y su instalación está acreditada
- **THEN** los tres modos y fallos previstos se recorren por controles reales, conservando huellas de destinos ajenos

#### Scenario: Scope ajeno o incompleto
- **WHEN** se intenta usar autorización de otra fase o faltan límites de efectos
- **THEN** el runner se detiene antes de autenticar o abrir una ventana escritora

#### Scenario: Cuenta existente para agregar y reemplazar
- **WHEN** el usuario autoriza su cuenta vigente para la prueba funcional y sus destinos de prueba están preparados
- **THEN** el runner reutiliza la interfaz y controles existentes para agregar y reemplazar con identidad, permisos y efectos fijados, sin exigir ni modificar permisos exclusivos; ese recorrido no acredita el caso de acceso exclusivo o restringido

### Requirement: RQ-15 — Compatibilidad y entrega verificable

El sistema SHALL entregar la interfaz compatible con los consumidores y la plataforma actuales, con compilación, regresión y evidencia que declaren el alcance real y permitan revertir el despliegue sin borrar datos. Trazabilidad: D-15.

#### Scenario: Compatibilidad de la entrega
- **WHEN** se compila y verifica la interfaz y sus regresiones
- **THEN** se conservan recepción, preparación, confirmación, navegación y consumidores existentes

#### Scenario: Evidencia diferenciada
- **WHEN** se registra el resultado de validaciones locales o reales
- **THEN** cada resultado declara SHA, ambiente, alcance y límites sin atribuir ejecución real a mocks

#### Scenario: Reversión de entrega
- **WHEN** se debe retirar la interfaz desplegada
- **THEN** se restauran assets/bin compatibles sin eliminar tablas, configuración, cargas ni resultados

### Requirement: RQ-16 — Crear plantilla desde Excel sin alterar la carga

El sistema SHALL permitir generar una propuesta editable de plantilla desde un XLS/XLSX sin plantilla previa, manteniendo autoridad y límites vigentes, sin efectos durables durante el análisis y conservando los contratos actuales de guardado, recepción y confirmación. Trazabilidad: D-16.

#### Scenario: Generación de campos y selección de clave
- **WHEN** el usuario analiza una hoja de un libro válido con encabezados en fila 1
- **THEN** obtiene campos de origen exactos, destinos únicos válidos, tipos conservadores y muestra acotada; elige la clave y guarda explícitamente mediante el CAS existente

#### Scenario: Archivo incompatible o ambiguo
- **WHEN** el archivo excede límites, contiene encabezados vacíos o repetidos, datos sin encabezado o errores de celda
- **THEN** se rechaza la propuesta sin registrar carga ni guardar plantilla y se conserva la posibilidad de edición manual

#### Scenario: Continuación explícita con el mismo archivo
- **WHEN** se guarda una propuesta analizada y se pulsa recibir
- **THEN** se utiliza el File retenido sólo en memoria, se contrasta el hash recibido y se exige la preparación actual antes de confirmar; no se importa al guardar y una recarga requiere seleccionar de nuevo

#### Scenario: Cambio de intención y compatibilidad
- **WHEN** cambia el archivo, hoja o contexto, se cierra durante análisis o se edita una plantilla existente
- **THEN** no se aplican respuestas obsoletas, una propuesta invalidada no se guarda como vigente y los contratos previos y su edición manual permanecen disponibles

#### Scenario: Nombre de destino sugerido y corrección de campos requeridos
- **WHEN** se generan campos para una plantilla nueva en modo crear o se intenta guardar con destino incompleto o clave no mapeada
- **THEN** se propone un nombre válido y editable desde el archivo conservando elecciones manuales, y el guardado inválido se detiene con un mensaje específico y foco en el campo correspondiente

### Requirement: RQ-17 — Diagnóstico explicativo de configuración

El sistema SHALL explicar los rechazos de configuración mediante reglas acreditadas por el servidor, identificando campo y fila, causa y corrección, conservando contratos, estado de guardado y valores históricos sin revelar información sensible. Trazabilidad: D-17.

#### Scenario: Opciones nuevas compatibles con la instalación
- **WHEN** se generan o añaden campos nuevos
- **THEN** se proponen opciones No/No/No y posición 0; los NULL de campos históricos permanecen intactos y el servidor conserva las restricciones físicas

#### Scenario: Configuración rechazada antes de modificar
- **WHEN** la matriz contiene opciones nulas no admitidas, longitud o codificación incompatible
- **THEN** el servidor identifica las filas y reglas antes de escribir, conserva el código anterior y devuelve hasta 25 detalles con el total

#### Scenario: Causa y corrección accesibles
- **WHEN** llega una regla de validación reconocida
- **THEN** la interfaz explica Qué ocurre y Cómo corregirlo, identifica la configuración y permite llevar el foco a su control sin perder el formulario ni mostrar valores de celda o mensajes del proveedor

#### Scenario: Respuesta sin causa acreditada
- **WHEN** el rechazo no identifica una regla utilizable o el guardado es incierto
- **THEN** se reconoce la limitación, se conserva el formulario y se exige consultar antes de repetir un alta, sin afirmar una causa o un resultado no acreditados

### Requirement: RQ-18 — Guía por semáforo y plantilla reutilizable

El sistema SHALL guiar la importación mediante etapas con color, texto y siguiente acción explícita, y permitir reutilizar la misma plantilla para operaciones compatibles conservando autoridad, CAS, intención congelada y confirmación expresa. Trazabilidad: D-18.

#### Scenario: Siguiente acción y pasos acreditados
- **WHEN** el usuario selecciona una plantilla, recibe el archivo y completa su validación
- **THEN** se distinguen etapas completadas en verde, siguiente paso en amarillo y pendientes en gris, con textos equivalentes; la guía lleva a revisar y confirmar sin enviar la confirmación automáticamente

#### Scenario: Incidencia o resultado desconocido
- **WHEN** hay incidencias, capacidad cerrada o resultado incierto
- **THEN** la etapa afectada requiere atención con color rojo y texto; la incertidumbre guía a consultar el mismo ID sin repetir escrituras, y consultar una referencia histórica no completa pasos de otro archivo

#### Scenario: Cambio de operación en la misma plantilla
- **WHEN** el usuario pulsa Cambiar operación de esta plantilla
- **THEN** se consulta el detalle vigente y se edita el mismo ID mediante guardado explícito con CAS, conservando campos históricos y exigiendo consentimiento separado para reemplazar; no se crea otra plantilla ni se importa al guardar

#### Scenario: Nueva importación después de crear tabla
- **WHEN** se inicia otra importación con la plantilla de una carga confirmada
- **THEN** se consulta su detalle vigente para reconocer AGREGAR cuando el destino ya existe, se exige seleccionar otro archivo y validar otra intención; un fallo de consulta impide reutilizar el modo obsoleto y los resultados anteriores permanecen consultables
