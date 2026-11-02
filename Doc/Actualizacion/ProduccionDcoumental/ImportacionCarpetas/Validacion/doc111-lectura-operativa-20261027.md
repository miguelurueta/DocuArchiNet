# DOC-111: lectura operativa de perfiles, catálogos y proveedores

Resultado: **PASS local y de los casos de lectura real descritos**. Se completan 6.1–6.5; progreso **43/130**. La regresión administrativa DOC-110 sigue pasando. La fachada HTTP operativa no se declara integrada por añadir estos puertos; 5.9 y 6.6 continúan pendientes de su evidencia de revocación/integración.

## Lectura de publicaciones sin administrar

`IPerfilImportacionLecturaOperativa` expone únicamente verificar esquema, obtener publicación por versión y listar publicaciones. No incluye crear, guardar, publicar, crear versión ni cambiar disponibilidad. Lo implementa el repositorio de perfiles existente, conservando su interfaz administrativa y sus condiciones de autorización.

La lectura operativa exige identidad/empresa válidas y PuedeImportar; filtra perfil activo, ámbito y versión PUBLICADA. Carga cabecera, configuración y reglas en un snapshot RepeatableRead. Comprueba pertenencia, identidad, número y config_hash antes de devolver una copia independiente. Una versión pedida explícitamente no se reemplaza por la versión actual de la cabecera. El listado paginado anuncia únicamente la versión publicada actual de cada perfil activo; la selección por ID permite una publicación anterior válida.

Las pruebas locales comprueban rechazo de borrador, perfil inactivo, versión discordante, huella alterada y falta de facultad. También verifican copia independiente y ausencia de mutaciones en el puerto. La entrada administrativa conserva su contexto y no acepta la facultad operativa como sustituto.

## Catálogos comunes con autoridad por propósito

`PerfilImportacionCatalogAdapter` separa las entradas administrativas y operativas mediante dos fábricas de autoridad privada. Comparten el núcleo de clasificación, pertenencia, metadatos de índices, tipologías, normalización de valores y permisos existentes. La autoridad compartida contiene usuario/empresa, no un flag de administrador creado artificialmente.

La validación operativa de una publicación usa el modo de publicación del núcleo; no completa silenciosamente una huella de gabinete ausente como haría la edición de un borrador. Permanecen vigentes relación RA/DA, permiso de gabinete, tipos/longitudes, clasificación y fuentes diferidas. Se reforzó la entrada directa de helpers administrativos para que no puedan consultarse omitiendo su condición de administración.

Casos locales sobre el adaptador real con consultas dobles: lectura de gabinete por importador, restricción de longitud de índice, permiso revocado, ausencia de facultad antes de IO y denegación administrativa. Se ejecuta también el conjunto administrativo anterior con sus casos de catálogos.

## Proveedor y política vigente

La lectura de versiones de proveedor y el motor de consulta tienen puertos operativos tipados; las tablas y el motor físico siguen siendo los existentes. El camino operativo requiere proveedor activo y PUBLICADA, empresa coherente y autorización operativa explícita. Un callback administrativo no concede autorización operativa. Sin política operativa se deniega antes de metadatos/filas de negocio.

`MySqlFuenteImportacionRepository.CargarPoliticaOperativa` carga solo fuentes activas y sus columnas desde RA. Comparte el núcleo de lectura de política con la administración, manteniendo gates separados. `PoliticaFuentesProveedor.PermiteOperativa` conserva la restricción literal de tablas, la inscripción de fuente/columnas y el aislamiento por conexión/columna. El consumidor debe componerla de nuevo por solicitud; no hay una concesión persistida en Session.

El motor común conserva parametrización, límite de dos coincidencias, comprobación de metadatos/tipos, filtro empresarial cuando corresponda, ceros iniciales y errores de consulta. `CoordinadorProveedorIndices.ValidarOperativo` usa el getter operativo y la política operativa para las reglas que hacen referencia a proveedor; no llama al getter administrativo. La resolución completa de valores en preparación se integra en el bloque 7.

Los tests usan una tabla y filas sintéticas mediante el doble de consulta existente. Verifican publicación/borrador, inactividad, pérdida de autorización sin nuevas lecturas, literal de tabla no autorizado, política ausente, claves parametrizadas y ausencia de fallback a la vía administrativa. **No se consultaron filas de negocio DA para probar esta capacidad**.

## Selección y snapshot para preparar

`AdaptadorPerfilImportacionOperativa` compone los puertos de lectura, nivel/catálogos y validación de fuentes. Exige contexto con destino fijado, mantiene la versión solicitada, clona el resultado y vuelve a comprobar la huella después de validar compatibilidad. Un catálogo que no esté disponible no se oculta como una lista vacía. Las publicaciones incompatibles no se anuncian en esa página. La paginación se conserva explícita; no se promete un catálogo completo leyendo solo su primera página.

Las reglas de proveedor exigen un validador operativo configurado. Sin él, no se acepta la publicación. El snapshot no incluye facultades administrativas ni se vuelve autoritativo un contenido enviado por cliente. Sus pruebas de composición emplean dobles de puertos explícitos, no un endpoint ni una sesión autenticada.

## Lecturas reales y restauración del perfil 24

Se ejecutó la variante PublicationRead del arnés existente reutilizando la disponibilidad temporal del perfil E2E 24 autorizada por el usuario y conservando la restauración en finally:

- Obtener versión 38 devolvió perfil 24, publicación número 2.
- Obtener versión 37 devolvió publicación número 1; no se sustituyó por 38.
- El listado operativo incluyó la publicación actual 38 mientras el perfil estuvo activo.
- Alterar el objeto recibido localmente no cambió la lectura posterior ni el contenido de la base.
- Versión desconocida y getter administrativo con PuedeAdministrar=false fueron rechazados.

Son pruebas directas del repositorio con contexto servidor de fixture, no login HTTP ni concesión de IMPORTAR_CARPETAS. La cuenta conserva su permiso operativo original en cero. El perfil pasó temporalmente por revisión 10 y quedó **inactivo en revisión 11**. Cabecera de contenido, versiones, reglas, demás perfiles y permisos conservan sus huellas. Todas las filas y contadores de las seis tablas operativas permanecieron iguales: no se crearon planes, lotes, intentos ni documentos.

Evidencia: `tools/e2e/artifacts/doc111-publication-integration-result.json`. Una lectura previa exclusivamente SELECT verificó el rechazo del perfil inactivo/versión desconocida y su exclusión del listado: `tools/e2e/artifacts/doc111-operative-publication-read-result.json` (corresponde al estado previo revisión 9).

## Verificaciones y continuación

`node --test tests/doc110-perfil-importacion-behavior.test.cjs`: PASS con las nuevas familias de publicaciones, catálogos, selección y proveedor operativos, más DOC-110. Build Debug con MSBuild, DeployOnBuild=false y salida `.tmp/doc111-build`: PASS, sin desplegar binarios.

Continúan pendientes preparación del inventario, servicio de apertura/preflight y dispatcher HTTP operativo, recepción/staging, coordinación documental y pruebas HTTP de revocación. No se ejecutó una carga por carpetas ni REC-01/02.
