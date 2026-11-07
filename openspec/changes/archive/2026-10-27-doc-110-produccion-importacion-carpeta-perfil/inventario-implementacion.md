# DOC-110 — Inventario de implementación y pruebas

Tarea documental 1.4. El inventario productivo de `design.md` sigue siendo el listado principal; este documento fija integración y pruebas faltantes. Archivos nuevos son propuestas concretas, no código existente. Los propietarios indicados son responsabilidades técnicas; no se inventan personas/equipos.

## Integración verificada

| Área propietaria | Archivos | Responsabilidad / consumidor |
|---|---|---|
| Contexto/seguridad | `Gestion/Class_remit_dest_interno_perfil_produccion.vb`, `Global.asax.vb` | Inicialización/carga de la capacidad nueva, default denegado y tolerancia a columna ausente. Consumidores previos preservados. |
| Contexto de solicitud | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionContextAdapter.vb`, `PerfilImportacionRequestGuard.vb` | Empresa RA, permiso vigente y comprobaciones HTTP; consumidos por fachada/página. No reglas de extracción. |
| Fachada | `webservice/WebServiceImportacionCarpetas.asmx`, `.asmx.vb`, `DTOs/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionDtos.vb` | Contrato común 07; servicios reciben contexto explícito. No bytes ni lotes. |
| Página | `Gestion/WebFormProducionDocumental.aspx`, `.aspx.vb`, `.aspx.designer.vb` si controles server lo requieren | Acceso/controles propios y emisión autorizada de token. Page_Load ya registra inicialización JS; respetar su ciclo y UpdatePanel. Designer generado por herramientas del proyecto, sin edición especulativa. |
| Cliente | `js/gestion/WebFormProducionDocumental.js`, `js/gestion/ImportacionCarpetas/PerfilImportacionApi.js`, `PerfilImportacionEditor.js` | Integrar acceso conservando árbol; API maneja cabecera/token/sobre; editor maneja estado y controles. Estilos del editor acotados a sus controles; no alterar selectores globales. |
| Servicios/modelo | Rutas exactas del inventario de `design.md` | Reglas puras, validación, publicación y modelos/puertos; no leer HttpContext dentro del resolvedor. |
| Persistencia | Repositorio y adaptador de catálogos del inventario de `design.md` | Origen RA, consultas autorizadas, CAS y transacciones propias; esquema SQL 01 manual. |
| Proyecto | `GestionDocumental-Docuarchi.net.vbproj` | Compile para VB; Content para ASMX/JS/recursos necesarios. No incluir pruebas, secretos ni evidencia en sitio publicado. |
| Asignación de permiso | Aplicación externa confirmada por el usuario | Contrato en contrato-administracion-externa.md; sin archivo productivo local de asignación. No inventar nombre/API externos. |

La ubicación externa está confirmada. La entrega local puede desarrollarse con el contrato documentado; la aceptación conjunta sigue abierta y no se declara integración externa probada.

## Arnés real observado y rutas propuestas

`tests/document-streaming.test.cjs` compila componentes VB con `vbc.exe`, un runner C# con `csc.exe` y ejecuta aserciones de comportamiento. `tests/doc87-registro-tarea-ruta-sii-behavior.test.cjs` usa MSBuild del proyecto y luego un ejecutable C# para probar servicios con puertos sin abrir bases.

Reutilizar esos patrones de compilación/ejecución y node:test; no introducir xUnit/NUnit/.NET moderno por preferencia ni modificar pruebas SII. Que exista un arnés no demuestra que las nuevas dependencias compilen: la tarea 9.2 debe comprobarlo.

| Archivo nuevo propuesto | Verificación principal |
|---|---|
| `tests/doc110-perfil-importacion-behavior.test.cjs` | Orquestar compilación/ejecución de las pruebas de comportamiento; usar el patrón existente apropiado según dependencias reales. |
| `tests/PerfilImportacionReglasBehaviorTests.cs` | Fuentes/tipos/fechas/código/responsable/diferidos e integridad; dobles de catálogos explícitos. |
| `tests/PerfilImportacionServiceBehaviorTests.cs` | Autorización, revisiones, publicación, políticas y preservación de historial mediante puertos. No acreditar persistencia real con dobles. |
| `tests/PerfilImportacionRequestGuardBehaviorTests.cs` | Sesión/contexto/token y rechazo previo al servicio con contexto de prueba. |
| `tests/PerfilImportacionBehaviorTestRunner.cs` | Ejecutar las clases anteriores y devolver resultado inequívoco/fallo al runner Node. |
| `tests/doc110-perfil-importacion-client.test.cjs` | Transporte JS y estados de error sin login; dobles explícitos no sustituyen UI real. |
| `tests/doc110-perfil-importacion-mysql.integration.cjs` | Entrada para integración real de repositorio, solo con autorización y esquema preparado; concurrencia/CAS/publicación. |
| `tests/PerfilImportacionRepositoryIntegrationTests.cs` | Comportamiento persistido ejecutado por la entrada anterior; secretos efímeros, controles saneados. |
| `tools/e2e/tests/doc110-perfil-importacion.spec.cjs` | PER-01–03 usando infraestructura existente documentada en inventario E2E. |
| `tools/e2e/package.json` | Comando focal de suite; no proyecto/configuración/login nuevo. |

No ejecutar compilación o pruebas funcionales vacías para declarar estas capacidades listas. Ninguno de estos archivos de prueba fue generado ni ejecutado en la fase documental.
