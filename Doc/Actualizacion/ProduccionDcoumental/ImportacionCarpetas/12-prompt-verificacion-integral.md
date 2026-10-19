# Prompt 12 — Verificación integral y regresión

## Encargo

Lee guía 00, los prompts 02–11 y la implementación real. Construye una matriz de verificación trazable a contratos, reglas y decisiones confirmadas. Esta entrega no autoriza cargas autenticadas ni despliegues por sí sola: respeta permisos de ambiente y AGENTS/runbook.

## Verificación

1. Auditar correspondencia entre DDL entregado, contratos y código: columnas/índices reales, MySQL 5.1, orden de scripts, versiones y estado de aplicación conocido. No considerar aplicadas migraciones porque sus archivos existan.
2. Probar perfil/publicación y ambos formatos de extracción con datos sintéticos; nivel recibido desde el árbol, perfiles compatibles, cambios de sesión/otra pestaña y controles de autorización servidor.
3. Validar dos raíces, homónimos, subcarpetas, restricciones, inventario fragmentado y revisión alterada. Comprobar política completa/parcial y ámbito confirmado en preflight y ejecución, no solo UI.
4. Probar confirmación concurrente, clave con payload distinto, cuotas, transferencia truncada, reenvío de contenido diferente, ruta maliciosa y limpieza protegida.
5. Validar creación bajo el nivel correcto, gabinete e índices, incorporación, inventario e índice documental cuando aplique. Verificar que repetir operaciones confirmadas no duplica efectos.
6. Inyectar pérdida de respuesta después de crear/almacenar, fallos entre fases, exclusión vencida con petición activa, cancelación, permiso revocado y proceso interrumpido. Demostrar recuperación o bloqueo seguro con incertidumbre explícita.
7. Verificar carga individual y rutas compartidas pertinentes. Ejecutar suites DOC-88 relevantes y declarar su carácter estático. Revisar diff/dependencias para demostrar SII sin cambios; no concluir compatibilidad real únicamente por un diff vacío.
8. Comprobar rendimiento contra volumen y navegadores acordados, con fixtures y entorno autorizados: tiempos, memoria cliente, límites de petición, disco temporal, consultas/concurrencia. Si no hay volumen objetivo o autorización, registrar pendiente; no inventar una capacidad máxima ni ejecutar carga real.
9. Verificar teclado/foco, estados de error, reabrir modal tras UpdatePanel, móvil objetivo y correspondencia de cifras/CSV con la base.

## Entregables

Matriz obligatoria de integridad aprobada:

| Caso | Resultado esperado |
|---|---|
| Publicar perfil completo sin ámbito | Rechazo; debe elegir EXPEDIENTE o LOTE. |
| Completo/EXPEDIENTE: A lista, B con archivo faltante o inválido | A puede avanzar; B no crea/almacena hasta completar sus condiciones. Ambas permanecen en el informe original. |
| Completo/LOTE: A lista, B con archivo faltante o inválido | Ninguna inicia efectos; el plan con error impeditivo global del lote no se confirma. |
| Completo/LOTE: todos recibidos y válidos | Se habilita procesamiento sin exigir una transacción global de todos los efectos. |
| Parcial | Ámbito efectivo NO_APLICA; procesar elegibles y conservar rechazados/pendientes, sin declarar completo con incidencias pendientes. |
| Publicar otra versión/cambiar perfil mientras el lote existe | No cambia modo/ámbito del lote; reanudar usa su snapshot. |
| Fallo después de almacenar en cualquiera de los ámbitos | Conservar IDs/efectos confirmados; recuperar pendientes sin duplicación ni borrado como rollback. |
| Dos peticiones coinciden con la última recepción | Barrera y reclamaciones impiden inicio prematuro o doble efecto. |

Esta matriz verifica dos ramas de un mismo motor y modelo de datos; no autoriza servicios/tablas duplicados por ámbito.

Agregar casos de integración transversales: esquema nuevo ausente sin romper Producción/carga individual; inclusión/publicación de archivos VB/recursos; CSRF en confirmación y multipart; modo parcial con errores individuales confirmable y errores globales bloqueantes; valores diferidos/fecha estable; nombre/extensión conservados con staging aislado; limpieza legacy que no consume el original; carga manual o SII concurrentes sobre recursos compartidos; consulta/reconciliación autorizada cuando se suspenden nuevos efectos. No cambiar expectativas o huellas DOC-88/DOC-90 para esconder una regresión.

Crear `Validacion/matriz-verificacion.md` y `Validacion/resultados.md` con caso, requisito, entorno, tipo de prueba, evidencia y estado: aprobado, fallido o no ejecutado. Mantener fixtures sin datos reales y scripts reproducibles. Entregar instrucciones de aplicación manual de SQL y validación posterior; no desplegar automáticamente.

Corregir defectos del alcance cuando sea posible y volver a verificar únicamente lo afectado. No cambiar SII para lograr pruebas verdes. Distinguir demostración con mocks, comprobación estática e integración autenticada. No declarar listo para operación si faltan evidencias críticas de permisos, integridad o recuperación; señalar exactamente qué validación queda pendiente y por qué.
