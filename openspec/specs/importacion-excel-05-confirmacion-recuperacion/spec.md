# importacion-excel-05-confirmacion-recuperacion Specification

## Purpose

Confirmar cargas Excel privadas con escrituras atómicas, resultado durable y recuperación por identidad de operación, manteniendo el ámbito autorizado y distinguiendo fallos revertidos de respuestas inciertas.

## Requirements

### Requirement: RQ-01 — Autoridad y petición de confirmación

El sistema SHALL confirmar únicamente peticiones autenticadas, autorizadas y correspondientes a la carga propia y su contenido registrado; el reemplazo exige confirmación explícita. Trazabilidad: D-01.

#### Scenario: Permiso revocado
- **WHEN** se confirma o consulta tras revocar el permiso
- **THEN** se rechaza sin escrituras ni exposición del resultado

#### Scenario: Petición manipulada
- **WHEN** se envía UUID, conexión o hashes ajenos, un cuerpo con autoridad cliente o CSRF inválido
- **THEN** se rechaza sin operar sobre el destino

#### Scenario: Contenido coincidente
- **WHEN** la petición propia coincide con los tres hashes persistidos
- **THEN** se evalúa el estado de esa misma carga

### Requirement: RQ-02 — Snapshots y validación completa

El sistema SHALL revalidar todo el archivo y los snapshots de una ejecución sin confirmar antes de escribir, rechazando cambios incompatibles sin actualizar silenciosamente su intención. Trazabilidad: D-02.

#### Scenario: Archivo o configuración cambiados
- **WHEN** cambia archivo, hoja, plantilla, revisión, política, identidad física o esquema desde la preparación
- **THEN** se rechaza la confirmación sin insertar ni borrar filas

#### Scenario: Lectura completa
- **WHEN** el archivo termina con una fila inválida o tiene duplicados fuera de la muestra
- **THEN** se rechaza toda la carga aunque la muestra inicial sea válida

#### Scenario: Carga DOC124 vigente
- **WHEN** se presenta una carga PREPARADA v1 cuyo contenido y límites siguen vigentes
- **THEN** se acepta su snapshot sin migrarlo ni recibir nuevamente el archivo

### Requirement: RQ-03 — Unidad transaccional durable

El sistema SHALL guardar las filas efectivas y el resultado CONFIRMADA de forma atómica, sin confirmaciones parciales ni resultados exitosos sin datos. Trazabilidad: D-03.

#### Scenario: Commit exitoso
- **WHEN** finaliza una carga válida
- **THEN** las filas y CONFIRMADA quedan persistidas juntas, con conteo exacto y fecha UTC

#### Scenario: Fallo intermedio
- **WHEN** falla una agrupación posterior a otra ya insertada
- **THEN** no queda ninguna fila parcial de esa ejecución

#### Scenario: Edición concurrente
- **WHEN** el editor moderno intenta modificar la plantilla durante la escritura
- **THEN** la lectura bloqueada conserva la configuración evaluada hasta terminar la transacción

### Requirement: RQ-04 — Bloqueos compatibles con MySQL 5.1

El sistema SHALL impedir efectos simultáneos del módulo sobre una misma carga o destino físico, incluso con distintas plantillas, usuarios o referencias al mismo destino. Trazabilidad: D-04.

#### Scenario: Mismo ID concurrente
- **WHEN** dos sesiones confirman la misma carga
- **THEN** hay una sola escritura efectiva; la otra recibe actividad conocida o el resultado durable

#### Scenario: IDs distintos y mismo destino
- **WHEN** dos cargas apuntan a la misma tabla física mediante referencias diferentes
- **THEN** el módulo serializa sus efectos sin liberar el bloqueo de carga

#### Scenario: Destinos distintos
- **WHEN** dos cargas usan tablas diferentes y no comparten configuración bloqueada
- **THEN** pueden avanzar sin un bloqueo global de todas las importaciones

### Requirement: RQ-05 — Escritura tipada y acotada

El sistema SHALL escribir los valores validados conservando tipos y significado, con comandos acotados y rechazo íntegro ante conversiones o tamaños incompatibles. Trazabilidad: D-05.

#### Scenario: Tipos preservados
- **WHEN** se confirman textos con ceros, NULL permitidos, fechas y decimales válidos
- **THEN** el destino conserva el valor normalizado sin interpolación SQL ni truncamiento

#### Scenario: Fila demasiado grande
- **WHEN** una fila supera el presupuesto del paquete antes de iniciar los cambios de datos
- **THEN** se rechaza sin borrar el contenido anterior

#### Scenario: Advertencia del proveedor
- **WHEN** MySQL informa conversión, precisión o longitud inválida durante la escritura
- **THEN** se revierte la carga completa y se devuelve un código saneado

### Requirement: RQ-06 — Agregar y carrera de clave

El sistema SHALL agregar todas las filas válidas o ninguna, rechazando duplicados internos, existentes o concurrentes según la igualdad efectiva del destino. Trazabilidad: D-06.

#### Scenario: Archivo íntegramente nuevo
- **WHEN** todas las claves son nuevas y únicas
- **THEN** se insertan todas las filas y se conserva el contenido previo

#### Scenario: Conflicto por collation
- **WHEN** una clave es equivalente a otra del archivo o destino por tipo/collation
- **THEN** se rechaza toda la carga sin omitir la fila conflictiva

#### Scenario: Escritor externo
- **WHEN** otro escritor gana una clave después de validarla y antes de insertarla
- **THEN** UNIQUE rechaza el intento y el rollback conserva sólo los efectos propios de ese otro escritor

### Requirement: RQ-07 — Reemplazar dentro del ámbito

El sistema SHALL sustituir atómicamente sólo el contenido autorizado, con confirmación explícita y conservación del contenido previo si el intento falla. Trazabilidad: D-07.

#### Scenario: Confirmación ausente
- **WHEN** se solicita reemplazar sin confirmación explícita
- **THEN** se rechaza sin ejecutar DELETE

#### Scenario: Filtro empresarial
- **WHEN** una fuente compartida autoriza sólo una empresa
- **THEN** se sustituyen únicamente sus filas y se rechaza cualquier fila de otra empresa

#### Scenario: Fallo después de borrar
- **WHEN** ocurre un error tras DELETE y antes del commit
- **THEN** el rollback restaura todas las filas anteriores y sus índices permanecen

### Requirement: RQ-08 — Crear con procedencia recuperable

El sistema SHALL identificar la estructura creada por cada carga y recuperar su creación sin confundirla con una tabla ajena ni repetir datos ya confirmados. Trazabilidad: D-08.

#### Scenario: Destino ausente
- **WHEN** se confirma CREAR válido sobre un nombre libre
- **THEN** se crea estructura con procedencia e índice y después se confirma la carga de filas

#### Scenario: Respuesta perdida de CREATE
- **WHEN** la estructura propia se creó pero no se recibió su respuesta
- **THEN** consultar reconoce su identidad y un reintento explícito compatible continúa sin recrearla

#### Scenario: Tabla ajena o alterada
- **WHEN** el destino existe con otra procedencia o estructura incompatible
- **THEN** se rechaza sin borrar, alterar ni insertar en esa tabla

#### Scenario: Estructura propia después de rollback
- **WHEN** falla la carga de filas posterior al CREATE
- **THEN** se conserva la estructura identificada y vacía, informando el fallo sin declarar que desapareció

### Requirement: RQ-09 — Fallos conocidos e incertidumbre

El sistema SHALL distinguir el rollback acreditado de una respuesta incierta, conservar la identidad de operación y evitar reintentos automáticos con efectos desconocidos. Trazabilidad: D-09.

#### Scenario: Rollback acreditado
- **WHEN** la transacción de datos revierte de forma conocida
- **THEN** se registra FALLIDA y su código sin perder los snapshots ni modificar una confirmación concurrente

#### Scenario: Commit de respuesta incierta
- **WHEN** se pierde la conexión al confirmar la transacción
- **THEN** se conserva el ID y se exige consultar antes de un reintento explícito

#### Scenario: Limpieza posterior fallida
- **WHEN** el commit ya terminó pero falla la liberación de recursos
- **THEN** CONFIRMADA no se degrada y su consulta permite recuperar el resultado durable

### Requirement: RQ-10 — Resultado histórico y consulta sin efectos

El sistema SHALL recuperar el resultado histórico de una carga confirmada sin depender de la disponibilidad posterior del archivo, plantilla, límites o estructura, manteniendo autorización vigente y consultas sin mutación. Trazabilidad: D-10.

#### Scenario: Cambios posteriores
- **WHEN** una carga ya CONFIRMADA pierde su archivo o cambia la plantilla, los límites o el esquema
- **THEN** la petición idéntica autorizada devuelve su resultado histórico sin reimportar

#### Scenario: Replay diferente
- **WHEN** se repite el ID confirmado con hashes distintos
- **THEN** se rechaza el contenido diferente sin repetir ni modificar el resultado

#### Scenario: Consulta en curso o ausente
- **WHEN** se consulta un ID propio durante actividad o no se observa un registro concluyente
- **THEN** no se modifica ni repara la carga y no se asegura que pueda repetirse sin consultar de nuevo

### Requirement: RQ-11 — Elegibilidad física antes de escribir

El sistema SHALL rechazar escrituras en destinos cuya estructura, motor, unicidad o efectos relacionados impidan acreditar la atomicidad y el ámbito de la carga. Trazabilidad: D-11.

#### Scenario: Motor o clave incompatibles
- **WHEN** falta InnoDB, NOT NULL o índice UNIQUE completo
- **THEN** se rechaza antes de cualquier mutación del destino

#### Scenario: Efectos relacionados
- **WHEN** el destino tiene triggers o participa en claves foráneas
- **THEN** se bloquea la confirmación sin desactivar ni modificar esas estructuras

#### Scenario: Metadatos no acreditados
- **WHEN** no se puede inspeccionar de manera confiable la estructura o sus efectos
- **THEN** la capacidad de escribir se mantiene cerrada y se informa el diagnóstico

### Requirement: RQ-12 — Límites y recuperación de ejecución

El sistema SHALL respetar los límites vigentes y permitir recuperar una interrupción mediante el ID de carga sin publicar avances parciales como si fueran una importación completada. Trazabilidad: D-12.

#### Scenario: Frontera de filas
- **WHEN** se intenta confirmar una carga en el límite y otra que lo supera
- **THEN** se admite la primera válida y se rechaza la segunda antes de escribir

#### Scenario: Límite modificado o tamaño excedido
- **WHEN** el límite actual difiere del snapshot o el archivo excede el máximo vigente
- **THEN** se exige corregir la intención antes de escribir sin alterar silenciosamente los snapshots

#### Scenario: Interrupción del cliente
- **WHEN** se corta la red o se cierra la pestaña durante la confirmación
- **THEN** se conserva el ID para consulta y no se asegura pausa, cancelación ni repetición automática

### Requirement: RQ-13 — Instalación y SQL manual

El sistema SHALL reutilizar el esquema compatible existente y mantener cualquier instalación o adecuación adicional como archivos SQL revisables para ejecución manual. Trazabilidad: D-13.

#### Scenario: Esquema existente compatible
- **WHEN** las 22 columnas y recursos acreditados siguen disponibles
- **THEN** se usan los campos de resultado existentes sin migración automática

#### Scenario: Adecuación requerida
- **WHEN** el diagnóstico identifica una diferencia de estructura, índice o permisos
- **THEN** se entrega un nuevo SQL y la ejecución dependiente espera su aplicación manual y verificación SELECT

### Requirement: RQ-14 — E2E existente y autorización de efectos

El sistema SHALL acreditar el recorrido real de confirmación con las E2E y recursos existentes, diferenciando pruebas locales de ejecución real y exigiendo autorización específica para sus escrituras. Trazabilidad: D-14.

#### Scenario: Reutilización del recorrido
- **WHEN** se prepara la suite de confirmación
- **THEN** usa autenticación, recursos DA, verificadores y utilidades actuales sin infraestructura paralela

#### Scenario: Scope de sólo lectura
- **WHEN** se presenta un scope DOC124 o uno sin autorización de efectos DOC125
- **THEN** la prueba no abre una ventana escritora ni confirma cargas

#### Scenario: Matriz real autorizada
- **WHEN** se ejecuta con ambiente, cuentas y destino descartable expresamente autorizados
- **THEN** se acreditan los tres modos, rollback, replay, respuesta perdida y carrera preservando destinos ajenos y evidencia de los efectos

### Requirement: RQ-15 — Compatibilidad y entrega verificable

El sistema SHALL entregar confirmación compatible con los contratos y consumidores existentes, con compilación y evidencia que indiquen alcance, ambiente y límites reales. Trazabilidad: D-15.

#### Scenario: Compatibilidad local
- **WHEN** se compila y ejecutan los harness afectados y su regresión
- **THEN** se conservan framework, conectores, recepción/preparación y consumidores existentes

#### Scenario: Fase de confirmación
- **WHEN** se entrega DOC125
- **THEN** no se atribuye interfaz, CSV, publicación de fuentes ni carga real sin la evidencia correspondiente
