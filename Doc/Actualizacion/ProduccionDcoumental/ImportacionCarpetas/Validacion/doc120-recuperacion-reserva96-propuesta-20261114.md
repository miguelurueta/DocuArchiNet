# DOC-120 — propuesta para retomar la reserva96

Estado: PROPUESTA_TECNICA_NO_IMPLEMENTADA_NO_AUTORIZADA. La corrección de fecha está compilada y probada localmente; **no recupera por sí sola el lote39**. El flujo actual no ofrece una reanudación de RESERVADO: Crear exige una reserva nueva, Verificar exige INCIERTO con evidencia, y Actualizar solo lee. No invocar ninguna de esas rutas como sustituto.

## Recursos conservados

Lote39/plan57, carpeta60, intento96 CREAR_EXPEDIENTE/RESERVADO, recepción95 CONFIRMADA y archivo86 RECIBIDO/NO_INICIADO. Detención solicitada posteriormente. Referencias observadas, no una autorización de efectos ni garantía de estado futuro. Mantener inventario, valores, reglas/snapshot y recibo; no crear otro lote, reenviar el PDF, almacenar documentos ni borrar staging. Las8 lecturas presupuestadas previas se conservan.

## Mecanismo propuesto para revisar e implementar

1. Añadir una acción explícita de reanudación mediante la fachada existente AvanzarLote, sin otro endpoint. La operación debe verificar actor/empresa/destino/permisos vigentes, lote/raíz/intento exactos y revisiones de lote y raíz mostradas al usuario. Pedir consentimiento explícito para levantar la detención y crear como máximo un expediente y su relación, conservando96. El servidor determina el intento activo; ninguna referencia del cliente concede autoridad.
2. Verificar antes de mutar el comando corregido, los catálogos actuales, barrera completa EXPEDIENTE/LOTE/parcial y staging recibido. Si aparece otra incompatibilidad, no levantar la detención ni tocar la reclamación. No inferir efecto ausente por antigüedad, lease o RESERVADO. Comprobar la procedencia del backend revisado y su orden durable de inicio antes del escritor.
3. Bajo bloqueos en orden lote→raíz→intento, exigir96 RESERVADO, activo de60, sin fin/evidencia/identidad y sin otras reclamaciones incompatibles; cotejar contexto/snapshot/hash y revisiones. Adquirir una nueva reclamación sobre **la misma fila96** mediante comparación de estado/token/revisiones, rotando el token y manteniendo solicitud_hash e inicio originales. Registrar actor y reanudación sin exponer tokens; levantar solo la detención observada por esa solicitud e incrementar revisiones. No fingir EsNuevaReserva sobre una lectura: la condición de nueva reclamación nace exclusivamente del cambio atómico confirmado.
4. Reutilizar el inicio durable, retenciones físicas, escritor y checkpoint existentes. La nueva reclamación impide al titular del token anterior iniciar: si aquel inicio ya ganó, la recuperación exige RESERVADO y debe fallar sin escribir; si la recuperación ganó, el inicio antiguo falla por token. La fase EN_CURSO nunca se revierte a RESERVADO. Una respuesta perdida o un resultado incierto conserva evidencia/reclamación y no autoriza otro escritor.
5. Al confirmar, consultar la identidad original, actualizar el árbol con la relación confirmada y mostrar Preparar índices para86. Índices y almacenamiento siguen siendo acciones posteriores separadas; esta propuesta no los ejecuta.

## Regresión previa a cualquier ejecución

Casos de usuario ajeno, permiso/destino cambiado, stop nuevo concurrente, revisión obsoleta, snapshot distinto, falta de staging, otro intento activo,96 ya EN_CURSO/CONFIRMADO/INCIERTO y dos solicitudes concurrentes. Probar que solo un token inicia una vez y que una respuesta perdida no duplica expediente/relación. Comprobar controles SELECT antes/después, recibo95/archivo86/reglas intactos y eventos de reanudación compatibles con la lectura/reconciliación existente. Este mecanismo necesita su desglose y revisión de implementación; el diseño no lo convierte en disponible.

## Ventana real propuesta tras implementar y revisar

Ambiente localhost previamente indicado por el usuario, cuenta propietaria autenticada y controles MySQL SELECT en consola enmascarada. Presupuesto propuesto:600 HTTP,2 sesiones,60 SELECT de control RA/DA; conservar8 intentos anteriores (máximo68 de control acumulados). Efectos máximos: una reclamación de recuperación de96, levantar la detención de39 observada y una creación de expediente/relación a través del escritor existente. Cero planes/lotes/recepciones/documentos almacenados/borrados nuevos. Los presupuestos y efectos **no están autorizados**.

No lanzar hasta contar con implementación verificable y autorización explícita de esa ventana. Si un preflight no cumple, detener sin cambios ni retry; si el resultado del escritor es incierto, conservarlo sin repetir. La aceptación6.4 sigue bloqueada hasta verificar la recuperación real y revisar los límites.
