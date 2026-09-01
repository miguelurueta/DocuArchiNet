# TRANSACCIONES-NOTAS

- Ticket: DOC-42
- Cambio OpenSpec: doc-42-transacciones-notas
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- [x] unit: `node --test tests/workflow-notes-contracts.test.cjs tests/workflow-notes-read-repository.test.cjs tests/workflow-notes-write-repository.test.cjs tools/e2e/tests/notes-workflow-policy.test.cjs tools/e2e/tests/workflow-e2e-platform-notes-write.test.cjs` — 24/24 exitosas (2026-09-01), sin conexión MySQL ni sesión autenticada.
- [x] manual_qa: no aplica; no hubo cambios de UI. `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal /nologo` aprobó con advertencias históricas de ensamblados.

## QA/E2E WebForms

El 2026-09-01 hubo autorización independiente de ambiente, cuenta y recursos descartables para `test:notes:write`. La primera corrida se detuvo antes de escribir por la CA autofirmada; una comprobación HTTPS estricta local confirmó el certificado en Windows y las corridas posteriores usaron temporalmente `--use-system-ca`, sin desactivar la validación TLS. Tras la aplicación manual y autorizada de la precondición de esquema de idempotencia, creación e idempotencia confirmaron éxito. La primera `ConsultarNota` posterior devolvió el código seguro `Unavailable`, aislando la dependencia de `SHA2()` SQL; no se actualizó ni borró la nota creada. La corrección local sustituye esa dependencia por `workflow_notas_version` InnoDB, cuya presencia InnoDB fue comprobada después por `SELECT`.

La siguiente corrida de escritura completó creación, reintento, consulta, actualización, conflicto y eliminación. El único rechazo fue una expectativa E2E contradictoria: la huella de `ANOTACION_TAREA` quedó igual antes y después, que es el resultado correcto de crear y borrar la nota; por ese orden de aserciones no se alcanzó a evaluar la comparación final de auditoría. Se corrigió el arnés para exigir estado sin cambio y auditoría con cambio, y una prueba local evita que se aparte del registro declarativo. No se declara aprobada la E2E completa ni se ejecutó concurrencia: se requiere una tarea descartable nueva y autorización independiente para validar auditoría y conservar evidencia saneada.

La corrida posterior de escritura autorizada pasó 1/1 en 24,5 s con una tarea descartable nueva. Creación, idempotencia, consulta, actualización, conflicto y eliminación fueron exitosos; el control final confirmó gate apagado, usuarios y grupos vacíos y WebForms legacy sin cambios. La concurrencia sigue pendiente y requiere autorización y recurso descartable independientes.

La carrera autorizada posterior produjo `0/2` actualizaciones efectivas y evidencia saneada, por lo que no cumple el criterio de una actualización exitosa y un conflicto. El diagnóstico pendiente es verificar que la nota semilla tenga su fila en `workflow_notas_version`; el diseño no realiza backfill sintético de notas históricas.

La carrera repetida con una nota semilla nueva produjo `1/2` actualizaciones efectivas y un conflicto de versión, con evidencia saneada. Los controles de cierre confirmaron gate apagado, usuarios y grupos vacíos y WebForms legacy sin cambios.
