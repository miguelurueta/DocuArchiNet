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

### Requirement: RQ-12 — Puerto V2 no destructivo y acotado

El proveedor SHALL cumplir D-12: ofrecer una retención y una guardia internas, no destructivas, sobre el mismo original existente de NTFS local. SHALL rechazar V1, recurso/hash/ámbito distintos, enlaces, original ausente y épocas de otro proceso/nodo o reinicio. SHALL conservar exclusión y handles durante su vida útil y permitir ejecutar una sección sincronizada sin que Dispose concurrente los libere. No SHALL crear originales, retirar archivos o decidir/persistir recuperaciones.

#### Scenario: Lector activo o guardia liberada
- **WHEN** otro proceso conserva el original abierto o se usa una guardia liberada
- **THEN** la adquisición o validación se rechaza sin cambiar contenido ni metadatos operativos.

#### Scenario: Nodo o instancia no acreditada
- **WHEN** la evidencia corresponde a otra época del proveedor o no se demuestra identidad local concordante
- **THEN** el puerto permanece bloqueado; un nombre de equipo o un hash igual no habilita recuperación.

### Requirement: RQ-13 — Adaptador V2 retenido y sin repetición

El adaptador SHALL cumplir D-13: conservar el original antes del inicio V2 y hasta salir. SHALL exigir acuses internos concordantes de inicio y autorización antes de llamar al escritor una sola vez. Una respuesta incierta no SHALL habilitar invocación ni fallback V1. SHALL intentar observar IDs en finally al salir del escritor, propagar cancelación/aborto y liberar la retención. La composición pública no SHALL activar V2 antes de disponer del repositorio transaccional RV-04.

#### Scenario: Autorización incierta o evidencia alterada
- **WHEN** falta un acuse de inicio/autorización, se pierde su respuesta o no coincide con el documento esperado
- **THEN** no se invoca el escritor ni se reintenta; se conserva incertidumbre y se libera retención al salir.

#### Scenario: Salida incierta o abortada del escritor
- **WHEN** el escritor deja IDs parciales o se interrumpe su llamada
- **THEN** se intenta registrar una observación del mismo intento sin inventar IDs; un aborto se propaga y nunca provoca una segunda invocación.

### Requirement: RQ-14 — Persistencia V2 y autorización de un solo uso

El repositorio SHALL cumplir D-14: usar las tablas y transacción existentes para persistir evidencia y diario V2 concordantes, comparando recurso, contexto, revisión, token y solicitud. SHALL emitir acuse solo después del commit y nunca reemitir una autorización ya consumida como nuevo permiso. SHALL conservar compatibilidad V1, autoridad e inventario; no SHALL habilitar backfill o migraciones.

#### Scenario: Respuesta perdida de autorización
- **WHEN** se persiste una autorización y su respuesta se pierde
- **THEN** otro intento de obtener autorización no recibe permiso de invocar ni provoca escrituras nuevas.

#### Scenario: Checkpoint o evento fallido
- **WHEN** falla una escritura, evento o consolidación durante el checkpoint
- **THEN** la transacción no confirma un resultado parcial; un replay confirmado solo devuelve la misma identidad y no vuelve a consolidar.

### Requirement: RQ-15 — Recuperación interna V2 sin escritor

El sistema SHALL cumplir D-15 mediante una operación interna sin puerto de almacenamiento. SHALL exigir inicio/diario V2 originales, autoridad, ámbito y vista concordantes y guardia viva hasta commit/rollback. V1, versión ausente o backfill no SHALL ser elegibles. Esta extensión interna no SHALL modificar la operación pública RQ-07 antes de RV-06.

#### Scenario: Preparación sin invocación
- **WHEN** V2 está PREPARADO sin autorización/retorno/resultado, no hay detención ni otra reclamación y el expediente carece de huellas documentales verificadas
- **THEN** puede cerrar únicamente ese intento como RECHAZADO/EJECUCION_CERRADA_SIN_INVOCACION, retirar su marcador y conservar recepción, original, expediente, valores e inventario; no confirma un documento ni encadena almacenamiento.

#### Scenario: Identidad observada verificable
- **WHEN** hay retorno V2 concordante con IDs completos y verificación física positiva por esos mismos IDs
- **THEN** puede confirmar esa identidad y consolidar atómicamente bajo guardia, conservando stop; no almacena de nuevo ni fuerza estados a INCIERTO.

#### Scenario: Vista cambiada o recuperación repetida
- **WHEN** cambian autoridad, revisión, evidencia, plan, diario o marcador antes de persistir, o se repite un cierre ya guardado
- **THEN** rechaza una escritura contra una vista obsoleta; un historial concordante se devuelve sin nuevos efectos ni tocar una reclamación posterior.

### Requirement: RQ-16 — Recuperación HTTP explícita y acotada

El sistema SHALL cumplir D-16: exponer RecuperarDocumentoV2 mediante el guard operativo existente y un DTO cerrado de cuatro referencias, sin pruebas ni capacidad aportadas por el cliente. SHALL exigir autoridad actual de Gestión/RECONCILIAR, contexto y orígenes RA/DA concordantes; SHALL conservar las restricciones V2 de RQ-15. No SHALL conectar un escritor, modificar V1 o habilitar recuperación retrospectiva.

#### Scenario: Entrada no autorizada o alterada
- **WHEN** falta sesión/permiso/CSRF, cambia origen/destino, el JSON incluye evidencia o IDs documentales, o las revisiones activas discrepan
- **THEN** rechaza la operación sin abrir guardia ni persistir recuperación; datos internos y excepciones no se exponen.

#### Scenario: Cierre de control y confirmación separados
- **WHEN** la recuperación devuelve un cierre sin invocación o una identidad confirmada
- **THEN** la respuesta diferencia explícitamente cierreSinInvocacion y documentoConfirmado; el cierre negativo no afirma documento ni inventario, y una repetición autorizada consulta historial sin nuevas escrituras.

### Requirement: RQ-17 — Enlace operativo V2 y driver cerrado

El sistema SHALL cumplir D-17:componer V2 para intentos documentales nuevos mediante los adaptadores y repositorio existentes, con revisión/contexto/snapshot obtenidos del servidor. SHALL conservar autoridad, rechazo ante cambios y retención/diario V2, sin modificar el escritor compartido ni convertir historiales V1. El driver existente SHALL admitir la recuperación explícita con solicitud/respuesta cerradas, correlación y cero reenvíos automáticos. No SHALL activar ejecución real o despliegue por esta implementación local.

#### Scenario: Lote de servidor concordante
- **WHEN** una petición nueva alcanza el enlace documental con lote vigente y original V2 admitido
- **THEN** el adaptador usa la revisión de servidor e inicio/gate/retorno/checkpoint V2, con una invocación máxima; cualquier discrepancia o falta de capacidad bloquea sin fallback V1.

#### Scenario: Historial V1 y escritor compartido preservados
- **WHEN** se consulta el mismo comando de un intento V1 existente o se revisan41/53/69
- **THEN** el coordinador conserva la vía histórica sin construir un escritor ni añadir evidencia V2; almacenamiento compartido y SII permanecen intactos.

#### Scenario: Driver de recuperación cerrado y sin replay
- **WHEN** el driver recibe referencias adicionales, respuesta discordante o transporte incierto, o está en readOnly
- **THEN** rechaza antes de efectos o de exponer la respuesta, no reintenta y distingue cierre de control de confirmación documental.

### Requirement: RQ-18 — Pérdida de acuse acotada en paquete de prueba

La preparación SHALL cumplir D-18:decorar solo los puertos de prueba con identidad/orden/revisión exactos y perder una única respuesta de INICIO/AUTORIZAR/OBSERVAR después de un acuse válido. SHALL conservar fallo/diario nativos y detener callbacks posteriores sin reintentos. El conductor SHALL usar driver/controles suministrados, presupuesto previo y evidencias concordantes, sin introducir login, endpoint de fallos o aceptar pruebas del cliente como autoridad. No SHALL incluir el decorador en el producto ni declarar aceptación real por pruebas locales.

#### Scenario: Fallo anterior al acuse o recurso distinto
- **WHEN** el callback nativo falla, el acuse es inválido o no coincide el comando/revisión/orden
- **THEN** no emite una pérdida atribuida a commit confirmado ni continúa callbacks; conserva bloqueo y cero reenvíos.

#### Scenario: Un acuse válido perdido
- **WHEN** el puerto nativo devuelve el acuse concordante del paso seleccionado
- **THEN** el decorador lo consume una sola vez y el adaptador conserva su semántica PREPARADO/AUTORIZADO/RETORNO_OBSERVADO, sin otro escritor o checkpoint inferido.

#### Scenario: Conductor sin aceptación implícita
- **WHEN** Q1/Q2/Q3 se coordinan con lectura/driver/enlace de prueba y presupuesto exterior
- **THEN** se verifica cierre, bloqueo o mismos IDs según la ventana; timeout, discrepancia o cierre incierto detienen sin replay del escritor y el resultado no acredita integración real.
