# DOC125 — Confirmación transaccional y recuperación

Estado 2026-11-23: backend implementado y verificado localmente; aceptación real abierta (26/31 tareas). [Diseño OpenSpec](../../../../openspec/changes/doc-125-importacion-excel-05-confirmacion-recuperacion/design.md), [quince requisitos/44 escenarios](../../../../openspec/changes/doc-125-importacion-excel-05-confirmacion-recuperacion/specs/importacion-excel-05-confirmacion-recuperacion/spec.md) y [evidencia local](../Validacion/doc125-implementacion-local-20261123.md).

## Contrato

Confirmar recibe ConexionId, IdCarga, ArchivoHash, ConfiguracionHash, EsquemaHash y ConfirmacionReemplazo. La sesión y los recursos existentes resuelven autoridad, archivo, plantilla, tabla, modo y política. ConsultarResultado/ConsultarCargaExcel conservan la solicitud ConexionId/IdCarga y devuelven estado, FilasConfirmadas y ConfirmadoUtc sin reparar datos. El estado estructural de una carga CREAR sin confirmar se consulta por SELECT, sin revelar ruta o documento privado.

Una confirmación histórica idéntica se devuelve antes de resolver archivo, límites, plantilla o esquema actuales, comprobando autoridad vigente. El bootstrap emite contexto/CSRF con capacidad de carga cerrada si los límites son inválidos; nuevas cargas se siguen rechazando. Una ejecución nueva revalida el archivo completo y los snapshots v1, y relee ejecución/plantilla/campos bloqueados antes de escribir.

## Escritura y recuperación

AGREGAR inserta íntegramente o revierte; un UNIQUE físico protege frente a otro escritor. REEMPLAZAR exige confirmación explícita y ejecuta DELETE/INSERT con la política empresarial vigente. La transacción de datos guarda CONFIRMADA/conteo/UTC en la misma conexión DA. La clave física completa y el motor se conservan. Los comandos tienen hasta 128 parámetros y presupuesto conservador min(65536,max_allowed_packet/2); una fila que no cabe se rechaza antes de borrar o insertar.

MySQL5.1 exige conexiones diferentes para los locks por ID y destino; se reutiliza la fábrica DA. CREAR identifica su tabla con `docuarchi/x1/<digest>`:56 caracteres ASCII que vinculan UUID y EsquemaHash completos mediante SHA256/base64url. Respeta el límite60 de las versiones anteriores a5.5.3, documentado en las [notas oficiales de MySQL](https://dev.mysql.com/doc/relnotes/workbench/en/news-5-2-41.html). Reconoce también la marca literal anterior sólo si coincide íntegramente con la misma carga/esquema, sin modificar tablas. Compara estructura materializada con la proyección normalizada. CREATE es una fase independiente: puede sobrevivir al rollback DML. No se adopta una tabla ajena ni se elimina una propia para ocultar un fallo. No hay DROP, TRUNCATE, upsert u omisión de filas.

La inspección global de relaciones se acredita una vez por operación/conexión/base/destino. Se midieron10–15s; esa consulta tiene presupuesto20s, las restantes SQL15s y HTTP30s. El resultado no se comparte entre sesiones. Siguen vigentes las comprobaciones de permisos/TRIGGER/motores/esquema/autoridad/locks y la exclusión de mantenimiento DDL concurrente. La consulta de resultado durable conserva la respuesta ante un corte; estos presupuestos no acreditan escritura real de20000filas.

FALLIDA acredita rollback conocido y conserva snapshots para un reintento explícito íntegro. Un error de commit o limpieza conserva RESULTADO_INCIERTO; no degrada una CONFIRMADA. Consultar por ID precede cualquier decisión de repetición. No existe pausa durable a mitad de filas ni commits por agrupación. Cerrar la pestaña/timeout no acredita cancelación o rollback. El ASMX puede serializar una misma sesión; las pruebas concurrentes usan sesiones independientes.

## Prerrequisitos y aceptación real

[SQL18](../Sql/18-preflight-doc125.sql) diagnostica por SELECT. [SQL19](../Sql/19-fixtures-da-doc125.sql) instala dos tablas descartables; [SQL20](../Sql/20-fuentes-e2e-ra-doc125.sql) registra dos fuentes y [SQL21](../Sql/21-plantillas-e2e-da-doc125.sql) tres plantillas y tres filas sintéticas iniciales. Todos quedan PENDIENTE_EJECUCION_MANUAL; no se ejecutaron. [Guía, orden y precondiciones](../Sql/README.md). Se reutilizan las 22 columnas de resultado existentes y no se cambia el esquema productivo de resultados.

La inspección rechaza triggers/FK de tablas que se escriben y exige visibilidad suficiente: TRIGGER efectivo y SELECT global para referencias entrantes desde otras bases en esta implementación. Si no se acredita, cierra la capacidad; estos archivos no conceden permisos ni recomiendan ampliar la cuenta automáticamente. La política de esa cuenta debe revisarse con SQL18.

La E2E real reutiliza la infraestructura actual con scope DOC125, autorización de ambiente/cuentas/efectos y tablas imp_02_doc125_e2e_. Se congelan conteos iniciales y máximos de inserción/eliminación; cualquier deriva detiene el caso. El rollback usa VALOR duplicado bajo un segundo UNIQUE después del DELETE; la carrera retiene una fila externa sin commit cinco segundos. Se exige CONFLICTO_CLAVE para acreditarla y se conservan todos los efectos. No se utiliza un endpoint de inyección de fallos.

## Evidencia y límites

27/27 tests locales y build aislado sin errores. Los 4348 checks nativos son sintéticos; la igualdad, rollback y carrera MySQL reales aún requieren la ventana autorizada. Se generaron cinco XLSX sintéticos con el mismo generador DOC123, verificados por su lector: 1000/1000/1000/300/10 filas. La ubicación/manifiesto se conserva en .tmp excluido de Git. El acuse ignorado se distingue de una pérdida física de red/commit, no inducida por esta matriz.

Mantener 20000 filas configuradas y 10000000 bytes; el lector conserva su default 10000. Las pruebas locales cubren presupuesto de agrupación para 20000, rechazo del lector para 20001 y 10MB exactos/exceso; no acreditan capacidad de escritura real para 20000. Los 12759 registros preparados en DOC124 no son benchmark de confirmación. Los 625 contratos anteriores y otras fuentes no fueron modificados por esta implementación ni son fixtures descartables por inferencia. No se desplegó. COLUMNA, fallo intermedio de CREATE y commit incierto tienen cobertura local, no fallos físicos MySQL inducidos. La interfaz y publicación administrativa siguen en sus fases.
