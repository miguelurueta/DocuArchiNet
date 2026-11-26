# Contexto común de implementación de radicación interna

Esta serie prepara la implementación operativa de la capacidad explorada: radicar desde Producción o desde carga directa, representar usuarios internos o terceros y aplicar trámites sin seguimiento, con confirmación o con respuesta formal. Redactar los prompts no ejecuta la implementación ni activa configuración del ambiente.

## Insumos y aprobaciones

Leer [diagnóstico](../diagnostico.md), [carga directa](../exploracion-carga-directa.md), [trámites con respuesta](../exploracion-tramites-con-respuesta.md), [mapa de funciones](../mapa-funciones-implementacion.md) y [guía](00-guia-ejecucion.md). La base revisada es `226d2000`; comprobar cambios posteriores antes de editar.

- D-RI-01 aprobada: usuario interno o tercero como remitente; el actor autenticado permanece como ejecutor y auditoría. El responsable Workflow es una identidad independiente.
- D-RI-02 aprobada: formulario compacto de `23464d6b`. Las variaciones solicitadas posteriormente están en `maqueta.html`: terceros, carga directa y tres modalidades de respuesta. Sus datos, límites y asociaciones son ficticios; no son una configuración aprobada del ambiente.
- Se conserva ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, MySQL y los contratos vigentes. No migrar a ASP.NET Core/React ni trasladar esta tarea a otros repositorios por conveniencia.
- Esta serie no es DOC-127 ni la modernización del árbol. No alterar sus tareas, tickets o evidencia para alojar el nuevo trabajo.

Al recibir la instrucción de ejecutar un paquete, refinar sus decisiones y aplicar el alcance autorizado. No pedir nuevamente permisos o acuerdos ya registrados. Resolver elecciones técnicas dentro del alcance con evidencia; consultar solamente las decisiones funcionales pendientes que afecten a esa parte. Su ausencia no impide avanzar con trabajo independiente. Una opción recomendada aquí no se transforma en aprobación por silencio.

## Requisitos de la capacidad

| Requisito | Resultado observable |
|---|---|
| RQ-RI-01 | Desde Producción se conserva principal/anexos, expediente, plantilla y autoridad; principal no se cuenta dos veces. |
| RQ-RI-02 | Entrada independiente autorizada permite recibir archivos reales, verificar su recepción y confirmar sin visitar Producción. |
| RQ-RI-03 | Remitente interno/tercero se resuelve con tipo e identidad; actor y responsable conservan su función. |
| RQ-RI-04 | Plantillas dinámicas preservan campos, tipos, catálogos, scripts, obligatoriedad, fechas y valores tras postback. |
| RQ-RI-05 | Trámite, flujo, actividad, grupo y responsable son coherentes y revalidados en servidor. |
| RQ-RI-06 | Modalidad de respuesta y vencimiento proceden de configuración autorizada; seguimiento tiene propietario correcto. |
| RQ-RI-07 | Una confirmación repetida o incierta no vuelve a crear el mismo radicado ni sus documentos/seguimiento. |
| RQ-RI-08 | Resultados distinguen rechazo, registro confirmado, preparación/asignación/notificación pendiente e incertidumbre. |
| RQ-RI-09 | Lecturas, uploads, cambios, descarga y recuperación verifican actor, ámbito, operación y recurso en cada petición. |
| RQ-RI-10 | Interfaz accesible y compacta muestra recepción real, validaciones, resumen y cierre correcto sin fingir éxito. |
| RQ-RI-11 | Se preservan consumidores de Producción, Radicación clásica/Simplificada y Workflow; no cambia indiscriminadamente su política. |
| RQ-RI-12 | Compilación, esquema, configuración, publicación y pruebas integradas acreditan un recorrido real autorizado. |

## Decisiones pendientes y paquetes afectados

| Puerta | Origen | Definición pendiente | Paquetes |
|---|---|---|---|
| G01 | Q-CD-01, Q-CD-06 | Expediente antes de confirmar y registros internos de Producción, o clasificación posterior con almacenamiento independiente. | RI00, RI02, RI06, RI07, RI10 |
| G02 | Q-RI-01/02 | Catálogo de terceros, ámbito y tipos admitidos. | RI04, RI10 |
| G03 | Q-RI-03, Q-CD-02 | Autoridad de representación, carga y radicación; acceso y plantillas permitidas. | RI02, RI04, RI05, RI10 |
| G04 | Q-RI-04 | Solo terceros existentes o creación/edición en este recorrido. Propuesta inicial: seleccionar existentes. | RI04, RI10 |
| G05 | Q-RI-05 | Datos del remitente en impresión, consultas y notificación; destinatario de la respuesta. | RI04, RI08, RI09 |
| G06 | Q-CD-03 | Tipos, límites por archivo/total/cantidad, verificación de contenido y folios. Los valores de la maqueta no deciden esta política. | RI05, RI06, RI10 |
| G07 | Q-CD-04 | Orígenes separados o mezcla en una operación. Propuesta inicial: separados. | RI02, RI05, RI06, RI10 |
| G08 | Q-CD-05 | Conservación/reanudación del borrador, caducidad y recuperación. | RI02, RI05, RI08 |
| G09 | Exploración de respuesta | Respuesta formal mediante radicado saliente existente u otra respuesta interna. | RI03, RI09, RI10 |
| G10 | Exploración de respuesta | Trámites elegibles, homologación y configuración por plantilla, sin retirar los clásicos inadvertidamente. | RI03, RI09 |
| G11 | Exploración de respuesta | Política de transferir/cerrar según operación y propietario; valor funcional de cero días y edición del plazo. | RI03, RI09, RI10 |

Cerrar estas decisiones al abrir el paquete que las consume; no exigirlas todas para empezar el refinamiento. G01 tiene dos alternativas completas: si se elige expediente previo, preparar temporales y materializar registros válidos al confirmar; si se admite clasificación posterior, adaptar relaciones/preparación/almacenamiento para no fabricar un ID de Producción. La segunda alternativa necesita su contrato específico en RI00 y su implementación en RI06/RI07: no se considera satisfecha pasando cero a la función antigua.

## Arquitectura y contratos propuestos

Los nombres nuevos del mapa son **propuestas**, no componentes existentes ni firmas ya aprobadas. RI00 fija nombres, tipos, rutas y contrato v1; luego cada paquete actualiza esa versión sin romper consumidores. Usar límites tipados entre presentación, aplicación y persistencia. Session y controles WebForms pertenecen a sus adaptadores; la lógica nueva no recibe `Page`, `UpdatePanel` ni rutas arbitrarias del navegador.

Durante RI00 crear `implementacion/contratos-v1.md` con inventario de archivos/firmas, transportes, esquema y secuencias, y `implementacion/decisiones.md` con acuerdo, alternativa, fecha, paquetes afectados y estado de G01–G11. Registrar quién aporta la decisión sin inventar aprobaciones. Mantener esos archivos y la revisión del contrato durante las entregas; actualmente son salidas previstas, no documentos aprobados existentes.

La operación de radicación reúne origen, actor autenticado, ámbito, plantilla/configuración, remitente tipado, contexto documental, manifiesto versionado, datos, destino, reglas de respuesta y resultado. El cliente envía intención y referencias; servidor determina autoridad, rutas físicas, nombres de tablas, reglas y consecutivos. Validar formatos/rangos antes de convertir IDs a los tipos legacy; si se transportan como cadenas decimales, no convertirlos a `Number` de JavaScript sin comprobar precisión.

Operaciones públicas candidatas: abrir, consultar/configurar borrador, buscar remitentes/destinos, actualizar datos/selección, preparar confirmación, recibir/consultar/retirar archivo, confirmar, consultar resultado/seguimiento y cancelar. Publicar únicamente las necesarias. El upload binario necesita transporte propio; no reutilizar `ADJUNTARADICACION` ni `ADJUNTARADICACION_CLASICA` para un radicado inexistente. Conservar esos contratos.

El borrador y sus recepciones requieren referencias internas, revisión y almacenamiento durable suficiente para la política elegida. Antes de confirmar, revalidar actor, expediente/tipología cuando aplique, plantilla, remitente, destino y versión exacta del manifiesto. Cada archivo tiene rol explícito, referencia propia, estado y evidencia de recepción. El nombre del usuario o archivo no identifica una operación.

Estados de negocio propuestos: BORRADOR, RECIBIENDO, LISTO_PARA_CONFIRMAR, CONFIRMANDO, REGISTRADO, REGISTRADO_CON_PENDIENTES, RECHAZADO_ANTES_DE_REGISTRAR, RESULTADO_INCIERTO y CANCELADO_ANTES_DE_REGISTRAR. Registrar por separado las fases documental, Workflow, respuesta y notificación. RI00 concreta transiciones y códigos públicos; un HTTP 200 o un texto `YES` no demuestra todas las fases.

La idempotencia necesita una relación durable de operación/clave/versión con el radicado y una reclamación de ejecución atómica. La inserción del radicado, su obligación de respuesta y la confirmación de la operación deben compartir transacción cuando pertenezcan al mismo recurso transaccional. Las conexiones/gabinetes/archivos externos se tratan con fases y reconciliación. No envolver la llamada monolítica actual y asumir que se ha vuelto atómica.

El código actual usa `SELECT ... FOR UPDATE` al reservar consecutivos. Preservar ámbitos, formato, actualización y reglas de código general/corto/simple al extraer el núcleo. Ante resultado incierto, consultar evidencia durable antes de reintentar. La recuperación no llama otra vez al registro completo ni repite correo/Workflow sin verificar su efecto.

## Recorrido de integración

Desde Producción: selección explícita → apertura autorizada con referencias propias → formulario/plantilla → revisión → confirmación → resultado y actualización del padre.

Desde carga directa: acceso autorizado → borrador → recepción real de principal/anexos → datos y clasificación acordada → revisión del manifiesto → incorporación controlada → registro común → seguimiento/Workflow/notificación → consulta y recuperación.

La nueva entrada usa el mismo comportamiento de aplicación. No requiere visitar Producción, aunque G01 puede conservar sus registros internos. Los datos locales de la maqueta y sus temporizadores no pasan a producción. El seguimiento pendiente no se presenta como una respuesta ya elaborada. Cerrar un modal después del commit no cancela el registro.

## Fronteras de código

Rutas propuestas, a concretar en RI00: `Modelo/RadicacionInterna/` para reglas/puertos, `DTOs/RadicacionInterna/` para contratos, `Services/RadicacionInterna/` para casos de uso, `Infrastructure/Repositories/RadicacionInterna/` para SQL parametrizado y transacciones, e `Infrastructure/RadicacionInterna/` para Session/IO/adaptadores legacy. Transportes: `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`, `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Cliente: página existente, `js/radicacion/WebFormRadicacionEntranteInterna.js`, recursos nuevos justificados bajo `js/radicacion/` y `Styles/radicacion-interna.css`.

No crear capas vacías ni introducir un ORM o dependencia por llenar esta lista. Registrar archivos Compile/Content en `GestionDocumental-Docuarchi.net.vbproj` y verificar su publicación. El esquema durable necesita migración versionada en la ubicación acordada con el patrón real del repositorio; no modificar bases reales al generar el script ni tratar una tabla propuesta como existente.

## Verificación y entrega

Cada paquete incorpora pruebas de comportamiento de su responsabilidad, compilación MSBuild cuando cambie VB.NET/proyecto y su integración autorizada cuando corresponda. Las pruebas estáticas y la maqueta complementan; no sustituyen recepción, transacción, descarga ni respuesta reales. La evidencia futura se registra bajo `implementacion/RIxx-resultado.md`; esos archivos todavía no existen por redactar la serie.

Reutilizar `tools/e2e`, autenticación/configuración y runbook del repositorio. No crear login ni proyecto Playwright paralelo. Sin autorización de ambiente, cuentas y datos descartables, continuar pruebas aisladas y registrar pendiente la aceptación integrada afectada; no fingirla completada. No introducir `WorkflowCentroTrabajoModernActive` ni listas de usuarios/grupos como activación. Documentar cambios, comandos, resultados, límites y reversión aplicable; conservar avances ajenos en Git.
