# DOC-93 — Resumen técnico

- Ticket: DOC-93
- Cambio OpenSpec: doc-93-segundo-factor-smtp
- Clasificacion: cross_cutting

## Objetivo

Se implementó infraestructura SMTP exclusiva para OTP, reutilizando la fila de `Config_Smpt_Side` con `ESTADO_ENVIO = 1`, la conexión de Radicación resuelta fuera de Infrastructure y los contratos 2FA existentes.

## Alcance y compatibilidad

El alcance incluye modelos/puertos tipados, repositorio de solo lectura, transporte síncrono, fachada `ISecondFactorEmailSender`, pruebas sin red y documentación. No incluye conexión al login, Session, UI, ASMX, challenges, cambio de esquema o envío SMTP real.

`radicador/ClassCorreo.vb`, `ClassRaEnvioCorrespondencia.vb`, recuperación de contraseña y demás recorridos legacy conservan diff funcional cero. La implementación permanece sin composición desde login. Rollback: retirar fuentes DOC-93 y sus entradas del `.vbproj`.

La documentación verificada, casos, inventario, diagramas y contrato Roslyn están en `Doc/Actualizacion/Login/Implementacion/DOC-93/`.
