# DOC-111 — Valores diferidos y checkpoint local

Fecha: 2026-10-30. Base HEAD: `b2e93b726f221868cf2b82d43200a8e568871117`; cambios locales sin commit. Tareas 9.11 y 9.12; D-04/RQ-04. Progreso: 99/130. No representa aceptación integral.

## Implementación

- `CoordinadorImportacionCarpetas.Valores.vb` agrega `MaterializarArchivo(loteId, archivoId, revisionEsperada)`. Reconstruye identidad/destino del lote, exige expediente/relación confirmados, snapshot intacto y valores con correspondencia única a las reglas. Una llamada procesa un archivo.
- Reutiliza `CoordinadorProveedorIndices.ResolverOperativo`, el resolvedor de reglas, conversión canónica y catálogo operativos. Solo envía reglas diferidas. Las propiedades de creación proceden de los valores de raíz confirmados y el ID comprobado; no se vuelven a ejecutar sus fuentes. Las propiedades de clasificación solo se consultan cuando aún hay una dependencia correspondiente.
- El catálogo recibe únicamente valores recién resueltos, evitando convertir de nuevo códigos externos ya sustituidos por IDs. Los valores del preflight se conservan exactamente, incluida procedencia y ceros iniciales.
- `MySqlImportacionCarpetasRepository.Valores.vb` pertenece al mismo repositorio. Lee lote/raíz/archivo bajo el orden de bloqueo habitual y comprueba la relación/clasificación por IDs con el SELECT existente, también antes de guardar. No consulta proveedores durante la transacción.
- El checkpoint escribe una vez `valores_efectivos_doc`, revisión de archivo/lote y evento. Los predicados exigen revisión, recepción RECIBIDO, efecto NO_INICIADO, ausencia de intento/resultado y columna SQL NULL. No agrega tablas, migraciones ni otra configuración.
- `ValoresEfectivosFijados` procede de la nulidad SQL: una colección `[]` fijada es distinta de NULL. Repetir la fase devuelve una copia de lo persistido, sin consultar publicación, autorización del proveedor, fuente o catálogo de valores. Se mantiene la autorización actual del usuario/destino/recursos.
- Un fallo de resolución/conversión/catálogo registra RECHAZADO/VALORES_INCOMPATIBLES sin valores efectivos parciales. La identidad de raíz y totales permanecen intactos. Una nueva reserva ALMACENAR exige un checkpoint válido. No se declara completitud ni se elimina el expediente; recuperación explícita de rechazos queda en las fases pendientes.
- `ComposicionPerfilImportacionOperativa` extrae la construcción operativa existente del ASMX y la comparte con la composición interna de materialización. El ASMX delega en ella sin agregar acciones HTTP. No se encadena automáticamente a creación o almacenamiento.

## Verificación ejecutada

| Verificación | Resultado y alcance |
|---|---|
| `node --test tests/doc110-perfil-importacion-behavior.test.cjs` | PASS, 2 pruebas Node; compila/ejecuta el arnés Framework completo DOC-110/DOC-111 y la comprobación DDL existente. |
| Casos nuevos en el arnés | PASS: dependencia directa y de proveedor por ID confirmado, conservación de valores/procedencia/ceros, una consulta pendiente, ausencia de consultas al repetir, copia sin mutación, checkpoint SQL repetido sin escrituras/eventos, valores discrepantes rechazados. |
| Fallos | PASS: incompatibilidad física/conversión, fuente sin coincidencia, raíz no confirmada, duplicados/omisiones/dependencia o procedencia cambiadas, detención, revisión/snapshot cambiados, revocación antes del checkpoint y CAS fallido en cada UPDATE. Expediente conservado y valores no fijados. |
| SQL NULL/[] | PASS: lector IDataRecord sintético distingue ambos y una colección sin reglas se fija una vez, sin proveedor. |
| Compilación Framework Debug | PASS, MSBuild con salida `.tmp/doc111-valores-build/bin/` e intermedios propios; sin despliegue. Log local `.tmp/doc111-valores-build.log`. |
| OpenSpec estricto y `git diff --check` | PASS. Comparación de las funciones legacy protegidas sin diferencias. |

Los proveedores y ejecutores SQL son dobles explícitos. Se ejercitan el servicio, resolvedor y núcleo SQL productivos; no acredita bloqueo, commit, rollback ni carrera de MySQL real. La composición productiva se compila; no se invoca contra ambiente real.

## Límites

HTTP/IIS autenticado, escrituras SQL, creación/almacenamiento/incorporación reales y E2E: **NO_EJECUTADO** en este corte. No se solicitan ni usan credenciales. No se despliega ni activa perfil/configuración. El adaptador de almacenamiento sigue pendiente de aislamiento; las funciones legacy de almacenamiento, conservación, SII y carga individual permanecen intactas. Las tareas generales 10.3–10.7 no se cierran por completar estas dos capacidades.
