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

### Tabla cerrada implementada, 2026-10-28

`CoordinadorImportacionCarpetas` contiene la legalidad de transiciones por dimensión, con comparación ordinal y listas devueltas como copias. Lo no enumerado, incluidos cambios a sí mismo, se rechaza con CONFLICTO_REVISION. Crear un registro en su estado inicial o devolver una repetición sin mutación son operaciones distintas de una transición. Una observación adicional puede conservar estado/reclamación y añadir auditoría; no necesita inventar una transición a sí mismo.

| Dimensión | Desde | Destinos enumerados |
|---|---|---|
| PLAN | REVISABLE | CONFIRMADO, EXPIRADO |
| PLAN | BLOQUEADO | EXPIRADO |
| PLAN | CONFIRMADO / EXPIRADO | Ninguno |
| RECEPCION | PENDIENTE | RECIBIENDO, RECHAZADO, CANCELADO |
| RECEPCION | RECIBIENDO | RECIBIDO, RECHAZADO, INCIERTO |
| RECEPCION | RECIBIDO | INCIERTO |
| RECEPCION | RECHAZADO | RECIBIENDO, CANCELADO |
| RECEPCION | INCIERTO | RECIBIDO, RECHAZADO |
| RECEPCION | CANCELADO | Ninguno |
| EFECTO | NO_INICIADO | EN_CURSO, RECHAZADO, CANCELADO |
| EFECTO | EN_CURSO | ALMACENADO, COMPLETADO, RECHAZADO, INCIERTO |
| EFECTO | ALMACENADO | EN_CURSO, COMPLETADO, INCIERTO |
| EFECTO | RECHAZADO | EN_CURSO |
| EFECTO | INCIERTO | ALMACENADO, COMPLETADO, RECHAZADO |
| EFECTO | COMPLETADO / CANCELADO | Ninguno |
| RAIZ | PENDIENTE | CREANDO, BLOQUEADO, CANCELADA |
| RAIZ | CREANDO | EXPEDIENTE_CONFIRMADO, INCIERTO, BLOQUEADO |
| RAIZ | EXPEDIENTE_CONFIRMADO | COMPLETADA, PARCIAL, INCIERTO |
| RAIZ | INCIERTO | EXPEDIENTE_CONFIRMADO, BLOQUEADO |
| RAIZ | BLOQUEADO | CREANDO, CANCELADA |
| RAIZ | PARCIAL | EXPEDIENTE_CONFIRMADO, COMPLETADA, INCIERTO |
| RAIZ | COMPLETADA / CANCELADA | Ninguno |
| INTENTO | RESERVADO | EN_CURSO, RECHAZADO, INCIERTO |
| INTENTO | EN_CURSO | CONFIRMADO, RECHAZADO, INCIERTO |
| INTENTO | INCIERTO | CONFIRMADO, RECHAZADO |
| INTENTO | CONFIRMADO / RECHAZADO | Ninguno |
| LOTE | RECIBIENDO | LISTO, EN_CURSO, PENDIENTE_VERIFICACION, DETENIDO, PARCIAL, FALLIDO |
| LOTE | LISTO | RECIBIENDO, EN_CURSO, PENDIENTE_VERIFICACION, DETENIDO, COMPLETADO, PARCIAL, FALLIDO |
| LOTE | EN_CURSO | RECIBIENDO, LISTO, PENDIENTE_VERIFICACION, DETENIDO, COMPLETADO, PARCIAL, FALLIDO |
| LOTE | PENDIENTE_VERIFICACION | RECIBIENDO, LISTO, DETENIDO, COMPLETADO, PARCIAL, FALLIDO |
| LOTE | DETENIDO | RECIBIENDO, LISTO, PENDIENTE_VERIFICACION, COMPLETADO, PARCIAL, FALLIDO |
| LOTE | PARCIAL | RECIBIENDO, LISTO, EN_CURSO, PENDIENTE_VERIFICACION, DETENIDO, COMPLETADO |
| LOTE | COMPLETADO / FALLIDO | Ninguno |

Actualización 2026-10-30, 10.8: LISTO → COMPLETADO permite consolidar los últimos efectos comprobados mediante reconciliación explícita. No inicia un efecto: exige todos los checkpoints positivos originales. La confirmación del intento, liberación de su reclamación y consolidación pertenecen a una misma transacción. La marca inmutable de los comandos de verificación impide utilizarlos en las rutas ordinarias de ejecución. Una reclamación antigua aún RESERVADO/EN_CURSO o sin IDs completos conserva su incertidumbre; la antigüedad no la libera. Evidencia local `Validacion/doc111-reconciliacion-local-20261030.md`; HTTP de esta capacidad sigue pendiente de 10.11.

La tabla **no concede autoridad ni prueba de efecto**. Toda transición necesita el recurso/revisión y las condiciones de su operación. En particular:

- INCIERTO solo sale con evidencia positiva o negativa inequívoca; no pasa directamente a una fase ejecutora por timeout, lease o nuevo token. Un intento terminal necesita nueva identidad para otra fase.
- RECIBIDO → INCIERTO corresponde a nueva verificación autorizada de contenido perdido/alterado, conservando evidencia histórica; no al fallo de respuesta de un commit positivo. El registro de fallos de recepción continúa sin degradar ese commit.
- ALMACENADO → EN_CURSO solo sirve a una fase pendiente compatible, con IDs conservados; nunca autoriza almacenar otra vez el documento.
- BLOQUEADO → CREANDO o RECHAZADO → EN_CURSO/RECIBIENDO requieren elegibilidad original y prueba de no efecto; no corrigen silenciosamente elementos ineligibles del plan congelado. Cambiar inventario o reglas requiere otro plan.
- DETENIDO → LISTO/RECIBIENDO requiere continuación explícita autorizada y señal de detención resuelta; esta tabla no implementa esa acción. Los agregados necesitan verificar sus condiciones/totales, no inferir éxito solo por la etiqueta previa.

La recepción existente aplica esta tabla antes de reservar, confirmar o registrar un cambio de estado de archivo/intento, manteniendo sus guards de recurso/token/evidencia y transacciones. La reclamación documental, ejecución, agregación y reconciliación del coordinador todavía no están implementadas por publicar la tabla.

## 3. Barrera de integridad

La barrera completa exige todos los contenidos recibidos/verificados, reglas resolubles y comprobaciones posibles antes de creación. Valores dependientes del ID del expediente validan su definición/fuente antes y su valor final después de crear. Si fallan, se conserva el expediente y se bloquea completar la unidad: no se prometió atomicidad.

| Política/ámbito | Condición para iniciar efectos | Si otra raíz tiene errores |
|---|---|---|
| COMPLETO_OBLIGATORIO + EXPEDIENTE | Todos los archivos previstos de esa raíz están recibidos y elegibles. | Avanza la raíz completa; la raíz con errores permanece bloqueada y visible. |
| COMPLETO_OBLIGATORIO + LOTE | Todos los archivos previstos de todas las raíces están recibidos y elegibles. | No avanza ninguna raíz hasta cumplir la barrera global. |
| PARCIAL_PERMITIDO | Procesar archivos elegibles/recibidos manteniendo rechazados; al menos uno elegible antes de crear raíz. | Errores individuales no bloquean automáticamente todo; errores globales sí. |

Ambos ámbitos están aprobados: implementar y probar ambas ramas en el mismo coordinador. En modo completo no publicar ámbito NULL ni asignar uno silenciosamente; el usuario del perfil elige EXPEDIENTE o LOTE. En parcial persistir NO_APLICA. La política queda fijada al confirmar el lote y no se modifica al reanudar.

### Agregación del lote

Implementación local del 2026-10-30: `CoordinadorImportacionCarpetas.Consolidacion.vb` y el parcial `.Consolidacion.vb` del repositorio único calculan/aplican estos agregados desde todos los originales y checkpoints terminales coherentes. COMPLETADA/COMPLETADO no se deduce solo de etiquetas; requiere identidad de creación, recepción, IDs/valores efectivos y resultado concordante con un intento final. Incertidumbre (incluidos marcadores/actividad huérfanos o IDs duplicados) precede a actividad, completitud y detención. Progreso confirmado con trabajo automático restante conserva EN_CURSO entre fases; rechazados y pendientes permanecen en los totales. Un INCIERTO no se reconcilia mediante esta agregación. Los cambios de estado/revisión y eventos se aplican en la misma transacción de checkpoint/detención; repetir sin cambios no escribe. Evidencia local `Validacion/doc111-consolidacion-local-20261030.md`; sin SQL/efectos reales. Las referencias históricas de este documento a exigir aislamiento adicional del XML quedan sustituidas por la decisión explícita del usuario del 2026-10-30; las funciones legacy permanecen intactas.

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

Precisión autorizada 2026-11-01: una reserva ALMACENAR que nunca pasó de RESERVADO puede cerrarse por acción explícita `CerrarReservaSinInicio`. El repositorio exige raíz confirmada, archivo recibido con valores fijados y reclamación exacta, ausencia de evidencia/fin/eventos de inicio o resultado, revisión vigente y autoridad. Bajo el mismo bloqueo de lote que el inicio, compara la vista, registra intento RECHAZADO con `RESERVA_CERRADA_SIN_INICIO`, libera solo el archivo a NO_INICIADO y aumenta revisiones con evento durable. El CAS y la revisión impiden el inicio del ejecutor anterior. No altera la tabla general para intentos EN_CURSO/INCIERTO ni usa timeout como prueba. Conserva originales, valores, expedientes e historial; repetir el cierre no libera una reserva nueva. El cierre no crea un documento ni autoriza replay del intento anterior.

Decisión aprobada 2026-10-20: publicar otra versión no altera lotes anteriores. Desactivar perfil impide nuevas selecciones y confirmaciones; lotes ya confirmados continúan con snapshot original y permisos vigentes. Detener lote es una acción explícita independiente; pérdida de autorización impide nuevos efectos afectados. Reactivar no reanuda automáticamente lotes detenidos. Contrato y carreras: [coincidencias/disponibilidad](06-coincidencias-disponibilidad.md).

Permisos diferenciados: administrar/publicar perfil, preparar/confirmar importación, recibir, ejecutar, consultar, reconciliar y mantenimiento. Resolverlos con el modelo existente, sin reutilizar indiscriminadamente permiso de gabinete. Pérdida de un permiso de ejecución no habilita consulta a un actor no autorizado; un operador autorizado debe poder investigar incertidumbre.

La reanudación requiere acceso al lote persistido. El navegador cerrado no mantiene lectura de archivos locales; solicitar nueva selección para pendientes de recepción. No permitir reemplazar contenido bajo identidad confirmada para resolver una validación sin nuevo plan.

## 8. Condiciones para aceptar adaptadores

`Registrar_Expediente_Conservacion`: comprobar límites, fechas/código, permisos externos, transacciones y efectos XML/relación; devuelve identidad real, pero no se ha demostrado correlación suficiente ante crash posterior.

`AlmacenamientoDocumentoProduccionDocumental`: recibe lista de campos pero usa sesión, obtiene nombre físico y borra archivos. Es candidato, no adaptador aprobado. No se modifica su implementación ni la de SII. Si no es posible aislar sin cambios prohibidos, la integración de esa fase queda bloqueada; el resto de entregas puede continuar contra contratos probados sin simular éxito productivo.

Corte local 2026-10-30, limpieza controlada (11.7): capacidad interna sin endpoint/job, deshabilitada por defecto y con retencion servidor positiva explicita. Solo lote completo con inventario/checkpoints concordantes y sin actividad/incertidumbre; fecha mas reciente de lote/finalizacion de intentos y SELECT actuales de documento/relaciones/valores. Reutiliza staging privado y registro PREPARAR_LIMPIEZA confirmado antes de retirar un .bin exacto; segunda transaccion revalida y registra conclusion, sin prometer atomicidad disco/MySQL. Respuesta perdida conserva diario para recuperar ausencia autorizada; nunca repite efecto documental ni borra metadatos/partes/historial. Revocacion, politica ampliada/deshabilitada, evidencia cambiante y original alterado/retenido impiden retiro. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-limpieza-local-20261030.md. Arnes Framework con PDF sintetico y SQL dobles, build Debug separado y coherencia PASS. 119/130; sin habilitar politica operativa ni limpiar datos del sistema, sin MySQL/HTTP/E2E reales. Funciones legacy/SII intactas; exclusion adicional del XML descartada por el usuario.
