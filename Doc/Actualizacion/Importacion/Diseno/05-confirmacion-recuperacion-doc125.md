# DOC125 — Confirmación transaccional y recuperación

Estado 2026-11-23: refinamiento técnico; implementación y aceptación real por ejecutar.
El [diseño OpenSpec](../../../../openspec/changes/doc-125-importacion-excel-05-confirmacion-recuperacion/design.md) contiene el inventario de rutas, quince decisiones y la secuencia. La [especificación](../../../../openspec/changes/doc-125-importacion-excel-05-confirmacion-recuperacion/specs/importacion-excel-05-confirmacion-recuperacion/spec.md) define quince requisitos y 44 escenarios.

## Contrato

Confirmar recibe ConexionId, IdCarga, ArchivoHash, ConfiguracionHash, EsquemaHash y ConfirmacionReemplazo. El servidor resuelve autoridad, archivo, plantilla, tabla, modo y política desde los recursos existentes. ConsultarResultado conserva la solicitud de consulta DOC124 y devuelve estado, filas confirmadas, fecha y diagnóstico seguro; no repara ni importa datos.

Una confirmación ya terminada y de contenido idéntico devuelve su resultado histórico antes de leer límites, archivo, plantilla o esquema actuales. La autoridad y propiedad siguen siendo obligatorias. Una carga por ejecutar revalida su archivo completo y snapshots; el destino y las plantillas se bloquean y releen antes de escribir.

## Escritura y recuperación

AGREGAR rechaza cualquier duplicado y revierte el intento completo. REEMPLAZAR exige confirmación explícita y ejecuta DELETE/INSERT dentro del ámbito autorizado en una sola transacción. CREAR identifica su tabla por UUID/hash en el comentario: el DDL puede sobrevivir a un rollback de filas y se recupera por procedencia/estructura compatible, sin apropiarse de tablas ajenas.

En MySQL5.1 los locks por ID y destino requieren conexiones diferentes. La conexión de datos comparte una transacción InnoDB para filas y CONFIRMADA/conteo/UTC. La conexión de lock de destino usa el mismo recurso DA, sin otra cadena en Web.config. Los snapshots v1 y las 22 columnas existentes se conservan.

FALLIDA acredita rollback conocido y conserva snapshots para un reintento explícito íntegro. RESULTADO_INCIERTO conserva el ID y exige consulta; no autoriza replay automático. No existe una pausa durable a mitad de filas ni commits parciales. Cerrar la pestaña no acredita cancelación.

Los métodos ASMX actuales pueden serializar peticiones de una misma sesión ASP.NET:
no se promete consulta de progreso en vivo desde esa sesión durante Confirmar. La
aceptación concurrente usa sesiones independientes; un timeout HTTP se recupera
consultando el ID y no demuestra rollback. Esta fase no añade un trabajo en segundo
plano ni un protocolo de pausa/reanudación por posición de fila.

## SQL y pruebas

Se entrega [SQL18](../Sql/18-preflight-doc125.sql), diez SELECT separados por bloques DA/RA, con [guía manual](../Sql/README.md). Estado PENDIENTE_EJECUCION_MANUAL; no se ejecutó. No hay una migración nueva de resultado: filas_confirmadas y confirmado_utc ya existen. Una adecuación que resulte necesaria tendrá otro archivo y confirmación manual.

Se reutilizan harness nativo DOC122–124, runner DOC110, suite importacion-excel y controles SELECT DOC32. La E2E escritora requiere autorización nueva de ambiente, cuentas, destinos descartables y efectos; las cargas previas y los 625 contratos siguen protegidos. El gobierno exige evidencia unit, e2e y manual_qa. No se atribuyen pruebas o importaciones reales a este refinamiento.

## Límites

Conservar 20000 filas configuradas y 10000000 bytes; el lector mantiene su predeterminado 10000. Los 12759 registros del mayor archivo real DOC124 no acreditan capacidad de escritura para 20000. El índice UNIQUE protege identidad frente a escritores externos; exclusividad completa requiere que éstos cooperen. El mantenimiento DDL externo queda fuera de la garantía del lock. Los metadatos y la visibilidad de triggers/relaciones deben acreditarse antes de habilitar escrituras. La interfaz pertenece a fase06 y la publicación de nuevas fuentes usa sus servicios administrativos existentes.
