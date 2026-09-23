# Pruebas y evidencia

- Ticket: DOC-78
- Cambio OpenSpec: doc-78-proteccion-contexto
- Clasificacion: cross_cutting

## Evidencia requerida

Ejecución del 23 de septiembre de 2026:

- `node --test Tests/importar-servicio-web-*.test.cjs`: 434 aprobadas, 0 fallidas.
- `msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m`: 0 errores y 310 advertencias preexistentes.

Las suites focales cubren contexto inmutable, preflight anterior al efecto, bloqueo/restauración, códigos normativos, recuperación con `IntentId`, señal multi-pestaña y rechazo de documentos de otra tarea.

## QA/E2E WebForms

Se ejecutó con autorización explícita el escenario `import-sii-recovery` para la tarea `219877` y una intención autoritativa persistida. La plataforma terminó correctamente con 7 controles y `sinCambios=SI`; la recuperación consultó estado verificable sin reejecutar la intención.

Al cierre se verificó la restauración del gate moderno con usuarios y grupos vacíos. La evidencia producida por la plataforma quedó saneada y no contiene credenciales, cookies ni cadenas de conexión.
