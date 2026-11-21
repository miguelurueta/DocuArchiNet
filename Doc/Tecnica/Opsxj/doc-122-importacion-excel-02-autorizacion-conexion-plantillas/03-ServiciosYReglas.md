# DOC-122 — IMPORTACION-EXCEL-02-AUTORIZACION-CONEXION-PLANTILLAS

- Ticket: DOC-122
- Cambio OpenSpec: doc-122-importacion-excel-02-autorizacion-conexion-plantillas
- Clasificacion: cross_cutting

## Servicios y reglas
ImportacionExcelContextAdapter valida usuario activo, empresa RA y permiso propio vigente. ImportacionExcelConexionAdapter exige configuración protegida, cuenta distinta de la histórica y correspondencia exacta con DA_ e ID_XMLCONEXION. ServicioPlantillasExcel valida campos, clave, modos, tipos y metadatos, sin alterar tablas. MySqlPlantillaExcelRepository usa parámetros, lectura consistente, transacción InnoDB y bloqueo antes de comparar revisión/huella. Cabecera y campos revierten juntos; IDs conservados permanecen iguales. Huella v1 canónica incluye NULL y transformaciones y detecta cambios legacy sin revisión. GUARDADO_NO_COMPLETADO exige consultar antes de repetir; no hay reintento automático. Detalle operativo en Doc/Actualizacion/Importacion/Diseno/02-autorizacion-plantillas-doc122.md.
