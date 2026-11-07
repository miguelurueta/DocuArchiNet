# DOC-110: política RA y consumidores de proveedores publicados

Fecha: 2026-10-27. Verificación de 15.9 (D-09/RQ-12) y 14.29 (D-09/RQ-12). Esta continuación añade pruebas de comportamiento y concilia evidencia; no cambia código productivo ni realiza nueva autenticación.

## Política RA: 15.9

Composición contrastada en WebServiceImportacionCarpetas:

| Consumidor | Autoridad y duración |
|---|---|
| ConsultarMetadatosProveedor | PoliticaActual(c), materializada desde MySqlFuenteImportacionRepository.CargarPolitica. |
| Administración de proveedor, incluida publicación | Misma PoliticaActual(c), compartida por validación/metadatos/consulta administrativa de esa operación. |
| Consulta de índices del perfil | Lazy(Of PoliticaFuentesProveedor) nuevo dentro de Ejecutar; su Value se consulta desde el callback de autorización del adaptador. Una carga como máximo dentro de esa petición; no se reconstruye por índice. |

PoliticaActual resuelve RA_ mediante el contexto servidor y CargarPolitica lee solo fuentes activas del ámbito, con sus columnas y aislamiento. No se encontraron lecturas de Doc110.Proveedores.FuentesAutorizadas en la fachada ni las capas de esta capacidad. Doc110.Fuentes.TablasAdicionales es un límite adicional de nombres y no una autoridad funcional alternativa. No hay fallback al archivo ni unión de listas.

CargarPolitica materializa dentro de la transacción de lectura; si la lectura MySQL falla, Transaccion lanza CAPACIDAD_NO_DISPONIBLE y no retorna una política de acceso. La fachada convierte ese código a 503. El constructor JSON de PoliticaFuentesProveedor sigue siendo el materializador/codec, no lectura de Web.config.

Se amplía ProveedorIndicesBehaviorTests con el lector de política simulado mediante el callback de autorización ya existente. Un fallo CAPACIDAD_NO_DISPONIBLE debe propagarse sin resolver ninguna conexión de origen ni consultar metadatos/filas; el contador del lector prueba que la validación efectivamente se alcanzó. Una política vacía/revocada debe devolver ACCESO_DENEGADO sin valores y sin I/O del origen. Esto ejecuta el adaptador productivo, con dependencias explícitamente dobles; no acredita una caída real de RA.

La revocación efectiva y la lectura de RA en el ambiente autorizado quedaron acreditadas en [aceptación de fuentes](doc110-fuentes-aceptacion-preparada-20261026.md): activación/edición visibles para proveedores, consulta real del índice, revocación con ValidarProveedor 403 y consumidores/publicaciones/historial intactos. El diagnóstico posterior confirma fuente inactiva y referencias publicadas conservadas. Una petición nueva recibe política nueva; no se promete cancelar una consulta ya iniciada. La duración Lazy se verifica por composición del código, sin afirmar que se midieron consultas RA en IIS.

## Solo proveedores publicados: 14.29

| Operación | Evidencia |
|---|---|
| Listado | MySqlProveedorIndicesRepository.Listar filtra ámbito, activo=1 y estado PUBLICADA. E2E autenticada exige catálogo filtrado vacío para ID de la versión BORRADOR de un proveedor activo. |
| Guardado y ensayo | E2E autenticada envía ID directo BORRADOR: 403 ACCESO_DENEGADO sin alterar perfil ni proveedor. Obtener del repositorio exige PUBLICADA; Compatible del coordinador vuelve a comprobar estado, disponibilidad, ámbito e identidad/versiones. |
| Publicación | Servicio.Publicar valida snapshot persistido mediante ExigirConfiguracion. El arnés .NET rechaza snapshot con proveedor BORRADOR y exige documento intacto, cero escrituras y cero consulta de filas del origen. |
| Selección administrativa | Servicio.Seleccionar revalida configuración persistida. El arnés .NET rechaza referencia BORRADOR aun desde perfil PUBLICADA; selección positiva con proveedor publicado/disponible usa el snapshot servidor sin escribir ni consultar datos de negocio. |
| Referencias históricas | E2E autenticada conserva dos publicaciones completas e historial tras cambio de versión y desactivación del proveedor; revocar la fuente tampoco los reescribe. |

Las defensas de publicación/selección contra un snapshot inválido son pruebas locales con repositorio doble. No se introdujo un perfil inválido en MySQL para fabricar ese caso por HTTP: el guardado ya lo impide. Evidencia autenticada y local detallada en [revalidación](doc110-revalidacion-proveedor-20261026.md) y en la aceptación de fuentes. La restricción operativa IMPORTACION sigue fuera de DOC-110; estas comprobaciones no conceden permisos de carga.

## Ejecución de esta continuación

```powershell
node --test tests/doc110-perfil-importacion-behavior.test.cjs
```

**1 prueba Node PASS**, que compila vbc/csc y ejecuta todos los grupos del arnés .NET Framework, incluidas las nuevas verificaciones de fallo de política. No es un build completo del proyecto WebForms. La búsqueda de la clave antigua en la fachada y capas de la capacidad no devolvió coincidencias. OpenSpec estricto y diff de espacios PASS.

Se completan 15.9 y 14.29 con el conjunto de código, pruebas de comportamiento y corridas reales existentes; no se generaliza evidencia. Permanecen pendientes los fallos transaccionales reales, concurrencia/aislamiento de fuentes, concesión/revocación externa de permisos y demás tareas abiertas. Sin DDL, interrupción de RA, cambios SII, despliegue ni nuevos secretos.
