# DOC-111 — Refinamiento inicial

Base: main 6313f91f, rama feature/DOC-111, worktree .tmp/doc111-work. Fecha 2026-10-27. Alcance de esta sesión: revisión estática y propuesta de contratos/tareas, sin implementación, SQL, autenticación o efectos documentales.

## Entregables

El cambio doc-111-produccion-importacion-carga-carpeta contiene propuesta concreta, refinamiento en draft, diseño, 12 requisitos con 25 escenarios y 41 tareas. Dos tareas de lectura/redacción están completas; 39 corresponden a aprobación, implementación y aceptación. Se conserva jira-context.md original.

Diseno/design.md fija archivos exactos nuevos/existentes, propietarios y consumidores; Diseno/15-integracion-carga-doc111.md precisa la relación con contratos comunes. Los cinco documentos OPSXJ explican el alcance propuesto y no presentan plantillas como implementación. El orden evita dependencias circulares: 04 → 07-A → 06 → 07-B → 08/09 → 10 → integración 05/11 → 12. El 13 queda fuera salvo nueva activación.

## Hallazgos del código

- DOC-110 está fusionado, pero ServicioPerfilImportacion.Seleccionar solo admite ADMINISTRACION; contexto/guard/repositorio y proveedor también exigen administración. Preparar requiere una vía operativa por propósito sin fingir PuedeAdministrar, sin configuración paralela y sin ampliar la autoridad de endpoints administrativos.
- ActivaCargaArchivos e IniLoadPerson tienen contrato individual; no representan identidad de raíces/archivos de una operación multiexpediente.
- Registrar_Expediente_Conservacion recibe nivel padre y relación de expediente; su wrapper recibe controles WebForms. Un ID aislado no demuestra ubicación o correlación tras crash.
- AlmacenamientoDocumentoProduccionDocumental toma File.Name y elimina temporales. Nombre/extensión, copia consumible, staging recuperable, sesión, exclusión frente a escritores ajenos, Multi-TIFF e incorporación/índice requieren caracterización antes de habilitar efectos.
- No se localizaron componentes operativos de plan/lote/recepción/coordinación de carpetas en las áreas inspeccionadas. Los componentes Workflow/SII no se consideran una implementación reutilizable mediante tareas ficticias.

Cuotas/TTL/retención deben ser política servidor válida antes de habilitación; no se inventan valores desde la maqueta. Formatos/navegadores se anuncian después de comprobarlos. Sin correlación fiable, INCIERTO y sin replay. La empresa única sigue siendo el alcance real; no elimina defensas de contexto. Las autorizaciones DOC-110 no se trasladan automáticamente a nuevas pruebas con efectos DOC-111.

## Comprobaciones

`openspec.cmd validate doc-111-produccion-importacion-carga-carpeta --strict`: PASS.

`npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-111 --sync`: sincroniza encabezados y comprueba existencia, decisiones, requisitos, referencias de design/spec/tasks y origen de todas las tareas. Todos esos controles PASS; único rechazo: refinement:approved, porque el marcador permanece draft hasta confirmación de la persona responsable. No se cambió a approved para obtener un resultado verde.

No se ejecutaron pruebas funcionales nuevas, compilación de componentes no implementados ni corridas autenticadas. Esto no certifica importación, persistencia operativa, reconciliación legacy o capacidad anunciada. La revisión del usuario es el siguiente paso antes de comenzar código por las fundaciones y autoridad operativa.
