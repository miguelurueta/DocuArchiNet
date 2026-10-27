# DOC-110 — Implementación base y verificación local

Fecha: 2026-10-20. El usuario aclaró expresamente que el flujo OPSXJ debe continuar implementando. Sustituye la restricción de trabajo exclusivamente documental; no autoriza migraciones, asignación de permisos ni E2E autenticadas.

## Implementado

| Componente | Archivos / alcance real |
|---|---|
| Modelos y puertos | `Modelo/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionModels.vb`, `PerfilImportacionInterfaces.vb`. Sin dependencia de HTTP, sesión o lotes. |
| Reglas | `ResolvedorReglasPerfilImportacion.vb`: constantes, carpeta/archivo, extracción completa/segmento literal de base cero, retiro explícito de extensión, espacios explícitos, propiedades de expediente diferidas, tipos y fechas. Código de expediente limitado a 45; destinos únicos. |
| Validación | `ValidadorPerfilImportacion.vb`: estructura, opciones, referencias canónicas, modo/ámbito y coincidencias. Catálogo RA/DA integrado. |
| JSON | `PerfilImportacionJson.vb`: nombres exactos, rechazo de propiedades desconocidas/duplicadas y coerciones numéricas, profundidad 16, máximo 262144 caracteres, ASCII escapado y huella determinista. Usa Newtonsoft ya existente. |
| Persistencia | `MySqlPerfilImportacionRepository.vb`: consultas parametrizadas por ámbito, transacciones, bloqueo cabecera→versión, CAS, publicación y nuevas versiones. Código compilado, no probado contra BD. Detección inicial de esquema comprueba motor/collation, columnas usadas y unicidades; no acredita comparación exhaustiva de todo el DDL. |
| Servicio | `ServicioPerfilImportacion.vb`: autorización y validación previas, ensayo sin escritura, publicación con huella, selección administrativa y disponibilidad. Consume puerto de catálogo; compuesto en la fachada ASMX. |
| Seguridad | `PerfilImportacionContextAdapter.vb`: usuario/empresa RA, rechazo de consulta pública y permiso vigente único. `PerfilImportacionRequestGuard.vb`: token aleatorio ligado a sesión/usuario/empresa, método/contenido/origen. Guard conectado a página y ASMX. |
| Permiso existente | Inicialización y carga de `GA_ADMINISTRAR_PERFILES_IMPORTACION`, default 0; columna ausente y filas ambiguas no lo conceden. No se asignaron permisos. |
| Transporte JS | `PerfilImportacionApi.js`: sobre ASMX, token en cabecera, errores seguros, timeout y bloqueo tras expiración/rechazo. No reintenta mutaciones. Conectado al editor y fachada. |
| Proyecto | Archivos VB, ASMX, JS y CSS registrados en el proyecto WebForms existente. |

Límites técnicos iniciales: hasta 128 reglas, 4096 caracteres por entrada/valor, 20 ejemplos y páginas de 1–50 elementos. No representan una capacidad medida de importación. Opciones implementadas: RAIZ_EXPEDIENTE; subcarpetas INCLUIR o RECHAZAR; vacías y archivos sueltos RECHAZAR. Esta última selección debe ser explícita para publicar; no hay omisión silenciosa ni ejecución de carpetas en DOC-110. Nuevas opciones requieren semántica y pruebas.

## Evidencia local

| Comando | Resultado / límite |
|---|---|
| `MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /p:DeployOnBuild=false /clp:ErrorsOnly /nologo` | PASS, salida 0. Compilación .NET Framework del proyecto real, sin despliegue. La compilación general presenta advertencias legacy; no se modificaron para ocultarlas. |
| `node --test tests/doc110-perfil-importacion-behavior.test.cjs tests/doc110-perfil-importacion-client.test.cjs tests/doc110-perfil-importacion-editor.test.cjs` | Siete grupos PASS incluyendo editor y catálogo con dobles. Compila/ejecuta VB y runner C# con el patrón existente; reglas, servicio con puertos dobles, guard con HttpContext sintético y cliente con transporte doble. Sin BD, login ni HTTP real. |
| `node --check js/gestion/ImportacionCarpetas/PerfilImportacionApi.js` | PASS. |
| `node --test tests/doc88-produccion-carga-tipologias.test.cjs tests/doc88-produccion-carga-preparacion.test.cjs tests/doc88-produccion-carga-configuracion.test.cjs tests/doc88-produccion-carga-almacenamiento.test.cjs tests/doc88-produccion-upload-contract.test.cjs tests/doc88-produccion-proyeccion.test.cjs` | 14 pruebas PASS. Regresión estática de contratos; no equivale a carga real. |

No se modificaron fuentes SII ni los handlers de carga individual. Los archivos compartidos modificados son el cargador/inicializador del permiso y las inclusiones de proyecto. Se preservan cambios ajenos existentes en el workspace. No hay commit, merge ni despliegue de esta implementación.

## Integración y evidencia actual — 2026-10-20

DA confirmado por usuario. Gabinete, índices y permisos DA; perfiles y clasificación RA. Origen y huella persistidos en JSON existente sin nueva DDL. La verificación de esquema del repositorio (columnas, tipos, nulabilidad, índices, PK, FK y motor/collation) pasó mediante SELECT real: doc110-verificacion-esquema-runtime.json. No se ejecutaron mutaciones de prueba.

Editor integrado en Producción: lista/búsqueda por código, creación, clasificación, reglas/ensayo, publicación, historial paginado y disponibilidad. Conserva el nivel existente. Fachada ASMX con doce operaciones, DTO cerrado, identidad del servidor y guard CSRF. No implementa transferencia, lotes ni almacenamiento.

MSBuild del proyecto real PASS. Siete grupos DOC-110 y catorce regresiones estáticas DOC-88 PASS (21 total). Las pruebas de editor usan JSDOM y las de servicio/catálogo usan dobles: no acreditan ciclo WebForms ni HTTP real.

E2E: npm.cmd --prefix tools/e2e run test:doc110:perfiles -- --list descubre cuatro escenarios: PER-01, PER-02, PER-03 y guardado concurrente. Reutiliza autenticación/configuración/reporteros existentes. Ejecución real NO_EJECUTADA.

Corrida requiere autorización explícita y variables DOC110_E2E_AUTHORIZED=YES, DOC110_E2E_BASE_URL, DOC110_E2E_MODULE, DOC110_E2E_USER/PASSWORD, DOC110_E2E_RESTRICTED_USER/PASSWORD, DOC110_E2E_FIXTURE_PATH y DOC110_E2E_MYSQL_URL de control SELECT. No guardar secretos en Git. Fixture: configuración/reglas válidas y dos ejemplos con metadatos/esperados. Crea perfiles descartables; no los borra automáticamente.

Pendientes: concurrencia/publicación real, aislamiento HTTP/CSRF/revocación, esquema indisponible, ciclo UpdatePanel/accesibilidad, regresión funcional de carga individual y concesión externa. No archivar DOC-110. No se modificó SII. No hubo despliegue, commit ni merge.
