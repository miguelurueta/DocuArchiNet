# Despliegue gradual y rollback DOC-105

DOC-105 no activa producción. Este procedimiento solo puede ejecutarse con autorización adicional del ambiente y ventana correspondiente.

## Precondiciones

1. OpenSpec y suites locales aprobadas.
2. Matriz transversal sin regresiones; filas no ejecutadas declaradas `BLOCKED`.
3. Paquete desplegable identificado por commit, sin documentos ni perfiles runtime.
4. Copia recuperable de los archivos reemplazados y configuración vigente.
5. Responsable y ventana de rollback confirmados.

## Orden de promoción

1. Componentes servidor de streaming de DOC-102: referencia autorizada, estrategia y escritor HTTP.
2. `workflow/Handler_image_wf.ashx.vb` con contrato `200|206`, validadores y caché privada.
3. Constructores del visor enriquecido en Docuarchi, Workflow y Producción.
4. Preparador DOC-105 desactivado; validar directorio de caché, permisos, timeout y licencia/distribución del linealizador.
5. Activar globalmente `PdfLinearizedDerivativeEnabled` y observar primero `STREAM`, luego `RANGE`; no usar gates por usuario/grupo.
6. Activos PDF.js de inicio, scheduler y carga selectiva DOC-100/DOC-101.
7. Cliente de guardado único DOC-103.
8. Procesamiento servidor transaccional DOC-104.

No se habilita `WorkflowCentroTrabajoModernActive`, ni listas de usuarios o grupos. La experiencia moderna permanece oficial y los fallbacks existentes se conservan por contrato del consumidor, no como gate humano.

## Observación por etapa

- Errores JavaScript clasificados y recursos requeridos.
- Estados HTTP del handler y modo `RANGE|STREAM` esperado.
- Primer canvas, tareas largas, scroll y zoom frente al presupuesto aprobado.
- Bytes únicos y físicos; un `200 STREAM` no se presenta como parcial.
- Permisos y registros normales de visualización.
- Conteo de POST de grafo igual a uno cuando la mutación esté autorizada.
- Ausencia de temporales compartidos y de solicitudes duplicadas.

## Señales de detención inmediata

- Error JavaScript requerido o no clasificado.
- Cambio de autorización o exposición de un documento sin permiso.
- Segundo POST, doble efecto o inconsistencia del PDF.
- Scroll de 3.548 ms o más, zoom de 435 ms o más, o presupuesto final excedido.
- Respuesta `206` inválida, longitud total inconsistente o afirmación parcial sin rangos.
- Fallo en un consumidor que compartía componente con una etapa ya promovida.

## Rollback

1. Detener la promoción y conservar únicamente evidencia saneada.
2. Restaurar en orden inverso los archivos desplegados desde la copia del paquete anterior.
3. Restaurar configuración previa; no crear gates, usuarios o grupos temporales.
4. Reciclar el sitio solo por el canal operativo autorizado.
5. Ejecutar un smoke de solo lectura: login, apertura del mismo consumidor y visualización del PDF.
6. Confirmar que el handler, permisos y comportamiento visible corresponden a la versión anterior.
7. Registrar `PASSED` únicamente si el ensayo fue ejecutado; de lo contrario registrar `BLOCKED`.

La reversión inmediata de la optimización DOC-105 consiste en establecer `PdfLinearizedDerivativeEnabled=false` y reciclar por el canal autorizado. Los derivados son temporales y pueden limpiarse después de expirar; nunca se reemplaza ni elimina el PDF almacenado.

La limpieza se realiza únicamente sobre la caché temporal configurada para derivados y vistas previas, después de desactivar la preparación y confirmar que no hay una generación activa. No se borra por patrón fuera de esa raíz, no se toca el documento original y una ausencia de permisos de limpieza no impide el fallback seguro a `STREAM`. El ensayo operativo y su smoke continúan `BLOCKED` hasta contar con una ventana de rollback expresamente autorizada; esta documentación no se presenta como ejecución.

## Reversión de datos

No se esperan migraciones ni cambios de almacenamiento. El guardado de grafo autorizado modifica exclusivamente el PDF descartable seleccionado y no se usa como mecanismo de rollback. Las consultas de control permanecen `SELECT` sobre `workflowdocument`.
