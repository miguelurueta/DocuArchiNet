# Diseño 03 — Estados, coordinación y recuperación

Estado: diseño propuesto con decisión funcional aprobada: ambos ámbitos, EXPEDIENTE y LOTE, configurables en el perfil. Las dos ramas son parte del alcance requerido.

## 1. Invariantes

- Inventario original y versión/configuración quedan fijados al confirmar. Nueva selección de árbol o publicación no cambia el destino.
- Ningún archivo pasa a COMPLETADO sin almacenamiento e incorporación comprobados. Rechazados, cancelados e inciertos permanecen en denominadores e informe.
- No hay transacción global entre disco, gabinete, expediente e índice XML. Una unidad incompleta puede contener efectos confirmados que se conservan.
- ID de efecto conocido no basta: debe pertenecer a la operación, gabinete, expediente y nivel esperados.
- Un reintento no se decide por timeout. Se decide por estado persistido y evidencia del efecto previo.
- Un bloqueo propio no excluye automáticamente carga manual, SII ni otro proceso legacy. Reutilización depende de caracterización de recursos compartidos.

## 2. Estados por dimensión

| Entidad/dimensión | Estados | Transición autorizada |
|---|---|---|
| Plan | REVISABLE, BLOQUEADO, CONFIRMADO, EXPIRADO | Preparación fija evaluación; confirmar solo REVISABLE elegible; nuevo contenido produce nuevo plan. |
| Recepción archivo | PENDIENTE, RECIBIENDO, RECIBIDO, RECHAZADO, INCIERTO, CANCELADO | Reclamar → recibir → validar/confirmar. Fallo sin parte válida → rechazo; crash con parte/fila ambigua → incierto hasta reconciliar. |
| Efecto archivo | NO_INICIADO, EN_CURSO, ALMACENADO, COMPLETADO, RECHAZADO, INCIERTO, CANCELADO | Solo RECIBIDO habilitado avanza. ALMACENADO significa documento confirmado pero falta verificar/completar vínculo/índice. |
| Raíz | PENDIENTE, CREANDO, EXPEDIENTE_CONFIRMADO, INCIERTO, BLOQUEADO, COMPLETADA, PARCIAL, CANCELADA | Creación se reclama tras barrera; efecto incierto bloquea recreación; agregados derivan de sus archivos sin borrar identidad del expediente. |
| Intento | RESERVADO, EN_CURSO, CONFIRMADO, RECHAZADO, INCIERTO | Un intento terminal no se reutiliza; nueva fase/reintento seguro crea otro enlazado por recurso. |
| Lote | RECIBIENDO, LISTO, EN_CURSO, PENDIENTE_VERIFICACION, DETENIDO, COMPLETADO, PARCIAL, FALLIDO | Estado resumido con detalle por raíz/archivo; no sustituye capacidades ni filas individuales. |

`detencion_solicitada` es señal independiente del estado agregado. `PARCIAL_PERMITIDO` es una política, no lo mismo que resultado PARCIAL. En modo completo puede haber resultado parcial debido a efectos ya confirmados y un fallo posterior: no se denomina éxito completo.

Antes de implementar, materializar en pruebas una tabla de transiciones exhaustiva de los estados anteriores; transiciones no enumeradas se rechazan. Al reintentar una recepción rechazada, el contenido debe corresponder al mismo manifiesto; cambios de nombre/tamaño/configuración requieren nueva preparación, no editar silenciosamente el lote. Un fallo técnico puede reintentar la misma fase con otra identidad de intento si está demostrado que no produjo efecto.

## 3. Barrera de integridad

La barrera completa exige todos los contenidos recibidos/verificados, reglas resolubles y comprobaciones posibles antes de creación. Valores dependientes del ID del expediente validan su definición/fuente antes y su valor final después de crear. Si fallan, se conserva el expediente y se bloquea completar la unidad: no se prometió atomicidad.

| Política/ámbito | Condición para iniciar efectos | Si otra raíz tiene errores |
|---|---|---|
| COMPLETO_OBLIGATORIO + EXPEDIENTE | Todos los archivos previstos de esa raíz están recibidos y elegibles. | Avanza la raíz completa; la raíz con errores permanece bloqueada y visible. |
| COMPLETO_OBLIGATORIO + LOTE | Todos los archivos previstos de todas las raíces están recibidos y elegibles. | No avanza ninguna raíz hasta cumplir la barrera global. |
| PARCIAL_PERMITIDO | Procesar archivos elegibles/recibidos manteniendo rechazados; al menos uno elegible antes de crear raíz. | Errores individuales no bloquean automáticamente todo; errores globales sí. |

Ambos ámbitos están aprobados: implementar y probar ambas ramas en el mismo coordinador. En modo completo no publicar ámbito NULL ni asignar uno silenciosamente; el usuario del perfil elige EXPEDIENTE o LOTE. En parcial persistir NO_APLICA. La política queda fijada al confirmar el lote y no se modifica al reanudar.

### Agregación del lote

Regla conservadora: COMPLETADO solo si todas las raíces/archivos exigidos tienen efectos completos; PENDIENTE_VERIFICACION si existe incertidumbre; EN_CURSO si hay intento activo; DETENIDO si hay detención solicitada sin efecto activo y quedan pendientes; PARCIAL si hay éxitos y fallos/pendientes terminales; FALLIDO si no hay éxitos y no hay trabajo elegible/recuperable; RECIBIENDO/LISTO si todavía se está preparando trabajo. La precedencia concreta debe conservar indicadores separados `hayIntentosActivos`, `hayIncertidumbre`, `hayRechazados` y totales: una etiqueta sola no autoriza reintentar. Trabajo recuperable pendiente se mantiene visible aun si el resumen es parcial.

## 4. Ejecución acotada y exclusión

1. Autorizar lote y fase; revalidar recurso/destino/política.
2. Transacción corta: bloquear fila de recurso, comprobar revisión y `intento_activo_id`, insertar intento y fijar reclamación. Commit.
3. Revalidar precondiciones inmediatas e invocar una única fase compatible, sin transacción operativa abierta alrededor de conexiones legacy.
4. Transacción corta: registrar evidencia/IDs, comprobar token/estado, cerrar intento y actualizar recurso/agregados.
5. Responder con fase y acciones reales. Otra petición puede solicitar el siguiente avance.

La barrera de raíz debe coordinar creación una sola vez y no permitir que dos archivos intenten crearla independientemente. Antes de modificar un expediente/índice, caracterizar la exclusión de escritores externos; no inventar un lock local que esos escritores desconocen.

No usar expiración de lease como permiso automático para recuperar efectos. Si una petición antigua puede seguir corriendo, CAS en nuestra tabla no impide su escritura en legacy. Mantener INCIERTO o EN_CURSO hasta prueba de finalización/no ejecución y reconciliación. Sin mecanismo verificable, bloquear y requerir investigación autorizada.

## 5. Matriz de fallos

| Frontera | Evidencia / recuperación segura | Acción prohibida |
|---|---|---|
| Commit de lote / respuesta perdida | Consultar clave/plan único y devolver IDs previos. | Crear otro lote por timeout. |
| Parte de archivo / recepción interrumpida | No marcar recibido; aislar parte y permitir reintento seguro conforme cuotas. | Procesar parte truncada. |
| Temporal completo / fila no confirmada | Verificar referencia, pertenencia, tamaño/huella y registrar una sola recepción. | Asumir que cualquier archivo de la carpeta temporal corresponde al lote. |
| Fila recibida / staging ausente | Bloquear efectos, diagnosticar pérdida y recuperar el mismo contenido cuando sea posible. | Reconstruir bytes desde nombre o avanzar sin contenido. |
| Antes de invocar creación | Rechazo conocido si se demuestra que no se llamó; liberar reclamo con evidencia. | Inferir ausencia de llamada solo por no tener ID. |
| Expediente creado / registro de resultado perdido | Reconciliar identidad/relación y correlación comprobables. Sin evidencia suficiente: INCIERTO. | Buscar por nombre y recrear si la consulta no concluye. |
| Almacenado / proyección o respuesta fallida | Reconciliar ID/documento/relación; registrar fase conocida. | Reenviar el archivo automáticamente. |
| Documento confirmado / incorporación pendiente | Ejecutar solo fase pendiente compatible e idempotente; verificar después. | Volver a almacenar para obtener la relación. |
| Limpieza legacy / error posterior | Copia consumible puede faltar; conservar staging original y evidencia. | Concluir que documento no existe porque falta copia. |
| Lease vencida / llamada sigue activa | No tomar nuevo efecto; verificar finalización y estado real. | Dos ejecutores sobre el mismo efecto sin fencing legacy. |
| Nivel/permisos cambiados | Suspender nuevos efectos afectados; conservar evidencias y permitir consulta/verificación autorizadas. | Redirigir a otro nivel o eliminar resultados. |

## 6. Sesión y ciclo de archivos

```text
Contenido del dispositivo
 → parte de recepción (privada)
 → staging completo retenido (huella + referencia persistida)
 → copia de trabajo por intento (consumible por legacy)
 → documento almacenado / incorporación verificada
 → limpieza controlada de staging según retención
```

El adaptador puede leer identidad autenticada existente en la petición inicial; su comando conserva autoridad/destino. Si necesita variables de sesión, caracterizarlas, aislar/restaurar solo las necesarias y probar concurrencia. No tocar selección global para simular navegación. Restore no revierte borrado de archivos ni reemplaza aislamiento.

Nombre original y extensión deben conservarse por contrato soportado. La función observada usa `FileInfo.Name`; una copia en directorio único con nombre seguro solo es admisible tras validar caracteres, longitud y correspondencia. Nunca concatenar ruta relativa no confiable. Si preservar nombre exacto es imposible sin alterar legacy, registrar el bloqueo o una política de nombre explícitamente aprobada, no renombrar silenciosamente.

## 7. Publicación, permisos y reanudación

Decisión aprobada 2026-10-20: publicar otra versión no altera lotes anteriores. Desactivar perfil impide nuevas selecciones y confirmaciones; lotes ya confirmados continúan con snapshot original y permisos vigentes. Detener lote es una acción explícita independiente; pérdida de autorización impide nuevos efectos afectados. Reactivar no reanuda automáticamente lotes detenidos. Contrato y carreras: [coincidencias/disponibilidad](06-coincidencias-disponibilidad.md).

Permisos diferenciados: administrar/publicar perfil, preparar/confirmar importación, recibir, ejecutar, consultar, reconciliar y mantenimiento. Resolverlos con el modelo existente, sin reutilizar indiscriminadamente permiso de gabinete. Pérdida de un permiso de ejecución no habilita consulta a un actor no autorizado; un operador autorizado debe poder investigar incertidumbre.

La reanudación requiere acceso al lote persistido. El navegador cerrado no mantiene lectura de archivos locales; solicitar nueva selección para pendientes de recepción. No permitir reemplazar contenido bajo identidad confirmada para resolver una validación sin nuevo plan.

## 8. Condiciones para aceptar adaptadores

`Registrar_Expediente_Conservacion`: comprobar límites, fechas/código, permisos externos, transacciones y efectos XML/relación; devuelve identidad real, pero no se ha demostrado correlación suficiente ante crash posterior.

`AlmacenamientoDocumentoProduccionDocumental`: recibe lista de campos pero usa sesión, obtiene nombre físico y borra archivos. Es candidato, no adaptador aprobado. No se modifica su implementación ni la de SII. Si no es posible aislar sin cambios prohibidos, la integración de esa fase queda bloqueada; el resto de entregas puede continuar contra contratos probados sin simular éxito productivo.
