# DOBLE-FACTOR-CONTRATO

- Ticket: DOC-91
- Cambio OpenSpec: doc-91-doble-factor-contrato
- Clasificacion: cross_cutting

## Evidencia requerida

El 2026-10-09, `node --test tests/login-second-factor-foundation.test.cjs` terminó con código 0 y 3 pruebas aprobadas. Cubrió OTP y rechazo, reloj, HMAC y manipulación de contexto, rotación e invalidez de llaves, invariantes, enmascarado, DTO, Session mínima, registro único en `.vbproj` y ausencia de referencias productivas. `msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal` terminó con código 0 y generó el ensamblado net461; se conservaron advertencias legacy existentes. Se usaron únicamente datos sintéticos.

La prueba documental `node --test tests/doc91-technical-documentation.test.cjs` terminó con código 0 y 4 pruebas aprobadas. El validador Roslyn terminó con código 0 y confirmó 32 declaraciones, 2 enums, 34 firmas y 4 tipos con propiedades. Ambas comprobaciones están integradas en `.github/workflows/opsxj-validation.yml`.

La revisión técnica contrastó las 13 tareas y los 16 escenarios de RQ-01 a RQ-09 contra código, pruebas y documentación. Se corrigieron durante la revisión dos brechas: faltaba demostrar `Clear()` explícito en Session y el destino enmascarado aceptaba un asterisco fuera de la parte local. La suite focal posterior terminó con 7 de 7 pruebas aprobadas y la inspección Git no encontró cambios en login, correo, Forms Authentication, SMTP, repositorios MySQL ni archivos legacy prohibidos.

## QA/E2E WebForms

DOC-91 no requiere ni autoriza E2E autenticado: no existe UI ni endpoint y la fundación está inactiva. La evidencia manual se limita a inspección del diff, rutas y límites. Los flujos E2E se incorporarán cuando un ticket posterior integre explícitamente ASMX e interfaz, con las autorizaciones del runbook vigente.
