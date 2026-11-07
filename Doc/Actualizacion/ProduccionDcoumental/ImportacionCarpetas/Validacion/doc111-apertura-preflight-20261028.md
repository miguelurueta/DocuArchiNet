# DOC-111 — Apertura, dispatcher y cálculo de preflight

Fecha: 2026-10-28. Base HEAD: `b2e93b726f221868cf2b82d43200a8e568871117`, con cambios locales previos de DOC-110/DOC-111. Las comprobaciones corresponden al workspace, no a una revisión publicada o desplegada. Progreso: **57/130** tareas; este corte completa 7.7–7.12 y 9.1–9.2, estas últimas mediante caracterización estática.

## Implementación

- `ServicioPreparacionImportacion.vb`: apertura de lectura con destino reconstruido, capacidad de esquema, publicaciones compatibles y límites explícitos. No crea plan, lote, expediente o documento al abrir. Se registra en Compile del proyecto.
- ASMX existente: `AbrirImportacion` y `ListarPerfilesImportacion` delegan mediante dispatcher operativo separado de `Ejecutar` administrativo. Cada llamada resuelve autoridad vigente, usa guard operativo y vuelve a interpretar el cuerpo cerrado. Se ignora la representación potencialmente coercionada por ASMX como fuente de autoridad; campos extra y duplicados se rechazan. Mantiene sobre estructurado, status HTTP, no-cache y errores sin detalles internos.
- Paginación de publicaciones: página/tamaño y `puedeHaberMas` calculado antes de filtrar incompatibles. Un resultado vacío por incompatibilidad no oculta páginas siguientes. El contrato y las siete claves obligatorias de límites se documentan en `Diseno/02-contratos-operacion.md`; no se modificó Web.config ni se eligieron cuotas productivas.
- `CalcularPlan`: cálculo interno sin persistencia ni endpoint. Canoniza inventario, selecciona publicación explícita, resuelve campos de raíz/archivo con reglas existentes, valida catálogos y restricciones del perfil y mantiene dependencias tipadas. El cálculo no recibe puertos de creación o almacenamiento.
- `CoordinadorProveedorIndices`: comparte el núcleo de resolución con el ensayo administrativo y mantiene getters/motor/autorización separados por propósito. Valores operativos conservan proveedor, versión y propiedad como procedencia. Una clave dependiente del expediente no consulta filas de negocio. La procedencia nueva solo se incorpora en el camino operativo; se conserva la salida administrativa anterior.
- `Diseno/15-integracion-carga-doc111.md`: parámetros, validaciones, contador/transacción, efectos en disco, expediente/relación y errores de la operación de creación. La ventana commit legacy → registro propio sin correlación inequívoca debe conservar INCIERTO y bloquear replay.

## Verificación local

| Comando | Resultado y alcance |
|---|---|
| `node --test tests/doc110-perfil-importacion-behavior.test.cjs` | PASS. Compila y ejecuta el arnés .NET Framework con todas las familias DOC-110 anteriores y DOC-111. Incluye el ASMX real invocado dentro de HttpContext sintético, sin servidor HTTP, conexión o autenticación real. |
| `node --test tests/doc111-importacion-carpetas-client.test.cjs tools/e2e/tests/doc88-production-document-upload-policy.test.cjs` | PASS: 15 pruebas locales. DOC-88 comprueba política y compatibilidad estática/dobles; no ejecuta una carga real. |
| `msbuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /p:DeployOnBuild=false /p:OutputPath=.tmp/doc111-opening-build/bin/ /p:IntermediateOutputPath=.tmp/doc111-opening-build/obj/ /nologo /verbosity:quiet /consoleLoggerParameters:ErrorsOnly /fileLogger /fileLoggerParameters:LogFile=.tmp/doc111-opening-build.log` | PASS, exit 0. Compilación del proyecto real; salida e intermedios separados. El proyecto conserva advertencias de dependencias y código legacy. Sin despliegue ni reemplazo de binarios del sitio. |
| `openspec.cmd validate doc-111-produccion-importacion-carga-carpeta --strict` | PASS estructural. No acredita implementación o aceptación del conjunto. |
| `git diff --check` | PASS; advertencia de conversión LF/CRLF sobre el reporte OPSXJ previamente modificado. |

Casos nuevos: destino no elegible o cambiado; permiso vigente revocado en el doble; importador sin administrar; token/origen inválidos antes de construir servicios; campos desconocidos o duplicados; destino del cuerpo comprobado frente al objeto ASMX; límite ausente/cero/negativo/fraccionario/desbordado; bytes UInt64 sin pérdida; publicaciones incompatibles y página filtrada; esquema no disponible; dos raíces homónimas; formatos `000123-20240229` y `000456#29-02-2024`; código textual, fecha canónica, tipología extraída, incidencias por archivo y dependencia EXPEDIENTE/ID sin ID ficticio. El mismo cálculo produce el mismo documento sin campos de control inventados.

La evaluación de elegibilidad se prueba por separado con metadatos sintéticos: completo/EXPEDIENTE conserva la raíz válida; completo/LOTE bloquea ante cualquier incidencia impeditiva; parcial admite un archivo válido dentro de una raíz afectada; error global bloquea siempre. La retirada de la incidencia de dependencia ocurre únicamente en el objeto local de prueba para verificar esa función; no habilita la capacidad productiva.

## Pendientes y límites

El cálculo incorpora una incidencia global `CAPACIDAD_NO_DISPONIBLE` para coincidencias, conserva `PuedeConfirmar=False` y no se publica como `PrepararPlan`. Falta integrar el lector parametrizado de identidad documental con comparación/collation, ámbito, volumen, cardinalidad y política BLOQUEAR/REUTILIZAR_VALIDADO comprobados. El lector legacy de código no devuelve identidad completa; el INSERT inspeccionado no fija volumen y su comprobación de existencia está comentada. No se sustituye esta dependencia por una búsqueda por nombre o por un resultado vacío ficticio.

7.13–7.15 siguen abiertas: evaluación integrada final, persistencia del plan calculado por servicio y entrada HTTP. El cálculo de barreras tiene cobertura local, pero no se presenta como preparación confirmable ni como persistencia integrada. El repositorio de planes previamente probado no se invocó en este corte.

5.9 y 6.6 siguen abiertas para su evidencia HTTP/autenticación y regresión integrada. La emisión del token en la vista y el recorrido visual operativo siguen pendientes. El hecho de añadir métodos al ASMX no acredita un endpoint desplegado o accesible desde la UI.

No se ejecutaron login, E2E real, consultas a bases reales, concesión/revocación de permisos, DDL/DML, cambios de perfiles, carga documental o REC-01/02. Su estado es **NO_EJECUTADO** en este corte. No se heredan autorizaciones de las pruebas de persistencia o publicación anteriores. La siguiente integración debe conservar estas restricciones y la incertidumbre documentada antes de habilitar efectos.
