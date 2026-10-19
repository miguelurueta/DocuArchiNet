# Prompt 11 — Seguimiento, reanudación y operación

## Encargo

Lee guía 00, prompt 03, diseño 04 y componentes reales 05/07/10. Conecta la experiencia de resultados y las tareas operativas al estado persistido. No reconstruir resultados desde la memoria del navegador ni implementar otro coordinador.

## Trabajo

Extender los componentes de la ventana 05; no reemplazarlos ni duplicar sus etapas. Los comandos de continuar/cancelar/reconciliar llaman a 10, mientras las consultas/CSV pertenecen a esta entrega. Separar staging retenido de las copias consumibles de 09; la retención nunca presupone que un método legacy conserva el archivo entregado.

- Consultar lotes/resultados con autorización por usuario/ámbito y recurso. Definir paginación, filtros por raíz/estado y totales estables. Mostrar política, nivel, versión, errores por campo, IDs confirmados y fases pendientes.
- Separar recibido de almacenado/incorporado. Porcentajes documentales usan archivos confirmados; incluir rechazados/cancelados/inciertos en el total original. Exportar resultados autorizados protegiendo fórmulas CSV y campos externos.
- Permitir recuperar un lote existente mediante su ID y permisos. Para archivos pendientes de recepción, pedir selección local y comprobar correspondencia; nombre/tamaño/fecha no prueban igualdad de contenido. No almacenar rutas absolutas del dispositivo para leerlas desde servidor.
- Acciones de continuar, cancelar y verificar deben derivarse de estados y capacidades del servicio. No ofrecer reintentar almacenamiento ante resultado incierto sin reconciliación. Explicar estados con mensajes útiles sin SQL, secretos ni rutas internas.
- Al cerrar la ventana, conservar identidad del lote confirmado; abrir una preparación nueva no lo mueve ni elimina. La recepción pendiente no continúa sin los archivos locales disponibles.
- Actualizar la rama del nivel de origen mediante mecanismos existentes, IDs de expediente/relación confirmados y deduplicación. Respetar la navegación actual; no seleccionar otro nodo automáticamente para mostrar un resultado.
- Definir retención de temporales e historial con parámetros aprobados. Implementar limpieza en seco/listado previo y eliminación acotada a rutas propias, sin tocar archivos documentales permanentes ni resultados inciertos. Evitar carreras con recepción/procesamiento activos; registrar resultado y reintento de limpieza.
- Reutilizar mecanismo de mantenimiento existente si es compatible. Si no existe, entregar operación/manual controlado documentado; no crear un temporizador web que prometa mantenimiento durable.
- Añadir observabilidad por lote/archivo/fase, tiempos y códigos sin nombres sensibles innecesarios. Documentar cómo investigar discrepancias entre disco, expediente e índice.

## Aceptación y entrega

Pruebas de acceso a lote ajeno, resultados paginados, exportación, recarga/reselección, operación incierta, actualización del árbol sin duplicación y limpieza concurrente/protegida. Acreditar que consultar estado no ejecuta efectos documentales. Entregar interfaz conectada, consultas, mantenimiento seguro y guía operativa con recuperación, retención y límites conocidos. Si faltan decisiones de retención, no habilitar borrado automático con un plazo inventado. SQL MySQL 5.1 manual para cambios justificados.
