<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->

# Diseño técnico — DOC-50 contratos multiproveedor

## Contexto

El recorrido actual de ImportarServicioWeb está acoplado a Web Forms/ASMX, estado de `Session` y ramas específicas de `INTEGRACIONSII`. DOC-50 crea una frontera contractual paralela sobre .NET Framework 4.6.1. No reemplaza endpoints, no consulta proveedores reales y no almacena documentos.

La inspección estática solo demuestra un proveedor implementado explícitamente: `INTEGRACIONSII`. La tabla de servicios puede contener otras identidades en datos, pero no se declararán como soportadas sin configuración y adaptador comprobados.

## Metas y no metas

### Metas

- Publicar contratos v1 compartidos para las ocho operaciones lógicas.
- Capturar un contexto autorizado e inmutable antes de cualquier `Await`.
- Resolver proveedores mediante registro explícito y resultados tipados.
- Separar DTOs, modelo y servicios de `HttpContext`, ASMX, SQL y red.
- Probar contratos, contexto y registro con fixtures deterministas.

### No metas

- Modificar firmas o comportamiento ASMX.
- Ejecutar preflight, intenciones o almacenamiento documental reales.
- Modificar `AlmacenaDocumentoTareaWorkflow(...)`.
- Crear adaptadores HTTP, persistencia, cachés o un gate productivo.
- Afirmar soporte productivo de proveedores distintos de `INTEGRACIONSII`.

## Arquitectura y dependencias

```text
DTOs/Workflow/ImportarServicioWeb
        ^ mapeo en frontera futura
        |
Services/Workflow/ImportarServicioWeb
        |
        v
Modelo/Workflow/ImportarServicioWeb
        |
        +--> IExternalImportProvider (adaptadores futuros)
        +--> autorización/repositorios/reloj (implementaciones futuras)
```

Las dependencias apuntan hacia el modelo. DTOs no contienen reglas; Modelo no conoce transporte; Services orquesta puertos explícitos. Web Forms/ASMX e infraestructura futura pueden depender de esta frontera, pero la frontera nunca depende de ellos.

## Decisiones

### D-01 — Contrato v1 único y versionado

`ImportarServicioWebDtos.vb` contendrá clases públicas `<Serializable()>` para las ocho operaciones canónicas: `ResolveCapabilities`, `QueryItems`, `GetPreview`, `PreflightImport`, `CreateImportIntent`, `ExecuteImportIntent`, `GetImportIntent` y `ReconcileImportIntent`. El comando y resultado documental por elemento serán contratos auxiliares de preflight/intención, no una novena operación. Todas las respuestas incluirán `SchemaVersion`, `OperationId`, `CorrelationId` y error seguro cuando aplique; requests incluirán `TaskId`, `ProviderId` y `ExternalKey` cuando la operación los requiera. Los fixtures JSON de `contracts-v1` serán ejemplos ejecutables y fuente de detección de deriva. No se incluirán campos SII como libro, matrícula, acto, noticia o código de barras.

La frontera futura se publica como contrato `v1` sobre HTTPS `POST`, con métodos ASMX modernos homónimos bajo `/webservice/WebServiceImportarServicioWeb.asmx/<Operation>`. DOC-50 define esa dirección contractual pero no crea ni modifica el ASMX. Valores ausentes usarán `Nothing` solo cuando la operación los declare opcionales; colecciones de respuesta se inicializarán vacías. Los códigos funcionales mínimos son `INVALID_CONTEXT`, `FORBIDDEN`, `TASK_NOT_OPERABLE`, `ROUTE_MISMATCH`, `PROCEDURE_MISMATCH`, `PROVIDER_NOT_SUPPORTED`, `EXTERNAL_ITEM_NOT_FOUND`, `CONCURRENCY_CONFLICT`, `TIMEOUT` e `INTERNAL_ERROR`, sin mensajes o detalles sensibles.

`ResolveCapabilities`, `QueryItems`, `GetPreview`, `PreflightImport`, `GetImportIntent` y `ReconcileImportIntent` son idempotentes por lectura. `CreateImportIntent` usa una clave de idempotencia por tarea, proveedor y selección; `ExecuteImportIntent` exige token de versión y rechaza reejecución concurrente. Los timeouts son declarativos por proveedor/capacidad en v1: DOC-50 no fija un valor global ni implementa clientes HTTP. Compatibilidad: `SchemaVersion = "1.0"`, campos aditivos opcionales dentro de v1 y nueva versión para cambios incompatibles.

### D-02 — Contexto como instantánea inmutable

`ContextoImportacionServicio` compondrá, sin modificar `ContextoModuloWorkflow`, usuario autenticado, grupo, login, tarea, ruta, trámite, proveedor y permisos requeridos. Sus valores se fijarán por constructor, se expondrán como solo lectura y se validarán antes de pasar a servicios o proveedores. Ningún archivo nuevo leerá `HttpContext.Current` o `Session`.

### D-03 — Registro explícito sin fallback

`RegistroProveedoresImportacion` recibirá una colección de `IExternalImportProvider`, normalizará la identidad canónica de forma consistente y rechazará entradas nulas, vacías o duplicadas. La resolución devolverá un resultado explícito; una identidad desconocida producirá `PROVIDER_NOT_SUPPORTED`. Nunca se instanciará SII como valor por defecto.

### D-04 — Autorización reutilizable y determinista

`ValidadorContextoImportacion` coordinará puertos inyectables para revalidar usuario, permiso vigente, tarea operable, coincidencia de ruta y trámite y proveedor habilitado. La primera falla detendrá el flujo con un código seguro. Los identificadores enviados por cliente solo podrán usarse para localizar y comparar; la autoridad será el contexto construido en servidor y los repositorios autorizados.

### D-05 — Fachada sin efectos en esta entrega

`ServicioImportarServicioWeb` expondrá únicamente resolución de capacidades y consulta contractual usando contexto explícito y proveedor registrado. Preflight e intenciones se publican como contrato para entregas posteriores, pero DOC-50 no ejecutará red, SQL, caché, escritura documental ni `AlmacenaDocumentoTareaWorkflow(...)`. Los ASMX y recorridos existentes permanecen intactos.

### D-06 — Ubicación canónica y compilación

Se crearán exactamente los archivos VB indicados bajo `DTOs/Workflow/ImportarServicioWeb`, `Modelo/Workflow/ImportarServicioWeb` y `Services/Workflow/ImportarServicioWeb`. Cada archivo se agregará a `GestionDocumental-Docuarchi.net.vbproj` con `Compile Include` en su agrupación. Una prueba estructural impedirá copias en ASMX, `App_Code`, `ServiciosIntegracion`, `Integracionccv`, `workflow`, `Infrastructure` o módulos Workflow no relacionados.

### D-07 — Verificación focal sin sesión ni red

Las pruebas CommonJS cubrirán forma y versionado contractual, fixtures, registro conocido/desconocido/duplicado e inmutabilidad y validación del contexto. Se ejecutará el build disponible para el proyecto y se registrará evidencia real. No se ejecutará E2E autenticado, carga ni activación de gates dentro de DOC-50; cualquier corrida posterior necesitará autorización expresa y su runbook.

### D-08 — Paquete técnico único

La documentación vivirá exclusivamente en `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-50-contratos-contexto-registro-multiproveedor/`, según la ubicación confirmada para la entrega. Contendrá `00-Indice.md` a `07-Metadata.md` y `Diagramas/`, con enlaces relativos. La tabla de funciones mostrará explícitamente que `AlmacenaDocumentoTareaWorkflow(...)` es existente y no fue modificada.

## Operaciones contractuales

| Operación | Transporte futuro | Autoridad | Mutación en DOC-50 |
| --- | --- | --- | --- |
| ResolveCapabilities | POST ASMX v1, request/response | Contexto y registro | No |
| QueryItems | POST ASMX v1, request/response | Contexto y proveedor | No; solo puerto/doble |
| GetPreview | POST ASMX v1, descriptor/stream mediado | Tarea, proveedor, elemento y operación | No |
| PreflightImport | POST ASMX v1, request/response | Contexto y validador | No |
| CreateImportIntent | POST ASMX v1, request/response | Contexto e idempotencia futura | No en DOC-50 |
| ExecuteImportIntent | POST ASMX v1, request/response | Intención y token de versión | No en DOC-50 |
| GetImportIntent | POST ASMX v1, request/response | Contexto e identidad futura | No |
| ReconcileImportIntent | POST ASMX v1, request/response | Estado persistido futuro | No |

El comando y resultado documental por elemento forman parte del modelo auxiliar de `PreflightImport` y del estado de intención; no son una operación transportable independiente.

## Riesgos y compensaciones

- La configuración real de proveedores no puede demostrarse solo con código. Se compensa registrando únicamente implementaciones suministradas y fallando cerrado.
- VB sobre .NET Framework 4.6.1 limita algunas construcciones modernas de inmutabilidad. Se usarán campos privados y propiedades `ReadOnly`, sin setters públicos.
- Los fixtures podrían divergir de consumidores futuros. Se compensa consumiéndolos desde pruebas estructurales de ambos lados y versionando cambios incompatibles.
- Publicar contratos de operaciones aún no ejecutables puede sugerir capacidad inexistente. Las respuestas de capacidades distinguirán lo soportado y la documentación declarará el alcance sin efectos.

## Plan de implementación y reversión

1. Crear modelos e interfaces internos, sin referencias Web Forms.
2. Crear DTOs v1 y fixtures compartidos.
3. Implementar registro, validador y fachada de capacidades/consulta.
4. Registrar únicamente los archivos nuevos en el proyecto.
5. Agregar pruebas focales y paquete técnico DOC-50.
6. Ejecutar pruebas y build permitidos; registrar resultados y limitaciones.

La reversión consiste en retirar los archivos, fixtures, documentación y entradas `Compile Include` añadidos por DOC-50. No exige migración de datos ni restauración de endpoints porque el recorrido legacy no se modifica.

## Preguntas resueltas y deuda posterior

- Proveedores comprobados en código: solo `INTEGRACIONSII`; otros requieren evidencia de datos y adaptadores.
- La fuente autorizada se abstrae detrás de puertos; el navegador y la sesión mutable no son autoridad dentro del núcleo.
- Timeouts, tamaños, clientes HTTP, persistencia de intenciones, ejecución, reconciliación real y adaptación ASMX corresponden a cambios posteriores.
