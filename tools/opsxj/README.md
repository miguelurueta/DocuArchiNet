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

`opsxj:validation:evidence` registra evidencia local por ticket y SHA. Use
`opsxj:validate` antes de `opsxj:archive`; los cambios nuevos se bloquean si
faltan tareas, revisión, documentos o evidencia exigida. Los cambios OpenSpec
históricos sin manifiesto conservan compatibilidad y no reciben requisitos
retroactivos.

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
