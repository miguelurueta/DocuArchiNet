## ADDED Requirements

### Requirement: RQ-01 — Procedencia y cambio mínimo

El sistema y su evidencia SHALL cumplir D-01: Reutilizar evidencia con su SHA y ambiente; modificar producto únicamente después de reproducir una brecha.

#### Scenario: Evidencia histórica conserva procedencia
- **WHEN** se reutiliza un REC o reporte DOC115/117
- **THEN** la matriz conserva su ruta, alcance, SHA y ambiente de origen y no lo presenta como corrida actual.

#### Scenario: Sin brecha reproducida no hay cambio preventivo
- **WHEN** el contraste muestra comportamiento existente compatible
- **THEN** el trabajo aporta evidencia y regresión local, sin crear otro motor, repositorio o endpoint.

### Requirement: RQ-02 — Exclusión documental por lote

El sistema y su evidencia SHALL cumplir D-02: Una petición ejecuta una fase sobre un recurso; toda reclamación documental activa o incierta impide otro escritor en el lote.

#### Scenario: Dos clientes sobre la misma solicitud
- **WHEN** dos sesiones autorizadas solicitan el mismo avance mientras la primera llamada documental sigue activa
- **THEN** la segunda devuelve historial o bloqueo seguro; no entra un segundo escritor ni aparece otra identidad documental.

#### Scenario: Otro comando o archivo en el mismo lote
- **WHEN** otra petición usa distinta clave o recurso mientras existe reclamación documental activa o incierta
- **THEN** la reserva no permite un segundo efecto documental del lote; la revisión obsoleta tampoco habilita ejecución.

### Requirement: RQ-03 — Barreras e inventario

El sistema y su evidencia SHALL cumplir D-03: Preservar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, sin reducir el inventario original.

#### Scenario: EXPEDIENTE con A lista y B por recibir
- **WHEN** A tiene sus entradas requeridas listas y B conserva entradas por recibir en modo completo EXPEDIENTE
- **THEN** A puede avanzar y B queda bloqueada sin alterar su inventario; el REC histórico acredita ese caso con su procedencia.

#### Scenario: LOTE exige todas las raíces
- **WHEN** el modo es completo LOTE y hay una entrada requerida sin recibir
- **THEN** ninguna raíz inicia almacenamiento hasta satisfacer la barrera global.

#### Scenario: Parcial preserva rechazos
- **WHEN** el modo es PARCIAL_PERMITIDO/NO_APLICA y hay un PDF elegible y una entrada TIFF rechazada
- **THEN** solo avanza el PDF elegible; el rechazo permanece contado en el inventario original y su resultado final.

### Requirement: RQ-04 — Antigüedad sin toma de posesión

El sistema y su evidencia SHALL cumplir D-04: No hay TTL documental habilitante: fecha antigua, desconexión o supuesta expiración no prueban ausencia de efecto ni liberan la reclamación.

#### Scenario: Fecha antigua durante llamada activa
- **WHEN** una reclamación tiene fecha antigua o el cliente la considera vencida y su llamada real sigue activa
- **THEN** la antigüedad no permite takeover, liberar marcadores ni volver a invocar el escritor.

#### Scenario: Incertidumbre conserva exclusión
- **WHEN** el resultado no puede demostrarse durablemente y existe reclamación INCIERTO o marcador activo
- **THEN** nuevos efectos quedan bloqueados hasta una verificación fiable por el contrato existente.

### Requirement: RQ-05 — Respuesta perdida y certeza durable

El sistema y su evidencia SHALL cumplir D-05: Repetir la misma solicitud consulta su resultado durable; efecto sin checkpoint conserva IDs observados y bloquea otra ejecución.

#### Scenario: Pérdida posterior al checkpoint
- **WHEN** la respuesta se pierde después del resultado durable confirmado y se repite la misma solicitud autorizada
- **THEN** se devuelve la misma identidad confirmada y el contador de entradas al escritor permanece uno.

#### Scenario: Efecto sin checkpoint
- **WHEN** la llamada produce un efecto conocido pero no se confirma su checkpoint
- **THEN** se conserva la identidad observada, se comunica incertidumbre y no se autoriza replay del escritor; EN_CURSO durable no se interpreta como fallo sin efecto.

### Requirement: RQ-06 — Detención y consulta posterior

El sistema y su evidencia SHALL cumplir D-06: La detención es una señal persistida; no cancela la llamada activa. Cerrar navegador no equivale a detener. Consultar lote no reanuda fases.

#### Scenario: Detención con llamada real activa
- **WHEN** una sesión autorizada solicita detención con revisión vigente durante ALMACENAR ya iniciado
- **THEN** la señal queda persistida, se conserva la reclamación y la llamada puede finalizar conservando efecto e IDs; ninguna fase nueva afectada comienza.

#### Scenario: Cerrar navegador y consultar desde otra sesión
- **WHEN** se cierra el navegador y otra sesión autorizada consulta el lote por su referencia
- **THEN** se observa su estado durable y snapshot sin asumir cancelación, detención implícita ni reanudación de fases.

### Requirement: RQ-07 — Recuperación existente y limitada

El sistema y su evidencia SHALL cumplir D-07: CerrarReservaSinInicio solo cierra una reserva ALMACENAR probada sin inicio y sin detención; ReconciliarRecurso verifica evidencia conocida de INCIERTO.

#### Scenario: Cierre preinicio probado
- **WHEN** CerrarReservaSinInicio recibe una reserva ALMACENAR/WEB_DOCUMENTO sin inicio, resultado ni evidencia, con vista exacta y sin detención
- **THEN** cierra únicamente esa reserva como RECHAZADO/RESERVA_CERRADA_SIN_INICIO y sin documento/inventario; la confirmación pública refiere al cierre.

#### Scenario: Recuperación no soportada permanece bloqueada
- **WHEN** el intento está EN_CURSO, hay detención activa para un cierre preinicio, o falta evidencia concordante para reconciliar
- **THEN** la operación no libera la reclamación ni fuerza estados por fecha; la evidencia registra el bloqueo y la vía permitida.

#### Scenario: Reconciliación con identidad conocida
- **WHEN** un intento INCIERTO tiene evidencia completa y concordante y autoridad vigente
- **THEN** el contrato existente verifica y persiste el efecto conocido sin almacenar, crear o incorporar de nuevo; los intentos históricos protegidos quedan excluidos.

### Requirement: RQ-08 — Autoridad vigente y snapshot

El sistema y su evidencia SHALL cumplir D-08: Cada consulta, avance, detención o verificación exige autoridad actual; una publicación posterior no reemplaza el snapshot del lote.

#### Scenario: Autoridad revocada o destino incompatible
- **WHEN** se solicita consultar, avanzar, detener o reconciliar con permiso revocado o contexto/destino incompatible
- **THEN** el contrato rechaza la operación sin exponer resultado ajeno ni iniciar nuevos efectos.

#### Scenario: Snapshot y revisión
- **WHEN** se publica otra versión después de confirmar el lote o se presenta una revisión obsoleta para una mutación nueva
- **THEN** la publicación no reemplaza el snapshot y la revisión obsoleta no inicia un efecto; el historial autorizado de una misma solicitud conserva su resultado.

### Requirement: RQ-09 — Consolidación exacta

El sistema y su evidencia SHALL cumplir D-09: Derivar estados del inventario original y checkpoints; conservar efectos e IDs confirmados aun con detención.

#### Scenario: Resultado desde inventario original
- **WHEN** se consolida un lote con raíces, archivos rechazados y checkpoints positivos
- **THEN** los conteos e identidades corresponden exactamente al inventario original y los efectos durables, sin eliminar rechazos ni duplicar documentos.

#### Scenario: Prioridad de actividad e incertidumbre
- **WHEN** se solicita detención y hay actividad o efecto incierto
- **THEN** no se declara completitud: la actividad mantiene EN_CURSO y la incertidumbre exige verificación; solo todos los efectos requeridos positivos permiten COMPLETADO aun con señal final conservada.

### Requirement: RQ-10 — Integración observable y autorizada

El sistema y su evidencia SHALL cumplir D-10: COORD-02/03 requieren prueba nueva de la llamada legacy activa, sin modificar el almacenamiento compartido ni heredar excepciones anteriores.

#### Scenario: Integración exige prueba dentro de llamada
- **WHEN** se pretende acreditar COORD-02 o COORD-03 con dos clientes, fallo o detención
- **THEN** la evidencia demuestra la llamada legacy real activa y el punto de coordinación, sin modificar almacenamiento compartido; pausas externas y superposición HTTP sola no bastan.

#### Scenario: Autorización específica antes de ejecutar
- **WHEN** se dispone de un plan compatible con ambiente, cuentas, recursos nuevos y límites definidos
- **THEN** se solicita su autorización explícita antes de autenticar, inyectar fallos o ejecutar; los controles son SELECT y la evidencia no contiene secretos.

### Requirement: RQ-11 — Documentos internos V2 sin capacidad de ejecución

El sistema SHALL cumplir D-11: disponer de contratos internos V2 para inicio, autorización y retorno observado con serialización cerrada y vínculos canónicos. SHALL rechazar versión/origen ausentes, V1, propiedades extra, datos incompletos y fases contradictorias. La validación de esos documentos no SHALL conceder autoridad, acreditar procedencia durable ni habilitar ejecución o recuperación; D-07/RQ-07 SHALL conservar su comportamiento.

#### Scenario: Documento válido no concede capacidad
- **WHEN** se construye o deserializa un documento V2 de forma válida, incluso si fue aportado por un cliente
- **THEN** solo se obtiene un dato interno validado; no se invoca almacenamiento, no se acredita proveedor ni se recupera ningún intento.

#### Scenario: Compatibilidad sin backfill
- **WHEN** se intenta leer evidencia V1, incompleta o contradictoria como V2
- **THEN** se rechaza sin añadir versión, origen, autorización o retorno por defecto; la serialización V1 y el intento69 permanecen intactos.
