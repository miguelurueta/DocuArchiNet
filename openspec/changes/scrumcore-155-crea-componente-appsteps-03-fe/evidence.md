## Evidencia Operativa SCRUMCORE-155

### 1) Validacion automatizada ejecutada

- Comando:
  - `npx.cmd vitest --run src/app/Components/UI/AppSteps/AppSteps.test.tsx src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.test.tsx`
- Resultado:
  - `2` archivos de prueba en verde
  - `15` pruebas en verde
  - incluye caso `[SPEC:APP-APPSTEPS-03-FE]` para integracion real en `GestionRespuestaMainTabContent`

- Comando:
  - `npm.cmd run spec:validate`
- Resultado:
  - `Specs covered: 16`
  - `Specs missing: 0`
  - `Unknown tags in tests: 0`

### 2) Evidencia requerida para PR

- [ ] Captura desktop del flujo con `AppSteps` integrado (`Redaccion -> Adjuntos -> Envio`).
- [ ] Captura mobile del mismo flujo.
- [ ] (Opcional) Video corto mostrando bloqueo de `Envio` sin adjuntos y habilitacion tras adjuntar archivo.
- [ ] Referencia explicita al ticket `SCRUMCORE-155` en descripcion del PR.
- [ ] Referencia explicita a la variante integrada de `AppSteps` (`form`) en descripcion del PR.
