# DOC-122 — IMPORTACION-EXCEL-02-AUTORIZACION-CONEXION-PLANTILLAS

- Ticket: DOC-122
- Cambio OpenSpec: doc-122-importacion-excel-02-autorizacion-conexion-plantillas
- Clasificacion: cross_cutting

## Servicios y reglas
ImportacionExcelContextAdapter valida usuario activo, empresa RA y permiso propio vigente. ImportacionExcelConexionAdapter reutiliza ModuleSessionConnectionStringResolver con DA_, verifica empresa/módulo de sesión y correspondencia exacta del catálogo histórico con ese destino. La fábrica se invalida ante cambios de conexión o módulo. No exige cuenta adicional ni conexión Excel en Web.config; no lee las credenciales de imp01_xmlconexion. ServicioPlantillasExcel valida campos, clave, modos, tipos y metadatos, sin alterar tablas. MySqlPlantillaExcelRepository usa parámetros, lectura consistente, transacción InnoDB y bloqueo antes de comparar revisión/huella. Cabecera y campos revierten juntos; IDs conservados permanecen iguales. Huella v1 canónica incluye NULL y transformaciones y detecta cambios legacy sin revisión. GUARDADO_NO_COMPLETADO exige consultar antes de repetir; no hay reintento automático. Detalle operativo en Doc/Actualizacion/Importacion/Diseno/02-autorizacion-plantillas-doc122.md.
