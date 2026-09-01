# Pruebas y evidencia

## Automatizadas

- 24/24 pruebas locales aprobadas: contratos, lectura, escritura, política y plataforma E2E declarativa.
- Build VB.NET Debug aprobado sin errores; solo advertencias históricas de ensamblados.
- OpenSpec estricto válido.

## E2E autorizada

- Escritura: 1/1 aprobada; creación idempotente, consulta, actualización, conflicto y eliminación.
- Concurrencia: 1/2 actualizaciones efectivas y 1 conflicto de versión; evidencia saneada generada.
- Controles finales: gate `false`, usuarios/grupos vacíos y WebForms legacy sin cambios.

La evidencia detallada y los bloqueos históricos están en la [matriz de pruebas](../matriz-pruebas.md) y [05-PruebasEvidencia.md](../../../Tecnica/Opsxj/doc-42-transacciones-notas/05-PruebasEvidencia.md). No se guardan credenciales, cookies, tokens, cadenas de conexión, contenido ni cuerpos HTTP.
