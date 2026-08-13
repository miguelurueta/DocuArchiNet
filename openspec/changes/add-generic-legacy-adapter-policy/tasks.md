## 1. Perfil y comandos de inicio

- [x] 1.1 Crear el catálogo versionado de perfiles de arquitectura y registrar `enterprise-legacy-modernization` con sus requisitos, marcadores y tareas obligatorias.
- [x] 1.2 Extender el parser de `opsxj:new` para aceptar `--profile`, validar el valor antes de verificaciones Git, Jira, ramas o escritura de artefactos, y propagar el perfil por el flujo de creación.
- [x] 1.3 Agregar `opsxj:orchestrate:new` al script npm, uso, registro de comandos y mensajes de resultado, delegando en el mismo handler de `opsxj:new`.
- [x] 1.4 Documentar en `tools/opsxj/README.md` el uso opcional del perfil, el alias y los casos en que corresponde aplicarlo.
- [x] 1.5 Crear un catálogo independiente de perfiles tecnológicos para revisión (`legacy-webforms-vb`, `tooling-node`, `frontend-react-ts`, `generic`) y propagar `--tech-profile` por el flujo de creación sin mezclarlo con el perfil de arquitectura.
- [x] 1.6 Actualizar `opsxj:technical-review` para respetar el perfil tecnológico explícito o autodetectado y excluir reglas incompatibles con Web Forms/VB y tooling Node.

## 2. Generación de gobierno y artefactos

- [x] 2.1 Implementar builders reutilizables para insertar la política de modernización enterprise en propuesta, diseño, especificación y tareas, con marcadores deterministas por artefacto.
- [x] 2.2 Incluir en el contenido generado la separación Presentation/Application/Domain/Infrastructure, DTOs/modelos tipados, validación de servidor, convivencia gradual, piloto, rollback y pruebas de equivalencia.
- [x] 2.3 Incluir la política general de datos: infraestructura común reutilizable, repositorios por dominio, consultas parametrizadas y prohibición de un `GenericRepository` que mezcle dominios.
- [x] 2.4 Incluir la política de frontera legacy: cada capacidad modernizada define un Gateway/Adapter tipado y Presentation/Application no invocan directamente clases legacy, sesión, controles de interfaz, SQL legacy ni cambios de estado.
- [x] 2.5 Definir contratos documentales estrictos por archivo y clasificación de impacto: secciones obligatorias, marcadores de identidad y restricciones de cierre.
- [x] 2.6 Extender `opsxj-governance.json` de forma aditiva con el contrato documental y, cuando aplique, `architectureProfile` con nombre, versión y requisitos/marcadores; conservar el manifiesto sin perfil para cambios normales.

## 3. Validación de perfil y compatibilidad

- [x] 3.1 Extender `opsxj:validate` para validar bloqueantemente toda documentación declarada por un manifiesto: existencia, secciones, contenido mínimo, identidad y restricciones de cierre.
- [x] 3.2 Rechazar documentación con archivos vacíos, `TBD`, instrucciones/comentarios de plantilla o checklists abiertos, e identificar el archivo y la regla exacta.
- [x] 3.3 Extender `opsxj:validate` para comprobar, cuando exista `architectureProfile`, sus artefactos, marcadores, tareas, documentos y evidencia declarados.
- [x] 3.4 Emitir un check independiente y accionable por cada incumplimiento documental o de perfil, sin modificar Jira, GitHub ni código de negocio.
- [x] 3.5 Preservar compatibilidad de validación para cambios históricos sin manifiesto y para cambios nuevos creados sin `--profile`.

## 4. Pruebas automatizadas

- [x] 4.1 Agregar pruebas del parser de perfil: perfil válido, perfil desconocido y garantía de que un perfil inválido no consulta Jira ni ejecuta Git, ramas o transiciones externas.
- [x] 4.2 Agregar pruebas de paridad entre `opsxj:new` y `opsxj:orchestrate:new`, incluidos `--impact` y `--profile`.
- [x] 4.3 Agregar pruebas de generación que verifiquen contenido de propuesta, diseño, especificación, tareas y bloque `architectureProfile` del manifiesto.
- [x] 4.4 Agregar pruebas de validación documental estricta: archivo faltante o vacío, sección faltante, metadato inconsistente, `TBD`, plantilla/instrucción y checklist abierto.
- [x] 4.5 Agregar pruebas de validación para perfil completo, marcador o artefacto faltante, tareas pendientes y evidencia faltante.
- [x] 4.6 Agregar pruebas de no regresión para manifiestos históricos y cambios creados sin perfil.
- [x] 4.7 Agregar pruebas que demuestren que un prompt Web Forms/VB y uno de tooling Node no reciben hallazgos React/TypeScript, y que un perfil explícito tiene precedencia sobre la autodetección.

## 5. Verificación y cierre técnico

- [x] 5.1 Ejecutar la suite de `tools/opsxj` y registrar el comando y resultado.
- [x] 5.2 Ejecutar validación OpenSpec estricta para este cambio y corregir hallazgos.
- [x] 5.3 Revisar el diff para confirmar que el perfil sigue siendo genérico y no contiene referencias obligatorias a Workflow ni a una clase legacy concreta.
