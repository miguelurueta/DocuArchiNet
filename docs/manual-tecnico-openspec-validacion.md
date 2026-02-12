# Manual técnico: OpenSpec + Validación Automática

Este manual define el funcionamiento y la evolución del sistema de validación de especificaciones en `DocuArchiCore.react`, manteniendo la arquitectura SPA en React + TypeScript estricto + ESM.

## 1) Funcionamiento del sistema de validación de especificaciones

### 1.1. Fuente de verdad funcional y contractual: `/openspec/*.yaml`

En este repositorio, los módulos de dominio documentan dos capas de especificación dentro de `openspec/`:

- `*.behavior.yaml`: escenarios de comportamiento (Given/When/Then) con IDs trazables.
- `*.contract.yaml`: contratos de request/response, campos requeridos y reglas funcionales/técnicas.

Ejemplo real:

- `openspec/auth.behavior.yaml` declara escenarios como `AUTH-001`, `AUTH-002`, `AUTH-003`.
- `openspec/auth.contract.yaml` describe el contrato `AuthResponse` y reglas de validación de token/expiración.

### 1.2. Motor de validación (`tools/spec-validator`)

El validador se ejecuta con:

```bash
npm run spec:validate
```

y sigue este flujo:

1. **Carga de configuración** (`tools/spec-validator/config.ts`): define archivos OpenSpec, globs de tests y regex de tags `[SPEC:<ID>]`.
2. **Parseo OpenSpec** (`tools/spec-validator/parseOpenSpec.ts`):
   - lee YAML;
   - extrae `scenarios[].id` válidos (`[A-Z]+-\d+`);
   - construye catálogo de IDs por archivo.
3. **Escaneo de tests** (`tools/spec-validator/extractTestCases.ts`):
   - recorre tests `*.test.ts(x)` bajo `src`;
   - filtra por `testGlobs` configurados;
   - extrae tags con regex `[SPEC:<ID>]`.
4. **Comparación final y reporte** (`tools/spec-validator/validateSpec.ts`):
   - genera métricas (`total specs`, `covered`, `missing`, `unknown tags`);
   - imprime el detalle por fuente OpenSpec y por archivo de test;
   - retorna `exitCode = 1` si hay desalineación.

### 1.3. Reglas de negocio de cobertura

El proceso de validación aplica estas reglas obligatorias:

1. **Cada SPEC ID en OpenSpec debe tener al menos un test taggeado**.
2. **Ningún test puede referenciar un SPEC ID inexistente en OpenSpec**.
3. **Si existe cualquier desalineación, el pipeline falla**.

Estas reglas convierten OpenSpec en contrato ejecutable dentro del ciclo de calidad.

---

## 2) Guía para agregar nuevas especificaciones

### 2.1. Crear archivos de especificación del módulo

Para un nuevo módulo `<modulo>`, crear:

- `openspec/<modulo>.contract.yaml`
- `openspec/<modulo>.behavior.yaml`

### 2.2. Convención de IDs

Definir escenarios con convención:

- `[MODULO-XXX]` (ej.: `AUTH-001`, `REC-004`, `RST-003`).
- `MODULO` en mayúsculas y número incremental.

### 2.3. Etiquetado de pruebas

En cada test de Vitest/Testing Library, incluir el ID en el nombre:

```ts
it("[SPEC:AUTH-001] Login SUCCESS emite token y navega al dashboard", () => {
  // ...
});
```

Sin este tag, el caso no cuenta para cobertura de especificación.

### 2.4. Registrar módulo en `tools/spec-validator/config.ts`

Mantener la configuración por módulo usando una estructura como `SPEC_MODULES` para centralizar:

- nombre de módulo;
- archivos OpenSpec (`behavior` + `contract`);
- rutas/globs de tests del módulo.

Implementación recomendada (TypeScript estricto + ESM):

```ts
export const SPEC_MODULES = [
  {
    module: "auth",
    openSpecFiles: ["openspec/auth.behavior.yaml", "openspec/auth.contract.yaml"],
    testGlobs: ["src/modules/login/**/*.test.ts", "src/modules/login/**/*.test.tsx"],
  },
  // ...
] as const;
```

Luego derivar `openSpecFiles` y `testGlobs` agregados desde `SPEC_MODULES`, evitando duplicación y errores de mantenimiento.

---

## 3) Desarrollo de nuevo código (React + TypeScript + SPA)

### 3.1. Estructura de módulo

Implementar el nuevo módulo en:

- `src/modules/<modulo>/components`
- `src/modules/<modulo>/hooks`
- `src/modules/<modulo>/services`
- `src/modules/<modulo>/pages`
- `src/modules/<modulo>/**/*.test.ts(x)`

### 3.2. Estándar técnico obligatorio

- **TypeScript estricto** y tipado explícito en DTOs, responses y estado.
- **ESM únicamente** (`import/export`), sin CommonJS.
- **Vitest + Testing Library**, sin Jest.
- Mantener navegación SPA con React Router v6+ (compatible con setup actual de rutas y `useRoutes`).

### 3.3. Patrón de pruebas recomendado

1. **Unitarias**: funciones puras, validadores, mappers.
2. **Integración**: componentes con hooks y servicios mockeados.
3. **Comportamiento (spec coverage)**: tests con `[SPEC:<ID>]` alineados a OpenSpec.

Prácticas obligatorias:

- usar `MemoryRouter` para simular navegación sin recarga;
- usar mocks con `vi.mock`, `vi.spyOn`, `vi.fn`;
- usar fake timers (`vi.useFakeTimers`) en flujos de expiración/reintento para evitar flakiness;
- evitar dependencias de red reales o estado global no controlado.

### 3.4. Consistencia de arquitectura transversal

Conservar patrones existentes:

- **Router SPA** centralizado en `src/app/routes`;
- **monitoreo de sesión** mediante `TokenWatcher` para rutas restringidas;
- **manejo de errores uniforme** con `ErrorBoundaryWithNotifier` + normalización de errores.

---

## 4) Centralización del control de especificaciones

### 4.1. Repositorio único de contratos/escenarios

Toda definición funcional/contractual debe vivir en `/openspec`. No distribuir reglas de negocio de especificación en múltiples carpetas desconectadas.

### 4.2. Punto único de verificación

`npm run spec:validate` debe ser el **gate oficial** para validación de trazabilidad OpenSpec ↔ Tests.

### 4.3. Integración CI/CD

La validación debe ejecutarse en GitHub Actions para cada `push`/`pull_request` junto con la suite de tests, generando reporte legible en logs:

1. instalación de dependencias;
2. ejecución de tests (`vitest --run`);
3. ejecución de validador (`spec:validate`);
4. fallo automático ante desalineación.

---

## 5) Integración con JIRA

### 5.1. Trazabilidad SPEC ↔ JIRA

Mapear cada SPEC ID a un ticket JIRA.

Ejemplo:

- `AUTH-001` → `JIRA-123`
- `AUTH-002` → `JIRA-124`

### 5.2. Convenciones operativas

- incluir el SPEC ID en la descripción del ticket;
- incluir el SPEC ID en commits y PRs relacionados;
- usar formato consistente en mensajes (`feat(auth): login success [SPEC:AUTH-001]`).

### 5.3. Workflow sugerido en JIRA

Configurar estados para reflejar evidencia automática:

- `In Progress` → desarrollo;
- `In Review` → PR abierto;
- `Ready for QA` **solo si** CI reporta `spec:validate` en verde y tests passing;
- `Done` → validación funcional completada.

### 5.4. Opcional: campos personalizados

Agregar `custom field` o etiqueta `SPEC-ID` para enlazar historias y escenarios OpenSpec, facilitando auditoría de cobertura por release.

---

## Checklist de adopción rápida

- [ ] Definir `behavior` + `contract` del módulo en `openspec/`.
- [ ] Crear tests con tags `[SPEC:<ID>]`.
- [ ] Registrar el módulo en configuración central del validador (`SPEC_MODULES`).
- [ ] Ejecutar `npm run test -- --run`.
- [ ] Ejecutar `npm run spec:validate`.
- [ ] Verificar pipeline CI/CD en verde.
- [ ] Vincular SPEC IDs con tickets JIRA.

Con esto, el repositorio mantiene trazabilidad extremo a extremo entre especificación, implementación, pruebas y gestión operativa.
