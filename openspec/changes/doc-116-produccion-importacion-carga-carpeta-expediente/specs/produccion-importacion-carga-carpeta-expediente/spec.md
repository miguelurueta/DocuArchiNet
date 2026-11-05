## ADDED Requirements
### Requirement: PRODUCCION-IMPORTACION-CARGA-CARPETA-EXPEDIENTE
El sistema SHALL implementar el alcance definido para DOC-116.
#### Scenario: Flujo principal
- **WHEN** se ejecuta el caso de uso principal del ticket
- **THEN** el comportamiento coincide con las reglas funcionales esperadas
#### Scenario: No-regresion
- **WHEN** se valida el modulo afectado
- **THEN** no se rompen flujos existentes
## Perfil tecnologico

- Perfil de revision: legacy-webforms-vb (v1).
- Alcance: ASP.NET Web Forms, code-behind VB.NET/C# y JavaScript gradual.
- Este perfil orienta la revision tecnica; no reemplaza los requisitos funcionales ni arquitectonicos del ticket.

### Requirement: Detalle funcional Jira
El sistema SHALL considerar las reglas detalladas del ticket.

#### Scenario: Reglas del ticket
- Alcance refinado
- Revisión de cobertura del adaptador de expedientes existente; cerrar solo brechas de contrato y aceptación. No implementar otro creador.
- Base y procedencia
- Revisión arquitectónica 2026-11-04 solicitada por el usuario frente a lo ya implementado. Base entregada: PR125 integrado en main, merge a3c7263763a89901ba78d05ccebc284a43a4cd76. DOC110–115 están Listo. Esta revisión es documental: no ejecuta nuevas pruebas ni certifica escenarios faltantes. Los resultados históricos conservan su SHA y ambiente; no equivalen a una nueva corrida del commit actual.
- Implementación existente comprobada
- Crear recibe contexto/raíz/lote/intento; usa autoridad y nivel fijados, inicio durable, operación legacy de conservación, captura de IDs y checkpoint en el repositorio único. El coordinador exige barrera y original verificado. Verificación por expediente/relación/ciclo Archivo Gestión, sin atribución por nombre. Fuentes diferidas se materializan después de confirmar el expediente. DOC111 ejecutó REC EXPEDIENTE/LOTE/PARCIAL con dos raíces y comprobación de IDs/relaciones; no equivale a los negativos EXP02/03.
- Rutas que deben reutilizarse
- https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb
- https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.Legacy.vb
- https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Creacion.vb
- https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.ResultadoCreacion.vb
- Evidencia existente y su alcance
- https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-adaptador-creacion-local-20261029.md
- https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-fase-creacion-local-20261029.md
- https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-verificacion-expediente-local-20261028.md
- https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-rec-expediente-pass-20261101.md
- Escenarios y trabajo pendiente
- EXP-01 — COBERTURA PARCIAL REAL/LOCAL: dos raíces bajo nivel7 e IDs/relaciones tienen evidencia REC; cambio de selección A→B y destino fijo tienen pruebas locales/UI. Verificar la combinación exacta en el recorrido autenticado antes de marcar EXP01 completo; reutilizar la parte ya probada.
- EXP-02 — LOCAL, aceptación específica pendiente: fallo tras creación y antes de checkpoint, respuesta perdida/IDs incompletos y reejecución de la misma raíz. La prueba debe reconciliar solo evidencia fiable o conservar INCIERTO sin otra creación; nunca buscar por nombre.
- EXP-03 — LOCAL, aceptación específica pendiente: revocar permiso del nivel antes de crear, comprobar rechazo y cero expedientes nuevos con SELECT. La revocación probada en DOC114 para PrepararPlan no sustituye este efecto.
- Contrato adicional — mapear homónimos en ámbitos distintos, códigos inválidos/límites funcionales, nivel movido y tipos/clasificación a las pruebas Framework existentes. Ejecutar solo lo no acreditado o afectado por un cambio. Caracterizar límites de coexistencia con creación manual sin prometer exclusión global.
- Criterio de cierre
- Entregar matriz criterio→código→prueba→SHA→estado; cubrir EXP01–03 o registrar explícitamente dependencia/no ejecución. No cerrar por existir el archivo ni exigir nuevas tablas/endpoint. Solo corregir defectos demostrados dentro de este adaptador.
- Reglas comunes de ejecución
- Reutilizar modelos, repositorio operativo, tablas, ASMX/ASHX, adaptadores, coordinador y herramientas E2E existentes. Antes de programar, contrastar cada criterio con código y evidencia; modificar producto solo ante una brecha reproducida y cubrir su regresión. Leer AGENTS.md y tools/e2e/AGENT-RUNBOOK.md antes de autenticar. Pruebas reales, cambios de permisos, fallos inyectados, limpieza y carga requieren autorización específica de ambiente, cuentas y recursos; este ticket no la concede. Controles RA/DA exclusivamente SELECT, secretos efímeros, evidencia saneada. No repetir efectos inciertos ni cargas ya acreditadas sin motivo.
- Decisiones vigentes que sustituyen exigencias históricas
- No modificar AlmacenamientoDocumentoProduccionDocumental ni SII. Se conserva PDF como formato soportado por este adaptador; TIFF/Multi-TIFF deben rechazarse, no implementarse para satisfacer el prompt anterior. El aislamiento adicional del XML posterior al commit frente a otras cargas fue descartado expresamente por el usuario: queda fuera del alcance y no se afirma como garantía. No habilitar workers, migraciones, permisos, cuotas globales ni limpieza automática por inferencia. Preservar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, snapshot, inventario original y efectos confirmados. Sin rollback global.
- Fuente transversal
- https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-matriz-final-20261102.md
- https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-verificacion-final-20261104.md
