## 1. Alineacion de contrato y modelo de claims

- [x] 1.1 Inventariar el contrato actual de autenticacion (login/OTP/rehidratacion) y documentar campos legacy vs campos nuevos de claims JWT
- [x] 1.2 Definir tipos TypeScript del modelo canonico de claims y sus utilidades de normalizacion en la capa Auth
- [x] 1.3 Establecer una matriz de mapeo claim->permiso efectivo para compatibilidad con `claimKey`, `requiredAny` y `requiredAll`

## 2. Implementacion del adaptador de claims

- [x] 2.1 Implementar normalizador central que priorice claims del contrato nuevo y use `usuario.permisos` como fallback legacy
- [x] 2.2 Integrar el adaptador en persistencia/rehidratacion de sesion para que `obtenerClaims` y refresco de contexto usen una sola fuente canonica
- [x] 2.3 Mantener estable la API publica de autorizacion (`useAuth`, `RutaProtegida`, guards) sin acoplarla al formato raw del token

## 3. Validacion compatible del contrato Auth

- [x] 3.1 Ajustar `validarRespuestaAutenticacion` para aceptar contrato nuevo y legacy, fallando solo ante ausencia de datos criticos
- [x] 3.2 Homologar hooks de entrada de sesion (login/OTP) para consumir el modelo compatible sin duplicar logica de mapeo
- [x] 3.3 Incluir manejo de errores de contrato con mensajes accionables para diagnostico rapido

## 4. Pruebas de autorizacion y compatibilidad

- [x] 4.1 Agregar pruebas unitarias del normalizador de claims para escenario nuevo, escenario legacy y escenarios mixtos
- [x] 4.2 Actualizar pruebas de login/OTP para validar aceptacion de contrato nuevo y fallback legacy
- [x] 4.3 Ajustar pruebas de autorizacion (`useAuth`, rutas protegidas y guards de acciones) para verificar evaluacion sobre claims normalizados
- [x] 4.4 Ejecutar suite focalizada y registrar evidencia de resultados en este archivo

## 5. Cierre tecnico y retiro planificado de legacy

- [x] 5.1 Documentar riesgos residuales y criterios de salida para desactivar fallback legacy
- [x] 5.2 Confirmar que no hay regresion funcional en acceso a rutas y acciones protegidas
- [ ] 5.3 Preparar cambio para `opsxj:archive` con trazabilidad entre spec, implementacion y evidencia de pruebas

## Evidencia

- `npm.cmd test -- src/modules/login/hooks/useLogin.spec.test.ts src/modules/login/models/validarRespuestaAutenticacion.test.ts src/app/auth/Infraestructura/authClaimsAdapter.test.ts src/app/auth/ProteccionRuta/Autorizado.spec.test.tsx`
- Resultado: `4` archivos de prueba en verde, `12` pruebas exitosas, `0` fallas.
