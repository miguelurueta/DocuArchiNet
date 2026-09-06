## Context

DOC-50: CONTRATOS-MULTI-PROVEEDOR

## Jira Details

> Prompt backend 01 — Contratos, contexto y registro multiproveedor
> Actúa como implementador senior de   sobre .NET Framework 4.6.1. Lee completas las dos exploraciones backend y funcional, inspecciona el código vigente y crea o continúa un cambio OpenSpec antes de modificar código productivo.
> Publica el contrato canónico exigido por ../CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md; ninguna integración frontend productiva puede adelantarse a este artefacto.
> Objetivo
> Crear el núcleo contractual de ImportarServicioWeb, independiente de HttpContext y de conceptos SII, con contexto autorizado e inmutable y resolución explícita de proveedores.
> Investigación obligatoria
> Confirmar qué proveedores están configurados además de INTEGRACIONSII.
> 
> Inventariar los endpoints que leen o escriben sesión y documentar cuáles pueden recibir contexto explícito.
> 
> Identificar la fuente autorizada de usuario, tarea, ruta, trámite, permisos y proveedor habilitado.
> 
> Implementa
> Contratos versionados para contexto, capacidades, consulta, preflight, intención, comando documental, resultado por elemento y reconciliación.
> 
> Definición completa de las ocho operaciones lógicas compartidas, incluyendo transporte, DTO, nulabilidad, códigos, autorización, idempotencia, concurrencia, ejemplos saneados y fixtures.
> 
> Un contexto inmutable con usuario autenticado, tarea, ruta, trámite, proveedor y datos necesarios de autorización, capturado antes de cualquier Await.
> 
> Un registro IExternalImportProvider por identidad canónica configurada.
> 
> Resultado explícito para proveedor no soportado; nunca resolver SII por defecto.
> 
> Validaciones comunes reutilizables que revaliden permiso vigente, tarea operable, ruta, trámite y proveedor.
> 
> Composición desacoplada de WebForms/ASMX y pruebas focales sin sesión ni red.
> 
> Rutas canónicas de código y contratos
> Crear únicamente la estructura aditiva siguiente:
> DTOs/Workflow/ImportarServicioWeb/
> └── ImportarServicioWebDtos.vb
> 
> Modelo/Workflow/ImportarServicioWeb/
> ├── ImportarServicioWebModels.vb
> └── ImportarServicioWebInterfaces.vb
> 
> Services/Workflow/ImportarServicioWeb/
> ├── ServicioImportarServicioWeb.vb
> ├── RegistroProveedoresImportacion.vb
> └── ValidadorContextoImportacion.vb
> 
> Tests/
> ├── importar-servicio-web-contracts.test.cjs
> ├── importar-servicio-web-provider-registry.test.cjs
> └── importar-servicio-web-context.test.cjs
> 
> Tests/Fixtures/Workflow/ImportarServicioWeb/contracts-v1/
> ├── resolve-capabilities-request.json
> ├── resolve-capabilities-response.json
> ├── query-items-response.json
> ├── preflight-import-response.json
> ├── create-import-intent-response.json
> ├── execute-import-intent-response.json
> ├── get-import-intent-response.json
> └── reconcile-import-intent-response.jsonResponsabilidad obligatoria por ruta:
> Ruta
> Contenido permitido
> DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb 
> Clases públicas <Serializable()> de request/response para las ocho operaciones; schemaVersion, nulabilidad, errores seguros e identificadores de correlación. No contiene reglas, SQL, sesión ni transporte. 
> Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb 
> Modelos internos e invariantes: contexto inmutable, proveedor, capacidades, identidad externa, plan, intención, elemento, fase y resultado. No contiene DTO de endpoint ni referencias a WebForms. 
> Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb 
> Puertos IExternalImportProvider, registro/resolución, autorización, reloj y repositorios futuros. No contiene implementaciones ni acceso a HttpContext. 
> Services/Workflow/ImportarServicioWeb/ServicioImportarServicioWeb.vb 
> Fachada de aplicación para capacidades y consulta contractual de esta entrega; recibe contexto explícito mediante constructor/parámetros. No ejecuta importaciones todavía. 
> Services/Workflow/ImportarServicioWeb/RegistroProveedoresImportacion.vb 
> Registro explícito por identidad canónica, sin fallback a SII. 
> Services/Workflow/ImportarServicioWeb/ValidadorContextoImportacion.vb 
> Validaciones comunes de usuario, tarea, ruta, trámite y proveedor sobre datos explícitos. 
> Tests/*.test.cjs 
> Verificaciones estructurales y contractuales coherentes con la infraestructura actual del repositorio. 
> Tests/Fixtures/Workflow/ImportarServicioWeb/contracts-v1/ 
> Ejemplos JSON saneados que consumirán frontend y backend como fuente contractual compartida. 
> Reutilizar Domain/Shared/ContextoModulo.vb y ContextoModuloWorkflow como fuente común cuando sus invariantes sean suficientes. Si se necesita contexto adicional, componer ContextoImportacionServicio en ImportarServicioWebModels.vb; no modificar ni duplicar ContextoModuloWorkflow.
> Agregar cada archivo .vb nuevo a GestionDocumental-Docuarchi.net.vbproj mediante entradas <Compile Include="..." />, respetando el agrupamiento existente de DTOs, Modelo y Services. No mover ni reemplazar entradas actuales.
> Queda prohibido definir estos contratos en:
> webservice/*.asmx.vb;
> 
> App_Code/;
> 
> ServiciosIntegracion/;
> 
> Integracionccv/;
> 
> workflow/;
> 
> Infrastructure/;
> 
> archivos de Terminar, Notas, Devolver o DevolverUsuarioAnterior;
> 
> el propio GestionDocumental-Docuarchi.net.vbproj mediante código embebido.
> 
> Los ASMX y adaptadores de infraestructura de prompts posteriores consumirán los contratos desde estas rutas; no crearán copias locales.
> Restricciones
> La implementación es aditiva y paralela al backend vigente; no reemplaza ni modifica sus clases, endpoints o recorridos.
> 
> No modificar AlmacenaDocumentoTareaWorkflow(...); el núcleo solo definirá la frontera necesaria para reutilizarla posteriormente.
> 
> No incluir libro, registro, matrícula, acto, noticia, código de barras ni cachés SII en los contratos comunes.
> 
> No aceptar tarea, usuario, gabinete, tipología o proveedor del navegador como autoridad.
> 
> No cambiar todavía persistencia ni ejecutar efectos de importación.
> 
> No romper las firmas ASMX existentes en esta entrega.
> 
> No crear un segundo DTO o interfaz con el mismo significado fuera de las rutas canónicas declaradas.
> 
> Aceptación
> Un proveedor conocido se resuelve únicamente por su identidad registrada.
> 
> Un proveedor desconocido falla de forma segura y tipada.
> 
> El contexto no cambia aunque la sesión seleccionada cambie después de capturarlo.
> 
> Las reglas de autorización pueden probarse con dobles deterministas.
> 
> Los contratos tienen schemaVersion o una estrategia explícita de evolución compatible.
> 
> Los fixtures contractuales se consumen desde pruebas frontend y backend y detectan deriva entre ambos lados.
> 
> Una verificación estructural confirma que cada contrato reside en su ruta canónica y que ningún ASMX contiene definiciones duplicadas.
> 
> Trazabilidad
> Exploración backend: secciones 5, 6, 13, 15 y 20; hallazgos B-01, B-02 y B-03; preguntas abiertas 2 y 10.
> Correcciones opsxj:prompt-review
> Estas reglas fueron agregadas desde opsxj:prompt-review para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.
> Ruta documental obligatoria
> Este cambio crea un núcleo compartido y reutilizable. Toda su documentación técnica debe quedar exclusivamente en:
> docs/Architecture/Workflow/ImportarServicioWeb/SCRUMCORE-000-contratos-contexto-registro-multiproveedor/Sustituir SCRUMCORE-000 por el identificador real del ticket antes de implementar. No crear el paquete en Doc/Actualizacion, docs/modulos, docs/Components, la raíz del repositorio ni una ruta paralela. Los documentos de Doc/Actualizacion/workflow/ImportarServicioWeb/ son fuentes de exploración y prompts, no el paquete técnico entregable del cambio.
> Paquete documental mínimo
> Crear en la ruta canónica exactamente:
> 00-Indice.md
> 01-Arquitectura.md
> 02-FlujoIntegracion.md
> 03-ContratoUploadYMapping.md
> 04-EstadosErroresYAntiregresion.md
> 05-PruebasEvidencia.md
> 06-Diagramas.md
> 07-Metadata.md
> Diagramas/00-Indice.md: objetivo, alcance, componentes, servicios, adaptadores, dependencias y listado documental.
> 
> 01-Arquitectura.md: implementación paralela, fronteras frontend/backend, contexto inmutable, registro multiproveedor, coexistencia ASMX y reutilización intocable de AlmacenaDocumentoTareaWorkflow(...).
> 
> 01-Arquitectura.md debe incluir además la tabla de rutas canónicas de código, dependencias permitidas y direcciones de referencia entre DTOs, Modelo, Services e infraestructura futura.
> 
> 02-FlujoIntegracion.md: las ocho operaciones lógicas, requests, validación, respuestas, dependencias y secuencia sin mutación de este primer cambio.
> 
> 03-ContratoUploadYMapping.md: aunque conserva el nombre canónico, documenta DTO, nulabilidad, versionado, request/response, mapeos y fixtures del contrato de importación; debe declarar que este prompt no carga ni almacena documentos.
> 
> 04-EstadosErroresYAntiregresion.md: estados, errores seguros, proveedor desconocido, sesión mutable, compatibilidad pública y garantías de no modificación del recorrido vigente.
> 
> 05-PruebasEvidencia.md: pruebas focales, fixtures compartidos, comandos, resultados, limitaciones y evidencia de que no hubo efectos de importación.
> 
> 06-Diagramas.md: componentes, dependencias, secuencia, flujo principal/alterno, casos de uso y estados mediante Mermaid; los diagramas individuales van en Diagramas/.
> 
> 07-Metadata.md: identificador SCRUMCORE, rama, fecha, estado, archivos, versión contractual, prompts, dependencias, riesgos y deuda técnica.
> 
> No duplicar estos documentos en otra ruta. Todos los enlaces del paquete deben ser relativos y verificables.
> Tabla documental de funciones
> 01-Arquitectura.md o 02-FlujoIntegracion.md debe incluir para cada función creada o modificada:
> Función
> Ruta
> Clase/interfaz
> Parámetros
> Responsabilidad
> Nueva/existente
> La tabla debe demostrar que AlmacenaDocumentoTareaWorkflow(...) no fue modificada.
> Rol esperado
> Definir el rol tecnico esperado para ejecutar el ticket.
> Objetivo
> Describir el objetivo funcional y tecnico verificable.
> Restricciones criticas
> No introducir cambios fuera del alcance declarado.
> 
> No romper comportamiento existente ni contratos publicos.
> 
> Criterios de aceptacion
> El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.
> 
> Contexto obligatorio
> Leer Domain/Shared/ContextoModulo.vb, Modelo/Workflow/Terminar/WorkflowModernModels.vb, Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb, DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb, Services/Workflow/Terminar/ServicioTransicionTarea.vb y las entradas correspondientes de GestionDocumental-Docuarchi.net.vbproj. Usarlos solo como referencia de convenciones; no modificarlos ni acoplar ImportarServicioWeb con Terminar.
> Pruebas obligatorias
> Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.
> Documentacion tecnica
> Crear y actualizar únicamente el paquete definido en Ruta documental obligatoria, con los ocho documentos, la carpeta Diagramas/, enlaces relativos válidos y contenido coherente con el código y las pruebas realmente entregados.
> Entregable final
> Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.
> Requisitos positivos
> Implementar el comportamiento esperado con contratos tipados y responsabilidades claras.
> 
> Mantener la integracion sobre los puntos de extension existentes del repo.
> 
> Dejar evidencia de pruebas y documentacion tecnica actualizada.
> 
> Exigir npm run build o tsc segun impacto y registrar el resultado.
> Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. Las decisiones funcionales y tecnicas se completan durante `opsxj:refine`; no se inyectan politicas de otro perfil tecnologico.


## Risks / Trade-offs

- El refinamiento debe identificar compatibilidad, riesgos y limites del modulo afectado antes de iniciar cambios.

## Migration Plan

1. Completar y aprobar `refinement.md` antes de marcar tareas de implementacion.
2. Sincronizar cada decision con design, spec y tasks mediante `opsxj:refine --sync`.

## Open Questions

- TBD
