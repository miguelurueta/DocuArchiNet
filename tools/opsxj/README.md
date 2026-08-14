# opsxj legacy tooling

Tooling interno para coordinar Jira, GitHub y OpenSpec desde el repo legacy.

## Uso

Desde `GestionDocumental-Docuarchi.net`:

```powershell
npm.cmd --prefix tools/opsxj run opsxj:doctor
npm.cmd --prefix tools/opsxj run jira:test
npm.cmd --prefix tools/opsxj run opsxj:status -- add-legacy-dev-script
npm.cmd --prefix tools/opsxj run opsxj:new -- SCRUM-123 --impact webforms_ui
npm.cmd --prefix tools/opsxj run opsxj:orchestrate:new -- SCRUM-123 --impact webforms_ui --profile enterprise-legacy-modernization --tech-profile legacy-webforms-vb
npm.cmd --prefix tools/opsxj run opsxj:refine -- SCRUM-123 --sync
npm.cmd --prefix tools/opsxj run opsxj:orchestrate:refine -- SCRUM-123 --sync
npm.cmd --prefix tools/opsxj run opsxj:validation:evidence -- SCRUM-123 --type manual_qa --reference "QA local: pasos y resultado"
npm.cmd --prefix tools/opsxj run opsxj:validate -- SCRUM-123
```

El tooling carga `.env.jira` desde la raiz del repo legacy. Use
`.env.jira.example` como referencia.

## Gobierno local legacy

`opsxj:new` acepta `--impact` con: `docs_only`, `frontend_legacy`,
`webforms_ui`, `backend_vb`, `handler_integration`, `database` o
`cross_cutting` (predeterminado). Genera el manifiesto OpenSpec y la
documentación técnica en `Doc/Tecnica/Opsxj/<change-name>/`.

`opsxj:orchestrate:new` es un alias de `opsxj:new`: ambos aceptan el mismo
`--impact`, el perfil arquitectónico opcional `--profile
enterprise-legacy-modernization` y `--tech-profile`. Use el perfil de
arquitectura solo cuando se modernice una capacidad legacy de forma gradual.
El perfil tecnológico es independiente y evita que la revisión aplique reglas
de un framework ajeno: `legacy-webforms-vb`, `tooling-node`,
`frontend-react-ts` o `generic`.

## Compuerta de refinement

Todo cambio nuevo creado con `opsxj:new` incluye
`openspec/changes/<change-name>/refinement.md` y un manifiesto de gobierno
v3. El artefacto empieza en estado `draft`: no se considera aprobado por
haber sido generado.

Antes de iniciar o cerrar tareas, complete decisiones `D-XX`, requisitos
`RQ-XX`, evidencia de codigo y compatibilidad; cambie el marcador a
`state=approved` y ejecute:

```powershell
npm.cmd --prefix tools/opsxj run opsxj:refine -- SCRUM-123 --sync
```

`--sync` agrega o actualiza únicamente encabezados de trazabilidad en
`design.md`, `spec.md` y `tasks.md`; nunca reescribe sus decisiones ni marca
tareas como terminadas. La validacion bloquea si una decision no aparece en
los tres artefactos, si una tarea no declara `Origen: D-XX, RQ-XX`, si quedan
marcadores pendientes o si se inyectan reglas de frontend en un perfil que no
sea `frontend-react-ts`.

`opsxj:orchestrate:refine` es un alias equivalente para conservar la
nomenclatura del flujo orquestado.

Los cambios existentes con manifiesto v2 no se alteran. Para migrar uno de
forma explícita y dejarlo en borrador controlado, use
`opsxj:refine -- <ISSUE-KEY> --bootstrap`; el resultado bloqueará el cierre
hasta que se complete el refinamiento real.

`opsxj:validation:evidence` registra evidencia local por ticket y SHA. Use
`opsxj:validate` antes de `opsxj:archive`; los cambios nuevos se bloquean si
falta refinement aprobado y trazable, tareas, revisión, documentos o evidencia
exigida. Los cambios OpenSpec históricos sin manifiesto conservan compatibilidad
y no reciben requisitos retroactivos.

## Checklist persistente de ejecución

Cada ejecución relevante conserva una bitácora local en
`.opsxj/runs/<ISSUE-KEY>.json`. El archivo es versionado, append-only e
ignorado por Git: es una ayuda operativa local, no un artefacto que deba entrar
en el PR ni una fuente de verdad remota.

Cada evento contiene únicamente `stage`, `status`, `sha`, `recordedAtUtc` y,
si aplica, `actor`, `source`, `reference` o `detail`. Las etapas permitidas
son `new`, `refine`, `review`, `validate`, `archive` y `close`; los resultados
son `pass` o `fail`. No incluya tokens, contraseñas, cabeceras de autorización
ni contenido de `.env` en referencias o detalles: el servicio los rechaza.

`review` y `validate` son sensibles al SHA. Una revisión aprobada para un SHA
anterior aparece como `STALE` y no habilita el archivo. Para compatibilidad,
`OPSXJ_OPENSPEC_REVIEW_CONFIRMED` y `OPSXJ_OPENSPEC_REVIEWED_BY` siguen siendo
válidos en `opsxj:validate`; esa ejecución persiste primero la revisión y luego
la validación. `opsxj:status` no escribe el archivo: muestra la variable como
observación temporal y consulta Git, OpenSpec, GitHub y Jira en vivo.

Use `opsxj:status <ISSUE-KEY> --json` para consumir el checklist ordenado
`new`, `refine`, `review`, `validate`, `archive`, `pull_request` y `close`.
La respuesta mantiene `checks`, `status` y `nextAction` existentes y agrega
`checklist`, con estado, fecha, SHA, referencia y siguiente acción por etapa.

Si el archivo local se elimina, se corrompe o pertenece a un ticket histórico,
el estado continúa siendo consultable y muestra `UNAVAILABLE` solo donde no hay
evidencia inferible. No copie un registro entre tickets ni lo edite durante una
ejecución. Para recuperarlo, corrija o retire manualmente el archivo inválido y
ejecute de nuevo la etapa correspondiente; las operaciones mutantes fallan
antes de anunciar éxito si no pueden persistir su evento.

`opsxj:technical-review` es el nombre neutral de la revisión técnica. El
comando `opsxj:prompt-review` permanece como alias compatible.
Puede recibir `--tech-profile <perfil>`; sin esa opción detecta señales
inequívocas del prompt y usa `generic` cuando no hay suficiente contexto.

La validación local no modifica Jira ni GitHub. `opsxj:archive` y
`opsxj:close` son las únicas operaciones que pueden ejecutar acciones remotas,
de manera explícita. Configure `GIT_AUTO_PUSH=false` salvo que el equipo haya
aprobado el envío automático de la rama inicial.

## Alcance

Este directorio es tooling de desarrollo. No es JavaScript publico de la
aplicacion WebForms y no debe referenciarse desde paginas `.aspx`.
