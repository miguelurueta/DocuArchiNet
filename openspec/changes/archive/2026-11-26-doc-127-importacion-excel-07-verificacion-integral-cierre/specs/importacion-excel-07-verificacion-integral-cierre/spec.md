<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12,D-13,D-14 -->
## Purpose

Acreditar la entrega integrada de la importación XLS/XLSX mediante evidencia trazable de autoridad, integridad, compatibilidad, recuperación y capacidad, con límites y condiciones de operación explícitos.

## ADDED Requirements

### Requirement: RQ-01 — Base y reutilización de evidencia

El sistema y su entrega de aceptación SHALL cumplir RQ-01 conforme a D-01: Fijar SHA, hashes normalizados de fuentes, hashes binarios del runtime, configuración efectiva y alcance por caso. Reutilizar resultados anteriores sólo con correspondencia de componente y condiciones; conservar diferencias y fechas.

#### Scenario: Resultado histórico compatible
- **WHEN** un caso previo mantiene sus componentes y precondiciones
- **THEN** el informe conserva su SHA y fecha y lo clasifica REAL_HISTORICA_REUTILIZADA sin anunciar otra ejecución

#### Scenario: Componente distinto
- **WHEN** una huella o condición relevante cambió
- **THEN** el caso pierde vigencia para ese componente y se selecciona su regresión afectada

#### Scenario: Declaración manual previa
- **WHEN** el usuario ya declaró crear plantilla e importar
- **THEN** se conserva esa declaración con su alcance, sin inventar mediciones ni solicitar repetirla para documentarla

### Requirement: RQ-02 — Infraestructura E2E compartida

El sistema y su entrega de aceptación SHALL cumplir RQ-02 conforme a D-02: Extender el runner DOC110, la suite Excel y los validadores DOC124–126. Crear alcances DOC127 cerrados por ventana; no reabrir scopes, IDs ni carreras consumidas.

#### Scenario: Ventana autorizada
- **WHEN** ambiente, cuentas y datos descartables están autorizados y el scope coincide con runtime y archivos
- **THEN** se ejecutan sólo sus casos y efectos permitidos con autenticación y controles existentes

#### Scenario: Ventana incompatible
- **WHEN** un scope venció, fue consumido o difiere de sus hashes y recursos
- **THEN** el runner detiene la ejecución antes de autenticar o escribir

#### Scenario: Separación administrativa
- **WHEN** se necesita instalar, sembrar, reparar o limpiar datos para un caso
- **THEN** la E2E no hace esa modificación y deriva a la entrega SQL manual

### Requirement: RQ-03 — Autoridad y preparación

El sistema y su entrega de aceptación SHALL cumplir RQ-03 conforme a D-03: Comprobar permiso exclusivo, denegado y revocado; empresa, propietario, CSRF y referencias. Preparar conserva tablas de negocio y configuración, aunque registra la ejecución y usa temporales propias.

#### Scenario: Permiso exclusivo y denegado
- **WHEN** se usan las identidades habilitada exclusivamente y restringida dentro de una ventana autorizada
- **THEN** menú y servicio admiten sólo la primera y los controles conservan datos y perfiles

#### Scenario: Revocación y referencias ajenas
- **WHEN** el permiso cambia o se manipula empresa, propietario, conexión, ID o CSRF
- **THEN** el servicio revalida y rechaza la operación; la matriz distingue prueba local y real

#### Scenario: Preparación sin escritura de negocio
- **WHEN** se recibe y prepara un archivo válido
- **THEN** las huellas completas de destinos y configuración se conservan y se enumeran los efectos técnicos autorizados

### Requirement: RQ-04 — Instalación y SQL manual

El sistema y su entrega de aceptación SHALL cumplir RQ-04 conforme a D-04: Revisar instalación existente sin reaplicar SQL00–27. Entregar un preflight DOC127 de SELECT y archivos nuevos específicos sólo para diferencias demostradas; el usuario aplica y confirma cada cambio antes del SELECT posterior.

#### Scenario: Instalación compatible
- **WHEN** los archivos ya confirmados y sus metadatos coinciden con el contrato
- **THEN** se reutiliza la instalación y se omite su reaplicación

#### Scenario: Diferencia de esquema o permiso
- **WHEN** el preflight detecta una diferencia que requiere modificación
- **THEN** se entrega otro archivo con precondiciones y destino y queda PENDIENTE_EJECUCION_MANUAL

#### Scenario: Confirmación de aplicación
- **WHEN** el usuario confirma archivo y aplicación en el destino acordado
- **THEN** se comprueba mediante SELECT y se conserva el resultado saneado antes del caso dependiente

### Requirement: RQ-05 — Formatos, plantillas y modos

El sistema y su entrega de aceptación SHALL cumplir RQ-05 conforme a D-05: Reutilizar el lector XLS/XLSX y las plantillas históricas/web. Verificar crear/agregar/reemplazar conservando revisión, mapeos y modo histórico; cada formato queda identificado.

#### Scenario: Plantilla histórica
- **WHEN** se consulta y usa una plantilla anterior con atributos compatibles
- **THEN** se conserva su interpretación de modo/formato y no se reescriben mapeos

#### Scenario: Plantilla creada en web
- **WHEN** el asistente analiza un Excel y el operador revisa y guarda la configuración
- **THEN** la plantilla y sus campos se validan con la misma autoridad y sirven para su modo autorizado

#### Scenario: Dos formatos y tres modos
- **WHEN** se ejecuta la matriz funcional con XLS y XLSX compatibles
- **THEN** se acredita CREAR, AGREGAR y REEMPLAZAR y se identifican los cruces no ejecutados

### Requirement: RQ-06 — Integridad del destino

El sistema y su entrega de aceptación SHALL cumplir RQ-06 conforme a D-06: Verificar claves requeridas y comparación efectiva MySQL según tipo/collation, índice UNIQUE completo, motor, política empresarial y compatibilidad de columnas. Agregar falla íntegramente ante duplicados; reemplazar conserva aislamiento.

#### Scenario: Clave nula, vacía o repetida
- **WHEN** la clave falta o hay equivalencia por la collation efectiva entre filas o con destino en AGREGAR
- **THEN** se rechaza la carga completa sin omitir ni actualizar filas

#### Scenario: Estructura incompatible
- **WHEN** el índice es parcial, falta aislamiento o existen motor/trigger/FK incompatibles
- **THEN** la operación falla antes del cambio funcional y registra el límite de evidencia

#### Scenario: Reemplazo autorizado
- **WHEN** el archivo es único y el destino cumple la política CONEXION o COLUMNA
- **THEN** se sustituye sólo el ámbito autorizado en una transacción DML

### Requirement: RQ-07 — Recuperación y concurrencia

El sistema y su entrega de aceptación SHALL cumplir RQ-07 conforme a D-07: Conservar confirmación por ID, consulta histórica, locks de carga/destino y marca CREATE. Diferenciar replay/acuse ignorado de cortes físicos de COMMIT/red; comprobar rollback y carrera con observación del escritor activo.

#### Scenario: Confirmación repetida o respuesta perdida
- **WHEN** el cliente desconoce el acuse o vuelve a confirmar el mismo ID e identidad
- **THEN** consulta o replay devuelve el resultado sin otra escritura y distingue el tipo de fallo realmente inducido

#### Scenario: CREATE recuperable
- **WHEN** la estructura propia permanece tras un fallo o la respuesta de creación es incierta
- **THEN** se inspecciona su marca/esquema y no se adopta ni elimina una tabla ajena

#### Scenario: Rollback y carrera
- **WHEN** falla REEMPLAZAR tras DELETE o compite otra sesión por la misma clave
- **THEN** se demuestra rollback íntegro o conflicto sin filas propias; el actor acredita INSERT esperando y locks antes de su commit

### Requirement: RQ-08 — Límites efectivos

El sistema y su entrega de aceptación SHALL cumplir RQ-08 conforme a D-08: Conservar MaxFilasDatos=20000 vigente y 10 MB decimales por defecto; el lector mantiene fallback10000. Cubrir 10000/10001 bajo configuración10000 y 20000/20001 bajo20000, además de 10000000/10000001 bytes y cambios entre fases.

#### Scenario: Fronteras de filas
- **WHEN** el servidor se configura localmente con10000 o20000 filas
- **THEN** acepta el máximo y rechaza máximo+1 sin contar el encabezado

#### Scenario: Frontera de bytes
- **WHEN** el archivo Excel válido tiene exactamente10000000 o10000001 bytes
- **THEN** se permite el primero por tamaño y se rechaza el segundo; se diferencia de formato y límites HTTP

#### Scenario: Configuración cambiante
- **WHEN** el límite efectivo cambia tras preparar y antes de confirmar
- **THEN** la revalidación detecta el conflicto sin escritura y se conservan consultas históricas confirmadas

### Requirement: RQ-09 — Capacidad medible

El sistema y su entrega de aceptación SHALL cumplir RQ-09 conforme a D-09: Completar seis celdas formato/tamaño: XLS y XLSX por1000/5000/10000, separadas por modo ejecutado. Medir preparación, confirmación y memoria con metodología, baseline, intervalo y cobertura; no extrapolar a20000.

#### Scenario: Matriz por formato y tamaño
- **WHEN** se programa capacidad con el usuario en recursos descartables
- **THEN** cada celda1000/5000/10000 por formato identifica crear/agregar/reemplazar ejecutados o faltantes y evidencia compatible reutilizada

#### Scenario: Medición acotada
- **WHEN** se ejecuta un recorrido específicamente autorizado
- **THEN** se guardan tiempos por etapa y memoria con método/intervalo/baseline y ventanas no observadas

#### Scenario: Saturación o presupuesto agotado
- **WHEN** una operación supera presupuesto o termina con resultado incierto
- **THEN** se detienen nuevas escrituras, se conserva el ID y se consulta el estado sin aumentar automáticamente timeouts, límites o reintentos

### Requirement: RQ-10 — Fuentes de índices y consumidores

El sistema y su entrega de aceptación SHALL cumplir RQ-10 conforme a D-10: Verificar la lectura existente por clave y la regresión de perfiles/proveedores, SII y carpetas. Importar una tabla no publica una fuente ni un proveedor: siguen su configuración/autorización propias. Entregar EXPLAIN para ejecución manual sobre la consulta real.

#### Scenario: Tabla como fuente elegible
- **WHEN** la tabla importada cumple metadatos, política y contrato de proveedor publicados
- **THEN** el consumidor autorizado compara la clave mediante igualdad y devuelve OK, SIN_COINCIDENCIA o AMBIGUO según cardinalidad

#### Scenario: Plan manual de consulta
- **WHEN** el operador ejecuta el EXPLAIN entregado para la consulta del proveedor
- **THEN** se revisa el índice y acceso efectivo con los metadatos SELECT; sin plan aportado no se certifica su rendimiento

#### Scenario: Regresión de consumidores
- **WHEN** se contrasta la huella del consumidor y su contrato con la evidencia anterior
- **THEN** se reutiliza la cobertura compatible y se ejecuta sólo la regresión afectada sin publicar fuentes automáticamente

### Requirement: RQ-11 — Interfaz vigente

El sistema y su entrega de aceptación SHALL cumplir RQ-11 conforme a D-11: Preservar semáforo, asistente, errores explicativos, recuperación por ID y foco/Cerrar/Escape. Distinguir positivos reales v5, lectura real v6 y ajuste v7 cubierto localmente; evaluar una lectura v7 sólo en ventana autorizada.

#### Scenario: Guía de acciones y diagnóstico
- **WHEN** la plantilla o archivo tiene una configuración incompatible
- **THEN** la interfaz señala campo/regla/corrección y habilita las acciones según sus prerrequisitos

#### Scenario: Cerrar durante consulta
- **WHEN** la apertura o consulta está en curso y se pulsa Escape o Cerrar
- **THEN** se cierra el diálogo manteniendo el foco y sin crear, preparar ni confirmar otra carga

#### Scenario: Corte de assets
- **WHEN** una prueba real usa una versión distinta a la publicada
- **THEN** el informe conserva los hashes originales y clasifica la regresión actual por su ejecución efectiva

### Requirement: RQ-12 — Build y correcciones focales

El sistema y su entrega de aceptación SHALL cumplir RQ-12 conforme a D-12: Compilar net461 con MSBuild en salida aislada y ejecutar los harness vigentes. Corregir únicamente brechas reproducibles con prueba de reproducción; vincular nueva evidencia a las huellas cambiadas sin desplegar automáticamente.

#### Scenario: Build aislado
- **WHEN** se valida el corte de aceptación
- **THEN** MSBuild registra framework, comando, SHA y errores/advertencias sin sustituir bin de IIS

#### Scenario: Suites existentes
- **WHEN** se ejecuta el harness nativo y la regresión UI/cliente/adaptadores
- **THEN** se distinguen checks nativos, tests Node y navegador sintético de E2E real

#### Scenario: Brecha reproducible
- **WHEN** un caso autorizado demuestra un fallo pertinente
- **THEN** se aplica la corrección mínima, se reproduce su resultado y se renueva la regresión afectada y su inventario de archivos

### Requirement: RQ-13 — Guía de operación y despliegue

El sistema y su entrega de aceptación SHALL cumplir RQ-13 conforme a D-13: Actualizar documentación existente con instalación RA/DA, confirmación manual, preflight, operación y recuperación. La carga confirma atómicamente; consultar/reintentar por ID no ofrece pausa por fila ni migración parcial.

#### Scenario: Instalación en otro ambiente
- **WHEN** el operador sigue la guía sobre RA/DA identificadas
- **THEN** aplica sólo archivos necesarios en orden, confirma y ejecuta la verificación sin secretos en entregables

#### Scenario: Interrupción de una carga
- **WHEN** el navegador se cierra o no conoce el resultado
- **THEN** el operador conserva ID/identidad, consulta y decide el reintento permitido sin asumir pausa o avance parcial

#### Scenario: Actualización y reversión
- **WHEN** se publica una corrección verificada
- **THEN** la guía identifica respaldo y restauración de código/configuración y trata el SQL estructural mediante un plan manual independiente

### Requirement: RQ-14 — Cierre verificable

El sistema y su entrega de aceptación SHALL cumplir RQ-14 conforme a D-14: Entregar matriz requisito-caso-prueba-SHA-ambiente-resultado, informe de capacidad y restricciones. Revisión, validación, publicación y archivo siguen OPSXJ por el SHA; los huecos críticos impiden cierre.

#### Scenario: Resultado limitado
- **WHEN** una comprobación no se ejecutó o sólo tiene cobertura local
- **THEN** se informa su estado, riesgo e impacto sin marcarla REAL_PASS

#### Scenario: Cierre técnico
- **WHEN** todas las tareas exigibles y brechas críticas están verificadas
- **THEN** se ejecutan gates por SHA y se prepara la entrega revisable para publicación/archivo conforme al flujo

#### Scenario: Bloqueo de aceptación
- **WHEN** falta validación autorizada, plan requerido o permanece una brecha crítica
- **THEN** el cambio sigue abierto con el motivo concreto y las tareas independientes continúan
