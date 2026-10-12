# DOBLE-FACTOR-DESPLIEGUE

- Ticket: DOC-98
- Cambio OpenSpec: doc-98-doble-factor-despliegue
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

- `OdbcSecondFactorLoginModuleRepository.Resolve(companyName, moduleName)` es la unica fuente de la decision transversal; lee la fila activa de `gestor_modulos`.
- `SecondFactorPreAuthenticationService.Execute(...)` conserva el camino legacy cuando la configuracion esta apagada y abre el challenge cuando esta activa y valida.
- `SecondFactorPrincipalRepositoryResolver` selecciona el repositorio de principal por `TIPO_MODULO`; esos repositorios resuelven identidad/correo y no activan usuarios.
- `MySqlSecondFactorChallengeRepository` usa el contrato fisico existente y estados derivados de `Consumed`, `Attempts` y `ExpiresAtUtc`.
- `MySqlSecondFactorSmtpConfigurationRepository` exige exactamente una configuracion existente con `ESTADO_ENVIO=1`.
- Identidad ambigua, correo invalido o configuracion invalida fallan cerrado. No se agrega bypass.
- Formato HMAC: `v1:keyId:mac`; rotacion install-first/remove-last y coexistencia minima de diez minutos mas margen aprobado.
