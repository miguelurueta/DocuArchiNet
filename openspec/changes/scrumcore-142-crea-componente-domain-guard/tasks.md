## 1. Definición de contrato y ubicación

- [ ] 1.1 Confirmar convención de paths (`src/app/Components/...`) y naming final (`DomainGuard` vs `AppDomainGuard`).
- [ ] 1.2 Definir contrato TypeScript (props + tipos auxiliares) sin `any`.
- [ ] 1.3 Definir API del hook helper `useDomainGuard` (opcional) y el enum/union de `reason`.

## 2. Implementación del componente reusable

- [ ] 2.1 Implementar `DomainGuard` que NO monte children cuando `isBlocked=true`.
- [ ] 2.2 Permitir `fallback` como `ReactNode` (full replacement).
- [ ] 2.3 Garantizar que en modo “enabled” (`isBlocked=false`) no se afecte el render de children.

## 3. Implementación del hook helper (si aplica)

- [ ] 3.1 Implementar `useDomainGuard({ isEmpty, error, condition }) -> { isBlocked, reason }`.
- [ ] 3.2 Definir `reason` con union string (no enum si no aporta valor) y documentar semántica.

## 4. Pruebas

- [ ] 4.1 Unit: renderiza children cuando `isBlocked=false`.
- [ ] 4.2 Unit: renderiza fallback cuando `isBlocked=true`.
- [ ] 4.3 Unit: children no se montan cuando está bloqueado (validar con un child que tenga `useEffect`/spy).
- [ ] 4.4 Integración UI: ejemplo de pantalla piloto mínima (fixture) usando fallback con CTA.

## 5. Documentación técnica

- [ ] 5.1 Crear `docs/Architecture/DomainGuard/SCRUMCORE-142-Arquitectura.md` (diagrama de estados/secuencia/clases).
- [ ] 5.2 Crear `docs/Architecture/DomainGuard/SCRUMCORE-142-Implementacion-Detallada.md`.
- [ ] 5.3 Crear `docs/Architecture/DomainGuard/SCRUMCORE-142-Pruebas.md`.
- [ ] 5.4 Crear `docs/Architecture/DomainGuard/SCRUMCORE-142-Integracion-BackEnd.md` (si aplica; probablemente “no aplica”).

## 6. Calidad (no regresión)

- [ ] 6.1 Ejecutar `npm test` focalizado en tests del guard.
- [ ] 6.2 Ejecutar `npm run build` o `tsc` (según convenciones del repo) para asegurar tipado estricto.
- [ ] 6.3 Confirmar cero warnings/errores nuevos de lint (si aplica).

