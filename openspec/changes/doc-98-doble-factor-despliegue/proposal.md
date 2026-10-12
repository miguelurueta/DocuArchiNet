## Why

El Login 2FA ya está implementado y verificado, pero su operación depende de conocimiento disperso entre DOC-91..97. Se requiere una entrega reproducible que permita instalar la infraestructura con 2FA apagado, habilitar un piloto acotado, observarlo y revertirlo sin exponer secretos ni afectar otros módulos.

## What Changes

- Consolidar un runbook parametrizado con responsables, orden de despliegue, puertas go/no-go y evidencia saneada.
- Incorporar prechecks de solo lectura para esquema existente, índices, SMTP activo, configuración central, llaves externas y timeout de Session.
- Definir scripts operativos guardados para activar o desactivar exclusivamente una fila piloto de `gestor_modulos`, sin tocar esquemas homónimos.
- Completar, después del piloto aprobado, la activación controlada de todas las filas centrales de los cuatro módulos para que 2FA aplique transversalmente a cualquier usuario que ingrese por el módulo.
- Mantener la activación exclusivamente en la configuración central del módulo: no crear banderas, listas, excepciones ni dependencias de activación por usuario. La resolución de identidad y correo se evalúa en tiempo de autenticación y falla cerrada cuando los datos del principal son inválidos.
- Definir observabilidad compatible con las señales físicas de challenges y los diagnósticos sanitizados existentes.
- Documentar rotación de llaves, limpieza manual autorizada de terminales mayores de 30 días y rollback por capas.
- Añadir pruebas estáticas y reproducibles; el piloto y la activación global real requieren autorización vigente y evidencia por oleada.
- No crear DDL: DOC-92 confirmó que la tabla, columnas e índices necesarios ya existen.

## Capabilities

### New Capabilities
- `doble-factor-despliegue`: preparación operativa, activación gradual, observación, rotación, limpieza y rollback compatible del Login 2FA.

### Modified Capabilities

Ninguna. Los contratos funcionales entregados por DOC-91..97 permanecen sin cambios.

## Impact

- Documentación y scripts: `Doc/Actualizacion/Login/Implementacion/DOC-98/` y `Sql/`.
- Validación: pruebas CJS locales y, solo si falta una puerta demostrada, `.github/workflows/opsxj-validation.yml`.
- Configuración existente: `AppSettings.local.config` no versionado, Session de 10 minutos, catálogo central `gestor_modulos` y repositorios de identidad de los cuatro módulos.
- Datos operativos: la implementación culmina con las filas objetivo en `RequiereSegundoFactor=1`, `EMAIL=1` y expiración válida, únicamente después de todas las puertas y autorizaciones.
- Sin cambios en modelos, DTOs, servicios, repositorios, ASMX, Login, recuperación, SMTP legacy ni esquema.
