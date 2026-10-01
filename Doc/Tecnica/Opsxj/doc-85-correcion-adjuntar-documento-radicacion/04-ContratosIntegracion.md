# CORRECION-ADJUNTAR-DOCUMENTO-RADICACION

- Ticket: DOC-85
- Cambio OpenSpec: doc-85-correcion-adjuntar-documento-radicacion
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

El handler y `uploadFiles` permanecen sin cambios. La llamada existente de doce argumentos resuelve la sobrecarga DOC-85; las siete llamadas de diez argumentos conservan la sobrecarga legacy. No hay migración de esquema, endpoint nuevo, gate ni autenticación alternativa.
