# Especificación candidata — recuperación V2 de EN_CURSO

Estado:protocolo interno y operación HTTP de recuperación implementados hasta RV-06 bajo RQ-11..RQ-16, con evidencia local separada por entrega. No se desplegó ni se calificó la ejecución real; el almacenamiento habitual conserva V1 y D-07/RQ-07 mantienen sus operaciones. Los escenarios RC no son resultados reales de integración por sí mismos y69 permanece inelegible.

## RC-01 — Elegibilidad por evidencia del servidor

La recuperación propuesta SHALL admitir únicamente un intento ALMACENAR con protocolo V2 registrado por el servidor desde su inicio. SHALL exigir coincidencia de propietario, ámbito, nivel, recurso, snapshot, recepción e identidad del proveedor. SHALL rechazar V1, versión ausente, pruebas incompletas y cualquier backfill de elegibilidad.

### Intento histórico o69

- WHEN se solicita recuperar69 o un intento sin evidencia V2
- THEN conserva estado y reclamación, sin almacenamiento, confirmación, cierre negativo o conversión a INCIERTO.

### Prueba aportada por el cliente

- WHEN una solicitud intenta suministrar actor, PID, ausencia física, versión o resultado como prueba de elegibilidad
- THEN rechaza esos datos como autoridad; el servidor obtiene su propia vista y capacidad.

## RC-02 — Exclusión comprobable del ejecutor

El adaptador V2 SHALL conservar el original desde antes del inicio durable hasta terminar su salida/checkpoint. La recuperación SHALL conservar una guardia exclusiva del mismo recurso físico mientras decide y confirma. Un lector compartido, lock breve, edad, cierre de cliente o certificado de depurador no SHALL sustituirla.

### Ejecutor vivo o pausado

- WHEN el ejecutor conserva su retención o el proveedor no puede acreditar la exclusión entre todos los ejecutores admitidos
- THEN la recuperación no modifica datos y devuelve bloqueo/no elegible.

### Pérdida de guardia o recurso diferente

- WHEN falta el original, cambia su identidad/hash o se pierde la guardia antes del commit
- THEN no confirma ninguna recuperación; no crea ni reemplaza archivos para continuar.

## RC-03 — Diario previo y posterior a la invocación

V2 SHALL exigir una autorización durable, única y vinculada al intento antes de invocar. Documento del intento y evento de autorización SHALL cambiar en una transacción; discrepancias entre ambos SHALL bloquear la recuperación. Ante respuesta incierta de esa autorización SHALL no invocar. SHALL intentar registrar una observación tipada al salir; no SHALL inferir un retorno que no quedó registrado ni prometer su persistencia ante terminación abrupta.

### Autorización sin retorno

- WHEN existe autorización de invocación pero falta retorno fiable o IDs completos
- THEN conserva el bloqueo aunque las consultas físicas devuelvan cero.

### Diario contradictorio

- WHEN hay duplicados, un retorno sin autorización o discrepancias de token/recurso/contexto
- THEN rechaza la recuperación y conserva la evidencia existente.

## RC-04 — Cierre explícito sin invocación

Con V2, guardia acreditada, prueba original coherente de preparación, ausencia de autorización/retorno/resultado y ausencia física concordante, la recuperación SHALL poder cerrar el intento como RECHAZADO con `EJECUCION_CERRADA_SIN_INVOCACION`. La ausencia aislada de un evento no SHALL probar no invocación. SHALL mantener PDF/recepción, expediente, valores e inventario y retirar solo el marcador del mismo intento bajo revisión exacta.

### Cierre no documental

- WHEN se confirma ese cierre
- THEN no devuelve DocumentoId/InventarioId ni afirma documento confirmado; no encadena ALMACENAR.

### Detención o intento posterior

- WHEN hay detención vigente o el recurso ya apunta a otro intento
- THEN no cierra ni borra el marcador actual. Un replay de un cierre previo solo devuelve su historial.

## RC-05 — Confirmación de una identidad observada

Con V2, guardia acreditada, autorización y retorno válidos con IDs completos, la recuperación SHALL verificar esos mismos IDs con el catálogo existente. SHALL confirmar resultado, punteros y consolidación en una transacción, sin volver a invocar el almacenamiento.

### Efecto verificado sin checkpoint final

- WHEN el retorno del intento conserva IDs completos y la verificación de autoridad, clasificación, valores, gabinete/inventario/índice es positiva
- THEN confirma exactamente esos IDs y conserva señal de detención e inventario original.

### Coincidencia encontrada por búsqueda

- WHEN una búsqueda encuentra una fila por nombre o propietario pero no hay IDs registrados por ese intento
- THEN no atribuye el efecto ni confirma por coincidencia.

## RC-06 — Comparación final e historial

La recuperación SHALL reautorizar y comparar vista, revisiones, token/hash y diario bajo el orden de locks existente. El resultado, evento y consolidación SHALL ser atómicos. Una respuesta perdida SHALL permitir consultar el historial sin repetir escrituras o efectos.

### Cambio entre verificación y commit

- WHEN cambian permiso, destino, snapshot, revisión, marcador o evidencia
- THEN no guarda una recuperación contra la vista anterior.

### Repetición confirmada

- WHEN se repite una recuperación ya persistida por el mismo intento autorizado
- THEN devuelve su resultado y no afecta un nuevo intento del recurso.

## RC-07 — Compatibilidad y límites

La extensión SHALL reutilizar los componentes y tablas actuales y no alterar almacenamiento compartido/SII, formato PDF, permisos, cuotas o workers. V1 SHALL conservar su conducta. El soporte de un proveedor se debe acreditar; si falta, SHALL mantener el bloqueo. Las pruebas locales y sintéticas no SHALL autorizar despliegue o ejecución real.
