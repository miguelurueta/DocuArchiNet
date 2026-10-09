<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## 1. Contratos y modelos

- [x] 1.1 [M] Crear modelos e invariantes de identidad, configuración, challenge, protección, entrega, finalización y contexto. Área/archivos: `Modelo/Login/SegundoFactor/SegundoFactorModels.vb`. Origen: D-02, RQ-02. Verificación: casos válidos, normalización y rechazo de valores fuera del contrato.
- [x] 1.2 [M] Definir puertos de reloj, OTP, HMAC, llaves, repositorios, destinatario, correo, finalización y contexto sin infraestructura. Área/archivos: `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb`. Origen: D-01, RQ-01. Verificación: análisis confirma ausencia de `System.Web`, SMTP, MySQL y conexiones.
- [x] 1.3 [S] Crear DTO serializables con superficie pública sanitizada. Área/archivos: `DTOs/Login/SegundoFactor/SegundoFactorDtos.vb`. Origen: D-07, RQ-07. Verificación: reflexión confirma campos permitidos y ausencia de secretos e IDs internos.

## 2. Seguridad criptográfica

- [x] 2.1 [M] Implementar reloj UTC y OTP de seis dígitos con RNG y rechazo. Área/archivos: `Infrastructure/Login/SegundoFactor/Security/SystemSecondFactorClock.vb`, `CryptographicSecondFactorOtpGenerator.vb`. Origen: D-03, RQ-03. Verificación: fuente determinista cubre aceptación, rechazo, límites, formato y expiración.
- [x] 2.2 [L] Implementar HMAC-SHA256 v1 ligado a propósito, challenge, identidad, sesión y OTP con comparación XOR. Área/archivos: `Infrastructure/Login/SegundoFactor/Security/HmacSecondFactorCodeProtector.vb`. Origen: D-04, RQ-04. Verificación: round-trip y alteración de cada componente, formato, versión, Base64, MAC y longitud.
- [x] 2.3 [M] Implementar configuración inyectable y resolución de llave activa/anterior con validación Base64 y 32 bytes. Área/archivos: `Infrastructure/Login/SegundoFactor/Security/AppSettingsSecondFactorKeyProvider.vb`. Origen: D-05, RQ-05. Verificación: activa, rotación, ausente, desconocida, Base64 inválido y longitud insuficiente con datos sintéticos.

## 3. Frontera WebForms

- [x] 3.1 [M] Implementar guardar, leer, expirar y limpiar contexto mínimo en `HttpSessionStateBase`. Área/archivos: `webservice/Login/SegundoFactor/SessionPendingSecondFactorContextStore.vb`. Origen: D-06, RQ-06. Verificación: Session simulada cubre round-trip, clear, expiración y ausencia de campos prohibidos.

## 4. Proyecto y pruebas

- [x] 4.1 [S] Registrar cada fuente VB nuevo con `Compile` sin crear aplicación paralela. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-01, RQ-01. Verificación: cada ruta aparece una vez y MSBuild la compila.
- [x] 4.2 [L] Crear suite focal con datos y llaves sintéticos para RQ-02 a RQ-07 y límites estructurales. Área/archivos: `tests/login-second-factor-foundation.test.cjs` y arnés auxiliar si se requiere. Origen: D-08, RQ-08. Verificación: comando focal termina en cero y casos son trazables por nombre.
- [x] 4.3 [M] Ejecutar MSBuild Debug y revisar el diff de archivos legacy prohibidos, sin E2E ni datos reales. Área/archivos: proyecto y diff Git. Origen: D-08, RQ-08. Verificación: código cero o evidencia del bloqueo real sin afirmar éxito.

## 5. Documentación y cierre

- [x] 5.1 [S] Documentar contratos, límites, comandos y resultados sanitizados. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-91/`, `Doc/Tecnica/Opsxj/doc-91-doble-factor-contrato/`. Origen: D-08, RQ-08. Verificación: identifica ticket, alcance inactivo, evidencia real y riesgos sin secretos ni listas abiertas.
- [x] 5.2 [S] Validar trazabilidad OpenSpec y reversa limitada a archivos nuevos. Área/archivos: `openspec/changes/doc-91-doble-factor-contrato/`. Origen: D-01, RQ-01. Verificación: refine sync y OpenSpec strict terminan en cero.
- [x] 5.3 [M] Documentar arquitectura, casos de uso e inventario y validar diagramas/referencias con Mermaid y Roslyn en CI. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-91/`, `tests/doc91-technical-documentation.test.cjs`, `tools/validation/Doc72SourceValidator/`, `.github/workflows/opsxj-validation.yml`. Origen: D-09, RQ-09. Verificación: prueba Mermaid y validador Roslyn terminan en cero e inventarían ausencias y límites.
