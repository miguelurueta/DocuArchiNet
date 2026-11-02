# DOC-111 — Adaptador documental local

Fecha: 2026-10-30. Base HEAD: `b2e93b726f221868cf2b82d43200a8e568871117`. Workspace con cambios locales previos; sin commit/despliegue. Tareas 9.7, 9.9 y 9.10 implementadas y verificadas localmente; progreso 102/130. 10.3–10.7 siguen parciales: no existe todavía el recorrido integral de ejecución HTTP y cierre de unidades/lote.

## Alcance y reutilización

Se respeta la instrucción explícita del usuario de mantener intacta la función de almacenamiento y descartar como requisito adicional resolver el aislamiento del XML posterior al commit frente a otras cargas. No se afirma exclusión global nueva. Se conservan autoridad, reclamación propia, original retenido, verificación e incertidumbre sin replay.

- `ComandoAlmacenamientoCarpetas` copia de forma inmutable contexto, raíz, archivo, snapshot e intento. Exige raíz/relación confirmadas, reserva nueva ALMACENAR, archivo EN_CURSO propio, recepción íntegra, valores efectivos fijados, huella/reglas coherentes y referencias compatibles con Integer legacy. Reutiliza validación de reglas/valores y las comprobaciones de texto/Integer del comando de creación.
- Admite PDF dentro de las restricciones del perfil. No cambia nombre, índices, extensión o configuración para sortear incompatibilidades. Rechaza antes de efectos nombres con comillas simples, barras, controles, separadores no admitidos o caracteres no representables en el SQL concatenado. No admite TIFF/Multi-TIFF.
- `AdaptadorDocumentoImportacion` implementa la capacidad estrecha `IAlmacenamientoDocumentoImportacion`. El puerto amplio de reconciliación hereda esa capacidad y queda pendiente de 10.8. El adaptador no reconsulta proveedores ni vuelve a resolver valores.
- Reutiliza `AlmacenStagingImportacion.ConservarVerificado` y `CrearCopiaTrabajo`. Después del inicio durable, conserva un handle del original durante llamada, verificación y checkpoint, crea una copia privada con nombre exacto y comprueba tamaño/SHA-256 antes de entregarla. El writer puede consumir la copia. Los paths no forman parte del resultado.
- El catálogo compartido `.Documento.vb` revalida gabinete/tipología e índices sin alterar lo ya fijado; obtiene los nombres físicos desde la consulta existente de `CamposNucleo`. Nunca usa el alias visible como columna. Comprueba que NOMBRE pueda almacenar el nombre original y rechaza metadatos físicos duplicados.
- Los campos técnicos reservados se rechazan. Los campos precargados NOMBRE/EXPEDIENTE/CLASEDOCUMENTO/TIPODOCUMENTO solo se admiten si coinciden exactamente con el valor que usará legacy y se omiten de la lista externa para evitar duplicidad. Reglas sobre FECHAELABORACION/NOMBRESERIE/NOMBRESUBSERIE se rechazan explícitamente en esta composición; no se ignoran silenciosamente.
- La clase `DOCUMENTO ELECTRONICO` reutiliza la elección de la carga existente (`ClassAlmacenamiento.vb:5929/6294`); se resuelve su ID sin fijarlo por ambiente y se rechaza ambigüedad. Lo mismo aplica a DA_EXTENSION para `.PDF` y al nombre del gabinete.

## Binding y persistencia

El parcial `.Legacy.vb` compone autoridad, catálogo y el mismo repositorio operativo. Usa la función existente `AlmacenamientoDocumentoProduccionDocumental` mediante parámetros nombrados: gabinete, ruta de copia, nombre original, expediente confirmado, tipología, índices efectivos y clase. `TipoAlmacenamiento=0` evita la limpieza adicional de WF_RUTA_TEMPO_ADJUNTA; no cambia selecciones o alias de Session ni crea tareas/rutas Workflow. Los IDs ByRef de documento e inventario se capturan en Finally.

La autoridad exige HttpContext.Current, identidad/destino/recursos vigentes, misma conexión RA/DA capturada al construir y correspondencia con los helpers legacy. Se generaliza la comprobación privada de conexión existente para RA_/DA_, manteniendo el caso de creación. Solo se manipulan cadenas en memoria; no se imprimen ni persisten. La relación/clasificación del expediente se vuelve a comprobar antes de iniciar y antes del efecto.

`MySqlImportacionCarpetasRepository.Documento.vb` amplía el único repositorio, sin tablas nuevas:

1. Inicio durable RESERVADO→EN_CURSO por IDs, fase, token/huella y evidencia inicial nula; compara contexto/snapshot/raíz/archivo/revisión y respeta detención antes del inicio. Libera la transacción antes del writer.
2. Resultado con actualización transaccional de intento, archivo, revisión/estado de lote y evento. No cambia la identidad de raíz ni reduce totales.
3. CONFIRMADO completa exclusivamente el archivo cuando se comprobó también su incorporación. La agregación de raíz/lote permanece pendiente.
4. INCIERTO conserva IDs conocidos, evidencia, valores efectivos y marcador activo. Una respuesta de error de legacy no se presenta como ausencia de efectos.
5. La repetición terminal exacta del checkpoint no escribe ni genera evento; evidencia distinta se rechaza. Un fallo de CAS o de checkpoint no permite otra invocación automática. Se conserva una señal de detención llegada durante el efecto.

`ResultadoEfectoImportacion.InventarioId` distingue inventario de imagen/documento; es opcional en JSON para preservar los contratos anteriores. La creación rechaza ese dato ajeno a su fase. Los nombres de tipos nuevos evitan colisiones con los modelos existentes de ImportarServicioWeb, que permanecen intactos.

## Verificación de resultado

El lector parametrizado del catálogo comprueba por IDs conocidos:

- En RA, relación al nivel, empresa/actor/autoridad y clasificación mediante el lector existente del repositorio.
- En DA, registro de gabinete, inventario documental e índice electrónico enlazados; sus IDs, expediente, empresa/actor, tipología, clase y gabinete deben coincidir.
- NOMBRE, segundo nombre del inventario e índice deben conservar el original; DBT debe coincidir con el tipo PDF resuelto y ambos formatos con `.PDF`.
- Cada índice efectivo se compara con la columna física validada. DATE se lee con CAST AS CHAR para conservar yyyy-MM-dd; DECIMAL admite la escala física equivalente. Texto/referencias se comparan exactamente, incluidos ceros.

Solo se acepta una fila. Ausencia, duplicados, cambio de valores/nombres/formato/relaciones o lectura/autoridad fallida conservan INCIERTO. El lector no incorpora, no busca efectos por nombre y no acredita el contenido físico final ni el XML.

## Evidencia ejecutada

| Verificación | Resultado |
|---|---|
| `node --test tests/doc110-perfil-importacion-behavior.test.cjs` | PASS, 2 pruebas Node: arnés Framework DOC-110/DOC-111 y comprobación de columnas UPDATE de intentos contra DDL, incluyendo inicio/resultado documental. |
| Adaptador con PDF real local y escritor/SQL dobles | PASS: copia Unicode/ceros consumida, original retenido y handle liberado, llamada única, checkpoint y repetición sin nueva escritura. |
| Fallos posteriores al inicio | PASS: rechazo/excepción del escritor, nombre discordante, verificación fallida, revocación antes de writer, checkpoint fallido/respuesta perdida y CAS fallido en cada UPDATE conservan incertidumbre/IDs sin replay. |
| Guards previos | PASS: reserva histórica, token/propietario/contexto/snapshot/valores cambiados, nombre inseguro, índices duplicados/conflictivos/técnicos y TIFF impiden el inicio/efecto correspondiente. |
| Catálogo/lector con SELECT sintéticos | PASS: mapeo por nombre físico, clase/PDF no ambiguos, permisos; ausencia/duplicidad/discrepancia de relaciones, nombres, formato, tipo e índices impiden confirmación. DATE y escala DECIMAL verificadas. |
| MSBuild Debug del proyecto real | PASS, salidas/intermedios `.tmp/doc111-adaptador-documento-build/`; log local `.tmp/doc111-adaptador-documento-build.log`, sin despliegue. |
| OpenSpec estricto y `git diff --check` | PASS. Funciones legacy protegidas y componentes SII/ImportarServicioWeb sin diferencias. |

Almacenamiento legacy real, SQL DML/commit/rollback/carrera real, HTTP/IIS autenticado, archivo final/XML y E2E: **NO_EJECUTADO**. El binding real se compiló; las invocaciones del writer y las escrituras SQL se sustituyeron por dobles explícitos. No se hereda autorización de pruebas anteriores. No se habilita endpoint, worker ni carga automática.
