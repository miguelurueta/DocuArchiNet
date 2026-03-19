# Manual de integración JIRA + GitHub + Codex

Este documento define el flujo operativo recomendado para `DocuArchiCore.react` con trazabilidad completa entre especificaciones OpenSpec, tickets JIRA, implementación asistida por Codex y validaciones CI/CD.

## Objetivo

Asegurar que cada cambio funcional o correctivo cumpla una cadena verificable:

`JIRA ↔ SPEC ID ↔ OpenSpec YAML ↔ Tests (Vitest) ↔ GitHub PR ↔ CI/CD`.

---

## 1) Creación en JIRA (origen de la especificación)

Toda funcionalidad/corrección inicia en JIRA.

### Reglas de creación de ticket

- Crear ticket por cada feature/fix.
- Incluir el SPEC ID en el título:
  - Ejemplo: `[SPEC:AUTH-001] Login SUCCESS`.
- Incluir escenario en descripción con formato Given/When/Then:
  - **Given:** contexto inicial y precondiciones.
  - **When:** acción/evento del usuario o sistema.
  - **Then:** resultado esperado verificable.
- Asignar responsable:
  - desarrollador humano, o
  - asistente Codex (con supervisión del equipo).

### Recomendación de plantilla de ticket

```md
Título: [SPEC:AUTH-001] Login SUCCESS

Descripción:
Given usuario con credenciales válidas
When envía formulario de login
Then se persiste token y navega a /dashboard sin recarga

OpenSpec:
- openspec/auth.behavior.yaml
- openspec/auth.contract.yaml

Criterios de aceptación:
- Test(s) con tag [SPEC:AUTH-001]
- npm run spec:validate en verde
- PR enlazado al ticket
```

---

## 2) Generación de código con Codex

Codex debe recibir explícitamente el contexto mínimo:

- ticket JIRA,
- SPEC ID,
- alcance de módulo (`src/modules/<modulo>/`),
- expectativas de pruebas.

### Flujo de implementación esperado

1. Codex implementa código en `src/modules/<modulo>/`.
2. Codex crea/ajusta pruebas en `src/modules/<modulo>/**/*.test.ts(x)`.
3. Cada prueba que cubra el escenario incluye el tag:
   - `[SPEC:<id>]` en el nombre del test.
4. Codex trabaja sobre rama vinculada al ticket:
   - `feature/AUTH-001-login-success`.

### Estándares técnicos obligatorios

- TypeScript estricto.
- Arquitectura SPA (React Router, navegación sin recarga, manejo consistente de sesión/errores).
- ESM (`import/export`) únicamente.
- Vitest + Testing Library (no Jest).
- Sin CommonJS.

---

## 3) Sincronización con GitHub

El repositorio debe reflejar trazabilidad explícita en commits y PRs.

### Convenciones de commit

Cada commit relacionado al escenario debe incluir SPEC ID:

- `feat: [SPEC:AUTH-001] implementación login success`
- `test: [SPEC:AUTH-001] cobertura de navegación dashboard`

### Convenciones de Pull Request

El PR debe incluir:

- referencia al ticket JIRA,
- referencia al SPEC ID,
- resumen de archivos OpenSpec impactados,
- evidencia de tests + `spec:validate`.

Ejemplo de bloque en PR:

```md
JIRA: JIRA-123
SPEC: AUTH-001
OpenSpec: openspec/auth.behavior.yaml
Validación: npm run test -- --run && npm run spec:validate
```

Con esto, la trazabilidad JIRA ↔ GitHub ↔ Codex es auditable en historial de rama, commits y pipeline.

### Atajo operativo Jira -> OpenSpec

Para crear proposal inicial desde Jira en un solo paso:

```bash
npm run opsxj:new -- <ISSUE-KEY>
```

Ejemplo:

```bash
npm run opsxj:new -- SCRUM-8
```

El comando:
- consulta resumen y descripción en Jira,
- genera `proposal.md`,
- crea carpeta en `openspec/changes/` usando base del resumen del ticket (kebab-case),
- muestra confirmación uniforme de resultado o error.

---

## 4) Validación automática (OpenSpec + CI/CD)

### Punto único de control

`npm run spec:validate` compara:

- IDs declarados en `openspec/*.yaml`, versus
- IDs usados en tests etiquetados `[SPEC:<id>]`.

### Reglas de fallo

La validación debe devolver `exit code 1` si ocurre alguno:

- existe SPEC ID en YAML sin test asociado;
- existe test con SPEC ID inexistente en YAML.

### Integración en GitHub Actions

El workflow ejecuta:

1. instalación de dependencias,
2. tests (`vitest --run`),
3. validación OpenSpec (`npm run spec:validate`).

Si algún paso falla, el merge queda bloqueado.

### Reporte legible

El validador publica en consola:

- total de specs,
- tags detectados,
- cobertura,
- faltantes,
- IDs desconocidos,
- detalle por archivo fuente y por archivo de test.

Esto habilita observabilidad rápida en CI/CD para QA y arquitectura.

---

## 5) Cierre en JIRA

### Regla de transición de estados

- **Ready for QA** cuando:
  - tests en verde,
  - `npm run spec:validate` confirma cobertura.
- **Done** cuando:
  - PR mergeado a `main`,
  - CI/CD final en verde.

### Resultado de gobierno técnico

Se garantiza trazabilidad completa:

`JIRA ↔ YAML ↔ tests ↔ GitHub ↔ Codex ↔ CI/CD`.

---

## Checklist operativo (rápido)

- [ ] Ticket JIRA creado con título `[SPEC:<id>]`.
- [ ] Escenario Given/When/Then documentado.
- [ ] OpenSpec actualizado en `openspec/*.yaml`.
- [ ] Rama `feature/<SPEC>-<slug>` creada.
- [ ] Código + tests implementados en TypeScript estricto (ESM).
- [ ] Tests con tags `[SPEC:<id>]`.
- [ ] `npm run test -- --run` en verde.
- [ ] `npm run spec:validate` en verde.
- [ ] PR con enlace JIRA + SPEC ID + evidencia CI.
- [ ] Ticket movido a Ready for QA / Done según política.
