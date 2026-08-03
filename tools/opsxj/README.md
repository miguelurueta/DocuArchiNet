# opsxj legacy tooling

Tooling interno para coordinar Jira, GitHub y OpenSpec desde el repo legacy.

## Uso

Desde `GestionDocumental-Docuarchi.net`:

```powershell
npm.cmd --prefix tools/opsxj run opsxj:doctor
npm.cmd --prefix tools/opsxj run jira:test
npm.cmd --prefix tools/opsxj run opsxj:status -- add-legacy-dev-script
npm.cmd --prefix tools/opsxj run opsxj:new -- SCRUM-123
```

El tooling carga `.env.jira` desde la raiz del repo legacy. Use
`.env.jira.example` como referencia.

## Alcance

Este directorio es tooling de desarrollo. No es JavaScript publico de la
aplicacion WebForms y no debe referenciarse desde paginas `.aspx`.
