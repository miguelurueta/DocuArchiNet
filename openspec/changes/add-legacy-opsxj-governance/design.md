## Context

El `opsxj` actual es un tooling Node local que orquesta Jira, GitHub y OpenSpec. No existe un contrato que traduzca el impacto de páginas WebForms, code-behind, VB.NET, handlers o base de datos en documentación y validación. Ver `proposal.md` y la especificación para la motivación y el comportamiento esperado.

## Goals / Non-Goals

**Goals:**

- Centralizar el catálogo de impacto en un módulo declarativo sin acoplarlo a la aplicación WebForms.
- Generar documentos técnicos y un manifiesto por cambio durante `opsxj:new`.
- Validar de forma local y determinista tareas, revisión, documentos y evidencia asociada al commit actual.
- Preservar el flujo existente para cambios históricos y mantener los comandos remotos como acciones explícitas.

**Non-Goals:**

- No agregar React, Vite, Playwright ni una segunda herramienta de orquestación.
- No requerir automatización E2E para WebForms cuando el entorno no la provee.
- No ejecutar transiciones Jira, creación de PR o escritura remota desde CI o desde `opsxj:validate`.
- No reescribir los cambios OpenSpec existentes para añadir manifiestos retroactivos.

## Decisions

1. **Manifiesto junto al cambio OpenSpec.** Cada cambio nuevo recibirá `opsxj-governance.json` en su carpeta OpenSpec, con la clasificación, documentación y evidencia requeridas. Así el contrato viaja con el cambio y no depende de una base central mutable.
   - Alternativa descartada: inferir siempre por nombres de archivos o etiquetas Jira. Es ambiguo y no permite revisión explícita.

2. **Documentación técnica en `Doc/Tecnica/Opsxj/<change-name>`.** El generador crea solo las secciones requeridas por el catálogo. Este directorio pertenece al proyecto legacy y permite auditar el ticket sin inventar estructura de un repo React.
   - Alternativa descartada: incrustar toda la documentación en `proposal.md`. Pierde separación entre decisión, implementación, contratos y evidencia.

3. **Evidencia local por ticket y SHA.** `opsxj:validation:evidence` guarda un único resultado vigente por tipo bajo `.opsxj/evidence/<ISSUE>.json`; `opsxj:validate` la considera válida únicamente si coincide con el SHA evaluado.
   - Alternativa descartada: aceptar evidencia sin SHA. Podría aprobar un cambio que ya no coincide con las pruebas registradas.

4. **Compatibilidad histórica explícita.** La ausencia de manifiesto devuelve una validación aplicable con éxito y mensaje de compatibilidad. Solo los cambios creados con el nuevo flujo son sujetos a sus requisitos.
   - Alternativa descartada: bloquear cambios antiguos. Interrumpiría archivo y mantenimiento de trabajo ya existente.

5. **Acciones remotas separadas.** Validación y CI son de solo lectura. `opsxj:archive` y `opsxj:close` conservan su carácter explícito y deberán consultar la compuerta antes de ejecutar acciones remotas.
   - Alternativa descartada: CI que transicione Jira o cree PR. Eso expone operaciones irreversibles a ejecuciones automáticas.

## Risks / Trade-offs

- [Documentación generada como plantilla] → Mitigación: la validación verifica su existencia; el responsable del ticket completa su contenido y registra evidencia.
- [QA WebForms no automatizable en todos los ambientes] → Mitigación: permitir evidencia `manual_qa` con pasos, ambiente y referencia reproducible.
- [Un commit posterior invalida evidencia anterior] → Mitigación: la comparación de SHA obliga a actualizar evidencia antes de archivar.
- [Cambio de formato de manifiesto] → Mitigación: versionar el JSON y mantener lectura tolerante para cambios históricos.

## Migration Plan

1. Incorporar catálogo, manifiesto, generación documental, evidencia y pruebas unitarias dentro del `opsxj` existente.
2. Publicar los comandos locales y actualizar el README y el ejemplo de entorno, sin cambiar el código WebForms.
3. Integrar la validación como compuerta de archivo solo cuando exista manifiesto.
4. Añadir CI de lectura para ejecutar pruebas del tooling y validación OpenSpec, sin credenciales de escritura.
5. Ejecutar un ticket de ensayo real hasta `opsxj:validate`; documentar la evidencia manual o automatizada disponible en el entorno legacy.

## Ticket de ensayo real — 2026-08-13

- Ticket usado: `DOC-2` (`doc-2-infraestructura-visual-aislada-reversible`), un cambio WebForms transversal con manifiesto, paquete técnico y pruebas manuales reales ya archivadas.
- Para reproducir la compuerta que debe ocurrir antes del archivo, el cambio se reactivó temporalmente en el commit local `ec4aa0e18d8ab4831d4d5e071b4e33e8e75b0e1d`; no hubo cambios funcionales, de Jira, GitHub ni despliegue.
- `npm.cmd --prefix tools/opsxj test` terminó con 12 archivos y 94 pruebas correctas. Se registraron las evidencias `unit` y `manual_qa` por `opsxj:validation:evidence`, ambas asociadas a ese SHA.
- Con la revisión OpenSpec confirmada, `opsxj:validate DOC-2 --json` devolvió `PASS`: seis documentos requeridos, cero tareas pendientes, revisión confirmada y evidencia vigente para ambos tipos exigidos.
- WebForms no dispone de una suite E2E automatizada en este repositorio. La evidencia disponible es QA manual reproducible contra IIS local, documentada en `Doc/Tecnica/Opsxj/doc-2-infraestructura-visual-aislada-reversible/05-PruebasEvidencia.md`: recorridos de 1366, 1024, 768 y 375 px, hover, foco, estado deshabilitado, menú abierto y documento seleccionado. Esta limitación se declara explícitamente; no se simula un reporte E2E.
- Tras la validación, DOC-2 vuelve a archivarse. La evidencia conservada corresponde al SHA exacto del ensayo, aunque el commit posterior solo restituye su ubicación de archivo y registra este resultado.

## Open Questions

- El ticket de ensayo real y su alcance requieren selección explícita del equipo antes de efectuar cambios remotos o de proyecto.
