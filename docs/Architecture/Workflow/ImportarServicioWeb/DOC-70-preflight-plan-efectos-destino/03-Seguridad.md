# Seguridad y pureza

- Gate y contexto se validan antes de componer el servicio.
- Tarea y tipología provienen o se confirman en backend.
- La configuración usa el repositorio MySQL parametrizado existente mediante una proyección de solo lectura.
- Preflight no depende de clientes SII ni repositorios de mutación.
- El fingerprint SHA-256 cubre usuario, tarea, ruta, trámite, proveedor, selección y configuración efectiva.
- La creación rechaza autoridad obsoleta antes del lock y la persistencia.
- El modo múltiple no afirma expedientes físicos ni relaciones todavía no resueltas.
