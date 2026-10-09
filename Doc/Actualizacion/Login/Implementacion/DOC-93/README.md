# Implementación DOC-93

DOC-93 agrega infraestructura SMTP interna y tipada para OTP. Reutiliza la configuración de `Config_Smpt_Side` cuyo `ESTADO_ENVIO` es `1`, así como `IModuleConnectionFactory` e `IDataExecutor`; no crea tablas, columnas, paquetes, endpoints ni configuración paralela.

La entrega permanece inactiva: ninguna página, ASMX o flujo de login instancia estas clases todavía. `radicador/ClassCorreo.vb`, `ClassRaEnvioCorrespondencia.vb` y los envíos legacy permanecen fuera del diff funcional.

## Recorrido implementado

`ISecondFactorEmailSender → SecondFactorSmtpEmailSender → ISecondFactorSmtpConfigurationRepository → MySqlSecondFactorSmtpConfigurationRepository → IModuleConnectionFactory/IDataExecutor → Config_Smpt_Side → ISecondFactorSmtpTransport → SecondFactorSmtpTransport → ISecondFactorSmtpClientFactory → FrameworkSmtpClientAdapter`.

## Evidencia

- Suite local DOC-91/DOC-92/DOC-93: 8 pruebas aprobadas.
- MSBuild .NET Framework 4.6.1: código 0; conserva advertencias históricas del proyecto.
- SMTP real: no ejecutado porque no existe autorización vigente específica de ambiente, cuenta y buzón para DOC-93.
- E2E WebForms: no aplica; DOC-93 no expone interfaz ni endpoint.

## Documentación técnica verificada

- [Arquitectura y diagramas](01-ARQUITECTURA-Y-DIAGRAMAS.md)
- [Casos de uso](02-CASOS-DE-USO.md)
- [Inventario técnico](03-INVENTARIO-TECNICO.md)
- [Validación y pendientes](04-VALIDACION-Y-PENDIENTES.md)
- Contrato estructural: `diagram-contract.json`
- Diagramas obligatorios: `Diagramas/*.mmd`
